using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Data.SqlClient;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Framework;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Specialized;
using Sushi.Mediakiwi.Framework.ContentInfoItem;
using Microsoft.AspNetCore.Http;
using Sushi.Mediakiwi.Data.Interfaces;

namespace Sushi.Mediakiwi.Data
{
    public class DataDemo : iDataHtmlNode, iDataContext
    {
        public DataDemo Me
        {
            get
            {
                DataDemo d = new DataDemo();
                return d;
            }
        }
        public ICountry[] Countries
        {
            get
            {
                return (from item in Country.SelectAll() select item).Take(5).ToArray();
            }
        }

        public string MyCustomFormat(object value, string parameter)
        {
            return parameter;
        }

        public ICountry Current
        {
            get
            {
                return Country.SelectOne(1);
            }
        }

        HtmlAgilityPack.HtmlNode _Node;
        public HtmlAgilityPack.HtmlNode Node
        {
            get { return _Node; }
            set { _Node = value; }
        }

        public string TESTNODE
        {
            get
            {
                return string.Concat("Dit is de huidige node: ", Node.Name);
            }
        }

        public DataContext Context { get;set;}
    }

    // FOR INTERFACE EXTENSION: SEE ParseNode_mk_register
    public interface iDataHtmlNode
    {
        HtmlNode Node { get; set; }
    }

    public interface iDataPage
    {
        Page Page { get; set; }
    }

    public class DataContext
    {
        public HttpContext Context { get; set; }
        public NameValueCollection MappedQuery { get; set; }
        public HttpRequest Request { get; set; }
        public HttpResponse Response { get; set; }
    }

    public interface iDataContext
    {
        DataContext Context { get; set; }
    }

    public class DataTemplate
    {
        DataContext _Context;
        internal void ApplyHttpContext(HttpContext context, NameValueCollection mapped)
        {
            _Context = new DataContext()
            {
                Context = context,
                Request = context.Request,
                Response = context.Response,
                MappedQuery = mapped
            };
        }

