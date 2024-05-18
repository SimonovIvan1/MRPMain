namespace ExternalModels
{
#nullable disable
    public class GoodsDto
    {
        public Guid? Id { get; set; }
        public Guid SupplierId { get; set; }
        public Guid? ParentItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Balance { get; set; }
        public bool IsMainItem { get; set; }
    }
}
