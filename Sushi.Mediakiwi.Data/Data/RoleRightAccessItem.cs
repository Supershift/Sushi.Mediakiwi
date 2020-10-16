using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.MircoORM;
using Sushi.Mediakiwi.Data.Interfaces;

namespace Sushi.Mediakiwi.Data
{
    [DataMap(typeof(RoleRightAccessItemMap))]
    public class RoleRightAccessItem : IRoleRightAccessItem
    {
        public class RoleRightAccessItemMap : DataMap<RoleRightAccessItem>
        {
            public RoleRightAccessItemMap()
            {
                Table("wim_RoleRights");

                Id(x => x.ID, "RoleRight_Child_Key");
                Map(x => x.TypeID, "RoleRight_Type");
                Map(x => x.ChildTypeID, "RoleRight_Child_Type");
                Map(x => x.RoleID, "RoleRight_Role_Key");
            }
        }

        #region Properties

        public virtual int ID { get; set; }

        public virtual int TypeID { get; set; }
        public virtual int ChildTypeID { get; set; }

        public virtual int RoleID { get; set; }

        #endregion Properties

        /// <summary>
        /// Selects all role rights by Role ID, Type ID and ChildType
        /// </summary>
        /// <param name="roleID">The role to get the rights for</param>
        /// <param name="typeID">The type to get the rights for</param>
        /// <param name="childTypeID">The child type to get the rights for</param>
        /// <returns></returns>
        public static IRoleRightAccessItem[] Select(int roleID, int typeID, int childTypeID)
        {
            var connector = ConnectorFactory.CreateConnector<RoleRightAccessItem>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.RoleID, roleID);
            filter.Add(x => x.TypeID, typeID);
            filter.Add(x => x.ChildTypeID, childTypeID);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all role rights by Role ID, Type ID and ChildType
        /// </summary>
        /// <param name="roleID">The role to get the rights for</param>
        /// <param name="typeID">The type to get the rights for</param>
        /// <param name="childTypeID">The child type to get the rights for</param>
        public static async Task<IRoleRightAccessItem[]> SelectAsync(int roleID, int typeID, int childTypeID)
        {
            var connector = ConnectorFactory.CreateConnector<RoleRightAccessItem>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.RoleID, roleID);
            filter.Add(x => x.TypeID, typeID);
            filter.Add(x => x.ChildTypeID, childTypeID);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }
    }
}