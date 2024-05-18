namespace ExternalModels
{
#nullable disable
    public class GoodsForSupplierDto
    {
        public Guid Id { get; set; }
        public Guid GoodsId { get; set; }
        public Guid SupplierId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }
}
