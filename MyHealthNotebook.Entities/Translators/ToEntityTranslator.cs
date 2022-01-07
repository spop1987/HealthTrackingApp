using MyHealthNotebook.Entities.DbSet;
using MyHealthNotebook.Entities.Dtos.Incoming;

namespace MyHealthNotebook.Entities.Translators
{
    public interface IToEntityTranslator
    {
        Task<User> ToUser(UserDto userDto);
    }
    public class ToEntityTranslator : IToEntityTranslator
    {
        public Task<User> ToUser(UserDto userDto)
        {
            var user = new User();
            return Task.FromResult<User>(user);
        }
    }
}