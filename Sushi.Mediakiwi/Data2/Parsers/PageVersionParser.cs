using System;
using System.Data;
using System.Data.Linq.Mapping;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.Data.Parsers
{
    [Table(Name = "wim_PageVersions")]
    public partial class PageVersionParser : IPageVersionParser
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

        public virtual IPageVersion SelectOne(int ID)
        {
            return DataParser.SelectOne<IPageVersion>(ID, false);
        }

        public virtual bool Save(IPageVersion entity)
        {
            DataParser.Save<IPageVersion>(entity);
            return true;
        }

        public List<IPageVersion> SelectAllOfPage(int pageID)
        {
            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            list.Add(new DatabaseDataValueColumn("PageVersion_Page_Key", SqlDbType.Int, pageID));
          
            return DataParser.SelectAll<IPageVersion>(list, null, null, null, null, "PageVersion_Created desc");
        }
    }
}