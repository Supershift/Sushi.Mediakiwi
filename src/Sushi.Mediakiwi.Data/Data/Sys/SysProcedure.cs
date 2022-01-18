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
    [DataMap(typeof(SysProcedureMap))]
    public class SysProcedure
    {
        internal class SysProcedureMap : DataMap<SysProcedure>
        {
            public SysProcedureMap()
            {
                Table("INFORMATION_SCHEMA.ROUTINES");
                Map(x => x.Definition, "ROUTINE_DEFINITION");
            }
        }

        #region Properties

        public string Definition { get; set; }
        #endregion Properties


        public static async Task<SysProcedure> FetchSingle(string tableName)
        {
            var connector = new Connector<SysProcedure>();
            var filter = connector.CreateQuery();

            filter.AddParameter("@name", System.Data.SqlDbType.VarChar, tableName);
            var sqlText = "select ROUTINE_DEFINITION from INFORMATION_SCHEMA.ROUTINES WHERE SPECIFIC_NAME = @name";

            return await connector.FetchSingleAsync(sqlText, filter).ConfigureAwait(false);
        }

    }
}
