using System;
using System.Web;
using System.Web.UI;

namespace Sushi.Mediakiwi.Framework.ControlLib
{
    /// <summary>
    /// 
    /// </summary>
    public class WimLink : Base.ContentInfo
    {
        /// <summary>
        /// CTor
        /// </summary>
        public WimLink() 
        {
            m_ApplyLinkText = true;
            m_Enabled = true;

            this.Init += new EventHandler(WimLink_Init);
            this.Load += new EventHandler(WimLink_Load);
        }

        void WimLink_Init(object sender, EventArgs e)
        {
            //HttpContext.Current.Response.Write("- 5 -");
            if (HasReflection)
            {
                //HttpContext.Current.Response.Write("- 6 -");
                if (GetInstance().IsNull)
                {
                    if (this.IsPagelink)
                        ApplyData(null, null, (int)Sushi.Mediakiwi.Framework.ContentType.PageSelect);
                    else
                        ApplyData(null, null, (int)Sushi.Mediakiwi.Framework.ContentType.Hyperlink);
                }
                else
                {
                    if (GetInstance().ParseInt().HasValue)
                    {
                        if (this.IsPagelink)
                            this.Page = Sushi.Mediakiwi.Data.Page.SelectOne(GetInstance().ParseInt().Value, true);
                        else
                            this.Link = Sushi.Mediakiwi.Data.Link.SelectOne(GetInstance().ParseInt().Value);

                        Set();
                    }
                }
            }
        }

        void WimLink_Load(object sender, EventArgs e)
        {
            //HttpContext.Current.Response.Write("- 3 -");
            //if (HasReflection)
            //{
            //    HttpContext.Current.Response.Write("- 4 -");
            //    if (GetInstance().IsNull)
            //    {
            //        if (this.IsPagelink)
            //            ApplyData(null, null, (int)Sushi.Mediakiwi.Framework.ContentType.PageSelect);
            //        else
            //            ApplyData(null, null, (int)Sushi.Mediakiwi.Framework.ContentType.Hyperlink);
            //    }
            //    else
            //    {
            //        if (GetInstance().ParseInt().HasValue)
            //        {
            //            if (this.IsPagelink)
            //                this.Page = Sushi.Mediakiwi.Data.Page.SelectOne(GetInstance().ParseInt().Value, true);
            //            else
            //                this.Link = Sushi.Mediakiwi.Data.Link.SelectOne(GetInstance().ParseInt().Value);

            //            Set();
            //        }
            //    }
            //}
        }

        Sushi.Mediakiwi.Data.Link m_Link;
        /// <summary>
        /// Navigation URL
        /// </summary>
        public Sushi.Mediakiwi.Data.Link Link
        {
            get { return m_Link; }
            set {
                m_Link = value;
                if (m_Link != null) m_Text = m_Link.Text;
            }
        }

