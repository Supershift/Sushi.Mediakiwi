using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

public static class ImageExtension 
{

    /// <summary>
    /// Creates the thumbnail.
    /// </summary>
    internal static void CreateThumbnail(this Sushi.Mediakiwi.Data.Image inImage)
    {
        if (!File.Exists(inImage.LocalFilePath))
            return;

        if (File.Exists(inImage.LocalThumbnailFilePath))
            return;

        using (System.Drawing.Image imageStreamed = System.Drawing.Image.FromFile(inImage.LocalFilePath))
        {
            //  Create the image.
            int size = 0, newWidth = 0, newHeight = 0;

            //  Without APP PATH!
            string fileThumbnailPath = HttpContext.Current.Server.MapPath(Wim.Utility.AddApplicationPath(string.Concat(Wim.CommonConfiguration.RelativeRepositoryImageThumbnailUrl, inImage.ID, ".jpg")));
            inImage.CreateNewSizeImage(imageStreamed, fileThumbnailPath, 95, 95, ref size, ref newWidth, ref newHeight, true, null);
        }
    }

    /// <summary>
    /// Apply the thumbnail image properties to a Image webcontrol (set url, toolTip and width/height)
    /// </summary>
    /// <param name="image"></param>
    public static void ApplyThumbnail(this Sushi.Mediakiwi.Data.Image inImage, System.Web.UI.WebControls.Image image)
    {
        image.Visible = false;
        if (inImage.ThumbnailPath == null || inImage.ThumbnailPath.Trim().Length == 0)
            return;

        image.ImageUrl = inImage.ThumbnailPath;

        if (inImage.Height != 0 && inImage.Width != 0)
        {
            image.Height = inImage.Height;
            image.Width = inImage.Width;
        }

        image.AlternateText = inImage.Description == null ? string.Empty : inImage.Description;
        image.Visible = true;
    }

    /// <summary>
    /// Apply the image properties to a Image webcontrol (set url, toolTip and width/height)
    /// </summary>
    /// <param name="image"></param>
    public static void Apply(this Sushi.Mediakiwi.Data.Image inImage, System.Web.UI.WebControls.Image image)
    {
        image.Visible = false;
        if (inImage.Path == null || inImage.Path.Trim().Length == 0)
            return;

        image.ImageUrl = inImage.Path;

        if (inImage.Height != 0)
            image.Height = inImage.Height;
        if (inImage.Width != 0)
            image.Width = inImage.Width;

        image.AlternateText = inImage.Description == null ? string.Empty : inImage.Description;
        image.Visible = true;
    }

    /// <summary>
    /// Applies the specified image.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="setFileFormatWidth">Width of the set file format.</param>
    /// <param name="setFileFormatHeigth">The set file format heigth.</param>
    public static void Apply(this Sushi.Mediakiwi.Data.Image inImage, System.Web.UI.WebControls.Image image, int setFileFormatWidth, int setFileFormatHeigth)
    {
        image.Visible = false;
        if (inImage.Path == null || inImage.Path.Trim().Length == 0)
            return;

        Sushi.Mediakiwi.Data.ImageFile foundFile = null;

        if (foundFile == null)
            return;

        inImage.Width = foundFile.Width;
        inImage.Height = foundFile.Height;
        string fileName = string.Concat(foundFile.Name, ".", foundFile.Extention);

        inImage.ApplyPath(fileName, false, true);

        image.ImageUrl = inImage.Path;

        if (inImage.Height != 0 && inImage.Width != 0)
        {
            image.Height = inImage.Height;
            image.Width = inImage.Width;
        }
        image.AlternateText = inImage.Description == null ? string.Empty : inImage.Description;
        image.Visible = true;
    }

    /// <summary>
    /// Apply the image properties to a Image webcontrol with a set width (set url, toolTip and width/height).
    /// The height is calculated based on the rescaling factor
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="width">The width.</param>
    public static void ApplyScaleWidth(this Sushi.Mediakiwi.Data.Image inImage, System.Web.UI.WebControls.Image image, int width)
    {
        if (inImage.Height != 0 && inImage.Width != 0 && width > 0)
        {
            decimal factor = decimal.Divide(width, inImage.Width);
            decimal newHeight = decimal.Round(decimal.Multiply(factor, inImage.Height), 0);
            inImage.Height = Convert.ToInt32(newHeight);
            inImage.Width = width;
        }
        Apply(inImage, image);
    }


