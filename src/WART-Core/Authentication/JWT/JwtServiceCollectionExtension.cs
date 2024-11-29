// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using WART_Core.Hubs;
using WART_Core.Services;

namespace WART_Core.Authentication.JWT
{
    public static class JwtServiceCollectionExtension
    {
        /// <summary>
        /// Adds JWT authentication middleware to the service collection.
        /// Configures the authentication parameters, SignalR settings, and response compression.
        /// </summary>
        /// <param name="services">The service collection to add the middleware to.</param>
        /// <param name="tokenKey">The secret key used to sign and validate the JWT tokens.</param>
        /// <returns>The updated service collection.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the token key is null or empty.</exception>
        public static IServiceCollection AddJwtMiddleware(this IServiceCollection services, string tokenKey)
        {
            // Validate that the token key is provided
            if (string.IsNullOrEmpty(tokenKey))
            {
                throw new ArgumentNullException("Invalid token key");
            }

            // Configure forwarded headers (to support proxy scenarios, e.g., when behind a load balancer)
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            // Add logging for debugging purposes
            services.AddLogging(configure => configure.AddConsole());

            // Create a symmetric security key from the provided token key
            var key = Encoding.UTF8.GetBytes(tokenKey);
            var securityKey = new SymmetricSecurityKey(key);

            // Configure authentication using JWT Bearer token
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.IncludeErrorDetails = true;
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        LifetimeValidator = (before, expires, token, parameters) => expires > DateTime.UtcNow,
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateActor = false,
                        ValidateLifetime = true,
                        IssuerSigningKey = securityKey
                    };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            // Register WART event queue as a singleton service.
            services.AddSingleton<WartEventQueueService>();

            // Register the WART event worker as a hosted service.
            services.AddHostedService<WartEventWorker<WartHubJwt>>();

            // Configure SignalR options, including error handling and timeouts
            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
                options.HandshakeTimeout = TimeSpan.FromSeconds(15);
                options.KeepAliveInterval = TimeSpan.FromSeconds(15);
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
            });

            // Configure response compression to support additional MIME types
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });

            return services;
        }
    }
}
