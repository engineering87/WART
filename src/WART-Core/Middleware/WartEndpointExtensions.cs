// (c) 2024-2026 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WART_Core.Filters;

namespace WART_Core.Middleware
{
    /// <summary>
    /// Extension methods for adding WART support to Minimal API endpoints.
    /// </summary>
    public static class WartEndpointExtensions
    {
        /// <summary>
        /// Adds the WART endpoint filter to a Minimal API route, enabling 
        /// real-time SignalR event broadcasting for the endpoint.
        /// </summary>
        /// <param name="builder">The route handler builder.</param>
        /// <returns>The updated <see cref="RouteHandlerBuilder"/> for chaining.</returns>
        public static RouteHandlerBuilder UseWart(this RouteHandlerBuilder builder)
        {
            return builder.AddEndpointFilter<WartEndpointFilter>();
        }

        /// <summary>
        /// Adds the WART endpoint filter to all endpoints in a Minimal API route group,
        /// enabling real-time SignalR event broadcasting for every endpoint in the group.
        /// </summary>
        /// <param name="builder">The route group builder.</param>
        /// <returns>The updated <see cref="RouteGroupBuilder"/> for chaining.</returns>
        public static RouteGroupBuilder UseWart(this RouteGroupBuilder builder)
        {
            return builder.AddEndpointFilter<WartEndpointFilter>();
        }

        /// <summary>
        /// Excludes a Minimal API endpoint from WART SignalR event broadcasting.
        /// </summary>
        /// <param name="builder">The route handler builder.</param>
        /// <returns>The updated <see cref="RouteHandlerBuilder"/> for chaining.</returns>
        public static RouteHandlerBuilder ExcludeFromWart(this RouteHandlerBuilder builder)
        {
            return builder.WithMetadata(new ExcludeWartAttribute());
        }

        /// <summary>
        /// Directs WART SignalR events for this Minimal API endpoint to specific groups.
        /// </summary>
        /// <param name="builder">The route handler builder.</param>
        /// <param name="groupNames">The SignalR group names to target.</param>
        /// <returns>The updated <see cref="RouteHandlerBuilder"/> for chaining.</returns>
        public static RouteHandlerBuilder WartGroup(this RouteHandlerBuilder builder, params string[] groupNames)
        {
            return builder.WithMetadata(new GroupWartAttribute(groupNames));
        }
    }
}
