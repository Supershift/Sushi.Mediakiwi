using Sushi.Mediakiwi.Beta.GeneratedCms.Source;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Interfaces;
using Sushi.Mediakiwi.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sushi.Mediakiwi.Framework.Presentation
{
    public class Presentation : iPresentationMonitor
    {
        public string FileVersion { get; set; }
        public Presentation()
        {
            FileVersion = GetFileVersion();
        }

        //width=device-width, user-scalable=yes,initial-scale=0.1
        //const string VIEWPORT = "width=device-width, user-scalable=yes,initial-scale=0.1";// width=device-width, initial-scale=1";

        //public static byte[] ImageToByte2(System.Drawing.Image img, System.Drawing.Imaging.ImageFormat format)
        //{
        //    if (img == null) return null;
        //    byte[] byteArray = new byte[0];
        //    using (MemoryStream stream = new MemoryStream())
        //    {
        //        img.Save(stream, format);
        //        stream.Close();

        //        byteArray = stream.ToArray();
        //    }
        //    return byteArray;
        //}

        string GetLoaderHTML()
        {
            if (m_container.CurrentListInstance.wim.Page.Body.Grid._PreLoader == null)
            {
                //  Do nothing.
            }
            else if (m_container.CurrentListInstance.wim.Page.Body.Grid._PreLoader.Hide)
                return null;

            else if (!string.IsNullOrEmpty(m_container.CurrentListInstance.wim.Page.Body.Grid._PreLoader.HTML))
                return m_container.CurrentListInstance.wim.Page.Body.Grid._PreLoader.HTML;

            return @"
    <div class=""loader"" id=""loader""></div>";

        }

        Dictionary<CallbackTarget, List<ICallback>> m_Callbacks;
        Dictionary<GlobalPlaceholder, string> m_Placeholders;
        Beta.GeneratedCms.Console m_container;

        bool _IsLocalTest = false;// CommonConfiguration.IS_LOCAL_TEST;

        string _Domain = "https://sushi-mediakiwi.azureedge.net/";

        string FolderVersion(string subfolder = null)
        {
            return CommonConfiguration.CDN_Folder(m_container, subfolder);
        }

        string GetFileVersion()
        {
            if (string.IsNullOrWhiteSpace(CommonConfiguration.FILE_VERSION))
            {
                return Utils.FullVersion.Replace(".", "_");
            }

            return CommonConfiguration.FILE_VERSION;
        }

        /// <summary>
        /// Gets the template wrapper.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public string GetTemplateWrapper(Beta.GeneratedCms.Console container, Dictionary<GlobalPlaceholder, string> placeholders, Dictionary<CallbackTarget, List<ICallback>> callbacks, WimControlBuilder builder)
        {
            m_container = container;
            m_Placeholders = placeholders;
            m_Callbacks = callbacks;
           

            if (builder == null)
            {
                builder = new WimControlBuilder();
            }

            var url = container.Url;

            string report = string.Empty;
            if (!string.IsNullOrEmpty(container.CurrentListInstance.wim.Page.TMP_ReportingSection))
            {
                report = string.Concat(container.CurrentListInstance.wim.Page.TMP_ReportingSection, @"
	<article>
 		<figure data-highcharts-chart=""0"" id=""stockchart_0"" style="" margin: 0 auto""></figure>
	</article>
");
            }

            string pageTitle = null;
            string pageHeaderTitle = null;
            var helpButton = string.Empty;
            var helpScript = string.Empty;
            var configButton = string.Empty;

            if (container.CurrentListInstance.wim.CurrentApplicationUserRole.CanChangeList)
            {
                configButton = $@"<a href=""{container.UrlBuild.GetListPropertiesRequest()}"" class=""flaticon icon-gears""></a>";
            }

            if (container.CurrentListInstance.wim.m_ListTitle == string.Empty)
            {
                pageHeaderTitle = container.CurrentList.Name;
                pageTitle = $"{helpScript}{configButton}{helpButton}";
            }
            else
            {
                pageHeaderTitle = container.CurrentListInstance.wim.ListTitle;
                pageTitle = string.Format("\n\t\t\t\t\t\t{2}<h1>{0}{3}{1}</h1>", container.CurrentListInstance.wim.ListTitle, helpButton, helpScript, configButton);
            }

            if (!string.IsNullOrEmpty(pageTitle))
            {
                pageTitle = pageTitle.Replace("[user]", container.CurrentListInstance.wim.CurrentApplicationUser.Displayname);
                if (pageHeaderTitle != null)
                    pageHeaderTitle = pageHeaderTitle.Replace("[user]", container.CurrentListInstance.wim.CurrentApplicationUser.Displayname);
            }


            string title = null;
            string section_start = null;
            string section_end = null;
            string section = null;

            string description = null;
            if (!string.IsNullOrEmpty(container.CurrentListInstance.wim.ListDescription))
                description = string.Format("<p>{0}</p>", Utility.CleanLineFeed(container.CurrentListInstance.wim.ListDescription, true, true, true));

            if (builder != null && (container.CurrentListInstance.wim.Page.Body.ShowInFullWidthMode || container.CurrentList.Option_HideNavigation)
                )
            {
                builder.Canvas.LeftNavigation.Hide = true;
                builder.Leftnav = null;
            }

            if (builder != null)
            {
                if (builder.Canvas.Type == CanvasType.Explorer)
                {

                    builder.Canvas.LeftNavigation.Hide = true;
                    builder.Leftnav = null;

                    section_start = "\n\t\t\t\t<section id=\"thumbs\" class=\"style\">";

                    //cboxTopCenter
                    section_end = "\n\t\t\t\t</section>";

                    title = string.Format("\n\t\t\t\t\t<header>{0}\n\t\t\t\t\t\t<h1>{1}</h1>\n\t\t\t\t\t\t<hr>\n\t\t\t\t\t</header>"
                        , true ? "\n\t\t\t\t\t\t<a id=\"toggle\" class=\"arrowU\" href=\"#\"> </a>" : string.Empty
                        , pageTitle
                    );

                    if (!container.CurrentListInstance.wim.Page.HideDataForm)
                    {
                        if (builder.Formdata.Contains("<bottombuttonbar />"))
                            builder.Formdata = builder.Formdata.Replace("<bottombuttonbar />", builder.Bottom);
                        else
                            builder.Formdata = string.Concat(builder.Formdata, builder.Bottom);
                    }

                    builder.Formdata = string.Concat(builder.Notifications.ToString()
                        , container.CurrentListInstance.wim.Page.HideDataForm
                            ? null
                            : string.Concat(builder.Rightnav, string.Format("<h2>{0}</h2><hr />", "&nbsp;"), builder.Formdata, "")
                    );

                    if (container.CurrentListInstance.wim.HideTitle)
                        title = null;

                    section = string.Concat(section_start
                        , title
                        , builder.Tabularnav
                        , section_end
                        , container.CurrentListInstance.wim.Page.HideDataGrid || string.IsNullOrEmpty(builder.SearchGrid)
                            ? null
                            : string.Concat((true ? "\n\t\t\t\t<section id=\"gridsection\">" : "\n\t\t\t\t<section class=\"nowidth\">")
                                , builder.SearchGrid
                                , section_end)
                        );
                }
                else
                {
                    #region List information
                    //if (container.CurrentListInstance.wim.Page.HideTabs)
                    //{
                    #region CanvasType.ListInLayer
                    if (builder.Canvas.Type == CanvasType.ListInLayer || builder.Canvas.Type == CanvasType.ListItemInLayer)
                    {
                        //section_start = "\n\t\t\t\t<section id=\"thumbs\" class=\"component style\">";

                        ////cboxTopCenter
                        section_end = "\n\t\t\t\t</section>";
                        //title = string.Format("\n\t\t\t\t\t<header>\n\t\t\t\t\t\t{1}\n\t\t\t\t\t\t<hr>\n\t\t\t\t\t</header>"
                        //        , pageTitle
                        //        , builder.Rightnav
                        //    );

                        if (!container.CurrentListInstance.wim.Page.HideTopIconBar)
                            title = string.Concat(builder.Rightnav, "<hr/>");
                        else
                            title = string.Empty;

                        if (!container.CurrentListInstance.wim.Page.HideDataForm)
                        {
                            if (builder.Formdata.Contains("<bottombuttonbar />"))
                                builder.Formdata = builder.Formdata.Replace("<bottombuttonbar />", builder.Bottom);
                            else
                                builder.Formdata = string.Concat(builder.Formdata, builder.Bottom);
                        }

                        builder.Formdata = string.Concat(builder.Notifications.ToString()
                            , container.CurrentListInstance.wim.Page.HideDataForm ? null : builder.Formdata

                            );

                        //section = string.Concat(section_start, title, builder.Tabularnav, description, section_end, "\n\t\t\t\t<section id=\"gridsection\" class=\"component forms\">", builder.Formdata
                        //    , report
                        //    , container.CurrentListInstance.wim.Page.HideDataGrid
                        //        ? null
                        //        : builder.SearchGrid
                        //    , section_end);

                        if (container.CurrentListInstance.wim.Page.Body.Grid._GridAddition != null)
                        {
                            if (container.CurrentListInstance.wim.Page.Body.Grid._ClearGridBase)
                                builder.SearchGrid = container.CurrentListInstance.wim.Page.Body.Grid._GridAddition.ToString();
                            else
                                builder.SearchGrid += container.CurrentListInstance.wim.Page.Body.Grid._GridAddition.ToString();

                        }

                        //class=\"forms\" remove (27-1-14)
                        section = string.Concat("\n\t\t\t\t<section id=\"gridsection\"><div class=\"container\">", title, builder.Formdata
                            //, report
                            , container.CurrentListInstance.wim.Page.HideDataGrid
                                ? null
                                : builder.SearchGrid
                            , "</div>"
                            , section_end);


                    }
                    #endregion CanvasType.ListInLayer
                    else
                    {
                        //section_start = "\n\t\t\t\t<section id=\"folders\" class=\"component style\">";
                        section_start = "\n\t\t\t\t<section id=\"pageHeaderV2\" class=\"pageHeader\">";

                        //cboxTopCenter
                        section_end = "\n\t\t\t\t</section>";

                        #region Breadcrumbs
                        string bread = null;
                        string urlAddition = Logic.Navigation.GetQueryStringRecording(container);

                        bool hideBreadCrumb = CommonConfiguration.HIDE_BREADCRUMB;

                        if (!container.CurrentListInstance.wim.Page.HideBreadCrumbs
                            && !container.CurrentList.Option_HideBreadCrumbs && !hideBreadCrumb)
                        {
                            if (container.Group.HasValue)
                            {

                                var c = ComponentList.SelectOne(container.Group.Value);
                                var f = Folder.SelectOne(c.FolderID.GetValueOrDefault(0));

                                bread = string.Format(@"
    <div class=""trail"">
		<a href=""?list={1}{8}"" class=""back"">Back</a>
        <menu id=""breadCrumbs"">
		    <li><a href=""{0}"">Home</a> /</li>
		    <li><a href=""?folder={6}"">{7}</a> /</li>
		    <li><a href=""?list={1}{8}"">{3}</a> /</li>
		    <li><a href=""?list={1}&item={2}{8}"">{4}</a> /</li>
		    <li>{5}</li>
	    </menu>
	</div>
"
                                    , container.AddApplicationPath(CommonConfiguration.PORTAL_PATH)
                                    , container.Group.GetValueOrDefault() // 1
                                    , container.GroupItem.GetValueOrDefault() //2
                                    , c.Name //3
                                    , c.SingleItemName // 4
                                    , container.CurrentList.SingleItemName //5
                                    , f.ID //6
                                    , f.Name //7
                                    , urlAddition //8
                                    );

                            }
                            else if (container.Item.HasValue && container.ItemType == RequestItemType.Page)
                            {
                                var back = string.Concat("?folder=", container.CurrentListInstance.wim.CurrentFolder.ID);

                                string currentFolderName = container.CurrentListInstance.wim.CurrentFolder.Name;
                                if (currentFolderName == "/")
                                    currentFolderName = container.CurrentListInstance.wim.CurrentSite.Name;

                                pageHeaderTitle = container.CurrentPage.Name;
                                pageTitle = string.Format("\n\t\t\t\t\t\t<h1>{0}</h1>", pageHeaderTitle);
                                description = container.CurrentPage.Description;

                                bread = string.Format(@"
    <div class=""trail"">
		<a href=""{3}{5}"" class=""back"">Back</a>
        <menu id=""breadCrumbs"">
		    <li><a href=""{0}"">Home</a> /</li>
		    <li><a href=""?folder={1}"">{2}</a> /</li>
		    <li>{4}</li>
	    </menu>
	</div>
"
                                    , container.AddApplicationPath(CommonConfiguration.PORTAL_PATH)
                                    , container.CurrentListInstance.wim.CurrentFolder.ID //1
                                    , currentFolderName //2
                                    , back
                                    , container.CurrentPage.Name
                                    , urlAddition //5
                                    );
                            }
                            else if (container.CurrentList.Type == ComponentListType.PageProperties)
                            {
                                if (container.Item.GetValueOrDefault(0) == 0)
                                {
                                    Folder f = Folder.SelectOne(Utility.ConvertToInt(container.Request.Query["folder"]));

                                    var back = string.Concat("?folder=", f.ID);

                                    string currentFolderName = f.Name;
                                    if (currentFolderName == "/")
                                        currentFolderName = container.CurrentListInstance.wim.CurrentSite.Name;

                                    pageHeaderTitle = "New page";
                                    pageTitle = string.Format("\n\t\t\t\t\t\t<h1>{0}</h1>", pageHeaderTitle);

                                    bread = string.Format(@"
    <div class=""trail"">
		<a href=""{3}{4}"" class=""back"">Back</a>
        <menu id=""breadCrumbs"">
		    <li><a href=""{0}"">Home</a> /</li>
		    <li><a href=""?folder={1}"">{2}</a> /</li>
		    <li>New page</li>
	    </menu>
	</div>
"
                                        , container.AddApplicationPath(CommonConfiguration.PORTAL_PATH)
                                        , f.ID //1
                                        , currentFolderName //2
                                        , back
                                        , urlAddition //4
                                        );
                                }
                                else
                                {
                                    Page p = Page.SelectOne(container.Item.Value);

                                    var back = string.Concat("?folder=", p.FolderID);

                                    string currentFolderName = p.Folder.Name;
                                    if (currentFolderName == "/")
                                        currentFolderName = container.CurrentListInstance.wim.CurrentSite.Name;

                                    pageHeaderTitle = p.Name;
                                    pageTitle = string.Format("\n\t\t\t\t\t\t<h1>{0}</h1>", pageHeaderTitle);
                                    description = p.Description;

                                    bread = string.Format(@"
    <div class=""trail"">
		<a href=""{3}{5}"" class=""back"">Back</a>
        <menu id=""breadCrumbs"">
		    <li><a href=""{0}"">Home</a> /</li>
		    <li><a href=""?folder={1}"">{2}</a> /</li>
            <li><a href=""?page={6}"">{4}</a> /</li>
		    <li>Page properties</li>
	    </menu>
	</div>
"
                                        , container.AddApplicationPath(CommonConfiguration.PORTAL_PATH)
                                        , p.Folder.ID //1
                                        , currentFolderName //2
                                        , back
                                        , p.Name
                                        , urlAddition //5
                                        , p.ID
                                        );
                                }
                            }
                            else if (container.Item.HasValue)
                            {
                                var back = string.Concat("?list=", container.CurrentList.ID);
                                if (container.CurrentList.Type == ComponentListType.ComponentListProperties)
                                {
                                    if (container.Item.GetValueOrDefault(0) != 0)
                                        back = string.Concat("?list=", container.CurrentList.ID);
                                    else
                                        back = string.Concat("?folder=", container.CurrentListInstance.wim.CurrentFolder.ID);
                                }

                                string currentFolderName = container.CurrentListInstance.wim.CurrentFolder.Name;
                                bread = string.Format(@"
    <div class=""trail"">
		<a href=""{6}{7}"" class=""back"">Back</a>
        <menu id=""breadCrumbs"">
		    <li><a href=""{0}"">Home</a> /</li>
		    <li><a href=""?folder={4}"">{5}</a> /</li>
		    <li><a href=""?list={1}{7}"">{2}</a> /</li>
		    <li>{3}</li>
	    </menu>
	</div>
"
                                    , container.AddApplicationPath(CommonConfiguration.PORTAL_PATH)
                                    , container.CurrentList.ID
                                    , container.CurrentList.Name
                                    , container.CurrentList.SingleItemName
                                    , container.CurrentListInstance.wim.CurrentFolder.ID //4
                                    , currentFolderName //5
                                    , back //6
                                    , urlAddition //7
                                    );
                            }
                            else
                            {
                                if (container.CurrentList.Type == ComponentListType.Browsing)
                                {
                                    pageTitle = null;
                                    if (container.CurrentListInstance.wim.CurrentFolder.Name == "/")
                                    {
                                        bread = string.Format(@"
    <div class=""trail"">
        <menu id=""breadCrumbs"">
		    <li><a href=""{0}"">Home</a></li>
	    </menu>
	</div>
"
                                            , container.AddApplicationPath(CommonConfiguration.PORTAL_PATH)
                                            );
                                    }
                                    else
                                    {
                                        bread = string.Format(@"
    <div class=""trail"">
		<a href=""?folder={2}"" class=""back"">Back</a>
        <menu id=""breadCrumbs"">
		    <li><a href=""{0}"">Home</a> /</li>
		    <li>{1}</li>
	    </menu>
	</div>
"
                                            , container.AddApplicationPath(CommonConfiguration.PORTAL_PATH)
                                            , container.CurrentListInstance.wim.CurrentFolder.Name //1
                                            , container.CurrentListInstance.wim.CurrentFolder.ParentID.GetValueOrDefault(0) //2
                                            );
                                    }
                                }
                                else
                                {

                                    string currentFolderName = container.CurrentListInstance.wim.CurrentFolder.Name;
                                    if (currentFolderName == "/")
                                    {
                                        currentFolderName = container.CurrentListInstance.wim.CurrentSite.Name;
                                    }

                                    bread = string.Format(@"
    <div class=""trail"">
		<a href=""?folder={2}"" class=""back"">Back</a>
        <menu id=""breadCrumbs"">
		    <li><a href=""{0}"">Home</a> /</li>
		    <li><a href=""?folder={2}"">{3}</a> /</li>
		    <li>{1}</li>
	    </menu>
	</div>
"
                                        , container.AddApplicationPath(CommonConfiguration.PORTAL_PATH)
                                        , container.CurrentList.Name
                                        , container.CurrentListInstance.wim.CurrentFolder.ID //2
                                        , currentFolderName //3
                                        );
                                }
                            }
                        }



                        //<a href=\"#\" class=\"button\">Wijzigen</a>
                        #endregion Breadcrumbs

                        title = string.Format("\n\t\t\t\t\t<header>{0}{1}\n\t\t\t\t\t</header>{2}"
                            , bread, pageTitle, report
                        );

                        if (!string.IsNullOrEmpty(description))
                            title += string.Format("<article>{0}</article>", description);

                        // [CB:24-06-2015] Functionality regarding the bar to add 
                        // TODO check CanAddNewPageComponents property
                        if (container.CurrentPage != null)
                        {
                            var target = container.Request.Query["tab"].ToString();
                            if (string.IsNullOrEmpty(target))
                            {
                                var sections = container.CurrentPage.Template.GetPageSections();
                                if (sections != null)
                                    target = sections.FirstOrDefault();
                            }
                            AddPageComponentControl addpageComponent = new AddPageComponentControl(container.CurrentPage, container);

                            var isSecundary = false;
                            var components = ComponentVersion.SelectAll(container.CurrentPage.ID);
                            var visibleComponents = Data.Component.VerifyVisualisation(container.CurrentPage.Template, components, target, ref isSecundary, true);

                            addpageComponent.StartOpen = visibleComponents.Length == 0; //handig voor dev, hoef je dat ding niet steeds te openen

                            int slot = 0;
                            if (target == Data.CommonConfiguration.DEFAULT_CONTENT_TAB)
                            {
                                slot = 1;
                                target = null;
                            }

                            IAvailableTemplate[] availableComponentTemplates = null;

                            //if (container.CurrentPage.Template.IsSourceBased)
                            //{
                            //    availableComponentTemplates = Sushi.Mediakiwi.Data.AvailableTemplate.SelectAllBySlot(slot);
                            //}
                            //else
                            //{
                            availableComponentTemplates = AvailableTemplate.SelectAll(container.CurrentPage.TemplateID);
                            //}

                            var visibleAvailableComponents = Data.Component.VerifyVisualisation(container.CurrentPage.Template, availableComponentTemplates, target, ref isSecundary, true);


                            int selectableElements = 0;
                            foreach (var item in visibleAvailableComponents)
                            {
                                var ct = item.Template;
                                // TODO: filter out for the container 


                                var newPco = new PageComponentOption()
                                {
                                    ID = ct.ID,
                                    Icon = "align-left",
                                    Name = ct.Name,
                                    OnlyOneUsage = !ct.CanReplicate
                                };

                                if (!ct.CanReplicate)
                                {
                                    //  Check if it is present on the page
                                    var count = visibleComponents.Count(x => x.Template.ID == ct.ID);
                                    if (count > 0)
                                        newPco.HideOnInit = true;
                                    else
                                        selectableElements++;
                                }
                                else
                                    selectableElements++;

                                if (!(newPco.HideOnInit && ct.IsFixedOnPage))  // filter out fixed on page that are on page. 
                                    addpageComponent.Options.Add(newPco);
                            }
                            //foreach (var item in possibleComponentTemplates)
                            //{
                            //    Sushi.Mediakiwi.Data.ComponentTemplate ct = Sushi.Mediakiwi.Data.ComponentTemplate.SelectOne(item.ComponentTemplateID);
                            //    // TODO: filter out for the container 

                            //    if (item.Target == target  )
                            //    {
                            //        var newPco = new PageComponentOption()
                            //          {
                            //              ID = item.ComponentTemplateID,
                            //              Icon = "align-left",
                            //              Name = item.ComponentTemplate,
                            //              OnlyOneUsage = !ct.CanReplicate
                            //          };


                            //        if (!ct.CanReplicate)
                            //        {
                            //            //  Check if it is present on the page
                            //            var count = currentComponents.Count(x => x.TemplateID == ct.ID);
                            //            if (count > 0)
                            //                newPco.HideOnInit = true;
                            //            else
                            //                selectableElements++;
                            //        }
                            //        else
                            //            selectableElements++;

                            //        if (!(newPco.HideOnInit && ct.IsFixedOnPage))  // filter out fixed on page that are on page. 
                            //            addpageComponent.Options.Add(newPco);
                            //    }
                            //}
                            if (selectableElements > 0)
                                builder.Formdata = string.Concat(builder.Formdata, addpageComponent.RenderControl());

                        }
                        if (!container.CurrentListInstance.wim.Page.HideDataForm)
                        {
                            if (builder.Formdata.Contains("<bottombuttonbar />"))
                                builder.Formdata = builder.Formdata.Replace("<bottombuttonbar />", builder.Bottom);
                            else
                                builder.Formdata = string.Concat(builder.Formdata, builder.Bottom);
                        }

                        //builder.Formdata = string.Concat(builder.Notifications.ToString()
                        //        , builder.Formdata
                        //);
                        //builder.Formdata = builder.Formdata;
                        builder.Rightnav += builder.Notifications.ToString();

                        //  When there is no form, report or description the topsection looks empty, so add a shrink class
                        bool noTopSectionData = !builder.Formdata.Contains("table");// && string.IsNullOrEmpty(report);

                        string reportTop = null;
                        string reportMiddle = null;

                        //if (container.CurrentListInstance.wim.Page.ShowReportFirst)
                        //{
                        //    reportTop = string.Concat(@"<section id=""dash2"" class=""component"">", report, "</section>");
                        //}
                        //else
                        //    reportMiddle = string.Concat(@"<section id=""dash2"" class=""component"">", report, "</section>");

                        if (container.CurrentListInstance.wim.HideTitle)
                            title = null;

                        if (string.IsNullOrEmpty(title))
                            section = string.Concat(reportTop
                            , builder.Rightnav);
                        else
                            section = string.Concat(reportTop, section_start
                                , title
                                , section_end
                                , builder.Rightnav);

                        if (!string.IsNullOrEmpty(builder.Formdata))
                        {
                            bool isListItem = (builder.Canvas.Type == CanvasType.ListItem
                                || builder.Canvas.Type == CanvasType.ListItemInLayer
                                );

                            section += string.Concat(
                                    isListItem
                                    ? "\n\t\t\t\t<section id=\"formStylesv2\" class=\"component forms\"><div class=\"container\">"
                                    : string.Format("\n\t\t\t\t<section id=\"formFilter\" class=\"formfilters\" style=\"display:{0}\">", container.CurrentListInstance.wim.Page.HideFormFilter ? "none" : "block")
                                , builder.Formdata
                                , isListItem
                                    ? "\n\t\t\t\t</div>"
                                    : string.Empty
                                , section_end
                                , reportMiddle);
                        }

                        if (container.CurrentListInstance.wim.Page.Body.Grid._GridAddition != null)
                        {
                            if (container.CurrentListInstance.wim.Page.Body.Grid._ClearGridBase)
                                section += container.CurrentListInstance.wim.Page.Body.Grid._GridAddition.ToString();
                            else
                                section +=
                                    container.CurrentListInstance.wim.Page.Body.Grid._GridAddition.ToString();
                        }
                        else
                        {
                            section += string.Concat(
                                    container.CurrentListInstance.wim.Page.HideDataGrid || string.IsNullOrEmpty(builder.SearchGrid)
                                    ? null
                                    : string.Concat(string.Format("\n\t\t\t\t<section id=\"datagrid\" class=\"{1}{0}\">"
                                            , container.CurrentListInstance.wim.CurrentList.Option_SearchAsync ? " async" : string.Empty
                                            , container.CurrentListInstance.wim.Page.Body.Grid.ClassName)
                                        , builder.SearchGrid
                                        , section_end
                                        )
                                );
                        }
                    }
                    #endregion List information
                }
            }

            string candidate = null;

            string styleAddition = GetStyleAddition();

            string addedHead = string.Empty;
            if (container.CurrentListInstance.wim.Page.Head._HeadAddition != null)
                addedHead += container.CurrentListInstance.wim.Page.Head._HeadAddition.ToString();


            if (m_container.CurrentListInstance.wim.Page.Head.EnableColorCodingLibrary)
            {
                if (m_container.CurrentListInstance.wim.HeaderScript == null)
                    m_container.CurrentListInstance.wim.HeaderScript = string.Empty;
                

                m_container.CurrentListInstance.wim.HeaderScript += $@"
        <link type=""text/css"" rel=""stylesheet"" href=""{FolderVersion("scripts/codemirror")}codemirror.min.css?v={FileVersion}"" />
        <script type=""text/javascript"" src=""{FolderVersion("scripts/codemirror")}codemirror.min.js?v={FileVersion}""></script>
        <script type=""text/javascript"" src=""{FolderVersion("scripts/codemirror")}codemirror.formatting.min.js?v={FileVersion}""></script>
        <script type=""text/javascript"" src=""{FolderVersion("scripts/codemirror/mode/clike")}clike.min.js?v={FileVersion}""></script>
        <script type=""text/javascript"" src=""{FolderVersion("scripts/codemirror/mode/xml")}xml.min.js?v={FileVersion}""></script>
        <script type=""text/javascript"" src=""{FolderVersion("scripts/codemirror/mode/htmlmixed")}htmlmixed.js?v={FileVersion}""></script>";
            }

            //"width=device-width, user-scalable=yes,initial-scale=0.1"
            string viewport = CommonConfiguration.VIEWPORT;

            #region Open In Frame > 0
            if (container.OpenInFrame > 0)
            {
                #region layerHead
                string layerHead = @"
		<meta charset=""UTF-8"" />
		<title>" + pageHeaderTitle + @"</title>
        <meta http-equiv=""X-UA-Compatible"" content=""IE=Edge"" />
        <meta name=""viewport"" content=""" + viewport + $@""">
		<!--[if IE]>
			<meta http-equiv=""imagetoolbar"" content=""no""/>
			<script type=""text/javascript"" src=""{FolderVersion("scripts")}html5.js?v={FileVersion}""></script>
		<![endif]-->
		<meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />"
        + GetHeader(styleAddition, (container.View == (int)ContainerView.ItemSelect && container.CurrentList.Option_FormAsync)) + addedHead + m_container.CurrentListInstance.wim.HeaderScript + container.CurrentListInstance.wim.Page.Body.Form.Elements.GetHeaderStyle();

                if (container.CurrentListInstance.wim.Page.Head._ClearHeadBase)
                    layerHead = string.Empty;

                //if (container.CurrentListInstance.wim.Page.Head._HeadAddition != null)
                //    layerHead += container.CurrentListInstance.wim.Page.Head._HeadAddition.ToString();
                if (string.IsNullOrEmpty(m_container.CurrentListInstance.wim.HeaderScript))
                    layerHead += m_container.CurrentListInstance.wim.HeaderScript;
                #endregion layerHead

                if (!container.CurrentListInstance.wim.Page.Body._ClearBodyBase)
                {
                    if (container.CurrentListInstance.wim.Page.Body._BodyAddition != null)
                        section += container.CurrentListInstance.wim.Page.Body._BodyAddition.ToString();
                }

                #region layerBody
                string layerBody = @"
    <form id=""uxForm"" method=""post"" action=""" + url + @""" enctype=""multipart/form-data"">
        <input type=""hidden"" name=""autopostback"" id=""autopostback"" value="""" />
		<section id=""popupContent"">
	        <article>
                " + section + @"
	        </article>
            <div class=""clear""></div>
		</section>" + GetLoaderHTML() + @"
		<footer class=""bodyFooter""></footer>" + m_container.CurrentListInstance.wim.OnSaveScript + m_container.CurrentListInstance.wim.OnDeleteScript + @"
    </form>
";
                if (container.CurrentListInstance.wim.Page.Body._ClearBodyBase)
                {
                    layerBody = string.Empty;
                    if (container.CurrentListInstance.wim.Page.Body._BodyAddition != null)
                        layerBody = container.CurrentListInstance.wim.Page.Body._BodyAddition.ToString();
                }



                #endregion layerBody

                string addedBodyClass = string.IsNullOrEmpty(container.CurrentListInstance.wim.Page.Body.ClassName)
                  ? string.Empty
                  : string.Concat(" ", container.CurrentListInstance.wim.Page.Body.ClassName);

                candidate = @"<!DOCTYPE html>
<html>
	<head>
		" + layerHead + @"
	</head>
	<body class=""popupPage" + addedBodyClass + @""">
    " + layerBody + @"
	</body>
</html>";
            }
            #endregion Open In Frame > 0
            else
            {
                string bodyAddition =
                    container.CurrentListInstance.wim.Page.Body._BodyAddition == null ? string.Empty :
                    container.CurrentListInstance.wim.Page.Body._BodyAddition.ToString();
                ;

                if (container.CurrentListInstance.wim.Page.Body._BodyTarget == Body.BodyTarget.Nested)
                    section += bodyAddition;

                string navigation = builder?.Leftnav;

                if (container.CurrentListInstance.wim.Page.Body.Navigation.Side._ClearBodyBase)
                    navigation = string.Empty;

                if (container.CurrentListInstance.wim.Page.Body.Navigation.Side._BodyAddition != null)
                {
                    if (container.CurrentListInstance.wim.Page.Body.Navigation.Side._BodyTarget == SideNavigation.SideNavigationTarget.Below)
                    {
                        navigation += container.CurrentListInstance.wim.Page.Body.Navigation.Side._BodyAddition.ToString();
                    }
                    else
                    {
                        navigation = container.CurrentListInstance.wim.Page.Body.Navigation.Side._BodyAddition.ToString() + navigation;
                    }
                }


                string homeContent = @"
			<section id=""homeContent"">
                " + navigation + @"
	            <article id=""homeArticle"">
                    " + section + @"
	            </article>
                <div class=""clear""></div>
            </section>
";
                if ((container.View == (int)ContainerView.ItemSelect && container.CurrentList.Option_FormAsync))
                {
                    homeContent = @"
			<section id=""homeContent"">
                " + navigation + @"
			<div id=""mediakiwi""></div>
            <style>
                v-container.half {
                    display: inline-block;
                    width: 48%;
                }
            </style>
                <div class=""clear""></div>
            </section>
";
                }

                if (container.CurrentListInstance.wim.Page.Body._BodyTarget == Body.BodyTarget.Below)
                    homeContent += bodyAddition;

                string homeContentOutsideFrame = string.Empty;

                #region Framed content
                //  Set an 100% iframe in the page; independed of the set width
                if (!string.IsNullOrEmpty(container.CurrentListInstance.wim.Page.Body._FrameUrl))
                {
                    string frameSection = @"
			<section id=""homeContent"" class=""framed"">
                <iframe src=""" + container.CurrentListInstance.wim.Page.Body._FrameUrl + @""" class=""framed"" frameBorder=""0""></iframe>
            </section>
";

                    homeContent = frameSection;
                }
                else if (container.CurrentListInstance.wim.Page.Body._ClearBodyBase)
                {
                    homeContent = string.Empty;
                    if (container.CurrentListInstance.wim.Page.Body._BodyAddition != null)
                    {
                        homeContent = @"
			<section id=""homeContent"">
                " + navigation + @"
	            <article id=""homeArticle" + (container.CurrentListInstance.wim.Page.Body.ShowInFullWidthMode ? " fill" : "") + @""">
                    " + bodyAddition + @"
	            </article>
                <div class=""clear""></div>
            </section>
";
                    }
                }
                //else if (container.CurrentListInstance.wim.Page.Body._BodyAddition != null)
                //{
                //    if (container.CurrentListInstance.wim.Page.Body._BodyAddition != null)
                //        homeContent += container.CurrentListInstance.wim.Page.Body._BodyAddition.ToString();
                //}
                #endregion Framed content

                string usernameLink = m_container.CurrentApplicationUser.Displayname;
                if (!m_container.CurrentVisitor.Data["Wim.Reset.Me"].IsNull)
                {
                    usernameLink = string.Format("{0} (<a href=\"?reset\">reset</a>)", usernameLink);
                }

                string addedBodyClass = string.IsNullOrEmpty(container.CurrentListInstance.wim.Page.Body.ClassName)
                    ? string.Empty
                    : string.Concat(" ", container.CurrentListInstance.wim.Page.Body.ClassName);

                //  [30.okt.14:MM] Added full width support
                //<link rel=""stylesheet"" href=""" + FolderVersion("styles") + @"jquery-ui-1.8.16.custom.css"" type=""text/css"" media=""all"" />

                if (m_container.ClientRedirectionUrl != null)
                {
                    m_container.CurrentListInstance.wim.OnSaveScript = @"<script type=""text/javascript"">window.location.replace('" + m_container.ClientRedirectionUrl + @"');</script>";
                    if (m_container.ClientRedirectionUrlOnEmpty)
                    {
                        return @"<!DOCTYPE html>
<html>
	<head>
		<meta charset=""UTF-8"" />
		<title>" + pageHeaderTitle + @"</title>
        <meta http-equiv=""X-UA-Compatible"" content=""IE=Edge"" />
        <meta name=""viewport"" content=""" + viewport + $@""">
		<!--[if IE]>
			<meta http-equiv=""imagetoolbar"" content=""no""/>
			<script type=""text/javascript"" src=""{FolderVersion("scripts")}html5.js?v={FileVersion}""></script>
		<![endif]-->
		<meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
	</head>
	<body class=""" + (builder.Canvas.LeftNavigation.Hide ? "full" : null) + addedBodyClass
                            + @""">
    <form id=""uxForm"" method=""post"" action=""" + url + @""" enctype=""multipart/form-data"">
        <input type=""hidden"" name=""autopostback"" id=""autopostback"" value="""" />
		<footer class=""bodyFooter""></footer><script type=""text/javascript"">window.location.replace('" + m_container.ClientRedirectionUrl + @"');</script>
    </form>
	</body>
</html>";

                    }
                }

                candidate = @"<!DOCTYPE html>
<html>
	<head>
		<meta charset=""UTF-8"" />
		<title>" + pageHeaderTitle + @"</title>
        <meta http-equiv=""X-UA-Compatible"" content=""IE=Edge"" />
        <meta name=""viewport"" content=""" + viewport + $@""">
		<!--[if IE]>
			<meta http-equiv=""imagetoolbar"" content=""no""/>
			<script type=""text/javascript"" src=""{FolderVersion("scripts")}html5.js?v={FileVersion}""></script>
		<![endif]-->
		<meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />"
        + GetHeader(styleAddition, (container.View == (int)ContainerView.ItemSelect && container.CurrentList.Option_FormAsync)) + addedHead + m_container.CurrentListInstance.wim.HeaderScript + container.CurrentListInstance.wim.Page.Body.Form.Elements.GetHeaderStyle() + @"
	</head>
	<body class=""" + (builder.Canvas.LeftNavigation.Hide ? "full" : null) + addedBodyClass
                    + @""">
    <form id=""uxForm"" method=""post"" action="""+url+ @""" enctype=""multipart/form-data"">
        <input type=""hidden"" name=""autopostback"" id=""autopostback"" value="""" />
		<section id=""bodySection"">
			<header id=""bodyHeader"">
				<a id=""logo"" href=""" + container.UrlBuild.GetHomeRequest() + @"""><img src=""" + LogoUrl(container) + @""" /></a>
			</header>" + homeContent + @"
		    <nav id=""bodyNav"">
			    " + Get_component_mainMenu(builder.TopNavigation) + @"
		    </nav>" +
            @"
<section class=""component loginNav"" id=""loginNav"">
	<ul>
		" + Get_component_channelNav() + @" 
		<li>
			<a class=""side active"" href=""#""><span class=""icon-person""></span></a>
			<div class=""channel"" style=""display: none;"">
				<a class=""active"" href=""#""><span class=""icon-person""></span></a>
				<h3>My profile</h3>
				<img alt=""noName"" src=""" + FolderVersion("images") + @"noName.png"">
				<strong>" + usernameLink + @"</strong><br/>
				<span>" + container.CurrentApplicationUser.Role().Name + @"</span>"
                        + (!CommonConfiguration.MY_PROFILE_LIST_ID.HasValue
                            ? string.Empty
                            : string.Format(@"<a href=""?list={0}"" class=""submit left"">My account</a>", CommonConfiguration.MY_PROFILE_LIST_ID.GetValueOrDefault())
                            )
                        + @"
				<a class=""submit"" id=""logout"" href=""?logout"">Logout</a>
			</div>
		</li>
	</ul>
</section>" + @"
        </section>" + homeContentOutsideFrame + GetLoaderHTML() + @"
		<footer class=""bodyFooter""></footer>" + m_container.CurrentListInstance.wim.OnSaveScript + m_container.CurrentListInstance.wim.OnDeleteScript + @"
    </form>
	</body>
</html>";

            }

            if ((_IsLocalTest))
            {
                //  Show full unminified version
                candidate = candidate.Replace("testdrivev2.min.js", "testdrivev2.js");
            }

            return candidate;
        }

        public string Get_component_channelNav()
        {
            if (this.m_container.OpenInFrame > 0)
                return string.Empty;

            if (CommonConfiguration.HIDE_CHANNEL)
                return string.Empty;

            Site[] allAccessible = Site.SelectAllAccessible(m_container.CurrentApplicationUser, AccessFilter.RoleAndUser);

            if (allAccessible.Count() < 2)
                return string.Empty;

            string channels = "";

            foreach (Site site in allAccessible)
            {
                if (!site.HasLists && !site.HasPages && !site.IsActive)
                    continue;

                channels += string.Format("\n\t\t\t\t\t\t\t<option value=\"{0}\"{2}>{1}</option>", site.ID, site.Name, site.ID == m_container.ChannelIndentifier ? " selected=\"selected\"" : string.Empty);
            }
            return @"
		<li>
			<a class=""side"" href=""#""><span class=""icon-globe""></span></a>
			<div class=""channel"" style=""display: none;"">
				<a class=""active"" href=""#""><span class=""icon-globe""></span></a>
				<h3>Channel selection</h3>
				<label for=""channel"">Channel</label>
				<select name=""channel"" id=""channel"" class=""postBack"">" + channels + @"
				</select>
			</div>
		</li>";
        }


        public string Get_component_mainMenu(string data)
        {
            return string.Concat(@"<menu id=""mainMenu"" class=""component"">", data, "</menu>");
        }

        public string Get_component_metaMenu()
        {
            string usernameLink = m_container.CurrentApplicationUser.Name;
            if (!m_container.CurrentVisitor.Data["Wim.Reset.Me"].IsNull)
            {
                usernameLink = string.Format("{0} (<a href=\"?reset\">reset</a>)", usernameLink);
            }




            return @"                <menu id=""metaMenu"" class=""component"">
                    <section class=""metaMenu"">
		                <li><img src=""testdrive/files/iconPeople.png"" alt=""Person"" /></li>
		                <li>" + usernameLink + @" <span>|</span></li>
		                <li class=""last""><a href=""?logout"" id=""logout"" class=""postBack"">uitloggen</a></li>
	                </section>
                </menu>";
        }


  

        /// <summary>
        /// Gets the login wrapper.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns></returns>
        public string GetLoginWrapper(Beta.GeneratedCms.Console container, Dictionary<GlobalPlaceholder, string> placeholders, Dictionary<CallbackTarget, List<ICallback>> callbacks)
        {
            m_container = container;
            m_Placeholders = placeholders;
            m_Callbacks = callbacks;

            string username = container.GetSafePost("username");
            string username2 = username;
            bool hasError = !string.IsNullOrEmpty(username);

            string password = container.GetSafePost("password");
            bool rememberMe = !string.IsNullOrEmpty(container.GetSafePost("frmRemember"));
            string emailaddress = container.GetSafePost("email");

            if (string.IsNullOrEmpty(username2) && !container.Request.HasFormContentType)
            {
                if (container.CurrentVisitor.ApplicationUserID.HasValue)
                {
                    username2 = ApplicationUser.SelectOne(container.CurrentVisitor.ApplicationUserID.Value).Email;
                }
            }

            #region ForgotMyPassword
            if (!string.IsNullOrEmpty(emailaddress))
            {
                IApplicationUser appUser = ApplicationUser.Select(emailaddress);

                if (appUser?.IsNewInstance == false)
                {
                    appUser.SendForgotPassword(container);

                    //return GetForgetPasswordScreen(container, username2, hasError, true);
                }
                //else
                //    hasError = true;

                return GetForgetPasswordScreen(container, username2, hasError, true);
            }
            if (HasQueryString(container, "reminder"))
            {
                return GetForgetPasswordScreen(container, username2, hasError, false);
            }

            #endregion ForgotMyPassword

            string pageTitle = string.Empty;
            //container.CurrentListInstance.wim.ListTitle

            if (!string.IsNullOrEmpty(container.GetSafeGet("reset")))
            {
                username2 = container.GetSafeGet("u");
                Guid resetKey = Utility.ConvertToGuid(container.GetSafeGet("reset"));

                IApplicationUser user = null;
                if (string.IsNullOrEmpty(username2))
                {
                    Guid userKey = Utility.ConvertToGuid(container.GetSafeGet("ug"));
                    user = ApplicationUser.SelectOne(userKey);
                    username2 = user.Email;
                }
                else
                    user = ApplicationUser.SelectOneByEmail(username2);

                if (user.ResetKey.Equals(resetKey))
                {
                    string password1 = container.GetSafePost("password1");
                    string password2 = container.GetSafePost("password2");

                    hasError = false;
                    if (!string.IsNullOrEmpty(password1))
                    {
                        if (password1.Equals(password2) && Utility.IsStrongPassword(password1))
                        {
                            user.ApplyPassword(password1);
                            user.Save(container.Context, true);

                            if (SetLogin(container, username2, password1, rememberMe, ref hasError))
                                return null;
                        }
                        else
                            hasError = true;
                    }
                    return GetResetScreen(container, username2, hasError);
                }
            }

            #region Login
            if (SetLogin(container, username, password, rememberMe, ref hasError))
                return null;

            #endregion

            return GetLoginScreen(container, username2, hasError);
        }

        bool HasQueryString(Beta.GeneratedCms.Console container, string name)
        {
            if (!container.Request.QueryString.HasValue)
                return false;
            return container.Request.QueryString.Value.Substring(1).Equals(name);
        }

        bool SetLogin(Beta.GeneratedCms.Console container, string username, string password, bool rememberMe, ref bool hasError)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(username))
                return false;

            if (m_Callbacks != null && m_Callbacks.ContainsKey(CallbackTarget.PRE_SIGNIN))
            {
                var parsers = m_Callbacks[CallbackTarget.PRE_SIGNIN];
                foreach (var parser in parsers)
                {
                    if (!parser.Run(container))
                        break;
                }
            }

            // select by email only to verify the user
            container.CurrentApplicationUser = ApplicationUser.SelectOneByEmail(username);

            if (m_Callbacks != null && m_Callbacks.ContainsKey(CallbackTarget.POST_SIGNIN))
            {
                var parsers = m_Callbacks[CallbackTarget.POST_SIGNIN];
                foreach (var parser in parsers)
                {
                    if (!parser.Run(container))
                        break;
                }
            }

            if (container.CurrentApplicationUser != null && !container.CurrentApplicationUser.IsNewInstance)
            {
                //  Verify password
                var isvalid = container.CurrentApplicationUser.IsValid(password);
                if (isvalid)
                {
                    if (!Utility.IsStrongPassword(password))
                    {
                        string resetlink;
                        container.CurrentApplicationUser.ResetPassword(container, out resetlink, false);
                        container.Response.Redirect(resetlink.ToString(), true);
                        return false;
                    }

                    container.CurrentApplicationUser.Store(container.Context, password, rememberMe);
                    container.CurrentVisitor.ApplicationUserID = container.CurrentApplicationUser.ID;
                    return true;
                }
            }
            else
                hasError = true;
            return false;
        }

        string GetStyleAddition()
        {
            var style = $@"<link rel=""stylesheet"" href=""{FolderVersion("compiled")}styles.min.css?v={FileVersion}"">";

            if (!string.IsNullOrEmpty(CommonConfiguration.STYLE_INCLUDE))
            {
                var custom = string.Format("<link rel=\"stylesheet\" href=\"{0}\" type=\"text/css\" media=\"all\" />"
                    , (CommonConfiguration.STYLE_INCLUDE.StartsWith("http", StringComparison.OrdinalIgnoreCase) 
                        ? CommonConfiguration.STYLE_INCLUDE 
                        : m_container.AddApplicationPath(CommonConfiguration.STYLE_INCLUDE)
                        ));

                return custom;
            }
            return style;
        }

        string GetHeader(string styleAddition, bool asyncEnabled = false)
        {
            string datepicker = string.Empty;
            var lang = "en";
            if (CommonConfiguration.FORM_DATEPICKER.Equals("nl", StringComparison.CurrentCultureIgnoreCase))
            {
                lang = CommonConfiguration.FORM_DATEPICKER.ToLowerInvariant();
                datepicker = $@"<script type=""text/javascript"" src=""{FolderVersion("scripts")}jquery-ui-datepicker-nl.js?v={FileVersion}""></script>";
            }

            string vuelibrary = string.Empty;
            if (asyncEnabled)
                vuelibrary = $@"<script type=""text/javascript"" src=""{FolderVersion("app/dist")}bundle.js?v={FileVersion}""></script>";

#if DEBUG

            return $@"
		<link rel=""stylesheet"" href=""{FolderVersion("compiled")}mediakiwiForm.min.css?v={FileVersion}"" type=""text/css"" media=""all"" />
		<link rel=""stylesheet"" href=""{FolderVersion("styles")}stylesFlatv2.css?v={FileVersion}"" type=""text/css"" media=""all"" />
		<link rel=""stylesheet"" href=""{FolderVersion("styles")}mainMenuFlatv2.css?v={FileVersion}"" type=""text/css""  media=""all"" />
        <link rel=""stylesheet"" href=""{FolderVersion("styles")}fontello.css?v={FileVersion}"" type=""text/css"">
        <link rel=""stylesheet"" href=""{FolderVersion("styles")}colorbox.css?v={FileVersion}"" type=""text/css""/>
		<link rel=""stylesheet"" href=""{FolderVersion("styles")}solid.css?v={FileVersion}"" type=""text/css""/>
		<link rel=""stylesheet"" href=""{FolderVersion("styles")}formalizev2.css?v={FileVersion}"" type=""text/css"" media=""all"" />
        <link rel=""stylesheet"" href=""{FolderVersion("styles")}jquery-ui-1-8-16-custom.css?v={FileVersion}"" type=""text/css"" media=""all"" />
        " + styleAddition + $@"
        <script type=""text/javascript"" src=""{FolderVersion("scripts")}jquery-1-7-1.js?v={FileVersion}""></script>
		<script type=""text/javascript"" src=""{FolderVersion("scripts")}jquery-ui-1-10-3-custom.min.js?v={FileVersion}""></script>
        " + datepicker + $@"
		<script type=""text/javascript"" src=""{FolderVersion("scripts")}jquery-shorten.js?v={FileVersion}""></script>
		<script type=""text/javascript"" src=""{FolderVersion("scripts")}jquery-tipTip.js?v={FileVersion}""></script>
		<script type=""text/javascript"" src=""{FolderVersion("scripts")}jquery-slimscroll.js?v={FileVersion}""></script>
        <script type=""text/javascript"" src=""{FolderVersion("scripts")}jquery-numeric.js?v={FileVersion}""></script>
		<script type=""text/javascript"" src=""{FolderVersion("scripts")}jquery-formalize.js?v={FileVersion}""></script>
		<script type=""text/javascript"" src=""{FolderVersion("scripts")}jquery-colorbox.js?v={FileVersion}""></script>
		<script type=""text/javascript"" src=""{FolderVersion("scripts")}jquery-ui-timepicker-addon.js?v={FileVersion}""></script>
		<script type=""text/javascript"" src=""{FolderVersion("scripts")}jquery-hoverIntent.js?v={FileVersion}""></script>
		<script type=""text/javascript"" src=""{FolderVersion("scripts")}jquery-curtainMenu.js?v={FileVersion}""></script>
        <script type=""text/javascript"" src=""{FolderVersion("scripts")}jquery-ambiance.js?v={FileVersion}""></script>
		<script type=""text/javascript"" src=""{FolderVersion("scripts")}slip.js?v={FileVersion}""></script>
		<script type=""text/javascript"" src=""{FolderVersion("scripts")}fixedBar.js?v={FileVersion}""></script>
        <script type=""text/javascript"" src=""{FolderVersion("scripts")}testdrivev2.js?v={FileVersion}""></script>
		<script type=""text/javascript"" src=""{FolderVersion("scripts/tinymce")}tinymce.js?v={FileVersion}""></script>
        <link rel=""stylesheet"" href=""{FolderVersion("scripts/dist/css")}select2.css?v={FileVersion}"" type=""text/css"" media=""all"" />
		<script type=""text/javascript"" src=""{FolderVersion("scripts")}select2.js?v={FileVersion}""></script>
		<script type=""text/javascript"" src=""{FolderVersion("scripts")}jquery-nicescroll.js?v={FileVersion}""></script>" + vuelibrary;

#else
          if (lang == "en")
            {
                return $@"
		<link rel=""stylesheet"" href=""{FolderVersion("compiled")}bundel.min.css?v={FileVersion}"" type=""text/css"" media=""all"" />
        " + styleAddition + $@"
        <script type=""text/javascript"" src=""{FolderVersion("compiled")}bundel.en.min.js?v={FileVersion}""></script>
		<script type=""text/javascript"" src=""{FolderVersion("scripts/tinymce")}tinymce.min.js?v={FileVersion}""></script>
        <link rel=""stylesheet"" href=""{FolderVersion("scripts/dist/css")}select2.css?v={FileVersion}"" type=""text/css"" media=""all"" />" + vuelibrary;
            }
            else
            {
                return $@"
		<link rel=""stylesheet"" href=""{FolderVersion("compiled")}bundel.min.css?v={FileVersion}"" type=""text/css"" media=""all"" />
        " + styleAddition + $@"
        <script type=""text/javascript"" src=""{FolderVersion("compiled")}bundel.nl.min.js?v={FileVersion}""></script>
		<script type=""text/javascript"" src=""{FolderVersion("scripts/tinymce")}tinymce.min.js?v={FileVersion}""></script>
        <link rel=""stylesheet"" href=""{FolderVersion("scripts/dist/css")}select2.css?v={FileVersion}"" type=""text/css"" media=""all"" />" + vuelibrary;
            }
  
#endif

        }


        public string LogoUrl(Beta.GeneratedCms.Console container)
        {
            if (string.IsNullOrEmpty(CommonConfiguration.LOGO_URL))
                return FolderVersion("images") + @"MK_logo.png";
            return container.AddApplicationPath(CommonConfiguration.LOGO_URL, true);
        }


        CustomData _UserData;
        CustomData UserData
        {
            get
            {
                if (_UserData == null)
                {
                    try
                    {
                        _UserData = ComponentList.SelectOne("Sushi.Mediakiwi.AppCentre.Data.Implementation.User").Settings;
                    }
                    catch (Exception)
                    {
                        _UserData = new CustomData();
                    }
                }
                return _UserData;
            }
        }

        void SetPlaceholder(ref string markup, string placeholder, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                markup = markup.Replace($"[[MK_{placeholder}]]", value);
            else
                markup = markup.Replace($"[[MK_{placeholder}]]", string.Empty);
        }
        void SetPlaceholder(ref string markup, GlobalPlaceholder placeholder)
        {
            if (m_Placeholders != null && m_Placeholders.ContainsKey(placeholder))
                markup = markup.Replace($"[[MK_{placeholder}]]", m_Placeholders[placeholder]);
            else
                markup = markup.Replace($"[[MK_{placeholder}]]", string.Empty);
        }

        string GetDefaultLoginElements(Beta.GeneratedCms.Console container, bool hasError, string username, string form, string intro)
        {
            string wimPath = CommonConfiguration.PORTAL_PATH;

            var markup = Markup.login;
            var inputclass = (hasError ? " error" : string.Empty);

            SetPlaceholder(ref markup, "FORM", form);
            SetPlaceholder(ref markup, "INPUT_INTRO", intro);
            SetPlaceholder(ref markup, "INPUT_ERR", inputclass);
            SetPlaceholder(ref markup, "INPUT_USER", username);
            SetPlaceholder(ref markup, "ENV_TITLE", container.CurrentEnvironment.Title);
            SetPlaceholder(ref markup, "CDN_PATH", FolderVersion());
            //SetPlaceholder(ref markup, "ENV_LOGO", container.CurrentEnvironment.LogoHrefFull);
            SetPlaceholder(ref markup, "CURR_PAGE", container.Url);
            SetPlaceholder(ref markup, "LOST_PWD", container.AddApplicationPath(string.Concat(wimPath, "?reminder")));
            SetPlaceholder(ref markup, "LOGIN", container.AddApplicationPath(wimPath));
            var stylesheet = GetStyleAddition();
            SetPlaceholder(ref markup, "STYLE", stylesheet);

            string replacement = string.Empty;
            if (!string.IsNullOrWhiteSpace(CommonConfiguration.LOGIN_BACKGROUND))
            {
                var image = CommonConfiguration.LOGIN_BACKGROUND.Replace("~/", container.Request.PathBase);
                replacement = $" style=\"background-image: url('{image}')\"";
            }
            SetPlaceholder(ref markup, "LOGINSECTION_STYLE", replacement);

            string loginboxlogo = string.Empty;
            if (!string.IsNullOrWhiteSpace(CommonConfiguration.LOGIN_BOXLOGO))
            {
                var image = CommonConfiguration.LOGIN_BOXLOGO.Replace("~/", container.Request.PathBase);
                loginboxlogo = $"<img src=\"{image}\" id=\"logo\" />";

                if (m_Placeholders.ContainsKey(GlobalPlaceholder.SIGNIN_LOGO_URL))
                {
                    loginboxlogo = $"<a href=\"{container.AddApplicationPath(m_Placeholders[GlobalPlaceholder.SIGNIN_LOGO_URL])}\">{loginboxlogo}</a>";
                }
            }

            SetPlaceholder(ref markup, "LOGINBOX_LOGO", loginboxlogo);

            SetPlaceholder(ref markup, GlobalPlaceholder.SIGNIN_HEAD);
            SetPlaceholder(ref markup, GlobalPlaceholder.SIGNIN_SIGNATURE);

            return markup;
        }

        string GetLoginScreen(Beta.GeneratedCms.Console container, string username2, bool hasError)
        {
            string intro = UserData["Page_Login"].Value;
            if (!string.IsNullOrEmpty(intro)) intro = string.Format("<p>{0}</p>", intro);

            if (container.Request.QueryString != null &&
                container.Request.QueryString.ToString().Equals("send", StringComparison.CurrentCultureIgnoreCase)
                )
            {
                if (string.IsNullOrEmpty(intro))
                    intro = string.Empty;

                intro += string.Format("<p>{0}</p>", "If your email is know to us, we have just send you an email to allows you to reset your password.");
            }
            var markup = GetDefaultLoginElements(container, hasError, username2, Markup.component_login, intro);
            SetPlaceholder(ref markup, GlobalPlaceholder.SIGNIN_AREA);

            return markup;
        }

        string GetResetScreen(Beta.GeneratedCms.Console container, string username2, bool hasError)
        {
            var markup = GetDefaultLoginElements(container, hasError, username2, Markup.component_applypassword, null);
            string wimPath = CommonConfiguration.PORTAL_PATH;
            return markup;
        }

        string GetForgetPasswordScreen(Beta.GeneratedCms.Console container, string username2, bool hasError, bool hasSend)
        {
            string wimPath = CommonConfiguration.PORTAL_PATH;

            if (hasSend)
                container.Response.Redirect($"{container.AddApplicationPath(wimPath)}?send");

            var markup = GetDefaultLoginElements(container, hasError, username2, Markup.component_reminder, null);
            return markup;
        }
    }
}
