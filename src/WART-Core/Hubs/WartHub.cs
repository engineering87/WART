// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace WART_Core.Hubs
{
    /// <summary>
    /// The WART SignalR hub.
    /// </summary>
    public class WartHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> _connections = new ConcurrentDictionary<string, string>();

        private readonly ILogger<WartHub> _logger;

        public WartHub(ILogger<WartHub> logger)
        {
            this._logger = logger;
        }

        public override Task OnConnectedAsync()
        {
            _connections.TryAdd(Context.ConnectionId, Context.User.Identity.Name);

            _logger?.LogInformation($"OnConnect {Context.ConnectionId}");

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _connections.TryRemove(Context.ConnectionId, out _);

            _logger?.LogInformation($"OnDisconnect {Context.ConnectionId}");

            return base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Broadcast the WartEvent to all clients.
        /// </summary>
        /// <param name="jsonWartEvent"></param>
        /// <returns></returns>
        public Task Send(string jsonWartEvent)
        {
            _logger?.LogInformation($"Send {jsonWartEvent}");

            return Clients.All.SendAsync("Send", jsonWartEvent);
        }

        /// <summary>
        /// Get the current number of active connection
        /// </summary>
        /// <returns></returns>
        public static int GetConnectionsCount()
        {
            return _connections.Count;
        }
    }
}
