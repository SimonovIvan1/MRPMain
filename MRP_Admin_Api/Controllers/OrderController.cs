using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MRP_DAL.Repository;
using MRP_DAL;
using MRP_DAL.Helpers;
using ExternalModels.PublicApiDto;

namespace MRP_Admin_Api.Controllers
{
    [ApiController]

    [Route("api/orders")]
    public class OrderController : Controller
    {
        private readonly OrderHelper _orderHelper;

        public OrderController(DbContextOptions<AppDbContext> db)
        {
            _orderHelper = new OrderHelper(db);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _orderHelper.GetAll());

        [HttpPost]
        public async Task<IActionResult> Create(NewOrderDTO newOrder) => Ok(await _orderHelper.CreateOrder(newOrder));
        
        [HttpPost("process-order")]
        public void ProcessOrder(Guid orderId) => _orderHelper.ProcessOrder(orderId);

    }
}
