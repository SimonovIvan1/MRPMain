using ExternalModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MRP_DAL.Repository;
using MRP_DAL;

namespace MRP_Admin_Api.Controllers
{
    [ApiController]

    [Route("api/admin/order-statuses")]
    public class OrderStatusController : Controller
    {
        private readonly OrderStatusRepository _repository;

        public OrderStatusController(DbContextOptions<AppDbContext> db)
        {
            _repository = new OrderStatusRepository(db);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var clients = await _repository.GetAll();
                if (clients == null) return NotFound("Статусы заказов пропали или их пока нет");
                return Ok(clients);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{orderStatusId}")]
        public async Task<IActionResult> Get(int orderStatusId)
        {
            try
            {
                var client = await _repository.Get(orderStatusId);
                if (client == null) return NotFound("Статус заказа не найден");
                return Ok(client);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task Delete(int id) => await _repository.Delete(id);

        [HttpPost]
        public async Task<IActionResult> Create(OrderStatusDto newOrderStatus)
        {
            try
            {
                await _repository.Create(newOrderStatus);
                return Ok("Создан новый статус заказа");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
