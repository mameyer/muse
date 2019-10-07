using BackgroundServices.Enums;
using BackgroundServices.Services;

namespace BackgroundServices.Interfaces
{
    public interface ILoggingService
    {
        void Log<T>(QueuedHostedService<T> service, LogAction action, object data = null);
    }
}