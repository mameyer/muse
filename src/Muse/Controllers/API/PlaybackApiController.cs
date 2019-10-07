using System.Threading.Tasks;

namespace Muse.Controllers.API
{
    public class PlaybackApiController : BaseApiController
    {
        public PlaybackApiController()
            : base()
        {
        }

        public async Task<object> Get(string deviceId)
        {
            // return await fluentSpotifyClient.Me.Player.Playback().GetAsync();
            return null;
        }
    }
}