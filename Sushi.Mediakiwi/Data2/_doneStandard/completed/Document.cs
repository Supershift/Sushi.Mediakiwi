using System;
using System.Web;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Sushi.Mediakiwi.Data.DalReflection;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class Document : Asset
    {
        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static new Document SelectOne(int ID)
        {
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            //whereClause.Add(new DatabaseDataValueColumn("Asset_IsImage", SqlDbType.Bit, false));

            whereClause.Add(new DatabaseDataValueColumn("Asset_Key", SqlDbType.Int, ID));
            //  Legacy
            whereClause.Add(new DatabaseDataValueColumn("Asset_Migration_Key", SqlDbType.Int, ID, DatabaseDataValueConnectType.Or));

            return (Document)new Document()._SelectOne(whereClause);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="GUID">The GUID.</param>
        /// <returns></returns>
        public static new Document SelectOne(Guid GUID)
        {
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();

            whereClause.Add(new DatabaseDataValueColumn("Asset_GUID", SqlDbType.UniqueIdentifier, GUID));
            //  Legacy
            //whereClause.Add(new DatabaseDataValueColumn("Asset_Migration_Key", SqlDbType.Int, ID, DatabaseDataValueConnectType.Or));

            return (Document)new Document()._SelectOne(whereClause);
        }

        /// <summary>
        /// Selects the all_by parent.
        /// </summary>
        /// <param name="galleryID">The gallery ID.</param>
        /// <returns></returns>
        public static new Document[] SelectAll(int galleryID)
        {
            List<Document> list = new List<Document>();
            Document implement = new Document();

            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();

            whereClause.Add(new DatabaseDataValueColumn("Asset_IsImage", SqlDbType.Bit, false));
            whereClause.Add(new DatabaseDataValueColumn("Asset_Gallery_Key", SqlDbType.Int, galleryID));
            whereClause.Add(new DatabaseDataValueColumn("Asset_Asset_Key", SqlDbType.Int, null));

            foreach (object o in implement._SelectAll(whereClause))
                list.Add((Document)o);
            return list.ToArray();
        }

        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard
    }
}