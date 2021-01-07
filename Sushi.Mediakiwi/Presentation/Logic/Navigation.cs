using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework.Api;
using Sushi.Mediakiwi.Data.Interfaces;
using Sushi.Mediakiwi.Interfaces;
using Sushi.Mediakiwi.Logic;

namespace Sushi.Mediakiwi.Framework.Presentation.Logic
{
    /// <summary>
    /// 
    /// </summary>
    public class Navigation : iPresentationNavigation
    {
        bool HasRoleAccess(IMenuItemView item, IApplicationUser user)
        {
            if (item.TypeID == 1)
            {
                var tmp = Sushi.Mediakiwi.Data.ComponentList.SelectOne(item.ItemID);
                return tmp.HasRoleAccess(user);
            }
            if (item.TypeID == 2 || item.TypeID == 8)
            {
                var tmp = Sushi.Mediakiwi.Data.Folder.SelectOne(item.ItemID);
                return tmp.HasRoleAccess(user);
            }
            return true;
        }

        bool HasRoleAccess(ISearchView item, Sushi.Mediakiwi.Data.IApplicationUser user)
        {
            if (item.TypeID == 1)
            {
                var tmp = Sushi.Mediakiwi.Data.ComponentList.SelectOne(item.ItemID);
                return tmp.HasRoleAccess(user);
            }
            if (item.TypeID == 2 || item.TypeID == 8)
            {
                var tmp = Sushi.Mediakiwi.Data.Folder.SelectOne(item.ItemID);
                return tmp.HasRoleAccess(user);
            }
            return true;
        }

        bool IsRequestPartOfNavigation(IMenuItemView item, Sushi.Mediakiwi.Beta.GeneratedCms.Console container)
        {
            var list = container.CurrentList;
            var folder = container.CurrentListInstance.wim.CurrentFolder;
            var page = container.CurrentListInstance.wim.CurrentPage;
            var site = container.CurrentListInstance.wim.CurrentSite;

            //  When the topnav is a section
            if (item.TypeID == 7)
            {
                //  When the item is of type 'website'
                if (item.ItemID == 1)
                {
                    if (folder != null && !folder.IsNewInstance && folder.Type == FolderType.Page && folder.SiteID == site.ID)
                    {
                        return true;
                    }
                }
                //  When the item is of type 'lists'
                if (item.ItemID == 2)
                {
                    if (folder != null && !folder.IsNewInstance && folder.Type == FolderType.List)
                    {
                        return true;
                    }
                }
                //  When the item is of type 'gallery'
                if (item.ItemID == 3)
                {
                    if (folder != null && !folder.IsNewInstance && folder.Type == FolderType.Gallery)
                    {
                        return true;
                    }
                }
                //  When the item is of type 'admin'
                if (item.ItemID == 4)
                {
                    if (folder != null && !folder.IsNewInstance && folder.Type == FolderType.Administration)
                    {
                        return true;
                    }
                }
            }

            //  When the topnav is a section
            if (item.TypeID == 6)
            {
                //  When the item is of type 'website'
                if (item.ItemID == site.ID)
                {
                    if (folder != null && !folder.IsNewInstance && folder.Type == FolderType.Page)
                    {
                        return true;
                    }
                }
            }

            //  When the topnav item is a list
            if (item.TypeID == 1)
            {
              
                if (list != null && !list.IsNewInstance)
                {
                    if (item.ItemID == container.CurrentList.ID)
                        return true;
                }
            }
            //  When the topnav item is a folder
            if (item.TypeID == 2 || item.TypeID == 8)
            {
                if (folder != null && !folder.IsNewInstance)
                {
                    if (item.ItemID == folder.ID)
                        return true;
                }
            }
            return false;
        }

