namespace MRP_Domain.Entity
{
    public class Client
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Surname{ get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public List<Order>? Orders { get; set; }
    }
}