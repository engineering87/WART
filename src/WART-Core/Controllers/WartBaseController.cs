// (c) 2024 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WART_Core.Entity;
using WART_Core.Filters;

namespace WART_Core.Controllers
{
    public abstract class WartBaseController<THub> : Controller where THub : Hub
    {
        private readonly ILogger _logger;
        private readonly IHubContext<THub> _hubContext;
        private const string RouteDataKey = "REQUEST";

        protected WartBaseController(IHubContext<THub> hubContext, ILogger logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        /// <summary>
        /// Adds the request objects to RouteData.
        /// </summary>
        /// <param name="context">The action executing context.</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context?.RouteData.Values.Add(RouteDataKey, context.ActionArguments);
            base.OnActionExecuting(context);
        }

        /// <summary>
        /// Processes the executed action and sends the event to the SignalR hub if applicable.
        /// </summary>
        /// <param name="context">The action executed context.</param>
        public override async void OnActionExecuted(ActionExecutedContext context)
        {
            if (context?.Result is ObjectResult objectResult)
            {
                var exclusion = context.Filters.Any(f => f.GetType().Name == nameof(ExcludeWartAttribute));
                if (!exclusion && context.RouteData.Values.TryGetValue(RouteDataKey, out var request))
                {
                    var httpMethod = context.HttpContext?.Request.Method;
                    var httpPath = context.HttpContext?.Request.Path;
                    var remoteAddress = context.HttpContext?.Connection.RemoteIpAddress?.ToString();
                    var response = objectResult.Value;

                    var wartEvent = new WartEvent(request, response, httpMethod, httpPath, remoteAddress);
                    await SendToHub(wartEvent, [.. context.Filters]);
                }
            }

            base.OnActionExecuted(context);
        }

        /// <summary>
        /// Sends the current event to the SignalR hub.
        /// </summary>
        /// <param name="wartEvent">The current WartEvent.</param>
        /// <param name="filters">The list of filters applied to the request.</param>
        private async Task SendToHub(WartEvent wartEvent, List<IFilterMetadata> filters)
        {
            try
            {
                // Retrieve the target groups based on the filters.
                var groups = GetTargetGroups(filters);

                // If there are groups specified, send the event to each group in parallel.
                if (groups.Any())
                {
                    // Sending events to multiple groups in parallel.
                    var tasks = groups.Select(group => SendEventToGroup(wartEvent, group));
                    await Task.WhenAll(tasks);
                }
                else
                {
                    // If no specific groups are defined, send the event to all connected clients.
                    await SendEventToAllClients(wartEvent);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error sending WartEvent to clients");
            }
        }

        /// <summary>
        /// Retrieves the list of groups that the WartEvent should be sent to, based on the provided filters.
        /// </summary>
        /// <param name="filters">The list of filters that may contain group-related information.</param>
        /// <returns>A list of group names to send the WartEvent to.</returns>
        private IEnumerable<string> GetTargetGroups(List<IFilterMetadata> filters)
        {
            var groups = new List<string>();

            // Check if there is a GroupWartAttribute filter indicating groups.
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
        /// <param name="wartEvent">The WartEvent to send to the group.</param>
        /// <param name="group">The group name to which the event will be sent.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task SendEventToGroup(WartEvent wartEvent, string group)
        {
            try
            {
                await _hubContext?.Clients
                    .Group(group)
                    .SendAsync("Send", wartEvent.ToString());

                _logger?.LogInformation($"Group: {group}, WartEvent: {wartEvent}");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error sending WartEvent to group {group}");
            }
        }

        /// <summary>
        /// Sends the WartEvent to all connected clients.
        /// </summary>
        /// <param name="wartEvent">The WartEvent to send to all clients.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task SendEventToAllClients(WartEvent wartEvent)
        {
            try
            {
                // Send the WartEvent to all connected clients using SignalR.
                await _hubContext?.Clients.All
                    .SendAsync("Send", wartEvent.ToString());

                _logger?.LogInformation("Event: {EventName}, Details: {EventDetails}", nameof(WartEvent), wartEvent.ToString());
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error sending WartEvent to all clients");
            }
        }
    }
}
