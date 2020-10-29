using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.MicroORM;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Represents a Image entity.
    /// </summary>
    [DataMap(typeof(ImageMap))]
    public class Image : Asset
    {
        public class ImageMap : DataMap<Image>
        {
            public ImageMap() : this(false) { }

            public ImageMap(bool isSave)
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
                Map(x => x.Extention, "Asset_Extention").SqlType(SqlDbType.VarChar).Length(20);
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

        /// <summary>
        /// Gets or sets the URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        /// <value>The format.</value>
        public string Format { get; set; }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static new Image SelectOne(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<Image>();

            return connector.FetchSingle(ID);
        }

        /// <summary>
        /// Selects the one Async.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static new async Task<Image> SelectOneAsync(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<Image>();

            return await connector.FetchSingleAsync(ID);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static Image SelectOne(Guid guid)
        {
            var connector = ConnectorFactory.CreateConnector<Image>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.GUID, guid);

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static async Task<Image> SelectOneAsync(Guid guid)
        {
            var connector = ConnectorFactory.CreateConnector<Image>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.GUID, guid);

            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Selects the all_by parent.
        /// </summary>
        /// <param name="galleryID">The gallery ID.</param>
        /// <returns></returns>
        public static Image[] SelectAll(int galleryID)
        {
            return SelectAll(galleryID, null);
        }

        /// <summary>
        /// Selects the all_by parent.
        /// </summary>
        /// <param name="galleryID">The gallery ID.</param>
        /// <returns></returns>
        public static async Task<Image[]> SelectAllAsync(int galleryID)
        {
            return await SelectAllAsync(galleryID, null);
        }

        /// <summary>
        /// Selects the all_by parent.
        /// </summary>
        /// <param name="galleryID">The gallery ID.</param>
        /// <returns></returns>
        public static Image[] SelectAll(int galleryID, int? assetTypeID)
        {
            var connector = ConnectorFactory.CreateConnector<Image>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.IsImage, true);
            filter.Add(x => x.IsActive, true);
            filter.Add(x => x.IsArchived, false);
            filter.Add(x => x.GalleryID, galleryID);
            if (assetTypeID.HasValue)
                filter.Add(x => x.AssetTypeID, assetTypeID.Value);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects the all_by parent.
        /// </summary>
        /// <param name="galleryID">The gallery ID.</param>
        /// <returns></returns>
        public static async Task<Image[]> SelectAllAsync(int galleryID, int? assetTypeID)
        {
            var connector = ConnectorFactory.CreateConnector<Image>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.IsImage, true);
            filter.Add(x => x.IsActive, true);
            filter.Add(x => x.IsArchived, false);
            filter.Add(x => x.GalleryID, galleryID);
            if (assetTypeID.HasValue)
                filter.Add(x => x.AssetTypeID, assetTypeID.Value);

            var result = await connector.FetchAllAsync(filter);
           return result.ToArray();
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        internal static Image[] SelectAll()
        {
            var connector = ConnectorFactory.CreateConnector<Image>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.IsImage, true);
            filter.Add(x => x.IsArchived, false);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        internal static async Task<Image[]> SelectAllAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Image>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.IsImage, true);
            filter.Add(x => x.IsArchived, false);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        public static List<Image> SelectRange(List<int> IDs)
        {
            var connector = ConnectorFactory.CreateConnector<Image>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.IsImage, true);
            filter.Add(x => x.IsArchived, false);
            filter.Add(x => x.ID, IDs, ComparisonOperator.In);

            return connector.FetchAll(filter);
        }

        public static async Task<List<Image>> SelectRangeAsync(List<int> IDs)
        {
            var connector = ConnectorFactory.CreateConnector<Image>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.ID, IDs, ComparisonOperator.In);
            filter.Add(x => x.IsImage, true);
            filter.Add(x => x.IsArchived, false);

            return await connector.FetchAllAsync(filter);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="imageArr">The image arr.</param>
        /// <returns></returns>
        [Obsolete("Use SelectRange for this")]
        public static Image[] SelectAll(int[] IDs)
        {
            return SelectRange(IDs.ToList()).ToArray();
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="imageArr">The image arr.</param>
        /// <returns></returns>
        [Obsolete("Use SelectRangeAsync for this")]
        public static async Task<Image[]> SelectAllAsync(int[] IDs)
        {
            var result = await SelectRangeAsync(IDs.ToList());
            return result.ToArray();
        }
    }
}