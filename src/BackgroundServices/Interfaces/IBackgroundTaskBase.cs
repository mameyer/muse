using BackgroundServices.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BackgroundServices.Interfaces
{
    public interface IBackgroundTaskBase<T>
    {
        T In { get; set; }

        T Out { get; }

        Guid? Id { get; set; }

        string Name { get; }

        bool Repeat { get; }

        TimeSpan SleepTime { get; }

        DateTime? LastRunAt { get; set; }

        Func<ClientMessage, bool> MessageHandler { get; set; }

        object Parameters { get; set; }

        object Result { get; set; }

        DateTime Timestamp { get; set; }

        void UpdateProgressInfo(int currentWritten, int totalRecords);

        void LogError(string errorMsg);

        void LogInfo(string infoMsg);

        /// <summary>
        /// Contains the work logic of this task.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Run(CancellationToken cancellationToken);

        bool CheckRun();

        bool Stop();
    }
}