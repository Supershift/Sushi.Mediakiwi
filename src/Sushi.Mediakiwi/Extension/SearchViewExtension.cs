using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Data.Interfaces;

public static class SearchViewExtension
{
    public static string GetUrl(this ISearchView inSearchView)
    {
        switch (inSearchView.TypeID)
        {
            case 1: return Utility.AddApplicationPath(string.Format("{0}?list={1}", Sushi.Mediakiwi.Data.Environment.Current.RelativePath, inSearchView.ItemID));
            case 2: return Utility.AddApplicationPath(string.Format("{0}?folder={1}", Sushi.Mediakiwi.Data.Environment.Current.RelativePath, inSearchView.ItemID));
            case 3: return Utility.AddApplicationPath(string.Format("{0}?page={1}", Sushi.Mediakiwi.Data.Environment.Current.RelativePath, inSearchView.ItemID));
            case 4: return Utility.AddApplicationPath(string.Format("{0}?dashboard={1}", Sushi.Mediakiwi.Data.Environment.Current.RelativePath, inSearchView.ItemID));
            case 5: return Utility.AddApplicationPath(string.Format("{0}?gallery={1}", Sushi.Mediakiwi.Data.Environment.Current.RelativePath, inSearchView.ItemID));
        }
        return "#";
    }
}
