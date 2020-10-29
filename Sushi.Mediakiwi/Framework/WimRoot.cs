using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.IO;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
//using System.Web.SessionState;
//using System.Web.UI;
//using System.Web.UI.WebControls;
//using System.Web.UI.HtmlControls;
using System.Xml;
using System.Xml.Serialization;

namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// 
    /// </summary>
    public class WimRoot
    {
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

        public Data.PageMapping PageMapping { get; set; }

        #region Site
        private Data.Site m_Site;
        /// <summary>
        /// 
        /// </summary>
        public Data.Site Site
        {
            set { m_Site = value; }
            get { return m_Site; }
        }
        #endregion Site

        #region Template (only set when called)
        /// <summary>
        /// 
        /// </summary>
        public Data.PageTemplate Template
        {
            get
            {
                if (m_Page != null)
                    return m_Page.Template;
                return null;
            }
        }
        #endregion

        #region Page
        private Data.Page m_Page;
        /// <summary>
        /// 
        /// </summary>
        public Data.Page Page
        {
            set { m_Page = value; }
            get { return m_Page; }
        }
        #endregion

        #region Folder (only set when called)
        /// <summary>
        /// Get the currently folder information.
        /// </summary>
        [Obsolete("This property can be found at wim.Page.Folder", false)]
        public Data.Folder Folder
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

        /// <summary>
        /// 
        /// </summary>
        public WimRoot()
        {
            Hooks = new Sushi.Mediakiwi.Framework.Hook();
        }

        //internal System.Web.UI.Page currentPage;
        /// <summary>
        /// 
        /// </summary>
        public Sushi.Mediakiwi.Framework.Hook Hooks;

        internal bool blockContentAddion = false;

        private void AddTrace(string message)
        {
        }

        private Data.Component[] m_Components;
        /// <summary>
        /// 
        /// </summary>
        public Data.Component[] Components
        {
            set { m_Components = value; }
            get { return m_Components; }
        }

        /// <summary>
        /// Gets or sets the current visitor.
        /// </summary>
        /// <value>The current visitor.</value>
        //public Data.Identity.IVisitor CurrentVisitor
        //{
        //    get { return Data.Identity.Visitor.Select(); }
        //    set { HttpContext.Current.Items["wim.visitor"] = value; }
        //}

        //public string CurrentUrl
        //{
        //    get { return HttpContext.Current.Items["Wim.Url"] as string; }
        //    set { HttpContext.Current.Items["Wim.Url"] = value; }
        //}

        /// <summary>
        /// Debugs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        //public void Debug(object message)
        //{
        //    if (message == null) return;
        //    string[] debugger = HttpContext.Current.Items["Wim.Debug"] as string[];
        //    if (debugger == null)
        //    {
        //        debugger = new string[1] { message.ToString() };
        //        HttpContext.Current.Items.Add("Wim.Debug", debugger);
        //    }
        //    else
        //    {
        //        string[] new_debugger = new string[debugger.Length + 1];
        //        debugger.CopyTo(new_debugger, 0);
        //        new_debugger[debugger.Length] = message.ToString();
        //        HttpContext.Current.Items["Wim.Debug"] = new_debugger;
        //    }
        //}

        /// <summary>
        /// Gets or sets the current profile.
        /// </summary>
        /// <value>The current profile.</value>
        //public Data.Identity.IProfile CurrentProfile
        //{
        //    get { return Data.Identity.Profile.Select(); }
        //    set { HttpContext.Current.Items["wim.profile"] = value; }
        //}

        /// <summary>
        /// Gets or sets the current application user.
        /// </summary>
        /// <value>The current application user.</value>
        //public Data.IApplicationUser CurrentApplicationUser
        //{
        //    get { return Data.ApplicationUserLogic.Select(); }
        //    set { HttpContext.Current.Items["wim.applicationuser"] = value; }
        //}

        /// <summary>
        /// Sets the cache variation.
        /// </summary>
        string GetCacheVariation(string keys)
        {
            //    if (keys == null) 
            return null;
            //    string candidate = null;
            //    foreach (string key in keys.Split(';'))
            //    {
            //        string value = currentPage.Request.Params[key];
            //        if (value == null) continue;

            //        if (candidate == null)
            //            candidate = string.Concat("?", key.ToLower(), "=", value);
            //        else
            //            candidate += string.Concat("&", key.ToLower(), "=", value);
            //    }
            //    return candidate;
        }

}
}
