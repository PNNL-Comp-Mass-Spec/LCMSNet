using System;
using System.Collections.Concurrent;
using System.Threading;

namespace LcmsNetData.Logging
{
    /// <summary>
    /// Class to basically run the logging on another thread as a producer-consumer queue, rather than creating a new task for every log message
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ThreadedLogger<T> : IDisposable
    {
        private readonly BlockingCollection<T> buffer = new BlockingCollection<T>(new ConcurrentQueue<T>());
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
            foreach (var item in buffer.GetConsumingEnumerable())
            {
                consumeAction(item);
            }
        }

        /// <summary>
        /// Add a new item to the queue (produce an item)
        /// </summary>
        /// <param name="item"></param>
        /// <returns>True if <paramref name="item"/> is null or was added; false if the logger is shutting down or shut down</returns>
        public bool AddItem(T item)
        {
            if (item != null && !isShutdown && !buffer.IsAddingCompleted)
            {
                buffer.Add(item);
                return true;
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
            if (isShutdown)
            {
                return;
            }

            isShutdown = true;

            buffer.CompleteAdding();
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
    }
}
