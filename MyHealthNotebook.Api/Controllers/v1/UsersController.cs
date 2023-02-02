using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyHealthNotebook.Configurations.Messages;
using MyHealthNotebook.DataService.IConfiguration;
using MyHealthNotebook.Entities.DbSet;
using MyHealthNotebook.Entities.Dtos.Generic;
using MyHealthNotebook.Entities.Dtos.Incoming;

namespace MyHealthNotebook.Api.Controllers.v1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : BaseController
    {
        public UsersController(IUnitOfWork unitOfWork,
                               UserManager<IdentityUser> userManager) : base(unitOfWork, userManager)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _unitOfWork.Users.All();
            var result = new Result<UserDto>();
            if(users == null)
            {
                result.Error = PopulateError(400,
                    ErrorMessages.Users.UserNotFound,
                    ErrorMessages.Generic.TypeBadRequest);
                 return BadRequest(result);
            }
            
            var listOfUsersDto = await _unitOfWork.TranslateListOfEntities(users);
            var resultDtos = new PagedResult<UserDto>
            {
                Page = 1,
                ResultPerPage = listOfUsersDto.Count(),
                ResultCount = listOfUsersDto.Count(),
                Content = listOfUsersDto,
            };
            return Ok(resultDtos);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(UserDto userDto)
        {
            var user = await _unitOfWork.ToEntityTranslator.ToUser(userDto, "");
            await _unitOfWork.Users.Add(user);
            await _unitOfWork.CompleteAsync();
            return Ok(user.Id);
        }
        
        [HttpGet]
        [Route("GetUser", Name = "GetUser")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = await _unitOfWork.Users.GetById(id);
            var result = new Result<UserDto>();
            if(user == null)
            {
                result.Error = PopulateError(400,
                    ErrorMessages.Users.UserNotFound,
                    ErrorMessages.Generic.ObjectNotFound);
                 return BadRequest(result);
            }

            var userDto = await _unitOfWork.ToDtoTranslator.ToUserDtoTranslator(user); 
            result.Content = userDto;
            return Ok(result);
        }

    }
}