using System.Collections.Generic;
using System.Threading.Tasks;
using FluentSpotifyApi;
using FluentSpotifyApi.Model;
using Microsoft.AspNetCore.Mvc;

namespace Muse.Controllers.API
{
    public class PlaybackApiController : BaseApiController
    {
        public PlaybackApiController(IFluentSpotifyClient fluentSpotifyClient)
            : base(fluentSpotifyClient)
        {
        }

        public async Task<PlayingContext> Get(string deviceId)
        {
            return await fluentSpotifyClient.Me.Player.Playback().GetAsync();
        }
    }
}