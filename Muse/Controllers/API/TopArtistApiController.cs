using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentSpotifyApi;
using Microsoft.AspNetCore.Mvc;
using Muse.Models;
using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Objects;
using System.Linq;

namespace Muse.Controllers.API
{ 
    public class TopArtistApiController : BaseApiController
    {
        public TopArtistApiController(IFluentSpotifyClient fluentSpotifyClient)
            : base(fluentSpotifyClient)
        {
        }

        private FluentSpotifyApi.Builder.Me.Personalization.Top.TimeRange TimeRangeFromString(string timeRangeStr)
        {
            FluentSpotifyApi.Builder.Me.Personalization.Top.TimeRange timeRange;
            if (!Enum.TryParse(timeRangeStr, true, out timeRange)
                || !Enum.IsDefined(typeof(FluentSpotifyApi.Builder.Me.Personalization.Top.TimeRange), timeRange))
            {
                timeRange = FluentSpotifyApi.Builder.Me.Personalization.Top.TimeRange.ShortTerm;
            }
            return timeRange;
        }

        private async Task<IEnumerable<Band>> GetTopArtists(string timeRange, int start, int count)
        {
            List<Band> bands = new List<Band>();

            if (start < 0
                || count <= 0)
            {
                return bands;
            }

            FluentSpotifyApi.Builder.Me.Personalization.Top.TimeRange range = TimeRangeFromString(timeRange);
            var lastfmClient = new LastfmClient("39520abbd99e05205e26166493059ff2", "e60062e594998f609305bf1999c371e2");

            const int stepSize = 20;
            int end = start + count;

            for (int i = start; i < end; i += stepSize)
            {
                var limit = Math.Min(stepSize, end - i);
                var topArtists = (await this.fluentSpotifyClient.Me.Personalization.TopArtists.GetAsync(limit: limit, offset: i, timeRange: range))
                    .Items
                    .Select(item => new Band
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Genres = item.Genres,
                        Popularity = item.Popularity,
                        ImageUri = item.Images.First()?.Url
                    }).ToList();

                if (topArtists.Count() == 0)
                {
                    break;
                }

                for (int j = 0; j < topArtists.Count(); j++)
                {
                    topArtists[j].Rank = i + j;

                    var response = await lastfmClient.Artist.GetInfoAsync(topArtists[j].Name);
                    LastArtist artist = response.Content;
                    if (artist != null)
                    {
                        topArtists[j].Bio = artist.Bio.Summary;
                    }
                }
                
                bands.AddRange(topArtists);
            }

            return bands;
        }

        public async Task<IEnumerable<Band>> Get(string timeRange = "", int start = 0, int count = 20)
        {
            return await GetTopArtists(timeRange, start, count);
        }
    }
}