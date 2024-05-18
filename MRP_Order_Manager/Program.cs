using Microsoft.EntityFrameworkCore;
using MRP_DAL;
using MRP_DAL.Helpers;
using MRP_Order_Manager;

class Program
{
    public readonly DbContextOptions<AppDbContext> _db;

    public Program(DbContextOptions<AppDbContext> db)
    {
        _db = db;
    }
    public void Main()
    {
        while (true)
        {
            Thread.Sleep(10000);
            Console.WriteLine("Поиск заказов для обработки");
            var manager = new MRPManager(_db);
            manager.ProcessOrder();
            Console.WriteLine("Обработка завершена");
            Thread.Sleep(10000);
            Console.WriteLine("Старт новой обработки");
        }
    }
}