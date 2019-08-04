using System.Threading;
using System.Threading.Tasks;

namespace BackgroundServices.Models
{
    /// <summary>
    /// Model for simple background tasks.
    /// </summary>
    public abstract class BackgroundTask
    {
        public BackgroundTask(string name)
        {
            this.Name = name;
        }

        public string Name { get; }

        /// <summary>
        /// Contains the work logic of this task.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public abstract Task Run(CancellationToken cancellationToken);
    }
}