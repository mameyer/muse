using System.Threading.Tasks;
using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Objects;
using Microsoft.AspNetCore.Mvc;

namespace Muse.Controllers.API
{
    public class LastFMApiController : Controller
    {
        private readonly LastfmClient lastfmClient;
        
        public LastFMApiController()
        {
            this.lastfmClient = new LastfmClient("39520abbd99e05205e26166493059ff2", "e60062e594998f609305bf1999c371e2");
        }

        public async Task<object> Get(string track, string artist)
        {
            var responseArtist = await lastfmClient.Artist.GetInfoAsync(artist);
            LastArtist lastArtist = responseArtist.Content;

            var responeTrack = await lastfmClient.Track.GetInfoAsync(track, artist);
            LastTrack lastTrack = responeTrack.Content;

            return new
            {
                Artist = lastArtist,
                Track = lastTrack
            };
        }
    }
}