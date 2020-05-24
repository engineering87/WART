// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace WART_Core.Hubs
{
    /// <summary>
    /// The WART SignalR hub
    /// </summary>
    public class WartHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Broadcast the WartEvent to all clients
        /// </summary>
        /// <param name="jsonWartEvent"></param>
        /// <returns></returns>
        public Task Send(string jsonWartEvent)
        {
            return Clients?.All.SendAsync("Send", jsonWartEvent);
        }
    }
}
