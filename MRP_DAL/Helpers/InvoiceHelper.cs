using ExternalModels.Dto;
using Microsoft.EntityFrameworkCore;
using MRP_DAL.Entity;

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
    }
}
