using Sushi.Mediakiwi.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Framework.Inheritance
{
    /// <summary>
    /// Represents a Page entity.
    /// </summary>
    public class Page
    {
        /// <summary>
        /// Creates the page tree.
        /// </summary>
        /// <param name="masterSiteID">The master site ID.</param>
        /// <param name="siteID">The site ID.</param>
        public static void CreatePageTree(int masterSiteID, int siteID)
        {
            var masterPages = Data.Page.SelectAllUninherited(masterSiteID, siteID);

            foreach (Data.Page page in masterPages)
            {
                SetChildPage(page, siteID);
            }
        }


        /// <summary>
        /// Disconnects the page tree from master.
        /// </summary>
        /// <param name="siteID">The site ID.</param>
        internal static void RemoveInheritence(int siteID)
        {
            var items = Data.Page.SelectAllBySite(siteID);

            foreach (var item in items)
            {
                item.MasterID = null;
                item.Save();
            }
        }

        /// <summary>
        /// Sets the child page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="siteID">The site ID.</param>
        /// <returns></returns>
        static Data.Page SetChildPage(Data.Page page, int siteID)
        {
            Data.Page childPage = new Data.Page();

            Utility.ReflectProperty(page, childPage);
            childPage.ID = 0;
            childPage.MasterID = page.ID;
            childPage.Created = Data.Common.DatabaseDateTime;
            childPage.FolderID = Data.Folder.SelectOne(page.FolderID, siteID).ID;

            if (childPage.FolderID == 0)
            {
                throw new Exception(string.Format("Could not find folder {0} for site {1}!", page.FolderID, siteID));
            }

            if (childPage.SubFolderID > 0)
            {
                childPage.SubFolderID = Data.Folder.SelectOne(page.SubFolderID, siteID).ID;
            }

            childPage.GUID = Guid.NewGuid();
            childPage.InheritContent = true;
            childPage.IsPublished = false;
            childPage.Updated = DateTime.MinValue;
            childPage.Save();

            Functions.FolderPathLogic.UpdateCompletePath(childPage.FolderID);

            return childPage;
        }


        /// <summary>
        /// Sets the child page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="siteID">The site ID.</param>
        /// <returns></returns>
        static async Task<Data.Page> SetChildPageAsync(Data.Page page, int siteID)
        {
            Data.Page childPage = new Data.Page();

            Utility.ReflectProperty(page, childPage);
            childPage.ID = 0;
            childPage.MasterID = page.ID;
            childPage.Created = Data.Common.DatabaseDateTime;
            childPage.FolderID = (await Data.Folder.SelectOneAsync(page.FolderID, siteID)).ID;

            if (childPage.FolderID == 0)
            {
                throw new Exception(string.Format("Could not find folder {0} for site {1}!", page.FolderID, siteID));
            }

            if (childPage.SubFolderID > 0)
            {
                childPage.SubFolderID = (await Data.Folder.SelectOneAsync(page.SubFolderID, siteID)).ID;
            }

            childPage.GUID = Guid.NewGuid();
            childPage.InheritContent = true;
            childPage.IsPublished = false;
            childPage.Updated = DateTime.MinValue;

            await childPage.SaveAsync();
            await Functions.FolderPathLogic.UpdateCompletePathAsync(childPage.FolderID);

            return childPage;
        }

        /// <summary>
        /// Creates the page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="currentSite">The current site.</param>
        public static void CreatePage(Data.Page page, Site currentSite)
        {
            if (!currentSite.HasChildren)
            {
                return;
            }
            CreatePage(page, currentSite, Site.SelectAll());
        }

        /// <summary>
        /// Creates the page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="currentSite">The current site.</param>
        public static async Task CreatePageAsync(Data.Page page, Site currentSite)
        {
            if (!currentSite.HasChildren)
            {
                return;
            }
            await CreatePageAsync(page, currentSite, await Site.SelectAllAsync());
        }

        /// <summary>
        /// Creates the page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="currentSite">The current site.</param>
        /// <param name="sites">The sites.</param>
        static void CreatePage(Data.Page page, Site currentSite, List<Site> sites)
        {
            if (!currentSite.HasChildren) return;

            foreach (Site site in sites)
            {
                if (!site.HasPages)
                {
                    continue;
                }

                if (site.MasterID.GetValueOrDefault() == currentSite.ID)
                {
                    Data.Page childPage = SetChildPage(page, site.ID);
                    CreatePage(childPage, site, sites);
                }
            }
        }


        /// <summary>
        /// Creates the page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="currentSite">The current site.</param>
        /// <param name="sites">The sites.</param>
        static async Task CreatePageAsync(Data.Page page, Site currentSite, List<Site> sites)
        {
            if (!currentSite.HasChildren)
            {
                return;
            }

            foreach (Site site in sites)
            {
                if (!site.HasPages)
                {
                    continue;
                }

                if (site.MasterID.GetValueOrDefault() == currentSite.ID)
                {
                    Data.Page childPage = await SetChildPageAsync(page, site.ID);
                    await CreatePageAsync(childPage, site, sites);
                }
            }
        }

        /// <summary>
        /// Moves the page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="currentSite">The current site.</param>
        public static void MovePage(Data.Page page, Site currentSite)
        {
            if (!currentSite.HasChildren)
            {
                return;
            }

            foreach (Data.Page child in Data.Page.SelectAllChildren(page.ID))
            {
                child.FolderID = Data.Folder.SelectOneChild(page.FolderID, child.SiteID).ID;
                child.Name = child.GetPageNameProposal(child.FolderID, child.Name);

                if (child.FolderID > 0)
                {
                    child.Save();
                }
            }
        }


        /// <summary>
        /// Moves the page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="currentSite">The current site.</param>
        public static async Task MovePageAsync(Data.Page page, Site currentSite)
        {
            if (!currentSite.HasChildren)
            {
                return;
            }

            foreach (Data.Page child in await Data.Page.SelectAllChildrenAsync(page.ID))
            {
                child.FolderID = (await Data.Folder.SelectOneChildAsync(page.FolderID, child.SiteID)).ID;
                child.Name = await child.GetPageNameProposalAsync(child.FolderID, child.Name);

                if (child.FolderID > 0)
                {
                    await child.SaveAsync();
                }
            }
        }
    }
}
