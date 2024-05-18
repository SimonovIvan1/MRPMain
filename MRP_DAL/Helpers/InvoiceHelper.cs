using Microsoft.EntityFrameworkCore;

namespace MRP_DAL.Helpers
{
    public class InvoiceHelper
    {
        private readonly AppDbContext _db;

        public InvoiceHelper(DbContextOptions<AppDbContext> db)
        {
            _db = new AppDbContext(db);
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
                var invDb = await _db.GoodsParams.FirstAsync(x => x.GoodId == invoice.GoodId);
                invDb.Quantity += invoice.Quantity;
                _db.GoodsParams.Update(invDb);
                await _db.SaveChangesAsync();
                invoice.IsAccounting = true;
                _db.Invoice.Update(invoice);
                await _db.SaveChangesAsync();
            }
            return;
        }
    }
}
