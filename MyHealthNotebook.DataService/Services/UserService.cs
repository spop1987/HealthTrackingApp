using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MyHealthNotebook.Entities.Dtos.Incoming;
using MyHealthNotebook.Entities.Translators;
using MyHealthNoteBook.DataService.Data;

namespace MyHealthNotebook.DataService.Services
{
    public interface IUserService
    {
        Task<UserDto> GetUser(Guid userId);
    }
    public class UserService : IUserService
    {
        private readonly IdentityDbContext _idntityContext;
        private readonly AppDbContext _dbContext;
        private readonly IToDtoTranslator _toDtoTranslator;
        public UserService(
            IdentityDbContext idntityContext,
            AppDbContext dbContext,
            IToDtoTranslator toDtoTranslator)
        {
            _idntityContext = idntityContext;
            _dbContext = dbContext;
            _toDtoTranslator = toDtoTranslator;
        }

        public Task<UserDto> GetUser(Guid userId)
        {
            var user = _dbContext.Users.SingleOrDefault(u => u.Id == userId);
            var userDto = _toDtoTranslator.ToUserDtoTranslator(user);
            return userDto;
        }
    }
}