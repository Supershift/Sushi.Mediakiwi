using System.Data;
using Sushi.Mediakiwi.Data.DalReflection;
using System.Linq;
using Sushi.Mediakiwi.Framework;
using System.Collections.Generic;
using Sushi.Mediakiwi.Data.Parsers;

namespace Sushi.Mediakiwi.Data
{
    [DatabaseTable("wim_RoleRights")]
    public class RoleRightAccessItem : IRoleRightAccessItem
    {
        static IRoleRightAccessItemParser _Parser;
        static IRoleRightAccessItemParser Parser
        {
            get
            {
                if (_Parser == null)
                    _Parser = Environment.GetInstance<IRoleRightAccessItemParser>();
                return _Parser;
            }
        }

        public static IRoleRightAccessItem[] Select(int roleID, int typeID, int childTypeID, string portal)
        {
            return Parser.Select(roleID, typeID, childTypeID, portal);
        }

        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

        #region Properties
        [DatabaseColumn("RoleRight_Child_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public virtual int ID { get; set; }

        [DatabaseColumn("RoleRight_Type", SqlDbType.Int)]
        public virtual int TypeID { get; set; }

        [DatabaseColumn("RoleRight_Child_Type", SqlDbType.Int)]
        public virtual int ChildTypeID { get; set; }

        [DatabaseColumn("RoleRight_Role_Key", SqlDbType.Int)]
        public virtual int RoleID { get; set; }
        #endregion Properties

        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard

    }
}
