﻿// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
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
                    // get the request objects from RouteData
                    var request = context.RouteData.Values[RouteDataKey];
                    var httpMethod = context.HttpContext?.Request.Method;
                    var httpPath = context.HttpContext?.Request.Path;
                    var remoteAddress = context.HttpContext?.Connection.RemoteIpAddress?.ToString();
                    // get the object response
                    var response = objectResult.Value;
                    // create the new WartEvent and broadcast to all clients
                    var wartEvent = new WartEvent(request, response, httpMethod, httpPath, remoteAddress);
                    await SendToHub(wartEvent);
                }
            }

            base.OnActionExecuted(context);
        }

        /// <summary>
        /// Send the current event to the SignalR hub.
        /// </summary>
        /// <param name="wartEvent">The current WartEvent</param>
        /// <returns></returns>
        private async Task SendToHub(WartEvent wartEvent)
        {
            try
            {
                await _hubContext?.Clients.All.SendAsync("Send", wartEvent.ToString());

                _logger?.LogInformation(message: nameof(WartEvent), wartEvent.ToString());
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error sending WartEvent to clients");
            }
        }
    }
}
