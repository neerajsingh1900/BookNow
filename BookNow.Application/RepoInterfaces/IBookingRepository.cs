using BookNow.Application.DTOs.CustomerDTOs.BookingDTOs;
using BookNow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.RepoInterfaces
{
    public interface IBookingRepository : IRepository<Booking>
    {
        IEnumerable<Booking> GetUserBookingsWithDetails(string userId);
        Task<BookingSummaryDTO?> GetBookingSummaryAsync(int bookingId);
        Task ExecuteStatusUpdateAsync(int bookingId, string newStatus);
    }
}
