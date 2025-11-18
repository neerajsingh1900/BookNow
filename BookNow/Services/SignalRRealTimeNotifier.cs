using BookNow.Application.Interfaces;
using BookNow.Web.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace BookNow.Web.Services
{
     public class SignalRRealTimeNotifier : IRealTimeNotifier
    {
        private readonly IHubContext<SeatMapHub> _hubContext;

        public SignalRRealTimeNotifier(IHubContext<SeatMapHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifySeatUpdatesAsync(int showId, List<int> seatInstanceIds, string newState)
        {
            if (!seatInstanceIds.Any()) return;

            var updates = seatInstanceIds.Select(id => new { seatInstanceId = id, state = newState });
            var jsonUpdate = JsonSerializer.Serialize(updates);

            await _hubContext.Clients.Group(showId.ToString()).SendAsync("ReceiveSeatUpdate", jsonUpdate);
        }
    }
}