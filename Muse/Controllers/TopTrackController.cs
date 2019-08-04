using Microsoft.AspNetCore.Mvc;

namespace Muse.Controllers
{
    public class TopTrackController : Controller
    {
        public IActionResult Index() {
            return View();
        }
    }
}