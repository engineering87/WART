// (c) 2024 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WART_Core.Entity;
using WART_Core.Filters;
using WART_Core.Hubs;

namespace WART_Core.Services
{
    /// <summary>
    /// Background service that processes WartEvent objects from a queue and sends them to SignalR clients.
    /// </summary>
    public class WartEventWorker<THub> : BackgroundService where THub : Hub
    {
        private readonly WartEventQueueService _eventQueue;
        private readonly IHubContext<THub> _hubContext;
        private readonly ILogger<WartEventWorker<THub>> _logger;

        /// <summary>
        /// Constructor that initializes the worker with the event queue, hub context, and logger.
        /// </summary>
        public WartEventWorker(WartEventQueueService eventQueue, IHubContext<THub> hubContext, ILogger<WartEventWorker<THub>> logger)
        {
            _eventQueue = eventQueue;
            _hubContext = hubContext;
            _logger = logger;
        }

        /// <summary>
        /// Method that runs in the background to dequeue events and send them to SignalR clients.
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("WartEventWorker started.");

            // The worker will keep running as long as the cancellation token is not triggered.
            while (!stoppingToken.IsCancellationRequested)
            {
                // Check if there are any connected clients.
                if (!WartHubBase.HasConnectedClients)
                {
                    _logger.LogInformation("No clients connected. Pausing event processing.");
                    await Task.Delay(500, stoppingToken);
                    continue;
                }

                // Dequeue events and process them.
                while (_eventQueue.TryDequeue(out var wartEventWithFilters))
                {
                    try
                    {
                        // Extract the event and filters.
                        var wartEvent = wartEventWithFilters.WartEvent;
                        var filters = wartEventWithFilters.Filters;

                        // Send the event to the SignalR hub.
                        await SendToHub(wartEvent, filters);

                        _logger.LogInformation("Event sent: {Event}", wartEvent);
                    }
                    catch (Exception ex)
                    {
                        // Log any errors that occur while sending the event.
                        _logger.LogError(ex, "Error while sending event.");

                        // Re-enqueue the event for retry
                        // We lost the order of the events, but we can't lose the events
                        _eventQueue.Enqueue(wartEventWithFilters);
                    }
                }

                // Wait for 200 ms before checking for new events in the queue.
                await Task.Delay(200, stoppingToken);
            }

            _logger.LogInformation("WartEventWorker stopped.");
        }

        /// <summary>
        /// Sends the current event to the SignalR hub.
        /// This method determines if the event should be sent to specific groups or all clients.
        /// </summary>
        private async Task SendToHub(WartEvent wartEvent, List<IFilterMetadata> filters)
        {
            try
            {
                // Retrieve the target groups based on the filters.
                var groups = GetTargetGroups(filters);

                // If specific groups are defined, send the event to each group in parallel.
                if (groups.Any())
                {
                    var tasks = groups.Select(group => SendEventToGroup(wartEvent, group));
                    await Task.WhenAll(tasks);
                }
                else
                {
                    // If no groups are defined, send the event to all clients.
                    await SendEventToAllClients(wartEvent);
                }
            }
            catch (Exception ex)
            {
                // Log errors that occur while sending events to SignalR clients.
                _logger?.LogError(ex, "Error sending WartEvent to clients");

                throw;
            }
        }

        /// <summary>
        /// Retrieves the list of groups that the WartEvent should be sent to, based on the provided filters.
        /// </summary>
        /// <param name="filters">The list of filters that may contain group-related information.</param>
        /// <returns>A list of group names to send the WartEvent to.</returns>
        private List<string> GetTargetGroups(List<IFilterMetadata> filters)
        {
            var groups = new List<string>();

            // Check if there is a GroupWartAttribute filter indicating the groups.
            if (filters.Any(f => f.GetType().Name == nameof(GroupWartAttribute)))
            {
                var wartGroup = filters.FirstOrDefault(f => f.GetType() == typeof(GroupWartAttribute)) as GroupWartAttribute;
                if (wartGroup != null)
                {
                    groups.AddRange(wartGroup.GroupNames);
                }
            }

            return groups;
        }

        /// <summary>
        /// Sends the WartEvent to a specific group of clients.
        /// </summary>
        private async Task SendEventToGroup(WartEvent wartEvent, string group)
        {
            // Send the event to the group using SignalR.
            await _hubContext?.Clients
                .Group(group)
                .SendAsync("Send", wartEvent.ToString());

            // Log the event sent to the group.
            _logger?.LogInformation($"Group: {group}, WartEvent: {wartEvent}");
        }

        /// <summary>
        /// Sends the WartEvent to all connected clients.
        /// </summary>
        private async Task SendEventToAllClients(WartEvent wartEvent)
        {
            // Send the event to all clients using SignalR.
            await _hubContext?.Clients.All
                .SendAsync("Send", wartEvent.ToString());

            // Log the event sent to all clients.
            _logger?.LogInformation("Event: {EventName}, Details: {EventDetails}", nameof(WartEvent), wartEvent.ToString());
        }
    }
}