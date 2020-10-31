using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Web;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.Data;
using System.Net;

namespace Sushi.Mediakiwi.Beta.GeneratedCms.Source
{
    /// <summary>
    /// Represents a Template entity.
    /// </summary>
    class Template
    {
        /// <summary>
        /// Dashboard2s the specified container.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="header">The header.</param>
        /// <param name="top">The top.</param>
        /// <param name="footer">The footer.</param>
        /// <param name="intro">The intro.</param>
        /// <param name="target1">The target1.</param>
        /// <param name="target2">The target2.</param>
        /// <returns></returns>
        internal static string Dashboard2(Console container, string header, string top, string footer, string intro, string target1, string target2)
        {
            return string.Format(@"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.1//EN"" ""http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"" xml:lang=""en"">
	<head>
		<title>Dashboard</title>
		{1}
	</head>
	<body class=""homePageDouble"">
		<form method=""post"" action=""{4}"" enctype=""multipart/form-data"">
        <div id=""canvas"">{2}
            <div id=""banner"" class=""short""></div>
			<div id=""content"">
				<div id=""leftWideColumn"">
					<div class=""centerContent"">{8}{5}
					</div>
				</div>
				<div id=""righWideColumn"">
					<div class=""centerContent"">{6}
					</div>
				</div>
				<div class=""clear""></div>
			</div>
		</div>{3}
        <form>
		<script type=""text/javascript"" src=""{0}/scripts/inline.js""></script>
	</body>
</html>
"
                , container.WimRepository //0
                , header //1
                , top //2
                , footer //3
                , container.GetSafeUrl() //4
                , target1 //5
                , target2 //6 
                , null //7
                , intro //8
                );
        }



        /// <summary>
        /// Dashboards the specified container.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="header">The header.</param>
        /// <param name="top">The top.</param>
        /// <param name="footer">The footer.</param>
        /// <param name="target1">The target1.</param>
        /// <param name="target2">The target2.</param>
        /// <param name="target3">The target3.</param>
        /// <returns></returns>
        internal static string Dashboard3(Console container, string header, string top, string footer, string target1, string target2, string target3)
        {
//            <!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.1//EN"" ""http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd"">
//<html xmlns=""http://www.w3.org/1999/xhtml"" xml:lang=""en"">
//    <head>
//        <title>Dashboard</title>
//        {1}
//    </head>
//    <body class=""homePage{9}"">
//        <form method=""post"" action=""{4}"" enctype=""multipart/form-data"">
//        <form>
//        <script type=""text/javascript"" src=""{0}/scripts/inline.js""></script>
//    </body>
//</html>


            return string.Format(@"
        <div id=""canvas"">{2}
			<div id=""banner"" class=""short""></div>
			<div id=""content"">
				<div id=""leftColumn"" class=""contentPage"">{5}
				</div>
				<div id=""centerColumn"">
					<div id=""centerContent"">{6}
					</div>
				</div>
				<div id=""rightColumn"">{7}
				</div>
				<div class=""clear""></div>
			</div>
		</div>{3}
"
                , container.WimRepository //0
                , header // 1
                , top // 2
                , footer // 3
                , container.GetSafeUrl()// 4
                , target1 // 5
                , target2 // 6
                , target3 // 7
                , null // 8
                , " portalPage hgt_2"//Sushi.Mediakiwi.Data.Common.HasWideInterface ? " reallyWide" : null // 9
                );
        }

        /// <summary>
        /// Opens the in frame.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="header">The header.</param>
        /// <param name="filters">The filters.</param>
        /// <param name="breadcrumbs">The breadcrumbs.</param>
        /// <param name="browsing">The browsing.</param>
        /// <param name="type">The type.</param>
        /// <param name="isThumbnailView">if set to <c>true</c> [is thumbnail view].</param>
        /// <returns></returns>
        internal static string OpenInFrame(Console container, string header, string filters, string breadcrumbs, string browsing, int type, bool isThumbnailView, string footer)
        {
            string tabTag = string.Empty;// GetTabularTags(container, null, 0, false);
            

            return string.Format(@"{9}{10}
		{6}{3}
        <div class=""{7}{8}"">
{2}
		</div>{11}
"
                , container.WimRepository
                , header
                , browsing
                , filters
                , container.GetSafeUrl()
                , container.CurrentList.Name
                , container.CurrentList.Description == null ? string.Empty : (container.CurrentList.Description.StartsWith("<p>", StringComparison.OrdinalIgnoreCase) ? container.CurrentList.Description : string.Concat("<p>", container.CurrentList.Description, "</p>"))
                , type == 1 ? "textOptionSelect" : "textOptionSelect multi_yes"
                , isThumbnailView ? "  type_li" : string.Empty
                , breadcrumbs
                , tabTag
                , footer

                );
        }

        /// <summary>
        /// Folders the browsing.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="header">The header.</param>
        /// <param name="top">The top.</param>
        /// <param name="footer">The footer.</param>
        /// <param name="leftnavigation">The leftnavigation.</param>
        /// <param name="breadcrumb">The breadcrumb.</param>
        /// <param name="filters">The filters.</param>
        /// <param name="browsing">The browsing.</param>
        /// <param name="title">The title.</param>
        /// <param name="showServiceUrl">if set to <c>true</c> [show service URL].</param>
        /// <returns></returns>
        internal static string FolderBrowsing(UI.Monitor monitor, Console container, string header, string top, string footer, string leftnavigation, string breadcrumb, string filters, string browsing, string title, bool showServiceUrl)
        {
            return FolderBrowsing(monitor, container, header, top, footer, leftnavigation, breadcrumb, filters, browsing, title, showServiceUrl, 0);
        }

