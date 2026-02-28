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
        public void Enqueue_Null_IsIgnored()
        {
            var queue = new WartEventQueueService();

            queue.Enqueue(null!);

            Assert.True(queue.IsEmpty);
            Assert.Equal(0, queue.Count);
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
    }
}
