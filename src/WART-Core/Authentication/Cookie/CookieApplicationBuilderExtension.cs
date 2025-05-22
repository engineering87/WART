// (c) 2025 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.Builder;

namespace WART_Core.Authentication.Cookie
{
    public static class CookieApplicationBuilderExtension
    {
        /// <summary>
        /// Use Cookie authentication dependency to IApplicationBuilder.
        /// </summary>
        /// <param name="app">The IApplicationBuilder to configure the middleware pipeline.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseCookieMiddleware(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }
    }
}