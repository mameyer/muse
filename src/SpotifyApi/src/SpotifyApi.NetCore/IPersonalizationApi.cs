using System.Threading.Tasks;
using SpotifyApi.NetCore.Models;

namespace SpotifyApi.NetCore
{
    public interface IPersonalizationApi
    {
        Task<Page<Track>> GetTopTracks(
            string accessToken = null,
            int? limit = null,
            int offset = 0,
            TimeRange timeRange = TimeRange.LongTerm);

        Task<Page<Artist>> GetTopArtists(
            string accessToken = null,
            int? limit = null,
            int offset = 0,
            TimeRange timeRange = TimeRange.LongTerm);
    }
}