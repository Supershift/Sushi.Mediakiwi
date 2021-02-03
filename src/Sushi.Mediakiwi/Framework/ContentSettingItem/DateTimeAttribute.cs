using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Framework.ContentSettingItem
{
    /// <summary>
    /// Possible return types: System.DateTime
    /// </summary>
    public class DateTimeAttribute : ContentInfoItem.DateTimeAttribute, IContentInfo, IContentSettingInfo
    {
        /// <summary>
        /// Possible return types: System.DateTime
        /// </summary>
        /// <param name="title"></param>
        public DateTimeAttribute(string title)
            : base(title, false) { }

        /// <summary>
        /// Possible return types: System.DateTime
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        public DateTimeAttribute(string title, bool mandatory)
            : base(title, mandatory, false) { }

        /// <summary>
        /// Possible return types: System.DateTime
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        /// <param name="isDbSortField"></param>
        public DateTimeAttribute(string title, bool mandatory, bool isDbSortField)
            : base(title, mandatory, isDbSortField, null) { }

        /// <summary>
        /// Possible return types: System.DateTime
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        /// <param name="interactiveHelp"></param>
        public DateTimeAttribute(string title, bool mandatory, string interactiveHelp)
            : base(title, mandatory, false, interactiveHelp) { }

        /// <summary>
        /// Possible return types: System.DateTime
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        /// <param name="isDbSortField"></param>
        /// <param name="interactiveHelp"></param>
        public DateTimeAttribute(string title, bool mandatory, bool isDbSortField, string interactiveHelp)
            : base(title, mandatory, isDbSortField, interactiveHelp) { }
    }
}
