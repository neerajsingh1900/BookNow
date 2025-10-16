using BookNow.DataAccess.Data;
using BookNow.Models;
using BookNow.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookNow.DataAccess.Repositories
{
    public class ScreenRepository : Repository<Screen>, IScreenRepository
    {
        // The base class constructor handles the _db context.

        public ScreenRepository(ApplicationDbContext db) : base(db) { }

        public async Task<bool> IsScreenNumberUniqueAsync(int theatreId, string screenNumber, int? excludeScreenId = null)
        {
            // Optimization: Use AsNoTracking for a simple existence check.
            var query = dbSet.AsNoTracking()
                .Where(s => s.TheatreId == theatreId && s.ScreenNumber == screenNumber);

            if (excludeScreenId.HasValue)
            {
                // Exclude the current screen when checking for updates
                query = query.Where(s => s.ScreenId != excludeScreenId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<IEnumerable<Screen>> GetScreensByTheatreAsync(int theatreId, string? includeProperties = null)
        {
            // Uses the base Repository's GetAllAsync method
            return await GetAllAsync(
               filter: s => s.TheatreId == theatreId,
               includeProperties: includeProperties);
        }
    }
}