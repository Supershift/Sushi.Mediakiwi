using System;
using System.Data;
using System.Data.Linq.Mapping;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.Data.Parsers
{
    /// <summary>
    /// 
    /// </summary>
    public class PortalRightParser : IPortalRightParser
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

        public virtual IPortalRight SelectOne(int ID)
        {
            return DataParser.SelectOne<IPortalRight>(ID, true);
        }

        public virtual IPortalRight[] SelectAll(int roleID)
        {
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("PortalRight_Role_Key", SqlDbType.Int, roleID));
            return DataParser.SelectAll<IPortalRight>(whereClause).ToArray();
        }
    }
}
