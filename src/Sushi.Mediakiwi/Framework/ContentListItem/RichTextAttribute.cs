namespace Sushi.Mediakiwi.Framework.ContentListItem
{
    /// <summary>
    /// Possible return types: System.String
    /// </summary>
    public class RichTextAttribute : ContentInfoItem.RichTextAttribute, IListContentInfo
    {
        /// <summary>
        /// Possible return types: System.String
        /// </summary>
        /// <param name="title"></param>
        /// <param name="maxlength"></param>
        public RichTextAttribute(string title, int maxlength)
            : base(title, maxlength, false) { }

        /// <summary>
        /// Possible return types: System.String
        /// </summary>
        /// <param name="title"></param>
        /// <param name="maxlength"></param>
        /// <param name="mandatory"></param>
        public RichTextAttribute(string title, int maxlength, bool mandatory)
            : base(title, maxlength, mandatory, null) { }

        /// <summary>
        /// Possible return types: System.String
        /// </summary>
        /// <param name="title"></param>
        /// <param name="maxlength"></param>
        /// <param name="mandatory"></param>
        /// <param name="interactiveHelp"></param>
        public RichTextAttribute(string title, int maxlength, bool mandatory, string interactiveHelp)
            : base(title, maxlength, mandatory, interactiveHelp) { }
    }
}
