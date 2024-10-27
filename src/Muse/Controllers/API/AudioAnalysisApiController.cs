using System.Threading.Tasks;
using SpotifyAPI.Web;

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
            var spotify = new SpotifyClient(accessToken);
            var tracksAudioAnalysis = await spotify.Tracks.GetAudioAnalysis(Id);
            return tracksAudioAnalysis;
        }
    }
}