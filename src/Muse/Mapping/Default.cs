using AutoMapper;
using Player.Models.DTO;
using SpotifyAPI.Web;

namespace Muse.Mapping
{
    public class Default : Profile
    {
        public Default()
        {
            CreateMap<Context, ContextDTO>();
            CreateMap<CurrentlyPlayingContext, CurrentlyPlayingDTO>();
            CreateMap<IPlayableItem, PlayableItemDTO>();
        }
    }
}