using Microsoft.AspNetCore.Mvc;

namespace Muse.Controllers
{
    public class PlayerController : Controller
    {
        public IActionResult Index() {
            return View();
        }
    }
}