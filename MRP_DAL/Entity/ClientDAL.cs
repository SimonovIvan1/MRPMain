namespace MRP_DAL.Entity
{
    internal class ClientDAL
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Surname{ get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public List<OrderDAL>? Orders { get; set; }
    }
}