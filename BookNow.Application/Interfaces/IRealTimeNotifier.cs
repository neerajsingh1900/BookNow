using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookNow.Application.Interfaces
{
    
    public interface IRealTimeNotifier
    {
        // Method signature that only uses primitive types or Application DTOs
        Task NotifySeatUpdatesAsync(int showId, List<int> seatInstanceIds, string newState);
    }
}