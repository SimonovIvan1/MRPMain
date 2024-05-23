using ExternalModels.Dto;
using Microsoft.EntityFrameworkCore;
using MRP_Domain.Entity;

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

        public async Task<List<NeededItems>> GetNeededItems(DateTime dateTimeNow)
        {
            var orders = await _db.Order.Where(x => x.DateTimeCreated < dateTimeNow).ToListAsync();
            var resultItems = new List<NeededItems>();
            foreach(var order in orders)
            {
                var itemsInOrder = await _orderHelper.GetTree(order.Id);
                foreach(var item in itemsInOrder)
                {
                    var checkItem = resultItems.FirstOrDefault(x => x.GoodId == item.GoodId);
                    if (checkItem == null) resultItems.Add(item);
                    else
                    {
                        resultItems.Remove(checkItem);
                        checkItem.Quantity += item.Quantity;
                        resultItems.Add(checkItem);
                    }
                }
            }
            
            var itemsInStore = await _invoiceHelper.ProcessOrder(dateTimeNow);
            var resultStoreItems = new List<NeededItems>();
            foreach(var item in itemsInStore)
            {
                var contItems = itemsInStore.Where(x => x.GoodId == item.GoodId).ToList();
                var totalCount = 0;
                foreach(var cItem in contItems)
                {
                    totalCount += cItem.Quantity;
                }
                item.Quantity = totalCount;
                resultStoreItems.Add(item);
            }
            var result = new List<NeededItems>();
            foreach(var item in resultItems)
            {
                var itemInStore = resultStoreItems.FirstOrDefault(x => x.GoodId == item.GoodId);
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
