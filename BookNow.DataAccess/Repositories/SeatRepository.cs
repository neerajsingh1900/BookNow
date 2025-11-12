using BookNow.DataAccess.Data;
using BookNow.Models;
using BookNow.Application.RepoInterfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookNow.DataAccess.Repositories
{
    public class SeatRepository : Repository<Seat>, ISeatRepository
    {
        public SeatRepository(ApplicationDbContext db) : base(db) { }

        public async Task<IEnumerable<Seat>> GetSeatsByScreenAsync(int screenId, string? includeProperties = null)
        {
            return await GetAllAsync(
                filter: s => s.ScreenId == screenId,
                orderBy: q => q.OrderBy(s => s.RowLabel).ThenBy(s => s.SeatIndex),
                includeProperties: includeProperties
            );
        }

    }
}