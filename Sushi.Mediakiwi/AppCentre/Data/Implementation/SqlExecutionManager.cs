using System;
using System.Collections.Generic;
using System.Text;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.Data.DalReflection;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    /// <summary>
    /// 
    /// </summary>
    public class SqlExecutionManager : ComponentListTemplate
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        [Sushi.Mediakiwi.Framework.ContentListSearchItem.TextArea("T-SQL", 5000)]
        public string Code { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlExecutionManager"/> class.
        /// </summary>
        public SqlExecutionManager()
        {
            wim.ShowInFullWidthMode = true;
            this.ListSearch += new ComponentSearchEventHandler(SqlExecutionManager_ListSearch);
        }

        void SqlExecutionManager_ListSearch(object sender, ComponentListSearchEventArgs e)
        {
            if (string.IsNullOrEmpty(this.Code)) return;
            try
            {
                System.Data.DataTable dt = null;
                foreach (string code in this.Code.Split(';'))
                {
                    if (string.IsNullOrEmpty(code.Trim())) continue;
                    using (Sushi.Mediakiwi.Data.Connection.DataCommander dac = new Sushi.Mediakiwi.Data.Connection.DataCommander(Sushi.Mediakiwi.Data.Common.DatabaseConnectionString))
                    {
                        dac.ConnectionType = Sushi.Mediakiwi.Data.Common.DatabaseConnectionType;
                        dac.Text = code;

                        dt = dac.ExecAdapterTable;

                        if (wim.ListDataTable == null && dt != null)
                        {
                            wim.ListDataTable = dt;
                            foreach (System.Data.DataColumn col in wim.ListDataTable.Columns)
                            {
                                wim.ListDataColumns.Add(col.ColumnName, col.ColumnName);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Exception ex2 = ex;
                string candidate = "";
                while (ex2 != null)
                {
                    if (candidate == "")
                        candidate = string.Format("{0}", ex2.Message);
                    else
                        candidate += string.Format("{0}<br/><hr/>{1}", ex2.Message, candidate);

                        ex2 = ex2.InnerException;
                }
                wim.Notification.AddError(candidate);
            }
        }
    }
}
