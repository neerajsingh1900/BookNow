using BookNow.DataAccess.Data;
using BookNow.Models;
using BookNow.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
