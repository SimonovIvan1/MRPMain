using ExternalModels;
using ExternalModels.Dto;
using ExternalModels.PublicApiDto;
using Microsoft.EntityFrameworkCore;
using MRP_DAL.Entity;
using MRP_Domain.Entity;
using MRP_Domain.Enum;

namespace MRP_DAL.Helpers
{
    public class OrderHelper
    {
        private readonly AppDbContext _db;

        public OrderHelper(DbContextOptions<AppDbContext> db)
        {
            _db = new AppDbContext(db);
        }

        public async Task<Order[]> GetAll()
        {
            var ordersDal = await _db.Order.ToArrayAsync();
            var orders = new List<Order>();
            foreach (var orderDal in ordersDal)
            {
                var order = new Order()
                {
                    Id = orderDal.Id,
                    DateTimeCreated = orderDal.DateTimeCreated,
                    Address = orderDal.Address,
                    ClientId = orderDal.ClientId,
                    OrderStatusId = orderDal.OrderStatusId,
                    StatusDescription = orderDal.StatusDescription,
                    ExpectedDelivery = orderDal.ExpectedDelivery,
                    TotalCost = orderDal.TotalCost
                };
                orders.Add(order);
            }
            return orders.ToArray();
        }

        public async Task<Order> CreateOrder(NewOrderDTO newOrder)
        {
            var order = new Order()
            { 
                Id = Guid.NewGuid(),
                DateTimeCreated = DateTime.UtcNow,
                Address = newOrder.Address,
                ClientId = newOrder.ClientId,
                OrderStatusId = (int)OrderStatusType.Created,
                StatusDescription = "Заказ создан",
                GoodsId = newOrder.GoodId,
                Quantity = newOrder.Quantity
            };
            var goodParams = await _db.GoodsParams.FirstAsync(x => x.GoodId == newOrder.GoodId);
            
            order.TotalCost = newOrder.Quantity * goodParams.Price;
            var newOrderDb = new OrderDAL()
            {
                Id = order.Id,
                DateTimeCreated = DateTime.UtcNow,
                Address = order.Address,
                ClientId = order.ClientId,
                OrderStatusId = (int)OrderStatusType.InProcessing,
                StatusDescription = "Заказ на стадии обработки. Дата возможного получения товаров " +
                "со склада будет доступна после обработки. Пожалуйста, ожидайте.",
                TotalCost = order.TotalCost
            };
            await _db.Order.AddAsync(newOrderDb);
            await _db.SaveChangesAsync();
            return order;
        }

        //Метод для разложения заказа
        public async Task<List<NeededItems>> ProcessOrder(Guid orderId)
        {
            var order = await _db.Order
                .FirstOrDefaultAsync(x => x.Id == orderId);
            if (order == null) throw new Exception("Заказа не существует");
            
            var parentItems = await GetParentItems(order.GoodsId);
            var needItems = new List<GoodsDto>();
            var result = new List<NeededItems>();
            while (parentItems.Count != 0)
            {
                var copyParents = new List<GoodsDto>((IEnumerable<GoodsDto>)parentItems);
                parentItems.Clear();
                foreach (var parentItem in copyParents)
                {
                    var needItem = await GetParentItems(parentItem.Id);
                    if(needItem.Count == 0)
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
                        continue;
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
