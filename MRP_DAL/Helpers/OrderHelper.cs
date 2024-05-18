using ExternalModels;
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
        public async void ProcessOrder(Guid orderId)
        {
            var order = await _db.Order
                .FirstOrDefaultAsync(x => x.Id == orderId);
            if (order == null) return;
            var parentItemsDal = await (from g in _db.Goods
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
            var parentItems = await GetParentItems(order.GoodsId);
            var needItems = new List<GoodsDto>();
            while (parentItems.Count != 0)
            {
                var copyParents = new List<GoodsDto>((IEnumerable<GoodsDto>)parentItems);
                parentItems.Clear();
                foreach (var parentItem in copyParents)
                {
                    if (parentItem.ParentItemId == null) continue;
                    var needItem = parentItemsDal.FirstOrDefault(x => x.Id == parentItem.ParentItemId);
                    if (needItem == null) throw new Exception("Зависимого товара не существует!");
                    needItems.Add(needItem);
                    parentItems.Add(needItem);
                }
            }
            order.OrderStatusId = (int)OrderStatusType.AwaitingSupply;
            order.ExpectedDelivery = DateTime.UtcNow.AddDays(7);
            order.StatusDescription = $"Ваши товары будут доступны для получения после {order.ExpectedDelivery}";
            _db.Order.Update(order);
            await _db.SaveChangesAsync();
        }

        private async Task<List<GoodsDto>> GetParentItems(Guid goodId)
        {
            var parentItems = new List<GoodsDto>();
            var good = await _db.Goods.FirstOrDefaultAsync(x => x.Id == goodId);
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
                                    }).ToListAsync();
            parentItems.AddRange(parentItem);
            return parentItems;
        }
    }
}
