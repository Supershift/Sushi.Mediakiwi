using System;
using System.Linq;
using System.Data;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sushi.Mediakiwi.Framework.Api;
using Microsoft.AspNetCore.Http;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.Data;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.Extensions.Configuration;

namespace Sushi.Mediakiwi.Beta.GeneratedCms
{
    /// <summary>
    /// Represents a Console entity.
    /// </summary>
    public class Console
    {
        /// <summary>
        /// Gets the safe post.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public string GetSafePost(string name)
        {
            return GetSafePost(name, false);
        }

        /// <summary>
        /// Gets the safe post.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="allowHTML">if set to <c>true</c> [allow HTML].</param>
        /// <returns></returns>
        public string GetSafePost(string name, bool allowHTML)
        {
            if (Context == null)
                return null;

            if (Context.Request == null)
                return null;

            if (!Context.Request.HasFormContentType)
                return null;

            if (Context.Request.Form.Count == 0)
                return null;

            if (Context.Request.Headers["Referer"].FirstOrDefault() == null)
                return null;

            var referer = new Uri(Context.Request.Headers["Referer"]);
            if (!referer.Host.Equals(Context.Request.Host.Host, StringComparison.CurrentCultureIgnoreCase))
                return null;

            if (allowHTML)
                return Context.Request.Form[name];
            return WebUtility.HtmlEncode(Context.Request.Form[name].FirstOrDefault());
        }

        /// <summary>
        /// Gets the safe get.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public string GetSafeGet(string name)
        {
            return GetSafeGet(name, false);
        }

        public string GetSafeUrl()
        {
            return UrlBuild.GetUrl();
        }

        /// <summary>
        /// Gets the safe get.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="allowHTML">if set to <c>true</c> [allow HTML].</param>
        /// <returns></returns>
        public string GetSafeGet(string name, bool allowHTML)
        {
            if (Context == null)
                return null;

            if (Context.Request == null)
                return null;

            if (Context.Request.Query.Count == 0)
                return null;

            if (allowHTML)
                return Context.Request.Query[name];
            return WebUtility.HtmlEncode(Context.Request.Query[name]);
        }


        internal string[] ConvertArray(string value)
        {
            if (IsJson)
            {
                if (JsonForm != null)
                {
                    return JsonConvert.DeserializeObject<string[]>(value);
                }
                return null;
            }
            else
                return value.Split(',');
        }

        internal bool IsPosted(string name)
        {


            if (IsJson)
            {
                if (JsonForm != null)
                    return JsonForm.ContainsKey(name);
                return false;
            }
            else
            {
                if (!Context.Request.HasFormContentType)
                    return false;

                return Context.Request.Form.ContainsKey(name);
            }
        }

        internal bool HasPost
        {
            get
            {
                if (!Context.Request.HasFormContentType)
                    return false;

                if (IsJson)
                {
                    if (JsonForm != null)
                    {
                        if (JsonForm.Count == 0) return false;
                        return true;
                    }
                    return false;
                }
                else
                {
                    if (!Context.Request.HasFormContentType)
                        return false;

                    if (Context.Request.Form.Count == 0) 
                        return false;

                    return true;
                }
            }
        }

        public string JsonReferrer
        {
            get
            {
                if (m_JsonRequest == null)
                    return string.Empty;

                return m_JsonRequest.Referrer;
            }
        }

        bool? m_IsJson;
        public bool IsJson
        {
            get
            {
                if (!m_IsJson.HasValue)
                    m_IsJson = (Request.ContentType != null && Request.ContentType.Contains("json"));

                return m_IsJson.Value;
            }
        }

        MediakiwiPostRequest m_JsonRequest;

        internal async Task LoadJsonStreamAsync()
        {
            if (IsJson)
            {
                var jsonString = String.Empty;

                Context.Request.EnableBuffering();
                using (var reader = new StreamReader(Context.Request.Body))
                {
                    try
                    {
                        reader.BaseStream.Seek(0, SeekOrigin.Begin);
                        jsonString = await reader.ReadToEndAsync();
                    }
                    catch(Exception ex)
                    {
                        throw ex;
                    }
                }

                if (!string.IsNullOrWhiteSpace(jsonString))
                {
                    m_JsonRequest = JsonConvert.DeserializeObject<MediakiwiPostRequest>(jsonString);
                    if (m_JsonRequest.FormFields != null)
                    {
                        string json = JsonConvert.SerializeObject(m_JsonRequest.FormFields);
                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            m_JsonForm = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                        }
                    }
                }
            }
        }

