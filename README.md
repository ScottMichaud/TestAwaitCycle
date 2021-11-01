# TestAwaitCycle

This example creates a single-producer, single-consumer FIFO queue using a [TaskCompletionSource](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskcompletionsource-1?view=net-5.0). This is a potential replacement for the pattern where a semaphore blocks a thread until another thread provides it with data. If data is available, consumers will return immediately; otherwise, consumers will wait until a producer adds a value. Because this uses async/await, instead of blocking threads on semaphores, the threads are returned to the dotnet thread pool (where they can be used for other tasks while the await is awaiting).

This example is intended as a learning reference.

A better container could experiment with lockless, removing the single-producer, single-consumer constraint, changing the backing data structure(s), and so forth.
