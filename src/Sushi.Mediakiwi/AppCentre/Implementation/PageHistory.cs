using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wim.Data;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.Data;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    public class PageHistory : BaseImplementation
    {
        //public PageHistory()
        //{
        //    wim.SearchListCanClickThrough = false;
        //    wim.DashBoardCanClickThrough = false;
        //    ListSearch += PageHistory_ListSearch;
        //}

        //private Task PageHistory_ListSearch(ComponentListSearchEventArgs arg)
        //{
        //    if (!String.IsNullOrEmpty(Request["rollback"]))
        //    {
        //        int rollbackVersion = Utility.ConvertToInt(Request.Query["rollback"]);
        //        var pageVersion = PageVersion.SelectOne(rollbackVersion);
        //        var page = Page.SelectOne(CurrentPageID);
        //        page.CopyFromVersion(pageVersion, wim.CurrentApplicationUser);

        //        // Refresh after 
        //        Response.Write(@"<script type=""text/javascript""> parent.location.href =parent.location.href; </script>");
        //    }
        //    var list = PageVersion.SelectAllOfPage(CurrentPageID);
        //    foreach (var item in list)
        //    {
        //        item.RollBackTo = $@"<a href=""{wim.Console.GetSafeUrl()}&rollback={item.ID}"" class=""submit"">Restore</a>";
        //    }
        //    wim.ListDataColumns.Add("", "ID", ListDataColumnType.UniqueIdentifier);
        //    wim.ListDataColumns.Add("Name", "Name", ListDataColumnType.HighlightPresent);
        //    wim.ListDataColumns.Add("Published", "Created", 80);

        //    wim.ListDataColumns.Add(new ListDataColumn("", "RollBackTo") { ColumnWidth = 150 });

        //    wim.ListDataApply<IPageVersion>(list);

        //}

        //public int CurrentPageID
        //{
        //    get { return Wim.Utility.ConvertToInt(Request["pageItem"]); }
        //}
    }
}