        public string TopNavigation(Sushi.Mediakiwi.Beta.GeneratedCms.Console container)
        {
            string navigation = null;

            #region MenuItemView
            var list = MenuItemView.SelectAll(container.ChannelIndentifier, container.CurrentApplicationUser.RoleID);
            if (list.Length > 0)
            {
                StringBuilder build = new StringBuilder();
                string className = "";
                var selection = (from item in list where item.Sort == 1 select item).ToArray();
                foreach (var item in selection)
                {
                    //IMenuItemView item = candidate;
                    //if (item.TypeID == 8 || item.TypeID == 2)
                    //{
                    //    // folder/container or folder
                    //    Data.Folder inherited = Data.Folder.SelectOneChild(item.ItemID, container.ChannelIndentifier);
                    //    item.ItemID = (inherited == null || inherited.IsNewInstance)
                    //        ? item.ItemID
                    //        : inherited.ID;
                    //}


                    StringBuilder innerbuild = new StringBuilder();
                    bool isSelected = false;
                    if (!HasRoleAccess(item, container.CurrentApplicationUser))
                        continue;

                    var subnavigation = (from subnav in list where subnav.Sort != 1 && subnav.Position == item.Position select subnav).ToList();
                   
                    if (item.TypeID == 8)
                    {
                        var subSubnavigation = Sushi.Mediakiwi.Data.SearchView.SelectAll(item.ItemID);

                        foreach (var subItem in subSubnavigation.Reverse())
                        {
                            //  If the item is the same as the callee 
                            if (subItem.ID == item.Tag || subItem.ID == string.Concat("2_", item.ItemID))
                                continue;

                            if (!HasRoleAccess(subItem, container.CurrentApplicationUser))
                                continue;

                            subnavigation.Insert(0, new MenuItemView() { ItemID = subItem.ItemID, TypeID = subItem.TypeID, Name = subItem.Title });
                        }
                    }
                    string firstInLineUrl = null;
                    bool linkable = true;

                    if (subnavigation.Count > 0)
                    {
                        innerbuild.AppendFormat("</a>");
                        innerbuild.AppendFormat("\n\t\t\t\t\t<div class=\"sub\"><div class=\"arrow-up\"></div><menu class=\"first\"><ul>");
                        className = "";


                        for (int index = 0; index < subnavigation.Count; index++)
                        {
                            if (index == 0)
                            {
                                if (item.TypeID == 8)
                                {
                                    firstInLineUrl = GetUrl(container, subnavigation[index], container.ChannelIndentifier);
                                    linkable = false;
                                    //continue;
                                }
                                className = " class=\"first\"";
                            }
                            else
                                className = string.Empty;

                            //  Only for folders
                            if (subnavigation[index].TypeID == 8)
                            {
                                if (AddSubSubNavigation(container, item, subnavigation[index], innerbuild, className))
                                    isSelected = true;
                            }
                            else
                            {
                                if (subnavigation[index].ItemID == container.CurrentList.ID)
                                    isSelected = true;

                                innerbuild.AppendFormat("\n\t\t\t\t\t\t<li><a href=\"{0}\"{2}>{1}</a></li>", GetUrl(container, subnavigation[index], container.ChannelIndentifier), subnavigation[index].Name, className);
                            }

                        }
                        innerbuild.AppendFormat("</ul></menu></div>\n\t\t\t\t</li>");
                    }
                    //else
                    //    build.AppendFormat(@"</a></li>");

                    if (!isSelected)
                        isSelected = IsRequestPartOfNavigation(item, container);

                    if (linkable)
                    {
                        build.AppendFormat("\n\t\t\t\t<li><a href=\"{0}\"{2}>{1}"
                            , string.IsNullOrEmpty(firstInLineUrl)
                                ? GetUrl(container, item, container.ChannelIndentifier)
                                : firstInLineUrl
                            , item.Name
                            , isSelected ? " class=\"active\"" : null);

                        build.Append(innerbuild);
                        if (innerbuild.Length == 0)
                            build.AppendFormat(@"</a></li>");
                    }
                    else
                    {
                        build.AppendFormat("\n\t\t\t\t<li><a style=\"cursor:initial\">{1}"
                            , string.IsNullOrEmpty(firstInLineUrl)
                                ? GetUrl(container, item, container.ChannelIndentifier)
                                : firstInLineUrl
                            , item.Name
                            , isSelected ? " class=\"active\"" : null);

                        build.Append(innerbuild);
                        if (innerbuild.Length == 0)
                            build.AppendFormat(@"</a></li>");
                    }

             
                }
                navigation = build.ToString();
            }
            #endregion MenuItemView
            else
            {
                string top_page = container.CurrentListInstance == null ? null : container.CurrentListInstance.wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Page ? " class=\"active\"" : null;
                string top_logic = container.CurrentListInstance == null ? null : container.CurrentListInstance.wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.List ? " class=\"active\"" : null;
                string top_gallery = container.CurrentListInstance == null ? null : container.CurrentListInstance.wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Gallery ? " class=\"active\"" : null;
                string top_administration = container.CurrentListInstance == null ? null : container.CurrentListInstance.wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Administration ? " class=\"active\"" : null;


                Sushi.Mediakiwi.Data.IApplicationRole role = Sushi.Mediakiwi.Data.ApplicationRole.SelectOne(container.CurrentApplicationUser.RoleID);
                bool hasWeb = role.CanSeePage;
                bool hasLogic = role.CanSeeList;
                bool hasLibraries = role.CanSeeGallery;
                bool hasAdministration = role.CanSeeAdmin;

                if (container.CurrentListInstance != null)
                {
                    if (hasWeb && !container.CurrentListInstance.wim.CurrentSite.HasPages) hasWeb = false;
                    if (hasLogic && !container.CurrentListInstance.wim.CurrentSite.HasLists) hasLogic = false;
                }

                string skin = null;
                navigation = string.Concat(""
                    , hasWeb ? string.Format(@"<li class=""first""><a href=""{0}?top=1""{2}>{4}</a></li>", container.WimPagePath, container.WimRepository, top_page, skin, Labels.ResourceManager.GetString("section_web", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))) : string.Empty
                    , hasLogic ? string.Format(@"<li><a href=""{0}?top=2""{2}>{4}</a></li>", container.WimPagePath, container.WimRepository, top_logic, skin, Labels.ResourceManager.GetString("section_logic", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))) : string.Empty
                    , hasLibraries ? string.Format(@"<li><a href=""{0}?top=3""{2}>{4}</a></li>", container.WimPagePath, container.WimRepository, top_gallery, skin, Labels.ResourceManager.GetString("section_galleries", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))) : string.Empty
                    , hasAdministration ? string.Format(@"<li><a href=""{0}?top=4""{2}>{4}</a></li>", container.WimPagePath, container.WimRepository, top_administration, skin, Labels.ResourceManager.GetString("section_admin", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))) : string.Empty
                    );
            }
            return navigation;
        }

        public string GetUrl(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, ISearchView entity)
        {
            var querystring = string.Empty;

            switch (entity.TypeID)
            {
                case 1:
                    return container.UrlBuild.GetListRequest(Convert.ToInt32(entity.ItemID));

                case 2: querystring = $"?folder={entity.ItemID}"; break;
                case 3: querystring = $"?page={entity.ItemID}"; break;
                case 4: querystring = $"?dashboard={entity.ItemID}"; break;
                case 5: querystring = $"?ligalleryst={entity.ItemID}"; break;
                case 6: querystring = $"?top=1"; break;
                case 7: querystring = $"?top={entity.ItemID}"; break;
                case 8: querystring = $"?folder={entity.ItemID}"; break;
            }
            
            return container.UrlBuild.GetUrl();
        }

        public string GetUrl(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, IMenuItemView entity, int channel)
        {
            var querystring = string.Empty;

            switch (entity.TypeID)
            {
                case 1:
                    return container.UrlBuild.GetListRequest(Convert.ToInt32(entity.ItemID));

                case 2: querystring = $"?folder={entity.ItemID}"; break;
                case 3: querystring = $"?page={entity.ItemID}"; break;
                case 4: querystring = $"?dashboard={entity.ItemID}"; break;
                case 5: querystring = $"?ligalleryst={entity.ItemID}"; break;
                case 6: querystring = $"?top=1"; break;
                case 7: querystring = $"?top={entity.ItemID}"; break;
                case 8: querystring = $"?folder={entity.ItemID}"; break;
            }

            return string.Concat(container.GetWimPagePath(channel), querystring);
        }

        bool AddSubSubNavigation(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, IMenuItemView topnav, IMenuItemView item, StringBuilder build, string className)
        {
            bool isCurrent = false;

            var subSubnavigation = SearchView.SelectAll(item.ItemID);

            if (subSubnavigation.Length > 0)
            {
                foreach (var subItem in subSubnavigation)
                {
                    //  If the item is the same as the callee 
                    if (subItem.ID == item.Tag || subItem.ID == string.Concat("2_", item.ItemID))
                        continue;

                    if (!HasRoleAccess(subItem, container.CurrentApplicationUser))
                        continue;

                    if (topnav.ItemID == subItem.ItemID)
                        continue;

                    if (subItem.ItemID == container.CurrentList.ID)
                        isCurrent = true;

                    build.AppendFormat("\n\t\t\t\t\t\t<li><a href=\"{0}\">{1}</a></li>", GetUrl(container, subItem), subItem.Title);
                }
                build.AppendFormat("\n\t\t\t\t\t\t<li><div class=\"empty\">&nbsp;</div></li>");
            }
            return isCurrent;
        }

        internal static string GetTabularTagNewDesign(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, string title, int selectedTab, bool showServiceUrl)
        {
            title = container.CurrentList.Name;

            if (container.CurrentListInstance.wim.Page.HideTabs)
            {
                return null;
            }

            string tabTag = null;
            #region Browsing
            bool isPageProperty = container.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.PageProperties;            
            if (container.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.Browsing || isPageProperty)
            {
                title = Labels.ResourceManager.GetString("list_browsing", new CultureInfo(container.CurrentApplicationUser.LanguageCulture));
                #region PAGE
                if (container.ItemType == RequestItemType.Page || isPageProperty)
                {                    
                    Page currentPage;
                    if (container.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.PageProperties)
                        currentPage = Sushi.Mediakiwi.Data.Page.SelectOne(container.Item.Value);
                    else
                        currentPage = container.CurrentPage;

                    if (currentPage.Template.GetPageSections().Length > 0)
                    {
                        var sections = currentPage.Template.GetPageSections();

                        StringBuilder build = new StringBuilder();

                        var selected = container.Request.Query["tab"];
                        bool? isSelected = null;

                        Sushi.Mediakiwi.Data.IComponentList pageSettings = Sushi.Mediakiwi.Data.ComponentList.SelectOne(new Guid("4E7BCF0F-844B-4877-AB2D-3154BE01BC0F"));

                        if (sections.Length > 0)
                        {
                            build.AppendFormat(string.Format(@" <li class=""active""><a href=""{0}"">{1}</a>"
                                , string.Concat(container.WimPagePath, "?list=", pageSettings.ID, "&item=", container.Item)//, "&tab=", sections[0])
                                , Labels.ResourceManager.GetString("page_properties", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                            ));
                        }
                        if (sections.Count() > 0)
                        {
                            build.Append("<ul>");
                            foreach (var section in sections)
                            {
                                if (string.IsNullOrEmpty(section))
                                    continue;

                                if ((!isPageProperty && string.IsNullOrEmpty(selected) && !isSelected.HasValue) || selected == section)
                                    isSelected = true;
                                else
                                    isSelected = false;

                                var tabName = currentPage.Template.Data[string.Format("T[{0}]", section)].Value;
                                if (string.IsNullOrEmpty(tabName))
                                    tabName = section;

                                build.AppendFormat(string.Format(@" <li{2}><a href=""{0}"">{1}</a></li>"
                                    , string.Concat(container.WimPagePath, "?page=", container.Item, "&tab="
                                    , section
                                        )
                                    , tabName
                                    , isSelected.GetValueOrDefault() ? " class=\"active\"" : null
                                ));
                            }
                            build.Append("</ul>");
                        }
                        build.Append("</li>");
                        build.Append(@"</ul>");
                        tabTag = build.ToString();

                    }
                    else
                    {
                        title = Labels.ResourceManager.GetString("tab_Content", new CultureInfo(container.CurrentApplicationUser.LanguageCulture));
                        //"Content"
                        tabTag = string.Format(@"
			                     <li{2}><a href=""{1}"">{0}</a></li>{3}"
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
			            <li class=""active""><a href=""{1}"">{0}</a></li>"
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
			            <li class=""active""><a href=""{1}"">{0}</a></li>"
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
                        <li{3}><a href=""{2}"">{4}</a></li>"
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
                    return null;
                }

                if (container.Item.HasValue || isSingleItemList)
                {
                    var master = container;

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
                                Sushi.Mediakiwi.Data.IComponentList innerlist = Sushi.Mediakiwi.Data.ComponentList.SelectOne(groupId);

                                //  The current requested list is not the list that is the base of the tabular menu
                                master = container.ReplicateInstance(innerlist);
                                master.CurrentListInstance.wim.Console = master;
                                master.CurrentListInstance.wim.Console.Item = groupElementId;

                                master.CurrentListInstance.wim.DoListInit();
                                if (!master.CurrentListInstance.wim.CurrentList.Option_FormAsync)
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

                            tabulars += string.Format(@"<li{2}><a href=""{1}"">{0}</a></li>"
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

                                        tabulars += string.Format(@"<li{2}><a href=""{1}"">{0}</a></li>"
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
                                IComponentListTemplate cl = t.List.GetInstance(container.Context);
                                if (cl != null)
                                {
                                    cl.wim.Console = master;

                                    int group2Id = Utility.ConvertToInt(container.Request.Query["group2"]);
                                    int group2ElementId = Utility.ConvertToInt(container.Request.Query["group2item"]);

                                    if (t.List.ID == group2Id)
                                        cl.wim.DoListLoad(group2ElementId, 0);
                                    else
                                        cl.wim.DoListLoad(container.Item.Value, 0);

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
                        Sushi.Mediakiwi.Data.IComponentList list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(baseID);
                        title = list.Name;
                        currentListId = list.ID;
                    }

                    if (isSingleItemList)
                    {
                        tabTag = string.Format(@"
                        <li{2}><a href=""{1}"">{3}</a></li>{4}"
                            , title
                            , container.UrlBuild.GetListRequest(currentListId)
                            , selectedTab == 0 ? " class=\"active\"" : null
                            , itemTitle, tabulars
                            );
                    }
                    else
                    {
                        string addition = GetQueryStringRecording(container);
                        tabTag = string.Format(@"
			            <li class=""active""><a href=""{1}"">{0}</a>{2}
                        </li>
                        "
                            , container.CurrentList.Name// itemTitle
                            , container.UrlBuild.GetListRequest(container.CurrentList)//tmp.Url
                            , string.IsNullOrEmpty(tabulars) ? string.Empty : string.Concat("<ul>", tabulars, "</ul>")
                            );
                    }
                }
                else
                {
                    //  Show NO tabs
                    //return null;
                    tabTag = string.Format(@"
			            <li class=""active""><a href=""{1}"">{0}</a></li>"
                        , container.CurrentList.Name
                        , container.GetSafeUrl()
                        );
                }
            }
            #endregion
            return tabTag;
        }

        static void ApplyTabularUrl(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, Sushi.Mediakiwi.Framework.WimComponentListRoot.Tabular t, int levelEntry)
        {
            ApplyTabularUrl(container, t, levelEntry, null);
        }

        internal static string GetQueryStringRecording(Sushi.Mediakiwi.Beta.GeneratedCms.Console container)
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

        static void ApplyTabularUrl(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, Sushi.Mediakiwi.Framework.WimComponentListRoot.Tabular t, int levelEntry, int? currentListID)
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

        /// <summary>
        /// News the left navigation.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="buttonList">The button list.</param>
        /// <returns></returns>
        public string NewLeftNavigation(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, Framework.ContentListItem.ButtonAttribute[] buttonList)
        {
            string tabs = GetTabularTagNewDesign(container, container.CurrentList.Name, 0, false);

            bool isFirstLevelRootnavigation = !true;
            bool isFirst = true;

            StringBuilder build = new StringBuilder();

            Sushi.Mediakiwi.Data.Folder currentFolder = container.CurrentListInstance.wim.CurrentFolder;

            //  If the request is in a tabular the left navigation should show the navigation of the primary list (group ID)
            if (string.IsNullOrEmpty(container.Request.Query["folder"]) && container.Group.HasValue)
            {
                Sushi.Mediakiwi.Data.IComponentList folderList = Sushi.Mediakiwi.Data.ComponentList.SelectOne(container.Group.Value);
                if (folderList.FolderID.HasValue)
                {
                    currentFolder = Sushi.Mediakiwi.Data.Folder.SelectOne(folderList.FolderID.Value);
                    if (currentFolder.SiteID != container.CurrentListInstance.wim.CurrentSite.ID)
                    {
                        if (currentFolder.MasterID.HasValue)
                            currentFolder = Sushi.Mediakiwi.Data.Folder.SelectOne(currentFolder.MasterID.Value, container.CurrentListInstance.wim.CurrentSite.ID);
                    }

                }
            }

            string currentName = currentFolder.Name;
            string currentLink = "";
            bool isPageProperty = container.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.PageProperties;

            #region Foldertype: Galleries
            if (currentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Gallery)
            {
                int currentListID = container.Group.HasValue ? container.Group.Value : container.CurrentList.ID;

                Data.Gallery root = Data.Gallery.SelectOneRoot();

                int rootID = root.ID;
                if (container.CurrentApplicationUser.Role().GalleryRoot.HasValue)
                    rootID = container.CurrentApplicationUser.Role().GalleryRoot.Value;

                currentName = "Documents";
                currentLink = container.UrlBuild.GetGalleryRequest(rootID);

                Data.Gallery currentGallery = Data.Gallery.SelectOne(currentFolder.ID);

                Data.Gallery level1 = Data.Gallery.SelectOne(currentGallery, 1);
                Data.Gallery level2 = Data.Gallery.SelectOne(currentGallery, 2);
                Data.Gallery level3 = Data.Gallery.SelectOne(currentGallery, 3);

                //  LEVEL 1 : Folders
                Data.Gallery[] galleries1 = Data.Gallery.SelectAllByParent(rootID);

                if (!CommonConfiguration.RIGHTS_GALLERY_SUBS_ARE_ALLOWED)
                    galleries1 = Data.Gallery.ValidateAccessRight(galleries1, container.CurrentApplicationUser);

                foreach (Data.Gallery folder in galleries1)
                {
                    bool isActive = currentGallery.ID == folder.ID || level1.ID == folder.ID;
                    build.AppendFormat(@"<li><a href=""{0}"" class=""{1}{4}{3}"">{2}</a>", container.UrlBuild.GetGalleryRequest(folder), "folder", folder.Name, isActive ? " active" : "", isFirst ? " first" : null);

                    isFirst = false;

                    #region Level 2
                    if (isActive && false)
                    {
                        build.AppendFormat(@"<ul>");

                        //  LEVEL 2 : Folders
                        Data.Gallery[] galleries2 = Data.Gallery.SelectAllByParent(folder.ID);
                        galleries2 = Data.Gallery.ValidateAccessRight(galleries2, container.CurrentApplicationUser);

                        foreach (Data.Gallery folder2 in galleries2)
                        {
                            bool isActive2 = (folder2.ID == currentGallery.ID) || level2.ID == folder2.ID;
                            build.AppendFormat(@"<li><a href=""{0}"" class=""{1}{3}"">{2}</a>", container.UrlBuild.GetGalleryRequest(folder2), "folder", folder2.Name, isActive2 ? " active" : "");

                            #region Level 3
                            if (isActive2)
                            {
                                build.AppendFormat(@"<ul>");
                                //  LEVEL 3 : Folders
                                foreach (Data.Gallery folder3 in Data.Gallery.SelectAllByParent(folder2.ID))
                                {
                                    bool isActive3 = (folder3.ID == currentGallery.ID) || level3.ID == folder3.ID;
                                    build.AppendFormat(@"<li><a href=""{0}"" class=""{1}{3}"">{2}</a>", container.UrlBuild.GetGalleryRequest(folder3), "folder", folder3.Name, isActive3 ? " active" : "");

                                }
                                build.AppendFormat(@"</ul>");
                            }
                            #endregion
                            build.AppendFormat(@"</li>");
                        }

                        build.AppendFormat(@"</ul>");
                    }
                    #endregion

                    build.AppendFormat(@"</li>");
                }
            }
            #endregion

            #region Foldertype: Lists
            if (isPageProperty && container.Item.HasValue)
            {
                Sushi.Mediakiwi.Data.Page p = Sushi.Mediakiwi.Data.Page.SelectOne(container.Item.Value);
                string currentFolderName = p.Folder.Name;
                if (currentFolderName == "/")
                    currentFolderName = p.Folder.Site.Name;

                build.AppendFormat(@"<li class=""back""><span class=""icon-arrow-left-04""></span><a href=""{0}"">{1}</a></li>"
                    , container.UrlBuild.GetFolderRequest(p.Folder.ID), currentFolderName

                    );

                build.AppendFormat(tabs);
            }
            else if (currentFolder.Type == Sushi.Mediakiwi.Data.FolderType.List || currentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Administration)
            {
                int currentListID = container.Group.HasValue ? container.Group.Value : container.CurrentList.ID;


                if (!container.Item.HasValue)
                {
                    if (!CommonConfiguration.HIDE_BREADCRUMB)
                    {
                        build.AppendFormat(@"<li class=""back""><span class=""icon-arrow-left-04""></span><a href=""{0}"">Home</a></li>"
                            , container.AddApplicationPath(CommonConfiguration.PORTAL_PATH)
                            );
                    }
                }

                if (container.CurrentList.Type != ComponentListType.Browsing)
                {
                    Data.IComponentList[] lists1 = Data.ComponentList.SelectAll(currentFolder.ID);
                    lists1 = Sushi.Mediakiwi.Data.ComponentList.ValidateAccessRight(lists1,
                        container.CurrentApplicationUser,
                        container.CurrentListInstance.wim.CurrentSite.ID);

                    foreach (Data.ComponentList list in lists1)
                    {
                        if (container.Item.HasValue)
                        {
                            if (container.Group.HasValue)
                            {
                                if (container.Group.Value != list.ID)
                                    continue;
                            }
                            else
                            {
                                if (list.ID != container.CurrentList.ID)
                                    continue;
                            }
                        }

                        string dataReport = null;
                        ComponentDataReportEventArgs e = null;
                        if (list.Option_HasDataReport && !(list.ID == currentListID && container.Item.HasValue))
                        {
                            var instance = list.GetInstance(container.Context);
                            if (instance != null)
                            {
                                e = instance.wim.DoListDataReport();
                                if (e != null && e.ReportCount.HasValue)
                                {
                                    string count = e.ReportCount.Value.ToString();
                                    if (e.ReportCount.Value > 99)
                                        count = "99+";

                                    dataReport = string.Format(" <span class=\"items\">{0}</span>", count);
                                }
                            }
                        }

                        if (list.IsVisible)
                        {
                            string x = string.Format(@"<li{3}>{6}<a href=""{0}"" class=""{1}{4}"">{8}{2}{7}</a>{5}</li>"
                                , container.UrlBuild.GetListRequest(list)
                                , "list"
                                , list.Name
                                , list.ID == currentListID ? (container.Item.HasValue ? @" class=""back""" : @" class=""active""") : string.Empty
                                , isFirst ? " first" : null
                                //, list.ID == currentListID ? tabs : string.Empty
                                , container.Item.HasValue ? (list.ID == currentListID ? tabs : string.Empty) : string.Empty
                                , list.ID == currentListID ? (container.Item.HasValue ? @"<span class=""icon-arrow-left-04""></span>" : string.Empty) : string.Empty
                                , dataReport
                                , string.IsNullOrWhiteSpace(list.Icon) ? null : $"<i class=\"{list.Icon}\"></i> " //8
                                );
                            build.Append(x);
                            isFirst = false;
                        }
                    }
                }

                if (isFirst)
                {
                    if (!string.IsNullOrEmpty(container.Request.Query["base"]))
                    {
                        var list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(Convert.ToInt32(container.Request.Query["base"]));
                        build.AppendFormat(@"<li class=""back""><span class=""icon-arrow-left-04""></span><a href=""{0}"">{2}{1}</a></li>"
                            , container.UrlBuild.GetListRequest(list)
                            , list.Name
                            , string.IsNullOrWhiteSpace(list.Icon) ? null : $"<i class=\"{list.Icon}\"></i> " //2

                            );
                    }

                    build.Append(tabs);
                }
            }
            #endregion
            #region Foldertype: Pages
            else if (currentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Page)
            {
                if (container.ItemType == RequestItemType.Page)
                {
                    string currentFolderName = currentFolder.Name;
                    if (currentFolderName == "/")
                        currentFolderName = currentFolder.Site.Name;

                    build.AppendFormat(@"<li class=""back""><span class=""icon-arrow-left-04""></span><a href=""{0}"">{1}</a></li>"
                        , container.UrlBuild.GetFolderRequest(currentFolder.ID), currentFolderName
                        );

                    build.AppendFormat(tabs);
                }
                else
                {
                    Data.Folder root;
                    if (isFirstLevelRootnavigation)
                        root = Data.Folder.SelectOne(currentFolder, 1);
                    else
                        root = Data.Folder.SelectOneBySite(container.CurrentListInstance.wim.CurrentSite.ID, currentFolder.Type);

                    var arr = Data.Folder.SelectAllByParent(root.ID);

                    foreach (var item in arr)
                    {
                        if (item.IsVisible)
                        {
                            build.AppendFormat(@"<li{3}><a href=""{0}"" class=""{1}{4}"">{2}</a></li>"
                                , container.UrlBuild.GetFolderRequest(item)
                                , "list"
                                , item.Name
                                , null
                                , isFirst ? " first" : null);
                            isFirst = false;
                        }
                    }
                }
            }
            #endregion

           
            if (container.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.InformationMessage)
            {
                build = new StringBuilder();
                build.AppendFormat(@"<li><a href=""{0}"" class=""{1}"">{2}</a></li>", container.UrlBuild.GetHomeRequest(), "list", "Home");
            }

            if (!string.IsNullOrEmpty(currentLink))
                currentName = string.Format("<a href=\"{1}\">{0}</a>", currentName, currentLink);
            
            return string.Format(@"
            <aside id=""homeAside"">
				<section id=""sideNav"">
	                <menu class=""sideNav"">
		                <ul>
                            {2}
		                </ul>
	                </menu>
                </section>
			</aside>"
                , container.WimRepository
                , currentName
                , build.ToString()
                );
        }

        public string NewBottomNavigation(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, Framework.ContentListItem.ButtonAttribute[] buttonList, bool hasFilters)
        {
            WimControlBuilder builder = new WimControlBuilder();
            return NewBottomNavigation(container, buttonList, hasFilters, builder);
        }

        bool _HasFilterButton = false;
        /// <summary>
        /// News the bottom navigation.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="buttonList">The button list.</param>
        /// <param name="hasFilters">if set to <c>true</c> [has filters].</param>
        /// <returns></returns>
        public string NewBottomNavigation(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, Framework.ContentListItem.ButtonAttribute[] buttonList, bool hasFilters, WimControlBuilder builder)
        {
            StringBuilder build = new StringBuilder();
            StringBuilder build2 = new StringBuilder();

            string saveRecord = container.CurrentList.Data["wim_LblSave"].Value;
            if (string.IsNullOrEmpty(saveRecord))
                saveRecord = Labels.ResourceManager.GetString("save", new CultureInfo(container.CurrentApplicationUser.LanguageCulture));

            Data.FolderType section = container.CurrentListInstance.wim.CurrentFolder.Type;

            //  When in the page section, but on a property list page the actual section = list
            if (section == Data.FolderType.Page && container.CurrentList.Type != Sushi.Mediakiwi.Data.ComponentListType.Browsing)
                section = Data.FolderType.List;

            bool isEditMode = container.CurrentListInstance.wim.IsEditMode;
            bool isTextMode = !isEditMode;
            bool isListMode = container.View == 2 || container.View == 1;
            bool isBrowseMode = container.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.Browsing && !container.Item.HasValue;

            List<Framework.ContentListItem.ButtonAttribute> bottom = new List<Sushi.Mediakiwi.Framework.ContentListItem.ButtonAttribute>();
            if (buttonList != null)
            {
                foreach (Framework.ContentListItem.ButtonAttribute item in buttonList)
                    if (item.IconTarget == Sushi.Mediakiwi.Framework.ButtonTarget.BottomLeft || item.IconTarget == Sushi.Mediakiwi.Framework.ButtonTarget.BottomRight) bottom.Add(item);
            }

            string search = container.CurrentList.Label_Search;

            if (hasFilters && string.IsNullOrEmpty(search))
                search = Labels.ResourceManager.GetString("search", new CultureInfo(container.CurrentApplicationUser.LanguageCulture));


            string className = " left";

            if (isTextMode)
            {
                if (
                    container.CurrentListInstance.wim.HasListSave
                    && container.CurrentListInstance.wim.CurrentList.Option_CanSave
                    && !container.CurrentListInstance.wim.IsSubSelectMode
                    && !container.CurrentListInstance.wim.HideCreateNew
                    && !container.CurrentListInstance.wim.HideEditOption
                    )
                {
                    builder.ApiResponse.Buttons.Add(new MediakiwiField()
                    {
                        PropertyName = "edit",
                        Title = Labels.ResourceManager.GetString("edit", new CultureInfo(container.CurrentApplicationUser.LanguageCulture)),
                        PropertyType = "bool",
                        ClassName = "action",
                        VueType = MediakiwiFormVueType.wimButton,
                        Event = MediakiwiJSEvent.click,
                        Section = ButtonSection.Bottom
                    });

                    build2.Append(string.Format("<li><a id=\"edit\" href=\"#\" class=\"postBack submit\">{0}</a></li>"
                        , Labels.ResourceManager.GetString("edit", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                        ));
                }
            }

            if (isEditMode && !isListMode)
            {
                if (section == Data.FolderType.List && !container.CurrentListInstance.wim.HasListSave)
                {
                    isEditMode = false;
                    isTextMode = true;
                }

                if (container.CurrentListInstance.wim.HasListSave && (container.CurrentListInstance.wim.CanAddNewItem || container.Item.GetValueOrDefault() > 0 || container.CurrentListInstance.wim.CurrentList.IsSingleInstance))
                {
                    if (!container.CurrentListInstance.wim.HideSaveButtons && container.CurrentListInstance.wim.CurrentList.Data["wim_CanSave"].ParseBoolean(true))
                    {
                        builder.ApiResponse.Buttons.Add(new MediakiwiField()
                        {
                            PropertyName = "save",
                            Title = saveRecord,
                            PropertyType = "bool",
                            VueType = MediakiwiFormVueType.wimButton,
                            Event = MediakiwiJSEvent.click,
                            Section = ButtonSection.Bottom,
                            ClassName = string.Format("{0} right", string.IsNullOrEmpty(container.CurrentListInstance.wim.Page.Body.Form._PrimairyAction) ? " action" : null)
                        });

                        build2.AppendFormat("<input id=\"save\" class=\"submit postBack{1} right\" type=\"submit\" value=\"{0}\">", saveRecord
                            , string.IsNullOrEmpty(container.CurrentListInstance.wim.Page.Body.Form._PrimairyAction) ? " action" : null
                            );

                        if (section != Sushi.Mediakiwi.Data.FolderType.Page && container.CurrentListInstance.wim.CanSaveAndAddNew && !container.CurrentListInstance.wim.CanContainSingleInstancePerDefinedList)
                        {
                            build2.AppendFormat("<input id=\"saveNew\" class=\"submit postBack right\" type=\"submit\" value=\"{0}\">"
                                , Labels.ResourceManager.GetString("save_and_new", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                                );
                        }

                    }
                }
            }

            //if (container.CurrentListInstance.wim.CanAddNewItem || container.Item.GetValueOrDefault() > 0)
            //{
            //    if (container.Item.GetValueOrDefault(0) != 0)
            //    {
            //        Data.ComponentList selectedList = container.CurrentList;
            //        if (container.Item.GetValueOrDefault(container.Logic) != container.CurrentList.ID)
            //        {
            //            selectedList = Data.ComponentList.SelectOne(container.Item.GetValueOrDefault(container.Logic));
            //        }

            //        if (container.CurrentList.Type != Sushi.Mediakiwi.Data.ComponentListType.ComponentListProperties || selectedList.Type == Sushi.Mediakiwi.Data.ComponentListType.Undefined)
            //        {
            //            if (container.CurrentListInstance.HasListDelete)
            //            {
            //                if (container.CurrentListInstance.wim.CurrentList.Data["wim_CanDelete"].ParseBoolean(true))
            //                {
            //                    build2.AppendFormat("<a id=\"delete\" class=\"abbr type_confirm left flaticon icon-trash-o\"{1} title=\"{0}\"></a>"
            //                                                 , Labels.ResourceManager.GetString("delete", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
            //                                                 , ConfirmationQuestion(true, container)
            //                                               );
            //                }
            //            }
            //        }
            //    }
            //}

            if (buttonList != null)
            {
                //var selection = (from item in buttonList where item.IconTarget == Sushi.Mediakiwi.Framework.ButtonTarget.BottomLeft select item);
                //foreach (Framework.ContentListItem.ButtonAttribute button in selection)
                buttonList.Where(x => (x.IconTarget == Sushi.Mediakiwi.Framework.ButtonTarget.BottomLeft || x.IconTarget == ButtonTarget.BottomRight))
                    .ToList()
                    .ForEach(button =>
                    {
                        var url = LayerList(button, container);
                        string layerclass;
                        build2.AppendFormat("<input{6} id=\"{1}{14}\" title=\"{12}\"{13} data-title=\"{2}\" type=\"submit\" class=\"submit{4}{5}{3}{7}{8}{10}{11}{15}\"{4} value=\"{0}\"{9}>"
                            , button.Title // 0
                            , button.ID // 1
                            , button.PopupTitle // 2
                            , button.AskConfirmation ? " type_confirm" : null // 3
                            //, button.OpenInPopupLayer ? string.Concat(" ", button.PopupLayerSize.ToString()) : null //4
                            , TargetInfo(button, container) // 4
                            , button.OpenUrl ? null : button.AskConfirmation ? null : !string.IsNullOrWhiteSpace(button.CustomUrl) ? null : " postBack" // 5
                            , url == "#" ? string.Empty : string.Format(" data-link=\"{0}\"", url) // 6
                            , button.IconTarget == Sushi.Mediakiwi.Framework.ButtonTarget.BottomLeft ? " left" : " right" //7
                            , button.IsPrimary ? " action" : null // 8
                            , GetDataLayer(button.PopupLayerSize, button, out layerclass) // 9
                            //, string.IsNullOrEmpty(button.PopupLayerHeight) ? null : string.Format(" data-height=\"{0}\"", button.PopupLayerHeight)
                            //, string.IsNullOrEmpty(button.PopupLayerWidth) ? null : string.Format(" data-width=\"{0}\"", button.PopupLayerWidth)
                            //, button.PopupLayerHasScrollBar.HasValue ? string.Format(" data-scroll=\"{0}\"", button.PopupLayerHasScrollBar.Value.ToString().ToLower()) : null
                            , container.CurrentListInstance.wim.Page.Body.Form.IsPrimairyAction(button) ? " action" : null //10
                            , layerclass //11
                            , button.InteractiveHelp //12
                            , ConfirmationQuestion(button, container) //13
                            , button.TriggerSaveEvent ? "$save" : null //14
                            , url == "#" ? null : button.OpenInPopupLayer ? null : " click" // 15
                            );
                    });
            }

            if (isListMode && !string.IsNullOrEmpty(search) && !container.CurrentListInstance.wim.HideSearchButton)
            {
                Framework.ContentListSearchItem.ButtonAttribute button = new Sushi.Mediakiwi.Framework.ContentListSearchItem.ButtonAttribute(search, false, true);
                button.ID = "searchBtn";
                button.m_IsFormElement = false;
                button.Console = container;
                button.IconTarget = Sushi.Mediakiwi.Framework.ButtonTarget.BottomRight;
                button.IconType = Sushi.Mediakiwi.Framework.ButtonIconType.Play;

                _HasFilterButton = true;

                if (container.CurrentListInstance.wim.CurrentList.Option_SearchAsync)
                    build2.Insert(0, string.Format("<input id=\"{1}\" type=\"submit\" class=\"action async\" value=\"{0}\">", button.Title, button.ID, button.AskConfirmation ? " type_confirm" : null));
                else
                    build2.Insert(0, string.Format("<input id=\"{1}\" type=\"submit\" class=\"action postBack\" value=\"{0}\">", button.Title, button.ID, button.AskConfirmation ? " type_confirm" : null));
            }

            if (build2.Length == 0) return null;

            build = new StringBuilder();

            //if (container.Item.HasValue)
            //{
                build.AppendFormat(@"
            <footer>
                <span class=""""> </span>
				<div>
                    {0}
                </div>
            </footer>
			<br class=""clear"" />", build2.ToString());
//            }
//            else
//            {
//                build.AppendFormat(@"
//            <div class=""navBar"">
//				{0}
//			    <br class=""clear"" />
//            </div>", build2.ToString());
//            }
            return build.ToString();
        }

        internal static string ConfirmationQuestion(Framework.ContentListItem.ButtonAttribute button, Sushi.Mediakiwi.Beta.GeneratedCms.Console container)
        {
            return ConfirmationQuestion(button.AskConfirmation, container, button.ConfirmationQuestion, button.ConfirmationTitle, button.ConfirmationRejectLabel, button.ConfirmationAcceptLabel);
        }

        internal static string ConfirmationQuestion(bool askConfirmation, Sushi.Mediakiwi.Beta.GeneratedCms.Console container, string question = null, string questionTitle = null, string rejectLabel = null, string acceptLabel = null)
        {
            if (askConfirmation)
            {
                question = string.IsNullOrEmpty(question)
                    ? Labels.ResourceManager.GetString("delete_confirm", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                    : question
                    ;
                var title = string.IsNullOrEmpty(questionTitle) ? Labels.ResourceManager.GetString("delete_confirm_title", new CultureInfo(container.CurrentApplicationUser.LanguageCulture)) : questionTitle;

                var option_y = string.Concat(
                    string.IsNullOrEmpty(acceptLabel) ? Labels.ResourceManager.GetString("yes", new CultureInfo(container.CurrentApplicationUser.LanguageCulture)) : acceptLabel
                    );

                var option_n = string.Concat(
                    string.IsNullOrEmpty(rejectLabel) ? Labels.ResourceManager.GetString("no", new CultureInfo(container.CurrentApplicationUser.LanguageCulture)) : rejectLabel
                    );

                return string.Format(" data-confirm=\"{0}\" data-confirm-title=\"{1}\" data-confirm-y=\"{2}\" data-confirm-n=\"{3}\"", question, title, option_y, option_n);
            }
            return null;
        }

        internal static string TargetInfo(Framework.ContentListItem.ButtonAttribute button, Sushi.Mediakiwi.Beta.GeneratedCms.Console container)
        {
            if (string.IsNullOrEmpty(button.Target))
                return null;
            return string.Format(" target=\"{0}\"", button.Target);
        }

        internal static string LayerList(Framework.ContentListItem.ButtonAttribute button, Sushi.Mediakiwi.Beta.GeneratedCms.Console container)
        {
            if (!string.IsNullOrEmpty(button.CustomUrlProperty))
            {
                return container.CurrentListInstance.GetType().GetProperty(button.CustomUrlProperty).GetValue(container.CurrentListInstance, null) as string;
            }
            if (!string.IsNullOrEmpty(button.CustomUrl))
            {
                return button.CustomUrl;
            }

            if (!string.IsNullOrEmpty(button.ListInPopupLayer))
            {
                var list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(new Guid(button.ListInPopupLayer));
                if (!list.IsNewInstance)
                {
                    string prefix = container.UrlBuild.GetListRequest(list, container.Item);
                    return string.Concat(prefix, "&openinframe=1");
                }
            }
            return "#";
        }

        StringBuilder _Build_TopLeft;
        StringBuilder Build_TopLeft
        {
            get { if (_Build_TopLeft == null) _Build_TopLeft = new StringBuilder(); return _Build_TopLeft; }
            set { _Build_TopLeft = value; }
        }

        StringBuilder _Build_TopRight;
        StringBuilder Build_TopRight
        {
            get { if (_Build_TopRight == null) _Build_TopRight = new StringBuilder(); return _Build_TopRight; }
            set { _Build_TopRight = value; }
        }

        StringBuilder _Build_BottomLeft;
        StringBuilder Build_BottomLeft
        {
            get { if (_Build_BottomLeft == null) _Build_BottomLeft = new StringBuilder(); return _Build_BottomLeft; }
            set { _Build_BottomLeft = value; }
        }

        StringBuilder _Build_BottomRight;
        StringBuilder Build_BottomRight
        {
            get { if (_Build_BottomRight == null) _Build_BottomRight = new StringBuilder(); return _Build_BottomRight; }
            set { _Build_BottomRight = value; }
        }

        public string GetDataLayer(LayerSize layersize, Framework.ContentListItem.ButtonAttribute button, out string layerclass)
        {
             layerclass = null;
             if (!button.OpenInPopupLayer)
                 return null;

            Sushi.Mediakiwi.Framework.Grid.LayerSpecification specification = new Grid.LayerSpecification(layersize);

            if (button.PopupLayerHasScrollBar.HasValue)
                specification.HasScrolling = button.PopupLayerHasScrollBar.Value;

            if (!string.IsNullOrEmpty(button.PopupLayerHeight))
            {
                specification.IsHeightPercentage = button.PopupLayerHeight.Contains("%");
                specification.Height = Convert.ToInt32(button.PopupLayerHeight.Replace("%", string.Empty).Replace("px", string.Empty));
            }
            if (!string.IsNullOrEmpty(button.PopupLayerWidth))
            {
                specification.IsWidthPercentage = button.PopupLayerWidth.Contains("%");
                specification.Width = Convert.ToInt32(button.PopupLayerWidth.Replace("%", string.Empty).Replace("px", string.Empty));
            }
           
            string result = specification.Parse(true);
            if (result != null)
                layerclass = " openlayer";

            return result;
        }

        public string RightSideNavigation(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, Framework.ContentListItem.ButtonAttribute[] buttonList)
        {
            WimControlBuilder m_builder = new WimControlBuilder();
            return RightSideNavigation(container, buttonList, m_builder);
        }

        public string RightSideNavigation(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, Framework.ContentListItem.ButtonAttribute[] buttonList, WimControlBuilder builder)
        {
            StringBuilder build = new StringBuilder();
            StringBuilder build2 = new StringBuilder();
            StringBuilder innerbuild = new StringBuilder();

            List<string> menuOptions = new List<string>();

            string newRecord = container.CurrentList.Data["wim_LblNew"].Value;
            string saveRecord = container.CurrentList.Data["wim_LblSave"].Value;
            
            if (string.IsNullOrEmpty(newRecord)) 
                newRecord = Labels.ResourceManager.GetString("new_record", new CultureInfo(container.CurrentApplicationUser.LanguageCulture));
            
            if (string.IsNullOrEmpty(saveRecord))
                saveRecord = Labels.ResourceManager.GetString("save", new CultureInfo(container.CurrentApplicationUser.LanguageCulture));

            Data.FolderType section = container.CurrentListInstance.wim.CurrentFolder.Type;

            //  When in the page section, but on a property list page the actual section = list
            if (section == Data.FolderType.Page && container.CurrentList.Type != Sushi.Mediakiwi.Data.ComponentListType.Browsing)
                section = Data.FolderType.List;

            bool isEditMode = container.CurrentListInstance.wim.IsEditMode;
            bool isTextMode = !isEditMode;
            bool isListMode = container.View == 2;
            bool isBrowseMode = container.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.Browsing && !container.Item.HasValue;

            string className = null;
            //  Lists

            #region Buttons
            if (buttonList != null)
            {
                var tr = (from item in buttonList where item.IconTarget == ButtonTarget.TopRight select item);
                foreach (Framework.ContentListItem.ButtonAttribute button in tr)
                {
                    string layerclass;
                    Build_TopRight.Append(string.Format("<li><a href=\"{6}\" id=\"{1}{13}\" title=\"{11}\"{12} data-title=\"{2}\" class=\"abbr {7}{5}{3}{9}{10}\"{4}{8}>{0}</a></li>"
                        , button.Title // 0
                        , button.ID // 1
                        , button.PopupTitle // 2
                        , button.AskConfirmation ? " type_confirm" : null // 3
                        //, button.OpenInPopupLayer ? string.Concat(" ", button.PopupLayerSize.ToString()) : null //4
                        , TargetInfo(button, container) // 4
                        , button.OpenUrl ? null : button.AskConfirmation ? null : !string.IsNullOrWhiteSpace(button.CustomUrl) ? null : " postBack" // 5
                        , LayerList(button, container) // 6
                        , string.IsNullOrEmpty(button.ButtonClassName) ? "submit" : button.ButtonClassName // 7
                        , GetDataLayer(button.PopupLayerSize, button, out layerclass) //8
                        , container.CurrentListInstance.wim.Page.Body.Form.IsPrimairyAction(button) ? " action" : null //9
                        , layerclass // 10
                        , button.InteractiveHelp //11
                        , ConfirmationQuestion(button, container) //12
                        , button.TriggerSaveEvent ? "$save" : null //13
                        ));
                }
                var tl = (from item in buttonList where item.IconTarget == ButtonTarget.TopLeft select item);
                foreach (Framework.ContentListItem.ButtonAttribute button in tl)
                {
                    string layerclass;
                    Build_TopLeft.Append(string.Format("<li><a href=\"{6}\" id=\"{1}{13}\" title=\"{11}\"{12} data-title=\"{2}\" class=\"abbr {7}{5}{3}{8}{10}\"{4}{9}>{0}</a></li>"
                        , button.Title // 0
                        , button.ID // 1
                        , button.PopupTitle // 2
                        , button.AskConfirmation ? " type_confirm" : null // 3
                        //, button.OpenInPopupLayer ? string.Concat(" ", button.PopupLayerSize.ToString()) : null //4
                        , TargetInfo(button, container) // 4
                        , button.OpenUrl ? null : button.AskConfirmation ? null : !string.IsNullOrWhiteSpace(button.CustomUrl) ? null : " postBack" // 5
                        , LayerList(button, container) // 6
                        , string.IsNullOrEmpty(button.ButtonClassName) ? "submit" : button.ButtonClassName // 7
                        , container.CurrentListInstance.wim.Page.Body.Form.IsPrimairyAction(button) ? " action" : null //8
                        , GetDataLayer(button.PopupLayerSize, button, out layerclass) // 9
                        , layerclass //10
                        , button.InteractiveHelp //11
                        , ConfirmationQuestion(button, container) //12
                        , button.TriggerSaveEvent ? "$save" : null //13
                        ));
                }
            }
            #endregion Buttons

            #region List && Administration
            if (section == Sushi.Mediakiwi.Data.FolderType.List || section == Sushi.Mediakiwi.Data.FolderType.Administration)
            {
                if (!container.CurrentListInstance.wim.HasListSave)
                {
                    isEditMode = false;
                    isTextMode = true;
                }

                if (isBrowseMode)
                {
                    if (container.CurrentListInstance.wim.CurrentApplicationUserRole.CanChangeList
                        && !container.CurrentListInstance.wim.IsSubSelectMode
                        )
                    {
                        Build_TopLeft.AppendFormat(@"<li><abbr title=""{1}""><a href=""{0}"" class=""flaticon icon-file-plus action""></a></abbr></li>"
                            , container.UrlBuild.GetNewListRequest()
                            , Labels.ResourceManager.GetString("list_new", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                            );
                    }

                    className = " left";

                    if (container.CurrentListInstance.wim.CurrentApplicationUserRole.CanCreateList && !container.CurrentListInstance.wim.IsSubSelectMode)
                    {

                        Build_TopRight.AppendFormat(@"<li><abbr title=""{1}""><a href=""{0}&openinframe=1"" class=""flaticon icon-folder-plus Small""></a></abbr></li>"
                            , container.UrlBuild.GetFolderCreateRequest()
                            , Labels.ResourceManager.GetString("folder_new", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                            );

                        if (container.CurrentListInstance.wim.CurrentApplicationUser.IsDeveloper)
                        {
                           
                            if (container.CurrentListInstance.wim.CurrentApplicationUser.ShowHidden)
                            {
                                Build_TopRight.AppendFormat(@"<li><abbr title=""{0}""><a href=""#"" id=""dev_showvisible"" class=""flaticon icon-eye2 postBack""></a></abbr></li>"
                                    , "Show only visible"
                                    );
                            }
                            else
                            {
                                Build_TopRight.AppendFormat(@"<li><abbr title=""{0}""><a href=""#"" id=""dev_showhidden"" class=""flaticon icon-eye-slash postBack""></a></abbr></li>"
                                 , "Show hidden"
                                 );
                        
                            }
                        }

                        Build_TopRight.AppendFormat(@"<li><abbr title=""{1}""><a href=""{0}&openinframe=1"" class=""flaticon icon-settings-02 Small""></a></abbr></li>"
                            , container.UrlBuild.GetFolderOptionsRequest()
                            , Labels.ResourceManager.GetString("folder_properties", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                            );

                    }

                }
                else if (isListMode)
                {
                    if (container.CurrentListInstance.wim.CanAddNewItem && container.CurrentListInstance.wim.HasListLoad)
                    {
                        bool hasPrimary = _HasFilterButton;
                        if (!hasPrimary && buttonList != null)
                        {
                            foreach (var button in buttonList)
                            {
                                if (button.IsPrimary || container.CurrentListInstance.wim.Page.Body.Form.IsPrimairyAction(button))
                                {
                                    hasPrimary = true;
                                    break;
                                }
                            }
                        }

                        if (!container.CurrentListInstance.wim.HideCreateNew)
                            Build_TopLeft.AppendFormat(string.Format("<li><a href=\"{0}\" class=\"{4}submit{2}\"{3} title=\"{1}\">{1}</a></li>"
                                , container.UrlBuild.GetListNewRecordRequest()
                                , newRecord
                                , container.CurrentListInstance.wim.Page.Body.Grid.ClickLayerClass
                                , container.CurrentListInstance.wim.Page.Body.Grid.ClickLayerTag
                                , hasPrimary ? null : "action "
                                ));
                    }


                }
                else if (isTextMode)
                {

                    if (container.CurrentListInstance.wim.CurrentApplicationUserRole.CanPublishPage && container.IsComponent)
                    {
                        var c = Sushi.Mediakiwi.Data.ComponentVersion.SelectOne(container.Item.Value);
                        var v = Sushi.Mediakiwi.Data.Component.SelectOne(c.GUID);

                        //page.Updated
                        if (!(v == null || v.ID == 0))
                            Build_TopRight.AppendFormat("<li><a href=\"{0}\" id=\"pageoffline\" class=\"takeOffline postBack\">{1}</a></li>", "#"
                            , Labels.ResourceManager.GetString("page_takeoffline", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                            );

                        if (c.Updated.Ticks != v.Updated.GetValueOrDefault().Ticks)
                            Build_TopRight.AppendFormat("<li><a href=\"{0}\" id=\"pagepublish\" class=\"publish postBack\">{1}</a></li>", "#"
                            , Labels.ResourceManager.GetString("page_publish", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                            );
                    }


                    //  Web content publication
                    if (container.CurrentListInstance.wim.HasPublishOption)
                    {
                        Build_TopRight.AppendFormat("<li><a id=\"PageContentPublication\" href=\"{0}\" class=\"publish _PostBack\">{1}</a></li>", "#"
                            , Labels.ResourceManager.GetString("page_publish", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                            );
                    }

                    if (
                        container.CurrentListInstance.wim.HasListSave
                        && container.CurrentListInstance.wim.CurrentList.Option_CanSave
                        && !container.CurrentListInstance.wim.IsSubSelectMode
                        && !container.CurrentListInstance.wim.HideCreateNew
                        && !container.CurrentListInstance.wim.HideEditOption
                        )
                    {
                        //builder.ApiResponse.Buttons.Add(new MediakiwiField()
                        //{
                        //    PropertyName = "edit",
                        //    Title = Labels.ResourceManager.GetString("edit", new CultureInfo(container.CurrentApplicationUser.LanguageCulture)),
                        //    PropertyType = "bool",
                        //    ClassName = "action",
                        //    VueType = MediakiwiFormVueType.wimButton,
                        //    Event = MediakiwiJSEvent.click,
                        //    Section = ButtonSection.Bottom
                        //});

                        //Build_TopRight.Append(string.Format("<li><a id=\"edit\" href=\"#\" class=\"postBack submit\">{0}</a></li>"
                        //    , Labels.ResourceManager.GetString("edit", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                        //    ));
                    }
                    else
                        className = " left";

                }
                else if (isEditMode)
                {
                    if (container.CurrentListInstance.wim.CanAddNewItem
                        && container.CurrentListInstance.wim.HasListLoad
                        && !container.CurrentListInstance.wim.CanContainSingleInstancePerDefinedList
                        && !container.CurrentListInstance.wim.IsSubSelectMode
                        && !container.CurrentListInstance.wim.HideCreateNew
                        && container.GroupItem.HasValue
                        )
                        
                        Build_TopLeft.Append(string.Format("<li><a id=\"new\" class=\"submit\" href=\"{0}\">{1}</a></li>", container.UrlBuild.GetListNewRecordRequest(), newRecord
                       , Labels.ResourceManager.GetString("edit", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))

                       ));

                    if (container.CurrentList.Type != Sushi.Mediakiwi.Data.ComponentListType.ComponentListProperties || container.CurrentListInstance.wim.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.Undefined)
                    {
                        if (container.CurrentListInstance.wim.HasListDelete && !container.CurrentListInstance.wim.HideDelete && container.Item.GetValueOrDefault() > 0)
                        {
                            if (container.CurrentListInstance.wim.CurrentList.Data["wim_CanDelete"].ParseBoolean(true))
                                Build_TopRight.Append(string.Format("<li><a href=\"#\" id=\"delete\" class=\"abbr type_confirm left flaticon icon-trash-o\"{1} title=\"{0}\"></a></li>"
                                    , Labels.ResourceManager.GetString("delete", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                                    , ConfirmationQuestion(true, container)
                                    ));

                            builder.ApiResponse.Buttons.Add(new MediakiwiField()
                            {
                                PropertyName = "delete",
                                Title = string.Empty,//Labels.ResourceManager.GetString("delete", new CultureInfo(container.CurrentApplicationUser.LanguageCulture)),
                                PropertyType = "bool",
                                VueType = MediakiwiFormVueType.wimButton,
                                Event = MediakiwiJSEvent.click,
                                ClassName = "abbr type_confirm left flaticon icon-trash-o",
                                Section = ButtonSection.Top
                            }); ;
                        }
                    }

                }
            }
            #endregion

            #region Gallery
            else if (section == Sushi.Mediakiwi.Data.FolderType.Gallery)
            {
                if (isBrowseMode)
                {
                    if (container.View == 1)
                    {
                        Build_TopLeft.AppendFormat(@"<li><abbr title=""{1}""><a href=""{0}"" class=""flaticon icon-file-up action Small""></a></abbr></li>"
                            , container.UrlBuild.GetGalleryNewAssetRequestInLayer()
                            , Labels.ResourceManager.GetString("new_asset", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                            );
                    }
                    else
                    {
                        Build_TopLeft.AppendFormat(@"<li><abbr title=""{1}""><a href=""{0}"" class=""flaticon icon-file-up action Small""></a></abbr></li>"
                            , container.UrlBuild.GetGalleryNewAssetRequestInLayer()
                            , Labels.ResourceManager.GetString("new_asset", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                            );
                    }

                    if (container.View != 1 && container.CurrentListInstance.wim.CurrentApplicationUserRole.CanCreateList)
                    {
                        Build_TopRight.AppendFormat(@"<li><abbr title=""{1}""><a href=""{0}&openinframe=1"" class=""flaticon icon-folder-plus Small""></a></abbr></li>"
                            , container.UrlBuild.GetNewGalleryRequest()
                            , Labels.ResourceManager.GetString("folder_new", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                            );

                        //Build_TopRight.AppendFormat(@"<li><abbr title=""{1}""><a href=""{0}&openinframe=1"" class=""flaticon icon-settings-02 Small""></a></abbr></li>"
                        //    , container.UrlBuild.GetGalleryOptionsRequest()
                        //    , Labels.ResourceManager.GetString("folder_options", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                        //    );
                    }
                }
                else if (isTextMode)
                {
                    if (container.CurrentListInstance.wim.HasListSave && !container.CurrentListInstance.wim.HideEditOption)
                    {
                        build2.AppendFormat("<li><a href=\"#\" id=\"edit\" class=\"postBack submit\">{0}</a></li>",
                            Labels.ResourceManager.GetString("edit", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                            );
                    }


                    Sushi.Mediakiwi.Data.Asset asset = Sushi.Mediakiwi.Data.Asset.SelectOne(container.Item.GetValueOrDefault());
                    if (asset.Exists)
                    {
                        build2.AppendFormat("<li><a href=\"{1}\" class=\"attachment\">{0}</a></li>"
                            , Labels.ResourceManager.GetString("download", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                            , asset.DownloadFullUrl
                            );
                    }

                    if (buttonList != null)
                    {
                        buttonList.Where(x => (x.IconTarget == Sushi.Mediakiwi.Framework.ButtonTarget.TopLeft || x.IconTarget == ButtonTarget.TopRight))
                            .ToList()
                            .ForEach(button =>
                            {
                                build2.AppendFormat("<li><a href=\"#\" id=\"{1}\" title=\"{2}\" class=\"custom postBack{3}\">{0}</a></li>", button.Title, button.ID, button.InteractiveHelp, button.AskConfirmation ? " type_confirm" : null);
                            });
                    }

                    if (container.CurrentListInstance.wim.HasListDelete && container.Item.GetValueOrDefault() > 0)
                    {
                        build2.AppendFormat("<li class=\"right\"><a href=\"#\" id=\"delete\"{1} class=\"delete postBack type_confirm flaticon icon-trash-o\" title=\"{0}\"></a></li>"
                            , Labels.ResourceManager.GetString("delete", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                            , ConfirmationQuestion(true, container)
                            );
                    }
                }
                else if (isEditMode)
                {
                    if (container.CurrentListInstance.wim.HasListDelete && container.Item.GetValueOrDefault() > 0)
                    {
                        Build_TopRight.Append(string.Format("<li><a href=\"#\" id=\"delete\" class=\"abbr type_confirm left flaticon icon-trash-o\"{1} title=\"{0}\"></a></li>"
                            , Labels.ResourceManager.GetString("delete", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                            , ConfirmationQuestion(true, container)
                            ));
                    }
                }
            }
            #endregion

            #region Page

            else if (section == Sushi.Mediakiwi.Data.FolderType.Page)
            {
                if (isBrowseMode)
                {
                    if (container.CurrentListInstance.wim.CurrentApplicationUserRole.CanCreatePage)
                        Build_TopLeft.AppendFormat(string.Format("<li><a href=\"{0}\" class=\"submit\">{1}</a></li>", container.UrlBuild.GetNewPageRequest()
                            , Labels.ResourceManager.GetString("page_new", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                            )
                        );

                    if (container.CurrentListInstance.wim.CurrentApplicationUserRole.CanCreateList)
                    {
                        Build_TopRight.AppendFormat(string.Format("<li><a href=\"{0}\" class=\"submit\">{1}</a></li>", container.UrlBuild.GetFolderCreateRequest()
                            , Labels.ResourceManager.GetString("folder_new", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                            )
                        );

                        Build_TopRight.AppendFormat(@"<li><a href=""{0}&openinframe=1"" class=""flaticon icon-settings-02 Small""></a></li>"
                            , container.UrlBuild.GetFolderOptionsRequest()
                            );

                    }
                }
                else if (isEditMode)
                {
                    Page page = Page.SelectOne(container.Item.Value, false);
                    
                    var pagePreviewHandler = new PagePreview();

                    // [MR:26-03-2019]
                    var pmodules = Data.Environment.Current.GetPageModules();
                    if (pmodules?.Count > 0)
                    {
                        foreach (var pmodule in pmodules)
                        {
                            if (pmodule.ShowOnPage(page, container.CurrentApplicationUser))
                            {
                                string confirmOption = "";
                                string iconClass = (string.IsNullOrWhiteSpace(pmodule.IconURL)) ? pmodule.IconClass : "";
                                string iconUrl = pmodule.IconURL;

                                if (pmodule.ConfirmationNeeded)
                                {
                                    iconClass += " type_confirm";
                                    confirmOption = ConfirmationQuestion(true, container, pmodule.ConfirmationQuestion, pmodule.ConfirmationTitle);
                                }
                                else
                                {
                                    iconClass += " postBack";
                                }

                                if (string.IsNullOrWhiteSpace(iconUrl) == false)
                                    iconUrl = $"<img src=\"{iconUrl}\" width=\"16px\" height=\"16px\" />";
                                Build_TopRight.AppendFormat("<li><a href=\"#\" id=\"pagemod_{3}\" class=\"abbr flaticon {1}\" title=\"{0}\"{2}>{4}</a></li>"
                                    , pmodule.Tooltip
                                    , iconClass
                                    , confirmOption
                                    , pmodule.GetType().Name
                                    , iconUrl
                                );
                            }
                        }
                    }
                    if (page.IsPublished)
                    {
                        Build_TopRight.AppendFormat("<li><a href=\"{0}\" target=\"preview\" class=\"abbr submit\">{1}</a></li>"
                                , pagePreviewHandler.GetOnlineUrl(page)
                                , Labels.ResourceManager.GetString("page_live", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                        );
                    }

                    Build_TopRight.AppendFormat("<li><a href=\"{0}\" target=\"preview\" class=\"abbr submit\">{1}</a></li>"
                            , pagePreviewHandler.GetPreviewUrl(page) 
                            , Labels.ResourceManager.GetString("page_preview", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                    );

                    Build_TopRight.AppendFormat("<li><a href=\"{0}&openinframe=1\" id=\"dev_refreshcode\" class=\"abbr flaticon icon-copy Small\" title=\"{1}\"></a></li>"
                        , container.UrlBuild.GetPageCopyRequest(container.CurrentPage.ID)
                        , "Copy page"
                    );

                    Build_TopRight.AppendFormat("<li><a href=\"{0}&openinframe=1\" id=\"dev_refreshcode\" class=\"abbr flaticon  icon-clipboard Small\" title=\"{1}\"></a></li>"
                     , container.UrlBuild.GetPageCopyContentRequest(container.CurrentPage.ID) 
                     , Labels.ResourceManager.GetString("copy_page_content", new CultureInfo(container.CurrentApplicationUser.LanguageCulture)) 
                     );
                    Build_TopRight.AppendFormat("<li><a href=\"{0}&openinframe=1\" id=\"dev_refreshcode\" class=\"abbr flaticon  icon-history Small\" title=\"{1}\"></a></li>"
                     , container.UrlBuild.GetPageHistoryRequest(container.CurrentPage.ID)
                     , Labels.ResourceManager.GetString("page_history", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                     );

                    if (!(container.CurrentPage == null || container.CurrentPage.ID == 0))
                    {
                        string path = container.MapPath(container.AddApplicationPath(page.Template.Location));
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.FileInfo nfo = new System.IO.FileInfo(path);
                            DateTime file = Utility.ConvertToSystemDataTime(nfo.LastWriteTimeUtc);

                            // [MR:26-04-2019] when a file is smaller than 120 bytes, it's probably a marker file and
                            // should not trigger an update.
                            if (nfo.Length > 120 && (file.Ticks > page.Template.LastWriteTimeUtc.Ticks))
                            {
                                Build_TopRight.AppendFormat("<li><a href=\"{0}&refreshcode=1\" id=\"dev_refreshcode\" class=\"abbr action flaticon icon-refresh\" title=\"{1}\"></a></li>"
                                    , container.UrlBuild.GetPageRequest(page)
                                    , "The page possible has new form elements, click to update!"
                                );
                            }

                        }
                    }

                    if (container.CurrentApplicationUser.IsDeveloper)
                    {
                        var list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(ComponentListType.PageTemplates);

                        Build_TopRight.AppendFormat("<li><a href=\"?list={0}&item={2}\" id=\"source\" class=\"abbr flaticon icon-code\" title=\"{1}\"></a></li>", list.ID
                            , "Visit the pagetemplate", container.CurrentPage.Template.ID
                            );
                    }

                    if (!page.IsPublished)
                    {
                        if (container.CurrentListInstance.wim.CurrentApplicationUserRole.CanDeletePage && !page.MasterID.HasValue)
                        {
                            Build_TopRight.AppendFormat("<li><a href=\"{0}\"{2} id=\"delete\" class=\"abbr type_confirm flaticon icon-trash-o\" title=\"{1}\"></a></li>"
                                , "#"
                                , Labels.ResourceManager.GetString("delete", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                                , ConfirmationQuestion(true, container)
                                );
                        }
                    }

                    if (page.MasterID.HasValue || page.Site.MasterID.HasValue)
                    {
                        if (page.InheritContentEdited)
                        {
                            //  Inherits content, so can localize
                            Build_TopRight.AppendFormat("<li><a href=\"{0}\" id=\"page.localize\" class=\"submit postBack abbr\">{1}</a></li>"
                                , "#"
                                , Labels.ResourceManager.GetString("page_localise", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                                );



                        }
                        else
                        {

                            Build_TopRight.AppendFormat("<li><a href=\"{0}\" id=\"page.copy\" class=\"submit type_confirm postBack abbr\" {2}>{1}</a></li>"
                                , "#"
                                , Labels.ResourceManager.GetString("page_pastemode", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                                , ConfirmationQuestion(true, container)
                                );

                            //  Inherits no content, so can unlocalize
                            Build_TopRight.AppendFormat("<li><a href=\"{0}\" id=\"page.inherit\" class=\"submit postBack abbr\">{1}</a></li>"
                                , "#"
                                , Labels.ResourceManager.GetString("page_inherit", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                                );

                        }
                    }

                    if (container.CurrentListInstance.wim.CurrentApplicationUserRole.CanPublishPage)
                    {
                        var pagePublicationHandler = new PagePublication();
                        pagePublicationHandler.ValidateConfirmation(container.CurrentApplicationUser, page);

                        if (pagePublicationHandler.CanTakeOffline(container.CurrentApplicationUser, page))
                        {
                            if (page.Published != DateTime.MinValue)
                                Build_TopRight.AppendFormat("<li><a href=\"{0}\" id=\"pageoffline\" class=\"abbr flaticon icon-power-off type_confirm\"{2} title=\"{1}\"></a></li>", "#"
                                    , Labels.ResourceManager.GetString("page_takeoffline", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                                    , ConfirmationQuestion(true, container)
                                    );
                        }

                        if (pagePublicationHandler.CanPublish(container.CurrentApplicationUser, page))
                        {
                            if (page.Published == DateTime.MinValue || page.Published.Ticks != page.Updated.Ticks)
                            {
                                Build_TopRight.AppendFormat("<li><a href=\"{0}\" id=\"pagepublish\" class=\"abbr submit {2}\"{3} title=\"{1}\">{1}</a></li>", "#"
                                    , Labels.ResourceManager.GetString("page_publish", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                                    , pagePublicationHandler.AskConfirmation ? "type_confirm" : "postBack"
                                    , ConfirmationQuestion(pagePublicationHandler.AskConfirmation, container
                                        , pagePublicationHandler.ConfirmationQuestion
                                        , pagePublicationHandler.ConfirmationTitle
                                        , pagePublicationHandler.ConfirmationRejectLabel
                                        , pagePublicationHandler.ConfirmationAcceptLabel
                                        )
                                    );
                            }
                        }
                    }
                }
            }
            #endregion


            if (!string.IsNullOrEmpty(container.CurrentListInstance.wim.m_sortOrderSqlTable))
            {
                bool ignoreSort = false;
                if (!isListMode && !container.CurrentListInstance.wim.HasSingleItemSortOrder)
                    ignoreSort = true;

                if (container.CurrentListInstance.wim.HasSortOrder && !ignoreSort)
                    Build_TopRight.Append(@"<li><a href=""#"" class=""flaticon icon-sort sortOrder""></a></li>");
            }

            //MV: Export button was always missing in ListItem view, even if there was ListData
            //if (container.CurrentListInstance.wim.HasExportOptionXLS && !container.Item.HasValue)
            if (container.CurrentListInstance.wim.HasExportOptionXLS &&  container.CurrentListInstance.wim.ListData != null)
                Build_TopRight.AppendFormat(@"<li><abbr title=""{0}""><a id=""export_xls"" href=""#"" class=""flaticon icon-export postBack nosync""></a></abrr></li>"
                    , Labels.ResourceManager.GetString("export_xls", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                    );


            if (_Build_TopRight != null)
            {
                Build_TopRight.Insert(0, @"<li class=""laster""><ul>");
                Build_TopRight.Append(@"</ul></li>");
            }
            else if (_Build_TopLeft == null)
            {
                return @"<section id=""iconBarz"" class=""component iconBar""></section>";
            }

            return string.Concat(@"
<section id=""iconBarz"" class=""component iconBar"">
					<div class=""footer"">
						<ul>
							" , (_Build_TopRight == null ? string.Empty : _Build_TopRight.ToString()) , @"
							" , (_Build_TopLeft == null ? string.Empty : _Build_TopLeft.ToString()) , @"
						</ul>
					</div>	
				</section>");
        }

        /// <summary>
        /// Breadcrumbs the navigation.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="buttonList">The button list.</param>
        /// <returns></returns>
        internal static string BreadcrumbNavigation(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, Framework.ContentListItem.ButtonAttribute[] buttonList)
        {
            bool isNewNavigation = true;

            List<Framework.ContentListItem.ButtonAttribute> top = new List<Sushi.Mediakiwi.Framework.ContentListItem.ButtonAttribute>();

            if (buttonList != null)
            {
                foreach (Framework.ContentListItem.ButtonAttribute item in buttonList)
                    if (item.IconTarget != Sushi.Mediakiwi.Framework.ButtonTarget.BottomRight) top.Add(item);
            }

            //if (isNewNavigation)
            //    return NewTopNavigation(container, top.ToArray());


            StringBuilder build = new StringBuilder();
            build.Append("\n<ul id=\"pathNavigation\" class=\"pathNavigation pseudoHover\">");

            if (container.IsCodeUpdate)
            {
                build.Append("\n</ul>");
                return build.ToString();
            }

            if (container.CurrentListInstance.wim.CurrentFolder.Type != Sushi.Mediakiwi.Data.FolderType.Gallery)
            {
                foreach (Data.Folder item in Data.Folder.SelectAllByBackwardTrail(container.CurrentListInstance.wim.CurrentFolder.ID))
                {
                    string url = string.Concat(container.WimPagePath, "?folder=", item.ID);
                    build.Append(string.Format("\n\t<li{2}><a href=\"{1}\">{0}</a>", item.Name, url, item.ID == container.CurrentListInstance.wim.CurrentFolder.ID ? " class=\"active\"" : string.Empty));

                    bool isFirst = false;
                    foreach (Data.Folder item2 in Data.Folder.SelectAllByParent(item.ID, container.CurrentListInstance.wim.CurrentFolder.Type, false))
                    {
                        if (!isFirst)
                            build.Append("\n\t<ul>");

                        isFirst = true;
                        string url2 = string.Concat(container.WimPagePath, "?folder=", item2.ID);
                        build.Append(string.Format("\n\t\t<li><a href=\"{1}\" class=\"folder\">{0}</a></li>", item2.Name, url2));

                    }
                    if (isFirst)
                        build.Append("\n\t</ul>");

                    build.Append("\n\t</li>");
                }
            }
            else
            {
                foreach (Data.Gallery item in Data.Gallery.SelectAllByBackwardTrail(container.CurrentListInstance.wim.CurrentFolder.ID))
                {
                    string url = string.Concat(container.WimPagePath, "?gallery=", item.ID);
                    build.Append(string.Format("\n\t<li{2}><a href=\"{1}\">{0}</a>", item.Name, url, item.ID == container.CurrentListInstance.wim.CurrentFolder.ID ? " class=\"active\"" : string.Empty));

                    bool isFirst = false;
                    foreach (Data.Gallery item2 in Data.Gallery.SelectAllByParent(item.ID))
                    {
                        if (!isFirst)
                            build.Append("\n\t<ul>");

                        isFirst = true;
                        string url2 = string.Concat(container.WimPagePath, "?gallery=", item2.ID);
                        build.Append(string.Format("\n\t\t<li><a href=\"{1}\" class=\"folder\">{0}</a></li>", item2.Name, url2));

                    }
                    if (isFirst)
                        build.Append("\n\t</ul>");

                    build.Append("\n\t</li>");
                }
            }
            build.Append("\n</ul>");
            return build.ToString();
        }

        /// <summary>
        /// Sets the page navigation.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="build">The build.</param>
        /// <param name="previousUrl">The previous URL.</param>
        /// <param name="isEditMode">if set to <c>true</c> [is edit mode].</param>
        /// <param name="isTextMode">if set to <c>true</c> [is text mode].</param>
        /// <param name="isListMode">if set to <c>true</c> [is list mode].</param>
        /// <param name="isBrowseMode">if set to <c>true</c> [is browse mode].</param>
        static void SetPageNavigation(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, StringBuilder build, ref string previousUrl, bool isEditMode, bool isTextMode, bool isListMode, bool isBrowseMode)
        {
            StringBuilder build2 = new StringBuilder();

            if (isBrowseMode)
            {
                previousUrl = string.Concat(container.WimPagePath, "?", "folder=", container.CurrentListInstance.wim.CurrentFolder.ParentID.GetValueOrDefault(0));
                
                Data.IComponentList list0 = Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.Folders);
                Data.IComponentList list1 = Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.PageProperties);

                if (container.CurrentListInstance.wim.CurrentApplicationUserRole.CanCreatePage)
                {
                    //  Create new page
                    build2.AppendFormat("<li><a href=\"{0}\" class=\"createPage\">New page</a></li>", string.Concat(container.WimPagePath, "?", "list=", list1.ID, "&folder=", container.CurrentListInstance.wim.CurrentFolder.ID, "&item=0"));
                    //  Create new folder
                    build2.AppendFormat("<li><a href=\"{0}\" class=\"createFolder\">Create folder</a></li>", string.Concat(container.WimPagePath, "?", "list=", list0.ID, "&folder=", container.CurrentListInstance.wim.CurrentFolder.ID, "&item=0"));
                }
                if (container.CurrentListInstance.wim.CurrentApplicationUserRole.CanChangePage)
                {
                    //  Show folder properties
                    build2.AppendFormat("<li><a href=\"{0}\" class=\"folderOptions\">Folder options</a></li>", string.Concat(container.WimPagePath, "?", "list=", list0.ID, "&folder=", container.CurrentListInstance.wim.CurrentFolder.ID, "&item=", container.CurrentListInstance.wim.CurrentFolder.ID));
                }



                build2.Append("</ul><ul class=\"subNavigation\" class=\"pseudoHover\">");
                if (container.CurrentApplicationUser.ShowDetailView)
                    build2.AppendFormat("<li><a href=\"{0}\" id=\"thumbview\" class=\"properties postBack\">Thumbnail view</a></li>", "#");
                else
                    build2.AppendFormat("<li><a href=\"{0}\" id=\"detailview\" class=\"properties postBack\">Detail view</a></li>", "#");

                if (build2.Length > 0)
                {
                    build.Append("<ul class=\"subNavigation\" class=\"pseudoHover\">");
                    build.Append(build2.ToString());
                    build.Append("</ul>");
                }


            }
            else if (isTextMode)
            {
                Sushi.Mediakiwi.Data.Page page = Sushi.Mediakiwi.Data.Page.SelectOne(container.Item.Value, false);

                Data.IComponentList list0 = Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.PageProperties);
                previousUrl = string.Concat(container.WimPagePath, "?", "folder=", container.CurrentListInstance.wim.CurrentFolder.ID);

                //  Edit
                if (container.CurrentListInstance.wim.CurrentApplicationUserRole.CanChangePage)
                    build2.AppendFormat("<li><a href=\"{0}\" id=\"edit\" class=\"edit postBack\">Edit</a></li>", "#");
                
                build2.AppendFormat("<li><a href=\"{0}?preview=1\" target=\"preview\" class=\"preview\">Page preview</a></li>", page.HRefFull);

                if (page.MasterID.HasValue && !isEditMode)
                {
                    if (page.InheritContent)
                    {
                        //  Inherits content, so can localize
                        build2.AppendFormat("<li><a href=\"{0}\" id=\"page.localize\" class=\"refresh postBack\">localise page</a></li>", "#");

                    }
                    else
                    {
                        //  Inherits no content, so can unlocalize
                        build2.AppendFormat("<li><a href=\"{0}\" id=\"page.inherit\" class=\"refresh postBack\">inherit content</a></li>", "#");

                    }
                    //  Show folder properties
                    if (container.CurrentApplicationUser.ShowTranslationView)
                        build2.AppendFormat("<li><a href=\"{0}\" id=\"page.normal\" class=\"properties postBack\">Normal mode</a></li>", "#");
                    else
                        build2.AppendFormat("<li><a href=\"{0}\" id=\"page.translate\" class=\"properties postBack\">Translation mode</a></li>", "#");

                    build2.Append("</ul><ul class=\"subNavigation\" class=\"pseudoHover\">");
                }

                if (container.CurrentListInstance.wim.CurrentApplicationUserRole.CanChangePage)
                    build2.AppendFormat("<li><a href=\"{0}\" class=\"properties\">Properties</a></li>", string.Concat(container.WimPagePath, "?", "list=", list0.ID, "&folder=", container.CurrentListInstance.wim.CurrentFolder.ID, "&item=", container.Item.Value));


                if (container.CurrentListInstance.wim.CurrentApplicationUserRole.CanPublishPage)
                {
                    build2.Append("</ul><ul class=\"subNavigation\" class=\"pseudoHover\">");

                    //page.Updated

                    if (page.Published != DateTime.MinValue)
                        build2.AppendFormat("<li><input/><a href=\"{0}\" id=\"pageoffline\" class=\"takeOffline postBack\">Take Offline</a></li>", "#");

                    if (page.Published == DateTime.MinValue || page.Published.Ticks != page.Updated.Ticks)
                        build2.AppendFormat("<li><a href=\"{0}\" id=\"pagepublish\" class=\"publish postBack\">Publish</a></li>", "#");
                }



                if (!page.IsPublished)
                {
                    if (container.CurrentListInstance.wim.CurrentApplicationUserRole.CanDeletePage && !page.MasterID.HasValue)
                    {
                        build2.Append("</ul><ul class=\"subNavigation\" class=\"pseudoHover\">");
                        build2.AppendFormat("<li><a href=\"{0}\" id=\"delete\" class=\"delete type_confirm\">Delete</a></li>", "#");
                    }
                }

                if (build2.Length > 0)
                {
                    build.Append("<ul class=\"subNavigation\" class=\"pseudoHover\">");
                    build.Append(build2.ToString());
                    build.Append("</ul>");
                }

                build.Append("</ul>");

//                build.AppendFormat(@"<dl class=""properties"">
//	<dt>
//		<span>Information</span>
//	</dt>
//	<dd>
//		<ul>
//			<li>Published: {1}</li>
//			<li>Modified: {2}</li>
//            <li>Created: {0}</li>
//		</ul>
//	</dd>
//</dl>
//"
//                    , page.Created.ToString("dd-MM-yyyy HH:mm")
//                    , page.Published == DateTime.MinValue ? "-" : page.Published.ToString("dd-MM-yyyy HH:mm")
//                    , page.Updated.ToString("dd-MM-yyyy HH:mm"));
            }
            else if (isEditMode)
            {
                previousUrl = string.Concat(container.WimPagePath, "?", "folder=", container.CurrentListInstance.wim.CurrentFolder.ID);
                //  Save
                build2.AppendFormat("<li><a href=\"{0}\" id=\"save\" class=\"edit postBack\">Save</a></li>", "#");

                if (build2.Length > 0)
                {
                    build.Append("<ul class=\"subNavigation\" class=\"pseudoHover\">");
                    build.Append(build2.ToString());
                    build.Append("</ul>");
                }
            }
            
        }

        /// <summary>
        /// Sets the list navigation.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="build">The build.</param>
        /// <param name="previousUrl">The previous URL.</param>
        /// <param name="isEditMode">if set to <c>true</c> [is edit mode].</param>
        /// <param name="isTextMode">if set to <c>true</c> [is text mode].</param>
        /// <param name="isListMode">if set to <c>true</c> [is list mode].</param>
        /// <param name="isBrowseMode">if set to <c>true</c> [is browse mode].</param>
        /// <param name="buttonList">The button list.</param>
        static void SetListNavigation(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, StringBuilder build, ref string previousUrl, bool isEditMode, bool isTextMode, bool isListMode, bool isBrowseMode, Framework.ContentListItem.ButtonAttribute[] buttonList)
        {
            StringBuilder build2 = new StringBuilder();

            if (isBrowseMode)
            {
                previousUrl = string.Concat(container.WimPagePath, "?", "folder=", container.CurrentListInstance.wim.CurrentFolder.ParentID.GetValueOrDefault(0));
                Data.IComponentList list0 = Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.Folders);
                Data.IComponentList list1 = Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.ComponentListInstance);

                if (container.CurrentListInstance.wim.CurrentApplicationUserRole.CanCreateList)
                {
                    //  Create new page
                    //build2.AppendFormat("<li><a href=\"{0}\" class=\"createPage\">New list</a></li>", string.Concat(container.WimPagePath, "?", "list=", list1.ID, "&folder=", container.CurrentListInstance.wim.CurrentFolder.Id, "&item=0"));
                    //  Create new folder
                    build2.AppendFormat("<li><a href=\"{0}\" class=\"createFolder\">Create folder</a></li>", string.Concat(container.WimPagePath, "?", "list=", list0.ID, "&folder=", container.CurrentListInstance.wim.CurrentFolder.ID, "&item=0"));
                }
                if (container.CurrentListInstance.wim.CurrentApplicationUserRole.CanChangeList)
                {
                    //  Show folder properties
                    build2.AppendFormat("<li><a href=\"{0}\" class=\"folderOptions\">Folder options</a></li>", string.Concat(container.WimPagePath, "?", "list=", list0.ID, "&folder=", container.CurrentListInstance.wim.CurrentFolder.ID, "&item=", container.CurrentListInstance.wim.CurrentFolder.ID));
                }

                build2.Append("</ul><ul class=\"subNavigation\" class=\"pseudoHover\">");
                if (container.CurrentApplicationUser.ShowDetailView)
                    build2.AppendFormat("<li><a href=\"{0}\" id=\"thumbview\" class=\"properties postBack\">Thumbnail view</a></li>", "#");
                else
                    build2.AppendFormat("<li><a href=\"{0}\" id=\"detailview\" class=\"properties postBack\">Detail view</a></li>", "#");
            }
            else if (isListMode)
            {
                //  Create new element
                Data.IComponentList list0 = Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.ComponentListProperties);

                previousUrl = string.Concat(container.WimPagePath, "?", "folder=", container.CurrentListInstance.wim.CurrentFolder.ID);

                if (container.CurrentListInstance.wim.CanAddNewItem && container.CurrentListInstance.wim.HasListLoad)
                {
                    build2.AppendFormat("<li><a href=\"{0}\" class=\"createPage\">New record</a></li>", string.Concat(container.CurrentListInstance.wim.SearchResultItemPassthroughParameter, "=0"));
                }

                if (container.CurrentListInstance.wim.CurrentApplicationUserRole.CanChangeList)
                {
                    if (container.CurrentList.Type != Sushi.Mediakiwi.Data.ComponentListType.ComponentListProperties)
                    {
                        if (container.CurrentList.Type != Sushi.Mediakiwi.Data.ComponentListType.Folders)
                            build2.AppendFormat("<li><a href=\"{0}\" class=\"properties\">Properties</a></li>", string.Concat(container.WimPagePath, "?", "list=", list0.ID, "&folder=", container.CurrentListInstance.wim.CurrentFolder.ID, "&item=", container.CurrentList.ID));
                    }
                }

                if (!string.IsNullOrEmpty(container.CurrentListInstance.wim.m_sortOrderSqlTable))
                {
                    if (container.CurrentListInstance.wim.HasSortOrder)
                    {
                        build2.Append("</ul><ul class=\"subNavigation\" class=\"pseudoHover\">");
                        if (container.IsSortorderOn)
                            build2.AppendFormat("<li><a href=\"{0}\" id=\"sortOrder\" class=\"sortOrder\">Turn sort order off</a></li>", container.GetSafeUrl());
                        else
                            build2.AppendFormat("<li><a href=\"{0}\" id=\"sortOrder\" class=\"sortOrder postBack\">Turn sort order on</a></li>", "#");
                    }
                }


                //  Custom buttons
                if (buttonList != null)
                {
                    build2.Append("</ul><div class=\"hr\"><hr /></div><ul class=\"subNavigation\" class=\"pseudoHover\">");
                    foreach (Framework.ContentListItem.ButtonAttribute button in buttonList)
                    {
                        build2.AppendFormat("<li><a href=\"#\" title=\"{2}\" id=\"{1}\" class=\"custom postBack{3}\">{0}</a></li>", button.Title, button.ID, button.InteractiveHelp, button.AskConfirmation ? " type_confirm" : null);
                    }
                }

            }
            else if (isTextMode)
            {

                Data.IComponentList list0 = Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.ComponentListProperties);

                if (container.CurrentListInstance.wim.CanContainSingleInstancePerDefinedList || container.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.Folders)
                    previousUrl = string.Concat(container.WimPagePath, "?", "folder=", container.CurrentListInstance.wim.CurrentFolder.ID);
                else
                {
                    previousUrl = string.Concat(container.WimPagePath, "?", "list=", container.CurrentList.ID);
                }

                //  Only create a New Record option when the state is tabular mode
                if (container.Group.HasValue && container.Group.GetValueOrDefault() != container.Item.GetValueOrDefault(0) && container.Item.GetValueOrDefault(0) > 0)
                {
                    if (container.CurrentListInstance.wim.CanAddNewItem && container.CurrentListInstance.wim.HasListLoad)
                    {
                        build2.AppendFormat("<li><a href=\"{0}\" class=\"createPage\">New record</a></li>", string.Concat(container.CurrentListInstance.wim.SearchResultItemPassthroughParameter, "=0"));
                    }
                }

                //  Edit
                if (container.CurrentListInstance.wim.HasListSave)
                    build2.AppendFormat("<li><a href=\"{0}\" id=\"edit\" class=\"edit postBack\">Edit</a></li>", "#");


                Data.IComponentList selectedList = container.CurrentList;
                if (container.Item.GetValueOrDefault(container.Logic) != container.CurrentList.ID)
                {
                    selectedList = Data.ComponentList.SelectOne(container.Item.GetValueOrDefault(container.Logic));
                }
                
                if (container.CurrentList.Type != Sushi.Mediakiwi.Data.ComponentListType.ComponentListProperties || selectedList.Type == Sushi.Mediakiwi.Data.ComponentListType.Undefined)
                {
                    if (container.CurrentList.Type != Sushi.Mediakiwi.Data.ComponentListType.ComponentListProperties 
                        && container.CurrentListInstance.wim.CurrentApplicationUserRole.CanChangeList)
                    {
                        if (container.CurrentList.Type != Sushi.Mediakiwi.Data.ComponentListType.Folders)// && container.CurrentListInstance.wim.CanContainSingleInstancePerDefinedList)
                            build2.AppendFormat("<li><a href=\"{0}\" class=\"properties\">Properties</a></li>", string.Concat(container.WimPagePath, "?", "list=", list0.ID, "&folder=", container.CurrentListInstance.wim.CurrentFolder.ID, "&item=", container.CurrentList.ID));
                    }

                    if (container.CurrentListInstance.wim.HasListDelete)
                    {
                        build2.Append("</ul><ul class=\"subNavigation\" class=\"pseudoHover\">");
                        build2.AppendFormat("<li><a href=\"{0}\" id=\"delete\" class=\"delete postBack type_confirm\">Delete</a></li>", "#");
                    }
                }

                if (container.CurrentListInstance.wim.HasSingleItemSortOrder && !string.IsNullOrEmpty(container.CurrentListInstance.wim.m_sortOrderSqlTable))
                {
                    if (container.CurrentListInstance.wim.HasSortOrder)
                    {
                        build2.Append("</ul><ul class=\"subNavigation\" class=\"pseudoHover\">");
                        if (container.IsSortorderOn)
                            build2.AppendFormat("<li><a href=\"{0}\" id=\"sortOrder\" class=\"sortOrder\">Turn sort order off</a></li>", container.GetSafeUrl());
                        else
                            build2.AppendFormat("<li><a href=\"{0}\" id=\"sortOrder\" class=\"sortOrder postBack\">Turn sort order on</a></li>", "#");
                    }
                }

                //  Custom buttons
                if (buttonList != null)
                {
                    build2.Append("</ul><div class=\"hr\"><hr /></div><ul class=\"subNavigation\" class=\"pseudoHover\">");
                    foreach (Framework.ContentListItem.ButtonAttribute button in buttonList)
                    {
                        build2.AppendFormat("<li><a href=\"#\" id=\"{1}\" title=\"{2}\" class=\"custom postBack{3}\">{0}</a></li>", button.Title, button.ID, button.InteractiveHelp, button.AskConfirmation ? " type_confirm" : null);
                    }
                }

            }
            else if (isEditMode)
            {

                if (container.CurrentListInstance.wim.CanContainSingleInstancePerDefinedList || container.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.Folders)
                    previousUrl = string.Concat(container.WimPagePath, "?", "folder=", container.CurrentListInstance.wim.CurrentFolder.ID);
                else
                    previousUrl = string.Concat(container.WimPagePath, "?", "list=", container.CurrentList.ID);

                //  Only create a New Record option when the state is tabular mode
                if (container.Group.HasValue && container.Group.GetValueOrDefault() != container.Item.GetValueOrDefault(0) && container.Item.GetValueOrDefault(0) > 0)
                {
                    if (container.CurrentListInstance.wim.CanAddNewItem && container.CurrentListInstance.wim.HasListLoad)
                    {
                        build2.AppendFormat("<li><a href=\"{0}\" class=\"createPage\">New record</a></li>", string.Concat(container.CurrentListInstance.wim.SearchResultItemPassthroughParameter, "=0"));
                    }
                }

                //  Save
                if (!container.CurrentListInstance.wim.CanContainSingleInstancePerDefinedList && container.CurrentListInstance.wim.CanSaveAndAddNew)
                {
                    build2.AppendFormat("<li><a href=\"{0}\" id=\"saveNew\" class=\"edit postBack\">Save and new record</a></li>", "#");
                    build2.Append("</ul><ul class=\"subNavigation\" class=\"pseudoHover\">");
                }

                if (container.CurrentListInstance.wim.CanAddNewItem || container.Item.GetValueOrDefault() > 0)
                {
                    build2.AppendFormat("<li><a href=\"{0}\" id=\"save\" class=\"edit postBack\">Save</a></li>", "#");

                    if (container.Item.GetValueOrDefault(0) != 0 && container.CurrentListInstance.wim.OpenInEditMode)
                    {
                        Data.IComponentList selectedList = container.CurrentList;
                        if (container.Item.GetValueOrDefault(container.Logic) != container.CurrentList.ID)
                        {
                            selectedList = Data.ComponentList.SelectOne(container.Item.GetValueOrDefault(container.Logic));
                        }

                        if (container.CurrentList.Type != Sushi.Mediakiwi.Data.ComponentListType.ComponentListProperties || selectedList.Type == Sushi.Mediakiwi.Data.ComponentListType.Undefined)
                        {
                            if (container.CurrentListInstance.wim.HasListDelete)
                            {
                                build2.Append("</ul><ul class=\"subNavigation\" class=\"pseudoHover\">");
                                build2.AppendFormat("<li><a href=\"{0}\" id=\"delete\" class=\"delete postBack type_confirm\">Delete</a></li>", "#");
                            }
                        }
                    }
                }

                if (container.CurrentListInstance.wim.HasSingleItemSortOrder && !string.IsNullOrEmpty(container.CurrentListInstance.wim.m_sortOrderSqlTable))
                {
                    if (container.CurrentListInstance.wim.HasSortOrder)
                    {

                        build2.Append("</ul><ul class=\"subNavigation\" class=\"pseudoHover\">");
                        if (container.IsSortorderOn)
                            build2.AppendFormat("<li><a href=\"{0}\" id=\"sortOrder\" class=\"sortOrder\">Turn sort order off</a></li>", container.GetSafeUrl());
                        else
                            build2.AppendFormat("<li><a href=\"{0}\" id=\"sortOrder\" class=\"sortOrder postBack\">Turn sort order on</a></li>", "#");
                    }
                }

                //  Custom buttons
                if (buttonList != null)
                {
                    build2.Append("</ul><div class=\"hr\"><hr /></div><ul class=\"subNavigation\" class=\"pseudoHover\">");
                    foreach (Framework.ContentListItem.ButtonAttribute button in buttonList)
                    {
                        build2.AppendFormat("<li><a href=\"#\" id=\"{1}\" title=\"{2}\" class=\"custom postBack{3}\">{0}</a></li>", button.Title, button.ID, button.InteractiveHelp, button.AskConfirmation ? " type_confirm" : null);
                    }
                }

            }
            if (build2.Length > 0)
            {
                build.Append("<ul class=\"subNavigation\" class=\"pseudoHover\">");
                build.Append(build2.ToString());
                build.Append("</ul>");
            }
        }

        /// <summary>
        /// Sets the library navigation.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="build">The build.</param>
        /// <param name="previousUrl">The previous URL.</param>
        /// <param name="isEditMode">if set to <c>true</c> [is edit mode].</param>
        /// <param name="isTextMode">if set to <c>true</c> [is text mode].</param>
        /// <param name="isListMode">if set to <c>true</c> [is list mode].</param>
        /// <param name="isBrowseMode">if set to <c>true</c> [is browse mode].</param>
        static void SetLibraryNavigation(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, StringBuilder build, ref string previousUrl, bool isEditMode, bool isTextMode, bool isListMode, bool isBrowseMode)
        {
            if (isBrowseMode)
            {
                previousUrl = string.Concat(container.WimPagePath, "?", "gallery=", container.CurrentListInstance.wim.CurrentFolder.ParentID.GetValueOrDefault(0));

                build.Append("<ul class=\"subNavigation\" class=\"pseudoHover\">");
                Data.IComponentList list0 = Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.ComponentListProperties);
                Data.IComponentList list1 = Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.Folders);

                previousUrl = string.Concat(container.WimPagePath, "?", "gallery=", container.CurrentListInstance.wim.CurrentFolder.ID);
                //  Create new element

                Sushi.Mediakiwi.Data.Gallery g = Sushi.Mediakiwi.Data.Gallery.SelectOne(container.CurrentListInstance.wim.CurrentFolder.ID);
                
                build.AppendFormat("<li><a href=\"{0}\" class=\"createPage\">New asset</a></li>", string.Concat(container.WimPagePath, "?gallery=", container.CurrentListInstance.wim.CurrentFolder.ID, "&asset=0"));
                build.AppendFormat("<li><a href=\"{0}\" class=\"createFolder\">Create folder</a></li>", string.Concat(container.WimPagePath, "?", "list=", list1.ID, "&gallery=", container.CurrentListInstance.wim.CurrentFolder.ID, "&item=0"));
                
                build.Append("</ul><ul class=\"subNavigation\" class=\"pseudoHover\">");

                if (container.CurrentApplicationUser.ShowDetailView)
                    build.AppendFormat("<li><a href=\"{0}\" id=\"thumbview\" class=\"properties postBack\">Thumbnail view</a></li>", "#");
                else
                    build.AppendFormat("<li><a href=\"{0}\" id=\"detailview\" class=\"properties postBack\">Detail view</a></li>", "#");
            }
            else if (isTextMode)
            {

                previousUrl = string.Concat(container.WimPagePath, "?", "gallery=", container.CurrentListInstance.wim.CurrentFolder.ID);

                if (container.CurrentList.Type != Sushi.Mediakiwi.Data.ComponentListType.ComponentListProperties)
                {
                    build.Append("<ul class=\"subNavigation\" class=\"pseudoHover\">");

                    //  Edit
                    build.AppendFormat("<li><a href=\"{0}\" id=\"edit\" class=\"edit postBack\">Edit</a></li>", "#");

                    if (container.CurrentListInstance.wim.HasListDelete)
                    {
                        build.Append("</ul><ul class=\"subNavigation\" class=\"pseudoHover\">");
                        build.AppendFormat("<li><a href=\"{0}\" id=\"delete\" class=\"delete postBack type_confirm\">Delete</a></li>", "#");
                    }
                }
                else
                    return;
            }
            else if (isEditMode)
            {
                build.Append("<ul class=\"subNavigation\" class=\"pseudoHover\">");
                previousUrl = string.Concat(container.WimPagePath, "?", "gallery=", container.CurrentListInstance.wim.CurrentFolder.ID);
                //  Save
                build.AppendFormat("<li><a href=\"{0}\" id=\"save\" class=\"edit postBack\">Save</a></li>", "#");
            }
            build.Append("</ul>");
        }

        /// <summary>
        /// Sets the admin navigation.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="build">The build.</param>
        /// <param name="previousUrl">The previous URL.</param>
        /// <param name="isEditMode">if set to <c>true</c> [is edit mode].</param>
        /// <param name="isTextMode">if set to <c>true</c> [is text mode].</param>
        /// <param name="isListMode">if set to <c>true</c> [is list mode].</param>
        /// <param name="isBrowseMode">if set to <c>true</c> [is browse mode].</param>
        /// <param name="buttonList">The button list.</param>
        static void SetAdminNavigation(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, StringBuilder build, ref string previousUrl, bool isEditMode, bool isTextMode, bool isListMode, bool isBrowseMode, Framework.ContentListItem.ButtonAttribute[] buttonList)
        {
            if (isBrowseMode)
            {
                previousUrl = string.Concat(container.WimPagePath, "?", "folder=", container.CurrentListInstance.wim.CurrentFolder.ParentID.GetValueOrDefault(0));
                Data.IComponentList list0 = Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.Folders);
                Data.IComponentList list1 = Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.PageProperties);

                build.Append("<ul class=\"subNavigation\" class=\"pseudoHover\">");
                if (container.CurrentApplicationUser.ShowDetailView)
                    build.AppendFormat("<li><a href=\"{0}\" id=\"thumbview\" class=\"properties postBack\">Thumbnail view</a></li>", "#");
                else
                    build.AppendFormat("<li><a href=\"{0}\" id=\"detailview\" class=\"properties postBack\">Detail view</a></li>", "#");
            }
            else if (isListMode)
            {
                build.Append("<ul class=\"subNavigation\" class=\"pseudoHover\">");

                Data.IComponentList list0 = Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.ComponentListProperties);

                previousUrl = string.Concat(container.WimPagePath, "?", "folder=", container.CurrentListInstance.wim.CurrentFolder.ID);
                //  Create new element
                build.AppendFormat("<li><a href=\"{0}\" class=\"createPage\">New record</a></li>", string.Concat(container.CurrentListInstance.wim.SearchResultItemPassthroughParameter, "=0"));
                build.AppendFormat("<li><a href=\"{0}\" class=\"properties\">Properties</a></li>", string.Concat(container.WimPagePath, "?", "list=", list0.ID, "&folder=", container.CurrentListInstance.wim.CurrentFolder.ID, "&item=", container.CurrentList.ID));

                //  Custom buttons
                if (buttonList != null)
                {
                    build.Append("</ul><div class=\"hr\"><hr /></div><ul class=\"subNavigation\" class=\"pseudoHover\">");
                    foreach (Framework.ContentListItem.ButtonAttribute button in buttonList)
                    {
                        build.AppendFormat("<li><a href=\"#\" title=\"{2}\" id=\"{1}\" class=\"custom postBack{3}\">{0}</a></li>", button.Title, button.ID, button.InteractiveHelp, button.AskConfirmation ? " type_confirm" : null);
                    }
                }
            }
            else if (isTextMode)
            {
                
                previousUrl = string.Concat(container.WimPagePath, "?", "list=", container.CurrentList.ID);

                if (container.CurrentList.Type != Sushi.Mediakiwi.Data.ComponentListType.ComponentListProperties)
                {
                    build.Append("<ul class=\"subNavigation\" class=\"pseudoHover\">");

                    //  Edit
                    build.AppendFormat("<li><a href=\"{0}\" id=\"edit\" class=\"edit postBack\">Edit</a></li>", "#");

                    if (container.CurrentListInstance.wim.HasListDelete)
                    {
                        build.Append("</ul><ul class=\"subNavigation\" class=\"pseudoHover\">");
                        build.AppendFormat("<li><a href=\"{0}\" id=\"delete\" class=\"delete postBack type_confirm\">Delete</a></li>", "#");
                    }

                    //  Custom buttons
                    if (buttonList != null)
                    {
                        build.Append("</ul><div class=\"hr\"><hr /></div><ul class=\"subNavigation\" class=\"pseudoHover\">");
                        foreach (Framework.ContentListItem.ButtonAttribute button in buttonList)
                        {
                            build.AppendFormat("<li><a href=\"#\" id=\"{1}\" title=\"{2}\" class=\"custom postBack{3}\">{0}</a></li>", button.Title, button.ID, button.InteractiveHelp, button.AskConfirmation ? " type_confirm" : null);
                        }
                    }
                }
                else
                    return;
            }
            else if (isEditMode)
            {
                build.Append("<ul class=\"subNavigation\" class=\"pseudoHover\">");
                previousUrl = string.Concat(container.WimPagePath, "?", "list=", container.CurrentList.ID);
                //  Save
                build.AppendFormat("<li><a href=\"{0}\" id=\"save\" class=\"edit postBack\">Save</a></li>", "#");

                //  Custom buttons
                if (buttonList != null)
                {
                    build.Append("</ul><div class=\"hr\"><hr /></div><ul class=\"subNavigation\" class=\"pseudoHover\">");
                    foreach (Framework.ContentListItem.ButtonAttribute button in buttonList)
                    {
                        build.AppendFormat("<li><a href=\"#\" title=\"{2}\" id=\"{1}\" class=\"custom postBack{3}\">{0}</a></li>", button.Title, button.ID, button.InteractiveHelp, button.AskConfirmation ? " type_confirm" : null);
                    }
                }
            }
            build.Append("</ul>");
        }

    }
}
