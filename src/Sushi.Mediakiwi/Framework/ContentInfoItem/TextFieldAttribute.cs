using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Sushi.Mediakiwi.Framework.Api;
using Newtonsoft.Json;
using Sushi.Mediakiwi.Data;
using System.Net;

namespace Sushi.Mediakiwi.Framework
{
    public enum InputType
    {
        Text,
        Money,
        Numeric
    }
}

namespace Sushi.Mediakiwi.Framework.ContentInfoItem
{
    /// <summary>
    /// Possible return types: System.String, System.Int32, System.Int32[nullable], System.Decimal, System.Decimal[nullable], System.Guid
    /// </summary>
    public class TextFieldAttribute : ContentEditableSharedAttribute, IContentInfo
    {
        /// <summary>
        /// Possible return types: System.String, System.Int32, System.Int32[nullable], System.Decimal, System.Decimal[nullable], System.Guid
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="maxlength">The maxlength.</param>
        public TextFieldAttribute(string title, int maxlength)
            : this(title, maxlength, false) { }


        /// <summary>
        /// Possible return types: System.String, System.Int32, System.Int32[nullable], System.Decimal, System.Decimal[nullable], System.Guid
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="maxlength">The maxlength.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        public TextFieldAttribute(string title, int maxlength, bool mandatory)
            : this(title, maxlength, mandatory, null) { }

        /// <summary>
        /// Possible return types: System.String, System.Int32, System.Int32[nullable], System.Decimal, System.Decimal[nullable], System.Guid
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="maxlength">The maxlength.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        public TextFieldAttribute(string title, int maxlength, bool mandatory, string interactiveHelp)
            : this(title, maxlength, mandatory, interactiveHelp, null) { }

        /// <summary>
        /// Possible return types: System.String, System.Int32, System.Int32[nullable], System.Decimal, System.Decimal[nullable], System.Guid
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="maxlength">The maxlength.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        /// <param name="mustMatchRegex">The must match regex.</param>
        public TextFieldAttribute(string title, int maxlength, bool mandatory, string interactiveHelp, string mustMatchRegex)
            : this(title, maxlength, mandatory, false, interactiveHelp, mustMatchRegex) { }

        /// <summary>
        /// Possible return types: System.String, System.Int32, System.Int32[nullable], System.Decimal, System.Decimal[nullable], System.Guid
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="maxlength">The maxlength.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        /// <param name="autoPostback">if set to <c>true</c> [auto postback].</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        public TextFieldAttribute(string title, int maxlength, bool mandatory, bool autoPostback, string interactiveHelp)
            : this(title, maxlength, mandatory, autoPostback, interactiveHelp, null) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="TextFieldAttribute"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="maxlength">The maxlength.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        /// <param name="autoPostback">if set to <c>true</c> [auto postback].</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        /// <param name="mustMatchRegex">The must match regex.</param>
        public TextFieldAttribute(string title, int maxlength, bool mandatory, bool autoPostback, string interactiveHelp, string mustMatchRegex)
            : this(title, maxlength, mandatory, autoPostback, interactiveHelp, mustMatchRegex, InputType.Text) { }



