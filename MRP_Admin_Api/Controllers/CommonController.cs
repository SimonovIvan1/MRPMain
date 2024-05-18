using Microsoft.AspNetCore.Mvc;

namespace MRP_Admin_Api.Controllers
{
    [ApiController]
    [Route("api/check-connection")]
    public class CommonController : Controller
    {
        [HttpGet]
        public IActionResult CheckConnection() => Ok("Connection established...");
    }
}
