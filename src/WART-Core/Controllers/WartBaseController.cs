// (c) 2024 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Linq;
using WART_Core.Entity;
using WART_Core.Filters;
using Microsoft.Extensions.DependencyInjection;
using WART_Core.Services;

namespace WART_Core.Controllers
{
    public abstract class WartBaseController<THub> : Controller where THub : Hub
    {
        private readonly ILogger _logger;
        private readonly IHubContext<THub> _hubContext;
        private const string RouteDataKey = "REQUEST";

        private WartEventQueueService _eventQueue;

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
        public override void OnActionExecuted(ActionExecutedContext context)
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

                    _eventQueue = context.HttpContext?.RequestServices.GetService<WartEventQueueService>();
                    _eventQueue?.Enqueue(new WartEventWithFilters(wartEvent, [.. context.Filters]));
                }
            }

            base.OnActionExecuted(context);
        }
    }
}
