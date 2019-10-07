using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpotifyApi.NetCore;

namespace Muse.Controllers.API
{
    public class CurrentlyPlayingApiController : BaseApiController
    {
        public CurrentlyPlayingApiController()
            : base()
        {
        }

        private async Task<CurrentPlaybackContext> GetCurrentPlaybackInfo()
        {
            var accessToken = await GetAccessToken();
            var playerApi = new PlayerApi(this.httpClient, accessToken);
            return await playerApi.GetCurrentPlaybackInfo(accessToken);
        }

        public object Get()
        {
            try
            {
                var currentPlaybackContext = GetCurrentPlaybackInfo().Result;
                return currentPlaybackContext;
            }
            catch (Exception ex)
            {
                return new { Error = ex.Message };
            }
        }
    }
}