        /// <summary>
        /// Folders the browsing.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="header">The header.</param>
        /// <param name="top">The top.</param>
        /// <param name="footer">The footer.</param>
        /// <param name="leftnavigation">The leftnavigation.</param>
        /// <param name="breadcrumb">The breadcrumb.</param>
        /// <param name="filters">The filters.</param>
        /// <param name="browsing">The browsing.</param>
        /// <param name="title">The title.</param>
        /// <param name="showServiceUrl">if set to <c>true</c> [show service URL].</param>
        /// <param name="selectedTab">The selected tab.</param>
        /// <returns></returns>
        internal static string FolderBrowsing(UI.Monitor monitor, Console container, string header, string top, string footer, string leftnavigation, string breadcrumb, string filters, string browsing, string title, bool showServiceUrl, int selectedTab)
        {
            return FolderBrowsing(monitor, container, header, top, footer, leftnavigation, breadcrumb, null, filters, browsing, title, showServiceUrl, selectedTab, null);
        }

        /// <summary>
        /// Folders the browsing.
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <param name="container">The container.</param>
        /// <param name="header">The header.</param>
        /// <param name="top">The top.</param>
        /// <param name="footer">The footer.</param>
        /// <param name="leftnavigation">The leftnavigation.</param>
        /// <param name="breadcrumb">The breadcrumb.</param>
        /// <param name="filters">The filters.</param>
        /// <param name="browsing">The browsing.</param>
        /// <param name="title">The title.</param>
        /// <param name="showServiceUrl">if set to <c>true</c> [show service URL].</param>
        /// <param name="exportUrl">The export URL.</param>
        /// <returns></returns>
        internal static string FolderBrowsing(Sushi.Mediakiwi.UI.Monitor monitor, Console container, string header, string top, string footer, string leftnavigation, string breadcrumb, string filters, string browsing, string title, bool showServiceUrl, string exportUrl)
        {
            return FolderBrowsing(monitor, container, header, top, footer, leftnavigation, breadcrumb, null, filters, browsing, title, showServiceUrl, 0, exportUrl);
        }

        static void ApplyTabularUrl(Console container, Sushi.Mediakiwi.Framework.WimComponentListRoot.Tabular t, int levelEntry)
        {
            ApplyTabularUrl(container, t, levelEntry, null);
        }

