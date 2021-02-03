using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// 
    /// </summary>
    public class WimComponentRoot
    {
        /// <summary>
        /// Gets the environment.
        /// </summary>
        public Sushi.Mediakiwi.Data.IEnvironment Environment
        {
            get { return Sushi.Mediakiwi.Data.Environment.Current; }
        }

        //private Sushi.Mediakiwi.Framework.CacheManagement m_CacheManagement;
        ///// <summary>
        ///// Gets or sets the cache management.
        ///// </summary>
        ///// <value>The cache management.</value>
        //public Sushi.Mediakiwi.Framework.CacheManagement CacheManagement
        //{
        //    set { m_CacheManagement = value; }
        //    get
        //    {
        //        if (m_CacheManagement == null)
        //            m_CacheManagement = new CacheManagement();
        //        return m_CacheManagement;
        //    }
        //}

        /// <summary>
        /// Gets or sets the current visitor.
        /// </summary>
        /// <value>The current visitor.</value>
        public Sushi.Mediakiwi.Data.Identity.IVisitor CurrentVisitor
        {
            get { return Sushi.Mediakiwi.Data.Identity.Visitor.Select(); }
            set { HttpContext.Current.Items["wim.visitor"] = value; }
        }

        /// <summary>
        /// Gets or sets the current profile.
        /// </summary>
        /// <value>The current profile.</value>
        public Sushi.Mediakiwi.Data.Identity.IProfile CurrentProfile
        {
            get { return Sushi.Mediakiwi.Data.Identity.Profile.Select(); }
            set { HttpContext.Current.Items["wim.profile"] = value; }
        }

        /// <summary>
        /// Logins the specified email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="password">The password.</param>
        /// <param name="shouldRememberVisitorForNextVisit">if set to <c>true</c> [should remember visitor for next visit].</param>
        /// <returns></returns>
        public bool Login(string email, string password, bool shouldRememberVisitorForNextVisit)
        {
            return Login(email, password, shouldRememberVisitorForNextVisit, null);
        }

        /// <summary>
        /// Logins the specified profile identifier.
        /// </summary>
        /// <param name="profileID">The profile identifier.</param>
        /// <param name="password">The password.</param>
        /// <param name="shouldRememberVisitorForNextVisit">if set to <c>true</c> [should remember visitor for next visit].</param>
        /// <returns></returns>
        public bool Login(int profileID, string password, bool shouldRememberVisitorForNextVisit)
        {
            return Login(profileID, password, shouldRememberVisitorForNextVisit, null);
        }

        /// <summary>
        /// Logins the specified profile.
        /// </summary>
        /// <param name="profile">The profile.</param>
        /// <param name="password">The password.</param>
        /// <param name="shouldRememberVisitorForNextVisit">if set to <c>true</c> [should remember visitor for next visit].</param>
        /// <returns></returns>
        public bool Login(Sushi.Mediakiwi.Data.Identity.Profile profile, string password, bool shouldRememberVisitorForNextVisit)
        {
            return Login(profile, password, shouldRememberVisitorForNextVisit, null);
        }

        /// <summary>
        /// Logins the specified email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="password">The password.</param>
        /// <param name="shouldRememberVisitorForNextVisit">if set to <c>true</c> [should remember visitor for next visit].</param>
        /// <param name="redirectionPageID">The redirection page ID.</param>
        /// <returns></returns>
        public bool Login(string email, string password, bool shouldRememberVisitorForNextVisit, int? redirectionPageID)
        {            
            Sushi.Mediakiwi.Data.Identity.IProfile p = Sushi.Mediakiwi.Data.Identity.Profile.Login(email, password);            
            if (p.IsNewInstance)
                return false;

            CurrentProfile = p;
            CurrentProfile.Save(shouldRememberVisitorForNextVisit, true);

            if (CurrentVisitor.IsLoggedIn && redirectionPageID.HasValue)
                HttpContext.Current.Response.Redirect(Sushi.Mediakiwi.Data.Page.SelectOne(redirectionPageID.Value).HRef);

            if (string.IsNullOrEmpty(CurrentProfile.Email))
                return false;
            else
                return CurrentVisitor.IsLoggedIn;
        }

        /// <summary>
        /// Logins the specified profile identifier.
        /// </summary>
        /// <param name="profileID">The profile identifier.</param>
        /// <param name="password">The password.</param>
        /// <param name="shouldRememberVisitorForNextVisit">if set to <c>true</c> [should remember visitor for next visit].</param>
        /// <param name="redirectionPageID">The redirection page identifier.</param>
        /// <returns></returns>
        public bool Login(int profileID, string password, bool shouldRememberVisitorForNextVisit, int? redirectionPageID)
        {
            Sushi.Mediakiwi.Data.Identity.IProfile p = Sushi.Mediakiwi.Data.Identity.Profile.SelectOne(profileID);
            if (p.IsNewInstance || p.Password != password)
                return false;

            CurrentProfile = p;
            CurrentProfile.Save(shouldRememberVisitorForNextVisit, true);

            if (CurrentVisitor.IsLoggedIn && redirectionPageID.HasValue)
                HttpContext.Current.Response.Redirect(Sushi.Mediakiwi.Data.Page.SelectOne(redirectionPageID.Value).HRef);

            if (string.IsNullOrEmpty(CurrentProfile.Email))
                return false;
            else
                return CurrentVisitor.IsLoggedIn;
        }

        /// <summary>
        /// Logins the specified profile.
        /// </summary>
        /// <param name="profile">The profile.</param>
        /// <param name="password">The password.</param>
        /// <param name="shouldRememberVisitorForNextVisit">if set to <c>true</c> [should remember visitor for next visit].</param>
        /// <param name="redirectionPageID">The redirection page identifier.</param>
        /// <returns></returns>
        public bool Login(Sushi.Mediakiwi.Data.Identity.Profile profile, string password, bool shouldRememberVisitorForNextVisit, int? redirectionPageID)
        {
            if (profile.IsNewInstance || profile.Password != password)
                return false;

            CurrentProfile = profile;
            CurrentProfile.Save(shouldRememberVisitorForNextVisit, true);

            if (CurrentVisitor.IsLoggedIn && redirectionPageID.HasValue)
                HttpContext.Current.Response.Redirect(Sushi.Mediakiwi.Data.Page.SelectOne(redirectionPageID.Value).HRef);

            if (string.IsNullOrEmpty(CurrentProfile.Email))
                return false;
            else
                return CurrentVisitor.IsLoggedIn;
        }

        /// <summary>
        /// Logins the specified email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="shouldRememberVisitorForNextVisit">if set to <c>true</c> [should remember visitor for next visit].</param>
        /// <returns></returns>
        public bool Login(string email, bool shouldRememberVisitorForNextVisit)
        {
            return Login(email, shouldRememberVisitorForNextVisit, null);
        }

        /// <summary>
        /// Logins the specified profile identifier.
        /// </summary>
        /// <param name="profileID">The profile identifier.</param>
        /// <param name="shouldRememberVisitorForNextVisit">if set to <c>true</c> [should remember visitor for next visit].</param>
        /// <returns></returns>
        public bool Login(int profileID, bool shouldRememberVisitorForNextVisit)
        {
            return Login(profileID, shouldRememberVisitorForNextVisit, null);
        }

        /// <summary>
        /// Logins the specified profile.
        /// </summary>
        /// <param name="profile">The profile.</param>
        /// <returns></returns>
        public bool Login(Sushi.Mediakiwi.Data.Identity.Profile profile, bool shouldRememberVisitorForNextVisit)
        {
            return Login(profile, shouldRememberVisitorForNextVisit, null);
        }

        /// <summary>
        /// Logins the specified email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="shouldRememberVisitorForNextVisit">if set to <c>true</c> [should remember visitor for next visit].</param>
        /// <param name="redirectionPageID">The redirection page identifier.</param>
        /// <returns></returns>
        public bool Login(string email, bool shouldRememberVisitorForNextVisit, int? redirectionPageID)
        {
            Sushi.Mediakiwi.Data.Identity.IProfile p = Sushi.Mediakiwi.Data.Identity.Profile.SelectOne(email);
            if (p.IsNewInstance)
                return false;

            CurrentProfile = p;
            CurrentProfile.Save(shouldRememberVisitorForNextVisit, true);

            if (redirectionPageID.HasValue)
                HttpContext.Current.Response.Redirect(Sushi.Mediakiwi.Data.Page.SelectOne(redirectionPageID.Value).HRef);

            if (string.IsNullOrEmpty(CurrentProfile.Email))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Logins the specified profile.
        /// </summary>
        /// <param name="profile">The profile.</param>
        /// <param name="shouldRememberVisitorForNextVisit">if set to <c>true</c> [should remember visitor for next visit].</param>
        /// <param name="redirectionPageID">The redirection page identifier.</param>
        /// <returns></returns>
        public bool Login(Sushi.Mediakiwi.Data.Identity.Profile profile, bool shouldRememberVisitorForNextVisit, int? redirectionPageID)
        {
            if (profile.IsNewInstance)
                return false;

            CurrentProfile = profile;
            CurrentProfile.Save(shouldRememberVisitorForNextVisit, true);

            if (redirectionPageID.HasValue)
                HttpContext.Current.Response.Redirect(Sushi.Mediakiwi.Data.Page.SelectOne(redirectionPageID.Value).HRef);

            if (string.IsNullOrEmpty(CurrentProfile.Email))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Logins the specified profile identifier.
        /// </summary>
        /// <param name="profileID">The profile identifier.</param>
        /// <param name="shouldRememberVisitorForNextVisit">if set to <c>true</c> [should remember visitor for next visit].</param>
        /// <param name="redirectionPageID">The redirection page identifier.</param>
        /// <returns></returns>
        public bool Login(int profileID, bool shouldRememberVisitorForNextVisit, int? redirectionPageID)
        {
            Sushi.Mediakiwi.Data.Identity.IProfile p = Sushi.Mediakiwi.Data.Identity.Profile.SelectOne(profileID);
            if (p.IsNewInstance)
                return false;

            CurrentProfile = p;
            CurrentProfile.Save(shouldRememberVisitorForNextVisit, true);

            if (redirectionPageID.HasValue)
                HttpContext.Current.Response.Redirect(Sushi.Mediakiwi.Data.Page.SelectOne(redirectionPageID.Value).HRef);

            if (string.IsNullOrEmpty(CurrentProfile.Email))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Logouts the specified redirection page ID.
        /// </summary>
        /// <param name="redirectionPageID">The redirection page ID.</param>
        /// <returns></returns>
        public bool Logout(int? redirectionPageID)
        {
            return this.CurrentProfile.Logout(redirectionPageID);
        }

        /// <summary>
        /// Gets or sets the current application user.
        /// </summary>
        /// <value>The current application user.</value>
        public Sushi.Mediakiwi.Data.IApplicationUser CurrentApplicationUser
        {
            get { return Sushi.Mediakiwi.Data.ApplicationUserLogic.Select(); }
            set { HttpContext.Current.Items["wim.applicationuser"] = value; }
        }

        /// <summary>
        /// Gets the web chart instance.
        /// </summary>
        /// <param name="chartType">Type of the chart.</param>
        /// <param name="width">The width.</param>
        /// <param name="heigth">The heigth.</param>
        /// <param name="dataSource">The data source.</param>
        /// <param name="hasLegend">if set to <c>true</c> [has legend].</param>
        /// <param name="backGroundColor">Color of the back ground.</param>
        /// <returns></returns>
        /// <value>The web chart instance.</value>
        //public string GetChartUrl(Winnovative.ChartType chartType, int width, int heigth, object dataSource, bool hasLegend, System.Drawing.Color backGroundColor)
        //{
        //    Winnovative.WebChart wc = new Winnovative.WebChart();
        //    wc.ID = "test";
        //    wc.Width = width;
        //    wc.Height = heigth;

        //    wc.DataSource = dataSource;
        //    wc.ChartType = chartType;
        //    wc.Labels.ItemLabels.Visible = false;
        //    wc.Axis.X.Space = 75;
        //    wc.Axis.Y.Space = 20;
        //    wc.Axis.Z.Space = 50;
        //    wc.LegendBox.Visible = hasLegend;
        //    wc.Charts3D.RotateAxisX = 120;
        //    wc.Charts3D.RotateAxisY = 0;
        //    wc.Charts3D.RotateAxisZ = 0;
        //    wc.LicenseKey = "KgEYChgbChgSHAoYBBoKGRsEGxgEExMTEw==";
        //    wc.BackColor = backGroundColor;// System.Drawing.Color.FromArgb(245, 246, 248);

        //    string file = Wim.Utility.AddApplicationPath(string.Concat(@"/repository/Charting/", Guid.NewGuid().ToString(), ".jpg"));
        //    wc.Save(HttpContext.Current.Request.MapPath(file));
        //    return file;
        //}

        /// <summary>
        /// 
        /// </summary>
        public WimComponentRoot()
        {
            Hooks = new Hook();
            Hooks.Site = this.Site;

        }

        /// <summary>
        /// Debugs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Debug(object message)
        {
            WimOutput output = new WimOutput();
            output.Debug(message);
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
                if (m_Site == null || string.IsNullOrEmpty(m_Site.Culture))
                    return System.Globalization.CultureInfo.CurrentCulture;

                m_CurrentCulture = new CultureInfo(m_Site.Culture);
                return m_CurrentCulture;
            }
        }

        private Sushi.Mediakiwi.Data.Component m_Component;
        /// <summary>
        /// 
        /// </summary>
        public Sushi.Mediakiwi.Data.Component Component
        {
            set { m_Component = value; }
            get { return m_Component; }
        }

        private Sushi.Mediakiwi.Data.Page m_Page;
        /// <summary>
        /// Gets or sets the page.
        /// </summary>
        /// <value>The page.</value>
        public Sushi.Mediakiwi.Data.Page Page
        {
            set { m_Page = value; }
            get {
                if (m_Page == null)
                    m_Page = HttpContext.Current.Items["Wim.Page"] as Sushi.Mediakiwi.Data.Page;
                return m_Page; 
            }
        }

        private Sushi.Mediakiwi.Data.Site m_Site;
        /// <summary>
        /// Gets or sets the site.
        /// </summary>
        /// <value>The site.</value>
        public Sushi.Mediakiwi.Data.Site Site
        {
            set { m_Site = value; }
            get {
                if (m_Site == null)
                    m_Site = HttpContext.Current.Items["Wim.Site"] as Sushi.Mediakiwi.Data.Site;
                
                return m_Site; 
            }
        }

        /// <summary>
        /// Gets the URL mapping configuration.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public Sushi.Mediakiwi.Framework.WimServerUrlMapping GetUrlMappingConfiguration(string name, int? type)
        {
            return Sushi.Mediakiwi.Data.Common.GetUrlMappingConfiguration(name, type);
        }

        /// <summary>
        /// <para>Function for converting parameters into a wimServerConfiguration>urlMappings.</para>
        /// <para>Specify the name of the mapping and the values of the groups and the url will get created.</para>
        /// <para>- All parameters are url encoded</para>
        /// <para>- Less parameters are filled up with 0 and a notification logged into Sushi.Mediakiwi.Data.notification CreateMappedUrl</para>
        /// <para>- more parameters are ignored</para>
        /// <para>- Application path is added</para>
        /// </summary>
        /// <param name="urlMappingName">The name of the url mapping in your web.config</param>
        /// <param name="args">The 'objects' into the url</param>
        /// <returns>A url represented as string</returns>
        public string CreateMappedUrl(string urlMappingName, params object[] args)
        {
            return Sushi.Mediakiwi.Data.Common.CreateMappedUrl(urlMappingName, args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="link"></param>
        /// <param name="itemID"></param>
        public void SetMappedUrl(Sushi.Mediakiwi.Framework.ControlLib.WimLink link, int itemID)
        {
            string url = GetMappedUrl(link.Page, itemID);
            if (url == null)
            {
                link.Visible = false;
                return;
            }
            link.Href = url;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="link"></param>
        /// <param name="pageRequest"></param>
        /// <param name="itemID"></param>
        public void SetMappedUrl(Sushi.Mediakiwi.Framework.ControlLib.WimLink link, Sushi.Mediakiwi.Data.Page pageRequest, int itemID)
        {
            string url = GetMappedUrl(pageRequest, itemID);
            if (url == null)
            {
                link.Visible = false;
                return;
            }
            link.Href = url;
        }

        public string GetMappedUrl(Sushi.Mediakiwi.Data.Page pageRequest, int itemID)
        {
            if (pageRequest == null || pageRequest.IsNewInstance) return null;
            var map = Sushi.Mediakiwi.Data.PageMapping.SelectOne(null, itemID, pageRequest.ID);
            if (map.IsNewInstance)
                return pageRequest.HRef;

            return map.NavigateURL;
        }
        /// <summary>
        /// Returns pagemapping path when available.
        /// if not available, href from inPage will be returned
        /// </summary>
        /// <param name="relativePath">The relative path</param>
        /// <param name="inPage">The current page</param>
        /// <returns>Pagemapping Path, when available. Else inpage.Href</returns>
        public string GetMappedUrl(string relativePath, Sushi.Mediakiwi.Data.Page inPage)
        {
            if (string.IsNullOrEmpty(relativePath))
                return null;

            var map = Sushi.Mediakiwi.Data.PageMapping.SelectOne(relativePath);
            if (map.IsNewInstance)
                return inPage.HRef;

            return map.NavigateURL;
        }
        public Sushi.Mediakiwi.Data.IPageMapping SetMappedUrl(int pageID, string pagePath, string pageTitle, string querystring, int itemID)
        {
            var map = Sushi.Mediakiwi.Data.PageMapping.SelectOne(pagePath);
            if (map.IsNewInstance)
                return Sushi.Mediakiwi.Data.PageMapping.RegisterUrl(pagePath, querystring, pageTitle, pageID, false, null, itemID); 
            return map;
        }

        #region Folder (only set when called)
        /// <summary>
        /// 
        /// </summary>
        [Obsolete("This property can be found at wim.Page.Folder", false)] 
        public Sushi.Mediakiwi.Data.Folder Folder
        {
            get
            {
                if (m_Page == null || m_Page.Folder == null) return null;
                return m_Page.Folder;
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public bool IsLoadedInWim;
        public bool IsLoadedInAJAX;

        bool? m_IsPreviewMode;
        /// <summary>
        /// Gets a value indicating whether this instance is preview mode.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is preview mode; otherwise, <c>false</c>.
        /// </value>
        public bool IsPreviewMode
        {
            get
            {
                if (m_IsPreviewMode.HasValue)
                    return m_IsPreviewMode.Value;

                m_IsPreviewMode = false;
                if (System.Web.HttpContext.Current.Request.QueryString["preview"] == "1")
                {
                    if (CurrentApplicationUser.IsLoggedIn())
                        m_IsPreviewMode = true;
                }
                return m_IsPreviewMode.Value;
            }
        }

        #region FindControlOnPage
        private List<System.Web.UI.Control> m_collection;
        /// <summary>
        /// Find controls on the current page and add these to a collection
        /// </summary>
        /// <param name="page"></param>
        /// <param name="type">The type of the to be found control</param>
        /// <param name="collection"></param>
        /// <param name="stopAfterFirstResult">Stop searching after finding the first control</param>
        /// <returns>Does the collection contain more the one item?</returns>
        public bool FindControlOnPage(System.Web.UI.Page page, Type type, out System.Web.UI.Control[] collection, bool stopAfterFirstResult)
        {
            return FindControlOnPage(page, type, out collection, stopAfterFirstResult, false);
        }

        /// <summary>
        /// Find controls on the current page and add these to a collection
        /// </summary>
        /// <param name="page"></param>
        /// <param name="type">The type of the to be found control</param>
        /// <param name="collection"></param>
        /// <param name="stopAfterFirstResult">Stop searching after finding the first control</param>
        /// <param name="traceOutput"></param>
        /// <returns>Does the collection contain more the one item?</returns>
        public bool FindControlOnPage(System.Web.UI.Page page, Type type, out System.Web.UI.Control[] collection, bool stopAfterFirstResult, bool traceOutput)
        {
            m_stopScan = false;
            m_collection = new List<System.Web.UI.Control>();
            ScanControls(type, page.Controls, stopAfterFirstResult, page, traceOutput);
            collection = m_collection.ToArray();

            if (m_collection.Count == 0)
                return false;
            return true;
        }

        private bool m_stopScan;
        private void ScanControls(Type type, System.Web.UI.ControlCollection controls, bool stopAfterFirstResult, System.Web.UI.Page page, bool traceOutput)
        {
            foreach (System.Web.UI.Control c in controls)
            {
                bool foundOne = false;
                if (c.GetType() == type || c.GetType().BaseType == type || c.GetType().BaseType.BaseType == type)
                {
                    foundOne = true;
                    if (traceOutput) page.Trace.Write("ScanControl", string.Concat("Found type: ", type));
                }
                else
                {
                    PartialCachingControl pcc = c as PartialCachingControl;
                    if (pcc != null)
                    {
                        if (pcc.CachedControl.GetType() == type || pcc.CachedControl.GetType().BaseType == type || pcc.CachedControl.GetType().BaseType.BaseType == type)
                        {
                            foundOne = true;
                            if (traceOutput) page.Trace.Write("ScanControl", string.Concat("Found type: ", type));
                        }
                    }
                }

                if (foundOne)
                {
                    if (m_stopScan) break;
                    if (stopAfterFirstResult) m_stopScan = true;
                    if (traceOutput) 
                        page.Trace.Write("ScanControl", string.Format("Found - Type: {0} - Base: {1} - BaseBase: {2}", c.GetType(), c.GetType().BaseType, c.GetType().BaseType.BaseType));
                    m_collection.Add(c);
                }
                else
                    if (traceOutput) page.Trace.Write("ScanControl", string.Format("Type: {0} - Base: {1} - BaseBase: {2}", c.GetType(), c.GetType().BaseType, c.GetType().BaseType.BaseType));

                if (m_stopScan) break;
                if (c.HasControls())
                    ScanControls(type, c.Controls, stopAfterFirstResult, page, traceOutput);
            }
        }
        #endregion

        internal bool blockContentAddion = false;
        //public PageInfo PageInformation;
        /// <summary>
        /// 
        /// </summary>
        public Sushi.Mediakiwi.Framework.Hook Hooks;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="loadedControl"></param>
        internal void TriggerFixedContentFill(System.Web.UI.UserControl loadedControl)
        {
            TriggerFixedContentFill(loadedControl, null);
        }

        /// <summary>
        /// Triggers the fixed content fill.
        /// </summary>
        /// <param name="loadedControl">The loaded control.</param>
        /// <param name="pageKey">The page key.</param>
        public void TriggerFixedContentFill(System.Web.UI.UserControl loadedControl, int? pageKey)
        {
            TriggerFixedContentFill(loadedControl, pageKey, false);
        }

        internal bool HasBeenTriggered;

        /// <summary>
        /// Sets content to all Wim enables properties. When a page identifier is applied the content is grabbed from a simular component that is found on that page.
        /// Important note: The components are matched on basis of their ID's.
        /// </summary>
        /// <param name="loadedControl">The loaded control.</param>
        /// <param name="pageKey">The page key.</param>
        /// <param name="overrideContentSetProperty">if set to <c>true</c> [override content set property].</param>
        public void TriggerFixedContentFill(System.Web.UI.UserControl loadedControl, int? pageKey, bool overrideContentSetProperty)
        {
            //if (HasBeenTriggered && !pageKey.HasValue) return;
            //if (!overrideContentSetProperty && ContentIsSet) return;

            if (loadedControl == null || loadedControl.ClientID == null) return;
            if (Page == null) return;

            Sushi.Mediakiwi.Data.Page hijackedPage = null;

            if (pageKey.HasValue)
            {
                if (Page.ID == pageKey.Value) pageKey = null;
                else
                {
                    hijackedPage = Sushi.Mediakiwi.Data.Page.SelectOneChild(pageKey.Value, this.Site.ID, false);
                        //Sushi.Mediakiwi.Data.PageManager.se.SelectOne(pageKey.Value, false);
                }
            }

            //  Introduced for CMS loading
            if (blockContentAddion)
            {
                return;
            }


            //  Get current component
            Sushi.Mediakiwi.Data.Component component;

            if (((ComponentTemplate)loadedControl).wim.Component != null && !((ComponentTemplate)loadedControl).wim.Component.IsNewInstance)
                component = ((ComponentTemplate)loadedControl).wim.Component;
            else
            {
                bool isPreview = (HttpContext.Current.Items["Wim.Preview"] != null);
                if (!isPreview)
                {
                    //  LIVE
                    if (pageKey.HasValue)
                    {
                        component = Sushi.Mediakiwi.Data.Component.SelectOne(hijackedPage.ID, loadedControl.GetType());
                    }
                    else
                    {
                        component = Sushi.Mediakiwi.Data.Component.SelectOne((Page.InheritContent ? Page.MasterID.Value : Page.ID), loadedControl.ClientID);
                    }
                }
                else
                {
                    //  EDIT
                    Sushi.Mediakiwi.Data.ComponentVersion componentEdited = null;
                    if (pageKey.HasValue)
                        componentEdited = Sushi.Mediakiwi.Data.ComponentVersion.SelectOneBasedOnType((hijackedPage.InheritContent ? hijackedPage.MasterID.Value : hijackedPage.ID), loadedControl.GetType());
                    else
                        componentEdited = Sushi.Mediakiwi.Data.ComponentVersion.SelectOneFixed((Page.InheritContent ? Page.MasterID.Value : Page.ID), loadedControl.ClientID);

                    if (componentEdited == null)
                        return;

                    component = componentEdited.Convert();
                }
            }

            Sushi.Mediakiwi.Data.Content content = Sushi.Mediakiwi.Data.Content.GetDeserialized(component.Serialized_XML);
            Templates.PropertySet pset = new Sushi.Mediakiwi.Framework.Templates.PropertySet();

            if (component != null && component.IsNewInstance)
            {
                Sushi.Mediakiwi.Data.ComponentTemplate ct = Sushi.Mediakiwi.Data.ComponentTemplate.SelectOne_BasedOnType(loadedControl.GetType().BaseType);

                if (ct.IsNewInstance)
                {
                    component.CacheLevel = 2;
                }
                else
                {
                    component.CacheLevel = ct.CacheLevel;
                    component.TemplateID = ct.ID;
                    component.TemplateLocation = ct.Location;
                    component.AjaxType = ct.AjaxType;
                }
            }


            ((ComponentTemplate)loadedControl).wim.Component = component;

            //  TODO: WHY first set it and then discard it?
            pset.SetValue(Site, loadedControl, content, Page);
            HasBeenTriggered = true;
        }


        internal System.Web.UI.Page currentPage;



        bool m_ContentIsSet;
        internal bool ContentIsSet
        {
            set { m_ContentIsSet = value; }
            get { return m_ContentIsSet; }
        }
    }
}
