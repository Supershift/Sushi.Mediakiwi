using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Sushi.Mediakiwi.API.Transport.Requests;
using Sushi.Mediakiwi.API.Transport.Responses;
using Sushi.Mediakiwi.Data;
using System;
using System.Collections.Generic;
using System.Security.Claims;
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

        public async Task<SetPasswordResponse> SetPassword(SetPasswordRequest request)
        {
            // Check the mandatory request values
            if (request == null 
                || string.IsNullOrWhiteSpace(request.EmailAddress) 
                || string.IsNullOrWhiteSpace(request.ResetGuid)
                || string.IsNullOrWhiteSpace(request.Password)
                || Guid.TryParse(request.ResetGuid, out Guid userGuid) == false)
            {
                return new SetPasswordResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }

            // Check the password strength
            if (Data.Utility.IsStrongPassword(request.Password) == false)
            {
                return new SetPasswordResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Message = "This password is not strong enough"
                };
            }

            // Retrieve the user
            var user = await ApplicationUser.SelectOneByEmailAsync(request.EmailAddress).ConfigureAwait(false);

            // Check for user existence, empty reset key, or resetkey mismatch
            if (user?.ID > 0 == false || user.ResetKey.HasValue == false || user.ResetKey.Value.Equals(userGuid) == false)
            {
                return new SetPasswordResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                };
            }

            // Set the password
            user.ApplyPassword(request.Password);
            user.ResetKey = null;

            // Save the user
            await user.SaveAsync();

            return new SetPasswordResponse()
            {
                StatusCode = System.Net.HttpStatusCode.OK
            };
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

            var SetResetGuid = await user.SendForgotPasswordAsync(console, request.SendEmail).ConfigureAwait(false);
            if (SetResetGuid)
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
            // Remove cookie
            var visManager = new VisitorManager(_httpAccessor.HttpContext);
            var visitor = await visManager.SelectAsync();
            visitor.ProfileID = null;
            await visManager.SaveAsync(visitor);
            await visitor.SaveAsync();

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

            // Set cookie
            var visManager = new VisitorManager(_httpAccessor.HttpContext);
            var visitor = await visManager.SelectAsync();
            visitor.ProfileID = user.ID;
            await visManager.SaveAsync(visitor);

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