        static string GetQueryStringRecording(Console container)
        {
            string addition = string.Empty;
            if (container.CurrentListInstance.wim._QueryStringRecording != null)
            {
                container.CurrentListInstance.wim._QueryStringRecording.ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(container.Request.Query[x]))
                        addition += string.Format("&{0}={1}", x, container.Request.Query[x]);
                });
            }
            return addition;
        }

        static void ApplyTabularUrl(Console container, Sushi.Mediakiwi.Framework.WimComponentListRoot.Tabular t, int levelEntry, int? currentListID)
        {
            int listID = currentListID.HasValue ? currentListID.Value : Utility.ConvertToInt(container.Request.Query["list"]);

            string addition = GetQueryStringRecording(container);

            int itemID = Utility.ConvertToInt(container.Request.Query["item"]);

            int? openinframeID = Utility.ConvertToIntNullable(container.Request.Query["openinframe"]);

            int groupID = Utility.ConvertToInt(container.Request.Query["group"]);
            int groupItemID = Utility.ConvertToInt(container.Request.Query["groupitem"]);

            int group2ID = Utility.ConvertToInt(container.Request.Query["group2"]);
            int group2ItemID = Utility.ConvertToInt(container.Request.Query["group2item"]);

            int folderID = Utility.ConvertToInt(container.Request.Query["folder"]);
            string folderInfo = null;
            if (folderID > 0) 
                folderInfo = string.Concat("&folder=", folderID);

            int baseID = Utility.ConvertToInt(container.Request.Query["base"]);
            string baseInfo = null;
            if (baseID > 0)
                baseInfo = string.Concat("&base=", baseID);

            if (levelEntry == 1)
            {
                if (groupID == 0)
                    t.Url = string.Concat(container.WimPagePath, "?group=", listID, addition, baseInfo, folderInfo, "&groupitem=", itemID, "&list=", t.List.ID, "&item=", t.SelectedItem);
                else
                {
                    t.Url = string.Concat(container.WimPagePath, "?group=", groupID, addition, baseInfo, folderInfo, "&groupitem=", groupItemID, "&list=", t.List.ID, "&item=", group2ID == t.List.ID ? group2ItemID : t.SelectedItem);
                }
            }
            else if (levelEntry == 2)
            {
                t.Url = string.Concat(container.WimPagePath, "?group=", groupID, addition, baseInfo, folderInfo, "&groupitem=", groupItemID, "&group2=", listID, "&group2item=", itemID, "&list=", t.List.ID, "&item=", t.SelectedItem);
            }
            else
            {
                t.Url = string.Concat(container.WimPagePath, "?list=", listID, addition, baseInfo, folderInfo, "&item=", t.SelectedItem);
            }

            if (openinframeID.HasValue && t.Url.Contains("?"))
                t.Url += string.Format("&openinframe={0}", openinframeID.GetValueOrDefault());


        }
       
        internal static string GetTabularTagNewDesign(Console container, string title, int selectedTab, bool showServiceUrl)
        {
            title = container.CurrentList.Name;

            if (container.CurrentListInstance.wim.Page.HideTabs)
            {
                return null; 
            }

            string tabTag = null;
            #region Browsing
            if (container.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.Browsing)
            {
                title = Labels.ResourceManager.GetString("list_browsing", new CultureInfo(container.CurrentApplicationUser.LanguageCulture));
                #region PAGE
                if (container.ItemType == RequestItemType.Page)
                {
                    
                    if (container.CurrentPage.Template.GetPageSections().Length > 0)
                    {
                        //var legacyContentTab = container.CurrentPage.Template.Data[string.Format("TAB.LCT")].Value;
                        //var legacyServiceTab = container.CurrentPage.Template.Data[string.Format("TAB.LST")].Value;

                        var sections = container.CurrentPage.Template.GetPageSections();

                        StringBuilder build = new StringBuilder();

                        var selected = container.Request.Query["tab"];
                        bool isSelected = string.IsNullOrEmpty(selected);

                        foreach (var section in sections)
                        {
                            if (selected == section)
                                isSelected = true;

                            //

                            var tabName = container.CurrentPage.Template.Data[string.Format("T[{0}]", section)].Value;
                            if (string.IsNullOrEmpty(tabName))
                                tabName = section;

                            build.AppendFormat(string.Format(@" <li><a href=""{0}""{2}>{1}</a></li>"
                                , string.Concat(container.WimPagePath, "?page=", container.Item, "&tab=", section)
                                , tabName
                                , isSelected ? " class=\"active\"" : null
                            ));

                            isSelected = false;
                        }
                        build.Append(@"</ul>");
                        tabTag = build.ToString();

                    }
                    else
                    {
                        title = Labels.ResourceManager.GetString("tab_Content", new CultureInfo(container.CurrentApplicationUser.LanguageCulture));
                        //"Content"
                        tabTag = string.Format(@"
			                     <li><a href=""{1}""{2}>{0}</a></li>{3}"
                            , title //0
                            , string.Concat(container.WimPagePath, "?page=", container.Item) //1
                            , selectedTab == 0 ? " class=\"active\"" : null //2
                            , showServiceUrl ?
                                string.Format("<li><a href=\"{0}\"{1}>{2}</a></li>"
                                    , string.Concat(container.WimPagePath, "?page=", container.Item, "&tab=1")//, selectedTab)// == 1 ? "0" : "1")
                                    , selectedTab == 1 ? " class=\"active\"" : null
                                    , Labels.ResourceManager.GetString("tab_ServiceColumn", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                                    ) : string.Empty //3

                            );
                    }
                }
                #endregion PAGE
                else
                {
                    //  Show NO tabs
                    //return null;
                    tabTag = string.Format(@"
			            <li><a href=""{1}"" class=""active"">{0}</a></li>"
                        , title
                        , container.GetSafeUrl()
                        );
                }
            }
            #endregion
            #region Folder & page
            else if (container.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.Folders || container.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.PageProperties)
            {
                tabTag = string.Format(@"
			            <li><a href=""{1}"" class=""active"">{0}</a></li>"
                    , title
                    , container.GetSafeUrl()
                    );
            }
            #endregion
            #region Assets
            else if (container.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.Documents || container.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.Images)
            {
                title = "Browsing";

                int galleryID = Utility.ConvertToInt(container.Request.Query["gallery"]);
                if (galleryID == 0)
                {
                    galleryID = Sushi.Mediakiwi.Data.Asset.SelectOne(container.Item.Value).GalleryID;
                    //if (container.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.Documents)
                    //    galleryID = Sushi.Mediakiwi.Data.Document.SelectOne(container.Item.Value).GalleryID;
                    //else if (container.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.Images)
                    //    galleryID = Sushi.Mediakiwi.Data.Image.SelectOne(container.Item.Value).GalleryID;
                }

                tabTag = string.Format(@"
			            <li><a href=""{1}"">{0}</a></li>
                        <li><a href=""{2}""{3}>{4}</a></li>"
                    , title
                    , string.Concat(container.WimPagePath, "?gallery=", galleryID)
                    , string.Concat(container.WimPagePath, "?gallery=", galleryID, (container.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.Documents ? "&gfx=" : "&gfx=")
                    , container.Item.GetValueOrDefault())
                    , selectedTab == 0 ? " class=\"active\"" : null
                    , container.CurrentList.SingleItemName
                    );
            }
            #endregion
            #region Custom lists
            else
            {
                bool isSingleItemList = (container.CurrentList.IsSingleInstance || container.CurrentListInstance.wim.CanContainSingleInstancePerDefinedList);

                //  Show NO tabs
                if (isSingleItemList)
                {
                    tabTag = string.Format(@"
			            <li><a href=""{1}"" class=""active"">{0}</a></li>"
                        , title
                        , container.GetSafeUrl()
                        );
                    return tabTag;
                }

                if (container.Item.HasValue)
                {
                    Console master = container;

                    int currentListId = container.Logic;
                    int currentListItemId = container.Item.GetValueOrDefault();
                    string itemTitle = container.CurrentList.SingleItemName;

                    //  Testcode
                    List<Sushi.Mediakiwi.Framework.WimComponentListRoot.Tabular> tabularList = null;
                    if (!string.IsNullOrEmpty(container.Request.Query["group"]))
                    {
                        int groupId = Utility.ConvertToInt(container.Request.Query["group"]);
                        int groupElementId = Utility.ConvertToInt(container.Request.Query["groupitem"]);
                        if (groupId != container.CurrentList.ID)
                        {

                            if (container.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.ComponentListProperties)
                            {
                                tabularList = container.CurrentListInstance.wim.m_Collection;
                            }
                            else
                            {
                                var innerlist = Sushi.Mediakiwi.Data.ComponentList.SelectOne(groupId);

                                //  The current requested list is not the list that is the base of the tabular menu
                                master = container.ReplicateInstance(innerlist);
                                master.CurrentListInstance.wim.Console = master;
                                master.CurrentListInstance.wim.Console.Item = groupElementId;

                                master.CurrentListInstance.wim.DoListLoad(groupElementId, 0);
                                tabularList = master.CurrentListInstance.wim.m_Collection;

                                currentListId = groupId;
                                currentListItemId = groupElementId;
                                title = master.CurrentList.Name;
                                itemTitle = master.CurrentList.SingleItemName;
                            }
                        }
                    }
                    else if (container.CurrentListInstance.wim.m_Collection != null)
                        tabularList = container.CurrentListInstance.wim.m_Collection;


                    string tabulars = null;
                    if (tabularList != null)
                    {
                        tabulars = "";
                        foreach (Sushi.Mediakiwi.Framework.WimComponentListRoot.Tabular t in tabularList)
                        {
                            if (t.List.IsNewInstance)
                                continue;

                            ApplyTabularUrl(container, t, 1);

                            tabulars += string.Format(@"<li><a href=""{1}""{2}>{0}</a></li>"
                                , t.TitleValue
                                , t.Url
                                , t.Selected ? " class=\"active\"" : null
                                );

                            if (t.Selected)
                                selectedTab = t.List.ID;

                            if (!container.Group.HasValue)
                                continue;

                            if (container.CurrentListInstance.wim.CurrentList.ID == t.List.ID)
                            {
                                if (container.CurrentListInstance.wim.m_Collection != null)
                                {
                                    foreach (Sushi.Mediakiwi.Framework.WimComponentListRoot.Tabular t2 in container.CurrentListInstance.wim.m_Collection)
                                    {
                                        ApplyTabularUrl(container, t2, 2);

                                        tabulars += string.Format(@"<li><a href=""{1}""{2}>{0}</a></li>"
                                            , t2.TitleValue
                                            , t2.Url
                                            , t2.Selected ? " class=\"active\"" : null
                                            );

                                        if (t2.Selected)
                                            selectedTab = t2.List.ID;
                                    }
                                }
                            }
                            else
                            {
                                var cl = t.List.GetInstance(container.Context);
                                if (cl != null)
                                {
                                    cl.wim.Console = master;

                                    //int group2Id = Utility.ConvertToInt(container.Request.Query["group2"]);
                                    //int group2ElementId = Utility.ConvertToInt(container.Request.Query["group2item"]);

                                    //if (t.List.ID == group2Id)
                                    //    cl.DoListLoad(group2ElementId, 0);
                                    //else
                                    //    cl.DoListLoad(container.Item.Value, 0);

                                    if (cl.wim.m_Collection != null)
                                    {
                                        foreach (Sushi.Mediakiwi.Framework.WimComponentListRoot.Tabular t2 in cl.wim.m_Collection)
                                        {
                                            tabulars += string.Format(@"<li><a href=""{1}""{2}>{0}</a></li>"
                                                , t2.TitleValue
                                                , t2.Url
                                                , t2.Selected ? " class=\"active\"" : null
                                                );

                                            if (t2.Selected)
                                                selectedTab = t2.List.ID;
                                        }
                                    }
                                }
                            }
                        }
                    }


                    Sushi.Mediakiwi.Framework.WimComponentListRoot.Tabular tmp = new Sushi.Mediakiwi.Framework.WimComponentListRoot.Tabular();
                    tmp.SelectedItem = currentListItemId;
                    ApplyTabularUrl(container, tmp, 0, currentListId);

                    //  For use in property tabbing.
                    int baseID = Utility.ConvertToInt(container.Request.Query["base"]);
                    if (baseID > 0)
                    {
                        var list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(baseID);
                        title = list.Name;
                        currentListId = list.ID;
                    }

                    if (isSingleItemList)
                    {
                        tabTag = string.Format(@"
                        <li><a href=""{2}""{3}>{4}</a></li>{5}"
                            , title
                            , string.Concat(container.WimPagePath, "?list=", currentListId)
                            , tmp.Url
                            , selectedTab == 0 ? " class=\"active\"" : null
                            , itemTitle, tabulars
                            );
                    }
                    else
                    {
                        string addition = GetQueryStringRecording(container);
                        tabTag = string.Format(@"
			            <li><a href=""{1}"">{0}</a></li>
                          <li><a href=""{2}""{3}>{4}</a></li>{5}
                        "
                            , title
                            , string.Concat(container.WimPagePath, "?list=", currentListId, addition)
                            , tmp.Url
                            , selectedTab == 0 ? " class=\"active\"" : null
                            , itemTitle, tabulars
                            );
                    }
                }
                else
                {
                    //  Show NO tabs
                    //return null;
                    tabTag = string.Format(@"
			            <li><a href=""{1}"" class=""active"">{0}</a></li>"
                        , container.CurrentList.Name
                        , container.GetSafeUrl()
                        );
                }
            }
            #endregion
            return tabTag;
        }
        

        /// <summary>
        /// Gets the tabular tags.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="title">The title.</param>
        /// <param name="selectedTab">The selected tab.</param>
        /// <param name="showServiceUrl">if set to <c>true</c> [show service URL].</param>
        /// <returns></returns>
        internal static string GetTabularTags(Console container, string title, int selectedTab, bool showServiceUrl)
        {
            string tabTag = null;
            #region Browsing
            if (container.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.Browsing)
            {
                title = Labels.ResourceManager.GetString("list_browsing", new CultureInfo(container.CurrentApplicationUser.LanguageCulture));
                if (container.ItemType == RequestItemType.Page)
                {
                    if (container.CurrentPage.Template.Data.HasProperty("TAB.INFO"))
                    {
                        var legacyContentTab = container.CurrentPage.Template.Data[string.Format("TAB.LCT")].Value;
                        var legacyServiceTab = container.CurrentPage.Template.Data[string.Format("TAB.LST")].Value;

                        var sections = container.CurrentPage.Template.Data["TAB.INFO"].Value.Split(',');

                        StringBuilder build = new StringBuilder(@"<ul id=""tabNavigation"">");

                        var selected = container.Request.Query["tab"];
                        bool isSelected = string.IsNullOrEmpty(selected);
                        
                        foreach (var section in sections)
                        {
                            if (selected == section)
                                isSelected = true;

                            build.AppendFormat(string.Format(@" <li><a href=""{0}""{2}><span>{1}</span></a></li>"
                                , string.Concat(container.WimPagePath, "?page=", container.Item, "&tab=", section) 
                                , container.CurrentPage.Template.Data[string.Format("T[{0}]", section)].Value
                                , isSelected ? " class=\"active\"" : null 
                            ));

                            isSelected = false;
                        }
                        build.Append(@"</ul>");
                        tabTag = build.ToString();

                    }
                    else
                    {
                        title = Labels.ResourceManager.GetString("tab_Content", new CultureInfo(container.CurrentApplicationUser.LanguageCulture));
                        //"Content"
                        tabTag = string.Format(@"
		            <ul id=""tabNavigation"">
			            <li><a href=""{1}""{2}><span>{0}</span></a></li>{3}
		            </ul>"
                            , title //0
                            , string.Concat(container.WimPagePath, "?page=", container.Item) //1
                            , selectedTab == 0 ? " class=\"active\"" : null //2
                            , showServiceUrl ?
                                string.Format("<li><a href=\"{0}\"{1}><span>{2}</span></a></li>"
                                    , string.Concat(container.WimPagePath, "?page=", container.Item, "&tab=1")//, selectedTab)// == 1 ? "0" : "1")
                                    , selectedTab == 1 ? " class=\"active\"" : null
                                    , Labels.ResourceManager.GetString("tab_ServiceColumn", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                                    ) : string.Empty //3

                            );
                    }
                }
                else
                {

                    tabTag = string.Format(@"
		            <ul id=""tabNavigation"">
			            <li><a href=""{1}"" class=""active""><span>{0}</span></a></li>
		            </ul>"
                        , title
                        , container.GetSafeUrl()
                        );
                }
            }
            #endregion
            #region Folder & page
            else if (container.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.Folders || container.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.PageProperties)
            {
                tabTag = string.Format(@"
		            <ul id=""tabNavigation"">
			            <li><a href=""{1}"" class=""active""><span>{0}</span></a></li>
		            </ul>"
                    , title
                    , container.GetSafeUrl()
                    );
            }
            #endregion
            #region Assets
            else if (container.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.Documents || container.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.Images)
            {
                title = "Browsing";

                int galleryID = Utility.ConvertToInt(container.Request.Query["gallery"]);
                if (galleryID == 0)
                {
                    galleryID = Sushi.Mediakiwi.Data.Asset.SelectOne(container.Item.Value).GalleryID;
                    //if (container.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.Documents)
                    //    galleryID = Sushi.Mediakiwi.Data.Document.SelectOne(container.Item.Value).GalleryID;
                    //else if (container.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.Images)
                    //    galleryID = Sushi.Mediakiwi.Data.Image.SelectOne(container.Item.Value).GalleryID;
                }

                tabTag = string.Format(@"
		            <ul id=""tabNavigation"">
			            <li><a href=""{1}""><span>{0}</span></a></li>
                        <li><a href=""{2}""{3}><span>{4}</span></a></li>
		            </ul>"
                    , title
                    , string.Concat(container.WimPagePath, "?gallery=", galleryID)
                    , string.Concat(container.WimPagePath, "?gallery=", galleryID, (container.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.Documents ? "&gfx=" : "&gfx=")
                    , container.Item.GetValueOrDefault())
                    , selectedTab == 0 ? " class=\"active\"" : null
                    , container.CurrentList.SingleItemName
                    );
            }
            #endregion
            #region Custom lists
            else
            {
                bool isSingleItemList = (container.CurrentList.IsSingleInstance || container.CurrentListInstance.wim.CanContainSingleInstancePerDefinedList);
                if (container.Item.HasValue || isSingleItemList)
                {
                    Console master = container;

                    int currentListId = container.Logic;
                    int currentListItemId = container.Item.GetValueOrDefault();
                    string itemTitle = container.CurrentListInstance.wim.ListTitle;

                    //  Testcode
                    List<Sushi.Mediakiwi.Framework.WimComponentListRoot.Tabular> tabularList = null;
                    if (!string.IsNullOrEmpty(container.Request.Query["group"]))
                    {
                        int groupId = Utility.ConvertToInt(container.Request.Query["group"]);
                        int groupElementId = Utility.ConvertToInt(container.Request.Query["groupitem"]);
                        if (groupId != container.CurrentList.ID)
                        {
                           
                            if (container.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.ComponentListProperties)
                            {
                                tabularList = container.CurrentListInstance.wim.m_Collection;
                            }
                            else
                            {
                                var innerlist = Sushi.Mediakiwi.Data.ComponentList.SelectOne(groupId);

                                //  The current requested list is not the list that is the base of the tabular menu
                                master = container.ReplicateInstance(innerlist);
                                master.CurrentListInstance.wim.Console = master;
                                master.CurrentListInstance.wim.Console.Item = groupElementId;

                                master.CurrentListInstance.wim.DoListLoad(groupElementId, 0);
                                tabularList = master.CurrentListInstance.wim.m_Collection;

                                currentListId = groupId;
                                currentListItemId = groupElementId;
                                title = master.CurrentList.Name;
                                itemTitle = master.CurrentList.SingleItemName;
                            }
                        }
                    }

                    //  layer in frame & tabs
                    if (container.OpenInFrame > 0 && master.CurrentListInstance.wim.ShowNoTabInLayer)
                        return null;

                    else if (container.CurrentListInstance.wim.m_Collection != null)
                        tabularList = container.CurrentListInstance.wim.m_Collection;


                    string tabulars = null;
                    if (tabularList != null)
                    {
                        tabulars = "";
                        foreach (Sushi.Mediakiwi.Framework.WimComponentListRoot.Tabular t in tabularList)
                        {
                            if (t.List.IsNewInstance)
                                continue;

                            ApplyTabularUrl(container, t, 1);

                            tabulars += string.Format(@"<li><a href=""{1}""{2}><span>{0}</span></a></li>"
                                , t.TitleValue 
                                , t.Url
                                , t.Selected ? " class=\"active\"" : null
                                );

                            if (t.Selected)
                                selectedTab = t.List.ID;

                            if (!container.Group.HasValue)
                                continue;

                            if (container.CurrentListInstance.wim.CurrentList.ID == t.List.ID)
                            {
                                if (container.CurrentListInstance.wim.m_Collection != null)
                                {
                                    foreach (Sushi.Mediakiwi.Framework.WimComponentListRoot.Tabular t2 in container.CurrentListInstance.wim.m_Collection)
                                    {
                                        ApplyTabularUrl(container, t2, 2);

                                        tabulars += string.Format(@"<li><a href=""{1}""{2}><span>{0}</span></a></li>"
                                            , t2.TitleValue
                                            , t2.Url
                                            , t2.Selected ? " class=\"active\"" : null
                                            );

                                        if (t2.Selected)
                                            selectedTab = t2.List.ID;
                                    }
                                }
                            }
                            else
                            {
                                var cl = t.List.GetInstance(container.Context);
                                if (cl != null)
                                {
                                    cl.wim.Console = master;

                                    int group2Id = Utility.ConvertToInt(container.Request.Query["group2"]);
                                    int group2ElementId = Utility.ConvertToInt(container.Request.Query["group2item"]);

                                    //if (t.List.ID == group2Id)
                                    //    cl.DoListLoad(group2ElementId, 0);
                                    //else
                                    //    cl.DoListLoad(container.Item.Value, 0);

                                    if (cl.wim.m_Collection != null)
                                    {
                                        foreach (Sushi.Mediakiwi.Framework.WimComponentListRoot.Tabular t2 in cl.wim.m_Collection)
                                        {
                                            tabulars += string.Format(@"<li><a href=""{1}""{2}><span>{0}</span></a></li>"
                                                , t2.TitleValue
                                                , t2.Url
                                                , t2.Selected ? " class=\"active\"" : null
                                                );

                                            if (t2.Selected)
                                                selectedTab = t2.List.ID;
                                        }
                                    }
                                }
                            }
                        }
                    }


                    Sushi.Mediakiwi.Framework.WimComponentListRoot.Tabular tmp = new Sushi.Mediakiwi.Framework.WimComponentListRoot.Tabular();
                    tmp.SelectedItem = currentListItemId;
                    ApplyTabularUrl(container, tmp, 0, currentListId);

                    //  For use in property tabbing.
                    int baseID = Utility.ConvertToInt(container.Request.Query["base"]);
                    if (baseID > 0)
                    {
                        var list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(baseID);
                        title = list.Name;
                        currentListId = list.ID;
                    }

                    

                    if (isSingleItemList || master.CurrentListInstance.wim.HideListSearchTabular)
                    {
                        tabTag = string.Format(@"
		            <ul id=""tabNavigation"">
                        <li><a href=""{2}""{3}><span>{4}</span></a></li>{5}
		            </ul>"
                            , title
                            , string.Concat(container.WimPagePath, "?list=", currentListId)
                            , tmp.Url
                            , selectedTab == 0 ? " class=\"active\"" : null
                            , itemTitle, tabulars
                            );
                    }
                    else
                    {
                        tabTag = string.Format(@"
		            <ul id=""tabNavigation"">
			            <li><a href=""{1}""><span>{0}</span></a></li>
                        <li><a href=""{2}""{3}><span>{4}</span></a></li>{5}
		            </ul>"
                            , title
                            , string.Concat(container.WimPagePath, "?list=", currentListId)
                            , tmp.Url
                            , selectedTab == 0 ? " class=\"active\"" : null
                            , itemTitle, tabulars
                            );
                    }
                }
                else
                {
                    if (container.CurrentListInstance.wim.ShowNoTabInLayer)
                        return null;

                    tabTag = string.Format(@"
		            <ul id=""tabNavigation"">
			            <li><a href=""{1}"" class=""active""><span>{0}</span></a></li>
		            </ul>"
                        , container.CurrentListInstance.wim.CurrentList.Name
                        , container.GetSafeUrl()
                        );
                }
            }
            #endregion
            return tabTag;
        }

        /// <summary>
        /// Folders the browsing.
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <param name="container">The container.</param>
        /// <param name="header">The header.</param>
        /// <param name="top">The top.</param>
        /// <param name="footer">The footer.</param>
        /// <param name="leftnavigation">The leftnavigation.</param>
        /// <param name="breadcrumb">The breadcrumb.</param>
        /// <param name="bottomnavigation">The bottomnavigation.</param>
        /// <param name="filters">The filters.</param>
        /// <param name="browsing">The browsing.</param>
        /// <param name="title">The title.</param>
        /// <param name="showServiceUrl">if set to <c>true</c> [show service URL].</param>
        /// <param name="selectedTab">The selected tab.</param>
        /// <param name="exportUrl">The export URL.</param>
        /// <returns></returns>
        internal static string FolderBrowsing(UI.Monitor monitor, Console container, string header, string top, string footer, string leftnavigation, string breadcrumb, string bottomnavigation, string filters, string browsing, string title, bool showServiceUrl, int selectedTab, string exportUrl)
        {
            string description =
                container.CurrentList.Description == null 
                ? string.Empty 
                : (container.CurrentList.Description.StartsWith("<p>", StringComparison.OrdinalIgnoreCase) 
                    ? container.CurrentList.Description 
                    : string.Concat("<p>", container.CurrentList.Description, "</p>"));

            string titleLine = "";
            if (container.ItemType == RequestItemType.Page && container.Item.HasValue)
            {
                if (container.ItemType == RequestItemType.Page)
                {
                    if (container.CurrentApplicationUser.IsDeveloper)
                    {
                        var templateList = Data.ComponentList.SelectOne(Data.ComponentListType.PageTemplates);
                        Data.PageTemplate template = container.CurrentPage.Template;
                        title = string.Format("<a href=\"?list={2}&item={3}\" title=\"{1}\" style=\"color: #FFF\">{0}</a>", title, template.Location, templateList.ID, template.ID);
                    }
                }

                titleLine = string.Format(@"
					<ul id=""optionNavigation"">
						<li class=""title left""><h1>{0}</h1></li>
					</ul>", title);
                description = null;
            }

            string tabTag = GetTabularTags(container, title, selectedTab, showServiceUrl);

            string file = null;

            bool containsSublist = false;
            if (monitor.GlobalWimControlBuilder != null)
            {
                foreach (var lit in monitor.GlobalWimControlBuilder.FindObject("<bottombuttonbar />"))
                {
                    lit.Text = lit.Text.Replace("<bottombuttonbar />", bottomnavigation);
                    containsSublist = true;
                }
            }

            string downloadUrl = null;
            if (!container.CurrentListInstance.wim.CurrentVisitor.Data["wim_export"].IsNull)
            {
                string[] split = container.CurrentListInstance.wim.CurrentVisitor.Data["wim_export"].Value.Split('.');
                downloadUrl = string.Format(@"<a href=""{0}"" class=""export"">Click here to download file ({1})</a>",
                    container.AddApplicationPath(string.Concat("/doc.ashx?", container.CurrentListInstance.wim.CurrentVisitor.Data["wim_export"].Value))
                    , split[split.Length -1].ToUpper()
                    );

                container.CurrentListInstance.wim.CurrentVisitor.Data.Apply("wim_export", null);
            }

            string textData = null;
            if (container.CurrentListInstance.wim.XHtmlDataTop != null)
            {
                string container2;
                //if (container.CurrentListInstance.wim.Page.HideMenuBar)
                //    container2 = "<div id=\"centerContent\" style=\"margin-top:10px;\">";
                //else
                container2 = "<div id=\"centerContent\">";
                textData = string.Format("{0}{1}</div>", container2, container.CurrentListInstance.wim.XHtmlDataTop.ToString());

                string grid = null;
                if (!container.CurrentListInstance.wim.Page.HideTabbedGrid)
                {
                    grid = string.Format(@"
                        {1}{0}
                        <fieldset class=""axcontainer"" id=""main"">
                            <div id=""mainContent"">{2}
                            {3}{4}{5}{6}
                            </div>
                        </fieldset>"
                        , container.CurrentListInstance.wim.Page.HideTabbedGrid ? null : tabTag //0
                        , titleLine //1
                        , description //2
                        , string.IsNullOrEmpty(filters) ? string.Empty : string.Concat("<fieldset>", filters, "</fieldset>") //3
                        , file == null ? null : string.Format("<img src=\"{0}\" />", file) //4
                        , containsSublist ? browsing.Replace("<bottombuttonbar />", bottomnavigation) : browsing //5
                        , containsSublist ? null : bottomnavigation //6
                        );
                }
                string service = null;
                if (container.CurrentListInstance.wim.XHtmlDataService != null)
                {
                    service = string.Format(@"<div class=""updates""><h2>{1}</h2></div><ul class=""links"">{0}</ul>",
                        container.CurrentListInstance.wim.XHtmlDataService, container.CurrentListInstance.wim.XHtmlDataServiceTitle
                        );
                }
                string buttons = null;
                if (container.CurrentListInstance.wim.XHtmlDataButtons != null)
                {
                    buttons = string.Format(@"<div class=""updates""><h2>{1}</h2></div><ul class=""links"">{0}</ul>",
                        container.CurrentListInstance.wim.XHtmlDataButtons, "Actions"
                        );
                }


                return string.Format(@"<input type=""hidden"" id=""repository"" value=""{13}"">
<input type=""hidden"" id=""no_link_sel"" value=""{21}"">
        <div id=""canvas"">{2}
			<div id=""banner"">
				<div id=""bannerContent"">
					&nbsp;
				</div>
			</div>
			<div id=""content""{20}>{4}{19}
                <div id=""centerColumn"">
                    {5}{22}{23}                   
				</div>
                <div id=""rightColumn"">
					{24}{25}
                </div>

				<div class=""clear""></div>
			</div>
		</div>{3}
"
                    , header //0
                    , container.WimRepository //1
                    , top //2
                    , footer //3
                    , leftnavigation //4
                    , container.CurrentListInstance.wim.Page.HideMenuBar ? null : breadcrumb //5
                    , containsSublist ? browsing.Replace("<bottombuttonbar />", bottomnavigation) : browsing //6
                    , title //7
                    , container.GetSafeUrl() //8
                    , string.IsNullOrEmpty(filters) ? string.Empty : string.Concat("<fieldset>", filters, "</fieldset>") //9
                    , description //10
                    , container.CurrentListInstance.wim.CurrentFolder.Name //11
                    , string.IsNullOrEmpty(exportUrl) ? null : string.Format("<iframe src=\"{0}\" class=\"hiddenx\" width=\"500\" height=\"500\">", string.IsNullOrEmpty(exportUrl) ? "about:blank" : exportUrl) //12
                    , container.BaseRepository //13
                    , container.CurrentListInstance.wim.Page.HideTabs ? null : tabTag //14
                    , titleLine //15
                    , Sushi.Mediakiwi.Data.Common.HasWideInterface ? " reallyWide reallyReallyWide" : null // 16
                    , file == null ? null : string.Format("<img src=\"{0}\" />", file) // 17
                    , containsSublist ? null : bottomnavigation //18
                    , downloadUrl // 19
                    , container.CurrentListInstance.wim.ShowInFullWidthMode ? @" class=""fullContent""" : null //20
                    , Labels.ResourceManager.GetString("no_link_selected", new CultureInfo(container.CurrentApplicationUser.LanguageCulture)) //21
                    , textData //22
                    , grid // 23
                    , service //24
                    , buttons // 25
                    );

            }

            bool showNew = true;

            return string.Format(@"<input type=""hidden"" id=""repository"" value=""{13}"">
<input type=""hidden"" id=""no_link_sel"" value=""{21}"">
        <div id=""canvas"">{2}
			<div id=""banner"">
				<div id=""bannerContent"">
					&nbsp;
				</div>
			</div>
			<div id=""content""{20}>{4}{5}{19}
				<div id=""mainColumn""{23}> {15}{14}
                    <fieldset class=""axcontainer"" id=""main"">
                        <div id=""mainContent"">{10}{24}
                        {9}{17}{6}{18}{22}
                        </div>
                    </fieldset>
				</div>
				<div class=""clear""></div>
			</div>
		</div>{3}
"
                , header //0
                , container.WimRepository //1
                , top //2
                , footer //3
                , leftnavigation //4
                , container.CurrentListInstance.wim.Page.HideMenuBar ? null : breadcrumb //5
                , containsSublist ? browsing.Replace("<bottombuttonbar />", bottomnavigation) : browsing //6
                , title //7
                , container.GetSafeUrl() //8
                , string.IsNullOrEmpty(filters) ? string.Empty : string.Concat("<fieldset>", filters, "</fieldset>") //9
                , description //10
                , container.CurrentListInstance.wim.CurrentFolder.Name //11
                , string.IsNullOrEmpty(exportUrl) ? null : string.Format("<iframe src=\"{0}\" class=\"hiddenx\" width=\"500\" height=\"500\">", string.IsNullOrEmpty(exportUrl) ? "about:blank" : exportUrl) //12
                , container.BaseRepository //13
                , container.CurrentListInstance.wim.Page.HideTabs ? "<ul id=\"tabNavigation\" class=\"empty\"></ul>" : tabTag //14
                , titleLine //15
                , Sushi.Mediakiwi.Data.Common.HasWideInterface ? " reallyWide reallyReallyWide" : null // 16
                , file == null ? null : string.Format("<img src=\"{0}\" />", file) // 17
                , containsSublist ? null : bottomnavigation //18
                , downloadUrl // 19
                , @" class=""fullContent""" //20
                , Labels.ResourceManager.GetString("no_link_selected", new CultureInfo(container.CurrentApplicationUser.LanguageCulture)) //21
                , container.CurrentListInstance.wim.XHtmlDataBottom == null ? null : container.CurrentListInstance.wim.XHtmlDataBottom.ToString() // 22
                , !showNew
                    ? null 
                    : container.CurrentList.Type != Data.ComponentListType.Browsing 
                        ? null 
                        : !container.CurrentApplicationUser.ShowDetailView
                            ? " class=\"appview\"" 
                            : null // 23

                    , container.CurrentListInstance.wim.Page.TMP_ReportingSection // 24
                );
        }

        /// <summary>
        /// Logins the specified container.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="header">The header.</param>
        /// <returns></returns>
        internal static string Login(Console container, string header)
        {
            string username = container.Request.Form["frmUsername"];
            string username2 = username;

            if (string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(container.Request.Query["u"]))
            {
                using (Utilities.Authentication auth = new Utilities.Authentication())
                {
                    auth.Password = "urlinfo";
                    //  The replacement is a patch
                    username2 = auth.Decrypt(container.Request.Query["u"]);
                }
            }


            string password = container.Request.Form["frmPassword"];
            bool rememberMe = !string.IsNullOrEmpty(container.Request.Form["frmRemember"]);
            string emailaddress = container.Request.Form["frmEmailAddress"];

            if (!string.IsNullOrEmpty(emailaddress))
            {
                var appUser = Data.ApplicationUser.Select(emailaddress);

                if (!appUser.IsNewInstance)
                {
                    string urlAddition = null;
                    using (Utilities.Authentication auth = new Utilities.Authentication())
                    {
                        auth.Password = "urlinfo";
                        urlAddition = string.Concat("?u=", WebUtility.UrlEncode(auth.Encrypt(appUser.Name)));
                    }

                    Utilities.Mail.Send(new System.Net.Mail.MailAddress(appUser.Email, appUser.Displayname)
                        , "Forgotten password"
                        , string.Format("You have requested your password through the Forgotten password page. Your password is: <b>{0}</b>", appUser.Password)
                        , string.Concat(container.AddApplicationPath(CommonConfiguration.PORTAL_PATH, true), urlAddition));

                    //container.CurrentEnvironment.LoginBody = string.Concat(container.CurrentEnvironment.LoginBody, "<br/><br/><b>Your credentials have been send to the applied e-mail address.</b>");

                    username = null;
                    password = null;
                    emailaddress = null;
                }
            }

            container.CurrentApplicationUser = Data.ApplicationUserLogic.Apply(username, password, rememberMe, container.Context);
            if (!container.CurrentApplicationUser.IsNewInstance)
            {
                container.CurrentVisitor.ApplicationUserID = container.CurrentApplicationUser.ID;
                container.SaveVisit();

                //Sushi.Mediakiwi.Data.AuditTrail.Insert(Data.AuditType.Action, Data.AuditAction.Login, null, container.CurrentApplicationUser.ID, string.Format("{0} ({1})", container.CurrentApplicationUser.Displayname, container.CurrentApplicationUser.Email)); 
                return null;
            }

            if (string.IsNullOrEmpty(username2) && container.Request.HasFormContentType && container.Request.Form.Count == 0)
            {
                if (container.CurrentVisitor.ApplicationUserID.HasValue)
                {
                    username2 = Data.ApplicationUser.SelectOne(container.CurrentVisitor.ApplicationUserID.Value).Name;
                }
            }
            return $@"
		<div id=""loginCanvas"">
			<div id=""loginLogo"">
				<a href=""?1""><img src=""{container.WimRepository}/logo.png"" width=""243"" height=""60"" /></a>
			</div>
			<div id=""loginContent"">
				<div id=""loginVersion"">
					Version {CommonConfiguration.VersionFull}
				</div>
				<ul id=""loginTabs""></ul>
				<div id=""loginForms"">
					<div id=""tab0"" class=""contentTab"">
						<h1> </h1>
						<p> </p>
						<fieldset>
							<table class=""form"">
								<tfoot>
									<tr>
										<th>* Required fields</th>
										<td>
											<div class=""remember"">
                                                <input type=""checkbox"" tabindex=""3"" class=""checkbox"" id=""frmRemember"" name=""frmRemember""{(rememberMe ? " checked=\"checked\"" : string.Empty)} value=""1"" />
												<label for=""frmRemember"">Remember my details</label>
											</div>
											<button class=""postBack"" tabindex=""4"">Login</button>
										</td>
									</tr>
								</tfoot>
								<tbody>
									<tr>
										<th><label for=""frmUsername"">Username:<em>*</em></label></th>
										<td>
											<input type=""text"" class=""text{(string.IsNullOrEmpty(username) ? string.Empty : " error")}"" tabindex=""1"" name=""frmUsername"" value=""{username2}"" />
										</td>
									</tr>
									<tr>
										<th><label for=""frmPassword"">Password:<em>*</em></label></th>
										<td>
											<input type=""password"" class=""text{(string.IsNullOrEmpty(username) ? string.Empty : " error")}"" tabindex=""2"" name=""frmPassword""/>
										</td>
									</tr>
								</tbody>
							</table>
						</fieldset>
					</div>
				</div>
			</div>
		</div>";
                //, container.WimRepository //0
               // , header //1
               // , container.GetSafeUrl() //2
                //, username2 //3
                //, emailaddress //4
               // , (rememberMe ? " checked=\"checked\"" : string.Empty) //5
                //, string.IsNullOrEmpty(username) ? string.Empty : " error" //6
                //, string.IsNullOrEmpty(emailaddress) ? string.Empty : " error" //7
                //, CommonConfiguration.VersionFull //8
                //, null //9
                //, null //10
                //, null //11
                //, null //12
                //, string.IsNullOrEmpty(container.CurrentApplicationUser.GetSkin()) ? null : string.Concat(container.CurrentApplicationUser.GetSkin(), "/") //13
                //);

        }
    }
}
