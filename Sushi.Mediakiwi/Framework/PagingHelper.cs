using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// Provides methods to convert between Sushi MicroORM's and Sushi.Mediakiwi.Framework's paging objects
    /// </summary>
    [Obsolete("Moved to Sushi.Mediakiwi.Framework.SushiMicroORM")]
    public static class PagingHelper
    {
        /// <summary>
        /// Converts a Wim paging object to a Sushi MicroORM paging object. Use this function to create an object that can be used to query with SushiMicroORM.
        /// </summary>
        /// <param name="gridDataDetail"></param>
        /// <returns></returns>
        //public static Sushi.MicroORM.PagingData ConvertWimToSushi(Sushi.Mediakiwi.DataEntities.GridDataDetail gridDataDetail)
        //{
        //    if (gridDataDetail != null && !gridDataDetail.ShowAll)
        //    {
        //        return new Sushi.MicroORM.PagingData()
        //        {
        //            NumberOfRows = gridDataDetail.PageSize,
        //            PageIndex = gridDataDetail.CurrentPage
        //        };
        //    }
        //    else
        //        return null;
        //}

        /// <summary>
        /// Set values from a Sushi MicroORM paging object to a Wim paging object. Use this after running a query with SushiMicroORM to set the paging specific results on a Wim paging object.
        /// </summary>
        /// <param name="paging"></param>
        /// <param name="gridDataDetail"></param>
        /// <returns></returns>
        //public static Sushi.Mediakiwi.DataEntities.GridDataDetail ApplySushiToWim(Sushi.MicroORM.PagingData paging, Sushi.Mediakiwi.DataEntities.GridDataDetail gridDataDetail)
        //{
        //    if (paging != null && gridDataDetail != null)
        //        gridDataDetail.ResultCount = paging.TotalNumberOfRows;
        //    return gridDataDetail;
        //}
    }
}
