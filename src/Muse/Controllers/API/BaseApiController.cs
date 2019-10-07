using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Muse.Controllers.API
{
    [Route("api/[controller]")] 
    [Authorize]
    public abstract class BaseApiController : Controller
    {
        protected readonly HttpClient httpClient;

        public BaseApiController()
        {
            httpClient = new HttpClient();
        }

        protected async Task<string> GetAccessToken() {
            return await HttpContext.GetTokenAsync("Spotify", "access_token");
        }
    }
}