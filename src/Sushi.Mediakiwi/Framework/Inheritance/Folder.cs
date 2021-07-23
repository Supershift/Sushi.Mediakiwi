using Sushi.Mediakiwi.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Framework.Inheritance
{
    /// <summary>
    /// Represents a Folder entity.
    /// </summary>
    public class Folder
    {
        public static void CopyFolder(int folderToCopy, int copyTargetFolder)
        {
            var source = Data.Folder.SelectOne(folderToCopy);
            var target = Data.Folder.SelectOne(copyTargetFolder);

            var query = source.CompletePath;
            //  Connect the base page folder
            var folders = Data.Folder.SelectAll(source.Type, source.SiteID, query, true);
            var pages = Data.Page.SelectAll(query, true);

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
        public static void CreateFolderTree(int masterSiteID, int siteID, FolderType type)
        {
            if (siteID == 0)
                return;

            Site siteInfo = Site.SelectOne(siteID);

            //  Connect the base page folder
            Data.Folder webFolder = Data.Folder.SelectOneBySite(siteID, type);

            if (webFolder == null || webFolder.IsNewInstance)
                return;

            if (!webFolder.MasterID.HasValue)
            {
                Data.Folder masterFolder = Data.Folder.SelectOneBySite(masterSiteID, type);
                webFolder.MasterID = masterFolder.ID;
                webFolder.Save();
            }

            Data.Folder[] masterFolders = Data.Folder.SelectAllUninherited(masterSiteID, siteID, (int)type);

            foreach (Data.Folder folder in masterFolders)
            {
                SetChildFolder(folder, siteInfo, siteID);
            }
        }

        /// <summary>
        /// Disconnects the folder tree from mater.
        /// </summary>
        /// <param name="siteID">The site ID.</param>
        /// <param name="type">The type.</param>
        internal static void RemoveInheritence(int siteID, FolderType type)
        {
            var items = Data.Folder.SelectAll(type, siteID);
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
        static Data.Folder SetChildFolder(Data.Folder folder, Site currentSite, int siteID)
        {
            Data.Folder childFolder = new Data.Folder();

            if (folder.Type == FolderType.Gallery || folder.Type == FolderType.Administration || folder.Type == FolderType.Undefined)
                return null;

            //  If this channel has no pages, don't replicate the page folders
            if (folder.Type == FolderType.Page && !currentSite.HasPages)
                return null;
            //  If this channel has no lists, don't replicate the list folders
            if (folder.Type == FolderType.List && !currentSite.HasLists)
                return null;

            childFolder = new Data.Folder();
            Utility.ReflectProperty(folder, childFolder);
            childFolder.ID = 0;
            childFolder.GUID = Guid.NewGuid();
            childFolder.SiteID = siteID;
            childFolder.MasterID = folder.ID;

            if (folder.ParentID.HasValue)
                childFolder.ParentID = Data.Folder.SelectOne(folder.ParentID.Value, siteID).ID;
            else
                folder.ParentID = null;

            childFolder.Changed = Data.Common.DatabaseDateTime;
            childFolder.Save();

            return childFolder;
        }

        /// <summary>
        /// Creates the folder.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="currentSite">The current site.</param>
        public static void CreateFolder(Data.Folder folder, Site currentSite)
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
        static void CreateFolder(Data.Folder folder, Site currentSite, List<Site> sites)
        {
            if (!currentSite.HasChildren) return;

            foreach (Site site in sites)
            {
                if (site.MasterID.GetValueOrDefault() == currentSite.ID)
                {
                    Data.Folder childFolder = SetChildFolder(folder, currentSite, site.ID);
                    CreateFolder(childFolder, site, sites);
                }
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
        static async Task<Data.Folder> SetChildFolderAsync(Data.Folder folder, Site currentSite, int siteID)
        {
            Data.Folder childFolder = new Data.Folder();

            if (folder.Type == FolderType.Gallery || folder.Type == FolderType.Administration || folder.Type == FolderType.Undefined)
            {
                return null;
            }
            //  If this channel has no pages, don't replicate the page folders
            if (folder.Type == FolderType.Page && !currentSite.HasPages)
            {
                return null;
            }
            //  If this channel has no lists, don't replicate the list folders
            if (folder.Type == FolderType.List && !currentSite.HasLists)
            {
                return null;
            }

            Utility.ReflectProperty(folder, childFolder);
            childFolder.ID = 0;
            childFolder.GUID = Guid.NewGuid();
            childFolder.SiteID = siteID;
            childFolder.MasterID = folder.ID;

            if (folder.ParentID.HasValue)
            {
                var parentFolder = await Data.Folder.SelectOneAsync(folder.ParentID.Value, siteID).ConfigureAwait(false);
                childFolder.ParentID = parentFolder.ID;
            }
            else
            {
                folder.ParentID = null;
            }

            childFolder.Changed = Data.Common.DatabaseDateTime;
            await childFolder.SaveAsync().ConfigureAwait(false);

            return childFolder;
        }

        /// <summary>
        /// Creates the folder.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="currentSite">The current site.</param>
        public static async Task CreateFolderAsync(Data.Folder folder, Site currentSite)
        {
            if (currentSite?.HasChildren != true)
            {
                return;
            }

            var allSites = await Site.SelectAllAsync().ConfigureAwait(false);
            await CreateFolderAsync(folder, currentSite, allSites).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates the folder.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="currentSite">The current site.</param>
        /// <param name="sites">The sites.</param>
        static async Task CreateFolderAsync(Data.Folder folder, Site currentSite, List<Site> sites)
        {
            if (currentSite?.HasChildren != true)
            {
                return;
            }

            foreach (Site site in sites)
            {
                if (site.MasterID.GetValueOrDefault() == currentSite.ID)
                {
                    Data.Folder childFolder = await SetChildFolderAsync(folder, currentSite, site.ID).ConfigureAwait(false);
                    await CreateFolderAsync(childFolder, site, sites).ConfigureAwait(false);
                }
            }
        }
    }
}
