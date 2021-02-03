using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Sushi.Mediakiwi.Data;

public static class AssetExtension
{
    /// <summary>
    /// Delete an implementation record.
    /// </summary>
    /// <returns></returns>
    public static bool Delete(this Asset inAsset)
    {
        DeleteFile(inAsset);
        CloudDelete(inAsset);

        bool isDeleted = inAsset.Delete();


        var gallery = (from item in Gallery.SelectAll() where item.ID == inAsset.GalleryID select item).ToArray();
        if (gallery.Count() > 0)
            gallery[0].UpdateCount();

        return isDeleted;
    }


    /// <summary>
    /// Images the scaled URL.
    /// </summary>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <returns></returns>
    public static string ImageScaledUrl(this Asset inAsset, int width, int height)
    {
        var m_ImageScaledUrl = string.Empty;
        if (inAsset.IsImage)
        {
            var portal = inAsset.DatabaseMappingPortal == null ? null : inAsset.DatabaseMappingPortal.Name;
            var imgUrl = Image.GetUrl(inAsset.ID, ImageFileFormat.FixedBorder, width, height, null, inAsset.CompletePath);

            m_ImageScaledUrl = $@"<a href=""{inAsset.FullPath}"" target=""_blank""><img src=""{imgUrl}"" width=""{width}"" height=""{height}""></a>";
        }

        return m_ImageScaledUrl;

    }

    internal static void CloudDelete(this Asset inAsset)
    {
        if (!Asset.HasCloudSetting) return;
        Asset.m_CloundInstance.Delete(inAsset);
    }

