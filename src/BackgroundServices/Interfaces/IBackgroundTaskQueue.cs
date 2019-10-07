using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BackgroundServices.Models;

namespace BackgroundServices.Interfaces
{
    public enum BackgroundTaskQueueState
    {
        RUNNING_TASK,
        WAITING_FOR_TASK,
        UNKNOWN
    }

    public class JobHistoryEntry
    {
        public string TaskName { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime ExecutedAt { get; set; }
        public object Parameters { get; set; }
        public object Result { get; set; }
    }

    /// <summary>
    /// Interface for a queue managing background tasks.
    /// </summary>
    public interface IBackgroundTaskQueue<T>
    {
        /// <summary>
        /// Enqueues a task.
        /// </summary>
        /// <param name="workItem">The task to enqueue</param>
        void Enqueue(IBackgroundTaskBase<T> workItem, bool force = false,
            bool keepTimestamp = false, bool generateNewId = true);

        /// <summary>
        /// Dequeues a task.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task wrapping the dequeue result</returns>
        Task<IBackgroundTaskBase<T>> DequeueAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Serves the list of currently enqueued tasks.
        /// </summary>
        /// <returns>The list of enqueued tasks</returns>
        IBackgroundTaskBase<T>[] GetCurrentEnqueuedTasks();

        /// <summary>
        /// Servers the number of currently enqueued tasks.
        /// </summary>
        /// <returns>The number of currently enqueued tasks</returns>
        int Count();

        bool IsInQueue(string name);

        bool MarkToStop(string name);

        bool IsMarkedToStop(string name);

        List<JobHistoryEntry> GetJobHistory();

        bool UpdateTaskResult(Guid id, object result);

        BackgroundTaskQueueState State { get; set; }

        bool ClearJobHistory(int max, int deleteCount);

        bool AddToHistory(IBackgroundTaskBase<T> workItem);

        bool Unregister(string taskName);

        bool IsRegistered(string taskName);
    }
}