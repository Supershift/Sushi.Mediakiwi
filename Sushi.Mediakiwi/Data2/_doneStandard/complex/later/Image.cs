using System;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Sushi.Mediakiwi.Data.DalReflection;
using System.Data.SqlClient;

using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using Sushi.Mediakiwi.DataEntities;

namespace Sushi.Mediakiwi.Data
{
    public enum ImagePosition
    {
        TopLeft,
        TopCenter,
        Center,
    }

    /// <summary>
    /// Represents a Image entity.
    /// </summary>
    public class Image : Asset
    {
        ImageFile[] m_FormatFiles;

        /// <summary>
        /// Delete an implementation record.
        /// </summary>
        /// <returns></returns>
        public override bool Delete()
        {
            if (System.Web.HttpContext.Current != null)
            {
                if (System.IO.File.Exists(this.Path))
                    System.IO.File.Delete(System.Web.HttpContext.Current.Server.MapPath(this.Path));

                if (System.IO.File.Exists(this.ThumbnailPath))
                    System.IO.File.Delete(System.Web.HttpContext.Current.Server.MapPath(this.ThumbnailPath));
            }
            return base.Delete();
        }

        /// <summary>
        /// Selects the one by gallery.
        /// </summary>
        /// <param name="relativePath">The relative path.</param>
        /// <param name="ID">The identifier.</param>
        /// <returns></returns>
        public static new Image SelectOneByGallery(string relativePath, int ID)
        {
            Image t = new Image();
            var a = Asset.SelectOneByGallery(relativePath, ID);
            Wim.Utility.ReflectProperty(a, t);
            t.DatabaseMappingPortal = a.DatabaseMappingPortal;

            if (t.IsImage)
                return t;
            else
                return new Image();
        }
        /// <summary>
        /// Selects the one by portal. 
        /// </summary>
        /// <param name="ID">The identifier.</param>
        /// <param name="portal">The portal.</param>
        /// <returns>An Image</returns>
        public static Image SelectOneByPortal(int ID, string portal)
        {
            Image implement = new Image();

            //  Other database connection
            implement.DatabaseMappingPortal = Sushi.Mediakiwi.Data.Common.GetPortal(portal, false);
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("Asset_Key", SqlDbType.Int, ID));

            implement = (Image)implement._SelectOne(whereClause, "ID", ID.ToString());
            return implement;
        }

