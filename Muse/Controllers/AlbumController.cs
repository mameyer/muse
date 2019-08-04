using Microsoft.AspNetCore.Mvc;

namespace Muse.Controllers
{
    public class AlbumController : Controller
    {
        public IActionResult Index() {
            return View();
        }
    }
}