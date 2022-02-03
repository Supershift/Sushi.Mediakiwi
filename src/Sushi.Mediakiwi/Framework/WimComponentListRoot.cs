using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.DataEntities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Framework
{
    public class Body
    {
        public bool ShowInFullWidthMode { get; set; }
        internal string _FrameUrl;
        public void AddFrameUrl(Uri uri, bool showInFullWidthMode)
        {
            _FrameUrl = uri.ToString();
            ShowInFullWidthMode = showInFullWidthMode;
        }

        internal bool _ClearBodyBase = false;
        internal StringBuilder _BodyAddition;
        internal BodyTarget _BodyTarget = BodyTarget.Below;

        WimComponentListRoot _instance;
        public Body(WimComponentListRoot instance)
        {
            _instance = instance;
            Grid = new Grid(instance);
            Filter = new Filter(instance);
            Navigation = new Navigation(instance);
            Form = new Form(instance);
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
            _ClearBodyBase = clearBaseTemplateBody;

            if (clearBaseTemplateBody)
            {
                return;
            }

            if (_BodyAddition != null)
            {
                _BodyAddition = new StringBuilder();
            }
        }

        /// <summary>
        /// Adds the layer.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="defaultSize">The default size.</param>
        /// <param name="alternativeHeight">Height of the alternative.</param>
        /// <param name="hasIframe">if set to <c>true</c> [has iframe].</param>
        /// <param name="hasScrolling">if set to <c>true</c> [has scrolling].</param>
        public async Task AddLayerAsync(string url, LayerSize defaultSize, int? alternativeHeight = null, bool hasScrolling = true, bool hasIframe = true)
        {
            var specs = new Grid.LayerSpecification(defaultSize);
            specs.HasScrolling = hasScrolling;
            specs.InFrame = hasIframe;
            if (alternativeHeight.HasValue)
            {
                specs.IsHeightPercentage = false;
                specs.Height = alternativeHeight;
            }

            string html = string.Concat("<span class=\"openlayerauto\" data-url=\"", url, "\" ", specs.Parse(true), " />");

            await AddAsync(html, false);
        }

        /// <summary>
        /// Adds the supplied HTML to the page
        /// </summary>
        /// <param name="html">The HTML code to add</param>
        public async Task AddAsync(string html)
        {
            await AddAsync(html, false);
        }


        /// <summary>
        /// Adds the supplied HTML to the page
        /// </summary>
        /// <param name="html">The HTML code to add</param>
        /// <param name="clearBaseTemplateBody">Should the existing body HTML be cleared ?</param>
        public async Task AddAsync(string html, bool clearBaseTemplateBody)
        {
            await AddAsync(html, clearBaseTemplateBody, BodyTarget.Below);
        }

        public enum BodyTarget
        {
            Nested,
            Below
        }

        /// <summary>
        /// Adds the supplied HTML to the page
        /// </summary>
        /// <param name="html">The HTML code to add</param>
        /// <param name="clearBaseTemplateBody">Should the existing body HTML be cleared ?</param>
        /// <param name="target">Where to add the HTML Code</param>
        public async Task AddAsync(string html, bool clearBaseTemplateBody, BodyTarget target)
        {
            ResourceLocation loc = ResourceLocation.NONE;
            switch (target)
            {
                default:
                case BodyTarget.Nested: loc = ResourceLocation.BODY_NESTED; break;
                case BodyTarget.Below: loc = ResourceLocation.BODY_BELOW; break;
            }

            await _instance.Page.Resources.AddAsync(loc, ResourceType.HTML, html, false, false, clearBaseTemplateBody);
        }

        /// <summary>
        /// Will be called by the <c>AdditionalResource</c> class
        /// </summary>
        /// <param name="html">The HTML code to add</param>
        /// <param name="clearBaseTemplateBody">Should the existing body HTML be cleared ?</param>
        /// <param name="target">Where to add the HTML Code</param>
        internal void AddResource(string html, bool clearBaseTemplateBody, BodyTarget target)
        {
            if (clearBaseTemplateBody)
            {
                Clear(clearBaseTemplateBody);
            }

            if (_BodyAddition == null)
            {
                _BodyAddition = new StringBuilder();
            }

            _BodyTarget = target;
            _BodyAddition.Append(html);
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
        WimComponentListRoot _instance;
        public Filter(WimComponentListRoot instance)
        {
            _instance = instance;
        }
    }

    public class Navigation
    {
        WimComponentListRoot _instance;
        public Navigation(WimComponentListRoot instance)
        {
            _instance = instance;
            Side = new SideNavigation(instance);
            Menu = new ButtonMenu(instance);
        }

        public SideNavigation Side { get; set; }
        public ButtonMenu Menu { get; set; }
    }

    public class ButtonMenu
    {
        public ButtonTarget DeleteButtonTarget = ButtonTarget.TopRight;
        public ButtonTarget SaveButtonTarget = ButtonTarget.BottomRight;

        WimComponentListRoot _instance;
        public ButtonMenu(WimComponentListRoot instance)
        {
            _instance = instance;
        }
    }

    public class SideNavigation
    {
        public enum SideNavigationTarget
        {
            Above,
            Below
        }

        WimComponentListRoot _instance;
        public SideNavigation(WimComponentListRoot instance)
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
            _ClearBodyBase = clearBaseTemplateBody;

            if (clearBaseTemplateBody)
            {
                return;
            }

            if (_BodyAddition != null)
            {
                _BodyAddition = new StringBuilder();
            }
        }

        public bool _ClearBodyBase;
        public StringBuilder _BodyAddition;
        public SideNavigationTarget _BodyTarget;

        public void Add(string html, bool clearBaseTemplateBody, SideNavigationTarget target)
        {
            if (clearBaseTemplateBody)
            {
                Clear(clearBaseTemplateBody);
            }

            if (_BodyAddition == null)
            {
                _BodyAddition = new StringBuilder();
            }

            _BodyTarget = target;
            _BodyAddition.Append(html);
        }
    }

    public class FormElements
    {
        public string GetHeaderStyle()
        {
            if (_Input == null && _Select == null)
            {
                return string.Empty;
            }
            StringBuilder html = new StringBuilder();
            html.Append("<style>");
            if (Input.Long.Width.HasValue)
            {
                html.Append(string.Concat(" input.long { width: ", Input.Long.Width.Value, "px !important; }"));
            }
            if (Input.Short.Width.HasValue)
            {
                html.Append(string.Concat(" input.short { width: ", Input.Short.Width.Value, "px !important; }"));
            }
            if (Select.Long.Width.HasValue)
            {
                html.Append(string.Concat(" select.long { width: ", Select.Long.Width.Value, "px !important; }"));
            }
            if (Select.Short.Width.HasValue)
            {
                html.Append(string.Concat(" select.short { width: ", Select.Short.Width.Value, "px !important; }"));
            }
            html.Append("</style>");
            return html.ToString();
        }

        FormElementType _Input;
        public FormElementType Input
        {
            get
            {
                if (_Input == null)
                {
                    _Input = new FormElementType();
                }
                return _Input;
            }
            set { _Input = value; }
        }

        FormElementType _Select;
        public FormElementType Select
        {
            get
            {
                if (_Select == null)
                {
                    _Select = new FormElementType();
                }
                return _Select;
            }
            set { _Select = value; }
        }
    }

    public class FormElementType
    {
        FormElementDesignType _Long;
        public FormElementDesignType Long
        {
            get
            {
                if (_Long == null)
                {
                    _Long = new FormElementDesignType();
                }
                return _Long;
            }
            set { _Long = value; }
        }

        FormElementDesignType _Short;
        public FormElementDesignType Short
        {
            get
            {
                if (_Short == null)
                {
                    _Short = new FormElementDesignType();
                }
                return _Short;
            }
            set { _Short = value; }
        }
    }

    public class FormElementDesignType
    {
        public int? Width { get; set; }
    }

    public class Form
    {
        FormElements _Elements;
        public FormElements Elements
        {
            get
            {
                if (_Elements == null)
                {
                    _Elements = new FormElements();
                }
                return _Elements;
            }
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
            if (_instance.OnSaveScript == null)
            {
                _instance.OnSaveScript = string.Empty;
            }

            _instance.OnSaveScript = $"<input type=\"hidden\" class=\"postParent\" data-url=\"{url}\" />";
        }

        /// <summary>
        /// Closes the layer (from within a layer) 
        /// </summary>
        public void CloseLayer()
        {
            if (_instance.OnSaveScript == null)
            {
                _instance.OnSaveScript = string.Empty;
            }

            _instance.OnSaveScript = @"<input type=""hidden"" class=""closeLayer"" />";
        }

        /// <summary>
        /// Posts the layer information to the parent container (Note, this is extracted from the querystring "referid" or the set dataTarget)
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="value">The value.</param>
        /// <param name="canAddMultiple">Should to be added or replace the existing value?</param>
        /// <param name="dataTarget">The target, if not set this will be taken from the querystring (referID), this is the ID of the field</param>
        /// <param name="editUrl">The URL where this item can be changed</param>
        /// <param name="listTitle">The title of the Edit list</param>
        public void PostDataToSubSelect(int id, string value, bool canAddMultiple = false, string dataTarget = null, string editUrl = null, string listTitle = null)
        {
            PostDataToSubSelect(id.ToString(), value, canAddMultiple, dataTarget, editUrl, listTitle);
        }

        /// <summary>
        /// Posts the layer information to the parent container (Note, this is extracted from the querystring "referid" or the set dataTarget)
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="value">The value.</param>
        /// <param name="canAddMultiple">Should to be added or replace the existing value?</param>
        /// <param name="dataTarget">The target, if not set this will be taken from the querystring (referID), this is the ID of the field</param>
        /// <param name="editUrl">The URL where this item can be changed</param>
        /// <param name="listTitle">The title of the Edit list</param>
        public void PostDataToSubSelect(string id, string value, bool canAddMultiple = false, string dataTarget = null, string editUrl = null, string listTitle = null)
        {
            if (_instance.OnSaveScript == null)
            {
                _instance.OnSaveScript = string.Empty;
            }

            var target = dataTarget != null ? $@" data-target=""{dataTarget}""" : null;
            var url = string.IsNullOrWhiteSpace(editUrl) ? null : $@" data-editurl=""{Uri.EscapeUriString(Utility.CleanUrl(editUrl))}""";
            var title = string.IsNullOrWhiteSpace(listTitle) ? null : $@" data-listtitle=""{listTitle}""";

            _instance.OnSaveScript += $@"<input type=""hidden"" class=""postparent""{(canAddMultiple ? @" data-multiple=""1""" : null)}{target}{url}{title} id=""{id}"" value=""{value}"" />";
        }

        /// <summary>
        /// Determines whether the button is of type Primairy action
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns></returns>
        public bool IsPrimairyAction(ContentListItem.ButtonAttribute button)
        {
            if (button.IsPrimary)
            {
                return true;
            }

            if (!string.IsNullOrEmpty(button.ID) && button.ID == _PrimairyAction)
            {
                return true;
            }

            return false;
        }

        WimComponentListRoot _instance;
        public Form(WimComponentListRoot instance)
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


        WimComponentListRoot _instance;
        public Table(WimComponentListRoot instance)
        {
            _instance = instance;
            ClassName = "selections";
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
                {
                    _Table = new Table(_instance);
                }
                return _Table;
            }
            set
            {
                _Table = value;
            }

        }

        internal bool _ClearGridBase = false;
        internal StringBuilder _GridAddition;

        WimComponentListRoot _instance;
        public Grid(WimComponentListRoot instance)
        {
            _instance = instance;
            ClassName = "searchTable";
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
                StringBuilder properties = new StringBuilder();

                if (Width.HasValue)
                {
                    properties.Append($"{(properties.Length > 0 ? "," : string.Empty)}width:{Width.Value}{(IsWidthPercentage ? "%" : "px")}");
                }

                if (Height.HasValue)
                {
                    properties.Append($"{(properties.Length > 0 ? "," : string.Empty)}height:{Height.Value}{(IsHeightPercentage ? "%" : "px")}");
                }

                if (InFrame.HasValue)
                {
                    properties.Append($"{(properties.Length > 0 ? "," : string.Empty)}iframe:{InFrame.Value.ToString().ToLowerInvariant()}");
                }

                if (HasScrolling.HasValue)
                {
                    properties.Append($"{(properties.Length > 0 ? "," : string.Empty)}scrolling:{HasScrolling.Value.ToString().ToLowerInvariant()}");
                }

                if (!string.IsNullOrEmpty(Class))
                {
                    properties.Append($"{(properties.Length > 0 ? "," : string.Empty)}class:{Class}");
                }

                if (properties.Length > 0)
                {
                    if (includeHtmlTag)
                    {
                        if (string.IsNullOrEmpty(Title))
                        {
                            return $" data-layer=\"{properties}\"";
                        }
                        else
                        {
                            return $" data-layer=\"{properties}\" data-title=\"{Title}\"";
                        }
                    }
                    return properties.ToString();
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
                        {
                            InFrame = true;
                            Width = 790;
                            Height = 90;
                            IsHeightPercentage = true;
                            HasScrolling = true;
                        }
                        break;
                    case LayerSize.Small:
                        {
                            InFrame = true;
                            Width = 790;
                            Height = 414;
                            HasScrolling = true;
                        }
                        break;
                    case LayerSize.Tiny:
                        {
                            InFrame = true;
                            Width = 472;
                            Height = 314;
                        }
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
                {
                    _PreLoader = new PreLoaderData();
                }
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
                    {
                        SetClickLayer(new LayerSpecification(LayerSize.Normal));
                    }
                    else
                    {
                        return null;
                    }
                }
                return _specification.Parse(true);
            }
        }

        internal string ClickLayerClass
        {
            get
            {
                if (_GetClickLayerSpecification == null)
                {
                    if (_instance.CurrentList.Option_LayerResult)
                    {
                        SetClickLayer(new LayerSpecification(LayerSize.Normal));
                    }
                    else
                    {
                        return null;
                    }
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

        public LayerSpecification LayerConfiguration => _specification;

        /// <summary>
        /// Clears the specified clear base template header.
        /// </summary>
        /// <param name="clearBaseTemplateBody">if set to <c>true</c> [clear base template body].</param>
        public void Clear(bool clearBaseTemplateGrid)
        {
            _ClearGridBase = clearBaseTemplateGrid;

            if (clearBaseTemplateGrid)
            {
                return;
            }

            if (_GridAddition != null)
            {
                _GridAddition = new StringBuilder();
            }
        }

        public void Add(string html)
        {
            Add(html, false);
        }

        public void Add(string html, bool clearBaseTemplateGrid)
        {
            if (clearBaseTemplateGrid)
            {
                Clear(clearBaseTemplateGrid);
            }

            if (_GridAddition == null)
            {
                _GridAddition = new StringBuilder();
            }

            _GridAddition.Append(html);
        }
    }

    public class Head
    {
        internal bool _ClearHeadBase = false;
        internal StringBuilder _HeadAddition;

        WimComponentListRoot _root;
        WimComponentListRoot.PageData _instance;
        public Head(WimComponentListRoot.PageData instance, WimComponentListRoot root)
        {
            _root = root;
            _instance = instance;
        }

        /// <summary>
        /// Adds a script element to the page header
        /// </summary>
        /// <param name="path">relative path to the file</param>
        public async Task AddScriptAsync(string path, bool appendApplicationPath = true)
        {
            await _instance.Resources.AddAsync(ResourceLocation.HEADER, ResourceType.JAVASCRIPT, path, appendApplicationPath);
        }

        /// <summary>
        /// Adds a style element to the page header
        /// </summary>
        /// <param name="path">relative path to the file</param>
        /// <param name="appendApplicationPath">when false the application path will not be added to the path param</param>
        public async Task AddStyleAsync(string path, bool appendApplicationPath = true)
        {
            await _instance.Resources.AddAsync(ResourceLocation.HEADER, ResourceType.STYLESHEET, path, appendApplicationPath);
        }

        /// <summary>
        /// Clears the specified clear base template header.
        /// </summary>
        /// <param name="clearBaseTemplateHeader">if set to <c>true</c> [the all header entries in the standard template].</param>
        public void Clear(bool clearBaseTemplateHeader)
        {
            _ClearHeadBase = clearBaseTemplateHeader;

            if (_HeadAddition != null)
            {
                _HeadAddition = new StringBuilder();
            }
        }

        public bool EnableColorCodingLibrary { get; set; }

        /// <summary>
        /// Adds the specified HTML.
        /// </summary>
        /// <param name="html">The HTML.</param>
        public async Task AddAsync(string html)
        {

            await _instance.Resources.AddAsync(ResourceLocation.HEADER, ResourceType.HTML, html);
        }

        /// <summary>
        /// Adds the specified HTML.
        /// </summary>
        /// <param name="html">The HTML.</param>
        internal void AddResource(string html)
        {
            if (_HeadAddition == null)
            {
                _HeadAddition = new StringBuilder();
            }
            _HeadAddition.Append(html);
        }

        internal Uri Logo { get; private set; }

        public void SetLogo(Uri logo)
        {
            Logo = logo;
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
        public string ToString(object value, ListDataColumn column)
        {
            string candidate = value == null ? string.Empty : value.ToString();

            if (!string.IsNullOrEmpty(candidate))
            {
                candidate = string.Concat(column.ColumnValuePrefix, candidate, column.ColumnValueSuffix);
            }

            if (_type == DataItemType.TableRow && IsRowSortingDisabled)
            {
                if (string.IsNullOrEmpty(Class))
                {
                    Class = "nosort";
                }
                else
                {
                    Class += " nosort";
                }
            }

            if (column.ColumnIsFixed)
            {
                if (!string.IsNullOrEmpty(candidate))
                {
                    candidate = string.Concat("<abbr>", candidate, "</abbr>");
                }
                else
                {
                    candidate = "&nbsp;";
                }

                if (string.IsNullOrEmpty(Class))
                {
                    Class = "fixed";
                }
                else
                {
                    Class += " fixed";
                }

                //  Add the needed width (position absolute)
                Add("width", column.SuggestedColumnLength.ToString());

                if (column.ColumnFixedLeftMargin > 0)
                {
                    Style.Add("margin-left", string.Format("{0}px", column.ColumnFixedLeftMargin));
                }
            }
            else
            {
                if (column.ShrinkTextWhenToLarge)
                {
                    if (!string.IsNullOrEmpty(candidate))
                    {
                        candidate = string.Concat("<abbr>", candidate, "</abbr>");
                    }

                    if (column.SuggestedColumnLength > 0)
                    {
                        Add("width", column.SuggestedColumnLength.ToString());
                    }
                }
            }

            if (column.ColumnHeight > 0)
            {
                if (_style == null)
                {
                    _style = new StyleAttribute();
                }
                _style.Add("height", string.Format("{0}px", column.ColumnHeight));
            }

            StringBuilder html = new StringBuilder();
            if (_arr != null)
            {
                foreach (var key in _arr.Keys)
                {
                    if (_arr[key] != null)
                    {
                        html.Append($" {key}=\"{_arr[key]}\"");
                    }
                }
            }

            if (_style != null)
            {
                html.Append($" style=\"{_style}\"");
            }

            if (_type == DataItemType.TableCell)
            {
                return $"<td{html}>{candidate}</td>";
            }
            if (_type == DataItemType.TableRow)
            {
                return $"{candidate}<tr{html}>";
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

        /// <summary>
        /// Gets or sets the <see cref="string"/> with the specified name.
        /// </summary>
        /// <value>
        /// The <see cref="string"/>.
        /// </value>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <exception cref="Exception">
        /// Please use the Style object
        /// or
        /// Please use the Style object
        /// </exception>
        public string this[string name]
        {
            get
            {
                if (name == "style")
                {
                    throw new Exception("Please use the Style object");
                }

                name = name.ToLowerInvariant();
                if (_arr == null)
                {
                    _arr = new Hashtable();
                }

                if (_arr.ContainsKey(name))
                {
                    return _arr[name].ToString();
                }
                return null;
            }
            set
            {
                if (name == "style")
                {
                    throw new Exception("Please use the Style object");
                }

                name = name.ToLowerInvariant();
                if (_arr == null)
                {
                    _arr = new Hashtable();
                }

                if (_arr.ContainsKey(name))
                {
                    _arr[name] = value;
                }
                else
                {
                    _arr.Add(name, value);
                }
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
            get
            {
                if (_style == null)
                {
                    _style = new StyleAttribute();
                }
                return _style;
            }
            set { _style = value; }
        }

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
            StringBuilder html = new StringBuilder();
            if (_arr != null)
            {
                foreach (var key in _arr.Keys)
                {
                    if (_arr[key] == null)
                        continue;

                    if (html.Length > 0)
                    {
                        html.Append(';');
                    }

                    html.Append($"{key}:{_arr[key]}");
                }
            }
            return html.ToString();
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
                if (_arr == null)
                {
                    _arr = new Hashtable();
                }
                if (_arr.ContainsKey(name))
                {
                    return _arr[name].ToString();
                }
                return null;
            }
            set
            {
                if (_arr == null)
                {
                    _arr = new Hashtable();
                }

                if (_arr.ContainsKey(name))
                {
                    _arr[name] = value;
                }
                else
                {
                    _arr.Add(name, value);
                }
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
            BackgroundColor = "#ededed";
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
        public void DoListLoad(int selectedKey, int componentVersionKey)
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

        public bool HasListLoad
        {
            get { return _origin.HasListLoad; }
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
            {
                AfterSaveElementIdentifier = null;
            }

            Utils.RunSync(() => _origin.OnListSave(new ComponentListEventArgs(selectedKey, previousItemID, componentVersionKey, groupID, groupItemID, isValidForm)));
        }

        public bool HasListSave
        {
            get { return _origin.HasListSave; }
        }

        internal void DoListDelete(int selectedKey, int componentVersionKey, bool? isValidForm)
        {
            int groupID = Utility.ConvertToInt(Context.Request.Query["group"]);
            int groupItemID = Utility.ConvertToInt(Context.Request.Query["groupitem"]);

            long start = DateTime.Now.Ticks;

            Utils.RunSync(() => _origin.OnListDelete(new ComponentListEventArgs(selectedKey, componentVersionKey, 0, groupID, groupItemID, isValidForm)));
        }

        public bool HasListDelete
        {
            get { return _origin.HasListDelete; }
        }

        public void DoListSearch()
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
                {
                    return;
                }
            }

            var itemID = Utility.ConvertToIntNullable(Context.Request.Query["item"], false);
            int groupID = Utility.ConvertToInt(Context.Request.Query["group"]);
            int groupItemID = Utility.ConvertToInt(Context.Request.Query["groupitem"]);

            long start = DateTime.Now.Ticks;

            if (!itemID.HasValue)
            {
                Page.HideTabs = true;

                //  SearchAsync should only work on toplevel lists, not inner lists (8/11/25)
                if ((CurrentList.Option_SearchAsync) && !_origin.IsFormatRequest_AJAX)
                {
                    return;
                }
            }

            if (_origin.wim._Grids == null)
            {
                // should be replaced with await (async all the way).
                Utils.RunSync(() => _origin.OnListSearch(new ComponentListSearchEventArgs(itemID.GetValueOrDefault(), groupID, groupItemID)));
            }
        }

        internal bool HasListSearch
        {
            get { return _origin.HasListSearch; }
        }

        internal ListDataItemCreatedEventArgs DoListDataItemCreated(
            DataItemType type,
            ListDataColumn[] columns,
            ListDataColumn column,
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
            {
                _origin.OnListDataItemCreated(output);
            }

            return output;
        }

        internal bool HasListDataItemCreated
        {
            get { return _origin.HasListDataItemCreated; }
        }

        public void DoListInit()
        {
            _origin.ApplyListSettings();

            long start = DateTime.Now.Ticks;
            Utils.RunSync(() => _origin.OnListInit());
        }

        public ComponentDataReportEventArgs DoListDataReport()
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
        /// When true the system keeps the list search filters for this instance of the list
        /// </summary>        
        public bool HasOwnSearchListCache { get; set; }
        public bool IsClearingListCache { get; set; }
        /// <summary>
        /// When <c>HasOwnSearchListCache</c> is <c>true</c>, this method will clear the specific cache
        /// </summary>
        /// <returns><c>true</c> when clear has been done, <c>false</c> if the cache was not possible to clear</returns>
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
                        {
                            return item.State;
                        }
                        foreach (System.Reflection.PropertyInfo p in sender.GetType().GetProperties())
                        {
                            if (p.Name == item.ValidationProperty)
                            {
                                bool isTrue = (bool)p.GetValue(sender, null);
                                if (!item.State)
                                {
                                    return !isTrue;
                                }
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
                                    {
                                        return !isTrue;
                                    }
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

            if (EditableItemList != null)
            {
                foreach (StateTypeItem item in EditableItemList)
                {
                    if (item.AssignedProperty == property)
                    {
                        if (item.ValidationProperty == null)
                        {
                            return item.State;
                        }

                        foreach (System.Reflection.PropertyInfo p in sender.GetType().GetProperties())
                        {
                            if (p.Name == item.ValidationProperty)
                            {
                                bool isTrue = (bool)p.GetValue(sender, null);
                                if (!item.State)
                                {
                                    return !isTrue;
                                }
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
                                    {
                                        return !isTrue;
                                    }
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
                        {
                            return item.State;
                        }

                        foreach (System.Reflection.PropertyInfo p in sender.GetType().GetProperties())
                        {
                            if (p.Name == item.ValidationProperty)
                            {
                                bool isTrue = (bool)p.GetValue(sender, null);
                                if (!item.State)
                                {
                                    return !isTrue;
                                }
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
                                    {
                                        return !isTrue;
                                    }
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
            {
                VisibilityItemList = new List<StateTypeItem>();
            }

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
            {
                item = new StateTypeItem();
            }

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
            {
                EditableItemList = new List<StateTypeItem>();
            }

            StateTypeItem item = (from x in EditableItemList where x.AssignedProperty == assignedProperty select x).FirstOrDefault();
            if (item == null)
            {
                item = new StateTypeItem();
            }

            item.AssignedProperty = assignedProperty;
            item.ValidationProperty = validationProperty;
            item.State = state;
            EditableItemList.Add(item);
        }

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
        /// Sets the report graph data.
        /// </summary>
        /// <value>The report graph data.</value>
        public string ReportGraphData
        {
            set { GraphUrl = value; }
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
            Title = string.IsNullOrEmpty(title) ? CurrentList.SingleItemName : title;

            if (!string.IsNullOrEmpty(description))
            {
                description = string.Format("<p style=\"color:#005770; font-family:Arial,Sans-Serif; margin: 10px 0px;\">{0}</p>", description);
            }

            Description = description;
        }

        /// <summary>
        /// Clean all relative links from the send mail
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        internal string Links(Match m)
        {
            string path = m.Value
                .ToLowerInvariant()
                .Replace("href=", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                .Replace("\"", string.Empty, StringComparison.InvariantCultureIgnoreCase);

            if (path.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
            {
                return $"href=\"{path}\"";
            }

            if (path.StartsWith("?", StringComparison.OrdinalIgnoreCase))
            {
                path = $"wim.ashx{path}";
            }

            return $"href=\"{Console.AddApplicationPath(path, true)}\"";
        }


        private IComponentList m_currentList;
        /// <summary>
        /// Gets or sets the current list.
        /// </summary>
        /// <value>The current list.</value>
        public IComponentList CurrentList
        {
            set { m_currentList = value; }
            get
            {
                if (m_currentList == null && Context != null)
                {
                    m_currentList = ComponentList.SelectOne(Utility.ConvertToInt(Context.Request.Query["list"]));
                }
                return m_currentList;
            }
        }

        /// <summary>
        /// Gets or sets the current data instance.
        /// </summary>
        /// <value>The current data instance.</value>
        public object CurrentDataInstance { get; set; }

        /// <summary>
        /// Gets or sets the property list type ID.
        /// </summary>
        /// <value>The property list type ID.</value>
        public int PropertyListTypeID { get; set; }

        /// <summary>
        /// Gets or sets the property list ignore list.
        /// </summary>
        /// <value>
        /// The property list ignore list.
        /// </value>
        public Property[] PropertyListIgnoreList { get; set; }

        /// <summary>
        /// Gets or sets the pass over class instance.
        /// This properties can only be assigned from the constructor of the componentlist. If assigned the initiated componentlist will be replaced by this
        /// other "pass-over" instance.
        /// </summary>
        /// <value>The pass over class instance.</value>
        public IComponentListTemplate PassOverClassInstance { get; set; }

        /// <summary>
        /// Gets or sets the property list override ID.
        /// </summary>
        /// <value>The property list override ID.</value>
        public int? PropertyListOverrideID { get; set; }

        /// <summary>
        /// Gets or sets the property sub selection.
        /// If applied the list will show only these property: this overrides the default properties and the PropertyListTypeID value!
        /// </summary>
        /// <value>The property sub selection.</value>
        public int[] PropertySubSelection { get; set; }

        ComponentListTemplate _origin;
        /// <summary>
        /// 
        /// </summary>
        public WimComponentListRoot(ComponentListTemplate origin)
        {
            _origin = origin;

            SearchListCanClickThrough = true;
            DashBoardElementIsVisible = true;
            DashBoardCanClickThrough = true;
        }

        /// <summary>
        /// Gets or sets the current environment.
        /// </summary>
        /// <value>The current environment.</value>
        public IEnvironment CurrentEnvironment { get; set; }

        private IApplicationRole m_CurrentApplicationUserRole;
        /// <summary>
        /// Gets or sets the current user role.
        /// </summary>
        /// <value>The current user role.</value>
        public IApplicationRole CurrentApplicationUserRole
        {
            get
            {
                if (m_CurrentApplicationUserRole == null && CurrentApplicationUser?.ID > 0)
                {
                    m_CurrentApplicationUserRole = CurrentApplicationUser.SelectRole();
                }
                return m_CurrentApplicationUserRole;
            }
            set
            {
                m_CurrentApplicationUserRole = value;
            }
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
        public IApplicationUser CurrentApplicationUser
        {
            get
            {
                var candidate = Context.Items["wim.applicationuser"] as IApplicationUser;
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
                if (m_CurrentCulture != null)
                {
                    return m_CurrentCulture;
                }

                if (CurrentSite == null || string.IsNullOrEmpty(CurrentSite.Culture))
                {
                    return System.Globalization.CultureInfo.CurrentCulture;
                }

                m_CurrentCulture = new System.Globalization.CultureInfo(CurrentSite.Culture);
                return m_CurrentCulture;
            }
        }

        /// <summary>
        /// The current site in which this list is viewed.
        /// </summary>
        public Site CurrentSite { get; set; }

        private Notifications m_Notification;
        /// <summary>
        /// 
        /// </summary>
        public Notifications Notification
        {
            get
            {
                if (m_Notification == null)
                {
                    m_Notification = new Notifications(this);
                }
                return m_Notification;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class Notifications
        {
            WimComponentListRoot wim;
            public Notifications(WimComponentListRoot _instance)
            {
                wim = _instance;
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
                {
                    return;
                }

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
                        {
                            Errors = new Dictionary<string, string>();
                        }
                        Errors.Add(property, errorMessage);
                    }
                }
            }

            internal IDictionary<string, string> Errors;
            internal List<string> GenericErrors;
            internal List<string> GenericInformation;
            internal List<string> GenericInformationAlert;

            public IDictionary<string, string> GetPropertyErrors => Errors ?? new Dictionary<string, string>();
            public IReadOnlyCollection<string> GetGenericErrors => GenericErrors ?? new List<string>();
            public IReadOnlyCollection<string> GetGenericInformation => GenericInformation ?? new List<string>();
            public IReadOnlyCollection<string> GetGenericInformationAlert => GenericInformationAlert ?? new List<string>();

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
                {
                    return;
                }

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
                if (!string.IsNullOrEmpty(notificationTitle) && notificationMessage != null)
                {
                    notificationMessage = string.Format("<h3>{1}</h3>{0}", notificationMessage, notificationTitle);
                }

                if (notificationMessage != null)
                {
                    notificationMessage = notificationMessage.Trim();
                }

                if (string.IsNullOrEmpty(notificationMessage) || Utility.CleanFormatting(notificationMessage).Trim().Length == 0)
                {
                    return;
                }

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

        public ICollection<string> GetQueryStringRecording() => _QueryStringRecording;

        /// <summary>
        /// Add querystring items to the tabs
        /// </summary>
        /// <param name="name"></param>
        public void AddTabQueryStringRecording(string name)
        {
            if (_QueryStringRecording == null)
            {
                _QueryStringRecording = new List<string>();
            }

            if (!_QueryStringRecording.Contains(name))
            {
                _QueryStringRecording.Add(name);
            }
        }

        /// <summary>
        /// Adds the tab.
        /// </summary>
        /// <param name="ID">The ID.</param>
        public void AddTab(Guid ID)
        {
            var list = ComponentList.SelectOne(ID);
            AddTab(list);
        }

        /// <summary>
        /// Adds the tab.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <param name="selectionID">The selection ID.</param>
        public void AddTab(Guid ID, int selectionID)
        {
            var list = ComponentList.SelectOne(ID);
            AddTab(list, selectionID);
        }

        /// <summary>
        /// Adds the tab.
        /// </summary>
        /// <param name="type">The type.</param>
        public void AddTab(Type type)
        {
            var list = ComponentList.SelectOne(type.ToString());
            AddTab(list);
        }

        /// <summary>
        /// Adds the tab.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="selectionID">The selection ID.</param>
        public void AddTab(Type type, int selectionID)
        {
            var list = ComponentList.SelectOne(type.ToString());
            AddTab(list, selectionID);
        }

        /// <summary>
        /// Adds the tab.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="selectionID">The selection identifier.</param>
        /// <param name="count">The count.</param>
        public void AddTab(Type type, int selectionID, int? count)
        {
            var list = ComponentList.SelectOne(type.ToString());
            AddTab(list.Name, list, selectionID, count);
        }

        /// <summary>
        /// Adds the tab.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="title">The title.</param>
        /// <param name="selectionID">The selection ID.</param>
        public void AddTab(Type type, string title, int selectionID)
        {
            var list = ComponentList.SelectOne(type.ToString());
            AddTab(title, list, selectionID);
        }

        /// <summary>
        /// Adds the tab.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="title">The title.</param>
        /// <param name="selectionID">The selection ID.</param>
        public void AddTab(Guid id, string title, int selectionID)
        {
            var list = ComponentList.SelectOne(id);
            AddTab(title, list, selectionID);
        }


        /// <summary>
        /// Adds the tab. These should be added in the ListLoad event
        /// </summary>
        /// <param name="list">The list.</param>
        public void AddTab(IComponentList list)
        {
            AddTab(list.Name, list, 0);
        }

        /// <summary>
        /// Adds the tab. These should be added in the ListLoad event
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="selectedItem">The selected item.</param>
        public void AddTab(IComponentList list, int selectedItem)
        {
            AddTab(list.Name, list, selectedItem);
        }

        /// <summary>
        /// Adds the tab. These should be added in the ListLoad event
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="list">The list.</param>
        public void AddTab(string title, IComponentList list)
        {
            AddTab(title, list, 0);
        }

        /// <summary>
        /// Adds the tab.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="list">The list.</param>
        /// <param name="selectedItem">The selected item.</param>
        public void AddTab(string title, IComponentList list, int selectedItem)
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
        public void AddTab(string title, IComponentList list, int selectedItem, int? count)
        {
            if (!list.HasRoleAccess(CurrentApplicationUser))
            {
                return;
            }

            if (m_TabCollection == null)
            {
                m_TabCollection = new List<Tabular>();
            }

            Tabular t = new Tabular();
            t.List = list;
            t.Selected = (Console.Request.Query["list"] == list.ID.ToString());
            t.SelectedItem = selectedItem;
            t.Title2 = title;
            t.Count = count;
            m_TabCollection.Add(t);
        }

        internal List<Tabular> m_TabCollection;

        public ICollection<Tabular> GetTabs() => m_TabCollection;

        /// <summary>
        /// Represents a Tabular entity.
        /// </summary>
        public class Tabular
        {
            /// <summary>
            /// 
            /// </summary>
            public string Title2;

            public string TitleValue
            {
                get
                {
                    if (Count.HasValue)
                    {
                        return string.Format("{0}<span class=\"items\">{1}</span>", Title2, Count.Value);
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


        /// <summary>
        /// 
        /// </summary>
        public string OnSaveScript { get; set; }

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
            set
            {
                if (value.GetValueOrDefault(0) != 0)
                {
                    if (Context != null)
                    {
                        Context.Items["wim.Saved.ID"] = value;
                    }
                    m_AfterSaveElementIdentifier = value;
                }
            }
            get
            {

                if (Context != null)
                {
                    return Utility.ConvertToIntNullable(Context.Items["wim.Saved.ID"]);
                }
                return m_AfterSaveElementIdentifier;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string OnDeleteScript { get; set; }

        PageData m_Page;
        public PageData Page
        {
            get
            {
                if (m_Page == null)
                {
                    m_Page = new PageData(this);
                }
                return m_Page;
            }
            set { m_Page = value; }
        }

        FormData m_Form;
        public FormData Form
        {
            get
            {
                if (m_Form == null)
                {
                    m_Form = new FormData(this);
                }
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
                if (m_GridDataCommunication == null)
                {
                    m_GridDataCommunication = new GridDataDetail(this);
                }
                return m_GridDataCommunication;
            }
        }

        GridData m_Grid;
        public GridData Grid
        {
            get
            {
                if (m_Grid == null)
                {
                    m_Grid = new GridData(this);
                }
                return m_Grid;
            }
            set { m_Grid = value; }
        }

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
                {
                    m_Root.AddedCheckboxPostCollection = new System.Collections.Specialized.NameValueCollection();
                }

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
                {
                    m_Root.AddedTextboxPostCollection = new System.Collections.Specialized.NameValueCollection();
                }

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
                {
                    m_Root.AddedTextboxPostCollection = new System.Collections.Specialized.NameValueCollection();
                }
                m_Root.AddedTextboxPostCollection.Add(string.Concat(propertyName, "_", ID), Utility.ConvertToDecimalString(value));
            }

            /// <summary>
            /// Adds the radiobox value.
            /// </summary>
            /// <param name="propertyName">Name of the property.</param>
            /// <param name="ID">The ID.</param>
            /// <param name="value">The value.</param>
            public void AddRadioboxValue(string propertyName, int ID, string value)
            {
                if (m_Root.AddedRadioboxPostCollection == null)
                {
                    m_Root.AddedRadioboxPostCollection = new System.Collections.Specialized.NameValueCollection();
                }

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
                {
                    m_Root.AddedRadioboxStateCollection = new System.Collections.Specialized.NameValueCollection();
                }

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
                {
                    m_Root.AddedCheckboxStateCollection = new System.Collections.Specialized.NameValueCollection();
                }

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
            public Type DataBindTemplate { get; set; }

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
                        {
                            foundkeyarr.Add(key.Split('_')[1]);
                        }
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
                            dict.Add(Convert.ToInt32(key.Split('_')[1]), m_Root.Console.Form(key));
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
            private AdditionalResource m_Resources;
            /// <summary>
            /// Contains all added custom resources
            /// </summary>
            public AdditionalResource Resources
            {
                get
                {
                    if (m_Resources == null)
                    {
                        m_Resources = new AdditionalResource(m_Root);
                    }
                    return m_Resources;
                }
            }

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


            Head _Head;
            /// <summary>
            /// Gets or sets the head.
            /// </summary>
            /// <value>
            /// The head.
            /// </value>
            public Head Head
            {
                get
                {
                    if (_Head == null)
                    {
                        _Head = new Head(this, m_Root);
                    }
                    return _Head;
                }
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
                get
                {
                    if (_Body == null)
                    {
                        _Body = new Body(m_Root);
                    }
                    return _Body;
                }
                set { _Body = value; }
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

            Content m_Content;
            /// <summary>
            /// Gets the filter field.
            /// </summary>
            /// <param name="property">The property.</param>
            /// <returns></returns>
            public Field GetFilterField(string property)
            {
                if (string.IsNullOrEmpty(m_Root.CurrentVisitor.Data.Serialized))
                {
                    return new Field();
                }
                if (m_Content == null)
                {
                    m_Content = Content.GetDeserialized(m_Root.CurrentVisitor.Data["wim_FilterInfo"].Value);
                }
                if (m_Content == null)
                {
                    return new Field();
                }

                return m_Content[property];
            }

            /// <summary>
            /// Applies the filter field.
            /// </summary>
            /// <param name="property">The property.</param>
            /// <param name="value">The value.</param>
            public async Task ApplyFilterFieldAsync(string property, object value)
            {
                if (m_Content == null)
                {
                    m_Content = Content.GetDeserialized(m_Root.CurrentVisitor.Data["wim_FilterInfo"].Value);
                }

                if (m_Content == null)
                {
                    m_Content = new Content();
                }

                List<Field> list = new List<Field>();
                if (m_Content.Fields != null)
                {
                    foreach (var x in m_Content.Fields)
                    {
                        list.Add(x);
                    }
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

                m_Root.CurrentVisitor.Data.Apply("wim_FilterInfo", Content.GetSerialized(m_Content));
                await m_Root.CurrentVisitor.SaveAsync();
            }

            public async Task ClearFilterAsync()
            {
                m_Root.CurrentVisitor.Data.Apply("wim_FilterInfo", null);
                await m_Root.CurrentVisitor.SaveAsync();
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
            public void AddElement(object sender, Property property, object value, bool valueIsSet)
            {
                bool isEditable = !property.IsOnlyRead;

                if (!m_Root.IsEditMode)
                {
                    isEditable = false;
                }

                if (m_Root.m_infoList == null)
                {
                    m_Root.m_infoList = new List<Beta.GeneratedCms.Source.Component.ListInfoItem>();
                    if (CustomDataInstance == null)
                    {
                        CustomDataInstance = new CustomData();
                    }
                }

                Beta.GeneratedCms.Source.Component.ListInfoItem tmp = new Beta.GeneratedCms.Source.Component.ListInfoItem();
                //tmp.ContentAttribute = contentInfo;
                tmp.SenderInstance = sender;

                if (sender != null && !string.IsNullOrEmpty(property.FieldName))
                {
                    tmp.Info = sender.GetType().GetProperty(property.FieldName);
                }

                tmp.IsVisible = !property.IsHidden;
                tmp.IsEditable = isEditable;

                if (tmp.Info == null)
                {
                    tmp.Info = GetType().GetProperty("CustomDataInstance");
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
                {
                    isEditable = false;
                }

                if (string.IsNullOrEmpty(property))
                {
                    throw new Exception(string.Format("Property for title ['{0}'] can not be NULL", contentInfo.Title));
                }

                if (m_Root.m_infoList == null)
                {
                    m_Root.m_infoList = new List<Beta.GeneratedCms.Source.Component.ListInfoItem>();
                    if (CustomDataInstance == null)
                    {
                        CustomDataInstance = new CustomData();
                    }
                }

                Beta.GeneratedCms.Source.Component.ListInfoItem tmp = new Beta.GeneratedCms.Source.Component.ListInfoItem();
                tmp.ContentAttribute = contentInfo;
                tmp.SenderInstance = sender;

                if (sender != null && !string.IsNullOrEmpty(property))
                {
                    tmp.Info = sender.GetType().GetProperty(property);
                }

                tmp.IsVisible = isVisible;
                tmp.IsEditable = isEditable;

                if (tmp.Info == null)
                {
                    tmp.Info = GetType().GetProperty("CustomDataInstance");
                    tmp.SenderInstance = this;
                    tmp.IsVirtualProperty = true;
                    tmp.Property = new Property();
                    tmp.Name = property;

                    if (CustomDataInstance.HasProperty(property))
                    {
                        if (valueIsSet)
                        {
                            CustomDataInstance.ApplyObject(property, value);
                        }
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
                {
                    return CustomDataInstance[property];
                }

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

                if (m_TranslationValues == null || !m_TranslationValues.ContainsKey(property))
                {
                    return null;
                }

                object x = m_TranslationValues[property];
                return x == null ? null : x.ToString();
            }

            internal string TranslationLastRequest { get; set; }

            /// <summary>
            /// Gets or sets the translation section start.
            /// </summary>
            /// <value>The translation section start.</value>
            public string TranslationSectionStart { get; set; }

            /// <summary>
            /// Gets or sets the list item element list.
            /// </summary>
            /// <value>The list item element list.</value>
            public List<Beta.GeneratedCms.Source.Component.ListInfoItem> ListItemElementList { get; set; }

            Dictionary<string, object> m_TranslationValues;
            bool m_ShowTranslationData;
            internal bool m_ShowTranslationDataStart;
            internal bool ShowTranslationData
            {
                get
                {

                    if (TranslationSectionStart != null && m_ShowTranslationDataStart)
                    {
                        if (TranslationLastRequest != TranslationSectionStart)
                        {
                            return false;
                        }
                        else
                        {
                            m_ShowTranslationDataStart = false;
                        }
                    }
                    return m_ShowTranslationData;
                }
                set
                {
                    m_ShowTranslationData = value;
                }
            }

            /// <summary>
            /// Adds the translation master.
            /// </summary>
            /// <param name="property">The property.</param>
            /// <param name="value">The value.</param>
            public void AddTranslationMaster(string property, object value)
            {
                if (m_TranslationValues == null)
                {
                    m_TranslationValues = new Dictionary<string, object>();
                }

                if (m_TranslationValues.ContainsKey(property))
                {
                    m_TranslationValues[property] = value;
                }
                else
                {
                    m_TranslationValues.Add(property, value);
                }

                ShowTranslationData = true;
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
                info = new ContentListItem.TextFieldAttribute(title, maxLength) { TextType = inputType };
                if (info == null)
                {
                    return;
                }
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
                info = new ContentListItem.TextLineAttribute(title);
                if (info == null)
                {
                    return;
                }
                AddElement(null, property, true, true, info, value, true);
            }

            /// <summary>
            /// Gets or sets the custom data.
            /// </summary>
            /// <value>The custom data.</value>
            public CustomData CustomDataInstance { get; set; }

            public void ApplyCustomDataInstance(System.Xml.Linq.XElement element)
            {
                CustomDataInstance = new CustomData();
                if (element != null)
                {
                    CustomDataInstance.ApplySerialized(element.ToString());
                }
            }

        }

        internal List<Beta.GeneratedCms.Source.Component.ListInfoItem> m_infoList;

        /// <summary>
        /// 
        /// </summary>
        [Obsolete("This property is obsolete, please use ListData or ListDataTable instead")]
        public DataTable Data { get; set; }

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
                    {
                        m_CurrentPage = -1;
                    }
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

            public void ApplyListDataColumns(ListDataColumns listDataColumns)
            {
                m_ListDataColumns = new ListDataColumns();
                ListDataColumn[] arr = new ListDataColumn[listDataColumns.List.Count];
                listDataColumns.List.CopyTo(arr);
                foreach (var i in arr)
                {
                    m_ListDataColumns.Add(i);
                }
            }

            internal IList m_ListData { get; set; }
            internal IList m_AppliedSearchGridItem;
            internal IList m_ChangedSearchGridItem;
            internal bool m_IsLinqUsed;
            internal bool m_IsListDataScrollable;
            internal int m_ListDataRecordCount;
            internal int m_ListDataRecordPageCount;
            internal ListDataColumns m_ListDataColumns;
            internal string m_DataTitle;

        }

        private ListDataColumns m_ListDataColumnsBackup;
        /// <summary>
        /// Set the backup grid for when having multiple grids.
        /// </summary>
        void ApplyListDataColumnBackup()
        {
            m_ListDataColumnsBackup = new ListDataColumns();
            ListDataColumn[] arr = new ListDataColumn[ListDataColumns.List.Count];
            ListDataColumns.List.CopyTo(arr);
            foreach (var i in arr)
            {
                m_ListDataColumnsBackup.Add(i);
            }
        }

        /// <summary>
        /// If miltiple grids exists the columns could be cleared after applying the first grid, so make a backup of it.
        /// </summary>
        void CheckListDataColumnBackup()
        {
            if (_Grids == null || _Grids.Count == 0)
            {
                return;
            }

            ListDataColumns = m_ListDataColumnsBackup;
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
                CheckListDataColumnBackup();
                _index++;
                return true;
            }

            if (_Grids == null)
            {
                return false;
            }

            if (_Grids.Count > _index)
            {
                _GridIndex = _index;
                AppliedSearchGridItem = _Grids[_index].m_AppliedSearchGridItem;
                ChangedSearchGridItem = _Grids[_index].m_ChangedSearchGridItem;
                ListDataColumns = _Grids[_index].m_ListDataColumns;
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
                {
                    return 0;
                }
                else return _Grids.Count;
            }
        }

        private ListDataColumns m_ListDataColumns;
        /// <summary>
        /// The columns that are used in the search result
        /// </summary>
        public ListDataColumns ListDataColumns
        {
            set { m_ListDataColumns = value; }
            get
            {
                if (m_ListDataColumns == null)
                {
                    m_ListDataColumns = new ListDataColumns();
                }
                return m_ListDataColumns;
            }
        }

        internal int _GridIndex = 0;

        /// <summary>
        /// The search result. Add columns by using ListDataColumns.Add
        /// </summary>
        //[Obsolete("[20120715:MM] Please use ListDataAdd", false)]
        //internal IList ListData { get; set; }
        internal IList ListData
        {
            get
            {
                if (_Grids == null || !_Grids.Any())
                {
                    return null;
                }
                return _Grids[_GridIndex].m_ListData;
            }
        }
        internal bool m_IsListDataScrollable
        {
            get
            {
                if (_Grids == null || !_Grids.Any())
                {
                    return false;
                }
                return _Grids[_GridIndex].m_IsListDataScrollable;
            }
        }

        internal string m_DataTitle
        {
            get
            {
                if (_Grids == null || !_Grids.Any())
                {
                    return null;
                }
                return _Grids[_GridIndex].m_DataTitle;
            }
        }

        internal bool m_IsLinqUsed
        {
            get
            {
                if (_Grids == null || !_Grids.Any())
                {
                    return false;
                }
                return _Grids[_GridIndex].m_IsLinqUsed;
            }
        }

        internal int m_ListDataRecordCount
        {
            get
            {
                if (_Grids == null || !_Grids.Any())
                {
                    return 0;
                }
                return _Grids[_GridIndex].m_ListDataRecordCount;
            }
        }

        internal int m_ListDataRecordPageCount
        {
            get
            {
                if (_Grids == null || !_Grids.Any())
                {
                    return 0;
                }
                return _Grids[_GridIndex].m_ListDataRecordPageCount;
            }
        }
        public int ListDataRecordCount => m_ListDataRecordCount;
        public int ListDataRecordPageCount => m_ListDataRecordPageCount;

        public IList AppliedSearchGridItem { get; set; }
        public IList ChangedSearchGridItem { get; set; }

        internal int m_ListDataInterLineCount;

        #region List Data Add

        /// <summary>
        /// Add another IEnumerable list of data entities to the grid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        public void ListDataAdd<T>(IEnumerable<T> source)
        {
            ListDataAdd(source, null);
        }

        /// <summary>
        /// Add another IEnumerable list of data entities to the grid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        public void ListDataAdd<T>(IEnumerable<T> source, string title)
        {
            ListDataAdd(source, title, false);
        }

        /// <summary>
        /// Add another IEnumerable list of data entities to the grid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="title">The title.</param>
        /// <param name="isScrollable"></param>
        public void ListDataAdd<T>(IEnumerable<T> source, string title, bool isScrollable)
        {
            if (source == null)
            {
                return;
            }

            if (_Grids == null)
            {
                _Grids = new List<GridInstance>();
            }

            GridInstance grid = new GridInstance();
            int step = GridDataCommunication.PageSize;

            grid.m_DataTitle = title;
            grid.m_IsLinqUsed = true;
            grid.m_IsListDataScrollable = isScrollable;
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
                    {
                        grid.m_ListData = source.Take(step).ToArray();
                    }
                    else
                    {
                        grid.m_ListData = source.Skip(start).Take(step).ToArray();
                    }
                }
                else
                {
                    grid.m_ListData = source.Take(step).ToArray();
                }
            }
            else
            {
                if (GridDataCommunication.ShowAll)
                {
                    grid.m_ListData = source.ToArray();
                }
                else
                {
                    grid.m_ListData = source.Take(step).ToArray();
                }
            }

            AppliedSearchGridItem = grid.m_ListData;

            grid.ApplyListDataColumns(ListDataColumns);
            //  Clone the current grid for when using multiple grids
            _Grids.Add(grid);
        }


        /// <summary>
        /// Add another IQueryable list of data entities to the grid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        public void ListDataAdd<T>(IQueryable<T> source)
        {
            ListDataAdd(source, null);
        }

        /// <summary>
        /// Add another IQueryable list of data entities to the grid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="title">The title.</param>
        public void ListDataAdd<T>(IQueryable<T> source, string title)
        {
            ListDataAdd(source, title, false);
        }

        /// <summary>
        /// Add another IQueryable list of data entities to the grid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="title">The title.</param>
        public void ListDataAdd<T>(IQueryable<T> source, string title, bool isScrollable)
        {
            ListDataAdd(source.ToList(), title, isScrollable);
        }
        #endregion

        #region List Data Apply (Obsolete, throw exception for migration purposes)
        /// <summary>
        /// Add an IEnumerable list of data entities to the grid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        [Obsolete("Use ListDataAdd<T>(IEnumerable<T> source)", true)]
        public void ListDataApply<T>(IEnumerable<T> source)
        {
            ListDataApply(source, null);
        }

        /// <summary>
        /// Lists the data apply.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="isScrollable">if set to <c>true</c> [is scrollable].</param>
        [Obsolete("Use ListDataAdd<T>(IEnumerable<T> source, bool isScrollable)", true)]
        public void ListDataApply<T>(IEnumerable<T> source, bool isScrollable)
        {
            ListDataApply(source, null, isScrollable);
        }

        /// <summary>
        /// Lists the data apply.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="title">The title.</param>
        [Obsolete("Use ListDataAdd<T>(IEnumerable<T> source, string title)", true)]
        public void ListDataApply<T>(IEnumerable<T> source, string title)
        {
            ListDataApply(source, title, false);
        }

        /// <summary>
        /// Add an IEnumerable list of data entities to the grid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="title">The title of the datagrid.</param>
        [Obsolete("Use ListDataAdd<T>(IEnumerable<T> source, string title, bool isScrollable)", true)]
        public void ListDataApply<T>(IEnumerable<T> source, string title, bool isScrollable)
        {
            ListDataAdd(source, title, isScrollable);
        }

        /// <summary>
        /// Add an IQueryable list of data entities to the grid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        [Obsolete("Use ListDataAdd<T>(IQueryable<T> source)", true)]
        public void ListDataApply<T>(IQueryable<T> source)
        {
            ListDataApply(source, null);
        }

        /// <summary>
        /// Add an IQueryable list of data entities to the grid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        [Obsolete("Use ListDataAdd<T>(IQueryable<T> source, string title)", true)]
        public void ListDataApply<T>(IQueryable<T> source, string title)
        {
            ListDataAdd(source, title);
        }
        #endregion

        /// <summary>
        /// The search result. Add columns by using ListDataColumns.Add
        /// </summary>
        public DataTable ListDataTable { get; set; }

        /// <summary>
        /// The search result. Add columns by using ListDataColumns.Add
        /// </summary>
        internal DataTable GraphDataTable { get; set; }

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
                {
                    m_ResetValueList = new List<string>();
                }
                return m_ResetValueList;
            }
        }

        public IComponentListTemplate DashBoardFilterTemplate { get; set; }

        public int DashBoardElementWidth { get; set; } = 1;

        /// <summary>
        /// Gets or sets the dash board HTML container.
        /// </summary>
        /// <value>The dash board HTML container.</value>
        public string DashBoardHtmlContainer { get; set; }

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
            get
            {
                if (_NoData == null)
                {
                    _NoData = new NoDataItem();
                }
                return _NoData;
            }
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

        /// <summary>
        /// Gets or sets the current list request property.
        /// </summary>
        /// <value>The current list request property.</value>
        public string CurrentListRequestProperty { get; set; }

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
            get
            {
                if (m_ListTitle == null)
                {
                    if (Console != null && Console.Item.HasValue)
                    {
                        m_ListTitle = CurrentList.SingleItemName;
                    }
                    else
                    {
                        m_ListTitle = CurrentList.Name;
                    }
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
            ListTitle = title;

            if (!string.IsNullOrEmpty(subtitle) && !string.IsNullOrEmpty(description))
            {
                ListDescription = string.Concat(string.Format("<h2>{0}</h2>", subtitle), description);
            }
            else if (!string.IsNullOrEmpty(subtitle) && string.IsNullOrEmpty(description))
            {
                ListDescription = string.Format("<h2>{0}</h2>", subtitle);
            }
            else if (string.IsNullOrEmpty(subtitle) && !string.IsNullOrEmpty(description))
            {
                ListDescription = description;
            }
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
                {
                    m_ListDescription = CurrentList.Description;
                }
                return m_ListDescription;
            }
        }

        /// <summary>
        /// The maximum record count shown per view on the dashboard (default value: 50).
        /// </summary>
        public int SearchViewDashboardMaxLength { get; set; } = 50;

        [Obsolete("Please use [wim.Page.Body.Grid.SetClickLayer()]", false)]
        public LayerSize SearchListTarget
        {
            set
            {
                var specification = new Grid.LayerSpecification();
                specification.Apply(value);

                Page.Body.Grid.SetClickLayer(specification);
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
            get
            {
                if (!m_SearchListCanClickThrough && IsSortOrderMode)
                {
                    m_SearchListCanClickThrough = true;
                }

                return m_SearchListCanClickThrough;
            }
        }

        /// <summary>
        /// When the NEW RECORDS buttons is pressed it deeplinks to a list item. This list can defer from the originating list by applying this other componentListId. 
        /// </summary>
        public int? ListCreateNewItemListId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has data dependency. 
        /// If data dependeny is set to 'true' the ListLoad event will be triggered twice.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has data dependency; otherwise, <c>false</c>.
        /// </value>
        public bool HasDataDependency { get; set; }

        /// <summary>
        /// Does this search list have the sortorder functionality? (this overrides the automatic visibility of the setSortOrder method)
        /// </summary>
        public bool HasSortOrder { get; set; }

        /// <summary>
        /// Does this search list have the sortorder functionality in the form mode? (this overrides the automatic visibility of the setSortOrder method)
        /// </summary>
        public bool HasSingleItemSortOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [auto expression].
        /// </summary>
        /// <value><c>true</c> if [auto expression]; otherwise, <c>false</c>.</value>
        public bool AutoExpression { get; set; }

        /// <summary>
        /// Gets the postback value.
        /// </summary>
        /// <value>The postback value.</value>
        public string PostbackValue
        {
            get
            {
                string value = "";
                if (Context == null)
                {
                    return value;
                }

                value = Console.Form("autopostback");
                if (value == null)
                {
                    value = "";
                }
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
                {
                    return false;
                }
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
            get
            {
                if (CurrentList == null)
                {
                    return true;
                }
                return CurrentList.Data["wim_hasExport_PDF"].ParseBoolean();
            }
        }

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
            HasSortOrder = true;
        }

        /// <summary>
        /// Gets the Mediakiwi Console. 
        /// Do not use when in ComponentList Routing mode, use a constructor with an IServiceProvider instead.
        /// And get the IHttpContextAccessor from the Service Provider 
        /// </summary>
        /// <value>The console.</value>
        public Beta.GeneratedCms.Console Console { get; set; }

        internal bool _IsRedirected;
        /// <summary>
        /// Redirect to an URL and inform the backend to stop processing this page
        /// </summary>
        /// <param name="url"></param>
        public void Redirect(string url)
        {
            _IsRedirected = true;
            Console.Response.Redirect(url, false);
        }

        public string AddApplicationPath(string path, bool appendUrl = false)
        {
            return Console.AddApplicationPath(path, appendUrl);
        }

        public void SaveVisit()
        {
            Console.SaveVisit();
        }

        public async Task SaveVisitAsync()
        {
            await Console.SaveVisitAsync();
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
            {
                url = string.Concat(url, (string.IsNullOrEmpty(url) ? "" : "&"), "item=", itemID.Value);
            }

            return url;
        }

        void AddToUrl(ref string url, string queryParam)
        {
            if (Console.Request.Query[queryParam].FirstOrDefault() == null)
            {
                return;
            }

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
            get { return m_SearchResultItemPassthroughParameter; }
        }

        /// <summary>
        /// Gets or sets the search result item passthrough parameter property which is taken from the ListData collection.
        /// This value overrides 'SearchResultItemPassthroughParameter'. The framework will use the response of the property as the clickthrough URL.
        /// </summary>
        /// <value>The search result item passthrough parameter property.</value>
        public string SearchResultItemPassthroughParameterProperty { get; set; }

        /// <summary>
        /// Used for migrating from old WIM to new WIM
        /// </summary>
        internal bool CanAddNewItemIsSet;

        /// <summary>
        /// Gets or sets a value indicating that after the PreRender the elements are filled using the POST values. 
        /// The default setting is that the values are filled using the set properties.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [post pre render load form request]; otherwise, <c>false</c>.
        /// </value>
        public bool ShouldPostPreRenderLoadFormRequest { get; set; }

        private bool m_CanAddNewItem;
        /// <summary>
        /// Can the application user create a new record?
        /// </summary>
        public bool CanAddNewItem
        {
            set
            {
                CanAddNewItemIsSet = true;
                m_CanAddNewItem = value;
            }
            get
            {
                if (!CanAddNewItemIsSet)
                {
                    m_CanAddNewItem = CurrentList.Data["wim_CanCreate"].ParseBoolean();
                }
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
            set
            {
                m_CanSaveAndAddNew = value;
                m_CanSaveAndAddNew_IsSet = true;
            }
            get
            {
                if (!CanAddNewItem)
                {
                    return false;
                }

                if (!m_CanSaveAndAddNew_IsSet)
                {
                    m_CanSaveAndAddNew = CurrentList.Option_CanSaveAndAddNew;
                }

                return m_CanSaveAndAddNew;
            }
        }

        /// <summary>
        /// Gets or sets the content info items.
        /// </summary>
        /// <value>The content info items.</value>
        public IContentInfo[] ContentInfoItems { get; set; }

        /// <summary>
        /// Gets the info item.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public IContentInfo GetInfoItem(string property)
        {
            if (ContentInfoItems == null)
            {
                return null;
            }

            foreach (IContentInfo info in ContentInfoItems)
            {
                if (info.Property.Name == property)
                {
                    return info;
                }
            }

            return null;
        }

        /// <summary>
        /// This the search result present in cache? If so there is no need for assignment to ListData or ListDataTable.
        /// </summary>
        public bool IsCachedSearchResult { get; set; }

        /// <summary>
        /// Force showing a new result set (bypassing the cached version)
        /// </summary>
        public bool ForceLoad { get; set; }

        internal Folder m_CurrentFolder;
        /// <summary>
        /// Gets the current folder.
        /// </summary>
        /// <value>The current folder.</value>
        public Folder CurrentFolder
        {
            get
            {
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
                                Page pageInstance = Mediakiwi.Data.Page.SelectOne(page, false);

                                //if (pageInstance.FolderID == 0)
                                //{
                                //    Wim.Utilities.CacheItemManager.FlushIndexOfCacheObjects("Data_Sushi.Mediakiwi.Data.Page");
                                //    pageInstance = Sushi.Mediakiwi.Data.Page.SelectOne(page, false);
                                //}

                                m_CurrentFolder = pageInstance.Folder;
                                return m_CurrentFolder;
                            }

                            Folder folderEntity = null;

                            if (!string.IsNullOrEmpty(Context.Request.Query["gallery"]))
                            {
                                Gallery gallery = Gallery.Identify(Context.Request.Query["gallery"]);

                                if (!(gallery == null || gallery.ID == 0))
                                {
                                    folderEntity = new Folder();
                                    folderEntity.ID = gallery.ID;
                                    folderEntity.ParentID = gallery.ParentID.GetValueOrDefault(0);
                                    folderEntity.Name = gallery.Name;
                                    //  Fix for galleries (add the / at the end for the Level)!
                                    folderEntity.CompletePath = gallery.CompletePath == "/" ? "/" : string.Concat(gallery.CompletePath, "/");
                                    folderEntity.Type = FolderType.Gallery;
                                }
                            }

                            else if (Utility.ConvertToInt(Context.Request.Query["asset"]) > 0)
                            {
                                int galleryId = 0;

                                Asset asset = Asset.SelectOne(Utility.ConvertToInt(Context.Request.Query["asset"]));
                                if (!(asset == null || asset.ID == 0))
                                {
                                    galleryId = asset.GalleryID;
                                }

                                if (galleryId > 0)
                                {
                                    Gallery gallery = Gallery.SelectOne(galleryId);
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
                            else if (Console.ItemType == RequestItemType.Undefined
                                && (CurrentList.FolderID.HasValue && CurrentList.Type != ComponentListType.Browsing))
                            {
                                if (CurrentList.Target == ComponentListTarget.List)
                                {
                                    folderEntity = Folder.SelectOneChild(CurrentList.FolderID.Value, CurrentSite.ID);
                                }
                                else
                                {
                                    folder = CurrentList.FolderID.Value;
                                }
                            }
                            else
                            {
                                if (CurrentSite.ID > 0)
                                {
                                    int top = Utility.ConvertToInt(Context.Request.Query["top"]); 

                                    if (top == 0 && CurrentSite.HasLists)
                                    {
                                        top = 2;
                                    }
                                    else if (top == 0 && CurrentSite.HasPages)
                                    {
                                        top = 1;
                                    }

                                    if (top == 1 && !CurrentApplicationUserRole.CanSeePage)
                                    {
                                        top = 2;
                                    }

                                    switch (top)
                                    {
                                        case 0:
                                            {
                                                folderEntity = new Folder();
                                                folderEntity.ID = 0;
                                                folderEntity.Name = "Dashboard";
                                                folderEntity.Type = FolderType.Undefined;
                                            }
                                            break;
                                        case 1:
                                            {
                                                folderEntity = Folder.SelectOneBySite(CurrentSite.ID, FolderType.Page);
                                            }
                                            break;
                                        case 2:
                                            {
                                                folderEntity = Folder.SelectOneBySite(CurrentSite.ID, FolderType.List);
                                            }
                                            break;
                                        case 3:
                                            {
                                                Gallery gallery = Gallery.SelectOneRoot();
                                                folderEntity = new Folder();
                                                folderEntity.ID = gallery.ID;
                                                folderEntity.Name = gallery.Name;
                                                folderEntity.Type = FolderType.Gallery;
                                            }
                                            break;
                                        case 4:
                                            {
                                                folderEntity = Folder.SelectOneBySite(CurrentSite.ID, FolderType.Administration);
                                            }
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
                        {
                            m_CurrentFolder = Folder.SelectOne(folder);
                        }
                    }
                    return m_CurrentFolder;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.StackTrace);
                }
            }
        }

        public bool IsSortOrderMode
        {
            get { return Console.Form("sortOrder") == "1"; }
        }

        /// <summary>
        /// Is ths list search requested for listitem collection created?
        /// </summary>
        public bool IsListItemCollectionMode { get; set; }

        /// <summary>
        /// Is this list shown in the subselect mode?
        /// </summary>
        public bool IsSubSelectMode { get; set; }

        public bool IsLayerMode
        {
            get { return Console.OpenInFrame > 0; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is search list mode.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is search list mode; otherwise, <c>false</c>.
        /// </value>
        public bool IsSearchListMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is export mode_ XLS.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is export mode_ XLS; otherwise, <c>false</c>.
        /// </value>
        public bool IsExportMode_XLS { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [item is component].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [item is component]; otherwise, <c>false</c>.
        /// </value>
        public bool ItemIsComponent { get; set; }

        /// <summary>
        /// Is this list shown in editmode?
        /// </summary>
        public bool IsEditMode { get; set; }

        /// <summary>
        /// Determines whether [is current list] [the specified me].
        /// </summary>
        /// <param name="me">Me.</param>
        /// <returns>
        ///   <c>true</c> if [is current list] [the specified me]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsCurrentList { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is in savemode.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is savemode; otherwise, <c>false</c>.
        /// </value>
        public bool IsSaveMode { get; set; }

        public bool ShouldValidate { get; set; }

        public bool IsDeleteMode { get; set; }

        // Wim Can be chosen as datastore for list input. In this case ComponentListVersion acts as datastore.
        // The data is stored in XML format. 
        // Note: Define this in the Constructor of the list!

        /// <summary>
        /// Wim Can be chosen as datastore for list input. In this case ComponentListVersion acts as datastore.
        /// This option automatically set the property 'CanContainSingleInstancePerDefinedList' to true.
        /// Note: Define this in the Constructor of the list!
        /// </summary>
        public bool ShouldActAsDataStore { get; set; }

        /// <summary>
        /// Wim Can be chosen as partial datastore for list input. In this case ComponentListVersion acts as a partial datastore.
        /// ListLoad event is not triggered and is managed internally.
        /// The data is stored in XML format.
        /// Note: Define this in the Constructor of the list!
        /// </summary>
        public bool ShouldActAsPartialDataStore { get; set; }

        /// <summary>
        /// The text to be shown on the search button
        /// </summary>
        internal string SearchButtonText
        {

            get
            {
                if (CurrentList == null)
                {
                    return null;
                }
                return CurrentList.Data["wim_LblSearch"].Value;
            }
        }

        /// <summary>
        /// Does this list item have a publish button in the action bar?
        /// </summary>
        public bool HasPublishOption { get; set; }

        public void CloseLayer(string notification = null, int timeOutInSeconds = 0, bool hideOutput = true)
        {
            if (!string.IsNullOrWhiteSpace(notification))
            {
                Notification.AddNotification(notification);
            }

            if (hideOutput)
            {
                Page.HideDataForm = true;
                Page.HideDataGrid = true;
                Page.HideFormFilter = true;
            }

            if (timeOutInSeconds == 0)
            {
                OnSaveScript = $"<script>mediakiwi.closeLayer();</script>";
            }
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
        /// Does this list item have a take-down button in the action bar?
        /// </summary>
        public bool HasTakeDownOption { get; set; }

        // Wim Can be chosen as datastore for list input (ShouldActAsDataStore). 
        // But to present the list accordingly in a search overview a listtag is needed for representation and search.
        // Note: Define this in the Constructor of the list!
        /// <summary>
        /// Wim Can be chosen as datastore for list input (ShouldActAsDataStore). 
        /// But to present the list accordingly in a search overview a listtag is needed for representation and search.
        /// Note: Define this in the Constructor of the list!
        /// </summary>
        public string DataStoreDescriptiveTag { get; set; }

        /// <summary>
        /// Can contain only one instance per defined list. 
        /// Note: Define this in the Constructor of the list!
        /// </summary>
        public bool CanContainSingleInstancePerDefinedList { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether [show in full width mode].
        /// </summary>
        /// <value>
        /// <c>true</c> if [show in full width mode]; otherwise, <c>false</c>.
        /// </value>
        [Obsolete("Not applicable in Mediakiwi")]
        public bool ShowInFullWidthMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [hide open close toggle].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [hide open close toggle]; otherwise, <c>false</c>.
        /// </value>
        public bool HideOpenCloseToggle { get; set; } = true;

        private bool m_OpenInEditModeIsSet;
        private bool m_OpenInEditMode = true;
        /// <summary>
        /// Should the list item always open in editmode?
        /// Note: Define this in the Constructor of the list!
        /// </summary>
        /// <value><c>true</c> if [open in edit mode]; otherwise, <c>false</c>.</value>
        public bool OpenInEditMode
        {
            set
            {
                m_OpenInEditMode = value;
                m_OpenInEditModeIsSet = true;
            }
            get
            {
                if (m_OpenInEditModeIsSet)
                {
                    return m_OpenInEditMode;
                }

                if (CurrentList == null)
                {
                    return false;
                }
                else
                {
                    //  Page content
                    if (CurrentList.Type == ComponentListType.Browsing)
                    {
                        return false;
                    }

                    return CurrentList.Data["wim_OpenInEdit"].ParseBoolean(true);
                }
            }
        }

        /// <summary>
        /// Loaded just before the ListSave event is handled.
        /// </summary>
        public ComponentListVersion ComponentListVersion { get; internal set; }

        /// <summary>
        /// Flush all caching, equivalent to "?Flush=me"
        /// </summary>
        public void FlushCache(bool redirectToSelf = false)
        {
            Caching.FlushAll(true);

            if (redirectToSelf && Console != null && Console.Context != null)
                Console.Context.Response.Redirect(GetUrl());

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