    /// <summary>
    /// Applies the maximum width of the forced.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="width">The width.</param>
    /// <param name="returnLocalUrl">if set to <c>true</c> [return local URL].</param>
    /// <returns></returns>
    public static string ApplyForcedMaximumWidth(this Sushi.Mediakiwi.Data.Image inImage, System.Web.UI.WebControls.Image image, int width, bool returnLocalUrl)
    {
        return ApplyForcedMaximumWidth(inImage, image, width, Sushi.Mediakiwi.Data.ImagePosition.Center, returnLocalUrl);
    }

    /// <summary>
    /// Applies the maximum width of the forced.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="width">The width.</param>
    /// <returns></returns>
    public static string ApplyForcedMaximumWidth(this Sushi.Mediakiwi.Data.Image inImage, System.Web.UI.WebControls.Image image, int width)
    {
        return ApplyForcedMaximumWidth(inImage, image, width, Sushi.Mediakiwi.Data.ImagePosition.Center);
    }

    /// <summary>
    /// Applies the maximum width of the forced.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="width">The width.</param>
    /// <param name="position">The position.</param>
    /// <returns></returns>
    public static string ApplyForcedMaximumWidth(this Sushi.Mediakiwi.Data.Image inImage, System.Web.UI.WebControls.Image image, int width, Sushi.Mediakiwi.Data.ImagePosition position)
    {
        return ApplyForcedMaximumWidth(inImage, image, width, position, false);
    }

    /// <summary>
    /// Applies the maximum width of the forced.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="width">The width.</param>
    /// <param name="position">The position.</param>
    /// <param name="returnLocalUrl">if set to <c>true</c> [return local URL].</param>
    /// <returns></returns>
    public static string ApplyForcedMaximumWidth(this Sushi.Mediakiwi.Data.Image inImage, System.Web.UI.WebControls.Image image, int width, Sushi.Mediakiwi.Data.ImagePosition position, bool returnLocalUrl)
    {
        return ApplyForcedMaximumWidth(inImage, image, width, position, returnLocalUrl, false);
    }

    /// <summary>
    /// Parses the image.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="skipGalleryMapping">if set to <c>true</c> [skip gallery mapping].</param>
    /// <param name="returnLocalUrl">if set to <c>true</c> [return local URL].</param>
    /// <param name="format">The format.</param>
    /// <param name="position">The position.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="bgColor">Color of the bg.</param>
    /// <returns></returns>
   internal static string ParseImage(this Sushi.Mediakiwi.Data.Image inImage, System.Web.UI.WebControls.Image image, bool skipGalleryMapping, bool returnLocalUrl, Sushi.Mediakiwi.Data.ImageFileFormat format, Sushi.Mediakiwi.Data.ImagePosition position, int width, int height, int[] bgColor)
    {
        if (bgColor == null || bgColor.Length != 3)
            bgColor = Wim.CommonConfiguration.DEFAULT_THUMB_BGCOLOR;

        string hash = Sushi.Mediakiwi.Data.Image.GetCdnHash(
            inImage.ID, 
            format, 
            width, 
            height, 
            bgColor, 
            inImage.CompletePath, 
            true);

        //  Skip gallery mapping from handler as this already is mapped.
        if (!skipGalleryMapping)
        {
            if (inImage.GalleryUrlMap != null)
            {
                string portal = null;
                if (!string.IsNullOrEmpty(inImage.GalleryUrlMap.Portal))
                    portal = string.Concat("/", Wim.Utilities.Encryption.Encrypt(inImage.GalleryUrlMap.Portal, "cdn", true));

                string path = string.Format("{0}/cdn{2}/{1}/{3}.jpg"
                    , inImage.GalleryUrlMap.MappedUrl
                    , hash
                    , portal
                    , inImage.ID
                    );

                path = Sushi.Mediakiwi.Framework.AssetHandler.ScaledAssetCacheManager.GetUrl(path);

                if (image != null)
                    image.ImageUrl = path;

                return path;
            }
        }

        if (returnLocalUrl)
        {
            string filecheck = Wim.Utility.AddApplicationPath(string.Format("/cdn/{0}/{1}.jpg"
                , hash
                , inImage.ID
                ));
            return Sushi.Mediakiwi.Framework.AssetHandler.ScaledAssetCacheManager.GetUrl(filecheck);

        }
        if (Sushi.Mediakiwi.Data.Asset.HasCloudSetting)
        {
            if (image != null)
            {
                string filecheck = Wim.Utility.AddApplicationPath(string.Format("/cdn/{0}/{1}.jpg"
                  , hash
                  , inImage.ID
                  ));

                filecheck = Sushi.Mediakiwi.Framework.AssetHandler.ScaledAssetCacheManager.GetUrl(filecheck);
                image.ImageUrl = filecheck;
                return filecheck;
            }
            else
            {

                if (Sushi.Mediakiwi.Data.Asset.m_CloundInstance.ExistsGeneratedContent(string.Concat(hash, "/", inImage.ID, ".jpg")))
                {
                    string url = string.Concat(Sushi.Mediakiwi.Data.Asset.m_CloundInstance.ContentDeliveryUrl, Sushi.Mediakiwi.Data.Asset.m_CloundInstance.ContainerGeneratedContent, "/", hash, "/", inImage.ID, ".jpg");
                    return url;
                }
            }
        }

        if (inImage.ID == 0)
        {
            if (image != null)
                image.Visible = false;
            return null;
        }

        Sushi.Mediakiwi.Data.Image tmp = inImage.CreateAttachedImageSize(width, height, format, bgColor, position);

        if (tmp == null)
        {
            if (image != null)
                image.Visible = false;
            return null;
        }
        if (image != null)
        {
            image.ImageUrl = tmp.CdnImagePath;
            image.Visible = true;

        }
        if (Sushi.Mediakiwi.Data.Asset.HasCloudSetting)
        {
            if (Sushi.Mediakiwi.Data.Asset.m_CloundInstance.ExistsGeneratedContent(string.Concat(hash, "/", inImage.ID, ".jpg")))
            {
                string url = string.Concat(Sushi.Mediakiwi.Data.Asset.m_CloundInstance.ContentDeliveryUrl, Sushi.Mediakiwi.Data.Asset.m_CloundInstance.ContainerGeneratedContent, "/", hash, "/", inImage.ID, ".jpg");
                return url;
            }
        }
        return tmp.GeneratedImagePath;
    }

