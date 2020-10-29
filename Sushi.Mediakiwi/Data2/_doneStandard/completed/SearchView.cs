using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Data.Parsers;
using Sushi.Mediakiwi.Framework;
using Wim.Utilities;

namespace Sushi.Mediakiwi.Data
{
    [DatabaseTable("wim_SearchView", Order = "SearchView_SortOrder asc")]
    public class SearchView : ISearchView
    {
        static ISearchViewParser _Parser;
        static ISearchViewParser Parser
        {
            get
            {
                if (_Parser == null)
                    _Parser = Environment.GetInstance<ISearchViewParser>();
                return _Parser;
            }
        }

        #region MOVED to EXTENSION / LOGIC

        //public virtual string Url
        //{
        //    get
        //    {
        //        switch (this.TypeID)
        //        {
        //            case 1: return Wim.Utility.AddApplicationPath(string.Format("{0}?list={1}", Sushi.Mediakiwi.Data.Environment.Current.RelativePath, this.ItemID));
        //            case 2: return Wim.Utility.AddApplicationPath(string.Format("{0}?folder={1}", Sushi.Mediakiwi.Data.Environment.Current.RelativePath, this.ItemID));
        //            case 3: return Wim.Utility.AddApplicationPath(string.Format("{0}?page={1}", Sushi.Mediakiwi.Data.Environment.Current.RelativePath, this.ItemID));
        //            case 4: return Wim.Utility.AddApplicationPath(string.Format("{0}?dashboard={1}", Sushi.Mediakiwi.Data.Environment.Current.RelativePath, this.ItemID));
        //            case 5: return Wim.Utility.AddApplicationPath(string.Format("{0}?gallery={1}", Sushi.Mediakiwi.Data.Environment.Current.RelativePath, this.ItemID));
        //        }
        //        return "#";
        //    }
        //}

        #endregion MOVED to EXTENSION / LOGIC

        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

        #region Properties
        public virtual string TitleHighlighted
        {
            get
            {
                return string.Format("{0} <b>({1})</b>", this.Title, this.Type);
            }
        }

        [DatabaseColumn("SearchView_Key", SqlDbType.VarChar)]
        public virtual string ID { get; set; }

        [DatabaseColumn("SearchView_Title", SqlDbType.NVarChar)]
        public virtual string Title { get; set; }

        [DatabaseColumn("SearchView_Description", SqlDbType.NVarChar, IsNullable = true)]
        public virtual string Description { get; set; }

        [DatabaseColumn("SearchView_Site_Key", SqlDbType.Int)]
        public virtual int SiteID { get; set; }

        [DatabaseColumn("SearchView_Folder_Key", SqlDbType.Int)]
        public virtual int FolderID { get; set; }

        [DatabaseColumn("SearchView_SortOrder", SqlDbType.Int)]
        public virtual int SortOrder { get; set; }

        [DatabaseColumn("SearchView_Type", SqlDbType.Int)]
        public virtual int TypeID { get; set; }

        [DatabaseColumn("SearchView_Item_Key", SqlDbType.Int)]
        public virtual int ItemID { get; set; }


        public virtual string Type
        {
            get
            {
                switch (TypeID)
                {
                    case 8: return "Folder/container";
                    case 7: return "Section";
                    case 6: return "Website";
                    case 5: return "Gallery";
                    case 4: return "Dashboard";
                    case 3: return "Page";
                    case 2: return "Folder";
                    case 1: return "List";
                }
                return null;
            }
        }
        #endregion Properties

        public static ISearchView[] SelectAll(int folderID)
        {
            return Parser.SelectAll(folderID);
        }

        public static ISearchView[] SelectAll(int? siteID, int? filterType, string search)
        {
            return Parser.SelectAll(siteID, filterType, search);
        }

        public static ISearchView[] SelectAll(string[] items)
        {
            return Parser.SelectAll(items);
        }

        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard
    }
}
