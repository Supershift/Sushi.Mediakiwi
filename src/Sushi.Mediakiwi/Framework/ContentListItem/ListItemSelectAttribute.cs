using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Framework.ContentListItem
{
    /// <summary>
    /// Possible return types: System.String[], System.Int32[], string (CSV)
    /// </summary>
    public class ListItemSelectAttribute : ContentInfoItem.ListItemSelectAttribute, IContentInfo, IListContentInfo
    {
        /// <summary>
        /// Possible return types: System.String[], System.Int32[], string (CSV)
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="collectionPropertyName">Name of the collection property.</param>
        public ListItemSelectAttribute(string title, string collectionPropertyName)
            : base(title, collectionPropertyName, false) { }

        /// <summary>
        /// Possible return types: System.String[], System.Int32[], string (CSV)
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="collectionPropertyName">Name of the collection property.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        public ListItemSelectAttribute(string title, string collectionPropertyName, bool mandatory)
            : base(title, collectionPropertyName, mandatory, null) { }

        /// <summary>
        /// Possible return types: System.String[], System.Int32[], string (CSV)
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="collectionPropertyName">Name of the collection property.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        public ListItemSelectAttribute(string title, string collectionPropertyName, bool mandatory, string interactiveHelp)
            : base(title, collectionPropertyName, mandatory, interactiveHelp) { }
    }
}
