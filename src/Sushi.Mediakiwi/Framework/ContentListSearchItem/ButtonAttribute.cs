using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Framework.ContentListSearchItem
{
    /// <summary>
    /// Possible return types: System.Boolean
    /// </summary>
    public class ButtonAttribute : ContentListItem.ButtonAttribute, IContentInfo, IListSearchContentInfo
    {
        /// <summary>
        /// Possible return types: System.Boolean
        /// </summary>
        /// <param name="title">The title.</param>
        public ButtonAttribute(string title)
            : base(title) { }

        /// <summary>
        /// Possible return types: System.Boolean
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="triggerSave">if set to <c>true</c> [trigger save].</param>
        public ButtonAttribute(string title, bool triggerSave)
            : base(title, triggerSave) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonAttribute"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="triggerSave">if set to <c>true</c> [trigger save].</param>
        /// <param name="isFormElement">if set to <c>true</c> [is form element].</param>
        [Obsolete("Obsolete, please replace", false)]
        public ButtonAttribute(string title, bool triggerSave, bool isFormElement)
            : base(title, triggerSave, isFormElement) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonAttribute"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="triggerSave">if set to <c>true</c> [trigger save].</param>
        /// <param name="width">The width.</param>
        [Obsolete("Obsolete, please replace", false)]
        public ButtonAttribute(string title, bool triggerSave, int width)
            : base(title, triggerSave, false, width) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonAttribute"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="triggerSave">if set to <c>true</c> [trigger save].</param>
        /// <param name="isFormElement">if set to <c>true</c> [is form element].</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        [Obsolete("Obsolete, please replace", false)]
        public ButtonAttribute(string title, bool triggerSave, bool isFormElement, string interactiveHelp)
            : base(title, triggerSave, false, 0, interactiveHelp) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonAttribute"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="triggerSave">if set to <c>true</c> [trigger save].</param>
        /// <param name="isFormElement">if set to <c>true</c> [is form element].</param>
        /// <param name="width">The width.</param>
        [Obsolete("Obsolete, please replace", false)]
        public ButtonAttribute(string title, bool triggerSave, bool isFormElement, int width)
            : base(title, triggerSave, isFormElement, width) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonAttribute"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="triggerSave">if set to <c>true</c> [trigger save].</param>
        /// <param name="isFormElement">if set to <c>true</c> [is form element].</param>
        /// <param name="width">The width.</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        [Obsolete("Obsolete, please replace", false)]
        public ButtonAttribute(string title, bool triggerSave, bool isFormElement, int width, string interactiveHelp)
            : base(title, triggerSave, isFormElement, width, interactiveHelp) { }
    }
}
