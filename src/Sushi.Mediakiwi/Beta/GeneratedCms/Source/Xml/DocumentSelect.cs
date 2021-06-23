using System.Text;

namespace Sushi.Mediakiwi.Beta.GeneratedCms.Source.Xml
{
    class DocumentSelect
    {
        internal static string Get(string repository, string pagePath, int folder, int page, bool wrapInXml)
        {
            StringBuilder build = new StringBuilder();

            if (wrapInXml)
                build.AppendFormat(@"<?xml version=""1.0""?><root>");

            Data.Gallery folderBase;

            if (folder > 0)
                folderBase = Data.Gallery.SelectOne(folder);
            else
                folderBase = Data.Gallery.SelectOneRoot();

            if (folderBase.ParentID.HasValue)
                build.AppendFormat(@"<li><a href=""{0}?xml=gallery&amp;id={1}&amp;page={3}"" class=""folder link"">{2}</a></li>", pagePath, folderBase.ParentID, "...", page);

            foreach (Data.Gallery item in Data.Gallery.SelectAllByParent(folderBase.ID))
            {
                build.AppendFormat(@"<li><a href=""{0}?xml=gallery&amp;id={1}&amp;page={3}"" class=""folder link"">{2}</a></li>", pagePath, item.ID, item.Name, page);
            }
            foreach (Data.Asset item in Data.Asset.SelectAll(folderBase.ID))
            {
                build.AppendFormat(@"<li><a id=""{1}"" href=""{0}/images/binaryImage_preview_2.png"" class=""file link{3}"">{2}</a></li>", repository, item.ID, item.Title, item.ID == page ? " active" : string.Empty);
            }
            
            if (wrapInXml)
                build.AppendFormat(@"</root>");

            return build.ToString();
        }
    }
}
