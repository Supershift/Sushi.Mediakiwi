using Sushi.Mediakiwi.Data.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    ///
    /// </summary>
    [DataMap(typeof(DocumentMap))]
    public class Document : Asset
    {
        public class DocumentMap : DataMap<Document>
        {
            public DocumentMap() : this(false) { }

            public DocumentMap(bool isSave)
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
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static new Document SelectOne(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<Document>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.ID, ID);
            var result = connector.FetchSingle(filter);

            return result;
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="GUID">The GUID.</param>
        /// <returns></returns>
        public static Document SelectOne(Guid GUID)
        {
            var connector = ConnectorFactory.CreateConnector<Document>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.GUID, GUID);

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Selects the one Async.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static async new Task<Document> SelectOneAsync(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<Document>();
            return await connector.FetchSingleAsync(ID);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="GUID">The GUID.</param>
        /// <returns></returns>
        public static async Task<Document> SelectOneAsync(Guid GUID)
        {
            var connector = ConnectorFactory.CreateConnector<Document>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.GUID, GUID);

            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Selects the all_by parent.
        /// </summary>
        /// <param name="galleryID">The gallery ID.</param>
        /// <returns></returns>
        public static Document[] SelectAll(int galleryID)
        {
            var connector = ConnectorFactory.CreateConnector<Document>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.IsImage, false);
            filter.Add(x => x.ParentID, null);
            filter.Add(x => x.GalleryID, galleryID);

            return connector.FetchAll(filter).ToArray();
        }


        /// <summary>
        /// Selects the all_by parent.
        /// </summary>
        /// <param name="galleryID">The gallery ID.</param>
        /// <returns></returns>
        public static async Task<Document[]> SelectAllAsync(int galleryID)
        {
            var connector = ConnectorFactory.CreateConnector<Document>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.IsImage, false);
            filter.Add(x => x.ParentID, null);
            filter.Add(x => x.GalleryID, galleryID);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }
    }
}