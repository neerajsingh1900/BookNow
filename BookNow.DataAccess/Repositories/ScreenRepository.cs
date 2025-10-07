using BookNow.DataAccess.Data;
using BookNow.Models;
using BookNow.Models.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace BookNow.DataAccess.Repositories
{
    public class ScreenRepository : Repository<Screen>, IScreenRepository
    {
        private readonly ApplicationDbContext _db;

        public ScreenRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<Screen> GetScreensByTheatre(int theatreId, string? includeProperties = null)
        {
            return GetAll(s => s.TheatreId == theatreId, includeProperties: includeProperties);
        }

        public bool IsScreenNumberUnique(int theatreId, string screenNumber, int? excludeScreenId = null)
        {
            var query = dbSet.Where(s => s.TheatreId == theatreId && s.ScreenNumber == screenNumber);

            if (excludeScreenId.HasValue)
            {
                // Exclude the current screen when checking for updates
                query = query.Where(s => s.ScreenId != excludeScreenId.Value);
            }

            return !query.Any();
        }
    }
}