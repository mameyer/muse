using System;
using System.Threading;
using System.Threading.Tasks;
using BackgroundServices.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BackgroundServices.Services
{
    /// <summary>
    /// Simple Service for background tasks that are managed in a queue.
    /// Usage for registering in the application startup:
    /// - Register BackgroundTaskQueue:
    ///   services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
    /// - Register QueuedHostedService:
    ///   services.AddHostedService<QueuedHostedService>();
    /// Usage in MVC controllers:
    /// - Create new BackgroundTask 'BTask' that derives from the base class and
    ///   implements the abstract Run() method.
    /// - Dependency inject BackgroundTaskQueue in the controllers constructor:
    ///   public XController(IBackgroundTaskQueue queue, ...)
    /// - Add intance of the background task 'BTask' to the queue:
    ///   queue.Enqueue(new BTask())
    /// For further details: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-2.1
    /// </summary>
    public class QueuedHostedService<T> : IHostedService
    {
        // cancellation token for shutdown request
        private CancellationTokenSource shutdown = new CancellationTokenSource();
        // background worker task for dequeuing tasks
        private Task backgroundWorker;
        // service logger
        private readonly ILogger logger;

        private ILoggingService loggingService;

        public T Out { get; set; }

        // the current (dequeued) task
        private IBackgroundTaskBase<T> currentTask;

        const int maxJobHistoryEntries = 200;
        const int jobHistoryDeleteCount = 100;

        public QueuedHostedService(IBackgroundTaskQueue<T> taskQueue, ILoggerFactory loggerFactory)
        {
            TaskQueue = taskQueue;
            logger = loggerFactory.CreateLogger<QueuedHostedService<T>>();
        }

        public void RegisterLoggingService(ILoggingService loggingService)
        {
            this.loggingService = loggingService;
        }

        public IBackgroundTaskQueue<T> TaskQueue { get; }

        /// <summary>
        /// Starts a background tasks that processes tasks of the internal task
        /// queue.
        /// Further details in the documentation of the IHostedService interface
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("QueuedHostedService is starting.");
            backgroundWorker = Task.Run(BackgroundProcessing);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Background task that runs the enqueued task in a loop.
        /// </summary>
        /// <returns></returns>
        private async Task BackgroundProcessing()
        {
            bool hasNextTask = false;
            Out = default(T);

            while (!shutdown.IsCancellationRequested)
            {
                hasNextTask = false;

                do
                {
                    // dequeue task
                    // blocking call
                    currentTask = await TaskQueue.DequeueAsync(shutdown.Token);

                    if (TaskQueue.IsMarkedToStop(currentTask.Name))
                    {
                        // check if task is stopped
                        // if true => dequeue task
                        if (loggingService != null) loggingService.Log(this, Enums.LogAction.RUN_TASK, currentTask);
                        TaskQueue.Unregister(currentTask.Name);
                        continue;
                    }
                    else if (!currentTask.CheckRun())
                    {
                        // check if current task needs to sleep
                        // enqueue current task
                        TaskQueue.Enqueue(currentTask, false, true, false);
                        Thread.Sleep(500);
                        continue;
                    }

                    hasNextTask = true;
                } while (!shutdown.IsCancellationRequested && !hasNextTask);

                try
                {
                    currentTask.LastRunAt = DateTime.Now;
                    currentTask.In = Out;

                    // update state
                    TaskQueue.State = BackgroundTaskQueueState.RUNNING_TASK;

                    // run task
                    // blocking
                    if (loggingService != null) loggingService.Log(this, Enums.LogAction.RUN_TASK, currentTask);
                    await currentTask.Run(shutdown.Token);
                    if (loggingService != null) loggingService.Log(this, Enums.LogAction.COMPLETED_TASK, currentTask);

                    // clear job history
                    TaskQueue.ClearJobHistory(maxJobHistoryEntries, jobHistoryDeleteCount);
                    // add new entry to job history
                    TaskQueue.AddToHistory(currentTask);

                    // update state
                    if (loggingService != null) loggingService.Log(this, Enums.LogAction.WAITING);
                    TaskQueue.State = BackgroundTaskQueueState.WAITING_FOR_TASK;

                    var result = currentTask.Result;
                    if (currentTask.Id != null)
                    {
                        TaskQueue.UpdateTaskResult(currentTask.Id.Value, result);
                    }
                    Out = currentTask.Out;
                    if (loggingService != null) loggingService.Log(this, Enums.LogAction.FINISHED_ITERATION, currentTask);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Error occurred executing '{currentTask.Name}'.");
                }

                // if task should be repeated => enqueue it again
                // might allready be in queue => then skip
                if (currentTask.Repeat
                    && !TaskQueue.IsInQueue(currentTask.Name))
                {
                    TaskQueue.Enqueue(currentTask, false, false, true);
                }
                else
                {
                    // unregister task
                    TaskQueue.Unregister(currentTask.Name);
                }
            }
        }

        /// <summary>
        /// Getter for the current (dequed) task.
        /// </summary>
        /// <returns>The currently dequeued task</returns>
        public IBackgroundTaskBase<T> GetLastDequeuedTask()
        {
            return currentTask;
        }

        /// <summary>
        /// Further details in the documentation of the IHostedService interface
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("QueuedHostedService is stopping.");
            // request for cancellation
            shutdown.Cancel();
            // return when background task cancelled OR cancellation is requested

            if (backgroundWorker == null)
            {
                return Task.CompletedTask;
            }

            return Task.WhenAny(backgroundWorker, Task.Delay(500, cancellationToken));
        }
    }
}
