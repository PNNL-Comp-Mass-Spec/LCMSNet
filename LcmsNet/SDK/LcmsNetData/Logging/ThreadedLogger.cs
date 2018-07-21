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
        /// Modeled after the idea of a BufferBlock, but not using a BufferBlock because that requires tracking a NuGet package. Uses a ManualResetEvent internally, so not safe for multiple consumers.
        /// </summary>
        /// <typeparam name="TU"></typeparam>
        private class BufferQueue<TU> : IDisposable
        {
            private readonly Queue<TU> queue = new Queue<TU>();
            private readonly ManualResetEventSlim trigger = new ManualResetEventSlim(false);
            private bool complete = false;

            /// <summary>
            /// Returns true if there is available output, false if an error or marked complete (with no more output)
            /// </summary>
            /// <returns></returns>
            public Task<bool> OutputAvailableAsync()
            {
                var tcs = new TaskCompletionSource<bool>();

                // Optimization: check for available items before waiting on the manual reset event
                var result = ItemsAvailable();
                if (result.HasValue)
                {
                    // safe - the task hasn't returned yet.
                    tcs.SetResult(result.Value);
                    trigger.Reset(); // reset to prevent triggering once empty
                    return tcs.Task;
                }

                // Run the wait in a new thread, so we can return tcs.Task and the await will work properly
                Task.Factory.StartNew(() =>
                {
                    trigger.Wait();
                    trigger.Reset(); // reset to be ready for a future trigger

                    var available = ItemsAvailable();
                    try
                    {
                        tcs.SetResult(available ?? false);
                    }
                    catch
                    {
                        // Silenced
                    }
                });

                return tcs.Task;
            }

            /// <summary>
            /// Retrieve an item off of the queue
            /// </summary>
            /// <returns></returns>
            public TU Receive()
            {
                return queue.Dequeue();
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
                queue.Enqueue(item);
                trigger.Reset();
                trigger.Set();
            }

            /// <summary>
            /// Mark the queue as complete, so that things will exit out properly
            /// </summary>
            public void Complete()
            {
                this.complete = true;
                trigger.Set();
            }

            /// <summary>
            /// Clean up.
            /// </summary>
            public void Dispose()
            {
                if (!complete)
                {
                    Complete();
                }
                trigger?.Dispose();
            }
        }
    }
}
