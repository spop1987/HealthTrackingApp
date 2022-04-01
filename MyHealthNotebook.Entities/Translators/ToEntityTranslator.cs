using MyHealthNotebook.Entities.DbSet;
using MyHealthNotebook.Entities.Dtos.Incoming;

namespace MyHealthNotebook.Entities.Translators
{
    public interface IToEntityTranslator
    {
        Task<User> ToUser(IUserDto userDto, string userId);
        Task<User> ToUserProfile(User userProfile, UpdateProfileDto profile);
    }
    public class ToEntityTranslator : IToEntityTranslator
    {
        public async Task<User> ToUser(IUserDto userDto, string userId)
        {
            var user = new User();
            user.IdentityId = string.IsNullOrWhiteSpace(userId) ? Guid.NewGuid() :  new Guid(userId);
            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.Email = userDto.Email;
            user.Phone = "";
            user.DateOfBirth = DateTime.UtcNow;
            user.Country = "";
            user.Address = "";
            user.MobileNumber = "";
            user.Sex = "";
            return await Task.FromResult(user);
        }

        public async Task<User> ToUserProfile(User userProfile, UpdateProfileDto profile)
        {
            userProfile.Phone = profile.Phone;
            userProfile.Country = profile.Country;
            userProfile.Address = profile.Address;
            userProfile.MobileNumber = profile.MobileNumber;
            userProfile.Sex = profile.Sex;
            userProfile.DateOfBirth = DateTime.Parse(profile.DateOfBirth);

            return await Task.FromResult(userProfile);
        }
    }

}