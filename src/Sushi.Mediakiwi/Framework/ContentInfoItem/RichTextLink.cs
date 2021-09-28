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
            if (string.IsNullOrEmpty(candidate))
            {
                return;
            }

            Regex rex = new Regex(@"<a href=""?link_(?<TEXT>\d*).*?>", RegexOptions.IgnoreCase);

            RichTextLink link = new RichTextLink();
            candidate = rex.Replace(candidate, link.CreateTitle);

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
            if (string.IsNullOrEmpty(candidate))
            {
                return;
            }

            Regex rex = new Regex(@"<a href=""?link_(?<TEXT>\d*).*?>", RegexOptions.IgnoreCase);

            RichTextLink link = new RichTextLink();
            candidate = rex.Replace(candidate, link.CreateCopy);

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
                            return $"<a href=\"wim:{link.ID}\">";
                        }
                    }
                    else 
                    {
                        link.ID = 0;
                        link.Save();
                        return $"<a href=\"wim:{link.ID}\">";
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
                        {
                            return $"<a class=\"link\" title=\"{page.HRef}\" href=\"{page.HRef}\" target=\"blank\">";
                        }
                    }
                    else if (link.Type == LinkType.InternalAsset)
                    {
                        Asset asset = Asset.SelectOne(link.AssetID.Value);
                        return $"<a class=\"link\" title=\"{asset.Path}\" href=\"{asset.DownloadUrl}\" target=\"blank\">";
                    }
                    else if (link.Type == LinkType.ExternalUrl && link.ExternalUrl != null && link.ExternalUrl.Trim().Length > 0)
                    {
                        return $"<a class=\"link\" title=\"{link.ExternalUrl}\" href=\"{link.ExternalUrl}\" target=\"blank\">";
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
                return $"<a id=\"link_{linkKey}\">";
            }
            return "<a>";
        }
    }
}
