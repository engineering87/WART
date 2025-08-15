// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using WART_Core.Authentication.Cookie;
using WART_Core.Authentication.JWT;
using WART_Core.Enum;
using WART_Core.Hubs;

namespace WART_Core.Middleware
{
    public static class WartApplicationBuilderExtension
    {
        private const string DefaultHubName = "warthub";

        private static string NormalizeHubPath(string name)
            => "/" + (name ?? string.Empty).Trim().Trim('/');

        /// <summary>
        /// Configures and adds the WART middleware to the IApplicationBuilder.
        /// This method sets up the default SignalR hub (warthub) without authentication.
        /// </summary>
        /// <param name="app">The IApplicationBuilder to configure the middleware pipeline.</param>
        /// <returns>The updated IApplicationBuilder to continue configuration.</returns>
        public static IApplicationBuilder UseWartMiddleware(this IApplicationBuilder app)
        {
            app.UseForwardedHeaders();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<WartHub>(NormalizeHubPath(DefaultHubName));
            });

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
            app.UseForwardedHeaders();
            app.UseRouting();

            switch(hubType)
            {
                default:
                case HubType.NoAuthentication:
                    {
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers();
                            endpoints.MapHub<WartHub>(NormalizeHubPath(DefaultHubName));
                        });
                        break;
                    }
                case HubType.JwtAuthentication:
                    {
                        app.UseJwtMiddleware();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers();
                            endpoints.MapHub<WartHubJwt>(NormalizeHubPath(DefaultHubName));
                        });
                        break;
                    }
                case HubType.CookieAuthentication:
                    {
                        app.UseCookieMiddleware();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers();
                            endpoints.MapHub<WartHubCookie>(NormalizeHubPath(DefaultHubName));
                        });
                        break;
                    }
            }

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
                throw new ArgumentException("Invalid hub name");

            app.UseForwardedHeaders();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<WartHub>(NormalizeHubPath(hubName));
            });

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

            app.UseForwardedHeaders();
            app.UseRouting();

            var unique = hubNameList
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(NormalizeHubPath)
                .Distinct()
                .ToList();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                foreach (var path in unique)
                    endpoints.MapHub<WartHub>(path);
            });

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
                throw new ArgumentException("Invalid hub name");

            app.UseForwardedHeaders();
            app.UseRouting();

            switch (hubType)
            {
                default:
                case HubType.NoAuthentication:
                    {
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers();
                            endpoints.MapHub<WartHub>(NormalizeHubPath(hubName));
                        });
                        break;
                    }
                case HubType.JwtAuthentication:
                    {
                        app.UseJwtMiddleware();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers();
                            endpoints.MapHub<WartHubJwt>(NormalizeHubPath(hubName));
                        });
                        break;
                    }
                case HubType.CookieAuthentication:
                    {
                        app.UseCookieMiddleware();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers();
                            endpoints.MapHub<WartHubCookie>(NormalizeHubPath(hubName));
                        });
                        break;
                    }
            }

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

            app.UseForwardedHeaders();
            app.UseRouting();

            foreach (var hubName in hubNameList.Distinct())
            {
                switch (hubType)
                {
                    default:
                    case HubType.NoAuthentication:
                        {
                            app.UseEndpoints(endpoints =>
                            {
                                endpoints.MapControllers();
                                endpoints.MapHub<WartHub>(NormalizeHubPath(hubName));
                            });
                            break;
                        }
                    case HubType.JwtAuthentication:
                        {
                            app.UseJwtMiddleware();
                            app.UseEndpoints(endpoints =>
                            {
                                endpoints.MapControllers();
                                endpoints.MapHub<WartHubJwt>(NormalizeHubPath(hubName));
                            });
                            break;
                        }
                    case HubType.CookieAuthentication:
                        {
                            app.UseCookieMiddleware();
                            app.UseEndpoints(endpoints =>
                            {
                                endpoints.MapControllers();
                                endpoints.MapHub<WartHubCookie>(NormalizeHubPath(hubName));
                            });
                            break;
                        }
                }
            }

            return app;
        }
    }
}
