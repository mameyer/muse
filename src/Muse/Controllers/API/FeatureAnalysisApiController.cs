using System.Threading.Tasks;
using SpotifyAPI.Web;

namespace Muse.Controllers.API
{
    public class FeatureAnalysisApiController : BaseApiController
    {
        public FeatureAnalysisApiController()
            : base()
        {
        }

        public async Task<TrackAudioFeatures> Get(string id)
        {
            var accessToken = await GetAccessToken();
            var spotify = new SpotifyClient(accessToken);
            var tracksAudioFeatures = await spotify.Tracks.GetAudioFeatures(id);
            return tracksAudioFeatures;
        }
    }
}