using System.Net.Http;
using System.Threading.Tasks;
using SpotifyApi.NetCore.Authorization;

namespace SpotifyApi.NetCore
{
    /// <summary>
    /// Player API endpoints.
    /// </summary>
    /// <remarks> https://developer.spotify.com/documentation/web-api/reference/player/ </remarks>
    public class LibraryApi : SpotifyWebApi, ILibraryApi
    {
        #region Constructors

        /// <summary>
        /// This constructor accepts a Spotify access token that will be used for all calls to the API 
        /// (except when an accessToken is provided using the optional `accessToken` parameter on each method).
        /// </summary>
        /// <param name="httpClient">An instance of <see cref="HttpClient"/></param>
        /// <param name="accessToken">A valid access token from the Spotify Accounts service</param>
        public LibraryApi(HttpClient httpClient, string accessToken) : base(httpClient, accessToken) { }

        public LibraryApi(HttpClient httpClient, IAccessTokenProvider accessTokenProvider)
            : base(httpClient, accessTokenProvider) { }

        #endregion

        public async Task<Page<SavedAlbum>> GetAlbums(string accessToken = null, int? limit = null, int offset = 0)
        {
            string url = $"{BaseUrl}/me/albums?";

            if (limit.HasValue) url += $"limit={limit.Value}&offset={offset}";

            return await GetModel<Page<SavedAlbum>>(url, accessToken);
        }
    }
}