// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.Extensions.DependencyInjection;

namespace WART_Core.Middleware
{
    public static class WartServiceCollectionExtension
    {
        /// <summary>
        /// Add WART dependency to IServiceCollection
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddWartMiddleware(this IServiceCollection services)
        {
            services.AddSignalR();

            return services;
        }
    }
}
