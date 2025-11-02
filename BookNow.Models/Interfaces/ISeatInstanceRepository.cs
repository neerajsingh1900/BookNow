using BookNow.Models;

namespace BookNow.Models.Interfaces
{

    public interface ISeatInstanceRepository : IRepository<SeatInstance>
    {
        Task<IEnumerable<SeatInstance>> GetSeatsWithStatusForShowAsync(int showId);
    }
}