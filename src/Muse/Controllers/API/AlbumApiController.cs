using System.Threading.Tasks;
using SpotifyAPI.Web;

namespace Muse.Controllers.API
{
    public class AlbumApiController : BaseApiController
    {
        public AlbumApiController()
            : base()
        {
        }

        public async Task<SavedAlbum[]> Get(int limit = 20, int offset = 0)
        {
            var accessToken = await GetAccessToken();
            var spotify = new SpotifyClient(accessToken);
            var albums = await spotify.Library.GetAlbums();
            return albums?.Items?.ToArray();
        }
    }
}