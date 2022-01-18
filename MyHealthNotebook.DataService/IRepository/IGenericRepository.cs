namespace MyHealthNotebook.DataService.IRepository
{
    public interface IGenericRepository<T> where T : class
    {
         Task<IEnumerable<T>> All();
         Task<T> GetById(Guid Id);
         Task<bool> Add(T entity);
         Task<bool> Delete(Guid id, string userId);
         // Update entity or add if it does not exist
         Task<bool> Upsert(T entity); 
    }
}