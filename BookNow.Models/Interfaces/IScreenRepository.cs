// In BookNow.Models.Interfaces/IScreenRepository.cs

using BookNow.Models;
using System.Collections.Generic;

namespace BookNow.Models.Interfaces
{
    public interface IScreenRepository : IRepository<Screen>
    {
        // Fetches all screens belonging to a specific theatre
        IEnumerable<Screen> GetScreensByTheatre(int theatreId, string? includeProperties = null);

        // Checks for screen number uniqueness within a theatre
        bool IsScreenNumberUnique(int theatreId, string screenNumber, int? excludeScreenId = null);
    }
}