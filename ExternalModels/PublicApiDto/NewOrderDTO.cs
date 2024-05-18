namespace ExternalModels.PublicApiDto
{
    public class NewOrderDTO
    {
#nullable disable
        public Guid ClientId { get; set; }
        public string Address { get; set; }
        public Guid GoodId { get; set; }
        public int Quantity { get; set; }
    }
}
