namespace Sushi.Mediakiwi.Framework.ContentListItem
{
    /// <summary>
    /// Possible return types: System.String
    /// </summary>
    public class TextAreaAttribute : ContentInfoItem.TextAreaAttribute, IContentInfo, IListContentInfo
    {
        /// <summary>
        /// Possible return types: System.String
        /// </summary>
        /// <param name="title"></param>
        /// <param name="maxlength"></param>
        public TextAreaAttribute(string title, int maxlength)
            : base(title, maxlength, false) { }

        /// <summary>
        /// Possible return types: System.String
        /// </summary>
        /// <param name="title"></param>
        /// <param name="maxlength"></param>
        /// <param name="mandatory"></param>
        public TextAreaAttribute(string title, int maxlength, bool mandatory)
            : base(title, maxlength, mandatory, null) { }

        /// <summary>
        /// Possible return types: System.String
        /// </summary>
        /// <param name="title"></param>
        /// <param name="maxlength"></param>
        /// <param name="mandatory"></param>
        /// <param name="interactiveHelp"></param>
        public TextAreaAttribute(string title, int maxlength, bool mandatory, string interactiveHelp)
            : base(title, maxlength, mandatory, interactiveHelp, null) { }

        /// <summary>
        /// Possible return types: System.String
        /// </summary>
        /// <param name="title"></param>
        /// <param name="maxlength"></param>
        /// <param name="mandatory"></param>
        /// <param name="interactiveHelp"></param>
        /// <param name="mustMatchRegex"></param>
        public TextAreaAttribute(string title, int maxlength, bool mandatory, string interactiveHelp, string mustMatchRegex, bool isSourceCode = false)
            : base(title, maxlength, mandatory, interactiveHelp, mustMatchRegex) { IsSourceCode = isSourceCode; }
    }
}
