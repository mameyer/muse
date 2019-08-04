using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BackgroundServices.Interfaces;
using BackgroundServices.Models;
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
    public class QueuedHostedService : IHostedService
    {
        // cancellation token for shutdown request
        private CancellationTokenSource shutdown = new CancellationTokenSource();
        // background worker task for dequeuing tasks
        private Task backgroundWorker;
        // service logger
        private readonly ILogger logger;
        // the current (dequeued) task
        private BackgroundTask currentTask;

        public QueuedHostedService(IBackgroundTaskQueue taskQueue, ILoggerFactory loggerFactory)
        {
            TaskQueue = taskQueue;
            logger = loggerFactory.CreateLogger<QueuedHostedService>();
        }

        public IBackgroundTaskQueue TaskQueue { get; }

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
            while (!shutdown.IsCancellationRequested)
            {
                // dequeue task
                currentTask = await TaskQueue.DequeueAsync(shutdown.Token);
                logger.LogInformation("Dequeue task '" + currentTask.Name + "'");

                try
                {
                    // run task
                    await currentTask.Run(shutdown.Token);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Error occurred executing '{currentTask.Name}'.");
                }
            }
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
            if (backgroundWorker != null)
            {
                logger.LogInformation("QueuedHostedService CANCELLING backgroundWorker");
                return Task.WhenAny(backgroundWorker, Task.Delay(Timeout.Infinite, cancellationToken));
            }

            return Task.CompletedTask;
        }
    }
}