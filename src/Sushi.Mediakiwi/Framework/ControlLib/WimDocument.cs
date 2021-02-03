using System;
using System.Web;
using System.Web.UI;

namespace Sushi.Mediakiwi.Framework.ControlLib
{
    /// <summary>
    /// 
    /// </summary>
    public class WimDocument : Base.ContentInfo
    {
        /// <summary>
        /// CTor
        /// </summary>
        public WimDocument() 
        {
            m_ApplyLinkText = true;
            this.Load += new EventHandler(WimDocument_Load);
        }

        /// <summary>
        /// Handles the Load event of the WimDocument control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void WimDocument_Load(object sender, EventArgs e)
        {
            if (HasReflection)
            {
                if (GetInstance().IsNull)
                {
                    ApplyData(null, null, (int)Sushi.Mediakiwi.Framework.ContentType.Binary_Document);
                }

                if (GetInstance().ParseInt().HasValue)
                {
                    m_Document = Sushi.Mediakiwi.Data.Document.SelectOne(GetInstance().ParseInt().Value);
                    m_IsDirty = false;
                    Set();
                }
            }
        }

        Sushi.Mediakiwi.Data.Document m_Document;
        /// <summary>
        /// Navigation URL
        /// </summary>
        public Sushi.Mediakiwi.Data.Document Document
        {
            get { return m_Document; }
            set {
                m_IsDirty = false;
                m_Document = value; 
            }
        }

        public bool IsEmpty
        {
            get
            {
                if (this.Document == null) return true;
                return false;
            }
        }

        string m_GalleryGuid;
        /// <summary>
        /// Gets or sets the gallery GUID.
        /// </summary>
        /// <value>The gallery GUID.</value>
        public string GalleryGuid
        {
            get { return m_GalleryGuid; }
            set { m_GalleryGuid = value; }
        }

        bool m_ApplyLinkText;
        /// <summary>
        /// Apply format replacement: {0}
        /// </summary>
        public bool ApplyLinkText
        {
            get { return m_ApplyLinkText; }
            set {
                m_IsDirty = false;
                m_ApplyLinkText = value; 
            }
        }

        string m_CssClass;
        /// <summary>
        /// Interactive help text
        /// </summary>
        public string CssClass
        {
            get { return m_CssClass; }
            set {
                m_IsDirty = false;
                m_CssClass = value; 
            }
        }

        string m_Text;
        /// <summary>
        /// The linktext
        /// </summary>
        public string Text
        {
            get { return m_Text; }
            set {
                m_IsDirty = false;
                m_Text = value; 
            }
        }

        string m_Target;
        /// <summary>
        /// Gets or sets the target.
        /// </summary>
        /// <value>The target.</value>
        public string Target
        {
            get { return m_Target; }
            set
            {
                m_IsDirty = false;
                m_Target = value;
            }
        }

        bool m_ApplyDirectDownload;
        /// <summary>
        /// Gets or sets a value indicating whether [apply direct download].
        /// </summary>
        /// <value><c>true</c> if [apply direct download]; otherwise, <c>false</c>.</value>
        public bool ApplyDirectDownload
        {
            get { return m_ApplyDirectDownload; }
            set
            {
                m_IsDirty = false;
                m_ApplyDirectDownload = value;
            }
        }

        bool m_ApplyFormat;
        /// <summary>
        /// Apply format replacement: {0}
        /// </summary>
        public bool ApplyFormat
        {
            get { return m_ApplyFormat; }
            set {
                m_IsDirty = false; 
                m_ApplyFormat = value;
            }
        }

        bool m_IsDirty;
        internal void Set()
        {
            if (m_Document != null)
            {
                if (m_CssClass != null) m_CssClass = string.Format(" class=\"{0}\"", m_CssClass);

                string targetCandidate = null;
                if (m_Target != null) targetCandidate = string.Concat(" target=\"", m_Target, "\"");

                m_candidate = string.Format("<a id=\"{0}\"{3} name=\"{0}\" href=\"{1}\"{5} title=\"{2}\">{4}", this.ID, this.ApplyDirectDownload ? m_Document.Path : m_Document.DownloadUrl, m_Document.Description == null ? m_Document.Title : m_Document.Description, m_CssClass,
                    m_ApplyLinkText ? m_Document.Title : Text, targetCandidate);
            }
            m_IsDirty = false;
        }

        string m_candidate;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {


            if (m_IsDirty)
            {
                m_IsDirty = false;
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

        /// <summary>
        /// 
        /// </summary>
        public override bool Visible
        {
            get
            {
                return !string.IsNullOrEmpty(m_candidate);
            }
            set
            {
                base.Visible = value;
            }
        }
    }
}