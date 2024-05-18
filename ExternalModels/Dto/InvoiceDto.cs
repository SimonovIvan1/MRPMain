namespace ExternalModels.Dto
{
    public class InvoiceDto
    {
        public Guid GoodId { get; set; }
        public DateTime AccountingTime { get; set; }
        public bool IsAccounting { get; set; }
        public int Quantity { get; set; }
    }
}
