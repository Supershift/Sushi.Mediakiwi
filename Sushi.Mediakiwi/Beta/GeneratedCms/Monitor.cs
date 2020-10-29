using System;
using System.Text;
using System.Net;
using System.Diagnostics;
using System.Data;
using System.Web;
using System.Web.Util;
using System.Web.Hosting;
using System.IO;
using System.Reflection;
using Sushi.Mediakiwi.Beta.GeneratedCms.Supporting;
using Sushi.Mediakiwi.Beta.GeneratedCms.Source;
using Sushi.Mediakiwi.Framework;
using System.Web.Security;

namespace Sushi.Mediakiwi.Beta.GeneratedCms
{
    /// <summary>
    /// 
    /// </summary>
    public class Monitor
    {
        internal Console m_Console;

        /// <summary>
        /// Initializes a new instance of the <see cref="Monitor"/> class.
        /// </summary>
        /// <param name="application">The application.</param>
        public Monitor(HttpApplication application)
            : this(application, false) { }

        bool IsLoadedInThinLayer;
        internal bool IsNewDesignOutput;

        public Monitor(HttpApplication application, bool isLoadedInThinLayer)
        {
            IsLoadedInThinLayer = isLoadedInThinLayer;

            application.Context.Items["newstyle"] = "1";
            application.Context.Items["no-cache"] = "1";

            m_Console = new Console(application, this.IsNewDesignOutput);
            m_Console.AddTrace("Monitor", "CTor");



            //bool isDifferentVersion = Data.Environment.SelectOne().Version.ToString("N").Replace(',', '.') != CommonConfiguration.Version;

            //isDifferentVersion = false;
            //if (isDifferentVersion)
            //{
            //    m_Console.ApplyInstance("Sushi.Mediakiwi.AppCentre.dll", "Sushi.Mediakiwi.AppCentre.Data.Implementation.VersionUpdater");

            //    m_Console.IsCodeUpdate = true;
            //    m_Console.CurrentList = new Sushi.Mediakiwi.Data.ComponentList();
            //    m_Console.CurrentApplicationUser = new Sushi.Mediakiwi.Data.ApplicationUser();
            //    m_Console.CurrentApplicationUser.Name = "Updating application";

            //    m_Console.CurrentListInstance.wim.CurrentSite = new Sushi.Mediakiwi.Data.Site();
            //    m_Console.CurrentListInstance.wim.m_CurrentFolder = new Sushi.Mediakiwi.Data.Folder();
            //    m_Console.CurrentListInstance.wim.CurrentList = Sushi.Mediakiwi.Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.VersionUpdater);
            //    m_Console.CurrentListInstance.wim.CurrentApplicationUser = m_Console.CurrentApplicationUser;
            //    m_Console.CurrentListInstance.wim.CurrentApplicationUserRole = new Sushi.Mediakiwi.Data.ApplicationRole();

            //    Source.GridCreation grid = new Source.GridCreation();
            //    Source.Component component = new Source.Component();

            //    this.GlobalWimControlBuilder = component.CreateList(m_Console, -1);

            //    AddToResponse(Source.Template.FolderBrowsing(this, 
            //        m_Console,
            //        Source.Generic.Header(m_Console),
            //        Source.Generic.TopContainer(m_Console),
            //        Source.Generic.Footer(),
            //        Source.Navigation.Leftnavigation(m_Console, component.m_ButtonList == null ? null : component.m_ButtonList.ToArray()),
            //        Source.Navigation.BreadcrumbNavigation(m_Console, component.m_ButtonList == null ? null : component.m_ButtonList.ToArray()),
            //        null,
            //        "[controlcollection]",
            //        m_Console.Title, false
            //        ));

            //    Wim.Utilities.CacheItemManager.FlushCacheObject("Data.Environment");

            //    m_Console.AddTrace("Monitor", "Done");
            //    m_Console.Response.Flush();
            //    m_Console.Response.End();
            //}
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

            m_Console.AddTrace("Monitor", "Start");

            //  Set the current environment
            m_Console.CurrentEnvironment = Data.Environment.Current;
            m_Console.SetDateFormat();

            m_Console.AddTrace("Monitor", "Start.CheckRoamingApplicationUser");
            //  When there is no roaming application user, redirect to the login.
            if (!this.CheckRoamingApplicationUser()) return;

            m_Console.AddTrace("Monitor", "Start.OutputAjaxRequest");
            //  If an xml (ajax) request comes in output a correct response.
            if (!this.OutputAjaxRequest()) return;

            m_Console.AddTrace("Monitor", "Start.SetRequestType()");
            //  Obtain querystring requests
            this.SetRequestType();
            //  Apply the current component list based on the roaming environment settings

            m_Console.AddTrace("Monitor", "Start.ApplyComponentList()");

            if (reStartWithNotificationList)
                m_Console.ApplyList(Sushi.Mediakiwi.Data.ComponentListType.InformationMessage);
            else
                ApplyComponentList();

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
            int openInFrame = Utility.ConvertToInt(m_Console.Request.QueryString["openinframe"]);

            m_Console.IsAdminFooter = Utility.ConvertToInt(m_Console.Request.QueryString["adminFooter"]) == 1;

            //  Create new instances
            Source.GridCreation grid = new Source.GridCreation();
            Source.Component component = new Source.Component();

            m_Console.Component = component;

            m_Console.AddTrace("Monitor", "Start.HandleRequest(..)");
            //  The request is in a popup
            if (openInFrame > 0)
                //  Handle the popup layer request
                HandlePopupLayerRequest(grid, component, openInFrame, isDeleteTriggered);
            else
                //  Handle all default requests
                HandleRequest(grid, component, isDeleteTriggered);

            //  Stop the response
            m_Console.CurrentListInstance.wim.CurrentVisitor.Save();

            //if (m_Console.CurrentListInstance.wim.CurrentProfile.IsLoggedIn)
            //    m_Console.CurrentListInstance.wim.CurrentProfile.Save();

            //if (m_Console.CurrentListInstance.wim.CurrentApplicationUser.IsLoggedIn)
            //    m_Console.CurrentListInstance.wim.CurrentApplicationUser.Save(true);

            
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

                    AddToResponse(Source.Template.FolderBrowsing(this,
                        m_Console,
                        Source.Generic.Header(m_Console),
                        Source.Generic.TopContainer(m_Console),
                        Source.Generic.Footer(m_Console.IsNewDesign),
                        Source.Navigation.Leftnavigation(m_Console, null),
                        Source.Navigation.BreadcrumbNavigation(m_Console, null),
                        Source.Navigation.NewBottomNavigation(m_Console, null, false),
                        null,
                        "[controlcollection]",
                        m_Console.Title, false, 0, null
                        ));


                    if (this.IsLoadedInThinLayer) return;
                }
            }
            
            if (this.IsLoadedInThinLayer) return;

