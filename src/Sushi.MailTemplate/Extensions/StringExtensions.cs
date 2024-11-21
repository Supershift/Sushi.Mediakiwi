using System;

namespace Sushi.MailTemplate.Extensions
{
    internal static class StringExtensions
    {
        public static string Replace(this string input, string oldValue, string newValue, StringComparison comparisonType)
        {
            if ((comparisonType & StringComparison.CurrentCultureIgnoreCase & StringComparison.InvariantCultureIgnoreCase & StringComparison.OrdinalIgnoreCase) == 0)
            {
                // case sensitive, so default
                return input.Replace(oldValue, newValue);
            }

            var output = input;
            var start = output.IndexOf(oldValue, comparisonType);
            if (start > -1)
            {
                var length = oldValue.Length;
                oldValue = output.Substring(start, length);
                output = output.Replace(oldValue, newValue);
            }

            return output;
        }
    }
}