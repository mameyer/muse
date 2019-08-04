using Microsoft.AspNetCore.Mvc;

namespace Muse.Controllers.API
{
    public class TestAPIController : Controller
    {
        public object[] Get()
        {
            return new object[]
            {
                new 
                {
                    ID = 1,
                    CompanyName = "Test Company",
                    City = "Test City",
                    State = "Test State",
                    Phone = "Test Phone",
                    Fax = "Test Fax"
                }
            };
        }
    }
}