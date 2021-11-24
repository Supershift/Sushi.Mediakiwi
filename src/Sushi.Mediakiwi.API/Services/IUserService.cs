using Sushi.Mediakiwi.API.Transport.Requests;
using Sushi.Mediakiwi.API.Transport.Responses;
using Sushi.Mediakiwi.Data;
using System;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.API.Services
{
    public interface IUserService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        
        Task<ResetPasswordResponse> ResetPassword(ResetPasswordRequest request);

        Task<IApplicationUser> GetUser(Guid userGuid);
    }
}
