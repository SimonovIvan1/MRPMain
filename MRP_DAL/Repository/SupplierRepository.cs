using ExternalModels;
using Microsoft.EntityFrameworkCore;
using MRP_DAL.Entity;

namespace MRP_DAL.Repository
{
    public class SupplierRepository : IRepository<SupplierDto>
    {
        private readonly AppDbContext _db;

        public SupplierRepository(DbContextOptions<AppDbContext> db)
        {
            _db = new AppDbContext(db);
        }
#nullable enable
        public async Task Create(SupplierDto item)
        {
            if (item.Id != null)
            {
                var clientDb = await _db.Supplier.FirstOrDefaultAsync(x => x.Id == item.Id);
                if (clientDb != null) throw new Exception("Клиент уже есть в базе!");
            }
            var client = new SupplierDAL()
            {
                Id = Guid.NewGuid(),
                Name = item.Name,
                Email = item.Email,
                PhoneNumber = item.PhoneNumber,
            };
            await _db.Supplier.AddAsync(client);
            await Save();
        }

        public async Task Delete(Guid id)
        {
            var client = await _db.Supplier.FirstOrDefaultAsync(x => x.Id == id);
            if (client == null) return;
            _db.Supplier.Remove(client);
        }

        public async Task<SupplierDto?> Get(Guid id)
        {
            var client = await _db.Supplier.FirstOrDefaultAsync(x => x.Id == id);
            if (client == null) return default;
            var clientDto = new SupplierDto()
            {
                Id = client.Id,
                Name = client.Name,
                Email = client.Email,
                PhoneNumber = client.PhoneNumber,
            };
            return clientDto;
        }

        public async Task<SupplierDto[]> GetAll()
        {
            var clients = await _db.Supplier.ToArrayAsync();
            var clientsDto = new List<SupplierDto>();
            foreach (var client in clients)
            {
                var clientDto = new SupplierDto()
                {
                    Id = client.Id,
                    Name = client.Name,
                    Email = client.Email,
                    PhoneNumber = client.PhoneNumber,
                };
                clientsDto.Add(clientDto);
            }
            return clientsDto.ToArray();
        }

        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }

        public async Task Update(SupplierDto item)
        {
            var client = await _db.Supplier.FirstOrDefaultAsync(x => x.Id == item.Id);
            if (client == null) return;
            if (!string.IsNullOrWhiteSpace(item.Name))
                client.Name = item.Name;
            if (!string.IsNullOrWhiteSpace(item.Email))
                client.Email = item.Email;
            if (!string.IsNullOrWhiteSpace(item.PhoneNumber))
                client.PhoneNumber = item.PhoneNumber;
            _db.Update(client);
            await Save();
        }
    }
}
