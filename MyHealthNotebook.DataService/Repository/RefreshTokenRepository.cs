using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyHealthNotebook.DataService.IRepository;
using MyHealthNotebook.Entities.DbSet;
using MyHealthNoteBook.DataService.Data;

namespace MyHealthNotebook.DataService.Repository
{
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(AppDbContext context, ILogger logger) : base(context, logger)
        {
        }

        public async Task<RefreshToken> GetByRefresToken(string refreshToken)
        {
            try
            {
                var refreshTokenInDb = await dbSet.Where(x => x.Token.ToUpper() == refreshToken.ToUpper())
                        .AsNoTracking().FirstOrDefaultAsync();
                
                if(refreshTokenInDb != null)
                    return refreshTokenInDb;

                return new RefreshToken();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} GetByRefreshToken method has generated an error", typeof(UserRepository));
                return new RefreshToken();
            }
        }

        public async Task<bool> MarkRefreshTokenAsUsed(RefreshToken refreshToken)
        {
            try
            {
                var token = await dbSet.Where(x => x.Token.ToUpper() == refreshToken.Token.ToUpper())
                        .AsNoTracking().FirstOrDefaultAsync();
                
                if(token == null) return false;

                token.IsUsed = refreshToken.IsUsed;
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} MarkRefreshTokenAsUsed method has generated an error", typeof(UserRepository));
                return false;
            }
        }
    }
}