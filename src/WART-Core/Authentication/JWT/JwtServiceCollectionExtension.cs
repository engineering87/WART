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

namespace WART_Core.Authentication.JWT
{
    public static class JwtServiceCollectionExtension
    {
        /// <summary>
        /// Add JWT authentication dependency.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="tokenKey"></param>
        /// <returns></returns>
        public static IServiceCollection AddJwtMiddleware(this IServiceCollection services, string tokenKey)
        {
            if (string.IsNullOrEmpty(tokenKey))
            {
                throw new ArgumentNullException("Invalid token key");
            }

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            services.AddLogging(configure => configure.AddConsole());

            var key = Encoding.UTF8.GetBytes(tokenKey);
            var securityKey = new SymmetricSecurityKey(key);

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

            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
                options.HandshakeTimeout = TimeSpan.FromSeconds(15);
                options.KeepAliveInterval = TimeSpan.FromSeconds(15);
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
            });

            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });

            return services;
        }
    }
}
