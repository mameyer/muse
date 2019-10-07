using BackgroundServices.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BackgroundServices.Models
{
    public enum MessageType
    {
        DEBUG,
        INFO,
        ERROR,
        PROCESS,
        RESULT,
        SUCCESS,
        NEW_DATA,
        LAST_REQUEST,
        TASK_COMPLETED
    }

    public class ClientMessage
    {
        public ClientMessage(MessageType type)
        {
            this.Type = type;
        }

        public MessageType Type { get; set; }
        public string Text { get; set; }
        public double Process { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public abstract class BackgroundTaskBase<T> : IBackgroundTaskBase<T>
    {
        public BackgroundTaskBase(string name, bool repeat = false, int sleepTimeMS = 0)
        {
            Name = name;
            Repeat = repeat;
            SleepTime = TimeSpan.FromMilliseconds(sleepTimeMS);
            LastRunAt = null;
        }

        public T In { get; set; }
        public T Out { get; set; }

        public Guid? Id { get; set; }

        public string Name { get; }

        public bool Repeat { get; internal set; }

        public TimeSpan SleepTime { get; }

        public DateTime? LastRunAt { get; set; }

        public Func<ClientMessage, bool> MessageHandler { get; set; }

        public object Parameters { get; set; }

        public object Result { get; set; }

        public DateTime Timestamp { get; set; }

        public void UpdateProgressInfo(int currentWritten, int totalRecords)
        {
            MessageHandler?.Invoke(new ClientMessage(MessageType.PROCESS)
            {
                Process = (double)currentWritten / totalRecords
            });
        }

        public virtual void LogError(string errorMsg)
        {
            MessageHandler?.Invoke(new ClientMessage(MessageType.ERROR)
            {
                Timestamp = DateTime.Now,
                Text = "[" + DateTime.Now.ToString("dd/MM/yy HH:mm:ss") + " ERROR] " + Name + ": " + errorMsg
            });
        }

        public virtual void LogInfo(string infoMsg)
        {
            MessageHandler?.Invoke(new ClientMessage(MessageType.INFO)
            {
                Timestamp = DateTime.Now,
                Text = "[" + DateTime.Now.ToString("dd/MM/yy HH:mm:ss") + " INFO] " + Name + ": " + infoMsg
            });
        }

        public virtual void LogDebug(string infoMsg)
        {
            MessageHandler?.Invoke(new ClientMessage(MessageType.DEBUG)
            {
                Timestamp = DateTime.Now,
                Text = "[" + DateTime.Now.ToString("dd/MM/yy HH:mm:ss") + " DEBUG] " + Name + ": " + infoMsg
            });
        }

        /// <summary>
        /// Contains the work logic of this task.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task Run(CancellationToken cancellationToken)
        {
            return Task.Run(this.Action(), cancellationToken);
        }

        public abstract Action Action();

        public bool CheckRun()
        {
            return LastRunAt == null
                || (Repeat && (DateTime.Now - LastRunAt.Value).TotalMilliseconds >= SleepTime.TotalMilliseconds);
        }

        public bool Stop()
        {
            if (!this.Repeat)
            {
                return false;
            }

            this.Repeat = false;
            return true;
        }
    }

    /// <summary>
    /// Model for simple background tasks.
    /// </summary>
    public abstract class BackgroundTask : BackgroundTaskBase<int>
    {
        public BackgroundTask(string name, bool repeat = false, int sleepTimeMS = 0)
            : base(name, repeat, sleepTimeMS)
        {
        }
    }
}