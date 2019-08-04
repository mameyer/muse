using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentSpotifyApi;
using FluentSpotifyApi.Model;
using Muse.Models;

namespace Muse.Controllers.API
{
    public class TopTrackApiController : BaseApiController
    {
        public TopTrackApiController(IFluentSpotifyClient fluentSpotifyClient)
            : base(fluentSpotifyClient)
        {
        }

        private FluentSpotifyApi.Builder.Me.Personalization.Top.TimeRange TimeRangeFromString(string timeRangeStr)
        {
            FluentSpotifyApi.Builder.Me.Personalization.Top.TimeRange timeRange;
            if (!Enum.TryParse(timeRangeStr, true, out timeRange) && Enum.IsDefined(typeof(FluentSpotifyApi.Builder.Me.Personalization.Top.TimeRange), timeRange))
            {
                timeRange = FluentSpotifyApi.Builder.Me.Personalization.Top.TimeRange.ShortTerm;
            }
            return timeRange;
        }

        private async Task<IEnumerable<Song>> GetTopTracks(string timeRange, int start, int count)
        {
            List<Song> songs = new List<Song>();

            if (start < 0
                || count <= 0)
            {
                return songs;
            }

            FluentSpotifyApi.Builder.Me.Personalization.Top.TimeRange range = TimeRangeFromString(timeRange);

            int end = start + count;
            const int stepSize = 20;

            for (int i = start; i < end; i += stepSize)
            {
                var limit = Math.Min(stepSize, end - i);
                var topTracks = (await this.fluentSpotifyClient.Me.Personalization.TopTracks.GetAsync(limit: limit, offset: i, timeRange: range))
                    .Items
                    .Select(item => new Song
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Artists = item.Artists.Select(artist => artist.Name),
                        Album = item.Album.Name,
                        Uri = item.Uri.Split(':').Last()
                    }).ToList();

                if (topTracks.Count() == 0)
                {
                    break;
                }

                for (int j = 0; j < topTracks.Count(); j++)
                {
                    topTracks[j].Rank = i + j;
                    topTracks[j].Popularities = new List<int>();
                    topTracks[j].Genres = new List<string>();

                    foreach (var artist in topTracks[j].Artists)
                    {
                        var artistForTrack = (await this.fluentSpotifyClient.Search.Artists.Matching(query => query.Artist.Contains(artist)).GetAsync());
                        if (artistForTrack != null && artistForTrack.Page != null && artistForTrack.Page.Items.Count() > 0)
                        {
                            FullArtist fullArtist = artistForTrack.Page.Items.ElementAt(0);

                            topTracks[j].Popularities.Add(fullArtist.Popularity);
                            topTracks[j].Genres.AddRange(fullArtist.Genres.ToList());
                        }
                    }
                }
                
                songs.AddRange(topTracks);
            }

            int rank = 0;
            foreach (Song song in songs)
            {
                song.Rank = rank;
                rank++;
            }

            return songs;
        }

        public async Task<IEnumerable<Song>> Get(string timeRange = "", int start = 0, int count = 20)
        {
            return await GetTopTracks(timeRange, start, count);
        }
    }
}