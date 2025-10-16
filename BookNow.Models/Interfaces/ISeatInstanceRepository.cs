using BookNow.Models;

namespace BookNow.Models.Interfaces
{
    // Currently extends IRepository<SeatInstance>, but ready for future methods 
    // like GetBookedSeatsForShowAsync or UpdateSeatStateBatchAsync.
    public interface ISeatInstanceRepository : IRepository<SeatInstance>
    {
    }
}