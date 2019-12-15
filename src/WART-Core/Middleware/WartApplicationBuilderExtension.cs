// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.Builder;
using WART_Core.Hubs;

namespace WART_Core.Middleware
{
    public static class WartApplicationBuilderExtension
    {
        /// <summary>
        /// Use WART dependency to IApplicationBuilder
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseWartMiddleware(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<WartHub>("/warthub");
            });

            return app;
        }
    }
}
