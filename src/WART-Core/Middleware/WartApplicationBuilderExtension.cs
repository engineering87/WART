// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
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
            app.UseRouting();

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
        /// <param name="app">The IApplicationBuilder</param>
        /// <param name="hubType">The SignalR hub type</param>
        /// <returns></returns>
        public static IApplicationBuilder UseWartMiddleware(this IApplicationBuilder app, HubType hubType)
        {
            app.UseRouting();

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
        /// <param name="app">The IApplicationBuilder</param>
        /// <param name="hubName">The SignalR hub name</param>
        /// <returns></returns>
        public static IApplicationBuilder UseWartMiddleware(this IApplicationBuilder app, string hubName)
        {
            if (string.IsNullOrEmpty(hubName))
                throw new ArgumentNullException("Invalid hub name");

            app.UseRouting();

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
        /// <param name="app">The IApplicationBuilder</param>
        /// <param name="hubNameList">The SignalR hub name list</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IApplicationBuilder UseWartMiddleware(this IApplicationBuilder app, IEnumerable<string> hubNameList)
        {
            if (hubNameList == null)
                throw new ArgumentNullException("Invalid hub list");

            app.UseRouting();

            foreach (var hubName in hubNameList.Distinct())
            {
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapHub<WartHub>($"/{hubName.Trim()}");
                });
            }

            app.UseForwardedHeaders();

            return app;
        }

        /// <summary>
        /// Use WART dependency to IApplicationBuilder.
        /// </summary>
        /// <param name="app">The IApplicationBuilder</param>
        /// <param name="hubName">The SignalR hub name</param>
        /// <param name="hubType">The SignalR hub type</param>
        /// <returns></returns>
        public static IApplicationBuilder UseWartMiddleware(this IApplicationBuilder app, string hubName, HubType hubType)
        {
            if (string.IsNullOrEmpty(hubName))
                throw new ArgumentNullException("Invalid hub name");

            app.UseRouting();

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

        /// <summary>
        /// Use WART dependency to IApplicationBuilder.
        /// </summary>
        /// <param name="app">The IApplicationBuilder</param>
        /// <param name="hubNameList">The SignalR hub name list</param>
        /// <param name="hubType">The SignalR hub type</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IApplicationBuilder UseWartMiddleware(this IApplicationBuilder app, IEnumerable<string> hubNameList, HubType hubType)
        {
            if (hubNameList == null)
                throw new ArgumentNullException("Invalid hub list");

            app.UseRouting();

            foreach (var hubName in hubNameList.Distinct())
            {
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
            }

            app.UseForwardedHeaders();

            return app;
        }
    }
}
