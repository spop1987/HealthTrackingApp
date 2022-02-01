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
using MyHealthNotebook.Entities.DbSet;
using MyHealthNotebook.Entities.Generic;

namespace MyHealthNotebook.Api.Controllers.v1
{
    public class AccountsController : BaseController
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly JwtConfig _jwtConfig;
        public AccountsController(
            IUnitOfWork unitOfWork,
            UserManager<IdentityUser> userManager,
            TokenValidationParameters tokenValidationParameters,
            IOptionsMonitor<JwtConfig> optionMonitor) : base(unitOfWork)
        {
            _userManager = userManager;
            _jwtConfig = optionMonitor.CurrentValue;
            _tokenValidationParameters = tokenValidationParameters;
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
                        "This e-mail is being used"
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
            var token = await GenerateJwtToken(newUser);

            return Ok(new UserRegistrationResponseDto{
                Succes = true,
                Token = token.JwtToken,
                RefreshToken = token.RefreshToken
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

            var jwtToken = await GenerateJwtToken(userExist);
            return Ok(new UserLoginResponseDto{
                Succes = true,
                Token = jwtToken.JwtToken,
                RefreshToken = jwtToken.RefreshToken
            });
        }

        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequest tokenRequest)
        {
            if(!ModelState.IsValid){
                return BadRequest(new UserRegistrationResponseDto{
                    Succes = false,
                    Errors = new List<string>{
                        "Invalid payload"
                    }
                });
            }

            var result = await VerifyToken(tokenRequest);
            if(result == null){
                return BadRequest(new UserRegistrationResponseDto{
                    Succes = false,
                    Errors = new List<string>(){
                        "token validation failed"
                    }
                });
            }

            return Ok(result); 
        }

        private async Task<AuthResult> VerifyToken(TokenRequest tokenRequest)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                // we need check the validity of this token
                var principal = tokenHandler.ValidateToken(tokenRequest.Token, _tokenValidationParameters, out var validatedToken);
                
                // We need to valdiate the results that has been generated for us
                // Validate if the string is an actual JWT token not a random string
                if(validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    // check if the jwt token is created with the same algorithm as our jwttoken
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

                    if(!result)
                        return null;
                }

                // we nned to check the expiry date of the token
                var utcExpiryDate = long.Parse(principal.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                // convert to date to check
                var expDate = UnixTimeStampToDateTime(utcExpiryDate);

                // check if the jwt token has expired
                if(expDate > DateTime.UtcNow)
                {
                    return new AuthResult(){
                        Succes = false,
                        Errors = new List<string>(){
                            "Jwt token has not expired"
                        }
                    };
                }

                // check if the refresh token exists
                var refreshTokenExist = await _unitOfWork.RefreshTokens.GetByRefresToken(tokenRequest.RefreshToken);

                if(refreshTokenExist.Token == null){
                    return new AuthResult(){
                        Succes = false,
                        Errors = new List<string>(){
                            "Invalid Refresh Token"
                        }
                    };
                }

                // check the expiry date of a refresh token
                if(refreshTokenExist.ExpiryDate < DateTime.UtcNow)
                {
                    return new AuthResult(){
                        Succes = false,
                        Errors = new List<string>(){
                            "Refresh Token has expired"
                        }
                    };
                }

                // check if the refresh token has been used or not
                if(refreshTokenExist.IsUsed)
                {
                    return new AuthResult(){
                        Succes = false,
                        Errors = new List<string>(){
                            "Refresh Token has been used, it cannot be reused"
                        }
                    };
                }

                // check if the refresh token it has been revoked
                if(refreshTokenExist.IsRevoked)
                {
                    return new AuthResult(){
                        Succes = false,
                        Errors = new List<string>(){
                            "Refresh Token has been revoked, it cannot be used"
                        }
                    };
                }

                var jti = principal.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                
                if(refreshTokenExist.JwtId != jti){
                    return new AuthResult(){
                        Succes = false,
                        Errors = new List<string>(){
                            "Refresh Token reference does not match the jwt token"
                        }
                    };
                }

                // start processing and get a new token 
                refreshTokenExist.IsUsed = true;

                var updateResult = await _unitOfWork.RefreshTokens.MarkRefreshTokenAsUsed(refreshTokenExist);
                if(!updateResult){
                    return new AuthResult(){
                        Succes = false,
                        Errors = new List<string>(){
                            "Refresh Token reference does not match the jwt token"
                        }
                    };
                }
                
                await _unitOfWork.CompleteAsync();
                var dbUser = await _userManager.FindByIdAsync(refreshTokenExist.UserId);

                if(dbUser == null)
                {
                    return new AuthResult(){
                        Succes = false,
                        Errors = new List<string>(){
                            "Error processing request"
                        }
                    };
                }
                // generate a jwt token
                var tokens = await GenerateJwtToken(dbUser);

                return new AuthResult{
                    Token = tokens.JwtToken,
                    Succes = true,
                    RefreshToken = tokens.RefreshToken
                };
            }
            catch (Exception)
            {
                // TODO: Add better error handling, and add a logger
                return null;
            }
        }

        private DateTime UnixTimeStampToDateTime(long utcExpiryDate)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(utcExpiryDate).ToUniversalTime();
            return dateTime;
        }

        private async Task<TokenData> GenerateJwtToken(IdentityUser user)
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
                Expires = DateTime.UtcNow.Add(_jwtConfig.ExpiryTimeFrame), // todo update the expiration time to minutes
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature
                )
            };
            // generate the security token
            var token = jwtHandler.CreateToken(tokenDescriptor);
            // convert the security obj token into a string
            var jwtToken = jwtHandler.WriteToken(token);
            // Generate a refresh token
            var refreshToken = new RefreshToken{
                AddedDate = DateTime.UtcNow,
                Token = $"{RandomStringGenerator(25)}_{Guid.NewGuid()}",
                UserId = user.Id,
                IsRevoked = false,
                IsUsed = false,
                Status = 1,
                JwtId = token.Id,
                ExpiryDate = DateTime.UtcNow.AddMinutes(10),
            };

            try
            {
                var isAdded = await _unitOfWork.RefreshTokens.Add(refreshToken);
                if(isAdded)
                    await _unitOfWork.CompleteAsync();
            }
            catch (Exception e)
            {
                throw new Exception("Problemas al adicionar:" + e);
            } 

            var tokenData = new TokenData{
                JwtToken = jwtToken,
                RefreshToken = refreshToken.Token
            };

            return tokenData;
        }

        private string RandomStringGenerator(int length)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var stringToReturn = new string(Enumerable.Repeat(chars, length)
                        .Select(s => s[random.Next(s.Length)]).ToArray());
            
            return stringToReturn;
        }
    }
}