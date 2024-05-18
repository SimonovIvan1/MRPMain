using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExternalModels
{
#nullable disable
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public string Address { get; set; }
        public double TotalCost { get; set; }
        public int OrderStatusId { get; set; }
    }
}
