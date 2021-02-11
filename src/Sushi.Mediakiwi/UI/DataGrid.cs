using System;
using System.Net;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Sushi.Mediakiwi.Utilities;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Logic;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class DataGrid
    {
        /// <summary>
        /// Gets the single item grid from list instance.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="container">The container.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        internal string GetSingleItemGridFromListInstance(Framework.WimComponentListRoot root, Sushi.Mediakiwi.Beta.GeneratedCms.Console container, int type)
        {
            //  Trigger list search event
            container.CurrentListInstance.wim.DoListSearch();

            //  if no data is assigned return null 
            if (root.ListDataTable == null && root.ListData == null)
                return null;

            bool isDataTable = root.ListData == null;

            StringBuilder build = new StringBuilder();

            IEnumerator whilelist;
            if (isDataTable)
                whilelist = root.ListDataTable.Rows.GetEnumerator();
            else
                whilelist = root.ListData.GetEnumerator();

            while (whilelist.MoveNext())
            {
                object item = whilelist.Current;
                

                PropertyInfo[] infoCollection = null;

                if (!isDataTable)
                    infoCollection = item.GetType().GetProperties();

                string uniqueIdentifier, highlightColumn;
                if (isDataTable)
                {
                    uniqueIdentifier = GetIndentifierKey((System.Data.DataRow)item, root.ListDataColumns.List);
                    highlightColumn = GetHighlightedValue((System.Data.DataRow)item, root.ListDataColumns.List).ToString();

                    if (highlightColumn != null)
                        highlightColumn = WebUtility.HtmlEncode(highlightColumn.ToString());
                }
                else
                {
                    uniqueIdentifier = GetIndentifierKey(infoCollection, item, root.ListDataColumns.List);
                    highlightColumn = GetHighlightedValue(infoCollection, item, root.ListDataColumns.List);

                    if (highlightColumn != null)
                        highlightColumn = WebUtility.HtmlEncode(highlightColumn.ToString());
                }
                build.AppendFormat("\n\t<li><a href=\"{1}?list={2}&item={3}\">{0}</a></li>", highlightColumn, container.WimPagePath, container.CurrentList.ID, uniqueIdentifier);

            }

            return string.Format(@"
<div class=""updates"">
	<h2>{0}</h2>{1}
	<ul class=""pager"">
		<li><a href=""{2}?list={3}"" class=""next"">Overview</a></li>
	</ul>
</div>
"
                , container.CurrentListInstance.wim.CurrentList.Name
                , build.Length == 0 ? "" : string.Format("\n\t<ul class=\"links\">{0}\t</ul>", build.ToString())
                , container.WimPagePath
                , container.CurrentList.ID
                );
        }

        internal string GetGridFromListInstanceForXLS(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, IComponentListTemplate listInstance, int type)
        {
            //if (listInstance == null)
            //    listInstance = container.CurrentListInstance;

            //listInstance.wim.ByPassAjaxRequest = true;
            ////  Trigger list search event
            //listInstance.wim.DoListSearch();

            ////  if no data is assigned return null 
            ////if (listInstance.wim.ListDataTable == null && listInstance.wim.ListData == null)
            ////    return null;

            ////New instance of XlsIO is created.[Equivalent to launching MS Excel with no workbooks open].
            ////The instantiation process consists of two steps.

            ////Step 1 : Instantiate the spreadsheet creation engine.
            //ExcelEngine excelEngine = new ExcelEngine();
            ////Step 2 : Instantiate the excel application object.
            //IApplication application = excelEngine.Excel;

            ////A new workbook is created.[Equivalent to creating a new workbook in MS Excel]
            ////The new workbook will have 5 worksheets
            //IWorkbook workbook = excelEngine.Excel.Workbooks.Create(1);
            //workbook.StandardFont = "Arial";

            //workbook.Author = container.CurrentApplicationUser.Displayname;

            ////The first worksheet object in the worksheets collection is accessed.
            //IWorksheet sheet = workbook.Worksheets[0];

            //string columnlist = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,AA,AB,AC,AD,AE,AF,AG,AH,AI,AJ,AK,AL,AM,AN,AO,AP,AQ,AR,AS,AT,AU,AV,AW,AX,AY,AZ,BA,BB,BC,BD,BE,BF,BG,BH,BI,BJ,BK,BL,BM,BN,BO,BP,BQ,BR,BS,BT,BU,BV,BW,BX,BY,BZ,CA,CB,CC,CD,CE,CF,CG,CH,CI,CJ,CK,CL,CM,CN,CO,CP,CQ,CR,CS,CT,CU,CV,CW,CX,CY,CZ";
            //string[] columns = columnlist.Split(',');

            //int currentRow = 1;
            //int currentCol = 0;

            //bool hasExportColumns = listInstance.wim.CurrentList.Data["wim_ExportCol_XLS"].ParseBoolean(true);

            //foreach (Framework.ListDataColumn column in listInstance.wim.ListDataColumns.List)
            //{
            //    if (!IsVisibleExportColumn(column.Type))
            //        continue;

            //    currentCol++;

            //    if (hasExportColumns)
            //        sheet.Range[string.Concat(columns[currentCol - 1], currentRow)].Text = column.ColumnName;
            //}

            //if (!hasExportColumns)
            //    currentRow = 0;

            //int columnCount = currentCol;
            //bool isDataTable = listInstance.wim.ListData == null;

            //IEnumerator whilelist;
            //if (isDataTable)
            //    whilelist = listInstance.wim.ListDataTable.Rows.GetEnumerator();
            //else
            //    whilelist = listInstance.wim.ListData.GetEnumerator();

            //try
            //{
            //    while (whilelist.MoveNext())
            //    {
            //        currentCol = 1;
            //        currentRow++;

            //        object item = whilelist.Current;

            //        PropertyInfo[] infoCollection = null;

            //        if (!isDataTable)
            //            infoCollection = item.GetType().GetProperties();

            //        foreach (Framework.ListDataColumn column in listInstance.wim.ListDataColumns.List)
            //        {
            //            object propertyValue;

            //            if (isDataTable)
            //                propertyValue = ((System.Data.DataRow)item)[column.ColumnValuePropertyName];
            //            else
            //                propertyValue = GetValue(infoCollection, item, column);

            //            if (!IsVisibleExportColumn(column.Type))
            //                continue;

            //            if (propertyValue == null)
            //            {
            //                currentCol++;
            //                continue;
            //            }

            //            System.Type propType = propertyValue.GetType();
            //            //propertyValue = OutputValue(container, propertyValue, false);

            //            if (propType == typeof(String))
            //            {
            //                string itemvalue = Utility.CleanFormatting(propertyValue.ToString());
            //                if (!string.IsNullOrEmpty(itemvalue))
            //                {
            //                    if (!column.WhenExportingTreatAsText && Utility.IsNumeric(itemvalue))
            //                        sheet.Range[string.Concat(columns[currentCol - 1], currentRow)].Value2 = itemvalue;
            //                    else
            //                        sheet.Range[string.Concat(columns[currentCol - 1], currentRow)].Text = itemvalue;
            //                }

            //            }
            //            else if (propType == typeof(DateTime))
            //            {
            //                DateTime dt = (DateTime)propertyValue;
            //                if (dt != DateTime.MinValue)
            //                {
            //                    sheet.Range[string.Concat(columns[currentCol - 1], currentRow)].DateTime = dt;
            //                }
            //            }
            //            else
            //            {
            //                sheet.Range[string.Concat(columns[currentCol - 1], currentRow)].Value2 = propertyValue;
            //            }
            //            currentCol++;
            //        }

            //    }
            //}
            //catch (Exception ex)
            //{
            //    string error = ex.Message;
            //}

            //currentCol = 0;
            //foreach (Framework.ListDataColumn column in listInstance.wim.ListDataColumns.List)
            //{
            //    if (!IsVisibleExportColumn(column.Type))
            //        continue;

            //    currentCol++;

            //    if (hasExportColumns)
            //    {
            //        if (column.Alignment == Align.Right)
            //            sheet.Range[string.Concat(columns[currentCol - 1], "2:", columns[currentCol - 1], currentRow)].CellStyle.HorizontalAlignment
            //                = ExcelHAlign.HAlignRight;
            //        else if (column.Alignment == Align.Left)
            //            sheet.Range[string.Concat(columns[currentCol - 1], "2:", columns[currentCol - 1], currentRow)].CellStyle.HorizontalAlignment
            //                = ExcelHAlign.HAlignLeft;
            //        else if (column.Alignment == Align.Center)
            //            sheet.Range[string.Concat(columns[currentCol - 1], "2:", columns[currentCol - 1], currentRow)].CellStyle.HorizontalAlignment
            //                = ExcelHAlign.HAlignCenter;
            //    }
            //}

            //if (hasExportColumns)
            //{
            //    sheet.Range[string.Concat("A1:", columns[columnCount - 1], 1)].CellStyle.Font.Bold = true;
            //    sheet.AutoFilters.FilterRange = sheet.Range[string.Concat("A1:", columns[columnCount - 1], currentRow)];
            //}

            //sheet.Range[string.Concat("A1:", columns[columnCount - 1], 1)].AutofitColumns();

            //foreach (IRange range in sheet.Columns)
            //{
            //    range.ColumnWidth = range.ColumnWidth + 1;
            //    if (range.ColumnWidth > 100)
            //    {
            //        sheet.Range[string.Concat(columns[range.Column - 1], "1")].ColumnWidth = 100;
            //    }

            //    if (hasExportColumns)
            //        sheet.Range[string.Concat(columns[range.Column - 1], "1:", columns[range.Column - 1], currentRow)].CellStyle.WrapText = true;
            //}

            //sheet.Range[string.Concat("A1:", columns[columnCount - 1], currentRow)].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;

            ////Saving the workbook to disk.
            //string filename = string.Format("{0}_{1}.xls", container.CurrentList.Name.Replace(" ", "_"), DateTime.Now.ToString("yyyyMMddmmHH"));

            //string path = listInstance.wim.GetTemporaryFilePath(filename, true, out filename);

            //workbook.Version = ExcelVersion.Excel97to2003;

            ////container.Response..Clear();
            //workbook.SaveAs(container.Response.Body);
            //container.Response.Headers.Add("Content-Disposition", string.Format("attachment;filename=\"{0}\"", filename));
          
            //excelEngine.Dispose();
            ////container.Response...End();
            //string url = Utility.AddApplicationPath(string.Concat(Wim.CommonConfiguration.RelativeRepositoryTmpUrl, filename));

            //return url;
            return null;
        }

        /// <summary>
        /// Gets the grid from list instance.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="container">The container.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public string GetGridFromListInstance(Framework.WimComponentListRoot root, Sushi.Mediakiwi.Beta.GeneratedCms.Console container, int type, bool isNewDesignOutput)
        {
            return GetGridFromListInstance(root, container, type, false, isNewDesignOutput);
        }

        string GetListPaging(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, Splitlist splitlist, int currentPage, bool isTop)
        {
            return GetListPaging(container, splitlist, currentPage, false, isTop);
        }

        List<Dictionary<string, object>> GetListPagingForJSON(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, Utilities.Splitlist splitlist, int currentPage)
        {
            var pager = new List<Dictionary<string, object>>();
            Dictionary<string, object> item = null;

            //item = new Dictionary<string, object>();
            //item.Add("cn", "prev");
            //item.Add("pg", null);
            //item.Add("tx", null);
            //pager.Add(item);
            
            StringBuilder paging = new StringBuilder();

            bool knockout = false;

            //if (root.m_IsLinqUsed)
            //    count = root.m_ListDataRecordCount - root.m_ListDataInterLineCount;

            //if (visibleColumnCount == 1)
            //{

            paging.Append("\n\t\t\t\t\t\t<div class=\"footer\">");
            paging.Append("\n\t\t\t\t\t\t\t<ul>");
            paging.Append("\n\t\t\t\t\t\t\t\t<li class=\"first\">");

            //paging.Append("&nbsp;</li>");
            paging.Append("\n\t\t\t\t\t\t\t\t\t<img class=\"candy\" alt=\"\" src=\"testdrive/files/lines.png\">");
            //paging.Append("\n\t\t\t\t\t\t\t\t\t<label for=\"frmRemove\">Selectie</label>");
            paging.Append("\n\t\t\t\t\t\t\t\t\t<select id=\"frmRemove\" name=\"frmRemove\">");
            paging.Append("\n\t\t\t\t\t\t\t\t\t\t<option>Selection..</option>");
            paging.Append("\n\t\t\t\t\t\t\t\t\t</select>");
            paging.Append("\n\t\t\t\t\t\t\t\t</li>");
            paging.Append("\n\t\t\t\t\t\t\t\t<li class=\"last\">");

            //paging.Append("&nbsp;</li>");
            //paging.Append("<label for=\"frmRemove\"></label>");
            //paging.Append("<select id=\"Select9\" name=\"frmRemove\">");
            //paging.Append("<option>Export to..</option>");
            //paging.Append("</select>");
            paging.Append("\n\t\t\t\t\t\t\t\t</li>");


            paging.Append("\n\t\t\t\t\t\t\t\t<li class=\"laster\">");

            if (container.CurrentList.Option_HasShowAll)
            {
                paging.AppendFormat("\n\t\t\t\t\t\t\t\t\t<a class=\"reset right\" href=\"{1}\">Toon Alles ({0})</a>"
                    , 0
                    , BuildUrl(container, "set", "all")
                );
            }
            paging.Append("\n\t\t\t\t\t\t\t\t</li>");

            if (!container.CurrentListInstance.wim.HidePaging)
            {
                int listCount = container.CurrentListInstance.wim.m_IsLinqUsed ?
                    container.CurrentListInstance.wim.m_ListDataRecordPageCount : (splitlist == null ? 0 : splitlist.ListCount);

                bool showPageNumbers = true;//= !((splitlist == null && !container.CurrentListInstance.wim.m_IsLinqUsed) || listCount < 2);
                if (showPageNumbers)
                {
                    paging.Append("\n\t\t\t\t\t\t\t\t<li class=\"last\">");

                    if (knockout)
                    {
                        //paging.Append("\n\t\t\t\t\t\t\t\t\t<ul data-bind=\"template:{foreach:pages}\">");
                        //paging.Append("\n\t\t\t\t\t\t\t\t\t\t<li data-bind=\"attr:{class:cn}\"><a class=\"pager\" data-bind=\"html:tx, attr:{href:pg}\"></a></li>");

                        paging.Append("\n\t\t\t\t\t\t\t\t\t<ul>");
                        paging.AppendFormat("\n\t\t\t\t\t\t\t\t\t\t<li><a href=\"\">1</a></li>");
                        paging.AppendFormat("\n\t\t\t\t\t\t\t\t\t\t<li><input id=\"set\" class=\"numeric{1}\" name=\"set\" type=\"text\" value=\"{0}\"></li>", container.ListPagingValue, knockout ? " async" : null);
                        paging.AppendFormat("\n\t\t\t\t\t\t\t\t\t\t<li><a href=\"\">{0}</a></li>", listCount);
                        paging.AppendFormat("\n\t\t\t\t\t\t\t\t\t\t<li class=\"prev\"><a href=\"\"><</a></li>");
                        paging.AppendFormat("\n\t\t\t\t\t\t\t\t\t\t<li class=\"next\"><a href=\"\">></a></li>");
                        paging.Append("\n\t\t\t\t\t\t\t\t\t</ul>");
                    }
                    else
                    {
                        paging.Append("\n\t\t\t\t\t\t\t\t\t<ul>");
                 

                        int maxPageReferenceCount = 10;

                        //  Calculate boundaries
                        Decimal surroundingCandidateCount = Decimal.Floor(Decimal.Divide((maxPageReferenceCount - 1), 2));

                        int runtoPageReference = currentPage + (int)surroundingCandidateCount;
                        int startPageReference = (currentPage - (int)surroundingCandidateCount);
                        //  Calculate left boundary
                        if (startPageReference < 1)
                        {
                            runtoPageReference = runtoPageReference - startPageReference + 1;
                            startPageReference = 1;
                        }
                        //  Calculate right boundary
                        if (runtoPageReference > listCount)
                        {
                            int overhead = runtoPageReference - listCount;
                            runtoPageReference = listCount;
                            while (startPageReference > 1)
                            {
                                overhead--;
                                startPageReference--;
                                if (overhead < 1) break;
                            }
                        }

                        paging.AppendFormat("\n\t\t\t\t\t\t\t\t\t\t<li><a href=\"\">1</a></li>");
                        paging.AppendFormat("\n\t\t\t\t\t\t\t\t\t\t<li><input id=\"set\" name=\"set\" type=\"text\"{1} value=\"{0}\"></li>", container.ListPagingValue, knockout ? " class=\"async\"" : null);
                        paging.AppendFormat("\n\t\t\t\t\t\t\t\t\t\t<li><a href=\"\">{0}</a></li>", listCount);
                        paging.AppendFormat("\n\t\t\t\t\t\t\t\t\t\t<li class=\"prev\"><a href=\"\"><</a></li>");
                        paging.AppendFormat("\n\t\t\t\t\t\t\t\t\t\t<li class=\"next\"><a href=\"\">></a></li>");
                        paging.Append("\n\t\t\t\t\t\t\t\t\t</ul>");

                        //  Create all page reference entries
                        //int itemCount = 0;

                        //pager.Add(GetPageLinkForJSON(container, false, "< Back", (currentPage - 1), "prev"));
                        //pager.Add(GetPageLinkForJSON(container, false, "Next >", (currentPage != listCount) ? (currentPage + 1) : 0, "next")); 

                        //GetPageLink(container, ref paging, "< Back", (currentPage - 1), "prev", (currentPage != 1));
                        //GetPageLink(container, ref paging, "Next >", (currentPage + 1), "next", (currentPage != listCount));

                        //while (itemCount < maxPageReferenceCount)
                        //{
                        //    itemCount++;
                        //    if (itemCount == 1)
                        //    {
                        //        pager.Add(GetPageLinkForJSON(container, (currentPage == 1), startPageReference.ToString(), startPageReference, null));
                        //        GetPageLink(container, ref paging, currentPage == 1, startPageReference);
                        //    }


                        //    else if (itemCount == maxPageReferenceCount || startPageReference == listCount)
                        //    {
                        //        pager.Add(GetPageLinkForJSON(container, (currentPage == startPageReference), startPageReference.ToString(), startPageReference, null));
                        //        GetPageLink(container, ref paging, currentPage == startPageReference, startPageReference);
                        //        break;
                        //    }
                        //    else
                        //    {
                        //        pager.Add(GetPageLinkForJSON(container, (currentPage == startPageReference), startPageReference.ToString(), startPageReference, null));
                        //        GetPageLink(container, ref paging, currentPage == startPageReference, startPageReference);
                        //    }
                        //    startPageReference++;
                        //}
                    }
         
                }
            }
            return pager;
        }

        string GetUri(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, int page)
        {
            return container.CurrentListInstance.wim.GetUrl(new KeyValue() { Key = "set", Value = page.ToString() });
        }

        string GetListPaging(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, Utilities.Splitlist splitlist, int currentPage, bool knockout, bool isTop)
        {
            if (container.CurrentListInstance.wim.Page.Body.Grid.HidePager) 
                return null;

            bool isFormatRequest_AJAX = container.Form(Constants.AJAX_PARAM) == "1";

            StringBuilder paging = new StringBuilder();
            
            int maxList = splitlist == null ? container.CurrentListInstance.wim.m_ListDataRecordPageCount : splitlist.ListCount;
            int maxItem = splitlist == null ? container.CurrentListInstance.wim.m_ListDataRecordCount : splitlist.ItemCount;

            if (false)//!container.CurrentApplicationUser.ShowNewDesign2)
            {
                paging.Append("\n\t\t\t\t\t\t<div class=\"footer\">");
                paging.Append("\n\t\t\t\t\t\t\t<ul>");
                if (isTop)
                {
                    #region Sortorder
                    if (!string.IsNullOrEmpty(container.CurrentListInstance.wim.m_sortOrderSqlTable))
                    {
                        //bool ignoreSort = false;
                        //if (!isListMode && !container.CurrentListInstance.wim.HasSingleItemSortOrder)
                        //    ignoreSort = true;

                        if (container.CurrentListInstance.wim.HasSortOrder)// && !ignoreSort)
                        {
                            paging.Append("\n\t\t\t\t\t\t\t\t<li class=\"first\"><a class=\"flaticon solid repeat-4 icon tiny sortOrder\" href=\"#\"></a></li>");
                        }
                    }
                    #endregion
                }

                paging.Append("\n\t\t\t\t\t\t\t\t\t<li class=\"last\">&nbsp;");
                //  Paging at the bottom
                if (isTop)
                {
                    if (container.CurrentListInstance.wim.HasExportOptionXLS)
                    {
                        paging.Append("\n\t\t\t\t\t\t\t\t\t<li class=\"last\">");
                        paging.Append("\n\t\t\t\t\t\t\t\t\t\t<div class=\"flaticon solid download\">");
                        paging.Append("\n\t\t\t\t\t\t\t\t\t\t\t<div class=\"hoverMenu\">");
                        paging.Append("\n\t\t\t\t\t\t\t\t\t\t\t\t<span class=\"flaticon solid download\"></span>");
                        paging.Append("\n\t\t\t\t\t\t\t\t\t\t\t\t<h3>Export</h3>");
                        paging.Append("\n\t\t\t\t\t\t\t\t\t\t\t\t<a href=\"#\" id=\"export_xls\" class=\"postBack nosync\">Excel</a>");
                        //paging.Append("\n\t\t\t\t\t\t\t\t\t\t\t\t<a href=\"#\" id=\"export_pdf\" class=\"postBack nosync\">PDF</a>");
                        paging.Append("\n\t\t\t\t\t\t\t\t\t\t\t</div>");
                        paging.Append("\n\t\t\t\t\t\t\t\t\t\t</div>");
                    }
                }
                paging.Append("\n\t\t\t\t\t\t\t\t\t</li>");

                if (maxList > 1)
                {
                    paging.Append("\n\t\t\t\t\t\t\t\t<li class=\"laster\">");

                    paging.Append("\n\t\t\t\t\t\t\t\t\t<ul>");
                    paging.AppendFormat("\n\t\t\t\t\t\t\t\t\t\t<li><a href=\"{0}\"{1}>1</a></li>", GetUri(container, 1), isFormatRequest_AJAX ? " class=\"async\"" : string.Empty);
                    paging.AppendFormat("\n\t\t\t\t\t\t\t\t\t\t<li><input id=\"set\" name=\"set\" type=\"text\" class=\"numeric postBack\" value=\"{0}\"></li>", currentPage);
                    paging.AppendFormat("\n\t\t\t\t\t\t\t\t\t\t<li><a href=\"{1}\"{2}>{0}</a></li>", maxList, GetUri(container, maxList), isFormatRequest_AJAX ? " class=\"async\"" : string.Empty);

                    int pageP = currentPage <= 1 ? 1 : currentPage - 1;
                    paging.AppendFormat("\n\t\t\t\t\t\t\t\t\t\t<li class=\"prev\"><a href=\"{0}\"{1}><</a></li>", GetUri(container, pageP), isFormatRequest_AJAX ? " class=\"async\"" : string.Empty);
                    int pageN = currentPage >= maxList ? maxList : currentPage + 1;
                    paging.AppendFormat("\n\t\t\t\t\t\t\t\t\t\t<li class=\"next\"><a href=\"{0}\"{1}>></a></li>", GetUri(container, pageN), isFormatRequest_AJAX ? " class=\"async\"" : string.Empty);
                    paging.Append("\n\t\t\t\t\t\t\t\t\t</ul>");

                    paging.Append("\n\t\t\t\t\t\t\t\t</li>");

                    paging.Append("\n\t\t\t\t\t\t\t\t</li>");
                }

                paging.Append("\n\t\t\t\t\t\t\t</ul>");
                paging.Append("\n\t\t\t\t\t\t</div>");
            }
            else
            {
                if (!isTop)
                {
                    bool isShowAll = container.Request.Query["set"] == "all";
                    paging.Append("\n\t\t\t\t\t\t<menu class=\"pager\">");
                    if (maxList > 1)
                    {
                        paging.Append("\n\t\t\t\t\t\t\t<li class=\"first\">");
                        if (container.CurrentListInstance.wim.CurrentList.Option_HasShowAll)
                        {
                            if (isShowAll)
                                paging.AppendFormat("\n\t\t\t\t\t\t\t\t<a href=\"{0}\" class=\"back{1}\"><span class=\"fa icon-list-ul\"></span>Reset</a>", BuildUrl(container, "set", "1"), isFormatRequest_AJAX ? " async" : string.Empty);
                            else
                                paging.AppendFormat("\n\t\t\t\t\t\t\t\t<a href=\"{0}\" class=\"back{1}\"><span class=\"fa icon-list-ul\"></span>Show All</a>", BuildUrl(container, "set", "all"), isFormatRequest_AJAX ? " async" : string.Empty);
                        }
                        paging.Append("\n\t\t\t\t\t\t\t</li>");
                    }

                    int results = container.CurrentListInstance.wim.m_ListDataRecordCount;
                    if (results == 0)
                        results = container.CurrentListInstance.wim.ListData.Count;

                    int pagesize = container.CurrentListInstance.wim.CurrentList.Option_Search_MaxResultPerPage;
                    int currsize = (pagesize * currentPage);
                    int actlsize = currsize - pagesize;
                    if (currsize > results)
                        currsize = results;

                    if (isShowAll || maxList == 1)
                    {
                        paging.Append("\n\t\t\t\t\t\t\t<li>");
                        paging.AppendFormat("\n\t\t\t\t\t\t\t\t{0} results", results);
                        paging.Append("\n\t\t\t\t\t\t\t</li>");
                    }
                    else
                    {
                        paging.Append("\n\t\t\t\t\t\t\t<li>");
                        paging.AppendFormat("\n\t\t\t\t\t\t\t\t<strong>{0} to {1}</strong> of {2} results", actlsize + 1, currsize, results);
                        paging.Append("\n\t\t\t\t\t\t\t</li>");

                        if (maxList > 1)
                        {
                            paging.Append("\n\t\t\t\t\t\t\t<li>");
                            paging.Append("\n\t\t\t\t\t\t\t\t<ul>");
                            paging.Append("\n\t\t\t\t\t\t\t\t\t<li>");

                            //paging.AppendFormat("\n\t\t\t\t\t\t<li class=\"prev\"><a class=\"flaticon icon-arrow-left-04{1}\" href=\"{0}\"></a></li>", GetUri(container, 1), isFormatRequest_AJAX ? " async" : string.Empty);
                            if (currentPage == 1)
                            {
                                paging.AppendFormat("\n\t\t\t\t\t\t<li><a class=\"active{2}\" href=\"{1}\">{0}</a></li>", currentPage, GetUri(container, currentPage), isFormatRequest_AJAX ? " async" : string.Empty);
                                paging.AppendFormat("\n\t\t\t\t\t\t<li><a href=\"{1}\"{2}>{0}</a></li>", currentPage + 1, GetUri(container, currentPage + 1), isFormatRequest_AJAX ? " class=\"async\"" : string.Empty);

                                if (maxList > 2)
                                    paging.AppendFormat("\n\t\t\t\t\t\t<li><a href=\"{1}\"{2}>{0}</a></li>", currentPage + 2, GetUri(container, currentPage + 2), isFormatRequest_AJAX ? " class=\"async\"" : string.Empty);
                            }
                            else
                            {
                                int prev = currentPage - 1;
                                if (prev > 1)
                                {
                                    paging.AppendFormat("\n\t\t\t\t\t\t<li class=\"prevAll\"><a class=\"fa icon-angle-double-left{1}\" href=\"{0}\"></a></li>", GetUri(container, 1), isFormatRequest_AJAX ? " async" : string.Empty);
                                    paging.AppendFormat("\n\t\t\t\t\t\t<li class=\"prev\"><a class=\"fa icon-angle-left{1}\" href=\"{0}\"></a></li>", GetUri(container, prev), isFormatRequest_AJAX ? " async" : string.Empty);
                                }

                                if (maxList == (currentPage) && currentPage != 2)
                                    paging.AppendFormat("\n\t\t\t\t\t\t<li><a href=\"{1}\"{2}>{0}</a></li>", currentPage - 2, GetUri(container, currentPage - 2), isFormatRequest_AJAX ? " class=\"async\"" : string.Empty);

                                paging.AppendFormat("\n\t\t\t\t\t\t<li><a href=\"{1}\"{2}>{0}</a></li>", currentPage - 1, GetUri(container, currentPage - 1), isFormatRequest_AJAX ? " class=\"async\"" : string.Empty);
                                paging.AppendFormat("\n\t\t\t\t\t\t<li><a class=\"active{2}\" href=\"{1}\">{0}</a></li>", currentPage, GetUri(container, currentPage), isFormatRequest_AJAX ? " async" : string.Empty);
                                if (maxList > (currentPage))
                                    paging.AppendFormat("\n\t\t\t\t\t\t<li><a href=\"{1}\"{2}>{0}</a></li>", currentPage + 1, GetUri(container, currentPage + 1), isFormatRequest_AJAX ? " class=\"async\"" : string.Empty);
                            }

                            if (currentPage < (maxList - 1))
                            {
                                paging.AppendFormat("\n\t\t\t\t\t\t<li class=\"next\"><a class=\"fa icon-angle-right{1}\" href=\"{0}\"></a></li>", GetUri(container, (currentPage + 1)), isFormatRequest_AJAX ? " async" : string.Empty);
                                paging.AppendFormat("\n\t\t\t\t\t\t<li class=\"nextAll\"><a class=\"fa icon-angle-double-right{1}\" href=\"{0}\"></a></li>", GetUri(container, maxList), isFormatRequest_AJAX ? " async" : string.Empty);
                            }
                            paging.Append("\n\t\t\t\t\t\t\t\t\t</li>");
                            paging.Append("\n\t\t\t\t\t\t\t\t<ul>");
                            paging.Append("\n\t\t\t\t\t\t\t</li>");

                            //paging.Append("\n\t\t\t\t\t\t\t\t<li class=\"laster\">");

                            //paging.Append("\n\t\t\t\t\t\t\t\t\t<ul>");
                            //paging.AppendFormat("\n\t\t\t\t\t\t\t\t\t\t<li><a href=\"{0}\"{1}>1</a></li>", GetUri(container, 1), isFormatRequest_AJAX ? " class=\"async\"" : string.Empty);
                            //paging.AppendFormat("\n\t\t\t\t\t\t\t\t\t\t<li><input id=\"set\" name=\"set\" type=\"text\"class=\"numeric postBack\" value=\"{0}\"></li>", currentPage);
                            //paging.AppendFormat("\n\t\t\t\t\t\t\t\t\t\t<li><a href=\"{1}\"{2}>{0}</a></li>", maxList, GetUri(container, maxList), isFormatRequest_AJAX ? " class=\"async\"" : string.Empty);

                            //int pageP = currentPage <= 1 ? 1 : currentPage - 1;
                            //paging.AppendFormat("\n\t\t\t\t\t\t\t\t\t\t<li class=\"prev\"><a href=\"{0}\"{1}><</a></li>", GetUri(container, pageP), isFormatRequest_AJAX ? " class=\"async\"" : string.Empty);
                            //int pageN = currentPage >= maxList ? maxList : currentPage + 1;
                            //paging.AppendFormat("\n\t\t\t\t\t\t\t\t\t\t<li class=\"next\"><a href=\"{0}\"{1}>></a></li>", GetUri(container, pageN), isFormatRequest_AJAX ? " class=\"async\"" : string.Empty);
                            //paging.Append("\n\t\t\t\t\t\t\t\t\t</ul>");

                            //paging.Append("\n\t\t\t\t\t\t\t\t</li>");

                            //paging.Append("\n\t\t\t\t\t\t\t\t</li>");
                        }
                    }
                    paging.Append("\n\t\t\t\t\t\t</menu>");
                    paging.Append("\n\t\t\t\t\t\t<br class=\"clear\">");
                }
            }
           
            //paging.Append("\n\t\t\t\t\t\t<br class=\"clear\">");
    
            return paging.ToString();
        }

        Dictionary<string, object> GetPageLinkForJSON(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, bool isSelected, string text, int page, string additionalClass)
        {
            var item = new Dictionary<string, object>();
            if (page == 0)
            {
                item.Add("cn", additionalClass);
                item.Add("pg", "#");
                item.Add("tx", null);
            }
            else
            {
                item.Add("cn", isSelected ? "active" : additionalClass);
                item.Add("pg", string.Concat("#", page));
                item.Add("tx", text);
            }
            return item;
        }

        void GetPageLink(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, ref StringBuilder paging, bool isSelected, int page)
        {
            if (isSelected)
            {
                paging.AppendFormat("\n\t\t\t\t\t\t\t\t\t\t<li><a class=\"active\">{0}</a></li>", page);
                return;
            }
            paging.AppendFormat("\n\t\t\t\t\t\t\t\t\t\t<li><a href=\"{1}\">{0}</a></li>", page, BuildUrl(container, "set", page.ToString()));
        }

        void GetPageLink(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, ref StringBuilder paging, string text, int page, string additionalClass, bool isEnabled)
        {
            if (!isEnabled)
            {
                paging.AppendFormat("\n\t\t\t\t\t\t\t\t\t\t<li class=\"{0}\">&nbsp;</li>", additionalClass);
                return;
            }
            if (page == 1)
                paging.AppendFormat("\n\t\t\t\t\t\t\t\t\t\t<li class=\"{0}\"><a href=\"{2}\">{1}</a></li>", additionalClass, text, BuildUrl(container, "set", null));
            else
                paging.AppendFormat("\n\t\t\t\t\t\t\t\t\t\t<li class=\"{0}\"><a href=\"{2}\">{1}</a></li>", additionalClass, text, BuildUrl(container, "set", page.ToString()));
        }

        string BuildUrl(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, string queryStringPropertyToApply, string value)
        {
            string url = container.WimPagePath;

            bool first = true, foundKey = false;
            if (container.Request.Query.Count > 0)
            {
                var keys = container.Request.Query.Keys;
                string candidate = null;
                foreach (string key in keys)
                {
                    if (key == "channel") continue;
                    if (key == "axt") continue;

                    if (key == queryStringPropertyToApply)
                    {
                        foundKey = true;
                        candidate = value;
                    }
                    else
                        candidate = container.Request.Query[key];

                    if (!string.IsNullOrEmpty(candidate))
                    {
                        if (first)
                            url += string.Concat("?", key, "=", candidate);
                        else
                            url += string.Concat("&", key, "=", candidate);

                        first = false;
                    }
                }
            }

            if (!foundKey)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (first)
                        url += string.Concat("?", queryStringPropertyToApply, "=", value);
                    else
                        url += string.Concat("&", queryStringPropertyToApply, "=", value);
                }
            }

            return url;
        }

        /// <summary>
        /// Applies the total information.
        /// </summary>
        /// <param name="isDataTable">if set to <c>true</c> [is data table].</param>
        /// <param name="root">The root.</param>
        bool ApplyTotalInformation(bool isDataTable, Framework.WimComponentListRoot root)
        {
            this.ResetTotalInformation(root);

            bool hasTotal = false;
            IEnumerator whilelist;
            if (isDataTable)
                whilelist = root.ListDataTable.Rows.GetEnumerator();
            else
                whilelist = root.ListData.GetEnumerator();

            while (whilelist.MoveNext())
            {
                object item = whilelist.Current;
                if (item == null)
                    continue;


                PropertyInfo[] infoCollection = null;

                if (!isDataTable)
                    infoCollection = item.GetType().GetProperties();

                foreach (Framework.ListDataColumn column in root.ListDataColumns.List)
                {
                    object propertyValue;

                    if (isDataTable)
                        propertyValue = ((System.Data.DataRow)item)[column.ColumnValuePropertyName];
                    else
                        propertyValue = GetValue(infoCollection, item, column);

                    if (column.Total == Sushi.Mediakiwi.Framework.ListDataTotalType.Sum || column.Total == Sushi.Mediakiwi.Framework.ListDataTotalType.Average)
                        hasTotal = true;

                    if (propertyValue != null)
                    {
                        if (propertyValue.GetType() == typeof(Decimal) || propertyValue.GetType() == typeof(int))
                        {
                            
                            if (column.Total == Sushi.Mediakiwi.Framework.ListDataTotalType.Sum || column.Total == Sushi.Mediakiwi.Framework.ListDataTotalType.Average)
                            {
                                column.TotalValueType = propertyValue.GetType();
                                column.TotalValue += Utility.ConvertToDecimal(propertyValue);
                            }
                        }
                    }
                }
                if (!hasTotal) break;
            }
            return hasTotal;
        }

        void ResetTotalInformation(Framework.WimComponentListRoot root)
        {
            foreach (Framework.ListDataColumn column in root.ListDataColumns.List)
                column.TotalValue = 0;
        }

        internal bool _ReturnKNOCKOUT = false;

        /// <summary>
        /// Gets the grid from list instance.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="container">The container.</param>
        /// <param name="type">The type. (0 = normal, 1 = popuplayer, 2 = popuplayer without paging, 3 = dashboard[middle] )</param>
        /// <param name="includeExportFields">if set to <c>true</c> [include export fields].</param>
        /// <param name="isNewDesignOutput">if set to <c>true</c> [is new design output].</param>
        /// <returns></returns>
        internal string GetGridFromListInstance(Framework.WimComponentListRoot root, Sushi.Mediakiwi.Beta.GeneratedCms.Console container, int type, bool includeExportFields, bool isNewDesignOutput, bool hidePaging = false)
        {

            container.ListPagingValue = root.CurrentPage.ToString();// container.Request.Params["set"];
            bool isEditMode = false;

            //  Trigger list search event
            container.CurrentListInstance.wim.DoListSearch();
            
            //if (container.CurrentListInstance.wim.Grid.IsAsyncRequestTemplateRequired)
            //    return GetGridFromListInstanceForKnockout(root, container, type, includeExportFields, isNewDesignOutput, true);

            //  if no data is assigned return null 
            if (root.ListDataTable == null && root.ListData == null)
                return null;

            StringBuilder build = new StringBuilder();

            //  Create the header and the columns
            //if (type != 4)
                //build.Append("\n\t\t\t\t\t\t\t<thead>\n\t\t\t\t\t\t\t\t<tr class=\"first\">");
            int visibleColumnCount = 0;

            bool hasEditProperties = false;
            bool isDataTable = root.ListData == null;

            int count = isDataTable ? root.ListDataTable.Rows.Count : root.ListData.Count;

            //  Create the content
            string className = null;

            IEnumerator whilelist = null;

            int currentPage = Utility.ConvertToInt(container.ListPagingValue, 1);
            if (currentPage < 1) currentPage = 1;
            //  When hiding paging, we still need the margin.
            string paging = hidePaging ? "<menu class=\"pager\"></menu>" : null;
            string pagingTop = null;

            if (root.m_IsLinqUsed)
            {
                count = root.m_ListDataRecordCount - root.m_ListDataInterLineCount;
                whilelist = root.ListData.GetEnumerator();

                if (!root.IsDashboardMode && !hidePaging)
                {
                    paging = GetListPaging(container, null, currentPage, false);
                    pagingTop = GetListPaging(container, null, currentPage, true);
                }
            }
            else
            {
                Utilities.Splitlist splitlist = null;

                int maxViewItemCount = root.IsDashboardMode ? root.SearchViewDashboardMaxLength : root.CurrentList.Option_Search_MaxResultPerPage;
                if (container.ListPagingValue == "-1")
                    maxViewItemCount = 1000;

                if (isDataTable)
                {
                    if (root.ListDataTable.Rows.Count > 0)
                    {
                        splitlist = new Splitlist(root.ListDataTable, maxViewItemCount, root.CurrentList.Option_Search_MaxViews);
                        if (currentPage > splitlist.ListCount)
                            currentPage = splitlist.ListCount;

                        whilelist = ((System.Data.DataTable)splitlist[currentPage - 1]).Rows.GetEnumerator();
                    }
                }
                else
                {
                    if (root.ListData.Count > 0)
                    {
                        splitlist = new Splitlist(root.ListData, maxViewItemCount, root.CurrentList.Option_Search_MaxViews);
                        if (currentPage > splitlist.ListCount)
                            currentPage = splitlist.ListCount;

                        whilelist = ((IList)splitlist[currentPage - 1]).GetEnumerator();
                    }
                }
                if (!root.IsDashboardMode && !hidePaging)
                { 
                    paging = GetListPaging(container, splitlist, currentPage, false);
                    pagingTop = GetListPaging(container, splitlist, currentPage, true);
                }
            }


            StringBuilder build2 = new StringBuilder();
            StringBuilder RowHTML = null;

            #region Scrolling
            bool hasScroll = root.m_IsListDataScrollable;
            string tableClass = null;
            string scrolClass = null;
            if (true)//container.CurrentApplicationUser.ShowNewDesign2)
            {
                tableClass = string.Format(" class=\"{0}\"", root.Page.Body.Grid.Table.ClassName);
                if (hasScroll)
                {
                    int w = 0;
                    root.ListDataColumns.List.ForEach(col =>
                    {
                        bool isHidden = (
                            col.Type == ListDataColumnType.ExportOnly 
                            || col.Type == ListDataColumnType.Highlight 
                            || col.Type == ListDataColumnType.UniqueIdentifier 
                            || col.Type == ListDataColumnType.UniqueHighlightedIdentifier);

                        if (col.ColumnWidth == 0 && !isHidden)
                            throw new Exception(string.Format("When applying scrolling all columns should have a fixed with. This is not set for '{0}'", col.ColumnValuePropertyName));

                        if (col.ColumnIsFixed && !isHidden)
                        {
                            col.ColumnFixedLeftMargin = w;
                            w += col.ColumnWidth;
                        }
                    });

                    if (w > 0)
                        scrolClass = string.Format("\n\t\t\t\t\t\t<div class=\"outer\"><div class=\"scroll inner\" style=\"margin-left: {0}px !important\">", w);
                    else
                        scrolClass = "\n\t\t\t\t\t\t<div class=\"outer\"><div class=\"scroll inner\">";
                }
            }
            #endregion Scrolling

            bool hasTotal = ApplyTotalInformation(isDataTable, root);
            if (whilelist == null)
            {
                return null;
            }
            else
            {
                ListDataSoure source = new ListDataSoure();
                foreach (Framework.ListDataColumn c in root.ListDataColumns.List)
                {
                    if (c.EditConfiguration != null)
                        hasEditProperties = true;


                    if (!IsVisibleColumn(c.Type, includeExportFields))
                        continue;

                    //if (c.ColumnWidth == 0)
                    //{
                    //    if (info.PropertyType == typeof(DateTime) || info.PropertyType == typeof(DateTime?))
                    //    {
                    //        c.ColumnWidth = 90;
                    //    }

                    //    //if (info.GetType() == typeof(Decimal) || info.GetType() == typeof(int))
                    //    //{
                    //    //}
                    //    //if (info.GetType() == typeof(Boolean))
                    //    //{

                    //    //}
                    //}

                    visibleColumnCount++;
                    //build.AppendFormat("\n\t\t\t\t\t\t\t\t\t<th{1}>{0}</th>", c.ColumnName, (c.ColumnWidth == 0 ? "" : string.Format(" width=\"{0}\"", c.ColumnWidth)));

                    source.DataEntities = whilelist;
                    source.VisibleColumns = visibleColumnCount;
                }
                
                int index = -1;



                while (whilelist.MoveNext())
                {
                    index++;

                    bool shouldSkipRowPresentation = false;
                    className = string.Empty;

                    RowHTML = new StringBuilder();

                    object item = whilelist.Current;

                    if (item == null) continue;

                    PropertyInfo[] infoCollection = null;

                    if (!isDataTable)
                        infoCollection = item.GetType().GetProperties();

                    bool isFirst = true;

                    string uniqueIdentifier = null, highlightColumn = null;

                    if (root.Page.Body.Grid.Table.IgnoreCreation)
                    {
                        if (isDataTable)
                        {
                            uniqueIdentifier = GetIndentifierKey((System.Data.DataRow)item, root.ListDataColumns.List);
                            highlightColumn = GetHighlightedValue((System.Data.DataRow)item, root.ListDataColumns.List);

                            if (highlightColumn != null)
                                highlightColumn = WebUtility.HtmlEncode(highlightColumn.ToString());

                        }
                        else
                        {
                            uniqueIdentifier = GetIndentifierKey(infoCollection, item, root.ListDataColumns.List);
                            highlightColumn = GetHighlightedValue(infoCollection, item, root.ListDataColumns.List);

                            if (highlightColumn != null)
                                highlightColumn = WebUtility.HtmlEncode(highlightColumn.ToString());

                        }

                        var parser = container.CurrentListInstance.wim.DoListDataItemCreated(DataItemType.Custom, root.ListDataColumns.List.ToArray(), null, item, uniqueIdentifier, null, index, null, source);
                        build2.Append(parser.InnerHTML);
                    }
                    else
                    #region Table cell creation
                    {
                        int columnCount = 0;
                        bool isFirstData = true;
                        string accordionPanelAddition = string.Empty;
                        foreach (Framework.ListDataColumn column in root.ListDataColumns.List)
                        {
                            object propertyValue = null;
                            System.Reflection.PropertyInfo info = null;

                            if (column.Type != Sushi.Mediakiwi.Framework.ListDataColumnType.Checkbox || column.Type == Sushi.Mediakiwi.Framework.ListDataColumnType.RadioBox)
                            {
                                if (isDataTable)
                                    propertyValue = ((System.Data.DataRow)item)[column.ColumnValuePropertyName];
                                else
                                    propertyValue = GetValue(infoCollection, item, column, out info);
                            }

                            int? listTypeID = null;
                            //19-10-20 depricated
                            //Sushi.Mediakiwi.Data.DalReflection.BaseSqlEntity tmp = item as Sushi.Mediakiwi.Data.DalReflection.BaseSqlEntity;
                            //if (tmp != null)
                            //    listTypeID = ((Sushi.Mediakiwi.Data.DalReflection.BaseSqlEntity)item).m_PropertyListTypeID;


                            if (column.Type == Sushi.Mediakiwi.Framework.ListDataColumnType.Checkbox || column.Type == Sushi.Mediakiwi.Framework.ListDataColumnType.RadioBox)
                            {
                                //  Is set in later code (twice!!!)
                                propertyValue = "#";
                            }
                            else
                                propertyValue = PropertyLogic.ConvertPropertyValue(column.ColumnValuePropertyName, propertyValue, container.CurrentList.ID, listTypeID);


                            bool hasNoWrap = false;
                            string align = null;
                            GridDataItemAttribute cell_attribute = new GridDataItemAttribute(DataItemType.TableCell);
                            GridDataItemAttribute row_attribute = new GridDataItemAttribute(DataItemType.TableRow);

                            #region Obtain uniqueIdentifier and highlightColumn
                            if (isFirst)
                            {

                                if (isDataTable)
                                {
                                    uniqueIdentifier = GetIndentifierKey((System.Data.DataRow)item, root.ListDataColumns.List);
                                    highlightColumn = GetHighlightedValue((System.Data.DataRow)item, root.ListDataColumns.List);

                                    if (highlightColumn != null)
                                        highlightColumn = WebUtility.HtmlEncode(highlightColumn.ToString());

                                }
                                else
                                {
                                    uniqueIdentifier = GetIndentifierKey(infoCollection, item, root.ListDataColumns.List);
                                    highlightColumn = GetHighlightedValue(infoCollection, item, root.ListDataColumns.List);

                                    if (highlightColumn != null)
                                        highlightColumn = WebUtility.HtmlEncode(highlightColumn.ToString());

                                }
                            }
                            #endregion Obtain uniqueIdentifier and highlightColumn

                            //var parser = container.CurrentListInstance.DoListDataItemCreated(DataItemType.TableCell, column, item, uniqueIdentifier, propertyValue, index, cell_attribute);
                            //propertyValue = parser.ItemValue;

                            #region Value specific cell markup
                            if (propertyValue != null)
                            {
                                if (column.Alignment == Sushi.Mediakiwi.Framework.Align.Default)
                                {
                                    if (propertyValue.GetType() == typeof(DateTime) || propertyValue.GetType() == typeof(Decimal) || propertyValue.GetType() == typeof(DateTime?) || propertyValue.GetType() == typeof(Decimal?))
                                        cell_attribute.Style.Add("white-space", "nowrap");

                                    if (propertyValue.GetType() == typeof(Decimal) || propertyValue.GetType() == typeof(int))
                                    {
                                        if (string.IsNullOrEmpty(cell_attribute.Class))
                                            cell_attribute.Class = "txt-r";
                                        else
                                            cell_attribute.Class += " txt-r";
                                    }
                                    if (propertyValue.GetType() == typeof(Boolean))
                                    {
                                        //cell_attribute.Align = "center";
                                        if (string.IsNullOrEmpty(cell_attribute.Class))
                                            cell_attribute.Class = "txt-c";
                                        else
                                            cell_attribute.Class += " txt-c";
                                    }
                                }
                                else
                                {
                                    switch (column.Alignment)
                                    {
                                        case Sushi.Mediakiwi.Framework.Align.Center:
                                            if (string.IsNullOrEmpty(cell_attribute.Class))
                                                cell_attribute.Class = "txt-c";
                                            else
                                                cell_attribute.Class += " txt-c";

                                            break;
                                        case Sushi.Mediakiwi.Framework.Align.Right:
                                            if (string.IsNullOrEmpty(cell_attribute.Class))
                                                cell_attribute.Class = "txt-r";
                                            else
                                                cell_attribute.Class += " txt-r";
                                            break;
                                    }
                                }
                            }
                            string cellClassName = null;

                            propertyValue = OutputValue(container, propertyValue, out cellClassName, column);
                            #endregion Value specific cell markup

                            bool hasInnerLink = (propertyValue != null && (propertyValue.ToString().Contains("<a ") || propertyValue.ToString().Contains("<input")));
                            if (hasInnerLink || column.Type == ListDataColumnType.RadioBox || column.Type == ListDataColumnType.Checkbox)
                            {
                                if (string.IsNullOrEmpty(cell_attribute.Class))
                                    cell_attribute.Class = "nopt";
                                else
                                    cell_attribute.Class += " nopt";
                            }

                            if (!string.IsNullOrEmpty(cellClassName))
                            {
                                className += string.Concat(" ", cellClassName);
                            }

                            if (!IsVisibleColumn(column.Type, includeExportFields))
                                continue;

                            #region Table row creation (and first TD)
                            object propertyHelp = null;
                            if (isFirst)
                            {
                                columnCount++;

                                AddFormCell(container, item, infoCollection, column, uniqueIdentifier, propertyHelp, ref propertyValue, ref shouldSkipRowPresentation, ref hasInnerLink);

                                #region Popuplayer click on item functionality
                                //  When highlightColumn is null or empty that particular row should act differtly (for popup layer/paging)
                                if (!root.Page.Body.Grid.IgnoreInLayerSubSelect && (type == 1 || type == 2 || container.OpenInFrame > 0) && highlightColumn != null && !string.IsNullOrEmpty(highlightColumn))
                                {
                                    row_attribute.Class = "classMouseHover parent link";
                                    var row_parser = container.CurrentListInstance.wim.DoListDataItemCreated(DataItemType.TableRow, root.ListDataColumns.List.ToArray(), column, item, uniqueIdentifier, null, index, row_attribute, source);
                                    var row_html = row_parser.ToString();


                                    RowHTML.Append(string.Format("\n\t\t\t\t\t\t\t\t{0}", row_html));
                                    //RowHTML.Append(string.Format("\n\t\t\t\t\t\t\t\t<tr class=\"{0} classMouseHover parent link{1}\">", className.Trim(), highlight));


                                    var cell_parser = container.CurrentListInstance.wim.DoListDataItemCreated(DataItemType.TableCell, root.ListDataColumns.List.ToArray(), column, item, uniqueIdentifier, propertyValue, index, cell_attribute, source);
                                    cell_parser.InnerHTML = string.Concat(string.Format("<input type=\"hidden\" id=\"T{0}\" value=\"{1}\" />", uniqueIdentifier, highlightColumn), cell_parser.InnerHTML);
                                    var cell_html = cell_parser.ToString();

                                    column.CalculateLength(cell_html, info);
                                    RowHTML.Append(string.Format("\n\t\t\t\t\t\t\t\t\t{0}", cell_html));

                                    //RowHTML.Append(string.Format("\n\t\t\t\t\t\t\t\t\t<td{3}{4}{5}><input type=\"hidden\" id=\"T{0}\" value=\"{1}\" />{2}"
                                    //    , uniqueIdentifier
                                    //    , highlightColumn
                                    //    , propertyValue
                                    //    , column.ColumnWidth == 0 ? "" : string.Format(" width=\"{0}\"", column.ColumnWidth)
                                    //    , hasNoWrap ? " nowrap" : string.Empty, align));
                                }
                                #endregion Popuplayer click on item functionality
                                else
                                #region Standaard table row functionality
                                {
                                    string passthrough = GetPassThroughValue(infoCollection, root, item);
                                    if (!string.IsNullOrEmpty(root.ListDataColumns.ColumnItemUrl))
                                    {
                                        //  Get URL from list item
                                        passthrough = GetValue(infoCollection, item, root.ListDataColumns.ColumnItemUrl).ToString();
                                    }
                                    //if (root._SearchListTargetIsSet || root.CurrentList.Option_LayerResult)
                                    //{
                                    //    if (root.CurrentList.Option_LayerResult && root.SearchListTarget == LayerSize.Undefined)
                                    //        root.SearchListTarget = LayerSize.Normal;


                                    //    //row_attribute["data-target"] = root.SearchListTarget.ToString();
                                    //}

                                    //if (passthrough != null && passthrough.Contains("?"))
                                    //    passthrough = passthrough.Split('?')[1];

                                    row_attribute["data-link"] = passthrough;

                                    row_attribute.Class = "parent";
                                 
                                    // CB: the accordion insertion
                                    //if (root.SearchListIsAccordion != null)
                                    //{
                                    //    accordionPanelAddition += $@"<tr><td class=""accordion""  colspan=""{root.ListDataColumns.List.Count}"">
                                    //                                        <div class=""first"" style=""height: {root.SearchListIsAccordion.PanelHeight}px"">
                                    //                                           <header> 
                                    //                                                <h4>{root.CurrentList.SingleItemName }</h4>
                                    //                                                <a href=""#closePanel"" onclick=""handleAccordionSelfClose(this)"">{root.SearchListIsAccordion.CloseLabel}</a>
                                    //                                            </header>
                                    //                                            <article />
                                    //                                        </div>
                                    //                                </td></tr>";
                                    //    row_attribute.Class += " hand";
                                    //    row_attribute["data-accordion"] = uniqueIdentifier;
                                    //    if (root.SearchListIsAccordion.CloseOtherPanelsOnClick)
                                    //        row_attribute["data-accordionSinglePanelOpen"] = "true";
                                    //}
                                    //else
                                    //{
                                        if (root.SearchListCanClickThrough && !string.IsNullOrEmpty(uniqueIdentifier) && (uniqueIdentifier != "0" || !string.IsNullOrEmpty(passthrough)))
                                        {
                                            row_attribute.ID = string.Format("id_{0}${1}", root.CurrentList.ID, uniqueIdentifier);
                                            row_attribute.Class += " hand";
                                        }
                                    //}

                                    var row_parser = container.CurrentListInstance.wim.DoListDataItemCreated(DataItemType.TableRow, root.ListDataColumns.List.ToArray(), column, item, uniqueIdentifier, null, index, row_attribute, source);
                                    var row_html = row_parser.ToString();

                                    RowHTML.Append(string.Format("\n\t\t\t\t\t\t\t\t\t{0}", row_html));
                                    var cell_parser = container.CurrentListInstance.wim.DoListDataItemCreated(DataItemType.TableCell, root.ListDataColumns.List.ToArray(), column, item, uniqueIdentifier, propertyValue, index, cell_attribute, source);
                                    var cell_html = cell_parser.ToString();

                                    column.CalculateLength(cell_html, info);
                                    RowHTML.Append(string.Format("\n\t\t\t\t\t\t\t\t\t{0}", cell_html));
                                }
                                #endregion Standaard table row functionality
                            }
                            #endregion Table row creation (and first TD)
                            else
                            {
                                AddFormCell(container, item, infoCollection, column, uniqueIdentifier, propertyHelp, ref propertyValue, ref shouldSkipRowPresentation, ref hasInnerLink);

                                var parser = container.CurrentListInstance.wim.DoListDataItemCreated(DataItemType.TableCell, root.ListDataColumns.List.ToArray(), column, item, uniqueIdentifier, propertyValue, index, cell_attribute, source);
                                var html = parser.ToString();

                                column.CalculateLength(html, info);
                                RowHTML.Append(string.Format("\n\t\t\t\t\t\t\t\t\t{0}", html));
                                //RowHTML.Append(string.Format("</td>\n\t\t\t\t\t\t\t\t\t<td{3}{4}{5}{6}{7}>{0}{1}{2}"
                                //    , column.ColumnValuePrefix
                                //    , propertyValue
                                //    , column.ColumnValueSuffix
                                //    , ""
                                //    , hasNoWrap ? " nowrap" : string.Empty
                                //    , propertyValue == null ? null : align
                                //    , hasInnerLink ? " class=\"nopt\"" : null
                                //    , string.IsNullOrEmpty(cellClassName) ? null : string.Format(" class=\"{0}\"", cellClassName) 
                                //    ));
                            }

                            isFirst = false;
                        }



                        RowHTML.Append("\n\t\t\t\t\t\t\t\t</tr>");
                        if (!String.IsNullOrEmpty(accordionPanelAddition))
                            RowHTML.Append("\n\t\t\t\t\t\t\t\t"+ accordionPanelAddition);
                        if (!shouldSkipRowPresentation)
                            build2.Append(RowHTML);
                    }
                    #endregion Table cell creation
                }
            }

            //  Set header
            #region Header
            build.Append("\n\t\t\t\t\t\t\t<thead>\n\t\t\t\t\t\t\t\t<tr class=\"first\">");
            foreach (Framework.ListDataColumn c in root.ListDataColumns.List)
            {
                if (c.EditConfiguration != null)
                    hasEditProperties = true;

                if (!IsVisibleColumn(c.Type, includeExportFields))
                    continue;

                string align = "";
                if (c.Alignment == Align.Right)
                    align = " class=\"txt-r\"";
                else if (c.Alignment == Align.Center)
                    align = " class=\"txt-c\"";

                string columnTitle = c.ColumnTitle;
                if (c.EditConfiguration != null && c.EditConfiguration.Type == ListDataEditConfigurationType.Checkbox)
                    columnTitle = "<input type=\"checkbox\" class=\"checkall\">";
                
                build.AppendFormat("\n\t\t\t\t\t\t\t\t\t<th{1}{2}>{0}</th>"
                    , columnTitle
                    , (
                        c.SuggestedColumnLength == 0 
                            ? "" 
                            : string.Format(" width=\"{0}\"{1}{2}"
                                , c.SuggestedColumnLength
                                , c.ColumnIsFixed ? " class=\"fixed\"" : null
                                , c.ColumnFixedLeftMargin > 0 ? string.Format(" style=\"margin-left:{0}px\"", c.ColumnFixedLeftMargin) : null
                                )
                        )
                    , align);
            }

            build.AppendFormat("\n\t\t\t\t\t\t\t\t</tr>\n\t\t\t\t\t\t\t</thead>\n\t\t\t\t\t\t\t<tbody{0}>", root.Page.Body.Grid.ClickLayerTag);
            #endregion Header

            build2.Append("\n\t\t\t\t\t\t\t</tbody>");

            string candidate = null;
            if (root.Page.Body.Grid.Table.IgnoreCreation)
            {
                candidate = string.Concat(build2.ToString(), paging);
            }
            else
            {
                #region Table clell creation (sum)
                if (hasTotal)
                {
                    StringBuilder build3 = new StringBuilder();

                    if (true)//container.CurrentApplicationUser.ShowNewDesign2)
                        build.Append("\n\t\t\t\t\t\t\t\t<tr class=\"totals nosort\">");
                    else
                        build.Append("\n\t\t\t\t\t\t\t\t<tr class=\"sum nosort\">");

                    foreach (Framework.ListDataColumn column in root.ListDataColumns.List)
                    {
                        if (!IsVisibleColumn(column.Type, includeExportFields)) continue;
                        if (column.Total == Sushi.Mediakiwi.Framework.ListDataTotalType.Sum)
                        {
                            decimal total = column.TotalValue;
                            if (root.GridDataCommunication.HasAppliedData)
                            {
                                var tmp = root.GridDataCommunication.GetResultData(Framework.ListDataTotalType.Sum, column.ColumnValuePropertyName);
                                total = Utility.ConvertToDecimal(tmp, total);
                            }

                            if (column.TotalValueType == typeof(int))
                                build.AppendFormat("\n\t\t\t\t\t\t\t\t\t<td class=\"txt-r{3}\"{4}>{0}{1}{2}</td>"
                                    , column.ColumnValuePrefix
                                    , OutputValue(container, Convert.ToInt32(total), column)
                                    , column.ColumnValueSuffix
                                    , column.ColumnIsFixed ? "fixed" : null
                                    , column.ColumnFixedLeftMargin > 0 ? string.Format(" style=\"margin-left:{0}px\"", column.ColumnFixedLeftMargin) : null
                                );
                            else
                                build.AppendFormat("\n\t\t\t\t\t\t\t\t\t<td class=\"txt-r{3}\"{4}>{0}{1}{2}</td>"
                                    , column.ColumnValuePrefix
                                    , OutputValue(container, total, column)
                                    , column.ColumnValueSuffix
                                    , column.ColumnIsFixed ? "fixed" : null
                                    , column.ColumnFixedLeftMargin > 0 ? string.Format(" style=\"margin-left:{0}px\"", column.ColumnFixedLeftMargin) : null
                                    );
                        }
                        else if (column.Total == Sushi.Mediakiwi.Framework.ListDataTotalType.Average)
                        {
                            build.AppendFormat("\n\t\t\t\t\t\t\t\t\t<td class=\"txt-r{3}\"{4}>{0}{1}{2}</td>"
                                , column.ColumnValuePrefix
                                , OutputValue(container, Decimal.Divide(column.TotalValue, count), column)
                                , column.ColumnValueSuffix
                                , column.ColumnIsFixed ? "fixed" : null
                                , column.ColumnFixedLeftMargin > 0 ? string.Format(" style=\"margin-left:{0}px\"", column.ColumnFixedLeftMargin) : null
                                );
                        }
                        else
                            build.AppendFormat("\n\t\t\t\t\t\t\t\t\t<td{0}{1}></td>", column.ColumnIsFixed ? " class=\"fixed\"" : null
                                , column.ColumnFixedLeftMargin > 0 ? string.Format(" style=\"margin-left:{0}px\"", column.ColumnFixedLeftMargin) : null);

                    }
                }





                if (type > 0)
                {
                    if (type == 3)
                    {
                        string titleBar = null;
                        candidate = string.Concat(titleBar
                            , "\n\t\t\t\t\t<article class=\"dataBlock\">"
                            , pagingTop
                            , scrolClass
                            , "\n\t\t\t\t\t\t<table"
                            , tableClass
                            , ">"
                            , build.ToString()
                            , build2.ToString()
                            , "\n\t\t\t\t\t\t</table>"
                            , (hasScroll ? "\n\t\t\t\t\t\t</div></div>" : null)
                            , paging
                            , "\n\t\t\t\t\t</article>"
                            );
                    }
                    else if (type == 4)
                    {
                        candidate = string.Concat("\n\t\t\t\t\t<article class=\"dataBlock\">"
                            , pagingTop
                            , scrolClass
                            , "\n\t\t\t\t\t\t<table"
                            , tableClass
                            , ">"
                            , build.ToString()
                            , build2.ToString()
                            , "\n\t\t\t\t\t\t</table>"
                            , (hasScroll ? "\n\t\t\t\t\t\t</div></div>" : null)
                            , paging
                            , "\n\t\t\t\t\t</article>"
                            );
                    }
                    else
                        candidate = string.Concat("\n\t\t\t\t\t<article class=\"dataBlock\">"
                            , pagingTop
                            , scrolClass
                            , "\n\t\t\t\t\t\t<table"
                            , tableClass, ">"
                            , build.ToString()
                            , build2.ToString()
                            , "\n\t\t\t\t\t\t</table>"
                            , (hasScroll ? "\n\t\t\t\t\t\t</div></div>" : null)
                            , paging
                            , "\n\t\t\t\t\t</article>"
                            );
                }
                else
                    candidate = string.Concat("\n\t\t\t\t\t<article class=\"dataBlock\">"
                        , pagingTop
                        , scrolClass
                        , "\n\t\t\t\t\t\t<table"
                        , tableClass
                        , ">"
                        , build.ToString()
                        , build2.ToString()
                        , "\n\t\t\t\t\t\t</table>"
                            , (hasScroll ? "\n\t\t\t\t\t\t</div></div>" : null)
                        , paging
                        , "\n\t\t\t\t\t</article>"
                        );
                #endregion Table clell creation (sum)
            }
            return candidate;
        }


        /// <summary>
        /// Gets the grid from list instance for JSON.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="container">The container.</param>
        /// <param name="type">The type.</param>
        /// <param name="includeExportFields">if set to <c>true</c> [include export fields].</param>
        /// <param name="isNewDesignOutput">if set to <c>true</c> [is new design output].</param>
        /// <returns></returns>
        internal string GetGridFromListInstanceForJSON(Framework.WimComponentListRoot root, Sushi.Mediakiwi.Beta.GeneratedCms.Console container, int type, bool includeExportFields, bool isNewDesignOutput)
        {
            container.ListPagingValue = root.CurrentPage.ToString();// container.Request.Params["set"];

            //bool isEditMode = false;
            bool sortOrderIsActive = container.IsSortorderOn;

            //  Trigger list search event
            
            container.CurrentListInstance.wim.DoListSearch();

            ////  if no data is assigned return null 
            if (root.ListDataTable == null && root.ListData == null)
                return Constants.JSON_NO_ACCESS;

            //StringBuilder build = new StringBuilder();
            //StringBuilder build = new StringBuilder();

            DataGridForJSON json = new DataGridForJSON();
            json.Set = root.CurrentPage;
            
            //if (container.CurrentList.Option_HasShowAll)
            //{
            //    json.All = container.ListPagingValue != "-1";
            //    json.Set = !json.All;
            //}
            
            ////  Create the header and the columns
            //if (type != 4)
            //    build.Append("\n\t\t\t\t\t\t\t<thead>\n\t\t\t\t\t\t\t\t<tr class=\"first\">");
            //int visibleColumnCount = 0;

            //bool hasEditProperties = false;

            //foreach (Framework.ListDataColumn column in root.ListDataColumns.List)
            //{
            //    if (column.EditConfiguration != null)
            //        hasEditProperties = true;

            //    if (!IsVisibleColumn(column.Type, includeExportFields))
            //        continue;

            //    visibleColumnCount++;
            //    build.AppendFormat("\n\t\t\t\t\t\t\t\t\t<th{1}>{0}</th>", column.ColumnName, (column.ColumnWidth == 0 ? "" : string.Format(" width=\"{0}\"", column.ColumnWidth)));
            //}

            //if (visibleColumnCount == 0)
            //    return string.Format("<table class=\"data\"><tfoot><tr><td>{0}</td></tr></tfoot></table>"
            //            , Labels.ResourceManager.GetString("grid_no_search_columns", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
            //        );

            bool isDataTable = root.ListData == null;
            int count = isDataTable ? root.ListDataTable.Rows.Count : root.ListData.Count;
            if (root.m_IsLinqUsed)
            {
                json.Tot = root.m_ListDataRecordCount;
                json.Max = root.m_ListDataRecordPageCount;
            }
            else
                json.Tot = count;

            ////  Create the content
            //string className = "odd";

            IEnumerator whilelist = null;

            int currentPage = Utility.ConvertToInt(container.ListPagingValue, 1);
            if (currentPage < 1) currentPage = 1;
            string paging = null;

            json.Lst = root.IsDashboardMode ? root.SearchViewDashboardMaxLength : root.CurrentList.Option_Search_MaxResultPerPage;

            if ((json.Set * json.Lst - json.Lst) > json.Tot)
                json.Set = 1;

            if (root.m_IsLinqUsed)
            {
                count = root.m_ListDataRecordCount - root.m_ListDataInterLineCount;
                whilelist = root.ListData.GetEnumerator();
            }
            else
            {
                Utilities.Splitlist splitlist = null;

                if (isDataTable)
                {
                    if (root.ListDataTable.Rows.Count > 0)
                    {
                        splitlist = new Splitlist(root.ListDataTable, json.Lst, root.CurrentList.Option_Search_MaxViews);
                        if (currentPage > splitlist.ListCount)
                            currentPage = splitlist.ListCount;

                        json.Max = splitlist.ListCount;

                        whilelist = ((System.Data.DataTable)splitlist[currentPage - 1]).Rows.GetEnumerator();
                    }
                }
                else
                {
                    if (root.ListData.Count > 0)
                    {
                        splitlist = new Splitlist(root.ListData, json.Lst, root.CurrentList.Option_Search_MaxViews);
                        if (currentPage > splitlist.ListCount)
                            currentPage = splitlist.ListCount;

                        json.Max = splitlist.ListCount;

                        whilelist = ((IList)splitlist[currentPage - 1]).GetEnumerator();
                    }
                }
                //if (!root.IsDashboardMode)
                //{
                //    paging = GetListPaging(container, splitlist, currentPage, true);
                //}
            }

            //StringBuilder build2 = new StringBuilder();
            //StringBuilder RowHTML = null;

            bool hasTotal = ApplyTotalInformation(isDataTable, root);
            if (whilelist != null)
            {
                while (whilelist.MoveNext())
                {
                    //bool shouldSkipRowPresentation = false;
                    //RowHTML = new StringBuilder();

                    object item = whilelist.Current;
                    if (item == null) continue;

                    PropertyInfo[] infoCollection = null;

                    if (!isDataTable)
                        infoCollection = item.GetType().GetProperties();

                    #region OLD
                    //if (className == "odd" || className == "odd2")
                    //    className = string.Empty;//(container.CurrentListInstance.wim.SearchListCanClickThrough ? "even2" : "even");
                    //else
                    //    className = "odd";// (container.CurrentListInstance.wim.SearchListCanClickThrough ? "odd2" : "odd");

                    //if (hasEditProperties)
                    //    className += " autoSubmit";


                    //bool isFirst = true;

                    //string uniqueIdentifier = null, highlightColumn = null;

                    //string classNameAddition =
                    //    this.GetTableRowClassName(infoCollection, item, container.CurrentListInstance.wim.ListDataClassNamePropertyValue);

                    //if (!string.IsNullOrEmpty(classNameAddition))
                    //    className = string.Concat(className, " ", classNameAddition);

                    //if (!string.IsNullOrEmpty(root.ListDataIsInterlinePropertyValue))
                    //{
                    //    bool isInterLine = (bool)GetValue(infoCollection, item, root.ListDataIsInterlinePropertyValue);

                    //    if (isInterLine)
                    //    {
                    //        RowHTML.AppendFormat("\n\t\t\t\t\t\t\t\t<tr>\n\t\t\t\t\t\t\t\t\t<td class=\"interline\" colspan=\"{0}\">{1}</td>\n\t\t\t\t\t\t\t\t</tr>", visibleColumnCount
                    //            , GetValue(infoCollection, item, root.ListDataInterlineTextPropertyValue)
                    //            );

                    //        root.m_ListDataInterLineCount++;
                    //        count--;
                    //        continue;
                    //    }
                    //}

                    //int columnCount = 0;
                    #endregion OLD
                    bool isFirstData = true;

                    var row = new Dictionary<string, object>();

                    foreach (Framework.ListDataColumn column in root.ListDataColumns.List)
                    {
                        object propertyValue = null;

                        if (column.Type != Sushi.Mediakiwi.Framework.ListDataColumnType.Checkbox || column.Type == Sushi.Mediakiwi.Framework.ListDataColumnType.RadioBox)
                        {
                            if (isDataTable)
                                propertyValue = ((System.Data.DataRow)item)[column.ColumnValuePropertyName];
                            else
                                propertyValue = GetValue(infoCollection, item, column);
                        }

                        int? listTypeID = null;
                        //19-10-20 depricated
                        //Sushi.Mediakiwi.Data.DalReflection.BaseSqlEntity tmp = item as Sushi.Mediakiwi.Data.DalReflection.BaseSqlEntity;
                        //if (tmp != null)
                        //    listTypeID = ((Sushi.Mediakiwi.Data.DalReflection.BaseSqlEntity)item).m_PropertyListTypeID;


                        if (column.Type == Sushi.Mediakiwi.Framework.ListDataColumnType.Checkbox || column.Type == Sushi.Mediakiwi.Framework.ListDataColumnType.RadioBox)
                        {
                            //  Is set in later code (twice!!!)
                            propertyValue = "#";
                        }
                        else
                            propertyValue = PropertyLogic.ConvertPropertyValue(column.ColumnValuePropertyName, propertyValue, container.CurrentList.ID, listTypeID);

                        bool hasNoWrap = false;
                        string align = null;

                        //if (propertyValue != null)
                        //{
                        //    if (column.Alignment == Sushi.Mediakiwi.Framework.Align.Default)
                        //    {
                        //        if (propertyValue.GetType() == typeof(DateTime) || propertyValue.GetType() == typeof(Decimal) || propertyValue.GetType() == typeof(DateTime?) || propertyValue.GetType() == typeof(Decimal?))
                        //            hasNoWrap = true;

                        //        if (propertyValue.GetType() == typeof(Decimal) || propertyValue.GetType() == typeof(int))
                        //        {
                        //            align = " align=\"right\"";
                        //        }
                        //        if (propertyValue.GetType() == typeof(Boolean))
                        //        {
                        //            align = " align=\"center\"";
                        //        }
                        //    }
                        //    else
                        //    {
                        //        switch (column.Alignment)
                        //        {
                        //            case Sushi.Mediakiwi.Framework.Align.Left: align = " align=\"left\""; break;
                        //            case Sushi.Mediakiwi.Framework.Align.Center: align = " align=\"center\""; break;
                        //            case Sushi.Mediakiwi.Framework.Align.Right: align = " align=\"right\""; break;
                        //        }
                        //    }
                        //}
                        string cellClassName = null;
                        propertyValue = OutputValue(container, propertyValue, out cellClassName, column);

                        //if (propertyValue != null)
                        //    propertyValue = propertyValue.ToString().Replace("\"", "\\\"");

                        row.Add(DataGridForJSON.ConvertToColumnName(column), propertyValue);
                        //json.Items.Add(new Record() {  Columns = 
                                
                        //    Key = column.ColumnValuePropertyName.Replace(".", "_"), Value = propertyValue });

                        //build.AppendFormat("\"{0}\":\"{1}\"", column.ColumnValuePropertyName.Replace(".", "_"), propertyValue);
                        //isFirstData = false;
                    }
                    json.Rows.Add(row);
                    //    bool hasInnerLink = (propertyValue != null && propertyValue.ToString().Contains("<a href"));

                    //    if (column.ColumnValuePropertyName == "Moment")
                    //    {
                    //        if (align == null)
                    //            align = "";


                    //        if (className == "even")
                    //            align += string.Format(" style=\"background: #EAF1F2 url('../../../assets/{0}') no-repeat\"", container.CurrentListInstance.wim.Grid.BackgroundImage_Even);
                    //        else
                    //            align += string.Format(" style=\"background: url('../../../assets/{0}') no-repeat\"", container.CurrentListInstance.wim.Grid.BackgroundImage_Odd);
                    //    }

                    //    if (!IsVisibleColumn(column.Type, includeExportFields))
                    //        continue;

                    //    if (isFirst)
                    //    {
                    //        columnCount++;
                    //        if (isDataTable)
                    //        {
                    //            uniqueIdentifier = GetIndentifierKey((System.Data.DataRow)item, root.ListDataColumns.List);
                    //            highlightColumn = GetHighlightedValue((System.Data.DataRow)item, root.ListDataColumns.List);

                    //            if (highlightColumn != null)
                    //                highlightColumn = WebUtility.HtmlEncode(highlightColumn.ToString());

                    //        }
                    //        else
                    //        {
                    //            uniqueIdentifier = GetIndentifierKey(infoCollection, item, root.ListDataColumns.List);
                    //            highlightColumn = GetHighlightedValue(infoCollection, item, root.ListDataColumns.List);

                    //            if (highlightColumn != null)
                    //                highlightColumn = WebUtility.HtmlEncode(highlightColumn.ToString());

                    //        }

                    //        ApplyCheckboxTableCell(container, column, uniqueIdentifier, ref propertyValue);

                    //        if (column.EditConfiguration != null)
                    //        {
                    //            object propertyHelp = null;
                    //            if (!string.IsNullOrEmpty(column.EditConfiguration.InteractiveHelp))
                    //                propertyHelp = GetValue(infoCollection, item, column.EditConfiguration.InteractiveHelp);

                    //            ApplyEditConfiguration(container, item, column, uniqueIdentifier, propertyHelp, ref propertyValue, ref shouldSkipRowPresentation);
                    //        }

                    //        string highlight =
                    //            this.GetHighlightedState(infoCollection, item, container.CurrentListInstance.wim.ListDataHighlightPropertyValue)
                    //            ? " highlight" : null;

                    //        //  When highlightColumn is null or empty that particular row should act differtly (for popup layer/paging)
                    //        if ((type == 1 || type == 2) && highlightColumn != null && !string.IsNullOrEmpty(highlightColumn))
                    //        {
                    //            RowHTML.Append(string.Format("\n\t\t\t\t\t\t\t\t<tr class=\"{0} classMouseHover link{1}\">", className, highlight));
                    //            RowHTML.Append(string.Format("\n\t\t\t\t\t\t\t\t\t<td{3}{4}{5}><input type=\"checkbox\" id=\"{0}\" value=\"{1}\" />{2}", uniqueIdentifier, highlightColumn, propertyValue, column.ColumnWidth == 0 ? "" : string.Format(" width=\"{0}\"", column.ColumnWidth), hasNoWrap ? " nowrap" : string.Empty, align));
                    //        }
                    //        else
                    //        {

                    //            RowHTML.Append(string.Format("\n\t\t\t\t\t\t\t\t<tr class=\"{1}{0}{2}{3}\">", className
                    //                , isEditMode ? "autoSubmit " : null
                    //                , highlight
                    //                , container.CurrentListInstance.wim.SearchListCanClickThrough ? " hand" : null
                    //                ));
                    //            string url = "#";

                    //            if (sortOrderIsActive)
                    //                url = string.Concat("#", uniqueIdentifier);

                    //            if (!isEditMode && container.CurrentListInstance.HasListLoad && !sortOrderIsActive)
                    //            {
                    //                if (!string.IsNullOrEmpty(root.ListDataColumns.ColumnItemUrl))
                    //                {
                    //                    //  Get URL from list item
                    //                    url = GetValue(infoCollection, item, root.ListDataColumns.ColumnItemUrl).ToString();

                    //                }
                    //                else
                    //                {
                    //                    if (string.IsNullOrEmpty(uniqueIdentifier) && string.IsNullOrEmpty(root.SearchResultItemPassthroughParameterProperty))
                    //                        url = "#";
                    //                    else
                    //                    {
                    //                        if (string.IsNullOrEmpty(uniqueIdentifier))
                    //                        {
                    //                            if (isDataTable)
                    //                                url = string.Concat(container.WimPagePath, "?", GetPassThroughValue(root, (System.Data.DataRow)item).Replace("[KEY]", uniqueIdentifier));
                    //                            else
                    //                                url = string.Concat(container.WimPagePath, "?", GetPassThroughValue(infoCollection, root, item).Replace("[KEY]", uniqueIdentifier));
                    //                        }
                    //                        else
                    //                        {
                    //                            if (isDataTable)
                    //                                url = string.Concat(container.WimPagePath, "?", GetPassThroughValue(root, (System.Data.DataRow)item).Replace("[KEY]", uniqueIdentifier), "=", uniqueIdentifier);
                    //                            else
                    //                                url = string.Concat(container.WimPagePath, "?", GetPassThroughValue(infoCollection, root, item).Replace("[KEY]", uniqueIdentifier), "=", uniqueIdentifier);
                    //                        }
                    //                    }

                    //                    if (type > 0)
                    //                    {
                    //                        int typeID = Utility.ConvertToInt(container.Context.Request.QueryString["type"]);
                    //                        //  Paging in a popup layer requires some addition url parameters
                    //                        if (typeID > 0)
                    //                            url = string.Concat(url, "&openinframe=", container.Request.QueryString["openinframe"], "&type=", typeID, "&referid=", container.Request.QueryString["referid"]
                    //                                , string.IsNullOrEmpty(container.Context.Request.QueryString["root"]) ? "" : string.Concat("&root=", container.Context.Request.QueryString["root"]));
                    //                        else
                    //                            url = string.Concat(url, "&openinframe=", container.Request.QueryString["openinframe"], "&referid=", container.Request.QueryString["referid"]
                    //                                , string.IsNullOrEmpty(container.Context.Request.QueryString["root"]) ? "" : string.Concat("&root=", container.Context.Request.QueryString["root"]));
                    //                    }
                    //                }
                    //            }


                    //            if (hasEditProperties)
                    //            {
                    //                url = Utility.AddApplicationPath(string.Concat(Wim.CommonConfiguration.RelativeRepositoryUrl, "/tcl.aspx?auto=", container.CurrentListInstance.wim.CurrentSite.ID, ",", container.CurrentListInstance.wim.CurrentList.ID, ",", uniqueIdentifier));
                    //                RowHTML.Append(string.Format("\n\t\t\t\t\t\t\t\t\t<td{2}{3}{4}><input type=\"hidden\" class=\"autoSubmitTarget\" value=\"{0}\">{1}"
                    //                    , url
                    //                    , propertyValue, column.ColumnWidth == 0 ? "" : string.Format(" width=\"{0}\"", column.ColumnWidth)
                    //                    , hasNoWrap ? " nowrap" : string.Empty, align
                    //                    ));
                    //            }
                    //            else
                    //            {
                    //                if (container.CurrentListInstance.wim.SearchListCanClickThrough)
                    //                {
                    //                    RowHTML.Append(string.Format("\n\t\t\t\t\t\t\t\t\t<td{2}{3}{4}{5}><a href=\"{0}\" class=\"clickOnParent\"></a>{1}"
                    //                        , url
                    //                        , propertyValue
                    //                        , ""
                    //                        , hasNoWrap ? " nowrap" : string.Empty
                    //                        , align
                    //                        , hasInnerLink ? " class=\"nopt\"" : null
                    //                        ));
                    //                }
                    //                else
                    //                    RowHTML.Append(string.Format("\n\t\t\t\t\t\t\t\t\t<td{1}{2}{3}>{0}", propertyValue, "", hasNoWrap ? " nowrap" : string.Empty, align));

                    //            }
                    //        }
                    //    }
                    //    else
                    //    {

                    //        ApplyCheckboxTableCell(container, column, uniqueIdentifier, ref propertyValue);

                    //        //if (isEditMode && column.EditConfiguration != null)
                    //        if (column.EditConfiguration != null)
                    //        {
                    //            object propertyHelp = null;
                    //            if (!string.IsNullOrEmpty(column.EditConfiguration.InteractiveHelp))
                    //                propertyHelp = GetValue(infoCollection, item, column.EditConfiguration.InteractiveHelp);

                    //            ApplyEditConfiguration(container, item, column, uniqueIdentifier, propertyHelp, ref propertyValue, ref shouldSkipRowPresentation);
                    //        }

                    //        RowHTML.Append(string.Format("</td>\n\t\t\t\t\t\t\t\t\t<td{3}{4}{5}{6}>{0}{1}{2}"
                    //            , column.ColumnValuePrefix
                    //            , propertyValue
                    //            , column.ColumnValueSuffix
                    //            , ""
                    //            , hasNoWrap ? " nowrap" : string.Empty
                    //            , align
                    //            , hasInnerLink ? " class=\"nopt\"" : null
                    //            ));
                    //    }

                    //    isFirst = false;
                    //}
                    //if (isEditMode)
                    //{
                    //    //build2.AppendFormat("<img class=\"progressIndicator\" alt=\"\" src=\"{0}/images/progressIndicator_passive.png\"/>", container.WimRepository);
                    //}

                    //RowHTML.Append("</td>\n\t\t\t\t\t\t\t\t</tr>");

                    //if (!shouldSkipRowPresentation)
                    //    build2.Append(RowHTML);
                    //else
                    //{
                    //    //  switch zebra
                    //    if (className == "odd") className = "even";
                    //    else className = "odd";
                    //}
                }
             
            }

            //build2.Append("\n\t\t\t\t\t\t\t</tbody>");


            //if (type != 4)
            //    build.AppendFormat("\n\t\t\t\t\t\t\t\t</tr>\n\t\t\t\t\t\t\t\t<tr class=\"empty\">\n\t\t\t\t\t\t\t\t\t<th colspan=\"{0}\">&nbsp;</th>\n\t\t\t\t\t\t\t\t</tr>\n\t\t\t\t\t\t\t</thead>\n\t\t\t\t\t\t\t<tbody>\n", visibleColumnCount);

            if (hasTotal && !sortOrderIsActive)
            {
                json.Sum = new Dictionary<string, object>();
                //StringBuilder build3 = new StringBuilder();

                //build.Append("\n\t\t\t\t\t\t\t\t</tr>\n\t\t\t\t\t\t\t\t<tr>");

                foreach (Framework.ListDataColumn column in root.ListDataColumns.List)
                {
                    if (!IsVisibleColumn(column.Type, includeExportFields)) continue;
                    if (column.Total == Sushi.Mediakiwi.Framework.ListDataTotalType.Sum)
                    {
                        decimal total = column.TotalValue;
                        if (root.GridDataCommunication.HasAppliedData)
                        {
                            var tmp = root.GridDataCommunication.GetResultData(Framework.ListDataTotalType.Sum, column.ColumnValuePropertyName);
                            total = Utility.ConvertToDecimal(tmp, total);
                        }

                        json.Sum.Add(DataGridForJSON.ConvertToColumnName(column), total);

                        //if (column.TotalValueType == typeof(int))
                        //    build.AppendFormat("\n\t\t\t\t\t\t\t\t\t<th class=\"sum\" align=\"right\">{0}{1}{2}</th>", column.ColumnValuePrefix, OutputValue(container, Convert.ToInt32(total), column), column.ColumnValueSuffix);
                        //else
                        //    build.AppendFormat("\n\t\t\t\t\t\t\t\t\t<th class=\"sum\" align=\"right\">{0}{1}{2}</th>", column.ColumnValuePrefix, OutputValue(container, total, column), column.ColumnValueSuffix);
                    }
                    else if (column.Total == Sushi.Mediakiwi.Framework.ListDataTotalType.Average)
                    {
                        json.Sum.Add(DataGridForJSON.ConvertToColumnName(column), Decimal.Divide(column.TotalValue, count));
                        //build.AppendFormat("\n\t\t\t\t\t\t\t\t\t<th class=\"sum\" align=\"right\">{0}{1}{2}</th>", column.ColumnValuePrefix, OutputValue(container, Decimal.Divide(column.TotalValue, count), column), column.ColumnValueSuffix);
                    }
                    //else
                    //    build.Append("\n\t\t\t\t\t\t\t\t\t<th class=\"sum\"></th>");

                }
                //build.Append("</tr>");
                //build2.Insert(0, build3.ToString());
            }



//            string candidate;

//            bool hasScroll = false;
//            string scrollClass = string.Empty;
//            if (hasScroll)
//            {
//                scrollClass = " class=\"scroll\"";
//            }
//            if (type > 0)
//            {


//                if (type == 3)
//                {
//                    string titleBar = null;
//                    candidate = string.Concat(titleBar, "\n\t\t\t\t\t<article class=\"dataBlock\">\n\t\t\t\t\t\t<table", scrollClass, ">", build.ToString(), build2.ToString(), "\n\t\t\t\t\t\t</table>", paging, "\n\t\t\t\t\t</article>");
//                }
//                else if (type == 4)
//                {
//                    candidate = string.Format(@"
//<div class=""updates"">
//	{0}
//</div>
//"
//                    , string.Concat("\n\t\t\t\t\t<article class=\"dataBlock\">\n\t\t\t\t\t\t<table", scrollClass, ">", build.ToString(), build2.ToString(), "\n\t\t\t\t\t\t</table>", paging, "\n\t\t\t\t\t</article>")
//                    , container.WimPagePath
//                    , container.CurrentList.ID
//                    );

//                }
//                else
//                    candidate = string.Concat("\n\t\t\t\t\t<article class=\"dataBlock\">\n\t\t\t\t\t\t<table", scrollClass, ">", build.ToString(), build2.ToString(), "\n\t\t\t\t\t\t</table>", paging, "\n\t\t\t\t\t</article>");
//            }
//            else
//                candidate = string.Concat("\n\t\t\t\t\t<article class=\"dataBlock\">\n\t\t\t\t\t\t<table", scrollClass, ">", build.ToString(), build2.ToString(), "\n\t\t\t\t\t\t</table>", paging, "\n\t\t\t\t\t</article>");

            return json.ToString();
        }


        /// <summary>
        /// Applies the checkbox table cell.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="column">The column.</param>
        /// <param name="uniqueIdentifier">The unique identifier.</param>
        /// <param name="propertyValue">The property value.</param>
        void ApplyCheckboxTableCell(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, Framework.ListDataColumn column, string uniqueIdentifier, ref object propertyValue)
        {
            if (column.Type == Sushi.Mediakiwi.Framework.ListDataColumnType.Checkbox
                || column.Type == Sushi.Mediakiwi.Framework.ListDataColumnType.RadioBox
                )
            {
                string key = string.Format("{0}_{1}", column.ColumnValuePropertyName, uniqueIdentifier);

                bool isChecked = false;
                bool isEnabled = true;
                //  Custom
                if (column.Type == Sushi.Mediakiwi.Framework.ListDataColumnType.Checkbox && container.CurrentListInstance.wim.AddedCheckboxStateCollection != null)
                {
                    string value = container.CurrentListInstance.wim.AddedCheckboxStateCollection[key];
                    if (!string.IsNullOrEmpty(value))
                    {
                        isEnabled = (value == "1");
                    }
                }
                else if (column.Type == Sushi.Mediakiwi.Framework.ListDataColumnType.RadioBox && container.CurrentListInstance.wim.AddedRadioboxStateCollection != null)
                {
                    string value = container.CurrentListInstance.wim.AddedRadioboxStateCollection[key];
                    if (!string.IsNullOrEmpty(value))
                    {
                        isEnabled = (value == "1");
                    }
                }

                //  Custom
                
                if (column.Type == Sushi.Mediakiwi.Framework.ListDataColumnType.Checkbox && container.CurrentListInstance.wim.AddedCheckboxPostCollection != null)
                {
                    string value = container.CurrentListInstance.wim.AddedCheckboxPostCollection[key];
                    if (!string.IsNullOrEmpty(value))
                    {
                        isChecked = (value == "1");
                    }
                }


                //  Via postback
                string post = container.Form(key);
                if (!string.IsNullOrEmpty(post))
                {
                    isChecked = container.Form(key) == "1";
                }

                if (column.Type == Sushi.Mediakiwi.Framework.ListDataColumnType.Checkbox)
                {
                    propertyValue = string.Format("<input id=\"{0}\" name=\"{0}\" value=\"1\"{1}{2} type=\"checkbox\">"
                        , key
                        , isChecked ? " checked=\"checked\"" : ""
                        , isEnabled ? "" : " disabled=\"disabled\""
                        );
                }
                //else if (column.Type == Sushi.Mediakiwi.Framework.ListDataColumnType.TextboxPercentage)
                //{
                //    string value = null;
                //    if (container.CurrentListInstance.wim.AddedTextboxPostCollection != null)
                //        value = container.CurrentListInstance.wim.AddedTextboxPostCollection[key];

                //    propertyValue = string.Format("<input type=\"text\" id=\"{0}\" name=\"{0}\" maxlength=\"5\" style=\"width: 34px; text-align:right\" value=\"{2}\"{1}>%"
                //        , key
                //        , isEnabled ? "" : " disabled=\"disabled\""
                //        , value
                //        );
                //}
                else
                {
                    string value = "";
                    if (column.Type == Sushi.Mediakiwi.Framework.ListDataColumnType.RadioBox && container.CurrentListInstance.wim.AddedRadioboxPostCollection != null)
                        value = container.CurrentListInstance.wim.AddedRadioboxPostCollection[key];

                    propertyValue = string.Format("<input name=\"{0}\" value=\"{3}\"{1}{2} type=\"radio\">"
                        , key
                        , value.Equals(column.ColumnName) ? " checked=\"checked\"" : ""
                        , isEnabled ? "" : " disabled=\"disabled\""
                        , column.ColumnName
                        );
                }
            }
        }

        void SetPolution(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, object sender)
        {
            if (container.CurrentListInstance.wim.m_ChangedSearchGridItem == null)
                container.CurrentListInstance.wim.m_ChangedSearchGridItem = new List<object>();
            container.CurrentListInstance.wim.m_ChangedSearchGridItem.Add(sender);
        }

        bool HasPostItem(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, string name)
        {
            if (container.Request.HasFormContentType)
            {
                var arr = container.Request.Form.Keys;
                foreach (var item in arr)
                {
                    if (item == name)
                        return true;
                }
            }
            return false;
        }

        void AddFormCell(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, object item, PropertyInfo[] infoCollection, Framework.ListDataColumn column, string uniqueIdentifier, object propertyHelp, ref object propertyValue, ref bool shouldSkipRowPresentation, ref bool hasInnerLink)
        {
            ApplyCheckboxTableCell(container, column, uniqueIdentifier, ref propertyValue);

            if (column.EditConfiguration != null)
            {

                if (!string.IsNullOrEmpty(column.EditConfiguration.InteractiveHelp))
                    propertyHelp = GetValue(infoCollection, item, column.EditConfiguration.InteractiveHelp);

                hasInnerLink = true;
                ApplyEditConfiguration(container, item, column, uniqueIdentifier, propertyHelp, ref propertyValue, ref shouldSkipRowPresentation);
            }
        }

        void ApplyEditConfiguration(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, object sender, Framework.ListDataColumn column, string uniqueIdentifier, object propertyHelp, ref object properyValue, ref bool shouldSkipRowPresentation)
        {
            shouldSkipRowPresentation = false;
            string propertyName = column.EditConfiguration.PropertyToSet == null ? column.ColumnValuePropertyName : column.EditConfiguration.PropertyToSet;
            string name = string.Format("{0}_{1}", propertyName, uniqueIdentifier);
            var property = sender.GetType().GetProperty(propertyName);

            if (string.IsNullOrEmpty(uniqueIdentifier))
            {
                throw new Exception("Please apply a [ListDataColumnType.UniqueIdentifier] column to the ListDataColumns");
            }

            string enabledTag = null;

            //  Set the visibility/enabled on postback
            if (!CheckState(container, sender, column, ref properyValue, ref enabledTag))
                return;

            bool skipRowOption = column.EditConfiguration.HideTableRowIfChanged;

            string htmlCandidate = null;
            if (column.EditConfiguration.Type == Sushi.Mediakiwi.Framework.ListDataEditConfigurationType.Dropdown)
            {
                if (!string.IsNullOrEmpty(column.EditConfiguration.CollectionProperty))
                {
                    ListItemCollection col =
                      GetProperty(container, sender, column.EditConfiguration.CollectionProperty) as ListItemCollection;

                    object candidate = GetProperty(container, sender, propertyName);
                    string value = candidate == null ? null : candidate.ToString();

                    if (container.Request.HasFormContentType && container.Request.Form.Count > 0 && value != container.Form(name) && HasPostItem(container, name))
                    {
                        value = container.Form(name);

                        if (property.PropertyType == typeof(int))
                            property.SetValue(sender, Utility.ConvertToInt(value), null);
                        else if (property.PropertyType == typeof(int?))
                            property.SetValue(sender, Utility.ConvertToIntNullable(value), null);
                        else
                            property.SetValue(sender, value, null);

                        SetPolution(container, sender);

                        if (skipRowOption)
                            shouldSkipRowPresentation = true;
                    }

                    StringBuilder optionList = new StringBuilder();
                    foreach (var li in col)
                    {
                        if (li.Value == value)
                            optionList.AppendFormat("<option value=\"{1}\" selected=\"selected\">{0}</option>", li.Text, li.Value);
                        else
                            optionList.AppendFormat("<option value=\"{1}\">{0}</option>", li.Text, li.Value);
                    }
                    htmlCandidate = string.Format("<span class=\"inputMode\"><select id=\"{0}\" name=\"{0}\"{2}{3}>{1}</select></span>"
                        , name
                        , optionList.ToString()
                        , column.EditConfiguration.Width > 0 ? string.Format(" style=\"width:{0}px\"", column.EditConfiguration.Width) : null
                        , enabledTag
                        );
                }
                properyValue = htmlCandidate;
            }
            else if (column.EditConfiguration.Type == Sushi.Mediakiwi.Framework.ListDataEditConfigurationType.Checkbox)
            {
                bool? value = null;

                if (property.PropertyType == typeof(bool))
                    value = (bool)property.GetValue(sender, null);
                else if (property.PropertyType == typeof(bool?))
                    value = (bool?)property.GetValue(sender, null);

                if (!value.HasValue)
                {
                    if (column.EditConfiguration.NullableCheckedState)
                        value = true;
                }

                bool formresult = !string.IsNullOrEmpty(container.Form(name));

                if (container.Request.HasFormContentType && container.Request.Form.Count > 0 && (value == null || value != formresult) && HasPostItem(container, name))
                {
                    value = formresult;
                    property.SetValue(sender, value, null);
                    SetPolution(container, sender);

                    if (skipRowOption)
                        shouldSkipRowPresentation = true;
                }

                htmlCandidate = string.Format("<span class=\"inputMode\"><input name=\"{0}\" id=\"{0}\" value\"1\" type=\"checkbox\"{1}{2} /></span>"
                    , name
                    , value.GetValueOrDefault(false) ? " checked=\"checked\"" : null
                    , enabledTag
                    );
                properyValue = htmlCandidate;
            }
            else if (column.EditConfiguration.Type == Sushi.Mediakiwi.Framework.ListDataEditConfigurationType.TextField)
            {
                if (container.Request.HasFormContentType && container.Request.Form.Count > 0 && HasPostItem(container, name) &&
                              ((properyValue == null && !string.IsNullOrEmpty(container.Form(name))) ||
                              (properyValue != null && properyValue.ToString() != container.Form(name)))
                              )
                {
                    properyValue = container.Form(name);
                    sender.GetType().GetProperty(propertyName).SetValue(sender, properyValue, null);

                    SetPolution(container, sender);

                    if (skipRowOption)
                        shouldSkipRowPresentation = true;
                }

                htmlCandidate = string.Format("<span class=\"inputMode\"><input id=\"{0}\" name=\"{0}\" value=\"{1}\" type=\"text\"{2}{3} /></span>&nbsp;<label>{3}</label>"
                    , name
                    , properyValue
                    , column.EditConfiguration.Width == 0 ? string.Empty : string.Format(" style=\"width:{0}px\"", column.EditConfiguration.Width)
                    , propertyHelp
                    , enabledTag
                    );
                properyValue = htmlCandidate;
            }
        }

        bool CheckState(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, object sender, Framework.ListDataColumn column, ref object properyValue, ref string enabledTag)
        {
            bool isEnabled = true;
            enabledTag = string.Empty;
            if (column.EditConfiguration.IsEnabledProperty != null)
            {
                var enabledProp = sender.GetType().GetProperty(column.EditConfiguration.IsEnabledProperty);
                if (enabledProp.PropertyType == typeof(bool))
                    isEnabled = (bool)enabledProp.GetValue(sender, null);

                if (column.EditConfiguration.ReverseEnabledProperty)
                    isEnabled = !isEnabled;

                if (!isEnabled)
                    enabledTag = @" disabled=""disabled""";
            }

            //  Set the visibility
            if (column.EditConfiguration.IsVisibleProperty != null)
            {
                bool isVisible = true;
                var visibleProp = sender.GetType().GetProperty(column.EditConfiguration.IsVisibleProperty);

                if (visibleProp.PropertyType == typeof(bool))
                {
                    if (visibleProp.PropertyType == typeof(bool))
                        isVisible = (bool)visibleProp.GetValue(sender, null);
                }

                if (column.EditConfiguration.ReverseVisibleProperty)
                    isVisible = !isVisible;

                if (!isVisible)
                {
                    properyValue = string.Empty;
                    return false;
                }
            }
            return true;
        }


        System.Reflection.PropertyInfo GetPropertyInfo(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, object sender, string property)
        {
            foreach (System.Reflection.PropertyInfo info in sender.GetType().GetProperties())
            {   //  Get all public properties
                if (info.CanRead)
                {   // Get all writable public properties
                    if (info.Name == property)
                    {
                        return info;
                    }
                }
            }
            //  Fall back
            if (container.CurrentListInstance != sender)
            {
                foreach (System.Reflection.PropertyInfo info in container.CurrentListInstance.GetType().GetProperties())
                {   //  Get all public properties
                    if (info.CanRead)
                    {   // Get all writable public properties
                        if (info.Name == property)
                        {
                            return info;
                        }
                    }
                }
            }
            throw new Exception(string.Format("Could not find the '{0}' property.", property));
        }

        object GetProperty(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, object sender, string property)
        {
            foreach (System.Reflection.PropertyInfo info in sender.GetType().GetProperties())
            {   //  Get all public properties
                if (info.CanRead)
                {   // Get all writable public properties
                    if (info.Name == property)
                    {
                        return info.GetValue(sender, null);
                    }
                }
            }
            //  Fall back
            if (container.CurrentListInstance != sender)
            {
                foreach (System.Reflection.PropertyInfo info in container.CurrentListInstance.GetType().GetProperties())
                {   //  Get all public properties
                    if (info.CanRead)
                    {   // Get all writable public properties
                        if (info.Name == property)
                        {
                            return info.GetValue(container.CurrentListInstance, null);
                        }
                    }
                }
            }
            throw new Exception(string.Format("Could not find the '{0}' property.", property));
        }

        internal string GetThumbnailGridFromListInstance(Framework.WimComponentListRoot root, Sushi.Mediakiwi.Beta.GeneratedCms.Console container, int type, bool includeExportFields)
        {
            //  Trigger list search event
            container.CurrentListInstance.wim.DoListSearch();

            //  if no data is assigned return null 
            if (root.ListDataTable == null && root.ListData == null)
                return null;

            StringBuilder build = new StringBuilder();


            bool isDataTable = root.ListData == null;

            IEnumerator whilelist = null;
            Utilities.Splitlist splitlist;

            int currentPage = Utility.ConvertToInt(container.ListPagingValue, 1);
            if (currentPage < 1) currentPage = 1;

            int maxViewItemCount = root.CurrentList.Option_Search_MaxResultPerPage;
            if (container.ListPagingValue == "-1")
                maxViewItemCount = 1000;

            if (isDataTable)
            {
                splitlist = new Splitlist(root.ListDataTable, maxViewItemCount, root.CurrentList.Option_Search_MaxViews);
                if (currentPage > splitlist.ListCount)
                    currentPage = splitlist.ListCount;

                object arr = splitlist[currentPage - 1];
                if (arr != null)
                    whilelist = ((System.Data.DataTable)arr).Rows.GetEnumerator();
            }
            else
            {
                splitlist = new Splitlist(root.ListData, maxViewItemCount, root.CurrentList.Option_Search_MaxViews);
                if (currentPage > splitlist.ListCount)
                    currentPage = splitlist.ListCount;

                object arr = splitlist[currentPage - 1];
                if (arr != null)
                    whilelist = ((IList)arr).GetEnumerator();
            }
            
            //  TESTDRIVE
            //build.Append("\n\t\t\t\t\t<div class=\"folders\">");
            build.Append("\n\t\t\t\t\t<div class=\"thumbs\">");


            int count = 0;
            if (whilelist != null)
            {
                while (whilelist.MoveNext())
                {
                    count++;
                    object item = whilelist.Current;

                    PropertyInfo[] infoCollection = null;

                    if (!isDataTable)
                        infoCollection = item.GetType().GetProperties();

                    bool isFirst = true;
                    bool isFolder = false;
                    string title = null;
                    string description = null;
                    string thumbnail = null;
                    string html = null;
                    string url = "#";
                    int id = 0;

                    foreach (Framework.ListDataColumn column in root.ListDataColumns.List)
                    {
                        object propertyValue;

                        if (isDataTable)
                            propertyValue = ((System.Data.DataRow)item)[column.ColumnValuePropertyName];
                        else
                            propertyValue = GetValue(infoCollection, item, column);

                        propertyValue = OutputValue(container, propertyValue, column);

                        if (propertyValue != null)
                        {
                            if (column.ColumnValuePropertyName == "Title")
                                title = propertyValue.ToString();
                            else if (column.ColumnValuePropertyName == "Icon")
                                thumbnail = propertyValue.ToString();
                            else if (column.ColumnValuePropertyName == "Info2")
                                description = propertyValue.ToString();
                            else if (column.ColumnValuePropertyName == "Info1")
                                html = propertyValue.ToString();
                            else if (column.ColumnValuePropertyName == "PassThrough") 
                            {
                                if (propertyValue.ToString() == "gallery")
                                    isFolder = true;
                            }
                        }

                        if (isFirst)
                        {
                            string uniqueIdentifier, highlightColumn;
                            if (isDataTable)
                            {
                                uniqueIdentifier = GetIndentifierKey((System.Data.DataRow)item, root.ListDataColumns.List);
                                highlightColumn = GetHighlightedValue((System.Data.DataRow)item, root.ListDataColumns.List);

                                if (highlightColumn != null)
                                    highlightColumn = WebUtility.HtmlEncode(highlightColumn.ToString());

                            }
                            else
                            {
                                uniqueIdentifier = GetIndentifierKey(infoCollection, item, root.ListDataColumns.List);
                                highlightColumn = GetHighlightedValue(infoCollection, item, root.ListDataColumns.List);

                                if (highlightColumn != null)
                                    highlightColumn = WebUtility.HtmlEncode(highlightColumn.ToString());

                            }
                            if (container.CurrentListInstance.wim.HasListLoad)
                            {
                                if (!string.IsNullOrEmpty(root.ListDataColumns.ColumnItemUrl))
                                {
                                    //  Get URL from list item
                                    url = GetValue(infoCollection, item, root.ListDataColumns.ColumnItemUrl).ToString();
                                }
                                else
                                {

                                    if ((type == 1 || type == 2) && !string.IsNullOrEmpty(highlightColumn))
                                    {
                                    }

                                    if (string.IsNullOrEmpty(uniqueIdentifier))
                                        url = "#";
                                    else
                                    {
                                        if (isDataTable)
                                            url = string.Concat(container.WimPagePath, GetPassThroughValue(root, (System.Data.DataRow)item), "=", uniqueIdentifier);
                                        else
                                            url = string.Concat(container.WimPagePath, GetPassThroughValue(infoCollection, root, item), "=", uniqueIdentifier);
                                    }

                                    if (type > 0)
                                    {
                                        //  Paging in a popup layer requires some addition url parameters
                                        url = string.Concat(url, "&openinframe=", container.Request.Query["openinframe"], "&referid=", container.Request.Query["referid"]);
                                    }
                                }
                            }
                            id = Convert.ToInt32(uniqueIdentifier);
                        }
                        isFirst = false;
                    }

                    if (string.IsNullOrEmpty(html))
                    {
                        build.AppendFormat("\n\t\t\t\t\t\t<a href=\"{2}\"{3}>\n\t\t\t\t\t\t\t<figure><img alt=\"Folder\" src=\"{1}\"></figure>\n\t\t\t\t\t\t\t<strong>{0}</strong>\n\t\t\t\t\t\t</a>{4}"
                            , title
                            , thumbnail
                            , url
                            , count % 5 == 0 ? " class=\"last\"" : string.Empty
                            , count % 5 == 0 ? "<br class=\"clear\">" : string.Empty
                            );
                    }
                    else
                    {
                        build.Append(html);
                    }
                }
            }

            build.Append("\n\t\t\t\t\t</div>");
            return build.ToString();
        }


        /// <summary>
        /// Determines whether [is key column] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// 	<c>true</c> if [is key column] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        bool IsKeyColumn(Framework.ListDataColumnType type)
        {
            if (type == Sushi.Mediakiwi.Framework.ListDataColumnType.UniqueHighlightedIdentifier ||
                type == Sushi.Mediakiwi.Framework.ListDataColumnType.UniqueHighlightedIdentifierPresent ||
                type == Sushi.Mediakiwi.Framework.ListDataColumnType.UniqueIdentifier ||
                type == Sushi.Mediakiwi.Framework.ListDataColumnType.UniqueIdentifierPresent)
                return true;
            return false;
        }

        /// <summary>
        /// Determines whether [is highlight column] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// 	<c>true</c> if [is highlight column] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        bool IsHighlightColumn(Framework.ListDataColumnType type)
        {
            if (type == Sushi.Mediakiwi.Framework.ListDataColumnType.UniqueHighlightedIdentifier ||
                type == Sushi.Mediakiwi.Framework.ListDataColumnType.UniqueHighlightedIdentifierPresent ||
                type == Sushi.Mediakiwi.Framework.ListDataColumnType.Highlight ||
                type == Sushi.Mediakiwi.Framework.ListDataColumnType.HighlightPresent)
                return true;
            return false;
        }

        /// <summary>
        /// Gets the pass through value.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        string GetPassThroughValue(Framework.WimComponentListRoot root, System.Data.DataRow item)
        {
            if (!string.IsNullOrEmpty(root.SearchResultItemPassthroughParameterProperty))
            {
                return item[root.SearchResultItemPassthroughParameterProperty].ToString();
            }
            if (!string.IsNullOrEmpty(root.SearchResultItemPassthroughParameter))
            {
                return root.SearchResultItemPassthroughParameter;
            }
            return "item";
        }

        /// <summary>
        /// Gets the pass through value.
        /// </summary>
        /// <param name="infoCollection">The info collection.</param>
        /// <param name="root">The root.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        string GetPassThroughValue(PropertyInfo[] infoCollection, Framework.WimComponentListRoot root, object item)
        {
            if (!string.IsNullOrEmpty(root.SearchResultItemPassthroughParameterProperty))
            {
                for (int index = 0; index < infoCollection.Length; index++)
                {
                    System.Reflection.PropertyInfo info = infoCollection[index];
                    if (info.Name == root.SearchResultItemPassthroughParameterProperty)
                    {
                        return info.GetValue(item, null).ToString();
                    }
                }
            }
            if (!string.IsNullOrEmpty(root.m_SearchResultItemPassthroughParameter))
            {
                return root.SearchResultItemPassthroughParameter;
            }
            return null;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="infoCollection">The info collection.</param>
        /// <param name="item">The item.</param>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        object GetValue(PropertyInfo[] infoCollection, object item, Framework.ListDataColumn column)
        {
            System.Reflection.PropertyInfo info;
            return GetValue(infoCollection, item, column.ColumnValuePropertyName, column.DataType, out info);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="infoCollection">The info collection.</param>
        /// <param name="item">The item.</param>
        /// <param name="column">The column.</param>
        /// <param name="info">The info.</param>
        /// <returns></returns>
        object GetValue(PropertyInfo[] infoCollection, object item, Framework.ListDataColumn column, out System.Reflection.PropertyInfo info)
        {
            return GetValue(infoCollection, item, column.ColumnValuePropertyName, column.DataType, out info);
        }


        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="infoCollection">The info collection.</param>
        /// <param name="item">The item.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        object GetValue(PropertyInfo[] infoCollection, object item, string propertyName)
        {
            System.Reflection.PropertyInfo info;
            return GetValue(infoCollection, item, propertyName, null, out info);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="infoCollection">The info collection.</param>
        /// <param name="item">The item.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="info">The info.</param>
        /// <returns></returns>
        object GetValue(PropertyInfo[] infoCollection, object item, string propertyName, out System.Reflection.PropertyInfo info)
        {
            return GetValue(infoCollection, item, propertyName, null, out info);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="infoCollection">The info collection.</param>
        /// <param name="item">The item.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="castType">Type of the cast.</param>
        /// <param name="info">The info.</param>
        /// <returns></returns>
        object GetValue(PropertyInfo[] infoCollection, object item, string propertyName, Type castType, out System.Reflection.PropertyInfo info)
        {
            return GetValue(infoCollection, item, propertyName, castType, true, out info);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="infoCollection">The info collection.</param>
        /// <param name="item">The item.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="castType">Type of the cast.</param>
        /// <param name="ifNullRecallWithData">if set to <c>true</c> [if null recall with data].</param>
        /// <param name="info">The info.</param>
        /// <returns></returns>
        object GetValue(PropertyInfo[] infoCollection, object item, string propertyName, Type castType, bool ifNullRecallWithData, out System.Reflection.PropertyInfo info)
        {
            info = null;
            string indexer = null;
            if (propertyName != null && propertyName.Contains("."))
            {
                indexer = propertyName.Split('.')[1];
                propertyName = propertyName.Split('.')[0];
            }

            for (int index = 0; index < infoCollection.Length; index++)
            {
                info = infoCollection[index];

                if (info.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
                {
                    //  The indexer is the object designation, f.e. Data.FullName. Data is a reserved word and will open a CustomData reference entity.
                    if (indexer != null)
                    {
                        if (propertyName.Equals("data", StringComparison.OrdinalIgnoreCase))
                        {
                            Sushi.Mediakiwi.Data.CustomData cst = info.GetValue(item, null) as Sushi.Mediakiwi.Data.CustomData;

                            if (cst == null) return null;

                            if (castType == typeof(DateTime))
                                return cst[indexer].ParseDateTime();
                            else if (castType == typeof(Decimal))
                                return cst[indexer].ParseDecimal();
                            else if (castType == typeof(int))
                                return cst[indexer].ParseInt();
                            else if (castType == typeof(Guid))
                                return cst[indexer].ParseGuid();
                            else if (!string.IsNullOrEmpty(cst[indexer].Value) && cst[indexer].Value.Contains(@"<?xml") && cst[indexer].ParseSubList() != null && cst[indexer].ParseSubList().Items != null && cst[indexer].ParseSubList().Items.Length > 0)
                                return cst[indexer].ParseSubList().Items[0].Description;
                            else
                                return cst[indexer].Value;
                        }
                        else
                        {
                            //  The initiated entities is stored seperatly to avoid additional initiation.
                            //if (string.IsNullOrEmpty(m_InnerPropertyName) || m_InnerPropertyName != propertyName)
                            //{
                                m_InnerObj = info.GetValue(item, null);
                                if (m_InnerObj != null)
                                {
                                    m_InfoCollection2 = m_InnerObj.GetType().GetProperties();

                                    m_InnerPropertyName = propertyName;
                                    //}

                                    for (int index2 = 0; index2 < m_InfoCollection2.Length; index2++)
                                    {
                                        System.Reflection.PropertyInfo info2 = m_InfoCollection2[index2];
                                        if (info2.Name == indexer)
                                        {
                                            return info2.GetValue(m_InnerObj, null);
                                        }
                                    }
                                }
                            return null;
                        }
                    }

                    //column.PropertyIndex = index;
                    if (info.PropertyType != null && info.PropertyType.BaseType != null)
                    {
                        if (info.PropertyType.BaseType.Equals(typeof(System.Enum)))
                        {
                            return (int)info.GetValue(item, null);
                        }
                    }

                    return info.GetValue(item, null);
                }
            }
            if (ifNullRecallWithData && indexer == null)
                return GetValue(infoCollection, item, string.Concat("data.", propertyName), castType, false, out info);
            
            return null;
        }
        string m_InnerPropertyName;
        PropertyInfo[] m_InfoCollection2;
        Object m_InnerObj;

        /// <summary>
        /// Gets the highlighted value.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        string GetHighlightedValue(System.Data.DataRow item, List<Framework.ListDataColumn> list)
        {
            foreach (Framework.ListDataColumn column in list)
            {
                
                if (IsHighlightColumn(column.Type))
                {
                    return item[column.ColumnValuePropertyName].ToString();
                }
            }
            return null;
            //throw new Exception(string.Format("Cound not find the object highlight on [{0}]", item.GetType()));
        }

        string GetHighlightedColumn(System.Data.DataRow item, List<Framework.ListDataColumn> list)
        {
            foreach (Framework.ListDataColumn column in list)
            {
                if (IsHighlightColumn(column.Type))
                {
                    return column.ColumnValuePropertyName;
                }
            }
            return null;
            //throw new Exception(string.Format("Cound not find the object highlight on [{0}]", item.GetType()));
        }

        /// <summary>
        /// Gets the indentifier key.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        string GetIndentifierKey(System.Data.DataRow item, List<Framework.ListDataColumn> list)
        {
            foreach (Framework.ListDataColumn column in list)
            {
                if (IsKeyColumn(column.Type))
                {
                    return item[column.ColumnValuePropertyName].ToString();
                }
            }
            return null;
            //throw new Exception(string.Format("Cound not find the object indentifier on [{0}]", item.GetType()));
        }

        /// <summary>
        /// Gets the highlighted value.
        /// </summary>
        /// <param name="infoCollection">The info collection.</param>
        /// <param name="item">The item.</param>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        string GetHighlightedValue(PropertyInfo[] infoCollection, object item, List<Framework.ListDataColumn> list)
        {
            foreach (Framework.ListDataColumn column in list)
            {
                if (IsHighlightColumn(column.Type))
                {
                    try
                    {
                        object x = GetValue(infoCollection, item, column);
                        if (x == null)
                            return "[no data]";

                        return GetValue(infoCollection, item, column).ToString();
                    }
                    catch (Exception)
                    {
                        return "[no data]";
                        //throw new Exception(string.Format("The column '{0}' with refering property '{1}' can not be NULL when set as highlighted column.", column.ColumnName, column.ColumnValuePropertyName));
                    }
                }
            }
            return null;
            //throw new Exception(string.Format("Cound not find the object highlight on [{0}]", item.GetType()));
        }

        string GetHighlightedColumn(PropertyInfo[] infoCollection, object item, List<Framework.ListDataColumn> list)
        {
            foreach (Framework.ListDataColumn column in list)
            {
                if (IsHighlightColumn(column.Type))
                    return column.ColumnValuePropertyName;
            }
            return null;
            //throw new Exception(string.Format("Cound not find the object highlight on [{0}]", item.GetType()));
        }

        /// <summary>
        /// Gets the indentifier key.
        /// </summary>
        /// <param name="infoCollection">The info collection.</param>
        /// <param name="item">The item.</param>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        string GetIndentifierKey(PropertyInfo[] infoCollection, object item, List<Framework.ListDataColumn> list)
        {
            foreach (Framework.ListDataColumn column in list)
            {
                if (IsKeyColumn(column.Type))
                {
                    var candidate = GetValue(infoCollection, item, column);
                    if (candidate != null)
                        return candidate.ToString();
                }
            }
            return null;
            //throw new Exception(string.Format("Cound not find the object indentifier on [{0}]", item.GetType()));
        }

        bool GetHighlightedState(PropertyInfo[] infoCollection, object item, string property)
        {
            if (string.IsNullOrEmpty(property)) return false;
            return Convert.ToBoolean(GetValue(infoCollection, item, property));
            //throw new Exception(string.Format("Cound not find the object indentifier on [{0}]", item.GetType()));
        }


        string GetTableRowClassName(PropertyInfo[] infoCollection, object item, string property)
        {
            if (string.IsNullOrEmpty(property)) return null;
            return GetValue(infoCollection, item, property) as string;
            //throw new Exception(string.Format("Cound not find the object indentifier on [{0}]", item.GetType()));
        }

        /// <summary>
        /// Gets the indentifier key.
        /// </summary>
        /// <param name="infoCollection">The info collection.</param>
        /// <param name="item">The item.</param>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        string GetColumnUrl(PropertyInfo[] infoCollection, object item, List<Framework.ListDataColumn> list)
        {
            foreach (Framework.ListDataColumn column in list)
            {
                if (IsKeyColumn(column.Type))
                {
                    return GetValue(infoCollection, item, column).ToString();
                }
            }
            return null;
            //throw new Exception(string.Format("Cound not find the object indentifier on [{0}]", item.GetType()));
        }

        /// <summary>
        /// Determines whether [is visible column] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// 	<c>true</c> if [is visible column] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        bool IsVisibleColumn(Framework.ListDataColumnType type)
        {
            if (type == Sushi.Mediakiwi.Framework.ListDataColumnType.ExportOnly ||
                type == Sushi.Mediakiwi.Framework.ListDataColumnType.Highlight ||
                type == Sushi.Mediakiwi.Framework.ListDataColumnType.UniqueHighlightedIdentifier ||
                type == Sushi.Mediakiwi.Framework.ListDataColumnType.UniqueIdentifier)
                return false;

            return true;
        }

        /// <summary>
        /// Determines whether [is visible column] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="includeExportFields">if set to <c>true</c> [include export fields].</param>
        /// <returns>
        /// 	<c>true</c> if [is visible column] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        bool IsVisibleColumn(Framework.ListDataColumnType type, bool includeExportFields)
        {
            if (includeExportFields && type == Sushi.Mediakiwi.Framework.ListDataColumnType.ExportOnly && type != Framework.ListDataColumnType.ViewOnly)
                return true;

            return IsVisibleColumn(type);
        }


        /// <summary>
        /// Determines whether [is visible export column] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if [is visible export column] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        bool IsVisibleExportColumn(Framework.ListDataColumnType type)
        {
            if (type == Sushi.Mediakiwi.Framework.ListDataColumnType.Highlight ||
                type == Sushi.Mediakiwi.Framework.ListDataColumnType.UniqueHighlightedIdentifier ||
                type == Sushi.Mediakiwi.Framework.ListDataColumnType.UniqueIdentifier ||
                type == Sushi.Mediakiwi.Framework.ListDataColumnType.ViewOnly)
                return false;

            return true;
        }

        /// <summary>
        /// Outputs the value.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="candidate">The candidate.</param>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        object OutputValue(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, object candidate, Framework.ListDataColumn column)
        {
            string cellClassName;
            return OutputValue(container, candidate, true, out cellClassName, column);
        }

        /// <summary>
        /// Outputs the value.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="candidate">The candidate.</param>
        /// <param name="cellClassName">Name of the cell class.</param>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        object OutputValue(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, object candidate, out string cellClassName, Framework.ListDataColumn column)
        {
            return OutputValue(container, candidate, true, out cellClassName, column);
        }

        /// <summary>
        /// Outputs the value.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="candidate">The candidate.</param>
        /// <param name="convertToWimDecimal">if set to <c>true</c> [convert to wim decimal].</param>
        /// <param name="cellClassName">Name of the cell class.</param>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        object OutputValue(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, object candidate, bool convertToWimDecimal, out string cellClassName, Framework.ListDataColumn column)
        {
            cellClassName = null;

            if (candidate == null) return null;

            if (candidate.GetType() == typeof(DateTime))
            {
                if (((DateTime)candidate) == DateTime.MinValue) return null;
                DateTime tmp = ((DateTime)candidate);
                // [MR:20-03-2019] Converts UTC (database) time to local timezone for display
                // 19-10-20 turned off core.
                //if (container.CurrentList.Option_ConvertUTCToLocalTime && tmp.Kind != DateTimeKind.Local)
                //    tmp = AppCentre.Data.Supporting.LocalDateTime.GetDate(tmp, container.CurrentListInstance.wim.CurrentSite, true);

                if (tmp.Hour == 0 && tmp.Minute == 0 && tmp.Second == 0 && tmp.Millisecond == 0)
                    return tmp.ToString(container.DateFormatShort);
                return tmp.ToString(container.DateTimeFormatShort);
            }
            else if (candidate.GetType() == typeof(DateTime?))
            {
                if (!((DateTime?)candidate).HasValue) return null;
                DateTime tmp = ((DateTime?)candidate).Value;
                
                // [MR:20-03-2019] Converts UTC (database) time to local timezone for display
                // 19-10-20 turned off core.
                //if (container.CurrentList.Option_ConvertUTCToLocalTime && tmp.Kind != DateTimeKind.Local)
                //    tmp = AppCentre.Data.Supporting.LocalDateTime.GetDate(tmp, container.CurrentListInstance.wim.CurrentSite, true);

                if (tmp.Hour == 0 && tmp.Minute == 0 && tmp.Second == 0 && tmp.Millisecond == 0)
                    return tmp.ToString(container.DateFormatShort);
                return tmp.ToString(container.DateTimeFormatShort);
            }
            else if (candidate.GetType() == typeof(Boolean))
            {
                if (column.ColumnWidth == 0)
                    column.ColumnWidth = 10;

                return ((Boolean)candidate)
                    ? Utils.GetIconImageString(container, Utils.IconImage.Yes)
                    : Utils.GetIconImageString(container, Utils.IconImage.No);
            }
            else if (candidate.GetType() == typeof(Decimal))
            {
                if (convertToWimDecimal)
                {
                    System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("EN-us");
                    return ((Decimal)candidate).ToString("N");
                }
                else
                    return ((Decimal)candidate).ToString("N");
            }
            return candidate;
        }

    }
}