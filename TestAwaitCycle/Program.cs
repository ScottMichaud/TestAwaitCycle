using System;
using System.Threading.Tasks;

namespace TestAwaitCycle
{
    public static class Program
    {
        public static async Task<int> Main()
        {
            AwaitableTaskContainer<int> tasks = new();
            
            // Start Producer but do not wait on "producer" Task (outside of Task.Run()).
            var producer = Task.Run(async () => { await Producer(argTasks: tasks); });

            // Wait six seconds for elements to queue up.
            await Task.Delay(6000);
            
            // Start Consumer but do not wait on "consumer" Task (outside of Task.Run()).
            var consumer = Task.Run(async () => { await Consumer(argTasks: tasks); });

            // Wait until both producer and consumer Task.Run() Tasks finish (which is never).
            await Task.WhenAll(producer, consumer);
            
            return 0;
        }

        private static async Task Producer(AwaitableTaskContainer<int> argTasks)
        {
            // Just a unique value per item.
            var idx = 0;
            // Demo-code has no end condition.
            while (true)
            {
                // Rate limit to one per second.
                await Task.Delay(1000);
                    
                Console.WriteLine($"Creating item {++idx}");
                argTasks.AddValue(value: idx);
            }
            // ReSharper disable once FunctionNeverReturns Reason: Infinite demo.
        }

        private static async Task Consumer(AwaitableTaskContainer<int> argTasks)
        {
            // Demo-code has no end condition.
            while (true)
            {
                Console.WriteLine($"Grabbing item {await argTasks.GetValue()}");
            }
            // ReSharper disable once FunctionNeverReturns Reason: Infinite demo.
        }
    }
}