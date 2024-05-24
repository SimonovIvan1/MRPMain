using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MRP_DAL.Repository;
using MRP_DAL;
using MRP_DAL.Helpers;
using ExternalModels;
using ExternalModels.Dto;

namespace MRP_Admin_Api.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly InvoiceHelper _helper;
        private readonly InvoiceRepository _repository;

        public InvoiceController(DbContextOptions<AppDbContext> db)
        {
            _helper = new InvoiceHelper(db);
            _repository = new InvoiceRepository(db);
        }

        public async Task<IActionResult> GetTree(Guid goodId) => View(await _helper.ProcessOrder(goodId));


        [HttpPut("create-or-update")]
        public async Task MakeAnAccount(InvoiceDto newInvoice) 
            => await _helper.CreateInvoice(newInvoice);

        public async Task<IActionResult> GetAll()
        {
            try
            {
                var clients = await _repository.GetAll();
                if (clients == null) return NotFound("Начисления товаров пропали или их пока нет");
                return View(clients);
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

        public async Task<IActionResult> Create(Guid goodId, int quantity)
        {
            try
            {
                var newClient = new InvoiceDto()
                {
                    Id = Guid.NewGuid(),
                    AccountingTime = DateTime.UtcNow,
                    GoodId = goodId,
                    IsAccounting = true,
                    Quantity = quantity
                };
                await _repository.Create(newClient);
                return Redirect("https://localhost:7201/Invoice/GetAll");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("make-an-account")]
        public async Task MakeAnAccount() => await _helper.MakeAnAccount();

        public async Task<IActionResult> GetAllSklad() => View(await _repository.GetAllSklad());      
    }
}
