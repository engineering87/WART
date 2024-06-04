// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WART_Core.Authentication.JWT;
using WART_Core.Enum;

namespace WART_Core.Middleware
{
    public static class WartServiceCollectionExtension
    {
        /// <summary>
        /// Add WART dependency to IServiceCollection without authentication.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddWartMiddleware(this IServiceCollection services)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            services.AddLogging(configure => configure.AddConsole());

            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true; 
            });

            return services;
        }

        /// <summary>
        /// Add WART dependency to IServiceCollection specifying the SignalR Hub type.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="hubType"></param>
        /// <param name="tokenKey"></param>
        /// <returns></returns>
        public static IServiceCollection AddWartMiddleware(this IServiceCollection services, HubType hubType, string tokenKey = "")
        {
            if (hubType == HubType.NoAuthentication)
            {
                // use WART without authentication
                services.AddWartMiddleware();
            }
            else
            {
                // use WART with JWT authentication
                services.AddJwtMiddleware(tokenKey);
            }

            return services;
        }
    }
}
