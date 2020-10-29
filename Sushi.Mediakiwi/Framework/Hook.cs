using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary ;

namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// This class is currently obsolete, please ignore.
    /// </summary>
    public class Hook
	{
        private HttpContext m_context;

        private HttpContext Content
        {
            get 
            {
                if ( m_context == null )
                    m_context = HttpContext.Current;
                return m_context;
            }
        }

        private Sushi.Mediakiwi.Data.Site m_Site;
        /// <summary>
        /// 
        /// </summary>
        public Sushi.Mediakiwi.Data.Site Site
        {
            set { 
                m_Site = value;
                SetUploadEnvironment(value);
            }
            get { return m_Site; }
        }

        /// <summary>
        /// Set upload environment
        /// </summary>
        /// <param name="site"></param>
        public void SetUploadEnvironment(Sushi.Mediakiwi.Data.Site site)
        {
            if (site != null)
            {
                m_repository = Wim.Utility.AddApplicationPath("/repository/image/");
                m_uploadPrefixPath = site.DefaultFolder;
                m_uploadPath = Wim.Utility.AddApplicationPath("/repository/image/");
                m_uploadThumbnailPath = Wim.Utility.AddApplicationPath("/repository/thumbnail/");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uploadPrefixPath"></param>
        /// <param name="uploadImagePath"></param>
        /// <param name="uploadThumbnailPath"></param>
        public void SetUploadEnvironment(string uploadPrefixPath, string uploadImagePath, string uploadThumbnailPath)
        {
            m_uploadPrefixPath = uploadPrefixPath;
            m_uploadPath = uploadImagePath;
            m_uploadThumbnailPath = uploadThumbnailPath;
        }

        private string m_repository;
        private string m_uploadPrefixPath;
        private string m_uploadPath;
        private string m_uploadThumbnailPath;

        #region GetImage
        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageId"></param>
        /// <returns></returns>
        public Sushi.Mediakiwi.Data.Image GetImage(int imageId)
        {
            Sushi.Mediakiwi.Data.Image image = Sushi.Mediakiwi.Data.Image.SelectOne(imageId);
            if (image == null)
                return null;
            return image;
        }
        #endregion GetImage

        #region Set image (overloaded)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        [Obsolete("Please do not use")]
        public Sushi.Mediakiwi.Data.Image SetImage(System.Web.HttpPostedFile file, string name, string description)
        {
            return SetImage(file, name, description, 60, 80, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="thumbNailWidth"></param>
        /// <param name="thumbNailHeight"></param>
        /// <param name="isthumbNailExactSizeWithBorder"></param>
        /// <returns></returns>
        [Obsolete("Please do not use")]
        public Sushi.Mediakiwi.Data.Image SetImage(System.Web.HttpPostedFile file, string name, string description, int thumbNailWidth, int thumbNailHeight, bool isthumbNailExactSizeWithBorder)
        {
            return SetImage(file, name, description, thumbNailWidth, thumbNailHeight, isthumbNailExactSizeWithBorder, 0, 0, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="thumbNailWidth"></param>
        /// <param name="thumbNailHeight"></param>
        /// <param name="isthumbNailExactSizeWithBorder"></param>
        /// <param name="originalMaxWidth"></param>
        /// <param name="originalMaxHeight"></param>
        /// <param name="isOriginalExactSizeWithBorder"></param>
        /// <returns></returns>
        [Obsolete("Please do not use")]
        public Sushi.Mediakiwi.Data.Image SetImage(System.Web.HttpPostedFile file, string name, string description, int thumbNailWidth, int thumbNailHeight, bool isthumbNailExactSizeWithBorder, int originalMaxWidth, int originalMaxHeight, bool isOriginalExactSizeWithBorder)
        {
            return SetImage(0, file, name, description, thumbNailWidth, thumbNailHeight, isthumbNailExactSizeWithBorder, originalMaxWidth, originalMaxHeight, false);
        }

        private bool m_needBinaryStorageForPush = false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="image_Key"></param>
        /// <param name="file"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="thumbNailWidth"></param>
        /// <param name="thumbNailHeight"></param>
        /// <param name="isthumbNailExactSizeWithBorder"></param>
        /// <param name="originalMaxWidth"></param>
        /// <param name="originalMaxHeight"></param>
        /// <param name="isOriginalExactSizeWithBorder"></param>
        /// <returns></returns>
        [Obsolete("Please do not use")]
        public Sushi.Mediakiwi.Data.Image SetImage(int image_Key, System.Web.HttpPostedFile file, string name, string description, int thumbNailWidth, int thumbNailHeight, bool isthumbNailExactSizeWithBorder, int originalMaxWidth, int originalMaxHeight, bool isOriginalExactSizeWithBorder)
        {
            return SetImage(image_Key, file, name, description, thumbNailWidth, thumbNailHeight, isthumbNailExactSizeWithBorder, originalMaxWidth, originalMaxHeight, false, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="thumbNailWidth"></param>
        /// <param name="thumbNailHeight"></param>
        /// <param name="isthumbNailExactSizeWithBorder"></param>
        /// <param name="originalMaxWidth"></param>
        /// <param name="originalMaxHeight"></param>
        /// <param name="isOriginalExactSizeWithBorder"></param>
        /// <param name="addToGallery"></param>
        /// <returns></returns>
        public Sushi.Mediakiwi.Data.Image SetImage(System.Web.HttpPostedFile file, string name, string description, int thumbNailWidth, int thumbNailHeight, bool isthumbNailExactSizeWithBorder, int originalMaxWidth, int originalMaxHeight, bool isOriginalExactSizeWithBorder, bool addToGallery)
        {
            return SetImage(0, file, name, description, thumbNailWidth, thumbNailHeight, isthumbNailExactSizeWithBorder, originalMaxWidth, originalMaxHeight, false, addToGallery);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image_Key"></param>
        /// <param name="file"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="thumbNailWidth"></param>
        /// <param name="thumbNailHeight"></param>
        /// <param name="isthumbNailExactSizeWithBorder"></param>
        /// <param name="originalMaxWidth"></param>
        /// <param name="originalMaxHeight"></param>
        /// <param name="isOriginalExactSizeWithBorder"></param>
        /// <param name="addToGallery"></param>
        /// <returns></returns>
        public Sushi.Mediakiwi.Data.Image SetImage(int image_Key, System.Web.HttpPostedFile file, string name, string description, int thumbNailWidth, int thumbNailHeight, bool isthumbNailExactSizeWithBorder, int originalMaxWidth, int originalMaxHeight, bool isOriginalExactSizeWithBorder, bool addToGallery)
        {
            if (m_uploadPath == null)
                throw new Exception("Sushi.Mediakiwi.Framework.Hook: Site not set so the upload path is unknown!");

            //  Check file existance
            if (file == null)
                return null;

            //  Check file size
            if (file.ContentLength == 0)
                return null;
            //  Check file type
            if (file.ContentType.ToLower().IndexOf("image") == -1)
                return null;

            Sushi.Mediakiwi.Data.Image image = null;
            string fileName = name;            

            image = new Sushi.Mediakiwi.Data.Image();
            image.GalleryID = Sushi.Mediakiwi.Data.Gallery.SelectOneRoot(Sushi.Mediakiwi.Data.GalleryType.Images).ID;
            image.Title = name;
            image.Description = description;

            int imageId = image_Key;
            if (imageId == 0 && addToGallery)
            {
                image.Insert();
                imageId = image.ID;
            }

            //  Something wend wrong with insert
            if (imageId == 0 && addToGallery)
                return null;

            image.ID = imageId;
            if (addToGallery) fileName = imageId.ToString();

            int extentionStart = file.FileName.Split('.').Length;
            string extention = file.FileName.Split('.')[extentionStart - 1].ToLower();
            string relative1 = Utility.AddApplicationPath(string.Format("{0}{1}.{2}", m_uploadPath, fileName, extention));
            string relative2 = Utility.AddApplicationPath(string.Format("{0}{1}{2}.{3}", m_uploadPrefixPath, m_uploadPath, fileName, extention));

            //            if ( !CreateThumbnail( image, thumbNailWidth, thumbNailHeight ) )

            System.Drawing.Image imageStreamed = System.Drawing.Image.FromStream(file.InputStream);

            byte[] imageArr = null;
            if (m_needBinaryStorageForPush)
            {
                //  Temporarily store the binary file in the database to push!
                using (MemoryStream ms = new MemoryStream())
                {
                    imageStreamed.Save(ms, imageStreamed.RawFormat);
                    imageArr = ms.ToArray();
                }
            }

            image.Width = imageStreamed.Width;
            image.Height = imageStreamed.Height;
            image.Extention = extention;

            string createdFilePath;
            int size = 0, newWidth = 0, newHeight = 0;

            //  Create a thumbnail image
            createdFilePath = Utility.AddApplicationPath(string.Format("{0}{1}.jpg", m_uploadThumbnailPath, fileName.ToString()));

            if (!CreateNewSizeImage(imageStreamed, string.Format("{0}{1}", m_uploadPrefixPath, m_uploadThumbnailPath), fileName, thumbNailWidth, thumbNailHeight, ref size, ref newWidth, ref newHeight))
                return null;

            //  Save original file to disk or scale it
            string newFile = Content.Server.MapPath(relative2);
            if (originalMaxHeight == 0)
            {
                imageStreamed.Save(newFile);
                image.Size = file.ContentLength;
                //image.Path = Utility.RemApplicationPath(relative1);
            }
            else
            {
                createdFilePath = Utility.AddApplicationPath(string.Format("{0}{1}.jpg", m_uploadPath, fileName.ToString()));

                if (!CreateNewSizeImage(imageStreamed, string.Format("{0}{1}", m_uploadPrefixPath, m_uploadPath), fileName, originalMaxWidth, originalMaxHeight, ref size, ref newWidth, ref newHeight))
                    return null;

                image.Size = size;
                image.Width = newWidth;
                image.Height = newHeight;
                //image.Path = Utility.RemApplicationPath(createdFilePath);
            }

            if (addToGallery)
            {
                image.Update();
            }

            return image;
        }

        #region Create new size
        private bool CreateNewSizeImage(System.Drawing.Image image, string storagePath, string filenameToSave, int width, int height, ref int size, ref int newWidth, ref int newHeight)
        {
            using (Wim.Utilities.Thumbnailer thumb = new Wim.Utilities.Thumbnailer())
            {
                thumb.ImageVirtualPath = storagePath;
                thumb.ContentType = ImageFormat.Jpeg;
                thumb.ThumbnailQuality = 80;
                thumb.IsAlwaysSpecifiedSize = true;
                thumb.IsEnlargedWhenSmallerThenSpecified = false;

                string thumbnailPath = Utility.AddApplicationPath(string.Format("{0}{1}.jpg", storagePath, filenameToSave));

                try
                {
                    System.Drawing.Image imgPhoto = null;
                    imgPhoto = thumb.FixedSize(image, width, height, 0, 0, Data.ImagePosition.Center);

                    //  Pass ref parameters
                    newWidth = imgPhoto.Width;
                    newHeight = imgPhoto.Height;
                    //  End passing

                    thumb.SaveThumbnail(imgPhoto);
                    imgPhoto.Dispose();

                    //  Get file size and pass it on
                    System.IO.FileInfo fi = new System.IO.FileInfo(Content.Server.MapPath(thumbnailPath));
                    size = Convert.ToInt32(fi.Length);

                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Trying to save @'{0}' resulted into error: {1}", thumbnailPath, ex.Message));
                }
            }
        }
        #endregion Create new size
        #endregion Set image (overloaded)

        #region PageRedirect
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageId"></param>
        public void PageRedirect( int pageId )
        {
            Sushi.Mediakiwi.Data.Page page = Sushi.Mediakiwi.Data.Page.SelectOne( pageId );
            if ( page != null && !page.IsNewInstance )
                Content.Response.Redirect(page.HRef, true);
        }

        /// <summary>
        /// Applies the content.
        /// </summary>
        /// <param name="loadedControl">The loaded control.</param>
        /// <param name="page">The page.</param>
        /// <param name="componentID">The component ID.</param>
        public void ApplyContent(System.Web.UI.Control loadedControl, Sushi.Mediakiwi.Data.Page page, int componentID)
        {
            if (loadedControl == null || loadedControl.ClientID == null) return;
            Sushi.Mediakiwi.Data.Component component = Sushi.Mediakiwi.Data.Component.SelectOne(componentID);
            if (component == null)
                return;

            Sushi.Mediakiwi.Data.Content content = Sushi.Mediakiwi.Data.Content.GetDeserialized(component.Serialized_XML);
            Templates.PropertySet pset = new Sushi.Mediakiwi.Framework.Templates.PropertySet();

            ((ComponentTemplate)loadedControl).wim.Component = component;

            //  TODO: WHY first set it and then discard it?
            pset.SetValue(Site, loadedControl, content, page);
        }

        #endregion PageRedirect

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public string GetPageUrl( int pageId )
        {
            Sushi.Mediakiwi.Data.Page page = Sushi.Mediakiwi.Data.Page.SelectOne( pageId );
            if ( page != null )
                return page.HRef;
            return null;
        }
    }
}
