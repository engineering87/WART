// (c) 2025 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WART_Core.Hubs;
using WART_Core.Services;

namespace WART_Core.Authentication.Cookie
{
    public static class CookieServiceCollectionExtension
    {
        /// <summary>
        /// Adds Cookie authentication middleware to the service collection.
        /// Configures the authentication parameters, SignalR settings, and response compression.
        /// </summary>
        /// <param name="services">The service collection to add the middleware to.</param>
        /// <param name="loginPath">Optional path for the login redirect (default: /Account/Login).</param>
        /// <param name="accessDeniedPath">Optional path for access denied redirect (default: /Account/Denied).</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddCookieMiddleware(
            this IServiceCollection services,
            string loginPath = "/Account/Login",
            string accessDeniedPath = "/Account/AccessDenied")
        {
            // Configure forwarded headers (support for reverse proxy)
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            // Add logging support
            services.AddLogging(configure => configure.AddConsole());

            // Add Data Protection with key persistence
            var keysPath = Path.Combine(AppContext.BaseDirectory, "keys");
            Directory.CreateDirectory(keysPath);
            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
                .SetApplicationName("WART_App");

            // Configure cookie-based authentication
            services.AddAuthentication("WartCookieAuth")
                .AddCookie("WartCookieAuth", options =>
                {
                    options.LoginPath = loginPath;
                    options.AccessDeniedPath = accessDeniedPath;
                    options.Cookie.Name = "WART.AuthCookie";
                    options.ExpireTimeSpan = TimeSpan.FromHours(1);
                    options.SlidingExpiration = true;
                });

            // Register WART event queue service
            services.AddSingleton<WartEventQueueService>();

            // Register the WART event worker for the cookie-authenticated hub
            services.AddHostedService<WartEventWorker<WartHubCookie>>();

            // SignalR configuration
            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
                options.HandshakeTimeout = TimeSpan.FromSeconds(15);
                options.KeepAliveInterval = TimeSpan.FromSeconds(15);
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
            });

            // Compression for SignalR WebSocket/Binary transport
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });

            return services;
        }
    }
}