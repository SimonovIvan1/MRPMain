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

        public async Task<IActionResult> Create(Guid clientId, string address, Guid goodId, int quantity)
        {
            var newOrder = new NewOrderDTO()
            {
                ClientId = clientId,
                Address = address,
                GoodId = goodId,
                Quantity = quantity
            };
            await _orderHelper.CreateOrder(newOrder);
            return Redirect("https://localhost:7201/Order/GetAll");
        }

            public async Task<IActionResult> GetTree(Guid? orderId) 
            => View(await _orderHelper.GetTree(orderId.Value));

    }
}
