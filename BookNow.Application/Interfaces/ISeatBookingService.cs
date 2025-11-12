using BookNow.Application.DTOs.CustomerDTOs.BookingDTOs;
using BookNow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.Interfaces
{
    public interface ISeatBookingService
    {
        Task<SeatLayoutPageDTO> GetSeatLayoutAsync(int showId, int cityId);

        Task<BookingRedirectDTO> CreateTransactionalHoldAsync(
            CreateHoldCommandDTO command,
            string userId,
            string userEmail
        );
     }
}
