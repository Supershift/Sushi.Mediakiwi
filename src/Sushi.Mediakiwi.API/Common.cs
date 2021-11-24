using System;

namespace Sushi.Mediakiwi.API
{
    public static class Common
    {
        public const string API_AUTHENTICATION_SCHEME = "MEDIAKIWI-API";
        public const string API_AUTHENTICATION_ISSUER = "MEDIAKIWI";
        public const string API_AUTHENTICATION_AUDIENCE = "MEDIAKIWI-USERS";
        public const string API_AUTHENTICATION_KEY = "vQe0Fom3UZ0V4FnHJFeGMX7T0Onsott6XiajbqjiSHosqaHh4taLX5Vh7e0qrsX";  // TODO: replace with string from settings
        public const string API_COOKIE_KEY = "MKAPI";
        public const int API_COOKIE_EXPIRATION_HOURS = 2;

        public const string MK_CONTROLLERS_PREFIX = "mkapi/";

        public const string API_USER_CONTEXT = "MKUSERCONTEXT";
    }
}
