using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BackgroundServices.Models;

namespace BackgroundServices.Interfaces
{
    /// <summary>
    /// Interface for a queue managing background tasks.
    /// </summary>
    public interface IBackgroundTaskQueue
    {
        /// <summary>
        /// Enqueues a task.
        /// </summary>
        /// <param name="workItem">The task to enqueue</param>
        void Enqueue(BackgroundTask workItem);

        /// <summary>
        /// Dequeues a task.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task wrapping the dequeue result</returns>
        Task<BackgroundTask> DequeueAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Serves the list of currently enqueued tasks.
        /// </summary>
        /// <returns>The list of enqueued tasks</returns>
        BackgroundTask[] GetCurrentEnqueuedTasks();

        /// <summary>
        /// Servers the number of currently enqueued tasks.
        /// </summary>
        /// <returns>The number of currently enqueued tasks</returns>
        int Count();

        IEnumerable<DateTime> GetHistoryForTask(string taskName);
    }
}