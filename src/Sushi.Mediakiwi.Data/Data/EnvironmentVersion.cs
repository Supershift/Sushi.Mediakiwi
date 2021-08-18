using Sushi.Mediakiwi.Data.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    [DataMap(typeof(EnvironmentVersionMap))]
    public class EnvironmentVersion : IEnvironmentVersion
    {
        public class EnvironmentVersionMap : DataMap<EnvironmentVersion>
        {
            public EnvironmentVersionMap()
            {
                Table("wim_Environments");
                Id(x => x.ID, "Environment_Key").Identity();
                Map(x => x.Updated, "Environment_Update");
                Map(x => x.Version, "Environment_Version");
            }
        }

        #region Properties

        internal static DateTime? m_serverEnvironmentVersion;

        public virtual DateTime ServerEnvironmentVersion
        {
            get
            {
                return m_serverEnvironmentVersion.GetValueOrDefault();
            }
            set
            {
                m_serverEnvironmentVersion = value;
            }
        }

        /// <summary>
        /// The primary key
        /// </summary>
        public virtual int ID { get; set; }

        public virtual DateTime? Updated { get; set; }

        public virtual decimal Version { get; set; }

        #endregion Properties

        #region Methods

        public static IEnvironmentVersion Select()
        {
            var connector = ConnectorFactory.CreateConnector<EnvironmentVersion>();
            var filter = connector.CreateDataFilter();
            return connector.FetchSingle(filter);
        }

        public static async Task<IEnvironmentVersion> SelectAsync()
        {
            var connector = ConnectorFactory.CreateConnector<EnvironmentVersion>();
            var filter = connector.CreateDataFilter();
            return await connector.FetchSingleAsync(filter);
        }

        public static bool SetUpdated()
        {
            var connector = ConnectorFactory.CreateConnector<EnvironmentVersion>();
            var sql = @"UPDATE wim_Environments SET Environment_Update = @update";
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@update", DateTime.UtcNow);
            connector.ExecuteNonQuery(sql, filter);
            return true;
        }

        public static async Task<bool> SetUpdatedAsync()
        {
            var connector = ConnectorFactory.CreateConnector<EnvironmentVersion>();
            var sql = @"UPDATE wim_Environments SET Environment_Update = @update";
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@update", DateTime.UtcNow);
            await connector.ExecuteNonQueryAsync(sql, filter).ConfigureAwait(false);
            return true;
        }

        #endregion Methods
    }
}