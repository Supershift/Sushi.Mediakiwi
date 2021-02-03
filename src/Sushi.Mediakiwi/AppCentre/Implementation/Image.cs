using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sushi.Mediakiwi.Framework;
using System.Threading;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    /// <summary>
    /// 
    /// </summary>
    public class Image : BaseImplementation
    {
		#region CTor
        /// <summary>
        /// Initializes a new instance of the <see cref="Image"/> class.
        /// </summary>
        public Image()
		{
            this.ListSave += new ComponentListEventHandler(wim_Images_ListSave);
            this.ListPreRender += new ComponentListEventHandler(Image_ListPreRender);
            this.ListLoad += new ComponentListEventHandler(wim_Images_ListLoad);
            this.ListDelete += new ComponentListEventHandler(wim_Images_ListDelete);
            this.ListSearch += new ComponentSearchEventHandler(wim_Images_ListSearch);
        }

        void Image_ListPreRender(object sender, ComponentListEventArgs e)
        {
            if (string.IsNullOrEmpty(m_Title))
            {
                if (m_File != null && m_File.ContentLength > 0)
                {
                    string[] split = m_File.FileName.Split('\\');
                    m_Title = split[split.Length - 1];
                }
            }
        }
        #endregion

        #region ListSearch
        void wim_Images_ListSearch(object sender, ComponentListSearchEventArgs e)
        {
        }
        #endregion

        #region ListLoad
        Sushi.Mediakiwi.Data.Image implement;
        private Supporting.Section m_section;
        private void wim_Images_ListLoad(object sender, ComponentListEventArgs e)
        {
            wim.ListTitle = "Image";

            IsExisting = (e.SelectedKey > 0);

            if (!this.IsPostBack)
                m_Gallery = Request.Params["gallery"];

            m_section = (Supporting.Section)Wim.Utility.ConvertToInt(Request.Params["env"]);

            if (e.SelectedKey == 0)
            {
                implement = new Sushi.Mediakiwi.Data.Image();
                return;
            }


            implement = 
                Sushi.Mediakiwi.Data.Image.SelectOne(e.SelectedKey );

            if (implement == null)
            {
                implement = new Sushi.Mediakiwi.Data.Image();
                return;
            }

            m_Title = implement.Title;
            //m_Displayname = implement.Displayname;
            m_Alt = implement.Description;
            m_Thumbnail = string.Format("<img src=\"{0}\" border=\"0\">", implement.ThumbnailPath);
            m_Format = string.Format("{0} x {1} (bxh)", implement.Width, implement.Height);
            m_Size = string.Format("{0} kb", implement.Size / 1024);
            m_Gallery = implement.GalleryID.ToString();
            m_IsActive = implement.IsActive;

            //Response.Write(implement.Path);
        }
        #endregion ListLoad

        #region ListSave
        private void wim_Images_ListSave(object sender, ComponentListEventArgs e)
        {
            if (!this.InpopupLayerOrNew)
                m_File = m_File1;

            if (implement == null)
                implement = Sushi.Mediakiwi.Data.Image.SelectOne(e.SelectedKey);

            if (m_File != null && m_File.ContentLength > 0)
            {
                string[] split = m_File.FileName.Split('\\');
                implement.FileName = split[split.Length - 1];
                implement.Type = m_File.ContentType;
            }

            implement.Title = m_Title;
            implement.Description = Wim.Utility.CleanLineFeed(m_Alt, false);
            implement.IsActive = m_IsActive;

            int galleryId;
            Guid guid;
            Sushi.Mediakiwi.Data.Gallery gallery;
            if (Wim.Utility.IsGuid(Request.QueryString["gallery"], out guid))
            {
                gallery = Sushi.Mediakiwi.Data.Gallery.SelectOne(guid);
                implement.GalleryID = gallery.ID;
            }
            else if (Wim.Utility.IsNumeric(Request.QueryString["gallery"], out galleryId))
            {
                gallery = Sushi.Mediakiwi.Data.Gallery.SelectOne(galleryId);
                implement.GalleryID = gallery.ID;
            }
            else
            {
                implement.GalleryID = Convert.ToInt32(m_Gallery);
                gallery = Sushi.Mediakiwi.Data.Gallery.SelectOne(implement.GalleryID);
            }

            implement.Save();

            if (m_File == null || m_File.ContentLength == 0)
            {

            }
            else
            {
                gallery.CreateFolder();
            }

            implement.SaveStream(m_File, gallery);

            wim.OnSaveScript = string.Format(@"<div class=""textOptionSelect now_yes""><input type=""hidden"" id=""{0}"" value=""{1} ({2} KB)"" /></div>", implement.ID, implement.Title, implement.Size > 0 ? (implement.Size / 1024) : 0);
        }
  
        #endregion ListSave

        #region ListDelete
        private void wim_Images_ListDelete(object sender, ComponentListEventArgs e)
        {
            Sushi.Mediakiwi.Data.Image.SelectOne(e.SelectedKey).Delete();
        }
        #endregion ListDelete

        #region Create
        ///// <summary>
        ///// Creates the specified do BLOB insert.
        ///// </summary>
        ///// <param name="doBlobInsert">if set to <c>true</c> [do BLOB insert].</param>
        ///// <returns></returns>
        //public bool Create(bool doBlobInsert, Sushi.Mediakiwi.Data.Image image, Sushi.Mediakiwi.Data.Gallery gallery)
        //{
        //    image.SaveStream(m_File);

        //    return true;

        //    //  Check file existance
        //    if ( this.m_File == null )
        //    {
        //        wim.Notification.Error.Add("The file does not exist");
        //        return false;
        //    }

        //    //  Check file size
        //    if ( m_File.ContentLength == 0 )
        //    {
        //        wim.Notification.Error.Add("The file is of size: 0");
        //        return false;
        //    }

        //    //  Check file type
        //    if ( m_File.ContentType.ToLower().IndexOf("image") == -1 )
        //    {
        //        wim.Notification.Error.Add("The uploaded file is not an image, please use the documents sections for uploading this file.");
        //        return false;
        //    }
            
        //    //  Something wend wrong with insert
        //    if ( image.Id == 0 )
        //    {
        //        return false;
        //    }

        //    //  Obtain image
        //    using (System.Drawing.Image imageStreamed = System.Drawing.Image.FromStream(m_File.InputStream))
        //    {
        //        image.Width = imageStreamed.Width;
        //        image.Height = imageStreamed.Height;

        //        byte[] imageArr = null;
        //        //  Temporarily store the binary file in the database to push!
        //        if (doBlobInsert)
        //        {
        //            using (MemoryStream ms = new MemoryStream())
        //            {
        //                imageStreamed.Save(ms, imageStreamed.RawFormat);
        //                imageArr = ms.ToArray();
        //            }
        //        }

        //        //  Create the image.
        //        int size = 0, newWidth = 0, newHeight = 0;

        //        //  Create thumbnail
        //        string filename = image.Id.ToString();

        //        //  Without APP PATH!
        //        string filepath = Wim.CommonConfiguration.RelativeRepositoryImageThumbnailUrl;
        //        CreateNewSizeImage(imageStreamed, filepath, filename, 80, 80, ref size, ref newWidth, ref newHeight, true, gallery.GetBackgroundRgb());

        //        //  Set original image
        //        filepath = string.Concat(Wim.CommonConfiguration.RelativeRepositoryImageUrl, gallery.BaseGallery, "/");

        //        int extentionStart = m_File.FileName.Split('.').Length;
        //        string extention = m_File.FileName.Split('.')[extentionStart - 1].ToLower();
        //        string relative = Utility.AddApplicationPath(string.Format("{0}{1}.{2}", filepath, filename, extention));

        //        if (gallery.FormatType.GetValueOrDefault(0) > 0)
        //        {
        //            //  Convert uploaded image to jpeg.
        //            if (gallery.FormatType == 1)
        //            {
        //                CreateNewSizeImage(imageStreamed, filepath, filename
        //                    , imageStreamed.Width
        //                    , imageStreamed.Height
        //                    , ref size, ref newWidth, ref newHeight, true, gallery.GetBackgroundRgb());

        //                image.Size = size;
        //                image.Width = newWidth;
        //                image.Height = newHeight;
        //                image.Extention = "jpg";
        //            }

        //            //  Convert uploaded image to jpeg with a fixed format (with border).
        //            if (gallery.FormatType == 2)
        //            {
        //                List<Sushi.Mediakiwi.Data.ImageFile> fileList = new List<Sushi.Mediakiwi.Data.ImageFile>();

        //                int imageCounter = 0;
        //                string[] formatElements = gallery.Format.Split(';');
        //                foreach (string formatItem in formatElements)
        //                {
        //                    imageCounter++;
                            
        //                    string[] formatSplit = formatItem.Split('x');

        //                    decimal newTmpWidth = Convert.ToDecimal(formatSplit[0]);
        //                    decimal newTmpHeight = Convert.ToDecimal(formatSplit[1]);

        //                    if (newTmpWidth == 0 && newTmpHeight == 0) throw new Exception("The Gallery format can not be 0x0");
        //                    if (newTmpWidth == 0) newTmpWidth = decimal.Multiply(decimal.Divide(newHeight, imageStreamed.Height), imageStreamed.Width);
        //                    if (newTmpHeight == 0) newTmpHeight = decimal.Multiply(decimal.Divide(newTmpWidth, imageStreamed.Width), imageStreamed.Height);

        //                    if (formatElements.Length > 0)
        //                        filename = string.Concat(image.Id, "_", imageCounter);

        //                    CreateNewSizeImage(imageStreamed, filepath, filename
        //                        , Convert.ToInt32(Math.Round(newTmpWidth, 0))
        //                        , Convert.ToInt32(Math.Round(newTmpHeight, 0))
        //                        , ref size, ref newWidth, ref newHeight, true, gallery.GetBackgroundRgb());

        //                    Sushi.Mediakiwi.Data.ImageFile file = new Sushi.Mediakiwi.Data.ImageFile();

        //                    file.Extention = "jpg";
        //                    file.Size = size;
        //                    file.Width = newWidth;
        //                    file.Height = newHeight;
        //                    file.Name = filename;
        //                    fileList.Add(file);

        //                    image.Size = size;
        //                    image.Width = newWidth;
        //                    image.Height = newHeight;
        //                    image.Extention = "jpg";
        //                }
        //                image.SerializedData = Wim.Utility.GetSerialized(fileList.ToArray());
        //            }

        //            //  Convert uploaded image to jpeg to and validate maximum format (no border).
        //            if (gallery.FormatType == 3)
        //            {
        //                List<Sushi.Mediakiwi.Data.ImageFile> fileList = new List<Sushi.Mediakiwi.Data.ImageFile>();

        //                int imageCounter = 0;
        //                string[] formatElements = gallery.Format.Split(';');
        //                foreach (string formatItem in formatElements)
        //                {
        //                    imageCounter++;

        //                    string[] formatSplit = formatItem.Split('x');

        //                    decimal newTmpWidth = Convert.ToDecimal(formatSplit[0]);
        //                    decimal newTmpHeight = Convert.ToDecimal(formatSplit[1]);

        //                    if (newTmpWidth != 0 && newTmpWidth > imageStreamed.Width)
        //                        newTmpWidth = imageStreamed.Width;

        //                    if (newTmpHeight != 0 && newTmpHeight > imageStreamed.Height)
        //                        newTmpHeight = imageStreamed.Height;

        //                    if (newTmpWidth == 0 && newTmpHeight == 0) throw new Exception("The Gallery format can not be 0x0");
        //                    if (newTmpWidth == 0) newTmpWidth = decimal.Multiply(decimal.Divide(newHeight, imageStreamed.Height), imageStreamed.Width);
        //                    if (newTmpHeight == 0) newTmpHeight = decimal.Multiply(decimal.Divide(newTmpWidth, imageStreamed.Width), imageStreamed.Height);

        //                    if (formatElements.Length > 0)
        //                        filename = string.Concat(image.Id, "_", imageCounter);

        //                    CreateNewSizeImage(imageStreamed, filepath, filename
        //                        , Convert.ToInt32(Math.Round(newTmpWidth, 0))
        //                        , Convert.ToInt32(Math.Round(newTmpHeight, 0))
        //                        , ref size, ref newWidth, ref newHeight, false, gallery.GetBackgroundRgb());

        //                    Sushi.Mediakiwi.Data.ImageFile file = new Sushi.Mediakiwi.Data.ImageFile();

        //                    file.Extention = "jpg";
        //                    file.Size = size;
        //                    file.Width = newWidth;
        //                    file.Height = newHeight;
        //                    file.Name = filename;
        //                    fileList.Add(file);

        //                    image.Size = size;
        //                    image.Width = newWidth;
        //                    image.Height = newHeight;
        //                    image.Extention = "jpg";
        //                }
        //                image.SerializedData = Wim.Utility.GetSerialized(fileList.ToArray());
        //            }
        //        }
        //        else
        //        {
        //            //  Save original image
        //            imageStreamed.Save(Server.MapPath(relative));
        //            image.Size = m_File.ContentLength;
        //            image.Extention = extention;
        //        }
        //        Sushi.Mediakiwi.Data.Image.UpdateOne(image, imageArr, true);
        //    }
        //    return true;
        //}
        #endregion Create

        #region Create new size
        //string CreateNewSizeImage(System.Drawing.Image image, string relativeFilePath, string filenameToSave, int width, int height, ref int size, ref int newWidth, ref int newHeight, bool isAlwaysSpecifiedSize, int[] backgroundRgb)
        //{
        //    return null;
        //    using ( Wim.Utilities.Thumbnailer thumb = new Wim.Utilities.Thumbnailer() )
        //    {
        //        if (backgroundRgb != null && backgroundRgb.Length == 3)
        //        {
        //            thumb.BackGroundColor = Color.FromArgb(backgroundRgb[0], backgroundRgb[1], backgroundRgb[2]);
        //        }
        //        thumb.ImageVirtualPath = relativeFilePath;
        //        //thumb.SavedFileName = filenameToSave; //optional
        //        //thumb.ThumbnailVirtualPath = relativeFilePath;
        //        //thumb.SavedThumbnailName = filenameToSave; //optional
        
        //        thumb.ContentType = ImageFormat.Jpeg;
        //        thumb.ThumbnailQuality = 80;
        //        thumb.IsAlwaysSpecifiedSize = isAlwaysSpecifiedSize;
        //        thumb.IsEnlargedWhenSmallerThenSpecified = false;
        
        //        string imagePath = Utility.AddApplicationPath(string.Format("{0}{1}.jpg", relativeFilePath, filenameToSave ));
        //        try
        //        {
        //            System.Drawing.Image imgPhoto = null;
        //            imgPhoto = thumb.FixedSize(image, width, height);
                    
        //            //  Pass ref parameters
        //            newWidth = imgPhoto.Width;
        //            newHeight = imgPhoto.Height;
        //            //  End passing

        //            thumb.SaveThumbnail(imgPhoto);
        //            imgPhoto.Dispose(); 

        //            //  Get file size and pass it on
        //            System.IO.FileInfo fi = new System.IO.FileInfo( Server.MapPath (imagePath) );
        //            size = Convert.ToInt32(fi.Length);

        //            return Utility.RemApplicationPath(imagePath);
        //        }
        //        catch(Exception ex ) 
        //        { 
        //            //throw new Exception( string.Format("Trying to save @'{0}' resulted into error: {1} and {2}", imagePath, ex.Message, thumb.ThumbnailFileServerPath(filenameToSave) ) );
        //        }
        //    }
        //}
        #endregion Create new size

        #region List attributes
        private HttpPostedFile m_File;
        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        /// <value>The file.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("InpopupLayerOrNew")]
        [Sushi.Mediakiwi.Framework.ContentListItem.FileUpload("Upload image", true, "")]
        public HttpPostedFile File
        {
            get { return m_File; }
            set { m_File = value; }
        }

        private HttpPostedFile m_File1;
        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        /// <value>The file.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("InpopupLayerOrNew", false)]
        [Sushi.Mediakiwi.Framework.ContentListItem.FileUpload("Upload image1", false, "")]
        public HttpPostedFile File1
        {
            get { return m_File1; }
            set { m_File1 = value; }
        }

        private string m_Title;
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Title", 50, true)]
        public string Title
        {
            get { return m_Title; }
            set { m_Title = value; }
        }

        private string m_Displayname;
        /// <summary>
        /// Gets or sets the displayname.
        /// </summary>
        /// <value>The displayname.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Displayname", 255, false)]
        public string Displayname
        {
            get { return m_Displayname; }
            set { m_Displayname = value; }
        }

        
        private string m_Alt;
        /// <summary>
        /// Gets or sets the alt.
        /// </summary>
        /// <value>The alt.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextArea("Description", 500, false, "The alt text.")]
        public string Alt
        {
            get { return m_Alt; }
            set { m_Alt = value; }
        }

        private bool m_IsActive = true;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Is active")]
        public bool IsActive
        {
            get { return m_IsActive; }
            set { m_IsActive = value; }
        }

        #region GalleryCollection
        private ListItemCollection m_GalleryCollection;
        /// <summary>
        /// Gets the gallery collection.
        /// </summary>
        /// <value>The gallery collection.</value>
        public ListItemCollection GalleryCollection
        {
            get
            {
                if (m_GalleryCollection != null)
                    return m_GalleryCollection;

                m_GalleryCollection = new ListItemCollection();

                Sushi.Mediakiwi.Data.Gallery[] galleries = Sushi.Mediakiwi.Data.Gallery.SelectAll();
                foreach (Sushi.Mediakiwi.Data.Gallery gallery in galleries)
                {
                    m_GalleryCollection.Add(new ListItem(gallery.CompletePath, gallery.ID.ToString()));
                }

                return m_GalleryCollection;
            }
        }
        #endregion

        private string m_Gallery;
        /// <summary>
        /// Gets or sets the gallery.
        /// </summary>
        /// <value>The gallery.</value>
        //[Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsGalleryFree")]
        //[Sushi.Mediakiwi.Framework.OnlyEditableWhenTrue("IsNewElement")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Gallery", "GalleryCollection")]
        public string Gallery
        {
            get { return m_Gallery; }
            set { m_Gallery = value; }
        }

        /// <summary>
        /// Gets a value indicating whether [inpopup layer or new].
        /// </summary>
        /// <value><c>true</c> if [inpopup layer or new]; otherwise, <c>false</c>.</value>
        public bool InpopupLayerOrNew
        {
            get
            {
                if (InpopupLayer || wim.Console.Item.GetValueOrDefault(0) == 0) return true;
                return false;
            }
        }




        private string m_Thumbnail;
        /// <summary>
        /// Gets or sets the thumbnail.
        /// </summary>
        /// <value>The thumbnail.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsExisting")]
        //[Sushi.Mediakiwi.Framework.ContentListItem.TextLine("Thumbnail")]
        public string Thumbnail
        {
            get { return m_Thumbnail; }
            set { m_Thumbnail = value; }
        }

        private string m_Format;
        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        /// <value>The format.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsExisting")]
        [Sushi.Mediakiwi.Framework.ContentListItem.TextLine("Format")]
        public string Format
        {
            get { return m_Format; }
            set { m_Format = value; }
        }

        private string m_Size;
        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsExisting")]
        [Sushi.Mediakiwi.Framework.ContentListItem.TextLine("Size")]
        public string Size
        {
            get { return m_Size; }
            set { m_Size = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is gallery free.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is gallery free; otherwise, <c>false</c>.
        /// </value>
        public bool IsGalleryFree
        {
            get { return string.IsNullOrEmpty(Request.QueryString["g"]); }
        }



        private bool m_IsExisting;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is existing.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is existing; otherwise, <c>false</c>.
        /// </value>
        public bool IsExisting
        {
            get { return m_IsExisting; }
            set { m_IsExisting = value; }
        }

        /// <summary>
        /// Gets a value indicating whether [inpopup layer].
        /// </summary>
        /// <value><c>true</c> if [inpopup layer]; otherwise, <c>false</c>.</value>
        public bool InpopupLayer
        {
            get { return (Request.QueryString["openinframe"] == "1"); }
        }
        

        private bool m_Apply;
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Link"/> is apply.
        /// </summary>
        /// <value><c>true</c> if apply; otherwise, <c>false</c>.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("InpopupLayer")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Button("Apply image", true, true, 0)]
        public bool Apply
        {
            get { return m_Apply; }
            set { m_Apply = value; }
        }
        #endregion List attributes
    }
}
