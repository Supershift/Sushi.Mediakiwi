using Sushi.Mediakiwi.Data;
using System.Text.RegularExpressions;

namespace Sushi.Mediakiwi.Framework.ContentInfoItem
{
    /// <summary>
    /// Represents a RichTextLink entity.
    /// </summary>
    internal class RichTextLink
    {
        /// <summary>
        /// Cleans the specified candidate.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        internal static void CreateLinkPreview(ref string candidate)
        {
            if (string.IsNullOrEmpty(candidate)) return;

            Regex rex = new Regex(@"<a href=""?link_(?<TEXT>\d*).*?>", RegexOptions.IgnoreCase);

            RichTextLink link = new RichTextLink();
            candidate = rex.Replace(candidate, link.CreateTitle);

            //Regex rex2 = new Regex(@"<a.*?wim:(?<TEXT>\d*).*?>", RegexOptions.IgnoreCase);
            Regex rex2 = new Regex(@"<a href=""?wim:(?<TEXT>\d*).*?>", RegexOptions.IgnoreCase);

            candidate = rex2.Replace(candidate, link.CreateTitle);
        }

        static int m_SiteID;
        /// <summary>
        /// Creates the link master copy.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="siteID">The site ID.</param>
        internal static void CreateLinkMasterCopy(ref string candidate, int siteID)
        {
            m_SiteID = siteID;
            if (string.IsNullOrEmpty(candidate)) return;

            Regex rex = new Regex(@"<a href=""?link_(?<TEXT>\d*).*?>", RegexOptions.IgnoreCase);

            RichTextLink link = new RichTextLink();
            candidate = rex.Replace(candidate, link.CreateCopy);

            //Regex rex2 = new Regex(@"<a.*?wim:(?<TEXT>\d*).*?>", RegexOptions.IgnoreCase);
            Regex rex2 = new Regex(@"<a href=""?wim:(?<TEXT>\d*).*?>", RegexOptions.IgnoreCase);

            candidate = rex2.Replace(candidate, link.CreateCopy);
        }

        internal string CreateCopy(Match m)
        {
            int linkKey;
            if (Utility.IsNumeric(m.Groups["TEXT"].Value, out linkKey))
            {
                Link link = Link.SelectOne(linkKey);
                if (link != null)
                {
                    if (link.Type == LinkType.InternalPage)
                    {
                        Page page = Page.SelectOneChild(link.PageID.Value, m_SiteID, false);
                        if (page != null)
                        {
                            link.ID = 0;
                            link.PageID = page.ID;
                            link.Save();
                            return string.Format("<a href=\"wim:{0}\">", link.ID);
                        }
                    }
                    else 
                    {
                        link.ID = 0;
                        link.Save();
                        return string.Format("<a href=\"wim:{0}\">", link.ID);
                    }
                }
            }
            return "<a class=\"link\">";
        }

        internal string CreateTitle(Match m)
        {
            int linkKey;
            if (Utility.IsNumeric(m.Groups["TEXT"].Value, out linkKey))
            {
                Link link = Link.SelectOne(linkKey);
                if (link != null)
                {
                    if (link.Type == LinkType.InternalPage)
                    {
                        Page page = Page.SelectOne(link.PageID.Value, false);
                        if (page != null)
                            return string.Format("<a class=\"link\" title=\"{0}\" href=\"{0}\" target=\"blank\">", page.HRef);
                    }
                    else if (link.Type == LinkType.InternalAsset)
                    {
                        Asset asset = Asset.SelectOne(link.AssetID.Value);
                        return string.Format("<a class=\"link\" title=\"{0}\" href=\"{1}\" target=\"blank\">", asset.Path, asset.DownloadUrl);
                    }
                    else if (link.Type == LinkType.ExternalUrl)
                    {
                        if (link.ExternalUrl != null && link.ExternalUrl.Trim().Length > 0)
                        {
                            return string.Format("<a class=\"link\" title=\"{0}\" href=\"{0}\" target=\"blank\">", link.ExternalUrl);
                        }
                    }
                }
            }
            return "<a class=\"link\">";
        }

        internal string Clean(Match m)
        {

            int linkKey;
            if (Utility.IsNumeric(m.Groups["TEXT"].Value, out linkKey))
            {
                return string.Format("<a id=\"link_{0}\">", linkKey);
            }
            return "<a>";
        }
    }
}
