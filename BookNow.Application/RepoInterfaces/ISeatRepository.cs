using BookNow.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookNow.Application.RepoInterfaces
{
    public interface ISeatRepository : IRepository<Seat>
    {
        Task<IEnumerable<Seat>> GetSeatsByScreenAsync(int screenId, string? includeProperties = null);
    }
}