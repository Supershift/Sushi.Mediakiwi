using Sushi.MicroORM.Mapping;
using System;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.MicroORM;

namespace Sushi.Mediakiwi.Data.Statistics
{
    /// <summary>
    /// 
    /// </summary>
    [DataMap(typeof(VisitorDownloadMap))]
    public class VisitorDownload 
    {
        public class VisitorDownloadMap : DataMap<VisitorDownload>
        {
            public VisitorDownloadMap()
            {
                Table("wim_VisitorDownloads");
                Id(x => x.ID, "VisitorDownload_Key").Identity();
                Map(x => x.VisitorLogID, "VisitorDownload_VisitorLog_Key");
                Map(x => x.AssetID, "VisitorDownload_Asset_Key");
                Map(x => x.Created, "VisitorDownload_Created");
            }
        }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public int ID { get; set; }


        /// <summary>
        /// Gets or sets the visitor log ID.
        /// </summary>
        /// <value>The visitor log ID.</value>
        public int VisitorLogID { get; set; }

        /// <summary>
        /// Gets or sets the profile ID.
        /// </summary>
        /// <value>The profile ID.</value>
        public int AssetID { get; set; }

        DateTime m_Created;
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created
        {
            get
            {
                if (m_Created == DateTime.MinValue) 
                    m_Created = Common.DatabaseDateTime;
                return m_Created;
            }
            set { m_Created = value; }
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static VisitorDownload SelectOne(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<VisitorDownload>();
            return connector.FetchSingle(ID);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static async Task<VisitorDownload> SelectOneAsync(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<VisitorDownload>();
            return await connector.FetchSingleAsync(ID);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public static VisitorDownload[] SelectAll()
        {
            var connector = ConnectorFactory.CreateConnector<VisitorDownload>();
            var filter = connector.CreateDataFilter();
            return connector.FetchAll(filter).ToArray();
        }


        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public static async Task<VisitorDownload[]> SelectAllAsync()
        {
            var connector = ConnectorFactory.CreateConnector<VisitorDownload>();
            var filter = connector.CreateDataFilter();

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

    }
}
