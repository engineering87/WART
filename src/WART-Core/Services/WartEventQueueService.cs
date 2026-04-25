// (c) 2024 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using WART_Core.Entity;

namespace WART_Core.Services
{
    /// <summary>
    /// A service that manages a channel-based queue for WartEvent objects with filters.
    /// Uses <see cref="Channel{T}"/> for efficient, non-polling async consumption.
    /// </summary>
    public class WartEventQueueService
    {
        private readonly Channel<WartEventWithFilters> _channel =
            Channel.CreateUnbounded<WartEventWithFilters>(new UnboundedChannelOptions
            {
                SingleReader = false,
                SingleWriter = false
            });

        /// <summary>
        /// Enqueues a WartEventWithFilters object to the channel.
        /// </summary>
        /// <param name="wartEventWithFilters">The WartEventWithFilters object to enqueue.</param>
        /// <returns>True if the event was successfully written; false if the channel has been completed.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when wartEventWithFilters is null.</exception>
        public bool Enqueue(WartEventWithFilters wartEventWithFilters)
        {
            ArgumentNullException.ThrowIfNull(wartEventWithFilters);

            if (!_channel.Writer.TryWrite(wartEventWithFilters))
            {
                throw new InvalidOperationException("Unable to enqueue the event. The channel may have been completed.");
            }

            return true;
        }

        /// <summary>
        /// Attempts to read a WartEventWithFilters object from the channel without waiting.
        /// </summary>
        /// <param name="item">The read WartEventWithFilters object.</param>
        /// <returns>True if an event was read; otherwise, false.</returns>
        public bool TryDequeue(out WartEventWithFilters item) => _channel.Reader.TryRead(out item);

        /// <summary>
        /// Attempts to peek at the next item without removing it.
        /// </summary>
        public bool TryPeek(out WartEventWithFilters item) => _channel.Reader.TryPeek(out item);

        /// <summary>
        /// Gets the current count of events in the channel.
        /// </summary>
        public int Count => _channel.Reader.Count;

        /// <summary>
        /// Check if the channel is empty.
        /// Note: This is an approximate check and may be subject to race conditions in concurrent scenarios.
        /// </summary>
        public bool IsEmpty => !_channel.Reader.TryPeek(out _);

        /// <summary>
        /// Waits asynchronously until data is available to read.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>True if data is available; false if the channel is completed.</returns>
        public ValueTask<bool> WaitToReadAsync(CancellationToken cancellationToken = default)
            => _channel.Reader.WaitToReadAsync(cancellationToken);

        /// <summary>
        /// Returns an async enumerable that reads all items from the channel.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An async enumerable of WartEventWithFilters.</returns>
        public IAsyncEnumerable<WartEventWithFilters> ReadAllAsync(CancellationToken cancellationToken = default)
            => _channel.Reader.ReadAllAsync(cancellationToken);

        /// <summary>
        /// Marks the channel as complete, signaling that no more items will be written.
        /// </summary>
        /// <param name="error">An optional exception to propagate to consumers.</param>
        public void Complete(Exception? error = null) => _channel.Writer.TryComplete(error);
    }
}