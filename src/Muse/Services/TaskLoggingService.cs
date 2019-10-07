using Microsoft.AspNetCore.SignalR;
using BackgroundServices.Enums;
using BackgroundServices.Services;

namespace Muse.Services
{
    public class TaskLoggingService : BackgroundServices.Interfaces.ILoggingService
    {
        private readonly IHubContext<Hubs.TaskLoggingHub> taskLoggingHub;

        public TaskLoggingService(IHubContext<Hubs.TaskLoggingHub> taskLoggingHub)
        {
            this.taskLoggingHub = taskLoggingHub;
        }

        public void Log<T>(QueuedHostedService<T> service, LogAction action, object data)
        {
            string taskName = null;
            object taskParameters = null;
            object taskResult = null;
            object result = null;

            if (data is BackgroundServices.Interfaces.IBackgroundTaskBase<T> task)
            {
                taskName = task.Name;
            }

            taskLoggingHub.Clients.Group("TaskLogging").SendAsync("update", new
            {
                queue = service.TaskQueue.GetType().Name,
                action = action.ToString(),
                task = new {
                    name = taskName,
                    parameters = taskParameters,
                    result = taskResult
                },
                result = result
            });
        }
    }
}