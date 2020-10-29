using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// 
    /// </summary>
    public class ErrorCode
    {
        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="languageCulture">The language culture.</param>
        /// <returns></returns>
        public static string GetMessage(int code, string languageCulture)
        {
            return string.Format("{0}: {1}",
                code,
                Labels.ResourceManager.GetString(string.Concat("err_", code), new CultureInfo(languageCulture))
                );
        }

        /// <summary>
        /// Gets the culture text.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="languageCulture">The language culture.</param>
        public static string GetCultureText(string value, string languageCulture)
        {
            return Labels.ResourceManager.GetString(value, new CultureInfo(languageCulture));
        }
    }
}
