using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Sushi.MailTemplate.Entities;

namespace Sushi.MailTemplate.Logic
{
    /// <summary>
    /// PlaceholderLogic class with logic for placeholders
    /// </summary>
    public class PlaceholderLogic
    {

        internal const string PatternSection = @"\[section:.+?\].+?\[/section:.+?\]";
        internal const string PatternGroup = @"\[g:.+?\].+?\[/g:.+?\]";

        /// <summary>
        /// Pattern searches for [, then a character in A-Z or 0-9, ending with ]
        /// Matches [T], [TEST123]
        /// </summary>
        internal const string Pattern = @"\[[A-Z0-9]*?\]";
        private readonly Data.MailTemplateListRepository _mailTemplateListRepository;
        private readonly Data.DefaultValuePlaceholderRepository _defaultValuePlaceholderRepository;

        // PlaceholderTag = [NAME]
        // PlaceholderReplacer = [PlaceholderTagName, PlaceholderValue]
        // PlaceholderValue = "Pietje Puk"
        // PlaceholderGroupTag = [g:rooms]<td>[NAME] in room [ROOM]</td>[/g:rooms]
        // PlaceholderGroupReplacements = [PlaceholderGroupTagName, [PlaceholderTagName, PlaceholderValue]]

        public PlaceholderLogic(Data.MailTemplateListRepository mailTemplateListRepository, Data.DefaultValuePlaceholderRepository defaultValuePlaceholderRepository)
        {
            _mailTemplateListRepository = mailTemplateListRepository;
            _defaultValuePlaceholderRepository = defaultValuePlaceholderRepository;
        }

        /// <summary>
        /// Provide a mail template ID, placeholder group replacements, placeholder replacements and optional sections to include
        /// </summary>
        /// <param name="mailTemplateIdentifier"></param>
        /// <param name="placeholderGroupReplacements"></param>
        /// <param name="placeholderReplacements"></param>
        /// <param name="optionalSectionsReplacements"></param>
        /// <param name="logger"></param>
        /// <returns>MailTemplate with Body and Subject replaced</returns>
        public async Task<Data.MailTemplate> ApplyPlaceholdersAsync(string mailTemplateIdentifier, List<PlaceholderGroup> placeholderGroupReplacements = null, List<Placeholder> placeholderReplacements = null, List<string> optionalSectionsReplacements = null, System.IO.TextWriter logger = null)
        {
            var mailTemplate = await _mailTemplateListRepository.FetchSingleByIdentifierAsync(mailTemplateIdentifier);
            if (mailTemplate != null)
            {
                return await ApplyPlaceholdersAsync(mailTemplate, placeholderGroupReplacements, placeholderReplacements, optionalSectionsReplacements, logger);
            }

            return mailTemplate;
        }

