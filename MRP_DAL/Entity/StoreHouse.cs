using System.ComponentModel.DataAnnotations.Schema;

namespace MRP_DAL.Entity
{
    internal class StoreHouse
    {
        public Guid Id { get; set; }
        public Guid GoodId { get; set; }
        public int Count { get; set; }
        [ForeignKey("GoodId")]
        public GoodsDAL Good{ get; set; }
    }
}
