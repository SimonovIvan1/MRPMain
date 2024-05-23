using ExternalModels.Dto;
using Microsoft.EntityFrameworkCore;

namespace MRP_DAL.Helpers
{
    public class ResultHelper
    {
        private readonly AppDbContext _db;
        private readonly InvoiceHelper _invoiceHelper;
        private readonly OrderHelper _orderHelper;

        public ResultHelper(DbContextOptions<AppDbContext> db)
        {
            _db = new AppDbContext(db);
            _invoiceHelper = new InvoiceHelper(db);
            _orderHelper = new OrderHelper(db);
        }

        public async Task<List<NeededItems>> GetNeededItems(Guid orderId)
        {
            var order = await _db.Order.FirstOrDefaultAsync(x => x.Id == orderId);
            if(order == null) return new List<NeededItems>();
            var itemsInOrder = await _orderHelper.GetTree(orderId);
            var itemsInStore = await _invoiceHelper.ProcessOrder(order.GoodsId);
            var result = new List<NeededItems>();
            foreach(var item in itemsInOrder)
            {
                var itemInStore = itemsInStore.FirstOrDefault(x => x.GoodId == item.GoodId);
                if(itemInStore == null)
                {
                    result.Add(item);
                }
                else
                {
                    item.Quantity -= itemInStore.Quantity;
                    result.Add(item);
                }
            }
            return result;
        }
    }
}
