using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Muse.Controllers.Local.API
{
    public class LocalPlaylistApiController : Controller
    {
        private readonly MuseContext _context;

        public LocalPlaylistApiController(MuseContext context)
        {
            this._context = context;
        }

        public async Task<IEnumerable<Muse.Models.Local.Playlist>> Get()
        {
            return await this._context.Playlists
                .ToArrayAsync();
        }
    }
}