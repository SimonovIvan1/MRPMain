using System.ComponentModel.DataAnnotations.Schema;

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
        public int Quantity { get; set; }
        [ForeignKey("GoodId")]
        public GoodDAL Good { get; set; }
    }
}