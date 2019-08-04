using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentSpotifyApi;
using Microsoft.AspNetCore.Mvc;
using Muse.Models;

namespace Muse.Controllers.API
{
    public class TopGenreApiController : BaseApiController
    {
        public TopGenreApiController(IFluentSpotifyClient fluentSpotifyClient)
            : base(fluentSpotifyClient)
        {
        }

        public async Task<IEnumerable<Genre>> Get(int limit = 20, int offset = 0)
        {
            List<string[]> genres = (await this.fluentSpotifyClient.Me.Personalization.TopArtists.GetAsync(limit: limit)).Items.Select(artist => artist.Genres).ToList();
            List<Genre> genreList = new List<Genre>();
            foreach (var genreGroup in genres)
            {
                genreList.AddRange(genreGroup.Select(e => new Genre
                {
                    Name = e
                }).ToList());
            }

            return genreList;
        }
    }
}