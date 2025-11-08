using BookNow.Application.Interfaces;
using BookNow.Web.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace BookNow.Web.Services
{
    // This service lives in the Web/Infrastructure layer and depends on the Application interface.
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

            // This is where the dependency on SignalR is contained (Infrastructure)
            await _hubContext.Clients.Group(showId.ToString()).SendAsync("ReceiveSeatUpdate", jsonUpdate);
        }
    }
}