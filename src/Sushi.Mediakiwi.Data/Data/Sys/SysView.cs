using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data.Sys
{
    /// <summary>
    ///
    /// </summary>
    [DataMap(typeof(SysViewMap))]
    public class SysView
    {
        internal class SysViewMap : DataMap<SysView>
        {
            public SysViewMap()
            {
                Table("INFORMATION_SCHEMA.VIEWS");
                Map(x => x.Definition, "VIEW_DEFINITION");
            }
        }

        #region Properties

        public string Definition { get; set; }
        #endregion Properties


        public static async Task<SysView> FetchSingle(string tableName)
        {
            var connector = new Connector<SysView>();
            var filter = connector.CreateDataFilter();

            filter.AddParameter("@name", System.Data.SqlDbType.VarChar, tableName);
            var sqlText = "select VIEW_DEFINITION from INFORMATION_SCHEMA.VIEWS WHERE TABLE_NAME = @name";

            return await connector.FetchSingleAsync(sqlText, filter).ConfigureAwait(false);
        }

    }
}
