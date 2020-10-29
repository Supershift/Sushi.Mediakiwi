using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    /// <summary>
    /// 
    /// </summary>
    public class Gallery : BaseImplementation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Gallery"/> class.
        /// </summary>
        public Gallery()
        {
            this.ListSave += new ComponentListEventHandler(Gallery_ListSave);
            this.ListLoad += new ComponentListEventHandler(Gallery_ListLoad);
            this.ListSearch += new ComponentSearchEventHandler(Gallery_ListSearch);
        }

        void Gallery_ListSearch(object sender, ComponentListSearchEventArgs e)
        {
            
            wim.CanAddNewItem = true;
            wim.ListDataColumns.Add("ID", "ID", ListDataColumnType.UniqueIdentifier);
            wim.ListDataColumns.Add("Gallery", "Name", ListDataColumnType.HighlightPresent);
            wim.ListDataColumns.Add("Created", "Created", 100);

            wim.ForceLoad = true;
            Sushi.Mediakiwi.Data.GalleryType type = (Sushi.Mediakiwi.Data.GalleryType)int.Parse(m_SearchFilter);

            wim.ListData = Sushi.Mediakiwi.Data.Gallery.SelectAll(type, false);
        }

        void Gallery_ListLoad(object sender, ComponentListEventArgs e)
        {
            if (e.SelectedKey == 0)
            {
                this.Implement = new Sushi.Mediakiwi.Data.Gallery();
                return;
            }

            this.Implement = Sushi.Mediakiwi.Data.Gallery.SelectOne(e.SelectedKey);
        }

        void Gallery_ListSave(object sender, ComponentListEventArgs e)
        {
            if (e.SelectedKey == 0)
            {
                this.Implement.Name = this.Implement.Name.Trim();
                Sushi.Mediakiwi.Data.Gallery root = Sushi.Mediakiwi.Data.Gallery.SelectOneRoot(this.Implement.Type);
                this.Implement.CompletePath = string.Format("{0}/{1}", root.CompletePath, this.Implement.Name);
                this.Implement.ParentID = root.ID;
            }
            this.Implement.Save();

            if (e.SelectedKey == 0)
            {
                Implement.BaseGalleryID = Implement.ID;
                Implement.Save();
            }

            this.Implement.CreateFolder();

            #region Junk
            //Sushi.Mediakiwi.Data.Gallery gallery;
            //if (e.SelectedKey == 0)
            //{
            //    gallery = new Sushi.Mediakiwi.Data.Gallery();
            //    gallery.Type = (Sushi.Mediakiwi.Data.Gallery.GalleryType)int.Parse(m_GalleryType);
            //}
            //else
            //    gallery = Sushi.Mediakiwi.Data.Gallery.SelectOne(e.SelectedKey);

            //gallery.Name = m_GalleryName;
            //gallery.BackgroundRgb = m_BorderRgb == "255.255.255" ? null : m_BorderRgb;
            
            //if (m_FormatType == 0)
            //    gallery.FormatType = null;
            //else
            //    gallery.FormatType = m_FormatType;

            //if (e.SelectedKey == 0)
            //{
            //    Sushi.Mediakiwi.Data.Gallery root = Sushi.Mediakiwi.Data.Gallery.SelectOne_Root(gallery.Type);
            //    gallery.CompletePath = string.Format("{0}/{1}", root.CompletePath, gallery.Name);
            //    gallery.Parent = root.Id;
            //}
            //else
            //    gallery.BaseGallery = gallery.Id;
            //gallery.Format = m_Format;

            //gallery.Save();

            //if (e.SelectedKey == 0)
            //    gallery.BaseGallery = gallery.Id;
            //gallery.Save();

            //gallery.CreateFolder(Request);
            #endregion
        }

        #region List attributes

        private string m_SearchFilter = "1";
        /// <summary>
        /// Gets or sets the search filter.
        /// </summary>
        /// <value>The search filter.</value>
        [Sushi.Mediakiwi.Framework.ContentListSearchItem.Choice_Dropdown("Type", "TypeCollection", false, true)]
        public string SearchFilter
        {
            get { return m_SearchFilter; }
            set { m_SearchFilter = value; }
        }

        /// <summary>
        /// Gets the type collection.
        /// </summary>
        /// <value>The type collection.</value>
        public ListItemCollection TypeCollection
        {
            get
            {
                ListItemCollection col = new ListItemCollection();
                ListItem li = new ListItem("Images", "1");
                li.Selected = true;
                col.Add(li);
                col.Add(new ListItem("Documents", "2"));
                return col;
            }
        }

        private ListItemCollection m_FormatCollection;
        /// <summary>
        /// Gets the format collection.
        /// </summary>
        /// <value>The format collection.</value>
        public ListItemCollection FormatCollection
        {
            get
            {
                if (m_FormatCollection == null)
                {
                    m_FormatCollection = new ListItemCollection();
                    m_FormatCollection.Add(new ListItem("Keep uploaded image as is.", "0"));
                    m_FormatCollection.Add(new ListItem("Convert uploaded image to jpeg.", "1"));
                    m_FormatCollection.Add(new ListItem("Convert uploaded image to jpeg with a fixed format (with border).", "2"));
                    m_FormatCollection.Add(new ListItem("Convert uploaded image to jpeg to and validate maximum format (no border).", "3"));
                }
                return m_FormatCollection;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [format type is visible].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [format type is visible]; otherwise, <c>false</c>.
        /// </value>
        public bool FormatTypeIsVisible
        {
            get
            {
                if (Implement.TypeID == 1) return true;
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [format is visible].
        /// </summary>
        /// <value><c>true</c> if [format is visible]; otherwise, <c>false</c>.</value>
        public bool FormatIsVisible
        {
            get {
                if (Implement.FormatType == 2 || Implement.FormatType == 3) return true; 
                return false; 
            }
        }

        /// <summary>
        /// Gets a value indicating whether [border is visible].
        /// </summary>
        /// <value><c>true</c> if [border is visible]; otherwise, <c>false</c>.</value>
        public bool BorderIsVisible
        {
            get
            {
                if (Implement.FormatType == 2) return true;
                return false;
            }
        }

        private Sushi.Mediakiwi.Data.Gallery m_Gallery;
        /// <summary>
        /// Gets or sets the gallery.
        /// </summary>
        /// <value>The gallery.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.DataExtend()]
        public Sushi.Mediakiwi.Data.Gallery Implement
        {
            get { return m_Gallery; }
            set { m_Gallery = value; }
        }
        #endregion List attributes
    }
}
