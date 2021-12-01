using Sushi.Mediakiwi.Data;
using System;
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
            {
                MustMatch = new Regex(mustMatchRegex);
            }
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

        /// <summary>
        /// 
        /// </summary>
        public Regex MustMatch { get; set; }

        private bool _allowHtmlTags;
        /// <summary>
        /// Gets or sets a value indicating if HTML tags are allowed. If set to true, no HTML encoding is applied to the user input.
        /// </summary>
        public bool AllowHtmlTags
        {
            get
            {
                if (IsSourceCode)
                {
                    return true;
                }
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
            SetMultiFieldTitleHTML("Text", "icon-align-left");
            if (Property != null && Property.PropertyType == typeof(CustomData))
            {
                SetContentContainer(field);
            }

            string candidate = null;

            bool isCandidateUserInput = false;
            if (IsInitialLoad || !isEditMode)
            {
                if (IsBluePrint)
                {
                    if (field != null)
                    {
                        candidate = field.Value;
                    }
                }
                else
                {
                    if (Property.PropertyType == typeof(CustomData))
                    {
                        candidate = m_ContentContainer[field.Property].Value;
                    }
                    else if (Property.PropertyType == typeof(string))
                    {
                        object value = Property.GetValue(SenderInstance, null);
                        if (value != null)
                        {
                            candidate = value.ToString();
                        }
                    }
                }
            }
            else
            {
                candidate = Console.Form(ID);
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

            if (candidate != null)
            {
                candidate = candidate.Trim();
            }

            // Possible return types: System.String

            if (!IsBluePrint && Property != null && Property.CanWrite)
            {
                if (Property.PropertyType == typeof(CustomData))
                {
                    ApplyContentContainer(field, candidate);
                }
                else if (Property.PropertyType == typeof(string))
                {
                    //if candidate is user input it needs to cleaned
                    if (isCandidateUserInput && candidate != null && candidate is string candidateString)
                    {
                        if (CommonConfiguration.HTML_ENCODE_TEXTAREA_INPUT && !AllowHtmlTags)
                        {
                            //encode html
                            candidateString = WebUtility.HtmlEncode(candidateString);
                        }

                        //enforce max length of string
                        if (MaxValueLength > 0)
                        {
                            candidateString = Utility.ConvertToFixedLengthText(candidateString, MaxValueLength);
                        }

                        candidate = candidateString;
                    }

                    Property.SetValue(SenderInstance, candidate, null);
                }
            }
            OutputText = candidate;

            //  Inherited content section
            if (ShowInheritedData && field != null && !string.IsNullOrEmpty(field.InheritedValue))
            {
                InhertitedOutputText = field.InheritedValue;
                InhertitedOutputText = Utility.CleanLineFeed(WebUtility.HtmlEncode(InhertitedOutputText), true);
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
            SetWriteEnvironment();
            string formName = GetFormMapClass();

            IsCloaked = isCloaked;
            Mandatory = isRequired;
            if (OverrideEditMode)
            {
                isEditMode = false;
            }

            //get the value of the property
            string outputValue = OutputText;
            if (Property?.PropertyType == typeof(string) && CommonConfiguration.HTML_ENCODE_TEXTAREA_INPUT && !AllowHtmlTags)
            {
                //in this case the user input was HTML encoded. it needs to be decoded again to get back to its orignal value
                outputValue = WebUtility.HtmlDecode(outputValue);
            }

            var isEnabled = IsEnabled();

            // [MR:03-06-2021] Apply shared field clickable icon.
            var sharedInfoApply = ApplySharedFieldInformation(isEnabled, outputValue);

            // If we have a document assigned, overwrite the current one
            if (sharedInfoApply.isShared)
            {
                // Enable readonly when shared
                isEnabled = sharedInfoApply.isEnabled;

                // When Currently not cloaked, do so if its a shared field
                if (IsCloaked == false && sharedInfoApply.isHidden)
                {
                    IsCloaked = sharedInfoApply.isHidden;
                }

                outputValue = sharedInfoApply.outputValue;
            }

            if (isEnabled && (isEditMode || IsSourceCode))
            {
                #region Element creation

                StringBuilder element = new StringBuilder();
                //  [MM:22.12.14] If this textarea acts as an code field, the breaks should be left alone
                string cleanData = outputValue;
                if (!IsSourceCode)
                {
                    cleanData = Utility.CleanLineFeed(outputValue, false);
                }

                element.AppendFormat("<textarea cols=\"32\" rows=\"4\"{2} name=\"{0}\" id=\"{0}\"{3}{4}>{1}</textarea>"
                  , ID
                  , cleanData
                  , InputClassName(IsValid(isRequired), (IsSourceCode ? string.Concat(" SourceCode", (isEditMode ? " editable" : string.Empty)) : string.Empty))
                  , IsSourceCode && string.IsNullOrEmpty(Console.Request.Query["xml"]) ? " data-done=\"1\"" : null
                  , isEnabled ? null : " disabled=\"disabled\""
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
                    if (IsSourceCode)
                    {
                        Console.CurrentListInstance.wim.Page.Head.EnableColorCodingLibrary = true;
                        if (string.IsNullOrEmpty(m_InteractiveHelp))
                        {
                            m_InteractiveHelp = "Press 'ESC' for fullscreen (and also to reset)";
                        }
                    }

                    //  If set all table cell/row creation will be ignored
                    if (!OverrideTableGeneration)
                    {
                        if (ShowInheritedData)
                        {
                            ApplyTranslation(build);
                        }
                        else
                        {
                            if ((Console.ExpressionPrevious == OutputExpression.Left && Expression == OutputExpression.FullWidth) || (Console.ExpressionPrevious == OutputExpression.Left && Expression == OutputExpression.Left))
                            {
                                build.Append("<th><label>&nbsp;</label></th><td>&nbsp;</td></tr>");
                            }

                            if ((Console.ExpressionPrevious == OutputExpression.FullWidth && Expression == OutputExpression.Right) || (Console.ExpressionPrevious == OutputExpression.Right && Expression == OutputExpression.Right))
                            {
                                build.Append("<tr><th><label>&nbsp;</label></th><td>&nbsp;</td>");
                            }

                            if (Expression == OutputExpression.FullWidth || Expression == OutputExpression.Left)
                            {
                                build.Append("<tr>");
                            }
                        }

                        if (string.IsNullOrWhiteSpace(EditSharedFieldLink) == false)
                        {
                            build.Append($"<th><label for=\"{ID}\">{EditSharedFieldLink.Replace("[LABEL]", TitleLabel)}</label></th>");
                        }
                        else
                        {
                            build.Append($"<th><label for=\"{ID}\">{TitleLabel}</label></th>");
                        }

                        //  build.AppendFormat("\n\t\t\t\t\t\t\t<th><label for=\"{0}\">{1}</label></th>", ID, TitleLabel);

                        //if (ShowInheritedData)
                        //    build.AppendFormat("\t\t\t\t\t\t\t<th class=\"local\"><label>{0}:</label></th>\t\t\t\t\t\t</tr>\t\t\t\t\t\t<tr>\t\t\t\t\t\t\t<td><div class=\"description\">{1}</div></td>\n", ID, TitleLabel);

                        build.AppendFormat("<td{0}{1}>{2}"
                            , (Expression == OutputExpression.FullWidth && Console.HasDoubleCols) ? " colspan=\"3\"" : null
                            , InputCellClassName(IsValid(isRequired))
                            , CustomErrorText
                            );
                    }
                    #endregion Wrapper
                }

                build.AppendFormat("<div class=\"{0}\">", (Expression == OutputExpression.FullWidth) ? Class_Wide
                    : (OverrideTableGeneration ? "halfer" : "half")
                );



                // Add Shared Icon (if any)
                if (string.IsNullOrWhiteSpace(SharedIcon) == false && IsCloaked == false)
                {
                    build.Append(SharedIcon);
                }

                build.Append(element.ToString());

                build.Append("</div>");

                //  If set all table cell/row creation will be ignored
                if (!OverrideTableGeneration)
                {
                    build.Append("</td>");

                    if (Expression == OutputExpression.FullWidth || Expression == OutputExpression.Right)
                    {
                        build.Append("</tr>\n");
                    }
                }
            }
            else
            {
                string output = outputValue;
                if (!string.IsNullOrEmpty(OutputText) && !IsSourceCode)
                {
                    output = Utility.CleanLineFeed(WebUtility.HtmlEncode(OutputText), true);
                }

                build.Append(GetSimpleTextElement(output));
            }


            build.ApiResponse.Fields.Add(new Api.MediakiwiField()
            {
                Event = m_AutoPostBack ? Api.MediakiwiJSEvent.change : Api.MediakiwiJSEvent.none,
                Title = MandatoryWrap(Title),
                Value = OutputText,
                Expression = Expression,
                PropertyName = ID,
                PropertyType = (Property == null) ? typeof(string).FullName : Property.PropertyType.FullName,
                VueType = Api.MediakiwiFormVueType.wimTextArea,
                ReadOnly = IsReadOnly,
                ContentTypeID = ContentTypeSelection,
                IsAutoPostback = m_AutoPostBack,
                IsMandatory = Mandatory,
                MaxLength = MaxValueLength,
                HelpText = InteractiveHelp,
                FormSection = formName
            });

            return ReadCandidate(OutputText);
        }

        /// <summary>
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        public override bool IsValid(bool isRequired)
        {
            Mandatory = isRequired;
            if (Console.CurrentListInstance.wim.IsSaveMode)
            {
                //  Custom error validation
                if (!base.IsValid(isRequired))
                {
                    return false;
                }

                if (Mandatory && string.IsNullOrEmpty(OutputText))
                {
                    var hasValue = HasSharedValue();
                    if (hasValue.isSharedField)
                    {
                        return hasValue.hasValue;
                    }

                    return false;
                }
                if (MaxValueLength > 0 && !string.IsNullOrEmpty(OutputText) && OutputText.Length > MaxValueLength)
                {
                    return false;
                }
            }
            return true;

        }
    }
}
