using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Linq;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.MircoORM;

namespace Sushi.Mediakiwi.Data.Statistics
{
    /// <summary>
    /// 
    /// </summary>
    [DataMap(typeof(VisitorPageMap))]
    public class VisitorPage
    {
        public class VisitorPageMap : DataMap<VisitorPage>
        {
            public VisitorPageMap()
            {
                Table("wim_VisitorPages");
                Id(x => x.ID, "VisitorPage_Key").Identity();
                Map(x => x.UrlID, "VisitorPage_Url_Key");
                Map(x => x.Name, "VisitorPage_Name").Length(255);
                Map(x => x.GUID, "VisitorPage_PageGuid");
                Map(x => x.Created, "VisitorPage_Created");                
            }
        }

        #region Properties
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the profile ID.
        /// </summary>
        /// <value>The profile ID.</value>
        public int UrlID { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the GUID.
        /// </summary>
        /// <value>The GUID.</value>
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
        #endregion Properties

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="urlID">The URL ID.</param>
        /// <param name="guid">The GUID.</param>
        /// <param name="absolutePath">The absolute path.</param>
        /// <returns></returns>
        public static VisitorPage SelectOne(int urlID, Guid guid, string absolutePath)
        {
            var connector = ConnectorFactory.CreateConnector<VisitorPage>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.Name, absolutePath);

            if (guid == Guid.Empty)
                filter.Add(x => x.GUID, null);
            else
                filter.Add(x => x.GUID, guid);

            filter.Add(x => x.UrlID, urlID);

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="urlID">The URL ID.</param>
        /// <param name="guid">The GUID.</param>
        /// <param name="absolutePath">The absolute path.</param>
        /// <returns></returns>
        public static async Task<VisitorPage> SelectOneAsync(int urlID, Guid guid, string absolutePath)
        {
            var connector = ConnectorFactory.CreateConnector<VisitorPage>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.Name, absolutePath);

            if (guid == Guid.Empty)
                filter.Add(x => x.GUID, null);
            else
                filter.Add(x => x.GUID, guid);

            filter.Add(x => x.UrlID, urlID);

            return await connector.FetchSingleAsync(filter);
        }


        /// <summary>
        /// Selects the keys.
        /// </summary>
        /// <param name="domain">The domain.</param>
        /// <param name="absolutePath">The absolute path.</param>
        /// <returns></returns>
        public static int[] SelectKeys(int domain, string absolutePath)
        {
            var connector = ConnectorFactory.CreateConnector<VisitorPage>();
            var filter = connector.CreateDataFilter();
            filter.AddSql("NOT EXISTS(SELECT * FROM wim_VisitorUrls WHERE VisitorUrl_Key = VisitorPage_Url_Key AND VisitorUrl_SiteGuid IS NOT NULL AND VisitorUrl_Name != 'localhost')");

            if (!string.IsNullOrEmpty(absolutePath))
            {
                absolutePath = string.Format("%{0}%", absolutePath);
                filter.Add(x => x.Name, absolutePath, ComparisonOperator.Like);
            }
            if (domain > 0)
            {
                filter.Add(x => x.UrlID, domain);
            }
            return connector.FetchAll(filter).Select(x => x.ID).ToArray();
        }


        /// <summary>
        /// Selects the keys.
        /// </summary>
        /// <param name="domain">The domain.</param>
        /// <param name="absolutePath">The absolute path.</param>
        /// <returns></returns>
        public static async Task<int[]> SelectKeysAsync(int domain, string absolutePath)
        {
            var connector = ConnectorFactory.CreateConnector<VisitorPage>();
            var filter = connector.CreateDataFilter();
            filter.AddSql("NOT EXISTS(SELECT * FROM wim_VisitorUrls WHERE VisitorUrl_Key = VisitorPage_Url_Key AND VisitorUrl_SiteGuid IS NOT NULL AND VisitorUrl_Name != 'localhost')");

            if (!string.IsNullOrEmpty(absolutePath))
            {
                absolutePath = string.Format("%{0}%", absolutePath);
                filter.Add(x => x.Name, absolutePath, ComparisonOperator.Like);
            }
            if (domain > 0)
            {
                filter.Add(x => x.UrlID, domain);
            }

            var result = await connector.FetchAllAsync(filter);
            return result.Select(x => x.ID).ToArray();
        }
    }
}
