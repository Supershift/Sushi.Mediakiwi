using Sushi.MicroORM.Mapping;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.MicroORM;

namespace Sushi.Mediakiwi.Data.Statistics
{
    /// <summary>
    /// 
    /// </summary>
    [DataMap(typeof(VisitorUrlMap))]
    public class VisitorUrl 
    {
        public class VisitorUrlMap : DataMap<VisitorUrl>
        {
            public VisitorUrlMap()
            {
                Table("wim_VisitorUrls");
                Id(x => x.ID, "VisitorUrl_Key").Identity();
                Map(x => x.Name, "VisitorUrl_Name").Length(50);
                Map(x => x.GUID, "VisitorUrl_SiteGuid");
                Map(x => x.Created, "VisitorUrl_Created");
            }
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <param name="host">The host.</param>
        /// <returns></returns>
        public static VisitorUrl SelectOne(Guid guid, string host)
        {
            var connector = ConnectorFactory.CreateConnector<VisitorUrl>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.Name, host);

            if (guid == Guid.Empty)
                filter.Add(x => x.GUID, null);
            else
                filter.Add(x => x.GUID, guid);

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <param name="host">The host.</param>
        /// <returns></returns>
        public static async Task<VisitorUrl> SelectOneAsync(Guid guid, string host)
        {
            var connector = ConnectorFactory.CreateConnector<VisitorUrl>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.Name, host);

            if (guid == Guid.Empty)
                filter.Add(x => x.GUID, null);
            else
                filter.Add(x => x.GUID, guid);

            return await connector.FetchSingleAsync(filter);
        }

        public static List<VisitorUrl> SelectAll()
        {
            var connector = ConnectorFactory.CreateConnector<VisitorUrl>();
            var filter = connector.CreateDataFilter();
            return connector.FetchAll(filter);
        }

        public static async Task<List<VisitorUrl>> SelectAllAsync()
        {
            var connector = ConnectorFactory.CreateConnector<VisitorUrl>();
            var filter = connector.CreateDataFilter();
            return await connector.FetchAllAsync(filter);
        }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public int ID { get; set; }

        public string Name { get; set; }

        public Guid GUID { get; set; }

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
    }
}
