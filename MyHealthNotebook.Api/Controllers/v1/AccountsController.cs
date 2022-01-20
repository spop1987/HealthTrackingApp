using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MyHealthNotebook.Authentication.Configuration;
using MyHealthNotebook.DataService.IConfiguration;

namespace MyHealthNotebook.Api.Controllers.v1
{
    public class AccountsController : BaseController
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfig _jwtConfig;
        public AccountsController(
            IUnitOfWork unitOfWork,
            UserManager<IdentityUser> userManager,
            IOptionsMonitor<JwtConfig> optionMonitor) : base(unitOfWork)
        {
            _userManager = userManager;
            _jwtConfig = optionMonitor.CurrentValue;
        }
    }
}