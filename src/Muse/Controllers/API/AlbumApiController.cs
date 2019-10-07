using System.Collections.Generic;
using System.Threading.Tasks;
using SpotifyApi.NetCore;

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
            var libraryApi = new LibraryApi(this.httpClient, accessToken);
            var page = await libraryApi.GetAlbums(accessToken, limit, offset);
            var albums = page.Items;

            return albums;
        }
    }
}