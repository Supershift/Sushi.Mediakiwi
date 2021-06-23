using Sushi.Mediakiwi.Data.MicroORM;
using Sushi.MicroORM.Mapping;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    ///
    /// </summary>
    [DataMap(typeof(PortalRightMap))]
    public class PortalRight : IPortalRight
    {
        public class PortalRightMap : DataMap<PortalRight>
        {
            public PortalRightMap()
            {
                Table("wim_PortalRights");
                Id(x => x.ID, "PortalRight_Key").Identity();
                Map(x => x.RoleID, "PortalRight_Role_Key");
                Map(x => x.PortalID, "PortalRight_Portal_Key");
            }
        }

        #region Properties

        public PortalRight()
        {
        }

        /// <summary>
        /// The Identifier for this Portal Right
        /// </summary>
        public virtual int ID { get; set; }

        /// <summary>
        /// The Role Identifier for this Portal Right
        /// </summary>
        public virtual int RoleID { get; set; }

        /// <summary>
        /// The Portal Identifier for this Portal Right
        /// </summary>
        public virtual int PortalID { get; set; }

        #endregion Properties

        /// <summary>
        /// Selects one Portal Right from DB based on the supplied ID
        /// </summary>
        /// <param name="ID">The identifier to fetch a Portal Right for</param>
        /// <returns></returns>
        public static IPortalRight SelectOne(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<PortalRight>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.ID, ID);
            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Selects one Portal Right from DB based on the supplied ID Async
        /// </summary>
        /// <param name="ID">The identifier to fetch a Portal Right for</param>
        /// <returns></returns>
        public static async Task<IPortalRight> SelectOneAsync(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<PortalRight>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.ID, ID);
            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Selects all Portal Rights from DB based on the Role ID
        /// </summary>
        /// <param name="roleID">The Role identifier to fetch all Portal Rights for</param>
        /// <returns></returns>
        public static IPortalRight[] SelectAll(int roleID)
        {
            var connector = ConnectorFactory.CreateConnector<PortalRight>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.RoleID, roleID);
            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all Portal Rights from DB based on the Role ID Async
        /// </summary>
        /// <param name="roleID">The Role identifier to fetch all Portal Rights for</param>
        /// <returns></returns>
        public static async Task<IPortalRight[]> SelectAllAsync(int roleID)
        {
            var connector = ConnectorFactory.CreateConnector<PortalRight>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.RoleID, roleID);
            var result = await connector.FetchAllAsync(filter);

            return result.ToArray();
        }
    }
}