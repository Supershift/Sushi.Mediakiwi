using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Framework.ContentListItem
{
    /// <summary>
    /// Possible return types: System.String
    /// </summary>
    public class HtmlContainerAttribute : ContentInfoItem.HtmlContainerAttribute, IContentInfo, IListContentInfo 
    {
        /// <summary>
        /// Possible return types: System.String
        /// </summary>
        public HtmlContainerAttribute()
            : base() { }

        public HtmlContainerAttribute(bool noPadding)
            : base(noPadding) { }
    }
}


    
