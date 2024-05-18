using Microsoft.EntityFrameworkCore;
using MRP_DAL;

namespace MRP_Manager
{
    public class MRPManager
    {
        private readonly AppDbContext _db;

        public MRPManager(DbContextOptions<AppDbContext> db)
        {
            _db = new AppDbContext(db);
        }
        public static void Main()
        {

            
        }
    }

}