using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Sushi.Mediakiwi.Data;

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
        public bool AutoPostBack
        {
            set { m_AutoPostBack = value; }
            get { return m_AutoPostBack; }
        }

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
            if (Property != null && Property.PropertyType == typeof(Data.CustomData))
                SetContentContainer(field);

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

                    if (Property.PropertyType == typeof(Data.CustomData))
                    {
                        m_Candidate = m_ContentContainer[field.Property].Value == "1";
                    }
                    else
                    {
                        object value = Property.GetValue(SenderInstance, null);
                        if (value != null)
                        {
                            if (Property.PropertyType == typeof(bool))
                                m_Candidate = Boolean.Parse(value.ToString());
                            if (Property.PropertyType == typeof(string))
                                m_Candidate = value.ToString() == "1";
                        }
                    }
                }
            }
            else
            {
                m_Candidate = Console.Form(this.ID) == "1" || Console.Form(this.ID) == "yes" || Console.Form(this.ID) == "true" || Console.Form(this.ID) == "True";
            }

            if (!IsBluePrint && Property != null && Property.CanWrite)
            {
                if (Property.PropertyType == typeof(Data.CustomData))
                    ApplyContentContainer(field, m_Candidate ? "1" : "0");
                else if (Property.PropertyType == typeof(bool))
                    Property.SetValue(SenderInstance, m_Candidate, null);
                else if (Property.PropertyType == typeof(string))
                    Property.SetValue(SenderInstance, m_Candidate ? "1" : "0", null);
            }


            OutputText = m_Candidate
                ? Labels.ResourceManager.GetString("yes", new CultureInfo(Console.CurrentApplicationUser.LanguageCulture))
                : Labels.ResourceManager.GetString("no", new CultureInfo(Console.CurrentApplicationUser.LanguageCulture));

            //  Inherited content section
            if (ShowInheritedData)
            {
                if (field != null && !string.IsNullOrEmpty(field.InheritedValue))
                {
                    InhertitedOutputText = (field.InheritedValue == "1").ToString();
                }
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
            this.SetWriteEnvironment();

            this.IsCloaked = isCloaked;
            this.Mandatory = isRequired;
            if (OverrideEditMode) isEditMode = false;

            bool isEnabled = true;
            if (!isEditMode)
                isEnabled = false;

            if (isEditMode || this.Console.CurrentListInstance.wim.IsEditMode)
            {
                #region Element creation
                StringBuilder element = new StringBuilder();
                string titleTag = string.Concat(Title, Mandatory ? "<em>*</em>" : "");

                // [MR:09-03-2018]  When this checkbox is disabled, but checked,  we still want to post the value with the formpost 
                // this didn't happen, so you would end up having 'false' values all the time
                if (isEnabled == false && m_Candidate)
                {
                    element.AppendFormat("<input type=\"hidden\" name=\"{0}\" id=\"{0}\" value=\"1\" />", this.ID);
                }
                element.AppendFormat("\n\t\t\t\t\t\t\t\t\t<input type=\"checkbox\" class=\"radio{1}\" value=\"1\" name=\"{0}\" id=\"{0}\"{2}{3}/>{4}"
                        , this.ID
                        , this.InputPostBackClassName()//AutoPostBack ? string.Concat(" ", PostBackValue) : string.Empty
                        , m_Candidate ? " checked=\"checked\"" : string.Empty
                        , isEnabled ? null : " disabled=\"disabled\""
                        , string.IsNullOrEmpty(this.InputPostText) ? null : this.InputPostText
                        );

                #endregion Element creation

                if (IsCloaked)
                    build.AppendCloaked(element.ToString());
                else
                {
                    #region Wrapper
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

                    build.AppendFormat("\n\t\t\t\t\t\t\t\t<div class=\"{0}\">", (Expression == OutputExpression.FullWidth) ? this.Class_Wide : "half");

                    build.Append(element.ToString());

                    build.Append("\n\t\t\t\t\t\t\t\t</div>");
                    build.Append("\n\t\t\t\t\t\t\t</td>");

                    if (Expression == OutputExpression.FullWidth || Expression == OutputExpression.Right)
                        build.Append("\n\t\t\t\t\t\t</tr>\n");

                    #endregion Wrapper
                }
            }
            else
                build.Append(GetSimpleTextElement(this.Title, this.Mandatory, this.OutputText, this.InteractiveHelp, true));

            build.ApiResponse.Fields.Add(new Api.MediakiwiField()
            {
                Event = AutoPostBack ? Api.MediakiwiJSEvent.change : Api.MediakiwiJSEvent.none,
                Title = MandatoryWrap(this.Title),
                Value = (this.m_Candidate ? true : false),
                Expression = this.Expression,
                PropertyName = this.ID,
                PropertyType = (Property == null) ? typeof(bool).FullName : Property.PropertyType.FullName,
                VueType = Api.MediakiwiFormVueType.wimChoiceCheckbox,
                InputPost = m_InputPostText,
                ReadOnly = this.IsReadOnly
            });

            return ReadCandidate(m_Candidate ? "1" : "0");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool IsValid(bool isRequired)
        {
            this.Mandatory = isRequired;
                if (Console.CurrentListInstance.wim.IsSaveMode)
                {
                    //  Custom error validation
                    if (!base.IsValid(isRequired))
                        return false;
                }
                return true;
            
        }


    }
}
