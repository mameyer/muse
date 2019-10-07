using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Muse.Models;
using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Objects;
using System.Linq;
using SpotifyApi.NetCore;
using SpotifyApi.NetCore.Models;

namespace Muse.Controllers.API
{ 
    public class TopArtistApiController : BaseApiController
    {
        public TopArtistApiController()
            : base()
        {
        }

        private async Task<IEnumerable<Models.Local.Band>> GetTopArtists(string timeRange, int start, int count)
        {
            List<Models.Local.Band> bands = new List<Models.Local.Band>();

            if (start < 0|| count <= 0)
                return bands;

            TimeRange tr = Enum.Parse<TimeRange>(timeRange);

            var accessToken = await GetAccessToken();
            
            var personalizationApi = new PersonalizationApi(this.httpClient, accessToken);

            var lastfmClient = new LastfmClient("39520abbd99e05205e26166493059ff2", "e60062e594998f609305bf1999c371e2");

            const int stepSize = 20;
            int end = start + count;

            for (int i = start; i < end; i += stepSize)
            {
                var limit = Math.Min(stepSize, end - i);
                var topArtists = await personalizationApi.GetTopArtists(accessToken, stepSize, i, tr);
                
                var artists = topArtists.Items
                    .Select(item => new Models.Local.Band(item))
                    .ToList();

                if (artists.Count() == 0)
                    break;

                for (int j = 0; j < artists.Count(); j++)
                {
                    artists[j].Rank = i + j;

                    var response = await lastfmClient.Artist.GetInfoAsync(artists[j].Name);
                    LastArtist lastArtist = response.Content;
                    if (lastArtist != null)
                        artists[j].Bio = lastArtist.Bio.Summary;
                }
                
                bands.AddRange(artists);
            }

            return bands;
        }

        public async Task<IEnumerable<Models.Local.Band>> Get(string timeRange = "", int start = 0, int count = 20)
        {
            return await GetTopArtists(timeRange, start, count);
        }
    }
}