        /// <summary>
        /// Possible return types: System.String, System.Int32, System.Int32[nullable], System.Decimal, System.Decimal[nullable], System.Guid
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="maxlength">The maxlength.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        /// <param name="autoPostback">if set to <c>true</c> [auto postback].</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        /// <param name="mustMatchRegex">The must match regex.</param>
        public TextFieldAttribute(string title, int maxlength, bool mandatory, bool autoPostback, string interactiveHelp, string mustMatchRegex, InputType inputType)
        {
            m_CanHaveExpression = true;
            ContentTypeSelection = ContentType.TextField;
            Title = title;
            MaxValueLength = maxlength;
            Mandatory = mandatory;
            InteractiveHelp = interactiveHelp;
            AutoPostBack = autoPostback;
            this.TextType = inputType;
            if (mustMatchRegex != null)
                MustMatch = new Regex(mustMatchRegex, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            else
            {
                if (this.TextType == InputType.Money)
                    MustMatch = new Regex(Data.Utility.GlobalRegularExpression.OnlyDecimal, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
                else if (this.TextType == InputType.Numeric)
                    MustMatch = new Regex(Data.Utility.GlobalRegularExpression.OnlyNumeric, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            }
        }

        public string PreInputHtml { get; set; }
        public string ClassName { get; set; }
        public string PostInputHtml { get; set; }

        /// <summary>
        /// Gets or sets the type of the text.
        /// </summary>
        /// <value>The type of the text.</value>
        public InputType TextType { get; set; }

        /// <summary>
        /// If true, than the 'onChange' event will trigger an Async call. 
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is asynchronous]; otherwise, <c>false</c>.
        /// </value>
        public bool IsAsync { get; set; }



        /// <summary>
        /// Gets or sets a value indicating whether this instance is password field.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is password field; otherwise, <c>false</c>.
        /// </value>
        public bool IsPasswordField { get; set; }

        private Regex m_MustMatch;
        /// <summary>
        /// 
        /// </summary>
        public Regex MustMatch
        {
            set { m_MustMatch = value; }
            get
            {
                return m_MustMatch;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if HTML tags are allowed. If set to true, no HTML encoding is applied to the user input.
        /// </summary>
        public bool AllowHtmlTags { get; set; }

        /// <summary>
        /// Sets the candidate.
        /// </summary>
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
            this.SetMultiFieldTitleHTML("Header", "icon-header");

            if (Property != null && Property.PropertyType == typeof(Data.CustomData))
            {
                SetContentContainer(field);
            }

            object candidate = null;
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
                    if (Property.PropertyType == typeof(Data.CustomData))
                    {
                        candidate = m_ContentContainer[field.Property].Value;
                    }
                    else
                        candidate = Property.GetValue(SenderInstance, null);
                }
            }
            else
            {
                candidate = Console.Form(this.ID);
                isCandidateUserInput = true;

            }

            if (!IsBluePrint && Property != null && Property.CanWrite)
            {
                //  Possible return types: System.String, System.Int32, System.Decimal

                if (Property.PropertyType == typeof(Data.CustomData))
                {
                    ApplyContentContainer(field, (candidate == null ? null : candidate.ToString()));
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
                        if (this.MaxValueLength > 0)
                            candidateString = Data.Utility.ConvertToFixedLengthText(candidateString, this.MaxValueLength);

                        candidate = candidateString;
                    }

                    Property.SetValue(SenderInstance, candidate, null);
                }

                else if (Property.PropertyType == typeof(int))
                {
                    candidate = Data.Utility.ConvertToInt(candidate);
                    Property.SetValue(SenderInstance, candidate, null);
                }
                else if (Property.PropertyType == typeof(int?))
                {
                    if (candidate == null || candidate.ToString() == string.Empty)
                        Property.SetValue(SenderInstance, null, null);
                    else
                        Property.SetValue(SenderInstance, Data.Utility.ConvertToInt(candidate), null);
                }
                else if (Property.PropertyType == typeof(decimal))
                {
                    if (candidate != null && Console.CurrentListInstance.wim.CurrentCulture.NumberFormat.CurrencyDecimalSeparator == ",")
                    {
                        candidate = candidate.ToString()
                           .Replace(".", ",")
                           .Replace(",", ".");
                    }
                    

                    decimal candidate2 = Data.Utility.ConvertToDecimal(candidate);
                    if (TextType == InputType.Money) candidate = Data.Utility.ConvertToDecimalString(candidate2);

                    Property.SetValue(SenderInstance, candidate2, null);
                }
                else if (Property.PropertyType == typeof(double))
                {
                    if (candidate != null && Console.CurrentListInstance.wim.CurrentCulture.NumberFormat.CurrencyDecimalSeparator == ",")
                    {
                        candidate = candidate.ToString()
                           .Replace(".", ",")
                           .Replace(",", ".");
                    }
                    

                    double candidate2 = Data.Utility.ConvertToDouble(candidate);
                    Property.SetValue(SenderInstance, candidate2, null);
                }
                else if (Property.PropertyType == typeof(decimal?))
                {
                    if (candidate == null || candidate.ToString() == string.Empty)
                    {
                        Property.SetValue(SenderInstance, null, null);
                    }
                    else
                    {
                        if (Console.CurrentListInstance.wim.CurrentCulture.NumberFormat.CurrencyDecimalSeparator == ",")
                        {
                            candidate = candidate.ToString()
                                .Replace(".", ",")
                                .Replace(",", ".");
                        }
                        decimal candidate2 = Data.Utility.ConvertToDecimal(candidate);
                        if (TextType == InputType.Money)
                            candidate = Data.Utility.ConvertToDecimalString(candidate2);
                        Property.SetValue(SenderInstance, candidate2, null);
                    }
                }
                else if (Property.PropertyType == typeof(Guid))
                {
                    candidate = Data.Utility.ConvertToGuid(candidate);
                    Property.SetValue(SenderInstance, candidate, null);
                }
            }
            else if (!IsBluePrint && field != null)
            {
                field.Value = candidate == null ? null : candidate.ToString();
            }

            this.OutputText = candidate == null ? null : candidate.ToString();

            //  Inherited content section
            if (ShowInheritedData)
            {
                if (field != null && !string.IsNullOrEmpty(field.InheritedValue))
                {
                    InhertitedOutputText = field.InheritedValue;
                    InhertitedOutputText = string.IsNullOrEmpty(InhertitedOutputText) ? null : WebUtility.HtmlEncode(this.InhertitedOutputText);
                }
            }

        }

