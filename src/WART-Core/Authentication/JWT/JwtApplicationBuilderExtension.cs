// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.Builder;

namespace WART_Core.Authentication.JWT
{
    public static class JwtApplicationBuilderExtension
    {
        /// <summary>
        /// Use JWT authentication dependency to IApplicationBuilder.
        /// </summary>
        /// <param name="app">The IApplicationBuilder to configure the middleware pipeline.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseJwtMiddleware(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }
    }
}
