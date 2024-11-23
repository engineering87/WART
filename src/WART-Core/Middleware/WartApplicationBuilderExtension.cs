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
        /// Configures and adds the WART middleware to the IApplicationBuilder.
        /// This method sets up the default SignalR hub (warthub) without authentication.
        /// </summary>
        /// <param name="app">The IApplicationBuilder to configure the middleware pipeline.</param>
        /// <returns>The updated IApplicationBuilder to continue configuration.</returns>
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
        /// Configures and adds the WART middleware to the IApplicationBuilder
        /// with a specified SignalR hub type. If the hub type requires authentication,
        /// JWT middleware will be included for secure access to the hub.
        /// </summary>
        /// <param name="app">The IApplicationBuilder to configure the middleware pipeline.</param>
        /// <param name="hubType">The type of SignalR hub to configure, determining if authentication is required.</param>
        /// <returns>The updated IApplicationBuilder to continue configuration.</returns>
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
        /// Configures and adds the WART middleware to the IApplicationBuilder
        /// with a custom SignalR hub name.
        /// </summary>
        /// <param name="app">The IApplicationBuilder to configure the middleware pipeline.</param>
        /// <param name="hubName">The custom SignalR hub name (URL path).</param>
        /// <returns>The updated IApplicationBuilder to continue configuration.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the hub name is null or empty.</exception>
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
        /// Configures and adds the WART middleware to the IApplicationBuilder
        /// with a list of SignalR hub names. This allows configuring multiple hubs
        /// at once by passing a list of custom names.
        /// </summary>
        /// <param name="app">The IApplicationBuilder to configure the middleware pipeline.</param>
        /// <param name="hubNameList">The list of custom SignalR hub names (URL paths).</param>
        /// <returns>The updated IApplicationBuilder to continue configuration.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the hub name list is null.</exception>
        public static IApplicationBuilder UseWartMiddleware(this IApplicationBuilder app, IEnumerable<string> hubNameList)
        {
            ArgumentNullException.ThrowIfNull(hubNameList);

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
        /// Configures and adds the WART middleware to the IApplicationBuilder
        /// with a custom SignalR hub name and hub type (with or without authentication).
        /// </summary>
        /// <param name="app">The IApplicationBuilder to configure the middleware pipeline.</param>
        /// <param name="hubName">The custom SignalR hub name (URL path).</param>
        /// <param name="hubType">The type of SignalR hub to configure, determining if authentication is required.</param>
        /// <returns>The updated IApplicationBuilder to continue configuration.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the hub name is null or empty.</exception>
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
        /// Configures and adds the WART middleware to the IApplicationBuilder
        /// with a list of custom SignalR hub names and a specified hub type
        /// (with or without authentication). This allows for multiple hubs
        /// with different authentication requirements to be configured.
        /// </summary>
        /// <param name="app">The IApplicationBuilder to configure the middleware pipeline.</param>
        /// <param name="hubNameList">The list of custom SignalR hub names (URL paths).</param>
        /// <param name="hubType">The type of SignalR hub to configure, determining if authentication is required.</param>
        /// <returns>The updated IApplicationBuilder to continue configuration.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the hub name list is null.</exception>
        public static IApplicationBuilder UseWartMiddleware(this IApplicationBuilder app, IEnumerable<string> hubNameList, HubType hubType)
        {
            ArgumentNullException.ThrowIfNull(hubNameList);

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
