using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Utilities.BasicAuthentication
{
    public interface IUserService
    {
        Task<User> Authenticate(string username, string password);
        Task<IEnumerable<User>> GetAll();
    }
}
