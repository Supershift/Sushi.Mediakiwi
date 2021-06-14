using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.UI;
using System;
using System.Collections.Generic;
using System.Text;


namespace Sushi.Mediakiwi.Framework.ContentInfoItem
{
    /// <summary>
    /// Possible return types: System.Int16, System.Int32, System.String, System.Int32[nullable]
    /// </summary>
    public class Choice_RadioAttribute : ContentSharedAttribute, IContentInfo
    {
        /// <summary>
        /// Possible return types: System.Int16, System.Int32, System.String, System.Int32[nullable]
        /// </summary>
        /// <param name="title"></param>
        /// <param name="collectionPropertyName"></param>
        /// <param name="groupName"></param>
        public Choice_RadioAttribute(string title, string collectionPropertyName, string groupName)
            : this(title, collectionPropertyName, groupName, false) { }

        /// <summary>
        /// Possible return types: System.Int16, System.Int32, System.String
        /// </summary>
        /// <param name="title"></param>
        /// <param name="collectionPropertyName"></param>
        /// <param name="groupName"></param>
        /// <param name="mandatory"></param>
        public Choice_RadioAttribute(string title, string collectionPropertyName, string groupName, bool mandatory)
            : this(title, collectionPropertyName, groupName, mandatory, null) { }

        /// <summary>
        /// Possible return types: System.Int16, System.Int32, System.String
        /// </summary>
        /// <param name="title"></param>
        /// <param name="collectionPropertyName"></param>
        /// <param name="groupName"></param>
        /// <param name="mandatory"></param>
        /// <param name="interactiveHelp"></param>
        public Choice_RadioAttribute(string title, string collectionPropertyName, string groupName, bool mandatory, string interactiveHelp)
            : this(title, collectionPropertyName, groupName, mandatory, false, interactiveHelp) { }

        /// <summary>
        /// Possible return types: System.Int16, System.Int32, System.String
        /// </summary>
        /// <param name="title"></param>
        /// <param name="collectionPropertyName"></param>
        /// <param name="groupName"></param>
        /// <param name="mandatory"></param>
        /// <param name="autoPostback"></param>
        public Choice_RadioAttribute(string title, string collectionPropertyName, string groupName, bool mandatory, bool autoPostback)
            : this(title, collectionPropertyName, groupName, mandatory, autoPostback, null) { }

        /// <summary>
        /// Possible return types: System.Int16, System.Int32, System.String
        /// </summary>
        /// <param name="title"></param>
        /// <param name="collectionPropertyName"></param>
        /// <param name="groupName"></param>
        /// <param name="mandatory"></param>
        /// <param name="autoPostback"></param>
        /// <param name="interactiveHelp"></param>
        public Choice_RadioAttribute(string title, string collectionPropertyName, string groupName, bool mandatory, bool autoPostback, string interactiveHelp)
        {
            m_CanHaveExpression = true;
            ContentTypeSelection = ContentType.Choice_Radio;
            Title = title;
            CollectionProperty = collectionPropertyName;
            Groupname = groupName;
            Mandatory = mandatory;
            InteractiveHelp = interactiveHelp;
            AutoPostBack = autoPostback;
        }

        private string m_Groupname;
        /// <summary>
        /// 
        /// </summary>
        public string Groupname
        {
            set { m_Groupname = value; }
            get { return m_Groupname; }
        }

        private string m_CollectionProperty;
        /// <summary>
        /// 
        /// </summary>
        public string CollectionProperty
        {
            set { m_CollectionProperty = value; }
            get { return m_CollectionProperty; }
        }

        public bool ShowVertically { get; set; }

        /// <summary>
        /// This places the contents of the public property above the select.
        /// </summary>
        public string SupplierChoiceHeaderProp { get; set; }

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

