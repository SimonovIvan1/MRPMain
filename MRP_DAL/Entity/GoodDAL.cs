using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MRP_DAL.Entity
{
#nullable disable
    internal class GoodDAL
    {
        [Key]
        public Guid Id { get; set; }
        public Guid SupplierId { get; set; }
        public Guid? ParentItemId { get; set; }
        public GoodsParamsDAL GoodsParams { get; set; }
    }
}