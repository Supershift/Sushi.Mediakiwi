using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Authentication
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
}
