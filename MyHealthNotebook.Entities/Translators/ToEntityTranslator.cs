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
            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.Email = userDto.Email;
            user.DateOfBirth = Convert.ToDateTime(userDto.DateOfBirth);
            user.Phone = userDto.Phone;
            user.Country = userDto.Country;
            return Task.FromResult(user);
        }
    }
}