        Sushi.Mediakiwi.Data.Page m_Page;
        /// <summary>
        /// Navigation URL
        /// </summary>
        public new Sushi.Mediakiwi.Data.Page Page
        {
            get { return m_Page; }
            set { m_Page = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        public bool IsEmpty
        {
            get
            {
                if (m_Link != null && m_Link.ID > 0)
                    return false;
                if (m_Page != null && m_Page.ID > 0 && m_Page.IsPublished)
                    return false;
                return true;
            }
        }

        /// <summary>
        /// Does this link point to the current page.
        /// </summary>
        public bool IsCurrentPage
        {
            get {
                if (m_IsPagelink)
                {
                    if (m_Page == null) return false;
                    if (HttpContext.Current.Items["Wim.Page"] != null)
                    {
                        Sushi.Mediakiwi.Data.Page page = (Sushi.Mediakiwi.Data.Page)HttpContext.Current.Items["Wim.Page"];
                        if (page.ID == m_Page.ID) return true;
                    }
                }
                else
                {
                    if (m_Link == null) return false;
                    if (HttpContext.Current.Items["Wim.Page"] != null)
                    {
                        Sushi.Mediakiwi.Data.Page page = (Sushi.Mediakiwi.Data.Page)HttpContext.Current.Items["Wim.Page"];
                        if (m_Link.IsInternal)
                        {
                            if (m_Link.PageID == page.ID) return true;
                        }
                        else
                        {
                            if (m_Link.ExternalUrl == page.HRefFull) return true;
                        }
                    }
                }
                return false;
            }
        }

        bool m_IsPagelink;
        /// <summary>
        /// Is the link an internal page link
        /// </summary>
        public bool IsPagelink
        {
            get { return m_IsPagelink; }
            set {
                m_IsPoluted = true;
                m_IsPagelink = value; }
        }

        bool m_Enabled;
        /// <summary>
        /// Is the link enabled
        /// </summary>
        public bool Enabled
        {
            get { return m_Enabled; }
            set {
                m_IsPoluted = true;
                m_Enabled = value; }
        }

        bool m_ApplyLinkText;
        /// <summary>
        /// Apply format replacement: {0}
        /// </summary>
        public bool ApplyLinkText
        {
            get { return m_ApplyLinkText; }
            set {
                m_IsPoluted = true;
                m_ApplyLinkText = value; 
            }
        }

        string m_CssClassTag;
        string m_CssClass;
        /// <summary>
        /// Interactive help text
        /// </summary>
        public string CssClass
        {
            get { return m_CssClass; }
            set {
                m_IsPoluted = true;
                m_CssClass = value; }
        }

        string m_Href;
        /// <summary>
        /// The URL Href
        /// </summary>
        public string Href
        {
            get { return m_Href; }
            set {
                m_IsPoluted = true;
                m_Href = value; }
        }

        bool m_IsPoluted;

        string m_Text;
        /// <summary>
        /// The linktext
        /// </summary>
        public string Text
        {
            get { return m_Text; }
            set { 
                
                m_Text = value;

                //if (m_Link != null) m_Link.Text = m_Text;
                //if (m_Page != null) m_Page.LinkText = m_Text;
                
                m_IsPoluted = true;
            }
        }

        public string Target { get; set; }

        bool m_ApplyFormat;
        /// <summary>
        /// Apply format replacement: {0}
        /// </summary>
        public bool ApplyFormat
        {
            get { return m_ApplyFormat; }
            set { m_ApplyFormat = value; }
        }

        internal void Set()
        {
            if (m_Link != null)
            {
                string className = m_CssClass;
                string target = null;

                if (m_Link.Target == 2)
                {
                    target = "target=\"_blank\"";
                }
                else if (m_Link.Target == 3)
                {
                    className = string.IsNullOrEmpty(className) ? "openPopUpLayer id_popUpWithIframe" : string.Concat(className, " openPopUpLayer id_popUpWithIframe");
                    target = " target=\"popUpContents\"";
                }
                if (m_Link.Target == 4)
                {
                    target = "target=\"_top\"";
                }

                if (!string.IsNullOrEmpty(Target))
                    target = string.Format("target=\"{0}\"", Target);

                if (string.IsNullOrEmpty(m_Href))
                    m_Href = GetUrl();

                string linktitle = string.IsNullOrEmpty(Title) ? m_Link.Alt : Title;

                if (string.IsNullOrEmpty(this.Text))
                {
                    if (m_Link.IsInternal)
                        this.Text = this.Page.LinkText;
                    else
                        this.Text = this.Link.Text;
                }

                if (!string.IsNullOrEmpty(m_Href))
                {
                    if (className != null) m_CssClassTag = string.Format(" class=\"{0}\"", className);

                    if (Enabled)
                    {
                        m_candidate = string.Format("<a id=\"{0}\"{3} name=\"{0}\" href=\"{1}\"{5}>{4}", this.ID, m_Href, linktitle, m_CssClassTag,
                            m_ApplyLinkText ? this.Text : string.Empty, target);
                    }
                    else
                    {
                        m_candidate = string.Format("<a disabled=\"disabled\" id=\"{0}\"{2} name=\"{0}\">{3}", this.ID, linktitle, m_CssClassTag,
                            m_ApplyLinkText ? this.Text : string.Empty);
                    }

                }
            }
            else if (m_Page != null)
            {
                string target = "";
                if (!string.IsNullOrEmpty(Target))
                    target = string.Format(" target=\"{0}\"", Target);

                if (string.IsNullOrEmpty(m_Href))
                    m_Href = m_Page.HRef;

                if (string.IsNullOrEmpty(this.Text))
                {
                    this.Text = m_Page.LinkText;
                }

                if (!string.IsNullOrEmpty(m_Href))
                {
                    if (m_CssClass != null) m_CssClassTag = string.Format(" class=\"{0}\"", m_CssClass);

                    if (Enabled)
                    {
                        m_candidate = string.Format("<a id=\"{0}\"{3} name=\"{0}\" href=\"{1}\"{5}>{4}"
                            , this.ID
                            , m_Href
                            , string.IsNullOrEmpty(m_Page.Description) ? m_Page.Title : ""
                            , m_CssClassTag
                            , m_ApplyLinkText ? this.Text : string.Empty
                            , target);
                    }
                    else
                    {
                        m_candidate = string.Format("<a disabled=\"disabled\" id=\"{0}\"{2} name=\"{0}\"{4}>{3}"
                            , this.ID
                            , string.IsNullOrEmpty(m_Page.Description) ? m_Page.Title : ""
                            , m_CssClassTag
                            , m_ApplyLinkText ? this.Text : string.Empty
                            , target);
                    }
                }
            }
            m_IsPoluted = false;
        }

        bool? m_isVisibleOverride;
        /// <summary>
        /// 
        /// </summary>
        public override bool Visible
        {
            get
            {
                if (m_isVisibleOverride.HasValue && !m_isVisibleOverride.Value)
                    return false;

                return !string.IsNullOrEmpty(m_candidate);
            }
            set
            {
                m_isVisibleOverride = value;
                base.Visible = value;
            }
        }

        string m_candidate;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            if (m_IsPoluted)
            {
                m_IsPoluted = false;
                Set();
                
            }
            if (string.IsNullOrEmpty(m_candidate))
            {
                return;
            }

            if (m_ApplyFormat)
            {
                FixApplyFormat(this.Controls, m_candidate);
                this.RenderChildren(writer);
            }
            else
            {
                writer.Write(m_candidate);
                this.RenderChildren(writer);
                writer.Write("</a>");
            }
        }

        string GetUrl()
        {
            if (Link.Type == Sushi.Mediakiwi.Data.Link.LinkType.InternalPage)
            {
                if (!m_Link.PageID.HasValue) return null;
                
                Sushi.Mediakiwi.Data.Page page = null;
                if (this.CurrentSite == null) page = Sushi.Mediakiwi.Data.Page.SelectOne(m_Link.PageID.Value);
                else
                    page = Sushi.Mediakiwi.Data.Page.SelectOneChild(m_Link.PageID.Value, this.CurrentSite.ID);

                if (page == null) return null;
                Title = page.LinkText;

                this.Page = page;

                return page.HRef;
            }
            else if (Link.Type == Sushi.Mediakiwi.Data.Link.LinkType.ExternalUrl)
            {
                if (m_Link.ExternalUrl == null || m_Link.ExternalUrl.Trim().Length == 0)
                    return null;

                if (m_Link.ExternalUrl.StartsWith("www"))
                    return string.Concat("http://", m_Link.ExternalUrl);

                return m_Link.ExternalUrl;
            }
            else if (Link.Type == Sushi.Mediakiwi.Data.Link.LinkType.InternalAsset)
            {
                Sushi.Mediakiwi.Data.Asset asset = Sushi.Mediakiwi.Data.Asset.SelectOne(m_Link.AssetID.Value);
                return asset.DownloadUrl;
            }
            else
                return null;
        }
    }
}