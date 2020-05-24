﻿// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.Builder;
using WART_Core.Hubs;

namespace WART_Core.Middleware
{
    public static class WartApplicationBuilderExtension
    {
        private const string DefaultHub = "warthub";

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
                endpoints.MapHub<WartHub>($"/{DefaultHub}");
            });

            app.UseForwardedHeaders();

            return app;
        }

        /// <summary>
        /// Use WART dependency to IApplicationBuilder
        /// </summary>
        /// <param name="app"></param>
        /// <param name="hubName">The SignalR hub name</param>
        /// <returns></returns>
        public static IApplicationBuilder UseWartMiddleware(this IApplicationBuilder app, string hubName)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<WartHub>($"/{hubName.Trim()}");
            });

            app.UseForwardedHeaders();

            return app;
        }
    }
}
