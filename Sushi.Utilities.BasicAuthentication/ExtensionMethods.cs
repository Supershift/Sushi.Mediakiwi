using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sushi.Utilities.BasicAuthentication
{
    public static class ExtensionMethods
    {
        public static IEnumerable<User> Clean(this IEnumerable<User> users)
        {
            return users.Select(x => x.Clean());
        }

        public static User Clean(this User user)
        {
            return new User() { Id = user.Id, Username = user.Username };
        }
    }
}
