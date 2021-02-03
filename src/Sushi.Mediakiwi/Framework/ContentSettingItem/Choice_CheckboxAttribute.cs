using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Framework.ContentSettingItem
{
    /// <summary>
    /// Possible return types: System.Boolean
    /// </summary>
    public class Choice_CheckboxAttribute : ContentInfoItem.Choice_CheckboxAttribute, IContentInfo, IContentSettingInfo
    {
        /// <summary>
        /// Possible return types: System.Boolean
        /// </summary>
        /// <param name="title">The title.</param>
        public Choice_CheckboxAttribute(string title)
            : base(title) { }

        /// <summary>
        /// Possible return types: System.Boolean
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        public Choice_CheckboxAttribute(string title, string interactiveHelp)
            : base(title, interactiveHelp) { }

        /// <summary>
        /// Possible return types: System.Boolean
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="autoPostback">if set to <c>true</c> [auto postback].</param>
        public Choice_CheckboxAttribute(string title, bool autoPostback)
            : base(title, autoPostback) { }

        /// <summary>
        /// Possible return types: System.Boolean
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="autoPostback">if set to <c>true</c> [auto postback].</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        public Choice_CheckboxAttribute(string title, bool autoPostback, string interactiveHelp)
            : base(title, autoPostback, interactiveHelp) { }

    }
}
