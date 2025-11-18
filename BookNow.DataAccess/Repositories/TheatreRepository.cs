using BookNow.DataAccess.Data;
using BookNow.Models;
using BookNow.Application.RepoInterfaces;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.DataAccess.Repositories
{
    public class TheatreRepository : Repository<Theatre>, ITheatreRepository
    {
        private readonly ApplicationDbContext _db;

        public TheatreRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<Theatre> GetTheatresByOwner(string ownerId)
        {
            return _db.Theatres
                .Where(t => t.OwnerId == ownerId)
                .Include(t => t.City)
                    .ThenInclude(c => c.Country)
                .ToList();
        }

       
        public async Task<bool> IsNameConflictingAsync(string name, int? theatreIdToExclude = null)
        {
            IQueryable<Theatre> query = _db.Theatres.AsNoTracking()
            .Where(t => t.TheatreName == name);

            if (theatreIdToExclude.HasValue && theatreIdToExclude.Value > 0)
            {
                query = query.Where(t => t.TheatreId != theatreIdToExclude.Value);
            }

            return await query.AnyAsync();

        }

       
        public async Task<bool> IsEmailConflictingAsync(string email, int? theatreIdToExclude)
        {
            IQueryable<Theatre> query = _db.Theatres.AsNoTracking()
                .Where(t => t.Email == email);

            if (theatreIdToExclude.HasValue && theatreIdToExclude.Value > 0)
            {
                query = query.Where(t => t.TheatreId != theatreIdToExclude.Value);
            }

            
            return await query.AnyAsync();
        }

        public async Task<bool> HasActiveBookings(int theatreId)
        {
            var now = DateTime.Now;

            return await _db.Bookings
                .AnyAsync(b =>
                    b.Show!.Screen!.TheatreId == theatreId &&
                    b.Show.EndTime > now &&
                    (b.BookingStatus == "Confirmed" || b.BookingStatus == "Pending")
                );
        }
        public async Task<Theatre?> GetTheatreForSoftDelete(int theatreId)
        {
            return await _db.Theatres
                .IgnoreQueryFilters()
                .Include(t => t.Screens)
                    .ThenInclude(s => s.Shows.Where(sh => sh.EndTime > DateTime.Now && !sh.IsDeleted)) 
                .FirstOrDefaultAsync(t => t.TheatreId == theatreId);
        }

        public async Task CascadeSoftDelete(int theatreId)
        {
            var theatre = await GetTheatreForSoftDelete(theatreId);

            if (theatre == null)
            {
                throw new KeyNotFoundException($"Theatre ID {theatreId} not found.");
            }

            theatre.SoftDelete();

            foreach (var screen in theatre.Screens)
            {
                screen.SoftDelete();
                _db.Screens.Update(screen);

                foreach (var show in screen.Shows)
                {
                    show.SoftDelete();
                }
                _db.Shows.UpdateRange(screen.Shows);
            }
            _db.Theatres.Update(theatre);
        }
}
}
