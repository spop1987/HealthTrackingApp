using Microsoft.Extensions.Logging;
using MyHealthNotebook.DataService.IConfiguration;
using MyHealthNotebook.DataService.IRepository;
using MyHealthNotebook.DataService.Repository;
using MyHealthNotebook.Entities.DbSet;
using MyHealthNotebook.Entities.Dtos.Incoming;
using MyHealthNotebook.Entities.Translators;
using MyHealthNoteBook.DataService.Data;

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
        }

        public IUserRepository Users {get; private set;}

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }

        public Task<List<UserDto>> TranslateListOfEntities(IEnumerable<User> users)
        {
            List<UserDto> listOfUsersDto = new List<UserDto>();
            users.ToList().ForEach(u => {
                var udto =  ToDtoTranslator.ToUserDtoTranslator(u);
                listOfUsersDto.Add(udto.Result);
            });
            return Task.FromResult(listOfUsersDto);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}