            m_Console.Response.Flush();
            m_Console.Context.Trace.Write("Parsing", "Done");
            m_Console.Response.End();
        }

        void GenerateExternalSource()
        {
            Source.External.PageTemplateConfiguration ptc = Source.External.PageTemplateConfiguration.Load();
            System.Collections.Hashtable ht = Source.External.ComponentConfiguration.Load();

            foreach (string key in ht.Keys)
            {
                string xhtmlKey = string.Format("<wim:{0} />", key);
                if (ptc.Data.Contains(xhtmlKey))
                {
                    Source.External.ComponentConfiguration cc = ht[key] as Source.External.ComponentConfiguration;

                    if (!string.IsNullOrEmpty(cc.Processing))
                    {
                        string[] x = cc.Processing.Split(',');
                        Wim.Processing.iProcessing iproces = Wim.Utility.CreateInstance(string.Concat(x[1].Trim(), ".dll"), x[0]) as Wim.Processing.iProcessing;

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
                m_Console.Response.Write(output);
        }

        public StringBuilder outputHTML;

        /// <summary>
        /// When there is no roaming application user, redirect to the homepage.
        /// </summary>
        /// <returns>Continue processing page?</returns>
        bool CheckRoamingApplicationUser()
        {
            //  Check if logout request is performed
            if (m_Console.Request.Params["logout"] == "1")
            {
                //m_Console.CurrentApplicationUser = Sushi.Mediakiwi.Data.ApplicationUser.Select();
                //  Clear roaming profile reference
                //Sushi.Mediakiwi.Data.AuditTrail.Insert(Data.AuditType.Action, Data.AuditAction.Logout, null, m_Console.CurrentApplicationUser.ID, string.Format("{0} ({1})", m_Console.CurrentApplicationUser.Displayname, m_Console.CurrentApplicationUser.Email));

     

                m_Console.CurrentVisitor.ApplicationUserID = null;
                m_Console.CurrentVisitor.Save();

                //Data.ApplicationUser.Clear();
                //Sushi.Mediakiwi.Data.Identity.Profile.Select().Logout();
                //m_Console.CurrentApplicationUser = new Sushi.Mediakiwi.Data.ApplicationUser();
                m_Console.Response.Redirect(Data.Environment.Current.RelativePath);

            }
            else
            {
                if (!string.IsNullOrEmpty(m_Console.Request.QueryString["negotiate"]))
                {
                    Guid applicationUser = Wim.Utility.ConvertToGuid(m_Console.Request.QueryString["negotiate"]);
                    m_Console.CurrentApplicationUser = Sushi.Mediakiwi.Data.ApplicationUserLogic.Apply(applicationUser, true);

                }
                else
                {
                    Guid visitor;


                    if (Wim.Utility.IsGuid(m_Console.Request.Headers["WIM-Transport-Visitor"], out visitor))
                    {
                        //Sushi.Mediakiwi.Data.Notification.InsertOne("WIM-Transport", string.Format("{0}<br/>{1}", m_Console.Request.Headers["WIM-Transport-Visitor"], m_Console.Request.Headers["WIM-Transport-Cookie"]));
                        m_Console.CurrentVisitor = Sushi.Mediakiwi.Data.Identity.Visitor.Select(visitor);
                    }

                    if (string.IsNullOrEmpty(m_Console.Request.Headers["WIM-Transport-Cookie"]))
                        m_Console.CurrentApplicationUser = Data.ApplicationUserLogic.Select();
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
                            //Sushi.Mediakiwi.Data.Notification.InsertOne("WIM-Transport", string.Format("{0}<br/>{1}", nameValue[0], nameValue[1]));

                        }

                        if (m_Console.Request.IsAuthenticated)
                            m_Console.CurrentApplicationUser = Data.ApplicationUserLogic.Select();
                        else
                            m_Console.CurrentApplicationUser = null;
                        //Sushi.Mediakiwi.Data.Notification.InsertOne("WIM-Transport", string.Format("{0}", m_Console.CurrentApplicationUser.ID));
                    }
                }
            }

            //if (!m_Console.CurrentVisitor.LastLoggedApplicationUserVisit.HasValue)
            //    isLoggedIn = false;

            //if (m_Console.Request.IsAuthenticated)
            //{
            //    int minutes = Wim.Utility.ConvertToInt(Sushi.Mediakiwi.Data.Environment.Current["EXPIRATION_COOKIE_APPUSER"], 0);
            //    if (minutes == 0) minutes = 60;

            //    //m_Console.Response.Write(m_Console.CurrentVisitor.LastLoggedApplicationUserVisit);
            //    TimeSpan lastclick = new TimeSpan(Sushi.Mediakiwi.Data.Common.DatabaseDateTime.Ticks - m_Console.CurrentVisitor.LastLoggedApplicationUserVisit.Value.Ticks);

            //    if (lastclick.TotalMinutes > minutes)
            //        isLoggedIn = false;
            //}

            //  Check roaming profile
            if (!m_Console.Request.IsAuthenticated)
            {
                AddToResponse(Source.Template.Login(m_Console, Source.Generic.Header(m_Console)));
                if (m_Console.CurrentApplicationUser.IsNewInstance)
                {
                    //  Login screen is shown, so stop response output.
                    if (this.IsLoadedInThinLayer)
                    {
                        return false;
                    }
                    else
                    {
                        m_Console.Response.Flush();
                        m_Console.Context.Trace.Write("Parsing", "Done");
                        m_Console.Response.End();
                    }
                }
                else
                {
                    //  Login screen is shown, but the user is logged in. Redirect to the correct page.
                    m_Console.Response.Redirect(Wim.Utility.GetSafeUrl(m_Console.Request));
                }
                return false;
            }
            else
                m_Console.CurrentVisitor.LastLoggedApplicationUserVisit = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
            return true;
        }

        /// <summary>
        /// Outputs the ajax request.
        /// </summary>
        /// <returns>Continue processing page?</returns>
        bool OutputAjaxRequest()
        {
            if (!string.IsNullOrEmpty(m_Console.Request.QueryString["xml"]))
            {
                m_Console.Response.ContentType = "text/xml";
                m_Console.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                m_Console.Response.Cache.SetAllowResponseInBrowserHistory(false);

                if (m_Console.Request.QueryString["xml"] == "richtext")
                    AddToResponse(Source.Xml.RichTextEditor.Get(m_Console.WimRepository, m_Console.WimPagePath, m_Console.Request.QueryString["error"] == "1"));

                if (m_Console.Request.QueryString["xml"] == "datetime")
                    AddToResponse(Source.Xml.DateTimeSelector.Get(m_Console.WimRepository));

                if (m_Console.Request.QueryString["xml"] == "gallery")
                    AddToResponse(Source.Xml.DocumentSelect.Get(m_Console.WimRepository, m_Console.WimPagePath, Utility.ConvertToInt(m_Console.Request.QueryString["id"]), Utility.ConvertToInt(m_Console.Request.QueryString["page"]), true));

                if (m_Console.Request.QueryString["xml"] == "folder")
                    AddToResponse(Source.Xml.PageSelect.Get(m_Console.WimRepository, m_Console.WimPagePath, Utility.ConvertToInt(m_Console.Request.QueryString["id"]), Utility.ConvertToInt(m_Console.Request.QueryString["page"]), true));

                if (m_Console.Request.QueryString["xml"] == "timesheet")
                    AddToResponse(Source.Xml.Timesheet.Get(m_Console, m_Console.Request.QueryString["list"]));

                if (m_Console.Request.QueryString["xml"] == "component")
                {


                    AddToResponse(Source.Xml.Component.Get(m_Console, Utility.ConvertToInt(m_Console.Request.QueryString["id"]), Utility.ConvertToInt(m_Console.Request.QueryString["page"]), Utility.ConvertToInt(m_Console.Request.QueryString["cmpt"]), null));
                }

                if (m_Console.Request.QueryString["xml"] == "delete")
                {
                    var c = Sushi.Mediakiwi.Data.ComponentVersion.SelectOne(Utility.ConvertToInt(m_Console.Request.QueryString["id"]));
                    c.Delete();
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

            m_Console.Group = Wim.Utility.ConvertToIntNullable(m_Console.Request.QueryString["group"]);
            m_Console.GroupItem = Wim.Utility.ConvertToIntNullable(m_Console.Request.QueryString["groupitem"]);

            //  Verify page request
            m_Console.Item = Wim.Utility.ConvertToIntNullable(m_Console.Request.QueryString["page"], false);
            if (m_Console.Item.HasValue)
            {
                m_Console.ItemType = RequestItemType.Page;
                return;
            }
            //  Verify asset request
            m_Console.Item = Wim.Utility.ConvertToIntNullable(m_Console.Request.QueryString["asset"], false);
            if (m_Console.Item.HasValue)
            {
                m_Console.ItemType = RequestItemType.Asset;
                return;
            }
            //  Verify asset request
            m_Console.Item = Wim.Utility.ConvertToIntNullable(m_Console.Request.QueryString["dashboard"], false);
            if (m_Console.Item.HasValue)
            {
                m_Console.ItemType = RequestItemType.Dashboard;
                return;
            }
            //  Verify list-item request
            m_Console.Item = Wim.Utility.ConvertToIntNullable(m_Console.Request.QueryString["item"], false);
            if (m_Console.Item.HasValue)
            {
                m_Console.ItemType = RequestItemType.Item;
            }
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
            m_Console.CurrentListInstance.wim.IsEditMode =
                   m_Console.Request.Form["state"] == "1" //    The edit link is clicked (see source/navigation.cs)
                || m_Console.Request.Form["state"] == "2" //    The save link is shown (see source/navigation.cs)
                || m_Console.Request.Form["state1"] == "2" //    The save link is shown (see source/navigation.cs)
                || m_Console.Request.QueryString["item"] == "0" //  a new item is requested
                || m_Console.Request.QueryString["asset"] == "0" //  a new asset is requested
                || m_Console.CurrentListInstance.wim.OpenInEditMode //  a forced editmode request
                || m_Console.CurrentListInstance.wim.PostbackValue.Equals("edit", StringComparison.InvariantCultureIgnoreCase)
                || m_Console.JsonReferrer.Equals("edit");
                ;

            //  Create new page
            if (!m_Console.CurrentListInstance.wim.IsEditMode && m_Console.ItemType == RequestItemType.Page && m_Console.Request.UrlReferrer != null && m_Console.Request.UrlReferrer.ToString().Contains("item=0"))
            {
                m_Console.CurrentListInstance.wim.IsEditMode = true;
            }

            //  Is the save link clicked?
            m_Console.CurrentListInstance.wim.IsSaveMode = 
                m_Console.Request.Form["save"] == "1" 
                || m_Console.Request.Form["saveNew"] == "1"
                || m_Console.Request.Form["save1"] == "1"
                || m_Console.Request.Form["saveNew1"] == "1";

            //  Is the delete link clicked?
            isDeleteMode = (m_Console.Request.Form["delete"] == "1" || m_Console.CurrentListInstance.wim.PostbackValue.Equals("delete", StringComparison.InvariantCultureIgnoreCase));
            m_Console.CurrentListInstance.wim.IsDeleteMode = isDeleteMode;
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
                    if (m_Console.CurrentListInstance.wim.CurrentApplicationUserRole.Sites(m_Console.CurrentApplicationUser) != null && m_Console.CurrentListInstance.wim.CurrentApplicationUserRole.Sites(m_Console.CurrentApplicationUser).Length > 0)
                        m_Console.Response.Redirect(m_Console.GetWimPagePath(m_Console.CurrentListInstance.wim.CurrentApplicationUserRole.Sites(m_Console.CurrentApplicationUser)[0].ID));
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
        void HandleRequest(Source.GridCreation grid, Source.Component component, bool isDeleteTriggered)
        {
            this.HandleActionRequest();
            bool x = false;
            if (!Wim.CommonConfiguration.NEW_NAVIGATION)
            {
                if (m_Console.ItemType == 0 &&
                    m_Console.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.Browsing &&
                    m_Console.CurrentListInstance.wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.List
                    )
                {
                    Sushi.Mediakiwi.Data.IComponentList[] lists = Sushi.Mediakiwi.Data.ComponentList.SelectAll(m_Console.CurrentListInstance.wim.CurrentFolder.ID);
                    foreach (var list in lists)
                    {
                        if (list.IsVisible && list.HasRoleAccess(m_Console.CurrentApplicationUser))
                        {
                            Source.UrlBuilder build = new Sushi.Mediakiwi.Beta.GeneratedCms.Source.UrlBuilder(m_Console);
                            m_Console.Response.Redirect(build.GetListRequest(list), true);
                            //m_Console.ApplyList(list);
                            //m_Console.Item = 0;
                            //m_Console.ItemType = RequestItemType.Item;

                            break;
                        }
                    }
                }
            }
            
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
            else if (m_Console.CurrentListInstance.wim.CurrentFolder.ID == 0)
            {

                //  [20091011:MM] Check if the list is part of a different folder. If so, redirect to this environment.
                if (m_Console.CurrentList.FolderID.HasValue && m_Console.CurrentList.Type != Sushi.Mediakiwi.Data.ComponentListType.Browsing)
                {
                    Sushi.Mediakiwi.Data.Folder candidate = Sushi.Mediakiwi.Data.Folder.SelectOne(m_Console.CurrentList.FolderID.Value);
                    if (candidate.SiteID != m_Console.ChannelIndentifier)
                    {
                        string url = m_Console.UrlBuild.GetListRequest(m_Console.CurrentList, Wim.Utility.ConvertToIntNullable(m_Console.Request.QueryString["item"]));
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
        void HandleListItemRequest(Source.GridCreation grid, Source.Component component, bool isDeleteTriggered)
        {
            m_Console.View = (int)ContainerView.ItemSelect;

            if (m_Console.CurrentList.Type == Data.ComponentListType.ListSettings)
                m_Console.View = (int)ContainerView.ListSettingRequest;

            m_Console.AddTrace("Monitor", "GetExportOptionUrl(..)");
            GetExportOptionUrl(grid, component);

            //  Create the form

            if (!m_Console.IsComponent)
                this.GlobalWimControlBuilder = component.CreateList(m_Console, 0);

            bool isCopyTriggered = m_Console.Context.Request["copyparent"] == "1";

            if (isCopyTriggered)
            {
                //
                //m_Console.CurrentListInstance.DoListDelete(m_Console.Item.GetValueOrDefault(0), 0);
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
                version.ComponentListItemID = m_Console.Item.Value;
                version.ApplicationUserID = m_Console.CurrentApplicationUser.ID;
                version.TypeID = 2;
                version.Save();

                
                //  Redirect to the containing folder
                if (m_Console.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.Documents)
                    m_Console.Response.Redirect(string.Concat(m_Console.WimPagePath, "?gallery=", m_Console.CurrentListInstance.wim.CurrentFolder.ID));
                else if (m_Console.CurrentListInstance.wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Gallery)
                    m_Console.Response.Redirect(string.Concat(m_Console.WimPagePath, "?gallery=", m_Console.CurrentListInstance.wim.CurrentFolder.ParentID));
                else if (m_Console.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.Folders)
                    m_Console.Response.Redirect(string.Concat(m_Console.WimPagePath, "?folder=", m_Console.CurrentListInstance.wim.CurrentFolder.ParentID));
                else if (m_Console.Group.HasValue)
                    m_Console.Response.Redirect(string.Concat(m_Console.WimPagePath, "?", m_Console.CurrentListInstance.wim.SearchResultItemPassthroughParameter, "=0"), true);
                    //m_Console.Response.Redirect(string.Concat(m_Console.WimGroupPagePath, "&list=", m_Console.CurrentList.ID, "&item=0"));
                else if (m_Console.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.ComponentListProperties)
                    m_Console.Response.Redirect(string.Concat(m_Console.WimPagePath, "?folder=", m_Console.CurrentListInstance.wim.CurrentFolder.ID));
                else
                    m_Console.Response.Redirect(string.Concat(m_Console.WimPagePath, "?list=", m_Console.CurrentList.ID));
                //else
                //    m_Console.Response.Redirect(string.Concat(m_Console.WimPagePath, "?folder=", m_Console.CurrentListInstance.wim.CurrentFolder.Id));
            }



            if (m_Console.IsComponent)
            {
                m_Console.CurrentListInstance.wim.DoListLoad(m_Console.Item.GetValueOrDefault(), 0);

                bool isPagePublishTriggered = m_Console.Request.Form["pagepublish"] == "1";
                bool isPageOfflineTriggered = m_Console.Request.Form["pageoffline"] == "1";

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

                AddToResponse(Source.Template.FolderBrowsing(this,
                        m_Console,
                        Source.Generic.Header(m_Console),
                        Source.Generic.TopContainer(m_Console),
                        Source.Generic.Footer(m_Console.IsNewDesign),
                        Source.Navigation.Leftnavigation(m_Console),
                        Source.Navigation.BreadcrumbNavigation(m_Console, null),
                        Source.Navigation.NewBottomNavigation(m_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null, false),
                        null,
                        "[controlcollection]",
                        m_Console.Title, false, 0, null
                        ));
                return;
            }

            if (m_Console.IsAdminFooter)
            {
                AddToResponse(Source.Template.OpenInFrame(
                    m_Console,
                    Source.Generic.Header(m_Console),
                    "[controlcollection]"
                    , Source.Navigation.BreadcrumbNavigation(m_Console, component.m_ButtonList == null ? null : component.m_ButtonList.ToArray()),
                    null,
                    1, false, Source.Generic.Footer(m_Console.IsNewDesign)
                    ));
            }
            else
            {
                //  Output the list item form
                AddToResponse(Source.Template.FolderBrowsing(this,
                    m_Console,
                    Source.Generic.Header(m_Console),
                    Source.Generic.TopContainer(m_Console),
                    Source.Generic.Footer(m_Console.IsNewDesign),
                    Source.Navigation.Leftnavigation(m_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null),
                    Source.Navigation.BreadcrumbNavigation(m_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null),
                    Source.Navigation.NewBottomNavigation(m_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null, false),
                    null,
                    "[controlcollection]",
                    m_Console.Title, false, 0, null
                    ));
            }
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
        void HandlePageItemRequest(Source.GridCreation grid, Source.Component component, bool isDeleteTriggered)
        {
            m_Console.View = 0;

           

            bool isPagePublishTriggered = m_Console.Request.Form["pagepublish"] == "1";
            bool isPageOfflineTriggered = m_Console.Request.Form["pageoffline"] == "1";
           
      

            bool isPageLocalised = m_Console.Request.Form["page.localize"] == "1";
            bool isPageInherited = m_Console.Request.Form["page.inherit"] == "1";

            int selectedTab = Utility.ConvertToInt(m_Console.Request.QueryString["tab"]);
            string section = m_Console.Request.QueryString["tab"];

            if (string.IsNullOrEmpty(section))
            {
                if (m_Console.CurrentPage.Template.Data.HasProperty("TAB.INFO"))
                {
                    var sections = m_Console.CurrentPage.Template.Data["TAB.INFO"].Value.Split(',');
                    if (sections.Length > 0)
                        section = sections[0];
                }
            }

            string redirect = string.IsNullOrEmpty(section) ? "" : string.Concat("&tab=", section);
            if (m_Console.IsNotFramed)
                redirect += string.Concat("&nf=", m_Console.Request.QueryString["nf"]);

            if (m_Console.Request.Form["page.translate"] == "1")
            {
                m_Console.CurrentApplicationUser.ShowTranslationView = true;
                m_Console.CurrentApplicationUser.Save();

                if (!m_Console.CurrentListInstance.IsEditMode)
                    m_Console.Response.Redirect(string.Concat(m_Console.WimPagePath, "?page=", m_Console.Item.Value, redirect));
            }
            if (m_Console.Request.Form["page.copy"] == "1")
            {
                Sushi.Mediakiwi.Data.ComponentVersionLogic.CopyFromMaster(m_Console.Item.Value);

                if (!m_Console.CurrentListInstance.IsEditMode)
                    m_Console.Response.Redirect(string.Concat(m_Console.WimPagePath, "?page=", m_Console.Item.Value, redirect));
            }
            if (m_Console.Request.Form["page.normal"] == "1")
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
                page.InheritContent = false;
                page.Save();

                Sushi.Mediakiwi.Framework2.Functions.AuditTrail.Insert(m_Console.CurrentApplicationUser, page, Framework2.Functions.Auditing.ActionType.Localised, null);
                m_Console.Response.Redirect(string.Concat(m_Console.WimPagePath, "?page=", m_Console.Item.Value, redirect));
                
            }
            else if (isPageInherited)
            {
                Sushi.Mediakiwi.Data.Page page = Sushi.Mediakiwi.Data.Page.SelectOne(m_Console.Item.Value, false);
                page.InheritContent = true;
                page.Save();
                Sushi.Mediakiwi.Framework2.Functions.AuditTrail.Insert(m_Console.CurrentApplicationUser, page, Framework2.Functions.Auditing.ActionType.Inherited, null);
                m_Console.Response.Redirect(string.Concat(m_Console.WimPagePath, "?page=", m_Console.Item.Value, redirect));
                
            }

            Data.Page pageInstance;

         
            //if (m_Console.IsNotFramed)
            //    m_Console.CurrentListInstance.wim.ShowInFullWidthMode = true;

            this.GlobalWimControlBuilder = component.CreateContentList(m_Console, 0, selectedTab == 1, out pageInstance, section);

            if (m_Console.IsAdminFooter)
            {
                AddToResponse(Source.Template.OpenInFrame(
                    m_Console,
                    Source.Generic.Header(m_Console),
                    "[controlcollection]"
                    , Source.Navigation.BreadcrumbNavigation(m_Console, component.m_ButtonList == null ? null : component.m_ButtonList.ToArray()),
                    null,
                    1, false, Source.Generic.Footer(m_Console.IsNewDesign)
                    ));
            }
            else
            {
                if (m_Console.IsNotFramed)
                {
                    AddToResponse(Source.Template.FolderBrowsing(this,
                                           m_Console,
                                           Source.Generic.Header(m_Console),
                                           null,//Source.Generic.TopContainer(m_Console),
                                           Source.Generic.Footer(m_Console.IsNewDesign),
                                           null,//Source.Navigation.Leftnavigation(m_Console),
                                           Source.Navigation.BreadcrumbNavigation(m_Console, null),
                                           Source.Navigation.NewBottomNavigation(m_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null, false),
                                           null,
                                           "[controlcollection]",
                                           pageInstance.Name, pageInstance.Template.HasSecundaryContentContainer, selectedTab, null
                                           ));
                }
                else
                {
                    AddToResponse(Source.Template.FolderBrowsing(this,
                            m_Console,
                            Source.Generic.Header(m_Console),
                            Source.Generic.TopContainer(m_Console),
                            Source.Generic.Footer(m_Console.IsNewDesign),
                            Source.Navigation.Leftnavigation(m_Console),
                            Source.Navigation.BreadcrumbNavigation(m_Console, null),
                            Source.Navigation.NewBottomNavigation(m_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null, false),
                            null,
                            "[controlcollection]",
                            pageInstance.Name, pageInstance.Template.HasSecundaryContentContainer, selectedTab, null
                            ));
                }
            }
        }

        internal Sushi.Mediakiwi.Framework.WimControlBuilder GlobalWimControlBuilder;

        void GetExportOptionUrl(Source.GridCreation grid, Source.Component component)
        {
            string exportUrl = null;
            //  Export to XLS: XLS Creation URL
            if (m_Console.Request.Form["export_xls"] == "1")
            {
                m_Console.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                m_Console.Response.Cache.SetAllowResponseInBrowserHistory(false);

                exportUrl = Wim.Utility.GetSafeUrl(m_Console.Request);

                m_Console.CurrentListInstance.wim.IsExportMode_XLS = true;
                component.CreateSearchList(m_Console, 0);
                var url = grid.GetGridFromListInstanceExport(m_Console.CurrentListInstance.wim, m_Console, 0);

                if (m_Console.Request.QueryString["xp"] == "1")
                {
                    m_Console.Response.Write(url);
                    m_Console.Response.End();
                }
                else
                    m_Console.Response.Redirect(url);
                //  Reset
                m_Console.CurrentListInstance.wim.IsExportMode_XLS = false;


            }
            //  Export to PDF: PDF Creation URL
            //if (m_Console.Request.Form["export_pdf"] == "1")
            //{
            //    m_Console.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //    m_Console.Response.Cache.SetAllowResponseInBrowserHistory(false);

            //    string downloadName = string.Concat(m_Console.CurrentList.Name.Replace(" ", "_"), "_", DateTime.Now.ToString("yyyyMMddmmHH"), ".pdf");

            //    string portInfo = null;
            //    if (m_Console.Request.Url.Port != 80)
            //        portInfo = string.Concat(":", m_Console.Request.Url.Port);

            //    string url = string.Concat(m_Console.WimPageLocalPath, "?", m_Console.Request.QueryString.ToString()).ToLower();


            //    ExpertPdf.HtmlToPdf.PdfConverter pdfConverter = new ExpertPdf.HtmlToPdf.PdfConverter();

            //    pdfConverter.PdfDocumentOptions.PdfPageSize = ExpertPdf.HtmlToPdf.PdfPageSize.A4;
            //    pdfConverter.PdfDocumentOptions.PdfCompressionLevel = ExpertPdf.HtmlToPdf.PdfCompressionLevel.NoCompression;
            //    pdfConverter.PdfDocumentOptions.LeftMargin = 10;
            //    pdfConverter.PdfDocumentOptions.RightMargin = 10;
            //    pdfConverter.PdfDocumentOptions.TopMargin = 15;
            //    pdfConverter.PdfDocumentOptions.BottomMargin = 25;
            //    pdfConverter.PdfDocumentOptions.GenerateSelectablePdf = true;
            //    pdfConverter.PdfDocumentOptions.ShowHeader = false;
            //    pdfConverter.LicenseKey = "DRn9DJyiH9zkyQWeYbDv04e9xbw/eBrOTteAXg6cj3EpqLgbCIh6I9MOOYlMQnkN";

            //    string channelParam = string.Concat("channel=", m_Console.CurrentListInstance.wim.CurrentSite.ID, "&");
            //    if (url.Contains(channelParam))
            //        url = url.Replace(channelParam, string.Empty);

            //    if (url.Contains("?"))
            //    {
            //        if (url.Contains("&set="))
            //            url = string.Concat(url, "&openinframe=1&formatpdf=1");
            //        else
            //            url = string.Concat(url, "&set=all&openinframe=1&formatpdf=1");
            //    }
            //    else
            //        url = string.Concat(url, "?openinframe=1&set=all&formatpdf=1");

            //    string html = null;
            //    if (!string.IsNullOrEmpty(Wim.CommonConfiguration.HTTP_IMPERSONATION_USER) && !string.IsNullOrEmpty(Wim.CommonConfiguration.HTTP_IMPERSONATION_PASSWORD))
            //    {
            //        NetworkCredential credentials =
            //            new NetworkCredential(Wim.CommonConfiguration.HTTP_IMPERSONATION_USER, Wim.CommonConfiguration.HTTP_IMPERSONATION_PASSWORD);
            //        html = Wim.Utilities.WebScrape.GetUrlResponse(url, credentials);
            //    }
            //    else
            //        html = Wim.Utilities.WebScrape.GetUrlResponse(url);

            //    System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex("<form.*?>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            //    html = rex.Replace(html.Replace("</form>", string.Empty), string.Empty);

            //    string folder = m_Console.Request.MapPath(Wim.Utility.AddApplicationPath(Wim.CommonConfiguration.RelativeRepositoryTmpUrl));
            //    if (!System.IO.Directory.Exists(folder)) 
            //        System.IO.Directory.CreateDirectory(folder);

            //    string path = m_Console.CurrentListInstance.wim.GetTemporaryFilePath(downloadName);
            //    string baseUrl = string.Concat(Wim.Utilities.WebScrape.Host, HttpContext.Current.Request.ApplicationPath == "/" ? string.Empty : HttpContext.Current.Request.ApplicationPath, "/repository/");

            //    if (!string.IsNullOrEmpty(Wim.CommonConfiguration.HTTP_IMPERSONATION_URL))
            //        baseUrl = string.Concat(Wim.CommonConfiguration.HTTP_IMPERSONATION_URL, HttpContext.Current.Request.ApplicationPath == "/" ? string.Empty : HttpContext.Current.Request.ApplicationPath, "/repository/");

            //    //html = html
            //    //    .Replace(@"type=""text/css"" href=""../repository/", string.Format(@"type=""text/css"" href=""{0}", baseUrl))
            //    //    .Replace(string.Format(@"rel=""stylesheet"" href=""{0}/repository/", HttpContext.Current.Request.ApplicationPath == "/" ? string.Empty : HttpContext.Current.Request.ApplicationPath), string.Format(@"rel=""stylesheet"" href=""{0}", baseUrl));
            //    //Sushi.Mediakiwi.Data.Notification.InsertOne("pdf export", string.Format("URL: {0}<br/>path: {1}<br/>html {2}", baseUrl, path, html));

            //    pdfConverter.SavePdfFromHtmlStringToFile(html, path, baseUrl);

            //    //m_Console.Response.Write(html);
            //    //m_Console.Response.Write("http://localhost/demo//" + "<br/>");
            //    //m_Console.Response.Write(baseUrl);
            //    //m_Console.Response.End();
            //}
        }

        bool IsFormatRequest_JSON { get { return m_Console.Request.Params[Wim.UI.Constants.JSON_PARAM] == "1"; } }
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
        /// Handles the browsing request.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="component">The component.</param>
        void HandleBrowsingRequest(Source.GridCreation grid, Source.Component component)
        {
            m_Console.AddTrace("Monitor", "HandleListItemRequest.Init");
            m_Console.View = 2;
            m_Console.CurrentListInstance.wim.IsEditMode = true;

            m_Console.AddTrace("Monitor", "GetExportOptionUrl(..)");
            GetExportOptionUrl(grid, component);

            m_Console.AddTrace("Monitor", "CreateSearchList(..)");

            string searchList = component.CreateSearchList(m_Console, 0).ToString();
            string searchListGrid;
            if (m_Console.CurrentApplicationUser.ShowDetailView || m_Console.CurrentList.Type != Sushi.Mediakiwi.Data.ComponentListType.Browsing)
            {
                m_Console.AddTrace("Monitor", "GetGridFromListInstance(..)");
                //|| m_Console.CurrentListInstance.wim.CurrentFolder.Type != Sushi.Mediakiwi.Data.FolderType.Gallery)

                Sushi.Mediakiwi.Framework.UI.DataGrid gridNew = new Framework.UI.DataGrid();
                if (this.IsFormatRequest_JSON)
                {
                    m_Console.Response.Clear();
                    m_Console.Response.ContentType = "application/json";
                    searchListGrid = gridNew.GetGridFromListInstanceForJSON(m_Console.CurrentListInstance.wim, m_Console, 0, false, IsNewDesignOutput);
                    m_Console.Response.Write(searchListGrid);
                    m_Console.CurrentVisitor.Save();
                    m_Console.Response.End();
                }
                //if (m_Console.CurrentListInstance.wim.Grid.IsAsyncRequestTemplateRequired)
                //    searchListGrid = gridNew.GetGridFromListInstanceForKnockout(m_Console.CurrentListInstance.wim, m_Console, 0, false, IsNewDesignOutput, false);
                //else
                //{
                    //m_Console.CurrentListInstance.wim.CheckListDataColumnBackup();
                    searchListGrid = null; // grid.GetGridFromListInstance(m_Console.CurrentListInstance.wim, m_Console, 0, false, IsNewDesignOutput);
                    while (m_Console.CurrentListInstance.wim.NextGrid())
                    {
                        searchListGrid +=
                            string.Concat(string.IsNullOrEmpty(m_Console.CurrentListInstance.wim.m_DataTitle) ? null : string.Format("<h2>{0}</h2>", m_Console.CurrentListInstance.wim.m_DataTitle),
                            grid.GetGridFromListInstance(m_Console.CurrentListInstance.wim, m_Console, 0, false, IsNewDesignOutput));
                    }
                //}

                if (!string.IsNullOrEmpty(component.m_ClickedButton) && m_Console.CurrentListInstance.wim.HasListSearchedAction)
                {
                    m_Console.CurrentListInstance.wim.DoListSearchedAction(m_Console.Item.GetValueOrDefault(0), 0, component.m_ClickedButton, null);
                }

                m_Console.CurrentListInstance.wim.SendReport(searchListGrid);
            }
            else
            {
                m_Console.AddTrace("Monitor", "GetThumbnailGridFromListInstance(..)");
                searchListGrid = grid.GetThumbnailGridFromListInstance(m_Console.CurrentListInstance.wim, m_Console, 0, false);
            }

            m_Console.AddTrace("Monitor", "AddToResponse(..)");

            string bottom = Source.Navigation.NewBottomNavigation(m_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null, !string.IsNullOrEmpty(searchList));

            if (IsNewDesignOutput)
            {
                //var m = new Sushi.Mediakiwi.Framework.PresentationLayer.Templates.Monitor();
                //AddToResponse(m.GetTemplateWrapper(searchListGrid));
            }
            else
            {
                AddToResponse(Source.Template.FolderBrowsing(this,
                    m_Console,
                    Source.Generic.Header(m_Console),
                    Source.Generic.TopContainer(m_Console),
                    Source.Generic.Footer(m_Console.IsNewDesign),
                    Source.Navigation.Leftnavigation(m_Console, component.m_ButtonList == null ? null : component.m_ButtonList.ToArray()),
                    Source.Navigation.BreadcrumbNavigation(m_Console, component.m_ButtonList == null ? null : component.m_ButtonList.ToArray()),
                    searchList + bottom,
                    searchListGrid,
                    m_Console.Title, false, null
                     ));
            }
        }

        /// <summary>
        /// Handles the dashboard request.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="component">The component.</param>
        void HandleDashboardRequest(Source.GridCreation grid, Source.Component component)
        {
            int dashboardID = Wim.Utility.ConvertToInt(m_Console.Request.QueryString["dashboard"], m_Console.CurrentListInstance.wim.CurrentApplicationUserRole.Dashboard);
            Data.Dashboard dashboard = Data.Dashboard.SelectOne(dashboardID);

            StringBuilder target1 = new StringBuilder();
            StringBuilder target1a = new StringBuilder();
            StringBuilder target2 = new StringBuilder();
            StringBuilder target3 = new StringBuilder();

            int row = 0;
            bool isFirst = true;

//            foreach (var list in dashboard.DashboardTarget1)
//            {
//                if (list.HasRoleAccess(m_Console.CurrentApplicationUser))
//                {
//                    if (!list.SiteID.HasValue || list.SiteID.GetValueOrDefault() == m_Console.ChannelIndentifier)
//                        target1a.AppendFormat("\n\t<li><a href=\"{0}?list={1}\" class=\"list{3}\">{2}</a></li>", m_Console.WimPagePath, list.ID, list.Name, isFirst ? " first" : null);
//                    else
//                        target1a.AppendFormat("\n\t<li><a href=\"{0}?list={1}\" class=\"list{3}\">{2}</a></li>", m_Console.GetWimPagePath(list.SiteID.GetValueOrDefault()), list.ID, list.Name, isFirst ? " first" : null);
//                }
//                isFirst = false;
//            }

//            if (target1a.Length > 0)
//            {
//                row++;
//                target1.AppendFormat(@"
//<div class=""hgt_0"">
//    <h2 class=""portalWindowTitle"">Quick links</h2>
//        <div class=""portalWindowBorder noscroll"">
//            <ul class=""subNavigationElse"">{0}
//            </ul>
//        </div>
//</div>
//"
//                    , target1a.ToString()
//                    );
//            }

            
//            foreach (var list in dashboard.DashboardTarget2)
//            {
//                if (!list.HasRoleAccess(m_Console.CurrentApplicationUser))
//                    continue;

//                m_Console.ApplyList(list);
//                m_Console.CurrentListInstance.wim.IsDashboardMode = true;
//                m_Console.View = 3;
//                m_Console.CurrentListInstance.wim.IsEditMode = true;

//                if (m_Console.CurrentListInstance.wim.DashBoardElementIsVisible)
//                {
//                    string searchList = m_Console.CurrentListInstance.wim.DashBoardShowFilterSection ? component.CreateSearchList(m_Console, m_Console.View).ToString() : null;
                    
//                    if (m_Console.CurrentListInstance.wim.DashBoardElementIsVisible)
//                    {
//                        string url;
//                        if (!list.SiteID.HasValue || list.SiteID.GetValueOrDefault() == m_Console.ChannelIndentifier)
//                            url = string.Concat(m_Console.WimPagePath, "?list=", list.ID);
//                        else
//                            url = string.Concat(m_Console.GetWimPagePath(list.SiteID.GetValueOrDefault()), "?list=", list.ID);


//                        if (m_Console.CurrentListInstance.wim.DashBoardCanClickThrough && m_Console.CurrentListInstance.wim.CurrentList.IsVisible)
//                        {
//                            target1.AppendFormat(@"
//<div class="" hgt_0"">
//    <h2 class=""portalWindowTitle""><a href=""{3}"">{2}</a></h2>
//        <div class=""portalWindowBorder noscroll"">{4}{5}
//            {0}
//        </div>
//</div>
//"
//                                    , grid.GetGridFromListInstance(m_Console.CurrentListInstance.wim, m_Console, 4, IsNewDesignOutput)
//                                    , row
//                                    , m_Console.CurrentListInstance.wim.ListTitle
//                                    , url
//                                    , m_Console.CurrentListInstance.wim.DashBoardTopHmlContainer
//                                    , searchList
//                                );
//                        }
//                        else
//                        {
//                            target1.AppendFormat(@"
//<div class="" hgt_0"">
//    <h2 class=""portalWindowTitle""><a>{2}</a></h2>
//        <div class=""portalWindowBorder noscroll"">{4}{5}
//            {0}
//        </div>
//</div>
//"
//                                    , grid.GetGridFromListInstance(m_Console.CurrentListInstance.wim, m_Console, 4, IsNewDesignOutput)
//                                    , row
//                                    , m_Console.CurrentListInstance.wim.ListTitle
//                                    , url
//                                    , m_Console.CurrentListInstance.wim.DashBoardTopHmlContainer
//                                    , searchList
//                                );
//                        }

//                        row++;
//                    }
//                }
//            }

//            row = 0;

//            foreach (var list in dashboard.DashboardTarget3)
//            {
//                if (!list.HasRoleAccess(m_Console.CurrentApplicationUser))
//                    continue;

//                m_Console.ApplyList(list);
//                m_Console.CurrentListInstance.wim.IsDashboardMode = true;
//                m_Console.View = 3;
//                m_Console.CurrentListInstance.wim.IsEditMode = true;

//                if (m_Console.CurrentListInstance.wim.DashBoardElementIsVisible)
//                {
//                    string searchList = component.CreateSearchList(m_Console, m_Console.View).ToString();

//                    if (!m_Console.CurrentListInstance.wim.DashBoardShowFilterSection)
//                        searchList = null;

//                    searchList += m_Console.CurrentListInstance.wim.XHtmlDataTop;

//                    //  Possibly set in search
//                    if (m_Console.CurrentListInstance.wim.DashBoardElementIsVisible)
//                    {
//                        string url;
//                        if (!list.SiteID.HasValue || list.SiteID.GetValueOrDefault() == m_Console.ChannelIndentifier)
//                            url = string.Concat(m_Console.WimPagePath, "?list=", list.ID);
//                        else
//                            url = string.Concat(m_Console.GetWimPagePath(list.SiteID.GetValueOrDefault()), "?list=", list.ID);

//                        if (m_Console.CurrentListInstance.wim.DashBoardCanClickThrough && m_Console.CurrentListInstance.wim.CurrentList.IsVisible)
//                        {
//                            target2.AppendFormat(@"
//<div class="" hgt_{6}"">
//<h1 class=""portalWindowTitle""><a href=""{3}"">{2}</a></h1>
//    <div class=""portalWindowBorder{4}"">{5}
//        {0}
//    </div>
//</div>
//"
//                                , grid.GetGridFromListInstance(m_Console.CurrentListInstance.wim, m_Console, 3, IsNewDesignOutput), row, m_Console.CurrentListInstance.wim.ListTitle
//                                , url
//                                , m_Console.CurrentListInstance.wim.DashBoardElementHeight == 0 ? " noscroll" : null
//                                , searchList 
//                                , m_Console.CurrentListInstance.wim.DashBoardElementHeight
//                                );
//                        }
//                        else
//                        {
//                            target2.AppendFormat(@"
//<div class="" hgt_{6}"">
//<h1 class=""portalWindowTitle""><a>{2}</a></h1>
//    <div class=""portalWindowBorder{4}"">{5}
//        {0}
//    </div>
//</div>
//"
//                                ,
//                                grid.GetGridFromListInstance(m_Console.CurrentListInstance.wim, m_Console, 3, IsNewDesignOutput), row, m_Console.CurrentListInstance.wim.ListTitle
//                                , url
//                                , m_Console.CurrentListInstance.wim.DashBoardElementHeight == 0 ? " noscroll" : null
//                                , searchList
//                                , m_Console.CurrentListInstance.wim.DashBoardElementHeight
//                                );
//                        }

//                        row++;
//                    }
//                }
//            }

//            //target2.Insert(0, intro);

//            row = 0;
//            foreach (var list in dashboard.DashboardTarget4)
//            {
//                if (!list.HasRoleAccess(m_Console.CurrentApplicationUser))
//                    continue;

//                m_Console.ApplyList(list);
//                m_Console.CurrentListInstance.wim.IsDashboardMode = true;
//                m_Console.View = 3;
//                m_Console.CurrentListInstance.wim.IsEditMode = true;

//                if (m_Console.CurrentListInstance.wim.DashBoardElementIsVisible)
//                {
//                    string searchList = m_Console.CurrentListInstance.wim.DashBoardShowFilterSection ? component.CreateSearchList(m_Console, m_Console.View).ToString() : null;

//                    string url;
//                    if (!list.SiteID.HasValue || list.SiteID.GetValueOrDefault() == m_Console.ChannelIndentifier)
//                        url = string.Concat(m_Console.WimPagePath, "?list=", list.ID);
//                    else
//                        url = string.Concat(m_Console.GetWimPagePath(list.SiteID.GetValueOrDefault()), "?list=", list.ID);

//                    if (m_Console.CurrentListInstance.wim.DashBoardCanClickThrough && m_Console.CurrentListInstance.wim.CurrentList.IsVisible)
//                    {
//                        //portalWindow hgt_0 
//                        target3.AppendFormat(@"
//<div class=""updates"">
//    <h2 class=""portalWindowTitle""><a href=""{3}"">{2}</a></h2>
//        <div class=""portalWindowBorder noscroll"">{4}
//            {0}
//        </div>
//</div>
//"
//                            , grid.GetGridFromListInstance(m_Console.CurrentListInstance.wim, m_Console, 4, IsNewDesignOutput)
//                            , row
//                            , m_Console.CurrentListInstance.wim.ListTitle
//                            , url
//                            , searchList
//                            );
//                    }
//                    else
//                    {
//                        target3.AppendFormat(@"
//<div class=""updates"">
//    <h2 class=""portalWindowTitle""><a>{2}</a></h2>
//        <div class=""portalWindowBorder noscroll"">{4}
//            {0}
//        </div>
//</div>
//"
//                            , grid.GetGridFromListInstance(m_Console.CurrentListInstance.wim, m_Console, 4, IsNewDesignOutput)
//                            , row
//                            , m_Console.CurrentListInstance.wim.ListTitle
//                            , url
//                            , searchList
//                            );
//                    }

//                    row++;
//                }
//            }

            AddToResponse(Source.Template.Dashboard3(
                m_Console,
                Source.Generic.Header(m_Console),
                Source.Generic.TopContainer(m_Console),
                Source.Generic.Footer(m_Console.IsNewDesign),
                target1.ToString(),
                target2.ToString(),
                target3.ToString()
                ));
        }

        /// <summary>
        /// Handle the popup layer request
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="component">The component.</param>
        /// <param name="openInFrame">The open in frame.</param>
        /// <param name="isDeleteTriggered">if set to <c>true</c> [is delete triggered].</param>
        void HandlePopupLayerRequest(Source.GridCreation grid, Source.Component component, int openInFrame, bool isDeleteTriggered)
        {
            //  In a popup the state is always in editmode
            m_Console.CurrentListInstance.wim.IsEditMode = true;

            m_Console.AddTrace("Monitor", "GetExportOptionUrl(..)");
            GetExportOptionUrl(grid, component);

            m_Console.CurrentListInstance.wim.IsSubSelectMode = true;

            if (isDeleteTriggered && m_Console.CurrentListInstance.wim.HasListDelete)
            {
                m_Console.CurrentListInstance.wim.DoListDelete(m_Console.Item.GetValueOrDefault(0), 0, null);

                //  Add deletion entry
                Sushi.Mediakiwi.Data.ComponentListVersion version = new Sushi.Mediakiwi.Data.ComponentListVersion();
                version.SiteID = m_Console.CurrentListInstance.wim.CurrentSite.ID;
                version.ComponentListID = m_Console.CurrentListInstance.wim.CurrentList.ID;
                version.ComponentListItemID = m_Console.Item.Value;
                version.ApplicationUserID = m_Console.CurrentApplicationUser.ID;
                version.TypeID = 2;
                version.Save();

                if (string.IsNullOrEmpty(m_Console.CurrentListInstance.wim.OnDeleteScript))
                {
                    //  Do something
                }

                ////  Redirect to the containing folder
                //    m_Console.Response.Redirect(string.Concat(m_Console.WimPagePath, "?list=", m_Console.CurrentList.ID));
            }

            //  If there is an item request or the list can only contain one item, open the forms editmode
            if (m_Console.Item.HasValue || m_Console.CurrentListInstance.wim.CanContainSingleInstancePerDefinedList)
            {
                this.GlobalWimControlBuilder = component.CreateList(m_Console, openInFrame);

                if (!string.IsNullOrEmpty(m_Console.CurrentListInstance.wim.OnDeleteScript))
                {
                    this.GlobalWimControlBuilder.Append(m_Console.CurrentListInstance.wim.OnDeleteScript);
                }

                AddToResponse(Source.Template.OpenInFrame(
                    m_Console,
                    Source.Generic.Header(m_Console),
                    "[controlcollection]", Source.Navigation.BreadcrumbNavigation(m_Console, component.m_ButtonList == null ? null : component.m_ButtonList.ToArray()), 
                    null,
                    openInFrame, false, Source.Generic.Footer(m_Console.IsNewDesign)
                    ));
            }
            //  Open the listmode
            else
            {
                //string form = component.CreateList(m_Console, openInFrame);
                if (m_Console.Request.QueryString["formatpdf"] == "1")
                {
                    m_Console.View = 1;
                    AddToResponse(Source.Template.OpenInFrame(
                        m_Console,
                        Source.Generic.Header(m_Console),
                        component.CreateSearchList(m_Console, 0).ToString(), null, 
                        grid.GetGridFromListInstance(m_Console.CurrentListInstance.wim, m_Console, openInFrame, true),
                        openInFrame, false, Source.Generic.Footer(m_Console.IsNewDesign)
                        ));
                    return;
                }

                m_Console.View = 1;

                if (m_Console.Request.Form["createNew"] == "1")
                    component.m_ClickedButton = "createNew";

                string searchList = component.CreateSearchList(m_Console, openInFrame).ToString();
                string searchListGrid;

                bool isThumbnailView = false;
                if (m_Console.CurrentApplicationUser.ShowDetailView || m_Console.CurrentList.Type != Sushi.Mediakiwi.Data.ComponentListType.Browsing)
                    //|| m_Console.CurrentListInstance.wim.CurrentFolder.Type != Sushi.Mediakiwi.Data.FolderType.Gallery)
                    searchListGrid = grid.GetGridFromListInstance(m_Console.CurrentListInstance.wim, m_Console, openInFrame, IsNewDesignOutput);
                else
                {
                    isThumbnailView = true;
                    searchListGrid = grid.GetThumbnailGridFromListInstance(m_Console.CurrentListInstance.wim, m_Console, openInFrame, false);
                }

                string bottom = Source.Navigation.NewBottomNavigation(m_Console, component.m_ButtonList != null ? component.m_ButtonList.ToArray() : null, !string.IsNullOrEmpty(searchList));

                string response = Source.Template.OpenInFrame(
                    m_Console,
                    Source.Generic.Header(m_Console),
                    searchList + bottom,
                    Source.Navigation.BreadcrumbNavigation(m_Console, component.m_ButtonList == null ? null : component.m_ButtonList.ToArray()),
                    searchListGrid,
                    openInFrame, isThumbnailView, Source.Generic.Footer(m_Console.IsNewDesign)
                    );

                AddToResponse(response);
            }
        }
    }
}
