using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Sushi.Mediakiwi.UI;
using Sushi.Mediakiwi.Data;
using System.Net;

namespace Sushi.Mediakiwi.Framework.ContentInfoItem
{
    /// <summary>
    /// Possible return types: System.Int32, System.String, List<int>, List<string>
    /// </summary>
    public class Choice_DropdownAttribute : ContentSharedAttribute, IContentInfo
    {
        public bool IsTagging { get; set; }

        public const string OPTION_ENABLE_MULTI = "multi";

        public override MetaData GetMetaData(string name, ListItemCollection collectionPropertyValue)
        {
            MetaData meta = new MetaData();
            Data.Utility.ReflectProperty(this, meta);
            meta.Name = name;
            meta.ContentTypeSelection = ((int)ContentTypeSelection).ToString();

            List<MetaDataList> list = new List<MetaDataList>();
            foreach (var li in collectionPropertyValue)
                list.Add(new MetaDataList(li.Text, li.Value));

            meta.CollectionList = list.ToArray();

            if (this.IsMultiSelect.GetValueOrDefault())
                meta.AddOption(OPTION_ENABLE_MULTI);
            else
                meta.RemoveOption(OPTION_ENABLE_MULTI);

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
        /// Gets or sets a value indicating whether the resultset is obtained via AJAX. Another importend property is [AsyncMinInput]. To avoind AJAX loading of the selected value use the [Sushi.Mediakiwi.Data.Option] property type.
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
        /// Gets or sets the componentlist; this can be an GUID, INT or a ClassName.
        /// </summary>
        /// <value>
        /// The componentlist.
        /// </value>
        //public string Componentlist { get; set; }

        private string m_CollectionProperty;
        /// <summary>
        /// Gets or sets the collection property.
        /// </summary>
        /// <value>The collection property.</value>
        public string CollectionProperty
        {
            set { m_CollectionProperty = value; }
            get { return m_CollectionProperty; }
        }

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



        //void OutputAsync()
        //{
        //    if (!IsAsync || Console.HasAsyncEvent)
        //        return;

        //    var async = Data.Utility.GetAsyncQuery();
        //    if (async == null)
        //        return;

        //    if (this.OnChangeReset != null && this.OnChangeReset.Length > 0)
        //        async.ApplyReset(false, this.OnChangeReset);
            
        //    Regex rex = null;

        //    var search = async.SearchQuery;
        //    if (!string.IsNullOrEmpty(search))
        //    {
        //        search = search.Replace(" ", ".*");

        //        rex = new Regex(search, RegexOptions.IgnoreCase);
        //    }
            
        //    if (async.Property == Property.Name)
        //    {
        //        List<Option> options = new List<Option>();
        //        for (int i = 0; i < m_ListItemCollection.Count; i++)
        //        {
        //            var option = m_ListItemCollection[i];

        //            if (async.SearchType == ASyncQueryType.SelectOneByID)
        //            {
        //                if (option.Value == async.SearchQuery)
        //                {
        //                    options.Add(new Option() { Text = option.Text, Value = option.Value
        //                        , Disabled = option.Enabled ? (bool?)null : true
        //                    });
        //                    break;
        //                }
        //            }
        //            else
        //            {
        //                if (rex == null || rex.IsMatch(option.Text))
        //                    options.Add(new Option() { Text = option.Text, Value = option.Value
        //                        , Disabled = option.Enabled ? (bool?)null : true
                            
        //                    });
        //            }
        //        }
        //        ASyncResult result = new ASyncResult()
        //        {
        //            Property = async.Property,
        //            Result = options,
        //            Reset = async.Reset
        //        };
        //        string val = Wim.Utilities.JSON.Instance.ToJSON(result,
        //            new Wim.Utilities.JSONParameters()
        //            {
        //                EnableAnonymousTypes = true,
        //                UsingGlobalTypes = false,
        //                SerializeNullValues = false
        //            }
        //        );

        //        this.Console.Response.Write(val);
        //        this.Console.Response.End();
        //    }
        //}

        //internal bool? _isMultiSelect;
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

            if (Property != null && Property.PropertyType == typeof(Data.CustomData))
                SetContentContainer(field);

            if (!IsBluePrint)
            {
                if (isSync)
                {
                    if (Property.PropertyType == typeof(Data.CustomData))
                    {
                        if (m_ListItemCollection == null)
                        {
                            if (field.PropertyInfo != null && field.PropertyInfo.ListSelect.HasValue && !string.IsNullOrEmpty(field.PropertyInfo.ListCollection))
                            {
                                Sushi.Mediakiwi.Data.IComponentList list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(field.PropertyInfo.ListSelect.Value);

                                if (Console.CurrentList.ID == list.ID)
                                    m_ListItemCollection = Utils.GetInstanceListCollection(list, field.PropertyInfo.ListCollection, this.Console.CurrentListInstance.wim.CurrentSite, this.Console.CurrentListInstance);
                                else
                                    m_ListItemCollection = Utils.GetInstanceListCollection(list, field.PropertyInfo.ListCollection, this.Console.CurrentListInstance.wim.CurrentSite, this.Console.Context);
                            }
                            else if (field.PropertyInfo != null && field.PropertyInfo.OptionListSelect.HasValue)
                            {
                                Sushi.Mediakiwi.Framework.iOption options = Utils.GetInstanceOptions("Wim.Module.FormGenerator.dll", "Wim.Module.FormGenerator.Data.FormElementOptionList");
                                if (options != null)
                                {
                                    m_ListItemCollection = new Sushi.Mediakiwi.UI.ListItemCollection();
                                    foreach (Sushi.Mediakiwi.Framework.iNameValue nv in options.Options(field.PropertyInfo.OptionListSelect.Value))
                                    {
                                        m_ListItemCollection.Add(new ListItem(nv.Name, nv.Value));
                                    }
                                }
                            }
                            else if (!string.IsNullOrEmpty(this.CollectionProperty))
                            {
                                m_ListItemCollection = GetCollection(this.CollectionProperty, Property.Name, SenderSponsorInstance, SenderInstance);
                            }
                            else
                            {
                                m_ListItemCollection = new Sushi.Mediakiwi.UI.ListItemCollection();
                                foreach (Sushi.Mediakiwi.Data.PropertyOption option in field.PropertyInfo.Options)
                                {
                                    m_ListItemCollection.Add(new ListItem(option.Name, option.Value));
                                }
                            }

                            if (field.PropertyInfo != null && field.PropertyInfo.IsEmptyFirst)
                                m_ListItemCollection.Insert(0, new ListItem(""));
                        }
                    }
                    else
                    {
                        m_ListItemCollection = GetCollection(this.CollectionProperty, Property.Name, SenderSponsorInstance, SenderInstance);
                    }
                }

            }
            // CB; the first 2 are added for issues with other projects
            if (Property != null && Property.PropertyType != null && !IsMultiSelect.HasValue)
            {
                this.IsMultiSelect = (Property.PropertyType == typeof(List<int>))
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
                initialLoad = !(Console.IsPosted(ID));

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
                                _OutputValues = new List<string>();
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

                    if (Property.PropertyType == typeof(Data.CustomData))
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
                            arr.ForEach(x => {
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
                                _OutputValues = new List<string>();
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
                                    _OutputValues = new List<string>();
                                foreach (var item in arr)
                                    _OutputValues.Add(item);
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
                                    _OutputValues = new List<string>();
                                foreach (var item in arr)
                                    _OutputValues.Add(item.ToString());
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
                                _OutputOption = new Option();
                            else
                                candidate = _OutputOption.Value;
                        }
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
                candidate = Console.Form(this.ID);

                if (IsMultiSelect.GetValueOrDefault())
                {
                    if (string.IsNullOrEmpty(candidate))
                        _OutputValues = new List<string>();
                    else
                    {
                        var split = Console.ConvertArray(candidate);

                        _OutputValues = new List<string>();
                        foreach (var item in split)
                            _OutputValues.Add(item);
                    }
                }
                else if (Property != null && Property.PropertyType == typeof(Option))
                {
                    _OutputOption = new Option() { Value = candidate };
                }
            }

            if (_OutputValues != null)
                _OutputValues = _OutputValues.Distinct().ToList();

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

            if (isSync && !IsMultiSelect.GetValueOrDefault() & !this.IsTagging)
            {
                if (!foundItem)
                {
                    if (m_ListItemCollection != null && m_ListItemCollection.Count > 0)
                        candidate = m_ListItemCollection[0].Value;
                    else
                        candidate = null;
                }

                if (this.IsInitialLoad && m_ListItemCollection != null && m_ListItemCollection.Count > 0 && (candidate == null || (candidate == "0")))
                {
                    candidate = m_ListItemCollection[0].Value;
                }
            }

            OutputText = candidate == null ? null : WebUtility.HtmlEncode(candidate);

            if (!IsBluePrint && Property != null && Property.CanWrite)
            {
                if (Property.PropertyType == typeof(Data.CustomData))
                    ApplyContentContainer(field, candidate);
                else if (Property.PropertyType == typeof(List<int>) || Property.PropertyType == typeof(int[]))
                {
                    List<int> arr = new List<int>();
                    if (_OutputValues != null)
                    {
                        _OutputValues.ForEach(x =>
                        {
                            arr.Add(Data.Utility.ConvertToInt(x));
                        });
                    }

                    if (Property.PropertyType == typeof(List<int>))
                        Property.SetValue(SenderInstance, arr, null);
                    else
                        Property.SetValue(SenderInstance, arr.ToArray(), null);

                }
                else if (Property.PropertyType == typeof(List<string>))
                {
                    Property.SetValue(SenderInstance, _OutputValues, null);
                }
                else if (Property.PropertyType == typeof(string[]))
                {
                    if (_OutputValues == null)
                        Property.SetValue(SenderInstance, null, null);
                    else
                        Property.SetValue(SenderInstance, _OutputValues.ToArray(), null);

                }
                else if (Property.PropertyType == typeof(int))
                    Property.SetValue(SenderInstance, Data.Utility.ConvertToInt(candidate), null);

                else if (Property.PropertyType == typeof(int?))
                {
                    if (string.IsNullOrEmpty(candidate) || candidate == "0")
                        Property.SetValue(SenderInstance, null, null);
                    else
                        Property.SetValue(SenderInstance, Data.Utility.ConvertToInt(candidate), null);
                }
                else if (Property.PropertyType == typeof(decimal))
                {
                    Property.SetValue(SenderInstance, Data.Utility.ConvertToDecimal(candidate), null);
                }
                else if (Property.PropertyType == typeof(decimal?))
                {
                    if (string.IsNullOrEmpty(candidate) || candidate == "0")
                        Property.SetValue(SenderInstance, null, null);
                    else
                        Property.SetValue(SenderInstance, Data.Utility.ConvertToDecimal(candidate), null);
                }
                else if (Property.PropertyType == typeof(Option))
                {
                    if (string.IsNullOrEmpty(candidate) || candidate == "0")
                        Property.SetValue(SenderInstance, null, null);
                    else
                        Property.SetValue(SenderInstance, _OutputOption, null);
                }
                else
                    Property.SetValue(SenderInstance, candidate, null);
            }

            //  Inherited content section
            if (ShowInheritedData)
            {
                if (field != null && !string.IsNullOrEmpty(field.InheritedValue))
                {
                    if (m_ListItemCollection != null)
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
            //this.OutputAsync();

            if (OverrideEditMode) isEditMode = false;

            string width = null;
            if (Width > 0)
                width = string.Format(" style=\"width: {0}px\"", this.Width);

            ListItemCollection optionsList = new ListItemCollection();

            if (isEditMode || this.Console.CurrentListInstance.wim.IsEditMode)
            {
                bool isEnabled = true;
                if (!isEditMode)
                    isEnabled = false;

                #region Element creation
                StringBuilder element = new StringBuilder();

                string className = string.Empty;

                if (AutoPostBack)
                    className = string.Format(" class=\"{2} {0}{1}\"", IsValid(isRequired) ? string.Empty : " error"
                        , Expression == OutputExpression.FullWidth ? null : " short"
                        , PostBackValue
                        );
                else if (!IsValid(isRequired))
                    className = string.Format(" class=\"{0}error\""
                        , Expression == OutputExpression.FullWidth ? null : "short "
                        );
                else if (Expression != OutputExpression.FullWidth)
                    className = " class=\"short\"";

                string options = "";
          
                int count = 0;
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

                        optionsList.Add(new ListItem()
                        {
                            Text = li.Text,
                            Value = li.Value,
                            Enabled = (m_ListItemCollection.Count == 1 && string.IsNullOrEmpty(li.Text)),
                            Selected = selected,                            
                        });

                        if (m_ListItemCollection.Count == 1 && string.IsNullOrEmpty(li.Text))
                        {
                            options += string.Format("\n\t\t\t\t\t\t\t\t\t\t<option value=\"{0}\"{2}{3}>{1}</option>", li.Value, "&nbsp;", selected ? " selected=\"selected\"" : string.Empty, li.Enabled ? string.Empty : " disabled=\"disabled\"");
                        }
                        else
                            options += string.Format("\n\t\t\t\t\t\t\t\t\t\t<option value=\"{0}\"{2}{3}>{1}</option>", li.Value, WebUtility.HtmlEncode(li.Text), selected ? " selected=\"selected\"" : string.Empty, li.Enabled ? string.Empty : " disabled=\"disabled\"");
                    }
                }

                if (string.IsNullOrEmpty(options))
                    options = "\n\t\t\t\t\t\t\t\t\t\t<option value=\"\">&nbsp;</option>";

                string titleTag = string.Concat(Title, Mandatory ? "<em>*</em>" : "");

                if (IsAsync)
                {
                    string opt = string.Empty;
                    if (this._OutputOption != null)
                        opt = string.Format(@"<option value=""{0}"" selected >{1}</option>", this._OutputOption.Value, this._OutputOption.Text);
                    element.AppendFormat(@"
                                <select  id=""{0}""{1} name=""{0}"" {2}{3} data-ph=""{5}"" data-len=""{6}""{7}{8}>
                                    <option value=""0""></option>
                                    {10}
                                </select>"
                        , this.ID  //0
                        , this.InputClassName(IsValid(isRequired), "aselect") //1
                        , isEnabled ? null : " disabled=\"disabled\"" // 2
                        , IsMultiSelect.GetValueOrDefault() ? " multiple=\"multiple\"" : null // 3
                        , this._OutputOption != null ? this._OutputOption.Value : OutputText // 4
                        , this.Placeholder //5 
                        , this.AsyncMinInput.ToString()
                        , this._OutputOption == null || String.IsNullOrEmpty(this._OutputOption.Text) ? null : string.Format(" data-text=\"{0}\"", this._OutputOption.Text)
                        , this.IsTagging ? " data-tags=\"true\" data-max=\"1\"" : null
                        , this._OutputOption == null || String.IsNullOrEmpty(this._OutputOption.Text) ? null : this._OutputOption.Text //9
                        , opt
                            );
                }
                else
                {
                    string reset = null;
                    if (this.OnChangeReset != null && this.OnChangeReset.Length > 0)
                    {
                        reset = string.Format(" data-reset=\"{0}\"", Data.Utility.ConvertToCsvString(this.OnChangeReset, false));
                    }
                    element.AppendFormat("\n\t\t\t\t\t\t\t\t\t<select id=\"{0}\"{1} name=\"{0}\"{2}{4}{5}{6}{7}>{3}"
                        , this.ID
                        , this.InputClassName(IsValid(isRequired), IgnoreStyle ? null : "styled")
                        , isEnabled ? null : " disabled=\"disabled\""
                        , options
                        , (this.IsTagging || IsMultiSelect.GetValueOrDefault()) ? " multiple=\"multiple\"" : null
                        , reset
                        , width
                        , this.IsTagging ? (IsMultiSelect.GetValueOrDefault() ? " data-tags=\"true\"" : " data-tags=\"true\" data-max=\"1\"") : null
                        );
                    element.Append("\n\t\t\t\t\t\t\t\t\t</select>");
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


                    build.AppendFormat("\n\t\t\t\t\t\t\t<td{0}{1}>{2}"
                        , (Expression == OutputExpression.FullWidth && Console.HasDoubleCols) ? " colspan=\"3\"" : null
                        , this.InputCellClassName(this.IsValid(isRequired))
                        , CustomErrorText
                        );

                    build.AppendFormat("\n\t\t\t\t\t\t\t\t<div class=\"{0}\">", (Expression == OutputExpression.FullWidth) ? this.Class_Wide : "half");

                    build.Append(element.ToString());

                    //if (Wim.CommonConfiguration.VISUAL_VERSION == 1)
                    build.Append("\n\t\t\t\t\t\t\t\t</div>");


                    build.Append("\n\t\t\t\t\t\t\t</td>");

                    if (Expression == OutputExpression.FullWidth || Expression == OutputExpression.Right)
                        build.Append("\n\t\t\t\t\t\t</tr>\n");
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
                build.Append(GetSimpleTextElement(this.Title, this.Mandatory, candidate, this.InteractiveHelp));
            }
            
            if (IsTagging || IsMultiSelect.GetValueOrDefault(false))
            {
                bool hasoptions = optionsList != null && optionsList.Count > 0;
                if (!hasoptions && !string.IsNullOrWhiteSpace(OutputText))
                {
                    var split = OutputText.Split(',');
                    this._OutputValues = split.ToList();
                    foreach (var item in this._OutputValues)
                        optionsList.Add(item);
                }

                build.ApiResponse.Fields.Add(new Api.MediakiwiField()
                {
                    Event = AutoPostBack ? Api.MediakiwiJSEvent.change : Api.MediakiwiJSEvent.none,
                    Title = MandatoryWrap(this.Title),
                    Value = this._OutputValues != null ? this._OutputValues : new List<string>(),                    
                    Expression = this.Expression,
                    PropertyName = this.ID,
                    PropertyType = (Property == null) ? typeof(string).FullName : Property.PropertyType.FullName,
                    VueType = hasoptions ? Api.MediakiwiFormVueType.wimTag : Api.MediakiwiFormVueType.wimTagVue,
                    Options = optionsList,
                    ReadOnly = this.IsReadOnly
                });
            }
            else
            {

                build.ApiResponse.Fields.Add(new Api.MediakiwiField()
                {
                    Event = AutoPostBack ? Api.MediakiwiJSEvent.change : Api.MediakiwiJSEvent.none,
                    Title = MandatoryWrap(this.Title),
                    Value = this.OutputText,
                    Expression = this.Expression,
                    PropertyName = this.ID,
                    PropertyType = (Property == null) ? typeof(string).FullName : Property.PropertyType.FullName,
                    VueType = Api.MediakiwiFormVueType.wimChoiceDropdown,
                    Options = optionsList
                });
            }
            return ReadCandidate(OutputText);
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

                if (Mandatory && string.IsNullOrEmpty(OutputText))
                {
                    if (IsAsync)
                        return false;
                    else if (_OutputValues == null || _OutputValues.Count == 0)
                        return false;
                }
            }
            return true;
        }
    }
}