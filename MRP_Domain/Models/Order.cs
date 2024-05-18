using System.ComponentModel.DataAnnotations;

namespace MRP_Domain.Entity
{
#nullable disable
    public class Order
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
        public Goods Goods { get; set; }
        public Client Client { get; set; }
        public OrderStatus OrderStatus { get; set; }
    }
}
