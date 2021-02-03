using Sushi.Mediakiwi.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Framework
{
    public enum ButtonIconType
    {
        Default = 0,
        Approve,
        Deny,
        NewItem,
        Sorting,
        Play
    }

    public enum ButtonSection
    {
        Top = 0,
        Bottom = 1
    }

    public enum ButtonTarget
    {
        TopRight = 0,
        TopLeft = 1,
        BottomLeft = 2,
        BottomRight = 3
    }

    public enum LayerSize
    {
        Undefined = 0,
        //  A screen of 864 x 614 pixels with a scrollbar
        Normal = 1,
        //  A screen of 760 x 414 pixels with a scrollbar
        Small = 2,
        //  A screen of 472 x 314 pixels with no scrollbar
        Tiny = 3
    }
}

namespace Sushi.Mediakiwi.Framework.ContentListItem
{
    /// <summary>
    /// Possible return types: System.Boolean
    /// </summary>
    public class ButtonAttribute : ContentSharedAttribute, IContentInfo, IListContentInfo
    {
        public bool NoPostBack { get; set; }

        public string ButtonClassName { get; set; }
        public string InteractiveHelp { get; set; }
        public string ConfirmationQuestion { get; set; }
        public string ConfirmationTitle { get; set; }
        public string ConfirmationRejectLabel { get; set; }
        public string ConfirmationAcceptLabel { get; set; }

