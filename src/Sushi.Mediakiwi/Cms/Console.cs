// CONSOLE
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.Framework.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
            {
                return null;
            }

            if (Context.Request == null)
            {
                return null;
            }

            if (!Context.Request.HasFormContentType)
            {
                return null;
            }

            if (Context.Request.Form.Count == 0)
            {
                return null;
            }

            if (Context.Request.Headers["Referer"].FirstOrDefault() == null)
            {
                return null;
            }

            if (allowHTML)
            {
                return Context.Request.Form[name];
            }

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


        /// <summary>
        /// Gets the safe get.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="allowHTML">if set to <c>true</c> [allow HTML].</param>
        /// <returns></returns>
        public string GetSafeGet(string name, bool allowHTML)
        {
            if (Context == null)
            {
                return null;
            }

            if (Context.Request == null)
            {
                return null;
            }

            if (Context.Request.Query.Count == 0)
            {
                return null;
            }

            if (allowHTML)
            {
                return Context.Request.Query[name];
            }
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
                return default(string[]);
            }
            else
            {
                return value.Split(',');
            }
        }

        internal bool IsPosted(string name)
        {
            if (IsJson)
            {
                if (JsonForm != null)
                {
                    return JsonForm.ContainsKey(name);
                }
                return false;
            }
            else
            {
                if (!Context.Request.HasFormContentType)
                {
                    return false;
                }

                return Context.Request.Form.ContainsKey(name);
            }
        }

        internal bool HasPost
        {
            get
            {
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
                {
                    return string.Empty;
                }

                return m_JsonRequest.Referrer;
            }
        }

        bool? m_IsJson;
        public bool IsJson
        {
            get
            {
                if (!m_IsJson.HasValue)
                {
                    m_IsJson = (Request.ContentType != null && Request.ContentType.Contains("json"));
                }

                return m_IsJson.Value;
            }
        }

        MediakiwiPostRequest m_JsonRequest;

        internal async Task LoadJsonStreamAsync()
        {
            if (IsJson)
            {
                var jsonString = string.Empty;

                Context.Request.EnableBuffering();
                using (var reader = new StreamReader(Context.Request.Body))
                {
                    try
                    {
                        reader.BaseStream.Seek(0, SeekOrigin.Begin);
                        jsonString = await reader.ReadToEndAsync();
                    }
                    catch (Exception ex)
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
                            JsonForm = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                        }
                    }
                }
            }
        }

        public Dictionary<string, object> JsonForm { get; set; }


        internal void SetDateFormat()
        {
            if (CommonConfiguration.FORM_DATEPICKER.Equals("nl", StringComparison.CurrentCultureIgnoreCase))
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

        readonly string NL_SHORT_DATE = "dd-MM-yy";
        readonly string EN_SHORT_DATE = "dd-MM-yy";

        readonly string NL_SHORT_DATETIME = "dd-MM-yy HH:mm";
        readonly string EN_SHORT_DATETIME = "dd-MM-yy HH:mm";

        readonly string NL_DATE = "dd-MM-yyyy";
        readonly string EN_DATE = "dd-MM-yyyy";

        readonly string NL_DATETIME = "dd-MM-yyyy HH:mm";
        readonly string EN_DATETIME = "dd-MM-yyyy HH:mm";

        public string DateFormat { get; private set; }
        public string DateTimeFormat { get; private set; }
        public string DateFormatShort { get; private set; }
        public string DateTimeFormatShort { get; private set; }
        public string GlobalisationCulture { get; private set; }


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
            if (!string.IsNullOrWhiteSpace(path))
            {
                if (path.StartsWith("http", StringComparison.CurrentCultureIgnoreCase))
                {
                    return path;
                }

                if (path.StartsWith('~'))
                {
                    path = path.Replace("~", Request.PathBase, StringComparison.CurrentCultureIgnoreCase);
                    if (appendUrl)
                    {
                        return string.Concat(CurrentDomain, path);
                    }
                    return path;
                }
                else if (!path.StartsWith('/'))
                {
                    // expect a relative path
                    path = $"/{path}";
                }
            }

            var prefix = Request.PathBase.HasValue ? Request.PathBase.Value : string.Empty;

            var url = string.Concat(prefix, path);

            if (url.Contains("//", StringComparison.CurrentCulture))
            {
                url = _CleanFormatting.Replace(url, "/");
            }

            if (appendUrl)
            {
                url = $"{CurrentDomain}{url}";
            }
            return url;
        }

        private static Regex _CleanFormatting = new Regex(@"\/.", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public string Url
        {
            get
            {
                return string.Concat(CurrentDomain, Request.PathBase, Request.Path, Request.QueryString);
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
            return string.Concat(Request.Scheme, "://", Request.Host.ToString(), Request.PathBase, Request.Path, "?", query);
        }

        /// <summary>
        /// Gets or sets the expression previous.
        /// </summary>
        /// <value>The expression previous.</value>
        public OutputExpression ExpressionPrevious { get; set; }

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
            get
            {
                if (!Request.HasFormContentType)
                {
                    return false;
                }
                return Request.Form["sortOrder"] == "1" || PostbackValue == "sortOrder";
            }
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

        Source.UrlBuilder m_UrlBuild;
        /// <summary>
        /// Gets the URL build.
        /// </summary>
        /// <value>The URL build.</value>
        public Source.UrlBuilder UrlBuild
        {
            get
            {
                if (m_UrlBuild == null)
                {
                    m_UrlBuild = new Source.UrlBuilder(this);
                }
                return m_UrlBuild;
            }
        }

        /// <summary>
        /// Replicates the instance.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        public Console ReplicateInstance(IComponentList list)
        {
            Console candidate = new Console(m_Application, _env);

            candidate.CurrentList = list;
            candidate.CurrentListInstance = list.GetInstance(m_Application);

            candidate.CurrentListInstance.wim.Console = this;
            candidate.CurrentListInstance.wim.CurrentList = list;
            candidate.CurrentListInstance.wim.IsEditMode = CurrentListInstance.wim.IsEditMode;
            candidate.CurrentListInstance.wim.CurrentEnvironment = CurrentEnvironment;
            candidate.CurrentListInstance.wim.CurrentApplicationUser = CurrentApplicationUser;
            candidate.CurrentListInstance.wim.CurrentApplicationUserRole = CurrentListInstance.wim.CurrentApplicationUserRole;
            candidate.CurrentListInstance.wim.CurrentSite = CurrentListInstance.wim.CurrentSite;

            return candidate;
        }

        public string MapPath(string path)
        {
            var webRoot = _env.ContentRootPath;
            return string.Concat(webRoot, path);
        }

        private IHostEnvironment _env;

        /// <summary>
        /// Initializes a new instance of the <see cref="Console"/> class.
        /// </summary>
        /// <param name="application">The application.</param>
        public Console(HttpContext application, IHostEnvironment env)
        {
            _env = env;
            m_Application = application;

            Response = m_Application.Response;
            Request = m_Application.Request;
            Context = m_Application;

            WimRepository = string.Concat(CurrentDomain, AddApplicationPath("testdrive/files"));
            BaseRepository = string.Concat(CurrentDomain, AddApplicationPath("repository"));

            _VisitorManager = new VisitorManager(application);
        }

        internal string Form(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            if (Context != null)
            {
                if (IsJson)
                {
                    if (JsonForm?.ContainsKey(name) == true && JsonForm[name] != null)
                    {
                        return JsonForm[name].ToString();
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
                    if (!Data.Environment.Current.DefaultSiteID.GetValueOrDefault().Equals(candidate.ID))
                    {
                        m_Channel = Utils.ToUrl(candidate.Name);
                    }
                    else
                    {
                        m_Channel = string.Empty;
                    }
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
                {
                    isChanged = Form("autopostback") == "channel";
                }

                //  Topnavigation postback (channel change)
                if (isChanged)
                {
                    var postedChannel = Utility.ConvertToInt(Form("channel"));
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
            {
                return IsPostBackSave;
            }
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
                            {
                                _IsPostBackSave = true;
                            }

                            if (value.Equals("save", StringComparison.InvariantCultureIgnoreCase))
                            {
                                _IsPostBackSave = true;
                            }
                        }
                    }
                }
                return _IsPostBackSave.Value;
            }
        }

        public Uri ClientRedirectionUrl { get; private set; }
        public bool ClientRedirectionUrlOnEmpty { get; private set; }

        public void SetClientRedirect(Uri url, bool emptyPage)
        {
            ClientRedirectionUrl = url;
            ClientRedirectionUrlOnEmpty = emptyPage;
        }

        public string RedirectionUrl { get; private set; }

        public void Redirect(string url, bool endresponse = false)
        {
            if (IsJson)
            {
                RedirectionUrl = url;
            }
            else
            {
                Context.Response.Redirect(url, endresponse);
            }
        }

        internal string PostbackValue
        {
            get
            {
                string value = "";
                if (Context == null) return value;

                value = IsJson ? JsonReferrer : Form("autopostback");
                if (value == null)
                {
                    value = "";
                }
                else
                {
                    if (value.Contains("$"))
                    {
                        value = value.Split('$')[0];
                    }
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
            set
            {
                m_CurrentListInstance = value;

                if (CurrentVisitor != null)
                {
                    if (value == null || value.wim == null)
                    {
                        throw new Exception("You forgot to add Sushi.Mediakiwi.AppCentre.dll add as reference");
                    }
                    value.wim.CurrentVisitor = CurrentVisitor;
                }
            }
        }

        /// <summary>
        /// Gets or sets the current list instance item.
        /// Use for code generation purposes.
        /// </summary>
        /// <value>The current list instance item.</value>
        public object CurrentListInstanceItem { get; set; }

        IApplicationUser m_CurrentApplicationUser;
        /// <summary>
        /// Gets or sets the current application user.
        /// </summary>
        /// <value>The current application user.</value>
        public IApplicationUser CurrentApplicationUser
        {
            get
            {
                //  [20090411:MM] Patch
                if (m_CurrentApplicationUser == null
                    && CurrentVisitor != null
                    && CurrentVisitor.ApplicationUserID.HasValue
                    && CurrentVisitor.ApplicationUserID.Value > 0
                    )
                {
                    m_CurrentApplicationUser = ApplicationUser.SelectOne(CurrentVisitor.ApplicationUserID.Value, true);
                    return m_CurrentApplicationUser;
                }

                if (m_CurrentApplicationUser == null && m_CurrentListInstance != null)
                {
                    m_CurrentApplicationUser = m_CurrentListInstance.wim.CurrentApplicationUser;
                }

                return m_CurrentApplicationUser;
            }
            set { m_CurrentApplicationUser = value; }
        }

        public void SaveVisit(bool shouldRememberVisitorForNextVisit = true)
        {
            _VisitorManager.Save(CurrentVisitor, shouldRememberVisitorForNextVisit);
        }

        public async Task SaveVisitAsync(bool shouldRememberVisitorForNextVisit = true)
        {
            await _VisitorManager.SaveAsync(CurrentVisitor, shouldRememberVisitorForNextVisit).ConfigureAwait(false);
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

        public Folder CurrentFolder { get; set; }

        /// <summary>
        /// Gets or sets the current list.
        /// </summary>
        /// <value>The current list.</value>
        public IComponentList CurrentList { get; set; }

        Page m_CurrentPage;
        public Page CurrentPage
        {
            get
            {
                if (Context == null)
                {
                    return null;
                }

                if (Context.Items["Wim.Page"] == null)
                {
                    return null;
                }

                m_CurrentPage = Context.Items["Wim.Page"] as Page;

                return m_CurrentPage;
            }
        }

        /// <summary>
        /// Gets or sets the current environment.
        /// </summary>
        /// <value>The current environment.</value>
        internal IEnvironment CurrentEnvironment { get; set; }

        internal bool IsCodeUpdate;

        /// <summary>
        /// Applies the instance.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="className">Name of the class.</param>
        internal void ApplyInstance(string assemblyName, string className)
        {
            CurrentListInstance = (IComponentListTemplate)Utils.CreateInstance(assemblyName, className, Context.RequestServices);
            if (CurrentListInstance.wim.PassOverClassInstance != null)
            {
                CurrentListInstance = CurrentListInstance.wim.PassOverClassInstance;
            }
        }

        internal async Task<bool> ApplyListAsync(Type classname)
        {
            CurrentList = await ComponentList.SelectOneAsync(classname.ToString()).ConfigureAwait(false);
            if (CurrentList == null || CurrentList.ID == 0)
            {
                //TMP REMOVE!
                var find = classname.ToString().Replace("Sushi.Mediakiwi", "Wim");
                CurrentList = await ComponentList.SelectOneAsync(find).ConfigureAwait(false);
            }
            if (CurrentList == null || CurrentList.ID == 0)
            {
                throw new Exception($"Mediakiwi - Could not initialize {classname}");
            }
            return await ApplyListAsync().ConfigureAwait(false);
        }

        internal async Task<bool> ApplyListAsync(ComponentListType type)
        {
            CurrentList = await ComponentList.SelectOneAsync(type).ConfigureAwait(false);
            return await ApplyListAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Applies the list.
        /// </summary>
        /// <param name="typeId">The type id.</param>
        async Task<bool> ApplyListAsync()
        {
            if (CurrentList.IsNewInstance)
            {
                return false;
            }

            IComponentListTemplate tmp = CurrentList.GetInstance(m_Application);
            if (tmp == null)
            {
                throw new Exception(string.Format("Could not find '{0}' or there could be a problem with initializing the class, this could be the case if there is coding present in the CTor!", CurrentList.ClassName));
            }

            if (CurrentEnvironment == null)
            {
                CurrentEnvironment = Data.Environment.Current;
            }

            CurrentListInstance = tmp;
            CurrentListInstance.wim.Console = this;
            CurrentListInstance.wim.CurrentList = CurrentList;
            CurrentListInstance.wim.CurrentEnvironment = CurrentEnvironment;
            CurrentListInstance.wim.CurrentApplicationUser = CurrentApplicationUser;
            CurrentListInstance.wim.CurrentApplicationUserRole = await ApplicationRole.SelectOneAsync(CurrentApplicationUser.RoleID).ConfigureAwait(false);

            AddTrace("Monitor", "Start.ApplyComponentList.ApplyList.GetInstance");

            int channel = Utility.ConvertToInt(ChannelIndentifier, CurrentEnvironment.DefaultSiteID.GetValueOrDefault());
            if (channel == 0)
            {
                AddTrace("Monitor", "Start.ApplyComponentList.ApplyList.GetAllSites");

                var list = await Site.SelectAllAsync().ConfigureAwait(false);
                if (list.Count > 0)
                {
                    channel = list[0].ID;
                }
            }

            CurrentListInstance.wim.CurrentSite = await Site.SelectOneAsync(channel).ConfigureAwait(false);

            if (CurrentListInstance.wim.CurrentSite == null)
            {
                CurrentListInstance.wim.CurrentSite = await Site.SelectOneAsync(Data.Environment.Current.DefaultSiteID.Value).ConfigureAwait(false);
            }

            System.Threading.Thread.CurrentThread.CurrentCulture = CurrentListInstance.wim.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = CurrentListInstance.wim.CurrentCulture;

            if (Form("thumbview") == "1")
            {
                CurrentApplicationUser.ShowDetailView = false;
                await CurrentApplicationUser.SaveAsync();
            }

            if (Form("detailview") == "1")
            {
                CurrentApplicationUser.ShowDetailView = true;
                await CurrentApplicationUser.SaveAsync();
            }

            //  Set the currentSite
            if (!CurrentListInstance.wim.CanAddNewItemIsSet
                && CurrentListInstance.wim.CurrentList != null
                && CurrentListInstance.wim.CurrentList.Data != null
                && !CurrentListInstance.wim.CurrentList.Data.HasProperty("wim_CanCreate")
                )
            {
                CurrentListInstance.wim.CanAddNewItem = true;
            }

            return true;
        }

        void ValidateChannelSwitch(int currentChannelID, int requestedChannelID)
        {
            if (currentChannelID == requestedChannelID)
            {
                return;
            }

            var currentChannel = Site.SelectOne(currentChannelID);
            var requestedChannel = Site.SelectOne(requestedChannelID);

            ValidateChannelSwitchPageInheritance(currentChannel, requestedChannel);

            //if (CurrentList != null && CurrentList.IsInherited && CurrentFolder != null)
            //{
            //    var requestedFolder =Folder.SelectOneChild(CurrentFolder.ID, requestedChannel.ID);
            //    if (requestedFolder != null && !requestedFolder.IsNewInstance)
            //    {
            //        Response.Redirect(UrlBuild.GetListRequest(CurrentList, null, requestedChannel.ID));
            //    }
            //}

            //  [16 nov 14:MM] Validate inherited pages, folder
            Response.Redirect(string.Concat(GetWimPagePath(requestedChannel.ID)));
        }

        void ValidateChannelSwitchPageInheritance(Site currentChannel, Site requestedChannel)
        {
            var pageID = Utility.ConvertToIntNullable(Request.Query["page"]);
            if (!pageID.HasValue)
            {
                return;
            }

            var page = Page.SelectOne(pageID.Value);

            if (requestedChannel.MasterID.HasValue && requestedChannel.MasterID.Value == currentChannel.ID)
            {
                //  The requested channel is the master of the current channel
                if (page != null && page.ID != 0)
                {
                    var pageCandidate = Page.SelectOneChild(page.ID, requestedChannel.ID, false);

                    if (pageCandidate != null && pageCandidate.ID != 0)
                    {
                        Response.Redirect(string.Concat(GetWimPagePath(requestedChannel.ID), "?page=", pageCandidate.ID));
                    }
                }
            }
            else if (currentChannel.MasterID.HasValue && currentChannel.MasterID.Value == requestedChannel.ID)
            {
                //  The current channel is the master of the requested channel
                if (page != null && page.ID != 0 && page.MasterID.HasValue)
                {
                    var pageCandidate = Page.SelectOne(page.MasterID.Value);
                    if (pageCandidate != null && pageCandidate.ID != 0)
                    {
                        Response.Redirect(string.Concat(GetWimPagePath(requestedChannel.ID), "?page=", pageCandidate.ID));
                    }
                }
            }
        }

        /// <summary>
        /// Applies the list.
        /// </summary>
        /// <param name="list">The list.</param>
        internal async Task<bool> ApplyListAsync(IComponentList list)
        {
            CurrentList = list;
            Logic = CurrentList.ID;
            Title = CurrentList.Name;
            return await ApplyListAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Applies the list.
        /// </summary>
        /// <param name="listInformation">The list information (can be GUID or ID).</param>
        internal async Task<bool> ApplyListAsync(string listInformation)
        {
            IComponentList list;

            if (Utility.IsNumeric(listInformation, out int candidate1))
            {
                list = await ComponentList.SelectOneAsync(candidate1).ConfigureAwait(false);
            }
            else
            {
                if (Utility.IsGuid(listInformation, out Guid candidate2))
                {
                    list = await ComponentList.SelectOneAsync(candidate2).ConfigureAwait(false);
                }
                else
                {
                    list = await ComponentList.SelectOneAsync(listInformation).ConfigureAwait(false);
                }
            }

            if (list == null && list.IsNewInstance)
            {
                throw new Exception($"Could not find the requested list with information [{listInformation}]");
            }

            return await ApplyListAsync(list).ConfigureAwait(false);
        }


        /// <summary>
        /// Applies the list.
        /// </summary>
        internal async Task<bool> ApplyListAsync(ComponentList list)
        {
            CurrentList = list;

            Logic = CurrentList.ID;
            Title = CurrentList.Name;
            AddTrace("Monitor", "Start.ApplyComponentList.ApplyList.end");

            return await ApplyListAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Gets or sets the logic.
        /// </summary>
        /// <value>The logic.</value>
        public int Logic { get; set; }

        /// <summary>
        /// Gets or sets the view.
        /// </summary>
        /// <value>The view.</value>
        public int View { get; set; }

        /// <summary>
        /// Gets or sets the list paging value.
        /// </summary>
        /// <value>The list paging value.</value>
        public string ListPagingValue { get; set; }

        int? m_OpenInFrame;
        /// <summary>
        /// Gets the open in frame.
        /// </summary>
        public int OpenInFrame
        {
            get
            {
                if (!m_OpenInFrame.HasValue)
                {
                    m_OpenInFrame = Utility.ConvertToInt(Request.Query["openinframe"]);
                }
                return m_OpenInFrame.Value;
            }
        }

        /// <summary>
        /// Gets or sets the item.
        /// </summary>
        /// <value>The item.</value>
        public int? Item { get; set; }

        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        /// <value>The group.</value>
        public int? Group { get; set; }

        /// <summary>
        /// Gets or sets the group item.
        /// </summary>
        /// <value>The group item.</value>
        public int? GroupItem { get; set; }

        public bool IsComponent
        {
            get { return CurrentListInstance.wim.ItemIsComponent; }
        }

        internal IConfiguration Configuration
        {
            get; set;
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

        internal bool HasAsyncEvent { get; set; }

        public bool IsAdminFooter { get; set; }

        /// <summary>
        /// Gets or sets the type of the item.
        /// </summary>
        /// <value>The type of the item.</value>
        public RequestItemType ItemType { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the current domain (host including schema).
        /// </summary>
        /// <value>The current host.</value>
        public string CurrentDomain
        {
            get
            {

                if (string.IsNullOrWhiteSpace(Request.Headers["X-Forwarded-Host"]))
                {
                    return string.Concat(Request.Scheme, "://", Request.Host.ToString());
                }
                return string.Concat(Request.Scheme, "://", Request.Headers["X-Forwarded-Host"]);
            }
        }

        /// <summary>
        /// Gets or sets the current host.
        /// </summary>
        /// <value>The current host.</value>
        public string CurrentHost
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Request.Headers["X-Forwarded-Host"]))
                {
                    return string.Concat(Request.Host.ToString());
                }

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
            get
            {
                int group2ID = Utility.ConvertToInt(Request.Query["group2"]);

                if (group2ID > 0)
                {
                    int group2ItemID = Utility.ConvertToInt(Request.Query["group2item"]);
                    return string.Concat(WimPagePath, "?group=", Group.GetValueOrDefault(), "&groupitem=", GroupItem.GetValueOrDefault(), "&group2=", group2ID, "&group2item=", group2ItemID);
                }
                return string.Concat(WimPagePath, "?group=", Group.GetValueOrDefault(), "&groupitem=", GroupItem.GetValueOrDefault());
            }
        }

        /// <summary>
        /// Gets or sets the wim page path.
        /// </summary>
        /// <value>The wim page path.</value>
        public string WimPagePath
        {
            get
            {
                // set the correct wim page
                return (Channel != null && Channel.Any())
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
        public string GetWimPagePath(int? channel, bool addApplicationPath = true)
        {
            if (channel.GetValueOrDefault(0) > 0)
            {
                var candidate = Site.SelectOne(channel.Value);
                if (candidate != null)
                {
                    if (addApplicationPath)
                    {
                        return AddApplicationPath(string.Concat(CommonConfiguration.PORTAL_PATH, "/", Utils.ToUrl(candidate.Name)), true);
                    }
                    else
                    {
                        return string.Concat(CommonConfiguration.PORTAL_PATH, "/", Utils.ToUrl(candidate.Name));
                    }
                }
            }

            if (addApplicationPath)
            {
                return AddApplicationPath(string.Concat(CommonConfiguration.PORTAL_PATH), true);
            }
            else
            {
                return string.Concat(CommonConfiguration.PORTAL_PATH);
            }
        }


        /// <summary>
        /// Gets or sets the wim repository.
        /// </summary>
        /// <value>The wim repository.</value>
        public string WimRepository { get; set; }

        /// <summary>
        /// Gets or sets the base repository.
        /// </summary>
        /// <value>The base repository.</value>
        public string BaseRepository { get; set; }
    }
}
