using System;
using System.Data;
using System.Data.Linq.Mapping;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Framework;

namespace  Sushi.Mediakiwi.Data
{
    public partial class ComponentTargetParser : IComponentTargetParser
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
        public virtual void Delete(IComponentTarget entity)
        {
            DataParser.Delete<IComponentTarget>(entity);
        }

        public virtual IComponentTarget SelectOne(int ID)
        {
            return DataParser.SelectOne<IComponentTarget>(ID, false);
        }

        public virtual IComponentTarget[] SelectAll(int pageID)
        {
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("ComponentTarget_Page_Key", SqlDbType.Int, pageID));
            return DataParser.SelectAll<IComponentTarget>(whereClause).ToArray();
        }


        public virtual void DeleteCompetion(IComponentTarget entity)
        {
            string sql = string.Format("delete from wim_ComponentTargets where ComponentTarget_Page_Key = {2} and ComponentTarget_Component_Target = '{0}' and not ComponentTarget_Component_Source = '{1}'", entity.Target, entity.Source, entity.PageID);
            DataParser.Execute(sql);
        }

        public virtual IComponentTarget[] SelectAll(Guid componentGuid)
        {
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("ComponentTarget_Component_Source", SqlDbType.UniqueIdentifier, componentGuid));
            return DataParser.SelectAll<IComponentTarget>(whereClause).ToArray();
        }

        public virtual bool Save(IComponentTarget entity)
        {
            DataParser.Save<IComponentTarget>(entity);
            return true;
        }
    }
}