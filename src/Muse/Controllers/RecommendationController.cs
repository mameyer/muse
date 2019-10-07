using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Muse.Models;

namespace Muse.Controllers
{
    public class RecommendationController : Controller
    {
        public RecommendationController()
        {
        }

        public async Task<IActionResult> Index()
        {
            return View(new Models.RecommendationOptions());
        }
    }
}