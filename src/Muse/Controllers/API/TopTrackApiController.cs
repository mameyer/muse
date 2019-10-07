using System.Collections.Generic;
using System.Threading.Tasks;
using Muse.Models;
using SpotifyApi.NetCore;
using System;
using System.Linq;
using SpotifyApi.NetCore.Models;

namespace Muse.Controllers.API
{
    public class TopTrackApiController : BaseApiController
    {
        public TopTrackApiController()
            : base()
        {
        }

        private async Task<IEnumerable<Models.Local.Song>> GetTopTracks(string timeRange, int start, int count)
        {
            List<Models.Local.Song> songs = new List<Models.Local.Song>();

            if (start < 0|| count <= 0)
                return songs;

            TimeRange tr = Enum.Parse<TimeRange>(timeRange);

            var accessToken = await GetAccessToken();
            
            var personalizationApi = new PersonalizationApi(this.httpClient, accessToken);
            var artistsApi = new ArtistsApi(this.httpClient, accessToken);

            int end = start + count;
            const int stepSize = 20;

            for (int i = start; i < end; i += stepSize)
            {
                var limit = Math.Min(stepSize, end - i);
                var topTracks = await personalizationApi.GetTopTracks(accessToken, stepSize, i, tr);
                var tracks = topTracks.Items
                    .Select(item => new Models.Local.Song(item))
                    .ToList();

                if (tracks.Count() == 0) break;

                for (int j = 0; j < tracks.Count(); j++)
                {
                    tracks[j].Rank = i + j;
                    tracks[j].Popularities = new List<int>();
                    tracks[j].Genres = new List<string>();

                    var artistsForTrack = await artistsApi.GetArtists(tracks[j].Artists.Select(e => e.Id).ToArray(), accessToken);

                    tracks[j].Popularities = artistsForTrack.Select(e => e.Popularity).ToList();
                    tracks[j].Genres = artistsForTrack.SelectMany(i => i.Genres).ToList();
                }
                
                songs.AddRange(tracks);
            }

            int rank = 0;
            foreach (Models.Local.Song song in songs)
            {
                song.Rank = rank;
                rank++;
            }

            return songs;
        }

        public async Task<IEnumerable<Models.Local.Song>> Get(string timeRange = "", int start = 0, int count = 20)
        {
            return await GetTopTracks(timeRange, start, count);
        }
    }
}