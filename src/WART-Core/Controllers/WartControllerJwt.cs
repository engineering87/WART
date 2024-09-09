// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using WART_Core.Entity;
using WART_Core.Hubs;
using System.Linq;
using WART_Core.Filters;
using System.Collections.Generic;

namespace WART_Core.Controllers
{
    /// <summary>
    /// The WART Controller
    /// </summary>
    public class WartControllerJwt : Controller
    {
        private readonly ILogger<WartControllerJwt> _logger;
        private readonly IHubContext<WartHubJwt> _hubContext;
        private const string RouteDataKey = "REQUEST";

        public WartControllerJwt(IHubContext<WartHubJwt> hubContext, ILogger<WartControllerJwt> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public WartControllerJwt(IHubContext<WartHubJwt> hubContext)
        {
            _hubContext = hubContext;
        }

        /// <summary>
        /// WART OnActionExecuting override
        /// </summary>
        /// <param name="context">ActionExecutedContext context</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // add the request objects to RouteData
            context?.RouteData.Values.Add(RouteDataKey, context.ActionArguments);

            base.OnActionExecuting(context);
        }

        /// <summary>
        /// WART OnActionExecuted override
        /// </summary>
        /// <param name="context">ActionExecutedContext context</param>
        public override async void OnActionExecuted(ActionExecutedContext context)
        {
            if (context?.Result is ObjectResult objectResult)
            {
                // check for wart exclusion
                var exclusion = context.Filters.Any(f => f.GetType().Name == nameof(ExcludeWartAttribute));
                if (!exclusion)
                {
                    // check for RouteData key existence
                    if (context.RouteData.Values.TryGetValue(RouteDataKey, out var request))
                    {
                        // get the request objects from RouteData
                        var httpMethod = context.HttpContext?.Request.Method;
                        var httpPath = context.HttpContext?.Request.Path;
                        var remoteAddress = context.HttpContext?.Connection.RemoteIpAddress?.ToString();
                        // get the object response
                        var response = objectResult.Value;
                        // create the new WartEvent and broadcast to all clients
                        var wartEvent = new WartEvent(request, response, httpMethod, httpPath, remoteAddress);
                        await SendToHub(wartEvent, [.. context.Filters]);
                    }
                }
            }

            base.OnActionExecuted(context);
        }

        /// <summary>
        /// Send the current event to the SignalR hub.
        /// </summary>
        /// <param name="wartEvent">The current WartEvent</param>
        /// <param name="filters">The current Filters</param>
        /// <returns></returns>
        private async Task SendToHub(WartEvent wartEvent, List<IFilterMetadata> filters)
        {
            try
            {
                // check for specific groups
                if (filters.Any(f => f.GetType().Name == nameof(GroupWartAttribute)))
                {
                    // get the list of filters of type WartGroupAttribute   
                    var wartGroup = filters.FirstOrDefault(f => f.GetType() == typeof(GroupWartAttribute)) as GroupWartAttribute;
                    var groups = wartGroup?.GroupNames;
                    foreach (var group in groups)
                    {
                        await _hubContext?.Clients
                            .Group(group)
                            .SendAsync("Send", wartEvent.ToString());
                    }
                }
                else
                {
                    // send to all clients
                    await _hubContext?.Clients.All
                        .SendAsync("Send", wartEvent.ToString());
                }

                _logger?.LogInformation(message: nameof(WartEvent), wartEvent.ToString());
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error sending WartEvent to clients");
            }
        }
    }
}
