using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Framework;
using Wim.Utilities;

namespace Sushi.Mediakiwi.Data.Parsers
{
    public class SearchViewParser : ISearchViewParser
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

        public virtual ISearchView[] SelectAll()
        {
            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            return DataParser.SelectAll<ISearchView>(list, "Sushi.Mediakiwi.Data.Page.Searchview").ToArray();
        }

        public virtual ISearchView[] SelectAll(int folderID)
        {
            var complete = SelectAll();
            return complete.Where(x => x.FolderID == folderID).ToArray();

            //List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            //list.Add(new DatabaseDataValueColumn("SearchView_Folder_Key", SqlDbType.Int, folderID));
            //return DataParser.SelectAll<ISearchView>(list).ToArray();
        }

        public virtual ISearchView[] SelectAll(int? siteID, int? filterType, string search)
        {
            DataRequest data = new DataRequest();

            if (!string.IsNullOrEmpty(search))
            {
                search = string.Format("%{0}%", search.Trim().Replace(" ", "%"));
                data.AddParam("SEARCH", search, SqlDbType.NVarChar);

                data.AddWhere("(SearchView_Title like @SEARCH OR SearchView_Description like @SEARCH)");
            }

            if (siteID.HasValue)
                data.AddWhere("SearchView_Folder_Key", SqlDbType.Int, siteID);

            if (filterType.HasValue)
                data.AddWhere("SearchView_Type", SqlDbType.Int, filterType);

            return DataParser.SelectAll<ISearchView>(data).ToArray();
        }

        public virtual ISearchView[] SelectAll(string[] items)
        {
            if (items?.Length == 0)
                return null;
            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            list.Add(new DatabaseDataValueColumn("SearchView_Key", SqlDbType.VarChar, items, DatabaseDataValueCompareType.In));
            return DataParser.SelectAll<ISearchView>(list).ToArray();
        }
    }
}