        public bool AutoPostBack
        {
            set { m_AutoPostBack = value; }
            get { return m_AutoPostBack; }
        }

        public bool IsRequired
        {
            set;
            get;
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
            this.IsRequired = isRequired;
            this.Mandatory = isRequired;
            if (OverrideEditMode) isEditMode = false;

            bool isMoneyMode = false;
            if (TextType == InputType.Money || TextType == InputType.Numeric)
            {
                if (!this.Console.CurrentListInstance.wim.IsEditMode)
                    isMoneyMode = true;
            }

            //get the value of the property
            string outputValue = this.OutputText;
            if (Property?.PropertyType == typeof(String) && CommonConfiguration.HTML_ENCODE_TEXTAREA_INPUT && !AllowHtmlTags)
            {
                //in this case the user input was HTML encoded. it needs to be decoded again to get back to its orignal value
                outputValue = WebUtility.HtmlDecode(outputValue);
            }

            if (!string.IsNullOrWhiteSpace(PostInputHtml))
            {
                PostInputHtml = $"<label>{PostInputHtml}</label>";
            }

            bool isEnabled = true;

            if (isMoneyMode || isEditMode || this.Console.CurrentListInstance.wim.IsEditMode)
            {
                if (!isEditMode || isMoneyMode)
                    isEnabled = false;

                isEnabled = this.IsEnabled(isEnabled);

                #region Element creation

                // [MR:03-06-2021] Apply shared field clickable icon.
                var sharedInfoApply = ApplySharedFieldInformation(isEnabled, outputValue);
                isEnabled = sharedInfoApply.isEnabled;
                outputValue = sharedInfoApply.outputValue;

                StringBuilder element = new StringBuilder();

                string cellClassName = "";
                string styleTag = "";
                int breakpoint = 10;

                if (!string.IsNullOrEmpty(cellClassName))
                    cellClassName = string.Format(" class=\"{0}\"", cellClassName.Trim());

                string titleTag = string.Concat(Title, Mandatory ? "<em>*</em>" : null);

                if ((TextType == InputType.Money || TextType == InputType.Numeric) && !isEnabled) styleTag += "color:#005770;";

                if (TextType == InputType.Money || TextType == InputType.Numeric
                    || (Property != null &&
                        (Property.PropertyType == typeof(decimal?)
                        || Property.PropertyType == typeof(decimal)
                        || Property.PropertyType == typeof(int)
                        || Property.PropertyType == typeof(int?)
                        ))
                    )
                {
                    styleTag += "text-align:right;";
                }

                if (!string.IsNullOrEmpty(styleTag))
                    styleTag = string.Format(" style=\"{0}\"", styleTag);

                element.AppendFormat("\n\t\t\t\t\t\t\t\t\t{10}<input type=\"{0}\"{1} name=\"{2}\" id=\"{2}\"{3} value=\"{4}\"{5}{6} {9}{12} />{7}{8}{11}"
                    , IsPasswordField ? "password" : "text" // 0
                    , this.InputClassName(IsValid(isRequired), IsAsync ? $"atext {ClassName}".TrimEnd() : ClassName) // 1
                    , this.ID // 2
                    , this.MaxValueLength > 0 ? string.Concat(" maxlength=\"", this.MaxValueLength, "\"") : string.Empty // 3
                    , WebUtility.HtmlEncode(outputValue) // 4
                    , styleTag // 5
                    , isEnabled ? null : " disabled=\"disabled\"" // 6
                    , (this.MaxValueLength != 0 && this.MaxValueLength <= breakpoint) ? this.InputPostText : null // 7
                    , (this.MaxValueLength == 0 || this.MaxValueLength > breakpoint) ? this.InputPostText : null // 8
                    , IsPasswordField ? "autocomplete=\"new-password\"" : "autocomplete=\"off\"" // 9
                    , PreInputHtml // 10
                    , PostInputHtml // 11
                    , string.IsNullOrWhiteSpace(this.InteractiveHelp) ? null : $" placeholder=\"{Data.Utility.CleanFormatting(this.InteractiveHelp)}\""
                    );

                #endregion Element creation


                if (IsCloaked)
                {
                    build.AppendCloaked(element.ToString());
                }
                else
                {
                    #region Wrapper

                    if (!OverrideTableGeneration)
                    {
                        if (ShowInheritedData)
                        {
                            this.ApplyTranslation(build);
                        }
                        else
                        {
                            if ((Console.ExpressionPrevious == OutputExpression.Left && Expression == OutputExpression.FullWidth) || (Console.ExpressionPrevious == OutputExpression.Left && Expression == OutputExpression.Left))
                                build.Append($"\t\t\t\t\t\t\t<th class=\"half\"><label>&nbsp;</label></th><td class=\"half\">&nbsp;</td></tr>");

                            if ((Console.ExpressionPrevious == OutputExpression.FullWidth && Expression == OutputExpression.Right) || (Console.ExpressionPrevious == OutputExpression.Right && Expression == OutputExpression.Right))
                                build.Append($"\t\t\t\t\t\t<tr><th class=\"half\"><label>&nbsp;</label></th>\n\t\t\t\t\t\t\t<td class=\"half\">&nbsp;</td>");

                            if (Expression == OutputExpression.FullWidth || Expression == OutputExpression.Left)
                                build.Append($"\t\t\t\t\t\t<tr>");
                        }

                        if (Expression == OutputExpression.FullWidth)
                        {
                            if (string.IsNullOrWhiteSpace(EditSharedFieldLink) == false)
                            {
                                build.Append($"<th class=\"full\"><label for=\"{ID}\">{EditSharedFieldLink.Replace("[LABEL]", this.TitleLabel)}</label></th>");
                            }
                            else
                            {
                                build.Append($"<th class=\"full\"><label for=\"{ID}\">{TitleLabel}</label></th>");
                            }
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(EditSharedFieldLink) == false)
                            {
                                build.Append($"<th class=\"half\"><label for=\"{ID}\">{EditSharedFieldLink.Replace("[LABEL]", this.TitleLabel)}</label></th>");
                            }
                            else 
                            {
                                build.Append($"<th class=\"half\"><label for=\"{ID}\">{TitleLabel}</label></th>");
                            }
                        }

                        //if (ShowInheritedData)
                        //    build.AppendFormat("\t\t\t\t\t\t\t<th class=\"local\"><label>{0}:</label></th>\t\t\t\t\t\t</tr>\t\t\t\t\t\t<tr>\t\t\t\t\t\t\t<td><div class=\"description\">{1}</div></td>\n", this.ID, this.TitleLabel);


                        build.AppendFormat("\n\t\t\t\t\t\t\t<td{0}{1}>{2}"
                            , (Expression == OutputExpression.FullWidth && Console.HasDoubleCols) ? " colspan=\"3\"" : null
                            , this.InputCellClassName(this.IsValid(isRequired))
                            , CustomErrorText
                            );
                    }

                    //build.AppendFormat("\n\t\t\t\t\t\t\t\t<div class=\"{0}\">", (Expression == OutputExpression.FullWidth) ? this.Class_Wide : "half");
                    build.AppendFormat("\n\t\t\t\t\t\t\t\t<div class=\"{0}\">"
                                    , (Expression == OutputExpression.FullWidth) ? this.Class_Wide
                                    : (OverrideTableGeneration ? "halfer" : "half")
                                );

                    // Add Shared Icon (if any)
                    if (string.IsNullOrWhiteSpace(SharedIcon) == false)
                    {
                        build.Append(SharedIcon);
                    }

                    // Add element 
                    build.Append(element.ToString());


                    build.Append("\n\t\t\t\t\t\t\t\t</div>");

                    //  If set all table cell/row creation will be ignored
                    if (!OverrideTableGeneration)
                    {
                        build.Append("\n\t\t\t\t\t\t\t</td>");

                        if (Expression == OutputExpression.FullWidth || Expression == OutputExpression.Right)
                            build.Append("\n\t\t\t\t\t\t</tr>\n");
                    }
                    #endregion Wrapper
                }
            }

