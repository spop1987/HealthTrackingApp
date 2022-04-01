using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyHealthNotebook.DataService.IConfiguration;
using MyHealthNotebook.DataService.IRepository;
using MyHealthNotebook.DataService.Repository;
using MyHealthNotebook.Entities.DbSet;
using MyHealthNotebook.Entities.Dtos.Incoming;
using MyHealthNotebook.Entities.Translators;
using MyHealthNoteBook.DataService.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MyHealthNotebook.Entities.Dtos.Generic;
using MyHealthNotebook.Entities.Dtos.Errors;
using MyHealthNotebook.Configurations.Messages;

namespace MyHealthNotebook.DataService.Data
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly AppDbContext _context;
        private readonly ILogger _logger;
        public IToDtoTranslator ToDtoTranslator {get; set;}
        public IToEntityTranslator ToEntityTranslator {get; set;}
        public UnitOfWork(
            AppDbContext context,
            ILoggerFactory loggerFactory)
        {
            _context = context;
            _logger = loggerFactory.CreateLogger("db_logs");
            ToDtoTranslator = new ToDtoTranslator();
            ToEntityTranslator = new ToEntityTranslator();
            Users = new UserRepository(_context, _logger);
            RefreshTokens = new RefreshTokenRepository(_context, _logger);
        }

        public IUserRepository Users {get; private set;}

        public IRefreshTokenRepository RefreshTokens {get; private set;}

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<UserDto>> TranslateListOfEntities(IEnumerable<User> users)
        {
            List<UserDto> listOfUsersDto = new List<UserDto>();
            users.ToList().ForEach(u => {
                var udto =  ToDtoTranslator.ToUserDtoTranslator(u);
                listOfUsersDto.Add(udto.Result);
            });
            return await Task.FromResult(listOfUsersDto);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}