        string m_IconClassName;
        /// <summary>
        /// 
        /// </summary>
        [Obsolete("This option has no effect in MediaKiwi (>4.0)", false)]
        public string IconClassName
        {
            get
            {
                string className;
                switch (IconType)
                {
                    case ButtonIconType.Approve: className = "save"; break;
                    case ButtonIconType.Deny: className = "delete"; break;
                    case ButtonIconType.Sorting: className = "refresh"; break;
                    case ButtonIconType.NewItem: className = "createPage"; break;
                    case ButtonIconType.Default: className = "Play"; break;
                    default: className = IconType.ToString(); break;
                }
                if (!string.IsNullOrEmpty(m_IconClassName))
                    className = string.Concat(className, " ", m_IconClassName);

                return className;
            }
            set
            {
                m_IconClassName = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// Possible return types: System.Boolean
        /// </summary>
        /// <param name="title">The title.</param>
        public ButtonAttribute(string title)
            : this(title, false) { }

        public ButtonAttribute(string title, bool triggerSave)
            : this(title, triggerSave, false) { }


        public ButtonAttribute(string title, bool triggerSave, bool triggerValidation)
            : this(title, triggerSave, triggerValidation, false) { }

        /// <summary>
        /// Possible return types: System.Boolean
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="triggerSave">if set to <c>true</c> [trigger save].</param>
        public ButtonAttribute(string title, bool triggerSave, bool triggerValidation, bool persistState)
        {
            m_CanHaveExpression = true;
            ContentTypeSelection = ContentType.Button;
            m_TriggerState = persistState;
            TriggerSaveEvent = triggerSave;
            m_TriggerValidation = triggerValidation;
            Title = title;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonAttribute"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="triggerSave">if set to <c>true</c> [trigger save].</param>
        /// <param name="icon">The icon.</param>
        [Obsolete("Obsolete, please replace", false)]
        public ButtonAttribute(string title, bool triggerSave, ButtonIconType icon)
            : this(title, triggerSave, false, false, 0, null, icon) { }


        /// <summary>
        /// Possible return types: System.Boolean
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="triggerSave">if set to <c>true</c> [trigger save].</param>
        /// <param name="width">The width.</param>
        [Obsolete("Obsolete, please replace", false)]
        public ButtonAttribute(string title, bool triggerSave, int width)
            : this(title, triggerSave, false, width) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonAttribute"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="triggerSave">if set to <c>true</c> [trigger save].</param>
        /// <param name="isFormElement">if set to <c>true</c> [is form element].</param>
        /// <param name="width">The width.</param>
        [Obsolete("Obsolete, please replace", false)]
        public ButtonAttribute(string title, bool triggerSave, bool isFormElement, int width)
            : this(title, triggerSave, isFormElement, width, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonAttribute"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="triggerSave">if set to <c>true</c> [trigger save].</param>
        /// <param name="isFormElement">if set to <c>true</c> [is form element].</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        [Obsolete("Obsolete, please replace", false)]
        public ButtonAttribute(string title, bool triggerSave, bool isFormElement, string interactiveHelp)
            : this(title, triggerSave, isFormElement, 0, interactiveHelp) { }

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
            : this(title, triggerSave, isFormElement, false, width, interactiveHelp) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonAttribute"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="triggerSave">if set to <c>true</c> [trigger save].</param>
        /// <param name="isFormElement">if set to <c>true</c> [is form element].</param>
        /// <param name="showAtBottom">if set to <c>true</c> [show at bottom].</param>
        /// <param name="width">The width.</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        [Obsolete("Obsolete, please replace", false)]
        public ButtonAttribute(string title, bool triggerSave, bool isFormElement, bool showAtBottom, int width, string interactiveHelp)
            : this(title, triggerSave, isFormElement, false, width, interactiveHelp, ButtonIconType.Default) { }

        /// <summary>
        /// Possible return types: System.Boolean
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="triggerSave">if set to <c>true</c> [trigger save].</param>
        /// <param name="isFormElement">if set to <c>true</c> [is form element].</param>
        /// <param name="width">The width.</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        [Obsolete("Obsolete, please replace", false)]
        public ButtonAttribute(string title, bool triggerSave, bool isFormElement, bool showAtBottom, int width, string interactiveHelp, Sushi.Mediakiwi.Framework.ButtonIconType icon)
        {
            m_CanHaveExpression = true;
            ContentTypeSelection = ContentType.Button;
            TriggerSaveEvent = triggerSave;
            m_IsFormElement = isFormElement;
            this.Width = width;
            this.InteractiveHelp = interactiveHelp;
            Title = title;
            this.IconType = icon;
        }

        internal bool m_IsFormElement;

        bool m_TriggerValidation;
        public bool TriggerValidation
        {
            get { return m_TriggerValidation; }
        }

        bool m_TriggerState;
        public bool TriggerState
        {
            get { return m_TriggerState; }
        }

        /// <summary>
        /// Gets a value indicating whether [trigger save event].
        /// </summary>
        /// <value><c>true</c> if [trigger save event]; otherwise, <c>false</c>.</value>
        public bool TriggerSaveEvent { get; internal set; }

        internal bool OpenUrl
        {
            get {
                if (OpenInPopupLayer || !string.IsNullOrEmpty(this.CustomUrlProperty))
                    return true;
                return false;
            }
        }

        bool m_OpenInPopupLayerIsSet;
        bool m_OpenInPopupLayer;
        public bool OpenInPopupLayer  
        {
            get {
                if (m_OpenInPopupLayerIsSet) return m_OpenInPopupLayer;
                return !string.IsNullOrEmpty(this.ListInPopupLayer); }
            set { m_OpenInPopupLayer = value; m_OpenInPopupLayerIsSet = true; }
        }
        public string CustomUrl { get; set; }
        [Obsolete("Obsolete, please use CustomUrl", false)]
        public string CustomUrlProperty { get; set; }
        public string ListInPopupLayer { get; set; }
        public bool AskConfirmation { get; set; }
        public bool IsPrimary { get; set; }

        [Obsolete("Obsolete, please replace", false)]
        public ButtonIconType IconType { get; set; }
        public ButtonTarget IconTarget { get; set; }
        /// <summary>
        /// Gets or sets the size of the popup layer.
        /// </summary>
        /// <value>
        /// The size of the popup layer.
        /// </value>

        LayerSize _PopupLayerSize;
        public LayerSize PopupLayerSize {
            get {
                if (_PopupLayerSize == LayerSize.Undefined)
                    return LayerSize.Normal;
                return _PopupLayerSize;
            }
            set { _PopupLayerSize = value; }
        }
        /// <summary>
        /// The popup layer height
        /// </summary>
        public string PopupLayerHeight { get; set; }
        /// <summary>
        /// Gets or sets the width of the popup layer.
        /// </summary>
        /// <value>
        /// The width of the popup layer.
        /// </value>
        public string PopupLayerWidth { get; set; }
        /// <summary>
        /// Gets or sets the popup layer has scroll bar.
        /// </summary>
        /// <value>
        /// The popup layer has scroll bar.
        /// </value>
        internal bool? PopupLayerHasScrollBar { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [popup layer scroll bar].
        /// </summary>
        /// <value>
        /// <c>true</c> if [popup layer scroll bar]; otherwise, <c>false</c>.
        /// </value>
        public bool PopupLayerScrollBar
        {
            get { return this.PopupLayerHasScrollBar.GetValueOrDefault(); }
            set { this.PopupLayerHasScrollBar = value; }
        }

        string _PopupTitle;
        /// <summary>
        /// Gets or sets the popup title.
        /// </summary>
        /// <value>
        /// The popup title.
        /// </value>
        public string PopupTitle { get { return _PopupTitle == null ? this.Title : this._PopupTitle; } set { this._PopupTitle = value; } }


        int m_Width;
        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>The width.</value>
        [Obsolete("Obsolete, please replace", false)]
        public int Width
        {
            get { return m_Width; }
            set { m_Width = value; }
        }

        bool m_IsClicked;
        /// <summary>
        /// Gets a value indicating whether this instance is clicked.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is clicked; otherwise, <c>false</c>.
        /// </value>
        public bool IsClicked
        {
            get { return m_IsClicked; }
        }

        /// <summary>
        /// Sets the candidate.
        /// </summary>
        /// <param name="isEditMode"></param>
        public void SetCandidate(bool isEditMode)
        {
            SetCandidate(null, isEditMode);
        }

        /// <summary>
        /// Sets the candidate.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="isEditMode">if set to <c>true</c> [is edit mode].</param>
        public void SetCandidate(Field field, bool isEditMode)
        {
            var value = Console.Form("autopostback");
            //  [MM:10.12.14] Addition actions (like save) can be set via [ID]$save, so strip it out
            if (!string.IsNullOrEmpty(value))
                value = value.Split('$')[0];

            m_IsClicked = value == this.ID;

            if (m_TriggerState && !m_IsClicked && string.IsNullOrWhiteSpace(value) && !string.IsNullOrWhiteSpace(Console.CurrentListInstance.FormState))
                m_IsClicked = (Console.CurrentListInstance.FormState == this.ID);

            Property.SetValue(SenderInstance, m_IsClicked, null);

            if (m_TriggerState && m_IsClicked)
                Console.CurrentListInstance.FormState = this.ID;

            if (m_TriggerValidation && m_IsClicked)
                Console.CurrentListInstance.wim.ShouldValidate = true;
        }

        /// <summary>
        /// Writes the candidate.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <param name="isEditMode"></param>
        /// <returns></returns>
        public virtual Field WriteCandidate(WimControlBuilder build, bool isEditMode, bool isRequired, bool isCloaked)
        {
            this.Mandatory = isRequired;
            this.IsCloaked = isCloaked;
            //if (!m_IsFormElement)
                return null;

            build.Append(string.Format(@"{8}{9}{5}
	<th></th>
	<td{7}>
        <input type=""hidden"" id=""{1}"" name=""{1}"" value=""""/>
		<button class=""{12}{10}""{4}><span>{0}</span></button><div class=""clear""></div>{2}
	</td>{6}
"
                , Title // 0
                , this.ID // 1
                , this.InteractiveHelp // 2
                , Console.WimRepository // 3
                , this.Width > 0 ? string.Format(" style=\"width:{0}px;\"", this.Width) : string.Empty // 4
                , (Expression == OutputExpression.FullWidth || Expression == OutputExpression.Left) ? "<tr>" : null // 5
                , (Expression == OutputExpression.FullWidth || Expression == OutputExpression.Right) ? "</tr>" : null // 6
                , (Expression == OutputExpression.FullWidth && Console.HasDoubleCols) ? " colspan=\"3\" class=\"triple\"" : null // 7
                , ((Console.ExpressionPrevious == OutputExpression.Left && Expression == OutputExpression.FullWidth) || (Console.ExpressionPrevious == OutputExpression.Left && Expression == OutputExpression.Left)) ? "<th><label>&nbsp;</label></th><td>&nbsp;</td></tr>" : null // 8
                , ((Console.ExpressionPrevious == OutputExpression.FullWidth && Expression == OutputExpression.Right) || (Console.ExpressionPrevious == OutputExpression.Right && Expression == OutputExpression.Right)) ? "<tr><th><label>&nbsp;</label></th><td>&nbsp;</td>" : null // 9
                , this.AskConfirmation ? " type_confirm" : null //10
                , Width > 0 ? string.Format(" style=\"width:{0}px\"", this.Width) : null //11 
                , string.IsNullOrWhiteSpace(CustomUrl) ? "postBack" : null //12

                )
            );
            return null;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        /// <returns></returns>
        [System.Xml.Serialization.XmlIgnore()]
        public new bool IsValid
        {
            get
            {
                //if (Mandatory && string.IsNullOrEmpty(m_output))
                //    return false;
                return true;
            }
        }


    }
}
