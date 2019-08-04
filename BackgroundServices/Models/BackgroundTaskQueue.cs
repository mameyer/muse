using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BackgroundServices.Interfaces;
using BackgroundServices.Models;
using System.Linq;

namespace BackgroundServices.Models
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private ConcurrentQueue<BackgroundTask> workItems = new ConcurrentQueue<BackgroundTask>();
        private SemaphoreSlim signal = new SemaphoreSlim(0);
        private List<Tuple<string, DateTime>> taskHistory;

        public BackgroundTaskQueue()
        {
            taskHistory = new List<Tuple<string, DateTime>>();
        }

        public void Enqueue(BackgroundTask workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            workItems.Enqueue(workItem);
            taskHistory.Add(new Tuple<string, DateTime>(workItem.Name, DateTime.Now));
            signal.Release();
        }

        public async Task<BackgroundTask> DequeueAsync(CancellationToken cancellationToken)
        {
            await signal.WaitAsync(cancellationToken);
            workItems.TryDequeue(out var workItem);
            return workItem;
        }

        public IEnumerable<DateTime> GetHistoryForTask(string taskName)
        {
            return taskHistory.Where(hist => hist.Item1 == taskName).Select(hist => hist.Item2);
        }

        public BackgroundTask[] GetCurrentEnqueuedTasks()
        {
            return workItems.ToArray();
        }

        public int Count()
        {
            return workItems.Count;
        }
    }
}