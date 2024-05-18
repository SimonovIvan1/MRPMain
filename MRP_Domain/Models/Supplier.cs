namespace MRP_Domain.Entity
{
    public class Supplier
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public List<Goods> Goods { get; set; }
    }
}
