using Microsoft.AspNetCore.Mvc;

namespace Muse.Controllers
{
    public class DeviceController : Controller
    {
        public IActionResult Index() {
            return View();
        }
    }
}