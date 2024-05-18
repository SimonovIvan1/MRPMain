using MRP_Domain.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MRP_DAL.Entity
{
#nullable disable
    internal class OrderDAL
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public string Address { get; set; }
        public double TotalCost { get; set; }
        public int OrderStatusId { get; set; }
#nullable enable
        public string? StatusDescription { get; set; }
        public DateTime? ExpectedDelivery { get; set; }
#nullable disable
        public Guid GoodsId { get; set; }
        public int Quantity { get; set; }
        [ForeignKey("GoodId")]
        public Goods Goods { get; set; }
        public ClientDAL Client { get; set; }
        public OrderStatusDAL OrderStatus { get; set; }
    }
}
