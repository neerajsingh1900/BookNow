using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace BookNow.Web.Hubs
{
    // The Hub manages client connections and group assignments
    public class SeatMapHub : Hub
    {
        // Clients call this when they load the seat map page
        public async Task JoinShowGroup(int showId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, showId.ToString());
        }

        // Clients call this when they navigate away
        public async Task LeaveShowGroup(int showId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, showId.ToString());
        }
    }
}