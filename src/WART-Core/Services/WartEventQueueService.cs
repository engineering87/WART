// (c) 2024 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System.Collections.Concurrent;
using WART_Core.Entity;

namespace WART_Core.Services
{
    /// <summary>
    /// A service that manages a concurrent queue for WartEvent objects with filters.
    /// This class provides methods for enqueuing and dequeuing events.
    /// </summary>
    public class WartEventQueueService
    {
        // A thread-safe queue to hold WartEvent objects along with their associated filters.
        private readonly ConcurrentQueue<WartEventWithFilters> _queue = new ConcurrentQueue<WartEventWithFilters>();

        /// <summary>
        /// Enqueues a WartEventWithFilters object to the queue.
        /// </summary>
        /// <param name="wartEventWithFilters">The WartEventWithFilters object to enqueue.</param>
        public void Enqueue(WartEventWithFilters wartEventWithFilters)
        {
            // Adds the event with filters to the concurrent queue.
            _queue.Enqueue(wartEventWithFilters);
        }

        /// <summary>
        /// Attempts to dequeue a WartEventWithFilters object from the queue.
        /// </summary>
        /// <param name="wartEventWithFilters">The dequeued WartEventWithFilters object.</param>
        /// <returns>True if an event was dequeued; otherwise, false.</returns>
        public bool TryDequeue(out WartEventWithFilters wartEventWithFilters)
        {
            // Attempts to remove and return the event with filters from the queue.
            return _queue.TryDequeue(out wartEventWithFilters);
        }

        /// <summary>
        /// Gets the current count of events in the queue.
        /// </summary>
        public int Count => _queue.Count;
    }
}