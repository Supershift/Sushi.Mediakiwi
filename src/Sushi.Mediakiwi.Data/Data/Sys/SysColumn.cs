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
    [DataMap(typeof(SysColumnMap))]
    public class SysColumn 
    {
        internal class SysColumnMap : DataMap<SysColumn>
        {
            public SysColumnMap()
            {
                Table("syscolumns");
                Id(x => x.Name, "Name");
                Map(x => x.Prec, "prec");
                Map(x => x.IsNullable, "isnullable");
            }
        }

        #region Properties

        public int Prec { get; set; }
        public string Name { get; set; }
        public int IsNullable { get; set; }
        /// <summary>
        /// This column is not derived from the sys tables.
        /// </summary>
        public string Type { get; set; }
        public string IndentityInfo { get; set; }
        #endregion Properties

        public static async Task<List<SysColumn>> SelectAllAsync(string tableName)
        {
            var connector = new Connector<SysColumn>();
            var filter = connector.CreateQuery();
            
            filter.AddParameter("@table", tableName);
            var sqlText = "select Name, prec, isnullable from syscolumns where id = (select object_id from sys.objects where name = @table) order by colorder";

            return await connector.FetchAllAsync(sqlText, filter).ConfigureAwait(false);
        }

        public static async Task<int> SelectCountAsync(string sqlText)
        {
            var connector = new Connector<SysColumn>();
            return await connector.ExecuteScalarAsync<int>(sqlText).ConfigureAwait(false);
        }

        public static async Task ExecuteAsync(string sqlText)
        {
            var connector = new Connector<SysColumn>();
            await connector.ExecuteNonQueryAsync(sqlText).ConfigureAwait(false);
        }

    }
}
