using BookNow.DataAccess.Data;
using BookNow.Models;
using BookNow.Application.RepoInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookNow.Application.DTOs.CustomerDTOs.BookingDTOs;

namespace BookNow.DataAccess.Repositories
{
    public class BookingRepository : Repository<Booking>, IBookingRepository
    {
        private readonly ApplicationDbContext _db;

        public BookingRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<Booking> GetUserBookingsWithDetails(string userId)
        {
            return _db.Bookings
                .Include(b => b.Show)
                    .ThenInclude(s => s.Movie)
                .Include(b => b.BookingSeats)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .ToList();
        }

        public async Task<BookingSummaryDTO?> GetBookingSummaryAsync(int bookingId)
        {
            return await _db.Bookings
                .Where(b => b.BookingId == bookingId)
                .Select(b => new BookingSummaryDTO
                {
                    BookingId = b.BookingId,
                    BookingStatus = b.BookingStatus,
                    UserId = b.UserId,
                    TicketNumber = b.TicketNumber,
                    TotalAmount = b.TotalAmount,
                    IdempotencyKey = b.IdempotencyKey,
                    UserEmail = b.User.Email!,
                    ShowId = b.Show.ShowId,
                    MovieTitle = b.Show.Movie.Title,
                    ShowTime = b.Show.StartTime,
                    SeatInstanceIds = b.BookingSeats.Select(bs => bs.SeatInstanceId).ToList()
                })
                .FirstOrDefaultAsync();
        }


        public async Task ExecuteStatusUpdateAsync(int bookingId, string newStatus)
        {
            await _db.Bookings
                .Where(b => b.BookingId == bookingId)
                .ExecuteUpdateAsync(s => s.SetProperty(b => b.BookingStatus, newStatus));
        }

    }
}
