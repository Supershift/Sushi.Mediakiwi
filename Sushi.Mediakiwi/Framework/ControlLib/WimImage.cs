using System;
using System.ComponentModel;
using System.Data;
using System.Collections;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Sushi.Mediakiwi.Framework.ControlLib
{
    /// <summary>
    /// 
    /// </summary>
    [ToolboxData("<{0}:WimImage runat='server'></{0}:WimImage>")]
    public class WimImage : Base.ContentInfo
    {
        /// <summary>
        /// CTor
        /// </summary>
        public WimImage() {
            this.Load += new EventHandler(WimImage_Load);
        }

        void WimImage_Load(object sender, EventArgs e)
        {
            if (HasReflection)
            {
                if (GetInstance().IsNull)
                {
                    ApplyData(null, null, (int)Sushi.Mediakiwi.Framework.ContentType.Binary_Image);
                }

                if (GetInstance().ParseInt().HasValue)
                {
                    m_Image = Sushi.Mediakiwi.Data.Image.SelectOne(GetInstance().ParseInt().Value);
                    Set();
                }
            }
        }


        Sushi.Mediakiwi.Data.Image m_Image;
        /// <summary>
        /// Navigation URL
        /// </summary>
        public Sushi.Mediakiwi.Data.Image Image
        {
            get { return m_Image; }
            set { m_Image = value; }
        }

        int m_Height;
        /// <summary>
        /// Height
        /// </summary>
        public int Height
        {
            get { return m_Height; }
            set { m_Height = value; }
        }

        int m_Width;
        /// <summary>
        /// Width
        /// </summary>
        public int Width
        {
            get { return m_Width; }
            set { m_Width = value; }
        }

        Sushi.Mediakiwi.Data.ImageFileFormat m_FileFormat;
        /// <summary>
        /// Interactive help text
        /// </summary>
        public Sushi.Mediakiwi.Data.ImageFileFormat FileFormat
        {
            get { return m_FileFormat; }
            set { m_FileFormat = value; }
        }

        string m_CssClass;
        /// <summary>
        /// Interactive help text
        /// </summary>
        public string CssClass
        {
            get { return m_CssClass; }
            set { m_CssClass = value; }
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

        bool m_CanOnlyAdd;
        /// <summary>
        /// Gets or sets a value indicating whether this instance can only add.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can only add; otherwise, <c>false</c>.
        /// </value>
        public bool CanOnlyAdd
        {
            get { return m_CanOnlyAdd; }
            set { m_CanOnlyAdd = value; }
        }

        private string m_Alt;
        /// <summary>
        /// The alt of an image
        /// </summary>
        public string Alt
        {
            get { return m_Alt; }
            set { m_Alt = value; }
        }


        internal void Set()
        {
            if (m_Image != null)
                CalculateSize(m_Image.Width, m_Image.Height, Width, Height);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            if (m_Image != null && !m_Image.IsNewInstance)
            {
                string path = m_Image.Path;
                if (FileFormat != Sushi.Mediakiwi.Data.ImageFileFormat.None)
                {
                    m_Image = m_Image.CreateAttachedImageSize(this.Width, this.Height, this.FileFormat);
                    if (m_Image == null) return;
                    path = m_Image.CdnImagePath;
                }

                string candidate = null;

                //CalculateSize(m_Image.Width, m_Image.Height, Width, Height);

                if (m_CssClass != null) m_CssClass = string.Format(" class=\"{0}\"", m_CssClass);

                candidate = string.Format("<img id=\"{0}\"{3} name=\"{0}\"{4}{5} src=\"{1}\" alt=\"{2}\" />", this.ID, path, ((!String.IsNullOrEmpty(m_Image.Description)) ? m_Image.Description : Alt), m_CssClass
                    , m_Image.Width == 0 ? "" : string.Format(" width=\"{0}\"", m_Image.Width)
                    , m_Image.Height == 0 ? "" : string.Format(" height=\"{0}\"", m_Image.Height)
                    );

                if (ApplyFormat)
                {
                    FixApplyFormat(this.Controls, candidate);
                    this.RenderChildren(writer);
                    //writer.Write(string.Format(InnerText, candidate));
                }
                else
                    writer.Write(candidate);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool Visible
        {
            get
            {
                if (m_Image == null) return false;
                return (base.Visible);
            }
            set
            {
                base.Visible = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        public bool IsEmpty
        {
            get
            {
                if (m_Image != null && m_Image.ID > 0)
                    return false;
                return true;
            }
        }
        bool m_ApplyFormat;
        /// <summary>
        /// Apply format replacement: {0}
        /// </summary>
        public bool ApplyFormat
        {
            get { return m_ApplyFormat; }
            set { m_ApplyFormat = value; }
        }

        void CalculateSize(int adoptedWidth, int adoptedHeight, int width, int height)
        {
            this.Width = adoptedWidth;
            this.Height = adoptedHeight;

            if (width != 0 && height != 0)
            {
                this.Width = width;
                this.Height = height;
                return;
            }

            if (Height != 0 && Width != 0 && width > 0)
            {
                decimal factor = decimal.Divide(width, Width);
                decimal newHeight = decimal.Round(decimal.Multiply(factor, Height), 0);
                this.Height = Convert.ToInt32(newHeight);
                this.Width = width;
                return;
            }
            if (Height != 0 && Width != 0 && height > 0)
            {
                decimal factor = decimal.Divide(height, Height);
                decimal newWidth = decimal.Round(decimal.Multiply(factor, Width), 0);
                this.Width = Convert.ToInt32(newWidth);
                this.Height = height;
            }
        }
    }
}