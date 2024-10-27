using System.Threading.Tasks;
using Muse.Models;
using SpotifyAPI.Web;

namespace Muse.Controllers.API
{
    public class PlaylistTrackApiController : BaseApiController
    {
        public PlaylistTrackApiController()
            : base()
        {
        }

        private async Task<PlaylistTrack<IPlayableItem>[]> GetPlaylistTracks(string playlistId, int start, int count)
        {
            var accessToken = await GetAccessToken();
            var spotify = new SpotifyClient(accessToken);

            var playlist = await spotify.Playlists.Get(playlistId);

            return playlist?.Tracks?.Items?.ToArray();
        }

        public async Task<PlaylistTrack<IPlayableItem>[]> Get(string playlistId, int start = 0, int count = 20)
        {
            return await GetPlaylistTracks(playlistId, start, count);
        }
    }
}