        /// <summary>
        /// Provide a mail template object, placeholder group replacements, placeholder replacements and optional sections to include.
        /// </summary>
        /// <param name="textWithPlaceholderTags"></param>
        /// <param name="logger"></param>
        /// <param name="placeholderGroupReplacements"></param>
        /// <param name="placeholderReplacements"></param>
        /// <param name="listOfDefaultValues"></param>
        /// <param name="optionalSectionsToInclude"></param>
        /// <returns></returns>
        public static string ApplyPlaceholders(string textWithPlaceholderTags, System.IO.TextWriter logger, List<PlaceholderGroup> placeholderGroupReplacements = null, List<Placeholder> placeholderReplacements = null, List<Data.DefaultValuePlaceholder> listOfDefaultValues = null, List<string> optionalSectionsToInclude = null)
        {
            if (string.IsNullOrWhiteSpace(textWithPlaceholderTags))
            {
                return string.Empty;
            }

            placeholderGroupReplacements ??= [];
            placeholderReplacements ??= [];
            listOfDefaultValues ??= [];
            optionalSectionsToInclude ??= [];

            var result = textWithPlaceholderTags;

            // start with unsubscribe tags to prevent accidental replacement via regex
            result = ReplaceUnsubscribe(result, logger);

            #region replace optional sections
            // replace the placeholders with empty strings so normal processing of placeholders can take place
            foreach (var optionalSectionToInclude in optionalSectionsToInclude)
            {
                result = result
                    .Replace($"[section:{optionalSectionToInclude}]", string.Empty, StringComparison.OrdinalIgnoreCase)
                    .Replace($"[/section:{optionalSectionToInclude}]", string.Empty, StringComparison.OrdinalIgnoreCase);
            }

            // replace all complete blocks that are left and not specified
            var regSections = new Regex(PatternSection, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var foundSections = regSections.Matches(result);
            foreach (Match sectionMatch in foundSections)
            {
                var replaceBlock = sectionMatch.Value;
                result = result.Replace(replaceBlock, string.Empty, StringComparison.OrdinalIgnoreCase);
            }
            #endregion

            var regGroup = new Regex(PatternGroup, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            // create a list of repeater placeholders
            var foundGroupTags = regGroup.Matches(result).OfType<Match>().ToList();

            foreach (var placeholderGroupReplacement in placeholderGroupReplacements)
            {
                string placeholderGroupTagName = placeholderGroupReplacement.Name;
                List<PlaceholderRow> placeholderRows = placeholderGroupReplacement.PlaceholderRows;

                string patternGroupForPlaceholderGroup = $@"\[g:{placeholderGroupTagName}\].+?\[/g:{placeholderGroupTagName}\]";
                Regex regGroupForPlaceholderGroup = new Regex(patternGroupForPlaceholderGroup, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                MatchCollection matchesGroupForPlaceholderGroup = regGroupForPlaceholderGroup.Matches(result);

                if (matchesGroupForPlaceholderGroup.Count > 0)
                {
                    // find the foundGroupTag that matches the placeholderGroup that we're currently processing
                    var regItem = new Regex(patternGroupForPlaceholderGroup, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    Match placeholderGroupTag = foundGroupTags.Find(x => regItem.Match(x.Value).Length > 0);

                    if (placeholderGroupTag == null)
                        continue;

                    // example [g:rooms]<td>Room [ROOMTYPE] for [LASTNAME] </td>[/g:rooms]
                    var replaceBlock = placeholderGroupTag.Value;

                    var replacementList = new List<string>();

                    foreach (var placeholderRow in placeholderRows)
                    {
                        var replacedRow = placeholderGroupTag.Value;

                        foreach (var placeholder in placeholderRow.Placeholders)
                        {
                            if (replacedRow.IndexOf($"[{placeholder.Name}]", StringComparison.OrdinalIgnoreCase) > -1 && !placeholder.Name.Equals("unsubscribe", StringComparison.OrdinalIgnoreCase))
                            {
                                logger?.WriteLine($"replacing {placeholder.Name} with {placeholder.Value}");
                                replacedRow = replacedRow.Replace($"[{placeholder.Name}]", placeholder.Value, StringComparison.OrdinalIgnoreCase);
                            }

                        }

                        replacementList.Add(replacedRow);
                    }

                    var replacedBlock = string.Concat(replacementList);

                    replacedBlock = replacedBlock
                        .Replace($"[g: {placeholderGroupTagName}]", string.Empty, StringComparison.OrdinalIgnoreCase)
                        .Replace($"[/g: {placeholderGroupTagName}]", string.Empty, StringComparison.OrdinalIgnoreCase)
                        .Replace($"[g:{placeholderGroupTagName}]", string.Empty, StringComparison.OrdinalIgnoreCase)
                        .Replace($"[/g:{placeholderGroupTagName}]", string.Empty, StringComparison.OrdinalIgnoreCase);

                    result = result.Replace(replaceBlock, replacedBlock, StringComparison.OrdinalIgnoreCase);

                }
            }

            foreach (var placeholder in placeholderReplacements)
            {
                // replaces all [placeholder] with their respective value, only if value is not null
                if (placeholder.Value != null && !placeholder.Name.Equals("unsubscribe", StringComparison.OrdinalIgnoreCase))
                {
                    result = result.Replace($"[{placeholder.Name}]", placeholder.Value, StringComparison.OrdinalIgnoreCase);
                    logger?.WriteLine($"replacing {placeholder.Name} with {placeholder.Value}");
                }
            }


            // replace defaults with values from database
            foreach (var defaultValue in listOfDefaultValues)
            {
                if (result.IndexOf($"[{defaultValue.Placeholder}]", StringComparison.OrdinalIgnoreCase) > -1 && !defaultValue.Placeholder.Equals("unsubscribe", StringComparison.OrdinalIgnoreCase))
                {
                    logger?.WriteLine($"replacing {defaultValue.Placeholder} with {defaultValue.Value}");
                    result = result.Replace($"[{defaultValue.Placeholder}]", defaultValue.Value, StringComparison.OrdinalIgnoreCase);
                }
            }

            // loop all matches to replace remaining placeholders
            var reg = new Regex(Pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var matches = reg.Matches(result);

            foreach (Match match in matches)
            {
                if (Helper.IsPlaceholder(match.Value))
                {
                    var replaceBlock = match.Value;

                    var oldResult = result;
                    result = result.Replace(replaceBlock, string.Empty, StringComparison.OrdinalIgnoreCase);

                    if (logger != null && result != oldResult)
                    {
                        // change happened
                        logger.WriteLine($"replacing {replaceBlock} with empty string");
                    }
                }
            }

            var groups = GetPlaceholderGroups(result, logger);
            foreach (var group in groups)
            {
                result = result.Replace(group, string.Empty, StringComparison.OrdinalIgnoreCase);
            }

            return result;
        }

        /// <summary>
        /// Provide a mail template object, placeholder group replacements, placeholder replacements and optional sections to include
        /// </summary>
        /// <param name="mailTemplate"></param>
        /// <param name="placeholderGroupReplacements"></param>
        /// <param name="placeholderReplacements"></param>
        /// <param name="optionalSectionsToInclude"></param>
        /// <param name="logger"></param>
        /// <returns>MailTemplate with Body and Subject replaced</returns>
        public async Task<Data.MailTemplate> ApplyPlaceholdersAsync(Data.MailTemplate mailTemplate, List<PlaceholderGroup> placeholderGroupReplacements = null, List<Placeholder> placeholderReplacements = null, List<string> optionalSectionsToInclude = null, System.IO.TextWriter logger = null)
        {
            var listOfDefaultValues = await _defaultValuePlaceholderRepository.FetchAllByMailTemplateAsync(mailTemplate.Identifier);

            mailTemplate.Body = HttpUtility.HtmlDecode(ApplyPlaceholders(mailTemplate.Body, logger, placeholderGroupReplacements, placeholderReplacements, listOfDefaultValues, optionalSectionsToInclude));
            mailTemplate.Subject = HttpUtility.HtmlDecode(ApplyPlaceholders(mailTemplate.Subject, logger, placeholderGroupReplacements, placeholderReplacements, listOfDefaultValues, optionalSectionsToInclude));

            return mailTemplate;
        }

        /// <summary>
        /// Get a list of all section tags and specify if the brackets are to be included
        /// </summary>
        /// <param name="textWithSectionTags"></param>
        /// <param name="mustStripBrackets"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static List<string> GetSectionTags(string textWithSectionTags, bool mustStripBrackets = true, System.IO.TextWriter logger = null)
        {
            if (string.IsNullOrWhiteSpace(textWithSectionTags))
            {
                logger?.WriteLine("Text with placeholder tags is empty");
                return [];
            }

            var reg = new Regex(PatternSection, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var matches = reg.Matches(textWithSectionTags);
            var sectionList = new List<string>();
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    var sectionTag = match.Value;

                    var firstClosingBracket = sectionTag.IndexOf("]");
                    sectionTag = sectionTag.Substring(0, firstClosingBracket + 1).Replace("section:", string.Empty, StringComparison.OrdinalIgnoreCase);

                    if (mustStripBrackets)
                    {
                        var strippedValue = StripBrackets(sectionTag);
                        sectionTag = strippedValue;
                    }

                    if (!sectionList.Contains(sectionTag))
                    {
                        sectionList.Add(sectionTag);
                    }
                }
            }

            logger?.WriteLine("({0} sections):", sectionList.Count);

            return sectionList;

        }

        /// <summary>
        /// Get a list of all placeholder tags and specify if the brackets are to be included
        /// </summary>
        /// <param name="textWithPlaceholderTags"></param>
        /// <param name="mustStripBrackets"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static List<string> GetPlaceholderTags(string textWithPlaceholderTags, bool mustStripBrackets = true, System.IO.TextWriter logger = null)
        {
            if (string.IsNullOrWhiteSpace(textWithPlaceholderTags))
            {
                logger?.WriteLine("Text with placeholder tags is empty");
                return [];
            }

            var reg = new Regex(Pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var matches = reg.Matches(textWithPlaceholderTags);
            var placeholderList = new List<string>();
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    if (Helper.IsPlaceholder(match.Value) && !placeholderList.Contains(match.Value))
                    {
                        var placeholderTag = match.Value;
                        if (mustStripBrackets)
                        {
                            var strippedValue = StripBrackets(match.Value);
                            placeholderTag = strippedValue;
                        }

                        if (!placeholderList.Contains(placeholderTag))
                        {
                            placeholderList.Add(placeholderTag);
                        }
                    }
                }
            }

            logger?.WriteLine("({0} placeholders):", placeholderList.Count);

            return placeholderList;

        }

        /// <summary>
        /// Returns a list of placeholder groups with placeholders
        /// </summary>
        /// <param name="textWithPlaceholderTags"></param>
        /// <param name="mustStripBrackets"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static List<PlaceholderGroup> GetPlaceholderGroupsWithPlaceholders(string textWithPlaceholderTags, bool mustStripBrackets = true, System.IO.TextWriter logger = null)
        {
            if (string.IsNullOrWhiteSpace(textWithPlaceholderTags))
            {
                logger?.WriteLine("Text with placeholder tags is empty");
                return [];
            }

            var regGroup = new Regex(PatternGroup, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var matchesGroup = regGroup.Matches(textWithPlaceholderTags);
            var placeholderGroupList = new List<PlaceholderGroup>();

            if (matchesGroup.Count > 0)
            {
                logger?.WriteLine("({0} matches):", matchesGroup.Count);
                foreach (Match match in matchesGroup)
                {
                    var strippedGroup = StripBracketsFromGroup(match.Value);
                    var placeholderTags = Logic.PlaceholderLogic.GetPlaceholderTags(match.Value, logger: logger);
                    placeholderGroupList.Add(new PlaceholderGroup { Name = strippedGroup, PlaceholderTags = placeholderTags });
                }
            }
            return placeholderGroupList;
        }

        /// <summary>
        /// Returns a list of placeholder groups, by name only
        /// </summary>
        /// <param name="textWithPlaceholderTags"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static List<string> GetPlaceholderGroups(string textWithPlaceholderTags, System.IO.TextWriter logger)
        {
            var regGroup = new Regex(PatternGroup, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var matchesGroup = regGroup.Matches(textWithPlaceholderTags);
            var placeholderList = new List<string>();
            if (matchesGroup.Count > 0)
            {
                logger?.WriteLine("({0} matches):", matchesGroup.Count);
                foreach (Match match in matchesGroup)
                {
                    placeholderList.Add(match.Value);
                }
            }
            return placeholderList;
        }

        private static string ReplaceUnsubscribe(string result, System.IO.TextWriter logger)
        {
            var unsubscribeStart = "[unsubscribe]";
            var unsubscribeEnd = "[/unsubscribe]";


            // replace placeholder [unsubscribe]here[/unsubscribe] with <a href="[unsubscribe]">here</a> logic
            if (result.IndexOf(unsubscribeStart, StringComparison.OrdinalIgnoreCase) >= 0 && result.IndexOf(unsubscribeEnd, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                result = Helper.ReplaceLegacyUnsubscribe(result);

                logger?.WriteLine($"replacing {unsubscribeStart} and {unsubscribeEnd} with <a href=\"[unsubscribe]\">...</a>");
            }

            return result;
        }

        private static string StripBrackets(string input)
        {
            return input.Replace("[", string.Empty).Replace("]", string.Empty);
        }

        private static string StripBracketsFromGroup(string input)
        {
            var strippedValue = input;

            var firstBracket = strippedValue.IndexOf("]");
            if (firstBracket > -1)
            {
                strippedValue = strippedValue.Substring(0, firstBracket);
            }
            var firstColon = strippedValue.IndexOf(":");
            if (firstColon > -1)
            {
                firstColon++;
                strippedValue = strippedValue.Substring(firstColon, firstBracket - firstColon);
            }

            return strippedValue;
        }
    }
}