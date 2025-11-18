using BookNow.Application.DTOs.EventDTOs;
using BookNow.Application.DTOs.PaymentDTOs;
using BookNow.Application.Interfaces;
using BookNow.Application.RepoInterfaces;
using BookNow.Models;
using BookNow.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.Pkcs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookNow.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisLockService _redisLockService;
        private readonly IRealTimeNotifier _notifier;
        private readonly ILogger<PaymentService> _logger;
        private readonly IMessageBus _bus;

        public PaymentService(IUnitOfWork unitOfWork, IRedisLockService redisLockService,
                              IRealTimeNotifier notifier, ILogger<PaymentService> logger, IMessageBus bus)
        {
            _unitOfWork = unitOfWork;
            _redisLockService = redisLockService;
            _notifier = notifier;
            _logger = logger;
            _bus = bus;
        }

        public async Task<PaymentSummaryDTO?> GetBookingSummaryAsync(int bookingId, int cityId)
        {
            var booking = await _unitOfWork.Booking.GetAsync(
                b => b.BookingId == bookingId && b.BookingStatus == SD.BookingStatus_Pending,
                includeProperties: "Show.Movie,Show.Screen.Theatre,BookingSeats.SeatInstance.Seat,User",
                tracked: false);
            
            if (booking == null) return null;
            var city = await _unitOfWork.City.GetAsync(c => c.CityId == cityId, includeProperties: "Country", 
        tracked: false);
            string countryCode = city?.Country?.Code ?? "IND";
            string currencySymbol = CurrencyMapper.GetSymbolByCountryCode(countryCode);
            var seatLabels = booking.BookingSeats
                .OrderBy(bs => bs.SeatInstance.Seat.RowLabel)
                .ThenBy(bs => bs.SeatInstance.Seat.SeatIndex)
                .Select(bs => $"{bs.SeatInstance.Seat.RowLabel}{bs.SeatInstance.Seat.SeatIndex}")
                .ToList();

            var holdDuration = TimeSpan.FromMinutes(2);
            
            var holdExpiryTime = DateTime.SpecifyKind(booking.CreatedAt, DateTimeKind.Utc).Add(holdDuration);

      
            long expiryTimestamp = new DateTimeOffset(holdExpiryTime).ToUnixTimeSeconds();
            var serverUnixNow = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

          
            return new PaymentSummaryDTO
            {
                BookingId = booking.BookingId,
                TicketNumber = booking.TicketNumber,
                MovieTitle = booking.Show.Movie.Title,
                TheatreName = booking.Show.Screen.Theatre.TheatreName,
                ScreenName = booking.Show.Screen.ScreenNumber,
                ShowDateTime = booking.Show.StartTime,
                SeatLabels = seatLabels,
                TotalAmount = booking.TotalAmount,
                CurrencySymbol = currencySymbol,
                UserEmail = booking.User?.Email!,
                HoldExpiryUnixTimeSeconds = expiryTimestamp,
                ServerUnixTimeSeconds = serverUnixNow
            };
        }


        public async Task<string> ProcessGatewayResponseAsync(GatewayResponseDTO response)
        {
            _logger.LogInformation("Processing gateway response for BookingId {BookingId} with status {Status}.",
                                   response.BookingId, response.Status);

            string finalBookingStatus, finalSeatState, redirectPath;
            bool isSuccess = response.Status == "success";

            if (isSuccess)
            {
                finalBookingStatus = SD.BookingStatus_Confirmed;
                finalSeatState = SD.State_Booked;
                redirectPath = "Success";
            }
            else
            {
                 finalBookingStatus = (response.Status == "timeout") ? SD.BookingStatus_Expired : SD.BookingStatus_Cancelled;
                finalSeatState = SD.State_Available;
                redirectPath = response.Status == "timeout" ? "Timeout" : "Failed";
            }

            var b = await _unitOfWork.Booking.GetBookingSummaryAsync(response.BookingId);
            if (b == null) return "Failed";

            if (b.BookingStatus != SD.BookingStatus_Pending)
                return b.BookingStatus.Replace(SD.BookingStatus_Prefix, "");


            City city = null!;
            string countryCode = null!;
            string currencyCode = null!;
            string currencySymbol = null!;

            if (isSuccess)
            {
                city = await _unitOfWork.City.GetAsync(c => c.CityId == response.CityId,
                                                       includeProperties: "Country",
                                                       tracked: false);

                countryCode = city?.Country?.Code ?? "IND";
                currencyCode = CurrencyMapper.GetCurrencyCode(countryCode);
                currencySymbol = CurrencyMapper.GetSymbolByCountryCode(countryCode);
            }
        
            
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.Booking.ExecuteStatusUpdateAsync(b.BookingId, finalBookingStatus);
                await _unitOfWork.SeatInstance.ExecuteSeatStateUpdateAsync(b.SeatInstanceIds, finalSeatState);

                if (isSuccess)
                {
                       var txn = new PaymentTransaction
                        {
                            BookingId = b.BookingId,
                            Gateway = "SIMULATION",
                            GatewayOrderId = b.TicketNumber,
                            GatewayPaymentId = Guid.NewGuid().ToString(),
                            Amount = b.TotalAmount,
                            Currency = currencyCode,
                            Status = SD.PaymentStatus_Success,
                            AttemptNumber = 1,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            IdempotencyKey = response.IdempotencyKey,
                            RawResponse = System.Text.Json.JsonSerializer.Serialize(response)
                        };
                        await _unitOfWork.PaymentTransaction.AddAsync(txn);
                }

                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Failed to finalize booking {BookingId}.", response.BookingId);
                return "Failed";
            }


           var finalizeTask = FinalizePostCommit(b.ShowId, b.SeatInstanceIds, finalSeatState, b.IdempotencyKey);

           
            if (isSuccess)
            {
                var confirmationEvent = new BookingConfirmedEventDTO
                {
                    BookingId = b.BookingId,
                    UserEmail = b.UserEmail,
                    MovieTitle = b.MovieTitle,
                    ShowTime = b.ShowTime,
                    TotalAmount = b.TotalAmount,
                    CurrencySymbol = currencySymbol!
                };

                var reminder = new ShowReminderEventDTO
                {
                    BookingId = b.BookingId,
                    UserId = b.UserId,
                    TriggerAtUtc = b.ShowTime.AddMinutes(-10)
                };

               
                _ = Task.Run(async () =>
                {
                    await finalizeTask; 
                    await _bus.PublishAsync(confirmationEvent);
                    await _bus.PublishAsync(reminder);
                });
            }
            else
            {
               
                _ = Task.Run(async () => await finalizeTask);
            }

           
            return redirectPath;
        }

      
        
        private async Task FinalizePostCommit(int showId, List<int> seatInstanceIds, string finalSeatState, string lockToken)
        {
          
          
            foreach (var seatInstanceId in seatInstanceIds)
            {
                var lockKey = $"hold:{seatInstanceId}";
              
                await _redisLockService.ReleaseLockAsync(lockKey, lockToken);
            }

            
            await _notifier.NotifySeatUpdatesAsync(showId, seatInstanceIds, finalSeatState);
        }

       
        public async Task ReleaseSeatsAndLocksAsync(int bookingId)
        {
            var booking = await _unitOfWork.Booking.GetAsync(
                b => b.BookingId == bookingId,
                includeProperties: "BookingSeats.SeatInstance,Show",
                tracked: true);

            if (booking == null || booking.BookingStatus == SD.BookingStatus_Confirmed) return;

            using IDbContextTransaction transaction = await _unitOfWork.BeginTransactionAsync();
            List<int> seatInstanceIdsToRelease = new List<int>();

            try
            {
               
                foreach (var bookingSeat in booking.BookingSeats)
                {
                    var seatInstance = bookingSeat.SeatInstance;
                    if (seatInstance.State == SD.State_Held)
                    {
                        seatInstance.State = SD.State_Available;
                        _unitOfWork.SeatInstance.Update(seatInstance);
                        seatInstanceIdsToRelease.Add(seatInstance.SeatInstanceId);
                    }
                }

               
                if (booking.BookingStatus == SD.BookingStatus_Pending)
                {
                    booking.BookingStatus = SD.BookingStatus_Expired;
                    _unitOfWork.Booking.Update(booking);
                }

                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                if (seatInstanceIdsToRelease.Any())
                    await FinalizePostCommit(booking.Show.ShowId, seatInstanceIdsToRelease, SD.State_Available, booking.IdempotencyKey);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Cleanup failed for Booking {BookingId}", bookingId);
            }
        }
    }
}