namespace MRP_Domain.Entity
{
    public class OrderStatus
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public List<Order>? Orders { get; set; }
    }
}
