using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Headless.BasicPrompt
{
    public class UserService : IUserService
    {
        public static void Add(string username, string password)
        {
            if (_users == null)
                _users = new List<User>();

            _users.Add(new User { Id = Guid.NewGuid(), Username = username, Password = password });
        }

        static List<User> _users;

        public async Task<User> Authenticate(string username, string password)
        {
            var user = await Task.Run(() => _users.SingleOrDefault(x => x.Username == username && x.Password == password));

            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so return user details without password
            return user.Clean();
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await Task.Run(() => _users.Clean());
        }
    }
}
