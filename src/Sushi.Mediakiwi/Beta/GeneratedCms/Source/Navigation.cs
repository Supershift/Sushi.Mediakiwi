using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.Data;

namespace Sushi.Mediakiwi.Beta.GeneratedCms.Source
{
    /// <summary>
    /// 
    /// </summary>
    public class Navigation
    {
        /// <summary>
        /// Leftnavigations the specified container.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns></returns>
        internal static string Leftnavigation(Console container)
        {
            return Leftnavigation(container, null);
        }

        /// <summary>
        /// Gets the left navigation.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="buttonList">The button list.</param>
        /// <returns></returns>
        internal static string NewLeftNavigationOLD(Console container, Framework.ContentListItem.ButtonAttribute[] buttonList)
        {
            StringBuilder build = new StringBuilder();

            Sushi.Mediakiwi.Data.Folder currentFolder = container.CurrentListInstance.wim.CurrentFolder;

            //  If the request is in a tabular the left navigation should show the navigation of the primary list (group ID)
            if (string.IsNullOrEmpty(container.Request.Query["folder"]) && container.Group.HasValue)
            {
                var folderList = Sushi.Mediakiwi.Data.ComponentList.SelectOne(container.Group.Value);
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

            if (true)
            {
                if (currentFolder.ID == 0)
                    return @"<div id=""leftColumn"" class=""contentPage""></div>";

                if (currentFolder.Type != Sushi.Mediakiwi.Data.FolderType.Gallery)
                {
                    if (currentFolder.ParentID.HasValue)
                    {
                        build.AppendFormat(@"<li><a href=""{0}"" class=""{1}"">{2}</a></li>", container.UrlBuild.GetFolderRequest(currentFolder), "folder", currentFolder.Name, container.CurrentApplicationUser.RoleID);
                        build.Append("</ul>");
                        build.Insert(0, "<ul class=\"subNavigation\" class=\"pseudoHover\">");
                    }

                    //  Get all folders except the current one
                    foreach (Data.Folder folder in Data.Folder.SelectAllByParent(currentFolder.ID, currentFolder.Type))
                    {
                        build.AppendFormat(@"<li><a href=""{0}"" class=""{1}"">{2}</a></li>", container.UrlBuild.GetFolderRequest(folder), "folder", folder.Name);
                    }
                }
                else
                {
                    if (currentFolder.ParentID.HasValue)
                    {
                        build.AppendFormat(@"<li><a href=""{0}"" class=""{1}"">{2}</a></li>", container.UrlBuild.GetGalleryRequest(currentFolder.ID), "folder", currentFolder.Name, container.CurrentApplicationUser.RoleID);
                        build.Append("</ul>");
                        build.Insert(0, "<ul class=\"subNavigation\" class=\"pseudoHover\">");
                    }

                    //  Get all folders except the current one
                    foreach (Data.Gallery gallery in Data.Gallery.SelectAllByParent(currentFolder.ID))
                    {
                        build.AppendFormat(@"<li><a href=""{0}"" class=""{1}"">{2}</a></li>", container.UrlBuild.GetGalleryRequest(gallery), "folder", gallery.Name);
                    }
                }
            }

            #region Foldertype: Lists
            if (currentFolder.Type == Sushi.Mediakiwi.Data.FolderType.List || currentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Administration)
            {
                int currentListID = container.Group.HasValue ? container.Group.Value : container.CurrentList.ID;
                //  Get all lists
                //foreach (Data.IComponentList list in Data.ComponentList.SelectAll(currentFolder.ID, container.CurrentApplicationUser.RoleID))
                //{
                //    if (list.IsVisible)
                //        build.AppendFormat(@"<li><a href=""{0}"" class=""{1}{3}"">{2}</a></li>", container.UrlBuild.GetListRequest(list), "list", list.Name, list.ID == currentListID ? " active" : "");
                //}
            }
            #endregion
            #region Foldertype: Pages
            else if (currentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Page)
            {
                //  Get all pages
                foreach (Data.Page page in Data.Page.SelectAll(currentFolder.ID, Sushi.Mediakiwi.Data.PageFolderSortType.Folder, Sushi.Mediakiwi.Data.PageReturnProperySet.All, Sushi.Mediakiwi.Data.PageSortBy.SortOrder, false))
                {
                    string className = "page";
                    if (!page.IsPublished)
                    {
                        if (page.MasterID.HasValue)
                        {
                            if (page.InheritContent)
                                className = "pageInheritUnpub";
                            else
                                className = "pageLocalUnpub";
                        }
                        else
                            className = "pageUnpub";
                    }
                    else if (page.IsEdited)
                    {
                        if (page.MasterID.HasValue)
                        {
                            if (page.InheritContent)
                                className = "pageInheritEdit";
                            else
                                className = "pageLocalEdit";
                        }
                        else
                            className = "pageEdit";
                    }
                    else if (page.IsPublished)
                    {
                        if (page.MasterID.HasValue)
                        {
                            if (page.InheritContent)
                                className = "pageInherit";
                            else
                                className = "pageLocal";
                        }
                        else
                            className = "page";
                    }

                    build.AppendFormat(@"<li><a href=""{0}"" class=""{1}{3}"">{2}</a></li>", container.UrlBuild.GetPageRequest(page), className, page.Name, (page.ID == container.Item.GetValueOrDefault() && container.ItemType == RequestItemType.Page)  ? " active" : "");
                }
            }
            #endregion

            if (build.Length > 0)
            {
                build.Insert(0, "<ul class=\"subNavigation\" class=\"pseudoHover\">");
                build.Append("</ul>");

                
            }


            return string.Format(@"
<div id=""leftColumn"" class=""contentPage"">
	<ul class=""subNavigation pseudoHover"">
		<li><a href=""{1}"" class=""folder"">(parent folder)</a></li>
	</ul>
	<div class=""hr""><hr></div>{2}
</div>
"
            , container.WimRepository
            , currentFolder.ParentID.HasValue ?
            (currentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Gallery ? container.UrlBuild.GetGalleryRequest(currentFolder.ParentID.Value) : container.UrlBuild.GetFolderRequest(currentFolder.ParentID.Value))
                : container.UrlBuild.GetSectionRequest(currentFolder.Type)
            , build.ToString()
            );
        }

        internal static string NewLeftNavigation(Console container, Framework.ContentListItem.ButtonAttribute[] buttonList)
        {
            bool isFirstLevelRootnavigation = false;
            bool isFirst = true;

            StringBuilder build = new StringBuilder();

            Sushi.Mediakiwi.Data.Folder currentFolder = container.CurrentListInstance.wim.CurrentFolder;

            //  If the request is in a tabular the left navigation should show the navigation of the primary list (group ID)
            if (string.IsNullOrEmpty(container.Request.Query["folder"]) && container.Group.HasValue)
            {
                var folderList = Sushi.Mediakiwi.Data.ComponentList.SelectOne(container.Group.Value);
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
                    if (isActive)
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
            if (currentFolder.Type == Sushi.Mediakiwi.Data.FolderType.List || currentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Administration)
            {
                if (currentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Administration)
                    isFirstLevelRootnavigation = false;

                int currentListID = container.Group.HasValue ? container.Group.Value : container.CurrentList.ID;


                int start = isFirstLevelRootnavigation ? 1 : 0;

                //Data.Folder root;

                Data.Folder level1;

                //start = 0;

                if (start == 0)
                    level1 = Data.Folder.SelectOneBySite(container.CurrentListInstance.wim.CurrentSite.ID, currentFolder.Type);
                else
                    level1 = Data.Folder.SelectOne(currentFolder, start);

                currentName = level1.Name == "/" ? "Settings" : level1.Name;
                currentLink = container.UrlBuild.GetFolderRequest(level1);

                Data.Folder level2 = Data.Folder.SelectOne(currentFolder, start + 1);
                Data.Folder level3 = Data.Folder.SelectOne(currentFolder, start + 2);
                Data.Folder level4 = Data.Folder.SelectOne(currentFolder, start + 3);

                //if (isFirstLevelRootnavigation)
                //    root = Data.Folder.SelectOne(currentFolder, 1);
                //else
                //    root = Data.Folder.SelectOneBySite(container.CurrentListInstance.wim.CurrentSite.ID, currentFolder.Type);

                //  LEVEL 1 : Folders
                Data.Folder[] folders1 = Data.Folder.SelectAllByParent(level1.ID);
                folders1 = Data.Folder.ValidateAccessRight(folders1, container.CurrentApplicationUser);

                foreach (Data.Folder folder in folders1)
                {
                    bool isActive;

                    if (start == 1)
                        isActive = currentFolder.ID == folder.ID || level1.ID == folder.ID;
                    else
                        isActive = currentFolder.ID == folder.ID || level2.ID == folder.ID;

                    build.AppendFormat(@"<li><a href=""{0}"" class=""{1}{4}{3}"">{2}</a>", container.UrlBuild.GetFolderRequest(folder), "folder", folder.Name, isActive ? " active" : "", isFirst ? " first" : null);

                    isFirst = false;

                    #region Level 2
                    if (isActive)
                    {
                        build.AppendFormat(@"<ul>");

                        //  LEVEL 2 : Folders
                        Data.Folder[] folders2 = Data.Folder.SelectAllByParent(folder.ID);
                        folders2 = Data.Folder.ValidateAccessRight(folders2, container.CurrentApplicationUser);

                        foreach (Data.Folder folder2 in folders2)
                        {
                            bool isActive2 = (folder2.ID == currentFolder.ID) || (isFirstLevelRootnavigation ? level2.ID : level3.ID) == folder2.ID;
                            build.AppendFormat(@"<li><a href=""{0}"" class=""{1}{3}"">{2}</a>", container.UrlBuild.GetFolderRequest(folder2), "folder", folder2.Name, isActive2 ? " active" : "");

                            #region Level 3
                            if (isActive2)
                            {
                                build.AppendFormat(@"<ul>");
                                //  LEVEL 3 : Folders
                                Data.Folder[] folders3 = Data.Folder.SelectAllByParent(folder2.ID);
                                folders3 = Data.Folder.ValidateAccessRight(folders3, container.CurrentApplicationUser);

                                foreach (Data.Folder folder3 in folders3)
                                {
                                    bool isActive3 = (folder3.ID == currentFolder.ID) || (isFirstLevelRootnavigation ? level3.ID : level4.ID) == folder3.ID;
                                    build.AppendFormat(@"<li><a href=""{0}"" class=""{1}{3}"">{2}</a>", container.UrlBuild.GetFolderRequest(folder3), "folder", folder3.Name, isActive3 ? " active" : "");
                                }
                                //  LEVEL 3 : Lists
                                Data.IComponentList[] lists3 = Data.ComponentList.SelectAll(folder2.ID);
                                lists3 = Sushi.Mediakiwi.Data.ComponentList.ValidateAccessRight(lists3, container.CurrentApplicationUser, container.CurrentListInstance.wim.CurrentSite.ID);

                                foreach (Data.IComponentList list in lists3)
                                {
                                    bool isActive3 = (list.ID == currentListID);
                                    if (list.IsVisible)
                                        build.AppendFormat(@"<li><a href=""{0}"" class=""{1}{3}"">{2}</a></li>", container.UrlBuild.GetListRequest(list), "list", list.Name, isActive3 ? " active" : "");
                                }
                                build.AppendFormat(@"</ul>");
                            }
                            #endregion

                            build.AppendFormat(@"</li>");
                        }

                        //  LEVEL 2 : Lists
                        Data.IComponentList[] lists2 = Data.ComponentList.SelectAll(folder.ID);
                        lists2 = Sushi.Mediakiwi.Data.ComponentList.ValidateAccessRight(lists2, container.CurrentApplicationUser, container.CurrentListInstance.wim.CurrentSite.ID);

                        foreach (Data.IComponentList list in lists2)
                        {
                            bool isActive2 = (list.ID == currentListID);
                            if (list.IsVisible)
                                build.AppendFormat(@"<li><a href=""{0}"" class=""{1}{3}"">{2}</a></li>", container.UrlBuild.GetListRequest(list), "list", list.Name, isActive2 ? " active" : "");
                        }
                        build.AppendFormat(@"</ul>");
                    }
                    #endregion 

                    build.AppendFormat(@"</li>");


                }
                //  LEVEL 1 : Lists
                Data.IComponentList[] lists1 = Data.ComponentList.SelectAll(level1.ID);
                lists1 = Sushi.Mediakiwi.Data.ComponentList.ValidateAccessRight(lists1, container.CurrentApplicationUser, container.CurrentListInstance.wim.CurrentSite.ID);

                foreach (Data.IComponentList list in lists1)
                {
                    if (list.IsVisible)
                    {
                        build.AppendFormat(@"<li><a href=""{0}"" class=""{1}{4}{3}"">{2}</a></li>", container.UrlBuild.GetListRequest(list), "list", list.Name, list.ID == currentListID ? " active" : "", isFirst ? " first" : null);
                        isFirst = false;
                    }
                }
            }
            #endregion
            #region Foldertype: Pages
            else if (currentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Page)
            {
                isFirstLevelRootnavigation = false;

                int currentListID = container.Group.HasValue ? container.Group.Value : container.CurrentList.ID;

                Data.Folder root;

                Data.Folder level1 = Data.Folder.SelectOne(currentFolder, 1);
                Data.Folder level2 = Data.Folder.SelectOne(currentFolder, 2);
                Data.Folder level3 = Data.Folder.SelectOne(currentFolder, 3);

                if (isFirstLevelRootnavigation)
                    root = Data.Folder.SelectOne(currentFolder, 1);
                else
                    root = Data.Folder.SelectOneBySite(container.CurrentListInstance.wim.CurrentSite.ID, currentFolder.Type);

                currentName = root.Name == "/" ? "Website" : root.Name;
                currentLink = container.UrlBuild.GetFolderRequest(root.ID);

                //  LEVEL 1 : Folders
                Data.Folder[] folders1 = Data.Folder.SelectAllByParent(root.ID);
                folders1 = Data.Folder.ValidateAccessRight(folders1, container.CurrentApplicationUser);
                foreach (Data.Folder folder in folders1)
                {
                    bool isActive = currentFolder.ID == folder.ID || level1.ID == folder.ID;
                    build.AppendFormat(@"<li><a href=""{0}"" class=""{1}{3}{4}{5}"">{2}</a>", container.UrlBuild.GetFolderRequest(folder), "folder", folder.Name, isActive ? " active" : "", null, isFirst ? " first" : null);

                    isFirst = false;

                    #region Level 2
                    if (isActive)
                    {
                        build.AppendFormat(@"<ul>");

                        //  LEVEL 2 : Folders
                        Data.Folder[] folders2 = Data.Folder.SelectAllByParent(folder.ID);
                        folders2 = Data.Folder.ValidateAccessRight(folders2, container.CurrentApplicationUser);
                        foreach (Data.Folder folder2 in folders2)
                        {
                            bool isActive2 = (folder2.ID == currentFolder.ID) || level2.ID == folder2.ID;
                            build.AppendFormat(@"<li><a href=""{0}"" class=""{1}{3}"">{2}</a>", container.UrlBuild.GetFolderRequest(folder2), "folder", folder2.Name, isActive2 ? " active" : "");

                            #region Level 3
                            if (isActive2)
                            {
                                build.AppendFormat(@"<ul>");
                                //  LEVEL 3 : Folders
                                Data.Folder[] folders3 = Data.Folder.SelectAllByParent(folder2.ID);
                                folders3 = Data.Folder.ValidateAccessRight(folders3, container.CurrentApplicationUser);
                                foreach (Data.Folder folder3 in folders3)
                                {
                                    bool isActive3 = (folder3.ID == currentFolder.ID) || level3.ID == folder3.ID;
                                    build.AppendFormat(@"<li><a href=""{0}"" class=""{1}{3}"">{2}</a>", container.UrlBuild.GetFolderRequest(folder3), "folder", folder3.Name, isActive3 ? " active" : "");

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

            //if (build.Length > 0)
            //{
            //    build.Insert(0, "<ul class=\"subNavigation\" class=\"pseudoHover\">");
            //    build.Append("</ul>");
            //}

            if (container.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.InformationMessage)
            {
                build = new StringBuilder();
                build.AppendFormat(@"<li><a href=""{0}"" class=""{1}"">{2}</a></li>", container.UrlBuild.GetHomeRequest(), "list", "Home");
            }

            //if (!container.CurrentWimNavigation.ShowFolders)
            //{
                //if (build.Length == 0)
                //{

            if (!string.IsNullOrEmpty(currentLink))
                currentName = string.Format("<a href=\"{1}\">{0}</a>", currentName, currentLink);
            
            return string.Format(@"
<div id=""leftColumn"" class=""contentPage"">
	<div class=""col_0 row_0 wdt_1"">
		<h2 class=""portalWindowTitle"">{1}</h2>
		<div class=""portalWindowBorder"">
            <ul class=""subNavigationElse"">
            {2}
            </ul>
		</div>
	</div>	
</div>
"
                , container.WimRepository
                , currentName
                , build.ToString()
                );


//                }
//                return string.Format(@"
//<div id=""leftColumn"">
//	{0}
//</div>
//", build.ToString());
//            }

//            return string.Format(@"
//<div id=""leftColumn"">
//	<ul class=""subNavigation pseudoHover"">
//		<li><a href=""{1}"" class=""folder"">(parent folder)</a></li>
//	</ul>
//	<div class=""hr""><hr></div>{2}
//</div>
//"
//            , container.WimRepository
//            , currentFolder.ParentID.HasValue ?
//            (currentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Gallery ? container.UrlBuild.GetGalleryRequest(currentFolder.ParentID.Value) : container.UrlBuild.GetFolderRequest(currentFolder.ParentID.Value))
//                : container.UrlBuild.GetSectionRequest(currentFolder.Type)
//            , build.ToString()
//            );
        }

        /// <summary>
        /// Leftnavigations this instance.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="buttonList">The button list.</param>
        /// <returns></returns>
        internal static string Leftnavigation(Console container, Framework.ContentListItem.ButtonAttribute[] buttonList)
        {
            bool isNewNavigation = true;

            if (isNewNavigation)
                return NewLeftNavigation(container, null);


            string previousUrl = null;

            StringBuilder build = new StringBuilder();

            if (container.IsCodeUpdate)
            {
                previousUrl = container.GetSafeUrl();

                //  Custom buttons
                StringBuilder build2 = new StringBuilder();
                if (buttonList != null)
                {
                    foreach (Framework.ContentListItem.ButtonAttribute button in buttonList)
                    {
                        build2.AppendFormat("<li><input type=\"hidden\" name=\"{1}\"/><a href=\"#\" title=\"{2}\" class=\"custom postBack\">{0}</a></li>", button.Title, button.ID, button.InteractiveHelp);
                    }
                }

                if (build2.Length > 0)
                {
                    build.Append("<ul class=\"subNavigation\" class=\"pseudoHover\">");
                    build.Append(build2.ToString());
                    build.Append("</ul>");
                }

            }
            else
            {
                if (container.CurrentListInstance.wim.CurrentFolder == null || container.CurrentListInstance.wim.CurrentFolder.IsNewInstance) return null;

                int section = (int)container.CurrentListInstance.wim.CurrentFolder.Type;

                //  When in the page section, but on a property list page the actual section = list
                if (section == (int)Data.FolderType.Page && container.CurrentList.Type != Sushi.Mediakiwi.Data.ComponentListType.Browsing)
                    section = (int)Data.FolderType.List;

                bool isEditMode = container.CurrentListInstance.wim.IsEditMode;
                bool isTextMode = !isEditMode;
                bool isListMode = container.View == 2;
                bool isBrowseMode = container.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.Browsing && !container.Item.HasValue;

                if (section == (int)Data.FolderType.Page)
                    //  Page navigation
                    SetPageNavigation(container, build, ref previousUrl, isEditMode, isTextMode, isListMode, isBrowseMode);
                else if (section == (int)Data.FolderType.List)
                {
                    if (!container.CurrentListInstance.wim.HasListSave)
                    {
                        isEditMode = false;
                        isTextMode = true;
                    }
                    //  List navigation
                    SetListNavigation(container, build, ref previousUrl, isEditMode, isTextMode, isListMode, isBrowseMode, buttonList);
                }
                else if (section == (int)Data.FolderType.Administration)
                    //  List navigation
                    SetAdminNavigation(container, build, ref previousUrl, isEditMode, isTextMode, isListMode, isBrowseMode, buttonList);
                else if (section == (int)Data.FolderType.Gallery)
                    //  library navigation
                    SetLibraryNavigation(container, build, ref previousUrl, isEditMode, isTextMode, isListMode, isBrowseMode);
            }

            return string.Format(@"
<div id=""leftColumn"" class=""contentPage"">
	<ul class=""subNavigation pseudoHover"">
		<li><a href=""{1}"" class=""back"">Back</a></li>
	</ul>
	<div class=""hr""><hr></div>{2}
</div>
"
    , container.WimRepository
    , previousUrl
    , build.ToString()
    );
        }

        /// <summary>
        /// News the bottom navigation.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="buttonList">The button list.</param>
        /// <param name="hasFilters">if set to <c>true</c> [has filters].</param>
        /// <returns></returns>
        internal static string NewBottomNavigation(Console container, Framework.ContentListItem.ButtonAttribute[] buttonList, bool hasFilters)
        {
            StringBuilder build = new StringBuilder();
            StringBuilder build2 = new StringBuilder();

            string saveRecord = container.CurrentList.Data["wim_LblSave"].Value;
            if (string.IsNullOrEmpty(saveRecord))
                saveRecord = Resource.ResourceManager.GetString("save", new CultureInfo(container.CurrentApplicationUser.LanguageCulture));

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
                search = Resource.ResourceManager.GetString("search", new CultureInfo(container.CurrentApplicationUser.LanguageCulture));


            string className = " left";

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
                        if (section != Sushi.Mediakiwi.Data.FolderType.Page && container.CurrentListInstance.wim.CanSaveAndAddNew && !container.CurrentListInstance.wim.CanContainSingleInstancePerDefinedList)
                            build2.AppendFormat("<li class=\"right\"><input type=\"hidden\" name=\"saveNew\"/><a href=\"#\" class=\"save postBack\">{0}</a></li>"
                                , Resource.ResourceManager.GetString("save_and_new", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                                );

                        build2.AppendFormat("<li class=\"right\"><input type=\"hidden\" name=\"save\"/><input type=\"hidden\" name=\"state\" value=\"2\" /><a href=\"#\" class=\"save postBack\">{0}</a></li>", saveRecord);
                    }

                }
            }

            if (buttonList != null)
            {
                var selection = (from item in buttonList where item.IconTarget == Sushi.Mediakiwi.Framework.ButtonTarget.BottomLeft select item);

                foreach (Framework.ContentListItem.ButtonAttribute button in selection)
                    build2.AppendFormat("<li><input type=\"hidden\" name=\"{1}\"/><a href=\"#\" title=\"{2}\" class=\"{4} postBack{3}\">{0}</a></li>", button.Title, button.ID, button.InteractiveHelp, button.AskConfirmation ? " type_confirm" : null, button.IconClassName);

                selection = (from item in buttonList where item.IconTarget == Sushi.Mediakiwi.Framework.ButtonTarget.BottomRight select item);
                foreach (Framework.ContentListItem.ButtonAttribute button in selection)
                    build2.AppendFormat("<li class=\"right\"><input type=\"hidden\" name=\"{1}\"/><a href=\"#\" title=\"{2}\" class=\"{4} postBack{3}\">{0}</a></li>", button.Title, button.ID, button.InteractiveHelp, button.AskConfirmation ? " type_confirm" : null, button.IconClassName);
            }

            if (isListMode && !string.IsNullOrEmpty(search) && !container.CurrentListInstance.wim.HideSearchButton)
            {
                Framework.ContentListSearchItem.ButtonAttribute button = new Sushi.Mediakiwi.Framework.ContentListSearchItem.ButtonAttribute(search, false, true);
                button.ID = "searchBtn";
                button.m_IsFormElement = false;
                button.Console = container;
                button.IconTarget = Sushi.Mediakiwi.Framework.ButtonTarget.BottomRight;
                button.IconType = Sushi.Mediakiwi.Framework.ButtonIconType.Play;
                build2.AppendFormat("<li class=\"right\"><input type=\"hidden\" name=\"{1}\"/><a href=\"#\" title=\"{2}\" class=\"{4} postBack{3}\">{0}</a></li>", button.Title, button.ID, button.InteractiveHelp, button.AskConfirmation ? " type_confirm" : null, button.IconClassName);
            }


            //foreach (Framework.ContentListItem.ButtonAttribute button in bottom)
            //{
            //    build2.AppendFormat("<li class=\"right\"><input type=\"hidden\" name=\"{1}\"/><a href=\"#\" title=\"{2}\" class=\"{4} postBack{3}\">{0}</a></li>", button.Title, button.ID, button.InteractiveHelp, button.AskConfirmation ? " type_confirm" : null, button.IconClassName);
            //}

            if (build2.Length == 0) return null;

            build.AppendFormat("\n<ul id=\"pathNavigation\" class=\"pathNavigation bottomPathNavigation{1}\">{0}</ul>", build2.ToString(), className);
            
            return build.ToString();
        }

        internal static string TargetInfo(Framework.ContentListItem.ButtonAttribute button, Console container)
        {
            if (string.IsNullOrEmpty(button.Target))
                return null;
            return string.Format(" target=\"{0}\"", button.Target);
        }

        internal static string LayerList(Framework.ContentListItem.ButtonAttribute button, Console container)
        {
            if (!string.IsNullOrEmpty(button.CustomUrlProperty))
            {
                return container.CurrentListInstance.GetType().GetProperty(button.CustomUrlProperty).GetValue(container.CurrentListInstance, null) as string;
            }

            if (!string.IsNullOrEmpty(button.ListInPopupLayer))
            {
                var list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(new Guid(button.ListInPopupLayer));
                if (!list.IsNewInstance)
                {
                    string prefix = container.UrlBuild.GetListRequest(list, container.Item);
                    return string.Concat(prefix, "&openinframe=2");
                }
            }
            return null;
        }

        //static void CleanButtonList(Console container, ref Framework.ContentListItem.ButtonAttribute[] buttonList)
        //{
        //    //List<Framework.ContentListItem.ButtonAttribute> selection = new List<Framework.ContentListItem.ButtonAttribute>();
        //    //foreach (var button in buttonList)
        //    //{
        //    //    bool isVisible = container.CurrentListInstance.wim.IsVisible(container.CurrentListInstance, container.CurrentListInstance, button.FieldName, true);
        //    //    if (isVisible)
        //    //        selection.Add(button);
        //    //}
        //    //buttonList = selection.ToArray();
        //}

        internal static string NewTopNavigation(Console container, Framework.ContentListItem.ButtonAttribute[] buttonList)
        {
            StringBuilder build = new StringBuilder();
            StringBuilder build2 = new StringBuilder();
            StringBuilder innerbuild = new StringBuilder();

            string newRecord = container.CurrentList.Data["wim_LblNew"].Value;
            string saveRecord = container.CurrentList.Data["wim_LblSave"].Value;
            
            if (string.IsNullOrEmpty(newRecord)) 
                newRecord = Resource.ResourceManager.GetString("new_record", new CultureInfo(container.CurrentApplicationUser.LanguageCulture));
            
            if (string.IsNullOrEmpty(saveRecord))
                saveRecord = Resource.ResourceManager.GetString("save", new CultureInfo(container.CurrentApplicationUser.LanguageCulture));

            if (    container.CurrentListInstance.wim.CurrentFolder == null 
                    || container.CurrentListInstance.wim.CurrentFolder.IsNewInstance
                )
            {
                build.Append("\n<ul id=\"pathNavigation\" class=\"pathNavigation pseudoHover left\"></ul>");
                return build.ToString();
            }

            Data.FolderType section = container.CurrentListInstance.wim.CurrentFolder.Type;

            //  When in the page section, but on a property list page the actual section = list
            if (section == Data.FolderType.Page && container.CurrentList.Type != Sushi.Mediakiwi.Data.ComponentListType.Browsing)
                section = Data.FolderType.List;

            bool isEditMode = container.CurrentListInstance.wim.IsEditMode;
            bool isTextMode = !isEditMode;
            bool isListMode = container.View == 2;
            bool isBrowseMode = container.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.Browsing && !container.Item.HasValue;
            bool showNewNavigation = false;// Wim.CommonConfiguration.NEW_NAVIGATION && !container.CurrentWimNavigation.ShowFolders;

            string className = null;
            //  Lists
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
                        build2.AppendFormat("<li><a href=\"{0}\" class=\"createPage\">{1}</a></li>", container.UrlBuild.GetNewListRequest()
                            , Resource.ResourceManager.GetString("list_new", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                            );
                    }

                    className = " left";

                    if (!showNewNavigation)
                    {
                        if (container.CurrentApplicationUser.ShowDetailView)
                            build2.AppendFormat("<li class=\"right\"><input type=\"hidden\" name=\"thumbview\"/><a href=\"{0}\" class=\"thumbview postBack\">{1}</a></li>", "#"
                                , Resource.ResourceManager.GetString("thumbnail_view", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                                );
                        else
                            build2.AppendFormat("<li class=\"right\"><input type=\"hidden\" name=\"detailview\"/><a href=\"{0}\" class=\"properties postBack\">{1}</a></li>", "#"
                                , Resource.ResourceManager.GetString("detail_view", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                                );
                    }
                    else
                    {
                        if (container.CurrentApplicationUser.ShowDetailView)
                        {
                            container.CurrentApplicationUser.ShowDetailView = false;
                            container.CurrentApplicationUser.Save();
                        }
                    }

                    if (container.CurrentListInstance.wim.CurrentApplicationUserRole.CanCreateList && !container.CurrentListInstance.wim.IsSubSelectMode)
                    {

                        build2.AppendFormat(@"<li class=""right""><a href=""#"" class=""folder"">{4}</a>
	<ul>
		<li><a href=""{0}"" class=""createFolder"">{2}</a></li>
		<li><a href=""{1}"" class=""folderOptions"">{3}</a></li>
	</ul>				    
</li>", container.UrlBuild.GetFolderCreateRequest(), container.UrlBuild.GetFolderOptionsRequest(), newRecord
      , Resource.ResourceManager.GetString("properties", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
      , Resource.ResourceManager.GetString("folder_options", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
      );
                    }
                }
                else if (isListMode)
                {
                    if (CommonConfiguration.NEW_NAVIGATION && container.OpenInFrame == 0)
                    {
                        build2.AppendFormat("<li><a href=\"{0}\" class=\"back\"></a></li>", container.UrlBuild.GetFolderRequest(container.CurrentList.FolderID.GetValueOrDefault())
                            );
                    }

                    if (container.CurrentListInstance.wim.CanAddNewItem && container.CurrentListInstance.wim.HasListLoad)
                        build2.AppendFormat("<li><a href=\"{0}\" class=\"createPage\">{1}</a></li>", container.UrlBuild.GetListNewRecordRequest(), newRecord);

                    if (buttonList != null)
                    {
                        var selection = (from item in buttonList where item.IconTarget == Sushi.Mediakiwi.Framework.ButtonTarget.TopLeft select item);

                        foreach (Framework.ContentListItem.ButtonAttribute button in selection)
                        {
                            if (button.OpenUrl)
                                build2.AppendFormat("<li><a href=\"{6}\" title=\"{2}\" class=\"{5} {4}\"{7}>{0}</a></li>"
                                    , button.Title
                                    , button.ID
                                    , button.InteractiveHelp
                                    , button.AskConfirmation 
                                        ? " type_confirm" 
                                        : null
                                    , button.IconClassName
                                    , button.OpenInPopupLayer 
                                        ? " openLayerPopUp id_popUpWithIframe" 
                                        : null
                                    , LayerList(button, container)
                                    , TargetInfo(button, container)
                                    );
                            else
                                build2.AppendFormat("<li><input type=\"hidden\" name=\"{1}\"/><a href=\"#\" title=\"{2}\" class=\"{5} {4} postBack{3}\">{0}</a></li>", button.Title, button.ID, button.InteractiveHelp, button.AskConfirmation ? " type_confirm" : null, button.IconClassName, button.OpenInPopupLayer ? " openLayerPopUp id_popUpWithIframe" : null);

                            //build2.AppendFormat("<li><input type=\"hidden\" name=\"{1}\"/><a href=\"#\" title=\"{2}\" class=\"{4} postBack{3}{5}\">{0}</a></li>"
                            //    , button.Title
                            //    , button.ID
                            //    , button.InteractiveHelp
                            //    , button.AskConfirmation ? " type_confirm" : null
                            //    , button.IconClassName
                            //    , button.OpenInPopupLayer ? " openLayerPopUp id_popUpWithIframe" : null
                            //    );
                        }

                        selection = (from item in buttonList where item.IconTarget == Sushi.Mediakiwi.Framework.ButtonTarget.TopRight select item);
                        foreach (Framework.ContentListItem.ButtonAttribute button in selection)
                            build2.AppendFormat("<li class=\"right\"><input type=\"hidden\" name=\"{1}\"/><a href=\"#\" title=\"{2}\" class=\"{4} postBack{3}\">{0}</a></li>", button.Title, button.ID, button.InteractiveHelp, button.AskConfirmation ? " type_confirm" : null, button.IconClassName);


                        //var selection = (from item in buttonList where item.IconTarget == Sushi.Mediakiwi.Framework.ButtonTarget.TopLeft select item);

                        //foreach (Framework.ContentListItem.ButtonAttribute button in selection)
                        //    build2.AppendFormat("<li><input type=\"hidden\" name=\"{1}\"/><a href=\"#\" title=\"{2}\" class=\"{4} postBack{3}\">{0}</a></li>", button.Title, button.ID, button.InteractiveHelp, button.AskConfirmation ? " type_confirm" : null, button.IconClassName);

                        //selection = (from item in buttonList where item.IconTarget == Sushi.Mediakiwi.Framework.ButtonTarget.TopRight select item);
                        //foreach (Framework.ContentListItem.ButtonAttribute button in selection)
                        //    build2.AppendFormat("<li class=\"right\"><input type=\"hidden\" name=\"{1}\"/><a href=\"#\" title=\"{2}\" class=\"{4} postBack{3}\">{0}</a></li>", button.Title, button.ID, button.InteractiveHelp, button.AskConfirmation ? " type_confirm" : null, button.IconClassName);
                    }

                    if (build2.Length == 0)
                        className = " left";


                    if (container.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.Undefined && container.CurrentListInstance.wim.CurrentApplicationUserRole.CanChangeList)
                        build2.AppendFormat("<li class=\"right\"><a href=\"{0}\" class=\"properties\">{1}</a></li>", container.UrlBuild.GetListPropertiesRequest()
                            , Resource.ResourceManager.GetString("properties", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                            );

                    if (!string.IsNullOrEmpty(container.CurrentListInstance.wim.m_sortOrderSqlTable))
                    {
                        if (container.CurrentListInstance.wim.HasSortOrder)
                        {
                            if (container.IsSortorderOn)
                                build2.AppendFormat("<li class=\"right\"><input type=\"hidden\" name=\"sortOrder\" value=\"1\"/><a href=\"{0}\" class=\"sortOrder\">{1}</a></li>", container.GetSafeUrl()
                                    , Resource.ResourceManager.GetString("turn_sort_order_off", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                                    );
                            else
                                build2.AppendFormat("<li class=\"right\"><input type=\"hidden\" name=\"sortOrder\"/><a href=\"{0}\" class=\"sortOrder postBack\">{1}</a></li>", "#"
                                    , Resource.ResourceManager.GetString("turn_sort_order_on", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                                    );
                        }
                    }
                    if (container.CurrentListInstance.wim.HasSubscribeOption)
                    {
                        build2.AppendFormat("<li class=\"right\"><a href=\"{0}\" class=\"subscribe\">{1}</a></li>", container.UrlBuild.GetSubscribeRequest()
                            , Resource.ResourceManager.GetString("subscribe_to_list", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                            );
                    }

                    if (!container.CurrentListInstance.wim.HideExportOptions
                        && (container.CurrentListInstance.wim.ListData != null || container.CurrentListInstance.wim.AppliedSearchGridItem != null))
                    {
//                        if (container.CurrentListInstance.wim.HasExportOptionPDF && container.CurrentListInstance.wim.HasExportOptionXLS)
//                        {
//                            build2.AppendFormat(@"<li class=""right""><a href=""#"" class=""xport"">{0}</a>
//    <ul>
//        <li><input type=""hidden"" name=""export_xls""/><a href=""#"" class=""exportToXls postBack"">{1}</a></li>
//        <li><input type=""hidden"" name=""export_pdf""/><a href=""#"" class=""exportToPdf postBack"">{2}</a></li>
//    </ul>				    
//</li>"
//                                , Resource.ResourceManager.GetString("export_data", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
//                                , Resource.ResourceManager.GetString("export_xls", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
//                                , Resource.ResourceManager.GetString("export_pdf", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
//                                );
//                        }
//                        else
//                        {
                           //if (container.CurrentListInstance.wim.HasExportOptionPDF)
//                                build2.AppendFormat(@"<li class=""right""><input type=""hidden"" name=""export_pdf""/><a href=""#"" class=""exportToPdf postBack"">{0}</a></li>"
//                                    , Resource.ResourceManager.GetString("export_pdf", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
//                                    );
//                            else 
                                if (container.CurrentListInstance.wim.HasExportOptionXLS)
                                build2.AppendFormat(@"<li class=""right""><input type=""hidden"" name=""export_xls""/><a href=""#"" class=""exportToXls postBack"">{0}</a></li>"
                                    , Resource.ResourceManager.GetString("export_xls", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                                    );
                        //}
                    }
                }
                else if (isTextMode)
                {
                    if (CommonConfiguration.NEW_NAVIGATION && container.OpenInFrame == 0)
                    {
                        build2.AppendFormat("<li><a href=\"{0}\" class=\"back\"></a></li>", container.UrlBuild.GetFolderRequest(container.CurrentList.FolderID.GetValueOrDefault())
                            );
                    }
        

                    if (
                        container.CurrentListInstance.wim.HasListSave 
                        && container.CurrentListInstance.wim.CurrentList.Data["wim_CanSave"].ParseBoolean(true)
                        && !container.CurrentListInstance.wim.IsSubSelectMode
                        && !container.CurrentListInstance.wim.HideCreateNew
                        && !container.CurrentListInstance.wim.HideEditOption
                        )
                    {
                        build2.AppendFormat("<li><input type=\"hidden\" name=\"state\"/><a href=\"#\" class=\"edit postBack\">{0}</a></li>"
                            , Resource.ResourceManager.GetString("edit", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                            );
                    }
                    else
                        className = " left";


                    if (buttonList != null)
                    {
                        var selection = (from item in buttonList where item.IconTarget == Sushi.Mediakiwi.Framework.ButtonTarget.TopLeft select item);

                        foreach (Framework.ContentListItem.ButtonAttribute button in selection)
                        {
                            if (button.OpenUrl)
                                build2.AppendFormat("<li><a href=\"{6}\" title=\"{2}\" class=\"{5} {4}\"{7}>{0}</a></li>", button.Title, button.ID, button.InteractiveHelp, button.AskConfirmation ? " type_confirm" : null, button.IconClassName, button.OpenInPopupLayer ? " openLayerPopUp id_popUpWithIframe" : null, LayerList(button, container), TargetInfo(button, container));
                            else
                                build2.AppendFormat("<li><input type=\"hidden\" name=\"{1}\"/><a href=\"#\" title=\"{2}\" class=\"{5} {4} postBack{3}\">{0}</a></li>", button.Title, button.ID, button.InteractiveHelp, button.AskConfirmation ? " type_confirm" : null, button.IconClassName, button.OpenInPopupLayer ? " openLayerPopUp id_popUpWithIframe" : null);
                        }

                        selection = (from item in buttonList where item.IconTarget == Sushi.Mediakiwi.Framework.ButtonTarget.TopRight select item);
                        foreach (Framework.ContentListItem.ButtonAttribute button in selection)
                        {
                            if (button.OpenUrl)
                                build2.AppendFormat("<li class=\"right\"><a href=\"{6}\" title=\"{2}\" class=\"{5} {4}\"{7}>{0}</a></li>", button.Title, button.ID, button.InteractiveHelp, button.AskConfirmation ? " type_confirm" : null, button.IconClassName, button.OpenInPopupLayer ? " openLayerPopUp id_popUpWithIframe" : null, LayerList(button, container), TargetInfo(button, container));
                            else
                                build2.AppendFormat("<li class=\"right\"><input type=\"hidden\" name=\"{1}\"/><a href=\"#\" title=\"{2}\" class=\"{4} postBack{3}\">{0}</a></li>", button.Title, button.ID, button.InteractiveHelp, button.AskConfirmation ? " type_confirm" : null, button.IconClassName);
                        }

                        //if (buttonList.Length > 1)
                        //    build2.AppendFormat("<li><a href=\"#\" class=\"custom\">{1} ({0})</a><ul>", buttonList.Length
                        //        , Resource.ResourceManager.GetString("custom_actions", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                        //        );

                        //foreach (Framework.ContentListItem.ButtonAttribute button in buttonList)
                        //{
                        //    build2.AppendFormat("<li><input type=\"hidden\" name=\"{1}\"/><a href=\"#\" title=\"{2}\" class=\"{4} postBack{3}\">{0}</a></li>", button.Title, button.ID, button.InteractiveHelp, button.AskConfirmation ? " type_confirm" : null, button.IconClassName);
                        //}

                        //if (buttonList.Length > 1)
                        //    build2.Append("</ul></li>");
                    }



                    if (container.CurrentListInstance.wim.HasListDelete)
                    {
                        if (container.CurrentListInstance.wim.CurrentList.Data["wim_CanDelete"].ParseBoolean())
                            build2.AppendFormat("<li class=\"right\"><input type=\"hidden\" name=\"delete\"/><a href=\"#\" class=\"delete postBack type_confirm\">{0}</a></li>"
                                , Resource.ResourceManager.GetString("delete", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                                );
                    }

                    if (
                        !container.CurrentListInstance.wim.HideProperties 
                        && container.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.Undefined 
                        && container.CurrentListInstance.wim.CurrentApplicationUserRole.CanChangeList
                        && !container.CurrentListInstance.wim.IsSubSelectMode
                        )
                        build2.AppendFormat("<li class=\"right\"><a href=\"{0}\" class=\"properties\">{1}</a></li>", container.UrlBuild.GetListPropertiesRequest()
                            , Resource.ResourceManager.GetString("properties", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                            );

                    if (container.CurrentListInstance.wim.CurrentApplicationUserRole.CanPublishPage && container.IsComponent)
                    {
                        var c = Sushi.Mediakiwi.Data.ComponentVersion.SelectOne(container.Item.Value);
                        var v = Sushi.Mediakiwi.Data.Component.SelectOne(c.GUID);

                        //page.Updated
                        if (!(v == null || v.ID == 0))
                        build2.AppendFormat("<li class=\"right\"><input type=\"hidden\" name=\"pageoffline\"/><a href=\"{0}\" class=\"takeOffline postBack\">{1}</a></li>", "#"
                            , Resource.ResourceManager.GetString("page_takeoffline", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                            );

                        if (c.Updated.Ticks != v.Updated.GetValueOrDefault().Ticks)
                        build2.AppendFormat("<li class=\"right\"><input type=\"hidden\" name=\"pagepublish\"/><a href=\"{0}\" class=\"publish postBack\">{1}</a></li>", "#"
                            , Resource.ResourceManager.GetString("page_publish", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                            );
                    }

                    if (!string.IsNullOrEmpty(container.CurrentListInstance.wim.m_sortOrderSqlTable))
                    {
                        if (container.CurrentListInstance.wim.HasSortOrder)
                        {
                            if (container.IsSortorderOn)
                                build2.AppendFormat("<li class=\"right\"><input type=\"hidden\" name=\"sortOrder\" value=\"1\"/><a href=\"{0}\" class=\"sortOrder\">{1}</a></li>", container.GetSafeUrl()
                                    , Resource.ResourceManager.GetString("turn_sort_order_off", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                                    );
                            else
                                build2.AppendFormat("<li class=\"right\"><input type=\"hidden\" name=\"sortOrder\"/><a href=\"{0}\" class=\"sortOrder postBack\">{1}</a></li>", "#"
                                    , Resource.ResourceManager.GetString("turn_sort_order_on", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                                    );
                        }
                    }

                    //  Web content publication
                    if (container.CurrentListInstance.wim.HasPublishOption)
                    {
                        build2.AppendFormat("<li class=\"right\"><a id=\"PageContentPublication\" href=\"{0}\" class=\"publish _PostBack\">{1}</a></li>", "#"
                            , Resource.ResourceManager.GetString("page_publish", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                            );
                    }

                    if (!container.CurrentListInstance.wim.HideExportOptions
                        && (container.CurrentListInstance.wim.ListData != null || container.CurrentListInstance.wim.AppliedSearchGridItem != null))
                    {
//                        if (container.CurrentListInstance.wim.HasExportOptionPDF && container.CurrentListInstance.wim.HasExportOptionXLS)
//                        {
//                            build2.AppendFormat(@"<li class=""right""><a href=""#"" class=""xport"">{0}</a>
//    <ul>
//        <li><input type=""hidden"" name=""export_xls""/><a href=""#"" class=""exportToXls postBack"">{1}</a></li>
//        <li><input type=""hidden"" name=""export_pdf""/><a href=""#"" class=""exportToPdf postBack"">{2}</a></li>
//    </ul>				    
//</li>"
//                                , Resource.ResourceManager.GetString("export_data", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
//                                , Resource.ResourceManager.GetString("export_xls", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
//                                , Resource.ResourceManager.GetString("export_pdf", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
//                                );
//                        }
//                        else
//                        {
//                            if (container.CurrentListInstance.wim.HasExportOptionPDF)
//                                build2.AppendFormat(@"<li class=""right""><input type=""hidden"" name=""export_pdf""/><a href=""#"" class=""exportToPdf postBack"">{0}</a></li>"
//                                    , Resource.ResourceManager.GetString("export_pdf", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
//                                    );
//                            else 
                                if (container.CurrentListInstance.wim.HasExportOptionXLS)
                                build2.AppendFormat(@"<li class=""right""><input type=""hidden"" name=""export_xls""/><a href=""#"" class=""exportToXls postBack"">{0}</a></li>"
                                    , Resource.ResourceManager.GetString("export_xls", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                                    );
                        //}
                    }

                }
                else if (isEditMode)
                {
                    if (CommonConfiguration.NEW_NAVIGATION && container.OpenInFrame == 0)
                    {
                        build2.AppendFormat("<li><a href=\"{0}\" class=\"back\"></a></li>", container.UrlBuild.GetFolderRequest(container.CurrentList.FolderID.GetValueOrDefault())
                            );
                    }

                    if (container.CurrentListInstance.wim.CanAddNewItem 
                        && container.CurrentListInstance.wim.HasListLoad 
                        && !container.CurrentListInstance.wim.CanContainSingleInstancePerDefinedList
                        && !container.CurrentListInstance.wim.IsSubSelectMode
                        && !container.CurrentListInstance.wim.HideCreateNew
                        )
                        build2.AppendFormat("<li><a href=\"{0}\" class=\"createPage\">{1}</a></li>", container.UrlBuild.GetListNewRecordRequest(), newRecord);
                    else
                        className = " left";


                    bool check1 = (!container.CurrentListInstance.wim.IsSubSelectMode || (container.Item.GetValueOrDefault() > 0 && container.CurrentListInstance.wim.HasListSave));
                    bool check2 = (container.CurrentListInstance.wim.CanAddNewItem || container.Item.GetValueOrDefault() > 0 || container.CurrentListInstance.wim.CurrentList.IsSingleInstance);

                    //bool check3 = (container.CurrentListInstance.wim.CanAddNewItem && container.CurrentListInstance.HasListSave);

                    if (
                        check1 && check2
                        )
                    {
                        if (!container.CurrentListInstance.wim.HideSaveButtons && container.CurrentListInstance.wim.CurrentList.Data["wim_CanSave"].ParseBoolean(true))
                        {
                            build2.AppendFormat("<li><input type=\"hidden\" name=\"save1\"/><input type=\"hidden\" name=\"state1\" value=\"2\" /><a href=\"#\" class=\"save postBack\">{0}</a></li>", saveRecord);
                        }
                    }

                    if (buttonList != null)
                    {
                        var selection = (from item in buttonList where item.IconTarget == Sushi.Mediakiwi.Framework.ButtonTarget.TopLeft select item);

                        foreach (Framework.ContentListItem.ButtonAttribute button in selection)
                        {
                            if (button.OpenUrl)
                                build2.AppendFormat("<li><a href=\"{6}\" title=\"{2}\" class=\"{5} {4}\"{7}>{0}</a></li>", button.Title, button.ID, button.InteractiveHelp, button.AskConfirmation ? " type_confirm" : null, button.IconClassName, button.OpenInPopupLayer ? " openLayerPopUp id_popUpWithIframe" : null, LayerList(button, container), TargetInfo(button, container));
                            else
                                build2.AppendFormat("<li><input type=\"hidden\" name=\"{1}\"/><a href=\"#\" title=\"{2}\" class=\"{4} postBack{3}{5}\">{0}</a></li>", button.Title, button.ID, button.InteractiveHelp, button.AskConfirmation ? " type_confirm" : null, button.IconClassName, button.OpenInPopupLayer ? " openLayerPopUp id_popUpWithIframe" : null);
                        }

                        selection = (from item in buttonList where item.IconTarget == Sushi.Mediakiwi.Framework.ButtonTarget.TopRight select item);
                        foreach (Framework.ContentListItem.ButtonAttribute button in selection)
                        {
                            if (button.OpenUrl)
                                build2.AppendFormat("<li class=\"right\"><a href=\"{6}\" title=\"{2}\" class=\"{5} {4}\"{7}>{0}</a></li>", button.Title, button.ID, button.InteractiveHelp, button.AskConfirmation ? " type_confirm" : null, button.IconClassName, button.OpenInPopupLayer ? " openLayerPopUp id_popUpWithIframe" : null, LayerList(button, container), TargetInfo(button, container));
                            else
                            build2.AppendFormat("<li class=\"right\"><input type=\"hidden\" name=\"{1}\"/><a href=\"#\" title=\"{2}\" class=\"{4} postBack{3}{5}\">{0}</a></li>", button.Title, button.ID, button.InteractiveHelp, button.AskConfirmation ? " type_confirm" : null, button.IconClassName, button.OpenInPopupLayer ? " openLayerPopUp id_popUpWithIframe" : null);
                        }
                    }

                    if (container.CurrentListInstance.wim.CanAddNewItem || container.Item.GetValueOrDefault() > 0)
                    {
                        if (container.Item.GetValueOrDefault(0) != 0 && container.CurrentListInstance.wim.OpenInEditMode)
                        {
                            var selectedList = container.CurrentList;
                            if (container.Item.GetValueOrDefault(container.Logic) != container.CurrentList.ID)
                            {
                                selectedList = Data.ComponentList.SelectOne(container.Item.GetValueOrDefault(container.Logic));
                            }

                            if (container.CurrentList.Type != Sushi.Mediakiwi.Data.ComponentListType.ComponentListProperties || selectedList.Type == Sushi.Mediakiwi.Data.ComponentListType.Undefined)
                            {
                                if (container.CurrentListInstance.wim.HasListDelete)
                                {
                                    if (container.CurrentListInstance.wim.CurrentList.Data["wim_CanDelete"].ParseBoolean(true))
                                        build2.AppendFormat("<li class=\"right\"><input type=\"hidden\" name=\"delete\"/><a href=\"{0}\" class=\"delete postBack type_confirm\">{1}</a></li>", "#"
                                            , Resource.ResourceManager.GetString("delete", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                                            );
                                }
                            }
                        }
                    }

                    if (
                        !container.CurrentListInstance.wim.HideProperties 
                        && container.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.Undefined 
                        && container.CurrentListInstance.wim.CurrentApplicationUserRole.CanChangeList
                        && !container.CurrentListInstance.wim.IsSubSelectMode
                        )
                        build2.AppendFormat("<li class=\"right\"><a href=\"{0}\" class=\"properties\">{1}</a></li>", container.UrlBuild.GetListPropertiesRequest()
                            , Resource.ResourceManager.GetString("properties", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                            );

                    if (
                        container.CurrentListInstance.wim.HasSingleItemSortOrder 
                        && !string.IsNullOrEmpty(container.CurrentListInstance.wim.m_sortOrderSqlTable)
                        && !container.CurrentListInstance.wim.IsSubSelectMode
                        )
                    {
                        if (container.CurrentListInstance.wim.HasSortOrder)
                        {
                            if (container.IsSortorderOn)
                                build2.AppendFormat("<li class=\"right\"><input type=\"hidden\" name=\"sortOrder\" value=\"1\"/><a href=\"{0}\" class=\"sortOrder\">{1}</a></li>", container.GetSafeUrl()
                                    , Resource.ResourceManager.GetString("turn_sort_order_off", new CultureInfo(container.CurrentApplicationUser.LanguageCulture)) 
                                    );
                            else
                                build2.AppendFormat("<li class=\"right\"><input type=\"hidden\" name=\"sortOrder\"/><a href=\"{0}\" class=\"sortOrder postBack\">{1}</a></li>", "#"
                                    , Resource.ResourceManager.GetString("turn_sort_order_on", new CultureInfo(container.CurrentApplicationUser.LanguageCulture)) 
                                    );
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
                        build2.AppendFormat("<li><a href=\"{0}\" class=\"createPage\">{1}</a></li>", container.UrlBuild.GetGalleryNewAssetRequestInLayer()
                            , Resource.ResourceManager.GetString("new_asset", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                            );
                    }
                    else
                    {
                        build2.AppendFormat("<li><a href=\"{0}\" class=\"createPage\">{1}</a></li>", container.UrlBuild.GetGalleryNewAssetRequest()
                            , Resource.ResourceManager.GetString("new_asset", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                            );
                    }

                    if (container.CurrentApplicationUser.ShowDetailView)
                        build2.AppendFormat("<li class=\"right\"><input type=\"hidden\" name=\"thumbview\"/><a href=\"{0}\" class=\"thumbview postBack\">{1}</a></li>", "#"
                            , Resource.ResourceManager.GetString("thumbnail_view", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                            );
                    else
                        build2.AppendFormat("<li class=\"right\"><input type=\"hidden\" name=\"detailview\"/><a href=\"{0}\" class=\"properties postBack\">{1}</a></li>", "#"
                            , Resource.ResourceManager.GetString("detail_view", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                            );

                    if (container.View != 1 && container.CurrentListInstance.wim.CurrentApplicationUserRole.CanCreateList)
                    {
                        build2.AppendFormat(@"<li class=""right""><a href=""#"" class=""folder"">{1}</a>
	<ul>
		<li><a href=""{0}"" class=""createFolder"">{2}</a></li>
	</ul>				    
</li>", container.UrlBuild.GetNewGalleryRequest()
      , Resource.ResourceManager.GetString("folder_options", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
      , Resource.ResourceManager.GetString("folder_new", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
        
      
      );
                    }
                }
                else if (isTextMode)
                {
                    if (container.CurrentListInstance.wim.HasListSave && !container.CurrentListInstance.wim.HideEditOption)
                    {
                        build2.AppendFormat("<li><input type=\"hidden\" name=\"state\"/><a href=\"#\" class=\"edit postBack\">{0}</a></li>",
                            Resource.ResourceManager.GetString("edit", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                            );
                    }


                    Sushi.Mediakiwi.Data.Asset asset = Sushi.Mediakiwi.Data.Asset.SelectOne(container.Item.GetValueOrDefault());
                    if (asset.Exists)
                    {
                        build2.AppendFormat("<li><a href=\"{1}\" class=\"attachment\">{0}</a></li>"
                            , Resource.ResourceManager.GetString("download", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                            , asset.DownloadFullUrl
                            );
                    }

                    if (buttonList != null)
                    {
                        if (buttonList.Length > 1)
                            build2.AppendFormat("<li><a href=\"#\" class=\"custom\">{1} ({0})</a><ul>", buttonList.Length
                                , Resource.ResourceManager.GetString("custom_actions", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                                );

                        foreach (Framework.ContentListItem.ButtonAttribute button in buttonList)
                        {
                            build2.AppendFormat("<li><input type=\"hidden\" name=\"{1}\"/><a href=\"#\" title=\"{2}\" class=\"custom postBack{3}\">{0}</a></li>", button.Title, button.ID, button.InteractiveHelp, button.AskConfirmation ? " type_confirm" : null);
                        }

                        if (buttonList.Length > 1)
                            build2.Append("</ul></li>");
                    }

                    if (container.CurrentListInstance.wim.HasListDelete)
                    {
                        build2.AppendFormat("<li class=\"right\"><input type=\"hidden\" name=\"delete\"/><a href=\"#\" class=\"delete postBack type_confirm\">{0}</a></li>"
                            , Resource.ResourceManager.GetString("delete", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                            );
                    }
                }
                else if (isEditMode)
                {
                    if (!container.CurrentListInstance.wim.HideSaveButtons && (container.CurrentListInstance.wim.CanAddNewItem || container.Item.GetValueOrDefault() > 0))
                    {

                        build2.AppendFormat("<li><input type=\"hidden\" name=\"save1\"/><input type=\"hidden\" name=\"state1\" value=\"2\" /><a href=\"#\" class=\"save postBack\">{0}</a></li>", saveRecord);
                        
                        //build2.Append("<li><input type=\"hidden\" name=\"save\"/><input type=\"hidden\" name=\"state\" value=\"2\" /><a href=\"#\" class=\"save postBack\">Save</a></li>");

                        //if (container.CurrentListInstance.wim.CanSaveAndAddNew && !container.CurrentListInstance.wim.CanContainSingleInstancePerDefinedList)
                        //    build2.Append("<li class=\"right\"><input type=\"hidden\" name=\"saveNew\"/><a href=\"#\" class=\"save postBack\">Save and new record</a></li>");
                    }

                    if (container.Item.HasValue)
                    {
                        Sushi.Mediakiwi.Data.Asset asset = Sushi.Mediakiwi.Data.Asset.SelectOne(container.Item.GetValueOrDefault());
                        if (asset.Exists)
                        {
                            build2.AppendFormat("<li><a href=\"{1}\" class=\"attachment\">{0}</a></li>"
                                , Resource.ResourceManager.GetString("download", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                                , asset.DownloadFullUrl
                                );
                        }
                    }

                    if (buttonList != null)
                    {
                        if (buttonList.Length > 1)
                            build2.AppendFormat("<li><a href=\"#\" class=\"custom\">{1} ({0})</a><ul>", buttonList.Length
                                , Resource.ResourceManager.GetString("custom_actions", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                                );

                        foreach (Framework.ContentListItem.ButtonAttribute button in buttonList)
                        {
                            build2.AppendFormat("<li><input type=\"hidden\" name=\"{1}\"/><a href=\"#\" title=\"{2}\" class=\"custom postBack{3}\">{0}</a></li>", button.Title, button.ID, button.InteractiveHelp, button.AskConfirmation ? " type_confirm" : null);
                        }

                        if (buttonList.Length > 1)
                            build2.Append("</ul></li>");
                    }
                    else
                        className = " left";

                    if (container.CurrentListInstance.wim.HasListDelete && container.Item.GetValueOrDefault() > 0)
                    {
                        build2.AppendFormat("<li class=\"right\"><input type=\"hidden\" name=\"delete\"/><a href=\"#\" class=\"delete postBack type_confirm\">{0}</a></li>"
                            , Resource.ResourceManager.GetString("delete", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                            );
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
                        build2.AppendFormat("<li><a href=\"{0}\" class=\"createPage\">{1}</a></li>", container.UrlBuild.GetNewPageRequest()
                            , Resource.ResourceManager.GetString("page_new", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                            );

                    if (container.CurrentApplicationUser.ShowDetailView)
                        build2.AppendFormat("<li class=\"right\"><input type=\"hidden\" name=\"thumbview\"/><a href=\"{0}\" class=\"thumbview postBack\">{1}</a></li>", "#"
                            , Resource.ResourceManager.GetString("thumbnail_view", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                            );
                    else
                        build2.AppendFormat("<li class=\"right\"><input type=\"hidden\" name=\"detailview\"/><a href=\"{0}\" class=\"properties postBack\">{1}</a></li>", "#"
                            , Resource.ResourceManager.GetString("detail_view", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                            );

                    if (container.CurrentListInstance.wim.CurrentApplicationUserRole.CanCreateList)
                    {
                        build2.AppendFormat(@"<li class=""right""><a href=""#"" class=""folder"">{2}</a>
	<ul>
		<li><a href=""{0}"" class=""createFolder"">{3}</a></li>
		<li><a href=""{1}"" class=""folderOptions"">{4}</a></li>
	</ul>				    
</li>", container.UrlBuild.GetFolderCreateRequest(), container.UrlBuild.GetFolderOptionsRequest()
      , Resource.ResourceManager.GetString("folder_options", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
      , Resource.ResourceManager.GetString("folder_new", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
      , Resource.ResourceManager.GetString("folder_properties", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
      );
                    }
                }
                else if (isTextMode)
                {
                    Sushi.Mediakiwi.Data.Page page = Sushi.Mediakiwi.Data.Page.SelectOne(container.Item.Value, false);

                    if (container.CurrentListInstance.wim.CurrentApplicationUserRole.CanChangePage)
                    {
                        if (container.Item.GetValueOrDefault() > 0 
                            && container.CurrentListInstance.wim.CurrentApplicationUserRole.CanCreatePage
                            && !container.IsNotFramed
                            )
                            build2.AppendFormat("<li><a href=\"{0}\" class=\"createPage\">{1}</a></li>", container.UrlBuild.GetNewPageRequest()
                                , Resource.ResourceManager.GetString("page_new", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                                );

                        build2.AppendFormat("<li><input type=\"hidden\" name=\"state\"/><a href=\"#\" class=\"edit postBack\">{0}</a></li>"
                            , Resource.ResourceManager.GetString("edit", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                            );
                    }

                    build2.AppendFormat("<li><a href=\"{0}?preview=1\" target=\"preview\" class=\"preview\">{1}</a></li>", page.HRefFull
                        , Resource.ResourceManager.GetString("page_preview", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                        );

                    if (page.MasterID.HasValue && !isEditMode)
                    {
                        if (page.InheritContent)
                        {
                            //  Inherits content, so can localize
                            innerbuild.AppendFormat("<li><input type=\"hidden\" name=\"page.localize\"/><a href=\"{0}\" class=\"more postBack\">{1}</a></li>", "#"
                                , Resource.ResourceManager.GetString("page_localise", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                                );

                        }
                        else
                        {
                            //  Inherits no content, so can unlocalize
                            innerbuild.AppendFormat("<li><input type=\"hidden\" name=\"page.inherit\"/><a href=\"{0}\" class=\"more postBack\">{1}</a></li>", "#"
                                , Resource.ResourceManager.GetString("page_inherit", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                                );

                        }

                        //  Show folder properties
                        if (container.CurrentApplicationUser.ShowTranslationView)
                            innerbuild.AppendFormat("<li><input type=\"hidden\" name=\"page.normal\"/><a href=\"{0}\" class=\"more postBack\">{1}</a></li>", "#"
                                , Resource.ResourceManager.GetString("page_normalmode", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                                );
                        else
                            innerbuild.AppendFormat("<li><input type=\"hidden\" name=\"page.translate\"/><a href=\"{0}\" class=\"more postBack\">{1}</a></li>", "#"
                                , Resource.ResourceManager.GetString("page_translationmode", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                                );

                        innerbuild.AppendFormat("<li><input type=\"hidden\" name=\"page.copy\"/><a href=\"{0}\" class=\"more postBack type_confirm\">{1}</a></li>", "#"
                            , Resource.ResourceManager.GetString("page_pastemode", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                            );
                    }



                    if (!page.IsPublished)
                    {
                        if (container.CurrentListInstance.wim.CurrentApplicationUserRole.CanDeletePage 
                            && !page.MasterID.HasValue
                            && !container.IsNotFramed
                            )
                        {
                            build2.AppendFormat("<li class=\"right\"><input type=\"hidden\" name=\"delete\"/><a href=\"{0}\" class=\"delete postBack type_confirm\">{1}</a></li>", "#"
                                , Resource.ResourceManager.GetString("delete", new CultureInfo(container.CurrentApplicationUser.LanguageCulture)));
                        }
                    }


                    if (innerbuild.Length > 0)
                    {
                        innerbuild.AppendFormat("<li class=\"right\"><a href=\"{0}\" class=\"properties\">{1}</a></li>", container.UrlBuild.GetPagePropertiesRequest()
                                , Resource.ResourceManager.GetString("properties", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                                );

                        build2.AppendFormat(@"<li class=""right""><a href=""#"" class=""menu"">{0}</a>
	<ul>{1}
	</ul>				    
</li>"
                            , Resource.ResourceManager.GetString("page_options", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                            , innerbuild.ToString()
                            );
                    }
                    else
                    {
                        if (container.CurrentListInstance.wim.CurrentApplicationUserRole.CanChangePage
                            && !container.IsNotFramed
                            )
                            build2.AppendFormat("<li class=\"right\"><a href=\"{0}\" class=\"properties\">Properties</a></li>", container.UrlBuild.GetPagePropertiesRequest()
                                , Resource.ResourceManager.GetString("properties", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                                );
                    }

                    if (container.CurrentListInstance.wim.CurrentApplicationUserRole.CanPublishPage)
                    {
                        //page.Updated

                        if (page.Published != DateTime.MinValue && !container.IsNotFramed)
                            build2.AppendFormat("<li class=\"right\"><input type=\"hidden\" name=\"pageoffline\"/><a href=\"{0}\" class=\"takeOffline postBack\">{1}</a></li>", "#"
                                , Resource.ResourceManager.GetString("page_takeoffline", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                                );

                        if (page.Published == DateTime.MinValue || page.Published.Ticks != page.Updated.Ticks)
                            build2.AppendFormat("<li class=\"right\"><input type=\"hidden\" name=\"pagepublish\"/><a href=\"{0}\" class=\"publish postBack\">{1}</a></li>", "#"
                                , Resource.ResourceManager.GetString("page_publish", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                                );
                    }


                }
                else if (isEditMode)
                {
                    if (container.CurrentListInstance.wim.CanAddNewItem || container.Item.GetValueOrDefault() > 0)
                    {
                        build2.AppendFormat("<li><input type=\"hidden\" name=\"save1\"/><input type=\"hidden\" name=\"state1\" value=\"2\" /><a href=\"#\" class=\"save postBack\">{0}</a></li>"
                            , Resource.ResourceManager.GetString("save", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                            );

                        if (container.CurrentListInstance.wim.CanSaveAndAddNew && !container.CurrentListInstance.wim.CanContainSingleInstancePerDefinedList)
                            build2.AppendFormat("<li class=\"right\"><input type=\"hidden\" name=\"saveNew1\"/><a href=\"#\" class=\"save postBack\">{0}</a></li>"
                                , Resource.ResourceManager.GetString("save_and_new", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                                );
                    }

                    if (buttonList != null)
                    {
                        if (buttonList.Length > 1)
                            build2.AppendFormat("<li><a href=\"#\" class=\"custom\">{1} ({0})</a><ul>", buttonList.Length
                                , Resource.ResourceManager.GetString("custom_actions", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                                );

                        foreach (Framework.ContentListItem.ButtonAttribute button in buttonList)
                        {
                            build2.AppendFormat("<li><input type=\"hidden\" name=\"{1}\"/><a href=\"#\" title=\"{2}\" class=\"custom postBack{3}\">{0}</a></li>", button.Title, button.ID, button.InteractiveHelp, button.AskConfirmation  ? " type_confirm" : null);
                        }

                        if (buttonList.Length > 1)
                            build2.Append("</ul></li>");
                    }
                    else
                        className = " left";
                }
            }
            #endregion

            build.AppendFormat("\n<ul id=\"pathNavigation\" class=\"pathNavigation pseudoHover{0}\">{1}</ul>", className, build2.ToString());
            return build.ToString();
        }

        /// <summary>
        /// Breadcrumbs the navigation.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="buttonList">The button list.</param>
        /// <returns></returns>
        internal static string BreadcrumbNavigation(Console container, Framework.ContentListItem.ButtonAttribute[] buttonList)
        {
            bool isNewNavigation = true;

            List<Framework.ContentListItem.ButtonAttribute> top = new List<Sushi.Mediakiwi.Framework.ContentListItem.ButtonAttribute>();

            if (buttonList != null)
            {
                foreach (Framework.ContentListItem.ButtonAttribute item in buttonList)
                    if (item.IconTarget != Sushi.Mediakiwi.Framework.ButtonTarget.BottomRight) top.Add(item);
            }

            if (isNewNavigation)
                return NewTopNavigation(container, top.ToArray());


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
        static void SetPageNavigation(Console container, StringBuilder build, ref string previousUrl, bool isEditMode, bool isTextMode, bool isListMode, bool isBrowseMode)
        {
            StringBuilder build2 = new StringBuilder();

            if (isBrowseMode)
            {
                previousUrl = string.Concat(container.WimPagePath, "?", "folder=", container.CurrentListInstance.wim.CurrentFolder.ParentID.GetValueOrDefault(0));

                var list0 = Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.Folders);
                var list1 = Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.PageProperties);

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
                    build2.AppendFormat("<li><input type=\"hidden\" name=\"thumbview\"/><a href=\"{0}\" class=\"properties postBack\">Thumbnail view</a></li>", "#");
                else
                    build2.AppendFormat("<li><input type=\"hidden\" name=\"detailview\"/><a href=\"{0}\" class=\"properties postBack\">Detail view</a></li>", "#");

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

                var list0 = Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.PageProperties);
                previousUrl = string.Concat(container.WimPagePath, "?", "folder=", container.CurrentListInstance.wim.CurrentFolder.ID);

                //  Edit
                if (container.CurrentListInstance.wim.CurrentApplicationUserRole.CanChangePage)
                    build2.AppendFormat("<li><input type=\"hidden\" name=\"state\"/><a href=\"{0}\" class=\"edit postBack\">Edit</a></li>", "#");
                
                build2.AppendFormat("<li><a href=\"{0}?preview=1\" target=\"preview\" class=\"preview\">Page preview</a></li>", page.HRefFull);

                if (page.MasterID.HasValue && !isEditMode)
                {
                    if (page.InheritContent)
                    {
                        //  Inherits content, so can localize
                        build2.AppendFormat("<li><input type=\"hidden\" name=\"page.localize\"/><a href=\"{0}\" class=\"refresh postBack\">localise page</a></li>", "#");

                    }
                    else
                    {
                        //  Inherits no content, so can unlocalize
                        build2.AppendFormat("<li><input type=\"hidden\" name=\"page.inherit\"/><a href=\"{0}\" class=\"refresh postBack\">inherit content</a></li>", "#");

                    }
                    //  Show folder properties
                    if (container.CurrentApplicationUser.ShowTranslationView)
                        build2.AppendFormat("<li><input type=\"hidden\" name=\"page.normal\"/><a href=\"{0}\" class=\"properties postBack\">Normal mode</a></li>", "#");
                    else
                        build2.AppendFormat("<li><input type=\"hidden\" name=\"page.translate\"/><a href=\"{0}\" class=\"properties postBack\">Translation mode</a></li>", "#");

                    build2.Append("</ul><ul class=\"subNavigation\" class=\"pseudoHover\">");
                }

                if (container.CurrentListInstance.wim.CurrentApplicationUserRole.CanChangePage)
                    build2.AppendFormat("<li><a href=\"{0}\" class=\"properties\">Properties</a></li>", string.Concat(container.WimPagePath, "?", "list=", list0.ID, "&folder=", container.CurrentListInstance.wim.CurrentFolder.ID, "&item=", container.Item.Value));


                if (container.CurrentListInstance.wim.CurrentApplicationUserRole.CanPublishPage)
                {
                    build2.Append("</ul><ul class=\"subNavigation\" class=\"pseudoHover\">");

                    //page.Updated

                    if (page.Published != DateTime.MinValue)
                        build2.AppendFormat("<li><input type=\"hidden\" name=\"pageoffline\"/><a href=\"{0}\" class=\"takeOffline postBack\">Take Offline</a></li>", "#");

                    if (page.Published == DateTime.MinValue || page.Published.Ticks != page.Updated.Ticks)
                        build2.AppendFormat("<li><input type=\"hidden\" name=\"pagepublish\"/><a href=\"{0}\" class=\"publish postBack\">Publish</a></li>", "#");
                }



                if (!page.IsPublished)
                {
                    if (container.CurrentListInstance.wim.CurrentApplicationUserRole.CanDeletePage && !page.MasterID.HasValue)
                    {
                        build2.Append("</ul><ul class=\"subNavigation\" class=\"pseudoHover\">");
                        build2.AppendFormat("<li><input type=\"hidden\" name=\"delete\"/><a href=\"{0}\" class=\"delete postBack type_confirm\">Delete</a></li>", "#");
                    }
                }

                //build2.Append("</ul><ul class=\"subNavigation\" class=\"pseudoHover\">");
                //build2.AppendFormat("<li><input type=\"hidden\" name=\"refresh\"/><a href=\"{0}\" class=\"refresh postBack\">Refresh</a></li>", "#");

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
                build2.AppendFormat("<li><input type=\"hidden\" name=\"save\"/><input type=\"hidden\" name=\"state\" value=\"2\" /><a href=\"{0}\" class=\"edit postBack\">Save</a></li>", "#");

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
        static void SetListNavigation(Console container, StringBuilder build, ref string previousUrl, bool isEditMode, bool isTextMode, bool isListMode, bool isBrowseMode, Framework.ContentListItem.ButtonAttribute[] buttonList)
        {
            StringBuilder build2 = new StringBuilder();

            if (isBrowseMode)
            {
                previousUrl = string.Concat(container.WimPagePath, "?", "folder=", container.CurrentListInstance.wim.CurrentFolder.ParentID.GetValueOrDefault(0));
                var list0 = Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.Folders);
                var list1 = Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.ComponentListInstance);

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
                    build2.AppendFormat("<li><input type=\"hidden\" name=\"thumbview\"/><a href=\"{0}\" class=\"properties postBack\">Thumbnail view</a></li>", "#");
                else
                    build2.AppendFormat("<li><input type=\"hidden\" name=\"detailview\"/><a href=\"{0}\" class=\"properties postBack\">Detail view</a></li>", "#");
            }
            else if (isListMode)
            {
                //  Create new element
                var list0 = Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.ComponentListProperties);

                previousUrl = string.Concat(container.WimPagePath, "?", "folder=", container.CurrentListInstance.wim.CurrentFolder.ID);

                if (container.CurrentListInstance.wim.CanAddNewItem && container.CurrentListInstance.wim.HasListLoad)
                {
                    build2.AppendFormat("<li><a href=\"{0}\" class=\"createPage\">New record</a></li>", string.Concat(container.WimPagePath, "?", container.CurrentListInstance.wim.SearchResultItemPassthroughParameter, "=0"));
                }

                if (container.CurrentListInstance.wim.CurrentApplicationUserRole.CanChangeList)
                {
                    if (container.CurrentList.Type != Sushi.Mediakiwi.Data.ComponentListType.ComponentListProperties)
                    {
                        if (container.CurrentList.Type != Sushi.Mediakiwi.Data.ComponentListType.Folders)
                            build2.AppendFormat("<li><a href=\"{0}\" class=\"properties\">Properties</a></li>", string.Concat(container.WimPagePath, "?", "list=", list0.ID, "&folder=", container.CurrentListInstance.wim.CurrentFolder.ID, "&item=", container.CurrentList.ID));
                    }
                }

                //if (container.CurrentListInstance.wim.HasExportOption)
                //{
                //    build2.Append("</ul><ul class=\"subNavigation\" class=\"pseudoHover\">");
                //    build2.AppendFormat("<li><input type=\"hidden\" name=\"export_xls\"/><a href=\"{0}\" class=\"exportToXls postBack\">Export to XLS</a></li>", "#");
                //    build2.AppendFormat("<li><input type=\"hidden\" name=\"export_pdf\"/><a href=\"{0}\" class=\"exportToPdf postBack\">Export to PDF</a></li>", "#");
                //}
                if (!string.IsNullOrEmpty(container.CurrentListInstance.wim.m_sortOrderSqlTable))
                {
                    if (container.CurrentListInstance.wim.HasSortOrder)
                    {
                        build2.Append("</ul><ul class=\"subNavigation\" class=\"pseudoHover\">");
                        if (container.IsSortorderOn)
                            build2.AppendFormat("<li><input type=\"hidden\" name=\"sortOrder\" value=\"1\"/><a href=\"{0}\" class=\"sortOrder\">Turn sort order off</a></li>", container.GetSafeUrl());
                        else
                            build2.AppendFormat("<li><input type=\"hidden\" name=\"sortOrder\"/><a href=\"{0}\" class=\"sortOrder postBack\">Turn sort order on</a></li>", "#");
                    }
                }


                //  Custom buttons
                if (buttonList != null)
                {
                    build2.Append("</ul><div class=\"hr\"><hr /></div><ul class=\"subNavigation\" class=\"pseudoHover\">");
                    foreach (Framework.ContentListItem.ButtonAttribute button in buttonList)
                    {
                        build2.AppendFormat("<li><input type=\"hidden\" name=\"{1}\"/><a href=\"#\" title=\"{2}\" class=\"custom postBack{3}\">{0}</a></li>", button.Title, button.ID, button.InteractiveHelp, button.AskConfirmation ? " type_confirm" : null);
                    }
                }

            }
            else if (isTextMode)
            {

                var list0 = Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.ComponentListProperties);

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
                        build2.AppendFormat("<li><a href=\"{0}\" class=\"createPage\">New record</a></li>", string.Concat(container.WimPagePath, "?", container.CurrentListInstance.wim.SearchResultItemPassthroughParameter, "=0"));
                    }
                }

                //  Edit
                if (container.CurrentListInstance.wim.HasListSave)
                    build2.AppendFormat("<li><input type=\"hidden\" name=\"state\"/><a href=\"{0}\" class=\"edit postBack\">Edit</a></li>", "#");


                var selectedList = container.CurrentList;
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
                        build2.AppendFormat("<li><input type=\"hidden\" name=\"delete\"/><a href=\"{0}\" class=\"delete postBack type_confirm\">Delete</a></li>", "#");
                    }
                }

                if (container.CurrentListInstance.wim.HasSingleItemSortOrder && !string.IsNullOrEmpty(container.CurrentListInstance.wim.m_sortOrderSqlTable))
                {
                    if (container.CurrentListInstance.wim.HasSortOrder)
                    {
                        build2.Append("</ul><ul class=\"subNavigation\" class=\"pseudoHover\">");
                        if (container.IsSortorderOn)
                            build2.AppendFormat("<li><input type=\"hidden\" name=\"sortOrder\" value=\"1\"/><a href=\"{0}\" class=\"sortOrder\">Turn sort order off</a></li>", container.GetSafeUrl());
                        else
                            build2.AppendFormat("<li><input type=\"hidden\" name=\"sortOrder\"/><a href=\"{0}\" class=\"sortOrder postBack\">Turn sort order on</a></li>", "#");
                    }
                }

                //  Custom buttons
                if (buttonList != null)
                {
                    build2.Append("</ul><div class=\"hr\"><hr /></div><ul class=\"subNavigation\" class=\"pseudoHover\">");
                    foreach (Framework.ContentListItem.ButtonAttribute button in buttonList)
                    {
                        build2.AppendFormat("<li><input type=\"hidden\" name=\"{1}\"/><a href=\"#\" title=\"{2}\" class=\"custom postBack{3}\">{0}</a></li>", button.Title, button.ID, button.InteractiveHelp, button.AskConfirmation ? " type_confirm" : null);
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
                        build2.AppendFormat("<li><a href=\"{0}\" class=\"createPage\">New record</a></li>", string.Concat(container.WimPagePath, "?", container.CurrentListInstance.wim.SearchResultItemPassthroughParameter, "=0"));
                    }
                }

                //  Save
                if (!container.CurrentListInstance.wim.CanContainSingleInstancePerDefinedList && container.CurrentListInstance.wim.CanSaveAndAddNew)
                {
                    build2.AppendFormat("<li><input type=\"hidden\" name=\"saveNew\"/><a href=\"{0}\" class=\"edit postBack\">Save and new record</a></li>", "#");
                    build2.Append("</ul><ul class=\"subNavigation\" class=\"pseudoHover\">");
                }

                if (container.CurrentListInstance.wim.CanAddNewItem || container.Item.GetValueOrDefault() > 0)
                {
                    build2.AppendFormat("<li><input type=\"hidden\" name=\"save\"/><input type=\"hidden\" name=\"state\" value=\"2\" /><a href=\"{0}\" class=\"edit postBack\">Save</a></li>", "#");

                    if (container.Item.GetValueOrDefault(0) != 0 && container.CurrentListInstance.wim.OpenInEditMode)
                    {
                        var selectedList = container.CurrentList;
                        if (container.Item.GetValueOrDefault(container.Logic) != container.CurrentList.ID)
                        {
                            selectedList = Data.ComponentList.SelectOne(container.Item.GetValueOrDefault(container.Logic));
                        }

                        if (container.CurrentList.Type != Sushi.Mediakiwi.Data.ComponentListType.ComponentListProperties || selectedList.Type == Sushi.Mediakiwi.Data.ComponentListType.Undefined)
                        {
                            if (container.CurrentListInstance.wim.HasListDelete)
                            {
                                build2.Append("</ul><ul class=\"subNavigation\" class=\"pseudoHover\">");
                                build2.AppendFormat("<li><input type=\"hidden\" name=\"delete\"/><a href=\"{0}\" class=\"delete postBack type_confirm\">Delete</a></li>", "#");
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
                            build2.AppendFormat("<li><input type=\"hidden\" name=\"sortOrder\" value=\"1\"/><a href=\"{0}\" class=\"sortOrder\">Turn sort order off</a></li>", container.GetSafeUrl());
                        else
                            build2.AppendFormat("<li><input type=\"hidden\" name=\"sortOrder\"/><a href=\"{0}\" class=\"sortOrder postBack\">Turn sort order on</a></li>", "#");
                    }
                }

                //  Custom buttons
                if (buttonList != null)
                {
                    build2.Append("</ul><div class=\"hr\"><hr /></div><ul class=\"subNavigation\" class=\"pseudoHover\">");
                    foreach (Framework.ContentListItem.ButtonAttribute button in buttonList)
                    {
                        build2.AppendFormat("<li><input type=\"hidden\" name=\"{1}\"/><a href=\"#\" title=\"{2}\" class=\"custom postBack{3}\">{0}</a></li>", button.Title, button.ID, button.InteractiveHelp, button.AskConfirmation ? " type_confirm" : null);
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
        static void SetLibraryNavigation(Console container, StringBuilder build, ref string previousUrl, bool isEditMode, bool isTextMode, bool isListMode, bool isBrowseMode)
        {
            if (isBrowseMode)
            {
                previousUrl = string.Concat(container.WimPagePath, "?", "gallery=", container.CurrentListInstance.wim.CurrentFolder.ParentID.GetValueOrDefault(0));

                build.Append("<ul class=\"subNavigation\" class=\"pseudoHover\">");
                var list0 = Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.ComponentListProperties);
                var list1 = Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.Folders);

                previousUrl = string.Concat(container.WimPagePath, "?", "gallery=", container.CurrentListInstance.wim.CurrentFolder.ID);
                //  Create new element

                Sushi.Mediakiwi.Data.Gallery g = Sushi.Mediakiwi.Data.Gallery.SelectOne(container.CurrentListInstance.wim.CurrentFolder.ID);
                
                build.AppendFormat("<li><a href=\"{0}\" class=\"createPage\">New asset</a></li>", string.Concat(container.WimPagePath, "?gallery=", container.CurrentListInstance.wim.CurrentFolder.ID, "&asset=0"));
                build.AppendFormat("<li><a href=\"{0}\" class=\"createFolder\">Create folder</a></li>", string.Concat(container.WimPagePath, "?", "list=", list1.ID, "&gallery=", container.CurrentListInstance.wim.CurrentFolder.ID, "&item=0"));
                
                build.Append("</ul><ul class=\"subNavigation\" class=\"pseudoHover\">");

                if (container.CurrentApplicationUser.ShowDetailView)
                    build.AppendFormat("<li><input type=\"hidden\" name=\"thumbview\"/><a href=\"{0}\" class=\"properties postBack\">Thumbnail view</a></li>", "#");
                else
                    build.AppendFormat("<li><input type=\"hidden\" name=\"detailview\"/><a href=\"{0}\" class=\"properties postBack\">Detail view</a></li>", "#");
            }
            else if (isTextMode)
            {

                previousUrl = string.Concat(container.WimPagePath, "?", "gallery=", container.CurrentListInstance.wim.CurrentFolder.ID);

                if (container.CurrentList.Type != Sushi.Mediakiwi.Data.ComponentListType.ComponentListProperties)
                {
                    build.Append("<ul class=\"subNavigation\" class=\"pseudoHover\">");

                    //  Edit
                    build.AppendFormat("<li><input type=\"hidden\" name=\"state\"/><a href=\"{0}\" class=\"edit postBack\">Edit</a></li>", "#");

                    if (container.CurrentListInstance.wim.HasListDelete)
                    {
                        build.Append("</ul><ul class=\"subNavigation\" class=\"pseudoHover\">");
                        build.AppendFormat("<li><input type=\"hidden\" name=\"delete\"/><a href=\"{0}\" class=\"delete postBack type_confirm\">Delete</a></li>", "#");
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
                build.AppendFormat("<li><input type=\"hidden\" name=\"save\"/><input type=\"hidden\" name=\"state\" value=\"2\" /><a href=\"{0}\" class=\"edit postBack\">Save</a></li>", "#");
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
        static void SetAdminNavigation(Console container, StringBuilder build, ref string previousUrl, bool isEditMode, bool isTextMode, bool isListMode, bool isBrowseMode, Framework.ContentListItem.ButtonAttribute[] buttonList)
        {
            if (isBrowseMode)
            {
                previousUrl = string.Concat(container.WimPagePath, "?", "folder=", container.CurrentListInstance.wim.CurrentFolder.ParentID.GetValueOrDefault(0));
                var list0 = Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.Folders);
                var list1 = Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.PageProperties);

                build.Append("<ul class=\"subNavigation\" class=\"pseudoHover\">");
                if (container.CurrentApplicationUser.ShowDetailView)
                    build.AppendFormat("<li><input type=\"hidden\" name=\"thumbview\"/><a href=\"{0}\" class=\"properties postBack\">Thumbnail view</a></li>", "#");
                else
                    build.AppendFormat("<li><input type=\"hidden\" name=\"detailview\"/><a href=\"{0}\" class=\"properties postBack\">Detail view</a></li>", "#");
            }
            else if (isListMode)
            {
                build.Append("<ul class=\"subNavigation\" class=\"pseudoHover\">");

                var list0 = Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.ComponentListProperties);

                previousUrl = string.Concat(container.WimPagePath, "?", "folder=", container.CurrentListInstance.wim.CurrentFolder.ID);
                //  Create new element
                build.AppendFormat("<li><a href=\"{0}\" class=\"createPage\">New record</a></li>", string.Concat(container.WimPagePath, "?", container.CurrentListInstance.wim.SearchResultItemPassthroughParameter, "=0"));
                build.AppendFormat("<li><a href=\"{0}\" class=\"properties\">Properties</a></li>", string.Concat(container.WimPagePath, "?", "list=", list0.ID, "&folder=", container.CurrentListInstance.wim.CurrentFolder.ID, "&item=", container.CurrentList.ID));

                //if (container.CurrentListInstance.wim.HasExportOption)
                //{
                //    build.Append("</ul><ul class=\"subNavigation\" class=\"pseudoHover\">");
                //    build.AppendFormat("<li><input type=\"hidden\" name=\"export_xls\"/><a href=\"{0}\" class=\"exportToXls postBack\">Export to XLS</a></li>", "#");
                //    build.AppendFormat("<li><input type=\"hidden\" name=\"export_pdf\"/><a href=\"{0}\" class=\"exportToPdf postBack\">Export to PDF</a></li>", "#");
                //}

                //  Custom buttons
                if (buttonList != null)
                {
                    build.Append("</ul><div class=\"hr\"><hr /></div><ul class=\"subNavigation\" class=\"pseudoHover\">");
                    foreach (Framework.ContentListItem.ButtonAttribute button in buttonList)
                    {
                        build.AppendFormat("<li><input type=\"hidden\" name=\"{1}\"/><a href=\"#\" title=\"{2}\" class=\"custom postBack{3}\">{0}</a></li>", button.Title, button.ID, button.InteractiveHelp, button.AskConfirmation ? " type_confirm" : null);
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
                    build.AppendFormat("<li><input type=\"hidden\" name=\"state\"/><a href=\"{0}\" class=\"edit postBack\">Edit</a></li>", "#");

                    if (container.CurrentListInstance.wim.HasListDelete)
                    {
                        build.Append("</ul><ul class=\"subNavigation\" class=\"pseudoHover\">");
                        build.AppendFormat("<li><input type=\"hidden\" name=\"delete\"/><a href=\"{0}\" class=\"delete postBack type_confirm\">Delete</a></li>", "#");
                    }

                    //  Custom buttons
                    if (buttonList != null)
                    {
                        build.Append("</ul><div class=\"hr\"><hr /></div><ul class=\"subNavigation\" class=\"pseudoHover\">");
                        foreach (Framework.ContentListItem.ButtonAttribute button in buttonList)
                        {
                            build.AppendFormat("<li><input type=\"hidden\" name=\"{1}\"/><a href=\"#\" title=\"{2}\" class=\"custom postBack{3}\">{0}</a></li>", button.Title, button.ID, button.InteractiveHelp, button.AskConfirmation ? " type_confirm" : null);
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
                build.AppendFormat("<li><input type=\"hidden\" name=\"save\"/><input type=\"hidden\" name=\"state\" value=\"2\" /><a href=\"{0}\" class=\"edit postBack\">Save</a></li>", "#");

                //  Custom buttons
                if (buttonList != null)
                {
                    build.Append("</ul><div class=\"hr\"><hr /></div><ul class=\"subNavigation\" class=\"pseudoHover\">");
                    foreach (Framework.ContentListItem.ButtonAttribute button in buttonList)
                    {
                        build.AppendFormat("<li><input type=\"hidden\" name=\"{1}\"/><a href=\"#\" title=\"{2}\" class=\"custom postBack{3}\">{0}</a></li>", button.Title, button.ID, button.InteractiveHelp, button.AskConfirmation ? " type_confirm" : null);
                    }
                }
            }
            build.Append("</ul>");
        }

    }
}
