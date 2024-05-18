using Microsoft.AspNetCore.Mvc;

namespace MRP_Public_API.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
