using System.Collections.Generic;
using System.Threading.Tasks;
using FluentSpotifyApi;
using Microsoft.AspNetCore.Mvc;

namespace Muse.Controllers.API
{
    public class AlbumApiController : BaseApiController
    {
        public AlbumApiController(IFluentSpotifyClient fluentSpotifyClient)
            : base(fluentSpotifyClient)
        {
        }

        public async Task<IEnumerable<FluentSpotifyApi.Model.SavedAlbum>> Get(int limit = 20, int offset = 0)
        {
            return (await this.fluentSpotifyClient.Me.Library.Albums().GetAsync(limit: limit, offset: offset))
                .Items;
        }
    }
}