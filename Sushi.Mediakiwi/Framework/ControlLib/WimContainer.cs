
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Data;
using System.Collections;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

namespace Sushi.Mediakiwi.Framework.ControlLib
{
    /// <summary>
    /// 
    /// </summary>
    [ToolboxData("<{0}:WimContainer runat='server'></{0}:WimContainer>")]
    public class WimContainer : System.Web.UI.Control
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }
        public int SortOrder { get; set; }

        /// <summary>
        /// CTor
        /// </summary>
        public WimContainer()
        {
            this.Init += new EventHandler(WimContainer_Init);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is legacy content tab.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is legacy content tab; otherwise, <c>false</c>.
        /// </value>
        public bool IsLegacyContentTab { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is legacy service column tab.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is legacy service column tab; otherwise, <c>false</c>.
        /// </value>
        public bool IsLegacyServiceColumnTab { get; set; }

        void WimContainer_Init(object sender, EventArgs e)
        {
            if (this?.IsLegacyContentTab == true)
                AddContent(this.ID, false, true);
            else if (this?.IsLegacyServiceColumnTab == true)
                AddContent(this.ID, true, true);
            else
                AddContent(this.ID, false);
        }

        Sushi.Mediakiwi.Data.Page m_Page;
        Sushi.Mediakiwi.Data.Page wimPage
        {
            get {

                if (HttpContext.Current == null)
                    return null;

                if (HttpContext.Current.Items["Wim.Page"] == null)
                    return null;

                m_Page = HttpContext.Current.Items["Wim.Page"] as Sushi.Mediakiwi.Data.Page;

                return m_Page; 
            }
        }


        Sushi.Mediakiwi.Data.Component[] Components { get; set; }

        void AddContent(string containerName, bool isSecundaryContentContainer, bool isLegacy = false)
        {
            ////  Introduced for CMS loading
            //if (blockContentAddion)
            //{
            //    AddTrace(string.Format("Content addition is blocked by the system."));
            //    return;
            //}

            if (Page == null || wimPage == null || wimPage.ID == 0)
                return;

            if (HttpContext.Current.Items["Wim.Preview"] == null)
            {
                var components = Sushi.Mediakiwi.Data.Component.SelectAll((wimPage.InheritContent ? wimPage.MasterID.Value : wimPage.ID));
                var componentsShared = Sushi.Mediakiwi.Data.Component.SelectAllShared1((wimPage.InheritContent ? wimPage.MasterID.Value : wimPage.ID));

                List<Sushi.Mediakiwi.Data.Component> list = new List<Data.Component>();

                foreach (var component in components)
                {

                    if (component.IsShared)
                    {
                        var shared = (from item in componentsShared where item.ComponentTarget == component.GUID select item).FirstOrDefault();
                        if (shared != null)
                        {
                            shared.Target = component.Target;
                            list.Add(shared);
                        }
                    }
                    else
                    {
                        list.Add(component);
                    }
                }
                Components = list.ToArray();
            }
            else
            {
                Sushi.Mediakiwi.Data.ComponentVersion[] componentEdited = Sushi.Mediakiwi.Data.ComponentVersion.SelectAllOnPage((wimPage.InheritContent ? wimPage.MasterID.Value : wimPage.ID));
                Sushi.Mediakiwi.Data.ComponentVersion[] componentEditedShared = Sushi.Mediakiwi.Data.ComponentVersion.SelectAllShared1((wimPage.InheritContent ? wimPage.MasterID.Value : wimPage.ID));

                List<Sushi.Mediakiwi.Data.Component> list = new List<Data.Component>();
                foreach (var component in componentEdited)
                {
                    if (!component.IsActive) continue;
                    if (component.IsShared)
                    {
                        var shared = (from item in componentEditedShared where item.ComponentTarget == component.GUID select item).FirstOrDefault();
                        if (shared != null) 
                        {
                            var tmp = shared.Convert2();
                            tmp.Target = component.Target;
                            list.Add(tmp);
                        }
                    }
                    else
                        list.Add(component.Convert2());
                }
                this.Components = list.ToArray();
            }

            //AddTrace(string.Format("Fetched {0} components from the system.", Components.Length));


            using (Wim.Utilities.CacheItemManager cman = new Wim.Utilities.CacheItemManager())
            {
                
                var visible = Data.Component.VerifyVisualisation(wimPage.Template, Components, containerName, ref isSecundaryContentContainer);
                foreach (Sushi.Mediakiwi.Data.Component component in visible)
                {
                    //if (containerName != component.Target)
                    //{
                    //    if (isLegacy && component.Target == null)
                    //    {
                    //        //  Do nothing
                    //    }
                    //    else
                    //    {
                    //        continue;
                    //    }
                    //}

                    ////  Legacy check
                    ////if (string.IsNullOrEmpty(containerName) && (!string.IsNullOrEmpty(component.FixedFieldName) || isSecundaryContentContainer != component.IsSecundary))
                    //if ((!string.IsNullOrEmpty(component.FixedFieldName) || isSecundaryContentContainer != component.IsSecundary))
                    //{
                    //    continue;
                    //}


                    string cachekey = string.Concat("Output.Page.", Page.ID, "$", component.ID, GetCacheVariation(component.VaryByParam));
                    if (!HttpContext.Current.Request.IsLocal)
                    {
                        if (cman.IsCached(cachekey))
                        {
                            string output = cman.GetItem(cachekey) as string;
                            if (!String.IsNullOrEmpty(output))
                                this.Controls.Add(new LiteralControl(output));
                            continue;
                        }
                    }

                    bool createLiteralEnding = false;
                    if (Wim.CommonConfiguration.IS_AJAX_ENABLED)
                    {
                        if (wimPage.IsPageFullyCachable())
                        {
                            if (component.AjaxType == 1 || component.AjaxType == 3)
                            {
                                try
                                {
                                    //LiteralControl lc = new LiteralControl(string.Format("<input type=\"hidden\" class=\"_ReloadFromUrl auto_yes source_c{0} target_c{0}\" id=\"wim_c{0}\" value=\"acc={0}\"><div id=\"c{0}\"></div>", component.ID));
                                    LiteralControl lc =
                                        new LiteralControl(string.Format("<placeholder id=\"ph{0}\"> ", component.ID));
                                    this.Controls.Add(lc);
                                    createLiteralEnding = true;
                                    //continue;
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(
                                        string.Format("Exception occurred for component {0}.",
                                                      component.TemplateLocation), ex);
                                }
                            }
                        }
                        if (component.AjaxType == 2 || component.AjaxType == 3)
                        {
                            try
                            {
                                LiteralControl lc1 = new LiteralControl(string.Format("<input type=\"hidden\" id=\"wim_c{0}\" value=\"acc={0}\"><div id=\"c{0}\">", component.ID));
                                LiteralControl lc2 =
                                    new LiteralControl(string.Format("<placeholder id=\"ph{0}\"> ", component.ID));
                                this.Controls.Add(lc1);
                                this.Controls.Add(lc2);
                                createLiteralEnding = true;
                                //continue;
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(
                                    string.Format("Exception occurred for component {0}.",
                                                  component.TemplateLocation), ex);
                            }
                        }

                    }

                    HttpContext.Current.Items["Wim.Component"] = component;
                    System.Diagnostics.Trace.WriteLine($"Load control [wc_ac]: {component.TemplateLocation}");
                    Control loadedControl = this.Page.LoadControl(Utility.AddApplicationPath(component.TemplateLocation));
                    System.Diagnostics.Trace.WriteLine($"control loaded [wc_ac]");

                    this.Controls.Add(loadedControl);

                    if (createLiteralEnding)
                    {
                        LiteralControl lc2 = new LiteralControl("</placeholder></div>");
                        this.Controls.Add(lc2);
                    }

                    Sushi.Mediakiwi.Framework.ComponentTemplate uc = loadedControl as Sushi.Mediakiwi.Framework.ComponentTemplate;
                    if (uc == null)
                    {
                        PartialCachingControl pcc = loadedControl as PartialCachingControl;
                        if (pcc != null)
                            uc = pcc.CachedControl as Sushi.Mediakiwi.Framework.ComponentTemplate;
                    }
                    if (uc != null)
                    {
                        if (component.CacheLevel == 1)
                        {
                            uc.CacheKey = cachekey;
                            uc.CacheDuration = Wim.CommonConfiguration.DefaultCacheTimeSpan;
                        }
                    }
                }
            }
        }

        string GetCacheVariation(string keys)
        {
            if (keys == null) return null;
            string candidate = null;
            foreach (string key in keys.Split(';'))
            {
                string value = this.Page.Request.Params[key];
                if (value == null) continue;

                if (candidate == null)
                    candidate = string.Concat("?", key.ToLower(), "=", value);
                else
                    candidate += string.Concat("&", key.ToLower(), "=", value);
            }
            return candidate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
        }
    }
}