    public static bool DeleteFile(this Asset inAsset)
    {
        if (System.Web.HttpContext.Current != null)
        {
            try
            {
                string filePath = System.Web.HttpContext.Current.Server.MapPath(inAsset.RelativePath);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }
            }
            catch (Exception) { }
        }
        return false;
    }

    /// <summary>
    /// Moves the file.
    /// </summary>
    /// <param name="galleryID">The gallery ID.</param>
    /// <returns></returns>
    public static bool MoveFile(this Asset inAsset, int galleryID)
    {
        Gallery gallery = Gallery.SelectOne(galleryID);
        string newFileLocation = string.Concat(gallery.LocalFolderPath, "/", inAsset.FileName);

        try
        {
            if (inAsset.Exists)
            {
                File.Move(inAsset.LocalFilePath, newFileLocation);
                inAsset.GalleryID = galleryID;
                inAsset.Save();
            }

        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Applies the specified hyperlink.
    /// </summary>
    /// <param name="hyperlink">The hyperlink.</param>
    public static void Apply(this Asset inAsset, System.Web.UI.WebControls.HyperLink hyperlink)
    {
        Apply(inAsset, hyperlink, false);
    }

    /// <summary>
    /// Applies the specified hyperlink.
    /// </summary>
    /// <param name="hyperlink">The hyperlink.</param>
    /// <param name="onlySetNavigationUrl">if set to <c>true</c> [only set navigation URL].</param>
    public static void Apply(this Asset inAsset, System.Web.UI.WebControls.HyperLink hyperlink, bool onlySetNavigationUrl)
    {
        hyperlink.NavigateUrl = inAsset.DownloadUrl;
        hyperlink.ToolTip = inAsset.Description;

        if (onlySetNavigationUrl)
            return;

        hyperlink.Text = inAsset.Title;
    }

    /// <summary>
    /// Saves the stream.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <param name="gallery">The gallery.</param>
    public static void SaveStream(this Asset inAsset, FileInfo file, Gallery gallery)
    {
        SaveStream(inAsset, file, gallery, false);
    }

    /// <summary>
    /// Saves the stream.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <param name="gallery">The gallery.</param>
    /// <param name="overWriteImage">if set to <c>true</c> [over write image].</param>
    public static void SaveStream(this Asset inAsset, FileInfo file, Gallery gallery, bool overWriteImage)
    {
        SaveStream(inAsset, file, gallery, false, overWriteImage);
    }

    /// <summary>
    /// Saves the stream.
    /// </summary>
    /// <param name="fileStream">The file stream.</param>
    /// <param name="filename">The filename.</param>
    /// <param name="gallery">The gallery.</param>
    public static void SaveStream(this Asset inAsset, Stream fileStream, string filename, Gallery gallery)
    {
        SaveStream(inAsset, fileStream, filename, gallery, false);
    }

    /// <summary>
    /// Saves the stream.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <param name="gallery">The gallery.</param>
    /// <param name="saveStream">if set to <c>true</c> [Saves the stream also to Azure].</param>
    /// <param name="overWriteImage">if set to <c>true</c> [over write image].</param>
    public static void SaveStream(this Asset inAsset, FileInfo file, Gallery gallery, bool saveStream, bool overWriteImage)
    {
        using (FileStream filestream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
        {
            if (filestream.Length == 0)
                return;

            inAsset.Extention = file.Extension.Replace(".", string.Empty);
            inAsset.FileName = file.Name;
            inAsset.GalleryID = gallery.ID;
            inAsset.IsActive = true;

            GuessType(inAsset);

            inAsset.Size = (int)file.Length;

            SaveStream2(inAsset, (Stream)filestream, gallery, saveStream, overWriteImage);
        }
    }


    /// <summary>
    /// Saves the stream.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <param name="gallery">The gallery.</param>
    public static void SaveStream(this Asset inAsset, System.Web.HttpPostedFile file, Gallery gallery)
    {
        if (file.ContentLength == 0)
            return;

        int index = file.FileName.LastIndexOf('.') + 1;
        string[] fileSplit = file.FileName.Split('\\');

        inAsset.Extention = file.FileName.Substring(index, file.FileName.Length - index);
        inAsset.FileName = fileSplit[fileSplit.Length - 1];

        inAsset.GalleryID = gallery.ID;
        inAsset.Type = file.ContentType;
        inAsset.Size = file.ContentLength;
        inAsset.Created = Common.DatabaseDateTime;

        SaveStream2(inAsset, file.InputStream, gallery, true);
    }

    /// <summary>
    /// Saves the stream.
    /// </summary>
    /// <param name="fileStream">The file stream.</param>
    /// <param name="filename">The filename.</param>
    /// <param name="gallery">The gallery.</param>
    /// <param name="overwriteExistingFile">If set to true, overwrites the file if it allready exists. If set to false, appends a number to the file to create a unique file</param>
    public static void SaveStream(this Asset inAsset, Stream fileStream, string filename, Gallery gallery, bool overwriteExistingFile)
    {
        if (fileStream.Length == 0)
            return;

        int index = filename.LastIndexOf('.') + 1;

        inAsset.Extention = filename.Substring(index, filename.Length - index);
        inAsset.FileName = filename;
        inAsset.GalleryID = gallery.ID;
        inAsset.Size = Convert.ToInt32(fileStream.Length);

        GuessType(inAsset);
        SaveStream2(inAsset, fileStream, gallery, true, overwriteExistingFile);
    }

    /// <summary>
    /// Saves the stream2.
    /// </summary>
    /// <param name="fileStream">The file stream.</param>
    /// <param name="gallery">The gallery.</param>
    /// <param name="saveStream">if set to <c>true</c> [save stream].</param>
    static void SaveStream2(this Asset inAsset, Stream fileStream, Gallery gallery, bool saveStream)
    {
        SaveStream2(inAsset, fileStream, gallery, saveStream, false);
    }

    /// <summary>
    /// Saves the stream2.
    /// </summary>
    /// <param name="fileStream">The file stream.</param>
    /// <param name="gallery">The gallery.</param>
    /// <param name="saveStream">if set to <c>true</c> [save stream].</param>
    /// <param name="overWriteFile">if set to <c>true</c> [over write file].</param>
    static void SaveStream2(this Asset inAsset, Stream fileStream, Gallery gallery, bool saveStream, bool overWriteFile)
    {
        if (gallery.IsNewInstance)
            throw new Exception("The request gallery does not exist.");
        inAsset.CompletePath = gallery.CompletePath;

        if (fileStream.Length == 0)
            return;

        if (Asset.HasCloudSetting == false)
            gallery.CreateFolder();

        string filepart = inAsset.FileName.Replace(string.Concat(".", inAsset.Extention), string.Empty);

        if (string.IsNullOrEmpty(inAsset.Title))
            inAsset.Title = filepart;

        if (!overWriteFile)
        {
            int refCount = 1;
            int count = 0;
            //  Check for digital file existance
            while (refCount > 0)
            {
                //EDIT DV 2013-05-13: Added 'and asset_key <> this.ID', otherwise it could take itself in the count...
                refCount = Convert.ToInt32(inAsset.Execute(string.Format("select count(Asset_Key) from wim_Assets where Asset_Gallery_Key = {0} and Asset_Filename = '{1}' and Asset_Key <> {2}", gallery.ID, inAsset.FileName, inAsset.ID)));
                if (refCount > 0)
                {
                    if (overWriteFile)
                    {
                        inAsset.ID = Asset.SelectOne(gallery.ID, inAsset.FileName).ID;
                        break;
                    }
                    else
                    {
                        count++;
                        inAsset.FileName = string.Concat(filepart, "(", count, ").", inAsset.Extention);
                    }
                }
            }

            if (count > 0 && inAsset.Title == filepart)
            {
                inAsset.Title = inAsset.FileName.Replace(string.Concat(".", inAsset.Extention), string.Empty);
            }
        }


        string localFilePath = string.Concat(Gallery.LocalRepositoryBase, gallery.CompletePath, "/");
        string localFileName = string.Concat(localFilePath, inAsset.FileName);

        if (!overWriteFile)
        {
            int count = 0;
            //  Check for physical file existance
            while (File.Exists(localFileName))
            {
                count++;
                inAsset.FileName = string.Concat(filepart, "(", count, ").", inAsset.Extention);
                localFileName = string.Format("{0}{1}", localFilePath, inAsset.FileName);
            }
        }

        //EvaluateAssetType(gallery, fileStream, localFilePath, localFileName, saveStream, overWriteFile);

        if (saveStream)
        {
            //first try to upload to cloud
            bool isUploadedToCloud = AssetLogic.CloudUpload(inAsset, gallery, fileStream, inAsset.FileName, inAsset.Type);

            //delete all generatedAssets for this asset, they are not valid anymore because we save new content
            if (inAsset.RelyOnGeneratedAssets && inAsset.ID > 0)
            {
                string portal = null;
                if (inAsset.DatabaseMappingPortal != null)
                    portal = inAsset.DatabaseMappingPortal.Name;
                //Sushi.Mediakiwi.DataEntities.GeneratedAsset.DeleteAllForID(this.ID, portal);
            }

            //if not, save local
            if (isUploadedToCloud)
            {
                //  Delete local file
                inAsset.DeleteFile();
            }
            else
            {
                //set local file create mode depending on if allowed to overwrite
                FileMode fileMode = overWriteFile ? FileMode.Create : FileMode.CreateNew;
                using (BinaryReader br = new BinaryReader(fileStream))
                {
                    // Line below added by MarkR on 03-01-2013, for fixing an issue with the line buffer = br.ReadBytes(1024);
                    // This was returning 0 so the loop became an infinite one.
                    if (br.BaseStream != null)
                        br.BaseStream.Position = 0;
                    using (FileStream fs = new FileStream(localFileName, fileMode))
                    {
                        long bytesRead = 0;
                        Byte[] buffer;

                        while (bytesRead < br.BaseStream.Length)
                        {
                            buffer = br.ReadBytes(1024);
                            fs.Write(buffer, 0, buffer.Length);
                            bytesRead += buffer.Length;
                        }
                    }
                }
            }
        }
    }

    internal static void ParseRemoteLocation(this Asset inAsset, string databaseMap, out bool hasRemoteError)
    {
        hasRemoteError = false;
        if (!inAsset.RemoteDownload || string.IsNullOrEmpty(inAsset.RemoteLocation))
        {
            if (!inAsset.RemoteDownload && !string.IsNullOrEmpty(inAsset.RemoteLocation) &&
                    (
                    string.IsNullOrEmpty(inAsset.Extention) ||
                    string.IsNullOrEmpty(inAsset.Type) ||
                    string.IsNullOrEmpty(inAsset.FileName)
                    )
                )
            {
                inAsset.Extention = inAsset.RemoteLocation.Split('/').Last().Split('.').Last();
                inAsset.FileName = string.Concat(inAsset.ID, ".", inAsset.Extention);
                GuessType(inAsset);
                inAsset.Save();
            }
            return;
        }

        inAsset.Extention = inAsset.RemoteLocation.Split('/').Last().Split('.').Last();
        string localFile = string.Concat(Gallery.SelectOne(inAsset.GalleryID).LocalFolderPath, @"\", inAsset.ID, ".", inAsset.Extention);

        if (File.Exists(localFile))
            return;

        try
        {
            WebClient client = new WebClient();
            client.DownloadFile(inAsset.RemoteLocation, localFile);

            inAsset.SaveStream(new FileInfo(localFile), Gallery.SelectOne(inAsset.GalleryID));

            inAsset.FileName = string.Concat(inAsset.ID, ".", inAsset.Extention);
            inAsset.Save();
        }
        catch (Exception ex)
        {
            hasRemoteError = true;
            Notification.InsertOne("Asset.ParseRemoteLocation", NotificationType.Warning, ex);
        }
    }

    public static void GuessType(this Asset inAsset)
    {
        inAsset.Type = Asset.GuessType(inAsset.Extention);
    }

    /// <summary>
    /// Save a database entity. This method will indentify the need for an INSERT or an UPDATE.
    /// </summary>
    /// <returns></returns>
    public static bool Save(this Asset inAsset)
    {
        if (string.IsNullOrEmpty(inAsset.Type))
            inAsset.GuessType();

        inAsset.Updated = Common.DatabaseDateTime;

        bool shouldSetSortorder = (inAsset.ID == 0);
        bool isSaved = inAsset.Save();

        if (!inAsset.SortOrder.HasValue)
        {
            inAsset.SortOrder = inAsset.ID;
            inAsset.Save();
        }

        return isSaved;
    }

    public static string GetAvailableName(this Asset inAsset, int galleryID, string fileName)
    {
        string possibleName = fileName;
        string extension = string.Empty;
        int index = fileName.LastIndexOf('.') + 1;

        extension = fileName.Substring(index, fileName.Length - index);
        string filepart = fileName.Replace(string.Concat(".", extension), string.Empty);

        int refCount = 1;
        int count = 0;
        //  Check for digital file existance
        while (refCount > 0)
        {
            refCount = Convert.ToInt32(inAsset.Execute(string.Format("select count(*) from wim_Assets where Asset_Gallery_Key = {0} and Asset_Filename = '{1}'", galleryID, possibleName)));
            if (refCount > 0)
            {
                count++;
                possibleName = string.Concat(filepart, "(", count, ").", extension);
            }
        }

        return possibleName;
    }

    /// <summary>
    /// Evaluates the type of the asset.
    /// </summary>
    public static void EvaluateAssetType(this Asset inAsset)
    {
        Gallery gallery = Gallery.SelectOne(inAsset.GalleryID);

        string localFilePath = string.Concat(Gallery.LocalRepositoryBase, gallery.CompletePath, "/");
        string localFileName = string.Concat(localFilePath, inAsset.FileName);

        FileInfo nfo = new FileInfo(localFileName);
        inAsset.Extention = nfo.Extension.Replace(".", string.Empty);

        if (string.IsNullOrEmpty(inAsset.Title))
            inAsset.Title = inAsset.FileName.Replace(nfo.Extension, string.Empty);

        if (!string.IsNullOrEmpty(inAsset.RemoteLocation))
        {
            EvaluateAssetType(inAsset, gallery, null, localFilePath, localFileName, false, false);
            inAsset.Save();
            return;
        }

        using (FileStream fileStream = new FileStream(localFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            EvaluateAssetType(inAsset, gallery, fileStream, localFilePath, localFileName, false, false);
        }
    }

    /// <summary>
    /// Evaluates the type of the asset.
    /// </summary>
    /// <param name="gallery">The gallery.</param>
    /// <param name="fileStream">The file stream.</param>
    /// <param name="localFilePath">The local file path.</param>
    /// <param name="localFileName">Name of the local file.</param>
    /// <param name="saveStream">if set to <c>true</c> [save stream].</param>
    /// <param name="overWriteImage">if set to <c>true</c> [over write image].</param>
    static void EvaluateAssetType(this Asset inAsset, Gallery gallery, Stream fileStream, string localFilePath, string localFileName, bool saveStream, bool overWriteImage)
    {
        inAsset.IsImage = (
            inAsset.Extention.Equals("gif", StringComparison.CurrentCultureIgnoreCase) ||
            inAsset.Extention.Equals("jpg", StringComparison.CurrentCultureIgnoreCase) ||
            inAsset.Extention.Equals("jpeg", StringComparison.CurrentCultureIgnoreCase) ||
            inAsset.Extention.Equals("png", StringComparison.CurrentCultureIgnoreCase)
            );

        inAsset.IsNewStyle = true;
        inAsset.IsOldStyle = false;

        if (inAsset.IsNewInstance)
            inAsset.Save();
    }

    /// <summary>
    /// Applies the path.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="appendUrl">if set to <c>true</c> [append URL].</param>
    /// <param name="checkRegistry">if set to <c>true</c> [check registry].</param>
    /// <returns></returns>
    public static string ApplyPath(this Asset inAsset, string fileName, bool appendUrl, bool checkRegistry)
    {
        string repository;
        if (inAsset.IsNewStyle)
            repository = string.Concat(Wim.CommonConfiguration.RelativeRepositoryUrl, inAsset.CompletePath);
        else
        {
            if (inAsset.IsImage)
            {
                if (inAsset.BaseGalleryID == 0)
                    repository = "repository/image";
                else
                {
                    if (inAsset.IsOldStyle)
                        repository = "repository/image";
                    else
                        repository = string.Concat("repository/image/", inAsset.BaseGalleryID);
                }
            }
            else
            {
                if (inAsset.BaseGalleryID == 0)
                    repository = "repository/document";
                else
                {

                    if (inAsset.IsOldStyle)
                        repository = "repository/document";
                    else
                        repository = string.Concat("repository/document/", inAsset.BaseGalleryID);
                }
            }
        }

        if (!string.IsNullOrEmpty(repository) && !string.IsNullOrEmpty(fileName) && repository.EndsWith(fileName))
            fileName = null;

        if (inAsset.GalleryUrlMap != null)
        {
            if (string.IsNullOrEmpty(inAsset.GalleryUrlMap.AssetFolder))
                return string.Concat(inAsset.GalleryUrlMap.MappedUrl, string.Format("/{0}/{1}", repository, fileName));

            return string.Concat(inAsset.GalleryUrlMap.MappedUrl, string.Format("/{0}/{1}/{2}", inAsset.GalleryUrlMap.AssetFolder, inAsset.CompletePath, fileName));
        }

        if (string.IsNullOrEmpty(fileName))
            return Wim.Utility.AddApplicationPath(string.Concat("/", repository), appendUrl);
        return Wim.Utility.AddApplicationPath(string.Format("/{0}/{1}", repository, fileName), appendUrl);
    }


    /// <summary>
    /// Creates the size of the attached image.
    /// </summary>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="format">The format.</param>
    /// <returns></returns>
    internal static Image CreateAttachedImageSize(this Asset inAsset, int width, int height, ImageFileFormat format)
    {
        return CreateAttachedImageSize(inAsset, width, height, format, null, ImagePosition.Center);
    }

    /// <summary>
    /// Creates the size of the attached image.
    /// </summary>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="format">The format.</param>
    /// <param name="backgroundRGB">The background RGB.</param>
    /// <param name="position">The position.</param>
    /// <returns></returns>
    internal static Image CreateAttachedImageSize(this Asset inAsset, int width, int height, ImageFileFormat format, int[] backgroundRGB, ImagePosition position)
    {
        return CreateAttachedImageSize(inAsset, width, height, format, backgroundRGB, position, null);
    }


    internal static void ValidateImage(this Asset inAsset, bool autoSave)
    {
        System.Drawing.Image gfx = null;

        if (File.Exists(inAsset.LocalFilePath))
            gfx = System.Drawing.Image.FromFile(inAsset.LocalFilePath);

        ValidateImage(inAsset, autoSave, gfx);
    }

    /// <summary>
    /// Validate if the stream is an image and if it is, determin the Width and Height.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="filename">The filename.</param>
    /// <param name="autoSave">if set to <c>true</c> [automatic save].</param>
    public static void ValidateImage(this Asset inAsset, Stream stream, string filename, bool autoSave = false)
    {
        if (string.IsNullOrEmpty(inAsset.Extention))
        {
            int pindex = filename.LastIndexOf('.') + 1;
            inAsset.Extention = filename.Substring(pindex, filename.Length - pindex).ToLower();
        }

        inAsset.IsImage = (
            inAsset.Extention.Equals("gif", StringComparison.CurrentCultureIgnoreCase) ||
            inAsset.Extention.Equals("jpg", StringComparison.CurrentCultureIgnoreCase) ||
            inAsset.Extention.Equals("jpeg", StringComparison.CurrentCultureIgnoreCase) ||
            inAsset.Extention.Equals("png", StringComparison.CurrentCultureIgnoreCase)
            );

        if (inAsset.IsImage && (inAsset.Width == 0 || inAsset.Height == 0))
        {
            if (stream == null)
                return;

            var gfx = System.Drawing.Image.FromStream(stream);
            ValidateImage(inAsset, autoSave, gfx);
        }
    }

    public static void ValidateImage(this Asset inAsset, bool autoSave, System.Drawing.Image gfx)
    {
        if (inAsset.IsImage)
        {
            if (inAsset.Height == 0 || inAsset.Width == 0)
            {
                try
                {
                    if (gfx != null)
                    {
                        inAsset.Height = gfx.Height;
                        inAsset.Width = gfx.Width;
                        if (autoSave)
                            inAsset.Save();
                    }
                }
                catch (Exception) { }
            }
        }
    }


    /// <summary>
    /// Creates the size of the attached image.
    /// </summary>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="format">The format.</param>
    /// <param name="backgroundRGB">The background RGB.</param>
    /// <param name="position">The position.</param>
    /// <param name="gallery">The gallery.</param>
    /// <returns></returns>
    internal static Image CreateAttachedImageSize(this Asset inAsset, int width, int height, ImageFileFormat format, int[] backgroundRGB, ImagePosition position, Gallery gallery)
    {
        if (format == ImageFileFormat.None || string.IsNullOrEmpty(inAsset.FileName))
            return null;

        bool generatedFileExists = false;
        var portal = inAsset.DatabaseMappingPortal == null ? null : inAsset.DatabaseMappingPortal.Name;

        string filename = string.Concat(inAsset.ID, ".jpg");
        string hash = Image.GetCdnHash(inAsset.ID, format, width, height, backgroundRGB, inAsset.CompletePath, true);

        string physicalFile = string.Concat(Gallery.LocalGeneratedImageRepositoryBase, hash, "/", filename);
        string physicalFolder = string.Concat(Gallery.LocalGeneratedImageRepositoryBase, hash, "/");

        bool hasGalleryLocation = (gallery != null);
        if (hasGalleryLocation)
        {
            physicalFile = string.Concat(Gallery.LocalRepositoryBase, gallery.CompletePath, "/", filename);
            physicalFolder = string.Concat(Gallery.LocalRepositoryBase, gallery.CompletePath);
        }

        Image tmp = new Image();
        Wim.Utility.ReflectProperty(inAsset, tmp);
        tmp.m_Path = null;
        tmp.FullPath = null;
        tmp.LocalFilePath = null;
        tmp.GeneratedImagePath = Wim.Utility.AddApplicationPath(string.Concat("/repository/generated/", hash, "/", filename));
        tmp.CdnImagePath = Wim.Utility.AddApplicationPath(string.Concat("/cdn/", hash, "/", filename));
        if (inAsset is Image tmpImage)
        {
            if (tmpImage != null && tmpImage.GalleryUrlMap != null)
                tmp.CdnImagePath = string.Concat(tmpImage.GalleryUrlMap.MappedUrl, "/cdn/", hash, "/", filename);
        }

        if (File.Exists(physicalFile) && !Asset.HasCloudSetting)
        {
            generatedFileExists = true;
            tmp.FileName = hasGalleryLocation ? filename : string.Concat("alt/", filename);
            tmp.Width = 0;
            tmp.Height = 0;
            return tmp;
        }

        string originalPhysicalFile = string.Concat(Gallery.LocalRepositoryBase, inAsset.CompletePath, "\\", inAsset.FileName);
        System.Drawing.Image gfx = null;
        Stream stream = null;
        try
        {
            if (!File.Exists(originalPhysicalFile))
            {
                if (!string.IsNullOrEmpty(inAsset.RemoteLocation) && !inAsset.RemoteDownload)
                {
                    WebClient client = new WebClient();

                    try
                    {
                        string location = inAsset.RemoteLocation;
                        if (location.StartsWith("//"))
                            location = "http:" + location;
                        if (!location.StartsWith("http://") && !location.StartsWith("https://"))
                            location = "http://" + location;
                        stream = client.OpenRead(location);
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            Image gf = Image.SelectOne(Wim.CommonConfiguration.NO_IMAGE_ASSET);
                            if (gf.RemoteDownload)
                                stream = client.OpenRead(Image.SelectOne(Wim.CommonConfiguration.NO_IMAGE_ASSET).FullPath);
                            else
                                stream = new FileStream(Image.SelectOne(Wim.CommonConfiguration.NO_IMAGE_ASSET).LocalFilePath, FileMode.Open);
                        }
                        catch
                        {

                        }
                    }
                    if (stream != null)
                    {
                        try
                        {
                            gfx = System.Drawing.Image.FromStream(stream);
                        }
                        catch
                        {
                            return null;
                        }
                    }
                    else
                        return null;
                }
                else
                {
                    tmp.Width = 0;
                    tmp.Height = 0;
                    return tmp;
                }
            }
            else
                gfx = System.Drawing.Image.FromFile(originalPhysicalFile);

            int size = 0;
            int newWidth = 0;
            int newHeight = 0;

            inAsset.ValidateImage(true, gfx);

            if (format == ImageFileFormat.FixedBorder)
            {
                decimal newTmpWidth = Convert.ToDecimal(width);
                decimal newTmpHeight = Convert.ToDecimal(height);

                if (!Directory.Exists(physicalFolder) && !Asset.HasCloudSetting)
                {
                    Directory.CreateDirectory(physicalFolder);
                }

                CreateNewSizeImage(inAsset,
                    gfx,
                    physicalFile,
                    Convert.ToInt32(Math.Round(newTmpWidth, 0)),
                    Convert.ToInt32(Math.Round(newTmpHeight, 0)),
                    ref size,
                    ref newWidth,
                    ref newHeight,
                    true,
                    backgroundRGB,
                    position);

                tmp.Size = size;
                tmp.Width = newWidth;
                tmp.Height = newHeight;
                tmp.FileName = hasGalleryLocation ? filename : string.Concat("alt/", filename);
                return tmp;
            }
            else if (format == ImageFileFormat.ValidateMaximumWidth || format == ImageFileFormat.ValidateMaximumHeight)
            {
                if (generatedFileExists && ((format == ImageFileFormat.ValidateMaximumHeight && inAsset.Height <= height) || (format == ImageFileFormat.ValidateMaximumWidth && inAsset.Width <= width)))
                    return tmp;

                if (!Directory.Exists(physicalFolder) && !Asset.HasCloudSetting)
                    Directory.CreateDirectory(physicalFolder);

                decimal newTmpWidth = Convert.ToDecimal(width);
                decimal newTmpHeight = Convert.ToDecimal(height);

                if (format == ImageFileFormat.ValidateMaximumHeight)
                {
                    newTmpWidth = decimal.Multiply(decimal.Divide(newTmpHeight, inAsset.Height), inAsset.Width);
                }
                if (format == ImageFileFormat.ValidateMaximumWidth)
                {
                    newTmpHeight = decimal.Multiply(decimal.Divide(newTmpWidth, inAsset.Width), inAsset.Height);
                }

                CreateNewSizeImage(inAsset, gfx, physicalFile
                    , Convert.ToInt32(Math.Round(newTmpWidth, 0))
                    , Convert.ToInt32(Math.Round(newTmpHeight, 0))
                    , ref size, ref newWidth, ref newHeight, true, backgroundRGB, position);

                tmp.Size = size;
                tmp.Width = newWidth;
                tmp.Height = newHeight;
                tmp.FileName = hasGalleryLocation ? filename : string.Concat("alt/", filename);
                return tmp;
            }
            else if (format == ImageFileFormat.ValidateMaximumWidthAndHeight)
            {
                if (generatedFileExists && ((format == ImageFileFormat.ValidateMaximumHeight && inAsset.Height <= height) || (format == ImageFileFormat.ValidateMaximumWidth && inAsset.Width <= width)))
                    return tmp;

                if (!Directory.Exists(physicalFolder) && !Asset.HasCloudSetting)
                    Directory.CreateDirectory(physicalFolder);

                decimal factorW = decimal.Divide(inAsset.Width, width);
                decimal factorH = decimal.Divide(inAsset.Height, height);

                decimal factor = factorW > factorH ? factorW : factorH;

                decimal newTmpWidth = decimal.Divide(inAsset.Width, factor);
                decimal newTmpHeight = decimal.Divide(inAsset.Height, factor);

                CreateNewSizeImage(
                    inAsset,
                    gfx,
                    physicalFile,
                    Convert.ToInt32(Math.Round(newTmpWidth, 0)),
                    Convert.ToInt32(Math.Round(newTmpHeight, 0)),
                    ref size,
                    ref newWidth,
                    ref newHeight,
                    true,
                    backgroundRGB,
                    position);

                tmp.Size = size;
                tmp.Width = newWidth;
                tmp.Height = newHeight;
                tmp.FileName = hasGalleryLocation ? filename : string.Concat("alt/", filename);
                return tmp;
            }
            else if (format == ImageFileFormat.ValidateMaximumAndCrop)
            {
                if (generatedFileExists && ((format == ImageFileFormat.ValidateMaximumHeight && inAsset.Height <= height) || (format == ImageFileFormat.ValidateMaximumWidth && inAsset.Width <= width)))
                    return tmp;

                if (!Directory.Exists(physicalFolder) && !Asset.HasCloudSetting)
                    Directory.CreateDirectory(physicalFolder);

                decimal factorW = decimal.Divide(inAsset.Width, width);
                decimal factorH = decimal.Divide(inAsset.Height, height);

                decimal factor = factorW > factorH ? factorW : factorH;

                decimal newTmpWidth = decimal.Divide(inAsset.Width, factor);
                decimal newTmpHeight = decimal.Divide(inAsset.Height, factor);


                CreateNewSizeImage(
                    inAsset,
                    gfx,
                    physicalFile,
                    Convert.ToInt32(Math.Round(newTmpWidth, 0)),
                    Convert.ToInt32(Math.Round(newTmpHeight, 0)),
                    width,
                    height,
                    ref size,
                    ref newWidth,
                    ref newHeight,
                    true,
                    backgroundRGB,
                    position);


                tmp.Size = size;
                tmp.Width = newWidth;
                tmp.Height = newHeight;
                tmp.FileName = hasGalleryLocation ? filename : string.Concat("alt/", filename);
                return tmp;
            }
        }
        finally
        {
            if (stream != null)
                stream.Close();
        }
        return tmp;
    }



    /// <summary>
    /// Creates the new size image.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="physicalFilepath">The physical filepath.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="size">The size.</param>
    /// <param name="newWidth">The new width.</param>
    /// <param name="newHeight">The new height.</param>
    /// <param name="isAlwaysSpecifiedSize">if set to <c>true</c> [is always specified size].</param>
    /// <param name="backgroundRgb">The background RGB.</param>
    /// <returns></returns>
    internal static string CreateNewSizeImage(this Asset inAsset, System.Drawing.Image image, string physicalFilepath, int width, int height, ref int size, ref int newWidth, ref int newHeight, bool isAlwaysSpecifiedSize, int[] backgroundRgb)
    {
        return CreateNewSizeImage(inAsset, image, physicalFilepath, width, height, ref size, ref newWidth, ref newHeight, isAlwaysSpecifiedSize, backgroundRgb, ImagePosition.Center);
    }

    /// <summary>
    /// Creates the new size image.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="physicalFilepath">The physical filepath.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="size">The size.</param>
    /// <param name="newWidth">The new width.</param>
    /// <param name="newHeight">The new height.</param>
    /// <param name="isAlwaysSpecifiedSize">if set to <c>true</c> [is always specified size].</param>
    /// <param name="backgroundRgb">The background RGB.</param>
    /// <param name="position">The position.</param>
    /// <returns></returns>
    internal static string CreateNewSizeImage(this Asset inAsset, System.Drawing.Image image, string physicalFilepath, int width, int height, ref int size, ref int newWidth, ref int newHeight, bool isAlwaysSpecifiedSize, int[] backgroundRgb, ImagePosition position)
    {
        return CreateNewSizeImage(inAsset, image, physicalFilepath, width, height, 0, 0, ref size, ref newWidth, ref newHeight, isAlwaysSpecifiedSize, backgroundRgb, position);
    }

    /// <summary>
    /// Creates the new size image.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="physicalFilepath">The physical filepath.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="cropWidth">Width of the crop.</param>
    /// <param name="cropHeight">Height of the crop.</param>
    /// <param name="size">The size.</param>
    /// <param name="newWidth">The new width.</param>
    /// <param name="newHeight">The new height.</param>
    /// <param name="isAlwaysSpecifiedSize">if set to <c>true</c> [is always specified size].</param>
    /// <param name="backgroundRgb">The background RGB.</param>
    /// <returns></returns>
    internal static string CreateNewSizeImage(this Asset inAsset, System.Drawing.Image image, string physicalFilepath, int width, int height, int cropWidth, int cropHeight, ref int size, ref int newWidth, ref int newHeight, bool isAlwaysSpecifiedSize, int[] backgroundRgb)
    {
        return CreateNewSizeImage(inAsset, image, physicalFilepath, width, height, 0, 0, ref size, ref newWidth, ref newHeight, isAlwaysSpecifiedSize, backgroundRgb, ImagePosition.Center);
    }

    /// <summary>
    /// Creates the new size image.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="physicalFilepath">The physical filepath.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="cropWidth">Width of the crop.</param>
    /// <param name="cropHeight">Height of the crop.</param>
    /// <param name="size">The size.</param>
    /// <param name="newWidth">The new width.</param>
    /// <param name="newHeight">The new height.</param>
    /// <param name="isAlwaysSpecifiedSize">if set to <c>true</c> [is always specified size].</param>
    /// <param name="backgroundRgb">The background RGB.</param>
    /// <param name="position">The position.</param>
    /// <returns></returns>
    internal static string CreateNewSizeImage(this Asset inAsset, System.Drawing.Image image, string physicalFilepath, int width, int height, int cropWidth, int cropHeight, ref int size, ref int newWidth, ref int newHeight, bool isAlwaysSpecifiedSize, int[] backgroundRgb, ImagePosition position)
    {
        using (Wim.Utilities.Thumbnailer thumb = new Wim.Utilities.Thumbnailer())
        {
            if (backgroundRgb != null && backgroundRgb.Length == 3)
                thumb.BackGroundColor = System.Drawing.Color.FromArgb(backgroundRgb[0], backgroundRgb[1], backgroundRgb[2]);

            thumb.Filepath = physicalFilepath;
            thumb.ContentType = ImageFormat.Jpeg;
            thumb.ThumbnailQuality = Wim.CommonConfiguration.GENERATED_IMAGE_QUALITY;
            thumb.IsAlwaysSpecifiedSize = isAlwaysSpecifiedSize;
            thumb.IsEnlargedWhenSmallerThenSpecified = false;

            try
            {
                System.Drawing.Image imgPhoto = null;

                imgPhoto = thumb.FixedSize(image, width, height, cropWidth, cropHeight, position);

                //  Pass ref parameters
                newWidth = imgPhoto.Width;
                newHeight = imgPhoto.Height;

                //  End passing
                thumb.SaveThumbnail(imgPhoto);
                if (imgPhoto != null)
                    imgPhoto.Dispose();

                //  Get file size and pass it on
                if (!Asset.HasCloudSetting)
                {
                    FileInfo fi = new FileInfo(physicalFilepath);
                    size = Convert.ToInt32(fi.Length);
                }
                else
                    size = 0;


                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Trying to save @'{0}' resulted into error: {1}", physicalFilepath, ex.Message), ex);
            }
        }
    }

}

