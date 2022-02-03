using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyHealthNotebook.DataService.IConfiguration;
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

            if(loggerInUser == null) return BadRequest("User not found");

            var identityId = new Guid(loggerInUser.Id);
            var profile = await _unitOfWork.Users.GetByIdentityId(identityId);

            if(profile == null) return BadRequest("User not found");

            return Ok(profile);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto profile)
        {
            if(!ModelState.IsValid) return BadRequest("Invalid payload");

            var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);
            
            if(loggedInUser == null) return BadRequest("User not found");

            var identityId = new Guid(loggedInUser.Id);

            var userProfile = await _unitOfWork.Users.GetByIdentityId(identityId);

            if(userProfile == null) return BadRequest("User not found");

            userProfile.Phone = profile.Phone;
            userProfile.Country = profile.Country;
            userProfile.Address = profile.Address;
            userProfile.MobileNumber = profile.MobileNumber;
            userProfile.Sex = profile.Sex;
            userProfile.DateOfBirth = DateTime.Parse(profile.DateOfBirth);

            var isUpdated = await _unitOfWork.Users.UpdateUserProfile(userProfile);

            if(isUpdated){
                await _unitOfWork.CompleteAsync();
                return Ok(userProfile);
            } 
                

            return BadRequest("Something went wrong");
        }
    }
}