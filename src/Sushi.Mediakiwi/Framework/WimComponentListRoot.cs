using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections;
using System.Data;
using System.Collections.Generic;
using System.Text;
using Sushi.Mediakiwi.Data;
using System.Net.Mail;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Sushi.Mediakiwi.DataEntities;

namespace Sushi.Mediakiwi.Framework
{
    public class Body
    {
        public bool ShowInFullWidthMode { get; set; }
        internal string _FrameUrl;
        public void AddFrameUrl(System.Uri uri, bool showInFullWidthMode)
        {
            this._FrameUrl = uri.ToString();
            this.ShowInFullWidthMode = showInFullWidthMode;
        }

        internal bool _ClearBodyBase = false;
        internal StringBuilder _BodyAddition;
        internal BodyTarget _BodyTarget = BodyTarget.Below;

        Sushi.Mediakiwi.Framework.WimComponentListRoot _instance;
        public Body(Sushi.Mediakiwi.Framework.WimComponentListRoot instance)
        {
            _instance = instance;
            this.Grid = new Grid(instance);
            this.Filter = new Filter(instance);
            this.Navigation = new Navigation(instance);
            this.Form = new Form(instance);
        }

        public Grid Grid { get; set; }
        public Filter Filter { get; set; }
        public Navigation Navigation { get; set; }
        public Form Form { get; set; }
        /// <summary>
        /// Set the classname of the body, this will be added to the existing classname
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Clears the specified clear base template header.
        /// </summary>
        /// <param name="clearBaseTemplateBody">if set to <c>true</c> [clear base template body].</param>
        public void Clear(bool clearBaseTemplateBody)
        {
            this._ClearBodyBase = clearBaseTemplateBody;

            if (clearBaseTemplateBody)
                return;

            if (this._BodyAddition != null)
                this._BodyAddition = new StringBuilder();
        }

        /// <summary>
        /// Adds the layer.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="defaultSize">The default size.</param>
        /// <param name="alternativeHeight">Height of the alternative.</param>
        /// <param name="hasIframe">if set to <c>true</c> [has iframe].</param>
        /// <param name="hasScrolling">if set to <c>true</c> [has scrolling].</param>
        public void AddLayer(string url, LayerSize defaultSize, int? alternativeHeight = null, bool hasScrolling = true, bool hasIframe = true)
        {
            var specs = new Sushi.Mediakiwi.Framework.Grid.LayerSpecification(defaultSize);
            specs.HasScrolling = hasScrolling;
            specs.InFrame = hasIframe;
            if (alternativeHeight.HasValue)
            {
                specs.IsHeightPercentage = false;
                specs.Height = alternativeHeight;
            }

            string height = "";
            string html = string.Concat("<span class=\"openlayerauto\" data-url=\"", url, "\" ", specs.Parse(true), " />");
            
            this.Add(html, false);
        }

        public void Add(string html)
        {
            Add(html, false);
        }

        public void Add(string html, bool clearBaseTemplateBody)
        {
            Add(html, clearBaseTemplateBody, BodyTarget.Below);
        }

        public enum BodyTarget
        {
            Nested,
            Below
        }

        public void Add(string html, bool clearBaseTemplateBody, BodyTarget target)
        {
            if (clearBaseTemplateBody)
                Clear(clearBaseTemplateBody);

            if (this._BodyAddition == null)
                this._BodyAddition = new StringBuilder();

            this._BodyTarget = target;
            this._BodyAddition.Append(html);
        }
    }

    public class Filter
    {
        /// <summary>
        /// If [true] than disable the setting of the filter fields from the stored previous filter setting (dropdown, textfield,..). 
        /// </summary>
        /// <value>
        ///   <c>true</c> if [disable default setup]; otherwise, <c>false</c>.
        /// </value>
        public bool DisableDefaultSetup { get; set; }
        Sushi.Mediakiwi.Framework.WimComponentListRoot _instance;
        public Filter(Sushi.Mediakiwi.Framework.WimComponentListRoot instance)
        {
            _instance = instance;
        }
    }

    public class Navigation
    {
        Sushi.Mediakiwi.Framework.WimComponentListRoot _instance;
        public Navigation(Sushi.Mediakiwi.Framework.WimComponentListRoot instance)
        {
            _instance = instance;
            this.Side = new SideNavigation(instance);
        }

        public SideNavigation Side { get; set; }
    }

    public class SideNavigation
    {
        public enum SideNavigationTarget
        {
            Above,
            Below
        }

        Sushi.Mediakiwi.Framework.WimComponentListRoot _instance;
        public SideNavigation(Sushi.Mediakiwi.Framework.WimComponentListRoot instance)
        {
            _instance = instance;
        }

        public void Add(string html)
        {
            Add(html, false);
        }

        public void Add(string html, bool clearBaseTemplateBody)
        {
            Add(html, false, SideNavigationTarget.Below);
        }

        /// <summary>
        /// Clears the specified clear base template header.
        /// </summary>
        /// <param name="clearBaseTemplateBody">if set to <c>true</c> [clear base template body].</param>
        public void Clear(bool clearBaseTemplateBody)
        {
            this._ClearBodyBase = clearBaseTemplateBody;

            if (clearBaseTemplateBody)
                return;

            if (this._BodyAddition != null)
                this._BodyAddition = new StringBuilder();
        }

        public bool _ClearBodyBase;
        public StringBuilder _BodyAddition;
        public SideNavigationTarget _BodyTarget;  

        public void Add(string html, bool clearBaseTemplateBody, SideNavigationTarget target)
        {
            if (clearBaseTemplateBody)
                Clear(clearBaseTemplateBody);

            if (this._BodyAddition == null)
                this._BodyAddition = new StringBuilder();

            this._BodyTarget = target;
            this._BodyAddition.Append(html);
        }
    }

    public class FormElements
    {
        public string GetHeaderStyle()
        {
            if (_Input == null && _Select == null)
                return string.Empty;
            StringBuilder html = new StringBuilder();
            html.Append("<style>");
            if (this.Input.Long.Width.HasValue)
                html.Append(string.Concat(" input.long { width: ", this.Input.Long.Width.Value, "px !important; }"));
            if (this.Input.Short.Width.HasValue)
                html.Append(string.Concat(" input.short { width: ", this.Input.Short.Width.Value, "px !important; }"));
            if (this.Select.Long.Width.HasValue)
                html.Append(string.Concat(" select.long { width: ", this.Select.Long.Width.Value, "px !important; }"));
            if (this.Select.Short.Width.HasValue)
                html.Append(string.Concat(" select.short { width: ", this.Select.Short.Width.Value, "px !important; }"));
            html.Append("</style>");
            return html.ToString();
        }

        FormElementType _Input;
        public FormElementType Input
        {
            get { if (_Input == null) _Input = new FormElementType(); return _Input; }
            set { _Input = value; }
        }
        FormElementType _Select;
        public FormElementType Select
        {
            get { if (_Select == null) _Select = new FormElementType(); return _Select; }
            set { _Select = value; }
        }
    }

    public class FormElementType
    {
        FormElementDesignType _Long;
        public FormElementDesignType Long
        {
            get { if (_Long == null) _Long = new FormElementDesignType(); return _Long; }
            set { _Long = value; }
        }
        FormElementDesignType _Short;
        public FormElementDesignType Short
        {
            get { if (_Short == null) _Short = new FormElementDesignType(); return _Short; }
            set { _Short = value; }
        }
    }

    public class FormElementDesignType
    {
        public int? Width { get;set; }
    }

    public class Form
    {
        FormElements _Elements;
        public FormElements Elements
        {
            get { if (_Elements == null) _Elements = new FormElements(); return _Elements; }
            set { _Elements = value; }
        }

        /// <summary>
        /// Disable all form input fields
        /// </summary>
        /// <value>
        /// The disable input.
        /// </value>
        public bool DisableInput { get; set; }
        /// <summary>
        /// Assigns the primairy action.
        /// </summary>
        /// <param name="buttonPropertyName">Name of the button property.</param>
        /// <returns></returns>
        internal string _PrimairyAction;
        public void AssignPrimairyAction(string buttonPropertyName)
        {
            _PrimairyAction = buttonPropertyName;
        }

        /// <summary>
        /// Refreshes the parent form (from within a layer)
        /// </summary>
        public void RefreshParent(string url = null)
        {
            if (this._instance.OnSaveScript == null)
                this._instance.OnSaveScript = string.Empty;

            this._instance.OnSaveScript = $"<input type=\"hidden\" class=\"postParent\" data-url=\"{url}\" />";
        }

        /// <summary>
        /// Closes the layer (from within a layer) 
        /// </summary>
        public void CloseLayer()
        {
            if (this._instance.OnSaveScript == null)
                this._instance.OnSaveScript = string.Empty;

            this._instance.OnSaveScript = string.Format(@"<input type=""hidden"" class=""closeLayer"" />");
        }

        /// <summary>
        /// Posts the layer information to the parent container (Note, this is extracted from the querystring "referid" or the set dataTarget)
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="value">The value.</param>
        /// <param name="canAddMultiple">Should to be added or replace the existing value?</param>
        /// <param name="dataTarget">The target, if not set this will be taken from the querystring (referID), this is the ID of the field</param>
        public void PostDataToSubSelect(int id, string value, bool canAddMultiple = false, string dataTarget = null)
        {
            PostDataToSubSelect(id.ToString(), value, canAddMultiple, dataTarget);
        }

        /// <summary>
        /// Posts the layer information to the parent container (Note, this is extracted from the querystring "referid" or the set dataTarget)
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="value">The value.</param>
        /// <param name="canAddMultiple">Should to be added or replace the existing value?</param>
        /// <param name="dataTarget">The target, if not set this will be taken from the querystring (referID), this is the ID of the field</param>
        public void PostDataToSubSelect(string id, string value, bool canAddMultiple = false, string dataTarget = null)
        {
            if (this._instance.OnSaveScript == null)
                this._instance.OnSaveScript = string.Empty;

            this._instance.OnSaveScript 
                += string.Format(@"<input type=""hidden"" class=""postparent""{2}{3} id=""{0}"" value=""{1}"" />"
                                    , id
                                    , value
                                    , canAddMultiple ? @" data-multiple=""1""" : null
                                    , dataTarget != null ? string.Format(@" data-target=""{0}""", dataTarget) : null
                                    );
        }

        /// <summary>
        /// Determines whether the button is of type Primairy action
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns></returns>
        public bool IsPrimairyAction(Framework.ContentListItem.ButtonAttribute button)
        {
            if (button.IsPrimary)
            {
                return true;
            }

            if (!string.IsNullOrEmpty(button.ID))
            {
                if (button.ID == _PrimairyAction)
                    return true;
            }
            return false;
        }

        Sushi.Mediakiwi.Framework.WimComponentListRoot _instance;
        public Form(Sushi.Mediakiwi.Framework.WimComponentListRoot instance)
        {
            _instance = instance;
        }
    }

    public class Table
    {
        /// <summary>
        /// Gets or sets a value indicating whether to ignore table cell creation.
        /// </summary>
        /// <value>
        /// <c>true</c> if [ignore table cell creation]; otherwise, <c>false</c>.
        /// </value>
        public bool IgnoreCreation { get; set; }
        public string ClassName { get; set; }


        Sushi.Mediakiwi.Framework.WimComponentListRoot _instance;
        public Table(WimComponentListRoot instance)
        {
            _instance = instance;
            this.ClassName = "selections";
        }
    }

    public class Grid
    {
        internal Table _Table;
        /// <summary>
        /// Define the preloader
        /// </summary>
        public Table Table
        {
            get
            {
                if (_Table == null)
                    _Table = new Table(_instance);
                return _Table;
            }
            set
            {
                _Table = value;
            }

        }

        internal bool _ClearGridBase = false;
        internal StringBuilder _GridAddition;

        Sushi.Mediakiwi.Framework.WimComponentListRoot _instance;
        public Grid(WimComponentListRoot instance)
        {
            _instance = instance;
            this.ClassName = "searchTable";
        }

        public class LayerSpecification
        {
            public LayerSpecification()
            {
            }
            public LayerSpecification(LayerSize defaultSize)
            {
                Apply(defaultSize);
            }

            /// <summary>
            /// Parse the data-layer tag
            /// </summary>
            /// <param name="includeHtmlTag">When true the data-layer tag is added (with a space in front)</param>
            /// <returns></returns>
            public string Parse(bool includeHtmlTag = false)
            {
                string properties = string.Empty;

                if (this.Width.HasValue)
                    properties += string.Format("{2}width:{0}{1}", this.Width.Value
                        , (this.IsWidthPercentage ? "%" : "px")
                        , (properties.Length > 0 ? "," : string.Empty));

                if (this.Height.HasValue)
                    properties += string.Format("{2}height:{0}{1}", this.Height.Value
                        , (this.IsHeightPercentage ? "%" : "px")
                        , (properties.Length > 0 ? "," : string.Empty));

                if (this.InFrame.HasValue)
                    properties += string.Format("{1}iframe:{0}", this.InFrame.Value.ToString().ToLower(), (properties.Length > 0 ? "," : string.Empty));

                if (this.HasScrolling.HasValue)
                    properties += string.Format("{1}scrolling:{0}", this.HasScrolling.Value.ToString().ToLower(), (properties.Length > 0 ? "," : string.Empty));

                if (!string.IsNullOrEmpty(this.Class))
                    properties += string.Format("{1}class:{0}", this.Class
                        , (properties.Length > 0 ? "," : string.Empty));

                if (!string.IsNullOrEmpty(properties))
                {
                    if (includeHtmlTag)
                    {
                        if (string.IsNullOrEmpty(Title))
                            return string.Format(" data-layer=\"{0}\"", properties);
                        else
                            return string.Format(" data-layer=\"{0}\" data-title=\"{1}\"", properties, Title);
                    }
                    return properties;
                }
                return null;
            }

            public string Title { get; set; }

            public bool? InFrame { get; set; }
            public bool? HasScrolling { get; set; }
            public int? Width { get; set; }
            public int? Height { get; set; }
            public bool IsHeightPercentage { get; set; }
            public bool IsWidthPercentage { get; set; }
            public string Url { get; set; }
            public string Class { get; set; }
            /// <summary>
            /// Applies the specified default size.
            /// </summary>
            /// <param name="defaultSize">The default size.</param>
            public void Apply(LayerSize defaultSize)
            {
                switch (defaultSize)
                {
                    case LayerSize.Normal:
                        InFrame = true;
                        Width = 790;
                        Height = 90;
                        IsHeightPercentage = true;
                        HasScrolling = true;
                        break;
                    case LayerSize.Small:
                        InFrame = true;
                        Width = 790;
                        Height = 414;
                        HasScrolling = true;
                        break;
                    case LayerSize.Tiny:
                        InFrame = true;
                        Width = 472;
                        Height = 314;
                        break;
                }
            }
        }

        string _GetClickLayerSpecification = null;
 
        public class PreLoaderData
        {
            /// <summary>
            /// If [True] hide the preloader
            /// </summary>
            public bool Hide { get; set; }
            //  Overrule the preloader HTML
            public string HTML { get; set; }
        }

        internal PreLoaderData _PreLoader;
        /// <summary>
        /// Define the preloader
        /// </summary>
        public PreLoaderData PreLoader
        {
            get
            {
                if (_PreLoader == null)
                    _PreLoader = new PreLoaderData();
                return _PreLoader;
            }
            set
            {
                _PreLoader = value;
            }

        }
       
        /// <summary>
        /// When in layer mode, ignore the sublist selection mode for the grid.
        /// </summary>
        public bool IgnoreInLayerSubSelect { get; set; }
        public bool HidePager { get; set; }

        /// <summary>
        /// Gets or sets the name of the class that is placed on the section.
        /// </summary>
        /// <value>
        /// The name of the class.
        /// </value>
        public string ClassName { get; set; }
        internal string ClickLayerTag
        {
            get
            {
                if (_GetClickLayerSpecification == null)
                {
                    if (_instance.CurrentList.Option_LayerResult)
                        SetClickLayer(new LayerSpecification(LayerSize.Normal));
                    else
                        return null;
                }
                return _specification.Parse(true);
                //return string.Format(" data-layer=\"{0}\"", _GetClickLayerSpecification);
            }
        }
        internal string ClickLayerClass
        {
            get
            {
                if (_GetClickLayerSpecification == null)
                {
                    if (_instance.CurrentList.Option_LayerResult)
                        SetClickLayer(new LayerSpecification(LayerSize.Normal));
                    else
                        return null;
                }
                return " openlayer";
            }
        }
        public void SetClickLayer(LayerSpecification specification)
        {
            _specification = specification;
            _GetClickLayerSpecification = specification.Parse();
        }

        LayerSpecification _specification;

        /// <summary>
        /// Clears the specified clear base template header.
        /// </summary>
        /// <param name="clearBaseTemplateBody">if set to <c>true</c> [clear base template body].</param>
        public void Clear(bool clearBaseTemplateGrid)
        {
            this._ClearGridBase = clearBaseTemplateGrid;

            if (clearBaseTemplateGrid)
                return;

            if (this._GridAddition != null)
                this._GridAddition = new StringBuilder();
        }

        public void Add(string html)
        {
            Add(html, false);
        }

        public void Add(string html, bool clearBaseTemplateGrid)
        {
            if (clearBaseTemplateGrid)
                Clear(clearBaseTemplateGrid);

            if (this._GridAddition == null)
                this._GridAddition = new StringBuilder();

            this._GridAddition.Append(html);
        }
    }

    public class Head
    {
        internal bool _ClearHeadBase = false;
        internal StringBuilder _HeadAddition;

        WimComponentListRoot _root;
        Sushi.Mediakiwi.Framework.WimComponentListRoot.PageData _instance;
        public Head(Sushi.Mediakiwi.Framework.WimComponentListRoot.PageData instance, WimComponentListRoot root)
        {
            _root = root;
            _instance = instance;
        }

        /// <summary>
        /// Adds a script element to the page header
        /// </summary>
        /// <param name="path">relative path to the file</param>
        public void AddScript(string path, bool appendApplicationPath = true)
        {
            string _path = (appendApplicationPath) ? _root.AddApplicationPath(path) : path;

            if (string.IsNullOrWhiteSpace(CommonConfiguration.FILE_VERSION))
            {
                Add($"<script type=\"text/javascript\" src=\"{_path}\"></script>");
                return;
            }

            Add($"<script type=\"text/javascript\" src=\"{_path}?v={CommonConfiguration.FILE_VERSION}\"></script>");
        }

        /// <summary>
        /// Adds a style element to the page header
        /// </summary>
        /// <param name="path">relative path to the file</param>
        /// <param name="appendApplicationPath">when false the application path will not be added to the path param</param>
        public void AddStyle(string path, bool appendApplicationPath = true)
        {
            string fileVersion = CommonConfiguration.FILE_VERSION;
            string _path = (appendApplicationPath) ? _root.AddApplicationPath(path) : path;
            Add($"<link rel=\"stylesheet\" href=\"{_path}?v={fileVersion}\" type=\"text/css\" media=\"all\" />");
        }

        /// <summary>
        /// Clears the specified clear base template header.
        /// </summary>
        /// <param name="clearBaseTemplateHeader">if set to <c>true</c> [the all header entries in the standard template].</param>
        public void Clear(bool clearBaseTemplateHeader)
        {
            this._ClearHeadBase = clearBaseTemplateHeader;

            if (this._HeadAddition != null)
                this._HeadAddition = new StringBuilder();
        }

        public bool EnableColorCodingLibrary { get; set; }

        /// <summary>
        /// Adds the specified HTML.
        /// </summary>
        /// <param name="html">The HTML.</param>
        public void Add(string html)
        {
            if (this._HeadAddition == null)
                this._HeadAddition = new StringBuilder();
            this._HeadAddition.Append(html);
        }

        Uri _Logo;
        internal Uri Logo
        {
            get
            {
                //if (_Logo == null && _root != null && _root.CurrentEnvironment != null)
                //    return new Uri(_root.CurrentEnvironment.LogoHrefFull, UriKind.RelativeOrAbsolute);
                return _Logo;
            }
        }

