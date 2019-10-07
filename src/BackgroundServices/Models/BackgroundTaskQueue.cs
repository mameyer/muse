using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BackgroundServices.Interfaces;

namespace BackgroundServices.Models
{
    public class BackgroundTaskQueue<T> : IBackgroundTaskQueue<T>
    {
        private readonly ConcurrentQueue<IBackgroundTaskBase<T>> workItems;
        private readonly SemaphoreSlim signal;
        public readonly ConcurrentDictionary<Guid, JobHistoryEntry> JobHistory;
        private readonly ConcurrentDictionary<string, DateTime> tasksMarkedToStop;
        private readonly ConcurrentDictionary<string, DateTime> registeredTasks;

        public BackgroundTaskQueue()
        {
            workItems = new ConcurrentQueue<IBackgroundTaskBase<T>>();
            signal = new SemaphoreSlim(0);
            JobHistory = new ConcurrentDictionary<Guid, JobHistoryEntry>();
            tasksMarkedToStop = new ConcurrentDictionary<string, DateTime>();
            registeredTasks = new ConcurrentDictionary<string, DateTime>();

            State = BackgroundTaskQueueState.WAITING_FOR_TASK;
        }

        public BackgroundTaskQueueState State { get; set; }

        private Guid GenerateId()
        {
            return Guid.NewGuid();
        }

        private void RegisterTask(IBackgroundTaskBase<T> workItem)
        {
            registeredTasks.TryAdd(workItem.Name, DateTime.Now);
        }

        public bool IsRegistered(string taskName)
        {
            return registeredTasks.ContainsKey(taskName);
        }

        public bool Unregister(string taskName)
        {
            if (tasksMarkedToStop.ContainsKey(taskName))
            {
                tasksMarkedToStop.TryRemove(taskName, out DateTime _);
            }

            return registeredTasks.TryRemove(taskName, out DateTime _);
        }

        public void Enqueue(IBackgroundTaskBase<T> workItem, bool force = false,
            bool keepTimestamp = false, bool generateNewId = true)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            RegisterTask(workItem);

            if (!keepTimestamp)
            {
                workItem.Timestamp = DateTime.Now;
            }

            if (workItem.Id == null
                || generateNewId)
            {
                workItem.Id = GenerateId();
            }

            workItems.Enqueue(workItem);
            signal.Release();
        }

        public async Task<IBackgroundTaskBase<T>> DequeueAsync(CancellationToken cancellationToken)
        {
            await signal.WaitAsync(cancellationToken);
            if (workItems.TryDequeue(out var workItem))
            {
                return workItem;
            }

            return null;
        }

        public bool ClearJobHistory(int max, int deleteCount)
        {
            if (JobHistory.Count >= max)
            {
                var orderedJobs = JobHistory.OrderBy(job => job.Value.Timestamp);
                for (int i = 0; i < Math.Min(deleteCount, JobHistory.Count); i++)
                {
                    JobHistory.TryRemove(orderedJobs.ElementAt(i).Key, out JobHistoryEntry item);
                }

                return true;
            }

            return false;
        }

        public bool AddToHistory(IBackgroundTaskBase<T> workItem)
        {
            if (workItem == null
                || workItem.Id == null)
            {
                return false;
            }

            JobHistory.TryAdd(workItem.Id.Value, new JobHistoryEntry
            {
                TaskName = workItem.Name,
                Timestamp = workItem.Timestamp,
                ExecutedAt = DateTime.Now,
                Parameters = workItem.Parameters,
                Result = workItem.Result
            });

            return true;
        }

        public IBackgroundTaskBase<T>[] GetCurrentEnqueuedTasks()
        {
            return workItems.ToArray();
        }

        public bool IsInQueue(string name)
        {
            return workItems.Any(task => task.Name == name);
        }

        public int Count()
        {
            return workItems.Count;
        }

        public List<JobHistoryEntry> GetJobHistory()
        {
            return JobHistory.Select(item => item.Value).ToList();
        }

        public bool MarkToStop(string name)
        {
            if (!IsRegistered(name))
            {
                return false;
            }

            if (tasksMarkedToStop.ContainsKey(name))
            {
                return false;
            }

            tasksMarkedToStop.TryAdd(name, DateTime.Now);
            return true;
        }

        public bool IsMarkedToStop(string name)
        {
            return tasksMarkedToStop.ContainsKey(name);
        }

        public bool UpdateTaskResult(Guid id, object result)
        {
            if (!JobHistory.ContainsKey(id))
            {
                return false;
            }

            JobHistory[id].Result = result;
            return true;
        }
    }
}