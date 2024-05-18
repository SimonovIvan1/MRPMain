namespace MRP_Domain.Entity
{
#nullable disable
    public class Goods
    {
        public Guid Id { get; set; }
        public Guid SupplierId { get; set; }
        public Guid StorehouseId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public Supplier Supplier { get; set; }
        public Storehouse Storehouse { get; set; }
    }
}
