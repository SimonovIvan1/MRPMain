using ExternalModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MRP_DAL.Repository;
using MRP_DAL;

namespace MRP_Admin_Api.Controllers
{
    [ApiController]

    [Route("api/admin/suppliers")]
    public class SupplierController : Controller
    {
        private readonly SupplierRepository _repository;

        public SupplierController(DbContextOptions<AppDbContext> db)
        {
            _repository = new SupplierRepository(db);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var clients = await _repository.GetAll();
                if (clients == null) return NotFound("Поставщики пропали или их пока нет");
                return Ok(clients);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{supplierId}")]
        public async Task<IActionResult> Get(Guid supplierId)
        {
            try
            {
                var client = await _repository.Get(supplierId);
                if (client == null) return NotFound("Поставщик не найден");
                return Ok(client);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task Delete(Guid id) => await _repository.Delete(id);

        [HttpPost]
        public async Task<IActionResult> Create(SupplierDto newSupplier)
        {
            try
            {
                await _repository.Create(newSupplier);
                return Ok("Поставщик создан");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
