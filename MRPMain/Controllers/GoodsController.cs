using ExternalModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MRP_DAL.Repository;
using MRP_DAL;
using MRP_Domain.Helpers;

namespace MRP_Admin_Api.Controllers
{
    public class GoodsController : Controller
    {
        private readonly GoodsRepository _repository;
        private readonly GoodsHelper _goodsHelper;

        public GoodsController(DbContextOptions<AppDbContext> db)
        {
            _repository = new GoodsRepository(db);
            _goodsHelper = new GoodsHelper(db);
        }

        public async Task<IActionResult> GetAll()
        {
            try
            {
                var clients = await _repository.GetAll();
                if (clients == null) return NotFound("Товары пропали или их пока нет");
                return View(clients);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{goodsId}")]
        public async Task<IActionResult> Get(Guid goodsId)
        {
            try
            {
                var client = await _repository.Get(goodsId);
                if (client == null) return NotFound("Товар не найден");
                return Ok(client);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task Delete(Guid id) => await _goodsHelper.GetParentsTree(id, true);

        public async Task<IActionResult> Create(string name, int quantity, Guid? parentItemId, string price,
            Guid supplierId)
        {
            try
            {
                var newGood = new GoodsDto
                {
                    Id = Guid.NewGuid(),
                    Description = "Новый товар",
                    Balance = quantity,
                    IsMainItem = true,
                    Name = name,
                    ParentItemId = parentItemId,
                    Price = Double.Parse(price),
                    SupplierId = supplierId
                };
                await _repository.Create(newGood);
                return Redirect("https://localhost:7201/Goods/GetAll");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        public async Task<IActionResult> GetTree(Guid? id)
            => View(await _goodsHelper.GetParentsTree(id, false));
        
    }
}
