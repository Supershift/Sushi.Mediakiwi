using Sushi.Mediakiwi.Data;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Sushi.Mediakiwi.Framework.ContentInfoItem
{
    /// <summary>
    /// Possible return types: System.String
    /// </summary>
    public class TextAreaAttribute : ContentEditableSharedAttribute, IContentInfo
    {
        /// <summary>
        /// Possible return types: System.String
        /// </summary>
        /// <param name="title"></param>
        /// <param name="maxlength"></param>
        public TextAreaAttribute(string title, int maxlength)
            : this(title, maxlength, false) { }

        /// <summary>
        /// Possible return types: System.String
        /// </summary>
        /// <param name="title"></param>
        /// <param name="maxlength"></param>
        /// <param name="mandatory"></param>
        public TextAreaAttribute(string title, int maxlength, bool mandatory)
            : this(title, maxlength, mandatory, null) { }

        /// <summary>
        /// Possible return types: System.String
        /// </summary>
        /// <param name="title"></param>
        /// <param name="maxlength"></param>
        /// <param name="mandatory"></param>
        /// <param name="interactiveHelp"></param>
        public TextAreaAttribute(string title, int maxlength, bool mandatory, string interactiveHelp)
            : this(title, maxlength, mandatory, interactiveHelp, null) { }

        /// <summary>
        /// Possible return types: System.String
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="maxlength">The maxlength.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        /// <param name="mustMatchRegex">The must match regex.</param>
        public TextAreaAttribute(string title, int maxlength, bool mandatory, string interactiveHelp, string mustMatchRegex)
        {
            ContentTypeSelection = ContentType.TextArea;
            Title = title;
            MaxValueLength = maxlength;
            Mandatory = mandatory;
            InteractiveHelp = interactiveHelp;
            if (mustMatchRegex != null)
                MustMatch = new Regex(mustMatchRegex);

        }

        /// <summary>
        /// Gets or sets a value indicating whether [is source code].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is source code]; otherwise, <c>false</c>.
        /// </value>
        public bool IsSourceCode { get; set; }
        /// <summary>
        /// When set the response is cleaned (enters/spaces) to keep a tight XML file
        /// </summary>
        public bool IsXml { get; set; }

        private Regex m_MustMatch;
        /// <summary>
        /// 
        /// </summary>
        public Regex MustMatch
        {
            set { m_MustMatch = value; }
            get { return m_MustMatch; }
        }

        private bool _allowHtmlTags;
        /// <summary>
        /// Gets or sets a value indicating if HTML tags are allowed. If set to true, no HTML encoding is applied to the user input.
        /// </summary>
        public bool AllowHtmlTags
        {
            get
            {
                if (IsSourceCode)
                    return true;
                return _allowHtmlTags;
            }
            set
            {
                _allowHtmlTags = value;
            }
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
        public virtual void SetCandidate(Field field, bool isEditMode)
        {
            this.SetMultiFieldTitleHTML("Text", "icon-align-left");
            if (Property != null && Property.PropertyType == typeof(Data.CustomData))
                SetContentContainer(field);

            string candidate = null;

            bool isCandidateUserInput = false;
            if (IsInitialLoad || !isEditMode)
            {
                if (IsBluePrint)
                {
                    if (field != null)
                        candidate = field.Value;
                }
                else
                {
                    if (Property.PropertyType == typeof(Data.CustomData))
                    {
                        candidate = m_ContentContainer[field.Property].Value;
                    }
                    else if (Property.PropertyType == typeof(string))
                    {
                        object value = Property.GetValue(SenderInstance, null);
                        if (value != null)
                            candidate = value.ToString();
                    }
                }
            }
            else
            {
                candidate = Console.Form(this.ID);
                isCandidateUserInput = true;
                if (IsSourceCode && IsXml)
                {
                    //  Clean up enters/spaces
                    Regex rex1 = new Regex(@"(?!>)[\f\n\r\t\v].*?(?=[A-Z0-9<])", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    //  Required, else it still will not be clean.
                    Regex rex2 = new Regex(@"[\f\n\r\t\v]", RegexOptions.Multiline | RegexOptions.IgnoreCase);

                    candidate = rex1.Replace(candidate, string.Empty);
                    candidate = rex2.Replace(candidate, string.Empty);
                }
            }

            if (candidate != null) candidate = candidate.Trim();

            // Possible return types: System.String

            if (!IsBluePrint && Property != null && Property.CanWrite)
            {
                if (Property.PropertyType == typeof(Data.CustomData))
                    ApplyContentContainer(field, candidate);
                else if (Property.PropertyType == typeof(string))
                {
                    //if candidate is user input it needs to cleaned
                    if (isCandidateUserInput && candidate != null && candidate is string candidateString)
                    {
                        if (CommonConfiguration.HTML_ENCODE_LIST_PROPERTIES && !AllowHtmlTags)
                        {
                            //encode html
                            candidateString = WebUtility.HtmlEncode(candidateString);
                        }
                        //enforce max length of string
                        if (this.MaxValueLength > 0)
                            candidateString = Data.Utility.ConvertToFixedLengthText(candidateString, this.MaxValueLength);

                        candidate = candidateString;
                    }

                    Property.SetValue(SenderInstance, candidate, null);
                }
            }
            OutputText = candidate;
            
            //  Inherited content section
            if (ShowInheritedData)
            {
                if (field != null && !string.IsNullOrEmpty(field.InheritedValue))
                {
                    InhertitedOutputText = field.InheritedValue;
                    InhertitedOutputText = Data.Utility.CleanLineFeed(WebUtility.HtmlEncode(InhertitedOutputText), true);
                }
            }
        }

        /// <summary>
        /// Writes the candidate.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <param name="isEditMode">if set to <c>true</c> [is edit mode].</param>
        /// <param name="isRequired">if set to <c>true</c> [is required].</param>
        /// <returns></returns>
        public Field WriteCandidate(WimControlBuilder build, bool isEditMode, bool isRequired, bool isCloaked)
        {
            this.SetWriteEnvironment();
            this.IsCloaked = isCloaked;
            this.Mandatory = isRequired;
            if (OverrideEditMode) isEditMode = false;

            //get the value of the property
            string outputValue = this.OutputText;
            if (Property?.PropertyType == typeof(String) && CommonConfiguration.HTML_ENCODE_LIST_PROPERTIES && !AllowHtmlTags)
            {
                //in this case the user input was HTML encoded. it needs to be decoded again to get back to its orignal value
                outputValue = WebUtility.HtmlDecode(outputValue);
            }

            if (isEditMode || (m_IsNewDesign && this.IsSourceCode))
            {
                #region Element creation
                StringBuilder element = new StringBuilder();
                //  [MM:22.12.14] If this textarea acts as an code field, the breaks should be left alone
                string cleanData = outputValue;
                if (!this.IsSourceCode)
                    cleanData = Data.Utility.CleanLineFeed(this.OutputText, false);

                element.AppendFormat("\n\t\t\t\t\t\t\t\t\t<textarea cols=\"32\" rows=\"4\"{2} name=\"{0}\" id=\"{0}\"{3}>{1}</textarea>"
                  , this.ID
                  , cleanData
                  , this.InputClassName(IsValid(isRequired), (this.IsSourceCode ? string.Concat(" SourceCode", (isEditMode ? " editable" : string.Empty)) : string.Empty))
                  , this.IsSourceCode && string.IsNullOrEmpty(Console.Request.Query["xml"]) ? " data-done=\"1\"" : null
                  );
                #endregion Element creation

                if (IsCloaked)
                {
                    build.AppendCloaked(element.ToString());
                }
                else
                {
                    #region Wrapper
                    string titleTag = string.Concat(Title, Mandatory ? "<em>*</em>" : "");
                    if (this.IsSourceCode)
                    {
                        this.Console.CurrentListInstance.wim.Page.Head.EnableColorCodingLibrary = true;
                        if (string.IsNullOrEmpty(this.m_InteractiveHelp))
                            this.m_InteractiveHelp = "Press 'ESC' for fullscreen (and also to reset)";
                    }

                    //  If set all table cell/row creation will be ignored
                    if (!OverrideTableGeneration)
                    {
                        if (ShowInheritedData)
                        {
                            this.ApplyTranslation(build);
                        }
                        else
                        {
                            if ((Console.ExpressionPrevious == OutputExpression.Left && Expression == OutputExpression.FullWidth) || (Console.ExpressionPrevious == OutputExpression.Left && Expression == OutputExpression.Left))
                                build.Append("\t\t\t\t\t\t\t<th><label>&nbsp;</label></th><td>&nbsp;</td></tr>");

                            if ((Console.ExpressionPrevious == OutputExpression.FullWidth && Expression == OutputExpression.Right) || (Console.ExpressionPrevious == OutputExpression.Right && Expression == OutputExpression.Right))
                                build.Append("\t\t\t\t\t\t<tr><th><label>&nbsp;</label></th>\n\t\t\t\t\t\t\t<td>&nbsp;</td>");

                            if (Expression == OutputExpression.FullWidth || Expression == OutputExpression.Left)
                                build.Append("\t\t\t\t\t\t<tr>");
                        }

                        build.AppendFormat("\n\t\t\t\t\t\t\t<th><label for=\"{0}\">{1}</label></th>", this.ID, this.TitleLabel);

                        //if (ShowInheritedData)
                        //    build.AppendFormat("\t\t\t\t\t\t\t<th class=\"local\"><label>{0}:</label></th>\t\t\t\t\t\t</tr>\t\t\t\t\t\t<tr>\t\t\t\t\t\t\t<td><div class=\"description\">{1}</div></td>\n", this.ID, this.TitleLabel);

                        build.AppendFormat("\n\t\t\t\t\t\t\t<td{0}{1}>{2}"
                            , (Expression == OutputExpression.FullWidth && Console.HasDoubleCols) ? " colspan=\"3\"" : null
                            , this.InputCellClassName(this.IsValid(isRequired))
                            , CustomErrorText
                            );
                    }
                    #endregion Wrapper
                }

                build.AppendFormat("\n\t\t\t\t\t\t\t\t<div class=\"{0}\">", (Expression == OutputExpression.FullWidth) ? this.Class_Wide
                    : (OverrideTableGeneration ? "halfer" : "half")
                );

               

                build.Append(element.ToString());

                build.Append("\n\t\t\t\t\t\t\t\t</div>");

                //  If set all table cell/row creation will be ignored
                if (!OverrideTableGeneration)
                {
                    build.Append("\n\t\t\t\t\t\t\t</td>");

                    if (Expression == OutputExpression.FullWidth || Expression == OutputExpression.Right)
                        build.Append("\n\t\t\t\t\t\t</tr>\n");
                }
            }
            else
            {
                string output = outputValue;
                if (!string.IsNullOrEmpty(this.OutputText))
                {
                    if (!this.IsSourceCode)
                        output = Data.Utility.CleanLineFeed(WebUtility.HtmlEncode(this.OutputText), true);
                }

                build.Append(GetSimpleTextElement(this.Title, this.Mandatory, output, this.InteractiveHelp));
            }


            build.ApiResponse.Fields.Add(new Api.MediakiwiField()
            {
                Event = m_AutoPostBack ? Api.MediakiwiJSEvent.change : Api.MediakiwiJSEvent.none,
                Title = MandatoryWrap(this.Title),
                Value = this.OutputText,
                Expression = this.Expression,
                PropertyName = this.ID,
                PropertyType = (Property == null) ? typeof(string).FullName : Property.PropertyType.FullName,
                VueType = Api.MediakiwiFormVueType.wimTextArea,
                ReadOnly = this.IsReadOnly
            });

            return ReadCandidate(OutputText);
        }

        /// <summary>
        /// </summary>
        /// <value></value>
        /// <returns></returns>
             public override bool IsValid(bool isRequired)
        {
            this.Mandatory = isRequired;
                if (Console.CurrentListInstance.wim.IsSaveMode)
                {
                    //  Custom error validation
                    if (!base.IsValid(isRequired))
                        return false;

                    if (Mandatory && string.IsNullOrEmpty(OutputText))
                        return false;
                    if (this.MaxValueLength > 0 && !string.IsNullOrEmpty(OutputText) && OutputText.Length > MaxValueLength)
                        return false;
                }
                return true;
            
        }

        #region IContentInfo Members
        #endregion
    }
}
