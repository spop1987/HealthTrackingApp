using MyHealthNotebook.Entities.DbSet;
using MyHealthNotebook.Entities.Dtos.Incoming;

namespace MyHealthNotebook.Entities.Translators
{
    public interface IToDtoTranslator
    {
        Task<UserDto> ToUserDtoTranslator(User user);
    }

    public class ToDtoTranslator : IToDtoTranslator
    {
        public Task<UserDto> ToUserDtoTranslator(User user)
        {
            var userDto = new UserDto();
            userDto.FirstName = user.FirstName;
            userDto.LastName = user.LastName;
            userDto.DateOfBirth = user.DateOfBirth.ToString();
            userDto.Phone = user.Phone;
            userDto.Email = user.Email;
            userDto.Country = user.Country;
            return Task.FromResult<UserDto>(userDto);
        }
    }
}