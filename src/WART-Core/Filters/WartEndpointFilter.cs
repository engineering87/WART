// (c) 2024-2026 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WART_Core.Entity;
using WART_Core.Services;

namespace WART_Core.Filters
{
    /// <summary>
    /// An <see cref="IEndpointFilter"/> that captures Minimal API request/response data
    /// and enqueues a <see cref="WartEvent"/> for SignalR broadcast.
    /// Respects <see cref="ExcludeWartAttribute"/> and <see cref="GroupWartAttribute"/> 
    /// when applied as endpoint metadata.
    /// </summary>
    public sealed class WartEndpointFilter : IEndpointFilter
    {
        public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var endpoint = context.HttpContext.GetEndpoint();
            var metadata = endpoint?.Metadata;

            // If the endpoint is decorated with ExcludeWartAttribute, skip processing.
            if (metadata?.GetMetadata<ExcludeWartAttribute>() is not null)
            {
                return await next(context);
            }

            // Capture request arguments as a dictionary.
            var requestArgs = new Dictionary<string, object>();
            for (int i = 0; i < context.Arguments.Count; i++)
            {
                requestArgs[$"arg{i}"] = context.Arguments[i];
            }

            // Invoke the next filter/handler.
            var result = await next(context);

            // Build the WartEvent from request/response data.
            var httpContext = context.HttpContext;
            var wartEvent = new WartEvent(
                request: requestArgs,
                response: result,
                httpMethod: httpContext.Request.Method,
                httpPath: httpContext.Request.Path,
                remoteAddress: httpContext.Connection.RemoteIpAddress?.ToString()
            );

            // Collect IFilterMetadata from endpoint metadata for group routing support.
            var filters = metadata?
                .OfType<IFilterMetadata>()
                .ToList() ?? [];

            var queue = httpContext.RequestServices.GetService(typeof(WartEventQueueService)) as WartEventQueueService;
            queue?.Enqueue(new WartEventWithFilters(wartEvent, filters));

            return result;
        }
    }
}
