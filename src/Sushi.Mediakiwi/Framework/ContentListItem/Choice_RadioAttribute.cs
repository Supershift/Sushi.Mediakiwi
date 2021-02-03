using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Framework.ContentListItem
{
    /// <summary>
    /// Possible return types: System.Int16, System.Int32, System.String, System.Int32[nullable]
    /// </summary>
    public class Choice_RadioAttribute : ContentInfoItem.Choice_RadioAttribute, IContentInfo, IListContentInfo
    {
        /// <summary>
        /// Possible return types: System.Int16, System.Int32, System.String
        /// </summary>
        /// <param name="title"></param>
        /// <param name="collectionPropertyName"></param>
        /// <param name="groupName"></param>
        public Choice_RadioAttribute(string title, string collectionPropertyName, string groupName)
            : base(title, collectionPropertyName, groupName, false) { }

        /// <summary>
        /// Possible return types: System.Int16, System.Int32, System.String
        /// </summary>
        /// <param name="title"></param>
        /// <param name="collectionPropertyName"></param>
        /// <param name="groupName"></param>
        /// <param name="mandatory"></param>
        public Choice_RadioAttribute(string title, string collectionPropertyName, string groupName, bool mandatory)
            : base(title, collectionPropertyName, groupName, mandatory, null) { }

        /// <summary>
        /// Possible return types: System.Int16, System.Int32, System.String
        /// </summary>
        /// <param name="title"></param>
        /// <param name="collectionPropertyName"></param>
        /// <param name="groupName"></param>
        /// <param name="mandatory"></param>
        /// <param name="interactiveHelp"></param>
        public Choice_RadioAttribute(string title, string collectionPropertyName, string groupName, bool mandatory, string interactiveHelp)
            : base(title, collectionPropertyName, groupName, mandatory, false, interactiveHelp) { }

        /// <summary>
        /// Possible return types: System.Int16, System.Int32, System.String
        /// </summary>
        /// <param name="title"></param>
        /// <param name="collectionPropertyName"></param>
        /// <param name="groupName"></param>
        /// <param name="mandatory"></param>
        /// <param name="autoPostback"></param>
        public Choice_RadioAttribute(string title, string collectionPropertyName, string groupName, bool mandatory, bool autoPostback)
            : base(title, collectionPropertyName, groupName, mandatory, autoPostback, null) { }

        /// <summary>
        /// Possible return types: System.Int16, System.Int32, System.String
        /// </summary>
        /// <param name="title"></param>
        /// <param name="collectionPropertyName"></param>
        /// <param name="groupName"></param>
        /// <param name="mandatory"></param>
        /// <param name="autoPostback"></param>
        /// <param name="interactiveHelp"></param>
        public Choice_RadioAttribute(string title, string collectionPropertyName, string groupName, bool mandatory, bool autoPostback, string interactiveHelp)
            : base(title, collectionPropertyName, groupName, mandatory, autoPostback, interactiveHelp) { }
    }
}
