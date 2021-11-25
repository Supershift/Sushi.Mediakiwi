using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Sushi.Mediakiwi.API.Transport.Requests;
using Sushi.Mediakiwi.API.Transport.Responses;
using Sushi.Mediakiwi.Data;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Sushi.Mediakiwi.API.Services
{
    public class UserService : IUserService
    {
        public async Task<ResetPasswordResponse> ResetPassword(ResetPasswordRequest request, Beta.GeneratedCms.Console console)
        {
            var user = await ApplicationUser.SelectOneByEmailAsync(request.EmailAddress).ConfigureAwait(false);
            
            // return Unauthorized if user not found
            if (user == null || user.ID == 0)
            {
                return new ResetPasswordResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                };
            }

            if (console == null)
            {
                return new ResetPasswordResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }

            var emailed = await user.SendForgotPasswordAsync(console).ConfigureAwait(false);
            if (emailed)
            {
                return new ResetPasswordResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            else 
            {
                return new ResetPasswordResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.FailedDependency
                };
            }
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await ApplicationUser.SelectOneByEmailAsync(request.EmailAddress).ConfigureAwait(false);

            // return Unauthorized if user not found
            if (user == null || user.ID == 0 || user.IsValid(request.Password) != true)
            {
                return new LoginResponse() 
                { 
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                };
            }

            // authentication successful so generate jwt token
            var jwtToken = GenerateJwtToken(user.Displayname, user.Email, user.GUID, request.ApiKey, user.RoleID);

            return new LoginResponse()
            {
                JwtToken = jwtToken,
                Message = "Login succesful",
                StatusCode = System.Net.HttpStatusCode.OK,
                UserEmail = user.Email,
                UserName = user.Displayname,
            };
        }

        public async Task<IApplicationUser> GetUser(Guid userGuid)
        { 
            return await ApplicationUser.SelectOneAsync(userGuid).ConfigureAwait(false);
        }

        private string GenerateJwtToken(string userName, string email, Guid userGuid, string apiKey, int roleId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Common.API_AUTHENTICATION_KEY);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] 
                { 
                    new Claim("id", userName),
                    new Claim("guid", userGuid.ToString()),
                    new Claim("email", email),
                    new Claim("apiKey", apiKey),
                    new Claim("roleId", roleId.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(Common.API_COOKIE_EXPIRATION_HOURS),
                Issuer = Common.API_AUTHENTICATION_ISSUER,
                Audience = Common.API_AUTHENTICATION_AUDIENCE,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
