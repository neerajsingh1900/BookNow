using AutoMapper;
using BookNow.Application.DTOs.CustomerDTOs.BookingDTOs;
using BookNow.Application.DTOs.CustomerDTOs.PaymentDTOs;
using BookNow.Application.Interfaces;
using BookNow.Application.RepoInterfaces;
using BookNow.Application.Validation.BookingValidations;
using BookNow.Models;
using BookNow.Utility;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;


namespace BookNow.Application.Services.Booking
{
    public class SeatBookingService : ISeatBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly  GetSeatLayoutQueryValidator _layoutValidator;
        private readonly ILogger<SeatBookingService> _logger;
        private readonly IRedisLockService _redisLockService;
        private readonly IRealTimeNotifier _notifier;
      
        public SeatBookingService( IUnitOfWork unitOfWork,IMapper mapper,GetSeatLayoutQueryValidator layoutValidator
            , ILogger<SeatBookingService> logger, IRedisLockService redisLockService, IRealTimeNotifier notifier)

        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _layoutValidator = layoutValidator;
            _logger = logger;
            _redisLockService = redisLockService;
            _notifier = notifier;
        }

        public async Task<SeatLayoutPageDTO> GetSeatLayoutAsync(int showId, int cityId)
        {
            _logger.LogInformation("Fetching seat layout for ShowId: {ShowId}, CityId: {CityId}", showId, cityId);

            var validationResult = _layoutValidator.Validate(showId);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for ShowId {ShowId}: {Errors}",
           showId, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

                throw new ValidationException(validationResult.Errors);
            }

           
            var cityData = await _unitOfWork.City.GetAllAsync( filter: c => c.CityId == cityId, includeProperties: "Country");

            var countryCode = cityData
                .AsQueryable() 
                .Select(c => c.Country.Code)
                .FirstOrDefault();

            var currencySymbol = CurrencyMapper.GetSymbolByCountryCode(countryCode);

            
            var seatInstances = await _unitOfWork.SeatInstance.GetSeatsWithStatusForShowAsync(showId);

            if (seatInstances == null || !seatInstances.Any())
            {
                _logger.LogWarning("No seat instances found for ShowId {ShowId}", showId);
                throw new KeyNotFoundException($"Show with ID {showId} not found or has no seat data.");
            }
            _logger.LogInformation("Fetched {SeatCount} seat instances for ShowId {ShowId}", seatInstances.Count(), showId);

            var allSeatsDto = _mapper.Map<IEnumerable<SeatStatusDTO>>(seatInstances).ToList();
            var firstSeat = seatInstances.First();

            var dto = new SeatLayoutPageDTO
            {
                ShowId = showId,
                MovieTitle = firstSeat.Show.Movie.Title,
                TheatreName = firstSeat.Show.Screen.Theatre.TheatreName,
                ScreenName = firstSeat.Show.Screen.ScreenNumber,
                StartTime = firstSeat.Show.StartTime,
                MovieLanguage = firstSeat.Show.Movie.Language,
                CurrencySymbol = currencySymbol,
                SeatsByRow = allSeatsDto
                    .GroupBy(s => s.RowLabel)
                    .OrderBy(g => g.Key)
                    .ToDictionary(
                        g => g.Key,
                        g => g.OrderBy(s => s.SeatIndex).ToList()
                    )
            };
            _logger.LogInformation("Seat layout successfully prepared for ShowId {ShowId}", showId);
            return dto;
        }
            
        public async Task<BookingRedirectDTO> CreateTransactionalHoldAsync(CreateHoldCommandDTO command,string userId,string userEmail)
        {
            _logger.LogInformation("Starting transactional hold (Hybrid) for User {UserId}, ShowId {ShowId}, Seats: {SeatCount}",
                 userId, command.ShowId, command.SeatInstanceIds.Count);

                var lockToken = $"{userId}:{Guid.NewGuid().ToString()}";
            var holdDuration = TimeSpan.FromMinutes(5); 

            var successfullyLockedSeats = new List<int>();

          
            foreach (var seatInstanceId in command.SeatInstanceIds)
            {
                var lockKey = $"hold:{seatInstanceId}";
                bool isLocked = await _redisLockService.AcquireLockAsync(lockKey, lockToken, holdDuration);

                if (!isLocked)
                {
                    _logger.LogWarning("Redis lock failed for Seat {SeatId}", seatInstanceId);

                    // Release any partial locks acquired in this batch
                    await ReleasePartialRedisLocks(successfullyLockedSeats, lockToken, command.ShowId);

                    return new BookingRedirectDTO
                    {
                        Success = false,
                        ErrorMessage = $"Seat  was just taken. Please refresh and try again."
                    };
                }
                successfullyLockedSeats.Add(seatInstanceId);
                await _notifier.NotifySeatUpdatesAsync(command.ShowId, successfullyLockedSeats, SD.State_Held); // 9️⃣ Construct redirect URL (same behaviour as before)
            }
            _logger.LogInformation("Successfully acquired Redis locks for all seats.");

            //1 Start DB transaction
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                // 3️⃣ Fetch all requested seat instances ONCE, tracked, including Seat + Screen
                var seatsListTracked = (await _unitOfWork.SeatInstance.GetAllAsync(
                        filter: si => command.SeatInstanceIds.Contains(si.SeatInstanceId),
                        includeProperties: "Seat,Seat.Screen",
                        orderBy: q => q.OrderBy(si => si.Seat.RowLabel).ThenBy(si => si.Seat.SeatIndex)))
                    .ToList();

                // Validate seat count integrity
                if (seatsListTracked.Count != command.SeatInstanceIds.Count)
                {
                    _logger.LogWarning("Seat validation failed: requested {Requested}, found {Found}", command.SeatInstanceIds.Count, seatsListTracked.Count);

                    await transaction.RollbackAsync();
                    return new BookingRedirectDTO
                    {
                        Success = false,
                        ErrorMessage = "One or more selected seats are invalid or missing."
                    };
                }

                decimal totalAmount = 0m;

                // 4️⃣ Manual concurrency + availability check
                foreach (var seat in seatsListTracked)
                {
                    // Must be available
                    if (seat.State != SD.State_Available)
                    {
              _logger.LogWarning("Seat {SeatId} ({SeatNumber}) unavailable during hold attempt", seat.SeatInstanceId, seat.Seat.SeatNumber);
    
                        await transaction.RollbackAsync();
                        return new BookingRedirectDTO
                        {
                            Success = false,
                            ErrorMessage = $"Seat {seat.Seat.SeatNumber} is no longer available. Please refresh."
                        };
                    }

                    // RowVersion (client vs server)
                    //if (!command.SeatVersions.TryGetValue(seat.SeatInstanceId, out string clientVersionBase64) ||
                    //    !Convert.ToBase64String(seat.RowVersion).Equals(clientVersionBase64))
                    //{
                    //    _logger.LogWarning("RowVersion mismatch for SeatInstanceId {SeatId}", seat.SeatInstanceId);

                    //    await transaction.RollbackAsync();
                    //    return new BookingRedirectDTO
                    //    {
                    //        Success = false,
                    //        ErrorMessage = "A concurrency error occurred (RowVersion mismatch). Please refresh the page."
                    //    };
                    //}
                }

                // 5️⃣ Lock seats (mark as held) and calculate total
                foreach (var seat in seatsListTracked)
                {
                    seat.State = SD.State_Held;
                    totalAmount += seat.Seat.Screen.DefaultSeatPrice;
                    _unitOfWork.SeatInstance.Update(seat);
                }

                // 6️⃣ Create a pending booking
                var booking = new BookNow.Models.Booking
                {
                    UserId = userId,
                    ShowId = command.ShowId,
                    TotalAmount = totalAmount,
                    BookingStatus = SD.BookingStatus_Pending,
                    CreatedAt = DateTime.UtcNow,
                    TicketNumber = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                    IdempotencyKey = lockToken
                };

                await _unitOfWork.Booking.AddAsync(booking);
                await _unitOfWork.SaveAsync();
         _logger.LogInformation("Created pending booking {BookingId} for User {UserId}", booking.BookingId, userId);

                // 7️⃣ Create BookingSeat entries
                var bookingSeats = seatsListTracked.Select(seat => new BookingSeat
                {
                    BookingId = booking.BookingId,
                    SeatInstanceId = seat.SeatInstanceId,
                    Price = seat.Seat.Screen.DefaultSeatPrice
                }).ToList();

                await _unitOfWork.BookingSeat.AddRangeAsync(bookingSeats);

                // 8️⃣ Save all + commit transaction (EF handles concurrency here)
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();
       _logger.LogInformation("Booking {BookingId} committed successfully with total amount {Amount}", booking.BookingId, totalAmount);

             //  await _notifier.NotifySeatUpdatesAsync(command.ShowId, successfullyLockedSeats, SD.State_Held); // 9️⃣ Construct redirect URL (same behaviour as before)
                string redirectUrl = $"/Customer/Payment/Gateway?bookingId={booking.BookingId}";

                return new BookingRedirectDTO
                {
                    Success = true,
                    RedirectUrl = redirectUrl
                };
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await transaction.RollbackAsync();
                await ReleasePartialRedisLocks(successfullyLockedSeats, lockToken, command.ShowId);

                _logger.LogError(ex, "DB concurrency or state mismatch error during commit.");

                return new BookingRedirectDTO
                {
                    Success = false,
                    ErrorMessage = "A race condition occurred! Some seats were just taken. Please refresh and try again."
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                await ReleasePartialRedisLocks(successfullyLockedSeats, lockToken, command.ShowId);
                _logger.LogError(ex, "Failed to create hold for User {UserId}", userId);
                return new BookingRedirectDTO
                {
                    Success = false,
                    ErrorMessage = "An unexpected error occurred during the hold process: " + ex.Message
                };
            }
        }



      
        private async Task ReleasePartialRedisLocks(List<int> seatInstanceIds, string lockToken, int showId) // <-- ADDED showId
        {
            if (!seatInstanceIds.Any()) return;
            
            foreach (var seatId in seatInstanceIds)
            {
                var lockKey = $"hold:{seatId}";
                if (await _redisLockService.ReleaseLockAsync(lockKey, lockToken))
                {
                    _logger.LogDebug("Released Redis lock for Seat {SeatId}", seatId);

                    // Use the passed-in showId
                    await _notifier.NotifySeatUpdatesAsync(showId, new List<int> { seatId }, SD.State_Available);
                }
            }
        }
    }
}
