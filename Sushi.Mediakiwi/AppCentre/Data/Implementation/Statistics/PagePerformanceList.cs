using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Data.Statistics;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation.Statistics
{
    public class PagePerformanceList : ComponentListTemplate
    {
        public PagePerformanceList()
        {
            this.ListSearch += PagePerformanceList_ListSearch;
            wim.HideSearchButton = true;         
        }

        [Sushi.Mediakiwi.Framework.ContentListSearchItem.Date("Van", true, AutoPostBack = true, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public DateTime? Period_From { get; set; }

        [Sushi.Mediakiwi.Framework.ContentListSearchItem.Date("Tot", AutoPostBack = true, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public DateTime? Period_To { get; set; }

        void PagePerformanceList_ListSearch(object sender, ComponentListSearchEventArgs e)
        {
            //  Set the mandatory default
            if (!this.Period_From.HasValue)
                this.Period_From = DateTime.UtcNow.AddDays(-7);

            var report = VisitorClickReport.Select(this.Period_From.Value, this.Period_To.GetValueOrDefault(DateTime.UtcNow.AddDays(2)));
            wim.ListDataApply(report);

            wim.ListDataColumns.Add(new ListDataColumn("URL", "CompleteURL") { });
            wim.ListDataColumns.Add(new ListDataColumn("Min", "Min_Time", ListDataColumnType.ExportOnly ) { Tooltip = "Minimale tijd(ms)" });
            wim.ListDataColumns.Add(new ListDataColumn("Max", "Max_Time", ListDataColumnType.ExportOnly) { Tooltip = "Maximale tijd(ms)" });
            wim.ListDataColumns.Add(new ListDataColumn("Sum", "Total") { Tooltip = "Aantal bezoeken", ColumnWidth = 100 });
            wim.ListDataColumns.Add(new ListDataColumn("Avg", "Avg_Time") { Tooltip = "Gemiddelde laadtijd (ms)", ColumnWidth = 100 });
            wim.ListDataColumns.Add(new ListDataColumn("Eerste", "First_Time", ListDataColumnType.ExportOnly) { });
            wim.ListDataColumns.Add(new ListDataColumn("Laatste", "Last_Time", ListDataColumnType.ExportOnly) {   });
        }

        public class VisitorClickReport 
        {
            static ISqlEntityParser _DataParser;
            static ISqlEntityParser DataParser
            {
                get
                {
                    if (_DataParser == null)
                        _DataParser = Sushi.Mediakiwi.Data.Environment.GetInstance<ISqlEntityParser>();
                    return _DataParser;
                }
            }

            #region Properties
            [DatabaseColumn("VisitorUrl_Name", SqlDbType.NVarChar)]
            public virtual string Url { get; set; }
            [DatabaseColumn("VisitorPage_Name", SqlDbType.NVarChar)]
            public virtual string Page { get; set; }
            [DatabaseColumn("Min_Time", SqlDbType.NVarChar)]
            public virtual int Min_Time { get; set; }
            [DatabaseColumn("Max_Time", SqlDbType.NVarChar)]
            public virtual int Max_Time { get; set; }
            [DatabaseColumn("Avg_Time", SqlDbType.NVarChar)]
            public virtual int Avg_Time { get; set; }
            [DatabaseColumn("Total", SqlDbType.NVarChar)]
            public virtual int Total { get; set; }
            [DatabaseColumn("First_Time", SqlDbType.NVarChar)]
            public virtual DateTime First_Time { get; set; }
            [DatabaseColumn("Last_Time", SqlDbType.NVarChar)]
            public virtual DateTime Last_Time { get; set; }
            #endregion Properties

            public virtual string CompleteURL
            {
                get
                {
                    return string.Concat(this.Url, this.Page);
                }
            }

            internal static List<VisitorClickReport> Select(DateTime start, DateTime end)
            {
                string sqlQuery = @"
    SELECT 
       [VisitorUrl_Name]
    ,	[VisitorPage_Name]
    ,	MIN([VisitorClick_RenderTime]) Min_Time
    ,	MAX([VisitorClick_RenderTime]) Max_Time
    ,   AVG([VisitorClick_RenderTime]) Avg_Time
    ,	COUNT(*) Total
--    ,	SUM(case when [VisitorClick_RenderTime] < 1000 then 1 else 0 end) Second_1000
    ,	MIN([VisitorClick_Created]) First_Time
    ,	MAX([VisitorClick_Created]) Last_Time
    FROM 
	    [dbo].[wim_VisitorClicks]
	    left join [dbo].[wim_VisitorPages] on [VisitorPage_Key] = [VisitorClick_Page_Key]
	    left join [dbo].[wim_VisitorUrls] on [VisitorUrl_Key] = [VisitorPage_Url_Key]
    where 
	    [VisitorClick_RenderTime] is not null 
	    and [VisitorClick_Created] between @START AND @END
	    and not [VisitorUrl_Name] = 'localhost'
    group by 
	    [VisitorUrl_Name], [VisitorPage_Name]
    order by
	    AVG([VisitorClick_RenderTime]) desc
";

                DataRequest request = new DataRequest();
                request.AddParam("START", (start.Ticks < end.Ticks ? start : end), System.Data.SqlDbType.DateTime);
                request.AddParam("END", (start.Ticks < end.Ticks ? end : start), System.Data.SqlDbType.DateTime);
                return DataParser.ExecuteList<VisitorClickReport>(sqlQuery, request);
            }
        }
    }
}