            string candidate = null;
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
                    {
                        object value = Property.GetValue(this.SenderInstance, null);
                        if (value != null)
                            candidate = value.ToString();
                    }
                }
            }
            else
            {
                candidate = Console.Form(this.ID);
            }

            //if (Property.PropertyType == typeof(bool) || Property.PropertyType == typeof(bool?))
            //{
            //    if (string.IsNullOrEmpty(candidate) || candidate == "0")
            //        candidate = "false";
            //    else if (string.IsNullOrEmpty(candidate) || candidate == "1")
            //        candidate = "true";
            //    else if (string.IsNullOrEmpty(candidate) || candidate.Equals("yes", StringComparison.CurrentCultureIgnoreCase))
            //        candidate = "true";
            //    else if (string.IsNullOrEmpty(candidate) || candidate.Equals("no", StringComparison.CurrentCultureIgnoreCase))
            //        candidate = "false";
            //    else if (string.IsNullOrEmpty(candidate) || candidate.Equals("true", StringComparison.CurrentCultureIgnoreCase))
            //        candidate = "true";
            //    else if (string.IsNullOrEmpty(candidate) || candidate.Equals("false", StringComparison.CurrentCultureIgnoreCase))
            //        candidate = "false";
            //}

            if (!IsBluePrint)
            {
                if (Property.PropertyType == typeof(Data.CustomData))
                {
                    m_ListItemCollection = new Sushi.Mediakiwi.UI.ListItemCollection();
                    foreach (Sushi.Mediakiwi.Data.PropertyOption option in field.PropertyInfo.Options)
                    {
                        m_ListItemCollection.Add(new ListItem(option.Name, option.Value));
                    }
                }
                else
                {
                    m_ListItemCollection = GetCollection(this.CollectionProperty, Property.Name, SenderSponsorInstance, SenderInstance);
                }
            }

            if (!IsBluePrint && Property != null && Property.CanWrite)
            {
                if (Property.PropertyType == typeof(Data.CustomData))
                    ApplyContentContainer(field, candidate);
                else if (Property.PropertyType == typeof(Int32))
                    Property.SetValue(this.SenderInstance, Data.Utility.ConvertToInt(candidate), null);
                else if (Property.PropertyType == typeof(Int16))
                    Property.SetValue(this.SenderInstance, Int16.Parse(candidate), null);
                else if (Property.PropertyType == typeof(int?))
                {
                    if (string.IsNullOrEmpty(candidate) || candidate == "0")
                        Property.SetValue(SenderInstance, null, null);
                    else
                        Property.SetValue(SenderInstance, Data.Utility.ConvertToInt(candidate), null);
                }
                else if (Property.PropertyType == typeof(bool) || Property.PropertyType == typeof(bool?))
                {
                    if (string.IsNullOrEmpty(candidate) || candidate == "0")
                        Property.SetValue(SenderInstance, false, null);
                    else if (string.IsNullOrEmpty(candidate) || candidate == "1")
                        Property.SetValue(SenderInstance, true, null);
                    else if (string.IsNullOrEmpty(candidate) || candidate.Equals("yes", StringComparison.CurrentCultureIgnoreCase))
                        Property.SetValue(SenderInstance, true, null);
                    else if (string.IsNullOrEmpty(candidate) || candidate.Equals("no", StringComparison.CurrentCultureIgnoreCase))
                        Property.SetValue(SenderInstance, false, null);
                    else if (string.IsNullOrEmpty(candidate) || candidate.Equals("true", StringComparison.CurrentCultureIgnoreCase))
                        Property.SetValue(SenderInstance, true, null);
                    else if (string.IsNullOrEmpty(candidate) || candidate.Equals("false", StringComparison.CurrentCultureIgnoreCase))
                        Property.SetValue(SenderInstance, false, null);
                }
                else
                    Property.SetValue(this.SenderInstance, candidate, null);
            }

            OutputText = candidate;

            //  Inherited content section
            if (ShowInheritedData)
            {
                if (field != null && !string.IsNullOrEmpty(field.InheritedValue))
                {
                    foreach (var li in m_ListItemCollection)
                    {
                        if (li.Value == field.InheritedValue)
                        {
                            InhertitedOutputText = li.Text;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Writes the candidate.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <param name="isEditMode"></param>
        /// <returns></returns>
        public Field WriteCandidate(WimControlBuilder build, bool isEditMode, bool isRequired, bool isCloaked)
        {
            this.SetWriteEnvironment();
            this.IsCloaked = isCloaked;
            this.Mandatory = isRequired;
            if (OverrideEditMode) isEditMode = false;

            ListItemCollection optionsList = new ListItemCollection();

            if (isEditMode)
            {
                #region Element creation
                var cloaked = isCloaked ? " hidden" : null;

                StringBuilder element = new StringBuilder();
                string className = $" class=\"radio{cloaked}\"";
                if (AutoPostBack)
                    className = string.Format(" class=\"radio {0}{1}\""
                        , PostBackValue, cloaked
                        );
                else if (Expression != OutputExpression.FullWidth)
                    className = $" class=\"radio{cloaked}\"";

                string options = "";
                int count = 0;
                if (!String.IsNullOrEmpty(SupplierChoiceHeaderProp))
                {
                    options += GetProperty(Console.CurrentListInstance, SupplierChoiceHeaderProp);
                }
                foreach (var li in m_ListItemCollection)
                {
                    count++;
                    
                    bool selected = OutputText == li.Value;
                    if (Property.PropertyType == typeof(bool) || Property.PropertyType == typeof(bool?))
                    {
                        bool? value = null;
                        if (OutputText == "0"
                            || OutputText.Equals("no", StringComparison.CurrentCultureIgnoreCase)
                            || OutputText.Equals("false", StringComparison.CurrentCultureIgnoreCase))
                            value = false;
                        else if (OutputText == "1"
                             || OutputText.Equals("yes", StringComparison.CurrentCultureIgnoreCase)
                             || OutputText.Equals("true", StringComparison.CurrentCultureIgnoreCase))
                            value = true;

                        bool? prop = null;
                        if (li.Value == "0"
                            || li.Value.Equals("no", StringComparison.CurrentCultureIgnoreCase)
                            || li.Value.Equals("false", StringComparison.CurrentCultureIgnoreCase))
                            prop = false;
                        else if (li.Value == "1"
                             || li.Value.Equals("yes", StringComparison.CurrentCultureIgnoreCase)
                             || li.Value.Equals("true", StringComparison.CurrentCultureIgnoreCase))
                            prop = true;

                        if (value.HasValue && prop.HasValue)
                            selected = (value.Value.Equals(prop.Value));
                    }

                    optionsList.Add(new ListItem()
                    {
                        Text = li.Text,
                        Value = li.Value,
                        Enabled = (m_ListItemCollection.Count == 1 && string.IsNullOrEmpty(li.Text)),
                        Selected = selected,
                    });

                    options += string.Format("\n\t\t\t\t\t\t\t\t\t<input type=\"radio\"{5} name=\"{0}\" id=\"{0}{1}\" value=\"{2}\"{4} {8}/> <label for=\"{0}{1}\"{7}>{3}</label>{6}"
                        , this.ID
                        , count
                        , li.Value
                        , li.Text
                        , selected ? " checked=\"checked\"" : string.Empty
                        , className
                        , ShowVertically ? "<br/>" : null
                        , IsValid(isRequired) ? $" class=\"{cloaked}\"" : $" class=\"error{cloaked}\""
                        , (li.Enabled == false) ? " disabled=\"true\" " : ""
                        );

                }
                element.Append(options);
                #endregion Element creation

                if (isCloaked)
                {
                    build.AppendCloaked(element.ToString());
                }
                else
                {
                    #region Wrapper
                    string titleTag = string.Concat(Title, Mandatory ? "<em>*</em>" : "");

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
            {
                string candidate = null;
                foreach (var li in m_ListItemCollection)
                    if (li.Value == OutputText)
                    {
                        candidate = li.Text;
                        break;
                    }

                build.Append(GetSimpleTextElement(candidate));
            }

            build.ApiResponse.Fields.Add(new Api.MediakiwiField()
            {
                Event = AutoPostBack ? Api.MediakiwiJSEvent.change : Api.MediakiwiJSEvent.none,
                Title = MandatoryWrap(this.Title),
                Value = this.OutputText,
                Expression = this.Expression,
                PropertyName = this.ID,
                PropertyType = (Property == null) ? typeof(string).FullName : Property.PropertyType.FullName,
                VueType = Api.MediakiwiFormVueType.wimChoiceRadio,
                Options = optionsList,
                GroupName = Groupname,
                ReadOnly = this.IsReadOnly,
                ContentTypeID = ContentTypeSelection
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
            }
            return true;
        }

    }
}
