using Sushi.Mediakiwi.Data;
using System;
using System.Globalization;
using System.Text;

namespace Sushi.Mediakiwi.Framework.ContentInfoItem
{
    /// <summary>
    /// Possible return types: System.Boolean
    /// </summary>
    public class Choice_CheckboxAttribute : ContentSharedAttribute, IContentInfo 
    {
        /// <summary>
        /// Possible return types: System.Boolean
        /// </summary>
        /// <param name="title">The title.</param>
        public Choice_CheckboxAttribute(string title)
            : this(title, null) { }

        /// <summary>
        /// Possible return types: System.Boolean
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        public Choice_CheckboxAttribute(string title, string interactiveHelp)
            : this(title, false, interactiveHelp) { }


        /// <summary>
        /// Possible return types: System.Boolean
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="autoPostback">if set to <c>true</c> [auto postback].</param>
        public Choice_CheckboxAttribute(string title, bool autoPostback)
            : this(title, autoPostback, null) { }

        /// <summary>
        /// Possible return types: System.Boolean
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="autoPostback">if set to <c>true</c> [auto postback].</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        public Choice_CheckboxAttribute(string title, bool autoPostback, string interactiveHelp)
        {
            m_CanHaveExpression = true;
            ContentTypeSelection = ContentType.Choice_Checkbox;
            Title = title;
            AutoPostBack = autoPostback;
            InteractiveHelp = interactiveHelp;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool AutoPostBack { get; set; }

        /// <summary>
        /// Sets the candidate.
        /// </summary>
        /// <param name="isEditMode">if set to <c>true</c> [is edit mode].</param>
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
            if (Property != null && Property.PropertyType == typeof(CustomData))
            {
                SetContentContainer(field);
            }

            m_Candidate = false;
            if (IsInitialLoaded || !isEditMode)
            {
                if (IsBluePrint)
                {
                    if (field != null)
                    {
                        m_Candidate = field.Value == "1";
                    }
                }
                else
                {

                    if (Property.PropertyType == typeof(CustomData))
                    {
                        m_Candidate = m_ContentContainer[field.Property].Value == "1";
                    }
                    else
                    {
                        object value = Property.GetValue(SenderInstance, null);
                        if (value != null)
                        {
                            if (Property.PropertyType == typeof(bool))
                            {
                                m_Candidate = bool.Parse(value.ToString());
                            }
                            if (Property.PropertyType == typeof(string))
                            {
                                m_Candidate = value.ToString() == "1";
                            }
                        }
                    }
                }
            }
            else
            {
                m_Candidate = Console.Form(ID) == "1" || Console.Form(ID) == "yes" || Console.Form(ID) == "true" || Console.Form(ID) == "True";
            }

            if (!IsBluePrint && Property != null && Property.CanWrite)
            {
                if (Property.PropertyType == typeof(CustomData))
                {
                    ApplyContentContainer(field, m_Candidate ? "1" : "0");
                }
                else if (Property.PropertyType == typeof(bool))
                {
                    Property.SetValue(SenderInstance, m_Candidate, null);
                }
                else if (Property.PropertyType == typeof(string))
                {
                    Property.SetValue(SenderInstance, m_Candidate ? "1" : "0", null);
                }
            }

            OutputText = m_Candidate
                ? Labels.ResourceManager.GetString("yes", new CultureInfo(Console.CurrentApplicationUser.LanguageCulture))
                : Labels.ResourceManager.GetString("no", new CultureInfo(Console.CurrentApplicationUser.LanguageCulture));

            //  Inherited content section
            if (ShowInheritedData && field != null && !string.IsNullOrEmpty(field.InheritedValue))
            {
                InhertitedOutputText = (field.InheritedValue == "1").ToString();
            }
        }

        bool m_Candidate;

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

            IsCloaked = isCloaked;
            Mandatory = isRequired;
            if (OverrideEditMode)
            {
                isEditMode = false;
            }

