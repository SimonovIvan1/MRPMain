using Microsoft.EntityFrameworkCore;
using MRP_DAL.Entity;

namespace MRP_DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
                : base(options)
        {
            Database.EnsureCreated();
        }
        internal DbSet<OrderStatusDAL> OrderStatus { get; set; }
        internal DbSet<ClientDAL> Client { get; set; }
        internal DbSet<GoodsForSupplierDAL> GoodsForSuppliers { get; set; }
        internal DbSet<OrderDAL> Order { get; set; }
        internal DbSet<GoodsParamsDAL> GoodsParams { get; set; }
        internal DbSet<InvoiceDAL> Invoice { get; set; }
        internal DbSet<GoodDAL> Good { get; set; }
        internal DbSet<StoreHouse> StoreHouse { get; set; }
    }
}