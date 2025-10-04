using BookNow.DataAccess.Data;
using BookNow.Models;
using BookNow.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
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

        public void UpdateTheatreStatus(int theatreId, string status)
        {
            var theatreFromDb = _db.Theatres.FirstOrDefault(t => t.TheatreId == theatreId);
            if (theatreFromDb != null)
            {
                // The 'Status' column exists in the Theatres schema, so we update it directly.
                theatreFromDb.Status = status;
                base.Update(theatreFromDb); // Calls generic update logic
            }
        }
    }
}
