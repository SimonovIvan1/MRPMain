using ExternalModels;
using Microsoft.EntityFrameworkCore;
using MRP_DAL.Entity;

namespace MRP_DAL.Repository
{
    public class OrderStatusRepository : IRepository<OrderStatusDto>
    {
        private readonly AppDbContext _db;

        public OrderStatusRepository(DbContextOptions<AppDbContext> db)
        {
            _db = new AppDbContext(db);
        }
#nullable enable
        public async Task Create(OrderStatusDto item)
        {
            var orderStatusDb = await _db.OrderStatus.FirstOrDefaultAsync(x => x.Id == item.Id);
            if (orderStatusDb != null) throw new Exception("Статус заказа уже есть в базе!");
            var orderStatus = new OrderStatusDAL()
            {
                Id = item.Id,
                Name = item.Name    
            };
            await _db.OrderStatus.AddAsync(orderStatus);
            await Save();
        }
       
        public async Task<OrderStatusDto[]> GetAll()
        {
            var orderStatuses = await _db.OrderStatus.ToArrayAsync();
            var orderStatusesDto = new List<OrderStatusDto>();
            foreach (var orderStatus in orderStatuses)
            {
                var orderStatusDto = new OrderStatusDto()
                {
                    Id = orderStatus.Id,
                    Name = orderStatus.Name
                };
                orderStatusesDto.Add(orderStatusDto);
            }
            return orderStatusesDto.ToArray();
        }

        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }

        public async Task Update(OrderStatusDto item)
        {
            var client = await _db.OrderStatus.FirstOrDefaultAsync(x => x.Id == item.Id);
            if (client == null) return;
            if (!string.IsNullOrWhiteSpace(item.Name))
                client.Name = item.Name;
            _db.Update(client);
            await Save();
        }

        public async Task Delete(int id)
        {
            var orderStatus = await _db.OrderStatus.FirstOrDefaultAsync(x => x.Id == id);
            if (orderStatus == null) return;
            _db.OrderStatus.Remove(orderStatus);
        }

        public async Task<OrderStatusDto?> Get(int id)
        {
            var orderStatus = await _db.OrderStatus.FirstOrDefaultAsync(x => x.Id == id);
            if (orderStatus == null) return default;
            var orderStatusDto = new OrderStatusDto()
            {
                Id = orderStatus.Id,
                Name = orderStatus.Name
            };
            return orderStatusDto;
        }

        public Task Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<OrderStatusDto?> Get(Guid id)
        {
            throw new NotImplementedException();
        }

    }
}
