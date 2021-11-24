using Microsoft.AspNetCore.Mvc;

namespace Sushi.Mediakiwi.API
{
    public class AuthenticatedMediakiwiApi : ControllerBase
    {
        protected Data.IApplicationUser User
        {
            get
            {
                if (HttpContext.Items.TryGetValue(Common.API_USER_CONTEXT, out object userObj) && userObj is Data.IApplicationUser)
                {
                    return (Data.IApplicationUser)userObj;
                }
                else 
                {
                    return default;
                }
            }
        }
    }
}
