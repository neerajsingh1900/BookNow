// In BookNow.Models.Interfaces/ISeatRepository.cs

using BookNow.Models;
using System.Collections.Generic;

namespace BookNow.Models.Interfaces
{
    public interface ISeatRepository : IRepository<Seat>
    {
        // Essential for performing an efficient bulk insert when a new screen is created
        void AddRange(IEnumerable<Seat> seats);

        // Fetches all seats for a specific screen (for configuration/layout)
        IEnumerable<Seat> GetSeatsByScreen(int screenId, string? includeProperties = null);
    }
}