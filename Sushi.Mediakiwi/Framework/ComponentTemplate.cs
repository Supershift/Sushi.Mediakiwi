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
using System.Collections.Specialized;

namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// 
    /// </summary>
    public class ComponentTemplate : System.Web.UI.UserControl
    {
        #region Administration Navigation
        /// <summary>
        /// Adds the administration navigation item.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="pageID">The page ID.</param>
        public void AddAdministrationNavigationItem(string name, int? pageID)
        {
            AddAdministrationNavigationItem(name, null, null, pageID, null, null, false, false);
        }
        /// <summary>
        /// Adds the administration navigation item.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="url">The URL.</param>
        public void AddAdministrationNavigationItem(string name, string url)
        {
            AddAdministrationNavigationItem(name, null, null, null, url, null, false, false);
        }
        /// <summary>
        /// Adds the administration navigation item.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="componentList">The component list.</param>
        public void AddAdministrationNavigationItem(string name, System.Type componentList)
        {
            AddAdministrationNavigationItem(name, componentList, null, null, null, null, false, false);
        }
        /// <summary>
        /// Adds the administration navigation item.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="componentList">The component list.</param>
        /// <param name="componentListItemID">The component list item ID.</param>
        public void AddAdministrationNavigationItem(string name, System.Type componentList, int? componentListItemID)
        {
            AddAdministrationNavigationItem(name, componentList, componentListItemID, null, null, null, false, false);
        }
        /// <summary>
        /// Adds the administration navigation item.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="componentList">The component list.</param>
        /// <param name="componentListItemID">The component list item ID.</param>
        /// <param name="urlOrWimDomain">The URL or wim domain.</param>
        /// <param name="portal">The portal.</param>
        public void AddAdministrationNavigationItem(string name, System.Type componentList, int? componentListItemID, string urlOrWimDomain, string portal)
        {
            AddAdministrationNavigationItem(name, componentList, componentListItemID, null, urlOrWimDomain, portal, false, false);
        }

        /// <summary>
        /// Adds an administration navigation item.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="componentList">The component list.</param>
        /// <param name="componentListItemID">The component list item ID.</param>
        /// <param name="pageID">The page ID.</param>
        /// <param name="urlOrWimDomain">The remote URL or TestDrive domain. When the TestDrive login is simular to the current IIS Instance this property can be left empty; if other URL then apply full prefix URL, f.e. [http://login.sitename.com/repository/wim/portal.ashx]</param>
        /// <param name="portal">The portal for connecting to the component list.</param>
        /// <param name="openInLayer">if set to <c>true</c> [open in layer].</param>
        /// <param name="showInMenu">if set to <c>true</c> [show in menu].</param>
        public void AddAdministrationNavigationItem(string name, System.Type componentList, int? componentListItemID, int? pageID, string urlOrWimDomain, string portal, bool openInLayer, bool showInMenu)
        {
            //bool isNew = false;
            //var customNavigationItems = HttpContext.Current.Items["TestDrive.Navigation"] as FooterNavigation;
            //if (customNavigationItems == null)
            //{
            //    isNew = true;
            //    customNavigationItems = new FooterNavigation();
            //}

            //if (customNavigationItems.HasKey(name))
            //    return;

            //customNavigationItems.Add(name, componentList, componentListItemID, pageID, urlOrWimDomain, portal, openInLayer, showInMenu);

            //if (isNew)
            //    HttpContext.Current.Items.Add("TestDrive.Navigation", customNavigationItems);
            //else
            //    HttpContext.Current.Items["TestDrive.Navigation"] = customNavigationItems;
        }
        #endregion #region Administration Navigation

        void FindAjaxControls(ControlCollection collection, string idnotation)
        {
            foreach (Control c in collection)
            {
                if (c.GetType() == typeof(System.Web.UI.WebControls.Button))
                {
                    ((System.Web.UI.WebControls.Button)c).CssClass +=
                        string.Concat(
                        (string.IsNullOrEmpty(((System.Web.UI.WebControls.Button)c).CssClass) ? "" : " "),
                        string.Format("_ReloadFromUrl id_wim_{0} source_{0} target_{0} noform_1", idnotation));
                }
                if (c.GetType() == typeof(System.Web.UI.WebControls.LinkButton))
                {
                    ((System.Web.UI.WebControls.LinkButton)c).CssClass +=
                        string.Concat(
                        (string.IsNullOrEmpty(((System.Web.UI.WebControls.LinkButton)c).CssClass) ? "" : " "),
                        string.Format("_ReloadFromUrl id_wim_{0} source_{0} target_{0} noform_1", idnotation));
                }
                if (c.GetType() == typeof(System.Web.UI.WebControls.ImageButton))
                {
                    ((System.Web.UI.WebControls.ImageButton)c).CssClass +=
                        string.Concat(
                        (string.IsNullOrEmpty(((System.Web.UI.WebControls.ImageButton)c).CssClass) ? "" : " "),
                        string.Format("_ReloadFromUrl id_wim_{0} source_{0} target_{0} noform_1", idnotation));
                }
                if (c.HasControls())
                    FindAjaxControls(c.Controls, idnotation);
            }
        }

        /// <summary>
        /// Gets or sets the cache key.
        /// </summary>
        /// <value>The cache key.</value>
        public string CacheKey { get; set; }
        /// <summary>
        /// This is used when the entire page is cached and a fixed control which is not in the Wim template repository, 
        /// like topnavigation should be visitor based.
        /// </summary>
        public bool IsVisitorBasedFixedControl { get; set; }

        bool m_killEventSubscription;
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            m_killEventSubscription = (Context.Items["Wim.stop"] != null);

            if (Context.Items.Contains("Wim.Component") && Context.Items["Wim.Component"] != null)
            {
                wim.Component = Context.Items["Wim.Component"] as Sushi.Mediakiwi.Data.Component;

                string componentID = wim.Component.ID.ToString();
                if (componentID == "0")
                {
                    componentID = wim.Component.GUID.ToString();
                }
                this.ID = string.Concat("_c", componentID);
                Context.Items["Wim.Component"] = null;
            }
            if (m_killEventSubscription) return;

            if (!wim.HasBeenTriggered)
            {
                if (this.Parent.GetType() != typeof(System.Web.UI.WebControls.PlaceHolder) && this.Parent.GetType() != typeof(ControlLib.WimContainer))
                    wim.TriggerFixedContentFill(this);
            }

            base.OnInit(e);
        }

        /// <summary>
        /// Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"></see> object, which writes the content to be rendered on the client.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"></see> object that receives the server control content.</param>
        protected override void Render(HtmlTextWriter writer)
        {
            bool useCaching = (this.CacheKey != null && wim.Component != null && CacheDuration.TotalSeconds > 0);

            if (useCaching)
            {
                using (Wim.Utilities.CacheItemManager cman = new Wim.Utilities.CacheItemManager())
                {
                    System.Text.StringBuilder build = new System.Text.StringBuilder();
                    System.IO.StringWriter x = new System.IO.StringWriter(build);
                    HtmlTextWriter writer2 = new HtmlTextWriter(x);
                    base.Render(writer2);
                    writer2.Flush();

                    writer.Write(build);

                    cman.Add(this.CacheKey, build.ToString(), DateTime.Now.AddSeconds(CacheDuration.TotalSeconds), null);
                    if (Trace.IsEnabled)
                        Page.Trace.Write("Wim Caching", string.Format("Found component level component [{0}] - key = {1}", this.ID, this.CacheKey));
                }

            }
            else
            {
                base.Render(writer);

                if (wim.Component != null && wim.Component.CacheLevel == 2)
                {
                    if (Trace.IsEnabled)
                        Page.Trace.Write("Wim Caching", string.Format("Found page level cachable component [{0}]", this.ID));

                    return;
                }

                if (wim.Component != null && wim.Component.CacheLevel == 1)
                {
                    if (Trace.IsEnabled)
                        Page.Trace.Write("Wim Caching", string.Format("Found component level cachable (FIXED) component [{0}]", this.ID));
                    return;
                }

                if (this.CacheKey == null || this.CacheKey != "[wim_fixed_cachable]")
                {
                    wim.Page.AddToOutputCache = false;

                    if (Trace.IsEnabled)
                        Page.Trace.Write("Wim Caching", string.Format("Found non-cachable component [{0}]", this.ID));
                }
                else
                {
                    if (Trace.IsEnabled)
                        Page.Trace.Write("Wim Caching", string.Format("Found fixed compent which can be page cached [{0}]", this.ID));
                }
            }
        }

        internal TimeSpan CacheDuration;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            if (m_killEventSubscription) return;
            base.OnLoad(e);


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            if (this.IsVisitorBasedFixedControl && this.wim.Page.IsPageFullyCachable())
            {
                this.Controls.AddAt(0, new LiteralControl(string.Format("<fixed template=\"{0}\" id=\"{1}\">", this.TemplateControl.AppRelativeVirtualPath.Replace("~", string.Empty), this.ID)));
                this.Controls.Add(new LiteralControl("</fixed>"));
            }

            if (m_killEventSubscription) return;
            base.OnPreRender(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnUnload(EventArgs e)
        {
            if (m_killEventSubscription) return;
            base.OnUnload(e);
        }

        /// <summary>
        /// 
        /// </summary>
        public WimComponentRoot wim;

        private string m_Title;
        /// <summary>
        /// 
        /// </summary>
        protected string Component
        {
            get { return m_Title; }
            set { m_Title = value; }
        }

        //private Guid m_ComponentGuid;
        ///// <summary>
        ///// GUID of the loaded content component
        ///// </summary>
        //public Guid ComponentGuid
        //{
        //    get { return m_ComponentGuid; }
        //    set { m_ComponentGuid = value; }
        //}

        //private int m_ContainerIndex = -1;
        ///// <summary>
        ///// What is the index of the current component
        ///// </summary>
        //public int ContainerIndex
        //{
        //    get { return m_ContainerIndex; }
        //    set { m_ContainerIndex = value; }
        //}

        private bool m_EnableVisibility;
        /// <summary>
        /// This property can be set on an instance on the usercontrol (a fixed component). When false the component is not visible to Wim for editing.
        /// </summary>
        public bool EnableVisibility
        {
            get { return m_EnableVisibility; }
            set { m_EnableVisibility = value; }
        }

        private bool m_ServiceColumn;
        /// <summary>
        /// This property can be set on an instance on the usercontrol (a fixed component). When true the component is editable in the service column content section within Wim.
        /// </summary>
        [Obsolete("Used for fixed components, use [AssociatedContainer] in stead", false)]
        public bool ServiceColumn
        {
            get { return m_ServiceColumn; }
            set { m_ServiceColumn = value; }
        }

        /// <summary>
        /// This property can be set to define in wich container it should show in the portal for a fixed component. 
        /// </summary>
        public string AssociatedContainerID { get; set; }

        /// <summary>
        /// 
        /// </summary>
		public ComponentTemplate()
        {
            m_EnableVisibility = true;

            this.Error += new EventHandler(ComponentTemplate_Error);
            this.Init += new EventHandler(ComponentTemplate_Init);
            this.Load += new EventHandler(ComponentTemplate_Load);
            this.PreRender += new EventHandler(ComponentTemplate_PreRender);

            wim = new WimComponentRoot();
            wim.currentPage = this.Page;

            if (Context.Items.Contains("Wim.Component") && Context.Items["Wim.Component"] != null)
            {
                wim.Component = Context.Items["Wim.Component"] as Sushi.Mediakiwi.Data.Component;
            }

            if (wim.Component != null)
            {
                string x = wim.Component.TemplateLocation;
                //IsAjaxSetSoSkip = SetAjax();
            }
        }

        void ComponentTemplate_Load(object sender, EventArgs e)
        {
        }

        void ComponentTemplate_PreRender(object sender, EventArgs e)
        {
            if (wim.Component == null) return;
            if (Wim.CommonConfiguration.IS_AJAX_ENABLED && wim.Component.AjaxType > 0)
            {
                string idnotation;
                if (wim.Component.ID == 0)
                    idnotation = string.Concat("ct", wim.Component.TemplateID);
                else
                    idnotation = string.Concat("c", wim.Component.ID);

                FindAjaxControls(this.Controls, idnotation);
            }
        }

        void ComponentTemplate_Error(object sender, EventArgs e)
        {
            System.Exception ex = Server.GetLastError();
            string error = string.Format("Error occured in componentTemplate: {0}", this.ClientID);
            while (ex != null)
            {
                error += "<br/>- " + ex.Message;
                ex = ex.InnerException;
            }

            Sushi.Mediakiwi.Data.Notification.InsertOne(Sushi.Mediakiwi.Data.Notification.Tags.InternalWimError, error);
        }

        //  Step 1
        #region SetPageInformation
        private void SetPageInformation()
        {


            //  Objects.Page is passed on from HttpRewrite.HttpRewrite


            //this.Trace.Write("Wim", "ContentIsSet #1");
            if (Context.Items["Wim.stop"] != null)
            {
                wim.IsLoadedInWim = true;
                return;
            }

            if (Context.Items["Wim.Action"] != null)
            {
                wim.blockContentAddion = true;
            }

            if (wim.Page == null || wim.Site == null)
            {
                //  TODO GOT ERROR
                //this.Trace.Write("Wim", "ContentIsSet #2 NO PAGE OR SITE");
                return;
            }

            wim.IsLoadedInWim = (Context.Items["Wim.Preview"] != null && Context.Items["Wim.Preview"].ToString() == "1");

            //  Set content
            //this.Trace.Write("Wim", "ContentIsSet #3");
            SetContent();
        }

        internal bool SetAjax()
        {
            bool isCachable = Wim.CommonConfiguration.IS_AJAX_ENABLED && wim.Component.AjaxType > 0 && wim.Page.IsPageFullyCachable();
            if (isCachable)
            {
                try
                {
                    //wim.IsLoadedInAJAX = false;
                    if (!wim.IsLoadedInAJAX)
                    {
                        string url, idnotation;
                        if (wim.Component.ID == 0)
                        {
                            idnotation = string.Concat("ct", wim.Component.TemplateID);
                            url = string.Format("act={1}&i={2}", wim.Page.HRef, wim.Component.TemplateID, this.ID);
                        }
                        else
                        {
                            idnotation = string.Concat("c", wim.Component.ID);
                            url = string.Format("acc={1}&i={2}", wim.Page.HRef, wim.Component.ID, this.ID);
                        }

                        if (wim.Component.AjaxType == 1)
                        {
                            //return false;
                            LiteralControl lc = new LiteralControl(@"<div id=""result" + wim.Component.ID + @"""></div><script>$(document).ready(function () { $(""#result" + wim.Component.ID + @""").load(""?" + url + @"""); }); </script>");//string.Format("<input type=\"hidden\" class=\"_ReloadFromUrl auto_yes source_{1} target_{1}\" id=\"wim_{1}\" value=\"{0}\"><div id=\"{1}\"></div>", url, idnotation));
                            try
                            {
                                this.Controls.Clear();
                                this.Controls.Add(lc);
                                return true;
                            }
                            catch (Exception)
                            {
                                foreach (Control c in this.Controls)
                                    c.Visible = false;

                                this.Controls.Add(lc);
                                return true;
                            }

                        }
                        //else if (wim.Component.AjaxType == 2 || wim.Component.AjaxType == 3)
                        //{
                        //    HtmlGenericControl span = new HtmlGenericControl("span");
                        //    span.ID = string.Format("{0}", idnotation);

                        //    if (this.Parent != null && this.Parent.GetType() == typeof(System.Web.UI.WebControls.PlaceHolder))
                        //    {
                        //        int index = 0;
                        //        foreach (Control c in this.Parent.Controls)
                        //        {
                        //            if (c.ID == this.ID)
                        //                break;
                        //            index++;
                        //        }
                        //        this.Parent.Controls.AddAt(index, new LiteralControl(string.Format("<div id=\"{0}\">", idnotation)));
                        //        this.Parent.Controls.AddAt(index, new LiteralControl(string.Format("<input type=\"hidden\" class=\"source_{1} target_{1}\" id=\"wim_{1}\" value=\"{0}\">", url, idnotation)));
                        //        this.Parent.Controls.AddAt(index + 3, new LiteralControl("</div>"));
                        //    }
                        //    else
                        //    {
                        //        this.Controls.AddAt(0, new LiteralControl(string.Format("<div id=\"{0}\">", idnotation)));
                        //        this.Controls.AddAt(0, new LiteralControl(string.Format("<input type=\"hidden\" class=\"source_{1} target_{1}\" id=\"wim_{1}\" value=\"{0}\">", url, idnotation)));
                        //        this.Controls.Add(new LiteralControl("</div>"));
                        //    }

                        //}
                    }
                }
                catch (Exception ex)
                {
                    Sushi.Mediakiwi.Data.Notification.InsertOne("Ajax insertion", Sushi.Mediakiwi.Data.NotificationType.Warning, ex);
                }
            }
            return false;
        }

        bool IsAjaxSetSoSkip;

        void SetContent()
        {
            if (wim.Component == null) return;

            IsAjaxSetSoSkip = SetAjax();
            if (IsAjaxSetSoSkip) return;


            Page.Trace.Write("Wim", string.Concat("Loading component: ", wim.Component.TemplateLocation));

            Sushi.Mediakiwi.Data.Content content = Sushi.Mediakiwi.Data.Content.GetDeserialized(wim.Component.Serialized_XML);

            Page.Trace.Write("Wim", "1");

            Sushi.Mediakiwi.Framework.Templates.PropertySet pset = new Sushi.Mediakiwi.Framework.Templates.PropertySet();
            pset.SetValue(wim.Site, this, content, wim.Component, wim.Page);

            Page.Trace.Write("Wim", "2");

            //  For TriggerContentFill exclusion
            //this.Trace.Write("Wim", "ContentIsSet");
            wim.ContentIsSet = true;
        }
        #endregion SetPageInformation

        private void ComponentTemplate_Init(object sender, EventArgs e)
        {
            //HttpContext.Current.Trace.Write("Custom", "ComponentTemplate_Init");
            Page.Trace.Write("Wim", "ComponentTemplate_Init() #");
            this.SetPageInformation();
            Page.Trace.Write("Wim", "ComponentTemplate_Init() END #");
        }

        #region ComponentSave
        /// <summary>
        /// This event is triggered when the component is saved.
        /// </summary>
        protected event ComponentTemplateEventHandler ComponentSave;

        /// <summary>
        /// Raises the <see cref="E:ComponentSave"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentTemplateEventArgs"/> instance containing the event data.</param>
        protected virtual void OnComponentSave(ComponentTemplateEventArgs e)
        {
            if (ComponentSave != null) ComponentSave(this, e);
        }

        /// <summary>
        /// Does the component save.
        /// </summary>
        public void DoComponentSave()
        {
            Page.Trace.Write("Wim.Event", "Begin ComponentSave");
            OnComponentSave(new ComponentTemplateEventArgs());
            Page.Trace.Write("Wim.Event", "End ComponentSave");
        }

        /// <summary>
        /// Gets a value indicating whether this instance has component save.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has component save; otherwise, <c>false</c>.
        /// </value>
        public bool HasComponentSave
        {
            get { return (ComponentSave == null) ? false : true; }
        }
        #endregion ComponentSave
    }
}