            bool isEnabled = IsEnabled();

            // [MR:03-06-2021] Apply shared field clickable icon.
            var sharedInfoApply = ApplySharedFieldInformation(isEnabled, OutputText);

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

                if (string.IsNullOrWhiteSpace(sharedInfoApply.outputValue) == false)
                {
                    OutputText = sharedInfoApply.outputValue;
                }
            }

            if (!isEditMode)
                isEnabled = false;

            if ((isEditMode || Console.CurrentListInstance.wim.IsEditMode) && isEnabled)
            {
                #region Element creation

                StringBuilder element = new StringBuilder();
                string titleTag = string.Concat(Title, Mandatory ? "<em>*</em>" : "");

                // [MR:09-03-2018]  When this checkbox is disabled, but checked,  we still want to post the value with the formpost 
                // this didn't happen, so you would end up having 'false' values all the time
                if (isEnabled == false && m_Candidate)
                {
                    element.AppendFormat("<input type=\"hidden\" name=\"{0}\" id=\"{0}\" value=\"1\" />", ID);
                }
                element.AppendFormat("<input type=\"checkbox\" class=\"radio{1}\" value=\"1\" name=\"{0}\" id=\"{0}\"{2}{3}/>{4}"
                        , ID
                        , InputPostBackClassName()//AutoPostBack ? string.Concat(" ", PostBackValue) : string.Empty
                        , m_Candidate ? " checked=\"checked\"" : string.Empty
                        , isEnabled ? null : " disabled=\"disabled\""
                        , string.IsNullOrEmpty(InputPostText) ? null : InputPostText
                        );

                #endregion Element creation

                if (IsCloaked)
                {
                    build.AppendCloaked(element.ToString());
                }
                else
                {
                    #region Wrapper

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

                    build.AppendFormat("<th><label for=\"{0}\">{1}</label></th>", ID, TitleLabel);

                    //if (ShowInheritedData)
                    //    build.AppendFormat("\t\t\t\t\t\t\t<th class=\"local\"><label>{0}:</label></th>\t\t\t\t\t\t</tr>\t\t\t\t\t\t<tr>\t\t\t\t\t\t\t<td><div class=\"description\">{1}</div></td>\n", ID, TitleLabel);

                    build.AppendFormat("<td{0}{1}>{2}"
                        , (Expression == OutputExpression.FullWidth && Console.HasDoubleCols) ? " colspan=\"3\"" : null
                        , InputCellClassName(IsValid(isRequired))
                        , CustomErrorText
                        );

                    build.AppendFormat("<div class=\"{0}\">", (Expression == OutputExpression.FullWidth) ? Class_Wide : "half");

                    build.Append(element.ToString());

                    build.Append("</div></td>");

                    if (Expression == OutputExpression.FullWidth || Expression == OutputExpression.Right)
                    {
                        build.Append("</tr>");
                    }

                    #endregion Wrapper
                }
            }
            else
            {
                build.Append(GetSimpleTextElement(OutputText, true));
            }

            build.ApiResponse.Fields.Add(new Api.MediakiwiField()
            {
                Event = AutoPostBack ? Api.MediakiwiJSEvent.change : Api.MediakiwiJSEvent.none,
                Title = MandatoryWrap(Title),
                Value = (m_Candidate ? true : false),
                Expression = Expression,
                PropertyName = ID,
                PropertyType = (Property == null) ? typeof(bool).FullName : Property.PropertyType.FullName,
                VueType = Api.MediakiwiFormVueType.wimChoiceCheckbox,
                InputPost = m_InputPostText,
                ReadOnly = IsReadOnly,
                ContentTypeID = ContentTypeSelection
            });

            return ReadCandidate(m_Candidate ? "1" : "0");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool IsValid(bool isRequired)
        {
            Mandatory = isRequired;
            if (Console.CurrentListInstance.wim.IsSaveMode && !base.IsValid(isRequired))
            {
                return false;
            }
            return true;
        }
    }
}
