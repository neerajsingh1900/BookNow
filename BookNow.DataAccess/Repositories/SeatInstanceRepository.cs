//using BookNow.DataAccess.Data;
//using BookNow.Models;
//using BookNow.Models.Interfaces;
//using Microsoft.EntityFrameworkCore;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace BookNow.DataAccess.Repositories
//{

//    public class SeatInstanceRepository : Repository<SeatInstance>, ISeatInstanceRepository
//    {
//        private readonly ApplicationDbContext _db;

//        public SeatInstanceRepository(ApplicationDbContext db) : base(db)
//        {
//            _db = db;
//        }

//        public async Task<IEnumerable<SeatInstance>> GetSeatsWithStatusForShowAsync(int showId)
//        {
//            return await dbSet.AsNoTracking()
//                .Where(si => si.ShowId == showId)
//                .Include(si => si.Seat)
//                    .ThenInclude(s => s.Screen)
//                        .ThenInclude(sc => sc.Theatre) 
//                .Include(si => si.Show)
//                    .ThenInclude(sh => sh.Movie) 
//                .OrderBy(si => si.Seat.RowLabel)
//                .ThenBy(si => si.Seat.SeatIndex)
//                .ToListAsync();
//        }
//    }
//}
using BookNow.DataAccess.Data;
using BookNow.Models;
using BookNow.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookNow.DataAccess.Repositories
{
    public class SeatInstanceRepository : Repository<SeatInstance>, ISeatInstanceRepository
    {
        private readonly ApplicationDbContext _db;

        public SeatInstanceRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        //public async Task<IEnumerable<SeatInstance>> GetSeatsWithStatusForShowAsync(int showId)
        //{
        //    return await dbSet
        //        .AsNoTracking()
        //        .Where(si => si.ShowId == showId)
        //        // Load Seat info for layout rows/columns
        //        .Include(si => si.Seat)
        //        // Load related Show + Movie
        //        .Include(si => si.Show)
        //            .ThenInclude(sh => sh.Movie)
        //        // Load Show → Screen → Theatre (for names and pricing)
        //        .Include(si => si.Show)
        //            .ThenInclude(sh => sh.Screen)
        //                .ThenInclude(sc => sc.Theatre)
        //        // Order by row/seat for easy UI grouping
        //        .OrderBy(si => si.Seat.RowLabel)
        //        .ThenBy(si => si.Seat.SeatIndex)
        //        .ToListAsync();
        //}
        public async Task<IEnumerable<SeatInstance>> GetSeatsWithStatusForShowAsync(int showId)
        {
            return await dbSet
                .AsNoTracking()
                .Where(si => si.ShowId == showId)
                // Seat + Screen for pricing
                .Include(si => si.Seat)
                    .ThenInclude(s => s.Screen)
                // Show + Movie
                .Include(si => si.Show)
                    .ThenInclude(sh => sh.Movie)
                // Show → Screen → Theatre (for names and metadata)
                .Include(si => si.Show)
                    .ThenInclude(sh => sh.Screen)
                        .ThenInclude(sc => sc.Theatre)
                // Ordered layout for UI
                .OrderBy(si => si.Seat.RowLabel)
                .ThenBy(si => si.Seat.SeatIndex)
                .ToListAsync();
        }

    }
}
