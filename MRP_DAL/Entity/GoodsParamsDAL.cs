namespace MRP_DAL.Entity
{
    internal class GoodsParamsDAL
    {
        public Guid Id { get; set; }
        public Guid GoodId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public bool IsMainItem { get; set; }
        public int Balance { get; set; }
        public GoodsDAL Good { get; set; }
    }
}