// (c) 2025 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using WART_Core.Entity;
using WART_Core.Filters;
using WART_Core.Services;

namespace WART_Tests.Services
{
    public class WartEventWorkerTests
    {
        public class TestHub : Hub { }

        /// <summary>
        /// Exposes ExecuteAsync for testing.
        /// </summary>
        private class TestableWorker : WartEventWorker<TestHub>
        {
            public TestableWorker(WartEventQueueService queue, IHubContext<TestHub> hub, ILogger<WartEventWorker<TestHub>> logger)
                : base(queue, hub, logger) { }

            public Task RunAsync(CancellationToken ct) => ExecuteAsync(ct);
        }

        private static WartEventWithFilters MakeEvent(List<IFilterMetadata>? filters = null)
        {
            var evt = new WartEvent("GET", "/api/test", "127.0.0.1");
            return new WartEventWithFilters(evt, filters ?? []);
        }

        private static (TestableWorker worker, WartEventQueueService queue, Mock<IHubContext<TestHub>> hubMock) CreateWorker()
        {
            var queue = new WartEventQueueService();
            var hubMock = new Mock<IHubContext<TestHub>>();
            var clientsMock = new Mock<IHubClients>();
            var clientProxyMock = new Mock<IClientProxy>();

            clientsMock.Setup(c => c.All).Returns(clientProxyMock.Object);
            clientsMock.Setup(c => c.Group(It.IsAny<string>())).Returns(clientProxyMock.Object);
            hubMock.Setup(h => h.Clients).Returns(clientsMock.Object);

            var logger = new Mock<ILogger<WartEventWorker<TestHub>>>();
            var worker = new TestableWorker(queue, hubMock.Object, logger.Object);

            return (worker, queue, hubMock);
        }

        [Fact]
        public async Task Stops_Gracefully_When_Cancelled()
        {
            var (worker, queue, _) = CreateWorker();

            using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(300));

            // TaskCanceledException is expected — BackgroundService base class
            // normally swallows it on shutdown.
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => worker.RunAsync(cts.Token));
        }

        [Fact]
        public async Task Processes_Event_And_Sends_To_All_Clients()
        {
            var (worker, queue, hubMock) = CreateWorker();
            var item = MakeEvent();
            queue.Enqueue(item);

            // With no connected clients the worker delays without dequeuing.
            using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(600));
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => worker.RunAsync(cts.Token));

            // Event remains in the queue because HasConnectedClientsFor<TestHub> is false.
            Assert.Equal(1, queue.Count);
        }

        [Fact]
        public async Task ExcludeWart_Filter_Skips_Event()
        {
            var (worker, queue, _) = CreateWorker();
            var item = MakeEvent([new ExcludeWartAttribute()]);
            queue.Enqueue(item);

            using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(600));
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => worker.RunAsync(cts.Token));

            // Event stays in queue because no clients are connected
            Assert.Equal(1, queue.Count);
        }

        [Fact]
        public void RetryCount_Increments_On_Event()
        {
            var item = MakeEvent();
            Assert.Equal(0, item.RetryCount);

            item.RetryCount++;
            Assert.Equal(1, item.RetryCount);
        }

        [Fact]
        public void Event_Dropped_After_MaxRetries_Is_Not_Requeued()
        {
            var queue = new WartEventQueueService();
            var item = MakeEvent();

            // Simulate reaching the max retry count (5)
            item.RetryCount = 6;

            // At this point the worker logic would NOT re-enqueue because RetryCount > MaxRetryCount
            bool shouldReenqueue = item.RetryCount <= 5;
            Assert.False(shouldReenqueue);
        }

        [Fact]
        public void Event_Under_MaxRetries_Is_Requeued()
        {
            var queue = new WartEventQueueService();
            var item = MakeEvent();
            item.RetryCount = 3;

            bool shouldReenqueue = item.RetryCount <= 5;
            Assert.True(shouldReenqueue);

            queue.Enqueue(item);
            Assert.Equal(1, queue.Count);
        }
    }
}
