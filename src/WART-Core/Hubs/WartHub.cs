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
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var wartGroup = httpContext.Request.Query["WartGroup"].ToString();
            var userName = Context.User?.Identity?.Name ?? "Anonymous";

            _connections.TryAdd(Context.ConnectionId, Context.User.Identity.Name);

            if (!string.IsNullOrEmpty(wartGroup))
            {
                await AddToGroup(wartGroup);
            }

            _logger?.LogInformation($"OnConnect: ConnectionId={Context.ConnectionId}, User={userName}");

            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _connections.TryRemove(Context.ConnectionId, out _);

            if (exception != null)
            {
                _logger?.LogWarning(exception, $"OnDisconnect with error: ConnectionId={Context.ConnectionId}");
            }
            else
            {
                _logger?.LogInformation($"OnDisconnect: ConnectionId={Context.ConnectionId}");
            }

            return base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Broadcast the WartEvent to all clients.
        /// </summary>
        /// <param name="jsonWartEvent"></param>
        /// <returns></returns>
        //public Task Send(string jsonWartEvent)
        //{
        //    if (string.IsNullOrWhiteSpace(jsonWartEvent))
        //    {
        //        _logger?.LogWarning("Attempted to send empty or null WartEvent.");
        //        return Task.CompletedTask;
        //    }

        //    _logger?.LogInformation($"Send {jsonWartEvent}");

        //    return Clients.All.SendAsync("Send", jsonWartEvent);
        //}

        /// <summary>
        /// Adds a connection to a group.
        /// </summary>
        /// <param name="groupName">The group name to add the connection to.</param>
        /// <returns></returns>
        public async Task AddToGroup(string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                _logger?.LogWarning("Attempted to add to a null or empty group.");
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            
            _logger?.LogInformation($"Connection {Context.ConnectionId} added to group {groupName}");
        }

        /// <summary>
        /// Removes a connection from a group.
        /// </summary>
        /// <param name="groupName">The group name to remove the connection from.</param>
        /// <returns></returns>
        public async Task RemoveFromGroup(string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                _logger?.LogWarning("Attempted to remove from a null or empty group.");
                return;
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            
            _logger?.LogInformation($"Connection {Context.ConnectionId} removed from group {groupName}");
        }

        /// <summary>
        /// Send a message to a specific group.
        /// </summary>
        /// <param name="groupName">The group name to send the message to.</param>
        /// <param name="jsonWartEvent">The JSON formatted WartEvent</param>
        /// <returns></returns>
        //public Task SendToGroup(string groupName, string jsonWartEvent)
        //{
        //    if (string.IsNullOrWhiteSpace(groupName) || string.IsNullOrWhiteSpace(jsonWartEvent))
        //    {
        //        _logger?.LogWarning("Attempted to send to a null or empty group or send a null or empty WartEvent.");
        //        return Task.CompletedTask;
        //    }

        //    _logger?.LogInformation($"Sending WartEvent to group {groupName}: {jsonWartEvent}");
            
        //    return Clients.Group(groupName).SendAsync("Send", jsonWartEvent);
        //}

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
