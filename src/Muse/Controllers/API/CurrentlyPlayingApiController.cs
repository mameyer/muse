using System;
using System.Threading.Tasks;
using SpotifyAPI.Web;
using Player.Models.DTO;
using AutoMapper;

namespace Muse.Controllers.API
{
    public class CurrentlyPlayingApiController : BaseApiController
    {
        private readonly IMapper _mapper;

        public CurrentlyPlayingApiController(IMapper mapper)
            : base()
        {
            this._mapper = mapper;
        }

        private async Task<CurrentlyPlayingContext> GetCurrentPlaybackInfo()
        {
            var accessToken = await GetAccessToken();
            var spotify = new SpotifyClient(accessToken);
            return await spotify.Player.GetCurrentPlayback();
        }

        public async Task<CurrentlyPlayingDTO> Get()
        {
            try
            {
                var currentPlaybackContext = await GetCurrentPlaybackInfo();
                return this._mapper.Map<CurrentlyPlayingDTO>(currentPlaybackContext);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}