        public void SetLogo(Uri logo)
        {
            _Logo = logo;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class GridDataItemAttribute
    {
        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="column">The column.</param>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public string ToString(object value, Framework.ListDataColumn column)
        {
            string candidate = value == null ? string.Empty : value.ToString();
            
            if (!string.IsNullOrEmpty(candidate))
                candidate = string.Concat(column.ColumnValuePrefix, candidate, column.ColumnValueSuffix);

            if (_type == DataItemType.TableRow && IsRowSortingDisabled)
            {
                if (string.IsNullOrEmpty(this.Class))
                    this.Class = "nosort";
                else
                    this.Class += " nosort";
            }

            if (column.ColumnIsFixed)
            {
                if (!string.IsNullOrEmpty(candidate))
                    candidate = string.Concat("<abbr>", candidate, "</abbr>");
                else
                    candidate = "&nbsp;";

                if (string.IsNullOrEmpty(this.Class))
                    this.Class = "fixed";
                else
                    this.Class += " fixed";

                //  Add the needed width (position absolute)
                this.Add("width", column.SuggestedColumnLength.ToString());

                if (column.ColumnFixedLeftMargin > 0)
                {
                    this.Style.Add("margin-left", string.Format("{0}px", column.ColumnFixedLeftMargin));
                }
            }
            else
            {
                if (column.ShrinkTextWhenToLarge)
                {
                    if (!string.IsNullOrEmpty(candidate))
                        candidate = string.Concat("<abbr>", candidate, "</abbr>");

                    if (column.SuggestedColumnLength > 0)
                        this.Add("width", column.SuggestedColumnLength.ToString());
                }
            }

            if (column.ColumnHeight > 0)
            {
                if (_style == null)
                    _style = new StyleAttribute();

                _style.Add("height", string.Format("{0}px", column.ColumnHeight));
            }

            string html = string.Empty;
            if (_arr != null)
            {
                foreach(var key in _arr.Keys)
                {
                    if (_arr[key] != null)
                    {
                        html += string.Format(" {0}=\"{1}\"", key, _arr[key]);
                    }
                }
            }

            if (_style != null)
               html += string.Format(" {0}=\"{1}\"", "style", _style.ToString());

            if (_type == DataItemType.TableCell)
            {
                return string.Format("<td{0}>{1}</td>", html, candidate);
            }
            if (_type == DataItemType.TableRow)
            {
                if (IsRowSortingDisabled)
                {

                }

                return string.Format("{1}<tr{0}>", html, candidate);
            }
            return null;
        }

        Hashtable _arr;
        DataItemType _type;
        /// <summary>
        /// Initializes a new instance of the <see cref="GridDataItemAttribute"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public GridDataItemAttribute(DataItemType type)
        {
            _type = type;
        }

        /// <summary>
        /// Adds the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void Add(string name, string value)
        {
            this[name] = value;
        }

        string m_Attribute;
        /// <summary>
        /// Gets or sets the <see cref="string"/> with the specified name.
        /// </summary>
        /// <value>
        /// The <see cref="string"/>.
        /// </value>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">
        /// Please use the Style object
        /// or
        /// Please use the Style object
        /// </exception>
        public string this[string name]
        {
            get
            {
                if (name == "style") 
                    throw new Exception("Please use the Style object");

                name = name.ToLower();
                if (_arr == null) _arr = new Hashtable();

                if (_arr.ContainsKey(name))
                    return _arr[name].ToString();
                return null;
            }
            set
            {
                if (name == "style") 
                    throw new Exception("Please use the Style object");

                name = name.ToLower();
                if (_arr == null) _arr = new Hashtable();

                if (_arr.ContainsKey(name))
                    _arr[name] = value;
                else
                    _arr.Add(name, value);          
            }
        }

        StyleAttribute _style;
        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        /// <value>
        /// The style.
        /// </value>
        public StyleAttribute Style
        {
            get { if (_style == null) _style = new StyleAttribute(); return _style; }
            set { _style = value; }
        }

        ///// <summary>
        ///// Gets or sets the align.
        ///// </summary>
        ///// <value>
        ///// The align.
        ///// </value>
        //public string Align
        //{
        //    get { return this["align"]; }
        //    set { this["align"] = value; }
        //}

        /// <summary>
        /// Gets or sets a value indicating whether this instance is row sorting disabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is row sorting disabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsRowSortingDisabled { get; set; }

        /// <summary>
        /// Gets or sets the class.
        /// </summary>
        /// <value>
        /// The class.
        /// </value>
        public string Class
        {
            get { return this["class"]; }
            set { this["class"] = value; }
        }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>
        /// The ID.
        /// </value>
        public string ID
        {
            get { return this["id"]; }
            set { this["id"] = value; }
        }
    }

    public class StyleAttribute
    {
        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            string html = string.Empty;
            if( _arr!= null)
            {
                foreach(var key in _arr.Keys)
                {
                    if (_arr[key] == null)
                        continue;

                    if (!string.IsNullOrEmpty(html))
                        html += ";";
                    html += string.Format("{0}:{1}", key, _arr[key]);
                }
            }
            return html;
        }

        Hashtable _arr;
        /// <summary>
        /// Initializes a new instance of the <see cref="StyleAttribute"/> class.
        /// </summary>
        public StyleAttribute()
        {
        }

        /// <summary>
        /// Adds the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void Add(string name, string value)
        {
            this[name] = value;
        }

        string m_Attribute;
        /// <summary>
        /// Gets or sets the <see cref="string"/> with the specified name.
        /// </summary>
        /// <value>
        /// The <see cref="string"/>.
        /// </value>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public string this[string name]
        {
            get
            {
                if (_arr == null) _arr = new Hashtable();

                if (_arr.ContainsKey(name))
                    return _arr[name].ToString();
                return null;
            }
            set
            {
                if (_arr == null) _arr = new Hashtable();

                if (_arr.ContainsKey(name))
                    _arr[name] = value;
                else
                    _arr.Add(name, value);          
            }
        }

        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>
        /// The color of the background.
        /// </value>
        public string BackgroundColor
        {
            get { return this["background-color"]; }
            set { this["background-color"] = value; }
        }

        public string Color
        {
            get { return this["color"]; }
            set { this["color"] = value; }
        }

/// <summary>
/// Gets or sets the apply background highlight.
/// </summary>
/// <value>
/// The apply background highlight.
/// </value>
        public void ApplyBackgroundHighlight()
        {
           this.BackgroundColor = "#ededed";
        }
    }


    /// <summary>
    /// The evensts
    /// </summary>
    public partial class WimComponentListRoot
    {
        HttpContext Context;
        public void Init(HttpContext context)
        {
            Context = context;
        }

        public bool ByPassAjaxRequest { set { _origin.ByPassAjaxRequest = value; } get { return _origin.ByPassAjaxRequest; } }
        internal void DoListLoad(int selectedKey, int componentVersionKey)
        {
            int previousItemID = Utility.ConvertToInt(Context.Request.Query["pitem"]);
            DoListLoad(selectedKey, componentVersionKey, previousItemID, null);
        }
        /// <summary>
        /// Does the list load.
        /// </summary>
        /// <param name="selectedKey">The selected key.</param>
        /// <param name="componentVersionKey">The component version key.</param>
        /// <param name="previousSelectedKey"></param>
        internal void DoListLoad(int selectedKey, int componentVersionKey, int previousSelectedKey, bool? isValidForm)
        {
            _origin.ApplyListSettings();

            int groupID = Utility.ConvertToInt(Context.Request.Query["group"]);
            int groupItemID = Utility.ConvertToInt(Context.Request.Query["groupitem"]);

            long start = DateTime.Now.Ticks;

            Utils.RunSync(() => _origin.OnListLoad(new ComponentListEventArgs(selectedKey, previousSelectedKey, componentVersionKey, groupID, groupItemID, isValidForm)));
        }

        internal bool HasListLoad
        {
            get { return _origin.HasListLoad;  }
        }
        internal void DoListAction(int selectedKey, int componentVersionKey, string propertyName, bool? isValidForm)
        {
            int groupID = Utility.ConvertToInt(Context.Request.Query["group"]);
            int groupItemID = Utility.ConvertToInt(Context.Request.Query["groupitem"]);

            long start = DateTime.Now.Ticks;
            Utils.RunSync(() => _origin.OnListAction(new ComponentActionEventArgs(selectedKey, componentVersionKey, propertyName, groupID, groupItemID, isValidForm)));
        }
        internal bool HasListAction
        {
            get { return _origin.HasListAction; }
        }
   
        internal void DoListPreRender(int selectedKey, int componentVersionKey, bool? isValidForm)
        {
            int groupID = Utility.ConvertToInt(Context.Request.Query["group"]);
            int groupItemID = Utility.ConvertToInt(Context.Request.Query["groupitem"]);


            long start = DateTime.Now.Ticks;

            Utils.RunSync(() => _origin.OnListPreRender(new ComponentListEventArgs(selectedKey, componentVersionKey, 0, groupID, groupItemID, isValidForm)));
        }
        internal bool HasListPreRender
        {
            get { return _origin.HasListPreRender; }
        }
        internal void DoListSave(int selectedKey, int componentVersionKey, bool? isValidForm)
        {
            int groupID = Utility.ConvertToInt(Context.Request.Query["group"]);
            int groupItemID = Utility.ConvertToInt(Context.Request.Query["groupitem"]);
            int previousItemID = Utility.ConvertToInt(Context.Request.Query["pitem"]);

            long start = DateTime.Now.Ticks;

            if (Context?.Items["wim.Saved.ID"] != null)
                AfterSaveElementIdentifier = null;

            Utils.RunSync(() => _origin.OnListSave(new ComponentListEventArgs(selectedKey, previousItemID, componentVersionKey, groupID, groupItemID, isValidForm)));
            //Sushi.Mediakiwi.Framework2.Functions.AuditTrail.Insert(wim.CurrentApplicationUser, wim.CurrentList, selectedKey, selectedKey == 0 ? Sushi.Mediakiwi.Framework2.Functions.Auditing.ActionType.Add : Sushi.Mediakiwi.Framework2.Functions.Auditing.ActionType.Update, componentVersionKey);
        }
        internal bool HasListSave
        {
            get { return _origin.HasListSave; }
        }
        internal void DoListDelete(int selectedKey, int componentVersionKey, bool? isValidForm)
        {
            int groupID = Utility.ConvertToInt(Context.Request.Query["group"]);
            int groupItemID = Utility.ConvertToInt(Context.Request.Query["groupitem"]);

            long start = DateTime.Now.Ticks;

            Utils.RunSync(() => _origin.OnListDelete(new ComponentListEventArgs(selectedKey, componentVersionKey, 0, groupID, groupItemID, isValidForm)));

            //Sushi.Mediakiwi.Framework2.Functions.AuditTrail.Insert(wim.CurrentApplicationUser, wim.CurrentList, selectedKey, Sushi.Mediakiwi.Framework2.Functions.Auditing.ActionType.Remove, componentVersionKey);
        }
        internal bool HasListDelete
        {
            get { return _origin.HasListDelete; }
        }
        internal void DoListSearch()
        {
            _origin.ApplyListSettings();

            if (!_origin.IsPostBack && CurrentList.Data["wim_PostbackSearch"].ParseBoolean())
            {
                if (Context != null)
                {
                    var item = Context.Request.Query["item"];

                    var set = Context.Request.Query["set"];
                    if (set == StringValues.Empty && item == StringValues.Empty)
                        return;
                }
                else
                    return;
            }

            var itemID = Utility.ConvertToIntNullable(Context.Request.Query["item"], false);
            int groupID = Utility.ConvertToInt(Context.Request.Query["group"]);
            int groupItemID = Utility.ConvertToInt(Context.Request.Query["groupitem"]);

            long start = DateTime.Now.Ticks;

            if (!itemID.HasValue)
            {
                Page.HideTabs = true;

                //  SearchAsync should only work on toplevel lists, not inner lists (8/11/25)
                if ((CurrentList.Option_SearchAsync && !IsDashboardMode) && !_origin.IsFormatRequest_AJAX)
                    return;
            }

            if (_origin.wim._Grids == null)
            {
                // should be replaced with await (async all the way).
                Utils.RunSync(() => _origin.OnListSearch(new ComponentListSearchEventArgs(itemID.GetValueOrDefault(), groupID, groupItemID)));
            }
            //Context.Trace.Write("Wim.Event", "Begin ListSearch");
        }
        internal bool HasListSearch
        {
            get { return _origin.HasListSearch; }
        }
        internal ListDataItemCreatedEventArgs DoListDataItemCreated(
            DataItemType type,
            Framework.ListDataColumn[] columns,
            Framework.ListDataColumn column,
            object item,
            string itemKey,
            object propertyValue,
            int index,
            GridDataItemAttribute attribute,
            ListDataSoure source
            )
        {
            var output = new ListDataItemCreatedEventArgs()
            {
                Type = type,
                ItemKey = Utility.ConvertToIntNullable(itemKey),
                Index = index,
                Item = item,
                InnerHTML = propertyValue == null ? null : propertyValue.ToString(),
                ColumnProperty = column == null ? null : column.ColumnValuePropertyName,
                Columns = columns,
                Column = column,
                Attribute = attribute,
                Source = source,
                NodeAttributeHTML = string.Format("id=\"id_{0}${1}\" {2}", CurrentList.ID, itemKey, Page.Body.Grid.ClickLayerTag)
            };

            if (HasListDataItemCreated)
                _origin.OnListDataItemCreated(output);
            return output;
        }
        internal bool HasListDataItemCreated
        {
            get { return _origin.HasListDataItemCreated; }
        }

        internal void DoListInit()
        {
            _origin.ApplyListSettings();

            long start = DateTime.Now.Ticks;
            Utils.RunSync(() => _origin.OnListInit());
        }

        internal void DoListSense()
        {
            _origin.ApplyListSettings();

            long start = DateTime.Now.Ticks;
            _origin.OnListSense();
        }

        /// <summary>
        /// Gets a value indicating whether this instance has list sense.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has list sense; otherwise, <c>false</c>.
        /// </value>
        internal bool HasListSense
        {
            get { return _origin.HasListSense; }
        }
        internal ComponentDataReportEventArgs DoListDataReport()
        {
            ComponentDataReportEventArgs e = new ComponentDataReportEventArgs();
            try
            {
                long start = DateTime.Now.Ticks;


                _origin.OnListDataReport(e);

                return e;
            }
            catch (Exception ex)
            {
                e.Exception = ex;
                return e;
            }
        }

        internal bool HasListDataReport
        {
            get { return _origin.HasListDataReport; }
        }

        internal ComponentAsyncEventArgs DoListAsync(ComponentAsyncEventArgs eventArgs)
        {
            _origin.ApplyListSettings();

            long start = DateTime.Now.Ticks;

            _origin.OnListAsync(eventArgs);

            return eventArgs;
        }
        internal bool HasListAsync
        {
            get { return _origin.HasListAsync; }
        }

        /// <summary>
        /// When a accordionconfig is supplied the list search is acting as an accordion UI element (example http://api.jqueryui.com/accordion/#entry-examples)
        /// The contents of the item will be loaded async 
        /// The rows won't click through a detail item url, instead it will be shown in the list search 
        /// </summary>
        //public AccordionConfig SearchListIsAccordion { get; set; }

        /// <summary>
        /// When true the system keeps the list search filters for this instance of the list
        /// </summary>        
        public bool HasOwnSearchListCache { get; set; }
        public bool IsClearingListCache { get; set; }
        /// <summary>
        /// When HasOwnSearchListCache=true, this method will clear the specific cache
        /// </summary>
        /// <returns>True when clear has been done, false if the cache was not possible to clear</returns>
        public bool ClearOwnListSearchCache()
        {
            if (HasOwnSearchListCache)
            {
                IsClearingListCache = true;
                CurrentVisitor.Data.Apply("wim_FilterInfo_" + CurrentList.GUID.ToString(), null); 
                CurrentVisitor.Save();

                return true;
            }
            return false;
        }
    }



    /// <summary>
    /// 
    /// </summary>
    public partial class WimComponentListRoot
    {
    

        /// <summary>
        /// 
        /// </summary>
        internal class StateTypeItem
        {
            /// <summary>
            /// Gets or sets the assigned property.
            /// </summary>
            /// <value>The assigned property.</value>
            public string AssignedProperty { get; set; }
            /// <summary>
            /// Gets or sets the validation property.
            /// </summary>
            /// <value>The validation property.</value>
            public string ValidationProperty { get; set; }
            /// <summary>
            /// Gets or sets a value indicating whether this <see cref="EditVisibleItem"/> is state.
            /// </summary>
            /// <value><c>true</c> if state; otherwise, <c>false</c>.</value>
            public bool State { get; set; }
        }

        /// <summary>
        /// Determines whether the specified sender is visible.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="listContainer">The list container.</param>
        /// <param name="property">The property.</param>
        /// <param name="currentState">if set to <c>true</c> [current state].</param>
        /// <returns>
        ///   <c>true</c> if the specified sender is visible; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsVisible(object sender, object listContainer, string property, bool currentState)
        {
            if (VisibilityItemList != null)
            {
                foreach (StateTypeItem item in VisibilityItemList)
                {
                    if (item.AssignedProperty == property)
                    {
                        if (item.ValidationProperty == null)
                            return item.State;
                        
                        foreach (System.Reflection.PropertyInfo p in sender.GetType().GetProperties())
                        {
                            if (p.Name == item.ValidationProperty)
                            {
                                bool isTrue = (bool)p.GetValue(sender, null);
                                if (!item.State)
                                    return !isTrue;
                                return isTrue;
                            }
                        }
                        if (listContainer != null && !sender.GetType().Equals(listContainer.GetType()))
                        {
                            foreach (System.Reflection.PropertyInfo p in listContainer.GetType().GetProperties())
                            {
                                if (p.Name == item.ValidationProperty)
                                {
                                    bool isTrue = (bool)p.GetValue(listContainer, null);
                                    if (!item.State)
                                        return !isTrue;
                                    return isTrue;
                                }
                            }
                        }
                    }
                }
            }
            return currentState;
        }
        /// <summary>
        /// Determines whether [is edit visible item true] [the specified sender].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="listContainer">The list container.</param>
        /// <param name="isVisibilityItem">if set to <c>true</c> [is visibility item].</param>
        /// <param name="property">The property.</param>
        /// <param name="currentState">if set to <c>true</c> [current state].</param>
        /// <returns>
        ///   <c>true</c> if [is edit visible item true] [the specified sender]; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsEditable(object sender, object listContainer, string property, bool currentState)
        {
            if (property == "Title")
            {
                //this.Console.Context.Response.WriteAsync(string.Format("<!-- infoItem1: {0} -->\n", currentState));
            }

            if (EditableItemList != null)
            {
                foreach (StateTypeItem item in EditableItemList)
                {
                    if (item.AssignedProperty == property)
                    {
                        if (item.ValidationProperty == null)
                            return item.State;

                        foreach (System.Reflection.PropertyInfo p in sender.GetType().GetProperties())
                        {
                            if (p.Name == item.ValidationProperty)
                            {
                                bool isTrue = (bool)p.GetValue(sender, null);
                                if (!item.State)
                                    return !isTrue;
                                return isTrue;
                            }
                        }
                        if (listContainer != null && !sender.GetType().Equals(listContainer.GetType()))
                        {
                            foreach (System.Reflection.PropertyInfo p in listContainer.GetType().GetProperties())
                            {
                                if (p.Name == item.ValidationProperty)
                                {
                                    bool isTrue = (bool)p.GetValue(listContainer, null);
                                    if (!item.State)
                                        return !isTrue;
                                    return isTrue;
                                }
                            }
                        }
                    }
                }
            }
            return currentState;
        }
        /// <summary>
        /// Determines whether the specified sender is required.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="listContainer">The list container.</param>
        /// <param name="isVisibilityItem">if set to <c>true</c> [is visibility item].</param>
        /// <param name="property">The property.</param>
        /// <param name="currentState">if set to <c>true</c> [current state].</param>
        /// <returns>
        ///   <c>true</c> if the specified sender is required; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsRequired(object sender, object listContainer, string property, bool currentState)
        {
            if (RequiredItemList != null)
            {
                foreach (StateTypeItem item in RequiredItemList)
                {
                    if (item.AssignedProperty == property)
                    {
                        if (item.ValidationProperty == null)
                            return item.State;

                        foreach (System.Reflection.PropertyInfo p in sender.GetType().GetProperties())
                        {
                            if (p.Name == item.ValidationProperty)
                            {
                                bool isTrue = (bool)p.GetValue(sender, null);
                                if (!item.State)
                                    return !isTrue;
                                return isTrue;
                            }
                        }
                        if (listContainer != null && !sender.GetType().Equals(listContainer.GetType()))
                        {
                            foreach (System.Reflection.PropertyInfo p in listContainer.GetType().GetProperties())
                            {
                                if (p.Name == item.ValidationProperty)
                                {
                                    bool isTrue = (bool)p.GetValue(listContainer, null);
                                    if (!item.State)
                                        return !isTrue;
                                    return isTrue;
                                }
                            }
                        }
                    }
                }
            }
            return currentState;
        }

