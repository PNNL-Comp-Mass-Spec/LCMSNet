using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LcmsNetData.Logging
{
    /// <summary>
    /// Class to basically run the logging on another thread as a producer-consumer queue, rather than creating a new task for every log message
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ThreadedLogger<T> : IDisposable
    {
        /// <summary>
        /// This could almost be directly replaced with a BufferBlock (but I don't want to add the TPL dependency).
        /// </summary>
        private readonly BufferQueue<T> buffer = new BufferQueue<T>();
        private readonly Thread consumerThread = null;
        private readonly Action<T> consumeAction;

        /// <summary>
        /// True if the logger has been shut down.
        /// </summary>
        private bool isShutdown = false;

        /// <summary>
        /// Consume all items, continuing until the queue is shutdown
        /// </summary>
        private void ConsumeAll()
        {
            T item = default(T);
            while (!buffer.IsComplete || buffer.HasItemsInQueue)
            {
                while ((item = Consume().Result) != null)
                {
                    consumeAction(item);
                }

                // Sleep 100 milliseconds to prevent busy-looping
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Consume a single item, asynchronously
        /// </summary>
        /// <returns></returns>
        private async Task<T> Consume()
        {
            if (await buffer.OutputAvailableAsync())
            {
                return buffer.Receive();
            }

            return default(T);
        }

        /// <summary>
        /// Add a new item to the queue (produce an item)
        /// </summary>
        /// <param name="item"></param>
        /// <returns>True if <paramref name="item"/> is null or was added; false if the logger is shutting down or shut down</returns>
        public bool AddItem(T item)
        {
            if (item != null && !isShutdown)
            {
                return buffer.Post(item);
            }

            return item == null;
        }

        ~ThreadedLogger()
        {
            Shutdown();
        }

        /// <summary>
        /// Clean up
        /// </summary>
        public void Dispose()
        {
            if (isShutdown)
            {
                return;
            }
            Shutdown();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Properly close down the threaded logging
        /// </summary>
        public void Shutdown()
        {
            isShutdown = true;
            buffer.Complete();
            consumerThread.Join();
            buffer.Dispose();
        }

        /// <summary>
        /// Create a consumer that will run on a separate thread than everything else for performance reasons
        /// </summary>
        /// <param name="actionOnConsume"></param>
        public ThreadedLogger(Action<T> actionOnConsume)
        {
            consumerThread = new Thread(() => ConsumeAll());
            consumeAction = actionOnConsume ?? throw new ArgumentNullException(nameof(actionOnConsume));
            consumerThread.Start();
        }

        /// <summary>
        /// Modeled after the idea of a BufferBlock, but not using a BufferBlock because that requires tracking a NuGet package.
        /// </summary>
        /// <typeparam name="TU"></typeparam>
        private class BufferQueue<TU> : IDisposable
        {
            private readonly Queue<TU> queue = new Queue<TU>();
            private readonly SemaphoreSlim trigger = new SemaphoreSlim(0);
            private readonly object addRemoveLock = new object();
            private bool isClosing = false;

            /// <summary>
            /// True if the BufferQueue has been marked complete, and will accept new items
            /// </summary>
            public bool IsComplete { get; private set; } = false;

            /// <summary>
            /// True if there are items waiting to be consumed; only for checking in case of error
            /// </summary>
            public bool HasItemsInQueue => queue.Count > 0;

            /// <summary>
            /// Returns true if there is available output, false if an error or marked complete (with no more output)
            /// </summary>
            /// <returns></returns>
            public async Task<bool> OutputAvailableAsync()
            {
                await trigger.WaitAsync().ConfigureAwait(false);
                return ItemsAvailable() ?? false;
            }

            /// <summary>
            /// Retrieve an item off of the queue
            /// </summary>
            /// <returns></returns>
            public TU Receive()
            {
                lock (addRemoveLock)
                {
                    return queue.Dequeue();
                }
            }

            private bool? ItemsAvailable()
            {
                if (queue.Count > 0)
                {
                    return true;
                }

                if (IsComplete)
                {
                    return false;
                }

                // error state, or pre-wait check
                return null;
            }

            /// <summary>
            /// Add an item to the queue, notifying any consumer(s) of the available item.
            /// </summary>
            /// <param name="item"></param>
            public bool Post(TU item)
            {
                if (IsComplete || isClosing)
                {
                    return false;
                }

                lock (addRemoveLock)
                {
                    queue.Enqueue(item);
                }

                trigger.Release();
                return true;
            }

            /// <summary>
            /// Mark the queue as complete, so that things will exit out properly
            /// </summary>
            public void Complete()
            {
                isClosing = true;
                if (!IsComplete)
                {
                    trigger.Release(queue.Count + 1); // Doesn't matter if we release() extra times after we set complete to true.
                }
                IsComplete = true;
            }

            /// <summary>
            /// Clean up.
            /// </summary>
            public void Dispose()
            {
                Complete();
                trigger?.Dispose();
            }
        }
    }
}
