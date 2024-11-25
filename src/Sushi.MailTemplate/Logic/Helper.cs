using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sushi.MailTemplate.Logic
{
    /// <summary>
    /// Helper class to work with mail templates
    /// </summary>
    public static class Helper
    {
        internal static bool IsValidTemplate(string body, string subject)
        {
            return IsValid(body) && IsValid(subject);
        }

        internal static bool IsPlaceholderGroup(string match)
        {
            return match.StartsWith("[g:", StringComparison.OrdinalIgnoreCase); // placeholder group
        }

        internal static bool IsPlaceholder(string match)
        {
            return (
                !match.StartsWith("[if", StringComparison.OrdinalIgnoreCase) && // conditional css
                match.IndexOf("endif", StringComparison.OrdinalIgnoreCase) == -1 && // conditional css
                !match.StartsWith("[g:", StringComparison.OrdinalIgnoreCase) && // placeholder group
                !match.StartsWith("[/g:", StringComparison.OrdinalIgnoreCase) && // placeholder group
                !match.StartsWith("[unsubscribe]", StringComparison.OrdinalIgnoreCase) && // unsubscribe
                !match.StartsWith("[/unsubscribe]", StringComparison.OrdinalIgnoreCase) // unsubscribe
            );
        }

        internal static bool IsValidPlaceholder(string placeholder, System.IO.TextWriter logger)
        {
            var isValid = true;

            // cut off leading [ and trailing ]
            placeholder = placeholder.Substring(1, placeholder.Length - 2);

            var placeholderWithoutUnderscores = placeholder.Replace("_", string.Empty);
            if (!placeholderWithoutUnderscores.All(char.IsLetterOrDigit))
            {
                //not just letters and digits.
                isValid = false;
                logger?.WriteLine($"{placeholder} contains more than alphanumeric characters.");
            }

            return isValid;
        }

        internal static bool IsValidPlaceholderGroup(string placeholderGroup, System.IO.TextWriter logger)
        {
            // [g:rooms]<td>[NAME] [ROOMNUMBER]</td>[/g:rooms]
            var isValid = true;

            var openingTagOpeningBracketIndex = placeholderGroup.IndexOf("[g:", StringComparison.OrdinalIgnoreCase);
            var openingTagClosingBracketIndex = placeholderGroup.IndexOf("]");
            var closingTagOpeningBracketIndex = placeholderGroup.LastIndexOf("[/g:", StringComparison.OrdinalIgnoreCase);
            var closingTagClosingBracketIndex = placeholderGroup.LastIndexOf("]");

            var openingTagLength = openingTagClosingBracketIndex - openingTagOpeningBracketIndex - 3;
            var closingTagLength = closingTagClosingBracketIndex - closingTagOpeningBracketIndex - 4;

            var openingTag = placeholderGroup.Substring(openingTagOpeningBracketIndex + 3, openingTagLength);
            var closingTag = placeholderGroup.Substring(closingTagOpeningBracketIndex + 4, closingTagLength);

            if (!openingTag.Equals(closingTag, StringComparison.OrdinalIgnoreCase))
            {

                logger?.WriteLine($"Opening tag '{openingTag}' and closing tag '{closingTag}' are different.");
                isValid = false;
            }
            else
            {
                if (!openingTag.All(char.IsLetterOrDigit))
                {
                    //not just letters and digits.
                    isValid = false;

                    logger?.WriteLine($"{placeholderGroup} contains more than alphanumeric characters.");
                }
            }

            var textWithPlaceholderTagsLength = closingTagOpeningBracketIndex - openingTagClosingBracketIndex - 1;
            var textWithPlaceholderTags = placeholderGroup.Substring(openingTagClosingBracketIndex + 1, textWithPlaceholderTagsLength);

            ValidateTextWithPlaceholders(textWithPlaceholderTags, ref isValid, logger);

            return isValid;
        }

        /// <summary>
        /// Validates a string with placeholder tags
        /// </summary>
        /// <param name="textWithPlaceholderTags"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static bool IsValid(string textWithPlaceholderTags, System.IO.TextWriter logger = null)
        {
            var isValid = true;

            if (string.IsNullOrWhiteSpace(textWithPlaceholderTags))
            {
                // skip validation for empty text
                return true;
            }
            
            ValidateTextWithPlaceholders(textWithPlaceholderTags, ref isValid, logger);
            ValidateTextWithPlaceholderGroups(textWithPlaceholderTags, ref isValid, logger);

            return isValid;
        }

        private static void ValidateTextWithPlaceholderGroups(string textWithPlaceholderGroupTags, ref bool isValid, System.IO.TextWriter logger)
        {
            var regGroup = new Regex(PlaceholderLogic.PatternGroup, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var groupMatches = regGroup.Matches(textWithPlaceholderGroupTags);
            foreach (Match match in groupMatches)
            {
                logger?.WriteLine($"Checking: {match.Value}");
                // if it is a placeholder group then check if it is valid
                if (IsPlaceholderGroup(match.Value) && !IsValidPlaceholderGroup(match.Value, logger))
                {
                    logger?.WriteLine($"Not valid: {match.Value}");

                    isValid = false;
                }
            }
        }

        private static void ValidateTextWithPlaceholders(string textWithPlaceholderTags, ref bool isValid, System.IO.TextWriter logger)
        {
            var pattern = PlaceholderLogic.Pattern;

            var reg = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var matches = reg.Matches(textWithPlaceholderTags);

            foreach (Match match in matches)
            {

                logger?.WriteLine($"Checking: {match.Value}");
                // if it is a placeholder then check if it is valid
                if (IsPlaceholder(match.Value) && !IsValidPlaceholder(match.Value, logger))
                {

                    logger?.WriteLine($"Not valid: {match.Value}");
                    isValid = false;
                }
            }
        }

        internal static string ReplaceLegacyUnsubscribe(string input)
        {
            if (!string.IsNullOrWhiteSpace(input) && input.Contains("[/unsubscribe]"))
            {
                // legacy detected, replace [unsubscribe]here[/unsubscribe] with <a href="[unsubscribe]">here</a>
                input = input
                    .Replace("[unsubscribe]", "<a href=\"[unsubscribe]\">", StringComparison.OrdinalIgnoreCase)
                    .Replace("[/unsubscribe]", "</a>", StringComparison.OrdinalIgnoreCase);
            }

            return input;
        }
    }
}