        internal List<StateTypeItem> VisibilityItemList;
        /// <summary>
        /// Sets the property visibility.
        /// </summary>
        /// <param name="assignedProperty">The assigned property.</param>
        /// <param name="state">if set to <c>true</c> [state].</param>
        public void SetPropertyVisibility(string assignedProperty, bool state)
        {
            SetPropertyVisibility(assignedProperty, null, state);
        }
        /// <summary>
        /// Sets the property visibility.
        /// </summary>
        /// <param name="assignedProperty">The assigned property.</param>
        /// <param name="validationProperty">The validation property (this should always return a Boolean.</param>
        public void SetPropertyVisibility(string assignedProperty, string validationProperty)
        {
            SetPropertyVisibility(assignedProperty, validationProperty, true);
        }
        /// <summary>
        /// Sets the property visibility.
        /// </summary>
        /// <param name="assignedProperty">The assigned property.</param>
        /// <param name="validationProperty">The validation property (this should always return a Boolean.</param>
        /// <param name="state">if set to <c>true</c> [state].</param>
        public void SetPropertyVisibility(string assignedProperty, string validationProperty, bool state)
        {

            if (VisibilityItemList == null)
                VisibilityItemList = new List<StateTypeItem>();

            StateTypeItem item = (from x in VisibilityItemList where x.AssignedProperty == assignedProperty select x).FirstOrDefault();
            if (item == null)
            {
                item = new StateTypeItem();
                VisibilityItemList.Add(item);
            }

            item.AssignedProperty = assignedProperty;
            item.ValidationProperty = validationProperty;
            item.State = state;
        }
        internal List<StateTypeItem> RequiredItemList;
        /// <summary>
        /// Sets the property required.
        /// </summary>
        /// <param name="assignedProperty">The assigned property.</param>
        /// <param name="state">if set to <c>true</c> [state].</param>
        public void SetPropertyRequired(string assignedProperty, bool state)
        {
            SetPropertyRequired(assignedProperty, null, state);
        }
        /// <summary>
        /// Sets the property required.
        /// </summary>
        /// <param name="assignedProperty">The assigned property.</param>
        /// <param name="validationProperty">The validation property.</param>
        public void SetPropertyRequired(string assignedProperty, string validationProperty)
        {
            SetPropertyRequired(assignedProperty, validationProperty, true);
        }
        /// <summary>
        /// Sets the property required.
        /// </summary>
        /// <param name="assignedProperty">The assigned property.</param>
        /// <param name="validationProperty">The validation property.</param>
        /// <param name="state">if set to <c>true</c> [state].</param>
        void SetPropertyRequired(string assignedProperty, string validationProperty, bool state)
        {
            if (RequiredItemList == null)
                RequiredItemList = new List<StateTypeItem>();

            StateTypeItem item = (from x in RequiredItemList where x.AssignedProperty == assignedProperty select x).FirstOrDefault();
            if (item == null)
                item = new StateTypeItem();

            item.AssignedProperty = assignedProperty;
            item.ValidationProperty = validationProperty;
            item.State = state;
            RequiredItemList.Add(item);
        }
        internal List<StateTypeItem> EditableItemList;
        /// <summary>
        /// Sets the property editable.
        /// </summary>
        /// <param name="assignedProperty">The assigned property.</param>
        /// <param name="state">if set to <c>true</c> [state].</param>
        public void SetPropertyEditable(string assignedProperty, bool state)
        {
            SetPropertyEditable(assignedProperty, null, state);
        }
        /// <summary>
        /// Sets the property editable.
        /// </summary>
        /// <param name="assignedProperty">The assigned property.</param>
        /// <param name="validationProperty">The validation property. (this should always return a Boolean).</param>
        public void SetPropertyEditable(string assignedProperty, string validationProperty)
        {
            SetPropertyEditable(assignedProperty, validationProperty, true);
        }
        /// <summary>
        /// Sets the property editable.
        /// </summary>
        /// <param name="assignedProperty">The assigned property.</param>
        /// <param name="validationProperty">The validation property. (this should always return a Boolean).</param>
        /// <param name="state">if set to <c>true</c> [state].</param>
        public void SetPropertyEditable(string assignedProperty, string validationProperty, bool state)
        {
            if (EditableItemList == null)
                EditableItemList = new List<StateTypeItem>();

            StateTypeItem item = (from x in EditableItemList where x.AssignedProperty == assignedProperty select x).FirstOrDefault();
            if (item == null)
                item = new StateTypeItem();
            
            item.AssignedProperty = assignedProperty;
            item.ValidationProperty = validationProperty;
            item.State = state;
            EditableItemList.Add(item);
        }

        /// <summary>
        /// Sends the report.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="user">The user.</param>
        /// <param name="subscription">The subscription.</param>
        //public void SendReport(IComponentListTemplate template, Sushi.Mediakiwi.Data.IApplicationUser user, Sushi.Mediakiwi.Data.Subscription subscription)
        //{
        //    this.Console = new Sushi.Mediakiwi.Beta.GeneratedCms.Console(Context, false);
        //    this.Console.CurrentListInstance = template;
        //    this.Console.CurrentList = template.wim.CurrentList;
        //    this.CurrentApplicationUser = user;
        //    this.CurrentSite = Sushi.Mediakiwi.Data.Site.SelectOne(subscription.SiteID);
        //    this.Console.View = 2;

        //    Sushi.Mediakiwi.Beta.GeneratedCms.Source.GridCreation grid = new Sushi.Mediakiwi.Beta.GeneratedCms.Source.GridCreation();
        //    Sushi.Mediakiwi.Beta.GeneratedCms.Source.Component component = new Sushi.Mediakiwi.Beta.GeneratedCms.Source.Component();

        //    string searchList = component.CreateSearchList(this.Console, 0, subscription.SetupXml).ToString();
        //    string searchListGrid = grid.GetGridFromListInstance(template.wim, template.wim.Console, 0, false, false);

        //    try
        //    {
        //        System.Net.Mail.MailAddress[] addresses = new MailAddress[] { new System.Net.Mail.MailAddress(user.Email, user.Displayname) };
        //        template.wim.SendReport(addresses, this.GraphUrl, subscription.Title, template.wim.CurrentList.Description, searchListGrid, string.Concat("wim.ashx?list=", template.wim.CurrentList.ID), MailPriority.Normal, -1);
        //    }
        //    catch (Exception ex)
        //    {
        //        Sushi.Mediakiwi.Data.Notification.InsertOne("SendReport", ex);
        //    }
        //}

        /// <summary>
        /// Sends the notification.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="title">The title.</param>
        /// <param name="body">The body.</param>
        /// <param name="url">The URL.</param>
        /// <param name="to">To.</param>
        public void SendNotification(string subject, string title, string body, string url, MailAddress to)
        {
            SendNotification(subject, title, body, url, to, false);
        }

        /// <summary>
        /// Sends the notification.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="title">The title.</param>
        /// <param name="body">The body.</param>
        /// <param name="url">The URL.</param>
        /// <param name="to">To.</param>
        /// <param name="hideHeader">if set to <c>true</c> [hide header].</param>
        public void SendNotification(string subject, string title, string body, string url, MailAddress to, bool hideHeader)
        {
            Utilities.Mail.Send(new Utilities.Mail.Payload()
            {
                Title = title,
                Subject = subject,
                Body = body,
                URL = url,
                To = new List<MailAddress>() { to },
                HideHeader = hideHeader
            }
            );
        }

        /// <summary>
        /// Sends the notification.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="title">The title.</param>
        /// <param name="body">The body.</param>
        /// <param name="url">The URL.</param>
        /// <param name="to">To.</param>
        public void SendNotification(string subject, string title, string body, string url, List<MailAddress> to)
        {
            SendNotification(subject, title, body, url, to, false);
        }

        /// <summary>
        /// Sends the notification.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="title">The title.</param>
        /// <param name="body">The body.</param>
        /// <param name="url">The URL.</param>
        /// <param name="to">To.</param>
        /// <param name="hideHeader">if set to <c>true</c> [hide header].</param>
        public void SendNotification(string subject, string title, string body, string url, List<MailAddress> to, bool hideHeader)
        {
            SendNotification(subject, title, body, url, to, false, false);
        }

        /// <summary>
        /// Sends the notification.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="title">The title.</param>
        /// <param name="body">The body.</param>
        /// <param name="url">The URL.</param>
        /// <param name="to">To.</param>
        /// <param name="hideHeader">if set to <c>true</c> [hide header].</param>
        /// <param name="replaceFullBody">if set to <c>true</c> [replace full body].</param>
        public void SendNotification(string subject, string title, string body, string url, List<MailAddress> to, bool hideHeader, bool replaceFullBody)
        {
            Utilities.Mail.Send(new Utilities.Mail.Payload()
            {
                Title = title,
                Subject = subject,
                Body = body,
                URL = url,
                To = to,
                HideHeader = hideHeader,
                ReplaceFullBody = replaceFullBody
            }
            );
        }

        /// <summary>
        /// Sends the report.
        /// </summary>
        /// <param name="to">To.</param>
        /// <param name="graphUrl">The graph URL.</param>
        /// <param name="title">The title.</param>
        /// <param name="description">The description.</param>
        public void SendReport(MailAddress to, string graphUrl, string title, string description)
        {
            SendReport(new MailAddress[] { to }, graphUrl, title, description);
        }

        /// <summary>
        /// Sends the report.
        /// </summary>
        /// <param name="to">To.</param>
        /// <param name="graphUrl">The graph URL.</param>
        /// <param name="title">The title.</param>
        /// <param name="description">The description.</param>
        /// <param name="data">The data.</param>
        //public void SendReport(MailAddress to, string graphUrl, string title, string description, string data)
        //{
        //    SendReport(new MailAddress[] { to }, graphUrl, title, description, data);
        //}

        /// <summary>
        /// Sends the report.
        /// </summary>
        /// <param name="to">To.</param>
        /// <param name="graphUrl">The graph URL.</param>
        /// <param name="title">The title.</param>
        /// <param name="description">The description.</param>
        /// <param name="data">The data.</param>
        //public void SendReport(MailAddress[] to, string graphUrl, string title, string description, string data)
        //{
        //    SendReport(to, graphUrl, title, description, data, Utility.GetSafeUrl(Console.Request), MailPriority.Normal, -1);
        //}

        /// <summary>
        /// Sends the report.
        /// </summary>
        /// <param name="to">To.</param>
        /// <param name="graphUrl">The graph URL.</param>
        /// <param name="title">The title.</param>
        /// <param name="description">The description.</param>
        /// <param name="data">The data.</param>
        /// <param name="relativeOnlineVersionPath">The relative online version path.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="followUpInHours">The follow up in hours.</param>
        //public void SendReport(MailAddress[] to, string graphUrl, string title, string description, string data, string relativeOnlineVersionPath, MailPriority priority, int followUpInHours)
        //{
        //    SendReport(to, graphUrl, title, description, data, Utility.GetSafeUrl(Console.Request), MailPriority.Normal, -1, title);
        //}

        /// <summary>
        /// Sends the report.
        /// </summary>
        /// <param name="to">To.</param>
        /// <param name="graphUrl">The graph URL.</param>
        /// <param name="title">The title.</param>
        /// <param name="description">The description.</param>
        /// <param name="data">The data.</param>
        /// <param name="relativeOnlineVersionPath">The relative online version path.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="followUpInHours">The follow up in hours.</param>
        //public void SendReport(MailAddress[] to, string graphUrl, string title, string description, string data, string relativeOnlineVersionPath, MailPriority priority, int followUpInHours, string subject)
        //{
        //    if (to.Length == 0) return;
        //    ReportTo = to;
        //    GraphUrl = graphUrl;
        //    ShouldSendReport = true;
        //    this.Title = string.IsNullOrEmpty(title) ? this.CurrentList.SingleItemName : title;
        //    this.Subject = subject;
        //    this.Priority = priority;
        //    this.FollowUpHours = followUpInHours;
        //    this.Description = description;
        //    this.RelativeOnlineVersionPath = relativeOnlineVersionPath;

        //    SendReport(data);
        //    ShouldSendReport = false;
        //}

        /// <summary>
        /// Sets the report graph data.
        /// </summary>
        /// <value>The report graph data.</value>
        public string ReportGraphData
        {
            set { this.GraphUrl = value; }
        }

        internal MailAddress[] ReportTo;
        internal string GraphUrl;
        internal bool ShouldSendReport;
        internal string Title;
        internal string Subject;
        internal string Description;
        internal MailPriority Priority = MailPriority.Normal;
        internal int FollowUpHours = -1;
        internal string RelativeOnlineVersionPath;

        /// <summary>
        /// Sends the report.
        /// </summary>
        /// <param name="to">To.</param>
        /// <param name="graphUrl">The graph URL.</param>
        /// <param name="title">The title.</param>
        /// <param name="description">The description.</param>
        public void SendReport(MailAddress[] to, string graphUrl, string title, string description)
        {
            ReportTo = to;
            GraphUrl = graphUrl;
            ShouldSendReport = true;
            this.Title = string.IsNullOrEmpty(title) ? this.CurrentList.SingleItemName : title;

            if (!string.IsNullOrEmpty(description))
                description = string.Format("<p style=\"color:#005770; font-family:Arial,Sans-Serif; margin: 10px 0px;\">{0}</p>", description);

            this.Description = description;
        }

