using Sushi.Mediakiwi.Framework;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.AppCentre.UI
{
    public class ListVersioning : ComponentListTemplate
    {
        public ListVersioning()
        {
            ListSearch += ListVersioning_ListSearch;
        }

        Task ListVersioning_ListSearch(ComponentListSearchEventArgs e)
        {
            wim.ListDataColumns.Add(new ListDataColumn("Created", "Created") { ColumnWidth = 90 });
            wim.ListDataColumns.Add(new ListDataColumn("User", "User") { ColumnWidth = 150 });
            wim.ListDataColumns.Add(new ListDataColumn("Change", "Type") { ColumnWidth = 90 });
            wim.ListDataColumns.Add(new ListDataColumn("Version", "Version") { ColumnWidth = 90 });
            wim.ListDataColumns.Add(new ListDataColumn("Key", "itemID") { Alignment = Align.Left });
            return Task.CompletedTask;
        }

        [Framework.ContentListItem.DataList()]
        public Mediakiwi.Data.DataList Listing { get; set; }

    }
}
