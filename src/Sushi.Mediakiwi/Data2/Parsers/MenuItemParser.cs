using System.Collections.Generic;
using System.Data;
using System.Linq;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.Data.Parsers
{
    public class MenuItemParser : IMenuItemParser
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

        public virtual bool Save(IMenuItem entity)
        {
            return DataParser.Save<IMenuItem>(entity) > 0;
        }

        public virtual void Delete(IMenuItem entity)
        {
            DataParser.Delete<IMenuItem>(entity);
        }

        public virtual IMenuItem[] SelectAll(int menuID)
        {
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("MenuItem_Menu_Key", SqlDbType.Int, menuID));

            return DataParser.SelectAll<IMenuItem>(whereClause).ToArray();
        }

        public virtual IMenuItem[] SelectAll_Dashboard(int dashboardID)
        {
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("MenuItem_Dashboard_Key", SqlDbType.Int, dashboardID));

            return DataParser.SelectAll<IMenuItem>(whereClause).ToArray();
        }
    }
}
