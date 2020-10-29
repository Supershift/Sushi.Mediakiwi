using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Framework.ContentListItem
{
    /// <summary>
    /// Possible return types: Possible return types: System.Int32, System.String, List<int>, List<string>
    /// </summary>
    public class Choice_DropdownAttribute : ContentInfoItem.Choice_DropdownAttribute, IContentInfo, IListContentInfo
    {
        /// <summary>
        /// Possible return types: System.Int32, System.String, List<int>, List<string>
        /// </summary>
        /// <param name="title"></param>
        /// <param name="collectionPropertyName"></param>
        public Choice_DropdownAttribute(string title, string collectionPropertyName)
            : base(title, collectionPropertyName, false) { }

        /// <summary>
        /// Possible return types: System.Int32, System.String, List<int>, List<string>
        /// </summary>
        /// <param name="title"></param>
        /// <param name="collectionPropertyName"></param>
        /// <param name="mandatory"></param>
        public Choice_DropdownAttribute(string title, string collectionPropertyName, bool mandatory)
            : base(title, collectionPropertyName, mandatory, null) { }

        /// <summary>
        /// Possible return types: System.Int32, System.String, List<int>, List<string>
        /// </summary>
        /// <param name="title"></param>
        /// <param name="collectionPropertyName"></param>
        /// <param name="mandatory"></param>
        /// <param name="interactiveHelp"></param>
        public Choice_DropdownAttribute(string title, string collectionPropertyName, bool mandatory, string interactiveHelp)
            : base(title, collectionPropertyName, mandatory, false, interactiveHelp) { }

        /// <summary>
        /// Possible return types: System.Int32, System.String, List<int>, List<string>
        /// </summary>
        /// <param name="title"></param>
        /// <param name="collectionPropertyName"></param>
        /// <param name="mandatory"></param>
        /// <param name="autoPostback"></param>
        public Choice_DropdownAttribute(string title, string collectionPropertyName, bool mandatory, bool autoPostback)
            : base(title, collectionPropertyName, mandatory, autoPostback, null) { }

        /// <summary>
        /// Possible return types: System.Int32, System.String, List<int>, List<string>
        /// </summary>
        /// <param name="title"></param>
        /// <param name="collectionPropertyName"></param>
        /// <param name="mandatory"></param>
        /// <param name="autoPostback"></param>
        /// <param name="interactiveHelp"></param>
        public Choice_DropdownAttribute(string title, string collectionPropertyName, bool mandatory, bool autoPostback, string interactiveHelp)
            : base(title, collectionPropertyName, mandatory, autoPostback, interactiveHelp) { }
    }
}
