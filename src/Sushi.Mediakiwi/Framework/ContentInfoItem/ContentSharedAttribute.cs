using Microsoft.AspNetCore.Http;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using static Sushi.Mediakiwi.Beta.GeneratedCms.Source.Component;

namespace Sushi.Mediakiwi.Framework
{

    public delegate string Translator(string property, string value);
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ContentSharedAttribute : Attribute
    {
        /// <summary>
        /// This will contain an Anchor tag containing the replacement field [LABEL]
        /// when an Edit Shared Field URL is available
        /// </summary>
        public string EditSharedFieldLink { get; set; }

        /// <summary>
        /// This will contain an Icon DIV to add to an element to show that it's a
        /// shared field.
        /// </summary>
        public string SharedIcon { get; set; }

        /// <summary>
        /// To which component template does this field belong.
        /// </summary>
        public int? ComponentTemplateID { get; set; }


        protected (bool isShared, bool isEnabled, bool isHidden, string outputValue) ApplySharedFieldInformation(bool isEnabled, string outputValue)
        {
            bool _isEnabled = isEnabled;
            string _outputValue = outputValue;
            bool _isSharedField = false;
            bool _isHidden = false;

            if (ComponentTemplateID.GetValueOrDefault(0) > 0)
            {
                var sharedField = SharedField.FetchSingleForComponentTemplate(FieldName, ContentTypeSelection, ComponentTemplateID.Value);

                // Shared Field addition
                _isSharedField = (sharedField?.ID > 0) ? true : false;

                // Disable the field when it is shared
                if (_isSharedField)
                {
                    var cList = ComponentList.SelectOne(typeof(AppCentre.Data.Implementation.SharedFieldList));

                    _isEnabled = false;
                    _isHidden = sharedField.IsHiddenOnPage;

                    var sharedValue = SharedFieldTranslation.FetchSingleForFieldAndSite(sharedField.ID, Console.ChannelIndentifier);
                    if (sharedValue?.ID > 0)
                    {
                        _outputValue = sharedValue.GetPublishedValue();
                    }

                    InteractiveHelp += " [This is a Shared Field, click to edit]";
                    var editSharedFieldUrl = Console.CurrentListInstance.wim.GetUrl(new KeyValue[]
                    {
                        new KeyValue()
                        {
                            Key ="list",
                            Value = cList.ID
                        },
                        new KeyValue()
                        {
                            Key="item",
                            Value = sharedField.ID
                        },
                        new KeyValue()
                        {
                            Key="openinframe",
                            Value="2"
                        },
                        new KeyValue()
                        {
                            Key="page",
                            RemoveKey = true
                        },
                        new KeyValue()
                        {
                            Key = "ctemplateid",
                            Value = ComponentTemplateID.GetValueOrDefault(0)
                        }
                    });

                    Grid.LayerSpecification specs = new Grid.LayerSpecification()
                    {
                        Height = AppCentre.Data.Implementation.SharedFieldList.LAYER_HEIGHT,
                        Width = AppCentre.Data.Implementation.SharedFieldList.LAYER_WIDTH,
                    };

                    SharedIcon = $"<div class=\"iconStatus\"style=\"position: absolute; margin-top:2px; right:32px;\"><i class=\"fas fa-retweet\" title=\"Shared field\"></i></div>";
                    EditSharedFieldLink = $"<a class=\"openlayer\" data-layer=\"{specs.Parse()}\" href=\"{editSharedFieldUrl}\">[LABEL]</a>";
                }
            }

            return (_isSharedField, _isEnabled, _isHidden, _outputValue);
        }

        /// <summary>
        /// Checks if this is a shared Field and if the shared field has a valaue
        /// </summary>
        /// <returns></returns>
        public (bool isSharedField, bool hasValue) HasSharedValue()
        {
            if (IsSharedField && ComponentTemplateID.GetValueOrDefault(0) > 0)
            {
                var sharedField = SharedField.FetchSingleForComponentTemplate(FieldName, ContentTypeSelection, ComponentTemplateID.Value);

                // Shared Field addition
                var _isSharedField = (sharedField?.ID > 0) ? true : false;

                // Disable the field when it is shared
                if (_isSharedField)
                {
                    var sharedValue = SharedFieldTranslation.FetchSingleForFieldAndSite(sharedField.ID, Console.ChannelIndentifier);
                    if (sharedValue?.ID > 0)
                    {
                        var _outputValue = sharedValue.GetPublishedValue();

                        return (true, string.IsNullOrWhiteSpace(_outputValue) == false);
                    }
                }
                return (true, false);
            }
            return (false, false);
        }

        public ListInfoItem InfoItem { get; set; }

        public bool IsCloaked { get; set; }
        public bool IsHidden { get; set; }

        /// <summary>
        /// Sets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        public bool IsReadOnly
        {
            set { OverrideEditMode = value; }
            get { return OverrideEditMode; }
        }

        protected string MandatoryWrap(string title)
        {
            if (Mandatory)
            {
                return $"{title}*";
            }
            return title;
        }

        protected string GetClose()
        {
            return "</tr>";
        }
        protected string GetStart()
        {
            Console.RowCount++;
            return "<tr>";
        }


        /// <summary>
        /// Gets the post back value.
        /// </summary>
        /// <value>The post back value.</value>
        public static string PostBackValue { get { return "postBack"; } }

        bool _OverrideEditMode;
        /// <summary>
        /// 
        /// </summary>
        protected bool OverrideEditMode
        {
            get
            {
                if (Console != null && Console.CurrentListInstance != null && Console.CurrentListInstance.wim.Page.Body.Form.DisableInput)
                {
                    return true;
                }
                return _OverrideEditMode;
            }
            set
            {
                _OverrideEditMode = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal protected CustomData m_ContentContainer;
        /// <summary>
        /// Sets the content container.
        /// </summary>
        internal protected void SetContentContainer(Field field)
        {
            if (field == null)
            {
                field = new Field();
                field.Property = FieldName;
            }

            if (Property.PropertyType != typeof(CustomData))
            {
                return;
            }

            m_ContentContainer = Property.GetValue(SenderInstance, null) as CustomData;

            if (m_ContentContainer == null)
            {
                m_ContentContainer = new CustomData();
            }

            if (!m_ContentContainer[field.Property].IsEditable)
            {
                OverrideEditMode = true;
            }

            CustomDataFieldName = field.Property;
        }

        protected string CustomDataFieldName;

        /// <summary>
        /// Applies the content container.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="candidate">The candidate.</param>
        internal void ApplyContentContainer(Field field, string candidate)
        {
            field.Value = candidate;
            m_ContentContainer.Apply(field.Property, field.Value, (int)ContentTypeSelection);
            Property.SetValue(SenderInstance, m_ContentContainer, null);
        }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>The context.</value>
        protected HttpContext Context
        {
            get
            {
                return Console.Context;
            }
        }



        protected bool m_ForceLoadEvent;
        /// <summary>
        /// Sets a value indicating whether [force load event].
        /// </summary>
        /// <value><c>true</c> if [force load event]; otherwise, <c>false</c>.</value>
        public bool ForceLoadEvent
        {
            //get { return m_ForceLoadEvent; }
            set { m_ForceLoadEvent = value; }
        }

        /// <summary>
        /// Gets or sets the expression.
        /// </summary>
        /// <value>The expression.</value>
        public OutputExpression Expression { get; set; }

        internal string Class_Wide
        {
            get
            {
                if (Console.OpenInFrame == (int)LayerSize.Tiny)
                {
                    return "half";
                }
                return "long";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal protected bool m_CanHaveExpression;
        /// <summary>
        /// Gets a value indicating whether this instance can have expression.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can have expression; otherwise, <c>false</c>.
        /// </value>
        public bool CanHaveExpression
        {
            get { return m_CanHaveExpression; }
        }

        internal ListItemCollection m_ListItemCollection { get; set; }

        /// <summary>
        /// Applies the meta data list.
        /// </summary>
        /// <param name="listItems">The list items.</param>
        public void ApplyMetaDataList(MetaDataList[] listItems)
        {
            if (listItems == null || listItems.Length == 0)
            {
                return;
            }

            m_ListItemCollection = new ListItemCollection();
            foreach (MetaDataList item in listItems)
            {
                m_ListItemCollection.Add(new ListItem(item.Text, item.Value));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is blue print.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is blue print; otherwise, <c>false</c>.
        /// </value>
        public bool IsBluePrint { get; set; }

        public bool IsSharedField { get; set; }

        /// <summary>
        /// Is this the edit mode start up
        /// </summary>
        protected bool IsInitialLoad
        {
            get
            {
                if (m_ForceLoadEvent)
                {
                    return true;
                }
                //  Return true when there is no postback or state == -1 detected 

                if (Context.Request.HasFormContentType)
                {
                    // Introduced as of $ which can not be used for the richtextbox
                    string cleanKey = ID.Replace("$", string.Empty);
                    foreach (string key in Context.Request.Form.Keys)
                    {
                        // Introduced as of $ which can not be used for the richtextbox
                        string candidate = key.Replace("$", string.Empty);
                        if (cleanKey == candidate)
                        {
                            return false;
                        }
                    }
                }
                return !(Console.IsPosted(ID));
            }
        }

        internal bool IsInitialLoaded
        {
            get
            {
                if (m_ForceLoadEvent)
                {
                    return true;
                }
                //  Return true when there is no postback or state == -1 detected 

                if (Context.Request.HasFormContentType)
                {
                    // Introduced as of $ which can not be used for the richtextbox
                    string cleanKey = ID.Replace("$", string.Empty);
                    foreach (string key in Context.Request.Form.Keys)
                    {
                        // Introduced as of $ which can not be used for the richtextbox
                        string candidate = key.Replace("$", string.Empty);
                        if (cleanKey == candidate)
                        {
                            return false;
                        }
                    }
                }
                return !Console.HasPost;
            }
        }

        public bool IsInheritedField { get; set; }
        internal protected bool IsEnabled(bool isEnabled = true)
        {
            if (IsInheritedField && isEnabled && Console.CurrentPage != null && Console.CurrentPage.InheritContentEdited)
            {
                return false;
            }
            return isEnabled;
        }

        internal protected void SetWriteEnvironment()
        {
            //  TEMPORARY!
            if (ShowInheritedData)
            {
                Console.HasDoubleCols = true;
                Expression = OutputExpression.Right;
            }
        }

        internal protected class NameItemValue
        {
            public string Name { get; set; }
            private int? m_ID;
            public int? ID 
            { 
                get 
                { 
                    return m_ID;                 
                } 
                set 
                { 
                    m_ID = value; 
                    TextID = value.ToString(); 
                } 
            }
            public string TextID { get; set; }
            public string Value { get; set; }
        }

        public bool OverrideTableGeneration { get; set; }

        /// <summary>
        /// Set the HTML title of for the MultiField instance
        /// </summary>
        /// <param name="title"></param>
        /// <param name="classTag"></param>
        protected void SetMultiFieldTitleHTML(string title, string classTag = "icon-cube")
        {
            string sortOption = @"<a href=""#"" class=""icon-caret-up simpleSortUp"" title=""Move component up""> </a>
                                  <a href=""#""  class=""icon-caret-down simpleSortDown"" title=""Move component down""> </a>";
            _MultiFieldTitleHTML = @$"<h3><span class=""{classTag}""></span> {title}</h3>";
            _MultiFieldTitleHTML_Edit = @$"<h3><span class=""{classTag}""></span> {title}<a href=""#"" class=""closer icon-x""></a> {sortOption}</h3>";
        }
        string _MultiFieldTitleHTML;
        string _MultiFieldTitleHTML_Edit;

        /// <summary>
        /// Get the HTML title of for the MultiField instance
        /// </summary>
        /// <param name="isEditMode"></param>
        /// <returns></returns>
        public string GetMultiFieldTitleHTML(bool isEditMode)
        {
            if (isEditMode)
            {
                return _MultiFieldTitleHTML_Edit;
            }
            return _MultiFieldTitleHTML;
        }

        /// <summary>
        /// Applies the single item select.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <param name="canContainSingleItem">if set to <c>true</c> [can contain single item].</param>
        /// <param name="title">The title.</param>
        /// <param name="id">The id.</param>
        /// <param name="selectionlistID">The selectionlist ID.</param>
        /// <param name="urlAddition">The URL addition.</param>
        /// <param name="autoPostBack">if set to <c>true</c> [auto post back].</param>
        /// <param name="isRequired">if set to <c>true</c> [is required].</param>
        /// <param name="canOnlyOrderSort">if set to <c>true</c> [can only order sort].</param>
        /// <param name="size">The size.</param>
        /// <param name="items">The items.</param>
        /// <param name="hasScrollbar">if set to <c>true</c> [has scrollbar].</param>
        /// <param name="layerHeight">Height of the layer.</param>
        internal protected void ApplyItemSelect(
            WimControlBuilder build,
            bool canContainSingleItem,
            bool itemIsClickable,
            string title,
            string id,
            string selectionlistID,
            string urlAddition,
            bool autoPostBack,
            bool isRequired,
            bool canOnlyOrderSort,
            bool addNewItemsOnTop,
            LayerSize size,
            bool? hasScrollbar,
            int? layerHeight,
            Grid.LayerSpecification specification,
            params NameItemValue[] items
            )
        {
            if (items != null)
            {
                items = items.Where(x => x != null).ToArray();
            }

            StringBuilder list = new StringBuilder();

            string path = Console.WimPagePath.Replace("http://", "//", StringComparison.InvariantCultureIgnoreCase).Replace("https://", "//", StringComparison.InvariantCultureIgnoreCase);
            string query = string.IsNullOrEmpty(selectionlistID) ? string.Empty : $"list={selectionlistID}&";
            string url = $"{path}?{query}openinframe={(int)size}&referid=_{id}{urlAddition}";

            if (specification == null)
            {
                specification = new Grid.LayerSpecification(size);
            }

            if (hasScrollbar.HasValue)
            {
                specification.HasScrolling = hasScrollbar.Value;
            }

            if (layerHeight.HasValue)
            {
                specification.IsHeightPercentage = false;
                specification.Height = layerHeight.Value;
            }

            string layerTitle = string.Empty;
            if (!string.IsNullOrEmpty(selectionlistID))
            {
                int listID;
                Guid listGUID;
                if (Utility.IsNumeric(selectionlistID, out listID))
                {
                    layerTitle = ComponentList.SelectOne(listID).Name;
                }
                else if (Utility.IsGuid(selectionlistID, out listGUID))
                {
                    layerTitle = ComponentList.SelectOne(listGUID).Name;
                }
            }


            int index = -1;
            foreach (var item in items)
            {
                if (!string.IsNullOrEmpty(item.TextID) && item.TextID != "0")
                {
                    index++;
                    if (canOnlyOrderSort)
                    {
                        if (IsCloaked)
                        {
                            build.AppendCloakedFormat("<input type=\"hidden\" id=\"{0}${2}${8}\" name=\"{0}${2}${8}\" value=\"{2}|{3}\"/>"
                            , item.Name
                            , item.Value
                            , item.TextID
                            , Utility.CleanFormatting(item.Value)
                            , itemIsClickable ? $" href=\"{string.Concat(url.Replace("&item=0", string.Empty), "&item=", item.TextID)}\"" : null //4
                            , specification.Parse() //1
                            , layerTitle
                            , itemIsClickable ? " class=\"openlayer\"" : null
                            , index // 8: Added for possible double records; required for client (17-12-12015)
                            );
                        }

                        list.AppendFormat("<li class=\"instant\"><a{7} data-layer=\"{5}\" title=\"{6}\"{4}>{1}</a><input type=\"hidden\" id=\"{0}${2}${8}\" name=\"{0}${2}${8}\" value=\"{2}|{3}\"/> </li>"
                            , item.Name
                            , item.Value
                            , item.TextID
                            , Utility.CleanFormatting(item.Value)
                            , itemIsClickable ? $" href=\"{string.Concat(url.Replace("&item=0", string.Empty), "&item=", item.TextID)}\"" : null //4
                            , specification.Parse() //1
                            , layerTitle
                            , itemIsClickable ? " class=\"openlayer\"" : null
                            , index // 8: Added for possible double records; required for client (17-12-12015)
                            );
                    }
                    else
                    {
                        if (IsCloaked)
                        {
                            build.AppendCloakedFormat("<input type=\"hidden\" id=\"{0}${2}${8}\" name=\"{0}${2}${8}\" value=\"{2}|{3}\"/>"
                            , item.Name
                            , item.Value
                            , item.TextID
                            , Utility.CleanFormatting(item.Value)
                            , itemIsClickable ? $" href=\"{string.Concat(url.Replace("&item=0", string.Empty), "&item=", item.TextID)}\"" : null //4
                            , specification.Parse() //1
                            , layerTitle
                            , itemIsClickable ? " class=\"openlayer\"" : null
                            , index // 8: Added for possible double records; required for client (17-12-12015)
                            );
                        }

                        list.AppendFormat("<li class=\"instant\"><a{7} data-layer=\"{5}\" title=\"{6}\"{4}>{1}</a><figure class=\"icon-x del\"></figure><input type=\"hidden\" id=\"{0}${2}${8}\" name=\"{0}${2}${8}\" value=\"{2}|{3}\"/></li>"
                            , item.Name
                            , item.Value
                            , item.TextID
                            , Utility.CleanFormatting(item.Value)
                            , itemIsClickable ? $" href=\"{string.Concat(url.Replace("&item=0", string.Empty), "&item=", item.TextID)}\"" : null //4
                            , specification.Parse() //1
                            , layerTitle
                            , itemIsClickable ? " class=\"openlayer\"" : null
                            , index // 8: Added for possible double records; required for client (17-12-12015)
                            );
                    }
                }
            }

            bool isEnabled = IsEnabled();

            // [MR:03-06-2021] Apply shared field clickable icon.
            var sharedInfoApply = ApplySharedFieldInformation(isEnabled, OutputText);
            if (sharedInfoApply.isShared)
            {
                // Enable readonly when shared
                isEnabled = sharedInfoApply.isEnabled;

                // When Currently not cloaked, do so if its a shared field
                if (IsCloaked == false && sharedInfoApply.isHidden)
                {
                    IsCloaked = sharedInfoApply.isHidden;
                }
            }

            if (IsCloaked)
            {
                build.AppendCloaked($"<input type=\"hidden\" name=\"{id}\" value=\"_MK$PH_\"/>");
            }
            else
            {
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
                        build.AppendFormat($"<th><label for=\"{ID}\">{EditSharedFieldLink.Replace("[LABEL]", TitleLabel)}</label></th>");
                    }
                    else
                    {
                        build.AppendFormat($"<th><label for=\"{ID}\">{TitleLabel}</label></th>");
                    }

                    build.AppendFormat("<td{0}{1}>{2}"
                        , (Expression == OutputExpression.FullWidth && Console.HasDoubleCols) ? " colspan=\"3\"" : null
                        , InputCellClassName(IsValid(isRequired))
                        , CustomErrorText
                        );
                }

                build.AppendFormat("<div class=\"{0}\">"
                    , (Expression == OutputExpression.FullWidth) ? Class_Wide
                    : (OverrideTableGeneration ? "halfer" : "half")
                );

                build.AppendFormat("<div class=\"multiSortable select\"><input type=\"hidden\" name=\"{0}\" value=\"_MK$PH_\"/>", id);
                if (canContainSingleItem)
                {
                    build.AppendFormat("<ul id=\"_{0}\" class=\"single {1} {2}\">"
                        , id
                        , canOnlyOrderSort ? " wide" : " add"
                        , autoPostBack ? "postBack" : string.Empty
                        );
                }
                else
                {
                    build.AppendFormat("<ul id=\"_{0}\" class=\"connectedSortable multiple{1}{2}{3}\">",
                        id, // 0
                        canOnlyOrderSort ? " wide" : " add", // 1
                        autoPostBack ? " postBack" : string.Empty, // 2
                        addNewItemsOnTop ? " newItemsOnTop" : string.Empty // 3
                        );
                }

                build.Append(list.ToString());
                build.Append("</ul></div>");

                if (!canOnlyOrderSort)
                {
                    build.Append("<div class=\"buttonContainer\">");

                    build.AppendFormat("<a class=\"openlayer\" data-layer=\"{1}\" href=\"{0}\" data-title=\"{2}\"><figure class=\"{3}\"></figure></a>"
                        , url // 0
                        , specification.Parse() //1
                        , layerTitle //2
                        , true ? "icon-plus" : "flaticon solid plus-3 icon free" //3
                                                                                 //, Console.CurrentApplicationUser.ShowNewDesign2 ? "icon-plus" : "flaticon solid plus-3 icon free" //3
                        );

                    build.Append("</div>");
                }
                build.Append("<div class=\"clear\"></div>");
                build.Append("</div>");

                //  If set all table cell/row creation will be ignored
                if (!OverrideTableGeneration)
                {
                    build.Append("</td>");

                    if (Expression == OutputExpression.FullWidth || Expression == OutputExpression.Right)
                    {
                        build.Append("</tr>");
                    }
                }
            }
        }


        bool m_SelectedKeySet;
        string m_SelectedKey;
        /// <summary>
        /// Gets the selected key.
        /// </summary>
        protected string SelectedKey
        {
            get
            {
                if (!m_SelectedKeySet)
                {
                    m_SelectedKeySet = true;
                    if (Context.Request.HasFormContentType)
                    {
                        foreach (string key in Context.Request.Form.Keys)
                        {
                            // Introduced as of $ which can not be used for the richtextbox
                            if (key.StartsWith(string.Concat(ID, "$")))
                            {
                                m_SelectedKey = key;
                                break;
                            }
                        }
                    }
                }
                return m_SelectedKey;
            }
        }
        /// <summary>
        /// Gets the selected ID.
        /// </summary>
        protected int? SelectedID
        {
            get
            {
                if (string.IsNullOrEmpty(SelectedKey))
                {
                    return null;
                }

                return Utility.ConvertToInt(SelectedKey.Split('$')[1]);
            }
        }

        /// <summary>
        /// Gets the selected value.
        /// </summary>
        protected string SelectedValue
        {
            get
            {
                if (!string.IsNullOrEmpty(Console.Form(SelectedKey)))
                {
                    return Console.Form(SelectedKey);
                }
                return null;
            }
        }

        bool m_SelectedKeysSet;
        string[] m_SelectedKeys;
        /// <summary>
        /// Gets the selected key.
        /// </summary>
        protected string[] SelectedKeys
        {
            get
            {
                if (!m_SelectedKeysSet)
                {
                    List<string> list = new List<string>();
                    if (Context.Request.HasFormContentType)
                    {
                        foreach (string key in Context.Request.Form.Keys)
                        {
                            // Introduced as of $ which can not be used for the richtextbox
                            if (key.StartsWith(string.Concat(ID, "$")))
                            {
                                list.Add(key);
                            }
                        }
                    }
                    m_SelectedKeys = list.ToArray();
                }
                return m_SelectedKeys;
            }
        }

        /// <summary>
        /// Gets the selected I ds.
        /// </summary>
        protected int[] SelectedIDs
        {
            get
            {
                List<int> list = new List<int>();
                foreach (var key in SelectedKeys)
                {
                    list.Add(Utility.ConvertToInt(key.Split('$')[1]));
                }
                return list.ToArray();
            }
        }

        /// <summary>
        /// Gets the selected value.
        /// </summary>
        protected string[] SelectedValues
        {
            get
            {
                List<string> list = new List<string>();
                foreach (var key in SelectedKeys)
                {
                    list.Add(Console.Form(key));
                }
                return list.ToArray();
            }
        }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the output text.
        /// </summary>
        /// <value>The output text.</value>
        public string OutputText { get; set; }

        /// <summary>
        /// Gets or sets the show inherited data.
        /// </summary>
        /// <value>The show inherited data.</value>
        public bool ShowInheritedData { get; set; }

        /// <summary>
        /// Gets or sets the inhertited output text.
        /// </summary>
        /// <value>The inhertited output text.</value>
        public string InhertitedOutputText { get; set; }


        string m_FieldName;
        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        /// <value>The name of the field.</value>
        public string FieldName
        {
            get
            {
                if (IsBluePrint)
                {
                    return m_FieldName;
                }
                else
                {
                    //  Ajax writeout output exception (example = timesheets - ?xml=timesheet&list=55a4fc1e-cbf7-4941-8e27-cd37577f4f15)
                    if (Property == null)
                    {
                        return m_FieldName;
                    }
                    else if (Property.PropertyType.Equals(typeof(CustomData)) && !string.IsNullOrWhiteSpace(m_FieldName))
                    {
                        return m_FieldName;
                    }

                    return Property.Name;
                }
            }
            set { m_FieldName = value; }
        }

        /// <summary>
        /// Reads the candidate.
        /// </summary>
        /// <returns></returns>
        public Field ReadCandidate(object value)
        {
            Field field = new Field();
            field.Property = FieldName;
            field.Type = (int)ContentTypeSelection;
            field.Value = value == null ? null : value.ToString();
            return field;
        }

        /// <summary>
        /// Gets or sets the console.
        /// </summary>
        /// <value>The console.</value>
        public Beta.GeneratedCms.Console Console { get; set; }

        public bool HasSenderInstance
        {
            get { return m_SenderInstance != null; }
        }

        object m_SenderInstance;
        /// <summary>
        /// Gets the sender instance.
        /// </summary>
        /// <value>The sender instance.</value>
        public object SenderInstance
        {
            get
            {
                if (m_SenderInstance == null)
                {
                    m_SenderInstance = Console.CurrentListInstance;
                }
                return m_SenderInstance;
            }
            set { m_SenderInstance = value; }
        }

        public object SenderSponsorInstance { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected string CustomErrorText;
        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        public virtual bool IsValid(bool isRequired)
        {
            Mandatory = isRequired;
            //  Custom errors
            if (Console?.CurrentListInstance?.wim?.Notification?.Errors != null)
            {
                if (Console.CurrentListInstance.wim.Notification.Errors.ContainsKey(FieldName))
                {
                    if (Console.CurrentListInstance.wim.Notification.Errors[FieldName] != null)
                    {
                        CustomErrorText = string.Format("<span class=\"error\">{0}</span>", Console.CurrentListInstance.wim.Notification.Errors[FieldName].ToString());
                    }
                    return false;
                }

                if (CustomDataFieldName != null && Console.CurrentListInstance.wim.Notification.Errors.ContainsKey(CustomDataFieldName))
                {
                    if (Console.CurrentListInstance.wim.Notification.Errors[CustomDataFieldName] != null)
                    {
                        CustomErrorText = string.Format("<span class=\"error\">{0}</span>", Console.CurrentListInstance.wim.Notification.Errors[CustomDataFieldName].ToString());
                    }
                    return false;
                }
            }


            return true;
        }

        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="requestProperty"></param>
        /// <param name="senders"></param>
        /// <returns></returns>
        protected ListItemCollection GetCollection(string property, string requestProperty, params object[] senders)
        {
            ListItemCollection instance = new ListItemCollection();

            bool hasError = false;
            foreach (var sender in senders)
            {
                if (sender == null)
                {
                    continue;
                }

                try
                {
                    IComponentListTemplate tmp = sender as IComponentListTemplate;
                    if (tmp != null)
                    {
                        tmp.wim.CurrentListRequestProperty = requestProperty;
                    }

                    ListItemCollection m_collection = null;
                    if (property != null)
                    {
                        if (property.Contains(":"))
                        {
                            m_collection = (ListItemCollection)GetMethod(sender, property);
                        }
                        else
                        {
                            m_collection = (ListItemCollection)GetProperty(sender, property);
                        }
                    }

                    if (m_collection != null)
                    {
                        foreach (ListItem li in m_collection)
                        {
                            instance.Add(li);
                        }
                    }
                    return instance;
                }
                catch (Exception ex)
                {
                    hasError = true;
                    return instance;
                }
            }
            if (hasError && instance.Count == 0)
            {
                instance.Add(string.Format("Not found property: {0}", property));
            }

            return instance;
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        protected object GetProperty(object sender, string property)
        {
            if (sender is MetaData)
            {
                MetaData item = ((MetaData)sender);
                return item.GetCollection(Console);
            }

            foreach (System.Reflection.PropertyInfo info in sender.GetType().GetProperties())
            {
                //  Get all public properties
                if (info.CanRead && info.Name == property)
                {   // Get all writable public properties
                    return info.GetValue(sender, null);
                }
            }

            if (!Console.CurrentListInstance.Equals(SenderInstance))
            {
                //  The SenderInstance is not equal to the current containing list. This occurs when a DataExtend is applied.
                //  This second run will check if the searched property can be found on the containing (parent object = Console.CurrentListInstance) object.
                sender = Console.CurrentListInstance;

                foreach (System.Reflection.PropertyInfo info in sender.GetType().GetProperties())
                {
                    //  Get all public properties
                    if (info.CanRead && info.Name == property)
                    {   // Get all writable public properties
                        return info.GetValue(sender, null);
                    }
                }
            }

            throw new Exception(string.Format("Could not find the '{0}' property.", property));
        }

        protected object GetMethod(object sender, string methodName)
        {
            string method = methodName.Split(':')[0];
            int value = Convert.ToInt32(methodName.Split(':')[1]);

            if (sender is MetaData)
            {
                MetaData item = ((MetaData)sender);
                return item.GetCollection(Console);
            }

            foreach (System.Reflection.MethodInfo info in sender.GetType().GetMethods())
            {
                if (info.Name == method)
                {
                    return info.Invoke(sender, new object[] { value });
                }
            }

            if (!Console.CurrentListInstance.Equals(SenderInstance))
            {
                //  The SenderInstance is not equal to the current containing list. This occurs when a DataExtend is applied.
                //  This second run will check if the searched property can be found on the containing (parent object = Console.CurrentListInstance) object.
                sender = Console.CurrentListInstance;

                foreach (System.Reflection.MethodInfo info in sender.GetType().GetMethods())
                {
                    if (info.Name == method)
                    {
                        return info.Invoke(sender, new object[] { value });
                    }
                }
            }

            throw new Exception(string.Format("Could not find the '{0}' method.", method));
        }

        protected void ApplyTranslation(StringBuilder build)
        {
            build.Append("<tr>");
            build.AppendFormat("<th class=\"local\"><label>{0}</label></th><td><div class=\"half\">{1}</div></td>\n", TitleLabel, InhertitedOutputText);

        }

        protected void ApplyTranslation(WimControlBuilder build)
        {
            build.Append("<tr>");
            build.AppendFormat("<th class=\"local\"><label>{0}</label></th><td><div class=\"half\">{1}</div></td>\n", TitleLabel, InhertitedOutputText);

        }

        /// <summary>
        /// Gets the simple text element.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="isMandatory">if set to <c>true</c> [is mandatory].</param>
        /// <param name="candidate">The candidate.</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        /// <returns></returns>
        protected string GetSimpleTextElement(string candidate, bool isShort = false, string inputPostHTML = null)
        {

            if (IsCloaked || IsHidden)
            {
                return string.Empty;
            }

            StringBuilder build = new StringBuilder();

            //  If set all table cell/row creation will be ignored
            if (!OverrideTableGeneration)
            {
                if (ShowInheritedData)
                {
                    ApplyTranslation(build);
                }
                else
                {
                    if ((Console.ExpressionPrevious == OutputExpression.Left && Expression == OutputExpression.FullWidth)
                        || (Console.ExpressionPrevious == OutputExpression.Left && Expression == OutputExpression.Left))
                    {
                        build.Append("<th class=\"half\"><label>&nbsp;</label></th><td>&nbsp;</td></tr>");
                    }

                    if ((Console.ExpressionPrevious == OutputExpression.FullWidth && Expression == OutputExpression.Right)
                        || (Console.ExpressionPrevious == OutputExpression.Right && Expression == OutputExpression.Right))
                    {
                        build.Append("<tr><th class=\"half\"><label>&nbsp;</label></th><td>&nbsp;</td>");
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
                        build.Append($"<th class=\"full\"><label for=\"{ID}\" class=\"input-text {(isShort ? " short" : null)}\">{EditSharedFieldLink.Replace("[LABEL]", TitleLabel)}</label></th>");
                    }
                    else
                    {
                        build.Append($"<th class=\"full\"><label for=\"{ID}\" class=\"input-text {(isShort ? " short" : null)}\">{TitleLabel}</label></th>");
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(EditSharedFieldLink) == false)
                    {
                        build.Append($"<th class=\"half\"><label for=\"{ID}\" class=\"input-text {(isShort ? " short" : null)}\">{EditSharedFieldLink.Replace("[LABEL]", TitleLabel)}</label></th>");
                    }
                    else
                    {
                        build.Append($"<th class=\"half\"><label for=\"{ID}\" class=\"input-text {(isShort ? " short" : null)}\">{TitleLabel}</label></th>");
                    }
                }

                build.AppendFormat("<td{0}{1}>"
                    , (Expression == OutputExpression.FullWidth && Console.HasDoubleCols) ? " colspan=\"3\"" : null
                    , (Expression == OutputExpression.FullWidth) ? " class=\"full\"" : " class=\"half\""
                    );
            }

            build.AppendFormat("<label class=\"input-text{0}\">", isShort ? " short" : null);

            if (string.IsNullOrEmpty(candidate))
            {
                candidate = "&nbsp;";
            }

            // Add Shared Icon (if any)
            if (string.IsNullOrWhiteSpace(SharedIcon) == false && IsCloaked == false)
            {
                build.Append(SharedIcon);
            }

            build.AppendFormat("{0}", candidate);

            build.AppendFormat("</label>{0}", inputPostHTML);

            //  If set all table cell/row creation will be ignored
            if (!OverrideTableGeneration)
            {
                build.Append("</td>");

                if (Expression == OutputExpression.FullWidth || Expression == OutputExpression.Right)
                {
                    build.Append("</tr>");
                }
            }

            return build.ToString();
        }

        private string m_Title;
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            set
            {
                m_Title = value;
            }
            get
            {
                if (m_Title != null && m_Title.StartsWith("_"))
                {
                    return Labels.ResourceManager.GetString(m_Title, new CultureInfo(Console.CurrentApplicationUser.LanguageCulture));
                }
                return m_Title;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public System.Reflection.PropertyInfo Property { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ContentType ContentTypeSelection { get; set; }

        internal bool HasCSS3
        {
            get
            {
                return false;
            }
        }

        internal string TitleLabel
        {
            get
            {
                if (!string.IsNullOrEmpty(Title) && Title.Contains("<"))
                {
                    return string.Concat(Title, Mandatory ? "<em>*</em>" : null);
                }
                else
                {
                    string titleTag =
                        string.IsNullOrEmpty(InteractiveHelp)
                            ? string.Concat("<abbr>", Title, Mandatory ? "<em>*</em>" : null, "</abbr>")
                            : HasCSS3
                                ? string.Format(@"<abbr data-title=""<b>{1}</b><br/>{0}""><span class=""dot"">{1}{2}<span></abbr>", InteractiveHelpResourceable, Title, Mandatory ? "<em>*</em>" : null)
                                : string.Format(@"<abbr title=""<b>{1}</b><br/>{0}""><span class=""dot"">{1}{2}<span></abbr>", InteractiveHelpResourceable, Title, Mandatory ? "<em>*</em>" : null)
                                ;
                    return titleTag;
                }
            }
        }

        internal static readonly int BREAKPOINT = 10;

        internal protected bool m_AutoPostBack { get; set; }

        /// <summary>
        /// Inputs the name of the class.
        /// </summary>
        /// <param name="isValid">if set to <c>true</c> [is valid].</param>
        /// <returns></returns>
        internal string InputClassName(bool isValid)
        {
            return InputClassName(isValid, null);
        }

        /// <summary>
        /// Inputs the name of the class.
        /// </summary>
        /// <param name="isValid">if set to <c>true</c> [is valid].</param>
        /// <param name="addition">The addition.</param>
        /// <returns></returns>
        internal string InputClassName(bool isValid, string addition, bool includeHtml = true)
        {
            string className;
            if (Expression == OutputExpression.FullWidth)
            {
                className = Class_Wide;
            }
            else
            {
                className = "half";
            }

            if (MaxValueLength != 0 && MaxValueLength <= BREAKPOINT)
            {
                className += " short";
            }

            if (m_AutoPostBack && !Console.IsJson)
            {
                className += " postBack";
            }

            if (!isValid)
            {
                className += " error";
            }

            if (IsCloaked)
            {
                return InputCellClassName(isValid, includeHtml);
            }

            className = string.Concat(className, " ", addition).Replace("  ", " ").Trim();
            if (string.IsNullOrEmpty(className))
            {
                return string.Empty;
            }

            if (includeHtml)
            {
                return string.Format(" class=\"{0}\"", className);
            }

            return className;
        }

        internal string InputPostBackClassName()
        {
            string className = string.Empty;

            if (m_AutoPostBack)
            {
                className += " postBack";
            }

            if (IsCloaked)
            {
                className += " hidden";
            }

            return className;
        }

        internal string InputCellClassName(bool isValid, bool includeHtml = true)
        {
            string className = string.Empty;
            if (!isValid)
            {
                className = "error";
            }

            if (IsCloaked)
            {
                className = "hidden";
            }

            if (Expression == OutputExpression.FullWidth)
            {
                className += " full";
            }
            else
            {
                className += " half";
            }

            if (!string.IsNullOrEmpty(className) && includeHtml)
            {
                className = string.Format(" class=\"{0}\"", className.Trim());
            }

            return className;
        }

        internal string InputCloaked()
        {
            string className = null;

            if (IsCloaked)
            {
                className = "hidden";
            }

            if (!string.IsNullOrEmpty(className))
            {
                className = string.Format(" class=\"{0}\"", className.Trim());
            }

            return className;
        }


        internal string m_InteractiveHelp;
        /// <summary>
        /// 
        /// </summary>
        public string InteractiveHelp
        {
            set
            {
                if (value == null)
                {
                    m_InteractiveHelp = value;
                }
                else
                {
                    m_InteractiveHelp = value.Replace("\"", "&#34;");
                }
            }
            get
            {

                if (!string.IsNullOrEmpty(m_InteractiveHelp))
                {
                    return string.Format("<label for=\"{1}\">{0}</label>", m_InteractiveHelp, ID);
                }
                return m_InteractiveHelp;
            }
        }

        /// <summary>
        ///  To bad this getter needs to exist.. the above getter has an extra label. We use the below in TitleLabel so resourceable labels can be used
        /// </summary>
        public string InteractiveHelpResourceable
        {
            get
            {
                return m_InteractiveHelp;
            }
        }
        internal string m_InputPostText;
        /// <summary>
        /// Gets or sets the input field post text (only applicable for Mediakiwi version CMS).
        /// </summary>
        public string InputPostText
        {
            set { m_InputPostText = value; }
            get
            {
                if (!string.IsNullOrEmpty(m_InputPostText))
                {
                    return string.Format("<label for=\"{1}\">{0}</label>", m_InputPostText, ID);
                }
                return m_InputPostText;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Mandatory { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int MaxValueLength { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Collection { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual MetaData GetMetaData(string name)
        {
            MetaData meta = new MetaData();
            Utility.ReflectProperty(this, meta);
            meta.Name = name;
            meta.ContentTypeSelection = ((int)ContentTypeSelection).ToString();
            return meta;
        }

        /// <summary>
        /// Gets the meta data.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public virtual MetaData GetMetaData(string name, string defaultValue)
        {
            MetaData meta = new MetaData();
            Utility.ReflectProperty(this, meta);
            meta.Name = name;
            meta.Default = defaultValue;
            meta.ContentTypeSelection = ((int)ContentTypeSelection).ToString();
            return meta;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="collectionPropertyValue"></param>
        /// <returns></returns>
        public virtual MetaData GetMetaData(string name, ListItemCollection collectionPropertyValue)
        {
            MetaData meta = new MetaData();
            Utility.ReflectProperty(this, meta);
            meta.Name = name;
            meta.ContentTypeSelection = ((int)ContentTypeSelection).ToString();

            List<MetaDataList> list = new List<MetaDataList>();
            foreach (ListItem li in collectionPropertyValue)
            {
                list.Add(new MetaDataList(li.Text, li.Value));
            }

            meta.CollectionList = list.ToArray();
            return meta;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="meta"></param>
        public void Apply(MetaData meta)
        {
            Utility.ReflectProperty(meta, this);
            ContentTypeSelection = (ContentType)Convert.ToInt32(meta.ContentTypeSelection);
        }

        public void Init(WimComponentListRoot wim)
        {
            Console = wim.Console;
            if (InfoItem != null)
            {
                SenderInstance = InfoItem.SenderInstance;
            }
        }

        public void Chain(string id)
        {
            if (_OnChange != null && Console.PostbackValue.Equals(id, StringComparison.InvariantCultureIgnoreCase))
            {
                var candidate = Console.Form(id);
                var e = new ContentInfoEventArgs(candidate);
                _OnChange(this, e);
            }
        }

        private ContentInfoEventHandler _OnChange;
        public event ContentInfoEventHandler OnChange
        {
            add { _OnChange += value; }
            remove { _OnChange -= value; }
        }

        public string GetFormMapClass()
        {
            if (SenderInstance is FormMap)
            {
                return SenderInstance.GetType().FullName;
            }
            else if (SenderSponsorInstance is FormMap)
            {
                return SenderSponsorInstance.GetType().FullName;
            }
            return string.Empty;
        }
    }
}
