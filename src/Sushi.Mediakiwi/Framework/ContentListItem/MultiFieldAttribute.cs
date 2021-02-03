using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Sushi.Mediakiwi.Data;
using System.Globalization;
using Sushi.Mediakiwi.Framework.ContentInfoItem;

namespace Sushi.Mediakiwi.Framework.ContentListItem
{
    /// <summary>
    /// Possible return types: System.String
    /// </summary>
    public class MultiFieldAttribute : ContentInfoItem.MultiFieldAttribute, IContentInfo, IListContentInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiFieldAttribute"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        public MultiFieldAttribute(string title)
            : base(title, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiFieldAttribute"/> class.
        /// </summary>
        public MultiFieldAttribute(string title, string interactiveHelp)
            : base(title, interactiveHelp) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiFieldAttribute"/> class.
        /// </summary>
        public MultiFieldAttribute(string title, string interactiveHelp, ForceContentTypes forcetypes)
            : base(title, interactiveHelp, forcetypes) { }
    }
}


    
