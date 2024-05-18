using ExternalModels;
using Microsoft.EntityFrameworkCore;
using MRP_DAL;
using MRP_DAL.Entity;
using MRP_Domain.Entity;

namespace MRP_Domain.Helpers
{
    public class GoodsHelper
    {
        private readonly AppDbContext _db;

        public GoodsHelper(DbContextOptions<AppDbContext> db)
        {
            _db = new AppDbContext(db);
        }
        public async Task<List<GoodsDto>> GetParentsTree(Guid? id, bool isDelete)
        {
            var parentItemsDal = new List<GoodsDto>();
            if(id == null)
            {
                parentItemsDal = await (from g in _db.Goods
                                        join gp in _db.GoodsParams on g.Id equals gp.GoodId
                                        where gp.IsMainItem == true
                                        select new GoodsDto()
                                        {
                                            Id = g.Id,
                                            Description = gp.Description,
                                            Name = gp.Name,
                                            Price = gp.Price,
                                            SupplierId = g.SupplierId,
                                            Balance = gp.Balance,
                                            IsMainItem = gp.IsMainItem,
                                            ParentItemId = g.ParentItemId
                                        }).ToListAsync();
            }
            else
            {
                parentItemsDal = await (from g in _db.Goods
                                        join gp in _db.GoodsParams on g.Id equals gp.GoodId
                                        where g.Id == id
                                        select new GoodsDto()
                                        {
                                            Id = g.Id,
                                            Description = gp.Description,
                                            Name = gp.Name,
                                            Price = gp.Price,
                                            SupplierId = g.SupplierId,
                                            Balance = gp.Balance,
                                            IsMainItem = gp.IsMainItem,
                                            ParentItemId = g.ParentItemId
                                        }).ToListAsync();
            }
            
            var parentItems = await GetParentItems(parentItemsDal);
            var needItems = new List<GoodsDto>();
            while (parentItems.Count != 0)
            {
                var copyParents = new List<GoodsDto>(parentItems);
                parentItems.Clear();
                foreach (var parentItem in copyParents)
                {
                    var childItem = await (from g in _db.Goods
                                          join gp in _db.GoodsParams on g.Id equals gp.GoodId
                                          where g.ParentItemId == parentItem.Id
                                          select new GoodsDto()
                                          {
                                              Id = g.Id,
                                              Description = gp.Description,
                                              Name = gp.Name,
                                              Price = gp.Price,
                                              SupplierId = g.SupplierId,
                                              Balance = gp.Balance,
                                              IsMainItem = gp.IsMainItem,
                                              ParentItemId = g.ParentItemId
                                          }).FirstOrDefaultAsync();
                    if (childItem == null) continue;
                    needItems.Add(childItem);
                    parentItems.Add(childItem);
                }
            }
            var mainItem = await _db.Goods.FirstAsync(x => x.Id == needItems[0].ParentItemId);
            if (!isDelete)
            {
                var mainItemInfo = await (from g in _db.Goods
                                          join gp in _db.GoodsParams on g.Id equals gp.GoodId
                                          where g.Id == mainItem.Id
                                          select new GoodsDto()
                                          {
                                              Id = g.Id,
                                              Description = gp.Description,
                                              Name = gp.Name,
                                              Price = gp.Price,
                                              SupplierId = g.SupplierId,
                                              Balance = gp.Balance,
                                              IsMainItem = gp.IsMainItem,
                                              ParentItemId = g.ParentItemId
                                          }).FirstAsync();
                needItems.Insert(0, mainItemInfo);
                return needItems;
            }
                
            
            needItems.Reverse();
            foreach(var item in needItems)
            {
                var deletedItem = await _db.Goods.FirstOrDefaultAsync(x => x.Id == item.Id);
                _db.Goods.Remove(deletedItem);
                await _db.SaveChangesAsync();
            }
            _db.Goods.Remove(mainItem);
            await _db.SaveChangesAsync();
            return needItems;
        }

        private async Task<List<GoodsDto>> GetParentItems(List<GoodsDto> goods)
        {
            var parentItems = new List<GoodsDto>();
            foreach (var item in goods)
            {
                var good = await _db.Goods.FirstOrDefaultAsync(x => x.Id == item.Id);
                if (good == null) throw new Exception("Товара не существует");
                var parentItem = await (from g in _db.Goods
                                        join gp in _db.GoodsParams on g.Id equals gp.GoodId
                                        where gp.IsMainItem == true
                                        select new GoodsDto()
                                        {
                                            Id = g.Id,
                                            Description = gp.Description,
                                            Name = gp.Name,
                                            Price = gp.Price,
                                            SupplierId = g.SupplierId,
                                            Balance = gp.Balance,
                                            IsMainItem = gp.IsMainItem,
                                            ParentItemId = g.ParentItemId
                                        }).FirstOrDefaultAsync();
                if (parentItem == null) continue;
                parentItems.Add(parentItem);
            }
            return parentItems;
        }
    }
}
