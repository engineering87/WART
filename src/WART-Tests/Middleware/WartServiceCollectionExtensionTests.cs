// (c) 2024 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WART_Core.Middleware;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace WART_Tests.Middleware
{
    public class WartServiceCollectionExtensionTests
    {
        [Fact]
        public void AddWartMiddleware_ShouldConfigureForwardedHeadersOptions()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddWartMiddleware();

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetService<IOptions<ForwardedHeadersOptions>>();
            Assert.NotNull(options);
            Assert.Equal(ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto, options.Value.ForwardedHeaders);
        }

        [Fact]
        public void AddWartMiddleware_ShouldAddConsoleLogging()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddWartMiddleware();

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("TestLogger");
            Assert.NotNull(logger);
        }

        [Fact]
        public void AddWartMiddleware_ShouldAddSignalRWithDetailedErrorsEnabled()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddWartMiddleware();

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetService<IOptions<HubOptions>>();
            Assert.NotNull(options);
            Assert.True(options.Value.EnableDetailedErrors);
        }
    }
}
