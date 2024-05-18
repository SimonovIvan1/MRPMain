using ExternalModels;
using Microsoft.EntityFrameworkCore;
using MRP_DAL.Entity;

namespace MRP_DAL.Repository
{
    public class ClientRepository : IRepository<ClientDto>
    {
        private readonly AppDbContext _db;

        public ClientRepository(DbContextOptions<AppDbContext> db)
        {
            _db = new AppDbContext(db);
        }
#nullable enable
        public async Task Create(ClientDto item)
        {
            if (item.Id != null)
            {
                var clientDb = await _db.Client.FirstOrDefaultAsync(x => x.Id == item.Id);
                if (clientDb != null) throw new Exception("Клиент уже есть в базе!");
            }
            var client = new ClientDAL()
            {
                Id = Guid.NewGuid(),
                Name = item.Name,
                Surname = item.Surname,
                Email = item.Email,
                PhoneNumber = item.PhoneNumber
            };
            await _db.Client.AddAsync(client);
            await Save();
        }

        public async Task Delete(Guid id)
        {
            var client = await _db.Client.FirstOrDefaultAsync(x => x.Id == id);
            if (client == null) return;
            _db.Client.Remove(client);
        }

        public async Task<ClientDto?> Get(Guid id)
        {
            var client = await _db.Client.FirstOrDefaultAsync(x => x.Id == id);
            if (client == null) return default;
            var clientDto = new ClientDto()
            { 
                Id = client.Id,
                Email = client.Email,
                Name = client.Name,
                PhoneNumber = client.PhoneNumber,
                Surname = client.Surname
            };
            return clientDto;
        }

        public async Task<ClientDto[]> GetAll()
        {
            var clients = await _db.Client.ToArrayAsync();
            var clientsDto = new List<ClientDto>();
            foreach(var client in clients)
            {
                var clientDto = new ClientDto()
                {
                    Id = client.Id,
                    Email = client.Email,
                    Name = client.Name,
                    PhoneNumber = client.PhoneNumber,
                    Surname = client.Surname
                };
                clientsDto.Add(clientDto);
            }
            return clientsDto.ToArray();
        }

        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }

        public async Task Update(ClientDto item)
        {
            var client = await _db.Client.FirstOrDefaultAsync(x => x.Id == item.Id);
            if (client == null) return;
            if (!string.IsNullOrWhiteSpace(item.Name))
                client.Name = item.Name;
            if (!string.IsNullOrWhiteSpace(item.Surname))
                client.Surname = item.Surname;
            if (!string.IsNullOrWhiteSpace(item.Email))
                client.Email = item.Email;
            if (!string.IsNullOrWhiteSpace(item.PhoneNumber))
                client.PhoneNumber = item.PhoneNumber;
            _db.Update(client);
            await Save();
        }
    }
}
