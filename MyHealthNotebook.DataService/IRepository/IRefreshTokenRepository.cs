using MyHealthNotebook.Entities.DbSet;

namespace MyHealthNotebook.DataService.IRepository
{
    public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
    {
         Task<RefreshToken> GetByRefresToken(string refresToken);
         Task<bool> MarkRefreshTokenAsUsed(RefreshToken refreshToken);
    }
}