    /// <summary>
    /// Applies a forced width. The image is physically scaled!
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="width">The width.</param>
    /// <param name="position">The position.</param>
    /// <param name="returnLocalUrl">if set to <c>true</c> [return local URL].</param>
    /// <param name="skipGalleryMapping">if set to <c>true</c> [skip gallery mapping].</param>
    /// <returns></returns>
    internal static string ApplyForcedMaximumWidth(this Sushi.Mediakiwi.Data.Image inImage, System.Web.UI.WebControls.Image image, int width, Sushi.Mediakiwi.Data.ImagePosition position, bool returnLocalUrl, bool skipGalleryMapping)
    {
        return ParseImage(inImage, image, skipGalleryMapping, returnLocalUrl, Sushi.Mediakiwi.Data.ImageFileFormat.ValidateMaximumWidth, position, width, 0, null);
    }



    /// <summary>
    /// Applies the maximum height of the forced.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="height">The height.</param>
    /// <returns></returns>
    public static string ApplyForcedMaximumHeight(this Sushi.Mediakiwi.Data.Image inImage, System.Web.UI.WebControls.Image image, int height)
    {
        return ApplyForcedMaximumHeight(inImage, image, height, false);
    }

    /// <summary>
    /// Applies the maximum height of the forced.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="height">The height.</param>
    /// <param name="returnLocalUrl">if set to <c>true</c> [return local URL].</param>
    /// <returns></returns>
    public static string ApplyForcedMaximumHeight(this Sushi.Mediakiwi.Data.Image inImage, System.Web.UI.WebControls.Image image, int height, bool returnLocalUrl)
    {
        return ApplyForcedMaximumHeight(inImage, image, height, returnLocalUrl, false);
    }

    /// <summary>
    /// Applies a forced height. The image is physically scaled!
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="height">The height.</param>
    /// <param name="returnLocalUrl">if set to <c>true</c> [return local URL].</param>
    /// <param name="skipGalleryMapping">if set to <c>true</c> [skip gallery mapping].</param>
    /// <returns></returns>
    public static string ApplyForcedMaximumHeight(this Sushi.Mediakiwi.Data.Image inImage, System.Web.UI.WebControls.Image image, int height, bool returnLocalUrl, bool skipGalleryMapping)
    {
        return ParseImage(inImage, image, skipGalleryMapping, returnLocalUrl, Sushi.Mediakiwi.Data.ImageFileFormat.ValidateMaximumHeight, Sushi.Mediakiwi.Data.ImagePosition.Center, 0, height, null);

    }

