using ExternalModels;
using Microsoft.EntityFrameworkCore;
using MRP_DAL.Entity;
using MRP_Domain.Entity;

namespace MRP_DAL.Repository
{
    public class GoodsRepository : IRepository<GoodsDto>
    {
        private readonly AppDbContext _db;

        public GoodsRepository(DbContextOptions<AppDbContext> db)
        {
            _db = new AppDbContext(db);
        }
#nullable enable
        public async Task Create(GoodsDto item)
        {
            if (item.Id != null)
            {
                var goodsDb = await _db.Goods.FirstOrDefaultAsync(x => x.Id == item.Id);
                if (goodsDb != null) throw new Exception("Товар уже есть в базе!");
            }
            var good = new GoodsDAL()
            {
                Id = Guid.NewGuid(),
                ParentItemId = item.ParentItemId,
                SupplierId = item.SupplierId,
            };
            var goodParams = new GoodsParamsDAL()
            {
                Description = item.Description,
                Name = item.Name,
                Price = item.Price,
                IsMainItem = item.IsMainItem,
                GoodId = good.Id,
                Balance = item.Balance
            };
            await _db.Goods.AddAsync(good);
            await Save();
            await _db.GoodsParams.AddAsync(goodParams);
            await Save();
        }

        public async Task Delete(Guid id)
        {
            var client = await _db.Goods.FirstOrDefaultAsync(x => x.Id == id);
            if (client == null) return;
            _db.Goods.Remove(client);
        }

        public async Task<GoodsDto?> Get(Guid id)
        {
            var good = await _db.Goods.FirstOrDefaultAsync(x => x.Id == id);
            if (good == null) return default;
            var goodParams = await _db.GoodsParams.FirstOrDefaultAsync(x => x.GoodId == good.Id);
            var clientDto = new GoodsDto()
            {
                Id = good.Id,
                Description = goodParams.Description,
                Name = goodParams.Name,
                Price = goodParams.Price,
                SupplierId = good.SupplierId,
                Balance = goodParams.Balance,
                IsMainItem = goodParams.IsMainItem,
                ParentItemId = good.ParentItemId
            };
            return clientDto;
        }

        public async Task<GoodsDto[]> GetAll()
        {
            var goods = await _db.Goods.ToArrayAsync();
            var goodsDtos = new List<GoodsDto>();
            foreach (var good in goods)
            {
                var goodParams = await _db.GoodsParams.FirstOrDefaultAsync(x => x.GoodId == good.Id);
                var goodDto = new GoodsDto()
                {
                    Id = good.Id,
                    Description = goodParams.Description,
                    Name = goodParams.Name,
                    Price = goodParams.Price,
                    SupplierId = good.SupplierId,
                    Balance = goodParams.Balance,
                    IsMainItem = goodParams.IsMainItem,
                    ParentItemId = good.ParentItemId
                };
                goodsDtos.Add(goodDto);
            }
            return goodsDtos.ToArray();
        }

        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }

        public async Task Update(GoodsDto item)
        {
            var client = await _db.Goods.FirstOrDefaultAsync(x => x.Id == item.Id);
            if (client == null) return;
            _db.Update(client);
            await Save();
        }
    }
}