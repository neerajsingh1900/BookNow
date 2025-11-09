using BookNow.Application.DTOs;
using BookNow.Application.DTOs.CustomerDTOs.BookingDTOs;
using BookNow.Application.Interfaces;
using BookNow.Application.RepoInterfaces;
using BookNow.Utility;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookNow.Application.Services
{
    public class BookingHistoryService : IBookingHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingHistoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<BookingHistoryDTO>> GetFullHistoryAsync(string userId)
        {
            // MOST OPTIMAL QUERY: Single database trip with Eager Loading and Projection (using DTO).
            var bookings = await _unitOfWork.Booking.GetAllAsync(
                filter: b => b.UserId == userId && b.BookingStatus == SD.BookingStatus_Confirmed,
                orderBy: q => q.OrderByDescending(b => b.Show.StartTime),
                includeProperties: "Show.Movie,Show.Screen.Theatre.City.Country,BookingSeats.SeatInstance.Seat,PaymentTransactions"
            );

            var history = bookings.Select(b =>
            {
                var seatLabels = b.BookingSeats
                    .OrderBy(bs => bs.SeatInstance.Seat.RowLabel)
                    .ThenBy(bs => bs.SeatInstance.Seat.SeatIndex)
                    .Select(bs => $"{bs.SeatInstance.Seat.RowLabel}{bs.SeatInstance.Seat.SeatIndex}")
                    .ToList();

                var latestPayment = b.PaymentTransactions
                    .Where(pt => pt.Status == SD.PaymentStatus_Success)
                    .OrderByDescending(pt => pt.CreatedAt)
                    .FirstOrDefault();

                return new BookingHistoryDTO
                {
                    BookingId = b.BookingId,
                    MovieTitle = b.Show.Movie.Title,
                    MovieImageUrl = b.Show.Movie.PosterUrl ?? "/images/placeholder.jpg",
                    ShowDateTime = b.Show.StartTime,
                    TheatreName = b.Show.Screen.Theatre.TheatreName,
                    TheatreAddress = b.Show.Screen.Theatre.Address ?? "Address not available",
                    ScreenName = b.Show.Screen.ScreenNumber,
                    SeatLabels = string.Join(", ", seatLabels),
                    City = b.Show.Screen.Theatre.City.Name ?? "N/A",          // Accesses the Name property of the City object
                    Country = b.Show.Screen.Theatre.City.Country.Name ?? "N/A",
                    TicketNumber = b.TicketNumber,
                    TotalAmount = b.TotalAmount,
                    CurrencySymbol = CurrencyMapper.GetSymbolByCurrencyCode(latestPayment?.Currency),
                    TxnNo = latestPayment?.PaymentTxnId ??0,
                };
            }).ToList();

            return history;
        }

        public async Task<int> GetUpcomingCountAsync(string userId)
        {
           DateTime now = DateTime.Now;

            var upcomingBookings = await _unitOfWork.Booking.GetAllAsync(
                filter: b => b.UserId == userId
                            && b.BookingStatus == SD.BookingStatus_Confirmed
                            && b.Show.StartTime > now,
                includeProperties: null 
            );

            return upcomingBookings.Count();
        }
    }
}