    /// <summary>
    /// Applies the forced maximum.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <returns></returns>
    public static string ApplyForcedMaximum(this Sushi.Mediakiwi.Data.Image inImage, System.Web.UI.WebControls.Image image, int width, int height)
    {
        return ApplyForcedMaximum(inImage, image, width, height, false);
    }

    /// <summary>
    /// Applies the forced maximum.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="returnLocalUrl">if set to <c>true</c> [return local URL].</param>
    /// <returns></returns>
    public static string ApplyForcedMaximum(this Sushi.Mediakiwi.Data.Image inImage, System.Web.UI.WebControls.Image image, int width, int height, bool returnLocalUrl)
    {
        return ApplyForcedMaximum(inImage, image, width, height, returnLocalUrl, false);
    }

    /// <summary>
    /// Applies the forced maximum.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="returnLocalUrl">if set to <c>true</c> [return local URL].</param>
    /// <param name="skipGalleryMapping">if set to <c>true</c> [skip gallery mapping].</param>
    /// <returns></returns>
    /// <exception cref="System.Exception">Can not generate an image with 0 as dimensions.</exception>
    internal static string ApplyForcedMaximum(this Sushi.Mediakiwi.Data.Image inImage, System.Web.UI.WebControls.Image image, int width, int height, bool returnLocalUrl, bool skipGalleryMapping)
    {
        if (width == 0 || height == 0)
        {
            if (width == 0)
                return ApplyForcedMaximumHeight(inImage, image, height, returnLocalUrl);

            if (height == 0)
                return ApplyForcedMaximumWidth(inImage, image, width, returnLocalUrl);

            throw new Exception("Can not generate an image with 0 as dimensions.");
        }

        return ParseImage(inImage, image, skipGalleryMapping, returnLocalUrl, Sushi.Mediakiwi.Data.ImageFileFormat.ValidateMaximumWidthAndHeight, Sushi.Mediakiwi.Data.ImagePosition.Center, width, height, null);
    }


    /// <summary>
    /// This option has two possible correction outcomes:
    /// If canvas is bigger the image is left as is and placed at coordinate (0,0)
    /// If canvas is smaller the image is simply cut viewed from coordinate (0,0), so cutting the bottom/right sections.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    public static string ApplyForcedBorder(this Sushi.Mediakiwi.Data.Image inImage, System.Web.UI.WebControls.Image image, int width, int height)
    {
        return ApplyForcedBorder(inImage, image, width, height, Wim.CommonConfiguration.DEFAULT_THUMB_BGCOLOR, true);
    }

    /// <summary>
    /// Applies the forced border no crop.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <returns></returns>
    public static string ApplyForcedBorderNoCrop(this Sushi.Mediakiwi.Data.Image inImage, System.Web.UI.WebControls.Image image, int width, int height)
    {
        return ApplyForcedBorder(inImage, image, width, height, Wim.CommonConfiguration.DEFAULT_THUMB_BGCOLOR, false, Sushi.Mediakiwi.Data.ImagePosition.Center);
    }

    public static string ApplyForcedBorder(this Sushi.Mediakiwi.Data.Image inImage, System.Web.UI.WebControls.Image image, int width, int height, bool returnLocalUrl)
    {
        return ApplyForcedBorder(inImage, image, width, height, Wim.CommonConfiguration.DEFAULT_THUMB_BGCOLOR, false, Sushi.Mediakiwi.Data.ImagePosition.Center, null, returnLocalUrl, false);
    }

    /// <summary>
    /// Scales the image to the specified size .
    /// </summary>
    /// <param name="image">The image to apply this image to (or NULL to get URL)</param>
    /// <param name="width">The desired width of the resulting image</param>
    /// <param name="height">The desired height of the resulting image</param>
    /// <param name="bgColor">The desired Background Fillcolor of the resulting image (in Hex : #FFFFFF)</param>
    /// <returns></returns>
    public static string ApplyForcedBorderNoCrop(this Sushi.Mediakiwi.Data.Image inImage, System.Web.UI.WebControls.Image image, int width, int height, string bgColor)
    {
        System.Drawing.Color color = System.Drawing.ColorTranslator.FromHtml(bgColor);
        List<int> cols = new List<int>();
        if (color != null)
        {
            cols.Add((int)color.R);
            cols.Add((int)color.G);
            cols.Add((int)color.B);
        }
        else
        {
            cols.Add(Wim.CommonConfiguration.DEFAULT_THUMB_BGCOLOR[0]); // R
            cols.Add(Wim.CommonConfiguration.DEFAULT_THUMB_BGCOLOR[1]); // G
            cols.Add(Wim.CommonConfiguration.DEFAULT_THUMB_BGCOLOR[2]); // B
        }

        return ApplyForcedBorder(inImage, image, width, height, cols.ToArray(), false, Sushi.Mediakiwi.Data.ImagePosition.Center, null, false, false);
    }

