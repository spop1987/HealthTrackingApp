using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyHealthNotebook.DataService.IRepository;
using MyHealthNoteBook.DataService.Data;

namespace MyHealthNotebook.DataService.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected AppDbContext _context;
        internal DbSet<T> dbSet;
        protected readonly ILogger _logger;

        public GenericRepository(
            AppDbContext context,
            ILogger logger)
        {
            _context = context;
            dbSet = _context.Set<T>();
            _logger = logger;
        }

        public virtual async Task<bool> Add(T entity)
        {
            await dbSet.AddAsync(entity);
            return true;
        }

        public virtual async Task<IEnumerable<T>> All()
        {
            var allEntities = await dbSet.ToListAsync();
            return allEntities;
        }

        public virtual async Task<bool> Delete(Guid id, string userId)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<T> GetById(Guid Id)
        {
            var entity = await dbSet.FindAsync(Id.ToString());
            return entity;
        }

        public Task<bool> Upsert(T entity)
        {
            throw new NotImplementedException();
        }
    }
}