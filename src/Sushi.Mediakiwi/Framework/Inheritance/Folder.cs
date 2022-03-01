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
        /// <summary>
        /// Creates the folder tree.
        /// </summary>
        /// <param name="masterSiteID">The master site ID.</param>
        /// <param name="siteID">The site ID.</param>
        /// <param name="type">The type.</param>
        /// <param name="isCopy">if set to <c>true</c> [is copy].</param>
        public static async Task CreateFolderTreeAsync(int masterSiteID, int siteID, FolderType type)
        {
            if (siteID == 0)
            {
                return;
            }

            Site siteInfo = await Site.SelectOneAsync(siteID).ConfigureAwait(false);

            //  Connect the base page folder
            Data.Folder webFolder = await Data.Folder.SelectOneBySiteAsync(siteID, type).ConfigureAwait(false);

            if (webFolder == null || webFolder.IsNewInstance)
            {
                return;
            }

            if (!webFolder.MasterID.HasValue)
            {
                Data.Folder masterFolder = await Data.Folder.SelectOneBySiteAsync(masterSiteID, type).ConfigureAwait(false);
                webFolder.MasterID = masterFolder.ID;
                await webFolder.SaveAsync().ConfigureAwait(false);
            }

            Data.Folder[] masterFolders = await Data.Folder.SelectAllUninheritedAsync(masterSiteID, siteID, (int)type).ConfigureAwait(false);

            foreach (Data.Folder folder in masterFolders)
            {
                await SetChildFolderAsync(folder, siteInfo, siteID).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Disconnects the folder tree from mater.
        /// </summary>
        /// <param name="siteID">The site ID.</param>
        /// <param name="type">The type.</param>
        internal static async Task RemoveInheritenceAsync(int siteID, FolderType type)
        {
            var items = await Data.Folder.SelectAllAsync(type, siteID).ConfigureAwait(false);
            foreach (var item in items)
            {
                item.MasterID = null;
                await item.SaveAsync().ConfigureAwait(false);
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
