using System.Collections.Generic;
using System.Threading.Tasks;
using SpotifyApi.NetCore;

namespace Muse.Controllers.API
{
    public class FeatureAnalysisApiController : BaseApiController
    {
        public FeatureAnalysisApiController()
            : base()
        {
        }

        public async Task<TrackAudioFeatures> Get(string Id)
        {
            var accessToken = await GetAccessToken();
            var tracksApi = new TracksApi(this.httpClient, accessToken);
            var tracksAudioFeatures = await tracksApi.GetTrackAudioFeatures(Id, accessToken);

            return tracksAudioFeatures;
        }
    }
}