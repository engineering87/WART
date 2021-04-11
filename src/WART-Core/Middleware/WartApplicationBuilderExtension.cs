// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.Builder;
using System;
using WART_Core.Authentication.JWT;
using WART_Core.Enum;
using WART_Core.Hubs;

namespace WART_Core.Middleware
{
    public static class WartApplicationBuilderExtension
    {
        private const string DefaultHubName = "warthub";

        /// <summary>
        /// Use WART dependency to IApplicationBuilder.
        /// The default SignalR hub name is warthub.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseWartMiddleware(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<WartHub>($"/{DefaultHubName}");
            });

            app.UseForwardedHeaders();

            return app;
        }

        /// <summary>
        /// Use WART dependency to IApplicationBuilder.
        /// The default SignalR hub name is warthub.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="hubType"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseWartMiddleware(this IApplicationBuilder app, HubType hubType)
        {
            if (hubType == HubType.NoAuthentication)
            {
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapHub<WartHub>($"/{DefaultHubName}");
                });
            }
            else
            {
                app.UseJwtMiddleware();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapHub<WartHubJwt>($"/{DefaultHubName}");
                });
            }

            app.UseForwardedHeaders();

            return app;
        }

        /// <summary>
        /// Use WART dependency to IApplicationBuilder.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="hubName"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseWartMiddleware(this IApplicationBuilder app, string hubName)
        {
            if (string.IsNullOrEmpty(hubName))
                throw new ArgumentNullException("Invalid hub name");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<WartHub>($"/{hubName.Trim()}");
            });

            app.UseForwardedHeaders();

            return app;
        }

        /// <summary>
        /// Use WART dependency to IApplicationBuilder.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="hubName">The SignalR hub name</param>
        /// <param name="hubType"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseWartMiddleware(this IApplicationBuilder app, string hubName, HubType hubType)
        {
            if (string.IsNullOrEmpty(hubName))
                throw new ArgumentNullException("Invalid hub name");

            if (hubType == HubType.NoAuthentication)
            {
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapHub<WartHub>($"/{hubName.Trim()}");
                });
            }
            else
            {
                app.UseJwtMiddleware();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapHub<WartHubJwt>($"/{hubName.Trim()}");
                });
            }

            app.UseForwardedHeaders();

            return app;
        }
    }
}
