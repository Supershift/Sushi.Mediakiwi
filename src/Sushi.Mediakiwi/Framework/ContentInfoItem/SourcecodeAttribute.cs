using Sushi.Mediakiwi.Data;
using System;
using System.Globalization;

namespace Sushi.Mediakiwi.Framework.ContentInfoItem
{
    /// <summary>
    /// Possible return types: System.String
    /// </summary>
    public class SourcecodeAttribute : TextAreaAttribute 
    {
        /// <summary>
        /// Possible return types: System.String
        /// </summary>
        /// <param name="title"></param>
        /// <param name="maxlength"></param>
        public SourcecodeAttribute(string title, int maxlength)
            : this(title, maxlength, false) { }

        /// <summary>
        /// Possible return types: System.String
        /// </summary>
        /// <param name="title"></param>
        /// <param name="maxlength"></param>
        /// <param name="mandatory"></param>
        public SourcecodeAttribute(string title, int maxlength, bool mandatory)
            : this(title, maxlength, mandatory, null) { }

        /// <summary>
        /// Possible return types: System.String
        /// </summary>
        /// <param name="title"></param>
        /// <param name="maxlength"></param>
        /// <param name="mandatory"></param>
        /// <param name="interactiveHelp"></param>
        public SourcecodeAttribute(string title, int maxlength, bool mandatory, string interactiveHelp)
            : this(title, maxlength, mandatory, interactiveHelp, null) { }

        /// <summary>
        /// Possible return types: System.String
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="maxlength">The maxlength.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        /// <param name="mustMatchRegex">The must match regex.</param>
        public SourcecodeAttribute(string title, int maxlength, bool mandatory, string interactiveHelp, string mustMatchRegex)
            : base(title, maxlength, mandatory, interactiveHelp, mustMatchRegex)
        {
            IsSourceCode = true; 
            ContentTypeSelection = ContentType.Sourcecode;
        }

        public override void SetCandidate(Field field, bool isEditMode)
        {
            IsSourceCode = true;
            base.SetCandidate(field, isEditMode);
            SetMultiFieldTitleHTML(Labels.ResourceManager.GetString("input_code", new CultureInfo(Console.CurrentApplicationUser.LanguageCulture)), "icon-code");
        }
    }
}
