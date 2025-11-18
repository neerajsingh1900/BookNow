using BookNow.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookNow.Application.RepoInterfaces
{
    public interface IScreenRepository : IRepository<Screen>
    {
        Task<bool> IsScreenNumberUniqueAsync(int theatreId, string screenNumber, int? excludeScreenId = null);
        Task<IEnumerable<Screen>> GetScreensByTheatreAsync(int theatreId, string? includeProperties = null);
        Task<Screen?> GetScreenForSoftDelete(int screenId);
        Task CascadeSoftDelete(int screenId);

        Task<bool> HasActiveBookings(int screenId);
    }
}