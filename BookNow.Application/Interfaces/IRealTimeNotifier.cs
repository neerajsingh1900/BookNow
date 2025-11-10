using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookNow.Application.Interfaces
{
    
    public interface IRealTimeNotifier
    {
        
        Task NotifySeatUpdatesAsync(int showId, List<int> seatInstanceIds, string newState);
    }
}