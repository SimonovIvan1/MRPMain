using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MRP_DAL.Repository;
using MRP_DAL;
using MRP_DAL.Helpers;
using ExternalModels.PublicApiDto;
using ExternalModels.Dto;

namespace MRP_Admin_Api.Controllers
{
    public class OrderController : Controller
    {
        private readonly OrderHelper _orderHelper;

        public OrderController(DbContextOptions<AppDbContext> db)
        {
            _orderHelper = new OrderHelper(db);
        }

        public async Task<IActionResult> GetAll() => View(await _orderHelper.GetAll());

        public async Task<IActionResult> Create(NewOrderDTO newOrder) => View(await _orderHelper.CreateOrder(newOrder));
        
        public async Task<IActionResult> GetTree(Guid? orderId) 
            => View(await _orderHelper.GetTree(orderId.Value));

    }
}
