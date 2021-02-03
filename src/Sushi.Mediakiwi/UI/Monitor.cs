using System;
using System.Text;
using System.Data;
using Sushi.Mediakiwi.Beta.GeneratedCms.Source;
using System.Linq;
using System.Collections.Generic;
using Sushi.Mediakiwi.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Sushi.Mediakiwi.Framework.UI
{
    public class Monitor
    {
        internal iPresentationMonitor m_PresentationMonitor;
        internal iPresentationNavigation m_PresentationNavigation;

        internal Sushi.Mediakiwi.Beta.GeneratedCms.Console m_Console;
        internal HttpContext m_Application;

        /// <summary>
        /// Initializes a new instance of the <see cref="Monitor"/> class.
        /// </summary>
        /// <param name="application">The application.</param>
        public Monitor(HttpContext application)
            : this(application, false) { }

        public Monitor(HttpContext application, bool isLoadedInThinLayer)
            : this(application, null, isLoadedInThinLayer) { }

        public Monitor(HttpContext application, Sushi.Mediakiwi.Data.IApplicationUser user, bool isLoadedInThinLayer)
            : this(application, user, null, null, isLoadedInThinLayer) { }

        bool IsLoadedInThinLayer;
        internal bool IsNewDesignOutput;

        Dictionary<GlobalPlaceholder, string> m_Placeholders;
        Dictionary<CallbackTarget, List<ICallback>> m_Callbacks;

        public Monitor(HttpContext application, Sushi.Mediakiwi.Data.IApplicationUser user, Dictionary<GlobalPlaceholder, string> placeholders, Dictionary<CallbackTarget, List<ICallback>> callbacks, bool isLoadedInThinLayer)
        {
            IsNewDesignOutput = true;
            IsLoadedInThinLayer = isLoadedInThinLayer;
            m_Application = application;
            m_Placeholders = placeholders;
            m_Callbacks = callbacks;

            application.Items["newstyle"] = "1";
            application.Items["no-cache"] = "1";

            m_Console = new Sushi.Mediakiwi.Beta.GeneratedCms.Console(application, true);
            m_Console.AddTrace("Monitor", "CTor");
            m_Console.CurrentApplicationUser = user;

            m_PresentationMonitor = Sushi.Mediakiwi.Data.Environment.GetInstance<iPresentationMonitor>();
            m_PresentationNavigation = Sushi.Mediakiwi.Data.Environment.GetInstance<iPresentationNavigation>();
        }

        /// <summary>
        /// Gets the channel indentifier.
        /// </summary>
        /// <returns></returns>
        public int GetChannelIndentifier()
        {
            return m_Console.ChannelIndentifier;
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            Start(false);
        }

        /// <summary>
        /// Proceses the request.
        /// </summary>
        /// <param name="reStartWithNotificationList">if set to <c>true</c> [re start with notification list].</param>
        void ProcesRequest(bool reStartWithNotificationList)
        {
            m_Console.IsNewDesign = this.IsNewDesignOutput;

            m_Console.AddTrace("Monitor", "Start");

            //  Set the current environment
            m_Console.CurrentEnvironment = Data.Environment.Current;
            m_Console.SetDateFormat();

            bool forcelogin = Data.Utility.RemApplicationPath(m_Console.Request.Path).Equals($"{Sushi.Mediakiwi.Data.Environment.Current.RelativePath}/login") 
                || m_Console.Request.Url.ToString().Contains($"{Sushi.Mediakiwi.Data.Environment.Current.RelativePath}?reset=")
                || m_Console.Request.Url.ToString().EndsWith($"{Sushi.Mediakiwi.Data.Environment.Current.RelativePath}?reminder")
                ;

            m_Console.AddTrace("Monitor", "Start.CheckRoamingApplicationUser");
            //  When there is no roaming application user, redirect to the login.
            if (!this.CheckRoamingApplicationUser(forcelogin))
            {
                return;
            }

            if (m_Console.Request.QueryString.ToString().EndsWith("reset") && !m_Console.CurrentVisitor.Data["Wim.Reset.Me"].IsNull)
            {
                int userID = m_Console.CurrentApplicationUser.ID;
                var user = Data.ApplicationUserLogic.Apply(m_Console.CurrentVisitor.Data["Wim.Reset.Me"].ParseGuid().Value, false);
                m_Console.CurrentVisitor.Data.Apply("Wim.Reset.Me", null);
                m_Console.CurrentVisitor.Save();

                PortalAuthentication auth = new PortalAuthentication();
                m_Console.Response.Cookies.Add(auth.SetAuthentication(user, m_Console.Request.IsSecureConnection));

                Sushi.Mediakiwi.Data.IComponentList userList = Sushi.Mediakiwi.Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.Users);
                m_Console.Response.Redirect(m_Console.UrlBuild.GetListRequest(userList, userID));
            }
          
            m_Console.AddTrace("Monitor", "Start.SetRequestType()");
            //  Obtain querystring requests
            this.SetRequestType();
            //  Apply the current component list based on the roaming environment settings

            m_Console.AddTrace("Monitor", "Start.ApplyComponentList()");

            if (reStartWithNotificationList)
                m_Console.ApplyList(Sushi.Mediakiwi.Data.ComponentListType.InformationMessage);
            else
                ApplyComponentList();

            m_Console.AddTrace("Monitor", "Start.OutputAjaxRequest");
            //  If an xml (ajax) request comes in output a correct response.
            if (!this.OutputAjaxRequest()) return;

            //  Define internal event-check value types
            bool isDeleteTriggered;

            m_Console.AddTrace("Monitor", "Start.SetFormModes(..)");
            //  Sets the form status (Edit/Save,Delete, etc)
            SetFormModes(out isDeleteTriggered);


            m_Console.AddTrace("Monitor", "Start.CheckFolder()");
            //  Checks and sets the current folder.
            if (!CheckFolder()) return;
            CheckSite();

            m_Console.AddTrace("Monitor", "Start.CheckSecurity()");
            //  Check the role base security
            CheckSecurity(reStartWithNotificationList);

            //  Is the request opened in a frame? 0 = no, 1 = yes, list mode, 2 = yes, form mode
            int openInFrame = Data.Utility.ConvertToInt(m_Console.Request.QueryString["openinframe"]);

            m_Console.IsAdminFooter = Data.Utility.ConvertToInt(m_Console.Request.QueryString["adminFooter"]) == 1;

            //  Create new instances
            DataGrid grid = new DataGrid();
            Component component = new Component() {  m_IsNewDesign = true };
            m_Console.Component = component;

            m_Console.AddTrace("Monitor", "Start.HandleRequest(..)");

            HandleRequest(grid, component, isDeleteTriggered);

             m_Console.CurrentListInstance.wim.CurrentVisitor.SetCookie();
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        void Start(bool reStartWithNotificationList)
        {
            if (m_Console.Request.IsLocal || Wim.CommonConfiguration.IS_LOCAL_DEVELOPMENT)
                ProcesRequest(reStartWithNotificationList);
            else
            {
                try
                {
                    ProcesRequest(reStartWithNotificationList);
                }
                catch (Exception ex)
                {
                    if (ex.GetType() == typeof(System.Threading.ThreadAbortException))
                        return;

                    if (!reStartWithNotificationList)
                    {
                        m_Console.CurrentVisitor.Data.Apply("last.error", Sushi.Mediakiwi.Data.Notification.InsertOne(Sushi.Mediakiwi.Data.Notification.Tags.InternalWimError, Sushi.Mediakiwi.Data.NotificationType.Error, m_Console.CurrentApplicationUser, ex));
                        Start(true);
                        return;
                    }
  
                    if (this.IsLoadedInThinLayer) return;
                }
            }
            
            if (this.IsLoadedInThinLayer) return;


            // MV 2018-08-16: end of the pipeline, always save the visitor (if not, strange things happen like filter selection is forgotten)
            m_Console.CurrentVisitor.Save();
            m_Console.Response.Flush();
            // [MR:26-01-2017] this line was suddenly commented, resulting in a very nasty errormessage
            // on the page output
            // MV: this should be replaced by something else, because it now stops the complete .NET processing of the request
            HttpContext.Current.ApplicationInstance.CompleteRequest();
            //m_Console.Response.End();
        }

        void GenerateExternalSource()
        {
            PageTemplateConfiguration ptc = PageTemplateConfiguration.Load();
            System.Collections.Hashtable ht = ComponentConfiguration.Load();

            foreach (string key in ht.Keys)
            {
                string xhtmlKey = string.Format("<wim:{0} />", key);
                if (ptc.Data.Contains(xhtmlKey))
                {
                    ComponentConfiguration cc = ht[key] as Sushi.Mediakiwi.Beta.GeneratedCms.Source.External.ComponentConfiguration;

                    if (!string.IsNullOrEmpty(cc.Processing))
                    {
                        string[] x = cc.Processing.Split(',');
                        Wim.Processing.iProcessing iproces = Data.Utility.CreateInstance(string.Concat(x[1].Trim(), ".dll"), x[0]) as Wim.Processing.iProcessing;

                        if (iproces != null)
                        {
                            iproces.wim = m_Console.CurrentListInstance.wim;
                            iproces.Init(cc);
                        }
                    }

                    ptc.Data = ptc.Data.Replace(xhtmlKey, cc.Data);
                }
            }

            m_Console.Response.Write(ptc.Data);

            m_Console.Response.Flush();
            m_Console.Context.Trace.Write("Parsing", "Done");
            m_Console.Response.End();
        }

        /// <summary>
        /// Adds to response.
        /// </summary>
        /// <param name="output">The output.</param>
        void AddToResponse(string output)
        {
            if (this.IsLoadedInThinLayer && outputHTML == null)
                outputHTML = new StringBuilder();

            if (this.IsLoadedInThinLayer)
                outputHTML.Append(output);
            else
            {
                if (m_Console.Request.QueryString["split"] == "homeArticle")
                {
                    var split = output.Split(new string[] { @"<article id=""homeArticle"">", "</article>" }, StringSplitOptions.RemoveEmptyEntries); 
                    output = split[1];
                }
                m_Console.Response.Write(output);
            }

        }

        public StringBuilder outputHTML;

        /// <summary>
        /// When there is no roaming application user, redirect to the homepage.
        /// </summary>
        /// <returns>Continue processing page?</returns>
        internal bool CheckRoamingApplicationUser(bool showLogin = false)
        {
            if (!showLogin)
            {
                //  Check if logout request is performed
                if (m_Console.IsPostBack("logout"))
                {
                    
                    bool force = true;

                    FormsAuthentication.SignOut();

                    m_Console.CurrentVisitor.ApplicationUserID = null;
                    m_Console.CurrentVisitor.Save();

                    string wimPath = Data.Environment.Current.RelativePath;
                   
                    m_Console.Response.Redirect(Data.Utility.AddApplicationPath(wimPath), true);

                }
                else
                {
                    if (!string.IsNullOrEmpty(m_Console.Request.QueryString["negotiate"]))
                    {
                        Guid applicationUser = Data.Utility.ConvertToGuid(m_Console.Request.QueryString["negotiate"]);
                        m_Console.CurrentApplicationUser = Data.ApplicationUserLogic.Apply(applicationUser, true);

                    }
                    else
                    {
                        Guid visitor;
                        if (Data.Utility.IsGuid(m_Console.Request.Headers["WIM-Transport-Visitor"], out visitor))
                        {
                            m_Console.CurrentVisitor = Sushi.Mediakiwi.Data.Identity.Visitor.Select(visitor);
                        }

                        if (string.IsNullOrEmpty(m_Console.Request.Headers["WIM-Transport-Cookie"]))
                        {
                            if (m_Console.CurrentApplicationUser == null)
                            {
                                if (m_Console.Request.IsAuthenticated)
                                    m_Console.CurrentApplicationUser = Data.ApplicationUserLogic.Select();
                            }
                        }
                        else
                        {
                            Encoding utf = Encoding.GetEncoding("utf-8");

                            string cookieData = m_Console.Request.Headers["WIM-Transport-Cookie"];

                            string[] split = cookieData.Split('&');
                            foreach (string cookie in split)
                            {
                                string[] nameValue = cookie.Split('|');
                                if (nameValue.Length > 1)
                                    m_Console.Response.Cookies.Add(new HttpCookie(nameValue[0], nameValue[1]));

                            }

                            if (m_Console.Request.IsAuthenticated)
                                m_Console.CurrentApplicationUser = Data.ApplicationUserLogic.Select();
                            else
                                m_Console.CurrentApplicationUser = null;
                        }
                    }
                }
            }
            //  Check roaming profile

            if (!showLogin && m_Console.Request.IsAuthenticated)
            {
                //  LOGGED IN!
            }
            else
            {
                if (!m_Console.IsPostBack("logout") && !m_Application.Request.QueryString.ToString().Equals("logout"))
                {
                    bool iscms = m_Application.Context.Request.Path.ToLower().StartsWith(Sushi.Mediakiwi.Data.Environment.Current.GetPath().ToLower());

                    if (!iscms)
                        m_Console.Response.Redirect($"{Sushi.Mediakiwi.Data.Environment.Current.GetPath()}?logout");
                }

                //  Verify if the call is JSON, XML, CSV, ..
                VerifyFormatRequest();

                string reaction = m_PresentationMonitor.GetLoginWrapper(m_Console, m_Placeholders, m_Callbacks);
                if (string.IsNullOrEmpty(reaction))
                {
                    m_Console.Response.Redirect(Data.Utility.GetSafeUrl(m_Console.Request));
                }
                else
                {
                    m_Console.Response.AppendHeader("Access-Control-Allow-Origin", "*");
                    m_Console.Response.AppendHeader("Access-Control-Allow-Methods", "GET, OPTIONS");
                    AddToResponse(reaction);
                }

                if (this.IsLoadedInThinLayer)
                {
                    return false;
                }
                else
                {
                    m_Console.Response.Flush();
                    m_Console.Context.Trace.Write("Parsing", "Done");
                }
                return false;
            }
            return true;
        }

        bool IsFormatRequest_AJAX { get { return !string.IsNullOrEmpty(m_Console.Request.Params[Wim.UI.Constants.AJAX_PARAM]); } }
        bool IsFormatRequest_JSON { get { 
                return m_Console.Request.Params[Wim.UI.Constants.JSON_PARAM] == "1" || m_Console.Request.ContentType.Contains("json"); 
            } 
        }

        bool IsFormatRequest { get { return this.IsFormatRequest_JSON; } }

        void VerifyFormatRequest()
        {
            if (IsFormatRequest_JSON)
            {
                m_Console.Response.Clear();
                m_Console.Response.ContentType = "application/json";
                m_Console.Response.Write(Wim.UI.Constants.JSON_NO_ACCESS);
                m_Console.Response.End();
            }
        }

        /// <summary>
        /// Outputs the ajax request.
        /// </summary>
        /// <returns>Continue processing page?</returns>
        bool OutputAjaxRequest()
        {
            if (!string.IsNullOrEmpty(m_Console.Request.QueryString["xml"]))
            {
                
                m_Console.IsNewDesign = true;
                m_Console.Response.ContentType = "text/xml";
                m_Console.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                m_Console.Response.Cache.SetAllowResponseInBrowserHistory(false);

                var type = Data.Utility.ConvertToIntNullable(m_Console.Request.QueryString["xml"]);
                if (type.HasValue)
                {
                    WimControlBuilder build = new WimControlBuilder();
                    MetaData meta = new MetaData();
                    meta.ContentTypeSelection = type.Value.ToString();
                    var element = meta.GetContentInfo();
                    string name = string.Concat(m_Console.Request.QueryString["id"], "__", type.Value, "__", m_Console.Request.QueryString["index"]);


                    ((Framework.ContentSharedAttribute)element).ID = name;
                    ((Framework.ContentSharedAttribute)element).OverrideTableGeneration = true;
                    ((Framework.ContentSharedAttribute)element).Expression = (m_Console.Request.QueryString["w"] == "2" ? OutputExpression.FullWidth : OutputExpression.Alternating );
                    ((Framework.ContentSharedAttribute)element).Console = m_Console;
                    ((Framework.ContentSharedAttribute)element).IsBluePrint = true;
                    if (element.ContentTypeSelection == ContentType.Binary_Image)
                        ((ContentInfoItem.Binary_ImageAttribute)element).GalleryPropertyUrl = m_Console.Request.QueryString["gallery"];


                    element.SetCandidate(new Content.Field(), true);
                    build.AppendFormat(@"
                    <div class=""cmsable"">
                        {0}
                        <table class=""formTable"">", element.GetMultiFieldTitleHTML(true));

                    build.AppendFormat(@"
                        <tbody>
                        <tr>
                        <td>");

                    element.WriteCandidate(build, true, false, false);

                    build.AppendFormat(@"
                        </td>
                        </tr>
                        </tbody>");

                    build.AppendFormat(@"
                        </table>
                    </div>");

                    AddToResponse(build.ToString());
                }

                if (m_Console.Request.QueryString["xml"] == "component")
                {
                    var page = Data.Utility.ConvertToInt(m_Console.Request.QueryString["page"]);
                    var target = m_Console.Request.QueryString["tab"];
                    if (String.IsNullOrEmpty(target) && page > 0)
                    {
                        var pageInstance = Sushi.Mediakiwi.Data.Page.SelectOne(page);
                        var sections = pageInstance.Template.GetPageSections();
                        if (sections != null)
                            target = sections.FirstOrDefault();
                    }

                    AddToResponse(Sushi.Mediakiwi.Beta.GeneratedCms.Source.Xml.Component.Get(m_Console, Data.Utility.ConvertToInt(m_Console.Request.QueryString["id"]), page, Data.Utility.ConvertToInt(m_Console.Request.QueryString["cmpt"]), target));
                }

                m_Console.Response.Flush();
                m_Console.Context.Trace.Write("Parsing", "Done");
                m_Console.Response.End();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Obtain querystring requests
        /// </summary>
        void SetRequestType()
        {
            //  Verify paging page
            m_Console.ListPagingValue = m_Console.Request.Params["set"];

            m_Console.Group = Data.Utility.ConvertToIntNullable(m_Console.Request.QueryString["group"]);
            m_Console.GroupItem = Data.Utility.ConvertToIntNullable(m_Console.Request.QueryString["groupitem"]);

            //  Verify page request
            m_Console.Item = Data.Utility.ConvertToIntNullable(m_Console.Request.QueryString["page"], false);
            if (m_Console.Item.HasValue)
            {
                m_Console.ItemType = RequestItemType.Page;
                return;
            }
            //  Verify asset request
            m_Console.Item = Data.Utility.ConvertToIntNullable(m_Console.Request.QueryString["asset"], false);
            if (m_Console.Item.HasValue)
            {
                m_Console.ItemType = RequestItemType.Asset;
                return;
            }
            //  Verify dashboard request
            m_Console.Item = Data.Utility.ConvertToIntNullable(m_Console.Request.QueryString["dashboard"], false);
            if (m_Console.Item.HasValue)
            {
                m_Console.ItemType = RequestItemType.Dashboard;
                return;
            }
            //  Verify list-item request
            m_Console.Item = Data.Utility.ConvertToIntNullable(m_Console.Request.QueryString["item"], false);
            if (m_Console.Item.HasValue) m_Console.ItemType = RequestItemType.Item;
        }



        /// <summary>
        /// Apply the current component list based on the roaming environment settings
        /// </summary>
        bool ApplyComponentList()
        {
            //  If the list is not know, take the default list in stead (browsing)
            if (!string.IsNullOrEmpty(m_Console.Request.QueryString["list"]))
            {
                //  The list reference can be a INT or a GUID
                return m_Console.ApplyList(m_Console.Request.QueryString["list"]);
            }
            else if (m_Console.ItemType == RequestItemType.Asset)
            {
                m_Console.ApplyList(Data.ComponentListType.Documents);

            }
            else
                m_Console.ApplyList(Data.ComponentListType.Browsing);
            return true;
        }

        /// <summary>
        /// Sets the form modes (Edit/Save,Delete, etc)
        /// </summary>
        /// <param name="isDeleteMode">if set to <c>true</c> [is delete mode].</param>
        void SetFormModes(out bool isDeleteMode)
        {
            //  Is the form state in editmode?
            m_Console.CurrentListInstance.wim.IsEditMode = m_Console.IsPostBack("edit")
                || m_Console.IsPostBack("save")
                || m_Console.CurrentListInstance.wim.OpenInEditMode
                || m_Console.Request.QueryString["item"] == "0"
                || m_Console.Request.QueryString["asset"] == "0"
                || m_Console.JsonReferrer.Equals("edit")
                || m_Console.JsonForm != null;

            //  Create new page
            if (!m_Console.CurrentListInstance.wim.IsEditMode 
                && m_Console.ItemType == RequestItemType.Page 
                && m_Console.Request.UrlReferrer != null 
                && m_Console.Request.UrlReferrer.ToString().Contains("item=0")
                )
            {
                m_Console.CurrentListInstance.wim.IsEditMode = true;
            }

            //  Is the save link clicked?
            m_Console.CurrentListInstance.wim.IsSaveMode = m_Console.IsPostBack("save") || m_Console.IsPostBack("saveNew");

            //  Is the delete link clicked?
            isDeleteMode = (m_Console.IsPostBack("delete") || m_Console.Request.HttpMethod == "DELETE");
            m_Console.CurrentListInstance.wim.IsDeleteMode = isDeleteMode;

            //  Set the developer mode
            if (m_Console.PostbackValue == "dev_showhidden")
            {
                m_Console.CurrentApplicationUser.ShowHidden = true;
                m_Console.CurrentApplicationUser.Save();
            }
            else if (m_Console.PostbackValue == "dev_showvisible")
            {
                m_Console.CurrentApplicationUser.ShowHidden = false;
                m_Console.CurrentApplicationUser.Save();
            }
        }

        /// <summary>
        /// Redirects to channel home page.
        /// </summary>
        /// <param name="siteID">The site ID.</param>
        void RedirectToChannelHomePage(int siteID)
        {
            //  Find the default homepage in the menu section
            var defaultHome = Sushi.Mediakiwi.Data.MenuItemView.SelectAll(siteID, m_Console.CurrentApplicationUser.RoleID, 0);
            if (defaultHome != null && defaultHome.Length > 0)
            { 
                string redirect = defaultHome[0].Url(siteID);
                m_Console.Response.Redirect(redirect);
            }
        }

        /// <summary>
        /// Checks the security.
        /// </summary>
        void CheckSecurity(bool reStartWithNotificationList)
        {
            //  ACL: Sites
            if (!m_Console.CurrentListInstance.wim.CurrentApplicationUserRole.All_Sites
                && m_Console.CurrentListInstance.wim.CurrentApplicationUserRole.Sites(m_Console.CurrentApplicationUser).Length == 0)
            {
                if (!reStartWithNotificationList)
                    throw new Exception(Sushi.Mediakiwi.Framework.ErrorCode.GetMessage(1002, m_Console.CurrentApplicationUser.LanguageCulture));
            }
            //  ACL: Sites
            if (!m_Console.CurrentListInstance.wim.CurrentApplicationUserRole.All_Sites)
            {
                if (!m_Console.CurrentListInstance.wim.CurrentSite.HasRoleAccess(m_Console.CurrentListInstance.wim.CurrentApplicationUser))
                {
                    var allowed = m_Console.CurrentListInstance.wim.CurrentApplicationUserRole.Sites(m_Console.CurrentApplicationUser);
                    if (allowed != null && allowed.Length > 0)
                    {
                        RedirectToChannelHomePage(allowed[0].ID);
                    }
                    else
                    {
                        if (m_Console.CurrentListInstance.wim.CurrentApplicationUser.Sites(Data.AccessFilter.RoleAndUser) != null && m_Console.CurrentListInstance.wim.CurrentApplicationUser.Sites(Data.AccessFilter.RoleAndUser).Length > 0)
                            m_Console.Response.Redirect(m_Console.GetWimPagePath(m_Console.CurrentListInstance.wim.CurrentApplicationUser.Sites(Data.AccessFilter.RoleAndUser)[0].ID));
                        else
                            throw new Exception("There are no active accessible channels available.");
                    }
                }
            }
            else
            {
                //  CHECK FOR UserBased exceptions!!!
            }


            //  20-01-13:MM Added dashboard hack
            if (m_Console.CurrentListInstance.wim.CurrentFolder.ID == 0 && string.IsNullOrEmpty(m_Console.Request.QueryString["dashboard"]))
                RedirectToChannelHomePage(m_Console.ChannelIndentifier);

            //  ACL: Folders
            if (m_Console.CurrentListInstance.wim.CurrentFolder.Type != Sushi.Mediakiwi.Data.FolderType.Gallery 
                && !m_Console.CurrentListInstance.wim.CurrentApplicationUserRole.All_Folders)
            {
                if (!m_Console.CurrentListInstance.wim.CurrentFolder.HasRoleAccess(m_Console.CurrentListInstance.wim.CurrentApplicationUser))
                {
                    if (m_Console.CurrentListInstance.wim.CurrentFolder.ParentID.HasValue)
                        m_Console.Response.Redirect(m_Console.UrlBuild.GetFolderRequest(m_Console.CurrentListInstance.wim.CurrentFolder.ParentID.Value));

                    m_Console.Response.Redirect(m_Console.WimPagePath);
                }
            }

            //  Check environment
            bool approved = false;
            switch (m_Console.CurrentListInstance.wim.CurrentFolder.Type)
            {
                case Sushi.Mediakiwi.Data.FolderType.Undefined: 
                    approved = true; break;

                case Sushi.Mediakiwi.Data.FolderType.Page: 
                    approved = m_Console.CurrentListInstance.wim.CurrentApplicationUserRole.CanSeePage; 
                    
                    break;
                case Sushi.Mediakiwi.Data.FolderType.List:
                    if (m_Console.CurrentListInstance.wim.CurrentList.Type == Data.ComponentListType.Browsing)
                    {
                        approved = m_Console.CurrentListInstance.wim.CurrentApplicationUserRole.CanSeeFolder;
                        if (!approved)
                            m_Console.Response.Redirect(m_Console.WimPagePath);
                    }
                    approved = m_Console.CurrentListInstance.wim.CurrentApplicationUserRole.CanSeeList;
                    if (m_Console.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.Undefined && !m_Console.CurrentListInstance.wim.CurrentApplicationUserRole.All_Lists)
                    {
                        if (!m_Console.CurrentListInstance.wim.CurrentList.HasRoleAccess(m_Console.CurrentListInstance.wim.CurrentApplicationUser))
                            m_Console.Response.Redirect(m_Console.WimPagePath);
                    }
                    break;
                case Sushi.Mediakiwi.Data.FolderType.Gallery: 
                    approved = true;
                    if (!m_Console.CurrentListInstance.wim.CurrentApplicationUserRole.All_Galleries)
                    {
                        Sushi.Mediakiwi.Data.Gallery currentGallery = Sushi.Mediakiwi.Data.Gallery.SelectOne(m_Console.CurrentListInstance.wim.CurrentFolder.ID);
                        if (!currentGallery.HasRoleAccess(m_Console.CurrentListInstance.wim.CurrentApplicationUser))
                        {
                            if (currentGallery.ParentID.HasValue)
                                m_Console.Response.Redirect(m_Console.UrlBuild.GetGalleryRequest(currentGallery.ParentID.Value));

                            m_Console.Response.Redirect(m_Console.WimPagePath);
                        }
                    }                   
                    break;
                case Sushi.Mediakiwi.Data.FolderType.Administration: 
                    approved = m_Console.CurrentListInstance.wim.CurrentApplicationUserRole.CanSeeAdmin; 
                    break;
            }

            if (!approved)
            {
                if (m_Console.CurrentListInstance.wim.CurrentList.Type != Sushi.Mediakiwi.Data.ComponentListType.Images
                    && m_Console.CurrentListInstance.wim.CurrentList.Type != Sushi.Mediakiwi.Data.ComponentListType.Documents
                    && m_Console.CurrentListInstance.wim.CurrentList.Type != Sushi.Mediakiwi.Data.ComponentListType.Links)
                {
                    m_Console.Response.Redirect(m_Console.WimPagePath);
                }
            }
        }

        /// <summary>
        /// Checks and sets the current folder.
        /// </summary>
        /// <returns>Continue processing page?</returns>
        bool CheckFolder()
        {
            if (m_Console.CurrentListInstance.wim.CurrentFolder == null)
                throw new Exception("No containing folder found for the requested item!");

            //  If in browsing mode, set the page title to the current folder
            if (m_Console.Logic == 0 && !m_Console.Item.HasValue && !m_Console.CurrentListInstance.wim.CurrentFolder.IsNewInstance)
                m_Console.Title = m_Console.CurrentListInstance.wim.CurrentFolder.Name;
            return true;
        }

        /// <summary>
        /// Checks the site for channel requests.
        /// </summary>
        void CheckSite()
        {
            if (m_Console.CurrentList.SiteID.HasValue && m_Console.ChannelIndentifier != m_Console.CurrentList.SiteID.Value)
            {
                if (m_Console.CurrentList.IsInherited) return;
                var site = Sushi.Mediakiwi.Data.Site.SelectOne(m_Console.CurrentList.SiteID.Value);
                if (site.Type.HasValue) return;
                m_Console.Response.Redirect(m_Console.CurrentListInstance.wim.GetCurrentQueryUrl(true, m_Console.CurrentList.SiteID.Value, null));
            }
        }

        /// <summary>
        /// Handles the page request.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="component">The component.</param>
        /// <param name="isDeleteTriggered">if set to <c>true</c> [is delete triggered].</param>
        void HandleRequest(DataGrid grid, Component component, bool isDeleteTriggered)
        {
            this.HandleActionRequest();
            if (this.HandleAsyncRequest(component))
                m_Console.Response.End();

            if ((m_Console.ItemType == RequestItemType.Item) || m_Console.CurrentListInstance.wim.CanContainSingleInstancePerDefinedList)
            {//  Handles the list item request.
                m_Console.AddTrace("Monitor", "HandleListItemRequest(...)");
                HandleListItemRequest(grid, component, isDeleteTriggered);
            }
            else if (m_Console.ItemType == RequestItemType.Page)
            {
                m_Console.AddTrace("Monitor", "HandlePageItemRequest(...)");
                //  Handles the page request.
                HandlePageItemRequest(grid, component, isDeleteTriggered);
            }
            else if (m_Console.ItemType == RequestItemType.Asset)
            {
                m_Console.AddTrace("Monitor", "HandleListItemRequest(...)");
                HandleListItemRequest(grid, component, isDeleteTriggered);
            }
            else if (m_Console.ItemType == RequestItemType.Dashboard)//(m_Console.CurrentListInstance.wim.CurrentFolder.ID == 0)
            {

                //  [20091011:MM] Check if the list is part of a different folder. If so, redirect to this environment.
                if (m_Console.CurrentList.FolderID.HasValue && m_Console.CurrentList.Type != Sushi.Mediakiwi.Data.ComponentListType.Browsing)
                {
                    Sushi.Mediakiwi.Data.Folder candidate = Sushi.Mediakiwi.Data.Folder.SelectOne(m_Console.CurrentList.FolderID.Value);
                    if (candidate.SiteID != m_Console.ChannelIndentifier)
                    {
                        string url = m_Console.UrlBuild.GetListRequest(m_Console.CurrentList, Data.Utility.ConvertToIntNullable(m_Console.Request.QueryString["item"]));
                        m_Console.Response.Redirect(url);
                    }
                }

                m_Console.AddTrace("Monitor", "HandleDashboardRequest(...)");
                //  Handles the dashboard request.
                HandleDashboardRequest(grid, component);
            }
            else
            {
                m_Console.AddTrace("Monitor", "HandleBrowsingRequest(...)");
                //  Handles the browsing request.
                m_Console.CurrentListInstance.wim.IsSearchListMode = true;
                HandleBrowsingRequest(grid, component);
            }
        }


        /// <summary>
        /// Handles the list item request.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="component">The component.</param>
        /// <param name="isDeleteTriggered">if set to <c>true</c> [is delete triggered].</param>
        void HandleListItemRequest(DataGrid grid, Component component, bool isDeleteTriggered)
        {
            m_Console.View = (int)ContainerView.ItemSelect;

            if (m_Console.CurrentList.Type == Data.ComponentListType.ListSettings)
                m_Console.View = (int)ContainerView.ListSettingRequest;

            m_Console.AddTrace("Monitor", "GetExportOptionUrl(..)");
            if (GetExportOptionUrl(grid, component))
                return;

            //  Create the form
            m_Console.CurrentListInstance.wim.HideTopSectionTag = true;

            if (m_Console.Request.QueryString["DBM"] == "1")
                m_Console.CurrentListInstance.wim.IsDashboardMode = true;

            if (!m_Console.IsComponent)
            {
                if (m_Console.CurrentList.Option_FormAsync && !this.IsFormatRequest_JSON)
                {

                    this.GlobalWimControlBuilder = new WimControlBuilder();
                    m_Console.CurrentListInstance.wim.DoListInit();

                }
                else
                {
                    this.GlobalWimControlBuilder = component.CreateList(m_Console, m_Console.OpenInFrame);
                    m_Console.CurrentListInstance.wim.DoListInit();

                }

                if (this.IsFormatRequest_JSON)
                {
                    m_Console.Response.Clear();
                    m_Console.Response.ContentType = "application/json";

                    m_PresentationNavigation.RightSideNavigation(m_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null, this.GlobalWimControlBuilder);
                    m_PresentationNavigation.NewBottomNavigation(m_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null, false, this.GlobalWimControlBuilder);

                    Dictionary<string, string> formFields = null;
                    if (formFields == null)
                        formFields = new Dictionary<string, string>();

                    this.GlobalWimControlBuilder.ApiResponse.ListDescription = m_Console.CurrentListInstance.wim.CurrentList.Description;
                    this.GlobalWimControlBuilder.ApiResponse.RedirectUrl = m_Console.RedirectionUrl;
                    if (!string.IsNullOrWhiteSpace(m_Console.RedirectionUrl))
                        m_Console.Response.StatusCode = 302;

                    this.GlobalWimControlBuilder.ApiResponse.IsEditMode = m_Console.CurrentListInstance.IsEditMode;
                    this.GlobalWimControlBuilder.ApiResponse.ListTitle = m_Console.CurrentListInstance.wim.ListTitle;
                    // if this item is a button add it to the button list

                    m_Console.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(this.GlobalWimControlBuilder.ApiResponse));

                    m_Console.CurrentVisitor.Save();
                    m_Console.Response.End();
                }
               
                else if (this.IsFormatRequest_AJAX)
                {
                    m_Console.Response.Clear();
                    m_Console.Response.ContentType = "text/plain";
                    string searchListGrid = this.GlobalWimControlBuilder.SearchGrid;
                    m_Console.Response.Write(searchListGrid);
                    m_Console.CurrentVisitor.Save();
                    m_Console.Response.End();
                }
                else
                    //  Needed to NULLafy it as it was required for AJAX call
                    this.GlobalWimControlBuilder.SearchGrid = null;
            }

            bool isCopyTriggered = m_Console.Context.Request["copyparent"] == "1";

            if (isCopyTriggered)
            {
                int childID = m_Console.CurrentListInstance.wim.CurrentSite.ID;
                int parentID = m_Console.CurrentListInstance.wim.CurrentSite.MasterID.GetValueOrDefault();

                m_Console.CurrentListInstance.wim.CurrentSite = Sushi.Mediakiwi.Data.Site.SelectOne(parentID);
                m_Console.CurrentListInstance.wim.IsCurrentList = true;
                m_Console.CurrentListInstance.wim.DoListLoad(m_Console.Item.GetValueOrDefault(0), 0);
        

                m_Console.Context.Response.Write(string.Format("{0}<br/>", m_Console.CurrentListInstance.wim.CurrentSite.ID));
                m_Console.Context.Response.Write(string.Format("{0}<br/>", m_Console.CurrentListInstance.wim.CurrentSite.MasterID.GetValueOrDefault()));

                m_Console.Context.Response.End();
            }

            //  Is the delete event triggered?
            if (isDeleteTriggered && m_Console.CurrentListInstance.wim.HasListDelete)
            {
                m_Console.CurrentListInstance.wim.DoListDelete(m_Console.Item.GetValueOrDefault(0), 0, null);
                
                //  Add deletion entry
                Sushi.Mediakiwi.Data.ComponentListVersion version = new Sushi.Mediakiwi.Data.ComponentListVersion();
                version.SiteID = m_Console.CurrentListInstance.wim.CurrentSite.ID;
                version.ComponentListID = m_Console.CurrentListInstance.wim.CurrentList.ID;
                if (m_Console.Item.HasValue)
                    version.ComponentListItemID = m_Console.Item.Value;

                version.ApplicationUserID = m_Console.CurrentApplicationUser.ID;
                version.TypeID = 2;
                version.Save();

                if (m_Console.OpenInFrame > 0)
                    return;
                
                //  Redirect to the containing folder
                if (m_Console.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.Documents)
                    m_Console.Response.Redirect(string.Concat(m_Console.WimPagePath, "?gallery=", m_Console.CurrentListInstance.wim.CurrentFolder.ID));
                else if (m_Console.CurrentListInstance.wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Gallery)
                    m_Console.Response.Redirect(string.Concat(m_Console.WimPagePath, "?gallery=", m_Console.CurrentListInstance.wim.CurrentFolder.ParentID));
                else if (m_Console.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.Folders)
                    m_Console.Response.Redirect(string.Concat(m_Console.WimPagePath, "?folder=", m_Console.CurrentListInstance.wim.CurrentFolder.ParentID));
                else if (m_Console.Group.HasValue)
                    m_Console.Response.Redirect(string.Concat(m_Console.WimPagePath, "?", m_Console.CurrentListInstance.wim.SearchResultItemPassthroughParameter, "=0"), true);
                else if (m_Console.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.ComponentListProperties)
                    m_Console.Response.Redirect(string.Concat(m_Console.WimPagePath, "?folder=", m_Console.CurrentListInstance.wim.CurrentFolder.ID));
                else
                    m_Console.Response.Redirect(string.Concat(m_Console.WimPagePath, "?list=", m_Console.CurrentList.ID));
            }



            if (m_Console.IsComponent)
            {
                m_Console.CurrentListInstance.wim.DoListLoad(m_Console.Item.GetValueOrDefault(), 0);

                bool isPagePublishTriggered = m_Console.IsPostBack("pagepublish");
                bool isPageOfflineTriggered = m_Console.IsPostBack("pageoffline");

                if (isPagePublishTriggered)
                {
                    Sushi.Mediakiwi.Data.ComponentVersion.SelectOne(m_Console.Item.Value).Publish();
                }
                if (isPageOfflineTriggered)
                {
                    Sushi.Mediakiwi.Data.ComponentVersion.SelectOne(m_Console.Item.Value).TakeDown();
                }

                if (isPageOfflineTriggered || isPagePublishTriggered)
                    m_Console.Response.Redirect(m_Console.UrlBuild.GetListRequest(m_Console.CurrentList, (m_Console.Item.Value)));

                Sushi.Mediakiwi.Data.Page pageInstance;

                this.GlobalWimControlBuilder = component.CreateContentList(m_Console, 0, true, out pageInstance, null);
                this.GlobalWimControlBuilder.Canvas.Type = m_Console.OpenInFrame > 0 ? CanvasType.ListItemInLayer : CanvasType.ListItem;
                this.GlobalWimControlBuilder.TopNavigation = m_PresentationNavigation.TopNavigation(m_Console);
                // [CB: 3-1-2016]  waarom deze geen bottom had... raadsel. zo krijg je nooit een save knop
                this.GlobalWimControlBuilder.Bottom = m_PresentationNavigation.NewBottomNavigation(m_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null, false);
                this.GlobalWimControlBuilder.Tabularnav = Sushi.Mediakiwi.Beta.GeneratedCms.Source.Template.GetTabularTagNewDesign(m_Console, m_Console.CurrentList.Name, 0, false);
                this.GlobalWimControlBuilder.Leftnav = m_PresentationNavigation.NewLeftNavigation(m_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null);

                AddToResponse(m_PresentationMonitor.GetTemplateWrapper(m_Console, m_Placeholders, m_Callbacks, this.GlobalWimControlBuilder));

                return;
            }



            if (!m_Console.IsAdminFooter)
            {
                this.GlobalWimControlBuilder.TopNavigation = m_PresentationNavigation.TopNavigation(m_Console);
                this.GlobalWimControlBuilder.Canvas.Type = m_Console.OpenInFrame > 0 ? CanvasType.ListItemInLayer : CanvasType.ListItem;
                this.GlobalWimControlBuilder.Rightnav = m_PresentationNavigation.RightSideNavigation(m_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null);
                
                
                if (
                    this.GlobalWimControlBuilder.Canvas.Type == CanvasType.ListInLayer ||
                    this.GlobalWimControlBuilder.Canvas.Type == CanvasType.ListItemInLayer
                    )
                {
                    //  Do nothing, this is an layer and has no leftnavigation.
                }
                else
                    this.GlobalWimControlBuilder.Leftnav = m_PresentationNavigation.NewLeftNavigation(m_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null);
                this.GlobalWimControlBuilder.Bottom = m_PresentationNavigation.NewBottomNavigation(m_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null, false);
                this.GlobalWimControlBuilder.Tabularnav = Sushi.Mediakiwi.Beta.GeneratedCms.Source.Template.GetTabularTagNewDesign(m_Console, m_Console.CurrentList.Name, 0, false);

                AddToResponse(m_PresentationMonitor.GetTemplateWrapper(m_Console, m_Placeholders, m_Callbacks, this.GlobalWimControlBuilder));
            }
        }

        bool HandleAsyncRequest(Component component)
        {
            if (m_Console.CurrentListInstance == null) return false;
            var async = Data.Utility.GetAsyncQuery();
            if (async == null)
                return false;


            if (m_Console.CurrentListInstance.wim.HasListAsync)
            {
                m_Console.HasAsyncEvent = true;
                ComponentAsyncEventArgs eventArgs = new ComponentAsyncEventArgs(m_Console.Item.GetValueOrDefault());

                eventArgs.Query = async.SearchQuery;
                eventArgs.SearchType = async.SearchType;
                eventArgs.Property = async.Property;

                eventArgs.Data = new ASyncResult();
                eventArgs.Data.Property = async.Property;
                eventArgs.ApplyData(component, m_Console);
                eventArgs.SelectedGroupItemKey = m_Console.GroupItem.GetValueOrDefault();
                eventArgs.SelectedGroupKey = m_Console.Group.GetValueOrDefault();

                var result = m_Console.CurrentListInstance.wim.DoListAsync(eventArgs);

                string val = Wim.Utilities.JSON.Instance.ToJSON(result.Data,
                    new Wim.Utilities.JSONParameters()
                    {
                        EnableAnonymousTypes = true,
                        UsingGlobalTypes = false,
                        SerializeNullValues = false
                    }
                );
                m_Console.Response.ContentType = "application/json";
                m_Console.Response.Write(val);
                
                return true;
            }
            return false;
        }

        /// <summary>
        /// Handles the action request.
        /// </summary>
        void HandleActionRequest()
        {
            if (m_Console.CurrentListInstance == null) return;
            if (string.IsNullOrEmpty(m_Console.CurrentListInstance.wim.PostbackValue)) return;

            
            switch (m_Console.CurrentListInstance.wim.PostbackValue)
            {
                case "PageContentPublication":
                    Sushi.Mediakiwi.Data.EnvironmentVersionLogic.Flush();
                    m_Console.CurrentListInstance.wim.Notification.AddNotification("The webcontent has been refreshed.");
                    return;
            }
        }

        /// <summary>
        /// Handles the page item request.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="component">The component.</param>
        /// <param name="isDeleteTriggered">if set to <c>true</c> [is delete triggered].</param>
        void HandlePageItemRequest(DataGrid grid, Component component, bool isDeleteTriggered)
        {
            m_Console.CurrentListInstance.wim.IsEditMode = 
                m_Console.CurrentApplicationUser.Role().CanChangePage;

            if (m_Console.CurrentPage == null)
            {

                Sushi.Mediakiwi.Data.Page page = Sushi.Mediakiwi.Data.Page.SelectOne(m_Console.Item.Value);
                m_Application.Context.Items.Add("Wim.Page", page);
                m_Application.Context.Items.Add("Wim.Site", page.Site);
            }

            m_Console.View = 0;

            bool isPagePublishTriggered = m_Console.IsPostBack("pagepublish");
            bool isPageOfflineTriggered = m_Console.IsPostBack("pageoffline");

            bool isPageLocalised = m_Console.IsPostBack("page.localize");
            bool isPageInherited = m_Console.IsPostBack("page.inherit");

            int selectedTab = Data.Utility.ConvertToInt(m_Console.Request.QueryString["tab"]);
            string section = m_Console.Request.QueryString["tab"];

            // [MR:26-03-2019] for Page Modules
            string pBack = string.Empty;
            if (m_Console.PostBackStartsWith("pagemod_", out pBack))
            {
                pBack = pBack.Replace("pagemod_", "");

                foreach (var pmodule in Data.Environment.GetPageModules())
                {
                    if (pmodule.GetType().Name == pBack)
                    {
                        var moduleResult = pmodule.Execute(m_Console.CurrentPage, m_Console.CurrentApplicationUser);
                        if (moduleResult.IsSuccess && string.IsNullOrWhiteSpace(moduleResult.WimNotificationOutput) == false)
                        {
                            m_Console.CurrentListInstance.wim.Notification.AddNotification(moduleResult.WimNotificationOutput);
                        }
                        else if (string.IsNullOrWhiteSpace(moduleResult.WimNotificationOutput) == false)
                        {
                            m_Console.CurrentListInstance.wim.Notification.AddError(moduleResult.WimNotificationOutput);
                        }

                        Data.Page page = Data.Page.SelectOne(m_Console.Item.Value, false);
                        if (page?.ID > 0)
                        {
                            Framework2.Functions.AuditTrail.Insert(m_Console.CurrentApplicationUser, page, Framework2.Functions.Auditing.ActionType.PageModuleExecution, null);
                        }
                    }
                }
            }



            if (string.IsNullOrEmpty(section))
            {
                //  26-08-14:MM
                var sections = m_Console.CurrentPage.Template.GetPageSections();
                if (sections.Length > 0)
                    section = sections[0];
            }

            string redirect = string.IsNullOrEmpty(section) ? "" : string.Concat("&tab=", section);

            if (m_Console.IsPostBack("page.translate"))
            {
                m_Console.CurrentApplicationUser.ShowTranslationView = true;
                m_Console.CurrentApplicationUser.Save();

                if (!m_Console.CurrentListInstance.IsEditMode)
                    m_Console.Response.Redirect(string.Concat(m_Console.WimPagePath, "?page=", m_Console.Item.Value, redirect));
            }
            if (m_Console.IsPostBack("page.copy"))
            {
                Sushi.Mediakiwi.Data.ComponentVersionLogic.CopyFromMaster(m_Console.Item.Value);
                m_Console.CurrentListInstance.wim.FlushCache(true);

                if (!m_Console.CurrentListInstance.IsEditMode)
                    m_Console.Response.Redirect(string.Concat(m_Console.WimPagePath, "?page=", m_Console.Item.Value, redirect));
            }
            if (m_Console.IsPostBack("page.normal"))
            {
                m_Console.CurrentApplicationUser.ShowTranslationView = false;
                m_Console.CurrentApplicationUser.Save();

                if (!m_Console.CurrentListInstance.IsEditMode)
                    m_Console.Response.Redirect(string.Concat(m_Console.WimPagePath, "?page=", m_Console.Item.Value, redirect));
            }

            if (isPagePublishTriggered)
            {
                Sushi.Mediakiwi.Data.Page page = Sushi.Mediakiwi.Data.Page.SelectOne(m_Console.Item.Value, false);
                var pagePublicationHandler = Sushi.Mediakiwi.Data.Environment.GetInstance<IPagePublication>();

                page.Publish(pagePublicationHandler, m_Console.CurrentApplicationUser);
                Sushi.Mediakiwi.Framework2.Functions.AuditTrail.Insert(m_Console.CurrentApplicationUser, page, Framework2.Functions.Auditing.ActionType.Publish, null);
                m_Console.Response.Redirect(string.Concat(m_Console.WimPagePath, "?page=", m_Console.Item.Value, redirect));
            }
            else if (isPageOfflineTriggered)
            {
                Sushi.Mediakiwi.Data.Page page = Sushi.Mediakiwi.Data.Page.SelectOne(m_Console.Item.Value, false);
                var pagePublicationHandler = Sushi.Mediakiwi.Data.Environment.GetInstance<IPagePublication>();

                page.TakeDown(pagePublicationHandler, m_Console.CurrentApplicationUser);
                Sushi.Mediakiwi.Framework2.Functions.AuditTrail.Insert(m_Console.CurrentApplicationUser, page, Framework2.Functions.Auditing.ActionType.TakeOffline, null);
                m_Console.Response.Redirect(string.Concat(m_Console.WimPagePath, "?page=", m_Console.Item.Value, redirect));
            }
            else if (isDeleteTriggered)
            {
                Sushi.Mediakiwi.Data.Page page = Sushi.Mediakiwi.Data.Page.SelectOne(m_Console.Item.Value, false);

                //  Save the version
                var currentversion = Sushi.Mediakiwi.Data.ComponentVersion.SelectAllOnPage(page.ID);
                component.SavePageVersion(page, currentversion, m_Console.CurrentApplicationUser, true);

                page.Delete();
                Sushi.Mediakiwi.Framework2.Functions.AuditTrail.Insert(m_Console.CurrentApplicationUser, page, Framework2.Functions.Auditing.ActionType.Remove, null);
                m_Console.Response.Redirect(string.Concat(m_Console.WimPagePath, "?folder=", m_Console.CurrentListInstance.wim.CurrentFolder.ID));
            }
            else if (isPageLocalised)
            {
                Sushi.Mediakiwi.Data.Page page = Sushi.Mediakiwi.Data.Page.SelectOne(m_Console.Item.Value, false);
                page.InheritContentEdited = false;
                page.Updated = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
                page.Save();

                Sushi.Mediakiwi.Framework2.Functions.AuditTrail.Insert(m_Console.CurrentApplicationUser, page, Framework2.Functions.Auditing.ActionType.Localised, null);
                m_Console.Response.Redirect(string.Concat(m_Console.WimPagePath, "?page=", m_Console.Item.Value, redirect));
                
            }
            else if (isPageInherited)
            {
                Sushi.Mediakiwi.Data.Page page = Sushi.Mediakiwi.Data.Page.SelectOne(m_Console.Item.Value, false);
                page.InheritContentEdited = true;
                page.Updated = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
                page.Save();

                Sushi.Mediakiwi.Framework2.Functions.AuditTrail.Insert(m_Console.CurrentApplicationUser, page, Framework2.Functions.Auditing.ActionType.Inherited, null);
                m_Console.Response.Redirect(string.Concat(m_Console.WimPagePath, "?page=", m_Console.Item.Value, redirect));
                
            }

            Data.Page pageInstance;

            this.GlobalWimControlBuilder = component.CreateContentList(m_Console, 0, selectedTab == 1, out pageInstance, section);

            if (!m_Console.IsAdminFooter)
            {
            
                this.GlobalWimControlBuilder.Canvas.Type = CanvasType.ListItem;

                this.GlobalWimControlBuilder.Leftnav = m_PresentationNavigation.NewLeftNavigation(m_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null);

                this.GlobalWimControlBuilder.TopNavigation = m_PresentationNavigation.TopNavigation(m_Console);
                this.GlobalWimControlBuilder.Rightnav = m_PresentationNavigation.RightSideNavigation(m_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null);
                this.GlobalWimControlBuilder.Bottom = m_PresentationNavigation.NewBottomNavigation(m_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null, false);

                AddToResponse(m_PresentationMonitor.GetTemplateWrapper(m_Console, m_Placeholders, m_Callbacks, this.GlobalWimControlBuilder));
            }
        }

        internal Sushi.Mediakiwi.Framework.WimControlBuilder GlobalWimControlBuilder;

        bool GetExportOptionUrl(DataGrid grid, Component component)
        {
            string exportUrl = null;
            //  Export to XLS: XLS Creation URL
            if (m_Console.IsPostBack("export_xls") || m_Console.Request.QueryString["xls"] == "1")
            {
                m_Console.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                m_Console.Response.Cache.SetAllowResponseInBrowserHistory(false);

                exportUrl = Data.Utility.GetSafeUrl(m_Console.Request);

                m_Console.CurrentListInstance.wim.IsExportMode_XLS = true;

                component.CreateSearchList(m_Console, 0);
                var url = grid.GetGridFromListInstanceForXLS(m_Console, m_Console.CurrentListInstance, 0);
                if (m_Console.Request.QueryString["xp"] == "1")
                {
                    m_Console.Response.Write(url);
                    m_Console.Response.End();
                }
                else
                {
                    m_Console.Response.Redirect(url);
                    return true;
                }
                //  Reset
                m_Console.CurrentListInstance.wim.IsExportMode_XLS = false;
            }
            //  Export to PDF: PDF Creation URL
            return false;
        }

        /// <summary>
        /// Handles the browsing request.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="component">The component.</param>
        void HandleBrowsingRequest(DataGrid grid, Component component)
        {
            m_Console.AddTrace("Monitor", "HandleListItemRequest.Init");
            m_Console.View = 2;
            m_Console.CurrentListInstance.wim.IsEditMode = true;

            if (m_Console.Request.QueryString["DBM"] == "1")
                m_Console.CurrentListInstance.wim.IsDashboardMode = true;

            m_Console.AddTrace("Monitor", "GetExportOptionUrl(..)");
            if (GetExportOptionUrl(grid, component))
                return;

                m_Console.AddTrace("Monitor", "CreateSearchList(..)");

                this.GlobalWimControlBuilder = component.CreateSearchList(m_Console, 0);
                this.GlobalWimControlBuilder.Canvas.Type = m_Console.OpenInFrame > 0 ? CanvasType.ListInLayer  : CanvasType.List;

                if (m_Console.OpenInFrame > 0)
                    m_Console.CurrentListInstance.wim.Page.HideTabs = true;
            //}
            
            string searchListGrid;
            if (true)//m_Console.CurrentList.Type != Sushi.Mediakiwi.Data.ComponentListType.Browsing || m_Console.CurrentApplicationUser.ShowNewDesign2)
            {
                m_Console.AddTrace("Monitor", "GetGridFromListInstance(..)");

                if (this.IsFormatRequest_JSON)
                {
                    m_Console.Response.Clear();
                    m_Console.Response.ContentType = "application/json";
                    searchListGrid = grid.GetGridFromListInstanceForJSON(m_Console.CurrentListInstance.wim, m_Console, 0, false, IsNewDesignOutput);

                    m_Console.Response.Write(searchListGrid);
                    m_Console.CurrentVisitor.Save();
                    m_Console.Response.End();
                }
                if (this.IsFormatRequest_AJAX)
                {
                    m_Console.Response.Clear();
                    m_Console.Response.ContentType = "text/plain";
                    searchListGrid = null;
                    while (m_Console.CurrentListInstance.wim.NextGrid())
                    {
                        bool hasNoTitle = string.IsNullOrEmpty(m_Console.CurrentListInstance.wim.m_DataTitle);
                        searchListGrid +=
                            string.Concat(
                                hasNoTitle
                                    ? null
                                    : string.Format("</section><section class=\"{1}\"><h2>{0}</h2>"
                                    , m_Console.CurrentListInstance.wim.m_DataTitle
                                    , m_Console.CurrentListInstance.wim.Page.Body.Grid.ClassName
                            )
                            , grid.GetGridFromListInstance(m_Console.CurrentListInstance.wim, m_Console, 0, false, IsNewDesignOutput)
                            , hasNoTitle
                                    ? null
                                    : ""

                            );
                    }
                    m_Console.Response.Write(searchListGrid);
                    m_Console.CurrentVisitor.Save();
                    m_Console.Response.End();
                }
                if (m_Console.CurrentListInstance.wim.CurrentList.Option_SearchAsync && !m_Console.CurrentListInstance.wim.IsDashboardMode)
                {
                    //  CLEANUP TWO LOCATIONS !!! (27.01.14:MM)
                    if (m_Console.OpenInFrame > 0)
                        searchListGrid = string.Format("<section id=\"datagrid\" class=\"{0} async\"> </section>", m_Console.CurrentListInstance.wim.Page.Body.Grid.ClassName);//"<section class=\"searchTable\"> </section>";//grid.GetGridFromListInstanceForKnockout(m_Console.CurrentListInstance.wim, m_Console, 0, false, IsNewDesignOutput, false);\
                    else
                        searchListGrid = " ";
                }
                else
                {
                    searchListGrid = null;// grid.GetGridFromListInstance(m_Console.CurrentListInstance.wim, m_Console, 0, false, IsNewDesignOutput);
                    while (m_Console.CurrentListInstance.wim.NextGrid())
                    {
                        bool hasNoTitle = string.IsNullOrEmpty(m_Console.CurrentListInstance.wim.m_DataTitle);
                        searchListGrid +=
                            string.Concat(
                                hasNoTitle
                                    ? null
                                    : string.Format("</section><section class=\"{1}\"><h2>{0}</h2>", m_Console.CurrentListInstance.wim.m_DataTitle, m_Console.CurrentListInstance.wim.Page.Body.Grid.ClassName
                            )
                            , grid.GetGridFromListInstance(m_Console.CurrentListInstance.wim, m_Console, 0, false, IsNewDesignOutput)
                            , hasNoTitle
                                    ? null
                                    : ""
                            );
                    }
                }

                //  Legacy event, will be removed!
                if (!string.IsNullOrEmpty(component.m_ClickedButton) && m_Console.CurrentListInstance.wim.HasListSearchedAction)
                    m_Console.CurrentListInstance.wim.DoListSearchedAction(m_Console.Item.GetValueOrDefault(0), 0, component.m_ClickedButton, null);

                //  Replacement event of ListSearchedAction
                if (!string.IsNullOrEmpty(component.m_ClickedButton) && m_Console.CurrentListInstance.wim.HasListAction)
                    m_Console.CurrentListInstance.wim.DoListAction(m_Console.Item.GetValueOrDefault(0), 0, component.m_ClickedButton, null);

                m_Console.CurrentListInstance.wim.SendReport(searchListGrid);
            }
            else
            {
                this.GlobalWimControlBuilder.Canvas.Type = CanvasType.Explorer;

                m_Console.AddTrace("Monitor", "GetThumbnailGridFromListInstance(..)");
                searchListGrid = grid.GetThumbnailGridFromListInstance(m_Console.CurrentListInstance.wim, m_Console, 0, false);
            }

            m_Console.AddTrace("Monitor", "AddToResponse(..)");

            this.GlobalWimControlBuilder.SearchGrid = searchListGrid;
            this.GlobalWimControlBuilder.TopNavigation = m_PresentationNavigation.TopNavigation(m_Console);
            this.GlobalWimControlBuilder.Bottom = m_PresentationNavigation.NewBottomNavigation(
                m_Console, 
                component.m_ButtonList != null 
                    ? component.m_ButtonList.ToArray() 
                    : null,
                !this.GlobalWimControlBuilder.IsNull
            );

            this.GlobalWimControlBuilder.Tabularnav = Sushi.Mediakiwi.Beta.GeneratedCms.Source.Template.GetTabularTagNewDesign(m_Console, m_Console.CurrentList.Name, 0, false);
            this.GlobalWimControlBuilder.Rightnav = m_PresentationNavigation.RightSideNavigation(m_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null);
            this.GlobalWimControlBuilder.Leftnav = m_PresentationNavigation.NewLeftNavigation(m_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null);

            AddToResponse(m_PresentationMonitor.GetTemplateWrapper(m_Console, m_Placeholders, m_Callbacks, this.GlobalWimControlBuilder));
            return;
        }



        /// <summary>
        /// Handles the dashboard request.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="component">The component.</param>
        void HandleDashboardRequest(DataGrid grid, Component component)
        {
            this.GlobalWimControlBuilder = new WimControlBuilder();
            this.GlobalWimControlBuilder.Canvas.Type = CanvasType.Dashboard;
            this.GlobalWimControlBuilder.TopNavigation = m_PresentationNavigation.TopNavigation(m_Console);

            int dashboardID = Data.Utility.ConvertToInt(m_Console.Request.QueryString["dashboard"], m_Console.CurrentListInstance.wim.CurrentApplicationUserRole.Dashboard);
            Data.Dashboard dashboard = Data.Dashboard.SelectOne(dashboardID);

            int index = 0;

            IComponentListTemplate filterInstance = null;
            var menu = Sushi.Mediakiwi.Data.MenuItemView.SelectAll_Dashboard(dashboard.ID);
            if (menu != null && menu.Length > 0)
            {
                StringBuilder shortcutMenu = new StringBuilder();
                foreach (var item in menu)
                {
                    ComponentDataReportEventArgs e = null;
                    if (item.TypeID == 1)
                    {
                        //  List, can have total
                        var list = Data.ComponentList.SelectOne(item.ItemID);
                        if (list.Option_HasDataReport)
                        {
                            var instance = Data.Utility.CreateInstance(list) as IComponentListTemplate;
                            if (instance != null)
                                e = instance.wim.DoListDataReport();
                        }
                    }
                    if (e == null || !e.ReportCount.HasValue)
                        shortcutMenu.AppendFormat("<a href=\"{0}\">{1}", item.Url(this.m_Console.ChannelIndentifier), item.Name);
                    else
                    {
                        string count = e.ReportCount.Value.ToString();
                        if (e.ReportCount.Value > 99)
                            count = "99+";

                        shortcutMenu.AppendFormat("<a href=\"{0}\">{1} <span class=\"items{3}\">{2}</span></a>"
                            , item.Url(this.m_Console.ChannelIndentifier), item.Name, count, e.IsAlert ? " attention" : null);
                    }
                }
                //<h2>Shortcuts</h2>
                this.GlobalWimControlBuilder.Canvas.Dashboard.AppendFormat("<div class=\"widget\">{0}</div>"
                     , shortcutMenu.ToString()
                 );
            }
           

            foreach (Sushi.Mediakiwi.Data.IComponentList list in dashboard.DashboardTarget)
            {
                m_Console.ApplyList(list);
             
                m_Console.CurrentListInstance.wim.IsDashboardMode = true;
                m_Console.View = 3;
                m_Console.CurrentListInstance.wim.IsEditMode = true;
                m_Console.CurrentListInstance.wim.DashBoardFilterTemplate = filterInstance;

                index += m_Console.CurrentListInstance.wim.DashBoardElementWidth;

                string searchList = m_Console.CurrentListInstance.wim.DashBoardShowFilterSection ? component.CreateSearchList(m_Console, m_Console.View).ToString() : null;

                string content = grid.GetGridFromListInstance(m_Console.CurrentListInstance.wim, m_Console, 4, IsNewDesignOutput);
                
                string report = m_Console.CurrentListInstance.wim.DashBoardHtmlContainer;

                if (!m_Console.CurrentListInstance.wim.DashBoardElementIsVisible)
                    continue;

                if (m_Console.CurrentListInstance.wim.DashBoardShowFilterSection)
                {
                    this.GlobalWimControlBuilder.Canvas.Dashboard.Append(report);
                    filterInstance = m_Console.CurrentListInstance;
                }
                else
                {
                    if (string.IsNullOrEmpty(content))
                    {
                        content = string.Format(@"<div class=""nodata""><figure class=""{0}""></figure><p class=""title"">{1}</p><p>{2}</p></div>"
                            , m_Console.CurrentListInstance.wim.NoData.IconClass
                            , m_Console.CurrentListInstance.wim.NoData.Title
                            , m_Console.CurrentListInstance.wim.NoData.SubTitle                            
                            );
                    }
                    this.GlobalWimControlBuilder.Canvas.Dashboard.AppendFormat("<section id=\"dash{0}\" class=\"widget\"><header><h2><a href=\"{3}\">{2}</a></h2></header>{5}{4}</section>"
                        , index
                        , index % 3 == 0 ? " last" : string.Empty
                        , list.Name
                        , new UrlBuilder(m_Console).GetListRequest(list)
                        , content
                        , report
                    );
                }
            }

            m_Console.CurrentListInstance.wim.ListTitle = dashboard.Title;
            AddToResponse(m_PresentationMonitor.GetTemplateWrapper(m_Console, m_Placeholders, m_Callbacks, this.GlobalWimControlBuilder));
        }
    }
}
