// (c) 2024 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WART_Core.Entity;
using WART_Core.Filters;
using WART_Core.Services;

namespace WART_Core.Controllers
{
    public abstract class WartBaseController<THub> : Controller where THub : Hub
    {
        private readonly ILogger _logger;
        private readonly IHubContext<THub> _hubContext;

        // Strongly-typed, collision-free key for HttpContext.Items
        private sealed class RequestSnapshotKey { }
        private static readonly object ItemsRequestKey = new RequestSnapshotKey();

        protected WartBaseController(IHubContext<THub> hubContext, ILogger logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        /// <summary>
        /// Stores a snapshot of action arguments in <see cref="HttpContext.Items"/> 
        /// under a collision-free typed key.
        /// </summary>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context is not null)
            {
                var snapshot = new ReadOnlyDictionary<string, object>(
                    new Dictionary<string, object>(context.ActionArguments)
                );
                context.HttpContext.Items[ItemsRequestKey] = snapshot;
            }

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
                // Opt-out if ExcludeWartAttribute is present
                var excluded = context.Filters.OfType<ExcludeWartAttribute>().Any();

                if (!excluded && context.HttpContext.Items.TryGetValue(ItemsRequestKey, out var snapshotObj))
                {
                    var http = context.HttpContext;

                    var wartEvent = new WartEvent(
                        request: snapshotObj,
                        response: objectResult.Value,
                        httpMethod: http?.Request.Method,
                        httpPath: http?.Request.Path,
                        remoteAddress: http?.Connection.RemoteIpAddress?.ToString()
                    );

                    var queue = context.HttpContext?.RequestServices.GetService<WartEventQueueService>();
                    queue?.Enqueue(new WartEventWithFilters(wartEvent, [.. context.Filters]));
                }
            }

            base.OnActionExecuted(context);
        }
    }
}