using System;
using System.Reflection;

namespace Sushi.Mediakiwi.API
{
    public static class Common
    {
        public static string GetLabelFromResource(string fieldName, System.Globalization.CultureInfo culture)
        {
            System.Resources.ResourceManager manager = new System.Resources.ResourceManager("Sushi.Mediakiwi.Labels", typeof(Mediakiwi.CommonConfiguration).Assembly);
            return manager.GetString(fieldName, culture);
        }

        public static readonly string API_AUTHENTICATION_SCHEME = "MEDIAKIWI-API";
        public static readonly string API_AUTHENTICATION_ISSUER = "MEDIAKIWI";
        public static readonly string API_AUTHENTICATION_AUDIENCE = "MEDIAKIWI-USERS";
        public static readonly string API_AUTHENTICATION_KEY = "vQe0Fom3UZ0V4FnHJFeGMX7T0Onsott6XiajbqjiSHosqaHh4taLX5Vh7e0qrsX";  // TODO: replace with string from settings
        public static readonly string API_COOKIE_KEY = "MKAPI";
        public static readonly string API_HEADER_URL = "original-url";
        public static readonly string API_HTTPCONTEXT_URLRESOLVER = "MKUrlResolver";
        public static readonly string API_HTTPCONTEXT_CONSOLE = "MKConsole";
        public static readonly string API_HTTPCONTEXT_ITEM = "MKAPICALL";
        public static readonly string API_ASSEMBLY_NAME = Assembly.GetExecutingAssembly().GetName().Name;
        public static readonly string API_CORS_POLICY = "MKCorsPolicy";

        public static readonly int API_COOKIE_EXPIRATION_HOURS = 2;

        public const string MK_CONTROLLERS_PREFIX = "mkapi/";
        public const string API_USER_CONTEXT = "MKUSERCONTEXT";
    }
}