        /// <summary>
        /// Parses the inheritence.
        /// </summary>
        /// <param name="mk_bind">The mk_bind.</param>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        bool ParseInheritence(DataTemplateAttribute mk_bind, ref Field field)
        {
            //System.Web.HttpContext.Current.Response.Write(string.Format("<!-- {0}: type:{1} noinput:{2} take:{3}  -->", mk_bind.ID, mk_bind.HasProperty("type"), mk_bind.HasProperty("no-input"), mk_bind.HasProperty("take")));

            if (mk_bind.HasProperty("take"))
            {
                var attribute = mk_bind.GetAttribute("take");

                //  When localisation is applied, use the localised content, BUT ignore this when the property has no set type (so can not be translated).
                //if (this.CurrentPage != null && !this.IsInherited && mk_bind.HasProperty("type") && !mk_bind.HasProperty("no-input"))
                //    return false;
                if (this.CurrentPage != null)
                {
               
                    if (this.IsInherited)
                    {
                        //  Do nothing
                    }
                    else
                    {
                        //  Check for inheritance ignore
                        if (mk_bind.HasProperty("type") && !mk_bind.HasProperty("no-input"))
                        {
                            return false;
                        }
                    }
                }
                
                if (attribute.IsInnerAttribute && attribute.Template.HasProperty("name"))
                {
                    int page = attribute.Template.HasProperty("page") ? Convert.ToInt32(attribute.Template["page"]) : 0;
                    bool useMaster = attribute.Template.HasProperty("parent");

                    if (useMaster)
                    {
                        if (CurrentPage != null && CurrentPage.MasterID.HasValue)
                            page = CurrentPage.MasterID.Value;
                        else
                            page = 0;
                    }

                    if (page > 0)
                    {
                        var pageInstance = Sushi.Mediakiwi.Data.Page.SelectOne(page);
                        if (!pageInstance.IsNewInstance)
                        {
                            //  Determine properties
                            var componentProperties = pageInstance.GetComponentProperties(attribute.Template.ID, attribute.Template["name"]);
                            if (componentProperties != null)
                            {
                                int index = attribute.Template.HasProperty("index") ? Convert.ToInt32(attribute.Template["index"]) : 0;
                                if (componentProperties != null && componentProperties.Length > index)
                                {
                                    field.Value = componentProperties[index];
                                    //CB; 17-09-2014; change om beter het data.link gevult te krijgen. Niet geheel duidelijk of dit 
                                    // de beste aanpak is maar het is nu om take hyperlinks te laten werken
                                    if (mk_bind["type"] == "href")
                                    {
                                        field.Type = (int)ContentType.Hyperlink;
                                    }
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        void ParseNode_mk_repeat(HtmlAgilityPack.HtmlNode node, Sushi.Mediakiwi.Data.Content content)
        {
            //  mk-repeat="instance in country.Countries"
            var mk_repeat = node.Attributes["mk-repeat"].Value;
            var value_split = mk_repeat.Split(new string[] { " in " }, StringSplitOptions.RemoveEmptyEntries);

            //  mk-repeat="instance"
            var collectionInstance = value_split[0].Trim();
            //  mk-repeat="country.Countries"
            var collectionGroup = value_split[1].Trim();
            //  mk-repeat="country"
            var collectionIdentifier = collectionGroup.Split('.')[0];
            //  mk-repeat="Countries"
            var collectionProperty = collectionGroup.Split('.')[1];

            //  find country"
            InstanceLibrary findInstance = null;
            _Instances.TryGetValue(collectionIdentifier, out findInstance);
            if (findInstance != null)
            {
                //  find Countries"
                // CB; a Little bit more flexibiliy on repeaters; not only accept arrays but all collections that are IEnumerable like lists
                var repeatItems = GetEntityProperty(findInstance.Instance, collectionProperty) as IEnumerable;
                var template = node.InnerHtml;
                StringBuilder build = new StringBuilder();
                foreach (var item in repeatItems)
                {
                    DataTemplate dt = new DataTemplate();
                    dt.CurrentPage = this.CurrentPage;
                    dt._Context = this._Context;

                    //  Needed for repeater in repeater, but not working YET!
                    dt._Instances = this._Instances;
                    if (dt._Instances.ContainsKey(collectionInstance))
                        dt._Instances[collectionInstance] = new InstanceLibrary() { Instance = item, Reserved = collectionInstance };
                    else
                        dt._Instances.Add(collectionInstance, new InstanceLibrary() { Instance = item, Reserved = collectionInstance });
                    var appliedTemplate = SerializeShortHand(template, item, collectionInstance);
                    dt.ParseRepeaterItem(appliedTemplate, item);
                    build.Append(dt.Rendered);
                }
                if (this._Instances.ContainsKey(collectionInstance))
                    this._Instances.Remove(collectionInstance);
                node.InnerHtml = build.ToString();
            }
        }

        /// <summary>
        /// Serializes the short hand {[....]} tags.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <returns></returns>
        string SerializeShortHand(string html)
        {
            return SerializeShortHand(html, null, null);
        }

        /// <summary>
        /// Applies the data parser.
        /// </summary>
        /// <param name="parser">The parser.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        string ApplyDataParser(string parser, object value)
        {
            if (value == null)
                return null;

            if (!string.IsNullOrEmpty(parser))
            {
                var methodsplit = parser.Split(new string[] { "(", ")" }, StringSplitOptions.RemoveEmptyEntries);
                var method = methodsplit[0].Trim();
                string parameters = null;

                if (methodsplit.Length == 2)
                {
                    parameters = methodsplit[1].Trim().Replace("'", string.Empty);
                }
                if (method == "NumberFormat")
                {
                    return Utility.ConvertToLong(value).ToString(parameters);
                }
                else if (method == "DateFormat")
                {
                    return ((DateTime)value).ToString(parameters);
                }
                else
                {
                    var split = method.Split('.');
                    InstanceLibrary findInstance = null;
                    _Instances.TryGetValue(split[0], out findInstance);
                    if (findInstance != null)
                    {
                        var propertyInstance = findInstance.Instance.GetType().GetMethod(split[1]);
                        List<object> arr = new List<object>();
                        arr.Add(value);
                        arr.Add(parameters);
                        return propertyInstance.Invoke(findInstance.Instance, arr.ToArray()).ToString();
                    }
                }
            }

            return value.ToString();
        }

        /// <summary>
        /// Serializes the short hand {[....]} tags (in instance,prefix for mk-repeat).
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="prefix">The prefix.</param>
        /// <returns></returns>
        string SerializeShortHand(string html, object instance, string prefix)
        {
            return Mk_Bind.Replace(html, delegate(Match m)
            {
                var shorthand = m.Groups["text"].Value;
                string parser = null;
                if (shorthand.Contains("|"))
                {
                    var split = shorthand.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    shorthand = split[0].Trim();
                    parser = split[1].Trim();
                }

                object value;

                var field = FindField(shorthand);
                if (field == null)
                {
                    if (instance != null && !string.IsNullOrEmpty(prefix) && shorthand.StartsWith(prefix))
                        //  Instance item
                        value = FindReserved(shorthand, instance);
                    else
                        //  Old way
                        value = FindReserved(shorthand);
                }
                else
                    value = field == null ? null : field.Value;

                //if (value == null)
                //    return null;
                //return value.ToString();
                return ApplyDataParser(parser, value);
            });
        }

        /// <summary>
        /// Parses the node_mk_register.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="content">The content.</param>
        void ParseNode_mk_register(HtmlAgilityPack.HtmlNode node, Sushi.Mediakiwi.Data.Content content)
        {
            var mk_load_source = GetAttribute(node.Attributes["mk-load-source"].Value, true);
            object instance = null;
            string dllName = string.Concat(mk_load_source[0], ".dll");
            try
            {
                instance = Utils.CreateInstance(dllName, mk_load_source[1]);
                SetInterfaces(node, instance);
                if (!string.IsNullOrEmpty(mk_load_source[2]))
                {
                    instance = GetEntityProperty(instance, mk_load_source[2]);
                    SetInterfaces(node, instance);
                }
            }
            catch (Exception exc)
            {
                throw new Exception(String.Format("Could not do load instance for {0} from {1}", mk_load_source[1], dllName));
            }

            if (_Instances == null)
                _Instances = new Dictionary<string, InstanceLibrary>();
            if (!this._Instances.ContainsKey(mk_load_source.ID))
                _Instances.Add( mk_load_source.ID, new InstanceLibrary() { Instance = instance, Reserved = mk_load_source.ID });
        }

        void SetInterfaces(HtmlAgilityPack.HtmlNode node, object instance)
        {
            if (instance != null && instance is iDataHtmlNode)
                ((iDataHtmlNode)instance).Node = node;

            if (instance != null && instance is iDataPage)
                ((iDataPage)instance).Page = this.CurrentPage;

            if (instance != null && instance is iDataContext)
                ((iDataContext)instance).Context = this._Context;
        }

        //
        void ParseNode_mk_replacement(HtmlAgilityPack.HtmlNode node, Sushi.Mediakiwi.Data.Content content)
        {
            SetNodeAttribute(node, "style");
            SetNodeAttribute(node, "class");
            SetNodeAttribute(node, "alt");
            SetNodeAttribute(node, "title");
            SetNodeAttribute(node, "src");
            SetNodeAttribute(node, "href");
            SetNodeAttribute(node, "id");
        }

        void SetNodeAttribute(HtmlAgilityPack.HtmlNode node, string attribute)
        {
            string mknode = string.Concat("mk-", attribute);
            if (!node.Attributes.Contains(mknode))
                return;

            var bind = node.Attributes[mknode];
            var value = bind.Value;

            if (node.Attributes.Contains(attribute))
                node.Attributes[attribute].Value = value;
            else
                node.Attributes.Add(attribute, value);

            node.Attributes.Remove(bind);
        }

        /// <summary>
        /// Parses the node_mk_bind.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="content">The content.</param>
        void ParseNode_mk_bind(HtmlAgilityPack.HtmlNode node, Sushi.Mediakiwi.Data.Content content)
        {
            if (!node.Attributes.Contains("mk-bind"))
                return;
            var bind = node.Attributes["mk-bind"];
            var mk_bind = GetAttribute(bind.Value);
            //bool removeBind = true;

            if (mk_bind != null)
            {
                #region Input property
                if (mk_bind.HasProperty("type"))
                {
                    var type = ContentType.Undefined;

                    var meta = new Sushi.Mediakiwi.Framework.MetaData();
                    meta.Name = mk_bind.ID;

                    if (mk_bind.HasProperty("default"))
                    {
                        var contentHTML = node.InnerHtml.Trim();
                        var match = Mk_Bind_Text.Match(contentHTML);
                        if (match.Success)
                        {
                            meta.Default = match.Groups["text"].Value;
                        }
                        else
                            meta.Default = contentHTML;
                    }

                    meta.Mandatory = (mk_bind.IsTrue("required") ? "1" : null);

                    meta.Title = mk_bind.HasProperty("title") ? mk_bind["title"] : "NO-TITLE";

                    if (!mk_bind.HasProperty("no-input"))
                        _MetaDataList.Add(meta);

                    bool isFallBack = false;
                    if (mk_bind.HasProperty("fallback"))
                        isFallBack = true;

                    Content.Field data = null;
                    if (content == null || content.IsNull(mk_bind.ID))
                    {
                        data = new Content.Field() { Property = mk_bind.ID };

                    }
                    else
                        data = content[mk_bind.ID];

                    //  Set inheritence
                    var isHerited = ParseInheritence(mk_bind, ref data);
                    

                    string value = data == null ? null : data.Value;
                    string fieldValue = null;

                    bool isSourceBased = false;
                    #region Determine input type
                    switch (mk_bind["type"])
                    {
                        case "tb":
                        case "text":
                        case "txt":
                            type = ContentType.TextField;
                            value = data == null ? meta.Default : data.Value;
                            break;
                        case "textarea":
                        case "ta":
                            type = ContentType.TextArea;
                            value = data == null ? meta.Default : data.Value;

                            break;
                        case "multi":
                            type = ContentType.MultiField;
                            value = data == null ? meta.Default : data.Value;
                            
                            var multiFieldHandler = Sushi.Mediakiwi.Data.Environment.GetInstance<IMultiFieldParser>();
                            value = multiFieldHandler.WriteHTML(MultiField.GetDeserialized(value, meta.Name), this.CurrentPage);

                            break;
                        case "richtext":
                        case "rte":
                            type = ContentType.RichText;

                            value = data == null ? meta.Default : data.Value;
                            if (mk_bind.IsTrue("clean"))
                                value = Utility.CleanParagraphWrap(value);

                            break;
                        case "image":
                        case "img":
                        case "gfx":
                            isSourceBased = true;
                            if (data != null && data.Image != null && !data.Image.IsNewInstance)
                            {
                                fieldValue = value;
                                value = data.Image.Url;
                            }
                            else
                                value = null;

                            type = ContentType.Binary_Image;
                            break;
                        case "document":
                        case "doc":
                            if (data != null && data.Document != null && !data.Document.IsNewInstance)
                            {
                                fieldValue = value;
                                if (data.Document != null)
                                {
                                    value = data.Document.Title;
                                    if (node.Attributes.Contains("href"))
                                        node.Attributes["href"].Value = data.Document.Url;
                                    else
                                        node.Attributes.Add("href", data.Document.Url);
                                }
                            }
                            else
                                value = null;
                            type = ContentType.Binary_Document;
                            break;
                        case "cb":
                        case "checkbox":
                            type = ContentType.Choice_Checkbox;
                            break;
                        case "dd":
                        case "dropdown":
                        case "select":
                            type = ContentType.Choice_Dropdown;
                            break;
                        case "rd":
                        case "radio":
                            type = ContentType.Choice_Radio;
                            break;
                        case "d":
                        case "date":
                            type = ContentType.Date;
                            if (mk_bind.HasProperty("format") && !string.IsNullOrEmpty(value))
                                if (Utility.IsNumeric(value))
                                    value = new DateTime(long.Parse(value)).ToString(mk_bind["format"].ToString());

                            break;
                        case "dt":
                        case "datetime":
                            type = ContentType.DateTime;
                            if (mk_bind.HasProperty("format") && !string.IsNullOrEmpty(value))
                                if (Utility.IsNumeric(value))
                                    value = new DateTime(long.Parse(value)).ToString(mk_bind["format"].ToString());
                            break;
                        case "lnk":
                        case "href":
                        case "link":
                            type = ContentType.Hyperlink;
                            if (!string.IsNullOrEmpty(value))
                            {
                                if (value == "0")
                                    value = null;
                                else
                                {
                                    if (node.Name != "a")
                                    {
                                        HtmlNode tmp = _doc.CreateElement("a");
                                        node.AppendChild(tmp);
                                        node = tmp;
                                    }
                                    fieldValue = value;
                                    if (data.Link != null)
                                    {
                                        value = data.Link.Text;
                                        if (node.Attributes.Contains("href"))
                                            node.Attributes["href"].Value = data.Link.Url;
                                        else
                                            node.Attributes.Add("href", data.Link.Url);
                                    }

                                }
                            }
                            break;
                        case "page":
                            type = ContentType.PageSelect;
                            if (!string.IsNullOrEmpty(value))
                            {
                                if (value == "0")
                                    value = null;
                                else
                                {
                                    if (node.Name != "a")
                                    {
                                        HtmlNode tmp = _doc.CreateElement("a");
                                        node.AppendChild(tmp);
                                        node = tmp;
                                    }
                                    fieldValue = value;
                                    if (data.Page != null)
                                    {
                                        value = data.Page.LinkText;
                                        if (node.Attributes.Contains("href"))
                                            node.Attributes["href"].Value = data.Page.Url;
                                        else
                                            node.Attributes.Add("href", data.Page.Url);
                                    }
                                }
                            }
                            break;
                    }
                    #endregion Determine input type

                    if (type == ContentType.Undefined)
                    {
                        //  Unknown type
                        return;
                    }
                    else
                    {
                        meta.IsInheritedField = mk_bind.HasProperty("take") ? "1" : null;
                        meta.InteractiveHelp = mk_bind["help"];
                        meta.ContentTypeSelection = ((int)type).ToString();

                        if (isFallBack && !isSourceBased)
                        {
                            if (string.IsNullOrEmpty(value))
                            {
                                var match = Mk_Bind_Text.Match(node.InnerHtml);
                                if (match.Success)
                                {
                                    value = match.Groups["text"].Value;
                                }
                                else
                                    value = node.InnerHtml;
                            }
                        }
                        if (mk_bind.HasProperty("hide") || mk_bind.HasProperty("show"))
                        {
                            if (_redoNodeList == null)
                                _redoNodeList = new List<HtmlNode>();
                            _redoNodeList.Add(node);
                        }

                        var field = new Sushi.Mediakiwi.Data.Content.Field(mk_bind.ID, type, fieldValue == null ? value : fieldValue);


                        _Component_Fields.Add(field);

                        if (string.IsNullOrEmpty(value) && !(isFallBack && isSourceBased))
                        {
                            //  If no value en not fallback of a source based item
                            if (mk_bind.HasProperty("show") && mk_bind["show"] == "*")
                            {
                            }
                            else
                            {
                                node.Remove();
                                return;
                            }
                        }
                        else
                        {

                            if (isSourceBased)
                            {
                                if (node.Attributes["src"] == null)
                                    node.Attributes.Add("src", value);
                                else
                                    node.Attributes["src"].Value = value;
                            }
                            else
                            {
                                if (type != ContentType.Choice_Checkbox)
                                {
                                    if (mk_bind.HasProperty("no-text"))
                                    {
                                        //  Do nothing
                                    }
                                    else
                                    {
                                        if (Mk_Bind_Text.IsMatch(node.InnerHtml))
                                            node.InnerHtml = ApplyDataParser(mk_bind.Parser, Mk_Bind_Text.Replace(node.InnerHtml, value));
                                        else
                                            node.InnerHtml = ApplyDataParser(mk_bind.Parser, (value == null ? string.Empty : value));
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion Input property
                else
                {
                    if (_redoNodeList == null)
                        _redoNodeList = new List<HtmlNode>();
                    _redoNodeList.Add(node);
                }
            }
        }

        List<DataComponentNode> _DataNodes;
        /// <summary>
        /// Parses the node_mk_component.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="content">The content.</param>
        void ParseNode_mk_component(HtmlAgilityPack.HtmlNode node, Sushi.Mediakiwi.Data.Content content)
        {
            if (node.Attributes.Contains("mk-component"))
            {
                var componentAttributeNode = node.Attributes["mk-component"];
                var componentAttribute = GetAttribute(componentAttributeNode.Value);
                //componentAttributeNode.Remove();

                if (!_isComponentInstance)
                {
                    if (this.CurrentPage != null && !this.CurrentPage.IsNewInstance)
                    {
                        foreach (var c in this.Components)
                        {
                            if (c.FixedFieldName == componentAttribute.ID)
                            {
                                DataTemplate dt = new DataTemplate();
                                dt.CurrentPage = this.CurrentPage;
                                dt._Context = this._Context;
                                dt.ParseComponent(c);

                                DataComponentNode dataComponent = new DataComponentNode()
                                {
                                    Node = node,
                                    Template = dt
                                };

                                if (_DataNodes == null)
                                    _DataNodes = new List<DataComponentNode>();
                                _DataNodes.Add(dataComponent);
                            }
                        }
                    }
                    return;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTemplate"/> class.
        /// </summary>
        public DataTemplate()
        {
        }

        /// <summary>
        /// Parses the source data; if the location is filesystem or url based that data will be consumed. 
        /// </summary>
        /// <param name="pageTemplate">The page template.</param>
        /// <returns>Did the parse run successfull? If not view the ParseError properties.</returns>
        public bool ParseSourceData(Sushi.Mediakiwi.Data.PageTemplate pageTemplate)
        {
            if (pageTemplate == null)
                return false;

            if (!pageTemplate.IsSourceBased)
                return true;

            var parseSource = Sushi.Mediakiwi.Data.Environment.Current["PARSE_SOURCE", true, "0", "When True, the source is updated when the changes"] == "1";
            if (parseSource)
            {
                if (pageTemplate.Location != null
                    && (pageTemplate.Location.EndsWith(".html")
                    || pageTemplate.Location.EndsWith(".shtml"))
                    )
                {
                    var context = System.Web.HttpContext.Current;
                    if (context == null)
                        return false;

                    var path = context.Server.MapPath(Utility.AddApplicationPath(pageTemplate.Location));
                    System.IO.FileInfo nfo = new System.IO.FileInfo(path);

                    if (!nfo.Exists)
                        return false;

                    var changed = Utility.ConvertToSystemDataTime(nfo.LastWriteTimeUtc);

                    if (changed.Ticks > pageTemplate.LastWriteTimeUtc.Ticks)
                    {
                        string code = null;
                        if (pageTemplate.Location.EndsWith(".shtml"))
                            code = Wim.Utilities.WebScrape.GetUrlResponse(Utility.AddApplicationPath(pageTemplate.Location, true));
                        else
                            code = System.IO.File.ReadAllText(path);

                        pageTemplate.Source = code;
                        pageTemplate.LastWriteTimeUtc = changed;
                    }
                }
            }

            if (!pageTemplate.m_IsSourceChanged)
                return true;

            string cleanHtml = null;
            var newAvailableTemplates = this.GetAvailableTemplates(pageTemplate.ID, pageTemplate.Source, out cleanHtml);
            var existingTemplates = Sushi.Mediakiwi.Data.AvailableTemplate.SelectAll(pageTemplate.ID);
            SaveAvailableTemplates(pageTemplate, existingTemplates, newAvailableTemplates);
            pageTemplate.Source = cleanHtml;
            pageTemplate.Save();

            Sushi.Mediakiwi.Data.EnvironmentVersionLogic.Flush();
            pageTemplate.CheckComponentTemplates();

            return true;
        }

        /// <summary>
        /// Saves the available templates.
        /// </summary>
        /// <param name="pageTemplate">The page template.</param>
        /// <param name="existingTemplates">The existing templates.</param>
        /// <param name="newtemplates">The newtemplates.</param>
        void SaveAvailableTemplates(Sushi.Mediakiwi.Data.PageTemplate pageTemplate, Sushi.Mediakiwi.Data.IAvailableTemplate[] existingTemplates, Sushi.Mediakiwi.Data.IAvailableTemplate[] newtemplates)
        {
            if (newtemplates == null || newtemplates.Length == 0)
            {
                Sushi.Mediakiwi.Data.AvailableTemplate.Delete(pageTemplate.ID, null);
                return;
            }
            //  Delete existing
            foreach (Sushi.Mediakiwi.Data.AvailableTemplate availableTemplate in existingTemplates)
            {
                var search = (from t in newtemplates where t.ID == availableTemplate.ID select t).FirstOrDefault();
                if (search == null)
                    availableTemplate.Delete();
            }
            foreach (var template in newtemplates)
            {
                template.Save();
            }
        }

        //Sushi.Mediakiwi.Data.Page _page;
        /// <summary>
        /// Initializes a new instance of the <see cref="DataTemplate"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        public DataTemplate(Sushi.Mediakiwi.Data.Page page)
        {
            this.CurrentPage = page;
            Sushi.Mediakiwi.Data.Content content = null;
            Parse(page.Template.Source, content);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTemplate"/> class.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <param name="content">The content.</param>
        public DataTemplate(string html, string content, bool isComponentInstance = false)
        {
            _isComponentInstance = isComponentInstance;
            Parse(html, content);
        }

        /// <summary>
        /// Parses the page.
        /// </summary>
        /// <param name="page">The page.</param>
        public void ParsePage(Sushi.Mediakiwi.Data.Page page, PageTemplate template)
        {
            m_CurrentPage = page;
            Sushi.Mediakiwi.Data.Content content = null;
            //CB: changed first parameter so it came from caller. Otherwise we can't overwrite the template
            Parse(template.Source, content);
        }

        bool _isComponentInstance = false;
        /// <summary>
        /// Parses the component template.
        /// </summary>
        /// <param name="componentTemplate">The component template.</param>
        /// <returns>The serialized metadata</returns>
        public string ParseComponentTemplate(Sushi.Mediakiwi.Data.ComponentTemplate componentTemplate)
        {
            _isComponentInstance = true;
            Sushi.Mediakiwi.Data.Content content = null;
            Parse(componentTemplate.Source, content);
            return Utility.GetSerialized(this.MetaData);
        }

        /// <summary>
        /// Parses the component.
        /// </summary>
        /// <param name="component">The component.</param>
        public void ParseComponent(Sushi.Mediakiwi.Data.Component component)
        {
            _isComponentInstance = true;
            this.CurrentComponent = component;
            Parse(component.Source, component.Content);
        }

        /// <summary>
        /// Parses the repeater item.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="instance">The instance.</param>
        public void ParseRepeaterItem(string source, object instance)
        {
            _isComponentInstance = true;
            Sushi.Mediakiwi.Data.Content content = null;
    
            Parse(source, content, instance, true);

        }


        /// <summary>
        /// Get the available templates
        /// </summary>
        /// <param name="pageTemplateID">The page template identifier.</param>
        /// <param name="html">The HTML.</param>
        /// <param name="cleanHtml">The clean HTML.</param>
        /// <returns></returns>
        public Sushi.Mediakiwi.Data.IAvailableTemplate[] GetAvailableTemplates(int pageTemplateID, string html, out string cleanHtml)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            List<Sushi.Mediakiwi.Data.IAvailableTemplate> arr = new List<IAvailableTemplate>();

            string attribute = "mk-component";
            var collection = doc.DocumentNode.SelectNodes("//@mk-component");

            int sortOrder = 0;
            if (collection != null)
            {
                #region Collection listing
                foreach (var item in collection)
                {
                    sortOrder++;
                    var attrib = GetAttribute(item.Attributes[attribute].Value);
                    Sushi.Mediakiwi.Data.ComponentTemplate ct = Sushi.Mediakiwi.Data.ComponentTemplate.SelectOneBySourceTag(attrib.ID);
                    if (ct == null || ct.IsNewInstance)
                    {
                        ct = new ComponentTemplate()
                        {
                            Source = item.OuterHtml,
                            SourceTag = attrib.ID,
                            Name = (attrib.HasProperty("name") ? attrib["name"].ToString() : attrib.ID),
                            ReferenceID = attrib["ref"],
                            Location = "#",
                            TypeDefinition = "#"
                        };

                        var dt = new Sushi.Mediakiwi.Data.DataTemplate();

                        ct.MetaData = dt.ParseComponentTemplate(ct);
                        ct.Save();
                        //  Created
                    }
                    else
                    {
                        //  Existing, save it.
                        string check1 = Utility.CreateChecksum(ct.Source);
                        string check2 = Utility.CreateChecksum(item.OuterHtml);

                        if (!check1.Equals(check2))
                        {
                            if (attrib.HasProperty("name"))
                                ct.Name = attrib["name"].ToString();
                            if (attrib.HasProperty("ref"))
                                ct.ReferenceID = attrib["ref"].ToString();
                            ct.Source = item.OuterHtml;

                            var dt = new Sushi.Mediakiwi.Data.DataTemplate();
                            dt.CurrentPage = this.CurrentPage;
                            ct.MetaData = dt.ParseComponentTemplate(ct);
                            ct.Save();
                        }
                    }
                    //string uid = (attrib["uid"] == null ? attrib.ID : attrib["uid"]);
                    Sushi.Mediakiwi.Data.IAvailableTemplate at = Sushi.Mediakiwi.Data.AvailableTemplate.SelectOne(pageTemplateID, attrib.ID);

                    var section = attrib["container"];
                    if (string.IsNullOrEmpty(section))
                    {
                        var containerNode = FindParentAttribute(item, "mk-container");
                        if (containerNode != null)
                            section = containerNode.ID;
                    }

                    if (at == null || at.IsNewInstance)
                    {
                        at = new AvailableTemplate()
                        {
                            Target = section,
                            ComponentTemplateID = ct.ID,
                            FixedFieldName = attrib.ID,
                            PageTemplateID = pageTemplateID,
                            IsPossible = true
                        };
                    }
                    at.Target = section;
                    at.SortOrder = sortOrder;

                    arr.Add(at);
                    item.RemoveAllChildren();
                }
                #endregion Collection listing
            }

            cleanHtml = doc.DocumentNode.OuterHtml;
            return arr.ToArray();
        }

        /// <summary>
        /// Cleans the component template by removing the mk-component tags.
        /// </summary>
        /// <param name="ct">The ct.</param>
        /// <returns></returns>
        public string CleanComponentTemplate(Sushi.Mediakiwi.Data.ComponentTemplate ct)
        {
            if (string.IsNullOrEmpty(ct.Source))
                return null;

            HtmlAgilityPack.HtmlDocument cmp = new HtmlAgilityPack.HtmlDocument();
            cmp.LoadHtml(ct.Source);
            RemoveAttribute(cmp, "mk-component");
            return cmp.DocumentNode.OuterHtml;
        }

        /// <summary>
        /// Loads the templates.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <returns></returns>
        public string CompleteComponentTemplates(string html)
        {
            if (string.IsNullOrEmpty(html))
                return null;

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

            doc.LoadHtml(html);
            var nodes = doc.DocumentNode.ChildNodes;

            List<Sushi.Mediakiwi.Data.AvailableTemplate> arr = new List<AvailableTemplate>();

            string attribute = "mk-component";
            var componentTemplateCollection = doc.DocumentNode.SelectNodes("//@mk-component");

            int sortOrder = 0;
            if (componentTemplateCollection != null)
            {
                #region Component collection
                foreach (var item in componentTemplateCollection)
                {
                    sortOrder++;
                    var attrib = GetAttribute(item.Attributes[attribute].Value);
                    Sushi.Mediakiwi.Data.ComponentTemplate ct = Sushi.Mediakiwi.Data.ComponentTemplate.SelectOneBySourceTag(attrib.ID);
                    if (ct == null || ct.IsNewInstance)
                    {
                        //  Created
                    }
                    else
                    {
                        var ct_doc = CleanFirstNode(ct, true);
                        var newNode = HtmlNode.CreateNode(ct_doc.DocumentNode.OuterHtml);
                        item.ParentNode.ReplaceChild(newNode.ParentNode, item);
                    }
                }
                #endregion Component collection
            }


            return doc.DocumentNode.OuterHtml;
        }

        /// <summary>
        /// Cleans the first node (used for component template cleanup)
        /// </summary>
        /// <param name="ct">The ct.</param>
        /// <param name="applyTemplateProperties">if set to <c>true</c> [apply template properties].</param>
        /// <returns></returns>
        HtmlAgilityPack.HtmlDocument CleanFirstNode(Sushi.Mediakiwi.Data.ComponentTemplate ct, bool applyTemplateProperties)
        {
            HtmlAgilityPack.HtmlDocument cmp = new HtmlAgilityPack.HtmlDocument();
            cmp.LoadHtml(ct.Source);
            var ctAttribute = cmp.DocumentNode.ChildNodes[0].Attributes["mk-component"];
            if (ctAttribute == null)
            {
                cmp.DocumentNode.ChildNodes[0].Attributes.Add("mk-component", ct.SourceTag);
                ctAttribute = cmp.DocumentNode.ChildNodes[0].Attributes["mk-component"];
            }

            var ctAttributeItem = GetAttribute(ctAttribute.Value);
            if (applyTemplateProperties)
            {
                ctAttributeItem["name"] = ct.Name;
                ctAttributeItem["ref"] = ct.ReferenceID;
                ctAttribute.Value = ctAttributeItem.ToString();
            }
            else
            {
                ctAttributeItem["name"] = null;
                ctAttributeItem["ref"] = null;
                ctAttribute.Value = ctAttributeItem.ToString();
            }
            return cmp;
        }


        /// <summary>
        /// Removes the attribute.
        /// </summary>
        /// <param name="doc">The document.</param>
        /// <param name="attribute">The attribute.</param>
        void RemoveAttribute(HtmlAgilityPack.HtmlDocument doc, string attribute)
        {
            var a = string.Concat("//@", attribute);
            var arr = doc.DocumentNode.SelectNodes(a);
            if (arr != null)
                foreach (var item in arr)
                    item.Attributes[attribute].Remove();
        }

        /// <summary>
        /// Parses the specified HTML.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <param name="data">The data.</param>
        void Parse(string html, string data)
        {
            var content = Sushi.Mediakiwi.Data.Content.GetDeserialized(data);
            Parse(html, content);
        }

        HtmlAgilityPack.HtmlDocument _doc;

        /// <summary>
        /// Parses the specified HTML.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <param name="content">The content.</param>
        void Parse(string html, Sushi.Mediakiwi.Data.Content content, object instance = null, bool forceLoadShowBeforeRedo = false)
        {
            if (!string.IsNullOrEmpty(html))
            {
                _doc = new HtmlAgilityPack.HtmlDocument();
                _doc.LoadHtml(html);

                _Component_Fields = new List<Sushi.Mediakiwi.Data.Content.Field>();

                _MetaDataList = new List<MetaData>();

                //var nodes = _doc.DocumentNode.ChildNodes;

                SerializeContent(_doc.DocumentNode, content);
                if (forceLoadShowBeforeRedo)
                    ForceRepeaterShowHide(_doc.DocumentNode, instance);

                _Content = content;

                //[08.12.14:MM] Added to clean up the output for list based components
                if (_Content == null)
                    _Content = new Data.Content();
                //[08.12.14:MM] Added to clean up the output for list based components
                _Content.ApplyMetaData(_MetaDataList.ToArray());

                this.Content = _Content;
                this.MetaData = _MetaDataList.ToArray();

                if (!_isComponentInstance)
                {
                    if (_Page_Fields == null)
                        _Page_Fields = new List<Data.Content.Field>();

                    //  Get all fields
                    if (_DataNodes != null)
                    {
                        _DataNodes.ForEach(node =>
                        {
                            if (node.Template._Component_Fields != null)
                                _Page_Fields.AddRange(node.Template._Component_Fields);

                            if (node.Template._Instances != null)
                            {
                                if (this._Instances == null)
                                    this._Instances = new Dictionary<string, InstanceLibrary>();
                                foreach (string key in node.Template._Instances.Keys)
                                {
                                    if ( !this._Instances.ContainsKey(key))
                                        this._Instances.Add(key, node.Template._Instances[key]);
                                }
                            }
                        });

                        //  Distribute all fields
                        _DataNodes.ForEach(node =>
                        {
                            node.Template._Page_Fields = _Page_Fields;
                            node.Template._Instances = _Instances;
                        });
                        //  Reparse all cross-component data
                        this.RedoSerializeContent();
                        _DataNodes.ForEach(node =>
                        {
                            node.Template.RedoSerializeContent();
                            node.Replace();
                        });
                    }
                    else
                        this.RedoSerializeContent();

                    //  Clean the page
                    this.Clean();
                }
            }
            else
                this._RenderIsEmpty = true;
        }

        Regex _relativePath = new Regex("\\.\\./*", RegexOptions.Compiled | RegexOptions.Multiline);

        /// <summary>
        /// Cleans this instance.
        /// </summary>
        public void Clean()
        {
            var mk_bind = _doc.DocumentNode.SelectNodes("//@mk-bind");
            if (mk_bind != null) foreach (var node in mk_bind) node.Attributes["mk-bind"].Remove();

            var mk_hide = _doc.DocumentNode.SelectNodes("//@mk-hide");
            if (mk_hide != null) foreach (var node in mk_hide) node.Attributes["mk-hide"].Remove();

            var mk_show = _doc.DocumentNode.SelectNodes("//@mk-show");
            if (mk_show != null) foreach (var node in mk_show) node.Attributes["mk-show"].Remove();

            var mk_component = _doc.DocumentNode.SelectNodes("//@mk-component");
            if (mk_component != null) foreach (var node in mk_component) node.Attributes["mk-component"].Remove();

            var mk_container = _doc.DocumentNode.SelectNodes("//@mk-container");
            if (mk_container != null) foreach (var node in mk_container) node.Attributes["mk-container"].Remove();

            var mk_source = _doc.DocumentNode.SelectNodes("//@mk-load-source");
            if (mk_source != null) foreach (var node in mk_source) node.Attributes["mk-load-source"].Remove();

        }

        public Sushi.Mediakiwi.Data.Content Content;
        public Sushi.Mediakiwi.Framework.MetaData[] MetaData;


        public bool _RenderIsEmpty;
        public string Rendered
        {
            get
            {
                if (_RenderIsEmpty || _doc == null)
                    return null;

                //[08.12.14:MM] Added to clean up the output for list based components
                this.Clean();
                string app = Utility.AddApplicationPath("/");
                string shortHand = SerializeShortHand(_doc.DocumentNode.OuterHtml);
                string candidate = _relativePath.Replace(Mk_Clean.Replace(shortHand, string.Empty), app);
                return candidate;
            }
        }

        Content _Content;
        List<Field> _Page_Fields;
        List<Field> _Component_Fields;
        List<MetaData> _MetaDataList;
        //Dictionary<string, string> _Dict;

        /// <summary>
        /// Finds the parent attribute.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="attribute">The attribute.</param>
        /// <returns></returns>
        DataTemplateAttribute FindParentAttribute(HtmlAgilityPack.HtmlNode node, string attribute)
        {
            HtmlAgilityPack.HtmlNode parent = node.ParentNode;
            while (parent != null)
            {
                if (parent.Attributes.Contains(attribute))
                    return GetAttribute(parent.Attributes[attribute].Value);

                parent = parent.ParentNode;
            }
            return null;
        }


        Regex rex = new System.Text.RegularExpressions.Regex("[\\r\\n\"'{}]", System.Text.RegularExpressions.RegexOptions.Multiline | RegexOptions.Compiled);
        Regex lineBreak = new System.Text.RegularExpressions.Regex("(<br>).*", System.Text.RegularExpressions.RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex commaOutSide = new Regex(@"(,)(?=(?:[^}]|{[^}]*})*$)", RegexOptions.Multiline | RegexOptions.Compiled);

        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="isIndexBased">if set to <c>true</c> [is index based].</param>
        /// <returns></returns>
        DataTemplateAttribute GetAttribute(string value, bool isIndexBased = false)
        {
            if (String.IsNullOrEmpty(value)) throw new Exception("Please remove empty mk-bind");
            DataTemplateAttribute t = new DataTemplateAttribute();
            List<DataTemplateAttributeValue> list = new List<DataTemplateAttributeValue>();

            var split = value.Split(new string[] {"[","]", " | "}, StringSplitOptions.RemoveEmptyEntries);
            t.ID = rex.Replace(split[0], string.Empty).Trim();

            int index = 0;
            if (split.Length > 1 && !string.IsNullOrEmpty(split[1]))
            {
                var collection = commaOutSide.Split(split[1]);

                if (value.Contains(" | "))
                {
                    if (split.Length == 2)
                        t.Parser = split[1].Trim();
                    else if (split.Length == 3)
                        t.Parser = split[2].Trim();
                }
                foreach (var item in collection)
                {
                    if (item == ",")
                        continue;

                    if (item.Contains("{") && item.Contains("}"))
                    {
                        DataTemplateAttribute innerTemplate = new DataTemplateAttribute();
                        var innerSplit = item.Split("{}".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        var nv = innerSplit[0].Split(':');
                        innerTemplate.ID = nv[1].Trim();

                        var innerCollection = innerSplit[1].Split(',');
                        List<DataTemplateAttributeValue> innerlist = new List<DataTemplateAttributeValue>();
                        foreach (var innerItem in innerCollection)
                        {
                            var innerNv = innerItem.Split(':');
                            if (innerNv.Length == 2)
                            {
                                innerlist.Add(new DataTemplateAttributeValue() { Name = rex.Replace(innerNv[0], string.Empty).Trim(), Value = rex.Replace(innerNv[1], string.Empty) });
                            }
                            else
                            {
                                if (isIndexBased)
                                    innerlist.Add(new DataTemplateAttributeValue() { Name = index.ToString(), Value = innerNv[0] });
                                else
                                    innerlist.Add(new DataTemplateAttributeValue() { Name = rex.Replace(innerNv[0], string.Empty).Trim(), Value = "1" });
                            }
                        }
                        innerTemplate.Values = innerlist;
                        list.Add(new DataTemplateAttributeValue() { IsInnerAttribute = true, Name = nv[0].Trim(), Template = innerTemplate });
                    }
                    else
                    {
                        var nv = item.Split(':');
                        if (nv.Length == 2)
                        {
                            list.Add(new DataTemplateAttributeValue() { Name = rex.Replace(nv[0], string.Empty).Trim(), Value = rex.Replace(nv[1], string.Empty) });
                        }
                        else
                        {
                            if (isIndexBased)
                                list.Add(new DataTemplateAttributeValue() { Name = index.ToString(), Value = nv[0] });
                            else
                                list.Add(new DataTemplateAttributeValue() { Name = rex.Replace(nv[0], string.Empty).Trim(), Value = "1" });
                        }
                    }
                    index++;

                }
                t.Values = list;
            }
            return t;

        }

        bool IsPreview
        {
            get
            {
                return System.Web.HttpContext.Current.Request["preview"] == "1";
            }
        }

        bool IsInherited
        {
            get
            {
                if (IsPreview)
                    return this.CurrentPage.InheritContentEdited;
                return this.CurrentPage.InheritContent;
            }
        }

        Sushi.Mediakiwi.Data.Component[] _Components;
        Sushi.Mediakiwi.Data.Component[] Components
        {
            get
            {
                if (_Components == null)
                {
                    if (IsPreview)
                    {
                        List<Sushi.Mediakiwi.Data.Component> tmp = new List<Component>();
                        var components = Sushi.Mediakiwi.Data.ComponentVersion.SelectAll(this.CurrentPage.ID);
                        foreach (var version in components)
                            tmp.Add(version.Convert());
                        _Components = tmp.ToArray();
                    }
                    else
                    {
                        //  [17 nov 2014:MM] Inheritence should be ignored when using from within the templating system (as of TAKE).
                        _Components = Sushi.Mediakiwi.Data.Component.SelectAllInherited(this.CurrentPage.ID, true);
                    }
                }
                return _Components;
            }
        }
        // CB; Wish of mark rienstra to may use an approot in the src/hrefs
        private const string APP_ROOT_SIGN = "~";
        List<HtmlAgilityPack.HtmlNode> _redoNodeList;
        /// <summary>
        /// Serializes the content.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="content">The content.</param>
        void SerializeContent(HtmlAgilityPack.HtmlNode node, Sushi.Mediakiwi.Data.Content content)
        {
            //System.Web.HttpContext.Current.Response.Write(string.Format("<!-- {0} IC{1}, ICE{2}, IN{3} -->", "HELP: "
            //    , this.CurrentPage.InheritContent
            //    , this.CurrentPage.InheritContentEdited
            //    , this.IsInherited
            //    )
            //);
            // CB; Wish of mark rienstra to may use an approot in the src/hrefs
            var appPathNodes = node.SelectNodes("//*[@*[starts-with(.,'" + APP_ROOT_SIGN + "')]]");
            if (appPathNodes != null)
            {
                string path = Utility.AddApplicationPath("/");
                // omit the last / for clearer paths
                if (path.Length >0)
                    path = path.Substring(0, path.Length - 1);

                foreach (var appPathNode in appPathNodes)
                {
                    var attributesToParse = appPathNode.Attributes.Where(w => w.Value.StartsWith(APP_ROOT_SIGN));
                    foreach(var att in attributesToParse)
                    {
                        att.Value = att.Value.Replace(APP_ROOT_SIGN, path);
                       
                    }
                }
            }
            var mk_sources = node.SelectNodes("//@mk-load-source");
            if (mk_sources != null)
            {
                foreach (var mk_source in mk_sources)
                {
                    if (!_isComponentInstance && mk_source.Attributes["mk-component"] != null)
                    {
                        //  Do not parse as part of a component (then it will be called twice).
                    }
                    else
                        ParseNode_mk_register(mk_source, content);
                }
            }
            var mk_replacements = node.SelectNodes("//@mk-style|//@mk-title|//@mk-class|//@mk-alt|//@mk-src|//@mk-href|//@mk-id");
            if (mk_replacements != null)
            {
                foreach (var mk_replacement in mk_replacements)
                    ParseNode_mk_replacement(mk_replacement, content);
            }

            if (_isComponentInstance)
            {
                var mk_binds = node.SelectNodes("//@mk-bind");
                if (mk_binds != null)
                {
                    foreach (var mk_bind in mk_binds)
                        ParseNode_mk_bind(mk_bind, content);
                }
                var mk_repeats = node.SelectNodes("//@mk-repeat");
                if (mk_repeats != null)
                {
                    foreach (var mk_repeat in mk_repeats)
                        ParseNode_mk_repeat(mk_repeat, content);
                }
            }
            else
            {
            

                var mk_binds = node.SelectNodes("//@mk-bind");
                if (mk_binds != null)
                {
                    if (_redoNodeList == null)
                        _redoNodeList = new List<HtmlNode>();
                    foreach (var mk_bind in mk_binds)
                        _redoNodeList.Add(mk_bind);
                }

                var mk_components = node.SelectNodes("//@mk-component");
                if (mk_components != null)
                {
                    foreach (var mk_component in mk_components)
                        ParseNode_mk_component(mk_component, content);
                }
            }

            var mk_hides = node.SelectNodes("//@mk-hide");
            if (mk_hides != null)
            {
                if (_redoNodeList == null)
                    _redoNodeList = new List<HtmlNode>();
                foreach (var mk_hide in mk_hides)
                    _redoNodeList.Add(mk_hide);
            }
            var mk_shows = node.SelectNodes("//@mk-show");
            if (mk_shows != null)
            {
                if (_redoNodeList == null)
                    _redoNodeList = new List<HtmlNode>();
                foreach (var mk_show in mk_shows)
                    _redoNodeList.Add(mk_show);
            }
        }

        void ForceRepeaterShowHide(HtmlAgilityPack.HtmlNode node, object instance)
        {
            var mk_hides = node.SelectNodes("//@mk-hide");
            if (mk_hides != null)
            {
                foreach (var mk_hide in mk_hides)
                    ParseHide(mk_hide, mk_hide.Attributes["mk-hide"].Value);
            }
            var mk_shows = node.SelectNodes("//@mk-show");
            if (mk_shows != null)
            {
                foreach (var mk_show in mk_shows)
                    ParseShow(mk_show, mk_show.Attributes["mk-show"].Value, instance);
            }
        }

        Content.Field FindField(string ID)
        {
            //  First look in the current component, then in the page!
            var candidate = (from field in _Component_Fields where field.Property == ID select field).FirstOrDefault();
            if (candidate == null && _Page_Fields != null)
                candidate = (from field in _Page_Fields where field.Property == ID select field).FirstOrDefault();
            return candidate;
        }

        object FindReserved(string ID)
        {
            return FindReserved(ID, null);
        }

        object FindReserved(string ID, object instance)
        {
            object candidate = null;
            if (ID.Contains("."))
            {
                var split = ID.Split('.');
                if (split.Length > 1)
                {
                    var reserved = split[0];
                    var property = split[1];
                    var property2 = split.Length > 2 ? split[2] : null;

                    if (reserved == "mk")
                    {
                        reserved = string.Concat(reserved, ".", property);
                        property = property2;
                        property2 = null;
                    }
                    switch (reserved)
                    {
                        case "mk.page": candidate = GetEntityPropertyValue(CurrentPage, property);
                            break;
                        case "mk.site": candidate = GetEntityPropertyValue(CurrentSite, property);
                            break;
                        default:
                            if (instance != null)
                            {
                                candidate = GetEntityPropertyValue(instance, property);
                                break;
                            }
                            var find = FindField(reserved);
                            if (find != null)
                                candidate = GetEntityPropertyValue(find.DataObject, property);
                            else
                            {
                                if (_Instances != null)
                                    foreach (var i in _Instances.Values)
                                    {
                                        if (i.Reserved == reserved)
                                        {
                                            if (property2 == null)
                                                candidate = GetEntityPropertyValue(i.Instance, property);
                                            else
                                                //  Can be much better (deeper)
                                                candidate = GetEntityPropertyValue(i.Instance, property, property2);

                                        }
                                    }
                            }
                            break;

                    }
                }
            }
            return candidate;
        }

        object GetEntityPropertyValue(object entity, string property, string property2)
        {
            //  Can be much better (deeper)
            if (entity == null)
                return null;

            var propertyInstance = entity.GetType().GetProperty(property);
            if (propertyInstance != null)
            {
                var value = propertyInstance.GetValue(entity, null);
                if (value != null)
                {
                    var propertyInstance2 = value.GetType().GetProperty(property2);
                    var value2 = propertyInstance2.GetValue(value, null);
                    return value2;
                }
            }
            return null;
        }

        object GetEntityProperty(object entity, string property)
        {
            if (entity == null)
                return null;

            var propertyInstance = entity.GetType().GetProperty(property);
            if (propertyInstance != null)
            {
                return propertyInstance.GetValue(entity, null);
            }
            return null;
        }

        string GetEntityPropertyValue(object entity, string property)
        {
            if (entity == null)
                return null;

            var propertyInstance = entity.GetType().GetProperty(property);
            if (propertyInstance != null)
            {
                var value = propertyInstance.GetValue(entity, null);
                if (value != null)
                    return value.ToString();
            }
            return null;
        }

        Sushi.Mediakiwi.Data.Site m_CurrentSite;
        Sushi.Mediakiwi.Data.Site CurrentSite
        {
            get
            {
                if (m_CurrentSite == null)
                {
                    if (HttpContext.Current == null)
                        return null;

                    if (HttpContext.Current.Items["Wim.Site"] == null)
                        return null;

                    m_CurrentSite = HttpContext.Current.Items["Wim.Site"] as Sushi.Mediakiwi.Data.Site;
                }
                return m_CurrentSite;
            }
        }

        public Sushi.Mediakiwi.Data.Component CurrentComponent { get; set; }

        Sushi.Mediakiwi.Data.Page m_CurrentPage;
        Sushi.Mediakiwi.Data.Page CurrentPage
        {
            get
            {
                if (m_CurrentPage == null)
                {
                    if (HttpContext.Current == null)
                        return null;

                    if (HttpContext.Current.Items["Wim.Page"] == null)
                        return null;

                    m_CurrentPage = HttpContext.Current.Items["Wim.Page"] as Sushi.Mediakiwi.Data.Page;
                }
                return m_CurrentPage;
            }
            set
            {
                m_CurrentPage = value;
                if (value != null)
                    m_CurrentSite = m_CurrentPage.Site;
            }
        }

        void SetNodeData(DataTemplateAttribute mk_bind, HtmlNode node)
        {
            string value;
            string url = null;
            var field = FindField(mk_bind.ID);
            if (field == null)
            {
                object o = FindReserved(mk_bind.ID);
                if (o != null)
                    value = o.ToString();
                else
                    value = null;
            }
            else
            {
                if (field.Page != null)
                    url = field.Page.Url;
                if (field.Link != null)
                    url = field.Link.Url;
                if (field.Document != null)
                    url = field.Document.Url;
                if (field.Image != null)
                    url = field.Image.Url;

                value = field == null ? null : field.Value;
            }

            value = ApplyDataParser(mk_bind.Parser, value);

            if (mk_bind.HasProperty("content") && mk_bind["content"] == "fallback")
            {
                if (string.IsNullOrEmpty(value))
                {
                    var match = Mk_Bind_Text.Match(node.InnerHtml);
                    if (match.Success)
                    {
                        value = match.Groups["text"].Value;
                    }
                    else
                        value = node.InnerHtml;
                }
            }

            if (url != null)
            {
                if (node.Name == "a")
                {
                    if (node.Attributes.Contains("href"))
                        node.Attributes["href"].Value = url;
                    else
                        node.Attributes.Add("href", url);
                }
                if (node.Name == "img")
                {
                    if (node.Attributes.Contains("src"))
                        node.Attributes["src"].Value = url;
                    else
                        node.Attributes.Add("src", url);
                }
            }

            if (mk_bind.HasProperty("no-text"))
            {
                //  Do nothing
            }
            else
            {

                if (Mk_Bind_Text.IsMatch(node.InnerHtml))
                    node.InnerHtml = Mk_Bind_Text.Replace(node.InnerHtml, value);
                else
                    node.InnerHtml = (value == null ? string.Empty : value);
            }
        }

        void ParseHide(HtmlNode node, string hideValue)
        {
            //  Always hide
            if (hideValue == "*")
            {
                node.Remove();
                return;
            }
            var value = FindReserved(hideValue);
            //  Indien de waarde van dit veld binnen het component(primair) of pagina (secundair) gevuld is (of bij checkbox true) dan hide
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
                node.Remove();
            else if (value != null && typeof(bool) == value.GetType() && (bool)value)
                node.Remove();
        }

        void ParseShow(HtmlNode node, string showValue)
        {
            ParseShow(node, showValue, null);
        }

        void ParseShow(HtmlNode node, string showValue, object instance)
        {
            string property = showValue.Trim();
            string condition = null;
            bool conditionEquals = true;
            if (showValue.Contains("==") || showValue.Contains("!="))
            {
                conditionEquals = showValue.Contains("==");
                var split = showValue.Split(new string[] {"==", "!=" }, StringSplitOptions.RemoveEmptyEntries);
                property = split[0].Trim();
                condition = split[1].Trim();
            }

            var value = FindReserved(property, instance);
            //  Indien de waarde van dit veld binnen het component(primair) of pagina (secundair) leeg is (of bij checkbox false) dan hide
            if (value == null)
            {
                if (condition != null)
                {
                    if (conditionEquals && condition == "null")
                        return;
                }
                node.Remove();
            }
            else if (typeof(string) == value.GetType())
            {
                if (condition != null)
                {
                    string check = null;
                    if (condition.Contains("'"))
                        check = condition.Replace("'", string.Empty);
                    else
                    {
                        var o = FindReserved(condition);
                        if (o != null)
                            check = o.ToString();
                    }

                    if (conditionEquals && check == value.ToString())
                        return;
                    if (!conditionEquals && check != value.ToString())
                        return;
                }
                node.Remove();
            }
            else if (typeof(bool) == value.GetType() && !(bool)value)
            {
                if (condition != null)
                {
                    if (conditionEquals && condition == "false")
                        return;
                    if (!conditionEquals && condition == "true")
                        return;
                }
                node.Remove();
            }
        }

        /// <summary>
        /// Redoes the content of the serialize.
        /// - What is it's purpose?
        /// - What can be expeceted?
        /// - What pitfalls?
        /// </summary>
        void RedoSerializeContent()
        {
            if (_redoNodeList == null)
                return;

            for (int i = 0; i < _redoNodeList.Count; i++)
            {
                if (_redoNodeList.Count == i)
                    break;

                var node = _redoNodeList[i];
                if (node.Attributes.Contains("mk-bind"))
                {
                    var mk_bind = GetAttribute(node.Attributes["mk-bind"].Value);
                    if (mk_bind.HasProperty("hide"))
                        ParseHide(node, mk_bind["hide"]);

                    if (mk_bind.HasProperty("show"))
                        ParseShow(node, mk_bind["show"]);

                    if (mk_bind.HasProperty("type") && mk_bind.HasProperty("take"))
                    {
                        ParseNode_mk_bind(node, null);
                    }
                    else
                        if (!mk_bind.HasProperty("type"))
                            SetNodeData(mk_bind, node);


                }
                if (node.Attributes.Contains("mk-hide"))
                    ParseHide(node, node.Attributes["mk-hide"].Value);
                if (node.Attributes.Contains("mk-show"))
                    ParseShow(node, node.Attributes["mk-show"].Value);
            }

        }

        static Regex Mk_Clean = new Regex(@"(<mk.*?>|</mk>)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static Regex Mk_Bind = new Regex(@"{\[(?<text>[^:].*?)\]}", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static Regex Mk_Bind_Text = new Regex(@"{\[:(?<text>.*?)\]}", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        // CB; This is changed to an dictionary because lookups in list are slower from arround 3 items
        // http://www.dotnetperls.com/dictionary-time
        Dictionary<string, InstanceLibrary> _Instances;
        class InstanceLibrary
        {
            public Object Instance { get; set; }
            public string Reserved { get; set; }
        }
    }

    public class DataTemplateAttributeValue
    {
        public string Name { get; set; }
        public bool IsInnerAttribute { get; set; }
        public string Value { get; set; }
        public DataTemplateAttribute Template { get; set; }
    }

    public class DataComponentNode
    {
        public HtmlNode Node { get; set; }
        public DataTemplate Template { get; set; }
        public void Replace()
        {
            var newNode = HtmlNode.CreateNode(Template.Rendered);
            Node.ParentNode.ReplaceChild(newNode.ParentNode, Node);
        }
    }

    public class DataTemplateAttribute
    {
        public override string ToString()
        {
            string info = string.Empty;
            foreach (var value in this.Values)
            {
                if (!string.IsNullOrEmpty(value.Value))
                    info += string.Concat(info.Length == 0 ? "" : ",", value.Name, ":", value.Value);
            }
            if (info.Length > 0)
                info = string.Concat("[", info, "]");
            return string.Concat(this.ID, info);
        }

        public string this[object property]
        {
            get
            {
                if (this.Values != null)
                {
                    var i = property.ToString();
                    foreach (var item in this.Values)
                        if (item.Name.Equals(i, StringComparison.InvariantCultureIgnoreCase))
                            return item.IsInnerAttribute ? "<inner>" : (string.IsNullOrEmpty(item.Value) ? null : item.Value.Trim());
                }
                return null;
            }
            set
            {
                bool exist = false;
                var i = property.ToString();
                if (this.Values != null)
                {
                    foreach (var item in this.Values)
                    {
                        if (item.Name.Equals(i, StringComparison.InvariantCultureIgnoreCase))
                        {
                            item.Value = value;
                            exist = true;
                            break;
                        }
                    }
                }
                if (!exist)
                {
                    if (this.Values == null)
                        this.Values = new List<DataTemplateAttributeValue>();
                    this.Values.Add(new DataTemplateAttributeValue() { Name = i, Value = value });
                }
            }
        }
        public bool IsTrue(string name)
        {
            if (HasProperty(name))
            {
                var candidate = this[name].ToLower();
                if (candidate == "1" || candidate == "true")
                    return true;
            }
            return false;
        }
        public bool HasProperty(string name)
        {
            if (this[name] != null)
                return true;

            return false;
        }
        public DataTemplateAttributeValue GetAttribute(string property)
        {
            if (this.Values != null)
            {
                var i = property.ToString();
                foreach (var item in this.Values)
                    if (item.Name.Equals(i, StringComparison.InvariantCultureIgnoreCase))
                        return item;
            }
            return null;
        }
        public string Parser { get; set; }
        public string ID { get; set; }
        public List<DataTemplateAttributeValue> Values { get; set; }
    }
}
