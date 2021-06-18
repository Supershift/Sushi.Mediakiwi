using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    public class PageHistory : BaseImplementation
    {
        public PageHistory()
        {
            wim.SearchListCanClickThrough = false;
            wim.DashBoardCanClickThrough = false;

            ListSearch += PageHistory_ListSearch;
        }

        private async Task PageHistory_ListSearch(ComponentListSearchEventArgs arg)
        {
            if (!string.IsNullOrEmpty(Context.Request.Query["rollback"]))
            {
                int rollbackVersion = Utility.ConvertToInt(Context.Request.Query["rollback"]);
                var pageVersion = await PageVersion.SelectOneAsync(rollbackVersion);
                var page = await Page.SelectOneAsync(CurrentPageID);
                await page.CopyFromVersionAsync(pageVersion, wim.CurrentApplicationUser);

                wim.Page.Body.Form.RefreshParent();
                // Refresh after 
              // Context.Response.Body..Write(@"<script type=""text/javascript""> parent.location.href =parent.location.href; </script>");
            }

            var list = await PageVersion.SelectAllOfPageAsync(CurrentPageID);
            foreach (var item in list)
            {
                item.RollBackTo = $@"<a href=""{wim.Console.GetSafeUrl()}&rollback={item.ID}"" class=""submit"">Restore</a>";
            }

            wim.ListDataColumns.Add(new ListDataColumn("", nameof(IPageVersion.ID), ListDataColumnType.UniqueIdentifier));
            wim.ListDataColumns.Add(new ListDataColumn("Name", nameof(IPageVersion.Name), ListDataColumnType.HighlightPresent));
            wim.ListDataColumns.Add(new ListDataColumn("Published", nameof(IPageVersion.Created)) { ColumnWidth = 80 });
            wim.ListDataColumns.Add(new ListDataColumn("", nameof(IPageVersion.RollBackTo)) { ColumnWidth = 150 });

            wim.ListDataAdd(list);
        }


        public int CurrentPageID
        {
            get { return Utility.ConvertToInt(Context.Request.Query["pageItem"]); }
        }
    }
}
