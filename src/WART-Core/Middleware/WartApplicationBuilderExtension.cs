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

        private static IReadOnlyList<string> GetDistinctPaths(IEnumerable<string> hubNameList)
            => (hubNameList ?? [])
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(NormalizeHubPath)
                .Distinct(StringComparer.Ordinal)
                .ToList();

        /// <summary>
        /// Configures and adds the WART middleware to the IApplicationBuilder.
        /// This method sets up the default SignalR hub (warthub) without authentication.
        /// </summary>
        /// <param name="app">The IApplicationBuilder to configure the middleware pipeline.</param>
        /// <returns>The updated IApplicationBuilder to continue configuration.</returns>
        /// <remarks>
        /// <b>SECURITY WARNING:</b> This overload maps an unauthenticated SignalR hub that broadcasts
        /// live API request and response payloads to every connected client with no credential check.
        /// Use <see cref="UseWartMiddleware(IApplicationBuilder, HubType)"/> with
        /// <c>HubType.JwtAuthentication</c> or <c>HubType.CookieAuthentication</c> instead.
        /// See https://github.com/engineering87/WART/security/advisories
        /// </remarks>
        [Obsolete("UseWartMiddleware() without authentication broadcasts all API events to any anonymous client and is insecure. " +
                  "Use UseWartMiddleware(HubType.JwtAuthentication) or UseWartMiddleware(HubType.CookieAuthentication) instead. " +
                  "See https://github.com/engineering87/WART/security/advisories")]
        public static IApplicationBuilder UseWartMiddleware(this IApplicationBuilder app)
        {
            app.UseForwardedHeaders();
            app.UseResponseCompression();
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
            app.UseResponseCompression();
            app.UseRouting();

            switch(hubType)
            {
                default:
#pragma warning disable CS0618 // Internal routing — intentional fallback to no-auth mode
                case HubType.NoAuthentication:
                    {
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers();
                            endpoints.MapHub<WartHub>(NormalizeHubPath(DefaultHubName));
                        });
                        break;
                    }
#pragma warning restore CS0618
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
        /// <exception cref="ArgumentException">Thrown when the hub name is null or empty.</exception>
        /// <remarks>
        /// <b>SECURITY WARNING:</b> This overload maps an unauthenticated SignalR hub that broadcasts
        /// live API request and response payloads to every connected client with no credential check.
        /// Use <see cref="UseWartMiddleware(IApplicationBuilder, string, HubType)"/> with
        /// <c>HubType.JwtAuthentication</c> or <c>HubType.CookieAuthentication</c> instead.
        /// See https://github.com/engineering87/WART/security/advisories
        /// </remarks>
        [Obsolete("UseWartMiddleware(string) without authentication broadcasts all API events to any anonymous client and is insecure. " +
                  "Use UseWartMiddleware(hubName, HubType.JwtAuthentication) or UseWartMiddleware(hubName, HubType.CookieAuthentication) instead. " +
                  "See https://github.com/engineering87/WART/security/advisories")]
        public static IApplicationBuilder UseWartMiddleware(this IApplicationBuilder app, string hubName)
        {
            if (string.IsNullOrWhiteSpace(hubName))
                throw new ArgumentException("Invalid hub name");

            app.UseForwardedHeaders();
            app.UseResponseCompression();
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
        /// <exception cref="ArgumentException">Thrown when the hub name list is null.</exception>
        /// <remarks>
        /// <b>SECURITY WARNING:</b> This overload maps unauthenticated SignalR hubs that broadcast
        /// live API request and response payloads to every connected client with no credential check.
        /// Use <see cref="UseWartMiddleware(IApplicationBuilder, IEnumerable{string}, HubType)"/> with
        /// <c>HubType.JwtAuthentication</c> or <c>HubType.CookieAuthentication</c> instead.
        /// See https://github.com/engineering87/WART/security/advisories
        /// </remarks>
        [Obsolete("UseWartMiddleware(IEnumerable<string>) without authentication broadcasts all API events to any anonymous client and is insecure. " +
                  "Use UseWartMiddleware(hubNameList, HubType.JwtAuthentication) or UseWartMiddleware(hubNameList, HubType.CookieAuthentication) instead. " +
                  "See https://github.com/engineering87/WART/security/advisories")]
        public static IApplicationBuilder UseWartMiddleware(this IApplicationBuilder app, IEnumerable<string> hubNameList)
        {
            ArgumentNullException.ThrowIfNull(hubNameList);

            app.UseForwardedHeaders();
            app.UseResponseCompression();
            app.UseRouting();

            var unique = hubNameList
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(NormalizeHubPath)
                .Distinct(StringComparer.Ordinal)
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
        /// <exception cref="ArgumentException">Thrown when the hub name is null or empty.</exception>
        public static IApplicationBuilder UseWartMiddleware(this IApplicationBuilder app, string hubName, HubType hubType)
        {
            if (string.IsNullOrWhiteSpace(hubName))
                throw new ArgumentException("Invalid hub name");

            app.UseForwardedHeaders();
            app.UseResponseCompression();
            app.UseRouting();

            switch (hubType)
            {
                default:
#pragma warning disable CS0618 // Internal routing — intentional fallback to no-auth mode
                case HubType.NoAuthentication:
                    {
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers();
                            endpoints.MapHub<WartHub>(NormalizeHubPath(hubName));
                        });
                        break;
                    }
#pragma warning restore CS0618
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
        /// <exception cref="ArgumentException">Thrown when the hub name list is null.</exception>
        public static IApplicationBuilder UseWartMiddleware(this IApplicationBuilder app, IEnumerable<string> hubNameList, HubType hubType)
        {
            ArgumentNullException.ThrowIfNull(hubNameList);

            var paths = GetDistinctPaths(hubNameList);

            app.UseForwardedHeaders();
            app.UseResponseCompression();
            app.UseRouting();

            switch (hubType)
            {
                default:
#pragma warning disable CS0618 // Internal routing — intentional fallback to no-auth mode
                case HubType.NoAuthentication:
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                        foreach (var path in paths)
                            endpoints.MapHub<WartHub>(path);
                    });
                    break;
#pragma warning restore CS0618

                case HubType.JwtAuthentication:
                    app.UseJwtMiddleware();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                        foreach (var path in paths)
                            endpoints.MapHub<WartHubJwt>(path);
                    });
                    break;

                case HubType.CookieAuthentication:
                    app.UseCookieMiddleware();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                        foreach (var path in paths)
                            endpoints.MapHub<WartHubCookie>(path);
                    });
                    break;
            }

            return app;
        }
    }
}
