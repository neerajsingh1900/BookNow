using BookNow.DataAccess.Data;
using BookNow.Models;
using BookNow.Models.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace BookNow.DataAccess.Repositories
{
    public class SeatRepository : Repository<Seat>, ISeatRepository
    {
        private readonly ApplicationDbContext _db;

        public SeatRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        // AddRange is implemented in the base Repository<T> now.

        public IEnumerable<Seat> GetSeatsByScreen(int screenId, string? includeProperties = null)
        {
            // Order by RowLabel and SeatIndex for consistent layout
            return GetAll(
                filter: s => s.ScreenId == screenId,
                orderBy: q => q.OrderBy(s => s.RowLabel).ThenBy(s => s.SeatIndex),
                includeProperties: includeProperties
            );
        }
    }
}