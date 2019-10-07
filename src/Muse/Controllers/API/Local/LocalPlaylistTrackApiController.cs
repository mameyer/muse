using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Muse.Controllers.Local.API
{
    public class LocalPlaylistTrackApiController : Controller
    {
        private readonly MuseContext _context;

        public LocalPlaylistTrackApiController(MuseContext context)
        {
            this._context = context;
        }

        public async Task<IEnumerable<Muse.Models.Local.PlaylistTrack>> Get(long? playlistId)
        {
            return await this._context.PlaylistTracks
                .Include(e => e.Track)
                    .ThenInclude(e => e.Song)
                .Include(e => e.Track)
                    .ThenInclude(e => e.Band)
                .Include(e => e.Track)
                    .ThenInclude(e => e.Album)
                .Where(e => e.PlaylistId == playlistId)
                .ToArrayAsync();
        }
    }
}