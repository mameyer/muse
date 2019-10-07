using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Muse.Hubs
{
    public class TaskLoggingHub : Hub
    {
        public async Task AssociateJob(string jobId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, jobId);
        }
    }
}