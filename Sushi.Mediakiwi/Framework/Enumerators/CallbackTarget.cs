using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Framework
{
    public enum CallbackTarget
    {
        //  Directly after the application user has logged in, but not written to the cookie
        POST_SIGNIN,
        //  Directly before the application user is validated against the user IDP
        PRE_SIGNIN
    }
}
