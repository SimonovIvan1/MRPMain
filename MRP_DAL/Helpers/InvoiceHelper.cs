using ExternalModels;
using ExternalModels.Dto;
using Microsoft.EntityFrameworkCore;
using MRP_DAL.Entity;
using MRP_Domain.Entity;

namespace MRP_DAL.Helpers
{
    public class InvoiceHelper
    {
        private readonly AppDbContext _db;

        public InvoiceHelper(DbContextOptions<AppDbContext> db)
        {
            _db = new AppDbContext(db);
        }

        public async Task CreateInvoice(InvoiceDto newInvoice)
        {
            var good = await _db.Good.FirstAsync(x => x.Id == newInvoice.GoodId);
            var storeInfo = await _db.StoreHouse.FirstOrDefaultAsync(x => x.GoodId == newInvoice.GoodId);
            if(storeInfo == null)
            {
                var newStoreInfo = new StoreHouse
                {
                    Id = Guid.NewGuid(),
                    GoodId = newInvoice.GoodId,
                    Count = newInvoice.Quantity
                };
                _db.StoreHouse.Add(newStoreInfo);
                await _db.SaveChangesAsync();
            }
            else
            {
                storeInfo.Count += newInvoice.Quantity;
                _db.StoreHouse.Update(storeInfo);
                await _db.SaveChangesAsync();
            }
            var newInvoiceDb = new InvoiceDAL
            {
                Id = Guid.NewGuid(),
                GoodId = newInvoice.GoodId,
                Quantity = newInvoice.Quantity,
                AccountingTime = newInvoice.AccountingTime,
                IsAccounting = false
            };
            await _db.Invoice.AddAsync(newInvoiceDb);
            await _db.SaveChangesAsync();
            return;
        }

        public async Task MakeAnAccount()
        {
            var invoices = await _db.Invoice.Where(x => x.AccountingTime <= DateTime.Now &&
                                                   x.IsAccounting == false)
                                            .ToListAsync();
            if (invoices.Count == 0)
                return;
            foreach(var invoice in invoices)
            {
                var invDb = await _db.StoreHouse.FirstAsync(x => x.GoodId == invoice.GoodId);
                invDb.Count += invoice.Quantity;
                _db.StoreHouse.Update(invDb);
                await _db.SaveChangesAsync();
                invoice.IsAccounting = true;
                _db.Invoice.Update(invoice);
                await _db.SaveChangesAsync();
            }
            return;
        }

        //Метод для разложения товара на складе
        public async Task<List<NeededItems>> ProcessOrder(Guid goodId)
        {
            var order = await _db.StoreHouse
                .FirstOrDefaultAsync(x => x.GoodId == goodId);
            if (order == null) throw new Exception("Товара на складе не существует");

            var parentItems = await GetParentItems(order.GoodId);
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
                        if (parentItem.ParentItemId == order.GoodId)
                        {
                            quantity = quantityMain == null ? parentItem.Balance * order.Count
                                                            : quantityMain.Quantity * parentItem.Balance;
                            newNeededItem = new NeededItems
                            {
                                IsMain = true,
                                GoodId = parentItem.Id,
                                ParentItemId = order.GoodId,
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
                        if (parentItem.ParentItemId == order.GoodId)
                        {
                            quantity = quantityMain == null ? parentItem.Balance * order.Count
                                                            : quantityMain?.Quantity * parentItem.Balance;
                            newNeededItem = new NeededItems
                            {
                                IsMain = false,
                                GoodId = parentItem.Id,
                                ParentItemId = order.GoodId,
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

        public async Task<List<NeededItems>> ProcessOrder(DateTime timeNow)
        {
            var allOrders = await _db.Order.Where(x => x.DateTimeCreated < timeNow).ToListAsync();
            var result = new List<NeededItems>();
            foreach (var order in allOrders)
            {
                var orders = await _db.StoreHouse
                .FirstOrDefaultAsync(x => x.GoodId == order.GoodsId);

                var parentItems = await GetParentItems(order.GoodsId);
                var needItems = new List<GoodsDto>();
                
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
                            if (parentItem.ParentItemId == order.GoodsId)
                            {
                                quantity = quantityMain == null ? parentItem.Balance * order.Quantity
                                                                : quantityMain.Quantity * parentItem.Balance;
                                newNeededItem = new NeededItems
                                {
                                    IsMain = true,
                                    GoodId = parentItem.Id,
                                    ParentItemId = order.GoodsId,
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
                            if (parentItem.ParentItemId == order.GoodsId)
                            {
                                quantity = quantityMain == null ? parentItem.Balance * order.Quantity
                                                                : quantityMain?.Quantity * parentItem.Balance;
                                newNeededItem = new NeededItems
                                {
                                    IsMain = false,
                                    GoodId = parentItem.Id,
                                    ParentItemId = order.GoodsId,
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
            }
            
            return result.Where(x => x.IsMain).ToList();
        }
    }
}
