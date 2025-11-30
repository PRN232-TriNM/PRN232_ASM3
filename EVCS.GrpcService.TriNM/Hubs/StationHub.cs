using Microsoft.AspNetCore.SignalR;

namespace EVCS.GrpcService.TriNM.Hubs
{
    public class StationHub : Hub
    {
        public async Task JoinStationGroup(string stationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"station_{stationId}");
        }

        public async Task LeaveStationGroup(string stationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"station_{stationId}");
        }
    }
}

