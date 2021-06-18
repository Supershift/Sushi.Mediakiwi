using Sushi.Mediakiwi.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Framework.Functions
{
    public class FolderPathLogic
    {
        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        static List<FolderPath> SelectAll()
        {
            return FolderPath.SelectAll();
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        static async Task<List<FolderPath>> SelectAllAsync()
        {
            return await FolderPath.SelectAllAsync();
        }


        /// <summary>
        /// Updates the complete page path.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="replacement">The replacement.</param>
        static void UpdateCompletePagePath(int folderID, string replacement)
        {
            foreach (Page page in Page.SelectAll(folderID, PageFolderSortType.Folder, PageReturnProperySet.All, PageSortBy.SortOrder, false))
            {
                page.InternalPath = string.Concat(page.SitePath, page.Folder.CompletePath, page.Name).Replace(" ", replacement);
                page.Save();
            }
        }

        /// <summary>
        /// Updates the complete page path.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="replacement">The replacement.</param>
        static async Task UpdateCompletePagePathAsync(int folderID, string replacement)
        {
            foreach (Page page in await Page.SelectAllAsync(folderID, PageFolderSortType.Folder, PageReturnProperySet.All, PageSortBy.SortOrder, false))
            {
                page.InternalPath = string.Concat(page.SitePath, page.Folder.CompletePath, page.Name).Replace(" ", replacement);
                await page.SaveAsync();
            }
        }

        /// <summary>
        /// Updates the complete path off all pages in the portal
        /// </summary>
        public static void UpdateCompletePath()
        {
            string replacement = Data.Environment.Current["SPACE_REPLACEMENT"];

            foreach (var item in SelectAll())
            {
                UpdateCompletePagePath(item.ID, replacement);
            }
        }

        /// <summary>
        /// Updates the complete path off all pages in the portal
        /// </summary>
        public static async Task UpdateCompletePathAsync()
        {
            string replacement = Data.Environment.Current["SPACE_REPLACEMENT"];

            foreach (var item in await SelectAllAsync())
            {
                await UpdateCompletePagePathAsync(item.ID, replacement);
            }
        }

        /// <summary>
        /// Updates the complete path.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        public static void UpdateCompletePath(int folderID)
        {
            FolderPath item = FolderPath.SelectOne(folderID);
            if (item.IsNewInstance)
            {
                return;
            }

            string replacement = Data.Environment.Current["SPACE_REPLACEMENT"];

            UpdateCompletePagePath(item.ID, replacement);
        }

        /// <summary>
        /// Updates the complete path.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        public static async Task UpdateCompletePathAsync(int folderID)
        {
            FolderPath item = await FolderPath.SelectOneAsync(folderID);
            if (item.IsNewInstance)
            {
                return;
            }

            string replacement = Data.Environment.Current["SPACE_REPLACEMENT"];

            UpdateCompletePagePath(item.ID, replacement);
        }
    }
}
