using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Framework.ContentListSearchItem
{
    /// <summary>
    /// Possible return types: System.Int32, System.String
    /// </summary>
    public class Choice_DropdownAttribute : ContentInfoItem.Choice_DropdownAttribute, IContentInfo, IListSearchContentInfo
    {
        /// <summary>
        /// Possible return types: System.Int32, System.Int32[nullable], System.String
        /// </summary>
        /// <param name="title"></param>
        /// <param name="collectionPropertyName"></param>
        public Choice_DropdownAttribute(string title, string collectionPropertyName)
            : base(title, collectionPropertyName, false) { }

        /// <summary>
        /// Possible return types: System.Int32, System.Int32[nullable], System.String
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="collectionPropertyName">Name of the collection property.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        public Choice_DropdownAttribute(string title, string collectionPropertyName, bool mandatory)
            : base(title, collectionPropertyName, mandatory, null) { }

        /// <summary>
        /// Possible return types: System.Int32, System.Int32[nullable], System.String
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="collectionPropertyName">Name of the collection property.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        public Choice_DropdownAttribute(string title, string collectionPropertyName, bool mandatory, string interactiveHelp)
            : base(title, collectionPropertyName, mandatory, false, interactiveHelp) { }

        /// <summary>
        /// Possible return types: System.Int32, System.Int32[nullable], System.String
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="collectionPropertyName">Name of the collection property.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        /// <param name="autoPostback">if set to <c>true</c> [auto postback].</param>
        public Choice_DropdownAttribute(string title, string collectionPropertyName, bool mandatory, bool autoPostback)
            : base(title, collectionPropertyName, mandatory, autoPostback, null) { }

        /// <summary>
        /// Possible return types: System.Int32, System.Int32[nullable], System.String
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="collectionPropertyName">Name of the collection property.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        /// <param name="autoPostback">if set to <c>true</c> [auto postback].</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        public Choice_DropdownAttribute(string title, string collectionPropertyName, bool mandatory, bool autoPostback, string interactiveHelp)
            : base(title, collectionPropertyName, mandatory, autoPostback, interactiveHelp) { }
    }
}
