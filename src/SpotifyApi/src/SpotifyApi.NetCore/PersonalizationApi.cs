using System.Net.Http;
using System.Threading.Tasks;
using SpotifyApi.NetCore.Authorization;
using SpotifyApi.NetCore.Extensions;
using SpotifyApi.NetCore.Models;

namespace SpotifyApi.NetCore
{
    /// <summary>
    /// Player API endpoints.
    /// </summary>
    /// <remarks> https://developer.spotify.com/documentation/web-api/reference/player/ </remarks>
    public class PersonalizationApi : SpotifyWebApi, IPersonalizationApi
    {
        #region Constructors

        /// <summary>
        /// This constructor accepts a Spotify access token that will be used for all calls to the API 
        /// (except when an accessToken is provided using the optional `accessToken` parameter on each method).
        /// </summary>
        /// <param name="httpClient">An instance of <see cref="HttpClient"/></param>
        /// <param name="accessToken">A valid access token from the Spotify Accounts service</param>
        public PersonalizationApi(HttpClient httpClient, string accessToken) : base(httpClient, accessToken) { }

        public PersonalizationApi(HttpClient httpClient, IAccessTokenProvider accessTokenProvider)
            : base(httpClient, accessTokenProvider) { }

        #endregion

        public async Task<Page<Track>> GetTopTracks(string accessToken = null, int? limit = null, int offset = 0, TimeRange timeRange = TimeRange.LongTerm)
        {
            string url = $"{BaseUrl}/me/top/tracks?";
            if (limit.HasValue) url += $"limit={limit.Value}&offset={offset}&";
            url += "time_range=" + timeRange.GetDescription();
            return await GetModel<Page<Track>>(url, accessToken);
        }

        public async Task<Page<Artist>> GetTopArtists(string accessToken = null, int? limit = null, int offset = 0, TimeRange timeRange = TimeRange.LongTerm)
        {
            string url = $"{BaseUrl}/me/top/artists?";
            if (limit.HasValue) url += $"limit={limit.Value}&offset={offset}&";
            url += "time_range=" + timeRange.GetDescription();
            return await GetModel<Page<Artist>>(url, accessToken);
        }
    }
}