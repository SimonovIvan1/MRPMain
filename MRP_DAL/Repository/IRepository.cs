namespace MRP_DAL.Repository
{
    interface IRepository<T>
        where T : class
    {
        Task<T[]> GetAll();
        Task<T?> Get(Guid id); 
        Task Create(T item); 
        Task Update(T item);
        Task Delete(Guid id); 
        Task Save();  
    }
}
