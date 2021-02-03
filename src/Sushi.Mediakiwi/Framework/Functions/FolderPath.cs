using Sushi.Mediakiwi.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

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
        /// Updates the complete page path.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="path">The path.</param>
        /// <param name="replacement">The replacement.</param>
        static void UpdateCompletePagePath(int folderID, string path, string replacement)
        {
            foreach (Sushi.Mediakiwi.Data.Page page in Sushi.Mediakiwi.Data.Page.SelectAll(folderID,  Sushi.Mediakiwi.Data.PageFolderSortType.Folder, Sushi.Mediakiwi.Data.PageReturnProperySet.All, Sushi.Mediakiwi.Data.PageSortBy.SortOrder, false))
            {
                page.InternalPath = string.Concat(page.SitePath, page.Folder.CompletePath, page.Name).Replace(" ", replacement);
                page.Save();
            }
        }

        /// <summary>
        /// Updates the complete path off all pages in the portal
        /// </summary>
        public static void UpdateCompletePath()
        {
            string replacement = Data.Environment.Current["SPACE_REPLACEMENT"];

            foreach (var item in SelectAll())
                UpdateCompletePagePath(item.ID, string.Concat(item.DefaultFolder, item.CompletePath), replacement);
        }

        /// <summary>
        /// Updates the complete path.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        public static void UpdateCompletePath(int folderID)
        {
            FolderPath item = FolderPath.SelectOne(folderID);
            if (item.IsNewInstance) return;

            string replacement = Data.Environment.Current["SPACE_REPLACEMENT"];

            UpdateCompletePagePath(item.ID, string.Concat(item.DefaultFolder, item.CompletePath), replacement);
        }
    }
}
