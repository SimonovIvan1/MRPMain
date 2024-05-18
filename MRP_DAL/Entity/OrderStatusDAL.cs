namespace MRP_DAL.Entity
{
    internal class OrderStatusDAL
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public List<OrderDAL>? Orders { get; set; }
    }
}
