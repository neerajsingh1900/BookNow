using BookNow.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookNow.Models.Interfaces
{
    public interface ISeatRepository : IRepository<Seat>
    {
        Task<IEnumerable<Seat>> GetSeatsByScreenAsync(int screenId, string? includeProperties = null);
    }
}