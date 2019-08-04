using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentSpotifyApi;
using Microsoft.AspNetCore.Mvc;
using Muse.Models;

namespace Muse.Controllers
{
    public class RecommendationController : Controller
    {
        private readonly IFluentSpotifyClient fluentSpotifyClient;

        public RecommendationController(IFluentSpotifyClient fluentSpotifyClient)
        {
            this.fluentSpotifyClient = fluentSpotifyClient;
        }

        public async Task<IActionResult> Index()
        {
            return View(new RecommendationOptions());
        }
    }
}