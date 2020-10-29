using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    public class PageHistory : BaseImplementation
    {
        //public PageHistory()
        //{
        //    wim.SearchListCanClickThrough = false;
        //    wim.DashBoardCanClickThrough = false;
        //    this.ListSearch += PageHistory_ListSearch;
        //}

        //private void PageHistory_ListSearch(object sender, ComponentListSearchEventArgs e)
        //{
        //    if (!String.IsNullOrEmpty(Request.Form["rollback"]))
        //    {
        //        int rollbackVersion = Utility.ConvertToInt(Request.Form["rollback"]);
        //        var pageVersion = PageVersion.SelectOne(rollbackVersion);
        //        var page = Page.SelectOne(CurrentPageID);
        //        page.CopyFromVersion(pageVersion, wim.CurrentApplicationUser);

        //        // Refresh after 
        //        Response.Write(@"<script type=""text/javascript""> parent.location.href =parent.location.href; </script>");
        //    }
        //    var list = Sushi.Mediakiwi.Data.PageVersion.SelectAllOfPage(CurrentPageID);
        //    foreach (var item in list)
        //    {
        //        item.RollBackTo = $@"<a href=""{Utility.GetSafeUrl(Request)}&rollback={item.ID}"" class=""submit"">{Resource.Rollback}</a>";
        //    }
        //    wim.ListDataColumns.Add("", "ID", ListDataColumnType.UniqueIdentifier);
        //    wim.ListDataColumns.Add(Resource._name, "Name", ListDataColumnType.HighlightPresent);
        //    wim.ListDataColumns.Add(Resource.Date, "Created");
        //    wim.ListDataColumns.Add(new ListDataColumn("", "RollBackTo") { ColumnWidth = 150 });

        //    wim.ListDataApply<IPageVersion>(list);

        //}



        //public int CurrentPageID
        //{
        //    get { return Wim.Utility.ConvertToInt(Request["pageItem"]); }
        //}
      
 

    
    }
}