    /// <summary>
    /// Scales the image to the specified size .
    /// </summary>
    /// <param name="image">The image to apply this image to (or NULL to get URL)</param>
    /// <param name="width">The desired width of the resulting image</param>
    /// <param name="height">The desired height of the resulting image</param>
    /// <param name="bgColor">The desired Background Fillcolor of the resulting image (R, G, B values)</param>
    /// <returns></returns>
    public static string ApplyForcedBorderNoCrop(this Sushi.Mediakiwi.Data.Image inImage, System.Web.UI.WebControls.Image image, int width, int height, int[] bgColor)
    {
        return ApplyForcedBorder(inImage, image, width, height, bgColor, false, Sushi.Mediakiwi.Data.ImagePosition.Center, null, false, false);
    }

    /// <summary>
    /// Applies the forced border.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="backgroundRgb">The background RGB.</param>
    public static string ApplyForcedBorder(this Sushi.Mediakiwi.Data.Image inImage, System.Web.UI.WebControls.Image image, int width, int height, int[] backgroundRgb)
    {
        return ApplyForcedBorder(inImage, image, width, height, backgroundRgb, true);
    }

    /// <summary>
    /// Applies the forced border.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="backgroundRgb">The background RGB.</param>
    /// <param name="CropImage">if set to <c>true</c> [crop image].</param>
    /// <returns></returns>
    public static string ApplyForcedBorder(this Sushi.Mediakiwi.Data.Image inImage, System.Web.UI.WebControls.Image image, int width, int height, int[] backgroundRgb, bool CropImage)
    {
        return ApplyForcedBorder(inImage, image, width, height, backgroundRgb, CropImage, Sushi.Mediakiwi.Data.ImagePosition.Center);
    }

    /// <summary>
    /// Applies the forced border.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="backgroundRgb">The background RGB.</param>
    /// <param name="CropImage">if set to <c>true</c> [crop image].</param>
    /// <param name="position">The position.</param>
    /// <returns></returns>
    public static string ApplyForcedBorder(this Sushi.Mediakiwi.Data.Image inImage, System.Web.UI.WebControls.Image image, int width, int height, int[] backgroundRgb, bool CropImage, Sushi.Mediakiwi.Data.ImagePosition position)
    {
        return ApplyForcedBorder(inImage, image, width, height, backgroundRgb, CropImage, position, null);
    }

    /// <summary>
    /// Applies the forced border.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="backgroundRgb">The background RGB.</param>
    /// <param name="CropImage">if set to <c>true</c> [crop image].</param>
    /// <param name="position">The position.</param>
    /// <param name="galleryToSaveTo">The gallery to save to.</param>
    /// <returns></returns>
    public static string ApplyForcedBorder(this Sushi.Mediakiwi.Data.Image inImage, System.Web.UI.WebControls.Image image, int width, int height, int[] backgroundRgb, bool CropImage, Sushi.Mediakiwi.Data.ImagePosition position, Sushi.Mediakiwi.Data.Gallery galleryToSaveTo)
    {
        return ApplyForcedBorder(inImage, image, width, height, backgroundRgb, CropImage, position, galleryToSaveTo, false, false);
    }

    /// <summary>
    /// Applies the forced border.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="backgroundRgb">The background RGB.</param>
    /// <param name="CropImage">if set to <c>true</c> [crop image].</param>
    /// <param name="position">The position.</param>
    /// <param name="galleryToSaveTo">The gallery to save to.</param>
    /// <param name="returnLocalUrl">if set to <c>true</c> [return local URL].</param>
    /// <param name="skipGalleryMapping">if set to <c>true</c> [skip gallery mapping].</param>
    /// <returns></returns>
    internal static string ApplyForcedBorder(this Sushi.Mediakiwi.Data.Image inImage, System.Web.UI.WebControls.Image image, int width, int height, int[] backgroundRgb, bool CropImage, Sushi.Mediakiwi.Data.ImagePosition position, Sushi.Mediakiwi.Data.Gallery galleryToSaveTo, bool returnLocalUrl, bool skipGalleryMapping)
    {
        return ParseImage(inImage, image, skipGalleryMapping, returnLocalUrl, Sushi.Mediakiwi.Data.ImageFileFormat.ValidateMaximumAndCrop, Sushi.Mediakiwi.Data.ImagePosition.Center, width, height, backgroundRgb);
    }


