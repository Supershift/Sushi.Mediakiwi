using System;
using System.Collections.Generic;
using System.Text;

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
            Sushi.Mediakiwi.Data.Page[] masterPages = Sushi.Mediakiwi.Data.Page.SelectAllUninherited(masterSiteID, siteID);

            foreach (Sushi.Mediakiwi.Data.Page page in masterPages)
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
            var items = Sushi.Mediakiwi.Data.Page.SelectAllBySite(siteID);

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
        static Sushi.Mediakiwi.Data.Page SetChildPage(Sushi.Mediakiwi.Data.Page page, int siteID)
        {
            Sushi.Mediakiwi.Data.Page childPage = new Sushi.Mediakiwi.Data.Page();
            Wim.Utility.ReflectProperty(page, childPage);
            childPage.ID = 0;
            childPage.MasterID = page.ID;

            childPage.Created = Data.Common.DatabaseDateTime;
            childPage.FolderID = Sushi.Mediakiwi.Data.Folder.SelectOne(page.FolderID, siteID).ID;

            if (childPage.FolderID == 0)
                throw new Exception(string.Format("Could not find folder {0} for site {1}!", page.FolderID, siteID));

            if (childPage.SubFolderID > 0)
                childPage.SubFolderID = Sushi.Mediakiwi.Data.Folder.SelectOne(page.SubFolderID, siteID).ID;

            childPage.GUID = Guid.NewGuid();
            childPage.InheritContent = true;
            childPage.IsPublished = false;
            childPage.Updated = DateTime.MinValue;
            childPage.Save();
            
            Sushi.Mediakiwi.Framework.Functions.FolderPath.UpdateCompletePath(childPage.FolderID);
            
            return childPage;
        }

        /// <summary>
        /// Creates the page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="currentSite">The current site.</param>
        public static void CreatePage(Sushi.Mediakiwi.Data.Page page, Sushi.Mediakiwi.Data.Site currentSite)
        {
            if (!currentSite.HasChildren) return;
            CreatePage(page, currentSite, Sushi.Mediakiwi.Data.Site.SelectAll());
        }

        /// <summary>
        /// Creates the page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="currentSite">The current site.</param>
        /// <param name="sites">The sites.</param>
        static void CreatePage(Sushi.Mediakiwi.Data.Page page, Sushi.Mediakiwi.Data.Site currentSite, Sushi.Mediakiwi.Data.Site[] sites)
        {
            if (!currentSite.HasChildren) return;

            foreach (Sushi.Mediakiwi.Data.Site site in sites)
            {
                if (!site.HasPages) continue;

                if (site.MasterID.GetValueOrDefault() == currentSite.ID)
                {
                    Sushi.Mediakiwi.Data.Page childPage = SetChildPage(page, site.ID);
                    CreatePage(childPage, site, sites);
                }
            }
        }

        /// <summary>
        /// Moves the page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="currentSite">The current site.</param>
        public static void MovePage(Sushi.Mediakiwi.Data.Page page, Sushi.Mediakiwi.Data.Site currentSite)
        {
            if (!currentSite.HasChildren) return;

            foreach (Sushi.Mediakiwi.Data.Page child in Sushi.Mediakiwi.Data.Page.SelectAllChildren(page.ID))
            {
                child.FolderID = Sushi.Mediakiwi.Data.Folder.SelectOneChild(page.FolderID, child.SiteID).ID;
                child.Name = child.GetPageNameProposal(child.FolderID, child.Name);

                if (child.FolderID > 0)
                    child.Save();
            }
            //MovePage(page, currentSite, Sushi.Mediakiwi.Data.Site.SelectAll());

        }

        ///// <summary>
        ///// Moves the page.
        ///// </summary>
        ///// <param name="page">The page.</param>
        ///// <param name="currentSite">The current site.</param>
        ///// <param name="sites">The sites.</param>
        //public static void MovePage(Sushi.Mediakiwi.Data.Page page, Sushi.Mediakiwi.Data.Site currentSite, Sushi.Mediakiwi.Data.Site[] sites)
        //{
        //    if (!currentSite.HasChildren) return;

            

        //    foreach (Sushi.Mediakiwi.Data.Site site in Sushi.Mediakiwi.Data.Site.SelectAll())
        //    {
        //        if (!site.HasPages) continue;

        //        if (site.MasterID.GetValueOrDefault() == currentSite.ID)
        //        {
        //            Sushi.Mediakiwi.Data.Page candidate = Sushi.Mediakiwi.Data.Page.SelectOneChild(page.ID, site.ID, false);
        //            if (!candidate.IsNewInstance)
        //            {
        //                candidate.FolderID = Sushi.Mediakiwi.Data.Folder.SelectOneChild(page.FolderID, site.ID).ID;
        //                candidate.Save();

        //                MovePage(candidate, site, sites);
        //            }
        //        }
        //    }
        //}
    }
}
