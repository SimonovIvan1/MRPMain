using Microsoft.EntityFrameworkCore;
using MRP_DAL;
using MRP_DAL.Helpers;

namespace MRP_Order_Manager
{
    public class MRPManager
    {
        private readonly OrderHelper _orderHelper;

        public MRPManager(DbContextOptions<AppDbContext> db)
        {
            _orderHelper = new OrderHelper(db);
        }
        public void ProcessOrder()
        {
            _orderHelper.ProcessOrder();
        }
    }
}
