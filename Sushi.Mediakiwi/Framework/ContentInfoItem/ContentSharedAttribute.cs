using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using static Sushi.Mediakiwi.Beta.GeneratedCms.Source.Component;
using Microsoft.AspNetCore.Http;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.UI;

namespace Sushi.Mediakiwi.Framework
{
    public delegate string Translator(string property, string value);
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ContentSharedAttribute : Attribute
    {
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
            set { this.OverrideEditMode = value; }
            get { return this.OverrideEditMode; }
        }

        protected string MandatoryWrap(string title)
        {
            if (this.Mandatory)
                return $"{title}*";
            return title;
        }

        protected string GetClose()
        {
            return "\n\t\t\t\t\t\t</tr>";
        }
        protected string GetStart()
        {
            Console.RowCount++;
            return "\t\t\t\t\t\t<tr>\n";
        }


        /// <summary>
        /// Gets the post back value.
        /// </summary>
        /// <value>The post back value.</value>
        public string PostBackValue
        {
            get
            {
                return "postBack";
            }
        }

        bool _OverrideEditMode;
        /// <summary>
        /// 
        /// </summary>
        protected bool OverrideEditMode
        {
            get {
                if (_Console != null && _Console.CurrentListInstance != null && _Console.CurrentListInstance.wim.Page.Body.Form.DisableInput)
                    return true;
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
        protected internal Data.CustomData m_ContentContainer;
        /// <summary>
        /// Sets the content container.
        /// </summary>
        protected internal void SetContentContainer(Field field)
        {

            if (Property.PropertyType != typeof(Data.CustomData))
                return;

            m_ContentContainer = Property.GetValue(SenderInstance, null) as Data.CustomData;

            if (m_ContentContainer == null)
                m_ContentContainer = new Sushi.Mediakiwi.Data.CustomData();

            if (!m_ContentContainer[field.Property].IsEditable)
                OverrideEditMode = true;

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
            m_ContentContainer.Apply(field.Property, field.Value);
            //m_ContentContainer.Add(field);
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

        OutputExpression m_Expression;
        /// <summary>
        /// Gets or sets the expression.
        /// </summary>
        /// <value>The expression.</value>
        public OutputExpression Expression
        {
            get { return m_Expression; }
            set { m_Expression = value; }
        }

        internal string Class_Wide
        {
            get
            {
                if (this.Console.OpenInFrame == (int)LayerSize.Tiny)
                    return "half";
                return "long";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected internal bool m_CanHaveExpression;
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
        Sushi.Mediakiwi.UI.ListItemCollection _ListItemCollection;
        internal Sushi.Mediakiwi.UI.ListItemCollection m_ListItemCollection
        {
            get { return _ListItemCollection; }
            set { _ListItemCollection = value;  }
        }


        /// <summary>
        /// Applies the meta data list.
        /// </summary>
        /// <param name="listItems">The list items.</param>
        public void ApplyMetaDataList(Sushi.Mediakiwi.Framework.MetaDataList[] listItems)
        {
            if (listItems == null || listItems.Length == 0) return;

            m_ListItemCollection = new Sushi.Mediakiwi.UI.ListItemCollection();
            foreach (Sushi.Mediakiwi.Framework.MetaDataList item in listItems)
            {
                m_ListItemCollection.Add(new ListItem(item.Text, item.Value));
            }
        }
        

        bool m_IsBluePrint;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is blue print.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is blue print; otherwise, <c>false</c>.
        /// </value>
        public bool IsBluePrint
        {
            set { m_IsBluePrint = value; }
            get { return m_IsBluePrint; }
        }

        //protected bool IsMultiFile { get; set; }

        /// <summary>
        /// Is this the edit mode start up
        /// </summary>
        protected bool IsInitialLoad
        {
            get
            {
                if (m_ForceLoadEvent) 
                    return true;
                //  Return true when there is no postback or state == -1 detected 

                if (Context.Request.HasFormContentType)
                {
                    //if (this.IsMultiFile) return false;

                    //if (Context.Request.ContentType.Contains("json")) return false;

                    // Introduced as of $ which can not be used for the richtextbox
                    string cleanKey = this.ID.Replace("$", string.Empty);
                    foreach (string key in Context.Request.Form.Keys)
                    {
                        // Introduced as of $ which can not be used for the richtextbox
                        string candidate = key.Replace("$", string.Empty);
                        if (cleanKey == candidate)
                            return false;
                    }
                }
                return !(Console.IsPosted(ID));
            }
        }

        public bool IsInheritedField { get; set;  }
        protected internal bool IsEnabled(bool isEnabled = true)
        {
            if (IsInheritedField && isEnabled)
            {
                if (this.Console.CurrentPage != null
                    && this.Console.CurrentPage.InheritContentEdited
                    
                    )
                {
                    return false;
                }
            }
            return isEnabled;
        }

        protected internal void SetWriteEnvironment()
        {
            //  TEMPORARY!
            if (ShowInheritedData)
            {
                Console.HasDoubleCols = true;
                Expression = OutputExpression.Right;
            }
        }

        protected bool IsInitialLoaded
        {
            get
            {
                if (m_ForceLoadEvent) return true;
                if (!Context.Request.HasFormContentType)
                    return true;

                //  Return true when there is no postback or state == -1 detected 
                if (Context.Request.ContentType.Contains("json"))
                { 
                    return Context.Request.Method == "GET";
                }

                return (Console.Request.Form.Count == 0 || Console.Form("state") == "1" || Console.Form("autopostback").Equals("edit", StringComparison.InvariantCultureIgnoreCase));
            }
        }

        protected internal class NameItemValue
        {
            public string Name { get; set; }
            public int? m_ID;
            public int? ID { get { return m_ID; } set { m_ID = value; TextID = value.ToString(); } }
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
            string sortOption = string.Empty; 
            
            sortOption = @"<a href=""#"" class=""icon-caret-up simpleSortUp"" title=""Move component up"">&nbsp;</a>
                                  <a href=""#""  class=""icon-caret-down simpleSortDown"" title=""Move component down"">&nbsp;</a>";
            this._MultiFieldTitleHTML = string.Format("<h3><span class=\"{0}\"></span> {1}</h3>", classTag, title);
            this._MultiFieldTitleHTML_Edit = string.Format("<h3><span class=\"{0}\"></span> {1}<a href=\"#\" class=\"closer icon-x\"></a> {2}</h3>", classTag, title, sortOption);
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
                return this._MultiFieldTitleHTML_Edit;
            return this._MultiFieldTitleHTML;
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
        protected internal void ApplyItemSelect(
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

            StringBuilder list = new StringBuilder();

            string url = string.Format("{0}?{1}openinframe={4}&referid=_{2}{3}"
                    , this.Console.WimPagePath.Replace("http://", "//").Replace("https://", "//")
                    , string.IsNullOrEmpty(selectionlistID) ? string.Empty : string.Format("list={0}&", selectionlistID)
                    , id
                    , urlAddition
                    , ((int)size).ToString()
                    );

            if (specification == null)
                specification = new Grid.LayerSpecification(size);

            if (hasScrollbar.HasValue)
                specification.HasScrolling = hasScrollbar.Value;

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
                if (Data.Utility.IsNumeric(selectionlistID, out listID))
                    layerTitle = Sushi.Mediakiwi.Data.ComponentList.SelectOne(listID).Name;
                else if (Data.Utility.IsGuid(selectionlistID, out listGUID))
                    layerTitle = Sushi.Mediakiwi.Data.ComponentList.SelectOne(listGUID).Name;
            }


            int index = -1;
            foreach (var item in items)
            {
                if (!string.IsNullOrEmpty(item.TextID) && item.TextID != "0")
                {
                    string classname = "ui-state-default";

                    index++;
                    if (canOnlyOrderSort)
                    {
                        if (IsCloaked)
                        {
                            build.AppendCloakedFormat("<input type=\"hidden\" id=\"{0}${2}${8}\" name=\"{0}${2}${8}\" value=\"{2}|{3}\"/>"
                            , item.Name
                            , item.Value
                            , item.TextID
                            , Data.Utility.CleanFormatting(item.Value)
                            , itemIsClickable ? $" href=\"{string.Concat(url.Replace("&item=0", string.Empty), "&item=", item.TextID)}\"" : null //4
                            , specification.Parse() //1
                            , layerTitle
                            , itemIsClickable ? " class=\"openlayer\"" : null
                            , index // 8: Added for possible double records; required for client (17-12-12015)
                            );
                        }

                        list.AppendFormat("\n\t\t\t\t\t\t\t\t\t\t\t<li class=\"instant\"><a{7} data-layer=\"{5}\" title=\"{6}\"{4}>{1}</a><input type=\"hidden\" id=\"{0}${2}${8}\" name=\"{0}${2}${8}\" value=\"{2}|{3}\"/> </li>"
                            , item.Name
                            , item.Value
                            , item.TextID
                            , Data.Utility.CleanFormatting(item.Value)
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
                            , Data.Utility.CleanFormatting(item.Value)
                            , itemIsClickable ? $" href=\"{string.Concat(url.Replace("&item=0", string.Empty), "&item=", item.TextID)}\"" : null //4
                            , specification.Parse() //1
                            , layerTitle
                            , itemIsClickable ? " class=\"openlayer\"" : null
                            , index // 8: Added for possible double records; required for client (17-12-12015)
                            );
                        }

                        list.AppendFormat("\n\t\t\t\t\t\t\t\t\t\t\t<li class=\"instant\"><a{7} data-layer=\"{5}\" title=\"{6}\"{4}>{1}</a><figure class=\"icon-x del\"></figure><input type=\"hidden\" id=\"{0}${2}${8}\" name=\"{0}${2}${8}\" value=\"{2}|{3}\"/></li>"
                            , item.Name
                            , item.Value
                            , item.TextID
                            , Data.Utility.CleanFormatting(item.Value)
                            , itemIsClickable ? $" href=\"{string.Concat(url.Replace("&item=0", string.Empty), "&item=", item.TextID)}\"" : null //4
                            , specification.Parse() //1
                            , layerTitle
                            , itemIsClickable ? " class=\"openlayer\"" : null
                            , index // 8: Added for possible double records; required for client (17-12-12015)
                            ); 
                    }
                }
            }

            //if ((items == null || items.Length == 0) || items[0].ID == 0)
            //{
            //    list.AppendFormat("\n\t\t\t\t\t\t\t\t\t\t\t<li class=\"instant\"><em style=\"color:#a7aab3;margin-left:-5px\">{0}</em></li>", Data.Utility.CleanFormatting(this.InteractiveHelp));
            //}


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
                }

                build.AppendFormat("\n\t\t\t\t\t\t\t\t<div class=\"{0}\">"
                    , (Expression == OutputExpression.FullWidth) ? this.Class_Wide
                    : (OverrideTableGeneration ? "halfer" : "half")
                );

                build.AppendFormat("\n\t\t\t\t\t\t\t\t\t<div class=\"multiSortable select\"><input type=\"hidden\" name=\"{0}\" value=\"_MK$PH_\"/>", id);
                if (canContainSingleItem)
                {
                    build.AppendFormat("\n\t\t\t\t\t\t\t\t\t\t<ul id=\"_{0}\" class=\"single {1} {2}\">", id,
                        canOnlyOrderSort ? " wide" : " add", autoPostBack ? "postBack" : string.Empty
                        );
                }
                else
                {
                    build.AppendFormat("\n\t\t\t\t\t\t\t\t\t\t<ul id=\"_{0}\" class=\"connectedSortable multiple{1}{2}{3}\">",
                        id, // 0
                        canOnlyOrderSort ? " wide" : " add", // 1
                        autoPostBack ? " postBack" : string.Empty, // 2
                        addNewItemsOnTop ? " newItemsOnTop" : string.Empty // 3
                        );
                }

                build.Append(list.ToString());
                build.Append("\n\t\t\t\t\t\t\t\t\t\t</ul>");
                build.Append("\n\t\t\t\t\t\t\t\t\t</div>");

                if (!canOnlyOrderSort)
                {
                    build.Append("\n\t\t\t\t\t\t\t\t\t<div class=\"buttonContainer\">");

                    //if (!url.Contains("item="))
                    //    url += "&item=0";

                    build.AppendFormat("<a class=\"openlayer\" data-layer=\"{1}\" href=\"{0}\" data-title=\"{2}\"><figure class=\"{3}\"></figure></a>"
                        , url // 0
                        , specification.Parse() //1
                        , layerTitle //2
                        , true ? "icon-plus" : "flaticon solid plus-3 icon free" //3
                        //, Console.CurrentApplicationUser.ShowNewDesign2 ? "icon-plus" : "flaticon solid plus-3 icon free" //3
                        );

                    build.Append("\n\t\t\t\t\t\t\t\t\t</div>");
                }
                build.Append("\n\t\t\t\t\t\t\t\t\t<div class=\"clear\"></div>");
                build.Append("\n\t\t\t\t\t\t\t\t</div>");

                //  If set all table cell/row creation will be ignored
                if (!OverrideTableGeneration)
                {
                    build.Append("\n\t\t\t\t\t\t\t</td>");

                    if (Expression == OutputExpression.FullWidth || Expression == OutputExpression.Right)
                        build.Append("\n\t\t\t\t\t\t</tr>\n");
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
                            if (key.StartsWith(string.Concat(this.ID, "$")))
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
                if (string.IsNullOrEmpty(this.SelectedKey))
                    return null;

                return Data.Utility.ConvertToInt(SelectedKey.Split('$')[1]);
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
                    return Console.Form(SelectedKey).ToString().Replace("T", string.Empty);
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
                    //m_SelectedKeysSet = true;
                    if (Context.Request.HasFormContentType)
                    {
                        foreach (string key in Context.Request.Form.Keys)
                        {
                            // Introduced as of $ which can not be used for the richtextbox
                            if (key.StartsWith(string.Concat(this.ID, "$")))
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
                    list.Add(Data.Utility.ConvertToInt(key.Split('$')[1]));
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




        string m_ID;
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public string ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        string m_OutputText;
        /// <summary>
        /// Gets or sets the output text.
        /// </summary>
        /// <value>The output text.</value>
        public string OutputText
        {
            get { return m_OutputText; }
            set { m_OutputText = value; }
        }

        internal bool m_ShowInheritedData;
        /// <summary>
        /// Gets or sets the show inherited data.
        /// </summary>
        /// <value>The show inherited data.</value>
        public bool ShowInheritedData
        {
            get { return m_ShowInheritedData; }
            set { m_ShowInheritedData = value; }
        }

        string m_InhertitedOutputText;
        /// <summary>
        /// Gets or sets the inhertited output text.
        /// </summary>
        /// <value>The inhertited output text.</value>
        public string InhertitedOutputText
        {
            get { return m_InhertitedOutputText; }
            set { m_InhertitedOutputText = value; }
        }

        string m_FieldName;
        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        /// <value>The name of the field.</value>
        public string FieldName
        {
            get {
                if (IsBluePrint)
                    return m_FieldName;
                else
                {
                    //  Ajax writeout output exception (example = timesheets - ?xml=timesheet&list=55a4fc1e-cbf7-4941-8e27-cd37577f4f15)
                    if (Property == null)
                        return m_FieldName;

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
            field.Property = this.FieldName;
            field.Type = (int)ContentTypeSelection;
            field.Value = value == null ? null : value.ToString();
            return field;
        }

        Beta.GeneratedCms.Console _Console;
        /// <summary>
        /// Gets or sets the console.
        /// </summary>
        /// <value>The console.</value>
        internal Beta.GeneratedCms.Console Console
        {
            set { 
                _Console = value; 
                
            }
            get { return _Console; }
        }

        public bool HasSenderInstance
        {
            get { return m_SenderInstance != null; }
        }

        Object m_SenderInstance;
        /// <summary>
        /// Gets the sender instance.
        /// </summary>
        /// <value>The sender instance.</value>
        public Object SenderInstance
        {
            get {
                if (m_SenderInstance == null)
                    m_SenderInstance = Console.CurrentListInstance;
                return m_SenderInstance; 
            }
            set { m_SenderInstance = value; }
        }
        public Object SenderSponsorInstance { get; set; }

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
            this.Mandatory = isRequired;
            //  Custom errors
            if (Console.CurrentListInstance.wim.Notification.Errors != null)
            {
                if (Console.CurrentListInstance.wim.Notification.Errors.Contains(this.FieldName))
                {
                    if (Console.CurrentListInstance.wim.Notification.Errors[this.FieldName] != null)
                        CustomErrorText = string.Format("<span class=\"error\">{0}</span>", Console.CurrentListInstance.wim.Notification.Errors[this.FieldName].ToString());
                    return false;
                }

                if (CustomDataFieldName != null)
                {
                    if (Console.CurrentListInstance.wim.Notification.Errors.Contains(this.CustomDataFieldName))
                    {
                        if (Console.CurrentListInstance.wim.Notification.Errors[this.CustomDataFieldName] != null)
                            CustomErrorText = string.Format("<span class=\"error\">{0}</span>", Console.CurrentListInstance.wim.Notification.Errors[this.CustomDataFieldName].ToString());
                        return false;
                    }
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
        protected Sushi.Mediakiwi.UI.ListItemCollection GetCollection(string property, string requestProperty, params object[] senders)
        {
            Sushi.Mediakiwi.UI.ListItemCollection instance = new Sushi.Mediakiwi.UI.ListItemCollection();

            bool hasError = false;
            foreach (var sender in senders)
            {
                if (sender == null)
                    continue;

                try
                {
                    IComponentListTemplate tmp = sender as IComponentListTemplate;
                    if (tmp != null)
                        tmp.wim.CurrentListRequestProperty = requestProperty;

                    Sushi.Mediakiwi.UI.ListItemCollection m_collection = null;
                    if (property.Contains(":"))
                        m_collection = (Sushi.Mediakiwi.UI.ListItemCollection)GetMethod(sender, property);
                    else
                        m_collection = (Sushi.Mediakiwi.UI.ListItemCollection)GetProperty(sender, property);

                    if (m_collection != null)
                    {
                        foreach (ListItem li in m_collection)
                            instance.Add(li);
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
                instance.Add(string.Format("Not found property: {0}", property));

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
            if (sender is Sushi.Mediakiwi.Framework.MetaData)
            {
                Sushi.Mediakiwi.Framework.MetaData item = ((Sushi.Mediakiwi.Framework.MetaData)sender);
                return item.GetCollection();
            }

            foreach (System.Reflection.PropertyInfo info in sender.GetType().GetProperties())
            {   //  Get all public properties
                if (info.CanRead)
                {   // Get all writable public properties
                    if (info.Name == property)
                    {
                        return info.GetValue(sender, null);
                    }
                }
            }

            if (!Console.CurrentListInstance.Equals(SenderInstance))
            {
                //  The SenderInstance is not equal to the current containing list. This occurs when a DataExtend is applied.
                //  This second run will check if the searched property can be found on the containing (parent object = Console.CurrentListInstance) object.
                sender = Console.CurrentListInstance;

                foreach (System.Reflection.PropertyInfo info in sender.GetType().GetProperties())
                {   //  Get all public properties
                    if (info.CanRead)
                    {   // Get all writable public properties
                        if (info.Name == property)
                        {
                            return info.GetValue(sender, null);
                        }
                    }
                }
            }

            throw new Exception(string.Format("Could not find the '{0}' property.", property));
        }

        protected object GetMethod(object sender, string methodName)
        {
            string method = methodName.Split(':')[0];
            int value = Convert.ToInt32(methodName.Split(':')[1]);

            if (sender is Sushi.Mediakiwi.Framework.MetaData)
            {
                Sushi.Mediakiwi.Framework.MetaData item = ((Sushi.Mediakiwi.Framework.MetaData)sender);
                return item.GetCollection();
            }

            foreach (System.Reflection.MethodInfo info in sender.GetType().GetMethods())
            {  
                if (info.Name == method)
                    return info.Invoke(sender, new object[] { value });
            }

            if (!Console.CurrentListInstance.Equals(SenderInstance))
            {
                //  The SenderInstance is not equal to the current containing list. This occurs when a DataExtend is applied.
                //  This second run will check if the searched property can be found on the containing (parent object = Console.CurrentListInstance) object.
                sender = Console.CurrentListInstance;

                foreach (System.Reflection.MethodInfo info in sender.GetType().GetMethods())
                {   
                    if (info.Name == method)
                        return info.Invoke(sender, new object[] { value });
                }
            }

            throw new Exception(string.Format("Could not find the '{0}' method.", method));
        }

        protected void ApplyTranslation(StringBuilder build)
        {
            build.Append("\t\t\t\t\t\t<tr>");
            build.AppendFormat("\t\t\t\t\t\t\t<th class=\"local\"><label>{0}</label></th>\t\t\t\t\t\t\t<td><div class=\"half\">{1}</div></td>\n", this.TitleLabel, this.InhertitedOutputText);

        }

        protected void ApplyTranslation(WimControlBuilder build)
        {
            build.Append("\t\t\t\t\t\t<tr>");
            build.AppendFormat("\t\t\t\t\t\t\t<th class=\"local\"><label>{0}</label></th>\t\t\t\t\t\t\t<td><div class=\"half\">{1}</div></td>\n", this.TitleLabel, this.InhertitedOutputText);

        }

        /// <summary>
        /// Gets the simple text element.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="isMandatory">if set to <c>true</c> [is mandatory].</param>
        /// <param name="candidate">The candidate.</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        /// <returns></returns>
        protected string GetSimpleTextElement(string title, bool isMandatory, string candidate, string interactiveHelp, bool isShort = false, string inputPostHTML = null)
        {
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
                        build.Append("\t\t\t\t\t\t\t<th class=\"half\"><label>&nbsp;</label></th><td>&nbsp;</td></tr>");

                    if ((Console.ExpressionPrevious == OutputExpression.FullWidth && Expression == OutputExpression.Right) 
                        || (Console.ExpressionPrevious == OutputExpression.Right && Expression == OutputExpression.Right))
                        build.Append("\t\t\t\t\t\t<tr><th class=\"half\"><label>&nbsp;</label></th>\n\t\t\t\t\t\t\t<td>&nbsp;</td>");

                    if (Expression == OutputExpression.FullWidth || Expression == OutputExpression.Left)
                        build.Append("\t\t\t\t\t\t<tr>");
                }

                if (Expression == OutputExpression.FullWidth)
                    build.AppendFormat("\n\t\t\t\t\t\t\t<th class=\"full\"><label class=\"input-text{2}\">{1}</label></th>", this.ID, this.TitleLabel, isShort ? " short" : null);
                else
                    build.AppendFormat("\n\t\t\t\t\t\t\t<th class=\"half\"><label class=\"input-text{2}\">{1}</label></th>", this.ID, this.TitleLabel, isShort ? " short" : null);

                build.AppendFormat("\n\t\t\t\t\t\t\t<td{0}{1}>"
                    , (Expression == OutputExpression.FullWidth && Console.HasDoubleCols) ? " colspan=\"3\"" : null
                    , (Expression == OutputExpression.FullWidth) ? " class=\"full\"" : " class=\"half\""
                    );
            }

            build.AppendFormat("\n\t\t\t\t\t\t\t\t<label class=\"input-text{0}\">", isShort ? " short" : null);

            if (string.IsNullOrEmpty(candidate))
                candidate = "&nbsp;";

            build.AppendFormat("\n\t\t\t\t\t\t\t\t\t{0}", candidate);

            build.AppendFormat("\n\t\t\t\t\t\t\t\t</label>{0}", inputPostHTML);

                //  If set all table cell/row creation will be ignored
            if (!OverrideTableGeneration)
            {
                build.Append("\n\t\t\t\t\t\t\t</td>");

                if (Expression == OutputExpression.FullWidth || Expression == OutputExpression.Right)
                    build.Append("\n\t\t\t\t\t\t</tr>\n");
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
            set {
                m_Title = value; 
            }
            get
            {
                if (m_Title != null && m_Title.StartsWith("_"))
                    return Labels.ResourceManager.GetString(m_Title, new CultureInfo(Console.CurrentApplicationUser.LanguageCulture));
                return m_Title;
            }             
        }

        private System.Reflection.PropertyInfo m_Property;
        /// <summary>
        /// 
        /// </summary>
        public System.Reflection.PropertyInfo Property
        {
            set { m_Property = value; }
            get { return m_Property; }
        }

        private ContentType _contenttype;
        /// <summary>
        /// 
        /// </summary>
        public ContentType ContentTypeSelection
        {
            set { this._contenttype = value; }
            get { return this._contenttype; }
        }

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
                        string.IsNullOrEmpty(this.InteractiveHelp)
                            ? string.Concat("<abbr>", Title, Mandatory ? "<em>*</em>" : null, "</abbr>")
                            : HasCSS3
                                ? string.Format(@"<abbr data-title=""<b>{1}</b><br/>{0}""><span class=""dot"">{1}{2}<span></abbr>", this.InteractiveHelpResourceable, Title, Mandatory ? "<em>*</em>" : null)
                                : string.Format(@"<abbr title=""<b>{1}</b><br/>{0}""><span class=""dot"">{1}{2}<span></abbr>", this.InteractiveHelpResourceable, Title, Mandatory ? "<em>*</em>" : null)
                                ;
                    return titleTag;
                }
            }
        }

        internal static int BREAKPOINT = 10;

        internal protected bool m_AutoPostBack;
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
                className = this.Class_Wide;
            else
                className = "half";

            if (this.MaxValueLength != 0 && this.MaxValueLength <= BREAKPOINT) 
                className += " short";

            if (this.m_AutoPostBack && !_Console.IsJson)
                className += " postBack";

            if (!isValid)
                className += " error";

            if (IsCloaked)
            {
                return InputCellClassName(isValid, includeHtml);
            }

            className = string.Concat(className, " ", addition).Replace("  ", " ").Trim();
            if (string.IsNullOrEmpty(className))
                return string.Empty;

            if (includeHtml)
                return string.Format(" class=\"{0}\"", className);
            return className;
        }

        internal string InputPostBackClassName()
        {
            string className = string.Empty;

            if (this.m_AutoPostBack)
                className += " postBack";

            if (IsCloaked)
                className += " hidden";

            return className;
        }

        internal string InputCellClassName(bool isValid, bool includeHtml = true)
        {
            string className = string.Empty;
            if (!isValid)
                className = "error";

            if (IsCloaked)
                className = "hidden";

            if (Expression == OutputExpression.FullWidth)
                className += " full";
            else
                className += " half";


            if (!string.IsNullOrEmpty(className) && includeHtml)
                className = string.Format(" class=\"{0}\"", className.Trim());

            return className;
        }

        internal string InputCloaked()
        {
            string className = null;

            if (IsCloaked)
                className = "hidden";

            if (!string.IsNullOrEmpty(className))
                className = string.Format(" class=\"{0}\"", className.Trim());

            return className;
        }


        internal string m_InteractiveHelp;
        /// <summary>
        /// 
        /// </summary>
        public string InteractiveHelp
        {
            set {
                if (value == null)
                    m_InteractiveHelp = value;
                else
                {
                    m_InteractiveHelp = value.Replace("\"", "&#34;");
                 
                }
                
            }
            get {
                
                if (!string.IsNullOrEmpty(this.m_InteractiveHelp))
                    return string.Format("<label for=\"{1}\">{0}</label>", m_InteractiveHelp, this.ID);
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
                if (!string.IsNullOrEmpty(this.m_InputPostText))
                    return string.Format("<label for=\"{1}\">{0}</label>", m_InputPostText, this.ID);
                return m_InputPostText;
            }
        }

        private bool m_Mandatory;
        /// <summary>
        /// 
        /// </summary>
        public bool Mandatory
        {
            set { m_Mandatory = value; }
            get { return m_Mandatory; }
        }

        private int m_MaxValueLength;
        /// <summary>
        /// 
        /// </summary>
        public int MaxValueLength
        {
            set { m_MaxValueLength = value; }
            get { return m_MaxValueLength; }
        }

        private string m_Collection;
        /// <summary>
        /// 
        /// </summary>
        public string Collection
        {
            set { m_Collection = value; }
            get { return m_Collection; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual MetaData GetMetaData(string name)
        {
            MetaData meta = new MetaData();
            Data.Utility.ReflectProperty(this, meta);
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
            Data.Utility.ReflectProperty(this, meta);
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
        public virtual MetaData GetMetaData(string name, Sushi.Mediakiwi.UI.ListItemCollection collectionPropertyValue)
        {
            MetaData meta = new MetaData();
            Data.Utility.ReflectProperty(this, meta);
            meta.Name = name;
            meta.ContentTypeSelection = ((int)ContentTypeSelection).ToString();

            List<MetaDataList> list = new List<MetaDataList>();
            foreach (ListItem li in collectionPropertyValue)
                list.Add(new MetaDataList(li.Text, li.Value));

            meta.CollectionList = list.ToArray();
            return meta;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="meta"></param>
        public void Apply(MetaData meta)
        {
            Data.Utility.ReflectProperty(meta, this);
            ContentTypeSelection = (ContentType)Convert.ToInt32(meta.ContentTypeSelection);
        }

        public void Init(WimComponentListRoot wim)
        {
            Console = wim.Console;
            if (this.InfoItem != null)
                SenderInstance = this.InfoItem.SenderInstance;
        }


        public void Chain(string id)
        {
            if (_OnChange != null)
            {
                if (Console.PostbackValue.Equals(id, StringComparison.InvariantCultureIgnoreCase))
                {
                    var candidate = Console.Form(id);
                    var e = new ContentInfoEventArgs(candidate);
                    _OnChange(this, e);
                }
            }
        }

        private ContentInfoEventHandler _OnChange;
        public event ContentInfoEventHandler OnChange
        {
            add { _OnChange += value; }
            remove { _OnChange -= value; }
        }
    }
}
