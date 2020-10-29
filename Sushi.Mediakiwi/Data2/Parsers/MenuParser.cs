using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.Data.Parsers
{
    [DatabaseTable("wim_Menus")]
    public class MenuParser : IMenuParser
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

        public virtual void Delete(IMenu entity)
        {
            DataParser.Delete<IMenu>(entity);
        }

        public virtual bool Save(IMenu entity)
        {
            return DataParser.Save<IMenu>(entity) > 0;
        }

        public virtual IMenu SelectOne(int ID)
        {
            return DataParser.SelectOne<IMenu>(ID, true);
        }

        public virtual IMenu[] SelectAll()
        {
            return DataParser.SelectAll<IMenu>().ToArray();
        }
    }
}
