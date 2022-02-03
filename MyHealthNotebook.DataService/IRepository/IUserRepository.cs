using MyHealthNotebook.Entities.DbSet;

namespace MyHealthNotebook.DataService.IRepository
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<bool> UpdateUserProfile(User user);
        Task<User> GetByIdentityId(Guid identityId);
    }
}