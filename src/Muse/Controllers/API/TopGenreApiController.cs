using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Muse.Models;
using SpotifyApi.NetCore;
using SpotifyApi.NetCore.Models;

namespace Muse.Controllers.API
{
    public class TopGenreApiController : BaseApiController
    {
        public TopGenreApiController()
            : base()
        {
        }

        private async Task<IEnumerable<Models.Local.Genre>> GetTopGenres(string timeRange, int start, int count)
        {
            Dictionary<string, int> genreStat = new Dictionary<string, int>();

            if (start < 0|| count <= 0)
                return new Models.Local.Genre[] { };

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
                var artists = topTracks.Items
                    .SelectMany(item => item.Artists)
                    .ToArray();

                if (artists.Length == 0) break;

                var fullArtists = await artistsApi.GetArtists(artists.Select(e => e.Id).ToArray(), accessToken);
                if (fullArtists.Length == 0) continue;

                var genres = fullArtists.SelectMany(e => e.Genres);

                foreach (var genre in genres)
                {
                    if (!genreStat.ContainsKey(genre)) genreStat[genre] = 0;
                    genreStat[genre] += 1;
                }
            }

            var orderedGenres = genreStat.ToList().OrderByDescending(e => e.Value).ToArray();
            var genresWithRank = new Models.Local.Genre[orderedGenres.Length];

            for (int i = 0; i < genresWithRank.Length; i++)
            {
                genresWithRank[i] = new Models.Local.Genre
                {
                    Name = orderedGenres[i].Key,
                    Rank = i
                };
            }

            return genresWithRank;
        }

        public async Task<IEnumerable<Models.Local.Genre>> Get(string timeRange = "", int start = 0, int count = 20)
        {
            return await GetTopGenres(timeRange, start, count);
        }
    }
}