using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Utilities
{
    public class AuthenticationToken
    {
        public string id_token
        {
            get; set;
        }
        public string provider_name
        {
            get; set;
        }
        public List<UserClaim> user_claims
        {
            get; set;
        }
        public string user_id
        {
            get; set;
        }
    }

    public class UserClaim
    {
        public string typ
        {
            get; set;
        }
        public string val
        {
            get; set;
        }
    }
}
