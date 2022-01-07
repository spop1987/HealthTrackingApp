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

            return Task.FromResult<UserDto>(userDto);
        }
    }
}