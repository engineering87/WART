// (c) 2024 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using WART_Core.Middleware;
using WART_Api;
using Microsoft.Extensions.DependencyInjection;

namespace WART_Tests.Middleware
{
    public class WartApplicationBuilderExtensionTests
    {
        [Fact]
        public async Task UseWartMiddleware_ShouldMapControllersAndHub()
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                // Arrange
                var builder = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(configure =>
                {
                    configure.ConfigureServices(services =>
                    {
                        services.AddControllers();
                        services.AddSignalR();
                    });
                    configure.Configure(app =>
                    {
                        app.UseWartMiddleware();
                    });
                });

                // Act
                var client = builder.CreateClient();
                var response = await client.GetAsync("/signalr");
            });

            // Assert
            Assert.Null(exception);
        }                
    }
}
