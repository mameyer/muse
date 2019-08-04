using BackgroundServices.Interfaces;
using BackgroundServices.Models;
using BackgroundServices.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BackgroundServicesTest.UnitTests
{
    public class DelayTask : BackgroundTask
    {
        private readonly int millisecondsDelay;

        public DelayTask(int millisecondsDelay)
            : base("DelayTask")
        {
            this.millisecondsDelay = millisecondsDelay;
        }

        public override async Task Run(CancellationToken cancellationToken)
        {
            // Task.Delay will create a task which will complete after a time delay.
            // Task.Delay is not blocking the calling thread
            // creates a task that is running in a different context
            await Task.Delay(millisecondsDelay, cancellationToken);
        }
    }

    public class QueuedHostedServiceTest
    {
        private IBackgroundTaskQueue backgroundTaskQueue;
        private QueuedHostedService queuedHostedService;

        public QueuedHostedServiceTest()
        {
        }

        /// <summary>
        /// 1. Enqueues a single task
        /// 2. Starts the service
        /// 3. Sleeps in milliseconds
        /// 4. Checks current dequeued task
        /// 5. Checks if task queue is empty
        /// Checks if the task is successfully enqueued.
        /// </summary>
        [Fact]
        public void RunSingleTaskTest()
        {
            #region Arrange
            ILoggerFactory loggerFactory = new LoggerFactory();
            backgroundTaskQueue = new BackgroundTaskQueue();
            queuedHostedService = new QueuedHostedService(backgroundTaskQueue, loggerFactory);

            const int millisecondsDelay = 100;
            BackgroundTask task = new DelayTask(millisecondsDelay);
            backgroundTaskQueue.Enqueue(task);
            #endregion

            #region AssertTaskInQueue
            Assert.Equal(1, backgroundTaskQueue.Count());
            #endregion

            #region Act
            // start service
            queuedHostedService.StartAsync(new CancellationToken());
            // task should be ready after 1000ms
            Thread.Sleep(1000);
            // BackgroundTask currentTask = queuedHostedService.GetLastDequeuedTask();
            // Assert.Equal(task, currentTask);
            Assert.Equal(0, backgroundTaskQueue.Count());
            #endregion

            #region Cleanup
            queuedHostedService.StopAsync(new CancellationToken());
            #endregion
        }

        /// <summary>
        /// 1. starts service
        /// 2. enqueues a single task
        /// 3. stops service
        /// Checks that the service is successfully stopped when there are running tasks.
        /// </summary>
        [Fact]
        public void StopOnRunningTest()
        {
            #region Arrange
            ILoggerFactory loggerFactory = new LoggerFactory();
            backgroundTaskQueue = new BackgroundTaskQueue();
            queuedHostedService = new QueuedHostedService(backgroundTaskQueue, loggerFactory);

            queuedHostedService.StartAsync(new CancellationToken());
            
            const int millisecondsDelay = 100000;
            BackgroundTask task = new DelayTask(millisecondsDelay);

            backgroundTaskQueue.Enqueue(task);
            #endregion

            #region AssertNotEmpty
            Assert.Equal(1, backgroundTaskQueue.Count());
            #endregion

            #region Act
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(100);
            var result = queuedHostedService.StopAsync(cancellationTokenSource.Token);
            #endregion

            #region Assert
            Assert.Equal(TaskStatus.WaitingForActivation, result.Status);
            #endregion
        }

        /// <summary>
        /// 1. enqueues long working task
        /// 2. enques some other tasks
        /// 3. starts services
        /// 4. short sleep until long working task is dequeued by service
        /// 5. dequeue other tasks manually
        /// Checks if non running sevices can successfully be dequeued.
        /// </summary>
        [Fact]
        public void EnqueueMultipleTest()
        {
            #region Arrange
            ILoggerFactory loggerFactory = new LoggerFactory();
            backgroundTaskQueue = new BackgroundTaskQueue();
            queuedHostedService = new QueuedHostedService(backgroundTaskQueue, loggerFactory);

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(100);

            // enqueue long working task
            DelayTask longWorkingTask = new DelayTask(10000);
            backgroundTaskQueue.Enqueue(longWorkingTask);

            // enqueue other tasks
            const int millisecondsDelay = 100;
            const int numTasks = 5;
            List<BackgroundTask> otherTasks = new List<BackgroundTask>();
            for (int i=0; i<numTasks; i++)
            {
                BackgroundTask task = new DelayTask(millisecondsDelay);
                otherTasks.Add(task);
                backgroundTaskQueue.Enqueue(task);
            }
            #endregion

            #region AssertQueueSize
            Assert.Equal(numTasks+1, backgroundTaskQueue.Count());
            #endregion

            #region Act
            queuedHostedService.StartAsync(new CancellationToken());
            // Sleep n ms
            Thread.Sleep(100);
            #endregion

            #region Assert
            Assert.Equal(numTasks, backgroundTaskQueue.Count());
            // Assert.Equal(queuedHostedService.GetLastDequeuedTask(), longWorkingTask);
            #endregion

            #region Act / Assert
            // Deque other tasks
            for (int i=0; i<numTasks; i++)
            {
                BackgroundTask task = backgroundTaskQueue.DequeueAsync(new CancellationToken()).Result;
                Assert.Equal(otherTasks[i], task);
            }
            Assert.Equal(0, backgroundTaskQueue.Count());
            // Timeout error when calling dequeue on empty queue
            Assert.ThrowsAsync<TaskCanceledException>(() => backgroundTaskQueue.DequeueAsync(cancellationTokenSource.Token));
            #endregion

            #region Cleanup
            queuedHostedService.StopAsync(cancellationTokenSource.Token);
            #endregion
        }
    }
}
