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

        public async Task<TrackAudioFeatures> Get(string Id)
        {
            var accessToken = await GetAccessToken();
            var spotify = new SpotifyClient(accessToken);
            var tracksAudioFeatures = await spotify.Tracks.GetAudioFeatures(Id);
            return tracksAudioFeatures;
        }
    }
}