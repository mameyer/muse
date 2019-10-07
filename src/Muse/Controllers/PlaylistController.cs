using Microsoft.AspNetCore.Mvc;

namespace Muse.Controllers
{
    public class PlaylistController : Controller
    {
        public PlaylistController()
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult LocalPlaylist()
        {
            return View();
        }
    }
}