namespace Sushi.Mediakiwi.Framework.ContentSettingItem
{
    /// <summary>
    /// Possible return types: System.Int32
    /// </summary>
    public class PageSelectAttribute : ContentInfoItem.PageSelectAttribute, IContentInfo, IContentSettingInfo
    {
        /// <summary>
        /// Possible return types: System.Int32
        /// </summary>
        /// <param name="title"></param>
        public PageSelectAttribute(string title)
            : base(title, false) { }

        /// <summary>
        /// Possible return types: System.Int32
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        public PageSelectAttribute(string title, bool mandatory)
            : base(title, mandatory, null) { }

        /// <summary>
        /// Possible return types: System.Int32
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        /// <param name="interactiveHelp"></param>
        public PageSelectAttribute(string title, bool mandatory, string interactiveHelp)
            : base(title, mandatory, interactiveHelp) { }
    }
}
