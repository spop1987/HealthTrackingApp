using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyHealthNotebook.DataService.IRepository;
using MyHealthNotebook.Entities.DbSet;
using MyHealthNoteBook.DataService.Data;

namespace MyHealthNotebook.DataService.Repository
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(
            AppDbContext context,
            ILogger logger) : base(context, logger)
        {}

        public override async Task<IEnumerable<User>> All()
        {
            try
            {
                 return await dbSet.Where(u => u.Status == 1).AsNoTracking().ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "{Repo} All method has generated an error", typeof(UserRepository));
                return new List<User>();
            }
        }
    }
}