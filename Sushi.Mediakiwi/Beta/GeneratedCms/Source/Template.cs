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
        static void ApplyTabularUrl(Console container, Sushi.Mediakiwi.Framework.WimComponentListRoot.Tabular t, int levelEntry)
        {
            ApplyTabularUrl(container, t, levelEntry, null);
        }

        static string GetQueryStringRecording(Console container)
        {
            string addition = string.Empty;
            if (container.CurrentListInstance.wim._QueryStringRecording != null)
            {
                container.CurrentListInstance.wim._QueryStringRecording.ForEach(x => {
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
    }
}
