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

        public async Task<bool> UpdateUserProfile(User user)
        {
            try
            {
                 var existingUser = await dbSet.Where(u => u.Status == 1 && u.Id == user.Id).FirstOrDefaultAsync();
                 if(existingUser == null)
                    return false;
                
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.MobileNumber = user.MobileNumber;
                existingUser.Phone = user.Phone;
                existingUser.Sex = user.Sex;
                existingUser.Address = user.Address;
                existingUser.UpdateDate = DateTime.UtcNow;
                existingUser.DateOfBirth = user.DateOfBirth;

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "{Repo} UpdateUserProfile method has generated an error", typeof(UserRepository));
                return false;
            }
        }

        public async Task<User> GetByIdentityId(Guid identityId)
        {
            try
            {
                var existingUser = await dbSet.Where(u => u.Status == 1 && u.IdentityId == identityId)
                                            .FirstOrDefaultAsync();

                return existingUser;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "{Repo} GetByIdentityId method has generated an error", typeof(UserRepository));
                return null;
            }
        }
    }
}