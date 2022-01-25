using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyHealthNotebook.Authentication.Configuration;
using MyHealthNotebook.Entities.Dtos.Incoming;
using MyHealthNotebook.Entities.Dtos.Outgoing;
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

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto registrationDto)
        {
            // check the model or obj we are recieving is valid
            if(!ModelState.IsValid){
                return BadRequest(new UserRegistrationResponseDto{
                    Succes = false,
                    Errors = new List<string>{
                        "Invalid payload"
                    }
                });
            }
                
            
            // check if the email already exist
            var userExist = await _userManager.FindByEmailAsync(registrationDto.Email);
            if(userExist != null)
                return BadRequest(new UserRegistrationResponseDto{
                    Succes = false,
                    Errors = new List<string>{
                        "Invalid payload"
                    }
                });
            // Add the user
            var newUser = new IdentityUser{
                Email = registrationDto.Email,
                UserName = registrationDto.Email,
                EmailConfirmed = true, // todo build email functionality to send to the user to confirm email
            };
            // save the user to the table
            var isCreated = await _userManager.CreateAsync(newUser, registrationDto.Password);
            if(!isCreated.Succeeded)
                return BadRequest(new UserRegistrationResponseDto{
                    Succes = false,
                    Errors  = isCreated.Errors.Select(s => s.Description).ToList()
                });
            // adding user to database
            var user = await _unitOfWork.ToEntityTranslator.ToUser(registrationDto, newUser.Id);
            await _unitOfWork.Users.Add(user);
            await _unitOfWork.CompleteAsync();
                
            //create a jwt token
            var token = GenerateJwtToken(newUser);

            return Ok(new UserRegistrationResponseDto{
                Succes = true,
                Token = token
            });
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDto loginDto)
        {
            // check the model or obj we are recieving is valid
            if(!ModelState.IsValid){
                return BadRequest(new UserRegistrationResponseDto{
                    Succes = false,
                    Errors = new List<string>{
                        "Invalid payload"
                    }
                });
            }
            
            var userExist = await _userManager.FindByEmailAsync(loginDto.Email);
            if(userExist == null){
                return BadRequest(new UserLoginResponseDto{
                    Succes = false,
                    Errors = new List<string>{
                        "Invalid authentication request"
                    }
                });
            }

            // check if the user has a valid password
            var isCorrect = await _userManager.CheckPasswordAsync(userExist, loginDto.Password);
            if(!isCorrect){
                return BadRequest(new UserLoginResponseDto{
                    Succes = false,
                    Errors = new List<string>{
                        "Invalid authentication request"
                    }
                });
            }

            var jwtToken = GenerateJwtToken(userExist);
            return Ok(new UserLoginResponseDto{
                Succes = true,
                Token = jwtToken
            });
        }

        private string GenerateJwtToken(IdentityUser user)
        {
            // the handler is going to be responsible for creating the token
            var jwtHandler = new JwtSecurityTokenHandler();

            // Get the securit key
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new []{
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email), // unique id
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // used by the refresh token
                }),
                Expires = DateTime.UtcNow.AddHours(3), // todo update the expiration time to minutes
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature
                )
            };
            // generate the security token
            var token = jwtHandler.CreateToken(tokenDescriptor);
            // convert the security obj token into a string
            var jwtToken = jwtHandler.WriteToken(token);
            return jwtToken;
        }
    }
}