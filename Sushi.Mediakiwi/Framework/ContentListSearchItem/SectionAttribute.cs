using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Framework.ContentListSearchItem
{
    /// <summary>
    /// Possible return types: System.String 
    /// </summary>
    public class SectionAttribute : ContentListItem.SectionAttribute, IContentInfo, IListSearchContentInfo
    {
        /// <summary>
        /// Possible return types: System.String
        /// </summary>
        public SectionAttribute()
            : base() { }
    }
}
