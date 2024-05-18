using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MRP_DAL.Repository;
using MRP_DAL;
using MRP_DAL.Helpers;
using ExternalModels;
using ExternalModels.Dto;

namespace MRP_Admin_Api.Controllers
{
    [ApiController]

    [Route("api/admin/invoices")]
    public class InvoiceController : Controller
    {
        private readonly InvoiceHelper _helper;
        private readonly InvoiceRepository _repository;

        public InvoiceController(DbContextOptions<AppDbContext> db)
        {
            _helper = new InvoiceHelper(db);
            _repository = new InvoiceRepository(db);
        }

        [HttpPut("make-an-account")]
        public async Task MakeAnAccount() => await _helper.MakeAnAccount();

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var clients = await _repository.GetAll();
                if (clients == null) return NotFound("Начисления товаров пропали или их пока нет");
                return Ok(clients);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{invoiceId}")]
        public async Task<IActionResult> Get(Guid invoiceId)
        {
            try
            {
                var client = await _repository.Get(invoiceId);
                if (client == null) return NotFound("Клиент не найден");
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
        public async Task<IActionResult> Create(InvoiceDto newClient)
        {
            try
            {
                await _repository.Create(newClient);
                return Ok("Клиент создан");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
