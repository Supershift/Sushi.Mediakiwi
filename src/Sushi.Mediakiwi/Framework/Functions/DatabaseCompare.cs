using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.Sys;

namespace Sushi.Mediakiwi.Framework.Functions
{
    public class DataBaseCompareLogic
    {
        System.Globalization.CultureInfo _culture;

        public DataBaseCompareLogic()
        {
            _culture = System.Globalization.CultureInfo.CurrentCulture;
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        string ConnectionString
        {
            get
            {
                return Data.Common.DatabaseConnectionString;
            }
        }
        /// <summary>
        /// Databases the columns.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns></returns>
        async Task<List<SysColumn>> DatabaseColumns(string table)
        {
            return await SysColumn.SelectAllAsync(table).ConfigureAwait(false);

        }

        public async Task Verify()
        {
            await Verify(Markup.sql_tables).ConfigureAwait(false);
            await Verify(Markup.sql_views).ConfigureAwait(false);
            await Verify(Markup.sql_data).ConfigureAwait(false);
            await Verify(Markup.sql_actions).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies the specified validation script.
        /// </summary>
        /// <param name="validationScript">The validation script.</param>
        public async Task Verify(string validationScript)
        {
            if (m_DataList == null)
                m_DataList = new List<DataItem>();

            string[] queries = validationScript.Split(';');

            bool ifStatementIsTrue = false;
            string info = null;

            foreach (string query in queries)
            {
                string candidate = query;

                if (candidate.IndexOf("CREATE PROCEDURE", StringComparison.InvariantCulture) > -1)
                {
                    int begin = candidate.IndexOf("(", StringComparison.CurrentCultureIgnoreCase) + 1;
                    int endin = candidate.LastIndexOf(")", StringComparison.CurrentCultureIgnoreCase) - begin;

                    string procedureData = candidate;//.Replace("\n", string.Empty).Replace("\r", string.Empty).Trim();

                    int start = procedureData.IndexOf("PROCEDURE", StringComparison.InvariantCulture) + 10;
                    int tblen = procedureData.IndexOf("\n", start, StringComparison.CurrentCultureIgnoreCase) - start;

                    procedureData = procedureData.Substring(start, tblen).Trim();

                    //  View
                    await CompareProcedure(procedureData, query.Trim()).ConfigureAwait(false);

                }
                else if (candidate.IndexOf("CREATE VIEW", StringComparison.InvariantCulture) > -1)
                {
                    int begin = candidate.IndexOf("(", StringComparison.CurrentCultureIgnoreCase) + 1;
                    int endin = candidate.LastIndexOf(")", StringComparison.CurrentCultureIgnoreCase) - begin;

                    string viewData = candidate.Replace("\n", string.Empty, StringComparison.CurrentCultureIgnoreCase).Replace("\r", string.Empty, StringComparison.CurrentCultureIgnoreCase).Trim();

                    int start = viewData.IndexOf("VIEW", StringComparison.CurrentCultureIgnoreCase) + 5;
                    int tblen = viewData.IndexOf(" AS", StringComparison.CurrentCultureIgnoreCase) - start;

                    viewData = viewData.Substring(start, tblen).Trim();

                    //  View
                    var presentColumns = await this.DatabaseColumns(viewData).ConfigureAwait(false);

                    await CompareView(presentColumns, viewData, query.Trim()).ConfigureAwait(false);

                }
                else if (candidate.IndexOf("CREATE TABLE", StringComparison.InvariantCulture) > -1)
                {
                    int begin = candidate.IndexOf("(", StringComparison.CurrentCultureIgnoreCase) + 1;
                    int endin = candidate.LastIndexOf(")", StringComparison.CurrentCultureIgnoreCase) - begin;

                    string tableInfo = candidate.Replace("\n", string.Empty, StringComparison.CurrentCultureIgnoreCase).Trim();

                    int start = tableInfo.IndexOf("TABLE", StringComparison.CurrentCultureIgnoreCase) + 6;
                    int tblen = tableInfo.IndexOf("(", StringComparison.CurrentCultureIgnoreCase) - start;

                    var tableData = tableInfo.Substring(start, tblen).Trim();

                    //  Table
                    var presentColumns = await DatabaseColumns(tableData).ConfigureAwait(false);

                    string columnData = candidate.Substring(begin, endin);
                    string[] columns = columnData.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                    var list = new List<SysColumn>();
                    foreach (string column in columns)
                    {
                        string colData = column.Trim();
                        if (string.IsNullOrEmpty(colData))
                            continue;

                        try
                        {
                            list.Add(Convert(colData));
                        }
                        catch( Exception ex)
                        {
                            throw new Exception($"Have a problem parsing {tableData}.", ex);
                        }
                    }
                    CompareTable(presentColumns, list, tableData, query.Trim());
                }

                else if (candidate.IndexOf(" INDEX ", StringComparison.InvariantCulture) > -1)
                {

                }

                else if (candidate.IndexOf("--", StringComparison.CurrentCultureIgnoreCase) > -1)
                    info = candidate.Replace("--", string.Empty, StringComparison.CurrentCultureIgnoreCase).Trim();

                else if (candidate.IndexOf("IF:", StringComparison.CurrentCultureIgnoreCase) > -1)
                    ifStatementIsTrue = await ExecuteIfNull(candidate).ConfigureAwait(false);

                else if (candidate.IndexOf("IF>:", StringComparison.CurrentCultureIgnoreCase) > -1)
                    ifStatementIsTrue = await ExecuteIfBigger(candidate).ConfigureAwait(false);

                else if (candidate.IndexOf("END IF", StringComparison.CurrentCultureIgnoreCase) > -1)
                    ifStatementIsTrue = false;

                else if (candidate.IndexOf("THEN:", StringComparison.CurrentCultureIgnoreCase) > -1)
                {
                    if (ifStatementIsTrue) AddCustom(candidate, info);
                }

            }

            if (m_DataList.Count == 0)
            {
                AddInfo("No results found");
            }
            else
            {
                AddInfo(string.Format("Verification done, found {0} queries", m_DataList.Count));
            }
        }

        static SysColumn Convert(string col)
        {
            var data = new SysColumn();

            try
            {
                string[] elements = col.Split(' ');
                data.Name = elements[0].Trim();
                data.Type = elements[1].Trim();

                if (elements.Length > 2 && elements[2].Trim().StartsWith("IDENTITY"))
                    data.IndentityInfo = elements[2].Trim();

                if (col.IndexOf("NOT NULL", StringComparison.InvariantCulture) == -1)
                    data.IsNullable = 1;
                else
                    data.IsNullable = 0;

                return data;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Could not obtain data from: {0}", col), ex);
            }
        }

        public void AddInfo(string info)
        {

        }

        /// <summary>
        /// Compares the table.
        /// </summary>
        /// <param name="origin">The origin.</param>
        /// <param name="template">The template.</param>
        /// <param name="table">The table.</param>
        /// <param name="query">The query.</param>
        void CompareTable(List<SysColumn> origin, List<SysColumn> template, string table, string query)
        {
            if (origin.Count == 0)
            {
                AddTable(table, query);
                return;
            }
            foreach (var col in template)
            {
                var arr = (from item in origin where item.Name.ToLower(System.Globalization.CultureInfo.CurrentCulture) == col.Name.ToLower(System.Globalization.CultureInfo.CurrentCulture) select item).ToArray();
                var match = arr.Length == 0 ? new SysColumn() : col;

                bool isnew = string.IsNullOrEmpty(match.Name);

                if (isnew) AddTableColumn(col, table);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public List<DataItem> m_DataList = new List<DataItem>();
        /// <summary>
        /// Adds the table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="createScript">The create script.</param>
        void AddTable(string table, string createScript)
        {
            createScript = ReplaceType(createScript
                .Replace("\n", string.Empty, StringComparison.CurrentCultureIgnoreCase)
                .Replace("\t", string.Empty, StringComparison.CurrentCultureIgnoreCase)
                .Replace("  ", " ", StringComparison.CurrentCultureIgnoreCase)
                .Trim());

            m_DataList.Add(new DataItem()
            {
                Object = table,
                Type = "Table",
                Script = string.Concat(createScript, ";")
            });
        }

        ////''

        /// <summary>
        /// Compares the view.
        /// </summary>
        /// <param name="procedure">The procedure.</param>
        /// <param name="query">The query.</param>
        async Task CompareProcedure(string procedure, string query)
        {
            var item = await SysProcedure.FetchSingle(procedure).ConfigureAwait(false);

            if (item == null)
            {
                AddProcedure(procedure, query);
                return;
            }
            string code = item.Definition;

            string a = CleanNoInfo(code);
            string b = CleanNoInfo(query);

            if (a.Contains("--ignoreupdate:1", StringComparison.CurrentCultureIgnoreCase))
            {
                AddInfo(string.Format(_culture, "Ignore update rule for stored procedure <b>{0}</b>", procedure));
                return;
            }

            if (!CleanNoInfo(a).Equals(CleanNoInfo(b)))
                AddProcedure(procedure, query);
        }

        /// <summary>
        /// Adds the procedure.
        /// </summary>
        /// <param name="procedure">The procedure.</param>
        /// <param name="createScript">The create script.</param>
        void AddProcedure(string procedure, string createScript)
        {
            m_DataList.Add(new DataItem()
            {
                Object = procedure,
                Type = "Procedure",
                Script = string.Format(_culture, "if (select COUNT(*) from sys.procedures where name = '{0}') > 0 drop procedure {0};", procedure)
            });

            m_DataList.Add(new DataItem()
            {
                Object = procedure,
                Type = "Procedure",
                Script = string.Concat(createScript, ";")
            });
        }

        /// <summary>
        /// Compares the view.
        /// </summary>
        /// <param name="origin">The origin.</param>
        /// <param name="view">The view.</param>
        /// <param name="query">The query.</param>
        async Task CompareView(List<SysColumn> origin, string view, string query)
        {
            if (origin.Count == 0)
            {
                AddView(view, query);
                return;
            }

            var data = await SysView.FetchSingle(view).ConfigureAwait(false);

            if (data != null)
            {
                var a = CleanNoInfo(data.Definition);
                var b = CleanNoInfo(query);

                if (a.Contains("--ignoreupdate:1", StringComparison.CurrentCultureIgnoreCase))
                {
                    AddInfo(string.Format(_culture, "Ignore update rule for {0}", view));
                    return;
                }

                if (!CleanNoInfo(a).Equals(CleanNoInfo(b), StringComparison.CurrentCultureIgnoreCase))
                    AddView(view, query);
            }
            else
            {
                AddView(view, query);
            }
        }

        /// <summary>
        /// Cleans the no info.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns></returns>
        string CleanNoInfo(string candidate)
        {
            candidate = candidate
                .Replace(" ", string.Empty, StringComparison.CurrentCultureIgnoreCase)
                .Replace("\n", string.Empty, StringComparison.CurrentCultureIgnoreCase)
                .Replace("\r", string.Empty, StringComparison.CurrentCultureIgnoreCase)
                .Replace("\t", string.Empty, StringComparison.CurrentCultureIgnoreCase)
                .ToLower(_culture)
                ;

            if (!candidate.EndsWith(";", StringComparison.CurrentCultureIgnoreCase))
                candidate += ';';

            return candidate;
        }

        /// <summary>
        /// Adds the view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="createScript">The create script.</param>
        void AddView(string view, string createScript)
        {
            m_DataList.Add(new DataItem()
            {
                Object = view,
                Type = "View",
                Script = string.Format(_culture, "if (select COUNT(*) from sys.views where name = '{0}') > 0 drop view {0};", view)
            });

            m_DataList.Add(new DataItem()
            {
                Object = view,
                Type = "View",
                Script = string.Concat(createScript, ";")
            });
        }

        /// <summary>
        /// 
        /// </summary>
        public class DataItem
        {
            public DataItem()
            {
                this.ShouldRun = true;
            }
            public bool ShouldRun { get; set; }
            public string Object { get; set; }
            public string Type { get; set; }
            public string Script { get; set; }
        }
        /// <summary>
        /// Replaces the type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="isIntersystemCache">if set to <c>true</c> [is intersystem cache].</param>
        /// <returns></returns>
        string ReplaceType(string type)
        {
            return type;
        }
        /// <summary>
        /// Adds the table column.
        /// </summary>
        /// <param name="col">The col.</param>
        /// <param name="table">The table.</param>
        void AddTableColumn(SysColumn col, string table)
        {

            m_DataList.Add(new DataItem()
            {
                Object = col.Name,
                Type = "Column",
                Script = ReplaceType(string.Format("ALTER TABLE {1} ADD {0} {2};", col.Name, table, col.Type))
            });

        }
    
        /// <summary>
        /// Executes if null.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <returns></returns>
        async Task<bool> ExecuteIfNull(string script)
        {
            script = script.Replace("IF:", string.Empty).Trim();

            try
            {
                var count = await SysColumn.SelectCountAsync(script).ConfigureAwait(false);
                return count < 1;
            }
            catch (Exception)
            {
                return true;
            }
        }

        /// <summary>
        /// Executes if bigger.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <returns></returns>
        async Task<bool> ExecuteIfBigger(string script)
        {
            script = script.Replace("IF>:", string.Empty).Trim();
            try
            {
                var count = await SysColumn.SelectCountAsync(script).ConfigureAwait(false);
                return count > 0;
            }
            catch (Exception)
            {
                return true;
            }
        }
        /// <summary>
        /// Adds the custom.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <param name="info">The info.</param>
        void AddCustom(string script, string info)
        {
            script = script.Replace("THEN:", string.Empty).Trim();

            m_DataList.Add(new DataItem()
            {
                Object = info,
                Type = "Custom",
                Script = string.Concat(script, ";")
            });


        }
        /// <summary>
        /// Handles the Click event of the uxRun control.
        /// </summary>
        public async Task Start()
        {
            AddInfo("Executing queries");

            foreach (DataItem row in m_DataList)
            {
                if (row.ShouldRun)
                {
                    try
                    {
                        await SysColumn.ExecuteAsync(row.Script).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        AddInfo(string.Format("Query could not run: {0} error {1}", row.Script, ex.Message));
                    }
                }
            }
        }
    }
}
