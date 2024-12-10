// (c) 2024 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace WART_Core.Hubs
{
    /// <summary>
    /// Base class for WART SignalR hubs. 
    /// Provides shared logic for managing connections, groups, and logging.
    /// </summary>
    public abstract class WartHubBase : Hub
    {
        /// <summary>
        /// Stores active connections with their respective identifiers.
        /// </summary>
        private static readonly ConcurrentDictionary<string, string> _connections = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Logger instance for logging hub activities.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="WartHubBase"/> class.
        /// </summary>
        /// <param name="logger">Logger instance for the hub.</param>
        protected WartHubBase(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Called when a new connection is established.
        /// Adds the connection to the dictionary and optionally assigns it to a group.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var wartGroup = httpContext.Request.Query["WartGroup"].ToString();
            var userName = Context.User?.Identity?.Name ?? "Anonymous";

            _connections.TryAdd(Context.ConnectionId, userName);

            if (!string.IsNullOrEmpty(wartGroup))
            {
                await AddToGroup(wartGroup);
            }

            _logger?.LogInformation($"OnConnect: ConnectionId={Context.ConnectionId}, User={userName}");

            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Called when a connection is disconnected.
        /// Removes the connection from the dictionary and logs the event.
        /// </summary>
        /// <param name="exception">The exception that occurred, if any.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
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
        /// Adds the current connection to a specified SignalR group.
        /// </summary>
        /// <param name="groupName">The name of the SignalR group to add the connection to.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
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
        /// Removes the current connection from a specified SignalR group.
        /// </summary>
        /// <param name="groupName">The name of the SignalR group to remove the connection from.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
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
        /// Gets the current number of active connections.
        /// </summary>
        /// <returns>The count of active connections.</returns>
        public static int GetConnectionsCount()
        {
            return _connections.Count;
        }

        /// <summary>
        /// Returns a value indicating whether there are connected clients.
        /// </summary>
        public static bool HasConnectedClients => !_connections.IsEmpty;
    }
}