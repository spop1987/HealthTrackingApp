using Microsoft.AspNetCore.Mvc;
using MyHealthNotebook.DataService.Services;
using MyHealthNotebook.Entities.DbSet;

namespace MyHealthNotebook.Api.Controllers
{
    [Route("api/controller")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public IActionResult AddUser(User user)
        {
            throw new NotImplementedException();
        }


    }
}