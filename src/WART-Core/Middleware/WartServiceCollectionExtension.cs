// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using WART_Core.Authentication.JWT;
using WART_Core.Enum;

namespace WART_Core.Middleware
{
    public static class WartServiceCollectionExtension
    {
        /// <summary>
        /// Registers WART middleware dependencies to the IServiceCollection without authentication.
        /// This method configures forwarded headers, logging, SignalR, and response compression.
        /// </summary>
        /// <param name="services">The IServiceCollection to configure.</param>
        /// <returns>The updated IServiceCollection with WART middleware dependencies.</returns>
        public static IServiceCollection AddWartMiddleware(this IServiceCollection services)
        {
            // Configure forwarded headers to support proxy scenarios (X-Forwarded-* headers).
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            // Add console logging.
            services.AddLogging(configure => configure.AddConsole());

            // Configure SignalR with custom options.
            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
                options.HandshakeTimeout = TimeSpan.FromSeconds(15);
                options.KeepAliveInterval = TimeSpan.FromSeconds(15);
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
            });

            // Configure response compression for specific MIME types.
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });

            return services;
        }

        /// <summary>
        /// Registers WART middleware dependencies to the IServiceCollection, specifying the SignalR Hub type 
        /// and optionally configuring JWT authentication with the provided token key.
        /// </summary>
        /// <param name="services">The IServiceCollection to configure.</param>
        /// <param name="hubType">The type of SignalR Hub to use (with or without authentication).</param>
        /// <param name="tokenKey">The JWT token key, if authentication is enabled.</param>
        /// <returns>The updated IServiceCollection with WART middleware dependencies and optional JWT authentication.</returns>
        public static IServiceCollection AddWartMiddleware(this IServiceCollection services, HubType hubType, string tokenKey = "")
        {
            // Check the hub type to determine if authentication is required.
            if (hubType == HubType.NoAuthentication)
            {
                // If no authentication is required, configure WART middleware without authentication.
                services.AddWartMiddleware();
            }
            else
            {
                // If authentication is required, configure JWT middleware for authentication.
                services.AddJwtMiddleware(tokenKey);
            }

            return services;
        }
    }
}
