using Sushi.Mediakiwi.Data;
using System.Text.RegularExpressions;

namespace Sushi.Mediakiwi.Framework.Templates
{
    internal class RichLink
    {
        public RichLink(Site site)
        {
            this.Site = site;
        }

        private static Regex m_rex;
        internal static Regex GetCleaner
        {
            get
            {
                if ( m_rex == null )
                    m_rex = new Regex(@"<a href=""link_(?<ID>.*?)("">)(?<TEXT>.*?)(</a>)", RegexOptions.IgnoreCase);
                    //m_rex = new Regex(@"<a.*?link_(?<ID>\d*).*?>", RegexOptions.IgnoreCase);
                return m_rex;
            }
        }

        private static Regex m_rex2;
        internal static Regex GetCleaner2
        {
            get
            {
                if (m_rex2 == null)
                    m_rex2 = new Regex(@"<a(?<ATTRIBUTES>.*?)href=""wim:(?<ID>.*?)("">)(?<TEXT>.*?)(</a>)", RegexOptions.IgnoreCase);
                    //m_rex2 = new Regex(@"<a href=""wim:(?<ID>(.|\n)*)"">(?<TEXT>(.|\n)*)(</a>)", RegexOptions.IgnoreCase);
                    //m_rex2 = new Regex(@"<a.*?wim:(?<TEXT>\d*).*?>", RegexOptions.IgnoreCase);
                return m_rex2;
            }
        }

        internal Site Site { get; set; }

        internal string CleanEmptyLink(Match m)
        {
            string id = m.Groups["ID"].Value;
            string text = m.Groups["TEXT"].Value;
            if (string.IsNullOrEmpty(text)) return string.Empty;
            if (string.IsNullOrEmpty(text.Trim())) return string.Empty;
            return m.Value;
        }

        internal string CleanLinkData(Match m)
        {
            int linkKey;
            Link link;
            string attributes = m.Groups["ATTRIBUTES"].Value;
            string text = m.Groups["TEXT"].Value;
            if (Utility.IsNumeric(m.Groups["ID"].Value, out linkKey))
            {
                
                link = Link.SelectOne(linkKey);
                //
                if (link != null)
                {
                    string target = null;
                    string className = null;
                    if (link.Target == 2) target = " target=\"_blank\"";
                    else if (link.Target == 3)
                        className = " class=\"openPopUpLayer id_popUpWithIframe\"";
                    if (link.Target == 4)
                        target = " target=\"_top\"";

                    if (link.Type == LinkType.InternalPage)
                    {
                        if (this.Site != null)
                        {
                            Page page = Page.SelectOneChild(link.PageID.Value, Site.ID, true);
                            //Sushi.Mediakiwi.Data.Page.SelectOne(link.PageId, true);
                            if (page != null)
                                return string.Format("<a{4}href=\"{0}\"{1}{3}>{2}</a>", page.HRefFull, target, text, className, attributes);
                        }
                    }
                    else if (link.Type == LinkType.ExternalUrl)
                    {
                        if (link.ExternalUrl != null && link.ExternalUrl.Trim().Length > 0)
                        {
                            if (link.ExternalUrl.StartsWith("www"))
                                return string.Format("<a{4}href=\"http://{0}\"{1}{3}>{2}</a>", link.ExternalUrl, target, text, className, attributes);

                            return string.Format("<a{4}href=\"{0}\"{1}{3}>{2}</a>", link.ExternalUrl, target, text, className, attributes);
                        }
                    }
                    else if (link.Type == LinkType.InternalAsset)
                    {
                        Asset asset = Asset.SelectOne(link.AssetID.Value);
                        return string.Format("<a{4}href=\"{0}\"{1}{3}>{2}</a>", asset.DownloadUrl, target, text, className, attributes);
                    }
                }
            }
            else
            {
                string info = m.Groups["ID"].Value;
            }
            return text;
            //return "<a>";
        }
    }
}
