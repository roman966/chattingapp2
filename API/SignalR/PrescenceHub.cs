using System;
using System.Threading.Tasks;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    [Authorize]
    public class PrescenceHub : Hub
    {
        private readonly PrescenceTracker _tracker;
        public PrescenceHub(PrescenceTracker tracker)
        {
            _tracker = tracker;
        }

        public override async Task OnConnectedAsync()
        {
            await _tracker.UserConnected(Context.User.GetUseremail(),Context.ConnectionId);
            await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUseremail());

            var currentUsers = await _tracker.GetOnlineUsers();

            await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {   
            await _tracker.UserDisonnected(Context.User.GetUseremail(), Context.ConnectionId);
            await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUseremail());

            await base.OnDisconnectedAsync(exception);
        }

    }
}