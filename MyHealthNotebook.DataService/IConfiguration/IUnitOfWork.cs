using MyHealthNotebook.DataService.IRepository;
using MyHealthNotebook.Entities.DbSet;
using MyHealthNotebook.Entities.Dtos.Incoming;
using MyHealthNotebook.Entities.Translators;

namespace MyHealthNotebook.DataService.IConfiguration
{
    public interface IUnitOfWork
    {
         IUserRepository Users {get;}
         IToDtoTranslator ToDtoTranslator {get; set;}
         IToEntityTranslator ToEntityTranslator {get; set;}
         Task CompleteAsync();
         Task<List<UserDto>> TranslateListOfEntities(IEnumerable<User> users);   
    }
}