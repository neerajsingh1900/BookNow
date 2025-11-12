using BookNow.Models;

namespace BookNow.Application.RepoInterfaces
{

    public interface ISeatInstanceRepository : IRepository<SeatInstance>
    {
        Task<IEnumerable<SeatInstance>> GetSeatsWithStatusForShowAsync(int showId);
        Task ExecuteSeatStateUpdateAsync(List<int> seatInstanceIds, string state);
}
}