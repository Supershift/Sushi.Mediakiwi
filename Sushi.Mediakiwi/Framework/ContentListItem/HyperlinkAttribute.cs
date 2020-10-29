using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Framework.ContentListItem
{
    /// <summary>
    /// Possible return types: System.Int32
    /// </summary>
    public class HyperlinkAttribute : ContentInfoItem.HyperlinkAttribute, IContentInfo, IListContentInfo
    {
        /// <summary>
        /// Possible return types: System.Int32
        /// </summary>
        /// <param name="title"></param>
        public HyperlinkAttribute(string title)
            : base(title, false) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        public HyperlinkAttribute(string title, bool mandatory)
            : base(title, mandatory, null) { }

        /// <summary>
        /// Possible return types: System.Int32
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        /// <param name="interactiveHelp"></param>
        public HyperlinkAttribute(string title, bool mandatory, string interactiveHelp)
            : base(title, mandatory, interactiveHelp) { }
    }
}