        Dictionary<string, object> m_JsonForm;
        public Dictionary<string, object> JsonForm
        {
            get
            {
                return m_JsonForm;
            }
        }


        internal void SetDateFormat()
        {

            if (CurrentEnvironment["FORM_DATEPICKER", true, "nl", "Set the datepicker format, options 'nl' (EU) or 'en' (US)"] == "nl")
            {
                GlobalisationCulture = "nl-nl";
                DateFormat = NL_DATE;
                DateTimeFormat = NL_DATETIME;

                DateFormatShort = NL_SHORT_DATE;
                DateTimeFormatShort = NL_SHORT_DATETIME;
            }
            else
            {
                GlobalisationCulture = "en-us";
                DateFormat = EN_DATE;
                DateTimeFormat = EN_DATETIME;

                DateFormatShort = EN_SHORT_DATE;
                DateTimeFormatShort = EN_SHORT_DATETIME;
            }
        }
        string NL_SHORT_DATE = "dd-MM-yy";
        string EN_SHORT_DATE = "dd-MM-yy";
        string US_SHORT_DATE = "MM/dd/yy";

        string NL_SHORT_DATETIME = "dd-MM-yy HH:mm";
        string EN_SHORT_DATETIME = "dd-MM-yy HH:mm";
        string US_SHORT_DATETIME = "MM/dd/yy HH:mm";

        string NL_DATE = "dd-MM-yyyy";
        string EN_DATE = "dd-MM-yyyy";
        string US_DATE = "MM/dd/yyyy";

        string NL_DATETIME = "dd-MM-yyyy HH:mm";
        string EN_DATETIME = "dd-MM-yyyy HH:mm";
        string US_DATETIME = "MM/dd/yyyy HH:mm";

        public string DateFormat { get; private set; }
        public string DateTimeFormat { get; private set; }
        public string DateFormatShort { get; private set; }
        public string DateTimeFormatShort { get; private set; }
        public string GlobalisationCulture { get; private set;  }
    

        /// <summary>
        /// Adds the trace.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="message">The message.</param>
        public void AddTrace(string category, string message)
        {
            System.Diagnostics.Trace.WriteLine($"{category}: {message}");
        }

        public string AddApplicationPath(string path, bool appendUrl = false)
        {
            if (path.StartsWith("http", StringComparison.CurrentCultureIgnoreCase))
                return path;

            if (path.StartsWith("~"))
            {
                path = path.Replace("~", Request.PathBase);
                if (appendUrl)
                    return String.Concat(CurrentDomain, path);
                return path;
            }

            if (appendUrl)
                return String.Concat(CurrentDomain, Request.PathBase, path);
            return String.Concat(Request.PathBase, path);
        }

        public string Url
        {
            get
            {
                return String.Concat(CurrentDomain, Request.PathBase, Request.Path, Request.QueryString);
            }
        }
        
        public string PortalUrl
        {
            get
            {
                return AddApplicationPath(CommonConfiguration.PORTAL_PATH, true);
            }
        }

        public string Query(string query)
        {
            return String.Concat(Request.Scheme, "://", Request.Host.ToString(), Request.PathBase, Request.Path, "?", query);
        }


        Sushi.Mediakiwi.Framework.OutputExpression m_ExpressionPrevious;
        /// <summary>
        /// Gets or sets the expression previous.
        /// </summary>
        /// <value>The expression previous.</value>
        public Sushi.Mediakiwi.Framework.OutputExpression ExpressionPrevious
        {
            get { return m_ExpressionPrevious; }
            set { m_ExpressionPrevious = value; }
        }

        internal int RowCount;
        internal bool HasDoubleCols;
        /// <summary>
        /// Gets a value indicating whether this instance is sortorder on.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is sortorder on; otherwise, <c>false</c>.
        /// </value>
        public bool IsSortorderOn
        {
            get {

                if (!Request.HasFormContentType)
                    return false;
                return Request.Form["sortOrder"] == "1" || PostbackValue == "sortOrder"; }
        }
        /// <summary>
        /// 
        /// </summary>
        HttpContext m_Application;
        /// <summary>
        /// 
        /// </summary>
        internal HttpContext Context;

