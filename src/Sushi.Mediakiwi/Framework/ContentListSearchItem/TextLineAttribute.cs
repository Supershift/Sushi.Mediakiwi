using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Framework.ContentListSearchItem
{
    /// <summary>
    /// Possible return types: System.String 
    /// </summary>
    public class TextLineAttribute : ContentInfoItem.TextLineAttribute, IContentInfo, IListSearchContentInfo
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
    }
}
