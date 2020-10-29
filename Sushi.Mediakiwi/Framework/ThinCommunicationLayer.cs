using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Reflection;
using System.IO;

namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// 
    /// </summary>
    public class ThinCommunicationLayer : System.Web.UI.Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThinCommunicationLayer"/> class.
        /// </summary>
        public ThinCommunicationLayer()
        {
            this.Init += new EventHandler(ThinCommunicationLayer_Init);
            this.PreRender += new EventHandler(ThinCommunicationLayer_PreRender);
        }

     

        void ThinCommunicationLayer_PreRender(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Request.QueryString["channel"]))
                return;

            Utility.WriteSqlHistoryData(this);
        }

        void LoadItem()
        {
            this.ClientScript.RegisterHiddenField("autopostback", "");
            PlaceHolder uxContent = this.FindControl("uxContent") as PlaceHolder;
            Literal uxLayer = this.FindControl("uxLayer") as Literal;

            if (uxLayer != null)
            {
                uxLayer.Text = @"
        <div id=""popUpWithIframe"" class=""layerPopUp"">
            <div class=""popupShadow""></div>
            <div class=""popupCanvas"">
                <div class=""popupContent"">
                    <span style=""color:#FFFFFF;margin:5px;line-height:24px;font-size:110%;""></span>
                    <a href=""#title"" class=""closeLayerPopUp"">close</a>
                    <iframe src=""about:blank"" width=""899"" height=""460"" frameborder=""no"" scrolling=""auto"" name=""popup0frame"" id=""popup0frame"" allowtransparency=""true""></iframe>
                </div>
            </div>
        </div>";
            }


            int pageID = Wim.Utility.ConvertToInt(Request.QueryString["page"]);
            if (pageID > 0)
            {
                Sushi.Mediakiwi.Framework.CommunicationLayer.MetadataUpdate metadata = new Sushi.Mediakiwi.Framework.CommunicationLayer.MetadataUpdate();
                metadata.Initiate(uxContent, pageID);
            }
            
            Literal uxJavascriptPost = this.FindControl("uxJavascriptPost") as Literal;

            string arr = System.Configuration.ConfigurationManager.AppSettings["CuteEditorColorSet"];

            if (!string.IsNullOrEmpty(arr))
            {
                string[] array = arr.Split(',');
                StringBuilder build = new StringBuilder();
                foreach (string item in array)
                {
                    if (build.Length > 0)
                        build.Append(string.Format(",\"{0}\"", item));
                    else
                        build.Append(string.Format("\"{0}\"", item));
                }

                uxJavascriptPost.Text = string.Format("<script>CuteEditorColorArray = new Array({0});</script>", build.ToString());
            }


            Sushi.Mediakiwi.Beta.GeneratedCms.Monitor monitor = new Sushi.Mediakiwi.Beta.GeneratedCms.Monitor(HttpContext.Current.ApplicationInstance, true);
            monitor.Start();

            string xhtml = monitor.outputHTML.ToString();//.Replace("[controlcollection]", monitor.GlobalWimControlBuilder.ToString());
            string[] split = xhtml.Split(new string[] { "[controlcollection]" }, StringSplitOptions.None);

            if (split.Length == 2)
            {
                uxContent.Controls.Add(new LiteralControl(split[0]));

                if (monitor.GlobalWimControlBuilder != null && monitor.GlobalWimControlBuilder.Controls != null)
                {
                    foreach (System.Web.UI.Control c in monitor.GlobalWimControlBuilder.Controls)
                    {
                        uxContent.Controls.Add(c);
                    }
                }
                uxContent.Controls.Add(new LiteralControl(split[1]));
            }
            else
                uxContent.Controls.Add(new LiteralControl(xhtml));
            
            string className = null;
            if (monitor.m_Console.CurrentApplicationUser.IsNewInstance)
                className = "loginPage";
            else if (Request.QueryString["openinframe"] == "1" || Request.QueryString["openinframe"] == "2")
                className = "iframe";
            else if (!string.IsNullOrEmpty(Request.QueryString["dashboard"]))
                className = "homePage portalPage hgt_2";
            else if (Request.QueryString["adminfooter"] == "1")
                className = "adminfooter";

            Literal uxSkin = FindControl("uxSkin") as Literal;
            if (!string.IsNullOrEmpty(monitor.m_Console.CurrentApplicationUser.GetSkin()))
            {
                uxSkin.Text = string.Format(@"<link rel=""stylesheet"" href=""{0}/styles/{1}.css"" type=""text/css"" media=""all"" id=""reskinStyles"" />"
                    , Wim.Utility.AddApplicationPath("repository/wim")
                    , monitor.m_Console.CurrentApplicationUser.GetSkin()
                );
            }

            if (className != null)
            {
                HtmlGenericControl uxBody = FindControl("uxBody") as HtmlGenericControl;
                uxBody.Attributes["class"] = className;
            }
        }

        void AddJquery()
        {
            Literal uxSkin = FindControl("uxSkin") as Literal;
            uxSkin.Text += @"<script src=""http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.7.1.min.js"" type=""text/javascript""></script>";
            uxSkin.Visible = true;
        }

        void ThinCommunicationLayer_Init(object sender, EventArgs e)
        {
            if (Request.Params["action"] != null)
            {
                switch (Request.Params["action"])
                {
                    case "CleanRichtText":
                        string result;
                        string data = Request.Form.ToString();
                        string data2 = Server.UrlDecode(data);

                        Sushi.Mediakiwi.Framework.RichRext.Cleaner cleaner = new Sushi.Mediakiwi.Framework.RichRext.Cleaner();

                        result = cleaner.ApplyFullClean(Server.HtmlDecode(data2), true);

                        Response.Write(result);
                        Response.End();
                        return;

                }
            }
            else if (!string.IsNullOrEmpty(Request.QueryString["channel"]))
            {
                LoadItem();
                AddJquery();
                return;
            }
            else
            {
                AddJquery();
            }
      
            this.Visible = false;

            string urlCall = Request.Url.Query.ToLower();

            #region Sorting QueryString["list"]
            if (!string.IsNullOrEmpty(Request.QueryString["list"]))
            {
                int list = Utility.ConvertToInt(Request.QueryString["list"]);
                int sortF = Utility.ConvertToInt(Request.QueryString["sortF"]);
                int sortT = Utility.ConvertToInt(Request.QueryString["sortT"]);

                if (list > 0 && sortF > 0 && sortT > 0)
                {
                    Data.IComponentList implement = Data.ComponentList.SelectOne(list);

                    System.IO.FileInfo nfo = new System.IO.FileInfo(Server.MapPath(string.Concat(Request.ApplicationPath, "/bin/", implement.AssemblyName)));
                    Assembly assem = Assembly.LoadFrom(nfo.FullName);
                    Type m_LoadedType = assem.GetType(implement.ClassName);
                    IComponentListTemplate currentListInstance = (IComponentListTemplate)System.Activator.CreateInstance(m_LoadedType);
                    if (currentListInstance != null)
                    {
                        if (currentListInstance.wim.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.Property)
                        {
                            string cachekey = string.Concat("Data_Sushi.Mediakiwi.Data.Property.All$List_", currentListInstance.wim.CurrentList.ID);
                            Wim.Utilities.CacheItemManager.FlushIndexOfCacheObjects(cachekey);
                        }
                        if (currentListInstance.wim.m_sortOrderSqlTable.Contains("wim_Form"))
                        {
                            Wim.Utilities.CacheItemManager.FlushIndexOfCacheObjects("Data_FormElement_");
                        }

                        if (Sushi.Mediakiwi.Beta.GeneratedCms.Supporting.SortOrderUpdate.UpdateSortOrder(currentListInstance.wim.m_sortOrderSqlTable, currentListInstance.wim.m_sortOrderSqlColumn, currentListInstance.wim.m_sortOrderSqlKey, sortF, sortT))
                            Response.Write("OK");
                        else
                            Response.Write("ERROR#2");
                    }
                    else
                        Response.Write("ERROR#1");
                }
            }
            #endregion

            #region Tagged by Request.Headers["wim.sense"]
            if (!string.IsNullOrEmpty(Request.QueryString["sense"]))
            {
                string license = Request.QueryString["sense"];
                if (license != Sushi.Mediakiwi.Data.Environment.Current.Secret)
                {
                    Response.Write("-");
                    return;
                }

                CommunicationLayer.Sense implement = new Sushi.Mediakiwi.Framework.CommunicationLayer.Sense();

                int count = 0;
                int error = 0;
                int done = 0;
                string errorList = "";


                var subscriptions = Sushi.Mediakiwi.Data.Subscription.SelectAllActive();
                foreach (Sushi.Mediakiwi.Data.Subscription subscription in subscriptions)
                {
                    try
                    {
                        var user = Sushi.Mediakiwi.Data.ApplicationUser.SelectOne(subscription.UserID);
                        var clist = Sushi.Mediakiwi.Data.ComponentList.SelectOne(subscription.ComponentListID);
                        IComponentListTemplate template = clist.GetInstance();
                        template.wim.SendReport(template, user, subscription);

                        subscription.Scheduled = subscription.Scheduled.AddMinutes(subscription.IntervalType);
                        while (subscription.Scheduled.Ticks < DateTime.Now.Ticks)
                        {
                            subscription.Scheduled = subscription.Scheduled.AddMinutes(subscription.IntervalType);
                        }

                        subscription.Save();
                    }
                    catch (Exception ex)
                    {
                        Sushi.Mediakiwi.Data.Notification.InsertOne("wim.sense.subscription", Sushi.Mediakiwi.Data.NotificationType.Error, ex);
                        error++;
                    }
                }

                var list = Sushi.Mediakiwi.Data.ComponentList.SelectAllScheduled();
                if (list.Length > 0)
                {
                    DateTime now = DateTime.UtcNow;
                    foreach (var item in list)
                    {
                        if (item.SenseScheduled.Value.Ticks - now.Ticks <= 0)
                        {
                            try
                            {
                                implement.Initiate(this.Page, item);

                                item.SenseScheduled = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
                                item.SenseScheduled = item.SenseScheduled.Value.AddMinutes((double)item.SenseInterval.Value);
                                item.Save();
                                done++;
                            }
                            catch (Exception ex)
                            {
                                Sushi.Mediakiwi.Data.Notification.InsertOne("wim.sense", Sushi.Mediakiwi.Data.NotificationType.Error, ex);
                                errorList += string.Concat(" ", item.Name);
                                error++;
                            }
                        }
                    }
                }
                Response.Write(string.Format("L={0} D={1} E={2}{3}", count, done, error, errorList));
                return;
            }
            #endregion

            #region Statistics QueryString["negotiate"]
            else if (!string.IsNullOrEmpty(Request.QueryString["negotiate"]))
            {
                try
                {
                    string[] authenticode = Request.QueryString["negotiate"].Split(',');
                    string code = Server.UrlDecode(authenticode[0]).Replace(" ", "+");

                    Guid userGUID = Wim.Utility.ConvertToGuid(authenticode[2]);
                    Guid portalGUID = Wim.Utility.ConvertToGuid(authenticode[1]);
                    Guid originalPortalGUID = Wim.Utility.ConvertToGuid(authenticode[0]);

                    var user = Sushi.Mediakiwi.Data.ApplicationUser.SelectOne(userGUID);
                    var portal = Sushi.Mediakiwi.Data.Portal.SelectOne(portalGUID);
                    var originalPortal = Sushi.Mediakiwi.Data.Portal.SelectOne(originalPortalGUID);

                    //if (Sushi.Mediakiwi.Framework.ThinCommunicationService.ValidateAuthenticationCode(authenticode[2], appUser, portal))
                    Sushi.Mediakiwi.Data.ApplicationUserLogic.Apply(user.GUID, true);
                    //else
                    //    Sushi.Mediakiwi.Data.Notification.InsertOne("Portal Negotiation", string.Format("Portal: {1} for {0} [ID:{2}]", appUser.Name, portalGUID, portal.UserID));
                }
                catch (Exception ex)
                {
                    Sushi.Mediakiwi.Data.Notification.InsertOne("Portal Negotiation", ex);
                    Response.Redirect(Wim.Utility.AddApplicationPath("wim.ashx"));
                }
                Response.Redirect(Wim.Utility.AddApplicationPath("wim.ashx"));
            }
            #endregion

            #region Statistics QueryString["m"]
            else if (!string.IsNullOrEmpty(Request.QueryString["m"]))
            {
                try
                {
                    //if (Request.IsLocal) return;
                    if (Request.UrlReferrer == null) return;

                    int pageID = Wim.Utility.ConvertToInt(Request.QueryString["m"]);
                    Guid visitorGUID = Wim.Utility.ConvertToGuid(Request.QueryString["dc"]);

                    System.Uri url = new Uri(Request.QueryString["w"]);

                    Sushi.Mediakiwi.Data.Identity.IVisitor visitor;

                    if (visitorGUID == Guid.Empty)
                        visitor = Sushi.Mediakiwi.Data.Identity.Visitor.Select();
                    else
                        visitor = Sushi.Mediakiwi.Data.Identity.Visitor.Select(visitorGUID);

                    var user = Sushi.Mediakiwi.Data.ApplicationUserLogic.Select();

                    Sushi.Mediakiwi.Data.Page page = Sushi.Mediakiwi.Data.Page.SelectOne(pageID);

                    int logID = visitor.Data["wim_logid"].ParseInt().GetValueOrDefault();

                    bool isEntry = false;
                    bool isUnique;
                    bool isNewSession;
                    if (Request.QueryString["s"] == "2")
                    {
                        isNewSession = visitor.IsNewSession;
                        isUnique = visitor.IsNewVisitor;
                    }
                    else
                    {
                        isNewSession = Request.QueryString["s"] == "1";
                        isUnique = Request.QueryString["u"] == "1";
                    }

                    if (isNewSession || logID == 0)
                    {
                        Sushi.Mediakiwi.Data.Statistics.VisitorLog visit = new Sushi.Mediakiwi.Data.Statistics.VisitorLog();
                        visit.Agent = Request.UserAgent;
                        visit.Browser = string.Concat(Request.Browser.Browser, " ", Request.Browser.Version);
                        visit.Referrer = Request.QueryString["r"];
                        visit.VisitorID = visitor.ID;
                        visit.IsUnique = isUnique;
                        visit.HasCookie = Request.QueryString["c"] == "1";
                        if (string.IsNullOrEmpty(Wim.CommonConfiguration.LOAD_BALANCER_IP_HEADER))
                            visit.IP = Request.UserHostAddress;
                        else
                        {
                            visit.IP = Request.Headers[Wim.CommonConfiguration.LOAD_BALANCER_IP_HEADER];
                            if (string.IsNullOrEmpty(visit.IP))
                                visit.IP = Request.UserHostAddress;
                        }

                        visit.Save();
                        logID = visit.ID;
                        visitor.Data.Apply("wim_logid", logID);
                        isEntry = true;
                    }

                    int lastPageID = visitor.Data["wim_lastpageid"].ParseInt().GetValueOrDefault();

                    Guid urlguid = page.IsNewInstance ? Guid.Empty : page.Site.m_GUID;
                    Sushi.Mediakiwi.Data.Statistics.VisitorUrl visitedUrl = Sushi.Mediakiwi.Data.Statistics.VisitorUrl.SelectOne(urlguid, url.Host);
                    visitedUrl.Name = url.Host.ToString();
                    visitedUrl.GUID = urlguid;
                    if (visitedUrl.IsNewInstance)
                        visitedUrl.Save();

                    string path = Wim.Utility.RemApplicationPath(url.AbsolutePath);
                    Guid pageguid = page.IsNewInstance ? Guid.Empty : page.m_GUID;
                    Sushi.Mediakiwi.Data.Statistics.VisitorPage visitedPage = Sushi.Mediakiwi.Data.Statistics.VisitorPage.SelectOne(visitedUrl.ID, pageguid, path);
                    visitedPage.Name = path;
                    visitedPage.GUID = pageguid;
                    visitedPage.UrlID = visitedUrl.ID;

                    if (visitedPage.IsNewInstance)
                        visitedPage.Save();

                    Sushi.Mediakiwi.Data.Statistics.VisitorClick click = new Sushi.Mediakiwi.Data.Statistics.VisitorClick();
                    click.VisitorLogID = logID;
                    click.ProfileID = visitor.ProfileID.GetValueOrDefault();
                    click.ApplicationUserID = user.ID;
                    click.ItemID = page.ID;
                    click.RenderTime = Wim.Utility.ConvertToIntNullable(Request.QueryString["t"]);
                    //click.Url = page.HRef;
                    click.IsEntry = isEntry;
                    click.Entry = isEntry ? 1 : 0;
                    click.Data = visitor.Data;
                    click.Query = string.IsNullOrEmpty(url.Query) ? null : url.Query.Replace("?", string.Empty);
                    click.PageID = visitedPage.ID;
                    click.CampaignID = visitor.Data["campaign.id"].ParseInt();
                    click.Save();

                    visitor.Data.Apply("wim_lastpageid", page.ID);
                    visitor.Save();

                    return;
                }
                catch (Exception ex)
                {
                    Sushi.Mediakiwi.Data.Notification.InsertOne("Statistics", ex);
                }
                return;
            }
            #endregion

            #region Cross Domain Cookie QueryString["dc"]
            else if (!string.IsNullOrEmpty(Request.QueryString["dc"]))
            {
                try
                {
                    if (Request.IsLocal) return;
                    if (Request.UrlReferrer == null) return;

                    Guid visitorGUID = Wim.Utility.ConvertToGuid(Request.QueryString["dc"]);
                    Sushi.Mediakiwi.Data.Identity.IVisitor visitor = Sushi.Mediakiwi.Data.Identity.Visitor.Select(visitorGUID);
                    if (!visitor.IsNewInstance)
                        visitor.SetCookie();

                    Response.Clear();
                    this.Visible = false;
                    return;
                }
                catch (Exception ex)
                {
                    Sushi.Mediakiwi.Data.Notification.InsertOne("Domain cookie", Sushi.Mediakiwi.Data.NotificationType.Error, ex);
                }
            }
            #endregion
            else if (!string.IsNullOrEmpty(Request.QueryString["c"]))
            {
                PlaceHolder uxContent = this.FindControl("uxContent") as PlaceHolder;

                int[] split = Wim.Utility.ConvertToIntArray(Request.QueryString["c"].Split(','));
                foreach (var item in split)
                {
                    Sushi.Mediakiwi.Data.Component c = Sushi.Mediakiwi.Data.Component.SelectOne(item);
                    this.Items["Component"] = c;

                    Sushi.Mediakiwi.Data.Page p = Sushi.Mediakiwi.Data.Page.SelectOne(c.PageID.Value);

                    HttpContext.Current.Items["Wim.Page"] = p;
                    HttpContext.Current.Items["Wim.Site"] = p.Site;

                    System.Web.UI.Control uc = LoadControl(Wim.Utility.AddApplicationPath(c.TemplateLocation));
                    uc.ID = Request.QueryString["i"];
                    Sushi.Mediakiwi.Framework.ComponentTemplate ct = uc as Sushi.Mediakiwi.Framework.ComponentTemplate;
                    ct.wim.IsLoadedInAJAX = true;
                    ct.wim.Component = c;
                    ct.ID = string.Format("_c{0}", c.ID);

                    //this.Controls.Clear();
                    HtmlGenericControl span = new HtmlGenericControl("placeholder");
                    span.ID = string.Format("c{0}", c.ID);
                    m_FilterSpanID = span.ID;

                    span.Controls.Add(ct);
                    uxContent.Controls.Add(span);
                    this.Visible = true;
                }
            }
            else if (!string.IsNullOrEmpty(Request.QueryString["p"]))
            {
                PlaceHolder uxContent = this.FindControl("uxContent") as PlaceHolder;

                int pageID = Convert.ToInt32(Request.QueryString["p"]);
                m_AjaxStripper = new AjaxStripper(Request, pageID);
                m_AjaxStripper.ExtractComponentReferences();

                m_FilterSpanID = "dummy";
                this.Visible = true;

                HttpContext.Current.Items["Wim.Page"] = m_AjaxStripper.Page;
                HttpContext.Current.Items["Wim.Site"] = m_AjaxStripper.Page.Site;

                if (m_AjaxStripper.Components != null)
                {
                    foreach (var item in m_AjaxStripper.Components)
                    {
                        Sushi.Mediakiwi.Data.Component c = Sushi.Mediakiwi.Data.Component.SelectOne(item);

                        if (c.IsNewInstance) continue;
                        
                        this.Items["Component"] = c;

                        int index = c.TemplateLocation.LastIndexOf('/');
                        string filename = c.TemplateLocation.Substring(index + 1, c.TemplateLocation.Length - index - 6);

                        System.Web.UI.Control uc = LoadControl(Wim.Utility.AddApplicationPath(c.TemplateLocation));
                        uc.ID = string.IsNullOrEmpty(Request.QueryString["i"]) ? filename : Request.QueryString["i"];
                        Sushi.Mediakiwi.Framework.ComponentTemplate ct = uc as Sushi.Mediakiwi.Framework.ComponentTemplate;
                        //ct.wim.IsLoadedInAJAX = true;
                        ct.wim.Component = c;
                        ct.ID = string.Format("_c{0}", c.ID); 

                        //this.Controls.Clear();
                        //HtmlGenericControl span = new HtmlGenericControl("span");
                        HtmlGenericControl span = new HtmlGenericControl("placeholder");
                        span.ID = string.Format("ph{0}", c.ID);

                        span.Controls.Add(ct);
                        uxContent.Controls.Add(span);
                    }

                    int count = 0;
                    foreach (var item in m_AjaxStripper.FixedComponents)
                    {
                        count++;
                        string[] parts = item.Split(',');
                        System.Web.UI.Control uc = LoadControl(Wim.Utility.AddApplicationPath(parts[0]));

                        uc.ID = string.IsNullOrEmpty(Request.QueryString["i"]) ? parts[1] : Request.QueryString["i"];
                        Sushi.Mediakiwi.Framework.ComponentTemplate ct = uc as Sushi.Mediakiwi.Framework.ComponentTemplate;
                        ct.wim.IsLoadedInAJAX = true;
                        ct.wim.Component = null;

                        HtmlGenericControl span = new HtmlGenericControl("placeholder");
                        span.ID = string.Format("fixed{0}", count);

                        span.Controls.Add(ct);
                        uxContent.Controls.Add(span);
                    }
                }
            }
            else if (!string.IsNullOrEmpty(Request.QueryString["ct"]))
            {
                Sushi.Mediakiwi.Data.ComponentTemplate template = Sushi.Mediakiwi.Data.ComponentTemplate.SelectOne(Convert.ToInt32(Request.QueryString["ct"]));
                Sushi.Mediakiwi.Data.Component c = new Sushi.Mediakiwi.Data.Component();
                //this.Items["Component"] = c;

                Sushi.Mediakiwi.Data.Page p = Sushi.Mediakiwi.Data.Page.SelectOne(Convert.ToInt32(Request.QueryString["p"]));

                HttpContext.Current.Items["Wim.Page"] = p;
                HttpContext.Current.Items["Wim.Site"] = p.Site;

                System.Web.UI.Control uc = LoadControl(Wim.Utility.AddApplicationPath(template.Location));
                //uc.ID = Request.QueryString["i"];
                Sushi.Mediakiwi.Framework.ComponentTemplate ct = uc as Sushi.Mediakiwi.Framework.ComponentTemplate;
                ct.wim.IsLoadedInAJAX = true;
                ct.wim.Component = c;

                //this.Controls.Clear();
                HtmlGenericControl span = new HtmlGenericControl("span");
                span.ID = string.Format("ct{0}", template.ID);
                m_FilterSpanID = span.ID;

                PlaceHolder uxContent = this.FindControl("uxContent") as PlaceHolder;

                span.Controls.Add(ct);
                uxContent.Controls.Add(span);
                //this.Controls.Add(ct);

                this.Visible = true;
            }
        }

        string m_FilterSpanID;
        AjaxStripper m_AjaxStripper;

        /// <summary>
        /// 
        /// </summary>
        public class AjaxStripper
        {
            public Sushi.Mediakiwi.Data.Page Page { get; set; }
            int PageID;
            HttpRequest Request;
            /// <summary>
            /// Initializes a new instance of the <see cref="AjaxStripper"/> class.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <param name="pageID">The page ID.</param>
            public AjaxStripper(HttpRequest request, int pageID)
            {
                this.PageID = pageID;
                this.Request = request;

                this.PageID = Convert.ToInt32(Request.QueryString["p"]);
                this.Page = Sushi.Mediakiwi.Data.Page.SelectOne(PageID);

                System.Collections.Specialized.NameValueCollection nv = new System.Collections.Specialized.NameValueCollection();

                foreach (string key in request.QueryString.AllKeys)
                {
                    if (key == "p") continue;
                    nv.Add(key, request.QueryString[key]);
                }

                string file = this.Page.GetLocalCacheFile(nv);

                if (System.IO.File.Exists(file))
                {
                    this.PageOutput = System.IO.File.ReadAllText(file);
                    //this.PageOutput = AdminFooter.Footer.HTML(PageOutput, this.Page, true);
                }
            }

            /// <summary>
            /// Gets or sets the components.
            /// </summary>
            /// <value>The components.</value>
            public int[] Components { get; set; }
            /// <summary>
            /// Gets or sets the fixed components.
            /// </summary>
            /// <value>The fixed components.</value>
            public string[] FixedComponents { get; set; }
            /// <summary>
            /// Gets or sets the page output.
            /// </summary>
            /// <value>The page output.</value>
            public string PageOutput { get; set; }

            /// <summary>
            /// Extracts the component references.
            /// </summary>
            public void ExtractComponentReferences()
            {
                Regex rexPlaceholders = new Regex(@"((<placeholder id=""ph).*?("">))", RegexOptions.Multiline);
                if (string.IsNullOrEmpty(this.PageOutput)) return;
                MatchCollection collection = rexPlaceholders.Matches(this.PageOutput);
                this.Components = new int[collection.Count];
                for (int index = 0; index < collection.Count; index++)
                {
                    int value = Wim.Utility.ConvertToInt(collection[index].Value.Replace("<placeholder id=\"ph", string.Empty).Replace("\">", string.Empty));
                    this.Components[index] = value;
                }
                ExtractFixedComponentReferences();
            }

            void ExtractFixedComponentReferences()
            {
                Regex rexPlaceholders = new Regex(@"((<fixed template="").*?("">))", RegexOptions.Multiline);
                if (string.IsNullOrEmpty(this.PageOutput)) return;
                MatchCollection collection = rexPlaceholders.Matches(this.PageOutput);
                this.FixedComponents = new string[collection.Count];
                
                for (int index = 0; index < collection.Count; index++)
                {
                    string[] parts = collection[index].Value.Split(new string[] { "<fixed template=\"", "\" id=\"", "\">" }, StringSplitOptions.RemoveEmptyEntries);
                    string name = string.Concat(parts[0], ",", parts[1]);
                    this.FixedComponents[index] = name;
                        //collection[index].Value.Split(
                        //new string[] { "template=\"" }, StringSplitOptions.RemoveEmptyEntries)[1]
                        //.Split('"')[0]; 
                }
            }

            /// <summary>
            /// Replaces the placeholders.
            /// </summary>
            /// <param name="placeholderOutput">The placeholder output.</param>
            /// <param name="stripPlaceholderTag">if set to <c>true</c> [strip placeholder tag].</param>
            public void ReplacePlaceholders(string placeholderOutput, bool stripPlaceholderTag)
            {
                if (this.Components == null) return;
                foreach (var item in this.Components)
                {
                    Regex rexPlaceholder = new Regex(string.Format(@"((<placeholder id=""ph{0}"">)(.|\n)*?(</placeholder>))", item), RegexOptions.Multiline);

                    if (stripPlaceholderTag)
                    {
                        string match = rexPlaceholder.Match(placeholderOutput).Value
                            .Replace(string.Format(@"<placeholder id=""ph{0}"">", item), null)
                            .Replace("</placeholder>", null)
                            ;
                        this.PageOutput = rexPlaceholder.Replace(this.PageOutput, match);
                    }
                    else
                    {
                        string match = rexPlaceholder.Match(placeholderOutput).Value;
                        this.PageOutput = rexPlaceholder.Replace(this.PageOutput, match);
                    }
                }

                int count = 0;
                foreach (var item in this.FixedComponents)
                {
                    count++;

                    string[] parts = item.Split(',');
                    Regex rexPlaceholder2 = new Regex(string.Format(@"((<fixed template=""{0}"" id=""{1}"">)(.|\n)*?(</fixed>))", parts[0], parts[1]), RegexOptions.Multiline);
                    Regex rexPlaceholder1 = new Regex(string.Format(@"((<placeholder id=""fixed{0}"">)(.|\n)*?(</placeholder>))", count), RegexOptions.Multiline);

                    if (stripPlaceholderTag)
                    {
                        string match = rexPlaceholder1.Match(placeholderOutput).Value
                            .Replace(string.Format(@"<placeholder id=""fixed{0}"">", count), null)
                            .Replace("</placeholder>", null)
                            ;                            
                            //.Replace(string.Format(@"<fixed template=""{0}"">", item), null)
                            //.Replace("</fixed>", null)
                            //;
                        this.PageOutput = rexPlaceholder2.Replace(this.PageOutput, match);
                    }
                    else
                    {
                        string match = rexPlaceholder1.Match(placeholderOutput).Value;
                        this.PageOutput = rexPlaceholder2.Replace(this.PageOutput, match);
                    }
                }
            }
        }

        /// <summary>
        /// Initializes the <see cref="T:System.Web.UI.HtmlTextWriter"/> object and calls on the child controls of the <see cref="T:System.Web.UI.Page"/> to render.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> that receives the page content.</param>
        protected override void Render(HtmlTextWriter writer)
        {
            StringWriter stringwriter = new StringWriter();
            HtmlTextWriter htmlTextWriter = new HtmlTextWriter(stringwriter);
            base.Render(htmlTextWriter);

            StringBuilder build = stringwriter.GetStringBuilder();
            WimOutput outputCleaner = new WimOutput(ref build);
            string output = build.ToString();


            Sushi.Mediakiwi.Data.Identity.IVisitor visitor = Sushi.Mediakiwi.Data.Identity.Visitor.Select();

            if (!string.IsNullOrEmpty(Request.Params["autopostback"]))
                outputCleaner.Debug(string.Concat("autopostback: ", Request.Params["autopostback"]));

            Sushi.Mediakiwi.Data.Page page = visitor.LastVisitedPage;
            if (page == null)
            {
                page = HttpContext.Current.Items["Wim.Page"] as Sushi.Mediakiwi.Data.Page;
                if (page == null || page.IsNewInstance)
                {
                    Sushi.Mediakiwi.Data.Site site = Context.Items["Wim.Site"] as Sushi.Mediakiwi.Data.Site;
                    if (site != null && !site.IsNewInstance && site.HomepageID.HasValue)
                    {
                        page = Sushi.Mediakiwi.Data.Page.SelectOne(site.HomepageID.Value);
                    }
                }
            }

            //output = AdminFooter.Footer.HTML(output, page, visitor, false, true, null);
            
            if (!string.IsNullOrEmpty(m_FilterSpanID))
            {
                if (m_AjaxStripper == null)
                {
                    Regex rexPlaceholder = new Regex(@"((<placeholder)(.|\n)*(</placeholder>))", RegexOptions.Multiline);
                    Response.Write(rexPlaceholder.Match(output).Value);
                }
                else
                {
                    m_AjaxStripper.ReplacePlaceholders(output, true);

                    string result = m_AjaxStripper.PageOutput;
                    outputCleaner.AddScripting(visitor, m_AjaxStripper.Page.ID, ref result, false);
                    Response.Write(result);
                }
                Response.StatusCode = 200;
             
            }
            else
            {
                string target = Request.QueryString["axt"];
                if (!string.IsNullOrEmpty(target))
                {
                    Regex rex = new Regex(string.Format(@"(<fieldset(.|\n)*id=""{0}"">(.|\n)*</fieldset>)", target), RegexOptions.Multiline);
                    Response.Write(ReplaceAll(rex.Match(output).Value));
                    Response.StatusCode = 200;
                }
                else
                {
                    Response.Write(output);
             
                }
            }
            Response.Flush();
        }

        /// <summary>
        /// Replaces all.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns></returns>
        string ReplaceAll(string candidate)
        {
            candidate.Replace("></div>", "> </div>");
            candidate.Replace("></textarea>", "> </textarea>");
            return candidate;
        }
    }
}
