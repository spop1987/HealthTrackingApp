using MyHealthNotebook.Entities.DbSet;
using MyHealthNotebook.Entities.Dtos.Incoming;

namespace MyHealthNotebook.Entities.Translators
{
    public interface IToEntityTranslator
    {
        Task<User> ToUser(IUserDto userDto, string userId);
    }
    public class ToEntityTranslator : IToEntityTranslator
    {
        public Task<User> ToUser(IUserDto userDto, string userId)
        {
            var user = new User();
            user.IdentityId = string.IsNullOrWhiteSpace(userId) ? Guid.NewGuid() :  new Guid(userId);
            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.Email = userDto.Email;
            user.DateOfBirth = DateTime.UtcNow;
            user.Phone = "";
            user.Country = "";
            return Task.FromResult(user);
        }
    }
}