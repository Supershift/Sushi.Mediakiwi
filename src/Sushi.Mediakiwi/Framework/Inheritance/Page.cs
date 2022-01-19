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
        public static async Task CreatePageTreeAsync(int masterSiteID, int siteID)
        {
            var masterPages = await Data.Page.SelectAllUninheritedAsync(masterSiteID, siteID).ConfigureAwait(false);

            foreach (Data.Page page in masterPages)
            {
                await SetChildPageAsync(page, siteID);
            }
        }


        /// <summary>
        /// Disconnects the page tree from master.
        /// </summary>
        /// <param name="siteID">The site ID.</param>
        internal static async Task RemoveInheritenceAsync(int siteID)
        {
            var items = await Data.Page.SelectAllBySiteAsync(siteID);

            foreach (var item in items)
            {
                item.MasterID = null;
                await item.SaveAsync();
            }
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

        /// <summary>
        /// Copies from master.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        public static async Task CopyFromMasterAsync(int pageID)
        {
            Data.Page page = await Data.Page.SelectOneAsync(pageID).ConfigureAwait(false);

            ComponentVersion[] versions = await ComponentVersion.SelectAllAsync(pageID).ConfigureAwait(false);
            ComponentVersion[] masterVersions = await ComponentVersion.SelectAllAsync(page.MasterID.GetValueOrDefault()).ConfigureAwait(false);
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture;

            foreach (ComponentVersion version in versions)
            {
                ComponentVersion master = null;
                foreach (ComponentVersion item in masterVersions)
                {
                    if (item.ID == version.MasterID)
                    {
                        master = item;
                        break;
                    }
                }
                if (master == null)
                {
                    continue;
                }

                Content content = master.GetContent();
                version.Serialized_XML = master.Serialized_XML;

                if (content != null && content.Fields != null)
                {
                    foreach (Field field in content.Fields)
                    {
                        if (string.IsNullOrEmpty(field.Value) || field.Value == "0")
                        {
                            continue;
                        }

                        if (field.Type == (int)ContentType.RichText)
                        {
                            string candidate = field.Value;
                            ContentInfoItem.RichTextLink.CreateLinkMasterCopy(ref candidate, page.SiteID);
                            field.Value = candidate;
                        }
                        else if (field.Type == (int)ContentType.FolderSelect)
                        {
                            Data.Folder folderInstance = await Data.Folder.SelectOneChildAsync(Utility.ConvertToInt(field.Value), page.SiteID).ConfigureAwait(false);
                            field.Value = folderInstance.ID.ToString(culture);
                        }
                        else if (field.Type == (int)ContentType.Hyperlink)
                        {
                            Link link = await Link.SelectOneAsync(Utility.ConvertToInt(field.Value)).ConfigureAwait(false);
                            if (link?.ID > 0)
                            {
                                if (link.Type == LinkType.InternalPage)
                                {
                                    Data.Page pageInstance = await Data.Page.SelectOneChildAsync(link.PageID.Value, page.SiteID, false).ConfigureAwait(false);
                                    if (page != null)
                                    {
                                        link.ID = 0;
                                        link.PageID = pageInstance.ID;
                                        await link.SaveAsync().ConfigureAwait(false);
                                        field.Value = link.ID.ToString(culture);
                                    }
                                }
                                else
                                {
                                    link.ID = 0;
                                    await link.SaveAsync().ConfigureAwait(false);
                                    field.Value = link.ID.ToString(culture);
                                }
                            }
                        }
                        else if (field.Type == (int)ContentType.PageSelect)
                        {
                            Data.Page pageInstance = await Data.Page.SelectOneChildAsync(Utility.ConvertToInt(field.Value), page.SiteID, false).ConfigureAwait(false);
                            field.Value = pageInstance.ID.ToString(culture);
                        }
                    }
                    version.Serialized_XML = Content.GetSerialized(content);
                }
                else
                {
                    version.Serialized_XML = null;
                }

                await version.SaveAsync();
            }
        }
    }
}
