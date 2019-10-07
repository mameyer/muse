using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Muse.Models;
using SpotifyApi.NetCore;

namespace Muse.Controllers.API
{
    public class PlaylistTrackApiController : BaseApiController
    {
        public PlaylistTrackApiController()
            : base()
        {
        }

        private async Task<IEnumerable<SpotifyApi.NetCore.PlaylistTrack>> GetPlaylistTracks(string playlistId, int start, int count)
        {
            List<SpotifyApi.NetCore.PlaylistTrack> playlistTracks = new List<SpotifyApi.NetCore.PlaylistTrack>();

            if (start < 0|| count <= 0)
                return playlistTracks;

            var accessToken = await GetAccessToken();
            
            var playlistsApi = new PlaylistsApi(this.httpClient, accessToken);

            const int stepSize = 20;
            int end = start + count;

            for (int i = start; i < end; i += stepSize)
            {
                var limit = Math.Min(stepSize, end - i);
                var tracks = await playlistsApi.GetTracks(playlistId, accessToken, limit: stepSize, offset: i);
                playlistTracks.AddRange(tracks.Items);
            }

            return playlistTracks;
        }

        public async Task<IEnumerable<SpotifyApi.NetCore.PlaylistTrack>> Get(string playlistId, int start = 0, int count = 20)
        {
            return await GetPlaylistTracks(playlistId, start, count);
        }
    }
}