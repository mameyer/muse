using System;
using System.Threading.Tasks;
using SpotifyAPI.Web;

namespace Muse.Controllers.API
{
    public class CurrentlyPlayingApiController : BaseApiController
    {
        public CurrentlyPlayingApiController()
            : base()
        {
        }

        private async Task<CurrentlyPlayingContext> GetCurrentPlaybackInfo()
        {
            var accessToken = await GetAccessToken();
            var spotify = new SpotifyClient(accessToken);
            return await spotify.Player.GetCurrentPlayback();
        }

        public async Task<object> Get()
        {
            try
            {
                var currentPlaybackContext = await GetCurrentPlaybackInfo();
                return currentPlaybackContext;
            }
            catch (Exception ex)
            {
                return new { Error = ex.Message };
            }
        }
    }
}