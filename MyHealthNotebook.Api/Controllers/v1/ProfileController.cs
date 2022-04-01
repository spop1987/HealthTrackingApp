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
    public class ProfileController : BaseController
    {
        public ProfileController(
            IUnitOfWork unitOfWork,
            UserManager<IdentityUser> userManager) : base(unitOfWork, userManager)
        { }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var loggerInUser = await _userManager.GetUserAsync(HttpContext.User);
            var result = new Result<User>();
            if(loggerInUser == null) 
            {
                result.Error = PopulateError(400,
                    ErrorMessages.Profile.UserNotFound,
                    ErrorMessages.Generic.TypeBadRequest);
                return BadRequest(result);
            }

            var identityId = new Guid(loggerInUser.Id);
            var profile = await _unitOfWork.Users.GetByIdentityId(identityId);

            if(profile == null)
            {
                result.Error = PopulateError(400,
                    ErrorMessages.Profile.ProfileNotFound,
                    ErrorMessages.Generic.TypeBadRequest);
                return BadRequest(result);
            } 
            
            result.Content = profile;
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto profile)
        {
            var result = new Result<User>();
            if(!ModelState.IsValid)
            {
                result.Error = PopulateError(400,
                    ErrorMessages.Generic.InvalidPayload,
                    ErrorMessages.Generic.TypeBadRequest);
                return BadRequest(result);
            }

            var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);
            
            if(loggedInUser == null)
            {
                result.Error = PopulateError(
                    400,
                    ErrorMessages.Profile.UserNotFound,
                    ErrorMessages.Generic.TypeBadRequest);
                return BadRequest(result);
            } 

            var identityId = new Guid(loggedInUser.Id);

            var userProfile = await _unitOfWork.Users.GetByIdentityId(identityId);

            if(userProfile == null)
            {
                result.Error = PopulateError(
                    400,
                    ErrorMessages.Profile.UserNotFound,
                    ErrorMessages.Generic.TypeBadRequest);
                return BadRequest(result);
            }

            var userProfileForUpdate = await _unitOfWork.ToEntityTranslator.ToUserProfile(userProfile, profile);

            var isUpdated = await _unitOfWork.Users.UpdateUserProfile(userProfileForUpdate);

            if(isUpdated){
                await _unitOfWork.CompleteAsync();
                result.Content = userProfile;
                return Ok(result);
            } 

            result.Error = PopulateError(500,
                ErrorMessages.Generic.SomethingWentWrong,
                ErrorMessages.Generic.TypeUnableToProcess);
            
            return BadRequest(result);    
        }
    }
}