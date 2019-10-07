using System.Collections.Generic;
using System.Threading.Tasks;
using SpotifyApi.NetCore;

namespace Muse.Controllers.API
{
    public class AudioAnalysisApiController : BaseApiController
    {
        public AudioAnalysisApiController()
            : base()
        {
        }

        public async Task<TrackAudioAnalysis> Get(string Id)
        {
            var accessToken = await GetAccessToken();
            var tracksApi = new TracksApi(this.httpClient, accessToken);
            var tracksAudioAnalysis = await tracksApi.GetTrackAudioAnalysis(Id, accessToken);

            return tracksAudioAnalysis;
        }
    }
}