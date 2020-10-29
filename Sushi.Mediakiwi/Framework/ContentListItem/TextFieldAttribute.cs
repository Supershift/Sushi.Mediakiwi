using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Sushi.Mediakiwi.Framework.ContentListItem
{
    /// <summary>
    /// Possible return types: System.String, System.Int32, System.Int32[nullable], System.Decimal, System.Decimal[nullable], System.Guid
    /// </summary>
    public class TextFieldAttribute : ContentInfoItem.TextFieldAttribute, IContentInfo, IListContentInfo
    {
        /// <summary>
        /// Possible return types: System.String, System.Int32, System.Int32[nullable], System.Decimal, System.Decimal[nullable], System.Guid
        /// </summary>
        /// <param name="title"></param>
        /// <param name="maxlength"></param>
        public TextFieldAttribute(string title, int maxlength)
            : base(title, maxlength, false) { }

        /// <summary>
        /// Possible return types: System.String, System.Int32, System.Int32[nullable], System.Decimal, System.Decimal[nullable], System.Guid
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="maxlength">The maxlength.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        public TextFieldAttribute(string title, int maxlength, bool mandatory)
            : base(title, maxlength, mandatory, null) { }

        /// <summary>
        /// Possible return types: System.String, System.Int32, System.Int32[nullable], System.Decimal, System.Decimal[nullable], System.Guid
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="maxlength">The maxlength.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        public TextFieldAttribute(string title, int maxlength, bool mandatory, string interactiveHelp)
            : base(title, maxlength, mandatory, interactiveHelp, null) { }

        /// <summary>
        /// Possible return types: System.String, System.Int32, System.Int32[nullable], System.Decimal, System.Decimal[nullable], System.Guid
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="maxlength">The maxlength.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        /// <param name="mustMatchRegex">The must match regex.</param>
        public TextFieldAttribute(string title, int maxlength, bool mandatory, string interactiveHelp, string mustMatchRegex)
            : base(title, maxlength, mandatory, interactiveHelp, mustMatchRegex) { }

        /// <summary>
        /// Possible return types: System.String, System.Int32, System.Int32[nullable], System.Decimal, System.Decimal[nullable], System.Guid
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="maxlength">The maxlength.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        /// <param name="autoPostback">if set to <c>true</c> [auto postback].</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        public TextFieldAttribute(string title, int maxlength, bool mandatory, bool autoPostback, string interactiveHelp)
            : base(title, maxlength, mandatory, autoPostback, interactiveHelp) { }

        /// <summary>
        /// Possible return types: System.String, System.Int32, System.Int32[nullable], System.Decimal, System.Decimal[nullable], System.Guid
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="maxlength">The maxlength.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        /// <param name="autoPostback">if set to <c>true</c> [auto postback].</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        /// <param name="mustMatchRegex">The must match regex.</param>
        public TextFieldAttribute(string title, int maxlength, bool mandatory, bool autoPostback, string interactiveHelp, string mustMatchRegex)
            : base(title, maxlength, mandatory, autoPostback, interactiveHelp, mustMatchRegex) { }


        SpecialType m_Type;
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public SpecialType Type
        {
            get { return m_Type;  }
            set { m_Type = value; }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum SpecialType
    {
        /// <summary>
        /// 
        /// </summary>
        Default = 0,
        /// <summary>
        /// 
        /// </summary>
        DaysOfWeek = 1
    }
}
