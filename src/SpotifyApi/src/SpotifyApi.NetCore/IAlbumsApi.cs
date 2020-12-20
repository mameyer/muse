using System.Threading.Tasks;

namespace SpotifyApi.NetCore
{
    /// <summary>
    /// Defines endpoints for retrieving information about one or more albums from the Spotify catalog.
    /// </summary>
    public interface IAlbumsApi
    {
        #region GetAlbum

        /// <summary>
        /// Get Spotify catalog information for a single album.
        /// </summary>
        /// <param name="albumId">The Spotify ID for the album.</param>
        /// <param name="market">Optional. An ISO 3166-1 alpha-2 country code or the string `from_token` 
        /// (See <see cref="SpotifyCountryCodes"/>). Provide this parameter if you want to apply Track Relinking.</param>
        /// <param name="accessToken">Optional. A valid access token from the Spotify Accounts service.</param>
        /// <returns>A Task that, once successfully completed, returns a full <see cref="Album"/> object.</returns>
        Task<Album> GetAlbum(string albumId, string market = null, string accessToken = null);

        /// <summary>
        /// Get Spotify catalog information for a single album.
        /// </summary>
        /// <param name="albumId">The Spotify ID for the album.</param>
        /// <param name="market">Optional. An ISO 3166-1 alpha-2 country code or the string `from_token` 
        /// (See <see cref="SpotifyCountryCodes"/>). Provide this parameter if you want to apply Track Relinking.</param>
        /// <param name="accessToken">Optional. A valid access token from the Spotify Accounts service.</param>
        /// <typeparam name="T">Optionally provide your own type to deserialise Spotify's response to.</typeparam>
        /// <returns>A Task that, once successfully completed, returns a Model of T.</returns>        
        Task<T> GetAlbum<T>(string albumId, string market = null, string accessToken = null);

        #endregion

        #region GetAlbumTracks

        /// <summary>
        /// Get Spotify catalog information about an album’s tracks. Optional parameters can be used to limit the number of tracks returned.
        /// </summary>
        /// <param name="albumId">The Spotify ID for the album.</param>
        /// <param name="limit">Optional. The maximum number of tracks to return. Default: 20. Minimum: 1. Maximum: 50.</param>
        /// <param name="offset">Optional. The index of the first track to return. Default: 0 (the first 
        /// object). Use with limit to get the next set of tracks.</param>
        /// <param name="market">Optional. An ISO 3166-1 alpha-2 country code or the string `from_token` 
        /// (See <see cref="SpotifyCountryCodes"/>). Provide this parameter if you want to apply Track Relinking.</param>
        /// <param name="accessToken">Optional. A valid access token from the Spotify Accounts service.</param>
        /// <returns>A Task that, once successfully completed, returns a full <see cref="Album"/> object.</returns>
        /// <remarks>https://developer.spotify.com/documentation/web-api/reference/albums/get-albums-tracks/</remarks>
        Task<Album> GetAlbumTracks(
            string albumId,
            int? limit = null,
            int offset = 0,
            string market = null,
            string accessToken = null);

        /// <summary>
        /// Get Spotify catalog information about an album’s tracks. Optional parameters can be used to limit the number of tracks returned.
        /// </summary>
        /// <param name="albumId">The Spotify ID for the album.</param>
        /// <param name="limit">Optional. The maximum number of tracks to return. Default: 20. Minimum: 1. Maximum: 50.</param>
        /// <param name="offset">Optional. The index of the first track to return. Default: 0 (the first 
        /// object). Use with limit to get the next set of tracks.</param>
        /// <param name="market">Optional. An ISO 3166-1 alpha-2 country code or the string `from_token` 
        /// (See <see cref="SpotifyCountryCodes"/>). Provide this parameter if you want to apply Track Relinking.</param>
        /// <param name="accessToken">Optional. A valid access token from the Spotify Accounts service.</param>
        /// <typeparam name="T">Optionally provide your own type to deserialise Spotify's response to.</typeparam>
        /// <returns>A Task that, once successfully completed, returns a Model of T.</returns>
        /// <remarks>https://developer.spotify.com/documentation/web-api/reference/albums/get-albums-tracks/</remarks>
        Task<T> GetAlbumTracks<T>(
            string albumId,
            int? limit = null,
            int offset = 0,
            string market = null,
            string accessToken = null);

        #endregion

        #region GetAlbums

        /// <summary>
        /// Get Spotify catalog information for multiple albums identified by their Spotify IDs.
        /// </summary>
        /// <param name="albumIds">An array of the Spotify IDs for the albums. Maximum: 20 IDs.</param>
        /// <param name="market">Optional. An ISO 3166-1 alpha-2 country code or the string `from_token` 
        /// (See <see cref="SpotifyCountryCodes"/>). Provide this parameter if you want to apply Track Relinking.</param>
        /// <param name="accessToken">Optional. A valid access token from the Spotify Accounts service.</param>
        /// <returns>A Task that, once successfully completed, returns an array of full <see cref="Album"/> objects.</returns>
        Task<Album[]> GetAlbums(string[] albumIds, string market = null, string accessToken = null);

        /// <summary>
        /// Get Spotify catalog information for multiple albums identified by their Spotify IDs.
        /// </summary>
        /// <param name="albumIds">An array of the Spotify IDs for the albums. Maximum: 20 IDs.</param>
        /// <param name="market">Optional. An ISO 3166-1 alpha-2 country code or the string `from_token` 
        /// (See <see cref="SpotifyCountryCodes"/>). Provide this parameter if you want to apply Track Relinking.</param>
        /// <param name="accessToken">Optional. A valid access token from the Spotify Accounts service.</param>
        /// <typeparam name="T">Optionally provide your own type to deserialise Spotify's response to.</typeparam>
        /// <returns>A Task that, once successfully completed, returns a Model of T.</returns>
        Task<T> GetAlbums<T>(string[] albumIds, string market = null, string accessToken = null);

        #endregion

        #region SearchAlbums

        /// <summary>
        /// Get Spotify Catalog information about albums that match a keyword string.
        /// </summary>
        /// <param name="query">Search query keywords and optional field filters and operators. See
        /// https://developer.spotify.com/documentation/web-api/reference/search/search/#writing-a-query---guidelines</param>
        /// <param name="market">Optional. Choose a <see cref="SpotifyCountryCodes"/>. If a country code
        /// is specified, only tracks with content that is playable in that market is returned. </param>
        /// <param name="limit">Optional. Maximum number of results to return. Default: 20, Minimum: 1,
        /// Maximum: 50.</param>
        /// <param name="offset">Optional. The index of the first result to return. Default: 0 (the
        /// first result). Maximum offset (including limit): 10,000. Use with limit to get the next
        /// page of search results.</param>
        /// <param name="accessToken">Optional. A valid access token from the Spotify Accounts service.</param>
        /// <returns>Task of <see cref="SearchResult" /></returns>
        Task<SearchResult> SearchAlbums(
            string query,
            int? limit = null,
            int offset = 0,
            string market = null,
            string accessToken = null);

        #endregion

    }
}