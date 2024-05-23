using ExternalModels;
using ExternalModels.Dto;
using Microsoft.EntityFrameworkCore;
using MRP_DAL.Entity;

namespace MRP_DAL.Repository
{
    public class InvoiceRepository : IRepository<InvoiceDto>
    {
        private readonly AppDbContext _db;

        public InvoiceRepository(DbContextOptions<AppDbContext> db)
        {
            _db = new AppDbContext(db);
        }
#nullable enable
        public async Task Create(InvoiceDto item)
        {
            var newInvoice = new InvoiceDAL
            {
                Id = Guid.NewGuid(),
                GoodId = item.GoodId,
                Quantity = item.Quantity,
                AccountingTime = item.AccountingTime,
                IsAccounting = false
            };
            await _db.Invoice.AddAsync(newInvoice);
            await Save();
        }

        public async Task Delete(Guid id)
        {
            var client = await _db.Invoice.FirstOrDefaultAsync(x => x.Id == id);
            if (client == null) return;
            _db.Invoice.Remove(client);
        }

        public async Task<InvoiceDto?> Get(Guid id)
        {
            var invoice = await _db.Invoice.FirstOrDefaultAsync(x => x.Id == id);
            if (invoice == null) return default;
            var invoiceDto = new InvoiceDto()
            {
                GoodId = invoice.GoodId,
                Quantity = invoice.Quantity,
                AccountingTime = invoice.AccountingTime,
                IsAccounting = invoice.IsAccounting
            };
            return invoiceDto;
        }

        public async Task<InvoiceDto[]> GetAll()
        {
            var clients = await _db.Invoice.ToArrayAsync();
            var clientsDto = new List<InvoiceDto>();
            foreach (var invoice in clients)
            {
                var clientDto = new InvoiceDto()
                {
                    Id = invoice.Id,
                    GoodId = invoice.GoodId,
                    Quantity = invoice.Quantity,
                    AccountingTime = invoice.AccountingTime,
                    IsAccounting = invoice.IsAccounting
                };
                clientsDto.Add(clientDto);
            }
            return clientsDto.ToArray();
        }

        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }

        public async Task Update(InvoiceDto item)
        {
            throw new NotImplementedException();
        }

        public async Task Update(Guid id, InvoiceDto item)
        {
            var client = await _db.Invoice.FirstOrDefaultAsync(x => x.Id == id);
            if (client == null) return;
            client.Quantity = item.Quantity;
            client.AccountingTime = item.AccountingTime;
            _db.Update(client);
            await Save();
            return;
        }
    }
}
