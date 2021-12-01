using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.UI;
using System;
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

        /// <summary>
        /// 
        /// </summary>
        public string CollectionProperty { get; set; }

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
            if (Property != null && Property.PropertyType == typeof(CustomData))
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
                    if (Property.PropertyType == typeof(CustomData))
                    {
                        candidate = m_ContentContainer[field.Property].Value;
                    }
                    else
                    {
                        object value = Property.GetValue(SenderInstance, null);
                        if (value != null)
                            candidate = value.ToString();
                    }
                }
            }
            else
            {
                candidate = Console.Form(ID);
            }

            if (!IsBluePrint)
            {
                if (Property.PropertyType == typeof(CustomData))
                {
                    m_ListItemCollection = new ListItemCollection();
                    foreach (PropertyOption option in field.PropertyInfo.Options)
                    {
                        m_ListItemCollection.Add(new ListItem(option.Name, option.Value));
                    }
                }
                else
                {
                    m_ListItemCollection = GetCollection(CollectionProperty, Property.Name, SenderSponsorInstance, SenderInstance);
                }
            }

            if (!IsBluePrint && Property != null && Property.CanWrite)
            {
                if (Property.PropertyType == typeof(CustomData))
                {
                    ApplyContentContainer(field, candidate);
                }
                else if (Property.PropertyType == typeof(int))
                {
                    Property.SetValue(SenderInstance, Utility.ConvertToInt(candidate), null);
                }
                else if (Property.PropertyType == typeof(short))
                {
                    Property.SetValue(SenderInstance, short.Parse(candidate), null);
                }
                else if (Property.PropertyType == typeof(int?))
                {
                    if (string.IsNullOrEmpty(candidate) || candidate == "0")
                    {
                        Property.SetValue(SenderInstance, null, null);
                    }
                    else
                    {
                        Property.SetValue(SenderInstance, Utility.ConvertToInt(candidate), null);
                    }
                }
                else if (Property.PropertyType == typeof(bool) || Property.PropertyType == typeof(bool?))
                {
                    if (string.IsNullOrEmpty(candidate) || candidate == "0")
                    {
                        Property.SetValue(SenderInstance, false, null);
                    }
                    else if (candidate == "1")
                    {
                        Property.SetValue(SenderInstance, true, null);
                    }
                    else if (candidate.Equals("yes", StringComparison.CurrentCultureIgnoreCase))
                    {
                        Property.SetValue(SenderInstance, true, null);
                    }
                    else if (candidate.Equals("no", StringComparison.CurrentCultureIgnoreCase))
                    {
                        Property.SetValue(SenderInstance, false, null);
                    }
                    else if (candidate.Equals("true", StringComparison.CurrentCultureIgnoreCase))
                    {
                        Property.SetValue(SenderInstance, true, null);
                    }
                    else if (candidate.Equals("false", StringComparison.CurrentCultureIgnoreCase))
                    {
                        Property.SetValue(SenderInstance, false, null);
                    }
                }
                else
                {
                    Property.SetValue(SenderInstance, candidate, null);
                }
            }

            OutputText = candidate;

            //  Inherited content section
            if (ShowInheritedData && field != null && !string.IsNullOrEmpty(field.InheritedValue))
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

        /// <summary>
        /// Writes the candidate.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <param name="isEditMode"></param>
        /// <returns></returns>
        public Field WriteCandidate(WimControlBuilder build, bool isEditMode, bool isRequired, bool isCloaked)
        {
            SetWriteEnvironment();
            string formName = GetFormMapClass();

            IsCloaked = isCloaked;
            Mandatory = isRequired;
            bool _isEditMode = isEditMode;

            if (OverrideEditMode)
            {
                _isEditMode = false;
            }

            ListItemCollection optionsList = new ListItemCollection();

            if (_isEditMode)
            {
                #region Element creation

                var cloaked = isCloaked ? " hidden" : null;

                string className = $" class=\"radio{cloaked}\"";
                if (AutoPostBack)
                {
                    className = $" class=\"radio {PostBackValue}{cloaked}\"";
                }

                StringBuilder element = new StringBuilder();
                StringBuilder options = new StringBuilder();
                int count = 0;
                if (!string.IsNullOrEmpty(SupplierChoiceHeaderProp))
                {
                    options.Append(GetProperty(Console.CurrentListInstance, SupplierChoiceHeaderProp));
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
                        {
                            value = false;
                        }
                        else if (OutputText == "1"
                             || OutputText.Equals("yes", StringComparison.CurrentCultureIgnoreCase)
                             || OutputText.Equals("true", StringComparison.CurrentCultureIgnoreCase))
                        {
                            value = true;
                        }

                        bool? prop = null;
                        if (li.Value == "0"
                            || li.Value.Equals("no", StringComparison.CurrentCultureIgnoreCase)
                            || li.Value.Equals("false", StringComparison.CurrentCultureIgnoreCase))
                        {
                            prop = false;
                        }
                        else if (li.Value == "1"
                             || li.Value.Equals("yes", StringComparison.CurrentCultureIgnoreCase)
                             || li.Value.Equals("true", StringComparison.CurrentCultureIgnoreCase))
                        {
                            prop = true;
                        }

                        if (value.HasValue && prop.HasValue)
                        {
                            selected = (value.Value.Equals(prop.Value));
                        }
                    }

                    optionsList.Add(new ListItem()
                    {
                        Text = li.Text,
                        Value = li.Value,
                        Enabled = (m_ListItemCollection.Count == 1 && string.IsNullOrEmpty(li.Text)),
                        Selected = selected,
                    });

                    var optionChecked = selected ? " checked=\"checked\"" : string.Empty;
                    var optionEnabled = (li.Enabled == false) ? " disabled=\"true\" " : "";
                    var optionValid = IsValid(isRequired) ? $" class=\"{cloaked}\"" : $" class=\"error{cloaked}\"";
                    var optionVertical = (ShowVertically ? "<br/>" : null);

                    options.Append($"<input type=\"radio\"{optionChecked}{className} name=\"{ID}\" id=\"{ID}{count}\" value=\"{li.Value}\"{optionChecked} {optionEnabled}/> <label for=\"{ID}{count}\"{optionValid}>{li.Text}</label>{optionVertical}");
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
                        build.Append("</tr>\n");
                    }

                    #endregion Wrapper
                }
            }
            else
            {
                string candidate = null;
                foreach (var li in m_ListItemCollection)
                {
                    if (li.Value == OutputText)
                    {
                        candidate = li.Text;
                        break;
                    }
                }

                build.Append(GetSimpleTextElement(candidate));
            }

            build.ApiResponse.Fields.Add(new Api.MediakiwiField()
            {
                Event = AutoPostBack ? Api.MediakiwiJSEvent.change : Api.MediakiwiJSEvent.none,
                Title = MandatoryWrap(Title),
                Value = OutputText,
                Expression = Expression,
                PropertyName = ID,
                PropertyType = (Property == null) ? typeof(string).FullName : Property.PropertyType.FullName,
                VueType = Api.MediakiwiFormVueType.wimChoiceRadio,
                Options = optionsList,
                GroupName = Groupname,
                ReadOnly = IsReadOnly,
                ContentTypeID = ContentTypeSelection,
                IsAutoPostback = AutoPostBack,
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
                    return false;
                }
            }
            return true;
        }

    }
}
