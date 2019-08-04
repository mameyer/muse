using Microsoft.AspNetCore.Mvc;

namespace Muse.Controllers.API
{
    public class TestChartAPIController : Controller
    {
        public IActionResult Get()
        {
            return new JsonResult (new object[] { new {
                country = "USA",
                hydro = 59.8,
                oil = 937.6,
                gas = 582,
                coal = 564.3,
                nuclear = 187.9
            } });
        }
    }
}