namespace MRP_DAL.Entity
{
#nullable disable
    internal class GoodsForSupplierDAL
    {
        public Guid Id { get; set; }
        public Guid GoodsId { get; set; }
        public Guid SupplierId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }
}
