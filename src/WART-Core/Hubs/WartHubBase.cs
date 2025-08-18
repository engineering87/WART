// (c) 2024 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using WART_Core.Utilities;

namespace WART_Core.Hubs
{
    /// <summary>
    /// Base class for WART SignalR hubs. 
    /// Provides shared logic for managing connections, groups, and logging.
    /// </summary>
    public abstract class WartHubBase : Hub
    {
        /// <summary>
        /// Stores active connections for each hub, with their respective identifiers.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, string>> _connectionsByHub
            = new();

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
            var wartGroup = httpContext?.Request?.Query["WartGroup"].ToString();
            var userName = Context.User?.Identity?.Name ?? Context.UserIdentifier ?? "Anonymous";

            var bucket = _connectionsByHub.GetOrAdd(GetType(), _ => new ConcurrentDictionary<string, string>());
            bucket[Context.ConnectionId] = userName;

            if (!string.IsNullOrEmpty(wartGroup))
            {
                await AddToGroup(wartGroup);
            }

            _logger?.LogInformation("OnConnected: ConnectionId={ConnectionId}, User={User}",
                Context.ConnectionId, LogSanitizer.Sanitize(userName));

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
            if (_connectionsByHub.TryGetValue(GetType(), out var dict))
            {
                dict.TryRemove(Context.ConnectionId, out _);
                if (dict.IsEmpty) _connectionsByHub.TryRemove(GetType(), out _);
            }

            if (exception != null)
            {
                _logger?.LogWarning(exception, "OnDisconnect with error: ConnectionId={ConnectionId}", Context.ConnectionId);
            }
            else
            {
                _logger?.LogInformation("OnDisconnect: ConnectionId={ConnectionId}", Context.ConnectionId);
            }

            return base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Adds the current connection to a specified SignalR group.
        /// </summary>
        /// <param name="groupName">The name of the SignalR group to add the connection to.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected async Task AddToGroup(string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                _logger?.LogWarning("Attempted to add to a null or empty group.");
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            _logger?.LogInformation("Connection {ConnectionId} added to group {GroupName}",
                Context.ConnectionId, LogSanitizer.Sanitize(groupName));
        }

        /// <summary>
        /// Removes the current connection from a specified SignalR group.
        /// </summary>
        /// <param name="groupName">The name of the SignalR group to remove the connection from.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected async Task RemoveFromGroup(string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                _logger?.LogWarning("Attempted to remove from a null or empty group.");
                return;
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            _logger?.LogInformation("Connection {ConnectionId} removed from group {GroupName}",
                Context.ConnectionId, LogSanitizer.Sanitize(groupName));
        }

        public static int GetConnectionsCount()
            => _connectionsByHub.Values.Sum(d => d.Count);

        public static bool HasConnectedClients
            => _connectionsByHub.Values.Any(d => !d.IsEmpty);

        public static int GetConnectionsCountFor<THub>() where THub : Hub
            => _connectionsByHub.TryGetValue(typeof(THub), out var d) ? d.Count : 0;

        public static bool HasConnectedClientsFor<THub>() where THub : Hub
            => GetConnectionsCountFor<THub>() > 0;
    }
}