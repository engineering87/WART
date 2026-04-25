// (c) 2025 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using WART_Core.Entity;
using WART_Core.Services;

namespace WART_Tests.Services
{
    public class WartEventQueueServiceTests
    {
        private static WartEventWithFilters MakeEvent()
        {
            return new WartEventWithFilters(new WartEvent("GET", "/api/test", "127.0.0.1"), []);
        }

        [Fact]
        public void NewQueue_IsEmpty()
        {
            var queue = new WartEventQueueService();

            Assert.True(queue.IsEmpty);
            Assert.Equal(0, queue.Count);
        }

        [Fact]
        public void Enqueue_IncreasesCount()
        {
            var queue = new WartEventQueueService();

            queue.Enqueue(MakeEvent());
            queue.Enqueue(MakeEvent());

            Assert.Equal(2, queue.Count);
            Assert.False(queue.IsEmpty);
        }

        [Fact]
        public void Enqueue_Null_ThrowsArgumentNullException()
        {
            var queue = new WartEventQueueService();

            Assert.Throws<ArgumentNullException>(() => queue.Enqueue(null!));
        }

        [Fact]
        public void TryDequeue_ReturnsItemsInFifoOrder()
        {
            var queue = new WartEventQueueService();
            var first = MakeEvent();
            var second = MakeEvent();

            queue.Enqueue(first);
            queue.Enqueue(second);

            Assert.True(queue.TryDequeue(out var item1));
            Assert.Same(first, item1);
            Assert.True(queue.TryDequeue(out var item2));
            Assert.Same(second, item2);
            Assert.True(queue.IsEmpty);
        }

        [Fact]
        public void TryDequeue_EmptyQueue_ReturnsFalse()
        {
            var queue = new WartEventQueueService();

            Assert.False(queue.TryDequeue(out var item));
            Assert.Null(item);
        }

        [Fact]
        public void TryPeek_ReturnsItemWithoutRemoving()
        {
            var queue = new WartEventQueueService();
            var evt = MakeEvent();
            queue.Enqueue(evt);

            Assert.True(queue.TryPeek(out var peeked));
            Assert.Same(evt, peeked);
            Assert.Equal(1, queue.Count);
        }

        [Fact]
        public void TryPeek_EmptyQueue_ReturnsFalse()
        {
            var queue = new WartEventQueueService();

            Assert.False(queue.TryPeek(out var item));
            Assert.Null(item);
        }

        [Fact]
        public async Task WaitToReadAsync_CompletesImmediately_WhenDataAvailable()
        {
            var queue = new WartEventQueueService();
            queue.Enqueue(MakeEvent());

            var result = await queue.WaitToReadAsync(CancellationToken.None);

            Assert.True(result);
        }

        [Fact]
        public async Task WaitToReadAsync_Unblocks_WhenItemEnqueued()
        {
            var queue = new WartEventQueueService();

            var waitTask = queue.WaitToReadAsync(CancellationToken.None);

            // Should not be completed yet since the queue is empty.
            Assert.False(waitTask.IsCompleted);

            // Enqueue an item to unblock.
            queue.Enqueue(MakeEvent());

            var result = await waitTask;
            Assert.True(result);
        }

        [Fact]
        public async Task WaitToReadAsync_ThrowsOnCancellation()
        {
            var queue = new WartEventQueueService();
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => queue.WaitToReadAsync(cts.Token).AsTask());
        }

        [Fact]
        public void Complete_PreventsEnqueue()
        {
            var queue = new WartEventQueueService();
            queue.Complete();

            Assert.Throws<InvalidOperationException>(() => queue.Enqueue(MakeEvent()));
        }

        [Fact]
        public async Task Complete_WithError_PropagatesExceptionToReaders()
        {
            var queue = new WartEventQueueService();
            var expected = new InvalidOperationException("test error");

            queue.Complete(expected);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await queue.ReadAllAsync().GetAsyncEnumerator().MoveNextAsync());
            Assert.Same(expected, ex);
        }

        [Fact]
        public async Task ReadAllAsync_ReturnsEnqueuedItems()
        {
            var queue = new WartEventQueueService();
            var evt1 = MakeEvent();
            var evt2 = MakeEvent();

            queue.Enqueue(evt1);
            queue.Enqueue(evt2);
            queue.Complete();

            var items = new List<WartEventWithFilters>();
            await foreach (var item in queue.ReadAllAsync())
            {
                items.Add(item);
            }

            Assert.Equal(2, items.Count);
            Assert.Same(evt1, items[0]);
            Assert.Same(evt2, items[1]);
        }
    }
}
