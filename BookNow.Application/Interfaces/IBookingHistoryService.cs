using BookNow.Application.DTOs.CustomerDTOs.BookingDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.Interfaces
{
    public interface IBookingHistoryService
    {
        Task<List<BookingHistoryDTO>> GetFullHistoryAsync(string userId);
        Task<int> GetUpcomingCountAsync(string userId);
    }
}
