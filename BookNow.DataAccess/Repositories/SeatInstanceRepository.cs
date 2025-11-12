using BookNow.DataAccess.Data;
using BookNow.Models;
using BookNow.Application.RepoInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFCore.BulkExtensions;


namespace BookNow.DataAccess.Repositories
{
    public class SeatInstanceRepository : Repository<SeatInstance>, ISeatInstanceRepository
    {
        private readonly ApplicationDbContext _db;

        public SeatInstanceRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<IEnumerable<SeatInstance>> GetSeatsWithStatusForShowAsync(int showId)
        {
            return await dbSet
                .AsNoTracking()
                .Where(si => si.ShowId == showId)
                .Include(si => si.Seat)
                    .ThenInclude(s => s.Screen)
                .Include(si => si.Show)
                    .ThenInclude(sh => sh.Movie)
                .Include(si => si.Show)
                    .ThenInclude(sh => sh.Screen)
                        .ThenInclude(sc => sc.Theatre)
                .OrderBy(si => si.Seat.RowLabel)
                .ThenBy(si => si.Seat.SeatIndex)
                .ToListAsync();
        }
        

  public async Task ExecuteSeatStateUpdateAsync(List<int> seatInstanceIds, string state)
    {
            if (seatInstanceIds == null || seatInstanceIds.Count == 0)
                return;

            const int batchSize = 10000; 
            int total = seatInstanceIds.Count;

            for (int i = 0; i < total; i += batchSize)
            {
                var batch = seatInstanceIds.Skip(i).Take(batchSize).ToList();

                await _db.SeatInstances
                    .Where(s => batch.Contains(s.SeatInstanceId))
                    .ExecuteUpdateAsync(updates =>
                        updates.SetProperty(s => s.State, state)
                    );
            }
        }


}
}
