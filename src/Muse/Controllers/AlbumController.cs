using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Muse.Controllers
{
    [Authorize]
    public class AlbumController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}