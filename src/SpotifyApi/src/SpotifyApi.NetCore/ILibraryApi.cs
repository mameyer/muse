using System.Threading.Tasks;

namespace SpotifyApi.NetCore
{
    public interface ILibraryApi
    {
        Task<Page<SavedAlbum>> GetAlbums(
            string accessToken = null,
            int? limit = null,
            int offset = 0);
    }
}