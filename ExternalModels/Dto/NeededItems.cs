namespace ExternalModels.Dto
{
    public class NeededItems
    {
        public Guid GoodId { get; set; }
        public Guid? ParentItemId { get; set; }
        public int Quantity { get; set; }
        public bool IsMain { get; set; }
    }
}
