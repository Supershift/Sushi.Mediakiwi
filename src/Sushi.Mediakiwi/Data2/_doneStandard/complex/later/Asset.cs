using System;
using System.Linq;
using System.Web;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Sushi.Mediakiwi.Data.DalReflection;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Net;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Represents a Document entity.
    /// </summary>
    [DatabaseTable("wim_Assets",
        Join = "left join wim_GalleryView on Gallery_Key = Asset_Gallery_Key left join wim_AssetTypes on AssetType_Key = Asset_AssetType_Key",
        Order = "Asset_SortOrder asc, Asset_Title asc")
    ]
    public class Asset : DatabaseEntity, iExportable
    {
        //string m_RelativePath;
        /// <summary>
        /// Gets or sets the relative path.
        /// </summary>
        /// <value>The path.</value>
        public string RelativePath
        {
            get
            {
                //if (this.ID > 0 && (string.IsNullOrEmpty(m_RelativePath) || !m_RelativePath.EndsWith(FileName)))
                //{
                return this.ApplyPath(this.FileName, false, false);
                //}
                //return m_RelativePath;
            }
            //set { m_RelativePath = value; }
        }

        internal string m_Path;
        /// <summary>
        /// Gets or sets the relative path (this can also be a remote path if set in the registry!).
        /// </summary>
        /// <value>The path.</value>
        public string Path
        {
            get
            {
                if (string.IsNullOrEmpty(this.RemoteLocation))
                    return this.ApplyPath(this.FileName, false, true);

                if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Request.IsSecureConnection)
                {
                    if (this.RemoteLocation.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase))
                        return this.RemoteLocation.Replace("http://", "https://");
                }

                return this.RemoteLocation;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Asset"/> is exists.
        /// </summary>
        /// <value><c>true</c> if exists; otherwise, <c>false</c>.</value>
        public bool Exists
        {
            get
            {
                bool exists = false;
                if (!string.IsNullOrEmpty(this.RemoteLocation) && Sushi.Mediakiwi.Data.Asset.HasCloudSetting)
                {
                    if (m_CloundInstance.IsPartOfCDN(this.RemoteLocation))
                        exists = Sushi.Mediakiwi.Data.Asset.m_CloundInstance.Exists(string.Concat(this.CompletePath, "/", this.FileName));
                    else
                        exists = true;
                }
                else
                    exists = System.IO.File.Exists(this.LocalFilePath);
                return exists;
            }
        }


        public bool IsRemote
        {
            get
            {
                return (this.GalleryUrlMap != null);
            }
        }

        public Stream Stream
        {
            get
            {
                if (!String.IsNullOrEmpty(this.RemoteLocation))
                {
                    if (Sushi.Mediakiwi.Data.Asset.HasCloudSetting)
                    {
                        if (this.RemoteLocation.StartsWith(Sushi.Mediakiwi.Data.Asset.m_CloundInstance.ContentDeliveryUrl, StringComparison.CurrentCultureIgnoreCase))
                        {
                            //  Remote CDN URL
                            var path = string.Concat(this.CompletePath, "/", this.FileName);
                            return m_CloundInstance.GetFileStream(path);
                        }
                    }
                    WebClient client = new WebClient();
                    byte[] file = client.DownloadData(this.RemoteLocation);
                    Stream stream = new MemoryStream(file);
                    return stream;
                }
                return new FileStream(this.LocalFilePath, FileMode.Open, FileAccess.Read);
            }
        }

        string m_LocalFilePath;
        /// <summary>
        /// Gets or sets the local file path.
        /// </summary>
        /// <value>The local file path.</value>
        public string LocalFilePath
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    try
                    {
                        //  Exception for remote files
                        if (this.GalleryUrlMap != null) return this.CompletePath;
                        //  [20091022:MM] Got problems with ? in filenames. This is possible from Apple, but not Win.
                        //  Have fixed this in the save function, but introduced this for backwards compatibility.
                        m_LocalFilePath = HttpContext.Current.Request.MapPath(RelativePath);
                    }
                    catch (Exception) { }
                }
                return m_LocalFilePath;
            }
            set { m_LocalFilePath = value; }
        }

        string m_LocalThumbnailFilePath;
        /// <summary>
        /// Gets or sets the local thumbnail file path.
        /// </summary>
        /// <value>The local thumbnail file path.</value>
        internal string LocalThumbnailFilePath
        {
            get
            {
                //  Exception for remote files
                if (this.GalleryUrlMap != null) return this.ThumbnailPath;

                if (!string.IsNullOrEmpty(this.ThumbnailPath) && HttpContext.Current != null)
                {
                    m_LocalThumbnailFilePath = HttpContext.Current.Request.MapPath(m_ThumbnailPath);
                }
                return m_LocalThumbnailFilePath;
            }
            set { m_LocalThumbnailFilePath = value; }
        }


        /// <summary>
        /// Gets the exposed URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public string Url
        {
            get
            {
                return ConvertUrl(FullPath);
            }
        }

        string m_FullPath;
        /// <summary>
        /// Gets or sets the full path.
        /// </summary>
        /// <value>The path.</value>
        public string FullPath
        {
            get
            {
                if (string.IsNullOrEmpty(this.RemoteLocation))
                    m_FullPath = this.ApplyPath(this.FileName, true, true);
                else
                    m_FullPath = this.RemoteLocation;

                return m_FullPath.Replace("http:", string.Empty).Replace("https:", string.Empty);
            }
            set { m_FullPath = value; }
        }

        internal string GeneratedImagePath { get; set; }
        internal string CdnImagePath { get; set; }

        internal static Sushi.Mediakiwi.Data.iDataUpload m_CloundInstance;
        internal static bool HasCloudSetting
        {
            get
            {
                if (m_CloundInstance != null)
                    return true;

                bool hasCloud = !(string.IsNullOrEmpty(Wim.CommonConfiguration.CLOUD_TYPE));

                if (hasCloud)
                {
                    var split = Wim.CommonConfiguration.CLOUD_TYPE.Split(',');
                    m_CloundInstance = Wim.Utility.CreateInstance(string.Format("{0}.dll", split[1].Trim()), split[0].Trim()) as Sushi.Mediakiwi.Data.iDataUpload;
                }
                return hasCloud;
            }
        }



        //string m_DownloadUrl;
        /// <summary>
        /// Gets or sets the download URL.
        /// </summary>
        /// <value>The download URL.</value>
        public string DownloadUrl
        {
            get
            {

                if (this.GalleryUrlMap != null)
                    return string.Concat(this.GalleryUrlMap.MappedUrl, "/doc.ashx?", this.ID);

                return Wim.Utility.AddApplicationPath(string.Concat("/doc.ashx?", this.ID));
            }
            //set { m_DownloadUrl = value; }
        }


        //string m_DownloadFullUrl;
        /// <summary>
        /// Gets or sets the download URL.
        /// </summary>
        /// <value>The download URL.</value>
        public string DownloadFullUrl
        {
            get
            {
                //if (string.IsNullOrEmpty(m_DownloadFullUrl) )
                //    m_DownloadFullUrl = Wim.Utility.AddApplicationPath(string.Concat("/doc.ashx?", this.ID), true);

                if (this.GalleryUrlMap != null)
                    return string.Concat(this.GalleryUrlMap.MappedUrl, "/doc.ashx?", this.ID);

                return Wim.Utility.AddApplicationPath(string.Concat("/doc.ashx?", this.ID), true);
            }
            //set { m_DownloadFullUrl = value; }
        }


        string m_DownloadUrlByGUID;
        /// <summary>
        /// Gets or sets the download URL.
        /// </summary>
        /// <value>The download URL.</value>
        public string DownloadUrlByGUID
        {
            get
            {
                if (this.GalleryUrlMap != null)
                    return string.Concat(this.GalleryUrlMap.MappedUrl, "/doc.ashx?", this.GUID);

                //if (string.IsNullOrEmpty(m_DownloadUrlByGUID))
                return m_DownloadUrlByGUID = Wim.Utility.AddApplicationPath(string.Concat("/doc.ashx?", this.GUID), true);
                //m_DownloadUrlByGUID;
            }
            set { m_DownloadUrlByGUID = value; }
        }

        string m_DownloadFullUrlByGUID;
        /// <summary>
        /// Gets or sets the download URL.
        /// </summary>
        /// <value>The download URL.</value>
        public string DownloadFullUrlByGUID
        {
            get
            {
                if (this.GalleryUrlMap != null)
                    return string.Concat(this.GalleryUrlMap.MappedUrl, "/doc.ashx?", this.GUID);

                // if (string.IsNullOrEmpty(m_DownloadFullUrlByGUID))
                return m_DownloadFullUrlByGUID = Wim.Utility.AddApplicationPath(string.Concat("/doc.ashx?", this.GUID), true);
                //  return m_DownloadFullUrlByGUID;
            }
            set { m_DownloadFullUrlByGUID = value; }
        }

        bool m_GalleryUrlMapIsSet;
        Sushi.Mediakiwi.Framework.WimServerGalleryMapping m_GalleryUrlMap;
        /// <summary>
        /// Gets the gallery URL map.
        /// </summary>
        /// <value>The gallery URL map.</value>
        public Sushi.Mediakiwi.Framework.WimServerGalleryMapping GalleryUrlMap
        {
            get
            {
                if (!m_GalleryUrlMapIsSet)
                {
                    m_GalleryUrlMapIsSet = true;
                    m_GalleryUrlMap = Sushi.Mediakiwi.Data.Common.GetCurrentGalleryMappingUrl(this.CompletePath);
                }
                return m_GalleryUrlMap;
            }
        }

        string m_ThumbnailPath;
        /// <summary>
        /// Gets the thumbnail path.
        /// </summary>
        /// <value>The thumbnail path.</value>
        public string ThumbnailPath
        {
            get
            {
                if (string.IsNullOrEmpty(m_ThumbnailPath))
                {
                    string candidate;

                    if (this.IsImage)
                    {
                        //if (HasCloudSetting)
                        //{
                        //var file = this.ImageInstance.ApplyForcedBorder(null, 80, 80, null, false, Sushi.Mediakiwi.Data.ImagePosition.Center, null, true);
                        var portal = this.DatabaseMappingPortal == null ? null : this.DatabaseMappingPortal.Name;

                        var file = Sushi.Mediakiwi.Data.Image.GetUrl(this.ID, ImageFileFormat.ValidateMaximumAndCrop, 80, 89, null, this.CompletePath);
                        //this.ImageInstance.ApplyForcedMaximum(null, 80, 80, true);
                        return file;
                        //}
                        //if (GalleryUrlMap != null)
                        //    candidate = string.Concat(GalleryUrlMap.MappedUrl, Wim.CommonConfiguration.RelativeRepositoryImageThumbnailUrl, "/", this.ID, ".jpg");
                        //else
                        //    candidate = string.Concat(Wim.CommonConfiguration.RelativeRepositoryImageThumbnailUrl, "/", this.ID, ".jpg");
                    }
                    else
                    {
                        return null;

                        candidate = "thumb_unknown.png";
                        switch (this.Extention)
                        {
                            case "doc": candidate = "thumb_word.png"; break;
                            case "pdf": candidate = "thumb_acrobat.png"; break;
                            case "xls": candidate = "thumb_excel.png"; break;
                            case "ppt": candidate = "thumb_powerpoint.png"; break;
                            case "zip": candidate = "thumb_zip.png"; break;
                            case "mov": candidate = "thumb_mov.png"; break;
                            case "wmv": candidate = "thumb_wmv.png"; break;
                        }
                        candidate = string.Concat(Wim.CommonConfiguration.RelativeRepositoryWimUrl, "/images/", candidate);
                    }

                    //if (!string.IsNullOrEmpty(AssetServerBaseUrl))
                    //    return string.Concat(AssetServerBaseUrl, candidate);

                    if (GalleryUrlMap == null)
                        m_ThumbnailPath = Wim.Utility.AddApplicationPath(candidate);
                    else
                        m_ThumbnailPath = candidate;

                }
                return m_ThumbnailPath;
            }
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <param name="databaseMapName">Name of the database map.</param>
        /// <returns></returns>
        public static Asset SelectOneByMap(int ID, string databaseMapName)
        {
            Asset implement = new Asset();
            //  Other database connection
            implement.DatabaseMappingPortal = Sushi.Mediakiwi.Data.Common.GetCurrentMappingConnectionByName(databaseMapName);

            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            //  Removed for mass upload [MM 07.11.14]
            //whereClause.Add(new DatabaseDataValueColumn("Asset_IsActive", SqlDbType.Bit, true));
            whereClause.Add(new DatabaseDataValueColumn("Asset_Key", SqlDbType.Int, ID));

            implement = (Asset)implement._SelectOne(whereClause, "ID", ID.ToString());

            return implement;
        }

        /// <summary>
        /// Selects the one by portal.
        /// </summary>
        /// <param name="ID">The identifier.</param>
        /// <param name="portal">The portal.</param>
        /// <returns></returns>
        public static Asset SelectOneByPortal(int ID, string portal)
        {
            Asset implement = new Asset();

            //  Other database connection
            implement.DatabaseMappingPortal = Sushi.Mediakiwi.Data.Common.GetPortal(portal, false);

            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            //  Removed for mass upload [MM 07.11.14]
            //whereClause.Add(new DatabaseDataValueColumn("Asset_IsActive", SqlDbType.Bit, true));
            whereClause.Add(new DatabaseDataValueColumn("Asset_Key", SqlDbType.Int, ID));

            implement = (Asset)implement._SelectOne(whereClause, "ID", ID.ToString());
            return implement;
        }

        public static Asset SelectOneByGallery(string relativePath, int ID)
        {
            string portal = null;
            if (!string.IsNullOrEmpty(relativePath))
            {
                var mapping = Sushi.Mediakiwi.Data.Common.GetCurrentGalleryMappingUrl(relativePath);
                if (mapping != null)
                    portal = mapping.Portal;
            }
            Asset candidate = SelectOneByPortal(ID, portal);

            bool hasRemoteError;
            candidate.ParseRemoteLocation(null, out hasRemoteError);
            return candidate;
        }

        /// <summary>
        /// Selects the one by gallery.
        /// </summary>
        /// <param name="relativePath">The relative path.</param>
        /// <param name="Guid">The unique identifier.</param>
        /// <returns></returns>
        public static Asset SelectOneByGallery(string relativePath, Guid Guid)
        {
            string portal = null;
            if (!string.IsNullOrEmpty(relativePath))
            {
                var mapping = Sushi.Mediakiwi.Data.Common.GetCurrentGalleryMappingUrl(relativePath);
                if (mapping != null)
                    portal = mapping.Portal;
            }
            Asset implement = new Asset();

            //  Other database connection
            implement.DatabaseMappingPortal = Sushi.Mediakiwi.Data.Common.GetPortal(portal, false);

            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("Asset_IsActive", SqlDbType.Bit, true));
            whereClause.Add(new DatabaseDataValueColumn("Asset_GUID", SqlDbType.UniqueIdentifier, Guid));

            implement = (Asset)implement._SelectOne(whereClause, "Guid", Guid.ToString());

            bool hasRemoteError;
            implement.ParseRemoteLocation(null, out hasRemoteError);
            return implement;
        }

        public static Asset SelectOne(Guid Guid)
        {
            Asset implement = new Asset();

            //  Other database connection
            var connection = Sushi.Mediakiwi.Data.Common.GetCurrentMappingConnectionByName(null);
            if (connection != null)
                implement.SqlConnectionString = connection.Connection;

            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("Asset_IsActive", SqlDbType.Bit, true));
            whereClause.Add(new DatabaseDataValueColumn("Asset_GUID", SqlDbType.UniqueIdentifier, Guid));

            implement = (Asset)implement._SelectOne(whereClause, "Guid", Guid.ToString());

            bool hasRemoteError;
            implement.ParseRemoteLocation(null, out hasRemoteError);
            return implement;
        }
        //protected static Asset SelectOneBase(int ID)
        //{
        //    List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
        //    whereClause.Add(new DatabaseDataValueColumn("Asset_IsActive", SqlDbType.Bit, true));
        //    whereClause.Add(new DatabaseDataValueColumn("Asset_Key", SqlDbType.Int, ID));

        //    return (Asset)new Asset()._SelectOne(whereClause, "ID", ID.ToString());
        //}

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="physicalFolder">The physical folder.</param>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        internal static Asset SelectOne(string physicalFolder, string filename)
        {
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("Asset_Filename", SqlDbType.VarChar, filename));

            List<Asset> list = new List<Asset>();
            foreach (object o in new Asset()._SelectAll(whereClause))
                list.Add((Asset)o);

            string path;
            if (physicalFolder.EndsWith("\\"))
                path = string.Concat(physicalFolder, filename);
            else
                path = string.Concat(physicalFolder, "\\", filename);

            foreach (Asset asset in list)
            {
                if (path.Equals(asset.LocalFilePath, StringComparison.OrdinalIgnoreCase))
                    return asset;
            }
            return new Asset();
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="galleryID">The gallery ID.</param>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        public static Asset SelectOne(int galleryID, string filename)
        {
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("Asset_Filename", SqlDbType.VarChar, filename));
            whereClause.Add(new DatabaseDataValueColumn("Asset_Gallery_Key", SqlDbType.Int, galleryID));

            return (Asset)new Asset()._SelectOne(whereClause);
        }

        /// <summary>
        /// Selects the type of the one by asset.
        /// </summary>
        /// <param name="galleryID">The gallery ID.</param>
        /// <param name="assetTag">The asset tag.</param>
        /// <param name="databaseMapName">Name of the database map.</param>
        /// <returns></returns>
        public static Asset SelectOneByAssetType(int galleryID, string assetTag, string databaseMapName)
        {
            Asset candidate = new Asset();

            //  Other database connection
            var connection = Sushi.Mediakiwi.Data.Common.GetCurrentMappingConnectionByName(databaseMapName);
            if (connection != null)
                candidate.SqlConnectionString = connection.Connection;

            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("AssetType_Tag", SqlDbType.VarChar, assetTag));
            whereClause.Add(new DatabaseDataValueColumn("Asset_Gallery_Key", SqlDbType.Int, galleryID));

            return (Asset)candidate._SelectOne(whereClause);
        }

        /// <summary>
        /// Select an asset based on the asset variation relation
        /// </summary>
        /// <param name="assetID">The asset identifier.</param>
        /// <param name="assetTag">The assettype tag.</param>
        /// <param name="databaseMapName">Name of the database map.</param>
        /// <returns></returns>
        public static Asset SelectOneByAssetTypeVariant(int assetID, string assetTag, string databaseMapName = null)
        {
            Asset candidate = new Asset();

            //  Other database connection
            if (!string.IsNullOrEmpty(databaseMapName))
            {
                var connection = Sushi.Mediakiwi.Data.Common.GetCurrentMappingConnectionByName(databaseMapName);
                if (connection != null)
                    candidate.SqlConnectionString = connection.Connection;
            }

            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("AssetType_Tag", SqlDbType.VarChar, assetTag));
            whereClause.Add(new DatabaseDataValueColumn("Asset_Asset_Key", SqlDbType.Int, assetID));

            return (Asset)candidate._SelectOne(whereClause);
        }

        public static Asset SelectOneByAssetTypeVariantPortal(int assetID, string assetTag, string portal = null)
        {
            Asset candidate = new Asset();

            //  Other database connection
            if (!string.IsNullOrEmpty(portal))
            {
                candidate.DatabaseMappingPortal = Sushi.Mediakiwi.Data.Common.GetPortal(portal, false);
            }

            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("AssetType_Tag", SqlDbType.VarChar, assetTag));
            whereClause.Add(new DatabaseDataValueColumn("Asset_Asset_Key", SqlDbType.Int, assetID));

            return (Asset)candidate._SelectOne(whereClause);
        }

        /// <summary>
        /// Selects the one (A NON-ACTIVE ASSET!).
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        internal static Asset SelectOne(string filename, int size)
        {
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("Asset_Filename", SqlDbType.VarChar, filename));
            whereClause.Add(new DatabaseDataValueColumn("Asset_Size", SqlDbType.Int, size));
            whereClause.Add(new DatabaseDataValueColumn("Asset_IsActive", SqlDbType.Bit, false));

            List<Asset> list = new List<Asset>();
            foreach (object o in new Asset()._SelectAll(whereClause))
            {
                Asset candidate = (Asset)o;
                if (!candidate.Exists)
                    list.Add(candidate);
            }

            if (list.Count > 0)
            {
                return list[0];
            }
            else if (list.Count == 0)
            {
                whereClause = new List<DatabaseDataValueColumn>();
                whereClause.Add(new DatabaseDataValueColumn("Asset_Filename", SqlDbType.VarChar, filename));

                foreach (object o in new Asset()._SelectAll(whereClause))
                {
                    Asset candidate = (Asset)o;
                    if (!candidate.Exists)
                        list.Add(candidate);
                }

                if (list.Count == 1)
                    return list[0];
            }

            //if (list.Count > 1)
            //{
            //    return list[0];
            //    //throw new Exception(string.Format("Found multiple assets with the same configuration ({0} / {1}).", filename, size));
            //}

            return new Asset();
        }

        public static Asset[] SelectAll(int galleryID)
        {
            return SelectAll(galleryID, false);
        }

        public static Asset[] SelectAll(int galleryID, int assetTypeID)
        {
            return SelectAll(galleryID, assetTypeID, false);
        }

        public static Asset[] SelectAll(int galleryID, bool onErrorIgnore)
        {
            int? i = null;
            return SelectAll(galleryID, i, onErrorIgnore);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="galleryID">The gallery ID.</param>
        /// <param name="assetTypeID">The asset type ID.</param>
        /// <param name="onErrorIgnore">if set to <c>true</c> [on error ignore].</param>
        /// <returns></returns>
        public static Asset[] SelectAll(int galleryID, int? assetTypeID, bool onErrorIgnore)
        {
            return SelectAll(galleryID, assetTypeID, onErrorIgnore, true);
        }

        /// <summary>
        /// Selects the all_by parent.
        /// </summary>
        /// <param name="galleryID">The gallery ID.</param>
        /// <param name="assetTypeID">The asset type ID.</param>
        /// <param name="onErrorIgnore">if set to <c>true</c> [on error ignore].</param>
        /// <param name="?">The ?.</param>
        /// <returns></returns>
        public static Asset[] SelectAll(int galleryID, int? assetTypeID, bool onErrorIgnore, bool onlyReturnActiveAssets)
        {
            List<Asset> list = new List<Asset>();
            Asset implement = new Asset();

            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("Asset_Gallery_Key", SqlDbType.Int, galleryID));
            whereClause.Add(new DatabaseDataValueColumn("Asset_Asset_Key", SqlDbType.Int, null));

            if (onlyReturnActiveAssets)
                whereClause.Add(new DatabaseDataValueColumn("Asset_IsActive", SqlDbType.Bit, true));

            if (assetTypeID.HasValue)
                whereClause.Add(new DatabaseDataValueColumn("Asset_AssetType_Key", SqlDbType.Int, assetTypeID.Value));

            foreach (object o in implement._SelectAll(whereClause))
            {
                var asset = (Asset)o;

                bool hasRemoteError;
                asset.ParseRemoteLocation(null, out hasRemoteError);
                if (onErrorIgnore && hasRemoteError)
                    continue;

                list.Add(asset);
            }
            return list.ToArray();
        }

        /// <summary>
        /// Selects all asset variants
        /// </summary>
        public static Asset[] SelectAll_Variant(int parentID, string relativeGalleryPath = null)
        {
            List<Asset> list = new List<Asset>();
            Asset implement = new Asset();

            if (!string.IsNullOrEmpty(relativeGalleryPath))
            {
                var mapping = Sushi.Mediakiwi.Data.Common.GetCurrentGalleryMappingUrl(relativeGalleryPath);
                if (mapping != null)
                    implement.DatabaseMappingPortal = Sushi.Mediakiwi.Data.Common.GetPortal(mapping.Portal, false);
            }

            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("Asset_Asset_Key", SqlDbType.Int, parentID));

            foreach (object o in implement._SelectAll(whereClause))
            {
                var asset = (Asset)o;
                list.Add(asset);
            }
            return list.ToArray();
        }

        /// <summary>
        /// Selects all assets in a gallery or from galleries in the gallery
        /// </summary>
        /// <param name="galleryID">The gallery ID.</param>
        /// <param name="assetTypeID">The asset type ID.</param>
        /// <param name="onErrorIgnore">if set to <c>true</c> [on error ignore].</param>
        /// <returns></returns>
        public static Asset[] SelectAllDoubleLevel(int galleryID, int? assetTypeID, bool onErrorIgnore)
        {
            List<Asset> list = new List<Asset>();
            Asset implement = new Asset();

            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn(string.Format("(Asset_Gallery_Key = {0} or Gallery_Gallery_Key = {0})", galleryID)));
            whereClause.Add(new DatabaseDataValueColumn("Asset_IsActive", SqlDbType.Bit, true));
            whereClause.Add(new DatabaseDataValueColumn("Asset_Asset_Key", SqlDbType.Int, null));

            if (assetTypeID.HasValue)
                whereClause.Add(new DatabaseDataValueColumn("Asset_AssetType_Key", SqlDbType.Int, assetTypeID.Value));

            foreach (object o in implement._SelectAll(whereClause))
            {
                var asset = (Asset)o;

                bool hasRemoteError;
                asset.ParseRemoteLocation(null, out hasRemoteError);
                if (onErrorIgnore && hasRemoteError)
                    continue;

                list.Add(asset);
            }
            return list.ToArray();
        }

        public static Asset[] SelectAll(int galleryID, string databaseMapName)
        {
            return SelectAll(galleryID, databaseMapName, false);
        }

        /// <summary>
        /// Selects all assets by GalleryID & DatabaseName
        /// </summary>
        /// <param name="galleryID">The Gallery ID</param>
        /// <param name="databaseMapName">The external database name</param>
        /// <returns></returns>
        public static Asset[] SelectAll(int galleryID, string databaseMapName, bool onErrorIgnore)
        {
            List<Asset> list = new List<Asset>();
            Asset implement = new Asset();

            var connection = Sushi.Mediakiwi.Data.Common.GetCurrentMappingConnectionByName(databaseMapName);
            if (connection != null)
                implement.SqlConnectionString = connection.Connection;

            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("Asset_Gallery_Key", SqlDbType.Int, galleryID));
            whereClause.Add(new DatabaseDataValueColumn("Asset_IsActive", SqlDbType.Bit, true));
            whereClause.Add(new DatabaseDataValueColumn("Asset_Asset_Key", SqlDbType.Int, null));

            foreach (object o in implement._SelectAll(whereClause))
            {
                var asset = (Asset)o;

                bool hasRemoteError;
                asset.ParseRemoteLocation(databaseMapName, out hasRemoteError);

                if (onErrorIgnore && hasRemoteError)
                    continue;

                list.Add(asset);
            }
            return list.ToArray();
        }

        public static Asset[] SelectAll()
        {
            return SelectAll(false);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public static Asset[] SelectAll(bool onErrorIgnore)
        {
            List<Asset> list = new List<Asset>();

            foreach (object o in new Asset()._SelectAll())
            {
                var asset = (Asset)o;

                bool hasRemoteError;
                asset.ParseRemoteLocation(null, out hasRemoteError);

                if (onErrorIgnore && hasRemoteError)
                    continue;

                list.Add(asset);
            }
            return list.ToArray();
        }

        public static List<Asset> SelectRange(int[] arr)
        {
            List<Asset> list = new List<Asset>();
            Asset implement = new Asset();

            string arrText = Wim.Utility.ConvertToCsvString(arr);
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("Asset_Key", SqlDbType.Int, arrText, DatabaseDataValueCompareType.In));

            foreach (object o in new Asset()._SelectAll(whereClause))
            {
                var asset = (Asset)o;

                bool hasRemoteError;
                asset.ParseRemoteLocation(null, out hasRemoteError);

                list.Add(asset);
            }

            return list;
        }

        public static List<Asset> SelectRange(List<int> IDs)
        {
            return SelectRange(IDs.ToArray());
        }

        public static List<Asset> SelectRange(List<int> IDs, string databaseMapName)
        {
            return SelectRange(IDs, databaseMapName, false);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public static List<Asset> SelectRange(List<int> IDs, string databaseMapName, bool IgnoreRemoteError)
        {
            Asset implement = new Asset();
            List<Asset> list = new List<Asset>();

            string arrText = Wim.Utility.ConvertToCsvString(IDs.ToArray());
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("Asset_Key", SqlDbType.Int, arrText, DatabaseDataValueCompareType.In));

            var connection = Sushi.Mediakiwi.Data.Common.GetCurrentMappingConnectionByName(databaseMapName);
            if (connection != null)
                implement.SqlConnectionString = connection.Connection;

            foreach (object o in implement._SelectAll(whereClause))
            {
                var asset = (Asset)o;

                bool hasRemoteError;
                asset.ParseRemoteLocation(databaseMapName, out hasRemoteError);
                if (IgnoreRemoteError && hasRemoteError)
                    continue;

                list.Add(asset);
            }

            return list;
        }

        public static Asset[] SearchAll(string searchCandidate, int? galleryID)
        {
            return SearchAll(searchCandidate, galleryID, false);
        }

        /// <summary>
        /// Searches all.
        /// </summary>
        /// <param name="searchCandidate">The search candidate.</param>
        /// <param name="galleryID">The gallery ID.</param>
        /// <returns></returns>
        public static Asset[] SearchAll(string searchCandidate, int? galleryID, bool onErrorIgnore)
        {
            List<Asset> list = new List<Asset>();
            if (string.IsNullOrEmpty(searchCandidate)) return list.ToArray();

            Asset implement = new Asset();

            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();

            searchCandidate = string.Concat("%", searchCandidate.Trim().Replace(" ", "%"), "%");
            whereClause.Add(new DatabaseDataValueColumn("Asset_Title", SqlDbType.VarChar, searchCandidate, DatabaseDataValueCompareType.Like));
            whereClause.Add(new DatabaseDataValueColumn("Asset_Description", SqlDbType.VarChar, searchCandidate, DatabaseDataValueCompareType.Like, DatabaseDataValueConnectType.Or));
            whereClause.Add(new DatabaseDataValueColumn("Asset_Asset_Key", SqlDbType.Int, null));

            if (galleryID.HasValue)
            {
                whereClause.Add(new DatabaseDataValueColumn("Asset_Gallery_Key", SqlDbType.Int, galleryID));
                whereClause.Add(new DatabaseDataValueColumn("Asset_IsActive", SqlDbType.Bit, true));
            }

            foreach (object o in implement._SelectAll(whereClause))
            {
                var asset = (Asset)o;

                bool hasRemoteError;
                asset.ParseRemoteLocation(null, out hasRemoteError);

                if (onErrorIgnore && hasRemoteError)
                    continue;

                list.Add(asset);
            }
            return list.ToArray();
        }

        //internal static void DeleteInactive()
        //{
        //    Asset implement = new Asset();

        //    List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
        //    whereClause.Add(new DatabaseDataValueColumn("Asset_Gallery_Key", SqlDbType.Int, galleryID));

        //    implement.Delete(
        //}


        /// <summary>
        /// Guesses the type.
        /// </summary>
        /// <param name="extention">The extention.</param>
        /// <returns></returns>
        public static string GuessType(string extention)
        {
            string candidate = null;
            if (MIMETypesDictionary.TryGetValue(extention.ToLower(), out candidate))
                return candidate;
            return "application/octet-stream";
            //switch (extention)
            //{
            //    case "jpg": return "image/jpeg"; 
            //    case "png": return "image/png";
            //    case "gif": return "image/gif";
            //    case "pdf": return " application/pdf"; 
            //    case "doc": return "application/msword";
            //    case "xls": return "application/octet-stream";
            //    case "txt": return "text/plain"; 
            //    case "xml": return "application/octet-stream"; 
            //    case "fla": return "application/octet-stream";
            //    case "flv": return "application/octet-stream";
            //    case "exe": return "application/exe";
            //    case "dmg": return "application/x-apple-diskimage";

            //    default: return "application/octet-stream";
            //}
        }


        /// <summary>
        /// Creates the thumbnail.
        /// </summary>
        //public void CreateThumbnail()
        //{
        //    CreateThumbnail(false);
        //}

        /// <summary>
        /// Creates the thumbnail if it does not exist yet.
        /// </summary>
        /// <param name="forceCreation">if set to <c>true</c> [force creation].</param>
        //public void CreateThumbnail(bool forceCreation)
        //{
        //    if (HttpContext.Current == null)
        //        throw new Exception("No HttpContent present!");

        //    if (!this.Exists)
        //    {
        //        if (string.IsNullOrEmpty(this.RemoteLocation) || this.RemoteDownload)
        //            return;
        //    }
        //    if (this.GalleryUrlMap != null) return;
        //    if (!forceCreation)
        //        if (File.Exists(this.LocalThumbnailFilePath)) 
        //            return;

        //    EvaluateAssetType();
        //}


        /// <summary>
        /// Creates the image thumbnail.
        /// </summary>
        /// <param name="gallery">The gallery.</param>
        /// <param name="fileStream">The file stream.</param>
        /// <param name="localFilePath">The local file path.</param>
        /// <param name="localFileName">Name of the local file.</param>
        /// <param name="saveStream">if set to <c>true</c> [save stream].</param>
        /// <param name="overWriteImage">if set to <c>true</c> [over write image].</param>
        //void CreateImageThumbnail(Sushi.Mediakiwi.Data.Gallery gallery, System.IO.Stream fileStream, string localFilePath, string localFileName, bool saveStream, bool overWriteImage)
        //{
        //    //CB: Check toegevoegd omdat de fromstream excepties gaf.Wat is nl. het geval als de stream null, niet leesbaar of niet zoekbaar is dan kan je niets met fromstream
        //    if (fileStream != null && fileStream.CanRead && fileStream.CanSeek  )
        //    {
        //        using (System.Drawing.Image imageStreamed = System.Drawing.Image.FromStream(fileStream))
        //        {
        //            this.Width = imageStreamed.Width;
        //            this.Height = imageStreamed.Height;

        //            if (fileStream.CanSeek)
        //                this.Size = fileStream.Length;

        //            //  Create the image.
        //            int size = 0, newWidth = 0, newHeight = 0;

        //            //  Without APP PATH!
        //            string fileThumbnailPath = string.Concat(Sushi.Mediakiwi.Data.Gallery.LocalThumbnailRepositoryBase, this.ID, ".jpg");
        //            CreateNewSizeImage(imageStreamed, fileThumbnailPath, 80, 80, ref size, ref newWidth, ref newHeight, true, gallery.GetBackgroundRgb());

        //            if (gallery.FormatType > 0)
        //            {
        //                #region Convert uploaded image to jpeg.
        //                if (gallery.FormatType == 1)
        //                {
        //                    localFilePath = string.Concat(localFilePath, this.ID, ".jpg");

        //                    CreateNewSizeImage(imageStreamed, localFilePath
        //                        , imageStreamed.Width
        //                        , imageStreamed.Height
        //                        , ref size, ref newWidth, ref newHeight, true, gallery.GetBackgroundRgb());

        //                    this.Size = size;
        //                    this.Width = newWidth;
        //                    this.Height = newHeight;
        //                    this.Extention = "jpg";
        //                }
        //                #endregion

        //                #region Convert uploaded image to jpeg with a fixed format (with border).
        //                if (gallery.FormatType == 2)
        //                {
        //                    string filename;
        //                    List<Sushi.Mediakiwi.Data.ImageFile> fileList = new List<Sushi.Mediakiwi.Data.ImageFile>();

        //                    int imageCounter = 0;
        //                    string[] formatElements = gallery.Format.Split(';');
        //                    foreach (string formatItem in formatElements)
        //                    {
        //                        imageCounter++;

        //                        string[] formatSplit = formatItem.Split('x');

        //                        decimal newTmpWidth = Convert.ToDecimal(formatSplit[0]);
        //                        decimal newTmpHeight = Convert.ToDecimal(formatSplit[1]);

        //                        if (newTmpWidth == 0 && newTmpHeight == 0) throw new Exception("The Gallery format can not be 0x0");
        //                        if (newTmpWidth == 0) newTmpWidth = decimal.Multiply(decimal.Divide(newHeight, imageStreamed.Height), imageStreamed.Width);
        //                        if (newTmpHeight == 0) newTmpHeight = decimal.Multiply(decimal.Divide(newTmpWidth, imageStreamed.Width), imageStreamed.Height);

        //                        if (formatElements.Length > 0)
        //                            filename = string.Concat(this.ID, "_", imageCounter);
        //                        else
        //                            filename = this.ID.ToString();

        //                        CreateNewSizeImage(imageStreamed, string.Concat(localFilePath, filename, ".jpg")
        //                            , Convert.ToInt32(Math.Round(newTmpWidth, 0))
        //                            , Convert.ToInt32(Math.Round(newTmpHeight, 0))
        //                            , ref size, ref newWidth, ref newHeight, true, gallery.GetBackgroundRgb());

        //                        Sushi.Mediakiwi.Data.ImageFile file = new Sushi.Mediakiwi.Data.ImageFile();

        //                        file.Extention = "jpg";
        //                        file.Size = size;
        //                        file.Width = newWidth;
        //                        file.Height = newHeight;
        //                        file.Name = filename;
        //                        fileList.Add(file);

        //                        this.Size = size;
        //                        this.Width = newWidth;
        //                        this.Height = newHeight;
        //                        this.Extention = "jpg";
        //                    }
        //                    //this.SerializedData = Wim.Utility.GetSerialized(fileList.ToArray());
        //                }
        //                #endregion

        //                #region Convert uploaded image to jpeg to and validate maximum format (no border).
        //                if (gallery.FormatType == 3)
        //                {
        //                    string filename;

        //                    List<Sushi.Mediakiwi.Data.ImageFile> fileList = new List<Sushi.Mediakiwi.Data.ImageFile>();

        //                    int imageCounter = 0;
        //                    string[] formatElements = gallery.Format.Split(';');
        //                    foreach (string formatItem in formatElements)
        //                    {
        //                        imageCounter++;

        //                        string[] formatSplit = formatItem.Split('x');

        //                        decimal newTmpWidth = Convert.ToDecimal(formatSplit[0]);
        //                        decimal newTmpHeight = Convert.ToDecimal(formatSplit[1]);

        //                        if (newTmpWidth != 0 && newTmpWidth > imageStreamed.Width)
        //                            newTmpWidth = imageStreamed.Width;

        //                        if (newTmpHeight != 0 && newTmpHeight > imageStreamed.Height)
        //                            newTmpHeight = imageStreamed.Height;

        //                        if (newTmpWidth == 0 && newTmpHeight == 0) throw new Exception("The Gallery format can not be 0x0");
        //                        if (newTmpWidth == 0) newTmpWidth = decimal.Multiply(decimal.Divide(newHeight, imageStreamed.Height), imageStreamed.Width);
        //                        if (newTmpHeight == 0) newTmpHeight = decimal.Multiply(decimal.Divide(newTmpWidth, imageStreamed.Width), imageStreamed.Height);

        //                        if (formatElements.Length > 0)
        //                            filename = string.Concat(this.ID, "_", imageCounter);
        //                        else
        //                            filename = this.ID.ToString();

        //                        CreateNewSizeImage(imageStreamed, string.Concat(localFilePath, filename, ".jpg")
        //                            , Convert.ToInt32(Math.Round(newTmpWidth, 0))
        //                            , Convert.ToInt32(Math.Round(newTmpHeight, 0))
        //                            , ref size, ref newWidth, ref newHeight, false, gallery.GetBackgroundRgb());

        //                        Sushi.Mediakiwi.Data.ImageFile file = new Sushi.Mediakiwi.Data.ImageFile();

        //                        file.Extention = "jpg";
        //                        file.Size = size;
        //                        file.Width = newWidth;
        //                        file.Height = newHeight;
        //                        file.Name = filename;
        //                        fileList.Add(file);

        //                        this.Size = size;
        //                        this.Width = newWidth;
        //                        this.Height = newHeight;
        //                        this.Extention = "jpg";
        //                    }
        //                    //this.SerializedData = Wim.Utility.GetSerialized(fileList.ToArray());
        //                }
        //                #endregion
        //            }
        //            else
        //            {
        //                //  Save original image
        //                Sushi.Mediakiwi.Data.Notification.InsertOne("ImageThumbnail", NotificationType.Information, string.Format("{0}-{1}-{2}", saveStream, File.Exists(localFileName), overWriteImage));
        //                if ((saveStream && !File.Exists(localFileName)) || overWriteImage)
        //                {
        //                    this.Created = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
        //                    imageStreamed.Save(localFileName);
        //                    this.Save();
        //                }
        //            }
        //            fileStream.Close();
        //            fileStream.Dispose();
        //            imageStreamed.Dispose();
        //        }
        //        Wim.Utilities.CacheItemManager.FlushIndexOfCacheObjects("Image");
        //        Wim.Utilities.CacheItemManager.FlushIndexOfCacheObjects("Asset");
        //    }
        //}

        /// <summary>
        /// Gets or sets a value indicating whether [show button multi].
        /// </summary>
        /// <value><c>true</c> if [show button multi]; otherwise, <c>false</c>.</value>
        public bool ShowButtonMulti { get; set; }

        /// <summary>
        /// Gets a value indicating whether [show single asset].
        /// </summary>
        /// <value><c>true</c> if [show single asset]; otherwise, <c>false</c>.</value>
        public bool ShowSingleAsset { get { return !ShowButtonMulti; } }

        /// <summary>
        /// Gets a value indicating whether [show single asset and not in cloud].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show single asset and not in cloud]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowSingleAssetAndNotInCloud
        {
            get
            {
                if (Sushi.Mediakiwi.Data.Asset.HasCloudSetting)
                    return false;
                return ShowSingleAsset;
            }
        }

        // TODO determine this setting on something. 
        public bool RelyOnGeneratedAssets { get { return System.Configuration.ConfigurationManager.AppSettings["Sushi.Mediakiwi.Data.Image.GeneratedAssets"] == "true"; } }

        /// <summary>
        /// Validates the access right.
        /// </summary>
        /// <param name="assets">The assets.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public static Sushi.Mediakiwi.Data.Asset[] ValidateAccessRight(Sushi.Mediakiwi.Data.Asset[] assets, Sushi.Mediakiwi.Data.ApplicationUser user)
        {
            return (from item in assets join relation in Sushi.Mediakiwi.Data.Gallery.SelectAllAccessible(user) on item.GalleryID equals relation.ID select item).ToArray();
        }







        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard







        Image m_Image;
        /// <summary>
        /// Gets the image instance.
        /// </summary>
        /// <value>The image instance.</value>
        public Image ImageInstance
        {
            get
            {
                if (m_Image == null && this.IsImage)
                {
                    m_Image = new Image();
                    Wim.Utility.ReflectProperty(this, m_Image);
                }
                return m_Image;
            }
        }


        Document m_Document;
        /// <summary>
        /// Gets the document instance.
        /// </summary>
        /// <value>The document instance.</value>
        public Document DocumentInstance
        {
            get
            {
                //if (m_Document == null && !this.IsImage)
                if (m_Document == null)
                {
                    m_Document = new Document();
                    Wim.Utility.ReflectProperty(this, m_Document);
                }
                return m_Document;
            }
        }

        public static Asset SelectOne(int galleryID, int assetTypeID)
        {
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("Asset_AssetType_Key", SqlDbType.Int, assetTypeID));
            whereClause.Add(new DatabaseDataValueColumn("Asset_Gallery_Key", SqlDbType.Int, galleryID));

            return (Asset)new Asset()._SelectOne(whereClause);
        }

        public static List<Asset> SelectAll_Local()
        {
            List<Asset> list = new List<Asset>();
            Asset implement = new Asset();

            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("Asset_RemoteLocation is null"));
            whereClause.Add(new DatabaseDataValueColumn("Asset_Asset_Key", SqlDbType.Int, null));

            foreach (object o in new Asset()._SelectAll(whereClause))
            {
                var asset = (Asset)o;
                list.Add(asset);
            }
            return list;
        }

        private static readonly Dictionary<string, string> MIMETypesDictionary = new Dictionary<string, string>()
  {
    {"ai", "application/postscript"},
    {"aif", "audio/x-aiff"},
    {"aifc", "audio/x-aiff"},
    {"aiff", "audio/x-aiff"},
    {"asc", "text/plain"},
    {"atom", "application/atom+xml"},
    {"au", "audio/basic"},
    {"avi", "video/x-msvideo"},
    {"bcpio", "application/x-bcpio"},
    {"bin", "application/octet-stream"},
    {"bmp", "image/bmp"},
    {"cdf", "application/x-netcdf"},
    {"cgm", "image/cgm"},
    {"class", "application/octet-stream"},
    {"cpio", "application/x-cpio"},
    {"cpt", "application/mac-compactpro"},
    {"csh", "application/x-csh"},
    {"css", "text/css"},
    {"dcr", "application/x-director"},
    {"dif", "video/x-dv"},
    {"dir", "application/x-director"},
    {"djv", "image/vnd.djvu"},
    {"djvu", "image/vnd.djvu"},
    {"dll", "application/octet-stream"},
    {"dmg", "application/octet-stream"},
    {"dms", "application/octet-stream"},
    {"doc", "application/msword"},
    {"docx","application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
    {"dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template"},
    {"docm","application/vnd.ms-word.document.macroEnabled.12"},
    {"dotm","application/vnd.ms-word.template.macroEnabled.12"},
    {"dtd", "application/xml-dtd"},
    {"dv", "video/x-dv"},
    {"dvi", "application/x-dvi"},
    {"dxr", "application/x-director"},
    {"eps", "application/postscript"},
    {"etx", "text/x-setext"},
    {"exe", "application/octet-stream"},
    {"ez", "application/andrew-inset"},
    {"gif", "image/gif"},
    {"gram", "application/srgs"},
    {"grxml", "application/srgs+xml"},
    {"gtar", "application/x-gtar"},
    {"hdf", "application/x-hdf"},
    {"hqx", "application/mac-binhex40"},
    {"htm", "text/html"},
    {"html", "text/html"},
    {"ice", "x-conference/x-cooltalk"},
    {"ico", "image/x-icon"},
    {"ics", "text/calendar"},
    {"ief", "image/ief"},
    {"ifb", "text/calendar"},
    {"iges", "model/iges"},
    {"igs", "model/iges"},
    {"jnlp", "application/x-java-jnlp-file"},
    {"jp2", "image/jp2"},
    {"jpe", "image/jpeg"},
    {"jpeg", "image/jpeg"},
    {"jpg", "image/jpeg"},
    {"js", "application/x-javascript"},
    {"kar", "audio/midi"},
    {"latex", "application/x-latex"},
    {"lha", "application/octet-stream"},
    {"lzh", "application/octet-stream"},
    {"m3u", "audio/x-mpegurl"},
    {"m4a", "audio/mp4a-latm"},
    {"m4b", "audio/mp4a-latm"},
    {"m4p", "audio/mp4a-latm"},
    {"m4u", "video/vnd.mpegurl"},
    {"m4v", "video/x-m4v"},
    {"mac", "image/x-macpaint"},
    {"man", "application/x-troff-man"},
    {"mathml", "application/mathml+xml"},
    {"me", "application/x-troff-me"},
    {"mesh", "model/mesh"},
    {"mid", "audio/midi"},
    {"midi", "audio/midi"},
    {"mif", "application/vnd.mif"},
    {"mov", "video/quicktime"},
    {"movie", "video/x-sgi-movie"},
    {"mp2", "audio/mpeg"},
    {"mp3", "audio/mpeg"},
    {"mp4", "video/mp4"},
    {"mpe", "video/mpeg"},
    {"mpeg", "video/mpeg"},
    {"mpg", "video/mpeg"},
    {"mpga", "audio/mpeg"},
    {"ms", "application/x-troff-ms"},
    {"msh", "model/mesh"},
    {"mxu", "video/vnd.mpegurl"},
    {"nc", "application/x-netcdf"},
    {"oda", "application/oda"},
    {"ogg", "application/ogg"},
    {"pbm", "image/x-portable-bitmap"},
    {"pct", "image/pict"},
    {"pdb", "chemical/x-pdb"},
    {"pdf", "application/pdf"},
    {"pgm", "image/x-portable-graymap"},
    {"pgn", "application/x-chess-pgn"},
    {"pic", "image/pict"},
    {"pict", "image/pict"},
    {"png", "image/png"},
    {"pnm", "image/x-portable-anymap"},
    {"pnt", "image/x-macpaint"},
    {"pntg", "image/x-macpaint"},
    {"ppm", "image/x-portable-pixmap"},
    {"ppt", "application/vnd.ms-powerpoint"},
    {"pptx","application/vnd.openxmlformats-officedocument.presentationml.presentation"},
    {"potx","application/vnd.openxmlformats-officedocument.presentationml.template"},
    {"ppsx","application/vnd.openxmlformats-officedocument.presentationml.slideshow"},
    {"ppam","application/vnd.ms-powerpoint.addin.macroEnabled.12"},
    {"pptm","application/vnd.ms-powerpoint.presentation.macroEnabled.12"},
    {"potm","application/vnd.ms-powerpoint.template.macroEnabled.12"},
    {"ppsm","application/vnd.ms-powerpoint.slideshow.macroEnabled.12"},
    {"ps", "application/postscript"},
    {"qt", "video/quicktime"},
    {"qti", "image/x-quicktime"},
    {"qtif", "image/x-quicktime"},
    {"ra", "audio/x-pn-realaudio"},
    {"ram", "audio/x-pn-realaudio"},
    {"ras", "image/x-cmu-raster"},
    {"rdf", "application/rdf+xml"},
    {"rgb", "image/x-rgb"},
    {"rm", "application/vnd.rn-realmedia"},
    {"roff", "application/x-troff"},
    {"rtf", "text/rtf"},
    {"rtx", "text/richtext"},
    {"sgm", "text/sgml"},
    {"sgml", "text/sgml"},
    {"sh", "application/x-sh"},
    {"shar", "application/x-shar"},
    {"silo", "model/mesh"},
    {"sit", "application/x-stuffit"},
    {"skd", "application/x-koan"},
    {"skm", "application/x-koan"},
    {"skp", "application/x-koan"},
    {"skt", "application/x-koan"},
    {"smi", "application/smil"},
    {"smil", "application/smil"},
    {"snd", "audio/basic"},
    {"so", "application/octet-stream"},
    {"spl", "application/x-futuresplash"},
    {"src", "application/x-wais-source"},
    {"sv4cpio", "application/x-sv4cpio"},
    {"sv4crc", "application/x-sv4crc"},
    {"svg", "image/svg+xml"},
    {"swf", "application/x-shockwave-flash"},
    {"t", "application/x-troff"},
    {"tar", "application/x-tar"},
    {"tcl", "application/x-tcl"},
    {"tex", "application/x-tex"},
    {"texi", "application/x-texinfo"},
    {"texinfo", "application/x-texinfo"},
    {"tif", "image/tiff"},
    {"tiff", "image/tiff"},
    {"tr", "application/x-troff"},
    {"tsv", "text/tab-separated-values"},
    {"txt", "text/plain"},
    {"ustar", "application/x-ustar"},
    {"vcd", "application/x-cdlink"},
    {"vrml", "model/vrml"},
    {"vxml", "application/voicexml+xml"},
    {"wav", "audio/x-wav"},
    {"wbmp", "image/vnd.wap.wbmp"},
    {"wbmxl", "application/vnd.wap.wbxml"},
    {"wml", "text/vnd.wap.wml"},
    {"wmlc", "application/vnd.wap.wmlc"},
    {"wmls", "text/vnd.wap.wmlscript"},
    {"wmlsc", "application/vnd.wap.wmlscriptc"},
    {"wrl", "model/vrml"},
    {"xbm", "image/x-xbitmap"},
    {"xht", "application/xhtml+xml"},
    {"xhtml", "application/xhtml+xml"},
    {"xls", "application/vnd.ms-excel"},
    {"xml", "application/xml"},
    {"xpm", "image/x-xpixmap"},
    {"xsl", "application/xml"},
    {"xlsx","application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
    {"xltx","application/vnd.openxmlformats-officedocument.spreadsheetml.template"},
    {"xlsm","application/vnd.ms-excel.sheet.macroEnabled.12"},
    {"xltm","application/vnd.ms-excel.template.macroEnabled.12"},
    {"xlam","application/vnd.ms-excel.addin.macroEnabled.12"},
    {"xlsb","application/vnd.ms-excel.sheet.binary.macroEnabled.12"},
    {"xslt", "application/xslt+xml"},
    {"xul", "application/vnd.mozilla.xul+xml"},
    {"xwd", "image/x-xwindowdump"},
    {"xyz", "chemical/x-xyz"},
    {"zip", "application/zip"}
  };


        public Asset()
        {
            this.IsNewStyle = true;
            this.IsActive = true;
        }

        string ConvertUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return url;

            if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                return url;

            //  Remove HTTP: and make it //
            return url.Substring(5, url.Length - 5);
        }

        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("ShowSingleAsset")]
        [Sushi.Mediakiwi.Framework.ContentListItem.TextLine("Remote location")]
        [DatabaseColumn("Asset_RemoteLocation", SqlDbType.VarChar, Length = 512, IsNullable = true)]
        public string RemoteLocation { get; set; }

        [DatabaseColumn("Asset_RemoteLocation_Thumb", SqlDbType.VarChar, Length = 512, IsNullable = true)]
        public string RemoteLocation_Thumb { get; set; }

        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("ShowSingleAssetAndNotInCloud")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Download", InteractiveHelp = "When checked this file is downloaded to the server")]
        [DatabaseColumn("Asset_RemoteDownload", SqlDbType.Bit, IsNullable = true)]
        public bool RemoteDownload { get; set; }


        Sushi.Mediakiwi.Data.CustomData m_Data;
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        [DatabaseColumn("Asset_Data", SqlDbType.Xml, IsNullable = true)]
        public Sushi.Mediakiwi.Data.CustomData Data
        {
            get
            {
                if (m_Data == null)
                    m_Data = new CustomData();
                return m_Data;
            }
            set { m_Data = value; }
        }
        int m_ID;
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        [DatabaseColumn("Asset_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }


        private Guid m_GUID;
        /// <summary>
        /// Gets or sets the GUID.
        /// </summary>
        /// <value>The GUID.</value>
        [DatabaseColumn("Asset_GUID", SqlDbType.UniqueIdentifier, IsMigrationKey = true)]
        public Guid GUID
        {
            get
            {
                if (this.m_GUID == Guid.Empty) this.m_GUID = Guid.NewGuid();
                return m_GUID;
            }
            set { m_GUID = value; }
        }

        int m_GalleryID;
        /// <summary>
        /// Gets or sets the gallery ID.
        /// </summary>
        /// <value>The gallery ID.</value>
        [DatabaseColumn("Asset_Gallery_Key", SqlDbType.Int)]
        //[Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Gallery", "GalleryCollection")]
        public int GalleryID
        {
            get { return m_GalleryID; }
            set { m_GalleryID = value; }
        }

        string m_Title;
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("ShowSingleAsset")]
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("_title", 255, true)]
        [DatabaseColumn("Asset_Title", SqlDbType.NVarChar, Length = 255, IsNullable = true)]
        public string Title
        {
            get { return m_Title; }
            set { m_Title = value; }
        }




        string m_FileName;
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        //[Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsExisting")]
        //[Sushi.Mediakiwi.Framework.ContentListItem.TextLine("Filename")]
        [DatabaseColumn("Asset_Filename", SqlDbType.VarChar, Length = 255, IsNullable = true)]
        public string FileName
        {
            get { return m_FileName; }
            set
            {
                if (this.ID == 0)
                {
                    m_FileName = value;
                    if (!string.IsNullOrEmpty(value))
                    {
                        m_FileName = m_FileName
                            .Replace("'", string.Empty)
                            .Replace("?", string.Empty);
                        return;
                    }
                }
                m_FileName = value;
            }
        }

        int m_BaseGalleryID;
        /// <summary>
        /// Gets or sets the base gallery ID.
        /// </summary>
        /// <value>The base gallery ID.</value>

        [DatabaseColumn("Gallery_Base_Key", SqlDbType.Int, IsOnlyRead = true, IsNullable = true)]
        public int BaseGalleryID
        {
            get { return m_BaseGalleryID; }
            set { m_BaseGalleryID = value; }
        }

        string m_CompletePath;
        /// <summary>
        /// Gets or sets the complete path.
        /// </summary>
        /// <value>The complete path.</value>
        [DatabaseColumn("Gallery_CompletePath", SqlDbType.NVarChar, Length = 1000, IsOnlyRead = true, IsNullable = true)]
        public string CompletePath
        {
            get { return m_CompletePath; }
            set { m_CompletePath = value; }
        }

        string m_Extention;
        /// <summary>
        /// Gets or sets the extention.
        /// </summary>
        /// <value>The extention.</value>
        [DatabaseColumn("Asset_Extention", SqlDbType.VarChar, Length = 20, IsNullable = true)]
        public string Extention
        {
            get { return m_Extention; }
            set { m_Extention = value; }
        }

        public string ExtentionClassName
        {
            get
            {
                if (!string.IsNullOrEmpty(Extention))
                {
                    switch (Extention.ToLower())
                    {
                        case "docx":
                        case "doc": return "doc";
                        case "pdf": return "pdf";
                        case "jpeg":
                        case "jpg":
                        case "png":
                        case "bmp":
                        case "gif": return "image";
                        case "xls":
                        case "xlsx": return "xls";
                        case "ppt":
                        case "pptx": return "ppt";
                        case "zip":
                        case "rar": return "zip";
                        case "vsd": return "vsd";
                        case "eml":
                        case "msg": return "msg";
                        case "txt": return "txt";
                        case "nof": return "nof";
                        default: return "unk";
                    }
                }
                else
                    return "unk";
            }
        }

        long m_Size;
        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        [DatabaseColumn("Asset_Size", SqlDbType.Int)]
        public long Size
        {
            get { return m_Size; }
            set { m_Size = value; }
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [DatabaseColumn("Asset_Type", SqlDbType.VarChar, Length = 150, IsNullable = true)]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the asset type ID.
        /// </summary>
        /// <value>The asset type ID.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("ShowSingleAsset")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Type", "AssetTypes")]
        [DatabaseColumn("Asset_AssetType_Key", SqlDbType.Int, IsNullable = true)]
        public int? AssetTypeID { get; set; }

        [DatabaseColumn("Asset_Asset_Key", SqlDbType.Int, IsNullable = true)]
        public int? ParentID { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("ShowSingleAsset")]
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("_description", 500, false)]
        [DatabaseColumn("Asset_Description", SqlDbType.NText, IsNullable = true)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the type of the asset.
        /// </summary>
        /// <value>The type of the asset.</value>
        [DatabaseColumn("AssetType_Name", SqlDbType.NVarChar, IsNullable = true, IsOnlyRead = true)]
        public string AssetType { get; set; }

        [DatabaseColumn("AssetType_Tag", SqlDbType.VarChar, IsNullable = true, IsOnlyRead = true)]
        public string AssetTypeTag { get; set; }

        DateTime m_Created;
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        [DatabaseColumn("Asset_Created", SqlDbType.DateTime)]
        public DateTime Created
        {
            get
            {
                if (this.m_Created == DateTime.MinValue) this.m_Created = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
                return m_Created;
            }
            set { m_Created = value; }
        }

        bool m_IsOldStyle = false;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is old style.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is old style; otherwise, <c>false</c>.
        /// </value>
        [DatabaseColumn("Asset_OldStyle", SqlDbType.Bit)]
        public bool IsOldStyle
        {
            get { return m_IsOldStyle; }
            set { m_IsOldStyle = value; }
        }

        bool m_IsNewStyle;
        /// <summary>
        /// Gets or sets a value indicating whether [new style].
        /// </summary>
        /// <value><c>true</c> if [new style]; otherwise, <c>false</c>.</value>
        [DatabaseColumn("Asset_NewStyle", SqlDbType.Bit)]
        public bool IsNewStyle
        {
            get { return m_IsNewStyle; }
            set { m_IsNewStyle = value; }
        }

        int m_Height;
        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        [DatabaseColumn("Asset_Height", SqlDbType.Int, IsNullable = true)]
        public int Height
        {
            get { return m_Height; }
            set { m_Height = value; }
        }

        [DatabaseColumn("Asset_SortOrder", SqlDbType.Int, IsNullable = true)]
        public int? SortOrder { get; set; }

        int m_Width;
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        [DatabaseColumn("Asset_Width", SqlDbType.Int, IsNullable = true)]
        public int Width
        {
            get { return m_Width; }
            set { m_Width = value; }
        }

        //string m_SerializedData;
        ///// <summary>
        ///// Gets or sets the serialized data used for ImageFiles.
        ///// </summary>
        ///// <value>The serialized data.</value>
        //[DatabaseColumn("Asset_Data", SqlDbType.NText, IsNullable = true)]
        //public string SerializedData
        //{
        //    get { return m_SerializedData; }
        //    set { m_SerializedData = value; }
        //}

        bool m_IsImage;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is old style.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is old style; otherwise, <c>false</c>.
        /// </value>
        [DatabaseColumn("Asset_IsImage", SqlDbType.Bit)]
        public bool IsImage
        {
            get { return m_IsImage; }
            set { m_IsImage = value; }
        }

        bool m_IsActive;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        [DatabaseColumn("Asset_IsActive", SqlDbType.Bit)]
        public bool IsActive
        {
            get { return m_IsActive; }
            set { m_IsActive = value; }
        }

        /// <summary>
        /// Deletes the inactive.
        /// </summary>
        /// <returns></returns>
        internal static bool DeleteInactive()
        {
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("Asset_IsActive", SqlDbType.Bit, false));

            return new Asset().Delete(whereClause);
        }

        #region iExportable Members

        private DateTime? m_updated;
        [DatabaseColumn("Asset_Updated", SqlDbType.DateTime, IsNullable = true)]
        public DateTime? Updated { get; set; }

        #endregion

        public static Asset SelectOne(int ID)
        {
            return SelectOneByGallery(null, ID);
        }


        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard

        #region MOVED to EXTENSION / LOGIC

        ///// <summary>
        ///// Selects the in active.
        ///// </summary>
        ///// <returns></returns>
        //internal static int SelectInActive()
        //{
        //    Asset implement = new Asset();
        //    return Utility.ConvertToInt(implement.Execute("select count(*) from wim_Assets where Asset_IsActive = 0"));
        //}

        //internal static void UpdateActive()
        //{
        //    Asset implement = new Asset();
        //    implement.Execute("update wim_Assets set Asset_IsActive = 0");
        //}

        //internal static void UpdateActive(int assetID)
        //{
        //    Asset implement = new Asset();
        //    implement.Execute(string.Concat("update wim_Assets set Asset_IsActive = 0 where Asset_Key = ", assetID));
        //}


        //internal static bool CloudUpload(Sushi.Mediakiwi.Data.Asset document, Sushi.Mediakiwi.Data.Gallery gallery, HttpPostedFile file)
        //{
        //    bool result = false;
        //    document.ValidateImage(file.InputStream, file.FileName);
        //    if (Sushi.Mediakiwi.Data.Asset.HasCloudSetting)
        //    {
        //        Sushi.Mediakiwi.Data.Asset.m_CloundInstance.Upload(document, gallery, file);
        //        result = true;
        //    }
        //    return result;
        //}

        //internal static bool CloudUpload(Sushi.Mediakiwi.Data.Asset document, Sushi.Mediakiwi.Data.Gallery gallery, System.IO.Stream fileStream, string fileName, string contentType)
        //{
        //    bool result = false;
        //    document.ValidateImage(fileStream, fileName);
        //    if (Sushi.Mediakiwi.Data.Asset.HasCloudSetting)
        //    {
        //        Sushi.Mediakiwi.Data.Asset.m_CloundInstance.Upload(document, gallery, fileStream, fileName, contentType);
        //        result = true;
        //    }
        //    return result;
        //}


        //internal void ValidateImage(bool autoSave)
        //{
        //    System.Drawing.Image gfx = null;

        //    if (File.Exists(this.LocalFilePath))
        //        gfx = System.Drawing.Image.FromFile(this.LocalFilePath);

        //    ValidateImage(autoSave, gfx);
        //}

        ///// <summary>
        ///// Validate if the stream is an image and if it is, determin the Width and Height.
        ///// </summary>
        ///// <param name="stream">The stream.</param>
        ///// <param name="filename">The filename.</param>
        ///// <param name="autoSave">if set to <c>true</c> [automatic save].</param>
        //public void ValidateImage(Stream stream, string filename, bool autoSave = false)
        //{
        //    if (string.IsNullOrEmpty(this.Extention))
        //    {
        //        int pindex = filename.LastIndexOf('.') + 1;
        //        this.Extention = filename.Substring(pindex, filename.Length - pindex).ToLower();
        //    }

        //    this.IsImage = (
        //        this.Extention.Equals("gif", StringComparison.CurrentCultureIgnoreCase) ||
        //        this.Extention.Equals("jpg", StringComparison.CurrentCultureIgnoreCase) ||
        //        this.Extention.Equals("jpeg", StringComparison.CurrentCultureIgnoreCase) ||
        //        this.Extention.Equals("png", StringComparison.CurrentCultureIgnoreCase)
        //        );

        //    if (this.IsImage && (this.Width == 0 || this.Height == 0))
        //    {
        //        if (stream == null)
        //            return;

        //        var gfx = System.Drawing.Image.FromStream(stream);
        //        ValidateImage(autoSave, gfx);
        //    }
        //}

        //public void ValidateImage(bool autoSave, System.Drawing.Image gfx)
        //{
        //    if (this.IsImage)
        //    {
        //        if (this.Height == 0 || this.Width == 0)
        //        {
        //            try
        //            {
        //                if (gfx != null)
        //                {
        //                    this.Height = gfx.Height;
        //                    this.Width = gfx.Width;
        //                    if (autoSave) this.Save();
        //                }
        //            }
        //            catch (Exception) { }
        //        }
        //    }
        //}

        ///// <summary>
        ///// Creates the new size image.
        ///// </summary>
        ///// <param name="image">The image.</param>
        ///// <param name="physicalFilepath">The physical filepath.</param>
        ///// <param name="width">The width.</param>
        ///// <param name="height">The height.</param>
        ///// <param name="size">The size.</param>
        ///// <param name="newWidth">The new width.</param>
        ///// <param name="newHeight">The new height.</param>
        ///// <param name="isAlwaysSpecifiedSize">if set to <c>true</c> [is always specified size].</param>
        ///// <param name="backgroundRgb">The background RGB.</param>
        ///// <returns></returns>
        //protected internal string CreateNewSizeImage(System.Drawing.Image image, string physicalFilepath, int width, int height, ref int size, ref int newWidth, ref int newHeight, bool isAlwaysSpecifiedSize, int[] backgroundRgb)
        //{
        //    return CreateNewSizeImage(image, physicalFilepath, width, height, ref size, ref newWidth, ref newHeight, isAlwaysSpecifiedSize, backgroundRgb, ImagePosition.Center);
        //}

        ///// <summary>
        ///// Creates the new size image.
        ///// </summary>
        ///// <param name="image">The image.</param>
        ///// <param name="physicalFilepath">The physical filepath.</param>
        ///// <param name="width">The width.</param>
        ///// <param name="height">The height.</param>
        ///// <param name="size">The size.</param>
        ///// <param name="newWidth">The new width.</param>
        ///// <param name="newHeight">The new height.</param>
        ///// <param name="isAlwaysSpecifiedSize">if set to <c>true</c> [is always specified size].</param>
        ///// <param name="backgroundRgb">The background RGB.</param>
        ///// <param name="position">The position.</param>
        ///// <returns></returns>
        //protected internal string CreateNewSizeImage(System.Drawing.Image image, string physicalFilepath, int width, int height, ref int size, ref int newWidth, ref int newHeight, bool isAlwaysSpecifiedSize, int[] backgroundRgb, ImagePosition position)
        //{
        //    return CreateNewSizeImage(image, physicalFilepath, width, height, 0, 0, ref size, ref newWidth, ref newHeight, isAlwaysSpecifiedSize, backgroundRgb, position);
        //}

        ///// <summary>
        ///// Creates the new size image.
        ///// </summary>
        ///// <param name="image">The image.</param>
        ///// <param name="physicalFilepath">The physical filepath.</param>
        ///// <param name="width">The width.</param>
        ///// <param name="height">The height.</param>
        ///// <param name="cropWidth">Width of the crop.</param>
        ///// <param name="cropHeight">Height of the crop.</param>
        ///// <param name="size">The size.</param>
        ///// <param name="newWidth">The new width.</param>
        ///// <param name="newHeight">The new height.</param>
        ///// <param name="isAlwaysSpecifiedSize">if set to <c>true</c> [is always specified size].</param>
        ///// <param name="backgroundRgb">The background RGB.</param>
        ///// <returns></returns>
        //protected internal string CreateNewSizeImage(System.Drawing.Image image, string physicalFilepath, int width, int height, int cropWidth, int cropHeight, ref int size, ref int newWidth, ref int newHeight, bool isAlwaysSpecifiedSize, int[] backgroundRgb)
        //{
        //    return CreateNewSizeImage(image, physicalFilepath, width, height, 0, 0, ref size, ref newWidth, ref newHeight, isAlwaysSpecifiedSize, backgroundRgb, ImagePosition.Center);
        //}

        ///// <summary>
        ///// Creates the new size image.
        ///// </summary>
        ///// <param name="image">The image.</param>
        ///// <param name="physicalFilepath">The physical filepath.</param>
        ///// <param name="width">The width.</param>
        ///// <param name="height">The height.</param>
        ///// <param name="cropWidth">Width of the crop.</param>
        ///// <param name="cropHeight">Height of the crop.</param>
        ///// <param name="size">The size.</param>
        ///// <param name="newWidth">The new width.</param>
        ///// <param name="newHeight">The new height.</param>
        ///// <param name="isAlwaysSpecifiedSize">if set to <c>true</c> [is always specified size].</param>
        ///// <param name="backgroundRgb">The background RGB.</param>
        ///// <param name="position">The position.</param>
        ///// <returns></returns>
        //protected internal string CreateNewSizeImage(System.Drawing.Image image, string physicalFilepath, int width, int height, int cropWidth, int cropHeight, ref int size, ref int newWidth, ref int newHeight, bool isAlwaysSpecifiedSize, int[] backgroundRgb, ImagePosition position)
        //{
        //    using (Wim.Utilities.Thumbnailer thumb = new Wim.Utilities.Thumbnailer())
        //    {
        //        if (backgroundRgb != null && backgroundRgb.Length == 3)
        //        {
        //            thumb.BackGroundColor = Color.FromArgb(backgroundRgb[0], backgroundRgb[1], backgroundRgb[2]);
        //        }
        //        thumb.Filepath = physicalFilepath;
        //        thumb.ContentType = ImageFormat.Jpeg;
        //        thumb.ThumbnailQuality = Wim.CommonConfiguration.GENERATED_IMAGE_QUALITY;
        //        thumb.IsAlwaysSpecifiedSize = isAlwaysSpecifiedSize;
        //        thumb.IsEnlargedWhenSmallerThenSpecified = false;

        //        try
        //        {
        //            System.Drawing.Image imgPhoto = null;

        //            imgPhoto = thumb.FixedSize(image, width, height, cropWidth, cropHeight, position);

        //            //  Pass ref parameters
        //            newWidth = imgPhoto.Width;
        //            newHeight = imgPhoto.Height;

        //            //  End passing
        //            thumb.SaveThumbnail(imgPhoto);
        //            if (imgPhoto != null)
        //                imgPhoto.Dispose();

        //            //  Get file size and pass it on
        //            if (!HasCloudSetting)
        //            {
        //                System.IO.FileInfo fi = new System.IO.FileInfo(physicalFilepath);
        //                size = Convert.ToInt32(fi.Length);
        //            }
        //            else
        //                size = 0;


        //            return null;

        //            //return Utility.RemApplicationPath(imagePath);
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception(string.Format("Trying to save @'{0}' resulted into error: {1}", physicalFilepath, ex.Message), ex);
        //        }
        //    }
        //}

        ///// <summary>
        ///// Creates the size of the attached image.
        ///// </summary>
        ///// <param name="width">The width.</param>
        ///// <param name="height">The height.</param>
        ///// <param name="format">The format.</param>
        ///// <returns></returns>
        //protected internal Sushi.Mediakiwi.Data.Image CreateAttachedImageSize(int width, int height, ImageFileFormat format)
        //{
        //    return CreateAttachedImageSize(width, height, format, null, ImagePosition.Center);
        //}

        ///// <summary>
        ///// Creates the size of the attached image.
        ///// </summary>
        ///// <param name="width">The width.</param>
        ///// <param name="height">The height.</param>
        ///// <param name="format">The format.</param>
        ///// <param name="backgroundRGB">The background RGB.</param>
        ///// <param name="position">The position.</param>
        ///// <returns></returns>
        //protected internal Sushi.Mediakiwi.Data.Image CreateAttachedImageSize(int width, int height, ImageFileFormat format, int[] backgroundRGB, ImagePosition position)
        //{
        //    return CreateAttachedImageSize(width, height, format, backgroundRGB, position, null);
        //}

        ///// <summary>
        ///// Creates the size of the attached image.
        ///// </summary>
        ///// <param name="width">The width.</param>
        ///// <param name="height">The height.</param>
        ///// <param name="format">The format.</param>
        ///// <param name="backgroundRGB">The background RGB.</param>
        ///// <param name="position">The position.</param>
        ///// <param name="gallery">The gallery.</param>
        ///// <returns></returns>
        //protected internal Sushi.Mediakiwi.Data.Image CreateAttachedImageSize(int width, int height, ImageFileFormat format, int[] backgroundRGB, ImagePosition position, Sushi.Mediakiwi.Data.Gallery gallery)
        //{
        //    if (format == ImageFileFormat.None || string.IsNullOrEmpty(FileName))
        //        return null;

        //    bool generatedFileExists = false;

        //    //if (backgroundRGB == null || backgroundRGB.Length != 3)
        //    //    backgroundRGB = Wim.CommonConfiguration.DEFAULT_THUMB_BGCOLOR;
        //    var portal = this.DatabaseMappingPortal == null ? null : this.DatabaseMappingPortal.Name;


        //    string filename = string.Concat(this.ID, ".jpg");
        //    string hash = Sushi.Mediakiwi.Data.Image.GetCdnHash(this.ID, format, width, height, backgroundRGB, this.CompletePath, true);

        //    string physicalFile = string.Concat(Sushi.Mediakiwi.Data.Gallery.LocalGeneratedImageRepositoryBase, hash, "/", filename);
        //    string physicalFolder = string.Concat(Sushi.Mediakiwi.Data.Gallery.LocalGeneratedImageRepositoryBase, hash, "/");

        //    bool hasGalleryLocation = (gallery != null);
        //    if (hasGalleryLocation)
        //    {
        //        physicalFile = string.Concat(Sushi.Mediakiwi.Data.Gallery.LocalRepositoryBase, gallery.CompletePath, "/", filename);
        //        physicalFolder = string.Concat(Sushi.Mediakiwi.Data.Gallery.LocalRepositoryBase, gallery.CompletePath);
        //    }

        //    Sushi.Mediakiwi.Data.Image tmp = new Image();
        //    Wim.Utility.ReflectProperty(this, tmp);
        //    tmp.m_Path = null;
        //    tmp.m_FullPath = null;
        //    tmp.m_LocalFilePath = null;
        //    tmp.GeneratedImagePath = Wim.Utility.AddApplicationPath(string.Concat("/repository/generated/", hash, "/", filename));
        //    tmp.CdnImagePath = Wim.Utility.AddApplicationPath(string.Concat("/cdn/", hash, "/", filename));
        //    if (this is Sushi.Mediakiwi.Data.Image)
        //    {
        //        Sushi.Mediakiwi.Data.Image iTmp = (Sushi.Mediakiwi.Data.Image)this;
        //        if (iTmp != null && iTmp.GalleryUrlMap != null)
        //            tmp.CdnImagePath = string.Concat(iTmp.GalleryUrlMap.MappedUrl, "/cdn/", hash, "/", filename);
        //    }
        //    //tmp.m_DownloadUrl = null;

        //    if (System.IO.File.Exists(physicalFile) && !HasCloudSetting)
        //    {
        //        generatedFileExists = true;
        //        tmp.FileName = hasGalleryLocation ? filename : string.Concat("alt/", filename);
        //        tmp.Width = 0;
        //        tmp.Height = 0;
        //        return tmp;
        //    }

        //    string originalPhysicalFile = string.Concat(Sushi.Mediakiwi.Data.Gallery.LocalRepositoryBase, CompletePath, "\\", this.FileName);
        //    System.Drawing.Image gfx = null;
        //    Stream stream = null;
        //    try
        //    {
        //        if (!System.IO.File.Exists(originalPhysicalFile))
        //        {
        //            if (!string.IsNullOrEmpty(this.RemoteLocation) && !this.RemoteDownload)
        //            {
        //                WebClient client = new WebClient();

        //                try
        //                {
        //                    string location = this.RemoteLocation;
        //                    if (location.StartsWith("//"))
        //                        location = "http:" + location;
        //                    if (!location.StartsWith("http://") && !location.StartsWith("https://"))
        //                        location = "http://" + location;
        //                    stream = client.OpenRead(location);
        //                }
        //                catch (Exception ex)
        //                {
        //                    try
        //                    {
        //                        Sushi.Mediakiwi.Data.Image gf = Sushi.Mediakiwi.Data.Image.SelectOne(Wim.CommonConfiguration.NO_IMAGE_ASSET);
        //                        if (gf.RemoteDownload)
        //                            stream = client.OpenRead(Sushi.Mediakiwi.Data.Image.SelectOne(Wim.CommonConfiguration.NO_IMAGE_ASSET).FullPath);
        //                        else
        //                            stream = new FileStream(Sushi.Mediakiwi.Data.Image.SelectOne(Wim.CommonConfiguration.NO_IMAGE_ASSET).LocalFilePath, FileMode.Open);
        //                    }
        //                    catch
        //                    {
        //                        //NOTE DV 2013-02-12: why repeat a statement that throws an exception? this just causes the exception to occur again...
        //                        //stream = new FileStream(Sushi.Mediakiwi.Data.Image.SelectOne(Wim.CommonConfiguration.NO_IMAGE_ASSET).LocalFilePath, FileMode.Open);                                
        //                    }
        //                }
        //                if (stream != null)
        //                {
        //                    //DV 20123-05-08: if the image file is corrupt, this call will fail. this happened in tristar project (not surprisingly),
        //                    //if it fails, we don't want the whole application to crash
        //                    //so, let's return null
        //                    try
        //                    {
        //                        gfx = System.Drawing.Image.FromStream(stream);
        //                    }
        //                    catch
        //                    {
        //                        return null;
        //                    }
        //                }
        //                else
        //                    return null;
        //                //stream.Close(); 
        //            }
        //            else
        //            {
        //                tmp.Width = 0;
        //                tmp.Height = 0;
        //                return tmp;
        //            }
        //        }
        //        else
        //            gfx = System.Drawing.Image.FromFile(originalPhysicalFile);

        //        int size = 0;
        //        int newWidth = 0;
        //        int newHeight = 0;

        //        this.ValidateImage(true, gfx);

        //        if (format == ImageFileFormat.FixedBorder)
        //        {
        //            decimal newTmpWidth = Convert.ToDecimal(width);
        //            decimal newTmpHeight = Convert.ToDecimal(height);

        //            if (!Directory.Exists(physicalFolder) && !HasCloudSetting)
        //            {
        //                Directory.CreateDirectory(physicalFolder);
        //            }

        //            CreateNewSizeImage(gfx, physicalFile
        //                , Convert.ToInt32(Math.Round(newTmpWidth, 0))
        //                , Convert.ToInt32(Math.Round(newTmpHeight, 0))
        //                , ref size, ref newWidth, ref newHeight, true, backgroundRGB, position);

        //            tmp.Size = size;
        //            tmp.Width = newWidth;
        //            tmp.Height = newHeight;
        //            tmp.FileName = hasGalleryLocation ? filename : string.Concat("alt/", filename);
        //            return tmp;
        //        }
        //        else if (format == ImageFileFormat.ValidateMaximumWidth || format == ImageFileFormat.ValidateMaximumHeight)
        //        {
        //            if (generatedFileExists && ((format == ImageFileFormat.ValidateMaximumHeight && this.Height <= height) || (format == ImageFileFormat.ValidateMaximumWidth && this.Width <= width)))
        //                return tmp;

        //            if (!Directory.Exists(physicalFolder) && !HasCloudSetting)
        //                Directory.CreateDirectory(physicalFolder);

        //            decimal newTmpWidth = Convert.ToDecimal(width);
        //            decimal newTmpHeight = Convert.ToDecimal(height);

        //            if (format == ImageFileFormat.ValidateMaximumHeight)
        //            {
        //                newTmpWidth = decimal.Multiply(decimal.Divide(newTmpHeight, this.Height), this.Width);
        //            }
        //            if (format == ImageFileFormat.ValidateMaximumWidth)
        //            {
        //                newTmpHeight = decimal.Multiply(decimal.Divide(newTmpWidth, this.Width), this.Height);
        //            }

        //            CreateNewSizeImage(gfx, physicalFile
        //                , Convert.ToInt32(Math.Round(newTmpWidth, 0))
        //                , Convert.ToInt32(Math.Round(newTmpHeight, 0))
        //                , ref size, ref newWidth, ref newHeight, true, backgroundRGB, position);

        //            tmp.Size = size;
        //            tmp.Width = newWidth;
        //            tmp.Height = newHeight;
        //            tmp.FileName = hasGalleryLocation ? filename : string.Concat("alt/", filename);
        //            return tmp;
        //        }
        //        else if (format == ImageFileFormat.ValidateMaximumWidthAndHeight)
        //        {
        //            if (generatedFileExists && ((format == ImageFileFormat.ValidateMaximumHeight && this.Height <= height) || (format == ImageFileFormat.ValidateMaximumWidth && this.Width <= width)))
        //                return tmp;

        //            if (!Directory.Exists(physicalFolder) && !HasCloudSetting)
        //                Directory.CreateDirectory(physicalFolder);

        //            decimal factorW = decimal.Divide(this.Width, width);
        //            decimal factorH = decimal.Divide(this.Height, height);

        //            decimal factor = factorW > factorH ? factorW : factorH;

        //            decimal newTmpWidth = decimal.Divide(Width, factor);
        //            decimal newTmpHeight = decimal.Divide(Height, factor);

        //            CreateNewSizeImage(gfx, physicalFile
        //                , Convert.ToInt32(Math.Round(newTmpWidth, 0))
        //                , Convert.ToInt32(Math.Round(newTmpHeight, 0))
        //                , ref size, ref newWidth, ref newHeight, true, backgroundRGB, position);

        //            tmp.Size = size;
        //            tmp.Width = newWidth;
        //            tmp.Height = newHeight;
        //            tmp.FileName = hasGalleryLocation ? filename : string.Concat("alt/", filename);
        //            return tmp;
        //        }
        //        else if (format == ImageFileFormat.ValidateMaximumAndCrop)
        //        {
        //            if (generatedFileExists && ((format == ImageFileFormat.ValidateMaximumHeight && this.Height <= height) || (format == ImageFileFormat.ValidateMaximumWidth && this.Width <= width)))
        //                return tmp;

        //            if (!Directory.Exists(physicalFolder) && !HasCloudSetting)
        //                Directory.CreateDirectory(physicalFolder);

        //            //decimal newTmpWidth = Convert.ToDecimal(width);
        //            //decimal newTmpHeight = Convert.ToDecimal(height);

        //            //decimal newTmpWidth1 = decimal.Multiply(decimal.Divide(newTmpHeight, this.Height), this.Width);
        //            //decimal newTmpHeight1 = decimal.Multiply(decimal.Divide(newTmpWidth, this.Width), this.Height);

        //            //if (newTmpWidth1 > width)
        //            //    newTmpWidth = newTmpWidth1;
        //            //else if (newTmpHeight1 > height)
        //            //    newTmpHeight = newTmpHeight1;

        //            decimal factorW = decimal.Divide(this.Width, width);
        //            decimal factorH = decimal.Divide(this.Height, height);

        //            decimal factor = factorW > factorH ? factorW : factorH;

        //            decimal newTmpWidth = decimal.Divide(Width, factor);
        //            decimal newTmpHeight = decimal.Divide(Height, factor);


        //            CreateNewSizeImage(gfx, physicalFile
        //                , Convert.ToInt32(Math.Round(newTmpWidth, 0))
        //                , Convert.ToInt32(Math.Round(newTmpHeight, 0))
        //                , width, height, ref size, ref newWidth, ref newHeight, true, backgroundRGB, position);


        //            tmp.Size = size;
        //            tmp.Width = newWidth;
        //            tmp.Height = newHeight;
        //            tmp.FileName = hasGalleryLocation ? filename : string.Concat("alt/", filename);
        //            return tmp;
        //        }
        //    }
        //    finally
        //    {
        //        if (stream != null)
        //            stream.Close();
        //    }
        //    return tmp;
        //}

        ///// <summary>
        ///// Evaluates the type of the asset.
        ///// </summary>
        //public void EvaluateAssetType()
        //{
        //    Gallery gallery = Gallery.SelectOne(this.GalleryID);
        //    //string filepath = string.Concat(Wim.CommonConfiguration.RelativeRepositoryUrl, gallery.CompletePath, "/");
        //    //string relative = Utility.AddApplicationPath(string.Format("{0}{1}", filepath, this.FileName));

        //    string localFilePath = string.Concat(Sushi.Mediakiwi.Data.Gallery.LocalRepositoryBase, gallery.CompletePath, "/");
        //    string localFileName = string.Concat(localFilePath, this.FileName);

        //    FileInfo nfo = new FileInfo(localFileName);
        //    this.Extention = nfo.Extension.Replace(".", string.Empty);

        //    if (string.IsNullOrEmpty(this.Title))
        //        this.Title = this.FileName.Replace(nfo.Extension, string.Empty);

        //    if (!string.IsNullOrEmpty(this.RemoteLocation))
        //    {
        //        // CB: Dit is helemaal uitgezet omdat het fouten gaf bij Tristar WorkerROle.
        //        //     Het zit het in het feit dat er nodeloos een stream wordt geopend. Deze zou naar EvaluateAssetType worden aangeboden maar daar doet deze functie niets met de stream
        //        //     
        //        //WebClient client = new WebClient();
        //        //Stream stream = null;
        //        // CB: De try catch weer ge-uncomment. Dit zorgde er voor dat de gehele galleries bij gametron niet werkte.
        //        //try
        //        //{
        //        //    stream = client.OpenRead(this.RemoteLocation);
        //        //}
        //        //catch (Exception)
        //        //{
        //        //    if (Wim.CommonConfiguration.NO_IMAGE_ASSET > 0)
        //        //        stream = client.OpenRead(Sushi.Mediakiwi.Data.Image.SelectOne(Wim.CommonConfiguration.NO_IMAGE_ASSET).FullPath);
        //        //    else
        //        //        return;
        //        //}                

        //        EvaluateAssetType(gallery, null, localFilePath, localFileName, false, false);
        //        this.Save();
        //        return;
        //    }

        //    using (FileStream fileStream = new FileStream(localFileName, System.IO.FileMode.Open, FileAccess.Read, FileShare.Read))
        //    {
        //        EvaluateAssetType(gallery, fileStream, localFilePath, localFileName, false, false);
        //    }
        //}

        ///// <summary>
        ///// Evaluates the type of the asset.
        ///// </summary>
        ///// <param name="gallery">The gallery.</param>
        ///// <param name="fileStream">The file stream.</param>
        ///// <param name="localFilePath">The local file path.</param>
        ///// <param name="localFileName">Name of the local file.</param>
        ///// <param name="saveStream">if set to <c>true</c> [save stream].</param>
        ///// <param name="overWriteImage">if set to <c>true</c> [over write image].</param>
        //void EvaluateAssetType(Gallery gallery, System.IO.Stream fileStream, string localFilePath, string localFileName, bool saveStream, bool overWriteImage)
        //{
        //    this.IsImage = (
        //        this.Extention.Equals("gif", StringComparison.CurrentCultureIgnoreCase) ||
        //        this.Extention.Equals("jpg", StringComparison.CurrentCultureIgnoreCase) ||
        //        this.Extention.Equals("jpeg", StringComparison.CurrentCultureIgnoreCase) ||
        //        this.Extention.Equals("png", StringComparison.CurrentCultureIgnoreCase)
        //        );

        //    this.IsNewStyle = true;
        //    this.IsOldStyle = false;

        //    if (this.IsNewInstance)
        //        this.Save();

        //    if (IsImage)
        //    {
        //        //ValidateImage(false, System.Drawing.Image.FromStream(fileStream));
        //        //CreateImageThumbnail(gallery, fileStream, localFilePath, localFileName, saveStream, overWriteImage);
        //        //this.Save();
        //    }
        //}
        ///// <summary>
        ///// Save a database entity. This method will indentify the need for an INSERT or an UPDATE.
        ///// </summary>
        ///// <returns></returns>
        //public override bool Save()
        //{
        //    if (string.IsNullOrEmpty(this.Type))
        //        this.GuessType();

        //    Updated = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;

        //    bool shouldSetSortorder = (this.ID == 0);
        //    bool isSaved = base.Save();

        //    if (!this.SortOrder.HasValue)
        //    {
        //        this.SortOrder = this.ID;
        //        this.Save();
        //    }

        //    //TURNED OFF AS OF SPEED PROBLEMS... MOVING TO STOREDPROC.
        //    //Execute("update wim_Galleries set Gallery_Count = (select count(*) from wim_Assets where Asset_Gallery_Key = Gallery_Key)");
        //    //var gallery = (from item in Sushi.Mediakiwi.Data.Gallery.SelectAll() where item.ID == this.GalleryID select item).ToArray();
        //    //if (gallery.Length > 0)
        //    //    gallery[0].AssetCount = Wim.Utility.ConvertToInt(Execute(string.Concat("select count(*) from wim_Assets where Asset_Gallery_Key = ", this.GalleryID)));

        //    return isSaved;
        //}

        //public string GetAvailableName(int galleryID, string fileName)
        //{
        //    string possibleName = fileName;
        //    string extension = string.Empty;
        //    int index = fileName.LastIndexOf('.') + 1;

        //    extension = fileName.Substring(index, fileName.Length - index);
        //    string filepart = fileName.Replace(string.Concat(".", extension), string.Empty);

        //    int refCount = 1;
        //    int count = 0;
        //    //  Check for digital file existance
        //    while (refCount > 0)
        //    {
        //        refCount = Convert.ToInt32(Execute(string.Format("select count(*) from wim_Assets where Asset_Gallery_Key = {0} and Asset_Filename = '{1}'", galleryID, possibleName)));
        //        if (refCount > 0)
        //        {
        //            count++;
        //            possibleName = string.Concat(filepart, "(", count, ").", extension);
        //        }
        //    }

        //    return possibleName;
        //}

        //void ParseRemoteLocation(string databaseMap, out bool hasRemoteError)
        //{
        //    hasRemoteError = false;
        //    if (!RemoteDownload || string.IsNullOrEmpty(RemoteLocation))
        //    {
        //        if (!RemoteDownload && !string.IsNullOrEmpty(RemoteLocation) &&
        //                (
        //                string.IsNullOrEmpty(this.Extention) ||
        //                string.IsNullOrEmpty(this.Type) ||
        //                string.IsNullOrEmpty(this.FileName)
        //                )
        //            )
        //        {
        //            this.Extention = this.RemoteLocation.Split('/').Last().Split('.').Last();
        //            this.FileName = string.Concat(this.ID, ".", this.Extention);
        //            GuessType();
        //            this.Save();
        //        }
        //        return;
        //    }

        //    this.Extention = this.RemoteLocation.Split('/').Last().Split('.').Last();
        //    string localFile = string.Concat(Sushi.Mediakiwi.Data.Gallery.SelectOne(this.GalleryID).LocalFolderPath, @"\", this.ID, ".", this.Extention);

        //    if (System.IO.File.Exists(localFile)) return;

        //    try
        //    {
        //        WebClient client = new WebClient();
        //        client.DownloadFile(RemoteLocation, localFile);

        //        this.SaveStream(new FileInfo(localFile), Gallery.SelectOne(this.GalleryID));

        //        this.FileName = string.Concat(this.ID, ".", this.Extention);
        //        this.Save();
        //    }
        //    catch (Exception ex)
        //    {
        //        hasRemoteError = true;
        //        Sushi.Mediakiwi.Data.Notification.InsertOne("Asset.ParseRemoteLocation", NotificationType.Warning, ex);
        //    }
        //}
        //void GuessType()
        //{
        //    this.Type = GuessType(this.Extention);
        //}

        ///// <summary>
        ///// Saves the stream2.
        ///// </summary>
        ///// <param name="fileStream">The file stream.</param>
        ///// <param name="gallery">The gallery.</param>
        ///// <param name="saveStream">if set to <c>true</c> [save stream].</param>
        ///// <param name="overWriteFile">if set to <c>true</c> [over write file].</param>
        //void SaveStream2(System.IO.Stream fileStream, Sushi.Mediakiwi.Data.Gallery gallery, bool saveStream, bool overWriteFile)
        //{
        //    if (gallery.IsNewInstance)
        //        throw new Exception("The request gallery does not exist.");
        //    this.CompletePath = gallery.CompletePath;

        //    if (fileStream.Length == 0)
        //        return;

        //    //System.Web.HttpContext context = System.Web.HttpContext.Current;
        //    if (Sushi.Mediakiwi.Data.Asset.HasCloudSetting == false)
        //        gallery.CreateFolder();

        //    string filepart = this.FileName.Replace(string.Concat(".", this.Extention), string.Empty);

        //    if (string.IsNullOrEmpty(this.Title))
        //        this.Title = filepart;

        //    if (!overWriteFile)
        //    {
        //        int refCount = 1;
        //        int count = 0;
        //        //  Check for digital file existance
        //        while (refCount > 0)
        //        {
        //            //EDIT DV 2013-05-13: Added 'and asset_key <> this.ID', otherwise it could take itself in the count...
        //            refCount = Convert.ToInt32(Execute(string.Format("select count(Asset_Key) from wim_Assets where Asset_Gallery_Key = {0} and Asset_Filename = '{1}' and Asset_Key <> {2}", gallery.ID, this.FileName, this.ID)));
        //            if (refCount > 0)
        //            {
        //                if (overWriteFile)
        //                {
        //                    this.ID = Sushi.Mediakiwi.Data.Asset.SelectOne(gallery.ID, this.FileName).ID;
        //                    break;
        //                }
        //                else
        //                {
        //                    count++;
        //                    this.FileName = string.Concat(filepart, "(", count, ").", Extention);
        //                }
        //            }
        //        }

        //        if (count > 0 && this.Title == filepart)
        //        {
        //            this.Title = this.FileName.Replace(string.Concat(".", this.Extention), string.Empty);
        //        }
        //    }


        //    string localFilePath = string.Concat(Sushi.Mediakiwi.Data.Gallery.LocalRepositoryBase, gallery.CompletePath, "/");
        //    string localFileName = string.Concat(localFilePath, this.FileName);
        //    //string filepath = string.Concat(Wim.CommonConfiguration.RelativeRepositoryUrl, gallery.CompletePath, "/");
        //    //string relative = string.Format("{0}{1}", filepath, this.FileName));


        //    if (!overWriteFile)
        //    {
        //        int count = 0;
        //        //  Check for physical file existance
        //        while (File.Exists(localFileName))
        //        {
        //            count++;
        //            this.FileName = string.Concat(filepart, "(", count, ").", Extention);
        //            localFileName = string.Format("{0}{1}", localFilePath, this.FileName);
        //        }
        //    }

        //    //EvaluateAssetType(gallery, fileStream, localFilePath, localFileName, saveStream, overWriteFile);

        //    if (saveStream)
        //    {
        //        //first try to upload to cloud
        //        bool isUploadedToCloud = Sushi.Mediakiwi.Data.Image.CloudUpload(this, gallery, fileStream, this.FileName, this.Type);

        //        //delete all generatedAssets for this asset, they are not valid anymore because we save new content
        //        if (this.RelyOnGeneratedAssets && this.ID > 0)
        //        {
        //            string portal = null;
        //            if (this.DatabaseMappingPortal != null)
        //                portal = this.DatabaseMappingPortal.Name;
        //            //Sushi.Mediakiwi.DataEntities.GeneratedAsset.DeleteAllForID(this.ID, portal);
        //        }

        //        //if not, save local
        //        if (isUploadedToCloud)
        //        {
        //            //  Delete local file
        //            this.DeleteFile();
        //        }
        //        else
        //        {
        //            //set local file create mode depending on if allowed to overwrite
        //            FileMode fileMode = overWriteFile ? FileMode.Create : FileMode.CreateNew;
        //            using (BinaryReader br = new BinaryReader(fileStream))
        //            {
        //                // Line below added by MarkR on 03-01-2013, for fixing an issue with the line buffer = br.ReadBytes(1024);
        //                // This was returning 0 so the loop became an infinite one.
        //                if (br.BaseStream != null)
        //                    br.BaseStream.Position = 0;
        //                using (FileStream fs = new FileStream(localFileName, fileMode))
        //                {
        //                    long bytesRead = 0;
        //                    Byte[] buffer;

        //                    while (bytesRead < br.BaseStream.Length)
        //                    {
        //                        buffer = br.ReadBytes(1024);
        //                        fs.Write(buffer, 0, buffer.Length);
        //                        bytesRead += buffer.Length;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}


        ///// <summary>
        ///// Saves the stream.
        ///// </summary>
        ///// <param name="file">The file.</param>
        ///// <param name="gallery">The gallery.</param>
        //public void SaveStream(System.Web.HttpPostedFile file, Sushi.Mediakiwi.Data.Gallery gallery)
        //{
        //    if (file.ContentLength == 0)
        //        return;

        //    int index = file.FileName.LastIndexOf('.') + 1;
        //    string[] fileSplit = file.FileName.Split('\\');

        //    this.Extention = file.FileName.Substring(index, file.FileName.Length - index);
        //    this.FileName = fileSplit[fileSplit.Length - 1];

        //    this.GalleryID = gallery.ID;
        //    this.Type = file.ContentType;
        //    this.Size = file.ContentLength;
        //    this.Created = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;

        //    this.SaveStream2(file.InputStream, gallery, true);
        //}

        ///// <summary>
        ///// Saves the stream.
        ///// </summary>
        ///// <param name="fileStream">The file stream.</param>
        ///// <param name="filename">The filename.</param>
        ///// <param name="gallery">The gallery.</param>
        //public void SaveStream(System.IO.Stream fileStream, string filename, Sushi.Mediakiwi.Data.Gallery gallery)
        //{
        //    this.SaveStream(fileStream, filename, gallery, false);
        //}

        ///// <summary>
        ///// Saves the stream.
        ///// </summary>
        ///// <param name="fileStream">The file stream.</param>
        ///// <param name="filename">The filename.</param>
        ///// <param name="gallery">The gallery.</param>
        ///// <param name="overwriteExistingFile">If set to true, overwrites the file if it allready exists. If set to false, appends a number to the file to create a unique file</param>
        //public void SaveStream(System.IO.Stream fileStream, string filename, Sushi.Mediakiwi.Data.Gallery gallery, bool overwriteExistingFile)
        //{
        //    if (fileStream.Length == 0)
        //        return;

        //    int index = filename.LastIndexOf('.') + 1;

        //    this.Extention = filename.Substring(index, filename.Length - index);
        //    this.FileName = filename;
        //    this.GalleryID = gallery.ID;
        //    //this.Type = file.ContentType;
        //    this.Size = Convert.ToInt32(fileStream.Length);

        //    GuessType();
        //    this.SaveStream2(fileStream, gallery, true, overwriteExistingFile);
        //}

        ///// <summary>
        ///// Saves the stream2.
        ///// </summary>
        ///// <param name="fileStream">The file stream.</param>
        ///// <param name="gallery">The gallery.</param>
        ///// <param name="saveStream">if set to <c>true</c> [save stream].</param>
        //void SaveStream2(System.IO.Stream fileStream, Sushi.Mediakiwi.Data.Gallery gallery, bool saveStream)
        //{
        //    SaveStream2(fileStream, gallery, saveStream, false);
        //}

        ///// <summary>
        ///// Saves the stream.
        ///// </summary>
        ///// <param name="file">The file.</param>
        ///// <param name="gallery">The gallery.</param>
        //public void SaveStream(System.IO.FileInfo file, Sushi.Mediakiwi.Data.Gallery gallery)
        //{
        //    SaveStream(file, gallery, false);
        //}

        ///// <summary>
        ///// Saves the stream.
        ///// </summary>
        ///// <param name="file">The file.</param>
        ///// <param name="gallery">The gallery.</param>
        ///// <param name="saveStream">if set to <c>true</c> [Saves the stream also to Azure].</param>
        ///// <param name="overWriteImage">if set to <c>true</c> [over write image].</param>
        //public void SaveStream(System.IO.FileInfo file, Sushi.Mediakiwi.Data.Gallery gallery, bool saveStream, bool overWriteImage)
        //{
        //    using (System.IO.FileStream filestream = new System.IO.FileStream(file.FullName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
        //    {
        //        if (filestream.Length == 0)
        //            return;

        //        this.Extention = file.Extension.Replace(".", string.Empty);
        //        this.FileName = file.Name;
        //        this.GalleryID = gallery.ID;
        //        this.IsActive = true;

        //        GuessType();

        //        this.Size = (int)file.Length;

        //        this.SaveStream2((System.IO.Stream)filestream, gallery, saveStream, overWriteImage);
        //    }
        //}

        ///// <summary>
        ///// Saves the stream.
        ///// </summary>
        ///// <param name="file">The file.</param>
        ///// <param name="gallery">The gallery.</param>
        ///// <param name="overWriteImage">if set to <c>true</c> [over write image].</param>
        //public void SaveStream(System.IO.FileInfo file, Sushi.Mediakiwi.Data.Gallery gallery, bool overWriteImage)
        //{
        //    SaveStream(file, gallery, false, overWriteImage);
        //}

        ///// <summary>
        ///// Applies the specified hyperlink.
        ///// </summary>
        ///// <param name="hyperlink">The hyperlink.</param>
        //public void Apply(System.Web.UI.WebControls.HyperLink hyperlink)
        //{
        //    Apply(hyperlink, false);
        //}

        ///// <summary>
        ///// Applies the specified hyperlink.
        ///// </summary>
        ///// <param name="hyperlink">The hyperlink.</param>
        ///// <param name="onlySetNavigationUrl">if set to <c>true</c> [only set navigation URL].</param>
        //public void Apply(System.Web.UI.WebControls.HyperLink hyperlink, bool onlySetNavigationUrl)
        //{
        //    hyperlink.NavigateUrl = this.DownloadUrl;
        //    hyperlink.ToolTip = this.Description;

        //    if (onlySetNavigationUrl)
        //        return;

        //    hyperlink.Text = this.Title;
        //}

        ///// <summary>
        ///// Moves the file.
        ///// </summary>
        ///// <param name="galleryID">The gallery ID.</param>
        ///// <returns></returns>
        //public bool MoveFile(int galleryID)
        //{
        //    Gallery gallery = Gallery.SelectOne(galleryID);
        //    string newFileLocation = string.Concat(gallery.LocalFolderPath, "/", this.FileName);

        //    try
        //    {
        //        if (Exists)
        //        {
        //            System.IO.File.Move(this.LocalFilePath, newFileLocation);
        //            this.GalleryID = galleryID;
        //            this.Save();
        //        }

        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //    return true;
        //}


        //private string m_ImageScaledUrl;
        ///// <summary>
        ///// Images the scaled URL.
        ///// </summary>
        ///// <param name="width">The width.</param>
        ///// <param name="height">The height.</param>
        ///// <returns></returns>
        //public string ImageScaledUrl(int width, int height)
        //{
        //    if (m_ImageScaledUrl == null && this.IsImage)
        //    {
        //        var portal = this.DatabaseMappingPortal == null ? null : this.DatabaseMappingPortal.Name;
        //        m_ImageScaledUrl = string.Format(@"<a href=""{0}"" target=""_blank""><img src=""{1}"" width=""{2}"" height=""{3}""></a>", this.FullPath,
        //        Sushi.Mediakiwi.Data.Image.GetUrl(this.ID, ImageFileFormat.FixedBorder, width, height, null, this.CompletePath), width, height);

        //    }
        //    return m_ImageScaledUrl;

        //}


        ///// <summary>
        ///// Applies the path.
        ///// </summary>
        ///// <param name="fileName">Name of the file.</param>
        ///// <param name="appendUrl">if set to <c>true</c> [append URL].</param>
        ///// <param name="checkRegistry">if set to <c>true</c> [check registry].</param>
        ///// <returns></returns>
        //public string ApplyPath(string fileName, bool appendUrl, bool checkRegistry)
        //{
        //    string repository;
        //    if (IsNewStyle)
        //        repository = string.Concat(Wim.CommonConfiguration.RelativeRepositoryUrl, CompletePath);
        //    else
        //    {
        //        if (IsImage)
        //        {
        //            if (BaseGalleryID == 0)
        //                repository = "repository/image";
        //            else
        //            {
        //                if (IsOldStyle)
        //                    repository = "repository/image";
        //                else
        //                    repository = string.Concat("repository/image/", BaseGalleryID);
        //            }
        //        }
        //        else
        //        {
        //            if (BaseGalleryID == 0)
        //                repository = "repository/document";
        //            else
        //            {

        //                if (IsOldStyle)
        //                    repository = "repository/document";
        //                else
        //                    repository = string.Concat("repository/document/", BaseGalleryID);
        //            }
        //        }
        //    }

        //    if (!string.IsNullOrEmpty(repository) && !string.IsNullOrEmpty(fileName) && repository.EndsWith(fileName))
        //        fileName = null;

        //    if (this.GalleryUrlMap != null)
        //    {
        //        if (string.IsNullOrEmpty(GalleryUrlMap.AssetFolder))
        //            return string.Concat(GalleryUrlMap.MappedUrl, string.Format("/{0}/{1}", repository, fileName));

        //        return string.Concat(GalleryUrlMap.MappedUrl, string.Format("/{0}/{1}/{2}", GalleryUrlMap.AssetFolder, CompletePath, fileName));
        //    }

        //    if (string.IsNullOrEmpty(fileName))
        //        return Wim.Utility.AddApplicationPath(string.Concat("/", repository), appendUrl);
        //    return Wim.Utility.AddApplicationPath(string.Format("/{0}/{1}", repository, fileName), appendUrl);
        //}

        //internal static void CloudDelete(Sushi.Mediakiwi.Data.Asset document)
        //{
        //    if (!Sushi.Mediakiwi.Data.Asset.HasCloudSetting) return;
        //    Sushi.Mediakiwi.Data.Asset.m_CloundInstance.Delete(document);
        //}

        ///// <summary>
        ///// Delete an implementation record.
        ///// </summary>
        ///// <returns></returns>
        //public override bool Delete()
        //{
        //    DeleteFile();
        //    CloudDelete(this);

        //    bool isDeleted = base.Delete();


        //    var gallery = (from item in Sushi.Mediakiwi.Data.Gallery.SelectAll() where item.ID == this.GalleryID select item).ToArray();
        //    if (gallery.Count() > 0)
        //    {
        //        gallery[0].UpdateCount();                
        //    }

        //    return isDeleted;
        //}

        //public bool DeleteFile()
        //{
        //    if (System.Web.HttpContext.Current != null)
        //    {
        //        try
        //        {
        //            string filePath = System.Web.HttpContext.Current.Server.MapPath(this.RelativePath);
        //            if (System.IO.File.Exists(filePath))
        //            {
        //                System.IO.File.Delete(filePath);
        //                return true;
        //            }
        //        }
        //        catch (Exception ) { }
        //    }
        //    return false;
        //}

        #endregion MOVED to EXTENSION / LOGIC

    }
}