            else
            {
                string output = string.IsNullOrEmpty(this.OutputText) ? null : WebUtility.HtmlEncode(outputValue);

                build.Append(GetSimpleTextElement(this.Title, this.Mandatory, output, this.InteractiveHelp, (isMoneyMode | MaxValueLength < BREAKPOINT), PostInputHtml));
            }

            var fieldata = new Api.MediakiwiField()
            {
                Event = AutoPostBack ? Api.MediakiwiJSEvent.change : Api.MediakiwiJSEvent.none,
                Title = MandatoryWrap(this.Title),
                Value = this.OutputText,
                Expression = this.Expression,
                PropertyName = this.ID,
                PropertyType = (Property == null) ? typeof(string).FullName : Property.PropertyType.FullName,
                VueType = Api.MediakiwiFormVueType.wimText,
                ClassName = this.InputClassName(IsValid(isRequired), IsAsync ? $"atext {ClassName}".TrimEnd() : ClassName, false),
                ReadOnly = this.IsReadOnly
            };

            if (IsCloaked)
                fieldata.Hidden = true;

            build.ApiResponse.Fields.Add(fieldata);

            return ReadCandidate(OutputText);
        }



        /// <summary>
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        public override bool IsValid(bool isRequired)
        {
            this.Mandatory = isRequired;
            if (Console.CurrentListInstance.wim.IsSaveMode || Console.CurrentListInstance.wim.ShouldValidate)
            {
                //  Custom error validation
                if (!base.IsValid(isRequired))
                    return false;

                if (Mandatory && string.IsNullOrEmpty(OutputText))
                    return false;
                if (!string.IsNullOrEmpty(OutputText) && this.MustMatch != null)
                {
                    bool isMatch = this.MustMatch.IsMatch(OutputText);
                    return isMatch;
                }
            }
            return true;
        }

     }
}
