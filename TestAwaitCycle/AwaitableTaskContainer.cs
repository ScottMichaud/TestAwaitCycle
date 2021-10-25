using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestAwaitCycle
{
    /// <summary>
    /// Simple example of using TaskCompletionSource to create an awaitable
    /// consumer/producer FIFO container.
    /// </summary>
    /// <typeparam name="T">The item to store in the container</typeparam>
    public class AwaitableTaskContainer<T>
    {
        // Mutex for Read/Write access.
        private readonly object _lock = new();
        // Stored values waiting to be consumed.
        private readonly Queue<T> _storedValues = new();
        // A promise left by a consumer.
        private TaskCompletionSource<T>? _waitingTask;

        /// <summary>
        /// The consumer calls this to acquire a Task (promise) for the next value.
        /// </summary>
        /// <returns>A Task that will be fulfilled when a value is available.</returns>
        public Task<T> GetValue()
        {
            TaskCompletionSource<T> newResult = CreateTaskSource();
            
            lock (_lock)
            {
                if (_storedValues.Count > 0)
                {
                    // If a producer has left at least one item for us, grab it without waiting.
                    newResult.SetResult(_storedValues.Dequeue());
                }
                else
                {
                    // Otherwise, leave a promise for a producer to fulfill.
                    _waitingTask = newResult;
                }
            }

            // Return the Task handle from the promise (which may or may not be fulfilled).
            return newResult.Task;
        }

        /// <summary>
        /// Producer(s) call this to add a value to the queue. This will wake up the consumer
        /// if it is waiting on a value.
        /// </summary>
        /// <param name="value">The value to add to the queue.</param>
        public void AddValue(T value)
        {
            lock (_lock)
            {
                // Add the value for the consumer at the tail of the queue.
                _storedValues.Enqueue(item: value);

                // If there is a consumer waiting, give them the item at the head.
                if (_waitingTask is null == false)
                {
                    _waitingTask.SetResult(_storedValues.Dequeue());
                }
            }
        }
        
        // Always run asynchronously.
        private static TaskCompletionSource<T> CreateTaskSource()
        {
            return new TaskCompletionSource<T>(creationOptions: TaskCreationOptions.RunContinuationsAsynchronously);
        }
    }
}