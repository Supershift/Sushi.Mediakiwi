namespace Sushi.Mediakiwi.Framework.ContentSettingItem
{
    /// <summary>
    /// Possible return types: System.String
    /// </summary>
    public class TextLineAttribute : ContentInfoItem.TextLineAttribute, IContentInfo, IContentSettingInfo
    {
        /// <summary>
        /// Possible return types: System.String
        /// </summary>
        /// <param name="title"></param>
        public TextLineAttribute(string title)
            : base(title, null) { }

        /// <summary>
        /// Possible return types: System.String
        /// </summary>
        /// <param name="title"></param>
        /// <param name="interactiveHelp"></param>
        public TextLineAttribute(string title, string interactiveHelp)
            : base(title, interactiveHelp) { }

        /// <summary>
        /// Possible return types: System.String
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="IsClosedContainer">If this text is set as a container (title = null), it has the option to be open or closed</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        public TextLineAttribute(string title, bool IsClosedContainer, string interactiveHelp)
            : base(title, IsClosedContainer, interactiveHelp) { }
    }
}
