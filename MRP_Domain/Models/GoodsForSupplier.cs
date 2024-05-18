namespace MRP_Domain.Entity
{
#nullable disable
    public class GoodsForSupplier
    {
        public Guid Id { get; set; }
        public Guid GoodsId { get; set; }
        public Guid SupplierId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }
}
