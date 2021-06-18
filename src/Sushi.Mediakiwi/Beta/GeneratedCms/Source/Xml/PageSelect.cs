using System.Text;

namespace Sushi.Mediakiwi.Beta.GeneratedCms.Source.Xml
{
    class PageSelect
    {
        internal static string Get(string repository, string pagePath, int folder, int page, bool wrapInXml)
        {
            StringBuilder build = new StringBuilder();

            if (wrapInXml)
                build.AppendFormat(@"<?xml version=""1.0""?><root>");

            Data.Folder folderBase = Data.Folder.SelectOne(folder);
            if (folderBase.ParentID.HasValue)
                build.AppendFormat(@"<li><a href=""{0}?xml=folder&amp;id={1}&amp;page={3}"" class=""folder link"">{2}</a></li>", pagePath, folderBase.ParentID, "...", page);

            foreach (Data.Folder item in Data.Folder.SelectAllByParent(folder, Sushi.Mediakiwi.Data.FolderType.Page, false))
            {
                build.AppendFormat(@"<li><a href=""{0}?xml=folder&amp;id={1}&amp;page={3}"" class=""folder link"">{2}</a></li>", pagePath, item.ID, item.Name, page);
            }
            foreach (Data.Page item in Data.Page.SelectAll(folder, Sushi.Mediakiwi.Data.PageFolderSortType.Folder, Sushi.Mediakiwi.Data.PageReturnProperySet.All, Sushi.Mediakiwi.Data.PageSortBy.SortOrder, false))
            {
                build.AppendFormat(@"<li><a id=""{1}"" href=""{0}/images/binaryImage_preview_2.png"" class=""file link{3}"">{2}</a></li>", repository, item.ID, item.Name, item.ID == page ? " active" : string.Empty);
            }
            
            if (wrapInXml)
                build.AppendFormat(@"</root>");

            return build.ToString();
        }
    }
}
