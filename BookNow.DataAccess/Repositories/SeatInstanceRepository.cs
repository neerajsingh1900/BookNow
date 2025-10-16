using BookNow.DataAccess.Data;
using BookNow.Models;
using BookNow.Models.Interfaces;
using System.Threading.Tasks;

namespace BookNow.DataAccess.Repositories
{
    public class SeatInstanceRepository : Repository<SeatInstance>, ISeatInstanceRepository
    {
        public SeatInstanceRepository(ApplicationDbContext db) : base(db) { }

        // Future methods for complex seat status updates or bulk changes will go here.
    }
}