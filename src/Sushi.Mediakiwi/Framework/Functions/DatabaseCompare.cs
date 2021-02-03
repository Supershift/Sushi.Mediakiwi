using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Sushi.Mediakiwi.Framework.Functions
{
    public class DataCompare
    {
        public DataCompare()
        {
        }

        /// <summary>
        /// Adds the info.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        void AddInfo(string candidate)
        {
            System.Web.HttpContext.Current.Response.Write(candidate + "<br/>");
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        string ConnectionString
        {
            get
            {
                return Sushi.Mediakiwi.Data.Common.DatabaseConnectionString;
            }
        }
        /// <summary>
        /// Databases the columns.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns></returns>
        DbColumn[] DatabaseColumns(string table)
        {
            List<DbColumn> list = new List<DbColumn>();
            using (Sushi.Mediakiwi.Data.Connection.SqlCommander dac = new Sushi.Mediakiwi.Data.Connection.SqlCommander(ConnectionString))
            {
                dac.SqlText = string.Format("select [Name], prec, isnullable from syscolumns where id = (select object_id from sys.objects where name = '{0}') order by colorder", table.Trim());

                try
                {
                    IDataReader reader = dac.ExecReader;

                    while (reader.Read())
                    {
                        list.Add(new DbColumn()
                        {
                            Name = reader.GetString(0)
                        });
                    }
                }
                catch (Exception ex)
                {
                    AddInfo(ex.Message);
                }
                return list.ToArray();
            }
        }
        /// <summary>
        /// Verifies the specified validation script.
        /// </summary>
        /// <param name="validationScript">The validation script.</param>
        public void Verify(string validationScript)
        {
            if (m_DataList == null)
                m_DataList = new List<DataItem>();

            using (Sushi.Mediakiwi.Data.Connection.SqlCommander dac = new Sushi.Mediakiwi.Data.Connection.SqlCommander(ConnectionString))
            {
                string[] queries = validationScript.Split(';');

                bool ifStatementIsTrue = false;
                string info = null;

                foreach (string query in queries)
                {
                    string candidate = query;

                    if (candidate.IndexOf("CREATE PROCEDURE", StringComparison.InvariantCulture) > -1)
                    {
                        int begin = candidate.IndexOf("(") + 1;
                        int endin = candidate.LastIndexOf(")") - begin;

                        string procedureData = candidate;//.Replace("\n", string.Empty).Replace("\r", string.Empty).Trim();

                        int start = procedureData.IndexOf("PROCEDURE", StringComparison.InvariantCulture) + 10;
                        int tblen = procedureData.IndexOf("\n", start) - start;

                        procedureData = procedureData.Substring(start, tblen).Trim();

                        //  View
                        CompareProcedure(procedureData, query.Trim());

                    }
                    else if (candidate.IndexOf("CREATE VIEW", StringComparison.InvariantCulture) > -1)
                    {
                        int begin = candidate.IndexOf("(") + 1;
                        int endin = candidate.LastIndexOf(")") - begin;

                        string viewData = candidate.Replace("\n", string.Empty).Replace("\r", string.Empty).Trim();

                        int start = viewData.IndexOf("VIEW", StringComparison.InvariantCulture) + 5;
                        int tblen = viewData.IndexOf(" AS") - start;

                        viewData = viewData.Substring(start, tblen).Trim();

                        //  View
                        DbColumn[] presentColumns = this.DatabaseColumns(viewData);

                        CompareView(presentColumns, viewData, query.Trim());

                    }
                    else if (candidate.IndexOf("CREATE TABLE", StringComparison.InvariantCulture) > -1)
                    {
                        int begin = candidate.IndexOf("(") + 1;
                        int endin = candidate.LastIndexOf(")") - begin;

                        string tableData = candidate.Replace("\n", string.Empty).Trim();

                        int start = tableData.IndexOf("TABLE", StringComparison.InvariantCulture) + 6;
                        int tblen = tableData.IndexOf("(") - start;

                        tableData = tableData.Substring(start, tblen).Trim();

                        //  Table
                        DbColumn[] presentColumns = this.DatabaseColumns(tableData);

                        string columnData = candidate.Substring(begin, endin);
                        string[] columns = columnData.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                        List<DbColumn> list = new List<DbColumn>();
                        foreach (string column in columns)
                        {
                            string colData = column.Trim();
                            if (string.IsNullOrEmpty(colData))
                                continue;

                            list.Add(new DbColumn(colData));
                        }
                        CompareTable(presentColumns, list.ToArray(), tableData, query.Trim());
                    }

                    else if (candidate.IndexOf(" INDEX ", StringComparison.InvariantCulture) > -1)
                    {

                    }

                    else if (candidate.IndexOf("--", StringComparison.InvariantCulture) > -1)
                        info = candidate.Replace("--", string.Empty).Trim();

                    else if (candidate.IndexOf("IF:", StringComparison.InvariantCulture) > -1)
                        ifStatementIsTrue = ExecuteIfNull(candidate);

                    else if (candidate.IndexOf("IF>:", StringComparison.InvariantCulture) > -1)
                        ifStatementIsTrue = ExecuteIfBigger(candidate);

                    else if (candidate.IndexOf("END IF", StringComparison.InvariantCulture) > -1)
                        ifStatementIsTrue = false;

                    else if (candidate.IndexOf("THEN:", StringComparison.InvariantCulture) > -1)
                    {
                        if (ifStatementIsTrue) AddCustom(candidate, info);
                    }

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

        /// <summary>
        /// Compares the table.
        /// </summary>
        /// <param name="origin">The origin.</param>
        /// <param name="template">The template.</param>
        /// <param name="table">The table.</param>
        /// <param name="query">The query.</param>
        void CompareTable(DbColumn[] origin, DbColumn[] template, string table, string query)
        {
            if (origin.Length == 0)
            {
                AddTable(table, query);
                return;
            }
            foreach (DbColumn col in template)
            {
                DbColumn[] arr = (from item in origin where item.Name.ToLower() == col.Name.ToLower() select item).ToArray();
                DbColumn match = arr.Length == 0 ? new DbColumn() : col;

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
            createScript = ReplaceType(createScript.Replace("\n", string.Empty).Replace("\t", string.Empty).Replace("  ", " ").Trim());

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
        void CompareProcedure(string procedure, string query)
        {
            using (Sushi.Mediakiwi.Data.Connection.SqlCommander dac = new Sushi.Mediakiwi.Data.Connection.SqlCommander(ConnectionString))
            {
                dac.SqlText = string.Format("select ROUTINE_DEFINITION from INFORMATION_SCHEMA.ROUTINES WHERE SPECIFIC_NAME = '{0}'", procedure);
                object item = dac.ExecScalar();

                if (item == null)
                {
                    AddProcedure(procedure, query);
                    return;
                }
                string code = item.ToString();

                string a = CleanNoInfo(code);
                string b = CleanNoInfo(query);

                if (a.Contains("--ignoreupdate:1"))
                {
                    AddInfo(string.Format("Ignore update rule for stored procedure <b>{0}</b>", procedure));
                    return;
                }

                if (!CleanNoInfo(a).Equals(CleanNoInfo(b)))
                    AddProcedure(procedure, query);
            }
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
                Script = string.Format("if (select COUNT(*) from sys.procedures where name = '{0}') > 0 drop procedure {0};", procedure)
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
        void CompareView(DbColumn[] origin, string view, string query)
        {
            if (origin.Length == 0)
            {
                AddView(view, query);
                return;
            }
            using (Sushi.Mediakiwi.Data.Connection.SqlCommander dac = new Sushi.Mediakiwi.Data.Connection.SqlCommander(ConnectionString))
            {
                dac.SqlText = string.Format("select VIEW_DEFINITION from INFORMATION_SCHEMA.VIEWS WHERE TABLE_NAME = '{0}'", view);
                string code = dac.ExecScalar().ToString();

                string a = CleanNoInfo(code);
                string b = CleanNoInfo(query);


                if (a.Contains("--ignoreupdate:1"))
                {
                    AddInfo(string.Format("Ignore update rule for {0}", view));
                    return;
                }

                if (!CleanNoInfo(a).Equals(CleanNoInfo(b)))
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
                .Replace(" ", string.Empty)
                .Replace("\n", string.Empty)
                .Replace("\r", string.Empty)
                .Replace("\t", string.Empty)
                .ToLower()
                ;

            if (!candidate.EndsWith(";"))
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
                Script = string.Format("if (select COUNT(*) from sys.views where name = '{0}') > 0 drop view {0};", view)
            });

            //createScript = ReplaceType(createScript.Replace("\n", string.Empty).Replace("\t", string.Empty).Replace("  ", " ").Trim());
            //createScript = 

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
        void AddTableColumn(DbColumn col, string table)
        {

            m_DataList.Add(new DataItem()
            {
                Object = col.Name,
                Type = "Column",
                Script = ReplaceType(string.Format("ALTER TABLE {1} ADD {0} {2};", col.Name, table, col.Type))
            });

        }
        /// <summary>
        /// 
        /// </summary>
        public class DbColumn
        {
            public DbColumn() { }
            public DbColumn(string col)
            {
                try {
                    string[] elements = col.Split(' ');
                    this.Name = elements[0].Trim();
                    this.Type = elements[1].Trim();

                    if (elements.Length > 2 && elements[2].Trim().StartsWith("IDENTITY"))
                        this.IndentityInfo = elements[2].Trim();

                    this.IsNullable = col.IndexOf("NOT NULL", StringComparison.InvariantCulture) == -1;
                }
                catch(Exception ex)
                {
                    throw new Exception(string.Format("Could not obtain data from: {0}", col), ex);
                }
            }

            public string Type { get; set; }
            public string IndentityInfo { get; set; }
            public string Name { get; set; }
            public bool IsNullable { get; set; }
        }
        /// <summary>
        /// Executes if null.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <returns></returns>
        bool ExecuteIfNull(string script)
        {
            script = script.Replace("IF:", string.Empty).Trim();
            using (Sushi.Mediakiwi.Data.Connection.SqlCommander dac = new Sushi.Mediakiwi.Data.Connection.SqlCommander(ConnectionString))
            {
                try
                {
                    dac.SqlText = script;
                    int count = Wim.Utility.ConvertToInt(dac.ExecScalar());
                    return count < 1;
                }
                catch (Exception)
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Executes if bigger.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <returns></returns>
        bool ExecuteIfBigger(string script)
        {
            script = script.Replace("IF>:", string.Empty).Trim();
            using (Sushi.Mediakiwi.Data.Connection.SqlCommander dac = new Sushi.Mediakiwi.Data.Connection.SqlCommander(ConnectionString))
            {
                try
                {
                    dac.SqlText = script;
                    int count = Wim.Utility.ConvertToInt(dac.ExecScalar());
                    return count > 0;
                }
                catch (Exception)
                {
                    return true;
                }
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
        public void Start()
        {
            AddInfo("Executing queries");

            foreach (DataItem row in m_DataList)
            {
                using (Sushi.Mediakiwi.Data.Connection.SqlCommander dac = new Sushi.Mediakiwi.Data.Connection.SqlCommander(ConnectionString))
                {
                    if (row.ShouldRun)
                    {
                        try
                        {
                            dac.SqlText = row.Script;
                            dac.ExecNonQuery();
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
}
