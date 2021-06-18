namespace Sushi.Mediakiwi.Framework.ContentSettingItem
{
    /// <summary>
    /// Possible return types: System.DateTime
    /// </summary>
    public class DateAttribute : ContentInfoItem.DateAttribute, IContentInfo, IContentSettingInfo
    {
        /// <summary>
        /// Possible return types: System.DateTime
        /// </summary>
        /// <param name="title"></param>
        public DateAttribute(string title)
            : base(title, false) { }

        /// <summary>
        /// Possible return types: System.DateTime
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        public DateAttribute(string title, bool mandatory)
            : base(title, mandatory) { }


        /// <summary>
        /// Possible return types: System.DateTime
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        /// <param name="interactiveHelp"></param>
        public DateAttribute(string title, bool mandatory, string interactiveHelp)
            : base(title, mandatory, interactiveHelp) { }
    }
}