        /// <summary>
        /// Selects the one by gallery.
        /// </summary>
        /// <param name="relative">The relative.</param>
        /// <param name="ID">The identifier.</param>
        /// <returns></returns>
        public static new Image SelectOneByGallery(string relative, Guid ID)
        {
            Image t = new Image();
            Wim.Utility.ReflectProperty(Asset.SelectOneByGallery(relative, ID), t);
            if (t.IsImage)
                return t;
            else
                return new Image();
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <param name="databaseMapName">Name of the database map.</param>
        /// <returns></returns>
        public static new Image SelectOne(int ID, string databaseMapName)
        {
            Image t = new Image();
            var asset = Asset.SelectOneByMap(ID, databaseMapName);
            Wim.Utility.ReflectProperty(asset, t);
            t.DatabaseMappingPortal = asset.DatabaseMappingPortal;

            if (t.IsImage)
                return t;
            else
                return new Image();
        }

        /// <summary>
        /// Select an Image based on the asset variation relation
        /// </summary>
        /// <param name="assetID">The asset identifier.</param>
        /// <param name="assetTag">The assettype tag.</param>
        /// <param name="databaseMapName">Name of the database map.</param>
        /// <returns></returns>
        public static new Image SelectOneByAssetTypeVariant(int assetID, string assetTag, string databaseMapName = null)
        {
            Image t = new Image();
            var asset = Asset.SelectOneByAssetTypeVariant(assetID, assetTag, databaseMapName);
            Wim.Utility.ReflectProperty(asset, t);
            t.DatabaseMappingPortal = asset.DatabaseMappingPortal;

            if (t.IsImage)
                return t;
            else
                return new Image();
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="galleryID">The gallery ID.</param>
        /// <param name="databaseMapName">Name of the database map.</param>
        /// <returns></returns>
        public static new Image[] SelectAll(int galleryID, string databaseMapName, bool randomize)
        {
            List<Image> list = new List<Image>();
            Image implement = new Image();

            //  Other database connection
            var connection = Sushi.Mediakiwi.Data.Common.GetCurrentMappingConnectionByName(databaseMapName);
            if (connection != null)
                implement.SqlConnectionString = connection.Connection;

            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("Asset_IsImage", SqlDbType.Bit, true));
            whereClause.Add(new DatabaseDataValueColumn("Asset_Gallery_Key", SqlDbType.Int, galleryID));
            whereClause.Add(new DatabaseDataValueColumn("Asset_IsArchived", SqlDbType.Bit, false));
            whereClause.Add(new DatabaseDataValueColumn("Asset_IsActive", SqlDbType.Bit, true));
            // CB 26-11-2014; Zodoende geen afbeeldinge varianten
            whereClause.Add(new DatabaseDataValueColumn("Asset_Asset_Key is null"));
            if (randomize)
                implement.SqlOrder = "newid()";

            foreach (object o in implement._SelectAll(whereClause))
            {
                var img = (Image)o;
                if (connection != null)
                    img.DatabaseMappingPortal = connection;
                list.Add(img);
            }
            return list.ToArray();
        }

        public static new Image[] SelectAll(int galleryID, string databaseMapName)
        {
            return SelectAll(galleryID, databaseMapName, false);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public static List<Image> SelectRange(List<int> IDs, string databaseMapName)
        {
            Image implement = new Image();
            List<Image> list = new List<Image>();

            string arrText = Wim.Utility.ConvertToCsvString(IDs.ToArray());
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("Asset_Key", SqlDbType.Int, arrText, DatabaseDataValueCompareType.In));

            var connection = Sushi.Mediakiwi.Data.Common.GetCurrentMappingConnectionByName(databaseMapName);
            if (connection != null)
                implement.SqlConnectionString = connection.Connection;

            foreach (object o in implement._SelectAll(whereClause))
            {
                var asset = (Image)o;
                if (connection != null)
                    asset.DatabaseMappingPortal = connection;
                list.Add(asset);
            }

            return list;
        }


        /// <summary>
        /// Gets the CDN hash.
        /// </summary>
        /// <param name="imageID">The image identifier.</param>
        /// <param name="format">The format.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="backgroundRGB">The background RGB.</param>
        /// <param name="completePath">The complete path.</param>
        /// <param name="encrypt">if set to <c>true</c> [encrypt].</param>
        /// <returns></returns>
        internal static string GetCdnHash(int imageID, Sushi.Mediakiwi.Data.ImageFileFormat format, int width, int height, int[] backgroundRGB, string completePath, bool encrypt)
        {
            if (backgroundRGB == null || backgroundRGB.Length != 3)
                backgroundRGB = Wim.CommonConfiguration.DEFAULT_THUMB_BGCOLOR;

            string conc = string.Format("{0},{1},{2},{3},{4},{5}{6}", (int)format, width, height
                , backgroundRGB[0]
                , backgroundRGB[1]
                , backgroundRGB[2]
                , Wim.CommonConfiguration.HAS_PATH_IN_CDN ? string.Concat(",", completePath) : null
                );

            if (encrypt)
                return Wim.Utilities.Encryption.Encrypt(conc, "cdn", true);
            return conc;
        }


        /// <summary>
        /// Gets the URL.
        /// </summary>
        /// <param name="imageID">The image identifier.</param>
        /// <param name="format">The format.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="backgroundRGB">The background RGB.</param>
        /// <param name="completePath">The complete path.</param>
        /// <returns></returns>
        public static string GetUrl(int imageID, Sushi.Mediakiwi.Data.ImageFileFormat format, int width, int height, int[] backgroundRGB, string completePath)
        {
            string fileFolder = GetCdnHash(imageID, format, width, height, backgroundRGB, completePath, true);
            string filename = string.Concat(imageID, ".jpg");
            string relative = Wim.Utility.AddApplicationPath(string.Concat("/cdn/", fileFolder, "/", filename));

            var mapped = Sushi.Mediakiwi.Data.Common.GetCurrentGalleryMappingUrl(completePath);
            if (mapped != null)
                return string.Concat(mapped.MappedUrl, relative);
            return relative;
        }

        #region MOVED to EXTENSION / LOGIC

        ///// <summary>
        ///// Creates the thumbnail.
        ///// </summary>
        //internal void CreateThumbnail()
        //{
        //    if (!System.IO.File.Exists(LocalFilePath))
        //        return;

        //    if (System.IO.File.Exists(LocalThumbnailFilePath))
        //        return;

        //    using (System.Drawing.Image imageStreamed = System.Drawing.Image.FromFile(LocalFilePath))
        //    {
        //        //  Create the image.
        //        int size = 0, newWidth = 0, newHeight = 0;

        //        //  Without APP PATH!
        //        string fileThumbnailPath = HttpContext.Current.Server.MapPath(Utility.AddApplicationPath(string.Concat(Wim.CommonConfiguration.RelativeRepositoryImageThumbnailUrl, this.ID, ".jpg")));
        //        CreateNewSizeImage(imageStreamed, fileThumbnailPath, 95, 95, ref size, ref newWidth, ref newHeight, true, null);
        //    }

        //}

        ///// <summary>
        ///// Apply the thumbnail image properties to a Image webcontrol (set url, toolTip and width/height)
        ///// </summary>
        ///// <param name="image"></param>
        //public void ApplyThumbnail(System.Web.UI.WebControls.Image image)
        //{
        //    image.Visible = false;
        //    if (ThumbnailPath == null || ThumbnailPath.Trim().Length == 0)
        //        return;

        //    image.ImageUrl = ThumbnailPath;

        //    //if (Description != null)
        //    //    image.ToolTip = Displayname;

        //    if (Height != 0 && Width != 0)
        //    {
        //        image.Height = Height;
        //        image.Width = Width;
        //    }
        //    image.AlternateText = Description == null ? string.Empty : Description;
        //    image.Visible = true;
        //}

        ///// <summary>
        ///// Apply the image properties to a Image webcontrol (set url, toolTip and width/height)
        ///// </summary>
        ///// <param name="image"></param>
        //public void Apply(System.Web.UI.WebControls.Image image)
        //{
        //    image.Visible = false;
        //    if (Path == null || Path.Trim().Length == 0)
        //        return;

        //    image.ImageUrl = Path;

        //    //if ( Description != null )
        //    //    image.ToolTip = Displayname;

        //    if (Height != 0)
        //        image.Height = Height;
        //    if (Width != 0)
        //        image.Width = Width;

        //    image.AlternateText = Description == null ? string.Empty : Description;
        //    image.Visible = true;
        //}

        ///// <summary>
        ///// Applies the specified image.
        ///// </summary>
        ///// <param name="image">The image.</param>
        ///// <param name="setFileFormatWidth">Width of the set file format.</param>
        ///// <param name="setFileFormatHeigth">The set file format heigth.</param>
        //public void Apply(System.Web.UI.WebControls.Image image, int setFileFormatWidth, int setFileFormatHeigth)
        //{
        //    image.Visible = false;
        //    if (Path == null || Path.Trim().Length == 0)
        //        return;

        //    ImageFile foundFile = null;

        //    if (foundFile == null)
        //        return;


        //    this.Width = foundFile.Width;
        //    this.Height = foundFile.Height;
        //    string fileName = string.Concat(foundFile.Name, ".", foundFile.Extention);

        //    this.ApplyPath(fileName, false, true);

        //    image.ImageUrl = Path;

        //    //if (Description != null)
        //    //    image.ToolTip = Displayname;

        //    if (Height != 0 && Width != 0)
        //    {
        //        image.Height = Height;
        //        image.Width = Width;
        //    }
        //    image.AlternateText = Description == null ? string.Empty : Description;
        //    image.Visible = true;
        //}

        ///// <summary>
        ///// Apply the image properties to a Image webcontrol with a set width (set url, toolTip and width/height).
        ///// The height is calculated based on the rescaling factor
        ///// </summary>
        ///// <param name="image">The image.</param>
        ///// <param name="width">The width.</param>
        //public void ApplyScaleWidth(System.Web.UI.WebControls.Image image, int width)
        //{
        //    if (Height != 0 && Width != 0 && width > 0)
        //    {
        //        decimal factor = decimal.Divide(width, Width);
        //        decimal newHeight = decimal.Round(decimal.Multiply(factor, Height), 0);
        //        this.Height = Convert.ToInt32(newHeight);
        //        this.Width = width;
        //    }
        //    Apply(image);
        //}


        ///// <summary>
        ///// Applies the maximum width of the forced.
        ///// </summary>
        ///// <param name="image">The image.</param>
        ///// <param name="width">The width.</param>
        ///// <param name="returnLocalUrl">if set to <c>true</c> [return local URL].</param>
        ///// <returns></returns>
        //public string ApplyForcedMaximumWidth(System.Web.UI.WebControls.Image image, int width, bool returnLocalUrl)
        //{
        //    return ApplyForcedMaximumWidth(image, width, ImagePosition.Center, returnLocalUrl);
        //}

        ///// <summary>
        ///// Applies the maximum width of the forced.
        ///// </summary>
        ///// <param name="image">The image.</param>
        ///// <param name="width">The width.</param>
        ///// <returns></returns>
        //public string ApplyForcedMaximumWidth(System.Web.UI.WebControls.Image image, int width)
        //{
        //    return ApplyForcedMaximumWidth(image, width, ImagePosition.Center);
        //}

        ///// <summary>
        ///// Applies the maximum width of the forced.
        ///// </summary>
        ///// <param name="image">The image.</param>
        ///// <param name="width">The width.</param>
        ///// <param name="position">The position.</param>
        ///// <returns></returns>
        //public string ApplyForcedMaximumWidth(System.Web.UI.WebControls.Image image, int width, ImagePosition position)
        //{
        //    return ApplyForcedMaximumWidth(image, width, position, false);
        //}

        ///// <summary>
        ///// Applies the maximum width of the forced.
        ///// </summary>
        ///// <param name="image">The image.</param>
        ///// <param name="width">The width.</param>
        ///// <param name="position">The position.</param>
        ///// <param name="returnLocalUrl">if set to <c>true</c> [return local URL].</param>
        ///// <returns></returns>
        //public string ApplyForcedMaximumWidth(System.Web.UI.WebControls.Image image, int width, ImagePosition position, bool returnLocalUrl)
        //{
        //    return ApplyForcedMaximumWidth(image, width, position, returnLocalUrl, false);
        //}

        ///// <summary>
        ///// Parses the image.
        ///// </summary>
        ///// <param name="image">The image.</param>
        ///// <param name="skipGalleryMapping">if set to <c>true</c> [skip gallery mapping].</param>
        ///// <param name="returnLocalUrl">if set to <c>true</c> [return local URL].</param>
        ///// <param name="format">The format.</param>
        ///// <param name="position">The position.</param>
        ///// <param name="width">The width.</param>
        ///// <param name="height">The height.</param>
        ///// <param name="bgColor">Color of the bg.</param>
        ///// <returns></returns>
        //string ParseImage(System.Web.UI.WebControls.Image image, bool skipGalleryMapping, bool returnLocalUrl, ImageFileFormat format, ImagePosition position, int width, int height, int[] bgColor)
        //{
        //    if (bgColor == null || bgColor.Length != 3)
        //        bgColor = Wim.CommonConfiguration.DEFAULT_THUMB_BGCOLOR;

        //    string hash = Sushi.Mediakiwi.Data.Image.GetCdnHash(this.ID, format, width, height
        //        , bgColor
        //        , this.CompletePath, true);

        //    //  Skip gallery mapping from handler as this already is mapped.
        //    if (!skipGalleryMapping)
        //    {
        //        if (this.GalleryUrlMap != null)
        //        {
        //            string portal = null;
        //            if (!string.IsNullOrEmpty(this.GalleryUrlMap.Portal))
        //                portal = string.Concat("/", Wim.Utilities.Encryption.Encrypt(this.GalleryUrlMap.Portal, "cdn", true));

        //            string path = string.Format("{0}/cdn{2}/{1}/{3}.jpg"
        //                , this.GalleryUrlMap.MappedUrl
        //                , hash
        //                , portal
        //                , this.ID
        //                );

        //            path = Framework.AssetHandler.ScaledAssetCacheManager.GetUrl(path);

        //            if (image != null)
        //                image.ImageUrl = path;

        //            return path;
        //        }
        //    }

        //    if (returnLocalUrl)
        //    {
        //        string filecheck = Wim.Utility.AddApplicationPath(string.Format("/cdn/{0}/{1}.jpg"
        //            , hash
        //            , this.ID
        //            ));
        //        return Framework.AssetHandler.ScaledAssetCacheManager.GetUrl(filecheck);

        //    }
        //    if (HasCloudSetting)
        //    {
        //        if (image != null)
        //        {
        //            string filecheck = Wim.Utility.AddApplicationPath(string.Format("/cdn/{0}/{1}.jpg"
        //              , hash
        //              , this.ID
        //              ));

        //            filecheck = Framework.AssetHandler.ScaledAssetCacheManager.GetUrl(filecheck);
        //            image.ImageUrl = filecheck;
        //            return filecheck;
        //        }
        //        else
        //        {

        //            if (Sushi.Mediakiwi.Data.Asset.m_CloundInstance.ExistsGeneratedContent(string.Concat(hash, "/", this.ID, ".jpg")))
        //            {
        //                string url = string.Concat(Sushi.Mediakiwi.Data.Asset.m_CloundInstance.ContentDeliveryUrl, Sushi.Mediakiwi.Data.Asset.m_CloundInstance.ContainerGeneratedContent, "/", hash, "/", this.ID, ".jpg");
        //                return url;
        //            }
        //        }
        //    }

        //    if (this.ID == 0)
        //    {
        //        if (image != null)
        //            image.Visible = false;
        //        return null;
        //    }

        //    Sushi.Mediakiwi.Data.Image tmp = this.CreateAttachedImageSize(width, height, format, bgColor, position);

        //    if (tmp == null)
        //    {
        //        if (image != null)
        //            image.Visible = false;
        //        return null;
        //    }
        //    if (image != null)
        //    {
        //        image.ImageUrl = tmp.CdnImagePath;
        //        image.Visible = true;

        //    }
        //    if (HasCloudSetting)
        //    {
        //        if (Sushi.Mediakiwi.Data.Asset.m_CloundInstance.ExistsGeneratedContent(string.Concat(hash, "/", this.ID, ".jpg")))
        //        {
        //            string url = string.Concat(Sushi.Mediakiwi.Data.Asset.m_CloundInstance.ContentDeliveryUrl, Sushi.Mediakiwi.Data.Asset.m_CloundInstance.ContainerGeneratedContent, "/", hash, "/", this.ID, ".jpg");
        //            return url;
        //        }
        //    }
        //    return tmp.GeneratedImagePath;

        //    // ZIE HIERONDER DIT MOET ER NOG IN!!!!

        //    ////edit DV 2013-05-16: issue found that generatedAssets only works with remoteLocation and localimages
        //    ////images hosted on another server (i.e. privilege beheer) don't work, because GenerateANewImage looks for a local file then
        //    ////I don't want to mess around in GenerateANewImage, so if an image has no remotelocation, but does have GalleryUrlMap, we use the old method
        //    //bool useRelyOnGeneratedAssets = RelyOnGeneratedAssets && !(string.IsNullOrEmpty(this.RemoteLocation) && this.GalleryUrlMap != null);
        //    //if (useRelyOnGeneratedAssets)
        //    //{
        //    //    string hash = Wim.Utilities.Encryption.Encrypt(String.Concat(this.FileName, ",", conc), "cdn", true);

        //    //    string portal = null;
        //    //    if (this.DatabaseMappingPortal != null)
        //    //        portal = this.DatabaseMappingPortal.Name;

        //    //    GeneratedAsset ga = GeneratedAsset.SelectOneByGenerationHash(hash, ID, portal);

        //    //    if (ga == null || ga.ID < 1)
        //    //    {
        //    //        string location = GenerateANewImage(image, conc, CropImage, width, height, backgroundRgb, position, galleryToSaveTo);

        //    //        if (!string.IsNullOrEmpty(location))
        //    //        {
        //    //            ga = new GeneratedAsset(portal);
        //    //            ga.AssetID = this.ID;
        //    //            ga.Location = location;
        //    //            ga.Created = DateTime.Now;
        //    //            ga.GenerationHash = hash;
        //    //            try
        //    //            {
        //    //                ga.Save();
        //    //            }
        //    //            catch
        //    //            {
        //    //                //could be a multi threading thing (some other process allready created the asset), so first check if it exists now
        //    //                ga = GeneratedAsset.SelectOneByGenerationHash(hash, ID, portal);
        //    //                if (ga == null || ga.ID < 1)
        //    //                    throw;
        //    //            }
        //    //        }
        //    //    }
        //    //    if (ga != null && image != null)
        //    //    {
        //    //        if (!String.IsNullOrEmpty(ga.Location))
        //    //        {

        //    //            image.ImageUrl = ga.Location;
        //    //            image.Visible = true;
        //    //        }
        //    //        else
        //    //        {
        //    //            image.Visible = false;
        //    //        }
        //    //        return ga.Location;
        //    //    }
        //    //    else if (ga != null)
        //    //        return ga.Location;
        //    //    else
        //    //        return null;


        //    //}
        //    //else
        //    //{

        //    //    if (this.GalleryUrlMap != null)
        //    //    {
        //    //        string path = string.Format("{0}/cdn/{1}/{2}.jpg"
        //    //            , this.GalleryUrlMap.MappedUrl
        //    //            , Wim.Utilities.Encryption.Encrypt(conc, "cdn", true)
        //    //            , this.ID
        //    //            );

        //    //        if (image != null)
        //    //            image.ImageUrl = path;

        //    //        return path;
        //    //    }

        //    //    if (returnLocalUrl)
        //    //    {
        //    //        return Wim.Utility.AddApplicationPath(string.Format("/cdn/{0}/{1}.jpg"
        //    //            , Wim.Utilities.Encryption.Encrypt(conc, "cdn", true)
        //    //            , this.ID
        //    //            ));
        //    //    }

        //    //    if (HasCloudSetting)
        //    //    {
        //    //        if (image != null)
        //    //        {
        //    //            string filecheck = Wim.Utility.AddApplicationPath(string.Format("/cdn/{0}/{1}.jpg"
        //    //              , Wim.Utilities.Encryption.Encrypt(conc, "cdn", true)
        //    //              , this.ID
        //    //              ));
        //    //            image.ImageUrl = filecheck;
        //    //            return filecheck;
        //    //        }
        //    //        else
        //    //        {
        //    //            string filecheck = string.Format("{0}/{1}.jpg"
        //    //                , Wim.Utilities.Encryption.Encrypt(conc, "cdn", true)
        //    //                , this.ID
        //    //                );

        //    //            if (Sushi.Mediakiwi.Data.Asset.m_CloundInstance.ExistsGeneratedContent(filecheck))
        //    //            {
        //    //                return string.Concat(Sushi.Mediakiwi.Data.Asset.m_CloundInstance.ContentDeliveryUrl, Sushi.Mediakiwi.Data.Asset.m_CloundInstance.ContainerGeneratedContent, "/", filecheck);
        //    //            }
        //    //        }
        //    //    }

        //    //    if (this.ID == 0)
        //    //    {
        //    //        if (image != null) image.Visible = false;
        //    //        return null;
        //    //    }
        //    //    return GenerateANewImage(image, conc, CropImage, width, height, backgroundRgb, position, galleryToSaveTo);
        //    //}
        //}

        ///// <summary>
        ///// Applies a forced width. The image is physically scaled!
        ///// </summary>
        ///// <param name="image">The image.</param>
        ///// <param name="width">The width.</param>
        ///// <param name="position">The position.</param>
        ///// <param name="returnLocalUrl">if set to <c>true</c> [return local URL].</param>
        ///// <param name="skipGalleryMapping">if set to <c>true</c> [skip gallery mapping].</param>
        ///// <returns></returns>
        //internal string ApplyForcedMaximumWidth(System.Web.UI.WebControls.Image image, int width, ImagePosition position, bool returnLocalUrl, bool skipGalleryMapping)
        //{
        //    return ParseImage(image, skipGalleryMapping, returnLocalUrl, Sushi.Mediakiwi.Data.ImageFileFormat.ValidateMaximumWidth, position, width, 0, null);
        //}

        ///// <summary>
        ///// Applies the maximum height of the forced.
        ///// </summary>
        ///// <param name="image">The image.</param>
        ///// <param name="height">The height.</param>
        ///// <returns></returns>
        //public string ApplyForcedMaximumHeight(System.Web.UI.WebControls.Image image, int height)
        //{
        //    return ApplyForcedMaximumHeight(image, height, false);
        //}

        ///// <summary>
        ///// Applies the maximum height of the forced.
        ///// </summary>
        ///// <param name="image">The image.</param>
        ///// <param name="height">The height.</param>
        ///// <param name="returnLocalUrl">if set to <c>true</c> [return local URL].</param>
        ///// <returns></returns>
        //public string ApplyForcedMaximumHeight(System.Web.UI.WebControls.Image image, int height, bool returnLocalUrl)
        //{
        //    return ApplyForcedMaximumHeight(image, height, returnLocalUrl, false);
        //}

        ///// <summary>
        ///// Applies a forced height. The image is physically scaled!
        ///// </summary>
        ///// <param name="image">The image.</param>
        ///// <param name="height">The height.</param>
        ///// <param name="returnLocalUrl">if set to <c>true</c> [return local URL].</param>
        ///// <param name="skipGalleryMapping">if set to <c>true</c> [skip gallery mapping].</param>
        ///// <returns></returns>
        //public string ApplyForcedMaximumHeight(System.Web.UI.WebControls.Image image, int height, bool returnLocalUrl, bool skipGalleryMapping)
        //{
        //    return ParseImage(image, skipGalleryMapping, returnLocalUrl, Sushi.Mediakiwi.Data.ImageFileFormat.ValidateMaximumHeight, ImagePosition.Center, 0, height, null);

        //}

        ///// <summary>
        ///// Applies the forced maximum.
        ///// </summary>
        ///// <param name="image">The image.</param>
        ///// <param name="width">The width.</param>
        ///// <param name="height">The height.</param>
        ///// <returns></returns>
        //public string ApplyForcedMaximum(System.Web.UI.WebControls.Image image, int width, int height)
        //{
        //    return ApplyForcedMaximum(image, width, height, false);
        //}

        ///// <summary>
        ///// Applies the forced maximum.
        ///// </summary>
        ///// <param name="image">The image.</param>
        ///// <param name="width">The width.</param>
        ///// <param name="height">The height.</param>
        ///// <param name="returnLocalUrl">if set to <c>true</c> [return local URL].</param>
        ///// <returns></returns>
        //public string ApplyForcedMaximum(System.Web.UI.WebControls.Image image, int width, int height, bool returnLocalUrl)
        //{
        //    return ApplyForcedMaximum(image, width, height, returnLocalUrl, false);
        //}

        ///// <summary>
        ///// Applies the forced maximum.
        ///// </summary>
        ///// <param name="image">The image.</param>
        ///// <param name="width">The width.</param>
        ///// <param name="height">The height.</param>
        ///// <param name="returnLocalUrl">if set to <c>true</c> [return local URL].</param>
        ///// <param name="skipGalleryMapping">if set to <c>true</c> [skip gallery mapping].</param>
        ///// <returns></returns>
        ///// <exception cref="System.Exception">Can not generate an image with 0 as dimensions.</exception>
        //internal string ApplyForcedMaximum(System.Web.UI.WebControls.Image image, int width, int height, bool returnLocalUrl, bool skipGalleryMapping)
        //{
        //    if (width == 0 || height == 0)
        //    {
        //        if (width == 0)
        //            return ApplyForcedMaximumHeight(image, height, returnLocalUrl);

        //        if (height == 0)
        //            return ApplyForcedMaximumWidth(image, width, returnLocalUrl);

        //        throw new Exception("Can not generate an image with 0 as dimensions.");
        //    }

        //    return ParseImage(image, skipGalleryMapping, returnLocalUrl, Sushi.Mediakiwi.Data.ImageFileFormat.ValidateMaximumWidthAndHeight, ImagePosition.Center, width, height, null);

        //    //int[] bgColor = Wim.CommonConfiguration.DEFAULT_THUMB_BGCOLOR;

        //    //string conc = Sushi.Mediakiwi.Data.Image.GetCdnHash(this.ID, Sushi.Mediakiwi.Data.ImageFileFormat.ValidateMaximumWidthAndHeight, width, height, bgColor, this.CompletePath, false);

        //    //bool useRelyOnGeneratedAssets = RelyOnGeneratedAssets && !(string.IsNullOrEmpty(this.RemoteLocation) && this.GalleryUrlMap != null);
        //    //if (useRelyOnGeneratedAssets)
        //    //{
        //    //    string hash = Wim.Utilities.Encryption.Encrypt(String.Concat(this.FileName, ",", conc), "cdn", true);

        //    //    string portal = null;
        //    //    if (this.DatabaseMappingPortal != null)
        //    //        portal = this.DatabaseMappingPortal.Name;
        //    //    GeneratedAsset ga = GeneratedAsset.SelectOneByGenerationHash(hash, ID, portal);

        //    //    if (ga == null || ga.ID < 1)
        //    //    {
        //    //        string location = GenerateANewImageForcedMax(image, conc, width, height);

        //    //        ga = new GeneratedAsset(portal);
        //    //        if (!string.IsNullOrEmpty(location))
        //    //        {
        //    //            ga.AssetID = this.ID;
        //    //            ga.Location = location;
        //    //            ga.Created = DateTime.Now;
        //    //            ga.GenerationHash = hash;
        //    //            try
        //    //            {
        //    //                ga.Save();
        //    //            }
        //    //            catch
        //    //            {
        //    //                //could be a multi threading thing (some other process allready created the asset), so first check if it exists now
        //    //                ga = GeneratedAsset.SelectOneByGenerationHash(hash, ID, portal);
        //    //                if (ga == null || ga.ID < 1)
        //    //                    throw;
        //    //            }
        //    //        }
        //    //    }
        //    //    if (ga != null && image != null)
        //    //    {
        //    //        if (!String.IsNullOrEmpty(ga.Location))
        //    //        {

        //    //            image.ImageUrl = ga.Location;
        //    //            image.Visible = true;
        //    //        }
        //    //        else
        //    //            image.Visible = false;
        //    //        return ga.Location;
        //    //    }
        //    //    else if (ga != null)
        //    //        return ga.Location;
        //    //    else
        //    //        return null;
        //    //}
        //    //else
        //    //{
        //    //    if (this.GalleryUrlMap != null)
        //    //    {
        //    //        string path = string.Format("{0}/cdn/{1}/{2}.jpg"
        //    //            , this.GalleryUrlMap.MappedUrl
        //    //            , Wim.Utilities.Encryption.Encrypt(conc, "cdn", true)
        //    //            , this.ID
        //    //            );

        //    //        if (image != null)
        //    //            image.ImageUrl = path;

        //    //        return path;
        //    //    }

        //    //    if (returnLocalUrl)
        //    //    {
        //    //        return Wim.Utility.AddApplicationPath(string.Format("/cdn/{0}/{1}.jpg"
        //    //            , Wim.Utilities.Encryption.Encrypt(conc, "cdn", true)
        //    //            , this.ID
        //    //            ));
        //    //    }
        //    //    if (HasCloudSetting)
        //    //    {
        //    //        if (image != null)
        //    //        {
        //    //            string filecheck = Wim.Utility.AddApplicationPath(string.Format("/cdn/{0}/{1}.jpg"
        //    //              , Wim.Utilities.Encryption.Encrypt(conc, "cdn", true)
        //    //              , this.ID
        //    //              ));
        //    //            image.ImageUrl = filecheck;
        //    //            return filecheck;
        //    //        }
        //    //        else
        //    //        {
        //    //            string filecheck = string.Format("{0}/{1}.jpg"
        //    //                , Wim.Utilities.Encryption.Encrypt(conc, "cdn", true)
        //    //                , this.ID
        //    //                );

        //    //            if (Sushi.Mediakiwi.Data.Asset.m_CloundInstance.ExistsGeneratedContent(filecheck))
        //    //            {
        //    //                return string.Concat(Sushi.Mediakiwi.Data.Asset.m_CloundInstance.ContentDeliveryUrl, Sushi.Mediakiwi.Data.Asset.m_CloundInstance.ContainerGeneratedContent, "/", filecheck);
        //    //            }
        //    //        }
        //    //    }

        //    //    if (this.ID == 0)
        //    //    {
        //    //        image.Visible = false;
        //    //        return null;
        //    //    }
        //    //}
        //    //return GenerateANewImageForcedMax(image, conc, width, height);
        //}


        ///// <summary>
        ///// This option has two possible correction outcomes:
        ///// If canvas is bigger the image is left as is and placed at coordinate (0,0)
        ///// If canvas is smaller the image is simply cut viewed from coordinate (0,0), so cutting the bottom/right sections.
        ///// </summary>
        ///// <param name="image">The image.</param>
        ///// <param name="width">The width.</param>
        ///// <param name="height">The height.</param>
        //public string ApplyForcedBorder(System.Web.UI.WebControls.Image image, int width, int height)
        //{
        //    return ApplyForcedBorder(image, width, height, Wim.CommonConfiguration.DEFAULT_THUMB_BGCOLOR, true);
        //}

        ///// <summary>
        ///// Applies the forced border no crop.
        ///// </summary>
        ///// <param name="image">The image.</param>
        ///// <param name="width">The width.</param>
        ///// <param name="height">The height.</param>
        ///// <returns></returns>
        //public string ApplyForcedBorderNoCrop(System.Web.UI.WebControls.Image image, int width, int height)
        //{
        //    return ApplyForcedBorder(image, width, height, Wim.CommonConfiguration.DEFAULT_THUMB_BGCOLOR, false, ImagePosition.Center);
        //}

        //public string ApplyForcedBorder(System.Web.UI.WebControls.Image image, int width, int height, bool returnLocalUrl)
        //{
        //    return ApplyForcedBorder(image, width, height, Wim.CommonConfiguration.DEFAULT_THUMB_BGCOLOR, false, ImagePosition.Center, null, returnLocalUrl, false);
        //}

        ///// <summary>
        ///// Scales the image to the specified size .
        ///// </summary>
        ///// <param name="image">The image to apply this image to (or NULL to get URL)</param>
        ///// <param name="width">The desired width of the resulting image</param>
        ///// <param name="height">The desired height of the resulting image</param>
        ///// <param name="bgColor">The desired Background Fillcolor of the resulting image (in Hex : #FFFFFF)</param>
        ///// <returns></returns>
        //public string ApplyForcedBorderNoCrop(System.Web.UI.WebControls.Image image, int width, int height, string bgColor)
        //{
        //    Color color = System.Drawing.ColorTranslator.FromHtml(bgColor);
        //    List<int> cols = new List<int>();
        //    if (color != null)
        //    {
        //        cols.Add((int)color.R);
        //        cols.Add((int)color.G);
        //        cols.Add((int)color.B);
        //    }
        //    else
        //    {
        //        cols.Add(Wim.CommonConfiguration.DEFAULT_THUMB_BGCOLOR[0]); // R
        //        cols.Add(Wim.CommonConfiguration.DEFAULT_THUMB_BGCOLOR[1]); // G
        //        cols.Add(Wim.CommonConfiguration.DEFAULT_THUMB_BGCOLOR[2]); // B
        //    }

        //    return ApplyForcedBorder(image, width, height, cols.ToArray(), false, ImagePosition.Center, null, false, false);
        //}
        ///// <summary>
        ///// Scales the image to the specified size .
        ///// </summary>
        ///// <param name="image">The image to apply this image to (or NULL to get URL)</param>
        ///// <param name="width">The desired width of the resulting image</param>
        ///// <param name="height">The desired height of the resulting image</param>
        ///// <param name="bgColor">The desired Background Fillcolor of the resulting image (R, G, B values)</param>
        ///// <returns></returns>
        //public string ApplyForcedBorderNoCrop(System.Web.UI.WebControls.Image image, int width, int height, int[] bgColor)
        //{
        //    return ApplyForcedBorder(image, width, height, bgColor, false, ImagePosition.Center, null, false, false);
        //}

        ///// <summary>
        ///// Applies the forced border.
        ///// </summary>
        ///// <param name="image">The image.</param>
        ///// <param name="width">The width.</param>
        ///// <param name="height">The height.</param>
        ///// <param name="backgroundRgb">The background RGB.</param>
        //public string ApplyForcedBorder(System.Web.UI.WebControls.Image image, int width, int height, int[] backgroundRgb)
        //{
        //    return ApplyForcedBorder(image, width, height, backgroundRgb, true);
        //}

        ///// <summary>
        ///// Applies the forced border.
        ///// </summary>
        ///// <param name="image">The image.</param>
        ///// <param name="width">The width.</param>
        ///// <param name="height">The height.</param>
        ///// <param name="backgroundRgb">The background RGB.</param>
        ///// <param name="CropImage">if set to <c>true</c> [crop image].</param>
        ///// <returns></returns>
        //public string ApplyForcedBorder(System.Web.UI.WebControls.Image image, int width, int height, int[] backgroundRgb, bool CropImage)
        //{
        //    return ApplyForcedBorder(image, width, height, backgroundRgb, CropImage, ImagePosition.Center);
        //}

        ///// <summary>
        ///// Applies the forced border.
        ///// </summary>
        ///// <param name="image">The image.</param>
        ///// <param name="width">The width.</param>
        ///// <param name="height">The height.</param>
        ///// <param name="backgroundRgb">The background RGB.</param>
        ///// <param name="CropImage">if set to <c>true</c> [crop image].</param>
        ///// <param name="position">The position.</param>
        ///// <returns></returns>
        //public string ApplyForcedBorder(System.Web.UI.WebControls.Image image, int width, int height, int[] backgroundRgb, bool CropImage, ImagePosition position)
        //{
        //    return ApplyForcedBorder(image, width, height, backgroundRgb, CropImage, position, null);
        //}

        ///// <summary>
        ///// Applies the forced border.
        ///// </summary>
        ///// <param name="image">The image.</param>
        ///// <param name="width">The width.</param>
        ///// <param name="height">The height.</param>
        ///// <param name="backgroundRgb">The background RGB.</param>
        ///// <param name="CropImage">if set to <c>true</c> [crop image].</param>
        ///// <param name="position">The position.</param>
        ///// <param name="galleryToSaveTo">The gallery to save to.</param>
        ///// <returns></returns>
        //public string ApplyForcedBorder(System.Web.UI.WebControls.Image image, int width, int height, int[] backgroundRgb, bool CropImage, ImagePosition position, Sushi.Mediakiwi.Data.Gallery galleryToSaveTo)
        //{
        //    return ApplyForcedBorder(image, width, height, backgroundRgb, CropImage, position, galleryToSaveTo, false, false);
        //}

        ///// <summary>
        ///// Applies the forced border.
        ///// </summary>
        ///// <param name="image">The image.</param>
        ///// <param name="width">The width.</param>
        ///// <param name="height">The height.</param>
        ///// <param name="backgroundRgb">The background RGB.</param>
        ///// <param name="CropImage">if set to <c>true</c> [crop image].</param>
        ///// <param name="position">The position.</param>
        ///// <param name="galleryToSaveTo">The gallery to save to.</param>
        ///// <param name="returnLocalUrl">if set to <c>true</c> [return local URL].</param>
        ///// <param name="skipGalleryMapping">if set to <c>true</c> [skip gallery mapping].</param>
        ///// <returns></returns>
        //internal string ApplyForcedBorder(System.Web.UI.WebControls.Image image, int width, int height, int[] backgroundRgb, bool CropImage, ImagePosition position, Sushi.Mediakiwi.Data.Gallery galleryToSaveTo, bool returnLocalUrl, bool skipGalleryMapping)
        //{
        //    return ParseImage(image, skipGalleryMapping, returnLocalUrl, Sushi.Mediakiwi.Data.ImageFileFormat.ValidateMaximumAndCrop, ImagePosition.Center, width, height, backgroundRgb);

        //    //if (backgroundRgb == null || backgroundRgb.Length != 3)
        //    //    backgroundRgb = Wim.CommonConfiguration.DEFAULT_THUMB_BGCOLOR;

        //    //string conc = Sushi.Mediakiwi.Data.Image.GetCdnHash(this.ID
        //    //    , CropImage
        //    //        ? Sushi.Mediakiwi.Data.ImageFileFormat.ValidateMaximumAndCrop
        //    //        : Sushi.Mediakiwi.Data.ImageFileFormat.FixedBorder
        //    //    , width, height, backgroundRgb, this.CompletePath, false);

        //    //string conc = string.Format("{0},{1},{2},{3},{4},{5},{6}"
        //    //    , CropImage
        //    //        ? (int)Sushi.Mediakiwi.Data.ImageFileFormat.ValidateMaximumAndCrop
        //    //        : (int)Sushi.Mediakiwi.Data.ImageFileFormat.FixedBorder
        //    //    , width, height, backgroundRgb[0], backgroundRgb[1], backgroundRgb[2]
        //    //    , this.CompletePath);

        //    ////edit DV 2013-05-16: issue found that generatedAssets only works with remoteLocation and localimages
        //    ////images hosted on another server (i.e. privilege beheer) don't work, because GenerateANewImage looks for a local file then
        //    ////I don't want to mess around in GenerateANewImage, so if an image has no remotelocation, but does have GalleryUrlMap, we use the old method
        //    //bool useRelyOnGeneratedAssets = RelyOnGeneratedAssets && !(string.IsNullOrEmpty(this.RemoteLocation) && this.GalleryUrlMap != null);
        //    //if (useRelyOnGeneratedAssets)
        //    //{
        //    //    string hash = Wim.Utilities.Encryption.Encrypt(String.Concat(this.FileName, ",", conc), "cdn", true);

        //    //    string portal = null;
        //    //    if (this.DatabaseMappingPortal != null)
        //    //        portal = this.DatabaseMappingPortal.Name;

        //    //    GeneratedAsset ga = GeneratedAsset.SelectOneByGenerationHash(hash, ID, portal);

        //    //    if (ga == null || ga.ID < 1)
        //    //    {
        //    //        string location = GenerateANewImage(image, conc, CropImage, width, height, backgroundRgb, position, galleryToSaveTo);

        //    //        if (!string.IsNullOrEmpty(location))
        //    //        {
        //    //            ga = new GeneratedAsset(portal);
        //    //            ga.AssetID = this.ID;
        //    //            ga.Location = location;
        //    //            ga.Created = DateTime.Now;
        //    //            ga.GenerationHash = hash;
        //    //            try
        //    //            {
        //    //                ga.Save();
        //    //            }
        //    //            catch
        //    //            {
        //    //                //could be a multi threading thing (some other process allready created the asset), so first check if it exists now
        //    //                ga = GeneratedAsset.SelectOneByGenerationHash(hash, ID, portal);
        //    //                if (ga == null || ga.ID < 1)
        //    //                    throw;
        //    //            }
        //    //        }
        //    //    }
        //    //    if (ga != null && image != null)
        //    //    {
        //    //        if (!String.IsNullOrEmpty(ga.Location))
        //    //        {

        //    //            image.ImageUrl = ga.Location;
        //    //            image.Visible = true;
        //    //        }
        //    //        else
        //    //        {
        //    //            image.Visible = false;
        //    //        }
        //    //        return ga.Location;
        //    //    }
        //    //    else if (ga != null)
        //    //        return ga.Location;
        //    //    else
        //    //        return null;


        //    //}
        //    //else
        //    //{

        //    //    if (this.GalleryUrlMap != null)
        //    //    {
        //    //        string path = string.Format("{0}/cdn/{1}/{2}.jpg"
        //    //            , this.GalleryUrlMap.MappedUrl
        //    //            , Wim.Utilities.Encryption.Encrypt(conc, "cdn", true)
        //    //            , this.ID
        //    //            );

        //    //        if (image != null)
        //    //            image.ImageUrl = path;

        //    //        return path;
        //    //    }

        //    //    if (returnLocalUrl)
        //    //    {
        //    //        return Wim.Utility.AddApplicationPath(string.Format("/cdn/{0}/{1}.jpg"
        //    //            , Wim.Utilities.Encryption.Encrypt(conc, "cdn", true)
        //    //            , this.ID
        //    //            ));
        //    //    }

        //    //    if (HasCloudSetting)
        //    //    {
        //    //        if (image != null)
        //    //        {
        //    //            string filecheck = Wim.Utility.AddApplicationPath(string.Format("/cdn/{0}/{1}.jpg"
        //    //              , Wim.Utilities.Encryption.Encrypt(conc, "cdn", true)
        //    //              , this.ID
        //    //              ));
        //    //            image.ImageUrl = filecheck;
        //    //            return filecheck;
        //    //        }
        //    //        else
        //    //        {
        //    //            string filecheck = string.Format("{0}/{1}.jpg"
        //    //                , Wim.Utilities.Encryption.Encrypt(conc, "cdn", true)
        //    //                , this.ID
        //    //                );

        //    //            if (Sushi.Mediakiwi.Data.Asset.m_CloundInstance.ExistsGeneratedContent(filecheck))
        //    //            {
        //    //                return string.Concat(Sushi.Mediakiwi.Data.Asset.m_CloundInstance.ContentDeliveryUrl, Sushi.Mediakiwi.Data.Asset.m_CloundInstance.ContainerGeneratedContent, "/", filecheck);
        //    //            }
        //    //        }
        //    //    }

        //    //    if (this.ID == 0)
        //    //    {
        //    //        if (image != null) image.Visible = false;
        //    //        return null;
        //    //    }
        //    //    return GenerateANewImage(image, conc, CropImage, width, height, backgroundRgb, position, galleryToSaveTo);
        //    //}
        //}
        ///// <summary>
        ///// Generates a new image for Forced max
        ///// </summary>
        ///// <param name="image">The asp.net image</param>
        ///// <param name="conc">The hash of the parameters to save the generated image by</param>
        ///// <param name="width">The width of the new image</param>
        ///// <param name="height">The height of the new image</param>
        ///// <returns></returns>
        ////private string GenerateANewImageForcedMax(System.Web.UI.WebControls.Image image, string conc, int width, int height)
        ////{
        ////    Sushi.Mediakiwi.Data.Image tmp = this.CreateAttachedImageSize(width, height, ImageFileFormat.ValidateMaximumWidthAndHeight);

        ////    if (tmp == null)
        ////    {
        ////        if (image != null)
        ////            image.Visible = false;
        ////        return null;
        ////    }
        ////    if (image != null)
        ////    {
        ////        image.ImageUrl = tmp.CdnImagePath;
        ////        image.Visible = true;
        ////    }

        ////    if (HasCloudSetting)
        ////    {
        ////        string filecheck = string.Format("{0}/{1}.jpg"
        ////            , Wim.Utilities.Encryption.Encrypt(conc, "cdn", true)
        ////            , this.ID
        ////            );

        ////        if (Sushi.Mediakiwi.Data.Asset.m_CloundInstance.ExistsGeneratedContent(filecheck))
        ////        {
        ////            return string.Concat(Sushi.Mediakiwi.Data.Asset.m_CloundInstance.ContentDeliveryUrl, Sushi.Mediakiwi.Data.Asset.m_CloundInstance.ContainerGeneratedContent, "/", filecheck);
        ////        }
        ////        return tmp.CdnImagePath;
        ////    }
        ////    else
        ////        return tmp.GeneratedImagePath;
        ////}

        ///// <summary>
        ///// Generates the new image based on the params. This code has been seperated from <c>ApplyForcedBorder</c>
        ///// </summary>
        ///// <param name="image">The image control</param>
        ///// <param name="conc">The hash of the parameters to save the generated image by</param>
        ///// <param name="cropImage">Crop the image</param>
        ///// <param name="width">The new width</param>
        ///// <param name="height">The new height</param>
        ///// <param name="backgroundRgb">The background</param>
        ///// <param name="position">The position</param>
        ///// <param name="galleryToSaveTo">The Gallery</param>
        ///// <returns>Location to which is has been saved</returns>
        //private string GenerateANewImage(System.Web.UI.WebControls.Image image, string conc, bool cropImage, int width, int height, int[] backgroundRgb, ImagePosition position, Sushi.Mediakiwi.Data.Gallery galleryToSaveTo)
        //{
        //    Sushi.Mediakiwi.Data.Image tmp = new Image();

        //    if (cropImage)
        //        tmp = this.CreateAttachedImageSize(width, height, ImageFileFormat.ValidateMaximumAndCrop, backgroundRgb, position);//Sushi.Mediakiwi.Data.ImagePosition.Center);
        //    else
        //        tmp = this.CreateAttachedImageSize(width, height, ImageFileFormat.FixedBorder, backgroundRgb, position, galleryToSaveTo);



        //    if (tmp == null || tmp.IsNewInstance)
        //    {
        //        if (image != null) image.Visible = false;
        //        return null;
        //    }
        //    else
        //    {
        //        //  Save a new instance of the image to a different location.
        //        if (galleryToSaveTo != null)
        //        {
        //            tmp.ID = 0;
        //            tmp.Save();
        //        }
        //    }

        //    if (image != null)
        //    {
        //        image.ImageUrl = tmp.CdnImagePath;
        //        image.Visible = true;
        //    }

        //    if (!HasCloudSetting)
        //    {
        //        // Check for newer version added by Casper
        //        string cahcedPath = HttpContext.Current.Server.MapPath(tmp.GeneratedImagePath);
        //        if (System.IO.File.Exists(cahcedPath) && this.Created > System.IO.File.GetLastWriteTime(cahcedPath))
        //        {
        //            System.IO.File.Delete(cahcedPath);
        //            return ApplyForcedBorder(image, width, height, backgroundRgb, cropImage, position, galleryToSaveTo);
        //        }
        //        return tmp.GeneratedImagePath;
        //    }
        //    else
        //    {
        //        string filecheck = string.Format("{0}/{1}.jpg"
        //            , Wim.Utilities.Encryption.Encrypt(conc, "cdn", true)
        //            , this.ID
        //            );
        //        if (Sushi.Mediakiwi.Data.Asset.m_CloundInstance.ExistsGeneratedContent(filecheck))
        //        {
        //            return string.Concat(Sushi.Mediakiwi.Data.Asset.m_CloundInstance.ContentDeliveryUrl, Sushi.Mediakiwi.Data.Asset.m_CloundInstance.ContainerGeneratedContent, "/", filecheck);
        //        }
        //        return null;
        //    }
        //}

        ///// <summary>
        ///// Apply the image properties to a Image webcontrol with a set height (set url, toolTip and width/height).
        ///// The height is calculated based on the rescaling factor
        ///// </summary>
        ///// <param name="image">The image.</param>
        ///// <param name="height">The height.</param>
        //public void ApplyScaleHeight(System.Web.UI.WebControls.Image image, int height)
        //{
        //    if (Height != 0 && Width != 0 && height > 0)
        //    {
        //        decimal factor = decimal.Divide(height, Height);
        //        decimal newWidth = decimal.Round(decimal.Multiply(factor, Width), 0);
        //        this.Width = Convert.ToInt32(newWidth);
        //        this.Height = height;
        //    }
        //    Apply(image);
        //}


        #endregion MOVED to EXTENSION / LOGIC

        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard



        string m_Format;
        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        /// <value>The format.</value>
        public string Format
        {
            get { return m_Format; }
            set { m_Format = value; }
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static new Image SelectOne(int ID)
        {
            Image t = new Image();
            Wim.Utility.ReflectProperty(Asset.SelectOne(ID), t);
            if (t.IsImage)
                return t;
            else
                return new Image();
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static new Image SelectOne(Guid ID)
        {
            Image t = new Image();
            Wim.Utility.ReflectProperty(Asset.SelectOne(ID), t);
            if (t.IsImage)
                return t;
            else
                return new Image();
        }
      
        /// <summary>
        /// Selects the all_by parent.
        /// </summary>
        /// <param name="galleryID">The gallery ID.</param>
        /// <returns></returns>
        public static new Image[] SelectAll(int galleryID)
        {
            List<Image> list = new List<Image>();
            Image implement = new Image();

            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("Asset_IsImage", SqlDbType.Bit, true));
            whereClause.Add(new DatabaseDataValueColumn("Asset_Gallery_Key", SqlDbType.Int, galleryID));
            whereClause.Add(new DatabaseDataValueColumn("Asset_IsArchived", SqlDbType.Bit, false));
            whereClause.Add(new DatabaseDataValueColumn("Asset_IsActive", SqlDbType.Bit, true));

            foreach (object o in implement._SelectAll(whereClause))
                list.Add((Image)o);
            return list.ToArray();
        }

        public static Image[] SelectAll(int galleryID, int assetTypeID)
        {
            return SelectAll(galleryID, assetTypeID, false);
        }

        public static Image[] SelectAll(int galleryID, bool onErrorIgnore)
        {
            int? i = null;
            return SelectAll(galleryID, i, onErrorIgnore);
        }

        /// <summary>
        /// Selects the all_by parent.
        /// </summary>
        /// <param name="galleryID">The gallery ID.</param>
        /// <returns></returns>
        public static Image[] SelectAll(int galleryID, int? assetTypeID, bool onErrorIgnore)
        {
            List<Image> list = new List<Image>();
            Image implement = new Image();

            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("Asset_Gallery_Key", SqlDbType.Int, galleryID));
            whereClause.Add(new DatabaseDataValueColumn("Asset_IsActive", SqlDbType.Bit, true));
            if (assetTypeID.HasValue)
                whereClause.Add(new DatabaseDataValueColumn("Asset_AssetType_Key", SqlDbType.Int, assetTypeID.Value));

            foreach (object o in implement._SelectAll(whereClause))
            {
                var asset = (Image)o;

                //bool hasRemoteError;
                //ParseRemoteLocation(null, out hasRemoteError);
                //if (onErrorIgnore && hasRemoteError)
                //    continue;

                list.Add(asset);
            }
            return list.ToArray();
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        internal static Image[] SelectAll()
        {
            List<Image> list = new List<Image>();
            Image implement = new Image();

            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("Asset_IsImage", SqlDbType.Bit, true));
            whereClause.Add(new DatabaseDataValueColumn("Asset_IsArchived", SqlDbType.Bit, false));

            foreach (object o in implement._SelectAll(whereClause))
                list.Add((Image)o);
            return list.ToArray();
        }

        public static List<Image> SelectRange(List<int> IDs)
        {
            List<Image> list = new List<Image>();
            Image implement = new Image();

            string arrText = Wim.Utility.ConvertToCsvString(IDs.ToArray());
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("Asset_Key", SqlDbType.Int, arrText, DatabaseDataValueCompareType.In));

            foreach (object o in new Image()._SelectAll(whereClause))
            {
                var asset = (Image)o;
                list.Add(asset);
            }

            return list;
        }


        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="imageArr">The image arr.</param>
        /// <returns></returns>
        public static Sushi.Mediakiwi.Data.Image[] SelectAll(int[] imageArr)
        {
            List<Image> list = new List<Image>();
            Image implement = new Image();

            string arrText = Wim.Utility.ConvertToCsvString(imageArr);

            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("Asset_IsImage", SqlDbType.Bit, true));
            whereClause.Add(new DatabaseDataValueColumn("Asset_Key", SqlDbType.Int, arrText, DatabaseDataValueCompareType.In));
            whereClause.Add(new DatabaseDataValueColumn("Asset_IsArchived", SqlDbType.Bit, false));

            foreach (object o in implement._SelectAll(whereClause))
                list.Add((Image)o);

            if (list.Count == 0) return list.ToArray();

            List<Image> sortedList = new List<Image>();
            foreach (int index in imageArr)
            {
                Image found = null;
                foreach (Image gfx in list)
                {
                    if (gfx.ID == index)
                    {
                        found = gfx;
                        sortedList.Add(gfx);
                        break;
                    }
                }
                if (found != null)
                    list.Remove(found);
            }
            return sortedList.ToArray();
        }

        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard
    }
}