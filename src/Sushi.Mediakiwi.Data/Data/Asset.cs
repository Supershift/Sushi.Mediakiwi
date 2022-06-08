using Sushi.Mediakiwi.Data.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    [DataMap(typeof(AssetMap))]
    public class Asset
    {
        public Asset()
        {
            this.IsNewStyle = true;
            this.IsActive = true;
        }

        public class AssetMap : DataMap<Asset>
        {
            public AssetMap() : this(false) { }            

            public AssetMap(bool isSave) 
            {
                /* TODO MJ 2019-01-03: Add this sort order, when calling data, like SelectAll()
                Order = "Asset_SortOrder asc, Asset_Title asc")
                */
                if (isSave)
                    Table("wim_Assets");
                else
                    Table("wim_Assets left join wim_GalleryView on Gallery_Key = Asset_Gallery_Key left join wim_AssetTypes on AssetType_Key = Asset_AssetType_Key");

                Id(x => x.ID, "Asset_Key").Identity().SqlType(SqlDbType.Int);
                Map(x => x.GUID, "Asset_GUID").SqlType(SqlDbType.UniqueIdentifier);
                Map(x => x.GalleryID, "Asset_Gallery_Key").SqlType(SqlDbType.Int);
                Map(x => x.Title, "Asset_Title").SqlType(SqlDbType.NVarChar).Length(255);
                Map(x => x.FileName, "Asset_Filename").SqlType(SqlDbType.NVarChar).Length(255);
                Map(x => x.BaseGalleryID, "Gallery_Base_Key").SqlType(SqlDbType.Int).ReadOnly();
                Map(x => x.CompletePath, "Gallery_CompletePath").SqlType(SqlDbType.NVarChar).Length(1000).ReadOnly();
                Map(x => x.Extension, "Asset_Extention").SqlType(SqlDbType.VarChar).Length(20);
                Map(x => x.Size, "Asset_Size").SqlType(SqlDbType.Int);
                Map(x => x.Type, "Asset_Type").SqlType(SqlDbType.VarChar).Length(150);
                Map(x => x.AssetTypeID, "Asset_AssetType_Key").SqlType(SqlDbType.Int);
                Map(x => x.ParentID, "Asset_Asset_Key").SqlType(SqlDbType.Int);
                Map(x => x.Description, "Asset_Description").SqlType(SqlDbType.NText);

                Map(x => x.Created, "Asset_Created").SqlType(SqlDbType.DateTime);
                Map(x => x.IsOldStyle, "Asset_OldStyle").SqlType(SqlDbType.Bit);
                Map(x => x.IsNewStyle, "Asset_NewStyle").SqlType(SqlDbType.Bit);
                Map(x => x.Height, "Asset_Height").SqlType(SqlDbType.Int);
                Map(x => x.SortOrder, "Asset_SortOrder").SqlType(SqlDbType.Int);
                Map(x => x.Width, "Asset_Width").SqlType(SqlDbType.Int);
                Map(x => x.DataString, "Asset_Data").SqlType(SqlDbType.Xml);
                Map(x => x.IsArchived, "Asset_IsArchived");
                Map(x => x.IsImage, "Asset_IsImage").SqlType(SqlDbType.Bit);
                Map(x => x.IsActive, "Asset_IsActive").SqlType(SqlDbType.Bit);
                Map(x => x.Updated, "Asset_Updated").SqlType(SqlDbType.DateTime);


                Map(x => x.RemoteLocation, "Asset_RemoteLocation").Length(512);
                Map(x => x.RemoteLocation_Thumb, "Asset_RemoteLocation_Thumb").Length(512);
                Map(x => x.RemoteDownload, "Asset_RemoteDownload");

                Map(x => x.AssetType, "AssetType_Name").ReadOnly();
                Map(x => x.AssetTypeTag, "AssetType_Tag").ReadOnly();
            }
        }

        #region properties
        public bool Exists { get; set; } = true;
        public string Path { get; set; }
        public string DownloadUrl { get; set; }
        public string DownloadFullUrl { get; set; }
        public string LocalFilePath { get; set; }


        /// <summary>
        /// Gets or sets the URL at which the <see cref="Asset"/> is stored.
        /// </summary>
        public string RemoteLocation { get; set; }
        
        public string RemoteLocation_Thumb { get; set; }

        public bool RemoteDownload { get; set; }
        
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public int ID { get; set; }

        private Guid m_GUID;

        /// <summary>
        /// Gets or sets the GUID.
        /// </summary>
        /// <value>The GUID.</value>
        public Guid GUID
        {
            get
            {
                if (this.m_GUID == Guid.Empty)
                    this.m_GUID = Guid.NewGuid();
                return m_GUID;
            }
            set { m_GUID = value; }
        }

        /// <summary>
        /// Gets or sets the gallery ID.
        /// </summary>
        /// <value>The gallery ID.</value>
        public int GalleryID { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }

        private string m_FileName;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
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

        /// <summary>
        /// Gets or sets the base gallery ID.
        /// </summary>
        /// <value>The base gallery ID.</value>
        public int BaseGalleryID { get; set; }

        /// <summary>
        /// Gets or sets the complete path.
        /// </summary>
        /// <value>The complete path.</value>
        public string CompletePath { get; set; }

        /// <summary>
        /// Gets or sets the extension.
        /// </summary>
        /// <value>The extension.</value>
        public string Extension { get; set; }

        public string ExtensionClassName
        {
            get
            {
                if (!string.IsNullOrEmpty(Extension))
                {
                    switch (Extension.ToLower())
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

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public long Size { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the asset type ID.
        /// </summary>
        /// <value>The asset type ID.</value>
        public int? AssetTypeID { get; set; }

        public int? ParentID { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }        

        /// <summary>
        /// Gets the name of the AssetType.
        /// </summary>
        public string AssetType { get; set; }     

        /// <summary>
        /// Gets the Tag of the AssetType.
        /// </summary>
        public string AssetTypeTag { get; set; }     

        private DateTime m_Created;

        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created
        {
            get
            {
                if (this.m_Created == DateTime.MinValue)
                    this.m_Created = Common.DatabaseDateTime;
                return m_Created;
            }
            set { m_Created = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is old style.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is old style; otherwise, <c>false</c>.
        /// </value>
        public bool IsOldStyle { get; set; }

        /// <summary>
        /// Is this asset archived ?
        /// </summary>
        public bool IsArchived { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [new style].
        /// </summary>
        /// <value><c>true</c> if [new style]; otherwise, <c>false</c>.</value>
        public bool IsNewStyle { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public int Height { get; set; }

        public int? SortOrder { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public int Width { get; set; }

        /// <summary>
        /// Represents the serialized XML value of the CustomData object
        /// </summary>
        public string DataString { get; set; }

        private CustomData m_Data;

        /// <summary>
        /// Holds all customData properties
        /// </summary>
        public CustomData Data
        {
            get
            {
                if (m_Data == null)
                    m_Data = new CustomData(DataString);

                return m_Data;
            }
            set
            {
                m_Data = value;
                DataString = m_Data.Serialized;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is old style.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is old style; otherwise, <c>false</c>.
        /// </value>
        public bool IsImage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool IsActive { get; set; }

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
                    Utility.ReflectProperty(this, m_Image);
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
                if (m_Document == null)
                {
                    m_Document = new Document();
                    Utility.ReflectProperty(this, m_Document);
                }
                return m_Document;
            }
        }

        #region iExportable Members

        public DateTime? Updated { get; set; }

        #endregion iExportable Members

        #endregion properties

        public static List<Asset> SelectAll_Local()
        {
            var connector = ConnectorFactory.CreateConnector<Asset>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.RemoteLocation, null);
            filter.Add(x => x.ParentID, null);

            return connector.FetchAll(filter);
        }
        
        public static List<Asset> SelectAll_Variant(int parentID, string relativeGalleryPath = null)
        {
            var connector = ConnectorFactory.CreateConnector<Asset>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.ParentID, parentID);

            return connector.FetchAll(filter);
        }

        public static async Task<List<Asset>> SelectAll_VariantAsync(int parentID, string relativeGalleryPath = null)
        {
            var connector = ConnectorFactory.CreateConnector<Asset>();
            var filter = connector.CreateQuery();
            if (parentID > 0)
            {
                filter.Add(x => x.ParentID, parentID);
            }

            return await connector.FetchAllAsync(filter).ConfigureAwait(false);
        }


        public static List<Asset> SelectRange(int[] items)
        {
            var connector = ConnectorFactory.CreateConnector<Asset>();
            var filter = connector.CreateQuery();

            int i = 0;
            var sql_in = new List<string>();
            foreach (var item in items)
            {
                i++;
                filter.AddParameter($"p{i}", SqlDbType.Int, item);
                sql_in.Add($"@p{i}");
            }

            return connector.FetchAll(@"
select 
    *
from 
    wim_Assets
where
    Asset_Key in (" + string.Join(",", sql_in) + @")
", filter);

        }

        public static async Task<List<Asset>> SelectRangeAsync(int[] items)
        {
            var connector = ConnectorFactory.CreateConnector<Asset>();
            var filter = connector.CreateQuery();

            int i = 0;
            var sql_in = new List<string>();
            foreach (var item in items)
            {
                i++;
                filter.AddParameter($"p{i}", SqlDbType.Int, item);
                sql_in.Add($"@p{i}");
            }

            return await connector.FetchAllAsync(@"
select 
    *
from 
    wim_Assets
where
    Asset_Key in (" + string.Join(",", sql_in) + @")
", filter);

        }

        public static List<Asset> SelectAll(int galleryID, int? assetTypeID = null, bool onlyReturnActiveAssets = false, bool onlyReturnDocuments = false, bool onlyReturnImages = false)
        {
            var connector = ConnectorFactory.CreateConnector<Asset>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.GalleryID, galleryID);
            filter.Add(x => x.ParentID, null);

            if (onlyReturnImages)
            {
                filter.Add(x => x.IsImage, true);
            }
            else if (onlyReturnDocuments)
            {
                filter.Add(x => x.IsImage, false);
            }

            if (onlyReturnActiveAssets)
            {
                filter.Add(x => x.IsActive, true);
            }

            if (assetTypeID.HasValue)
            {
                filter.Add(x => x.AssetTypeID, assetTypeID.Value);
            }

            return connector.FetchAll(filter);
        }

        public static async Task<List<Asset>> SelectAllAsync(bool onlyReturnImages = false)
        {
            var connector = ConnectorFactory.CreateConnector<Asset>();
            var filter = connector.CreateQuery();
            if (onlyReturnImages)
            {
                filter.Add(x => x.IsImage, true);
            }
            return await connector.FetchAllAsync(filter);
        }

        public static async Task<List<Asset>> SelectAllAsync(int galleryID, int? assetTypeID = null, bool onlyReturnActiveAssets = false, bool onlyReturnDocuments = false, bool onlyReturnImages = false)
        {
            var connector = ConnectorFactory.CreateConnector<Asset>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.GalleryID, galleryID);
            filter.Add(x => x.ParentID, null);
            
            if (onlyReturnImages)
            {
                filter.Add(x => x.IsImage, true);
            }
            else if (onlyReturnDocuments)
            {
                filter.Add(x => x.IsImage, false);
            }

            if (onlyReturnActiveAssets)
            {
                filter.Add(x => x.IsActive, true);
            }

            if (assetTypeID.HasValue)
            {
                filter.Add(x => x.AssetTypeID, assetTypeID.Value);
            }

            return await connector.FetchAllAsync(filter);
        }

        public static List<Asset> SearchAll(string searchCandidate, int? galleryID = null, bool onlyReturnActiveAssets = false)
        {
            var connector = ConnectorFactory.CreateConnector<Asset>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.ParentID, null);

            if (!string.IsNullOrWhiteSpace(searchCandidate))
            {
                searchCandidate = string.Concat("%", searchCandidate.Trim().Replace(" ", "%"), "%");
                filter.AddParameter("search", searchCandidate);
            }

            filter.AddSql("(Asset_Title like @search or Asset_Description like @search)");

            if (onlyReturnActiveAssets)
                filter.Add(x => x.IsActive, true);

            if (galleryID.HasValue)
                filter.Add(x => x.GalleryID, galleryID.Value);

            return connector.FetchAll(filter);
        }

        public static async Task<List<Asset>> SearchAllAsync(string searchCandidate, int? galleryID = null, bool onlyReturnActiveAssets = false, bool onlyReturnDocuments = false, bool onlyReturnImages = false)
        {
            var connector = ConnectorFactory.CreateConnector<Asset>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.ParentID, null);

            if (!string.IsNullOrWhiteSpace(searchCandidate))
            {
                searchCandidate = string.Concat("%", searchCandidate.Trim().Replace(" ", "%"), "%");
                filter.AddParameter("search", searchCandidate);
            }

            if (onlyReturnImages)
            {
                filter.Add(x => x.IsImage, true);
            }
            else if (onlyReturnDocuments)
            {
                filter.Add(x => x.IsImage, false);
            }

            filter.AddSql("(Asset_Title like @search or Asset_Description like @search)");

            if (onlyReturnActiveAssets)
            {
                filter.Add(x => x.IsActive, true);
            }

            if (galleryID.HasValue)
            {
                filter.Add(x => x.GalleryID, galleryID.Value);
            }

            return await connector.FetchAllAsync(filter);
        }

        public static async Task<List<Asset>> SelectAll_LocalAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Asset>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.RemoteLocation, null);
            filter.Add(x => x.ParentID, null);

            return await connector.FetchAllAsync(filter).ConfigureAwait(false);
        }

        public static Asset SelectOne(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<Asset>();
            return connector.FetchSingle(ID);
        }

        public static async Task<Asset> SelectOneAsync(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<Asset>();
            return await connector.FetchSingleAsync(ID).ConfigureAwait(false);
        }

        public static Asset SelectOne(Guid guid)
        {
            var connector = ConnectorFactory.CreateConnector<Asset>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.GUID, guid);
            return connector.FetchSingle(filter);
        }

        public static async Task<Asset> SelectOneAsync(Guid guid)
        {
            var connector = ConnectorFactory.CreateConnector<Asset>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.GUID, guid);
            return await connector.FetchSingleAsync(filter);
        }

        public static Asset SelectOne(int galleryID, int assetTypeID)
        {
            var connector = ConnectorFactory.CreateConnector<Asset>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.AssetTypeID, assetTypeID);
            filter.Add(x => x.GalleryID, galleryID);
            
            return connector.FetchSingle(filter);
        }

        public static async Task<Asset> SelectOneAsync(int galleryID, int assetTypeID)
        {
            var connector = ConnectorFactory.CreateConnector<Asset>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.AssetTypeID, assetTypeID);
            filter.Add(x => x.GalleryID, galleryID);

            return await connector.FetchSingleAsync(filter).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the inactive.
        /// </summary>
        internal static bool DeleteInactive()
        {
            var connector = ConnectorFactory.CreateConnector<Asset>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.IsActive, false);
            connector.Delete(filter);
            return true;
        }

        public bool Save()
        {
            Updated = Common.DatabaseDateTime;

            var connector = ConnectorFactory.CreateConnector(new AssetMap(true));

            connector.Save(this);
            if (!SortOrder.HasValue)
            {
                SortOrder = ID;

                connector.Save(this);
            }

            return true;
        }

        public void Delete()
        {
            var connector = ConnectorFactory.CreateConnector(new AssetMap(true));
            connector.Delete(this);
        }


        public async Task DeleteAsync()
        {
            var connector = ConnectorFactory.CreateConnector(new AssetMap(true));
            await connector.DeleteAsync(this).ConfigureAwait(false);
        }

        public async Task<bool> SaveAsync()
        {
            Updated = Common.DatabaseDateTime;

            var connector = ConnectorFactory.CreateConnector(new AssetMap(true));

            await connector.SaveAsync(this).ConfigureAwait(false);
            if (!SortOrder.HasValue)
            {
                SortOrder = ID;

                await connector.SaveAsync(this).ConfigureAwait(false);
            }

            return true;
        }
    }
}