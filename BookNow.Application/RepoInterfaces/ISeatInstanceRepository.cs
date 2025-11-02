using BookNow.Models;

namespace BookNow.Application.RepoInterfaces
{

    public interface ISeatInstanceRepository : IRepository<SeatInstance>
    {
        Task<IEnumerable<SeatInstance>> GetSeatsWithStatusForShowAsync(int showId);
    }
}