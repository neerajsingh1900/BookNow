using BookNow.Application.DTOs.EventDTOs;
using BookNow.Application.DTOs.PaymentDTOs;
using BookNow.Application.Interfaces;
using BookNow.Application.RepoInterfaces;
using BookNow.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

            return new PaymentSummaryDTO
            {
                BookingId = booking.BookingId,
                TicketNumber = booking.TicketNumber,
                MovieTitle = booking.Show.Movie.Title,
                TheatreName = booking.Show.Screen.Theatre.TheatreName,
                ScreenName = booking.Show.Screen.ScreenNumber,
                ShowDateTime = booking.Show.StartTime,
                SeatLabels = seatLabels,
                TotalAmount = booking.TotalAmount +18,
                CurrencySymbol = currencySymbol,
                UserEmail = booking.User.Email
            };
        }

       
        public async Task<string> ProcessGatewayResponseAsync(GatewayResponseDTO response)
        {
            var booking = await _unitOfWork.Booking.GetAsync(
                b => b.BookingId == response.BookingId,
                includeProperties: "BookingSeats.SeatInstance,Show",
                tracked: true);

            if (booking == null) return "Failed";

            var existingTxn = await _unitOfWork.PaymentTransaction.GetAsync(
     pt => pt.IdempotencyKey == response.IdempotencyKey
 );

            if (existingTxn != null)
            {
                _logger.LogInformation(
       "Duplicate payment callback detected. Booking {BookingId}, Key {Key}",
       response.BookingId, response.IdempotencyKey
   );
                return existingTxn.Status switch
                {
                    SD.PaymentStatus_Success => "Success",
                    SD.PaymentStatus_Failed => "Failed",
                    _ => "Timeout"
                };
            }

            int cityId = response.CityId;
            var city = await _unitOfWork.City.GetAsync( c => c.CityId == cityId,
        includeProperties: "Country",
        tracked: false);

            string countryCode = city?.Country?.Code ?? "IND";
            string currencyCode = CurrencyMapper.GetCurrencyCode(countryCode);

            if (booking.BookingStatus != SD.BookingStatus_Pending)
            {
                return booking.BookingStatus.Replace(SD.BookingStatus_Prefix, ""); 
            }

            string finalBookingStatus;
            string finalSeatState;
            string redirectPath;

            if (response.Status == "success")
            {
                finalBookingStatus = SD.BookingStatus_Confirmed;
                finalSeatState = SD.State_Booked;
                redirectPath = "Success";
            }
            else
            {
                finalBookingStatus = (response.Status == "timeout")
                    ? SD.BookingStatus_Expired : SD.BookingStatus_Cancelled;
                finalSeatState = SD.State_Available;
                redirectPath = response.Status == "timeout" ? "Timeout" : "Failed";
            }
        
            using IDbContextTransaction transaction = await _unitOfWork.BeginTransactionAsync();
            List<int> seatInstanceIds = booking.BookingSeats.Select(bs => bs.SeatInstanceId).ToList();
                
            try
            {
                
                booking.BookingStatus = finalBookingStatus;
                _unitOfWork.Booking.Update(booking);

                foreach (var bookingSeat in booking.BookingSeats)
                {
                    bookingSeat.SeatInstance.State = finalSeatState;
                    _unitOfWork.SeatInstance.Update(bookingSeat.SeatInstance);
                }
                if (response.Status == "success")
                {
                    int lastAttempt = (await _unitOfWork.PaymentTransaction.GetAllAsync(
         pt => pt.BookingId == booking.BookingId
     ))
     .Select(pt => pt.AttemptNumber)
     .DefaultIfEmpty(0)
     .Max();

                   

                    var paymentTxn = new BookNow.Models.PaymentTransaction
                    {
                        BookingId = booking.BookingId,
                        Gateway = "SIMULATION",
                        GatewayOrderId = booking.TicketNumber,
                        GatewayPaymentId = Guid.NewGuid().ToString(),
                        Amount = booking.TotalAmount,
                        Currency = currencyCode, 
                        Status = SD.PaymentStatus_Success, 
                        AttemptNumber = lastAttempt + 1,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IdempotencyKey = response.IdempotencyKey,   
                        RawResponse = System.Text.Json.JsonSerializer.Serialize(response)
                    };

                    await _unitOfWork.PaymentTransaction.AddAsync(paymentTxn);
                    _logger.LogInformation("PaymentTransaction record created for Booking {BookingId}", booking.BookingId);
                }

                
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();
               
                var reminder = new ShowReminderEventDTO
                {
                    BookingId = booking.BookingId,
                    UserId = booking.UserId,
                    TriggerAtUtc = booking.Show.StartTime.AddMinutes(-10)
                };

               

                await FinalizePostCommit(booking.Show.ShowId, seatInstanceIds, finalSeatState, booking.IdempotencyKey);
              
               await _bus.PublishAsync(reminder);

                return redirectPath;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                await ReleaseSeatsAndLocksAsync(response.BookingId); // Attempt cleanup regardless of DB failure
                _logger.LogError(ex, "Failed to finalize booking {BookingId}.", response.BookingId);
                return "Failed";
            }
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