using System;
using System.Web;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Represents a ImageFile entity.
    /// </summary>
    public class ImageFile
    {
        #region MOVED to EXTENSION / LOGIC

        ///// <summary>
        ///// Apply the image properties to a Image webcontrol (set url, toolTip and width/height)
        ///// </summary>
        ///// <param name="image">The image.</param>
        //public void Apply(System.Web.UI.WebControls.Image image)
        //{
        //    image.ImageUrl = Path;
        //    image.ToolTip = AlternateText;
        //    image.Height = Height;
        //    image.Width = Width;
        //    image.AlternateText = AlternateText;
        //    image.Visible = true;
        //}

        #endregion MOVED to EXTENSION / LOGIC

        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

        string m_Extention;
        /// <summary>
        /// Gets or sets the extention.
        /// </summary>
        /// <value>The extention.</value>
        public string Extention
        {
            get { return m_Extention; }
            set { m_Extention = value; }
        }

        string m_Name;
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        string m_AlternateText;
        /// <summary>
        /// Gets or sets the alternate text.
        /// </summary>
        /// <value>The alternate text.</value>
        public string AlternateText
        {
            get { return m_AlternateText; }
            set { m_AlternateText = value; }
        }

        string m_Path;
        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <value>The path.</value>
        public string Path
        {
            get
            {
                return m_Path;
            }
            set
            {
                m_Path = value;
            }
        }

        int m_Width;
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public int Width
        {
            get { return m_Width; }
            set { m_Width = value; }
        }

        int m_Height;
        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public int Height
        {
            get { return m_Height; }
            set { m_Height = value; }
        }

        int m_Size;
        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public int Size
        {
            get { return m_Size; }
            set { m_Size = value; }
        }

        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard
    }
}
