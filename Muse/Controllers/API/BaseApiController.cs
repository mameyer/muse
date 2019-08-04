using FluentSpotifyApi;
using Microsoft.AspNetCore.Mvc;

namespace Muse.Controllers.API
{
    [Route("api/[controller]")] 
    public abstract class BaseApiController : Controller
    {
        protected readonly IFluentSpotifyClient fluentSpotifyClient;

        public BaseApiController(IFluentSpotifyClient fluentSpotifyClient)
        {
            this.fluentSpotifyClient = fluentSpotifyClient;
        }
    }
}