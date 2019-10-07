using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Muse.Models;
using Localization.SqlLocalizer.DbStringLocalizer;

namespace Muse.Controllers
{
    public class HomeController : Controller
    {
        private readonly LocalizationModelContext context;

        public HomeController(LocalizationModelContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