        /// <summary>
        /// 
        /// </summary>
        public HttpRequest Request { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public HttpResponse Response { get; set; }

        Sushi.Mediakiwi.Beta.GeneratedCms.Source.UrlBuilder m_UrlBuild;
        /// <summary>
        /// Gets the URL build.
        /// </summary>
        /// <value>The URL build.</value>
        public Sushi.Mediakiwi.Beta.GeneratedCms.Source.UrlBuilder UrlBuild
        {
            get {
                if (m_UrlBuild == null)
                    m_UrlBuild = new Sushi.Mediakiwi.Beta.GeneratedCms.Source.UrlBuilder(this);
                return m_UrlBuild;
            }
        }

        /// <summary>
        /// Replicates the instance.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        internal Console ReplicateInstance(Data.IComponentList list)
        {
            Console candidate = new Console(m_Application, _env);

            candidate.CurrentList = list;

            candidate.CurrentListInstance = list.GetInstance(m_Application);
            //candidate.ApplyInstance(list.AssemblyName, list.ClassName);

            candidate.CurrentListInstance.wim.Console = this;
            //candidate.CurrentListInstance.wim = this.CurrentListInstance.wim;
            candidate.CurrentListInstance.wim.CurrentList = list;
            candidate.CurrentListInstance.wim.IsDashboardMode = this.CurrentListInstance.wim.IsDashboardMode;
            candidate.CurrentListInstance.wim.IsEditMode = this.CurrentListInstance.wim.IsEditMode;
            candidate.CurrentListInstance.wim.CurrentEnvironment = this.CurrentEnvironment;
            candidate.CurrentListInstance.wim.CurrentApplicationUser = this.CurrentApplicationUser;
            candidate.CurrentListInstance.wim.CurrentApplicationUserRole = this.CurrentListInstance.wim.CurrentApplicationUserRole;
            candidate.CurrentListInstance.wim.CurrentSite = this.CurrentListInstance.wim.CurrentSite;

            return candidate;
        }

        public string MapPath(string path)
        {
            var webRoot = _env.ContentRootPath;
            return String.Concat(webRoot, path);
        }

        private IHostingEnvironment _env;

        /// <summary>
        /// Initializes a new instance of the <see cref="Console"/> class.
        /// </summary>
        /// <param name="application">The application.</param>
        public Console(HttpContext application, IHostingEnvironment env)
        {
            _env = env;
            m_Application = application;
            Response = m_Application.Response;
            Request = m_Application.Request;
            Context = m_Application;


            this.WimRepository = string.Concat(this.CurrentDomain, AddApplicationPath("testdrive/files"));
            this.BaseRepository = string.Concat(this.CurrentDomain, AddApplicationPath("repository"));

            _VisitorManager = new VisitorManager(application);
        }

        internal string Form(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            if (Context != null)
            {
                if (IsJson)
                {
                    if (JsonForm != null)
                    {
                        if (JsonForm.ContainsKey(name))
                        {
                            if (JsonForm[name] != null)
                                return JsonForm[name].ToString();
                        }
                    }
                    return null;
                }
                else if (Context.Request.HasFormContentType)
                {
                    
                    return Context.Request.Form[name];
                }
            }
            return null;
        }

        string m_Channel;
        internal string Channel
        {
            get
            {
                if (m_Channel == null)
                {
                    var candidate = Site.SelectOne(ChannelIndentifier);
                    if (!Sushi.Mediakiwi.Data.Environment.Current.DefaultSiteID.GetValueOrDefault().Equals(candidate.ID))
                        m_Channel = Utils.ToUrl(candidate.Name);
                    else
                        m_Channel = string.Empty;

                }
                return m_Channel;
            }
        }

        int m_ChannelIndentifier;
        internal int ChannelIndentifier
        {
            get
            {
                bool isChanged = !string.IsNullOrEmpty(Form("channel"));
                if (isChanged)
                    isChanged = Form("autopostback") == "channel";

                //  Topnavigation postback (channel change)
                if (isChanged)
                {
                    var postedChannel = Data.Utility.ConvertToInt(Form("channel"));
                    if (postedChannel > 0)
                    {
                        ValidateChannelSwitch(m_ChannelIndentifier, postedChannel);
                        m_ChannelIndentifier = postedChannel;
                    }
                }
                return m_ChannelIndentifier;
            }
            set
            {
                m_ChannelIndentifier = value;
            }

        }

        internal bool IsPostBack(string postBackValue)
        {
            //  [MM:10.12.14] Reserved word save, also set as an ID extention $save
            if (postBackValue == "save")
                return IsPostBackSave;
            return PostbackValue.Equals(postBackValue, StringComparison.InvariantCultureIgnoreCase);
        }

        internal bool PostBackStartsWith(string prefix, out string fullPostback)
        {
            fullPostback = string.Empty;

            if (PostbackValue.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
            {
                fullPostback = PostbackValue;
                return true;
            }

            return false;
        }

        bool? _IsPostBackSave;
        internal bool IsPostBackSave
        {
            get
            {
                if (!_IsPostBackSave.HasValue)
                {
                    _IsPostBackSave = false;
                    if (Context != null) 
                    {
                        var value = IsJson ? JsonReferrer : Form("autopostback");
                        if (value != null) 
                        {
                            if (value.EndsWith("$save", StringComparison.InvariantCultureIgnoreCase))
                                _IsPostBackSave = true;

                            if (value.Equals("save", StringComparison.InvariantCultureIgnoreCase))
                                _IsPostBackSave = true;
                        }
                    }
                }
                return _IsPostBackSave.Value;
            }
        }

        public string RedirectionUrl { get; private set; }

        public void Redirect(string url, bool endresponse = false)
        {
            if (IsJson)
                RedirectionUrl = url;
            else
                Context.Response.Redirect(url, endresponse);
        }

        internal string PostbackValue
        {
            get
            {
                string value = "";
                if (Context == null) return value;

                value = IsJson ? JsonReferrer : Form("autopostback");
                if (value == null) value = "";
                else
                {
                    if (value.Contains("$"))
                        value = value.Split('$')[0];
                }
                return value;
            }
        }

        IComponentListTemplate m_CurrentListInstance;
        /// <summary>
        /// Gets or sets the current list object instance.
        /// </summary>
        /// <value>The current list instance.</value>
        public IComponentListTemplate CurrentListInstance
        {
            get { return m_CurrentListInstance; }
            set { m_CurrentListInstance = value;

            if (this.CurrentVisitor != null)
            {
                if (value == null || value.wim == null)
                    throw new Exception("You forgot to add Sushi.Mediakiwi.AppCentre.dll add as reference");
                value.wim.CurrentVisitor = this.CurrentVisitor;
            }
            }
        }

        /// <summary>
        /// Gets or sets the current list instance item.
        /// Use for code generation purposes.
        /// </summary>
        /// <value>The current list instance item.</value>
        public Object CurrentListInstanceItem { get; set; }

        Data.IApplicationUser m_CurrentApplicationUser;
        /// <summary>
        /// Gets or sets the current application user.
        /// </summary>
        /// <value>The current application user.</value>
        public Data.IApplicationUser CurrentApplicationUser
        {
            get {
                //  [20090411:MM] Patch
                if (m_CurrentApplicationUser == null 
                    && CurrentVisitor != null 
                    && CurrentVisitor.ApplicationUserID.HasValue
                    && CurrentVisitor.ApplicationUserID.Value > 0
                    )
                {
                    m_CurrentApplicationUser = Data.ApplicationUser.SelectOne(CurrentVisitor.ApplicationUserID.Value, true);
                    return m_CurrentApplicationUser;
                }

                if (m_CurrentApplicationUser == null && m_CurrentListInstance != null)
                    m_CurrentApplicationUser = m_CurrentListInstance.wim.CurrentApplicationUser;

                return m_CurrentApplicationUser; }
            set { m_CurrentApplicationUser = value; }
        }

        public void SaveVisit(bool shouldRememberVisitorForNextVisit = true)
        {
            _VisitorManager.Save(CurrentVisitor, shouldRememberVisitorForNextVisit);
        }

        VisitorManager _VisitorManager;
        IVisitor m_CurrentVisitor;
        /// <summary>
        /// Gets or sets the current visitor.
        /// </summary>
        /// <value>The current visitor.</value>
        public IVisitor CurrentVisitor
        {
            get
            {
                if (m_CurrentVisitor == null)
                {
                    m_CurrentVisitor = _VisitorManager.Select();
                }
                return m_CurrentVisitor;
            }
            set { m_CurrentVisitor = value; }
        }

        public Data.Folder CurrentFolder
        {
            get;set;
        }

        Data.IComponentList m_CurrentList;
        /// <summary>
        /// Gets or sets the current list.
        /// </summary>
        /// <value>The current list.</value>
        public Data.IComponentList CurrentList
        {
            get { return m_CurrentList; }
            set { m_CurrentList = value; }
        }

        Sushi.Mediakiwi.Data.Page m_CurrentPage;
        public Sushi.Mediakiwi.Data.Page CurrentPage
        {
            get
            {

                if (Context == null)
                    return null;

                if (Context.Items["Wim.Page"] == null)
                    return null;

                m_CurrentPage = Context.Items["Wim.Page"] as Sushi.Mediakiwi.Data.Page;

                return m_CurrentPage;
            }
        }

        /// <summary>
        /// Gets or sets the current environment.
        /// </summary>
        /// <value>The current environment.</value>
        internal Data.IEnvironment CurrentEnvironment { get; set; }
      
        internal bool IsCodeUpdate;

        /// <summary>
        /// Applies the instance.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="className">Name of the class.</param>
        internal void ApplyInstance(string assemblyName, string className)
        {
            CurrentListInstance = (IComponentListTemplate)Utils.CreateInstance(assemblyName, className);
            if (CurrentListInstance.wim.PassOverClassInstance != null)
                CurrentListInstance = CurrentListInstance.wim.PassOverClassInstance;
        }

        internal bool ApplyList(System.Type classname)
        {
            m_CurrentList = Data.ComponentList.SelectOne(classname.ToString());
            if (m_CurrentList == null || m_CurrentList.ID == 0)
            {
                //TMP REMOVE!
                var find = classname.ToString().Replace("Sushi.Mediakiwi", "Wim");
                m_CurrentList = Data.ComponentList.SelectOne(find);

            }
            if (m_CurrentList == null || m_CurrentList.ID == 0)
            {
                throw new Exception($"Mediakiwi - Could not initialize {classname}");
            }
            return ApplyList();
        }

        internal bool ApplyList(Data.ComponentListType type)
        {
            m_CurrentList = Data.ComponentList.SelectOne(type);
            return ApplyList();
        }

        /// <summary>
        /// Applies the list.
        /// </summary>
        /// <param name="typeId">The type id.</param>
        bool ApplyList()
        {
            if (m_CurrentList.IsNewInstance)
            {
                //throw new Exception("This list does not exists.");
                return false;
                //m_CurrentList = Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.VersionUpdater);
            }

            IComponentListTemplate tmp = m_CurrentList.GetInstance(this.m_Application);
            if (tmp == null)
                throw new Exception(string.Format("Could not find '{0}' or there could be a problem with initializing the class, this could be the case if there is coding present in the CTor!", m_CurrentList.ClassName));
            CurrentListInstance = tmp;
            CurrentListInstance.wim.Console = this;
            CurrentListInstance.wim.CurrentList = this.m_CurrentList;
            CurrentListInstance.wim.CurrentEnvironment = this.CurrentEnvironment;
            CurrentListInstance.wim.CurrentApplicationUser = this.CurrentApplicationUser;
            CurrentListInstance.wim.CurrentApplicationUserRole = Data.ApplicationRole.SelectOne(this.CurrentApplicationUser.RoleID);

            this.AddTrace("Monitor", "Start.ApplyComponentList.ApplyList.GetInstance");

            //Response.Write(Request.Path.ToString());
            
            //int channel = Data.Utility.ConvertToInt(this.Request.Form["channel"], this.CurrentApplicationUser.Channel);
            int channel = Data.Utility.ConvertToInt(this.ChannelIndentifier, CurrentEnvironment.DefaultSiteID.GetValueOrDefault());
            if (channel == 0)
            {
                this.AddTrace("Monitor", "Start.ApplyComponentList.ApplyList.GetAllSites");

                var list = Data.Site.SelectAll();
                if (list.Count > 0)
                    channel = list[0].ID;
            }

            CurrentListInstance.wim.CurrentSite = Data.Site.SelectOne(channel);

            if (CurrentListInstance.wim.CurrentSite == null)
                CurrentListInstance.wim.CurrentSite = Data.Site.SelectOne(Sushi.Mediakiwi.Data.Environment.Current.DefaultSiteID.Value);

            System.Threading.Thread.CurrentThread.CurrentCulture = CurrentListInstance.wim.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = CurrentListInstance.wim.CurrentCulture;

            if (Form("thumbview") == "1")
            {
                this.CurrentApplicationUser.ShowDetailView = false;
                this.CurrentApplicationUser.Save();
            }
            if (Form("detailview") == "1")
            {
                this.CurrentApplicationUser.ShowDetailView = true;
                this.CurrentApplicationUser.Save();
            }            

            //  Set the currentSite
            if (!this.CurrentListInstance.wim.CanAddNewItemIsSet
                && this.CurrentListInstance.wim.CurrentList != null 
                && this.CurrentListInstance.wim.CurrentList.Data != null 
                && !this.CurrentListInstance.wim.CurrentList.Data.HasProperty("wim_CanCreate")
                )
                this.CurrentListInstance.wim.CanAddNewItem = true;

            return true;
        }

        /// <summary>
        /// Switches the channel.
        /// </summary>
        //void SwitchChannel()
        //{
        //    string url = this.WimPagePath;
        //    //  Channel switch
        //    if (this.CurrentListInstance.wim.CurrentFolder.SiteID != this.CurrentApplicationUser.Channel)
        //    {
        //        Data.Folder folder = Data.Folder.SelectOneChild(this.CurrentListInstance.wim.CurrentFolder.MasterID.GetValueOrDefault(this.CurrentListInstance.wim.CurrentFolder.ID), this.CurrentApplicationUser.Channel);
        //        if (folder == null)
        //        {
        //            switch (this.CurrentListInstance.wim.CurrentFolder.Type)
        //            {
        //                case Sushi.Mediakiwi.Data.FolderType.Page: url = string.Concat(this.WimPagePath, "?top=1"); break;
        //                case Sushi.Mediakiwi.Data.FolderType.List: url = string.Concat(this.WimPagePath, "?top=2"); break;
        //                case Sushi.Mediakiwi.Data.FolderType.Gallery: url = string.Concat(this.WimPagePath, "?top=3"); break;
        //                case Sushi.Mediakiwi.Data.FolderType.Administration: url = string.Concat(this.WimPagePath, "?top=4"); break;
        //            }
                   
        //        }
        //        else
        //        {
        //            url = string.Concat(this.WimPagePath, "?folder=", folder.ID);
        //        }
        //    }
        //    this.Response.Redirect(url, true);
        //}

        void ValidateChannelSwitch(int currentChannelID, int requestedChannelID)
        {
            if (currentChannelID == requestedChannelID)
                return;

            var currentChannel = Sushi.Mediakiwi.Data.Site.SelectOne(currentChannelID);
            var requestedChannel = Sushi.Mediakiwi.Data.Site.SelectOne(requestedChannelID);

            int masterID = requestedChannel.MasterID.GetValueOrDefault();

            this.ValidateChannelSwitchPageInheritance(currentChannel, requestedChannel);

            //if (CurrentList != null && CurrentList.IsInherited && CurrentFolder != null)
            //{
            //    var requestedFolder = Data.Folder.SelectOneChild(CurrentFolder.ID, requestedChannel.ID);
            //    if (requestedFolder != null && !requestedFolder.IsNewInstance)
            //    {
            //        Response.Redirect(UrlBuild.GetListRequest(CurrentList, null, requestedChannel.ID));
            //    }
            //}

            //  [16 nov 14:MM] Validate inherited pages, folder
            Response.Redirect(string.Concat(this.GetWimPagePath(requestedChannel.ID)));
        }

        void ValidateChannelSwitchPageInheritance(Sushi.Mediakiwi.Data.Site currentChannel, Sushi.Mediakiwi.Data.Site requestedChannel)
        {
            var pageID = Data.Utility.ConvertToIntNullable(Request.Query["page"]);
            if (!pageID.HasValue)
                return;

            var page = Sushi.Mediakiwi.Data.Page.SelectOne(pageID.Value);

            if (requestedChannel.MasterID.HasValue && requestedChannel.MasterID.Value == currentChannel.ID)
            {
                //  The requested channel is the master of the current channel
                if (page != null && page.ID != 0)
                {
                    var pageCandidate = Page.SelectOneChild(page.ID, requestedChannel.ID, false);

                    if (pageCandidate != null && pageCandidate.ID != 0)
                        Response.Redirect(string.Concat(this.GetWimPagePath(requestedChannel.ID), "?page=", pageCandidate.ID));
                }
            }
            else if (currentChannel.MasterID.HasValue && currentChannel.MasterID.Value == requestedChannel.ID)
            {
                //  The current channel is the master of the requested channel
                if (page != null && page.ID != 0 && page.MasterID.HasValue)
                {
                    var pageCandidate = Sushi.Mediakiwi.Data.Page.SelectOne(page.MasterID.Value);
                    if (pageCandidate != null && pageCandidate.ID != 0)
                        Response.Redirect(string.Concat(this.GetWimPagePath(requestedChannel.ID), "?page=", pageCandidate.ID));
                }
            }
        }

        /// <summary>
        /// Applies the list.
        /// </summary>
        /// <param name="list">The list.</param>
        internal bool ApplyList(Data.IComponentList list)
        {
            m_CurrentList = list;
            this.Logic = m_CurrentList.ID;
            this.Title = m_CurrentList.Name;
            return ApplyList();
        }

        /// <summary>
        /// Applies the list.
        /// </summary>
        /// <param name="listInformation">The list information (can be GUID or ID).</param>
        internal bool ApplyList(string listInformation)
        {
            var list = default(Data.IComponentList);

            int candidate1;
            if (Data.Utility.IsNumeric(listInformation, out candidate1))
                list = Data.ComponentList.SelectOne(candidate1);
            else
            {
                Guid candidate2;
                if (Data.Utility.IsGuid(listInformation, out candidate2))
                    list = Data.ComponentList.SelectOne(candidate2);
                else
                    list = Data.ComponentList.SelectOne(listInformation);
            }

            if (list == null && list.IsNewInstance)
                throw new Exception($"Could not find the requested list with information [{listInformation}]");

            return ApplyList(list);
        }

        /// <summary>
        /// Applies the list.
        /// </summary>
        internal bool ApplyList(Data.ComponentList list)
        {
            m_CurrentList = list;

            this.Logic = m_CurrentList.ID;
            this.Title = m_CurrentList.Name;
            this.AddTrace("Monitor", "Start.ApplyComponentList.ApplyList.end");

            return ApplyList();
        }

        int m_Logic;
        /// <summary>
        /// Gets or sets the logic.
        /// </summary>
        /// <value>The logic.</value>
        public int Logic
        {
            get { return m_Logic; }
            set { m_Logic = value; }
        }

        int m_View;
        /// <summary>
        /// Gets or sets the view.
        /// </summary>
        /// <value>The view.</value>
        public int View
        {
            get { return m_View; }
            set { m_View = value; }
        }

        string m_ListPagingValue;
        /// <summary>
        /// Gets or sets the list paging value.
        /// </summary>
        /// <value>The list paging value.</value>
        public string ListPagingValue
        {
            get { return m_ListPagingValue; }
            set { m_ListPagingValue = value; }
        }

        int? m_OpenInFrame;
        /// <summary>
        /// Gets the open in frame.
        /// </summary>
        public int OpenInFrame
        {
            get {
                if (!m_OpenInFrame.HasValue)
                    m_OpenInFrame = Data.Utility.ConvertToInt(Request.Query["openinframe"]);
                return m_OpenInFrame.Value;
            }
        }

        int? m_Item;
        /// <summary>
        /// Gets or sets the item.
        /// </summary>
        /// <value>The item.</value>
        public int? Item
        {
            get { return m_Item; }
            set { m_Item = value; }
        }

        int? m_Group;
        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        /// <value>The group.</value>
        public int? Group
        {
            get { return m_Group; }
            set { m_Group = value; }
        }

        int? m_GroupItem;
        /// <summary>
        /// Gets or sets the group item.
        /// </summary>
        /// <value>The group item.</value>
        public int? GroupItem
        {
            get { return m_GroupItem; }
            set { m_GroupItem = value; }
        }

        public bool IsComponent
        {
            get { return this.CurrentListInstance.wim.ItemIsComponent; }
        }

        internal IConfiguration Configuration
        {
            get;set;
        }

        public T ConfigurationValue<T>(string value)
        {
            return Configuration.GetValue<T>(value);
        }
        public string ConfigurationValue(string value)
        {
            return Configuration.GetValue<string>(value);
        }

        public Source.Component Component { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is not framed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is not framed; otherwise, <c>false</c>.
        /// </value>
        public bool IsNotFramed
        {
            get { return Request.Query["nf"] == "1"; }
        }

        internal bool HasAsyncEvent
        {
            get;
            set;
        }

        public bool IsAdminFooter
        {
            get;
            set; 
        }

        /// <summary>
        /// Gets or sets the type of the item.
        /// </summary>
        /// <value>The type of the item.</value>
        public RequestItemType ItemType { get; set; }

        string m_Title;
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get { return m_Title; }
            set { m_Title = value; }
        }

        /// <summary>
        /// Gets or sets the current domain (host including schema).
        /// </summary>
        /// <value>The current host.</value>
        public string CurrentDomain
        {
            get
            {

                if (string.IsNullOrWhiteSpace(Request.Headers["X-Forwarded-Host"]))
                    return string.Concat(Request.Scheme, "://", Request.Host.ToString());

                return string.Concat(Request.Scheme, "://", Request.Headers["X-Forwarded-Host"]);
            }
        }

        /// <summary>
        /// Gets or sets the current host.
        /// </summary>
        /// <value>The current host.</value>
        public string CurrentHost
        {
            get {

                if (string.IsNullOrWhiteSpace(Request.Headers["X-Forwarded-Host"]))
                    return string.Concat(Request.Host.ToString());

                return string.Concat(Request.Headers["X-Forwarded-Host"]);
            }
        }


        /// <summary>
        /// Gets or sets the current host.
        /// </summary>
        /// <value>The current host.</value>
        //public string CurrentHost
        //{
        //    get
        //    {

        //        if (string.IsNullOrWhiteSpace(Request.Headers["X-Forwarded-Host"]))
        //            return string.Concat(Request.Host.ToString());

        //        return string.Concat(Request.Headers["X-Forwarded-Host"]);
        //    }
        //}

        /// <summary>
        /// Gets the wim group page path: wim.ashx?group=NN&amp;groupitem=NN
        /// </summary>
        /// <value>The wim group page path.</value>
        public string WimGroupPagePath
        {
            get {
                int group2ID = Data.Utility.ConvertToInt(this.Request.Query["group2"]);

                if (group2ID > 0)
                {
                    int group2ItemID = Data.Utility.ConvertToInt(this.Request.Query["group2item"]);
                    return string.Concat(this.WimPagePath, "?group=", this.Group.GetValueOrDefault(), "&groupitem=", this.GroupItem.GetValueOrDefault(), "&group2=", group2ID, "&group2item=", group2ItemID); 
                }
                return string.Concat(this.WimPagePath, "?group=", this.Group.GetValueOrDefault(), "&groupitem=", this.GroupItem.GetValueOrDefault()); 
                
            }
        }

        /// <summary>
        /// Gets or sets the wim page path.
        /// </summary>
        /// <value>The wim page path.</value>
        public string WimPagePath {
            get
            {
                // set the correct wim page
                return Channel.Any()
                    ? AddApplicationPath(string.Concat(CommonConfiguration.PORTAL_PATH, "/", Channel))
                    : AddApplicationPath(CommonConfiguration.PORTAL_PATH)
                    ;
            }
        }

        /// <summary>
        /// Gets the wim page path.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <returns></returns>
        public string GetWimPagePath(int? channel)
        {
            if (channel.HasValue)
            {
                var candidate = Site.SelectOne(channel.Value);
                if (candidate != null)
                {
                    return AddApplicationPath(string.Concat(CommonConfiguration.PORTAL_PATH, "/", Utils.ToUrl(candidate.Name)), true);
                }
            }
            return AddApplicationPath(string.Concat(CommonConfiguration.PORTAL_PATH), true);
        }

        string m_WimRepository;
        /// <summary>
        /// Gets or sets the wim repository.
        /// </summary>
        /// <value>The wim repository.</value>
        public string WimRepository
        {
            get { return m_WimRepository; }
            set { m_WimRepository = value; }
        }

        string m_BaseRepository;
        /// <summary>
        /// Gets or sets the base repository.
        /// </summary>
        /// <value>The base repository.</value>
        public string BaseRepository
        {
            get { return m_BaseRepository; }
            set { m_BaseRepository = value; }
        }
    }
}
