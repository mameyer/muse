using Microsoft.AspNetCore.Mvc;

namespace Muse.Controllers
{
    public class TopArtistController : Controller
    {
        public IActionResult Index() {
            return View();
        }
    }
}