    /// <summary>
    /// Generates the new image based on the params. This code has been seperated from <c>ApplyForcedBorder</c>
    /// </summary>
    /// <param name="image">The image control</param>
    /// <param name="conc">The hash of the parameters to save the generated image by</param>
    /// <param name="cropImage">Crop the image</param>
    /// <param name="width">The new width</param>
    /// <param name="height">The new height</param>
    /// <param name="backgroundRgb">The background</param>
    /// <param name="position">The position</param>
    /// <param name="galleryToSaveTo">The Gallery</param>
    /// <returns>Location to which is has been saved</returns>
    private static string GenerateANewImage(this Sushi.Mediakiwi.Data.Image inImage, System.Web.UI.WebControls.Image image, string conc, bool cropImage, int width, int height, int[] backgroundRgb, Sushi.Mediakiwi.Data.ImagePosition position, Sushi.Mediakiwi.Data.Gallery galleryToSaveTo)
    {
        Sushi.Mediakiwi.Data.Image tmp = new Sushi.Mediakiwi.Data.Image();

        if (cropImage)
            tmp = inImage.CreateAttachedImageSize(width, height, Sushi.Mediakiwi.Data.ImageFileFormat.ValidateMaximumAndCrop, backgroundRgb, position);//Sushi.Mediakiwi.Data.ImagePosition.Center);
        else
            tmp = inImage.CreateAttachedImageSize(width, height, Sushi.Mediakiwi.Data.ImageFileFormat.FixedBorder, backgroundRgb, position, galleryToSaveTo);


        if (tmp == null || tmp.IsNewInstance)
        {
            if (image != null) image.Visible = false;
            return null;
        }
        else
        {
            //  Save a new instance of the image to a different location.
            if (galleryToSaveTo != null)
            {
                tmp.ID = 0;
                tmp.Save();
            }
        }

        if (image != null)
        {
            image.ImageUrl = tmp.CdnImagePath;
            image.Visible = true;
        }

        if (!Sushi.Mediakiwi.Data.Asset.HasCloudSetting)
        {
            // Check for newer version added by Casper
            string cahcedPath = HttpContext.Current.Server.MapPath(tmp.GeneratedImagePath);
            if (File.Exists(cahcedPath) && inImage.Created > File.GetLastWriteTime(cahcedPath))
            {
                File.Delete(cahcedPath);
                return ApplyForcedBorder(inImage, image, width, height, backgroundRgb, cropImage, position, galleryToSaveTo);
            }
            return tmp.GeneratedImagePath;
        }
        else
        {
            string filecheck = string.Format("{0}/{1}.jpg"
                , Wim.Utilities.Encryption.Encrypt(conc, "cdn", true)
                , inImage.ID
                );
            if (Sushi.Mediakiwi.Data.Asset.m_CloundInstance.ExistsGeneratedContent(filecheck))
            {
                return string.Concat(Sushi.Mediakiwi.Data.Asset.m_CloundInstance.ContentDeliveryUrl, Sushi.Mediakiwi.Data.Asset.m_CloundInstance.ContainerGeneratedContent, "/", filecheck);
            }
            return null;
        }
    }

    /// <summary>
    /// Apply the image properties to a Image webcontrol with a set height (set url, toolTip and width/height).
    /// The height is calculated based on the rescaling factor
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="height">The height.</param>
    public static void ApplyScaleHeight(this Sushi.Mediakiwi.Data.Image inImage, System.Web.UI.WebControls.Image image, int height)
    {
        if (inImage.Height != 0 && inImage.Width != 0 && height > 0)
        {
            decimal factor = decimal.Divide(height, inImage.Height);
            decimal newWidth = decimal.Round(decimal.Multiply(factor, inImage.Width), 0);
            inImage.Width = Convert.ToInt32(newWidth);
            inImage.Height = height;
        }
        Apply(inImage, image);
    }


}
