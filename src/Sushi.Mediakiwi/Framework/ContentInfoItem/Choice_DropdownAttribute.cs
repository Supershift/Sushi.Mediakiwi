using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Framework.ContentInfoItem
{
    /// <summary>
    /// Possible return types: System.Int32, System.String, List<int>, List<string>
    /// </summary>
    public class Choice_DropdownAttribute : ContentSharedAttribute, IContentInfo
    {
        public async Task<Api.MediakiwiField> GetApiFieldAsync()
        {
            ListItemCollection optionsList = new ListItemCollection();

            // Check if we selected a Datasource List for this dropdown
            if (m_ListItemCollection == null || m_ListItemCollection?.Count == 0)
            {
                m_ListItemCollection = GetCollection(CollectionProperty, Property.Name, SenderInstance);
            }

            if (m_ListItemCollection != null)
            {
                foreach (var li in m_ListItemCollection)
                {
                    bool selected = OutputText == li.Value;
                    if (_OutputValues != null)
                    {
                        var find = _OutputValues.Find(x => x == li.Value);
                        selected = find != null && find.Length > 0;
                    }

                    optionsList.Add(new ListItem()
                    {
                        Text = li.Text,
                        Value = li.Value,
                        Enabled = (m_ListItemCollection.Count == 1 && string.IsNullOrEmpty(li.Text)),
                        Selected = selected,
                    });

                }
            }

            if (IsTagging || IsMultiSelect.GetValueOrDefault(false))
            {
                bool hasoptions = optionsList.Count > 0;
                if (!hasoptions && !string.IsNullOrWhiteSpace(OutputText))
                {
                    var split = OutputText.Split(',');
                    _OutputValues = split.ToList();
                    foreach (var item in _OutputValues)
                    {
                        optionsList.Add(item);
                    }
                }

                return new Api.MediakiwiField()
                {
                    Event = AutoPostBack ? Api.MediakiwiJSEvent.Change : Api.MediakiwiJSEvent.None,
                    Title = MandatoryWrap(Title),
                    Value = _OutputValues != null ? _OutputValues : new List<string>(),
                    Expression = Expression,
                    PropertyName = ID,
                    PropertyType = (Property == null) ? typeof(string).FullName : Property.PropertyType.FullName,
                    VueType = hasoptions ? Api.MediakiwiFormVueType.wimTag : Api.MediakiwiFormVueType.wimTagVue,
                    Options = optionsList,
                    ReadOnly = IsReadOnly,
                    ContentTypeID = ContentTypeSelection,
                    IsAutoPostback = AutoPostBack,
                    IsMandatory = Mandatory,
                    MaxLength = MaxValueLength,
                    HelpText = InteractiveHelp,
                    FormSection = GetFormMapClass()
                };
            }
            else
            {
                return new Api.MediakiwiField()
                {
                    Event = AutoPostBack ? Api.MediakiwiJSEvent.Change : Api.MediakiwiJSEvent.None,
                    Title = MandatoryWrap(Title),
                    Value = OutputText,
                    Expression = Expression,
                    PropertyName = ID,
                    PropertyType = (Property == null) ? typeof(string).FullName : Property.PropertyType.FullName,
                    VueType = Api.MediakiwiFormVueType.wimChoiceDropdown,
                    Options = optionsList,
                    ContentTypeID = ContentTypeSelection,
                    IsAutoPostback = AutoPostBack,
                    IsMandatory = Mandatory,
                    MaxLength = MaxValueLength,
                    HelpText = InteractiveHelp,
                    FormSection = GetFormMapClass()
                };
            }
        }

        public bool IsTagging { get; set; }

        public const string OPTION_ENABLE_MULTI = "multi";

        public override MetaData GetMetaData(string name, ListItemCollection collectionPropertyValue)
        {
            MetaData meta = new MetaData();
            Utility.ReflectProperty(this, meta);
            meta.Name = name;
            meta.ContentTypeSelection = ((int)ContentTypeSelection).ToString();

            List<MetaDataList> list = new List<MetaDataList>();
            foreach (var li in collectionPropertyValue)
            {
                list.Add(new MetaDataList(li.Text, li.Value));
            }

            meta.CollectionList = list.ToArray();

            if (IsMultiSelect.GetValueOrDefault())
            {
                meta.AddOption(OPTION_ENABLE_MULTI);
            }
            else
            {
                meta.RemoveOption(OPTION_ENABLE_MULTI);
            }

            return meta;
        }
        /// <summary>
        /// Possible return types: System.Int32, System.String, List<int>, List<string>
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="collectionPropertyName">Name of the collection property.</param>
        public Choice_DropdownAttribute(string title, string collectionPropertyName)
            : this(title, collectionPropertyName, false) { }

        /// <summary>
        /// Possible return types: System.Int32, System.String, List<int>, List<string>
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="collectionPropertyName">Name of the collection property.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        public Choice_DropdownAttribute(string title, string collectionPropertyName, bool mandatory)
            : this(title, collectionPropertyName, mandatory, null) { }

        /// <summary>
        /// Possible return types: System.Int32, System.String, List<int>, List<string>
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="collectionPropertyName">Name of the collection property.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        public Choice_DropdownAttribute(string title, string collectionPropertyName, bool mandatory, string interactiveHelp)
            : this(title, collectionPropertyName, mandatory, false, interactiveHelp) { }

        /// <summary>
        /// Possible return types: System.Int32, System.String, List<int>, List<string>
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="collectionPropertyName">Name of the collection property.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        /// <param name="autoPostback">if set to <c>true</c> [auto postback].</param>
        public Choice_DropdownAttribute(string title, string collectionPropertyName, bool mandatory, bool autoPostback)
            : this(title, collectionPropertyName, mandatory, autoPostback, null) { }

        /// <summary>
        /// Possible return types: System.Int32, System.String, List<int>, List<string>
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="collectionPropertyName">Name of the collection property.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        /// <param name="autoPostback">if set to <c>true</c> [auto postback].</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        public Choice_DropdownAttribute(string title, string collectionPropertyName, bool mandatory, bool autoPostback, string interactiveHelp)
        {
            m_CanHaveExpression = true;
            ContentTypeSelection = ContentType.Choice_Dropdown;
            Title = title;
            CollectionProperty = collectionPropertyName;
            Mandatory = mandatory;
            InteractiveHelp = interactiveHelp;
            AutoPostBack = autoPostback;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the resultset is obtained via AJAX. Another importend property is [AsyncMinInput]. To avoind AJAX loading of the selected value use the [Option] property type.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is asynchronous]; otherwise, <c>false</c>.
        /// </value>
        public bool IsAsync { get; set; }

        /// <summary>
        /// Gets or sets the on change reset.
        /// </summary>
        /// <value>
        /// The on change reset.
        /// </value>
        public string[] OnChangeReset { get; set; }

        /// <summary>
        /// Gets or sets the placeholder.
        /// </summary>
        /// <value>
        /// The placeholder.
        /// </value>
        public string Placeholder { get; set; }

        /// <summary>
        /// The widht of the dropdown set in px
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Ignore setting the select2 style
        /// </summary>
        public bool IgnoreStyle { get; set; }

        /// <summary>
        /// Gets or sets the asynchronous minimum input length.
        /// </summary>
        /// <value>
        /// The asynchronous minimum input.
        /// </value>
        public int AsyncMinInput { get; set; }
      
        /// <summary>
        /// Gets or sets the collection property.
        /// </summary>
        /// <value>The collection property.</value>
        public string CollectionProperty { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [auto post back].
        /// </summary>
        /// <value><c>true</c> if [auto post back]; otherwise, <c>false</c>.</value>
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

        List<string> _OutputValues;
        Option _OutputOption;

        public bool? IsMultiSelect { get; set; }

        /// <summary>
        /// Sets the candidate.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="isEditMode">if set to <c>true</c> [is edit mode].</param>
        public void SetCandidate(Field field, bool isEditMode)
        {
            bool isAsyncCall = Console.Form(Constants.JSON_PARAM) == "1";
            bool isSync = (!IsAsync || (IsAsync && isAsyncCall));

            if (Property != null && Property.PropertyType == typeof(CustomData))
            {
                SetContentContainer(field);
            }

            if (!IsBluePrint && isSync)
            {
                if (Property.PropertyType == typeof(CustomData))
                {
                    if (m_ListItemCollection == null)
                    {
                        if (field.PropertyInfo != null && field.PropertyInfo.ListSelect.HasValue && !string.IsNullOrEmpty(field.PropertyInfo.ListCollection))
                        {
                            IComponentList list = ComponentList.SelectOne(field.PropertyInfo.ListSelect.Value);

                            if (Console.CurrentList.ID == list.ID)
                            {
                                m_ListItemCollection = Utils.GetInstanceListCollection(field.PropertyInfo.ListCollection, Console.CurrentListInstance);
                            }
                            else
                            {
                                m_ListItemCollection = Utils.GetInstanceListCollection(list, field.PropertyInfo.ListCollection, Console.Context);
                            }
                        }
                        else if (field.PropertyInfo != null && field.PropertyInfo.OptionListSelect.HasValue)
                        {
                            iOption options = Utils.GetInstanceOptions("Wim.Module.FormGenerator.dll", "Wim.Module.FormGenerator.Data.FormElementOptionList", Context.RequestServices);
                            if (options != null)
                            {
                                m_ListItemCollection = new ListItemCollection();
                                foreach (iNameValue nv in options.Options(field.PropertyInfo.OptionListSelect.Value))
                                {
                                    m_ListItemCollection.Add(new ListItem(nv.Name, nv.Value));
                                }
                            }
                        }
                        else if (!string.IsNullOrEmpty(CollectionProperty))
                        {
                            m_ListItemCollection = GetCollection(CollectionProperty, Property.Name, SenderSponsorInstance, SenderInstance);
                        }
                        else
                        {
                            m_ListItemCollection = new ListItemCollection();
                            foreach (PropertyOption option in field.PropertyInfo.Options)
                            {
                                m_ListItemCollection.Add(new ListItem(option.Name, option.Value));
                            }
                        }

                        if (field.PropertyInfo != null && field.PropertyInfo.IsEmptyFirst)
                        {
                            m_ListItemCollection.Insert(0, new ListItem(""));
                        }
                    }
                }
                else
                {
                    m_ListItemCollection = GetCollection(CollectionProperty, Property.Name, SenderSponsorInstance, SenderInstance);
                }
            }

            // CB; the first 2 are added for issues with other projects
            if (Property != null && Property.PropertyType != null && !IsMultiSelect.HasValue)
            {
                IsMultiSelect = (Property.PropertyType == typeof(List<int>))
                    || (Property.PropertyType == typeof(int[]))
                    || (Property.PropertyType == typeof(List<string>))
                    || (Property.PropertyType == typeof(string[]))
                    || (Property.PropertyType == typeof(Option[]))
                    || (Property.PropertyType == typeof(List<Option>));
            }

            string candidate = null;

            bool initialLoad = IsInitialLoad;
            //  [08.jul 2015:MM] Added because of empty post values (which where not possible)
            if (IsMultiSelect.GetValueOrDefault())
            {
                initialLoad = !(Console.IsPosted(ID));
            }

            if (initialLoad || !isEditMode)
            {
                if (IsBluePrint)
                {
                    if (field != null)
                    {
                        candidate = field.Value;

                        if (IsMultiSelect.GetValueOrDefault())
                        {
                            if (candidate == null)
                            {
                                _OutputValues = new List<string>();
                            }
                            else
                            {
                                var split = candidate.Split(',');
                                _OutputValues = new List<string>();
                                foreach (var item in split)
                                {
                                    _OutputValues.Add(item);
                                }
                            }
                        }
                    }
                }
                else
                {

                    if (Property.PropertyType == typeof(CustomData))
                    {
                        candidate = m_ContentContainer[field.Property].Value;
                    }
                    else if (Property.PropertyType == typeof(List<int>))
                    {
                        object value = Property.GetValue(SenderInstance, null);
                        if (value != null)
                        {
                            _OutputValues = new List<string>();
                            List<int> arr = value as List<int>;
                            arr.ForEach(x =>
                            {
                                _OutputValues.Add(x.ToString());
                            });
                        }
                    }
                    else if (Property.PropertyType == typeof(List<string>))
                    {
                        object value = Property.GetValue(SenderInstance, null);
                        if (value != null)
                        {
                            _OutputValues = value as List<string>;
                            if (_OutputValues == null)
                            {
                                _OutputValues = new List<string>();
                            }
                        }
                    }
                    else if (Property.PropertyType == typeof(string[]))
                    {
                        object value = Property.GetValue(SenderInstance, null);
                        if (value != null)
                        {
                            var arr = value as string[];
                            if (arr != null)
                            {
                                if (_OutputValues == null)
                                {
                                    _OutputValues = new List<string>();
                                }
                                foreach (var item in arr)
                                {
                                    _OutputValues.Add(item);
                                }
                            }
                        }
                    }
                    else if (Property.PropertyType == typeof(int[]))
                    {
                        object value = Property.GetValue(SenderInstance, null);
                        if (value != null)
                        {
                            var arr = value as int[];
                            if (arr != null)
                            {
                                if (_OutputValues == null)
                                {
                                    _OutputValues = new List<string>();
                                }
                                foreach (var item in arr)
                                {
                                    _OutputValues.Add(item.ToString());
                                }
                            }
                        }
                    }
                    else if (Property.PropertyType == typeof(Option))
                    {
                        object value = Property.GetValue(SenderInstance, null);
                        if (value != null)
                        {
                            _OutputOption = value as Option;
                            if (_OutputOption == null)
                            {
                                _OutputOption = new Option();
                            }
                            else
                            {
                                candidate = _OutputOption.Value;
                            }
                        }
                    }
                    else if (Property.PropertyType == typeof(ContentType))
                    {
                        object value = Property.GetValue(SenderInstance, null);
                        if (value != null)
                        {
                            candidate = ((int)value).ToString();
                        }
                    }
                    else
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

                if (IsMultiSelect.GetValueOrDefault())
                {
                    if (string.IsNullOrEmpty(candidate))
                    {
                        _OutputValues = new List<string>();
                    }
                    else
                    {
                        var split = Console.ConvertArray(candidate);

                        _OutputValues = new List<string>();
                        foreach (var item in split)
                        {
                            _OutputValues.Add(item);
                        }
                    }
                }
                else if (Property != null && Property.PropertyType == typeof(Option))
                {
                    _OutputOption = new Option() { Value = candidate };
                }
            }

            if (_OutputValues != null)
            {
                _OutputValues = _OutputValues.Distinct().ToList();
            }

            bool foundItem = false;
            if (m_ListItemCollection != null)
            {
                foreach (var li in m_ListItemCollection)
                {
                    if (li.Value == candidate)
                    {
                        foundItem = true;
                        break;
                    }
                }
            }

            if (isSync && !IsMultiSelect.GetValueOrDefault() && !IsTagging)
            {
                if (!foundItem)
                {
                    if (m_ListItemCollection != null && m_ListItemCollection.Count > 0)
                    {
                        candidate = m_ListItemCollection[0].Value;
                    }
                    else
                    {
                        candidate = null;
                    }
                }

                if (IsInitialLoad && m_ListItemCollection != null && m_ListItemCollection.Count > 0 && (candidate == null || (candidate == "0")))
                {
                    candidate = m_ListItemCollection[0].Value;
                }
            }

            OutputText = candidate == null ? null : WebUtility.HtmlEncode(candidate);

            if (!IsBluePrint && Property != null && Property.CanWrite)
            {
                if (Property.PropertyType == typeof(CustomData))
                {
                    ApplyContentContainer(field, candidate);
                }
                else if (Property.PropertyType == typeof(List<int>) || Property.PropertyType == typeof(int[]))
                {
                    List<int> arr = new List<int>();
                    if (_OutputValues != null)
                    {
                        _OutputValues.ForEach(x =>
                        {
                            arr.Add(Utility.ConvertToInt(x));
                        });
                    }

                    if (Property.PropertyType == typeof(List<int>))
                    {
                        Property.SetValue(SenderInstance, arr, null);
                    }
                    else
                    {
                        Property.SetValue(SenderInstance, arr.ToArray(), null);
                    }
                }
                else if (Property.PropertyType == typeof(List<string>))
                {
                    Property.SetValue(SenderInstance, _OutputValues, null);
                }
                else if (Property.PropertyType == typeof(string[]))
                {
                    if (_OutputValues == null)
                    {
                        Property.SetValue(SenderInstance, null, null);
                    }
                    else
                    {
                        Property.SetValue(SenderInstance, _OutputValues.ToArray(), null);
                    }
                }
                else if (Property.PropertyType == typeof(int))
                {
                    Property.SetValue(SenderInstance, Utility.ConvertToInt(candidate), null);
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
                else if (Property.PropertyType == typeof(decimal))
                {
                    Property.SetValue(SenderInstance, Utility.ConvertToDecimal(candidate), null);
                }
                else if (Property.PropertyType == typeof(decimal?))
                {
                    if (string.IsNullOrEmpty(candidate) || candidate == "0")
                    {
                        Property.SetValue(SenderInstance, null, null);
                    }
                    else
                    {
                        Property.SetValue(SenderInstance, Utility.ConvertToDecimal(candidate), null);
                    }
                }
                else if (Property.PropertyType == typeof(Option))
                {
                    if (string.IsNullOrEmpty(candidate) || candidate == "0")
                    {
                        Property.SetValue(SenderInstance, null, null);
                    }
                    else
                    {
                        Property.SetValue(SenderInstance, _OutputOption, null);
                    }
                }
                else if (Property.PropertyType == typeof(ContentType))
                {
                    if (string.IsNullOrEmpty(candidate) || candidate == "0")
                    {
                        Property.SetValue(SenderInstance, null, null);
                    }
                    else
                    {
                        Property.SetValue(SenderInstance, (ContentType)Enum.Parse(typeof(ContentType), candidate), null);
                    }
                }
                else
                {
                    Property.SetValue(SenderInstance, candidate, null);
                }
            }

            //  Inherited content section
            if (ShowInheritedData && field != null && !string.IsNullOrEmpty(field.InheritedValue) && m_ListItemCollection != null)
            {
                foreach (var li in m_ListItemCollection)
                {
                    if (li.Value == field.InheritedValue)
                    {
                        InhertitedOutputText = WebUtility.HtmlEncode(li.Text);
                        break;
                    }
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
            SetWriteEnvironment();
            IsCloaked = isCloaked;
            Mandatory = isRequired;
            string formName = GetFormMapClass();

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

            string width = null;
            if (Width > 0)
            {
                width = string.Format(" style=\"width: {0}px\"", Width);
            }

            if (isEditMode || Console.CurrentListInstance.wim.IsEditMode)
            {
                if (!isEditMode)
                    isEnabled = false;

                #region Element creation

                StringBuilder element = new StringBuilder();

                string className = string.Empty;

                if (AutoPostBack)
                {
                    className = string.Format(" class=\"{2} {0}{1}\"", IsValid(isRequired) ? string.Empty : " error"
                        , Expression == OutputExpression.FullWidth ? null : " short"
                        , PostBackValue
                        );
                }
                else if (!IsValid(isRequired))
                {
                    className = string.Format(" class=\"{0}error\""
                        , Expression == OutputExpression.FullWidth ? null : "short "
                        );
                }
                else if (Expression != OutputExpression.FullWidth)
                {
                    className = " class=\"short\"";
                }

                string options = "";

                int count = 0;

                // Check if we selected a Datasource List for this dropdown
                if (string.IsNullOrWhiteSpace(CollectionProperty) == false && Utils.IsGuid(CollectionProperty))
                {
                    IComponentList list = ComponentList.SelectOne(Utility.ConvertToGuid(CollectionProperty));
                    if (list?.ID > 0)
                    {
                        try
                        {
                            m_ListItemCollection = Utils.GetListCollection(Console, list);
                        }
                        catch (Exception ex)
                        {
                            Notification.InsertOne("Sushi.Mediakiwi.Choice_DropdownAttribute", $"Does the list assigned to the dropdown field '{FieldName}' exist?.<br/>{ex.Message}");
                        }
                    }
                }

                if (m_ListItemCollection != null)
                {
                    foreach (var li in m_ListItemCollection)
                    {
                        count++;

                        bool selected = OutputText == li.Value;
                        if (_OutputValues != null)
                        {
                            var find = _OutputValues.Find(x => x == li.Value);
                            selected = find != null && find.Length > 0;
                        }

                        if (m_ListItemCollection.Count == 1 && string.IsNullOrEmpty(li.Text))
                        {
                            options += string.Format("<option value=\"{0}\"{2}{3}>{1}</option>", li.Value, "&nbsp;", selected ? " selected=\"selected\"" : string.Empty, li.Enabled ? string.Empty : " disabled=\"disabled\"");
                        }
                        else
                        {
                            options += string.Format("<option value=\"{0}\"{2}{3}>{1}</option>", li.Value, WebUtility.HtmlEncode(li.Text), selected ? " selected=\"selected\"" : string.Empty, li.Enabled ? string.Empty : " disabled=\"disabled\"");
                        }
                    }
                }

                if (string.IsNullOrEmpty(options))
                {
                    options = "<option value=\"\">&nbsp;</option>";
                }

                string titleTag = string.Concat(Title, Mandatory ? "<em>*</em>" : "");

                if (IsAsync)
                {
                    string opt = string.Empty;
                    if (_OutputOption != null)
                    {
                        opt = $"<option value=\"{_OutputOption.Value}\" selected >{_OutputOption.Text}</option>";
                    }
                    element.AppendFormat(@"
                                <select  id=""{0}""{1} name=""{0}"" {2}{3} data-ph=""{5}"" data-len=""{6}""{7}{8}>
                                    <option value=""0""></option>
                                    {10}
                                </select>"
                        , ID  //0
                        , InputClassName(IsValid(isRequired), "aselect") //1
                        , isEnabled ? null : " disabled=\"disabled\"" // 2
                        , IsMultiSelect.GetValueOrDefault() ? " multiple=\"multiple\"" : null // 3
                        , _OutputOption != null ? _OutputOption.Value : OutputText // 4
                        , Placeholder //5 
                        , AsyncMinInput.ToString()
                        , _OutputOption == null || string.IsNullOrEmpty(_OutputOption.Text) ? null : string.Format(" data-text=\"{0}\"", _OutputOption.Text)
                        , IsTagging ? " data-tags=\"true\" data-max=\"1\"" : null
                        , _OutputOption == null || string.IsNullOrEmpty(_OutputOption.Text) ? null : _OutputOption.Text //9
                        , opt
                            );
                }
                else
                {
                    string reset = null;
                    if (OnChangeReset != null && OnChangeReset.Length > 0)
                    {
                        reset = $" data-reset=\"{Utility.ConvertToCsvString(OnChangeReset, false)}\"";
                    }
                    element.AppendFormat("<select id=\"{0}\"{1} name=\"{0}\"{2}{4}{5}{6}{7}>{3}"
                        , ID
                        , InputClassName(IsValid(isRequired), IgnoreStyle ? null : "styled")
                        , isEnabled ? null : " disabled=\"disabled\""
                        , options
                        , (IsTagging || IsMultiSelect.GetValueOrDefault()) ? " multiple=\"multiple\"" : null
                        , reset
                        , width
                        , IsTagging ? (IsMultiSelect.GetValueOrDefault() ? " data-tags=\"true\"" : " data-tags=\"true\" data-max=\"1\"") : null
                        );
                    element.Append("</select>");
                }

                #endregion Element creation

                if (IsCloaked)
                {
                    build.AppendCloaked(element.ToString());
                }
                else
                {
                    #region Wrapper creation

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

                    build.AppendFormat("<td{0}{1}>{2}"
                        , (Expression == OutputExpression.FullWidth && Console.HasDoubleCols) ? " colspan=\"3\"" : null
                        , InputCellClassName(IsValid(isRequired))
                        , CustomErrorText
                        );

                    build.Append($"<div class=\"{((Expression == OutputExpression.FullWidth) ? Class_Wide : "half")}\">");

                    // Add Shared Icon (if any)
                    if (string.IsNullOrWhiteSpace(SharedIcon) == false && IsCloaked == false)
                    {
                        build.Append(SharedIcon);
                    }

                    build.Append(element.ToString());

                    build.Append("</div></td>");

                    if (Expression == OutputExpression.FullWidth || Expression == OutputExpression.Right)
                    {
                        build.Append("</tr>");
                    }

                    #endregion Wrapper creation
                }
            }
            else
            {
                string candidate = null;
                if (m_ListItemCollection != null)
                {
                    foreach (var li in m_ListItemCollection)
                        if (li.Value == OutputText)
                        {
                            candidate = li.Text;
                            break;
                        }
                }
                build.Append(GetSimpleTextElement(candidate));
            }

            // Get API field and add it to response
            var apiField = Task.Run(async () => await GetApiFieldAsync()).Result;
            build.ApiResponse.Fields.Add(apiField);

            return ReadCandidate(OutputText);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool IsValid(bool isRequired)
        {
            Mandatory = isRequired;
            if (Console?.CurrentListInstance?.wim?.IsSaveMode == true)
            {
                //  Custom error validation
                if (!base.IsValid(isRequired))
                {
                    return false;
                }

                if (Mandatory && string.IsNullOrEmpty(OutputText))
                {
                    if (IsAsync)
                    {
                        return false;
                    }
                    else if (_OutputValues == null || _OutputValues.Count == 0)
                    {
                        var hasValue = HasSharedValue();
                        if (hasValue.isSharedField)
                        {
                            return hasValue.hasValue;
                        }

                        return false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}