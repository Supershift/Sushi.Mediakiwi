using Sushi.Mediakiwi.Data;
using System;
using System.Collections.Generic;

namespace Sushi.Mediakiwi.Framework.Inheritance
{
    /// <summary>
    /// Represents a Folder entity.
    /// </summary>
    public class Folder
    {
        public static void CopyFolder(int folderToCopy, int copyTargetFolder)
        {
            var source = Sushi.Mediakiwi.Data.Folder.SelectOne(folderToCopy);
            var target = Sushi.Mediakiwi.Data.Folder.SelectOne(copyTargetFolder);

            var query = source.CompletePath;
            //  Connect the base page folder
            var folders = Sushi.Mediakiwi.Data.Folder.SelectAll(source.Type, source.SiteID, query, true);
            var pages = Sushi.Mediakiwi.Data.Page.SelectAll(query, true);

            //if (!webFolder.MasterID.HasValue)
            //{
            //    Sushi.Mediakiwi.Data.Folder masterFolder = Sushi.Mediakiwi.Data.Folder.SelectOneBySite(masterSiteID, type);
            //    webFolder.MasterID = masterFolder.ID;
            //    webFolder.Save();
            //}

            //Sushi.Mediakiwi.Data.Folder[] masterFolders = Sushi.Mediakiwi.Data.Folder.SelectAllUninherited(masterSiteID, siteID, (int)type);

            //foreach (Sushi.Mediakiwi.Data.Folder folder in masterFolders)
            //{
            //    SetChildFolder(folder, siteInfo, siteID);
            //}
        }

        /// <summary>
        /// Creates the folder tree.
        /// </summary>
        /// <param name="masterSiteID">The master site ID.</param>
        /// <param name="siteID">The site ID.</param>
        /// <param name="type">The type.</param>
        /// <param name="isCopy">if set to <c>true</c> [is copy].</param>
        public static void CreateFolderTree(int masterSiteID, int siteID, Sushi.Mediakiwi.Data.FolderType type)
        {
            if (siteID == 0)
                return;

            Sushi.Mediakiwi.Data.Site siteInfo = Sushi.Mediakiwi.Data.Site.SelectOne(siteID);

            //  Connect the base page folder
            Sushi.Mediakiwi.Data.Folder webFolder = Sushi.Mediakiwi.Data.Folder.SelectOneBySite(siteID, type);

            if (webFolder == null || webFolder.IsNewInstance)
                return;
            
            if (!webFolder.MasterID.HasValue)
            {
                Sushi.Mediakiwi.Data.Folder masterFolder = Sushi.Mediakiwi.Data.Folder.SelectOneBySite(masterSiteID, type);
                webFolder.MasterID = masterFolder.ID;
                webFolder.Save();
            }

            Sushi.Mediakiwi.Data.Folder[] masterFolders = Sushi.Mediakiwi.Data.Folder.SelectAllUninherited(masterSiteID, siteID, (int)type);

            foreach (Sushi.Mediakiwi.Data.Folder folder in masterFolders)
            {
                SetChildFolder(folder, siteInfo, siteID);
            }
        }

        /// <summary>
        /// Disconnects the folder tree from mater.
        /// </summary>
        /// <param name="siteID">The site ID.</param>
        /// <param name="type">The type.</param>
        internal static void RemoveInheritence(int siteID, Sushi.Mediakiwi.Data.FolderType type)
        {
            var items = Sushi.Mediakiwi.Data.Folder.SelectAll(type, siteID);
            foreach (var item in items)
            {
                item.MasterID = null;
                item.Save();
            }
        }

        /// <summary>
        /// Sets the child folder.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="currentSite">The current site.</param>
        /// <param name="siteID">The site ID.</param>
        /// <param name="isCopy">if set to <c>true</c> [is copy].</param>
        /// <returns></returns>
        static Sushi.Mediakiwi.Data.Folder SetChildFolder(Sushi.Mediakiwi.Data.Folder folder, Sushi.Mediakiwi.Data.Site currentSite, int siteID)
        {
            Sushi.Mediakiwi.Data.Folder childFolder = new Sushi.Mediakiwi.Data.Folder();

            if (folder.Type == Sushi.Mediakiwi.Data.FolderType.Gallery || folder.Type == Sushi.Mediakiwi.Data.FolderType.Administration || folder.Type == Sushi.Mediakiwi.Data.FolderType.Undefined)
                return null;

            //  If this channel has no pages, don't replicate the page folders
            if (folder.Type == Sushi.Mediakiwi.Data.FolderType.Page && !currentSite.HasPages)
                return null;
            //  If this channel has no lists, don't replicate the list folders
            if (folder.Type == Sushi.Mediakiwi.Data.FolderType.List && !currentSite.HasLists)
                return null;

            childFolder = new Sushi.Mediakiwi.Data.Folder();
            Utility.ReflectProperty(folder, childFolder);
            childFolder.ID = 0;
            childFolder.GUID = Guid.NewGuid();
            childFolder.SiteID = siteID;
            childFolder.MasterID = folder.ID;

            if (folder.ParentID.HasValue)
                childFolder.ParentID = Sushi.Mediakiwi.Data.Folder.SelectOne(folder.ParentID.Value, siteID).ID;
            else
                folder.ParentID = null;

            childFolder.Changed = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
            childFolder.Save();

            return childFolder;
        }

        /// <summary>
        /// Creates the folder.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="currentSite">The current site.</param>
        public static void CreateFolder(Sushi.Mediakiwi.Data.Folder folder, Sushi.Mediakiwi.Data.Site currentSite)
        {
            if (!currentSite.HasChildren) return;
            CreateFolder(folder, currentSite, Site.SelectAll());
        }

        /// <summary>
        /// Creates the folder.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="currentSite">The current site.</param>
        /// <param name="sites">The sites.</param>
        static void CreateFolder(Sushi.Mediakiwi.Data.Folder folder, Sushi.Mediakiwi.Data.Site currentSite, List<Site> sites)
        {
            if (!currentSite.HasChildren) return;

            foreach (Sushi.Mediakiwi.Data.Site site in sites)
            {
                if (site.MasterID.GetValueOrDefault() == currentSite.ID)
                {
                    Sushi.Mediakiwi.Data.Folder childFolder = SetChildFolder(folder, currentSite, site.ID);
                    CreateFolder(childFolder, site, sites);
                }
            }
        }
    }
}
