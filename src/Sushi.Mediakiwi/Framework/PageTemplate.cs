using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;

namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// 
    /// </summary>
	public class PageTemplate : System.Web.UI.Page
	{
        void PageTemplate_PreRenderComplete(object sender, EventArgs e)
        {
            wim.CurrentVisitor.Save();

            long startPageRendering = (long)Context.Items["wim.executionTime"];
            var renderTime = new TimeSpan(DateTime.Now.Ticks - startPageRendering).TotalSeconds;
            var renderTime2 = new TimeSpan(DateTime.Now.Ticks - startPageRendering).TotalMilliseconds;



            Utility.WriteSqlHistoryData(this);
        }

        /// <summary>
        /// Gets or sets a value indicating whether [ignore statistics].
        /// </summary>
        /// <value><c>true</c> if [ignore statistics]; otherwise, <c>false</c>.</value>
        public bool IgnoreStatistics { get; set; }

        private Regex m_bodyClose = new Regex(@"</body>", RegexOptions.IgnoreCase);
        private Regex m_formAction = new Regex(@"action="".*?""", RegexOptions.IgnoreCase);

        private Regex m_robots = new Regex(@"meta name=""robots""", RegexOptions.IgnoreCase);
        private string m_robotsPlacement = "<meta name=\"robots\" content=\"noindex\">";
        private Regex m_head = new Regex(@"</head>", RegexOptions.IgnoreCase);


        /// <summary>
        /// Initializes the <see cref="T:System.Web.UI.HtmlTextWriter"></see> object and calls on the child controls of the <see cref="T:System.Web.UI.Page"></see> to render.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"></see> that receives the page content.</param>
        protected override void Render(HtmlTextWriter writer)
        {
            if (wim.Site != null && !wim.Site.IsNewInstance)
            {
                System.Text.StringBuilder build = new System.Text.StringBuilder();
                using (System.IO.StringWriter x = new System.IO.StringWriter(build))
                {
                    using (HtmlTextWriter writer2 = new HtmlTextWriter(x))
                    {
                        base.Render(writer2);
                        writer2.Flush();
                        
                        WimOutput outputCleaner = new WimOutput(ref build);

                        string output = Wim.Utility.ApplyRichtextLinks(wim.Site, build.ToString());
                        string action = string.IsNullOrEmpty(wim.CurrentUrl) ? wim.Page.HRef : wim.CurrentUrl;
                        output = m_formAction.Replace(output, string.Format("action=\"{0}\"", action));

                        if (wim.Page != null && !wim.Page.IsSearchable && !m_robots.IsMatch(output))
                        {
                            output = m_head.Replace(output, $"{m_robotsPlacement}\n</head>");
                        }

                        if (Request.QueryString["Ignore"] == "Statistics")
                            IgnoreStatistics = true;

                        bool useCaching = wim.Page.AddToOutputCache;
                        bool useFileCaching = wim.Page.IsPageFullyCachable();

                        if (useFileCaching)
                            useCaching = true;

                        if (useCaching && !Wim.CommonConfiguration.UseLocalPageCache)
                            useCaching = false;

                        //  Preview avoid
                        if (Request.QueryString["preview"] == "1" && wim.CurrentApplicationUser != null && !wim.CurrentApplicationUser.IsNewInstance)
                            useCaching = false;

                        build = new StringBuilder();
           
                        if (useFileCaching)
                        {
                            Regex rexPlaceholderB = new Regex(@"((<placeholder)(.|\n)*?(>))", RegexOptions.Multiline);
                            Regex rexPlaceholderC = new Regex(@"((<fixed)(.|\n)*?(>))", RegexOptions.Multiline);

                            string responseWrite = rexPlaceholderB.Replace(output, string.Empty)
                                .Replace("</placeholder>", string.Empty);

                            responseWrite = rexPlaceholderC.Replace(responseWrite, string.Empty)
                                .Replace("</fixed>", string.Empty);

                            //if (!this.HideAdministrationNavigation) 
                            //    responseWrite = AdminFooter.Footer.HTML(responseWrite, wim.Page, wim.CurrentVisitor, useFileCaching, false, customNavigationItems);

                            if (!IgnoreStatistics)
                                outputCleaner.AddScripting(wim.CurrentVisitor, wim.Page.ID, ref responseWrite, false);

                            writer.Write(build.Append(responseWrite));
                        }
                        else
                        {
                            if (!IgnoreStatistics)
                                outputCleaner.AddScripting(wim.CurrentVisitor, wim.Page.ID, ref output, useFileCaching);

                            //if (!this.HideAdministrationNavigation) 
                            //    writer.Write(build.Append(AdminFooter.Footer.HTML(output, wim.Page, wim.CurrentVisitor, useFileCaching, false, customNavigationItems)));
                            //else
                                writer.Write(build.Append(output));
                        }

                        if (useCaching)
                        {
                            if (useFileCaching)
                            {
                                output = DeleteDCCode(output);

                                if (!Request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase))
                                {

                                    //if (System.Web.HttpContext.Current.Request.QueryString.Keys.Count > 0)
                                    //    output = m_formAction.Replace(output, string.Format("action=\"{0}?{1}\"", wim.Page.HRef, System.Web.HttpContext.Current.Request.QueryString.ToString()));
                                    //else
                                    //    output = m_formAction.Replace(output, string.Format("action=\"{0}\"", wim.Page.HRef));

                                    System.IO.TextWriter w = new StringWriter();

                                    // [MR:23-01-2020] Must not be used again
                                    //if (!System.IO.Directory.Exists(wim.Page.Folder.LocalCacheDirectory))
                                    //    System.IO.Directory.CreateDirectory(wim.Page.Folder.LocalCacheDirectory);

                                    try
                                    {
                                        string file = wim.Page.GetLocalCacheFile(Request.QueryString);
                                        using (TextWriter streamWriter = new StreamWriter(file, false, System.Text.Encoding.UTF8))
                                        {
                                            output = output.Replace("[tm]", "&trade;").Replace("[c]", "&copy;");
                                            streamWriter.Write(output);
                                            streamWriter.Flush();
                                            streamWriter.Close();
                                            streamWriter.Dispose();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Sushi.Mediakiwi.Data.Notification.InsertOne("File caching", Sushi.Mediakiwi.Data.NotificationType.Warning, ex);
                                    }
                                }
                            }
                        }
                        writer2.Close();
                        writer2.Dispose();
                    }
                    x.Close();
                    x.Dispose();
                }
            }
            else
            {
                base.Render(writer);
            }
        }

        private string DeleteDCCode(string output)
        {
            int locDCStart = output.LastIndexOf("<!-- DC -->");
            int locDCEnd = output.LastIndexOf("<!-- END DC -->") + "<!-- END DC -->".Length;
            if (locDCEnd > locDCStart && locDCStart > 0 && locDCStart < output.Length)
            {
                int count = locDCEnd - locDCStart;
                string toRemove = output.Substring(locDCStart, count);
                output = output.Replace(toRemove, "<!-- DCDeleted -->");
                return DeleteDCCode(output);
            }
            return output;
        }

   

        /// <summary>
        /// 
        /// </summary>
        protected WimRoot wim;

        #region CTor
        /// <summary>
        /// 
        /// </summary>
        public PageTemplate()
		{
            this.Error += new EventHandler(PageTemplate_Error);
            this.PreRenderComplete += new EventHandler(PageTemplate_PreRenderComplete);

            wim = new WimRoot();
            wim.currentPage = this.Page;

            this.Init += new EventHandler(PageTemplate_Init);
        }


        void PageTemplate_Error(object sender, EventArgs e)
        {

            System.Exception ex = Server.GetLastError();

            if (string.IsNullOrEmpty(Request.Headers["User-Agent"]))
            {
                Server.ClearError();
                return;
            }
            
            string error = null;
            if (wim.Page != null)
                error = string.Format("{2}<br/><br/><b>Page:</b><br/>{0} (ID:{1})", wim.Page.HRef, wim.Page.ID, Wim.Utility.GetHtmlFormattedLastServerError(ex));
            else
                error = Wim.Utility.GetHtmlFormattedLastServerError(ex);

            Sushi.Mediakiwi.Data.Notification.InsertOne(Sushi.Mediakiwi.Data.Notification.Tags.InternalWimError, error);

            try
            {
                var env = Sushi.Mediakiwi.Data.Environment.Current;

                Utilities.Errormailer mail =
                    new Utilities.Errormailer(this,
                    env.SmtpClient(),
                    env.DefaultMailAddress,
                    env.ErrorMailAddress,
                    string.Format("An error has occured at {0}", System.Environment.MachineName)
                    );
                mail.Send();
            }
            catch (Exception) { }
            finally
            {
                if (!this.Request.IsLocal)
                {
                    if (wim.Site != null && wim.Site.ErrorPageID.HasValue)
                    {
                        wim.Hooks.PageRedirect(wim.Site.ErrorPageID.Value);
                    }
                }
            }
        }
        #endregion CTor

        #region Init
        private void PageTemplate_Init(object sender, EventArgs e)
        {
            
            string scheme = Request.Url.Scheme.ToLower();
            //Response.Write(scheme);
            //Response.Write(Request.IsSecureConnection.ToString());
            //Response.Write(Request.Url.ToString());

            //if (page.IsSecure)
            //{
            //    if (scheme == "http" && !m_Application.Context.Request.IsLocal)
            //    {
            //        string full = page.HRefFull.Replace(scheme, "https");
            //        m_Application.Context.Response.Redirect(full, true);
            //    }
            //}
            //else
            //{
            //    if (scheme == "https")
            //    {
            //        string full = page.HRefFull.Replace(scheme, "http");
            //        m_Application.Context.Response.Redirect(full, true);
            //    }
            //}

            Page.Trace.Write( "Wim", "PageTemplate_Init()" );
            Page.EnableViewState = false;
            SetPageInformation();
            if (wim == null)
            {
                Page.Trace.Write("Wim", "wim object is not loaded");
                return;
            }

            if (wim.Page == null)
            {
                Page.Trace.Write("Wim", "wim.Page does not exist");
                return;
            }

            if (Request.QueryString["loadcomponentinstance"] == "1")
            {
                wim.IsLoadedInWim = true;
                SetWim_FixedComponent();
                return;
            }

            if (Page.Header != null)
            {
                string title =
                    wim.PageMapping != null ? wim.PageMapping.Title :
                    (wim.Page.Title == null ? wim.Site.DefaultPageTitle : wim.Page.Title);


                if (!string.IsNullOrEmpty(title))
                    Page.Header.Title = title.Trim();

                if (!string.IsNullOrEmpty(wim.Page.Description))
                    Page.Header.Controls.Add(new LiteralControl(string.Format("<meta name=\"description\" content=\"{0}\" />\n", wim.Page.Description)));
                
                if (!string.IsNullOrEmpty(wim.Page.Keywords))
                    Page.Header.Controls.Add(new LiteralControl(string.Format("<meta name=\"keywords\" content=\"{0}\" />\n", wim.Page.Keywords)));
                
                if (!string.IsNullOrEmpty(wim.Site.Culture))
                    Page.Header.Controls.Add(new LiteralControl(string.Format("<meta http-equiv=\"Content-Language\" content=\"{0}\" />\n", wim.Site.Culture)));
            }
            else
                Page.Trace.Write("Wim", "Page.Header does not exist");

            Page.Trace.Write("Wim", "PageTemplate_Init() END");

        }
        #endregion Init

        //  Step 1
        #region SetPageInformation
        private void SetPageInformation()
        {
            //  Objects.Page is passed on from HttpRewrite.HttpRewrite
            Sushi.Mediakiwi.Data.Page currentPage = (Sushi.Mediakiwi.Data.Page)Context.Items["Wim.Page"];
            Sushi.Mediakiwi.Data.Site currentSite = (Sushi.Mediakiwi.Data.Site)Context.Items["Wim.Site"];
            Sushi.Mediakiwi.Data.PageMapping currentMap = (Sushi.Mediakiwi.Data.PageMapping)Context.Items["Wim.PageMap"];

            if ( Context.Items["Wim.Action"] != null )
            {
                wim.blockContentAddion = true;
            }

            if ( currentPage == null || currentSite == null )
            {
                //  TODO GOT ERROR
                return;
            }

            if (currentSite.Culture != null && currentSite.Culture.Length > 0)
            {
                CultureInfo info = new CultureInfo(currentSite.Culture);
                if (!info.IsNeutralCulture)
                {
                    System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo(currentSite.Culture);
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo(currentSite.Culture);
                }
            }

            wim.Page = currentPage;
            wim.Site = currentSite;
            wim.PageMapping = currentMap;

            wim.IsLoadedInWim = (Context.Items["Wim.Preview"] != null && Context.Items["Wim.Preview"].ToString() == "1");

            if (wim.IsLoadedInWim)
                Page.ClientScript.RegisterClientScriptBlock(currentPage.GetType(), "FocusMe", Wim.Utility.WrapJavaScript("this.focus();"));
        }
        #endregion

        List<Sushi.Mediakiwi.Framework.ControlLib.WimContainer> m_FoundTabs;

        #region SetWim_FixedComponent
        private void ScanForHTMLForm(Sushi.Mediakiwi.Data.IAvailableTemplate[] availableTemplates, ControlCollection collection, int level, int pageTemplateId, bool addFixedComponentTemplate = true)
        {
            bool debug = this.IsDebug;


            foreach (Control controlItem in collection)
            {
                if (debug) Response.Write(string.Format("Control: {0} - {1}<br>", controlItem.ID, controlItem.GetType()));

                if (controlItem.GetType().ToString().ToLower().IndexOf("system") == 0)
                {
                    ScanForHTMLForm(availableTemplates, controlItem.Controls, (level + 1), pageTemplateId);
                    continue;
                }

                if (controlItem.GetType() == typeof(Sushi.Mediakiwi.Framework.ControlLib.WimContainer))
                {
                    if (m_FoundTabs == null)
                        m_FoundTabs = new List<ControlLib.WimContainer>();

                    m_FoundTabs.Add((Sushi.Mediakiwi.Framework.ControlLib.WimContainer)controlItem);
                    if (debug)
                        Response.Write(string.Format("WimContainer: <b>{0}</b><br/>", controlItem.GetType().BaseType.ToString()));
                    continue;
                }

                System.Type typeToValidate = controlItem.GetType();
                while (typeToValidate.ToString().ToLower().IndexOf("system") != 0)
                {
                    if (typeToValidate.BaseType == typeof(ComponentTemplate))
                    {
                        break;
                    }
                    typeToValidate = typeToValidate.BaseType;
                }

                if (debug)
                    Response.Write(string.Format("Validate: {0} - {1} ({2})<br>", controlItem.ID, typeToValidate.BaseType.ToString(), typeToValidate.ToString()));

                if (typeToValidate.BaseType == typeof(ComponentTemplate))
                {

                    Sushi.Mediakiwi.Data.ComponentTemplate template =
                        Sushi.Mediakiwi.Data.ComponentTemplate.SelectOne_BasedOnType(controlItem.GetType().BaseType);

                    if (debug) Response.Write(string.Format("<b>{0} ({1})</b><br/>", controlItem.GetType().BaseType.ToString(), template.IsNewInstance ? "[TEMPLATE DOES NOT EXIST]" : null));

                    if (!template.IsNewInstance)
                    {
                        Sushi.Mediakiwi.Framework.ComponentTemplate foundComponentTemplate = (Sushi.Mediakiwi.Framework.ComponentTemplate)controlItem;

                        if (debug) 
                            Response.Write(string.Format("- Component is known and is visible: {0}", foundComponentTemplate.EnableVisibility));

                        bool componentIsPresent = false;
                        foreach (Sushi.Mediakiwi.Data.AvailableTemplate availableTemplate in availableTemplates)
                        {
                            if (debug)
                            {
                                Response.Write(availableTemplate.PageTemplateID.ToString() + "<br/>");
                                Response.Write("- SCANNING FOR EXISTING COMPONENT '" + controlItem.ID.ToString() + "': " + availableTemplate.FixedFieldName + "<br>");
                            }
                            if (availableTemplate.FixedFieldName == controlItem.ID.ToString())
                            {
                                if (debug) Response.Write(" - Component exists");
                                //  Setting ID to -1 to validate existance!
                                componentIsPresent = foundComponentTemplate.EnableVisibility;
                                if (!foundComponentTemplate.EnableVisibility)
                                {
                                    if (debug) Response.Write("- Removing from visibility list");
                                    availableTemplate.Delete();
                                }
                                else
                                {
                                    //if (debug) Response.Write("- Just updating");
                                    availableTemplate.IsSecundary = template.IsSecundaryContainerItem;
                                    availableTemplate.IsPossible = true;
                                    availableTemplate.Checked = true;

                                    if (availableTemplate.Target != foundComponentTemplate.AssociatedContainerID)
                                    {
                                        availableTemplate.Target = foundComponentTemplate.AssociatedContainerID;
                                        Sushi.Mediakiwi.Data.ComponentVersion.UpdateContainerTargetByAvailableTemplate(availableTemplate.ID, foundComponentTemplate.AssociatedContainerID);
                                        if (debug) Response.Write(" - Updated AssociatedContainerID");
                                    }

                                    availableTemplate.Save();

                                }
                                //Sushi.Mediakiwi.Data.ComponentVersion.UpdateOne(currentComponent.Id, foundComponentTemplate.ServiceColumn, currentComponent.Serialized_XML);
                                break;
                            }
                        }
                        if (!componentIsPresent && foundComponentTemplate.EnableVisibility)
                        {
                            if (debug) Response.Write(" - Newly added component so had to add it");
                            //Sushi.Mediakiwi.Data.ComponentVersion.InsertOne(template.Id, null, wim.Page.Id, controlItem.ID.ToString(),
                            //    controlItem.ID.ToString(), null, true, true, foundComponentTemplate.ServiceColumn, 0);
                            Data.AvailableTemplate implement = new Sushi.Mediakiwi.Data.AvailableTemplate();
                            implement.ComponentTemplateID = template.ID;
                            implement.PageTemplateID = pageTemplateId;
                            implement.IsSecundary = template.IsSecundaryContainerItem;
                            implement.SortOrder = 1000;
                            implement.FixedFieldName = controlItem.ID.ToString();
                            implement.IsPossible = true;
                            implement.Save();
                        }
                        else
                        {
                            if (debug) Response.Write("- SKIPPING COMPONENT<br>");
                        }
                    }
                }
                ScanForHTMLForm(availableTemplates, controlItem.Controls, (level + 1), pageTemplateId);
            }
        }

        bool IsDebug;

        private void SetWim_FixedComponent()
        {
            bool refreshcode = false;
            if (wim.CurrentApplicationUser.IsDeveloper)
                refreshcode = Request.QueryString["refreshcode"] == "1";

            //if ( !wim.blockContentAddion ) return;
            IsDebug = false;
            Response.Clear();

            if (IsDebug)
                Response.Write("<b><font color=\"red\">Please press [F5] to see the normal WIM interface again!</font><br/><br/>Scanning for components...</b><hr/>");

            System.Diagnostics.Trace.WriteLine($"Metadata searcher started");

            Data.IAvailableTemplate[] availableTemplates = Sushi.Mediakiwi.Data.AvailableTemplate.SelectAll(wim.Page.TemplateID);
            this.ScanForHTMLForm(availableTemplates, this.Controls, 0, wim.Page.TemplateID);

            Sushi.Mediakiwi.Framework.CommunicationLayer.MetadataUpdate metadata = new Sushi.Mediakiwi.Framework.CommunicationLayer.MetadataUpdate();
            metadata.Initiate(this, wim.Page.ID, false, refreshcode);

            System.Diagnostics.Trace.WriteLine($"Metadata searcher ended");

            string tabularInfo = null;
            if (m_FoundTabs != null)
            {
                StringBuilder b = new StringBuilder();
                m_FoundTabs = (from item in m_FoundTabs orderby item.SortOrder select item).ToList();
                foreach (var item in m_FoundTabs)
                {
                    if (b.Length == 0)
                        b.Append(item.ID);
                    else
                        b.AppendFormat(",{0}", item.ID);

                    if (item.IsLegacyContentTab)
                        wim.Page.Template.Data.Apply("TAB.LCT", item.ID);
                    else if (item.IsLegacyServiceColumnTab)
                        wim.Page.Template.Data.Apply("TAB.LST", item.ID);

                    wim.Page.Template.Data.Apply(string.Format("T[{0}]", item.ID), item.Title);
                }
                tabularInfo = b.ToString();
            }
            wim.Page.Template.Data.Apply(string.Format("TAB.INFO"), tabularInfo);
            wim.Page.Template.Save();

            if (IsDebug)
                Response.Write("<br/><br/><b>Scanning for removals...</b><hr/>");
            foreach (Sushi.Mediakiwi.Data.AvailableTemplate template in availableTemplates)
            {
                if (string.IsNullOrEmpty(template.FixedFieldName)) 
                    continue;

                if (template.Checked)
                    continue;

                if (IsDebug)
                    Response.Write(string.Concat(template.FixedFieldName, "<br/>"));
                //Response.Write("- DELETING COMPONENT<br>");
                template.Delete();
            }

            var url = string.Concat(Wim.Utility.GetSafeUrl(Request).Split('?')[0], "?page=", wim.Page.ID);
            url = string.Concat("wim.ashx?page=", wim.Page.ID);

            System.Diagnostics.Trace.WriteLine($"Trying to redirect: {url}");

            if (IsDebug)
                Response.End();
            else
                Response.Redirect(url, false);
        }
        #endregion SetWim_FixedComponent
    }
}
