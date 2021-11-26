using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Sushi.Mediakiwi.API.Transport.Requests;
using Sushi.Mediakiwi.API.Transport.Responses;
using Sushi.Mediakiwi.Data;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.API.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpAccessor;
        public UserService(IHttpContextAccessor _accessor)
        {
            _httpAccessor = _accessor;
        }

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

        public async Task LogoutAsync() 
        {
            await _httpAccessor.HttpContext.SignOutAsync().ConfigureAwait(false);
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

            var claims = new List<Claim>
            {
                new Claim("id", user.Displayname),
                new Claim("guid", user.GUID.ToString()),
                new Claim("email", user.Email),
                new Claim("apiKey", request.ApiKey),
                new Claim("roleId", user.RoleID.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTime.UtcNow.AddHours(Common.API_COOKIE_EXPIRATION_HOURS),
                IsPersistent = true,
                IssuedUtc = DateTime.UtcNow,
            };

            await _httpAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties).ConfigureAwait(false);

            return new LoginResponse()
            {
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
    }
}
