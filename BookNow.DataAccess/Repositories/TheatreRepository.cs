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
    }
}