        /// <summary>
        /// Clean all relative links from the send mail
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        internal string Links(Match m)
        {
            string path = m.Value.ToLower().Replace("href=", string.Empty).Replace("\"", string.Empty);
            if (path.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                return string.Format("href=\"{0}\"", path);

            if (path.StartsWith("?", StringComparison.OrdinalIgnoreCase))
                path = string.Concat("wim.ashx", path);
            
            return string.Format("href=\"{0}\"", this.Console.AddApplicationPath(path, true));
        }


        private Sushi.Mediakiwi.Data.IComponentList m_currentList;
        /// <summary>
        /// Gets or sets the current list.
        /// </summary>
        /// <value>The current list.</value>
        public Sushi.Mediakiwi.Data.IComponentList CurrentList
        {
            set { m_currentList = value; }
            get {
                if (m_currentList == null && Context != null)
                    m_currentList = Sushi.Mediakiwi.Data.ComponentList.SelectOne(Utility.ConvertToInt(Context.Request.Query["list"]));
                return m_currentList; 
            }
        }

        private Sushi.Mediakiwi.Data.Dashboard m_currentDashboard;
        /// <summary>
        /// Gets or sets the current dashboard.
        /// </summary>
        /// <value>The current list.</value>
        public Sushi.Mediakiwi.Data.Dashboard CurrentDashboard
        {
            set { m_currentDashboard = value; }
            get
            {
                if (m_currentDashboard == null && Context != null)
                    m_currentDashboard = Sushi.Mediakiwi.Data.Dashboard.SelectOne(Utility.ConvertToInt(Context.Request.Query["dashboard"]));
                return m_currentDashboard;
            }
        }

        /// <summary>
        /// Gets or sets the current data instance.
        /// </summary>
        /// <value>The current data instance.</value>
        public object CurrentDataInstance { get; set; }

        private int m_PropertyListTypeID;
        /// <summary>
        /// Gets or sets the property list type ID.
        /// </summary>
        /// <value>The property list type ID.</value>
        public int PropertyListTypeID
        {
            set { m_PropertyListTypeID = value; }
            get { return m_PropertyListTypeID; }
        }

        /// <summary>
        /// Gets or sets the property list ignore list.
        /// </summary>
        /// <value>
        /// The property list ignore list.
        /// </value>
        public Sushi.Mediakiwi.Data.Property[] PropertyListIgnoreList { get; set; }

        /// <summary>
        /// Gets or sets the pass over class instance.
        /// This properties can only be assigned from the constructor of the componentlist. If assigned the initiated componentlist will be replaced by this
        /// other "pass-over" instance.
        /// </summary>
        /// <value>The pass over class instance.</value>
        public IComponentListTemplate PassOverClassInstance { get; set; }

        private int? m_PropertyListOverrideID;
        /// <summary>
        /// Gets or sets the property list override ID.
        /// </summary>
        /// <value>The property list override ID.</value>
        public int? PropertyListOverrideID
        {
            set { m_PropertyListOverrideID = value; }
            get { return m_PropertyListOverrideID; }
        }

        private int[] m_PropertySubSelection;
        /// <summary>
        /// Gets or sets the property sub selection.
        /// If applied the list will show only these property: this overrides the default properties and the PropertyListTypeID value!
        /// </summary>
        /// <value>The property sub selection.</value>
        public int[] PropertySubSelection
        {
            set { m_PropertySubSelection = value; }
            get { return m_PropertySubSelection; }
        }

        //private Sushi.Mediakiwi.Framework.CacheManagement m_CacheManagement;
        ///// <summary>
        ///// Gets or sets the cache management.
        ///// </summary>
        ///// <value>The cache management.</value>
        //public Sushi.Mediakiwi.Framework.CacheManagement CacheManagement
        //{
        //    set { m_CacheManagement = value; }
        //    get {
        //        if (m_CacheManagement == null)
        //            m_CacheManagement = new CacheManagement();
        //        return m_CacheManagement; 
        //    }
        //}

        ///// <summary>
        ///// Gets or sets the graph manager.
        ///// </summary>
        ///// <value>The graph manager.</value>
        //public Sushi.Mediakiwi.Framework.GraphManager GraphManager { get; set; }


        ComponentListTemplate _origin;
        /// <summary>
        /// 
        /// </summary>
        public WimComponentListRoot(ComponentListTemplate origin)
        {
            _origin = origin;

            //DashBoardElementHeight = 1;
            SearchListCanClickThrough = true;
            DashBoardElementIsVisible = true;
            DashBoardCanClickThrough = true;

            //HttpContext context = Context;
            //if (context == null)
            //    throw new Exception("Wim: No HttpContent found; this class can only function if there is an active HttpContent.");

            //m_currentUser = Sushi.Mediakiwi.Framework.Templates.CurrentUser.Get;

            //System.Nullable<int> list = null;
            //System.Nullable<int> site = null;
            //System.Nullable<int> folder = null;
            //System.Nullable<int> page = null;

            //if (!string.IsNullOrEmpty(context.Request.Params["page"])) page = Convert.ToInt32(context.Request.Params["page"]);
            //if (!string.IsNullOrEmpty(context.Request.Params["folder"])) folder = Convert.ToInt32(context.Request.Params["folder"]);
            //if (!string.IsNullOrEmpty(context.Request.Params["site"])) site = Convert.ToInt32(context.Request.Params["site"]);

            //int currentList;
            //if (Utility.IsNumeric(context.Request.Params["list"], out currentList))
            //{
            //    m_currentList = Sushi.Mediakiwi.Data.ComponentList.SelectOne(currentList);
            //    list = m_currentList.Id;
            //}
            //else
            //{
            //    Guid guid;
            //    if (Utility.IsGuid(context.Request.Params["list"], out guid))
            //    {
            //        m_currentList = Sushi.Mediakiwi.Data.ComponentList.SelectOne(guid);
            //        list = m_currentList.Id;
            //    }
            //}

            //if (site.HasValue) m_CurrentSite = Sushi.Mediakiwi.Data.Site.SelectOne(site.Value);
            //else
            //{
            //    if (page.HasValue) m_CurrentSite = Sushi.Mediakiwi.Data.Site.SelectOne_byPage(page.Value);
            //    else if (folder.HasValue) m_CurrentSite = Sushi.Mediakiwi.Data.Site.SelectOne_byFolder(folder.Value);
            //    else if (list.HasValue)
            //    {
            //        if (m_currentList.SiteId > 0)
            //            m_CurrentSite = Sushi.Mediakiwi.Data.Site.SelectOne(m_currentList.SiteId, true);
            //    }
            //}
            //if (context.Request.Query["EXCLUDE"] == "1") return;

            ////if (m_CurrentSite == null)
            ////    throw new Exception("Wim: Could not determine the site that this class belongs to.");
        }

        /// <summary>
        /// Gets or sets the current environment.
        /// </summary>
        /// <value>The current environment.</value>
        public Sushi.Mediakiwi.Data.IEnvironment CurrentEnvironment { get; set; }

        private Sushi.Mediakiwi.Data.IApplicationRole m_CurrentApplicationUserRole;
        /// <summary>
        /// Gets or sets the current user role.
        /// </summary>
        /// <value>The current user role.</value>
        public Sushi.Mediakiwi.Data.IApplicationRole CurrentApplicationUserRole
        {
            set { m_CurrentApplicationUserRole = value; }
            get { return m_CurrentApplicationUserRole; }
        }

        /// <summary>
        /// Gets or sets the current visitor.
        /// </summary>
        /// <value>The current visitor.</value>
        public IVisitor CurrentVisitor
        {
            get { return new VisitorManager(Context).Select(); }
            set { Context.Items["wim.visitor"] = value; }
        }

        /// <summary>
        /// Gets or sets the current application user.
        /// </summary>
        /// <value>The current application user.</value>
        public Sushi.Mediakiwi.Data.IApplicationUser CurrentApplicationUser
        {
            get {
                var candidate = Context.Items["wim.applicationuser"] as Sushi.Mediakiwi.Data.IApplicationUser;
                return candidate;
            }
            set { Context.Items["wim.applicationuser"] = value; }
        }

        private System.Globalization.CultureInfo m_CurrentCulture;
        /// <summary>
        /// The set culture. This could be different from Culture set in CurrentThread.CurrentCulture because this does not
        /// accept neutral cultures.
        /// </summary>
        public System.Globalization.CultureInfo CurrentCulture
        {
            get
            {
                if (m_CurrentCulture != null) return m_CurrentCulture;
                if (m_CurrentSite == null || string.IsNullOrEmpty(m_CurrentSite.Culture))
                    return System.Globalization.CultureInfo.CurrentCulture;

                m_CurrentCulture = new System.Globalization.CultureInfo(m_CurrentSite.Culture);
                return m_CurrentCulture;
            }
        }

        private Sushi.Mediakiwi.Data.Site m_CurrentSite;
        /// <summary>
        /// The current site in which this list is viewed.
        /// </summary>
        public Sushi.Mediakiwi.Data.Site CurrentSite
        {
            set { m_CurrentSite = value; }
            get { return m_CurrentSite; }
        }

        //private string[] m_ListColumns;
        ///// <summary>
        ///// Columns shown on the search bar
        ///// </summary>
        //[Obsolete("This property is obsolete, please use 'wim.ListDataColumns.Add' instead.")]
        //public string[] ListColumns
        //{
        //    set { m_ListColumns = value; }
        //    get { return m_ListColumns; }
        //}

        private Notifications m_Notification;
        /// <summary>
        /// 
        /// </summary>
        public Notifications Notification
        {
            get
            {
                if (m_Notification == null)
                    m_Notification = new Notifications(this);
                return m_Notification;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class Notifications
        {
            WimComponentListRoot wim;
            public Notifications(WimComponentListRoot wim)
            {
                this.wim = wim;
            }

            /// <summary>
            /// Adds the error.
            /// </summary>
            /// <param name="property">The property (form element) shown on the page.</param>
            /// <param name="errorMessage">The error message.</param>
            public void AddError(string property, string errorMessage)
            {
                AddError(new string[] { property }, errorMessage);
            }

            /// <summary>
            /// Adds the error.
            /// </summary>
            /// <param name="errorMessage">The error message.</param>
            public void AddError(string errorMessage)
            {
                if (string.IsNullOrEmpty(errorMessage))
                    return;

                if (GenericErrors == null)
                {
                    GenericErrors = new List<string>();
                }
                GenericErrors.Add(errorMessage);
            }

            /// <summary>
            /// Adds the error.
            /// </summary>
            /// <param name="properties">The properties.</param>
            public void AddError(string[] properties)
            {
                AddError(properties, null);
            }

            /// <summary>
            /// Adds the error.
            /// </summary>
            /// <param name="properties">The property (form element) shown on the page.</param>
            /// <param name="errorMessage">The error message.</param>
            public void AddError(string[] properties, string errorMessage)
            {
                if (properties != null)
                {
                    foreach (string property in properties)
                    {
                        if (property == null)
                        {
                            if (GenericErrors == null)
                            {
                                GenericErrors = new List<string>();
                            }
                            GenericErrors.Add(errorMessage);
                            continue;
                        }
                        if (Errors == null)
                            Errors = new Hashtable();

                        Errors.Add(property, errorMessage);
                    }
                }
            }

            internal Hashtable Errors;
            internal List<string> GenericErrors;
            internal List<string> GenericInformation;
            internal List<string> GenericInformationAlert;

            /// <summary>
            /// Adds the notification alert.
            /// </summary>
            /// <param name="notificationMessage">The notification message.</param>
            public void AddNotificationAlert(string notificationMessage)
            {
                AddNotificationAlert(notificationMessage, false);
            }

            /// <summary>
            /// Adds the notification alert.
            /// </summary>
            /// <param name="notificationMessage">The notification message.</param>
            /// <param name="rememberNotification">if True the notification will be remembered accross redirects (default False)</param>
            public void AddNotificationAlert(string notificationMessage, bool rememberNotification)
            {
                if (notificationMessage != null)
                    notificationMessage = notificationMessage.Trim();

                if (string.IsNullOrEmpty(notificationMessage) || Utility.CleanFormatting(notificationMessage).Trim().Length == 0)
                    return;

                if (GenericInformationAlert == null)
                {
                    GenericInformationAlert = new List<string>();
                }
                GenericInformationAlert.Add(notificationMessage);
                if (rememberNotification)
                {
                    string alerts = string.Empty;
                    GenericInformationAlert.ForEach(x => { alerts += string.Concat("||", x); });
                    wim.CurrentVisitor.Data.Apply("alerts", alerts);
                    wim.CurrentVisitor.Save();
                }
            }

            internal void ClearNotificationAlert()
            {
                if (wim.CurrentVisitor.Data.HasProperty("alerts"))
                { 
                    wim.CurrentVisitor.Data.Apply("alerts", null);
                    wim.CurrentVisitor.Save();
                }
            }

            internal void RestoreNotificationAlert()
            {
                if (wim.CurrentVisitor.Data.HasProperty("alerts"))
                {
                    if (GenericInformation == null)
                    {
                        GenericInformation = new List<string>();
                    }

                    var notes = wim.CurrentVisitor.Data["alerts"].Value.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var note in notes)
                    {
                        GenericInformation.Add(note);
                    }
                    ClearNotificationAlert();
                }
            }

            /// <summary>
            /// Adds the notification.
            /// </summary>
            /// <param name="notificationMessage">The notification message.</param>
            public void AddNotification(string notificationMessage)
            {
                AddNotification(notificationMessage, null);
            }

            /// <summary>
            /// Adds the information tag.
            /// </summary>
            /// <param name="notificationMessage">The notification message.</param>
            /// <param name="notificationTitle">The notification title.</param>
            public void AddNotification(string notificationMessage, string notificationTitle)
            {
                AddNotification(notificationMessage, notificationTitle, false);
            }

            /// <summary>
            /// Adds the information tag.
            /// </summary>
            /// <param name="notificationMessage">The notification message.</param>
            /// <param name="notificationTitle">The notification title.</param>
            /// <param name="rememberNotification">if True the notification will be remembered accross redirects (default False)</param>
            public void AddNotification(string notificationMessage, string notificationTitle, bool rememberNotification)
            {
                if (!string.IsNullOrEmpty(notificationTitle))
                {
                    if (notificationMessage != null)
                        notificationMessage = string.Format("<h3>{1}</h3>{0}", notificationMessage, notificationTitle);
                }

                if (notificationMessage != null)
                    notificationMessage = notificationMessage.Trim();

                if (string.IsNullOrEmpty(notificationMessage) || Utility.CleanFormatting(notificationMessage).Trim().Length == 0)
                    return;

                if (GenericInformation == null)
                {
                    GenericInformation = new List<string>();
                }
                GenericInformation.Add(notificationMessage);
                if (rememberNotification)
                {
                    string notes = string.Empty;
                    GenericInformation.ForEach(x => { notes += string.Concat("||", x); });
                    wim.CurrentVisitor.Data.Apply("notes", notes);
                    wim.CurrentVisitor.Save();
                }
            }
            internal void ClearNotification()
            {
                if (wim.CurrentVisitor.Data.HasProperty("notes"))
                { 
                    wim.CurrentVisitor.Data.Apply("notes", null);
                    wim.CurrentVisitor.Save();
                }
            }

            internal void RestoreNotification()
            {
                if (wim.CurrentVisitor.Data.HasProperty("notes"))
                {
                    if (GenericInformation == null)
                    {
                        GenericInformation = new List<string>();
                    }

                    var notes = wim.CurrentVisitor.Data["notes"].Value.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var note in notes)
                    {
                        GenericInformation.Add(note);
                    }
                }
            }
        }

        

        internal List<string> _QueryStringRecording;
        /// <summary>
        /// Add querystring items to the tabs
        /// </summary>
        /// <param name="name"></param>
        public void AddTabQueryStringRecording(string name)
        {
            if (_QueryStringRecording == null)
                _QueryStringRecording = new List<string>();

            if (!_QueryStringRecording.Contains(name))
                _QueryStringRecording.Add(name);
        }

        /// <summary>
        /// Adds the tab.
        /// </summary>
        /// <param name="ID">The ID.</param>
        public void AddTab(Guid ID)
        {
            var list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(ID);
            AddTab(list);
        }

        /// <summary>
        /// Adds the tab.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <param name="selectionID">The selection ID.</param>
        public void AddTab(Guid ID, int selectionID)
        {
            var list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(ID);
            AddTab(list, selectionID);
        }

        /// <summary>
        /// Adds the tab.
        /// </summary>
        /// <param name="type">The type.</param>
        public void AddTab(System.Type type)
        {
            var list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(type.ToString());
            AddTab(list);
        }

        /// <summary>
        /// Adds the tab.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="selectionID">The selection ID.</param>
        public void AddTab(System.Type type, int selectionID)
        {
            var list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(type.ToString());
            AddTab(list, selectionID);
        }

        /// <summary>
        /// Adds the tab.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="selectionID">The selection identifier.</param>
        /// <param name="count">The count.</param>
        public void AddTab(System.Type type, int selectionID, int? count)
        {
            var list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(type.ToString());
            AddTab(list.Name, list, selectionID, count);
        }

        /// <summary>
        /// Adds the tab.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="title">The title.</param>
        /// <param name="selectionID">The selection ID.</param>
        public void AddTab(System.Type type, string title, int selectionID)
        {
            var list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(type.ToString());
            AddTab(title, list, selectionID);
        }

        /// <summary>
        /// Adds the tab.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="title">The title.</param>
        /// <param name="selectionID">The selection ID.</param>
        public void AddTab(System.Guid id, string title, int selectionID)
        {
            var list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(id);
            AddTab(title, list, selectionID);
        }


        /// <summary>
        /// Adds the tab. These should be added in the ListLoad event
        /// </summary>
        /// <param name="list">The list.</param>
        public void AddTab(Data.IComponentList list)
        {
            AddTab(list.Name, list, 0);
        }

        /// <summary>
        /// Adds the tab. These should be added in the ListLoad event
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="selectedItem">The selected item.</param>
        public void AddTab(Data.IComponentList list, int selectedItem)
        {
            AddTab(list.Name, list, selectedItem);
        }

        /// <summary>
        /// Adds the tab. These should be added in the ListLoad event
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="list">The list.</param>
        public void AddTab(string title, Data.IComponentList list)
        {
            AddTab(title, list, 0);
        }

        /// <summary>
        /// Adds the tab.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="list">The list.</param>
        /// <param name="selectedItem">The selected item.</param>
        public void AddTab(string title, Data.IComponentList list, int selectedItem)
        {
            AddTab(title, list, selectedItem, null);
        }

        /// <summary>
        /// Adds the tab. These should be added in the ListLoad event
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="list">The list.</param>
        /// <param name="selectedItem">The selected item.</param>
        /// <param name="count">The count.</param>
        public void AddTab(string title, Data.IComponentList list, int selectedItem, int? count)
        {
            if (!list.HasRoleAccess(this.CurrentApplicationUser))
                return;

            if (m_Collection == null)
                m_Collection = new List<Tabular>();

            Tabular t = new Tabular();
            t.List = list;
            t.Selected = (_Console.Request.Query["list"] == list.ID.ToString());
            t.SelectedItem = selectedItem;
            t.Title2 = title;
            t.Count = count;
            m_Collection.Add(t);
        }

        internal List<Tabular> m_Collection;

        /// <summary>
        /// Represents a Tabular entity.
        /// </summary>
        public class Tabular
        {
            /// <summary>
            /// 
            /// </summary>
            public string Title2;

            internal string TitleValue
            {
                get
                {
                    if (this.Count.HasValue)
                    {
                        return string.Format("{0}<span class=\"items\">{1}</span>", this.Title2, this.Count.Value);
                    }
                    return Title2;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public IComponentList List;
            /// <summary>
            /// 
            /// </summary>
            public string Url;
            /// <summary>
            /// 
            /// </summary>
            public bool Selected;
            /// <summary>
            /// 
            /// </summary>
            public int SelectedItem;
            /// <summary>
            /// The count
            /// </summary>
            public int? Count;
        }


        private string m_OnSaveScript;
        /// <summary>
        /// 
        /// </summary>
        public string OnSaveScript
        {
            set { m_OnSaveScript = value; }
            get { return m_OnSaveScript; }
        }

        /// <summary>
        /// Gets or sets the header script.
        /// </summary>
        /// <value>
        /// The header script.
        /// </value>
        public string HeaderScript { get; set; }
        /// <summary>
        /// Gets or sets the on load script.
        /// </summary>
        /// <value>
        /// The on load script.
        /// </value>
        public string OnLoadScript { get; set; }


        private int? m_AfterSaveElementIdentifier;
        /// <summary>
        /// When adding a new item to a list the unique identifier of this element is not known
        /// to Wim until after the ListSave event is triggered.
        /// The result is that the page is redirected the the list overview page. If the just 
        /// saved element should be presented; the newly created entity identifier should be assigned 
        /// to this property.
        /// </summary>
        internal int? AfterSaveElementIdentifier
        {
            set {
                if (value.HasValue && value.Value != 0)
                {
                    if (Context != null)
                        Context.Items["wim.Saved.ID"] = value;
                    m_AfterSaveElementIdentifier = value;
                }
            }
            get {

                if (Context != null)
                    return Utility.ConvertToIntNullable(Context.Items["wim.Saved.ID"]);
                return m_AfterSaveElementIdentifier;
            }
        }

        private string m_OnDeleteScript;
        /// <summary>
        /// 
        /// </summary>
        public string OnDeleteScript
        {
            set { m_OnDeleteScript = value; }
            get { return m_OnDeleteScript; }
        }

        PageData m_Page;
        public PageData Page
        {
            get
            {
                if (m_Page == null) m_Page = new PageData(this);
                return m_Page;
            }
            set { m_Page = value; }
        }

        FormData m_Form;
        public FormData Form
        {
            get
            {
                if (m_Form == null) m_Form = new FormData(this);
                return m_Form;
            }
            set { m_Form = value; }
        }

        GridDataDetail m_GridDataCommunication;
        /// <summary>
        /// Gets the grid data communication.
        /// </summary>
        public GridDataDetail GridDataCommunication
        {
            get
            {
                if (m_GridDataCommunication == null) m_GridDataCommunication = new GridDataDetail(this);
                return m_GridDataCommunication;
            }
        }

        GridData m_Grid;
        public GridData Grid
        {
            get
            {
                if (m_Grid == null) m_Grid = new GridData(this);
                return m_Grid;
            }
            set { m_Grid = value; }
        }

        internal StringBuilder XHtmlDataTop;
        internal StringBuilder XHtmlDataBottom;
        internal StringBuilder XHtmlDataService;
        internal StringBuilder XHtmlDataButtons;

        internal string XHtmlDataServiceTitle;
        internal System.Collections.Specialized.NameValueCollection AddedCheckboxPostCollection;
        internal System.Collections.Specialized.NameValueCollection AddedCheckboxStateCollection;
        internal System.Collections.Specialized.NameValueCollection AddedRadioboxPostCollection;
        internal System.Collections.Specialized.NameValueCollection AddedRadioboxStateCollection;
        internal System.Collections.Specialized.NameValueCollection AddedTextboxPostCollection;



        /// <summary>
        /// 
        /// </summary>
        public class GridData
        {
            WimComponentListRoot m_Root;
            /// <summary>
            /// Initializes a new instance of the <see cref="FormData"/> class.
            /// </summary>
            /// <param name="root">The root.</param>
            public GridData(WimComponentListRoot root)
            {
                m_Root = root;
            }
            /// <summary>
            /// Adds the checkbox value.
            /// </summary>
            /// <param name="propertyName">Name of the property.</param>
            /// <param name="ID">The ID.</param>
            /// <param name="isChecked">if set to <c>true</c> [is checked].</param>
            public void AddCheckboxValue(string propertyName, int ID, bool isChecked)
            {
                if (m_Root.AddedCheckboxPostCollection == null)
                    m_Root.AddedCheckboxPostCollection = new System.Collections.Specialized.NameValueCollection();

                m_Root.AddedCheckboxPostCollection.Add(string.Concat(propertyName, "_", ID), isChecked ? "1" : "0");
            }

            /// <summary>
            /// Adds the textbox value.
            /// </summary>
            /// <param name="propertyName">Name of the property.</param>
            /// <param name="ID">The ID.</param>
            /// <param name="value">The value.</param>
            public void AddTextboxValue(string propertyName, int ID, string value)
            {
                if (m_Root.AddedTextboxPostCollection == null)
                    m_Root.AddedTextboxPostCollection = new System.Collections.Specialized.NameValueCollection();

                m_Root.AddedTextboxPostCollection.Add(string.Concat(propertyName, "_", ID), value);
            }

            /// <summary>
            /// Adds the textbox value.
            /// </summary>
            /// <param name="propertyName">Name of the property.</param>
            /// <param name="ID">The ID.</param>
            /// <param name="value">The value.</param>
            public void AddTextboxValue(string propertyName, int ID, decimal value)
            {
                if (m_Root.AddedTextboxPostCollection == null)
                    m_Root.AddedTextboxPostCollection = new System.Collections.Specialized.NameValueCollection();

                m_Root.AddedTextboxPostCollection.Add(string.Concat(propertyName, "_", ID), Utility.ConvertToDecimalString(value));
            }

            [Obsolete("Not part of mediakiwi", false)]
            public string BackgroundImage_Odd { get; set; }
            [Obsolete("Not part of mediakiwi", false)]
            public string BackgroundImage_Even { get; set; }

            //public StringBuilder _HTML;

            /// <summary>
            /// Adds the radiobox value.
            /// </summary>
            /// <param name="propertyName">Name of the property.</param>
            /// <param name="ID">The ID.</param>
            /// <param name="value">The value.</param>
            public void AddRadioboxValue(string propertyName, int ID, string value)
            {
                if (m_Root.AddedRadioboxPostCollection == null)
                    m_Root.AddedRadioboxPostCollection = new System.Collections.Specialized.NameValueCollection();

                m_Root.AddedRadioboxPostCollection.Add(string.Concat(propertyName, "_", ID), value);
            }

            /// <summary>
            /// Adds the state of the radiobox.
            /// </summary>
            /// <param name="propertyName">Name of the property.</param>
            /// <param name="ID">The ID.</param>
            /// <param name="isEnabled">if set to <c>true</c> [is enabled].</param>
            public void AddRadioboxState(string propertyName, int ID, bool isEnabled)
            {
                if (m_Root.AddedRadioboxStateCollection == null)
                    m_Root.AddedRadioboxStateCollection = new System.Collections.Specialized.NameValueCollection();

                m_Root.AddedRadioboxStateCollection.Add(string.Concat(propertyName, "_", ID), isEnabled ? "1" : "0");
            }
            
            /// <summary>
            /// Adds the state of the checkbox.
            /// </summary>
            /// <param name="propertyName">Name of the property.</param>
            /// <param name="ID">The ID.</param>
            /// <param name="isEnabled">if set to <c>true</c> [is enabled].</param>
            public void AddCheckboxState(string propertyName, int ID, bool isEnabled)
            {
                if (m_Root.AddedCheckboxStateCollection == null)
                    m_Root.AddedCheckboxStateCollection = new System.Collections.Specialized.NameValueCollection();

                m_Root.AddedCheckboxStateCollection.Add(string.Concat(propertyName, "_", ID), isEnabled ? "1" : "0");
            }
   
            /// <summary>
            /// Determines whether [is checkbox checked] [the specified property name].
            /// </summary>
            /// <param name="propertyName">Name of the property.</param>
            /// <param name="ID">The ID.</param>
            /// <returns>
            /// 	<c>true</c> if [is checkbox checked] [the specified property name]; otherwise, <c>false</c>.
            /// </returns>
            public bool IsCheckboxChecked(string propertyName, int ID)
            {
                return (from item in GetCheckboxChecked(propertyName) where item == ID select item).Count() == 1;
            }

            /// <summary>
            /// Gets or sets the type of the data bind.
            /// </summary>
            /// <value>
            /// The type of the data bind.
            /// </value>
            public System.Type DataBindTemplate { get; set; }

            //internal bool IsAsyncRequest;
            //internal bool? _IsDataBind;
            ///// <summary>
            ///// Gets a value indicating whether this thread should bind the data; when requested the main thread automatically converts to an JSON update call.
            ///// </summary>
            //public bool IsDataBinding { 
            //    get {
            //        if (!_IsDataBind.HasValue)
            //        {
            //            _IsDataBind = true;
            //            if (this.m_Root.CurrentApplicationUser.ShowNewDesign && this.IsAsyncRequest)
            //            {
            //                if (this.m_Root.Console.Request.Params[Wim.UI.Constants.AJAX_PARAM] == "1")
            //                    _IsDataBind = true;
            //                else
            //                    _IsDataBind = false;
            //            }
            //        }
            //        return _IsDataBind.Value; 
            //    } 
            //}

            /// <summary>
            /// Gets the checkbox checked.
            /// </summary>
            /// <param name="propertyName">Name of the property.</param>
            /// <returns></returns>
            public int[] GetCheckboxChecked(string propertyName)
            {
                List<string> foundkeyarr = new List<string>();
                if (m_Root.Console.Request.HasFormContentType)
                {
                    foreach (string key in m_Root.Console.Request.Form.Keys)
                    {
                        if (key.StartsWith(propertyName, StringComparison.CurrentCultureIgnoreCase) && key.Contains("_") && m_Root.Console.Form(key) == "1")
                            foundkeyarr.Add(key.Split('_')[1]);
                    }
                }
                return Utility.ConvertToIntArray(foundkeyarr.ToArray());
            }

            public string GetTextboxValue(string propertyName, int ID)
            {
                var value = m_Root.Console.Form(string.Concat(propertyName, "_", ID));
                return value;
            }

            public decimal? GetTextboxDecimalValue(string propertyName, int ID)
            {
                var value = m_Root.Console.Form(string.Concat(propertyName, "_", ID));
                return Utility.ConvertToDecimalNullable(value);
            }

            /// <summary>
            /// Gets the radio value.
            /// </summary>
            /// <param name="propertyName">Name of the property.</param>
            /// <returns></returns>
            public Dictionary<int, string> GetRadioValue(string propertyName)
            {
                Dictionary<int, string> dict = new Dictionary<int, string>();

                List<string> foundkeyarr = new List<string>();

                string validation = propertyName + "_";
                if (m_Root.Console.Request.HasFormContentType)
                {
                    foreach (string key in m_Root.Console.Request.Form.Keys)
                    {
                        if (key.StartsWith(validation, StringComparison.CurrentCultureIgnoreCase))
                        {
                            dict.Add(Convert.ToInt32(key.Split('_')[1]), m_Root.Console.Form(key).ToString());
                        }
                    }
                }
                return dict;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class PageData
        {            
            WimComponentListRoot m_Root;
            /// <summary>
            /// Initializes a new instance of the <see cref="FormData"/> class.
            /// </summary>
            /// <param name="root">The root.</param>
            public PageData(WimComponentListRoot root)
            {
                m_Root = root;
            }

            /// <summary>
            /// Converts the specified data.
            /// </summary>
            /// <param name="data">The data.</param>
            /// <returns></returns>
            string Convert(string data)
            {
                return data
                    .Replace("[code]", "<pre>")
                    .Replace("[/code]", "</pre>");
            }

            #region Obsolete methods 
            /// <summary>
            /// Adds the XHTML body.
            /// </summary>
            /// <param name="data">The data.</param>
            [Obsolete("Can not be used in Mediakiwi", false)]
            public void AddXhtmlBody(string data)
            {
                if (string.IsNullOrEmpty(data)) return;
                AddXHtml(string.Format(@"<p>{0}</p>", Convert(data)));
            }

            /// <summary>
            /// Adds the XHTML intro.
            /// </summary>
            /// <param name="data">The data.</param>
            [Obsolete("Can not be used in Mediakiwi", false)]
            public void AddXhtmlIntro(string data)
            {
                if (string.IsNullOrEmpty(data)) return;
                AddXHtml(string.Format(@"<p class=""intro"">{0}</p>", Convert(data)));
            }

            /// <summary>
            /// Adds the XHTML intro.
            /// </summary>
            /// <param name="data">The data.</param>
            /// <param name="url">The URL.</param>
            [Obsolete("Can not be used in Mediakiwi", false)]
            public void AddXhtmlIntro(string data, string url)
            {
                if (string.IsNullOrEmpty(data)) return;
                AddXHtml(string.Format(@"<p class=""intro""><a class=""more"" href=""{1}"">{0}</a></p>", Convert(data), url));
            }

            /// <summary>
            /// Adds the XHTML body.
            /// </summary>
            /// <param name="data">The data.</param>
            /// <param name="url">The URL.</param>
            [Obsolete("Can not be used in Mediakiwi", false)]
            public void AddXhtmlBody(string data, string url)
            {
                if (string.IsNullOrEmpty(data)) return;
                AddXHtml(string.Format(@"<p><a class=""more"" href=""{1}"">{0}</a></p>", Convert(data), url));
            }

            /// <summary>
            /// Adds the XHTML body.
            /// </summary>
            /// <param name="data">The data.</param>
            /// <param name="url">The URL.</param>
            /// <param name="added">The added.</param>
            [Obsolete("Can not be used in Mediakiwi", false)]
            public void AddXhtmlBody(string data, string url, string added)
            {
                if (string.IsNullOrEmpty(data)) return;
                AddXHtml(string.Format(@"<p><a class=""more"" href=""{1}"">{0}</a><br/>{2}</p>", Convert(data), url, added));
            }

            /// <summary>
            /// Adds the XHTML title.
            /// </summary>
            /// <param name="data">The data.</param>
            /// <param name="url">The URL.</param>
            [Obsolete("Can not be used in Mediakiwi", false)]
            public void AddXhtmlTitle(string data, string url)
            {
                if (string.IsNullOrEmpty(data)) return;
                AddXHtml(string.Format(@"<h1><a href=""{1}"">{0}</a></h1>", Convert(data), url));
            }

            public string TMP_ReportingSection { get; set; }

            /// <summary>
            /// Adds the XHTML sub title.
            /// </summary>
            /// <param name="data">The data.</param>
            /// <param name="url">The URL.</param>
            [Obsolete("Can not be used in Mediakiwi", false)]
            public void AddXhtmlSubTitle(string data, string url)
            {
                if (string.IsNullOrEmpty(data)) return;
                AddXHtml(string.Format(@"<a href=""{1}""><h2 class=""sub""><span class=""label"">{0}</span></h2></a>", Convert(data), url));
            }

            /// <summary>
            /// Adds the XHTML sub title italic.
            /// </summary>
            /// <param name="data">The data.</param>
            [Obsolete("Can not be used in Mediakiwi", false)]
            public void AddXhtmlSubTitleItalic(string data)
            {
                if (string.IsNullOrEmpty(data)) return;
                AddXHtml(string.Format(@"<h2 class=""sub""><i class=""right"">{0}</i></h2>", Convert(data)));
            }

            /// <summary>
            /// Adds the XHTML title.
            /// </summary>
            /// <param name="data">The data.</param>
            [Obsolete("Can not be used in Mediakiwi", false)]
            public void AddXhtmlTitle(string data)
            {
                if (m_Root.IsDashboardMode) return;
                if (string.IsNullOrEmpty(data)) return;
                AddXHtml(string.Format(@"<h1>{0}</h1>", Convert(data)));
            }

            /// <summary>
            /// Adds the XHTML sub title.
            /// </summary>
            /// <param name="data">The data.</param>
            [Obsolete("Can not be used in Mediakiwi", false)]
            public void AddXhtmlSubTitle(string data)
            {
                if (string.IsNullOrEmpty(data))
                    return;

                AddXHtml(string.Format(@"<h2 class=""sub""><span class=""label"">{0}</span></h2>", Convert(data)));
            }

            /// <summary>
            /// Adds the XHTML read more.
            /// </summary>
            /// <param name="url">The URL.</param>
            [Obsolete("Can not be used in Mediakiwi", false)]
            public void AddXhtmlReadMore(string url)
            {
                if (string.IsNullOrEmpty(url))
                    return;

                AddXHtml(string.Format(@"<p class=""toRight""><a class=""more"" href=""{0}"">Lees meer</a></p>", url));
            }
            #endregion Obsolete methods 



            Head _Head;
            /// <summary>
            /// Gets or sets the head.
            /// </summary>
            /// <value>
            /// The head.
            /// </value>
            public Head Head
            {
                get { if (_Head == null) _Head = new Head(this, m_Root); return _Head; }
                set { _Head = value; }
            }

            Body _Body;
            /// <summary>
            /// Gets or sets the body.
            /// </summary>
            /// <value>
            /// The body.
            /// </value>
            public Body Body
            {
                get { if (_Body == null) _Body = new Body(m_Root); return _Body; }
                set { _Body = value; }
            }

            /// <summary>
            /// Adds the X HTML.
            /// </summary>
            /// <param name="data">The data.</param>
            public void AddXHtml(string data)
            {
                if (string.IsNullOrEmpty(data)) return;

                if (m_Root.XHtmlDataTop == null)
                    m_Root.XHtmlDataTop = new StringBuilder();

                m_Root.XHtmlDataTop.Append(data);
            }

            /// <summary>
            /// Adds the X HTML bottom.
            /// </summary>
            /// <param name="data">The data.</param>
            public void AddXHtmlBottom(string data)
            {
                if (string.IsNullOrEmpty(data)) return;

                if (m_Root.XHtmlDataBottom == null)
                    m_Root.XHtmlDataBottom = new StringBuilder();

                m_Root.XHtmlDataBottom.Append(data);
            }

            /// <summary>
            /// Adds the service title.
            /// </summary>
            /// <param name="title">The title.</param>
            public void AddServiceTitle(string title)
            {
                m_Root.XHtmlDataServiceTitle = title;
            }

            /// <summary>
            /// Adds the service.
            /// </summary>
            /// <param name="link">The link.</param>
            public void AddService(Link link)
            {
                if (link == null || link.ID == 0) return;

                if (m_Root.XHtmlDataService == null)
                    m_Root.XHtmlDataService = new StringBuilder();

                string url = link.GetUrl(m_Root.CurrentSite);
                if (!string.IsNullOrEmpty(url))
                {
                    if (link.AssetID.HasValue && !link.Asset.Exists)
                        m_Root.XHtmlDataService.AppendFormat("<li><a href=\"#\" class=\"{2}\">{0}</a></li>", link.Text, url, "nof");
                    else
                        m_Root.XHtmlDataService.AppendFormat("<li><a href=\"{1}\" class=\"{2}\">{0}</a></li>", link.Text, url, link.AssetID.HasValue ? link.Asset.ExtentionClassName : "hyperlink");
                }
            }

            /// <summary>
            /// Adds the button.
            /// </summary>
            /// <param name="property">The property.</param>
            /// <param name="title">The title.</param>
            /// <param name="classType">Type of the class.</param>
            public void AddButton(string property, string title, string classType)
            {
                if (m_Root.XHtmlDataButtons == null)
                    m_Root.XHtmlDataButtons = new StringBuilder();

                m_Root.XHtmlDataButtons.AppendFormat("<li><a href=\"#\" id=\"{1}\" class=\"{2}\">{0}</a></li>", title, property, classType);
            }

            /// <summary>
            /// Adds the button.
            /// </summary>
            /// <param name="property">The property.</param>
            /// <param name="title">The title.</param>
            /// <param name="classType">Type of the class.</param>
            /// <param name="url">The URL.</param>
            public void AddButton(string property, string title, string classType, string url)
            {
                if (m_Root.XHtmlDataButtons == null)
                    m_Root.XHtmlDataButtons = new StringBuilder();

                m_Root.XHtmlDataButtons.AppendFormat("<li><a href=\"{3}\" id=\"{1}\" class=\"{2}\">{0}</a></li>", title, property, classType, url);
            }
            /// <summary>
            /// Gets or sets a value indicating whether [top icon bar].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [top icon bar]; otherwise, <c>false</c>.
            /// </value>
            public bool HideTopIconBar { get; set; }
            /// <summary>
            /// Gets or sets a value indicating whether [hide menu bar].
            /// </summary>
            /// <value><c>true</c> if [hide menu bar]; otherwise, <c>false</c>.</value>
            public bool HideMenuBar { get; set; }
            /// <summary>
            /// Gets or sets a value indicating whether [hide tabbed grid].
            /// </summary>
            /// <value><c>true</c> if [hide tabbed grid]; otherwise, <c>false</c>.</value>
            public bool HideTabbedGrid { get; set; }
            /// <summary>
            /// Gets or sets a value indicating whether [hide tabs].
            /// </summary>
            /// <value><c>true</c> if [hide tabs]; otherwise, <c>false</c>.</value>
            public bool HideTabs { get; set; }
            /// <summary>
            /// Gets or sets a value indicating whether [hide data grid].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [hide data grid]; otherwise, <c>false</c>.
            /// </value>
            public bool HideDataGrid { get; set; }
            /// <summary>
            /// Gets or sets a value indicating whether [hide data form].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [hide data form]; otherwise, <c>false</c>.
            /// </value>
            public bool HideDataForm { get; set; }
            public bool HideBreadCrumbs { get; set; }
            public bool HidePageTitle { get; set; }
            public bool ShowReportFirst { get; set; }
            public bool HideFormFilter { get; set; }

     
        }


        /// <summary>
        /// 
        /// </summary>
        public class FormData
        {
            WimComponentListRoot m_Root;
            /// <summary>
            /// Initializes a new instance of the <see cref="FormData"/> class.
            /// </summary>
            /// <param name="root">The root.</param>
            public FormData(WimComponentListRoot root)
            {
                m_Root = root;
            }

            Sushi.Mediakiwi.Data.Content m_Content;
            /// <summary>
            /// Gets the filter field.
            /// </summary>
            /// <param name="property">The property.</param>
            /// <returns></returns>
            public Field GetFilterField(string property)
            {
                if (string.IsNullOrEmpty(m_Root.CurrentVisitor.Data.Serialized)) return new Field();
                if (m_Content == null)
                    m_Content = Sushi.Mediakiwi.Data.Content.GetDeserialized(m_Root.CurrentVisitor.Data["wim_FilterInfo"].Value);
                if (m_Content == null) return new Field();

                return m_Content[property];
            }

            /// <summary>
            /// Applies the filter field.
            /// </summary>
            /// <param name="property">The property.</param>
            /// <param name="value">The value.</param>
            public void ApplyFilterField(string property, object value)
            {
                //m_Root.CurrentVisitor.Data.Apply("wim_FilterInfo", 
                //if (string.IsNullOrEmpty(m_Root.CurrentVisitor.Data.Serialized)) return new Content.Field();
                if (m_Content == null)
                    m_Content = Sushi.Mediakiwi.Data.Content.GetDeserialized(m_Root.CurrentVisitor.Data["wim_FilterInfo"].Value);
                if (m_Content == null)
                    m_Content = new Content();

                List<Field> list = new List<Field>();
                if (m_Content.Fields != null)
                {
                    foreach (var x in m_Content.Fields)
                        list.Add(x);
                }

                if (m_Content.IsNull(property))
                {
                    var field = new Field() { Property = property, Value = value == null ? null : value.ToString() };
                    list.Add(field);
                    m_Content.Fields = list.ToArray();
                }
                else
                {
                    m_Content[property].Property = property;
                    m_Content[property].Value = value == null ? null : value.ToString();
                }
                m_Root.CurrentVisitor.Data.Apply("wim_FilterInfo", Sushi.Mediakiwi.Data.Content.GetSerialized(m_Content));
            }

            public void ClearFilter()
            {
                m_Root.CurrentVisitor.Data.Apply("wim_FilterInfo", null);
            }

            /// <summary>
            /// Adds the element.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="property">The property.</param>
            /// <param name="isVisible">if set to <c>true</c> [is visible].</param>
            /// <param name="isEditable">if set to <c>true</c> [is editable].</param>
            /// <param name="contentInfo">The content info.</param>
            public void AddElement(object sender, string property, bool isVisible, bool isEditable, IContentInfo contentInfo)
            {
                AddElement(sender, property, isVisible, isEditable, contentInfo, null, false);
            }

            /// <summary>
            /// Adds the element.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="property">The property.</param>
            /// <param name="isVisible">if set to <c>true</c> [is visible].</param>
            /// <param name="isEditable">if set to <c>true</c> [is editable].</param>
            /// <param name="contentInfo">The content info.</param>
            /// <param name="value">The value.</param>
            public void AddElement(object sender, string property, bool isVisible, bool isEditable, IContentInfo contentInfo, object value)
            {
                AddElement(sender, property, isVisible, isEditable, contentInfo, value, true);
            }

            /// <summary>
            /// Adds the element.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="property">The property.</param>
            /// <param name="value">The value.</param>
            /// <param name="valueIsSet">if set to <c>true</c> [value is set].</param>
            public void AddElement(object sender, Sushi.Mediakiwi.Data.Property property, object value, bool valueIsSet)
            {
                bool isEditable = !property.IsOnlyRead;

                if (!m_Root.IsEditMode)
                    isEditable = false;

                if (m_Root.m_infoList == null)
                {
                    m_Root.m_infoList = new List<Sushi.Mediakiwi.Beta.GeneratedCms.Source.Component.ListInfoItem>();
                    if (this.CustomDataInstance == null)
                        this.CustomDataInstance = new CustomData();
                }

                Sushi.Mediakiwi.Beta.GeneratedCms.Source.Component.ListInfoItem tmp = new Sushi.Mediakiwi.Beta.GeneratedCms.Source.Component.ListInfoItem();
                //tmp.ContentAttribute = contentInfo;
                tmp.SenderInstance = sender;

                if (sender != null && !string.IsNullOrEmpty(property.FieldName))
                    tmp.Info = sender.GetType().GetProperty(property.FieldName);

                tmp.IsVisible = !property.IsHidden;
                tmp.IsEditable = isEditable;

                if (tmp.Info == null)
                {
                    tmp.Info = this.GetType().GetProperty("CustomDataInstance");
                    tmp.SenderInstance = this;
                    tmp.IsVirtualProperty = true;
                    tmp.Property = property;
                    tmp.Name = property.FieldName;

                    tmp.ContentAttribute = property.GetContentInfo();

                    if (CustomDataInstance.HasProperty(property.FieldName))
                    {
                        if (valueIsSet)
                            CustomDataInstance.ApplyObject(property.FieldName, value);
                    }
                    else
                    {
                        CustomDataInstance.ApplyObject(property.FieldName, value);
                    }
                }
                m_Root.m_infoList.Add(tmp);
            }

            /// <summary>
            /// Adds the element.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="property">The property.</param>
            /// <param name="isVisible">if set to <c>true</c> [is visible].</param>
            /// <param name="isEditable">if set to <c>true</c> [is editable].</param>
            /// <param name="contentInfo">The content info.</param>
            /// <param name="value">The value.</param>
            /// <param name="valueIsSet">if set to <c>true</c> [value is set].</param>
            public void AddElement(object sender, string property, bool isVisible, bool isEditable, IContentInfo contentInfo, object value, bool valueIsSet)
            {
                if (!m_Root.IsEditMode)
                    isEditable = false;

                if (string.IsNullOrEmpty(property))
                    throw new Exception(string.Format("Property for title ['{0}'] can not be NULL", contentInfo.Title));

                if (m_Root.m_infoList == null)
                {
                    m_Root.m_infoList = new List<Sushi.Mediakiwi.Beta.GeneratedCms.Source.Component.ListInfoItem>();
                    if (this.CustomDataInstance == null)
                        this.CustomDataInstance = new CustomData();
                }

                Sushi.Mediakiwi.Beta.GeneratedCms.Source.Component.ListInfoItem tmp = new Sushi.Mediakiwi.Beta.GeneratedCms.Source.Component.ListInfoItem();
                tmp.ContentAttribute = contentInfo;
                tmp.SenderInstance = sender;
                
                if (sender != null && !string.IsNullOrEmpty(property))
                    tmp.Info = sender.GetType().GetProperty(property);

                tmp.IsVisible = isVisible;
                tmp.IsEditable = isEditable;

                if (tmp.Info == null)
                {
                    tmp.Info = this.GetType().GetProperty("CustomDataInstance");
                    tmp.SenderInstance = this;
                    tmp.IsVirtualProperty = true;
                    tmp.Property = new Property();
                    tmp.Name = property;

                    if (CustomDataInstance.HasProperty(property))
                    {
                        if (valueIsSet)
                            CustomDataInstance.ApplyObject(property, value);
                    }
                    else
                    {
                        CustomDataInstance.ApplyObject(property, value);
                    }
                }

                m_Root.m_infoList.Add(tmp);
            }

            /// <summary>
            /// Gets the value.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="property">The property.</param>
            /// <returns></returns>
            public CustomDataItem GetValue(object sender, string property)
            {
                if (CustomDataInstance != null && CustomDataInstance.HasProperty(property))
                    return CustomDataInstance[property];

                if (sender == null)
                {
                    return new CustomDataItem() { Value = null, Property = property };
                }

                CustomDataItem tmp = new CustomDataItem();
                object value =
                    sender.GetType().GetProperty(property) == null ? null :
                    sender.GetType().GetProperty(property).GetValue(sender, null);
                tmp.Value = value == null ? null : value.ToString();
                tmp.Property = property;

                return tmp;
            }

            #region Translation
            /// <summary>
            /// Gets the translation master value.
            /// </summary>
            /// <param name="property">The property.</param>
            /// <returns></returns>
            public string GetTranslationMaster(string property)
            {
                TranslationLastRequest = property;

                if (m_TranslationValues == null) return null;
                if (!m_TranslationValues.ContainsKey(property))
                    return null;

                object x = m_TranslationValues[property];
                return x == null ? null : x.ToString();
            }

            internal string TranslationLastRequest { get; set; }

            string m_TranslationSectionStart;
            /// <summary>
            /// Gets or sets the translation section start.
            /// </summary>
            /// <value>The translation section start.</value>
            public string TranslationSectionStart {
                get { return m_TranslationSectionStart; }
                set {
                    m_TranslationSectionStart = value; }
            }

            /// <summary>
            /// Gets or sets the list item element list.
            /// </summary>
            /// <value>The list item element list.</value>
            public List<Sushi.Mediakiwi.Beta.GeneratedCms.Source.Component.ListInfoItem> ListItemElementList { get; set; }

            System.Collections.Generic.Dictionary<string, object> m_TranslationValues;
            bool m_ShowTranslationData;
            internal bool m_ShowTranslationDataStart;
            internal bool ShowTranslationData {
                get {

                    if (TranslationSectionStart != null && m_ShowTranslationDataStart)
                    {
                        if (TranslationLastRequest != TranslationSectionStart)
                            return false;
                        else
                        {
                            m_ShowTranslationDataStart = false;
                        }
                    }
                    return m_ShowTranslationData; 
                }
                set {
                    m_ShowTranslationData = value; }
            }

            /// <summary>
            /// Adds the translation master.
            /// </summary>
            /// <param name="property">The property.</param>
            /// <param name="value">The value.</param>
            public void AddTranslationMaster(string property, object value)
            {
                if (m_TranslationValues == null)
                    m_TranslationValues = new Dictionary<string, object>();

                if (m_TranslationValues.ContainsKey(property))
                    m_TranslationValues[property] = value;
                else
                    m_TranslationValues.Add(property, value);

                this.ShowTranslationData = true;
            }
            #endregion Translation

            public void AddTextElement(string property, string title, int maxLength)
            {
                AddTextElement(property, title, maxLength, false);
            }

            public void AddTextElement(string property, string title, int maxLength, bool isMandatory)
            {
                AddTextElement(property, title, maxLength, isMandatory, true);
            }

            public void AddTextElement(string property, string title, int maxLength, bool isMandatory, bool isEditable)
            {
                AddTextElement(property, title, maxLength, isMandatory, isEditable, InputType.Text);
            }

            public void AddTextElement(string property, string title, int maxLength, bool isMandatory, bool isEditable, InputType inputType)
            {
                AddTextElement(property, title, maxLength, isMandatory, isEditable, inputType, null);
            }

            public void AddTextElement(string property, string title, int maxLength, bool isMandatory, bool isEditable, InputType inputType, string interactiveHelp)
            {
                AddTextElement(property, title, maxLength, isMandatory, isEditable, inputType, interactiveHelp, null);
            }

            public void AddTextElement(string property, string title, int maxLength, bool isMandatory, bool isEditable, InputType inputType, string interactiveHelp, object sender)
            {
                IContentInfo info = null;
                info = new Sushi.Mediakiwi.Framework.ContentListItem.TextFieldAttribute(title, maxLength) { TextType = inputType }; 
                if (info == null) return;

                AddElement(sender, property, true, isEditable, info);
            }

            int m_propertyCount;
            public void AddInfoElement(string title, string value)
            {
                m_propertyCount++;
                AddInfoElement(string.Concat("p", m_propertyCount), title, value);
            }

            public void AddInfoElement(string property, string title, string value)
            {
                IContentInfo info = null;
                info = new Sushi.Mediakiwi.Framework.ContentListItem.TextLineAttribute(title);
                if (info == null) return;

                AddElement(null, property, true, true, info, value, true);
            }

            /// <summary>
            /// Gets or sets the custom data.
            /// </summary>
            /// <value>The custom data.</value>
            public Data.CustomData CustomDataInstance { get; set; }

            public void ApplyCustomDataInstance(System.Xml.Linq.XElement element)
            {
                this.CustomDataInstance = new CustomData();
                if (element != null)
                    this.CustomDataInstance.ApplySerialized(element.ToString());
            }

        }
        
        internal List<Sushi.Mediakiwi.Beta.GeneratedCms.Source.Component.ListInfoItem> m_infoList;


        /// <summary>
        /// 
        /// </summary>
        [Obsolete("This property is obsolete, please use ListData or ListDataTable instead")]
        public DataTable Data
        {
            set { m_ListDataTable = value; }
            get { return m_ListDataTable; }
        }


        int? m_CurrentPage;
        /// <summary>
        /// Gets or sets the current page.
        /// </summary>
        /// <value>
        /// The current page.
        /// </value>
        public int CurrentPage
        {
            get
            {
                if (!m_CurrentPage.HasValue)
                {
                    var set = Context.Request.Query["set"].ToString();
                    if (!string.IsNullOrEmpty(set) && set.Contains(','))
                    {
                        set = set.Split(',')[0];
                    }

                    if (set == "all")
                        m_CurrentPage = -1;
                    else
                    {
                        m_CurrentPage = Utility.ConvertToInt(set, 1);
                    }
                }
                return m_CurrentPage.Value;
            }
            set
            {
                m_CurrentPage = value;
            }
        }

        /// <summary>
        /// Class for multiple grids
        /// </summary>
        internal class GridInstance
        {
            public GridInstance()
            { }

            public void ApplyListDataColumns(Sushi.Mediakiwi.Framework.ListDataColumns listDataColumns)
            {
                m_ListDataColumns = new ListDataColumns();
                ListDataColumn[] arr = new ListDataColumn[listDataColumns.List.Count];
                listDataColumns.List.CopyTo(arr);
                foreach (var i in arr)
                    m_ListDataColumns.Add(i);
            }

            internal IList m_ListData { get; set; }
            internal IList m_AppliedSearchGridItem;
            internal IList m_ChangedSearchGridItem;
            internal bool m_IsLinqUsed;
            internal int m_ListDataRecordCount;
            internal int m_ListDataRecordPageCount;
            internal Sushi.Mediakiwi.Framework.ListDataColumns m_ListDataColumns;
            internal string m_DataTitle;
        }

        private Sushi.Mediakiwi.Framework.ListDataColumns m_ListDataColumnsBackup;
        /// <summary>
        /// Set the backup grid for when having multiple grids.
        /// </summary>
        void ApplyListDataColumnBackup()
        {
            m_ListDataColumnsBackup = new ListDataColumns();
            ListDataColumn[] arr = new ListDataColumn[this.ListDataColumns.List.Count];
            this.ListDataColumns.List.CopyTo(arr);
            foreach (var i in arr)
                m_ListDataColumnsBackup.Add(i);
        }

        /// <summary>
        /// If miltiple grids exists the columns could be cleared after applying the first grid, so make a backup of it.
        /// </summary>
        void CheckListDataColumnBackup()
        {
            if (_Grids == null || _Grids.Count == 0)
                return;

            this.ListDataColumns = this.m_ListDataColumnsBackup;
        }

        internal int _index = 0;

        /// <summary>
        /// When having multiple grids on one page, this method forces the load of the next grid.
        /// </summary>
        /// <returns></returns>
        internal bool NextGrid()
        {
            if (_index == -1)
            {
                this.CheckListDataColumnBackup();
                _index++;
                return true;
            }

            if (_Grids == null)
                return false;

            if (_Grids.Count > _index)
            {
                this.ListData = _Grids[_index].m_ListData;
                this.m_DataTitle = _Grids[_index].m_DataTitle;
                this.m_IsLinqUsed = _Grids[_index].m_IsLinqUsed;
                this.m_ListDataRecordCount = _Grids[_index].m_ListDataRecordCount;
                this.m_ListDataRecordPageCount = _Grids[_index].m_ListDataRecordPageCount;
                this.m_AppliedSearchGridItem = _Grids[_index].m_AppliedSearchGridItem;
                this.m_ChangedSearchGridItem = _Grids[_index].m_ChangedSearchGridItem;
                this.ListDataColumns = _Grids[_index].m_ListDataColumns;
                _index++;
                return true;
            }
            return false;
        }

        internal List<GridInstance> _Grids;

        // [MR:010713] Returns the number of items in the Grid.
        /// <summary>
        /// Returns the number of items in the Grid
        /// </summary>
        public int GridCount
        {
            get 
            {
                if (_Grids == null)
                    return 0;
                else return _Grids.Count;
            }
        }

        private Sushi.Mediakiwi.Framework.ListDataColumns m_ListDataColumns;
        /// <summary>
        /// The columns that are used in the search result
        /// </summary>
        public Sushi.Mediakiwi.Framework.ListDataColumns ListDataColumns
        {
            set { m_ListDataColumns = value; }
            get
            {
                if (m_ListDataColumns == null)
                    m_ListDataColumns = new Sushi.Mediakiwi.Framework.ListDataColumns();
                return m_ListDataColumns;
            }
        }


        /// <summary>
        /// The search result. Add columns by using ListDataColumns.Add
        /// </summary>
        [Obsolete("[20120715:MM] Please use ListDataAdd", false)]
        internal IList ListData { get; set; }

        internal bool m_IsListDataScrollable;
        internal string m_DataTitle;
        internal bool m_IsLinqUsed;
        internal int m_ListDataRecordCount;
        internal int m_ListDataRecordPageCount;

        public IList m_AppliedSearchGridItem;
        public IList AppliedSearchGridItem { get { return m_AppliedSearchGridItem; } }

        public IList m_ChangedSearchGridItem;
        public IList ChangedSearchGridItem { get { return m_ChangedSearchGridItem; } }
        
        internal int m_ListDataInterLineCount;

        /// <summary>
        /// Add another IEnumerable list of data entities to the grid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        public void ListDataAdd<T>(IEnumerable<T> source)
        {
            ListDataAdd<T>(source, null);
        }

        /// <summary>
        /// Add another IEnumerable list of data entities to the grid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="title">The title.</param>
        public void ListDataAdd<T>(IEnumerable<T> source, string title)
        {
            if (source == null) return;

            if (_Grids == null)
                _Grids = new List<GridInstance>();

            GridInstance grid = new GridInstance();
            int step = GridDataCommunication.PageSize;

            grid.m_DataTitle = title;
            grid.m_IsLinqUsed = true;
            grid.m_ListDataRecordCount = GridDataCommunication.ResultCount.HasValue ? GridDataCommunication.ResultCount.Value : source.Count();

            grid.m_ListDataRecordPageCount = grid.m_ListDataRecordCount == 0 ? 0 : Convert.ToInt32(decimal.Ceiling(((decimal)grid.m_ListDataRecordCount / (decimal)step)));

            if (!GridDataCommunication.ResultCount.HasValue)
            {
                int page = CurrentPage - 1;
                if (page < 0)
                {
                    grid.m_ListData = source.ToArray();
                }
                else if (page > 0)
                {
                    int start = (page * step);
                    if (start >= grid.m_ListDataRecordCount)
                        grid.m_ListData = source.Take(step).ToArray();
                    else
                        grid.m_ListData = source.Skip(start).Take(step).ToArray();
                }
                else
                    grid.m_ListData = source.Take(step).ToArray();
            }
            else
            {
                if (GridDataCommunication.ShowAll)
                    grid.m_ListData = source.ToArray();
                else
                    grid.m_ListData = source.Take(step).ToArray();
            }

            this.m_AppliedSearchGridItem = grid.m_ListData;

            grid.ApplyListDataColumns(this.ListDataColumns);
            //  Clone the current grid for when using multiple grids
            _Grids.Add(grid);
        }

        /// <summary>
        /// Add an IEnumerable list of data entities to the grid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        public void ListDataApply<T>(IEnumerable<T> source)
        {
            ListDataApply<T>(source, null);
        }

        /// <summary>
        /// Lists the data apply.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="isScrollable">if set to <c>true</c> [is scrollable].</param>
        public void ListDataApply<T>(IEnumerable<T> source, bool isScrollable)
        {
            ListDataApply<T>(source, null, isScrollable);
        }

        /// <summary>
        /// Lists the data apply.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="title">The title.</param>
        public void ListDataApply<T>(IEnumerable<T> source, string title)
        {
            ListDataApply<T>(source, title, false);
        }

        /// <summary>
        /// Add an IEnumerable list of data entities to the grid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="title">The title of the datagrid.</param>
        public void ListDataApply<T>(IEnumerable<T> source, string title, bool isScrollable)
        {
            if (source == null) return;

            if (this.ListData != null)
                this.Notification.AddError(string.Format("The method [ListDataApply<T>(IEnumerable)] is called multiple times; this should only be called once, please correct this."));

            int step = GridDataCommunication.PageSize;// this.CurrentList.Option_Search_MaxResultPerPage;

            this.m_IsListDataScrollable = isScrollable;
            this.m_DataTitle = title;
            this.m_IsLinqUsed = true;

            this.m_ListDataRecordCount = GridDataCommunication.ResultCount.HasValue ? GridDataCommunication.ResultCount.Value : source.Count();

            this.m_ListDataRecordPageCount = this.m_ListDataRecordCount == 0 ? 0 : Convert.ToInt32(decimal.Ceiling(((decimal)this.m_ListDataRecordCount / (decimal)step)));

            if (!GridDataCommunication.ResultCount.HasValue)
            {
                int page = CurrentPage - 1;
                if (page < 0)
                {
                    this.ListData = source.ToArray();
                }
                else if (page > 0)
                {
                    int start = (page * step);
                    if (start >= this.m_ListDataRecordCount)
                        this.ListData = source.Take(step).ToArray();
                    else
                        this.ListData = source.Skip(start).Take(step).ToArray();
                }
                else
                    this.ListData = source.Take(step).ToArray();
            }
            else
            {
                if (GridDataCommunication.ShowAll)
                    this.ListData = source.ToArray();
                else
                    this.ListData = source.Take(step).ToArray();
            }

            this.m_AppliedSearchGridItem = this.ListData;
            this.ApplyListDataColumnBackup();
        }

        /// <summary>
        /// Add an IQueryable list of data entities to the grid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        public void ListDataApply<T>(IQueryable<T> source)
        {
            ListDataApply<T>(source, null);
        }

        /// <summary>
        /// Add an IQueryable list of data entities to the grid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        public void ListDataApply<T>(IQueryable<T> source, string title)
        {
            if (source == null) return;
            if (this.ListData != null)
                this.Notification.AddError(string.Format("The method [ListDataApply<T>(IQueryable)] is called multiple times; this should only be called once, please correct this."));

            int step = this.CurrentList.Option_Search_MaxResultPerPage;

            //context.Log = Context.Response.Output;
            this.m_DataTitle = title;
            this.m_IsLinqUsed = true;
            this.m_ListDataRecordCount = source.Count();
            this.m_ListDataRecordPageCount = this.m_ListDataRecordCount == 0 ? 0 : Convert.ToInt32(decimal.Ceiling(((decimal)this.m_ListDataRecordCount / (decimal)step)));

            int page = CurrentPage - 1;
            if (page < 0)
            {
                this.ListData = source.ToArray();
            }
            else if (page > 0)
            {
                int start = (page * step);
                if (start >= this.m_ListDataRecordCount)
                    this.ListData = source.Take(step).ToArray();
                else
                    this.ListData = source.Skip(start).Take(step).ToArray();
            }
            else
                this.ListData = source.Take(step).ToArray();

            this.m_AppliedSearchGridItem = this.ListData;
        }

        /// <summary>
        /// Add another IQueryable list of data entities to the grid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        public void ListDataAdd<T>(IQueryable<T> source)
        {
            ListDataAdd<T>(source, null);
        }

        /// <summary>
        /// Add another IQueryable list of data entities to the grid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="title">The title.</param>
        public void ListDataAdd<T>(IQueryable<T> source, string title)
        {
            if (source == null) return;

            if (_Grids == null)
                _Grids = new List<GridInstance>();

            GridInstance grid = new GridInstance();

            int step = this.CurrentList.Option_Search_MaxResultPerPage;

            //context.Log = Context.Response.Output;
            grid.m_DataTitle = title;
            grid.m_IsLinqUsed = true;
            grid.m_ListDataRecordCount = source.Count();
            grid.m_ListDataRecordPageCount = this.m_ListDataRecordCount == 0 ? 0 : Convert.ToInt32(decimal.Ceiling(((decimal)grid.m_ListDataRecordCount / (decimal)step)));

            int page = CurrentPage - 1;
            if (page < 0)
            {
                grid.m_ListData = source.ToArray();
            }
            else if (page > 0)
            {
                int start = (page * step);
                if (start >= grid.m_ListDataRecordCount)
                    grid.m_ListData = source.Take(step).ToArray();
                else
                    grid.m_ListData = source.Skip(start).Take(step).ToArray();
            }
            else
                grid.m_ListData = source.Take(step).ToArray();

            grid.m_AppliedSearchGridItem = grid.m_ListData;

            grid.ApplyListDataColumns(this.ListDataColumns);
            //  Clone the current grid for when using multiple grids
            _Grids.Add(grid);
        }

        private DataTable m_ListDataTable;
        /// <summary>
        /// The search result. Add columns by using ListDataColumns.Add
        /// </summary>
        public DataTable ListDataTable
        {
            set { m_ListDataTable = value; }
            get { return m_ListDataTable; }
        }

        private DataTable m_GraphDataTable;
        /// <summary>
        /// The search result. Add columns by using ListDataColumns.Add
        /// </summary>
        internal DataTable GraphDataTable
        {
            set { m_GraphDataTable = value; }
            get { return m_GraphDataTable; }
        }



        [Obsolete("This is not valid any more in mediakiwi", false)]
        public string ListDataIsInterlinePropertyValue { get; set; }
        [Obsolete("This is not valid any more in mediakiwi", false)]
        public string ListDataInterlineTextPropertyValue { get; set; }

        /// <summary>
        /// Gets or sets the list data highlight property value.
        /// This (boolean) poperty will be called be ListData record. If true the Table row will get a higlighted state.
        /// </summary>
        /// <value>The list data highlight property value.</value>
        [Obsolete("This is not valid any more in mediakiwi", false)]
        public string ListDataHighlightPropertyValue { get; set; }

        /// <summary>
        /// Gets or sets the list data table row className property value.
        /// </summary>
        /// <value>The list data highlight property value.</value>
        [Obsolete("This is not valid any more in mediakiwi", false)]
        public string ListDataClassNamePropertyValue { get; set; }
        
        private List<string> m_ResetValueList;
        /// <summary>
        /// These properties will be reset after the initial content has been applied. This can be used for dropdown dependency, et al.
        /// Applied values are the names of the properties.
        /// NOTE: This doesn't change values for OnSave and should be used for AutoPostback (dropdownlist) and Custom action buttons.
        /// </summary>
        public List<string> ListDataDependendProperties
        {
            set
            {
                m_ResetValueList = value;
            }
            get
            {
                if (m_ResetValueList == null)
                    m_ResetValueList = new List<string>();
                return m_ResetValueList;
            }
        }

        public IComponentListTemplate DashBoardFilterTemplate { get; set; }

        int m_DashBoardElementWidth = 1;
        public int DashBoardElementWidth { get { return m_DashBoardElementWidth; } set { m_DashBoardElementWidth = value; } }

        private string m_DashBoardHtmlContainer;
        /// <summary>
        /// Gets or sets the dash board HTML container.
        /// </summary>
        /// <value>The dash board HTML container.</value>
        public string DashBoardHtmlContainer
        {
            set { m_DashBoardHtmlContainer = value; }
            get { return m_DashBoardHtmlContainer; }
        }

        /// <summary>
        /// Gets or sets the height of the dash board element (options: 0,1,2 [0=free fill] ).
        /// </summary>
        /// <value>The height of the dash board element.</value>
        public int DashBoardElementHeight { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [dash board element is visible].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [dash board element is visible]; otherwise, <c>false</c>.
        /// </value>
        public bool DashBoardElementIsVisible { get; set; }

        NoDataItem _NoData;
        public NoDataItem NoData 
        { 
            get { if (_NoData == null) _NoData = new NoDataItem(); return _NoData;  }  
        }


     
        /// <summary>
        /// Gets or sets a value indicating whether [dash board can click through].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [dash board can click through]; otherwise, <c>false</c>.
        /// </value>
        public bool DashBoardCanClickThrough { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [dash board show filter section].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [dash board show filter section]; otherwise, <c>false</c>.
        /// </value>
        public bool DashBoardShowFilterSection { get; set; }

        /// <summary>
        /// Gets or sets the dash board top HML container.
        /// </summary>
        /// <value>The dash board top HML container.</value>
        public string DashBoardTopHmlContainer { get; set; }

        private string m_CurrentListRequestProperty;
        /// <summary>
        /// Gets or sets the current list request property.
        /// </summary>
        /// <value>The current list request property.</value>
        public string CurrentListRequestProperty
        {
            set { m_CurrentListRequestProperty = value; }
            get { return m_CurrentListRequestProperty; }
        }
        /// <summary>
        /// Determines whether this instance has focus.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance has focus; otherwise, <c>false</c>.
        /// </returns>
        public bool HasFocus(object entity)
        {
            return entity.GetType().ToString() == CurrentList.ClassName; 
        }

        public string FirstSectionTitle { get; set; }

        internal string m_ListTitle;
        /// <summary>
        /// The title of the list shown in the element overview form (you can also use [ListInfoApply(string title, string subtitle, string description)])
        /// </summary>
        public string ListTitle
        {
            set
            {
                m_ListTitle = value; 
            }
            get {
                if (m_ListTitle == null)
                {
                    if (Console != null && Console.Item.HasValue)
                        m_ListTitle = this.CurrentList.SingleItemName;
                    else
                        m_ListTitle = this.CurrentList.Name;
                }
                return m_ListTitle;
            }
        }

        /// <summary>
        /// Lists the information apply 
        /// </summary>
        /// <param name="title">The title of the current list (if null this is skipped).</param>
        /// <param name="subtitle">The subtitle of the current list (if null this is skipped).</param>
        /// <param name="description">The description of the current list (if null this is skipped).</param>
        public void ListInfoApply(string title, string subtitle, string description)
        {
            this.ListTitle = title;

            if (!string.IsNullOrEmpty(subtitle) && !string.IsNullOrEmpty(description))
                this.ListDescription = string.Concat(string.Format("<h2>{0}</h2>", subtitle), description);

            else if (!string.IsNullOrEmpty(subtitle) && string.IsNullOrEmpty(description))
                this.ListDescription = string.Format("<h2>{0}</h2>", subtitle);

            else if (string.IsNullOrEmpty(subtitle) && !string.IsNullOrEmpty(description))
                this.ListDescription = description;
        }

        internal string m_ListDescription;
        /// <summary>
        /// The title of the list shown in the element overview form (you can also use [ListInfoApply(string title, string subtitle, string description)])
        /// </summary>
        public string ListDescription
        {
            set
            {
                m_ListDescription = value;
            }
            get
            {
                if (string.IsNullOrEmpty(m_ListDescription))
                    m_ListDescription = this.CurrentList.Description;
                return m_ListDescription;
            }
        }

        //private int m_SearchViewMaxLength;
        ///// <summary>
        ///// The maximum record count shown per paged view (default value: 15).
        ///// </summary>
        //internal int SearchViewMaxLength
        //{
        //    set { m_SearchViewMaxLength = value; }
        //    get
        //    {
        //        if (m_SearchViewMaxLength == 0)
        //        {
        //            if (CurrentList == null)
        //                m_SearchViewMaxLength = 15;
        //            else
        //                m_SearchViewMaxLength = CurrentList.Option_Search_MaxResultPerPage;
        //        }
        //        return m_SearchViewMaxLength;
        //    }
        //}

        private int m_SearchViewDashboardMaxLength = 50;
        /// <summary>
        /// The maximum record count shown per view on the dashboard (default value: 50).
        /// </summary>
        public int SearchViewDashboardMaxLength
        {
            set { m_SearchViewDashboardMaxLength = value; }
            get { return m_SearchViewDashboardMaxLength; }
        }

        ///// <summary>
        ///// The maximum record count stored in a search result (default value: 500).
        ///// </summary>
        //internal int SearchListMaxLength
        //{
        //    get { 
        //        if (CurrentList == null)
        //            return 500;
        //        return CurrentList.Data["wim_MaxViews"].ParseInt().GetValueOrDefault(500);
        //     }
        //}


        [Obsolete("Please use [wim.Page.Body.Grid.SetClickLayer()]", false)]
        public LayerSize SearchListTarget  
        { 
            set {
                var specification = new Grid.LayerSpecification();
                specification.Apply(value);

                this.Page.Body.Grid.SetClickLayer(specification);
            }
        }

        private bool m_SearchListCanClickThrough;
        /// <summary>
        /// Gets or sets the search list can click through.
        /// </summary>
        /// <value>The search list can click through.</value>
        public bool SearchListCanClickThrough
        {
            set { m_SearchListCanClickThrough = value; }
            get {
                if (!m_SearchListCanClickThrough && this.IsSortOrderMode)
                    m_SearchListCanClickThrough = true;

                return m_SearchListCanClickThrough; 
            }
        }

        private int? m_ListCreateNewItemListId;
        /// <summary>
        /// When the NEW RECORDS buttons is pressed it deeplinks to a list item. This list can defer from the originating list by applying this other componentListId. 
        /// </summary>
        public int? ListCreateNewItemListId
        {
            set { m_ListCreateNewItemListId = value; }
            get { return m_ListCreateNewItemListId; }
        }

        private bool m_HasDataDependency;
        /// <summary>
        /// Gets or sets a value indicating whether this instance has data dependency. 
        /// If data dependeny is set to 'true' the ListLoad event will be triggered twice.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has data dependency; otherwise, <c>false</c>.
        /// </value>
        public bool HasDataDependency
        {
            set { m_HasDataDependency = value; }
            get { return m_HasDataDependency; }
        }


        private bool m_HasSortOrder;
        /// <summary>
        /// Does this search list have the sortorder functionality? (this overrides the automatic visibility of the setSortOrder method)
        /// </summary>
        public bool HasSortOrder
        {
            set { m_HasSortOrder = value; }
            get { return m_HasSortOrder; }
        }

        private bool m_HasSingleItemSortOrder;
        /// <summary>
        /// Does this search list have the sortorder functionality in the form mode? (this overrides the automatic visibility of the setSortOrder method)
        /// </summary>
        public bool HasSingleItemSortOrder
        {
            set { m_HasSingleItemSortOrder = value; }
            get { return m_HasSingleItemSortOrder; }
        }

        private bool m_AutoExpression;
        /// <summary>
        /// Gets or sets a value indicating whether [auto expression].
        /// </summary>
        /// <value><c>true</c> if [auto expression]; otherwise, <c>false</c>.</value>
        public bool AutoExpression
        {
            set { m_AutoExpression = value; }
            get { return m_AutoExpression; }
        }

        /// <summary>
        /// Gets the postback value.
        /// </summary>
        /// <value>The postback value.</value>
        public string PostbackValue
        {
            get {
                string value = "";
                if (Context == null) return value;

                value = Console.Form("autopostback");
                if (value == null) value = "";
                return value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has subscribe option.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has subscribe option; otherwise, <c>false</c>.
        /// </value>
        internal bool HasSubscribeOption
        {
            get
            {
                if (CurrentList == null)
                    return false;
                return CurrentList.Data["wim_hasSubscribeOption"].ParseBoolean();
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance has export option PDF.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has export option PDF; otherwise, <c>false</c>.
        /// </value>
        internal bool HasExportOptionPDF
        {
            get {
                if (CurrentList == null)
                    return true;
                return CurrentList.Data["wim_hasExport_PDF"].ParseBoolean();
            }
        }

        ///// <summary>
        ///// Gets the temporary file path.
        ///// </summary>
        ///// <param name="filename">The filename.</param>
        ///// <returns></returns>
        //public string GetTemporaryFilePath(string filename)
        //{
        //    string candidate = null;
        //    return GetTemporaryFilePath(filename, true, out candidate);
        //}

        ///// <summary>
        ///// Gets the temporary file path.
        ///// </summary>
        ///// <param name="filename">The filename.</param>
        ///// <param name="addToAutoDownloadUrl">if set to <c>true</c> [add to auto download URL].</param>
        ///// <returns></returns>
        //public string GetTemporaryFilePath(string filename, bool addToAutoDownloadUrl)
        //{
        //    string candidate = null;
        //    return GetTemporaryFilePath(filename, addToAutoDownloadUrl, out candidate);
        //}

        ///// <summary>
        ///// Gets the package file path.
        ///// </summary>
        ///// <param name="filename">The filename.</param>
        ///// <param name="addToAutoDownloadUrl">if set to <c>true</c> [add to auto download URL].</param>
        ///// <returns></returns>
        //public string GetPackageFilePath(string filename, bool addToAutoDownloadUrl)
        //{
        //    string candidate = null;
        //    return GetPackageFilePath(filename, addToAutoDownloadUrl, out candidate);
        //}

        ///// <summary>
        ///// Gets the package file path.
        ///// </summary>
        ///// <param name="filename">The filename.</param>
        ///// <param name="addToAutoDownloadUrl">if set to <c>true</c> [add to auto download URL].</param>
        ///// <param name="newFileName">New name of the file.</param>
        ///// <returns></returns>
        //public string GetPackageFilePath(string filename, bool addToAutoDownloadUrl, out string newFileName)
        //{
        //    Regex clean = new Regex(@"[^A-Z0-9_\.]", RegexOptions.IgnoreCase);
        //    string candidate = clean.Replace(filename.Replace(" ", "_"), string.Empty);

        //    string folder = _Console.Request.MapPath(Utility.AddApplicationPath(Wim.CommonConfiguration.RelativeRepositoryPackageUrl));
        //    if (!System.IO.Directory.Exists(folder))
        //        System.IO.Directory.CreateDirectory(folder);

        //    string path = _Console.Request.MapPath(Utility.AddApplicationPath(string.Concat(Wim.CommonConfiguration.RelativeRepositoryPackageUrl, candidate)));

        //    if (addToAutoDownloadUrl)
        //        this.CurrentVisitor.Data.Apply("wim_export", candidate);
        //    newFileName = candidate;

        //    return path;
        //}

        /// <summary>
        /// Gets the current query URL with a certain keys replaced.
        /// </summary>
        /// <returns></returns>
        public string GetUrl(params KeyValue[] keyvalues)
        {
            return Console.UrlBuild.GetUrl(keyvalues);
        }

        /// <summary>
        /// Gets the current query URL with a certain keys replaced.
        /// </summary>
        /// <returns></returns>
        public string GetUrl(int listId, params KeyValue[] keyvalues)
        {
            return Console.UrlBuild.GetUrl(listId, keyvalues);
        }

        /// <summary>
        /// Gets the current query URL with a certain keys replaced.
        /// </summary>
        /// <returns></returns>
        public string GetUrl(IComponentList list, params KeyValue[] keyvalues)
        {
            return Console.UrlBuild.GetUrl(list, keyvalues);
        }

        /// <summary>
        /// Gets the current query URL with a certain keys replaced.
        /// </summary>
        /// <returns></returns>
        public string GetUrl(Type listType, params KeyValue[] keyvalues)
        {
            return Console.UrlBuild.GetUrl(listType, keyvalues);
        }

        /// <summary>
        /// Gets a value indicating whether this instance has export option XLS.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has export option XLS; otherwise, <c>false</c>.
        /// </value>
        internal bool HasExportOptionXLS
        {
            get
            {
                if (CurrentList == null)
                    return true;
                return CurrentList.Data["wim_hasExport_XLS"].ParseBoolean();
            }
        }

        internal string m_sortOrderSqlKey;
        internal string m_sortOrderSqlTable;
        internal string m_sortOrderSqlColumn;

        /// <summary>
        /// Sets the sortOrder SQL specification: This requires to be set in the Constructor of the class.
        /// The listdataColumn should also be set to Wim.Templates.ListDataColumnType.SortOrderIndentifier.
        /// </summary>
        /// <param name="sqlTable">The SQL table.</param>
        /// <param name="sqlKeyColumn">The SQL key column.</param>
        /// <param name="sqlSortOrderColumn">The SQL sort order column.</param>
        public void SetSortOrder(string sqlTable, string sqlKeyColumn, string sqlSortOrderColumn)
        {
            m_sortOrderSqlTable = sqlTable;
            m_sortOrderSqlKey = sqlKeyColumn;
            m_sortOrderSqlColumn = sqlSortOrderColumn;
            m_HasSortOrder = true;
        }

        Beta.GeneratedCms.Console _Console;
        /// <summary>
        /// Gets or sets the console.
        /// </summary>
        /// <value>The console.</value>
        public Beta.GeneratedCms.Console Console
        {
            set { _Console = value; }
            get { return _Console; }
        }

        internal bool _IsRedirected;
        /// <summary>
        /// Redirect to an URL and inform the backend to stop processing this page
        /// </summary>
        /// <param name="url"></param>
        public void Redirect(string url)
        {
            _IsRedirected = true;
            this.Console.Response.Redirect(url, false);
        }

        public string AddApplicationPath(string path, bool appendUrl = false)
        {
            return this.Console.AddApplicationPath(path, appendUrl);
        }

        public void SaveVisit()
        {
            Console.SaveVisit();
        }

        /// <summary>
        /// Gets the current URL Parameters (without the ?). If the itemID is set to NULL the item=X will not be added (usable for SearchResultItemPassthroughParameter)
        /// </summary>
        /// <param name="itemID">The item ID.</param>
        /// <returns></returns>
        public string GetCurrentUrlParameters(int? itemID)
        {
            string url = "";
            AddToUrl(ref url, "list");
            AddToUrl(ref url, "folder");
            AddToUrl(ref url, "base");
            AddToUrl(ref url, "group");
            AddToUrl(ref url, "groupitem");
            AddToUrl(ref url, "group2");
            AddToUrl(ref url, "group2item");

            if (itemID.HasValue)
                url = string.Concat(url, (string.IsNullOrEmpty(url) ? "" : "&"), "item=", itemID.Value);

            return url;
        }

        void AddToUrl(ref string url, string queryParam)
        {
            if (Console.Request.Query[queryParam].FirstOrDefault() == null)
                return;

            url = string.Concat(url, (string.IsNullOrEmpty(url) ? "" : "&"), queryParam, "=", Console.Request.Query[queryParam]);
        }

        internal string m_SearchResultItemPassthroughParameter;
        /// <summary>
        /// Gets or sets the search result item passthrough parameter. The framework will concat this as follows string.Concat("?", param, "=", KEY). The [KEY] tag can be used as an additional placeholder.
        /// </summary>
        /// <value>The search result item passthrough parameter.</value>
        public string SearchResultItemPassthroughParameter
        {
            set { m_SearchResultItemPassthroughParameter = value; }
            get {
                if (string.IsNullOrEmpty(m_SearchResultItemPassthroughParameter))
                {
                    m_SearchResultItemPassthroughParameter = GetUrl(new KeyValue() { Key = "item", RemoveKey = true });
                    if (m_SearchResultItemPassthroughParameter.Contains('?'))
                        m_SearchResultItemPassthroughParameter += "&item";
                    else
                        m_SearchResultItemPassthroughParameter += "?item";
                }

                return m_SearchResultItemPassthroughParameter; 
            }
        }

        private string m_SearchResultItemPassthroughParameterProperty;
        /// <summary>
        /// Gets or sets the search result item passthrough parameter property which is taken from the ListData collection.
        /// This value overrides 'SearchResultItemPassthroughParameter'. The framework will use the response of the property as the clickthrough URL.
        /// </summary>
        /// <value>The search result item passthrough parameter property.</value>
        public string SearchResultItemPassthroughParameterProperty
        {
            set { m_SearchResultItemPassthroughParameterProperty = value; }
            get { return m_SearchResultItemPassthroughParameterProperty; }
        }

        /// <summary>
        /// Used for migrating from old WIM to new WIM
        /// </summary>
        internal bool CanAddNewItemIsSet;



        private bool m_ShouldPostPreRenderLoadFormRequest;
        /// <summary>
        /// Gets or sets a value indicating that after the PreRender the elements are filled using the POST values. 
        /// The default setting is that the values are filled using the set properties.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [post pre render load form request]; otherwise, <c>false</c>.
        /// </value>
        public bool ShouldPostPreRenderLoadFormRequest
        {
            set { m_ShouldPostPreRenderLoadFormRequest = value; }
            get { return m_ShouldPostPreRenderLoadFormRequest; }
        }

        private bool m_CanAddNewItem;
        /// <summary>
        /// Can the application user create a new record?
        /// </summary>
        public bool CanAddNewItem
        {
            set { CanAddNewItemIsSet = true; m_CanAddNewItem = value; }
            get {

                if (!CanAddNewItemIsSet)
                    m_CanAddNewItem = CurrentList.Data["wim_CanCreate"].ParseBoolean();

                return m_CanAddNewItem; 
            }
        }

        bool m_CanSaveAndAddNew_IsSet;
        private bool m_CanSaveAndAddNew;
        /// <summary>
        /// Show the option of "Save and add new record". Default state is True (only when CanAddNewItem = True).
        /// </summary>
        public bool CanSaveAndAddNew
        {
            set { m_CanSaveAndAddNew = value; m_CanSaveAndAddNew_IsSet = true; }
            get
            {
                if (!this.CanAddNewItem) return false;

                if (!m_CanSaveAndAddNew_IsSet)
                    m_CanSaveAndAddNew = CurrentList.Option_CanSaveAndAddNew;

                return m_CanSaveAndAddNew;
            }
        }

        IContentInfo[] m_ContentInfoItems;
        /// <summary>
        /// Gets or sets the content info items.
        /// </summary>
        /// <value>The content info items.</value>
        public IContentInfo[] ContentInfoItems
        {
            set { m_ContentInfoItems = value; }
            get { return m_ContentInfoItems; }
        }

        /// <summary>
        /// Gets the info item.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public IContentInfo GetInfoItem(string property)
        {
            if (ContentInfoItems == null) return null;
            foreach(IContentInfo info in ContentInfoItems)
            {
                if (info.Property.Name == property)
                    return info;
            }
            return null;
        }

        private bool m_IsCachedSearchResult;
        /// <summary>
        /// This the search result present in cache? If so there is no need for assignment to ListData or ListDataTable.
        /// </summary>
        public bool IsCachedSearchResult
        {
            set { m_IsCachedSearchResult = value; }
            get { return m_IsCachedSearchResult; }
        }

        private bool m_ForceLoad;
        /// <summary>
        /// Force showing a new result set (bypassing the cached version)
        /// </summary>
        public bool ForceLoad
        {
            set { m_ForceLoad = value; }
            get { return m_ForceLoad; }
        }

        internal Sushi.Mediakiwi.Data.Folder m_CurrentFolder;
        /// <summary>
        /// Gets the current folder.
        /// </summary>
        /// <value>The current folder.</value>
        public Sushi.Mediakiwi.Data.Folder CurrentFolder
        {
            get {
                try
                {
                    if (m_CurrentFolder == null)
                    {
                        int folder = Utility.ConvertToInt(Context.Request.Query["folder"]);
                    
                        if (folder == 0)
                        {
                            int page = Utility.ConvertToInt(Context.Request.Query["page"]);
                            if (page > 0)
                            {
                                Sushi.Mediakiwi.Data.Page pageInstance = Sushi.Mediakiwi.Data.Page.SelectOne(page, false);

                                //if (pageInstance.FolderID == 0)
                                //{
                                //    Wim.Utilities.CacheItemManager.FlushIndexOfCacheObjects("Data_Sushi.Mediakiwi.Data.Page");
                                //    pageInstance = Sushi.Mediakiwi.Data.Page.SelectOne(page, false);
                                //}

                                m_CurrentFolder = pageInstance.Folder;
                                return m_CurrentFolder;
                            }

                            Data.Folder folderEntity = null;

                            if (!string.IsNullOrEmpty(Context.Request.Query["gallery"]))
                            {
                                Sushi.Mediakiwi.Data.Gallery gallery = Sushi.Mediakiwi.Data.Gallery
                                    .Identify(Context.Request.Query["gallery"]);

                                if (!(gallery == null || gallery.ID == 0))
                                {
                                    folderEntity = new Folder();
                                    folderEntity.ID = gallery.ID;
                                    folderEntity.ParentID = gallery.ParentID.GetValueOrDefault(0);
                                    folderEntity.Name = gallery.Name;
                                    //  Fix for galleries (add the / at the end for the Level)!
                                    folderEntity.CompletePath = gallery.CompletePath == "/" ? "/" : string.Concat(gallery.CompletePath, "/");
                                    folderEntity.Type = FolderType.Gallery;
                                    //folderEntity.DatabaseMappingPortal = gallery.DatabaseMappingPortal;
                                }
                            }

                            else if (Utility.ConvertToInt(Context.Request.Query["asset"]) > 0)
                            {
                                int galleryId = 0;

                                Sushi.Mediakiwi.Data.Asset asset = Sushi.Mediakiwi.Data.Asset.SelectOne(Utility.ConvertToInt(Context.Request.Query["asset"]));
                                if (!(asset == null || asset.ID == 0))
                                    galleryId = asset.GalleryID;

                                if (galleryId > 0)
                                {
                                    Sushi.Mediakiwi.Data.Gallery gallery = Sushi.Mediakiwi.Data.Gallery.SelectOne(galleryId);
                                    if (!(gallery == null || gallery.ID == 0))
                                    {
                                        folderEntity = new Folder();
                                        folderEntity.ID = gallery.ID;
                                        folderEntity.ParentID = gallery.ParentID.GetValueOrDefault(0);
                                        folderEntity.Name = gallery.Name;
                                        folderEntity.Type = FolderType.Gallery;
                                    }
                                }
                            }
                            else if (this.Console.ItemType == RequestItemType.Undefined && this.CurrentList.FolderID.HasValue)
                            {
                                if (this.CurrentList.Target == ComponentListTarget.List)
                                    folderEntity = Sushi.Mediakiwi.Data.Folder.SelectOneChild(this.CurrentList.FolderID.Value, this.CurrentSite.ID);
                                else
                                    folder = this.CurrentList.FolderID.Value;
                            }
                            else if (Utility.ConvertToInt(Context.Request.Query["asset"]) > 0)
                            {
                                int galleryId = 0;

                                Sushi.Mediakiwi.Data.Asset asset = Sushi.Mediakiwi.Data.Asset.SelectOne(Utility.ConvertToInt(Context.Request.Query["asset"]));
                                if (!(asset == null || asset.ID == 0))
                                    galleryId = asset.GalleryID;

                                if (galleryId > 0)
                                {
                                    Sushi.Mediakiwi.Data.Gallery gallery = Sushi.Mediakiwi.Data.Gallery.SelectOne(galleryId);
                                    if (!(gallery == null || gallery.ID == 0))
                                    {
                                        folderEntity = new Folder();
                                        folderEntity.ID = gallery.ID;
                                        folderEntity.ParentID = gallery.ParentID.GetValueOrDefault(0);
                                        folderEntity.Name = gallery.Name;
                                        folderEntity.Type = FolderType.Gallery;
                                    }
                                }
                            }
                            else
                            {
                                if (this.CurrentSite.ID > 0)
                                {

                                    int top = Utility.ConvertToInt(Context.Request.Query["top"]);

                                    if (top == 0 && this.CurrentSite.HasLists)
                                        top = 2;
                                    else if (top == 0 && this.CurrentSite.HasPages)
                                        top = 1;

                                    if (top == 1 && !CurrentApplicationUserRole.CanSeePage)
                                        top = 2;

                                    switch (top)
                                    {
                                        case 0:
                                            folderEntity = new Folder();
                                            folderEntity.ID = 0;
                                            folderEntity.Name = "Dashboard";
                                            folderEntity.Type = FolderType.Undefined;
                                            break;

                                        case 1:
                                            folderEntity = Sushi.Mediakiwi.Data.Folder.SelectOneBySite(this.CurrentSite.ID, Sushi.Mediakiwi.Data.FolderType.Page);
                                            break;
                                        case 2:
                                            folderEntity = Sushi.Mediakiwi.Data.Folder.SelectOneBySite(this.CurrentSite.ID, Sushi.Mediakiwi.Data.FolderType.List);
                                            break;
                                        case 3:
                                            Sushi.Mediakiwi.Data.Gallery gallery = Sushi.Mediakiwi.Data.Gallery.SelectOneRoot();
                                            folderEntity = new Folder();
                                            folderEntity.ID = gallery.ID;
                                            folderEntity.Name = gallery.Name;
                                            folderEntity.Type = FolderType.Gallery;
                                            break;
                                        case 4:
                                            folderEntity = Sushi.Mediakiwi.Data.Folder.SelectOneBySite(this.CurrentSite.ID, Sushi.Mediakiwi.Data.FolderType.Administration);
                                            break;

                                    }
                                }
                            }
                            if (folderEntity != null)
                            {
                                m_CurrentFolder = folderEntity;
                            }
                        }
                        if (folder > 0)
                            m_CurrentFolder = Sushi.Mediakiwi.Data.Folder.SelectOne(folder);

                    }
                    return m_CurrentFolder;
                }
                catch(Exception ex)
                {
                    throw new Exception(ex.StackTrace);
                }
                return null;
            
            }
        }

        //public bool IsSortOrderMode { get; set; }

        public bool IsSortOrderMode
        {
            get { return Console.Form("sortOrder") == "1"; }
        }

        private bool m_IsSubSelectMode;
        /// <summary>
        /// Is this list shown in the subselect mode?
        /// </summary>
        public bool IsSubSelectMode
        {
            set { m_IsSubSelectMode = value; }
            get { return m_IsSubSelectMode; }
        }

        public bool IsLayerMode
        {
            get { return this._Console.OpenInFrame > 0; }
        }

        private bool m_IsSearchListMode;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is search list mode.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is search list mode; otherwise, <c>false</c>.
        /// </value>
        public bool IsSearchListMode
        {
            set { m_IsSearchListMode = value; }
            get { return m_IsSearchListMode; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is export mode_ XLS.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is export mode_ XLS; otherwise, <c>false</c>.
        /// </value>
        public bool IsExportMode_XLS { get; set; }
        //public bool IsExportMode_PDF { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [item is component].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [item is component]; otherwise, <c>false</c>.
        /// </value>
        public bool ItemIsComponent { get; set; }

        private bool m_IsEditMode;
        /// <summary>
        /// Is this list shown in editmode?
        /// </summary>
        public bool IsEditMode
        {
            set { m_IsEditMode = value; }
            get { return m_IsEditMode; }
        }

        /// <summary>
        /// Determines whether [is current list] [the specified me].
        /// </summary>
        /// <param name="me">Me.</param>
        /// <returns>
        ///   <c>true</c> if [is current list] [the specified me]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsCurrentList { get; set; }

        private bool m_IsDashboardMode;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is dashboard mode.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is dashboard mode; otherwise, <c>false</c>.
        /// </value>
        public bool IsDashboardMode
        {
            set { m_IsDashboardMode = value; }
            get { return m_IsDashboardMode; }
        }

        private bool m_IsSaveMode;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is in savemode.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is savemode; otherwise, <c>false</c>.
        /// </value>
        public bool IsSaveMode
        {
            set { m_IsSaveMode = value; }
            get { return m_IsSaveMode; }
        }

        public bool ShouldValidate { get; set; }

        public bool IsDeleteMode { get; set; }

        // Wim Can be chosen as datastore for list input. In this case ComponentListVersion acts as datastore.
        // The data is stored in XML format. 
        // Note: Define this in the Constructor of the list!
        #region ShouldActAsDataStore
        private bool m_ShouldActAsDataStore;
        /// <summary>
        /// Wim Can be chosen as datastore for list input. In this case ComponentListVersion acts as datastore.
        /// This option automatically set the property 'CanContainSingleInstancePerDefinedList' to true.
        /// Note: Define this in the Constructor of the list!
        /// </summary>
        public bool ShouldActAsDataStore
        {
            set
            {
                m_ShouldActAsDataStore = value;
            }
            get { return m_ShouldActAsDataStore; }
        }
        #endregion ShouldActAsDataStore

        #region ShouldActAsPartialDataStore
        private bool m_ShouldActAsPartialDataStore;
        /// <summary>
        /// Wim Can be chosen as partial datastore for list input. In this case ComponentListVersion acts as a partial datastore.
        /// ListLoad event is not triggered and is managed internally.
        /// The data is stored in XML format.
        /// Note: Define this in the Constructor of the list!
        /// </summary>
        public bool ShouldActAsPartialDataStore
        {
            set
            {
                m_ShouldActAsPartialDataStore = value;
            }
            get { return m_ShouldActAsPartialDataStore; }
        }
        #endregion ShouldActAsPartialDataStore

        /// <summary>
        /// The text to be shown on the search button
        /// </summary>
        internal string SearchButtonText
        {

            get {
                if (CurrentList == null)
                    return null;
                return CurrentList.Data["wim_LblSearch"].Value; }
        }

        #region HasPublishOption
        private bool m_HasPublishOption;
        /// <summary>
        /// Does this list item have a publish button in the action bar?
        /// </summary>
        public bool HasPublishOption
        {
            set { m_HasPublishOption = value; }
            get { return m_HasPublishOption; }
        }
        #endregion HasPublishOption

        public void CloseLayer(string notification = null, int timeOutInSeconds = 0, bool hideOutput = true)
        {
            if (!string.IsNullOrWhiteSpace(notification))
                Notification.AddNotification(notification);

            if (hideOutput)
            {
                Page.HideDataForm = true;
                Page.HideDataGrid = true;
                Page.HideFormFilter = true;
            }

            if (timeOutInSeconds == 0)
                OnSaveScript = $"<script>mediakiwi.closeLayer();</script>";
            else
            {
                var timeOut = timeOutInSeconds * 1000;
                OnSaveScript = $"<script>setTimeout(function(){{ mediakiwi.closeLayer(); }}, {timeOut});</script>";
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [hide top section tag].
        /// </summary>
        /// <value><c>true</c> if [hide top section tag]; otherwise, <c>false</c>.</value>
        public bool HideTopSectionTag { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [hide save buttons].
        /// </summary>
        /// <value><c>true</c> if [hide save buttons]; otherwise, <c>false</c>.</value>
        public bool HideSaveButtons { get; set; }
        public bool HideEditOption { get; set; }
        public bool HideTitle { get; set; }
        //public bool HideShowAll { get; set; }
        public bool HideCreateNew { get; set; }
        public bool HideDelete { get; set; }
        public bool HideListSearchTabular { get; set; }
        public bool ShowNoTabInLayer { get; set; }
        public bool HideExportOptions { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [hide properties].
        /// </summary>
        /// <value><c>true</c> if [hide properties]; otherwise, <c>false</c>.</value>
        public bool HideProperties { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool HidePaging { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [hide search button].
        /// </summary>
        /// <value><c>true</c> if [hide search button]; otherwise, <c>false</c>.</value>
        public bool HideSearchButton { get; set; }

        /// <summary>
        /// [CB:24-06-2015] Indicates the page has components that can be replicated. 
        /// TODO! make the determination correct
        /// </summary>
        public bool CanAddNewPageComponents { get { return true; } }

        #region HasTakeDownOption
        private bool m_HasTakeDownOption;
        /// <summary>
        /// Does this list item have a take-down button in the action bar?
        /// </summary>
        public bool HasTakeDownOption
        {
            set { m_HasTakeDownOption = value; }
            get { return m_HasTakeDownOption; }
        }
        #endregion HasTakeDownOption

        // Wim Can be chosen as datastore for list input (ShouldActAsDataStore). 
        // But to present the list accordingly in a search overview a listtag is needed for representation and search.
        // Note: Define this in the Constructor of the list!
        #region DataStoreDescriptiveTag
        private string m_DataStoreDescriptiveTag;
        /// <summary>
        /// Wim Can be chosen as datastore for list input (ShouldActAsDataStore). 
        /// But to present the list accordingly in a search overview a listtag is needed for representation and search.
        /// Note: Define this in the Constructor of the list!
        /// </summary>
        public string DataStoreDescriptiveTag
        {
            set { m_DataStoreDescriptiveTag = value; }
            get { return m_DataStoreDescriptiveTag; }
        }
        #endregion DataStoreDescriptiveTag

        //#region DataItemRecognitionTag
        //private string m_DataItemRecognitionTag;
        ///// <summary>
        ///// Wim Can be chosen as datastore for list input (ShouldActAsDataStore). 
        ///// But to easily find this listitem a recognition string (referring to a property) is required.
        ///// The value of the reffering property has to be 
        ///// Note: Define this in the Constructor of the list!
        ///// </summary>
        //public string DataItemRecognitionTag
        //{
        //    set { m_DataItemRecognitionTag = value; }
        //    get { return m_DataItemRecognitionTag; }
        //}
        //#endregion DataItemRecognitionTag

        //  Can contain only one instance per defined list.
        //  Note: Define this in the Constructor of the list!
        #region CanContainSingleInstancePerDefinedList
        private bool m_CanContainSingleInstancePerDefinedList;
        /// <summary>
        /// Can contain only one instance per defined list. 
        /// Note: Define this in the Constructor of the list!
        /// </summary>
        public bool CanContainSingleInstancePerDefinedList
        {
            set { m_CanContainSingleInstancePerDefinedList = value; }
            get { return m_CanContainSingleInstancePerDefinedList; }
        }
        #endregion CanContainSingleInstancePerDefinedList


        bool m_ShowInFullWidthMode;
        /// <summary>
        /// Gets or sets a value indicating whether [show in full width mode].
        /// </summary>
        /// <value>
        /// <c>true</c> if [show in full width mode]; otherwise, <c>false</c>.
        /// </value>
        [Obsolete("Not applicable in Mediakiwi")]
        public bool ShowInFullWidthMode {
            get {
                //if (!this.CurrentApplicationUser.ShowNewDesign2 && Wim.CommonConfiguration.NEW_NAVIGATION)
                //     return true;
                return m_ShowInFullWidthMode;
            }
            set { m_ShowInFullWidthMode = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [hide open close toggle].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [hide open close toggle]; otherwise, <c>false</c>.
        /// </value>
        public bool HideOpenCloseToggle { get; set; } = true;

        #region OpenInEditMode
        private bool m_OpenInEditModeIsSet;
        private bool m_OpenInEditMode = true;
        /// <summary>
        /// Should the list item always open in editmode?
        /// Note: Define this in the Constructor of the list!
        /// </summary>
        /// <value><c>true</c> if [open in edit mode]; otherwise, <c>false</c>.</value>
        public bool OpenInEditMode
        {
            set { m_OpenInEditMode = value; m_OpenInEditModeIsSet = true; }
            get
            {
                if (m_OpenInEditModeIsSet)
                    return m_OpenInEditMode;

                if (CurrentList == null)
                    return false;
                else
                {
                    //  Page content
                    if (CurrentList.Type == ComponentListType.Browsing)
                        return false;

                    return CurrentList.Data["wim_OpenInEdit"].ParseBoolean(true);
                }

            }
        }
        #endregion

        /// <summary>
        /// Loaded just before the ListSave event is handled.
        /// </summary>
        public ComponentListVersion ComponentListVersion { get; internal set; }

        /// <summary>
        /// Flush all caching, equivalent as "?Flush=me"
        /// </summary>
        public void FlushCache(bool redirectToSelf = false)
        {
            var environment = EnvironmentVersion.Select();
            environment.Updated = DateTime.UtcNow;
            environment.Save();

            Caching.FlushAll(true);

            if (redirectToSelf && this.Console != null && this.Console.Context != null)
                this.Console.Context.Response.Redirect(GetUrl());

            //this.Console.Context.Response.Redirect(Utility.GetSafeUrl(this.Console.Context.Request).Replace("?flush=me", string.Empty).Replace("&flush=me", string.Empty), true);

        }

        public void FlushCacheIndex(params string[] index)
        {
            foreach (string item in index)
            {
                Caching.FlushIndexOfCache(item);
            }
        }
    }
}
