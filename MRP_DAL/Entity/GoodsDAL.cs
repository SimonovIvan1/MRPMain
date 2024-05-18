namespace MRP_DAL.Entity
{
#nullable disable
    internal class GoodsDAL
    {
        public Guid Id { get; set; }
        public Guid SupplierId { get; set; }
        public Guid? ParentItemId { get; set; }
        public SupplierDAL Supplier { get; set; }
        public GoodsParamsDAL GoodsParams { get; set; }
    }
}