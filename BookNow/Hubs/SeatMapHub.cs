using BookNow.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace BookNow.Web.Hubs
{
   
    public class SeatMapHub : Hub
    {
        public async Task JoinShowGroup(int showId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, showId.ToString());
        }

        
        public async Task LeaveShowGroup(int showId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, showId.ToString());
        }
       
    }
}