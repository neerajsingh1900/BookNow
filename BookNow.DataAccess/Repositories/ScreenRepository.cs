using BookNow.DataAccess.Data;
using BookNow.Models;
using BookNow.Application.RepoInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookNow.DataAccess.Repositories
{
    public class ScreenRepository : Repository<Screen>, IScreenRepository
    {
        private readonly ApplicationDbContext _db;

        public ScreenRepository(ApplicationDbContext db) : base(db) {
            _db = db;
        }

        public async Task<bool> IsScreenNumberUniqueAsync(int theatreId, string screenNumber, int? excludeScreenId = null)
        {
            
            var query = dbSet.AsNoTracking()
                .Where(s => s.TheatreId == theatreId && s.ScreenNumber == screenNumber);

            if (excludeScreenId.HasValue)
            {
                
                query = query.Where(s => s.ScreenId != excludeScreenId.Value);
            }

            return !await query.AnyAsync();
        }
      


        public async Task<IEnumerable<Screen>> GetScreensByTheatreAsync(int theatreId, string? includeProperties = null)
        {
            
            return await GetAllAsync(
               filter: s => s.TheatreId == theatreId,
               includeProperties: includeProperties);
        }

        public async Task<Screen?> GetScreenForSoftDelete(int screenId)
        {
            return await _db.Screens
                .IgnoreQueryFilters()
                .Include(s => s.Shows.Where(sh => sh.EndTime > DateTime.Now && !sh.IsDeleted)) // Load only future, non-deleted shows
                .FirstOrDefaultAsync(s => s.ScreenId == screenId);
        }

        public async Task<bool> HasActiveBookings(int screenId)
        {
            var now = DateTime.Now;
          return await _db.Bookings
                .AnyAsync(b =>
                    b.Show!.ScreenId == screenId &&
                    b.Show.EndTime > now &&
                    (b.BookingStatus == "Confirmed" || b.BookingStatus == "Pending")
                );
        }

        public async Task CascadeSoftDelete(int screenId)
        {
            var screen = await GetScreenForSoftDelete(screenId);

            if (screen == null)
            {
                throw new KeyNotFoundException($"Screen ID {screenId} not found.");
            }

         
            screen.SoftDelete();
            _db.Screens.Update(screen);

          
            foreach (var show in screen.Shows)
            {
                show.SoftDelete();
            }
            _db.Shows.UpdateRange(screen.Shows);

        }
    }
}