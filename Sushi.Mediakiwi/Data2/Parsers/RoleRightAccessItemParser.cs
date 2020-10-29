using System.Data;
using Sushi.Mediakiwi.Data.DalReflection;
using System.Linq;
using Sushi.Mediakiwi.Framework;
using System.Collections.Generic;

namespace Sushi.Mediakiwi.Data.Parsers
{
    internal class RoleRightAccessItemParser : IRoleRightAccessItemParser
    {
        static ISqlEntityParser _DataParser;
        static ISqlEntityParser DataParser
        {
            get
            {
                if (_DataParser == null)
                    _DataParser = Environment.GetInstance<ISqlEntityParser>();
                return _DataParser;
            }
        }

        public virtual IRoleRightAccessItem[] Select(int roleID, int typeID, int childTypeID, string portal = null)
        {
            string alternativeConnectionString = null;
            if (!string.IsNullOrEmpty(portal))
            {
                var instance = Sushi.Mediakiwi.Data.Common.GetPortal(portal);
                alternativeConnectionString = instance.Connection;
            }

            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            list.Add(new DatabaseDataValueColumn("RoleRight_Role_Key", SqlDbType.Int, roleID));
            list.Add(new DatabaseDataValueColumn("RoleRight_Type", SqlDbType.Int, typeID));
            list.Add(new DatabaseDataValueColumn("RoleRight_Child_Type", SqlDbType.Int, childTypeID));
            return DataParser.SelectAll<IRoleRightAccessItem>(list, null, null, alternativeConnectionString).ToArray();
        }
    }
}
