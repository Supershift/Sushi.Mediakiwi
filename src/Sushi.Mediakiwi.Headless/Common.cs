using Microsoft.AspNetCore.Http;

namespace Sushi.Mediakiwi.Headless
{
    public static class Common
    {       
        /// <summary>
            /// Gets the encryption key.
            /// </summary>
            /// <value>The encryption key.</value>
        internal static string EncryptionKey
        {
            get { return "wimserver"; }
        }

        public static string GetPassPhraseFromContext(HttpContext context)
        {
            string returnValue = string.Empty;

            if (context?.Request?.Headers?.ContainsKey("User-Agent") == true)
            {
                returnValue = context.Request.Headers["User-Agent"];
            }
            else if (context?.Request?.Headers?.ContainsKey("Request-Id") == true)
            {
                returnValue = context.Request.Headers["Request-Id"];
            }
            else if (context?.Request?.Host != null && context?.Request?.Host.HasValue == true)
            {
                returnValue = context?.Request?.Host.Value;
            }
            else
            {
                returnValue = EncryptionKey;
            }

            return returnValue;
        }
    }
}
