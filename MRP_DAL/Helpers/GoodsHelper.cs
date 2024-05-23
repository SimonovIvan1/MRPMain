using ExternalModels;
using ExternalModels.Dto;
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
        public async Task<List<NeededItems>> GetParentsTree(Guid? id, bool isDelete)
        {
            var order = await _db.Good
                .FirstOrDefaultAsync(x => x.Id == id);
            var paramsGood = await _db.GoodsParams.FirstOrDefaultAsync(x => x.GoodId == order.Id);
            var count = paramsGood == null ? 1 : paramsGood.Quantity;
            if (order == null) throw new Exception("Товара на складе не существует");

            var parentItems = await GetParentItems(order.Id);
            var needItems = new List<GoodsDto>();
            var result = new List<NeededItems>();
            while (parentItems.Count != 0)
            {
                var copyParents = new List<GoodsDto>((IEnumerable<GoodsDto>)parentItems);
                parentItems.Clear();
                foreach (var parentItem in copyParents)
                {
                    var needItem = await GetParentItems(parentItem.Id);
                    if (needItem.Count == 0)
                    {
                        var quantityMain = result.FirstOrDefault(x => x.GoodId == parentItem.ParentItemId);
                        int quantity = 0;
                        var newNeededItem = new NeededItems();
                        if (parentItem.ParentItemId == order.Id)
                        {
                            quantity = quantityMain == null ? parentItem.Balance * count
                                                            : quantityMain.Quantity * parentItem.Balance;
                            newNeededItem = new NeededItems
                            {
                                IsMain = true,
                                GoodId = parentItem.Id,
                                ParentItemId = order.Id,
                                Quantity = quantity
                            };
                        }

                        else
                        {
                            quantity = quantityMain == null ? -2
                                                            : quantityMain.Quantity * parentItem.Balance;
                            newNeededItem = new NeededItems
                            {
                                IsMain = true,
                                GoodId = parentItem.Id,
                                ParentItemId = quantityMain?.GoodId,
                                Quantity = quantity
                            };
                        }

                        result.Add(newNeededItem);
                    }

                    else
                    {
                        var quantityMain = result.FirstOrDefault(x => x.GoodId == parentItem.ParentItemId);
                        int? quantity = 0;
                        var newNeededItem = new NeededItems();
                        if (parentItem.ParentItemId == order.Id)
                        {
                            quantity = quantityMain == null ? parentItem.Balance * count
                                                            : quantityMain?.Quantity * parentItem.Balance;
                            newNeededItem = new NeededItems
                            {
                                IsMain = false,
                                GoodId = parentItem.Id,
                                ParentItemId = order.Id,
                                Quantity = quantity.Value
                            };
                        }

                        else
                        {
                            quantity = quantityMain == null ? -1
                                                            : quantityMain.Quantity * parentItem.Balance;
                            newNeededItem = new NeededItems
                            {
                                IsMain = false,
                                GoodId = parentItem.Id,
                                ParentItemId = quantityMain?.GoodId,
                                Quantity = quantity.Value
                            };
                        }

                        result.Add(newNeededItem);
                    }

                    needItems.AddRange(needItem);
                    parentItems.AddRange(needItem);
                }
            }
            return result.Where(x => x.IsMain).ToList();
        }

        private async Task<List<GoodsDto>> GetParentItems(Guid goodId)
        {
            var parentItems = new List<GoodsDto>();
            var parentItem = await (from g in _db.Good
                                    join gp in _db.GoodsParams on g.Id equals gp.GoodId
                                    where g.ParentItemId == goodId
                                    select new GoodsDto()
                                    {
                                        Id = g.Id,
                                        Description = gp.Description,
                                        Name = gp.Name,
                                        Price = gp.Price,
                                        SupplierId = g.SupplierId,
                                        Balance = gp.Quantity,
                                        IsMainItem = gp.IsMainItem,
                                        ParentItemId = g.ParentItemId,
                                    }).ToListAsync();
            parentItems.AddRange(parentItem);
            return parentItems;
        }
    }
}
