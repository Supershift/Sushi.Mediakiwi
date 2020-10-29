using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.AppCentre.Data.UI
{
    public class PageSelectListList : ComponentListTemplate
    {
        public PageSelectListList()
        {
            this.ListSearch += new ComponentSearchEventHandler(PageSelectList_ListSearch);
        }

        [Sushi.Mediakiwi.Framework.ContentListSearchItem.TextField("Search for page", 50)]
        public string FilterSearch { get; set; }

        void PageSelectList_ListSearch(object sender, ComponentListSearchEventArgs e)
        {
            wim.ListDataColumns.Add(new ListDataColumn("ID", "ID", ListDataColumnType.UniqueIdentifier));
            wim.ListDataColumns.Add(new ListDataColumn("Name", "Name", ListDataColumnType.HighlightPresent));
            wim.ListDataColumns.Add(new ListDataColumn("Path", "CompletePath"));
            wim.ListDataColumns.Add(new ListDataColumn("Created", "Created") { ColumnWidth = 90 });

            var list = Sushi.Mediakiwi.Data.Page.SelectAll(FilterSearch, true);

            wim.ListDataApply(list);
        }
    }
}
