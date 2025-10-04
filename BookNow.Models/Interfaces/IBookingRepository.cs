using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Models.Interfaces
{
    public interface IBookingRepository : IRepository<Booking>
    {
        IEnumerable<Booking> GetUserBookingsWithDetails(string userId);
    }
}
