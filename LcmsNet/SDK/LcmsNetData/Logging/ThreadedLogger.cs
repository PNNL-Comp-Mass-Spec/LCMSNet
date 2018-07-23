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
        /// This could be directly replaced with a BufferBlock.
        /// </summary>
        private readonly BufferQueue<T> buffer = new BufferQueue<T>();
        private readonly Thread consumerThread = null;
        private readonly Action<T> consumeAction;
        private bool isShutdown = false;

        /// <summary>
        /// Consume all items, continuing until the queue is shutdown
        /// </summary>
        private void ConsumeAll()
        {
            T item = default(T);
            while ((item = Consume().Result) != null)
            {
                consumeAction(item);
            }
        }

        /// <summary>
        /// Consume a single item, asynchronously
        /// </summary>
        /// <returns></returns>
        private async Task<T> Consume()
        {
            while (await buffer.OutputAvailableAsync())
            {
                return buffer.Receive();
            }

            return default(T);
        }

        /// <summary>
        /// Add a new item to the queue (produce an item)
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(T item)
        {
            if (item != null && !isShutdown)
            {
                buffer.Post(item);
            }
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
            private bool complete = false;

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

                if (complete)
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
            public void Post(TU item)
            {
                lock (addRemoveLock)
                {
                    queue.Enqueue(item);
                }

                trigger.Release();
            }

            /// <summary>
            /// Mark the queue as complete, so that things will exit out properly
            /// </summary>
            public void Complete()
            {
                if (!complete)
                {
                    trigger.Release(); // Doesn't matter if we release() extra times after we set complete to true.
                }
                complete = true;
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
