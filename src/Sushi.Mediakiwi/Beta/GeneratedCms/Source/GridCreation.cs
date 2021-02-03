using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Wim.SyncFusion.XlsIO;
using System.Globalization;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.Beta.GeneratedCms.Source
{
    /// <summary>
    /// 
    /// </summary>
    public class GridCreation
    {
        /// <summary>
        /// Gets the single item grid from list instance.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="container">The container.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        internal string GetSingleItemGridFromListInstance(Framework.WimComponentListRoot root, Console container, int type)
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
                        highlightColumn = container.Context.Server.HtmlEncode(highlightColumn.ToString());
                }
                else
                {
                    uniqueIdentifier = GetIndentifierKey(infoCollection, item, root.ListDataColumns.List);
                    highlightColumn = GetHighlightedValue(infoCollection, item, root.ListDataColumns.List);

                    if (highlightColumn != null)
                        highlightColumn = container.Context.Server.HtmlEncode(highlightColumn.ToString());
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

        string GetGridFromListInstanceSyncFusionExport(Framework.WimComponentListRoot root, Console container, int type)
        {
            //  Trigger list search event
            container.CurrentListInstance.wim.DoListSearch();

            //  if no data is assigned return null 
            if (root.ListDataTable == null && root.ListData == null)
                return null;

            //New instance of XlsIO is created.[Equivalent to launching MS Excel with no workbooks open].
            //The instantiation process consists of two steps.

            //Step 1 : Instantiate the spreadsheet creation engine.
            ExcelEngine excelEngine = new ExcelEngine();
            //Step 2 : Instantiate the excel application object.
            IApplication application = excelEngine.Excel;

            // [MR:010713] Calculate number of worksheets, based on the Amount of Grids.
            // if no additional grids are available, revert to count 1
            int NumberWorkSheets = (root.GridCount > 0) ? root.GridCount + 1 : 1;
            int currentSheet = 0;

            // [MR:010713] Create a new XLS workbook with [NumberWorkSheets] sheets
            IWorkbook workbook = excelEngine.Excel.Workbooks.Create(NumberWorkSheets);
            workbook.StandardFont = "Arial";
            workbook.Author = container.CurrentApplicationUser.Displayname;

            // [MR:010713] Loop through available grids
            while (root.NextGrid())
            {
                //The first worksheet object in the worksheets collection is accessed.
                IWorksheet sheet = workbook.Worksheets[currentSheet];

                string columnlist = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,AA,AB,AC,AD,AE,AF,AG,AH,AI,AJ,AK,AL,AM,AN,AO,AP,AQ,AR,AS,AT,AU,AV,AW,AX,AY,AZ";
                string[] columns = columnlist.Split(',');

                int currentRow = 1;
                int currentCol = 0;

                bool hasExportColumns = root.CurrentList.Data["wim_ExportCol_XLS"].ParseBoolean(true);

                foreach (Framework.ListDataColumn column in root.ListDataColumns.List)
                {
                    if (!IsVisibleExportColumn(column.Type))
                        continue;

                    currentCol++;

                    if (hasExportColumns)
                        sheet.Range[string.Concat(columns[currentCol - 1], currentRow)].Text = column.ColumnName;
                }

                // [MR:010713] Set sheet title to current DataList title
                if (!string.IsNullOrEmpty(root.m_DataTitle))
                {
                    try
                    {
                        sheet.Name = root.m_DataTitle;
                    }
                    catch (Exception ex)
                    {
                        string error = ex.Message;
                    }
                }

                if (!hasExportColumns)
                    currentRow = 0;

                int columnCount = currentCol;
                bool isDataTable = root.ListData == null;

                IEnumerator whilelist;
                if (isDataTable)
                    whilelist = root.ListDataTable.Rows.GetEnumerator();
                else
                    whilelist = root.ListData.GetEnumerator();

                try
                {
                    while (whilelist.MoveNext())
                    {
                        currentCol = 1;
                        currentRow++;

                        object item = whilelist.Current;

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

                            if (!IsVisibleExportColumn(column.Type))
                                continue;

                            if (propertyValue == null)
                            {
                                currentCol++;
                                continue;
                            }

                            System.Type propType = propertyValue.GetType();
                            //propertyValue = OutputValue(container, propertyValue, false);

                            if (propType == typeof(String))
                            {
                                //string itemvalue = Utility.CleanFormatting(propertyValue.ToString().Replace("\t", " ").Replace("\n", " ").Replace("\r", string.Empty));
                                string itemvalue = Utility.CleanFormatting(propertyValue.ToString().Replace("\t", string.Empty).Replace("\r", string.Empty));


                                if (!string.IsNullOrEmpty(itemvalue))
                                    sheet.Range[string.Concat(columns[currentCol - 1], currentRow)].Text = itemvalue;
                            }
                            else if (propType == typeof(DateTime))
                            {
                                DateTime dt = (DateTime)propertyValue;
                                if (dt != DateTime.MinValue)
                                    sheet.Range[string.Concat(columns[currentCol - 1], currentRow)].Value2 = propertyValue;
                            }
                            else
                            {
                                sheet.Range[string.Concat(columns[currentCol - 1], currentRow)].Value2 = propertyValue;
                            }
                            currentCol++;
                        }

                    }
                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                }

                if (hasExportColumns)
                {
                    sheet.Range[string.Concat("A1:", columns[columnCount - 1], 1)].CellStyle.Font.Bold = true;
                    sheet.AutoFilters.FilterRange = sheet.Range[string.Concat("A1:", columns[columnCount - 1], currentRow)];
                }

                sheet.Range[string.Concat("A1:", columns[columnCount - 1], 1)].AutofitColumns();

                foreach (IRange range in sheet.Columns)
                {
                    if (range.ColumnWidth > 60)
                    {
                        sheet.Range[string.Concat(columns[range.Column - 1], "1")].ColumnWidth = 60;
                    }

                    if (hasExportColumns)
                        sheet.Range[string.Concat(columns[range.Column - 1], "1:", columns[range.Column - 1], currentRow)].CellStyle.WrapText = true;
                }

                sheet.Range[string.Concat("A1:", columns[columnCount - 1], currentRow)].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                //sheet.Range[string.Concat("A1:", columns[columnCount - 1], currentRow)].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
                currentSheet++;
            }
            

            //Saving the workbook to disk.
            string filename = string.Format("{0}_{1}.xls", container.CurrentList.Name.Replace(" ", "_"), DateTime.Now.ToString("yyyyMMddmmHH"));

            string path = container.CurrentListInstance.wim.GetTemporaryFilePath(filename, true, out filename);

            ////container.Response.Flush();
            ////workbook.SaveAs(container.Response.OutputStream, ExcelSaveType.SaveAsXLS);
            ////container.Response.End();
            //workbook.SaveAs(path);
            ////workbook.SaveAs(filename, ExcelSaveType.SaveAsXLS, container.Response, ExcelDownloadType.PromptDialog);
            ////No exception will be thrown if there are unsaved workbooks.
            //excelEngine.ThrowNotSavedOnDestroy = true;
            //excelEngine.Dispose();
            //return filename;

            workbook.SaveAs(path);
            excelEngine.ThrowNotSavedOnDestroy = true;
            excelEngine.Dispose();

            string url = Wim.Utility.AddApplicationPath(string.Concat(Wim.CommonConfiguration.RelativeRepositoryTmpUrl, filename));

            return url;

        }

        /// <summary>
        /// Gets the grid from list instance export.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="container">The container.</param>
        /// <param name="type">The type.</param>
        internal string GetGridFromListInstanceExport(Framework.WimComponentListRoot root, Console container, int type)
        {
            return GetGridFromListInstanceSyncFusionExport(root, container, type);
        }

        /// <summary>
        /// Gets the grid from list instance.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="container">The container.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public string GetGridFromListInstance(Framework.WimComponentListRoot root, Console container, int type, bool isNewDesignOutput)
        {
            return GetGridFromListInstance(root, container, type, false, isNewDesignOutput);
        }

        string GetListPaging(Console container, Utilities.Splitlist splitlist, int currentPage)
        {
            //if (container.CurrentListInstance.wim.HidePaging) return null;

            if (container.ListPagingValue == "-1")
            {
                if (Wim.CommonConfiguration.IS_AJAX_ENABLED_IN_WIM)
                    return string.Format(@"<ul class=""pager pagerIsHigher""><li><a href=""{0}"" class=""_ReloadFromUrl target_main"">Reset paging</a></li></ul>", BuildUrl(container, "set", "1"));
                else
                    return string.Format(@"<ul class=""pager pagerIsHigher""><li><a href=""{0}"">Reset paging</a></li></ul>", BuildUrl(container, "set", "1"));
            }

            int listCount = container.CurrentListInstance.wim.m_IsLinqUsed ?
                container.CurrentListInstance.wim.m_ListDataRecordPageCount : (splitlist == null ? 0 : splitlist.ListCount);

            if ((splitlist == null && !container.CurrentListInstance.wim.m_IsLinqUsed) || listCount < 2) return null;

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

            //  Create all page reference entries
            int itemCount = 0;
            

            StringBuilder paging = new StringBuilder();
            paging.Append(@"<ul class=""pager pagerIsHigher"">");

            GetPageLink(container, ref paging, "Back", (currentPage - 1), "previous", (currentPage != 1));
            GetPageLink(container, ref paging, "Next", (currentPage + 1), "next", (currentPage != listCount));

            while (itemCount < maxPageReferenceCount)
            {
                itemCount++;
                if (itemCount == 1)
                {
                    GetPageLink(container, ref paging, currentPage == 1, startPageReference);
                }


                else if (itemCount == maxPageReferenceCount || startPageReference == listCount)
                {
                    GetPageLink(container, ref paging, currentPage == startPageReference, startPageReference);
                    break;
                }
                else
                {
                    GetPageLink(container, ref paging, currentPage == startPageReference, startPageReference);
                }
                startPageReference++;
            }

            if (container.CurrentList.Option_HasShowAll)
            {
                if (Wim.CommonConfiguration.IS_AJAX_ENABLED_IN_WIM)
                    paging.AppendFormat("<li>| <a href=\"{0}\" class=\"_ReloadFromUrl target_main\">Show all</a></li>", BuildUrl(container, "set", "all"));
                else
                    paging.AppendFormat("<li>| <a href=\"{0}\">Show all</a></li>", BuildUrl(container, "set", "all"));
            }

            paging.Append(@"</ul>");

            return paging.ToString();
        }

        void GetPageLink(Console container, ref StringBuilder paging, bool isSelected, int page)
        {
            if (isSelected)
            {
                paging.AppendFormat("\n<li><a class=\"active\">{0}</a></li>", page);
                return;
            }
            if (Wim.CommonConfiguration.IS_AJAX_ENABLED_IN_WIM)
                paging.AppendFormat("\n<li><a href=\"{1}\" class=\"_ReloadFromUrl target_main\">{0}</a></li>", page, BuildUrl(container, "set", page.ToString()));
            else
                paging.AppendFormat("\n<li><a href=\"{1}\" class=\"\">{0}</a></li>", page, BuildUrl(container, "set", page.ToString()));
        }

        void GetPageLink(Console container, ref StringBuilder paging, string text, int page, string additionalClass, bool isEnabled)
        {
            if (Wim.CommonConfiguration.IS_AJAX_ENABLED_IN_WIM)
                paging.AppendFormat("\n<li><a href=\"{1}\"{3} class=\"{2} _ReloadFromUrl target_main\">{0}</a></li>", text, isEnabled ? BuildUrl(container, "set", page.ToString()) : "#", additionalClass, isEnabled ? null : " disabled=\"disabled\"");
            else
                paging.AppendFormat("\n<li><a href=\"{1}\"{3} class=\"{2}\">{0}</a></li>", text, isEnabled ? BuildUrl(container, "set", page.ToString()) : "#", additionalClass, isEnabled ? null : " disabled=\"disabled\"");

        }

        string BuildUrl(Console container, string queryStringPropertyToApply, string value)
        {
            string url = container.WimPagePath;

            bool first = true, foundKey = false;
            if (container.Request.QueryString.Count > 0)
            {
                string[] keys = container.Request.QueryString.AllKeys;
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
                        candidate = container.Request.QueryString[key];

                    if (first)
                        url += string.Concat("?", key, "=", candidate);
                    else
                        url += string.Concat("&", key, "=", candidate);

                    first = false;
                }
            }

            if (!foundKey)
            {
                if (first)
                    url += string.Concat("?", queryStringPropertyToApply, "=", value);
                else
                    url += string.Concat("&", queryStringPropertyToApply, "=", value);
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
                    throw new Exception("One of the items in Wim.ListData is NULL");

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

                    if (propertyValue != null)
                    {
                        if (propertyValue.GetType() == typeof(Decimal) || propertyValue.GetType() == typeof(int))
                        {
                            
                            if (column.Total == Sushi.Mediakiwi.Framework.ListDataTotalType.Sum || column.Total == Sushi.Mediakiwi.Framework.ListDataTotalType.Average)
                            {
                                column.TotalValueType = propertyValue.GetType();
                                hasTotal = true;
                     
                                column.TotalValue += Utility.ConvertToDecimal(propertyValue);
                            }
                        }
                    }
                }
                if (!hasTotal) break;
            }
            return hasTotal;
        }

        /// <summary>
        /// Gets the grid from list instance.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="container">The container.</param>
        /// <param name="type">The type. (0 = normal, 1 = popuplayer, 2 = popuplayer without paging, 3 = dashboard[middle] )</param>
        /// <param name="includeExportFields">if set to <c>true</c> [include export fields].</param>
        /// <param name="isNewDesignOutput">if set to <c>true</c> [is new design output].</param>
        /// <returns></returns>
        internal string GetGridFromListInstance(Framework.WimComponentListRoot root, Console container, int type, bool includeExportFields, bool isNewDesignOutput)
        {
            container.ListPagingValue = root.CurrentPage.ToString();// container.Request.Params["set"];

            bool isEditMode = false;
            bool sortOrderIsActive = container.IsSortorderOn;

            //  Trigger list search event
            container.CurrentListInstance.wim.DoListSearch();

            //  Dashboard graph
            //if ((type == 4 || type == 3) && !string.IsNullOrEmpty(container.CurrentListInstance.wim.DashBoardHtmlContainer))
            //{
            //    return container.CurrentListInstance.wim.DashBoardHtmlContainer;
            //}

            //  if no data is assigned return null 
            if (root.ListDataTable == null && root.ListData == null)
                return null;

            StringBuilder build = new StringBuilder();
            

            //  Create the header and the columns
            if (type != 4) 
                build.Append("\n<thead>\n<tr>");
            int visibleColumnCount = 0;

            bool hasEditProperties = false;
            
            foreach (Framework.ListDataColumn column in root.ListDataColumns.List)
            {
                //if (column.EditConfiguration != null)
                //    hasEditProperties = true;

                if (!IsVisibleColumn(column.Type, includeExportFields))
                    continue;

                visibleColumnCount++;

                if (type != 4)
                {
                    if (column.EditConfiguration != null && column.EditConfiguration.Type == ListDataEditConfigurationType.Checkbox)
                        build.AppendFormat("\n\t<th><input type=\"checkbox\" class=\"checkall\"></th>");
                    else
                        build.AppendFormat("\n\t<th>{0}</th>", column.ColumnName);
                }
            }

            if (visibleColumnCount == 0)
                return string.Format("<table class=\"data\"><tfoot><tr><td>{0}</td></tr></tfoot></table>"
                        , Resource.ResourceManager.GetString("grid_no_search_columns", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                    );

            if (type != 4)
                build.Append("\n</tr>\n</thead>");

            bool isDataTable = root.ListData == null;

            int count = isDataTable ? root.ListDataTable.Rows.Count : root.ListData.Count;

            //  Create the content
            string className = "odd";

            IEnumerator whilelist = null;

            int currentPage = Utility.ConvertToInt(container.ListPagingValue, 1);
            if (currentPage < 1) currentPage = 1;
            string paging = null;

            if (root.m_IsLinqUsed)
            {
                count = root.m_ListDataRecordCount - root.m_ListDataInterLineCount;
                whilelist = root.ListData.GetEnumerator();
                
                if (!root.IsDashboardMode)
                    paging = GetListPaging(container, null, currentPage);
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
                        splitlist = new Wim.Utilities.Splitlist(root.ListDataTable, maxViewItemCount, root.CurrentList.Option_Search_MaxViews);
                        if (currentPage > splitlist.ListCount)
                            currentPage = splitlist.ListCount;

                        whilelist = ((System.Data.DataTable)splitlist[currentPage - 1]).Rows.GetEnumerator();
                    }
                }
                else
                {
                    if (root.ListData.Count > 0)
                    {
                        splitlist = new Wim.Utilities.Splitlist(root.ListData, maxViewItemCount, root.CurrentList.Option_Search_MaxViews);
                        if (currentPage > splitlist.ListCount)
                            currentPage = splitlist.ListCount;

                        whilelist = ((IList)splitlist[currentPage - 1]).GetEnumerator();
                    }
                }
                if (!root.IsDashboardMode)
                    paging = GetListPaging(container, splitlist, currentPage);
            }


            StringBuilder build2 = new StringBuilder();
            StringBuilder RowHTML = null;

            bool hasTotal = ApplyTotalInformation(isDataTable, root);
            int index = -1;
            if (whilelist != null)
            {
                ListDataSoure source = new ListDataSoure();
                source.DataEntities = whilelist;
                source.VisibleColumns = visibleColumnCount;

                while (whilelist.MoveNext())
                {
                    index++;
                    bool shouldSkipRowPresentation = false;

                    RowHTML = new StringBuilder();

                    object item = whilelist.Current;

                    if (item == null) continue;

                    if (root.Page.Body.Grid.Table.IgnoreCreation)
                    {
                        string uniqueIdentifier = null, highlightColumn = null;

                        PropertyInfo[] infoCollection = null;

                        if (!isDataTable)
                            infoCollection = item.GetType().GetProperties();

                        if (isDataTable)
                        {
                            uniqueIdentifier = GetIndentifierKey((System.Data.DataRow)item, root.ListDataColumns.List);
                            highlightColumn = GetHighlightedValue((System.Data.DataRow)item, root.ListDataColumns.List);

                            if (highlightColumn != null)
                                highlightColumn = container.Context.Server.HtmlEncode(highlightColumn.ToString());

                        }
                        else
                        {
                            uniqueIdentifier = GetIndentifierKey(infoCollection, item, root.ListDataColumns.List);
                            highlightColumn = GetHighlightedValue(infoCollection, item, root.ListDataColumns.List);

                            if (highlightColumn != null)
                                highlightColumn = container.Context.Server.HtmlEncode(highlightColumn.ToString());

                        }

                        var parser = container.CurrentListInstance.wim.DoListDataItemCreated(DataItemType.Custom, root.ListDataColumns.List.ToArray(), null, item, uniqueIdentifier, null, index, null, source);
                        build2.Append(parser.InnerHTML);
                    }
                    else
                    {
                        #region Normal Table>Tr>Td flow
                        PropertyInfo[] infoCollection = null;

                        if (!isDataTable)
                            infoCollection = item.GetType().GetProperties();

                        if (className == "odd") className = "even";
                        else className = "odd";

                        if (hasEditProperties)
                            className += " autoSubmit";


                        bool isFirst = true;

                        string uniqueIdentifier = null, highlightColumn = null;

                        string classNameAddition =
                            this.GetTableRowClassName(infoCollection, item, container.CurrentListInstance.wim.ListDataClassNamePropertyValue);

                        if (!string.IsNullOrEmpty(classNameAddition))
                            className = string.Concat(className, " ", classNameAddition);

                        if (!string.IsNullOrEmpty(root.ListDataIsInterlinePropertyValue))
                        {
                            bool isInterLine = (bool)GetValue(infoCollection, item, root.ListDataIsInterlinePropertyValue);

                            if (isInterLine)
                            {
                                RowHTML.AppendFormat("\n<tr>\n\t<td class=\"interline\" colspan=\"{0}\">{1}</td>\n</tr>", visibleColumnCount
                                    , GetValue(infoCollection, item, root.ListDataInterlineTextPropertyValue)
                                    );

                                root.m_ListDataInterLineCount++;
                                count--;
                                continue;
                            }
                        }


                        foreach (Framework.ListDataColumn column in root.ListDataColumns.List)
                        {
                            object propertyValue = null;

                            if (column.Type != Sushi.Mediakiwi.Framework.ListDataColumnType.Checkbox
                                || column.Type == Sushi.Mediakiwi.Framework.ListDataColumnType.RadioBox
                                )
                            {
                                if (isDataTable)
                                    propertyValue = ((System.Data.DataRow)item)[column.ColumnValuePropertyName];
                                else
                                    propertyValue = GetValue(infoCollection, item, column);
                            }

                            int? listTypeID = null;
                            Sushi.Mediakiwi.Data.DalReflection.BaseSqlEntity tmp = item as Sushi.Mediakiwi.Data.DalReflection.BaseSqlEntity;
                            if (tmp != null)
                                listTypeID = ((Sushi.Mediakiwi.Data.DalReflection.BaseSqlEntity)item).m_PropertyListTypeID;


                            if (column.Type == Sushi.Mediakiwi.Framework.ListDataColumnType.Checkbox || column.Type == Sushi.Mediakiwi.Framework.ListDataColumnType.RadioBox)
                            {
                                //  Is set in later code (twice!!!)
                                propertyValue = "#";
                            }
                            else
                                propertyValue = Sushi.Mediakiwi.Data.Property.ConvertPropertyValue(column.ColumnValuePropertyName, propertyValue, container.CurrentList.ID, listTypeID);

                            bool hasNoWrap = false;
                            string align = null;

                            if (isFirst)
                            {
                                if (isDataTable)
                                {
                                    uniqueIdentifier = GetIndentifierKey((System.Data.DataRow)item, root.ListDataColumns.List);
                                    highlightColumn = GetHighlightedValue((System.Data.DataRow)item, root.ListDataColumns.List);

                                    if (highlightColumn != null)
                                        highlightColumn = container.Context.Server.HtmlEncode(highlightColumn.ToString());

                                }
                                else
                                {
                                    uniqueIdentifier = GetIndentifierKey(infoCollection, item, root.ListDataColumns.List);
                                    highlightColumn = GetHighlightedValue(infoCollection, item, root.ListDataColumns.List);

                                    if (highlightColumn != null)
                                        highlightColumn = container.Context.Server.HtmlEncode(highlightColumn.ToString());

                                }
                            }

                            var parser = container.CurrentListInstance.wim.DoListDataItemCreated(DataItemType.TableCell, root.ListDataColumns.List.ToArray(), column, item, uniqueIdentifier, propertyValue, index, null, source);

                            propertyValue = parser.InnerHTML;

                            bool hasInnerLink = (propertyValue != null && (propertyValue.ToString().Contains("<a") || propertyValue.ToString().Contains("<input")));

                            if (parser.InnerHTML != null)
                            {
                                if (column.Alignment == Sushi.Mediakiwi.Framework.Align.Default)
                                {
                                    if (propertyValue.GetType() == typeof(DateTime) || propertyValue.GetType() == typeof(Decimal) || propertyValue.GetType() == typeof(DateTime?) || propertyValue.GetType() == typeof(Decimal?))
                                        hasNoWrap = true;

                                    if (propertyValue.GetType() == typeof(Decimal) || propertyValue.GetType() == typeof(int))
                                    {
                                        align = " align=\"right\"";
                                    }
                                    if (propertyValue.GetType() == typeof(Boolean))
                                    {
                                        align = " align=\"center\"";
                                    }
                                }
                                else
                                {
                                    switch (column.Alignment)
                                    {
                                        case Sushi.Mediakiwi.Framework.Align.Left: align = " align=\"left\""; break;
                                        case Sushi.Mediakiwi.Framework.Align.Center: align = " align=\"center\""; break;
                                        case Sushi.Mediakiwi.Framework.Align.Right: align = " align=\"right\""; break;
                                    }
                                }
                            }


                            propertyValue = OutputValue(container, propertyValue);


                            if (column.ColumnValuePropertyName == "Moment")
                            {
                                if (align == null)
                                    align = "";


                                if (className == "even")
                                    align += string.Format(" style=\"background: #EAF1F2 url('../../../assets/{0}') no-repeat\"", container.CurrentListInstance.wim.Grid.BackgroundImage_Even);
                                else
                                    align += string.Format(" style=\"background: url('../../../assets/{0}') no-repeat\"", container.CurrentListInstance.wim.Grid.BackgroundImage_Odd);
                            }

                            if (!IsVisibleColumn(column.Type, includeExportFields))
                                continue;



                            #region First column
                            if (isFirst)
                            {
                                ApplyCheckboxTableCell(container, column, uniqueIdentifier, ref propertyValue);

                                if (column.EditConfiguration != null)
                                {
                                    object propertyHelp = null;
                                    if (!string.IsNullOrEmpty(column.EditConfiguration.InteractiveHelp))
                                        propertyHelp = GetValue(infoCollection, item, column.EditConfiguration.InteractiveHelp);

                                    ApplyEditConfiguration(container, item, column, uniqueIdentifier, propertyHelp, ref propertyValue, ref shouldSkipRowPresentation);
                                }


                                string tableRowIdentifier = string.Format(" id=\"item{0}\"", uniqueIdentifier);

                                string highlight =
                                    this.GetHighlightedState(infoCollection, item, container.CurrentListInstance.wim.ListDataHighlightPropertyValue)
                                    ? " highlight" : null;

                                GridDataItemAttribute attribute = new GridDataItemAttribute(DataItemType.TableRow);
                                var row = container.CurrentListInstance.wim.DoListDataItemCreated(DataItemType.TableRow, root.ListDataColumns.List.ToArray(), column, item, uniqueIdentifier, propertyValue, index, attribute, source);
                                string style = string.Empty;

                                if (row.Attribute != null && row.Attribute.Style != null)
                                {
                                    if (!string.IsNullOrEmpty(row.Attribute.Style.BackgroundColor))
                                        className = className.Replace("odd", string.Empty).Replace("even", string.Empty);
                                    style = string.Format(" style=\"{0}\"", row.Attribute.Style.ToString());
                                }

                                //  When highlightColumn is null or empty that particular row should act differtly (for popup layer/paging)
                                if ((type == 1 || type == 2) && highlightColumn != null && !string.IsNullOrEmpty(highlightColumn))
                                {
                                    RowHTML.Append(string.Format("\n<tr{2} class=\"{0} classMouseHover link{1}\"{3}>", className, highlight, tableRowIdentifier, style));
                                    RowHTML.Append(string.Format("\n\t<td{3}{4}{5}><input type=\"hidden\" id=\"{0}\" value=\"{1}\" />{2}", uniqueIdentifier, highlightColumn, propertyValue, column.ColumnWidth == 0 ? "" : string.Format(" width=\"{0}\"", column.ColumnWidth), hasNoWrap ? " nowrap" : string.Empty, align));
                                }
                                else
                                {
                                    RowHTML.Append(string.Format("\n<tr{3} class=\"{1}{0} classMouseHover{2}\"{4}>", className, isEditMode ? "autoSubmit " : null, highlight, tableRowIdentifier, style));
                                    string url = "#";

                                    if (sortOrderIsActive)
                                        url = string.Concat("#", uniqueIdentifier);

                                    if (!isEditMode && container.CurrentListInstance.wim.HasListLoad && !sortOrderIsActive)
                                    {
                                        if (!string.IsNullOrEmpty(root.ListDataColumns.ColumnItemUrl))
                                        {
                                            //  Get URL from list item
                                            url = GetValue(infoCollection, item, root.ListDataColumns.ColumnItemUrl).ToString();
                                        }
                                        else
                                        {
                                            if (string.IsNullOrEmpty(uniqueIdentifier) && string.IsNullOrEmpty(root.SearchResultItemPassthroughParameterProperty))
                                                url = "#";
                                            else
                                            {
                                                if (string.IsNullOrEmpty(uniqueIdentifier))
                                                {
                                                    if (isDataTable)
                                                        url = string.Concat(container.WimPagePath, "?", GetPassThroughValue(root, (System.Data.DataRow)item).Replace("[KEY]", uniqueIdentifier));
                                                    else
                                                        url = string.Concat(container.WimPagePath, "?", GetPassThroughValue(infoCollection, root, item).Replace("[KEY]", uniqueIdentifier));
                                                }
                                                else
                                                {
                                                    if (isDataTable)
                                                        url = string.Concat(container.WimPagePath, "?", GetPassThroughValue(root, (System.Data.DataRow)item).Replace("[KEY]", uniqueIdentifier), "=", uniqueIdentifier);
                                                    else
                                                        url = string.Concat(container.WimPagePath, "?", GetPassThroughValue(infoCollection, root, item).Replace("[KEY]", uniqueIdentifier), "=", uniqueIdentifier);
                                                }
                                            }

                                            if (type > 0)
                                            {
                                                int typeID = Utility.ConvertToInt(container.Context.Request.QueryString["type"]);
                                                //  Paging in a popup layer requires some addition url parameters
                                                if (typeID > 0)
                                                    url = string.Concat(url, "&openinframe=", container.Request.QueryString["openinframe"], "&type=", typeID, "&referid=", container.Request.QueryString["referid"]
                                                        , string.IsNullOrEmpty(container.Context.Request.QueryString["root"]) ? "" : string.Concat("&root=", container.Context.Request.QueryString["root"]));
                                                else
                                                    url = string.Concat(url, "&openinframe=", container.Request.QueryString["openinframe"], "&referid=", container.Request.QueryString["referid"]
                                                        , string.IsNullOrEmpty(container.Context.Request.QueryString["root"]) ? "" : string.Concat("&root=", container.Context.Request.QueryString["root"]));
                                            }
                                        }
                                    }



                                    if (hasEditProperties)
                                    {
                                        url = Wim.Utility.AddApplicationPath(string.Concat(Wim.CommonConfiguration.RelativeRepositoryUrl, "/tcl.aspx?auto=", container.CurrentListInstance.wim.CurrentSite.ID, ",", container.CurrentListInstance.wim.CurrentList.ID, ",", uniqueIdentifier));
                                        RowHTML.Append(string.Format("\n\t<td{2}{3}{4}><input type=\"hidden\" class=\"autoSubmitTarget\" value=\"{0}\">{1}"
                                            , url
                                            , propertyValue, column.ColumnWidth == 0 ? "" : string.Format(" width=\"{0}\"", column.ColumnWidth)
                                            , hasNoWrap ? " nowrap" : string.Empty, align
                                            ));
                                    }
                                    else
                                    {
                                        if (shouldSkipRowPresentation)
                                            continue;

                                        if (root.SearchListCanClickThrough)
                                        {
                                            RowHTML.Append(string.Format("\n\t<td{2}{3}{4}{5}><a href=\"{0}\" class=\"clickOnParent{6}\"></a>{1}"
                                                , url
                                                , propertyValue
                                                , column.ColumnWidth == 0 ? "" : string.Format(" width=\"{0}\"", column.ColumnWidth), hasNoWrap ? " nowrap" : string.Empty, align
                                                , hasInnerLink ? " class=\"nopt\"" : null
                                                , (root.CurrentList.Option_LayerResult) ? " openInPopupLayer" : string.Empty
                                                ));
                                        }
                                        else
                                            RowHTML.Append(string.Format("\n\t<td{1}{2}{3}>{0}", propertyValue, column.ColumnWidth == 0 ? "" : string.Format(" width=\"{0}\"", column.ColumnWidth), hasNoWrap ? " nowrap" : string.Empty, align));

                                    }
                                }
                            }
                            #endregion First column
                            else
                            {

                                ApplyCheckboxTableCell(container, column, uniqueIdentifier, ref propertyValue);

                                if (column.EditConfiguration != null)
                                {
                                    object propertyHelp = null;
                                    if (!string.IsNullOrEmpty(column.EditConfiguration.InteractiveHelp))
                                        propertyHelp = GetValue(infoCollection, item, column.EditConfiguration.InteractiveHelp);

                                    ApplyEditConfiguration(container, item, column, uniqueIdentifier, propertyHelp, ref propertyValue, ref shouldSkipRowPresentation);
                                }

                                if (propertyValue == null || propertyValue.ToString().Trim() == string.Empty)
                                {
                                    RowHTML.Append(string.Format("</td>\n\t<td{3}{4}{5}{6}>{0}{1}{2}", null, propertyValue, null, column.ColumnWidth == 0 ? "" : string.Format(" width=\"{0}\"", column.ColumnWidth), hasNoWrap ? " nowrap" : string.Empty, align
                                        , hasInnerLink ? " class=\"nopt\"" : null
                                        ));
                                }
                                else
                                    RowHTML.Append(string.Format("</td>\n\t<td{3}{4}{5}{6}>{0}{1}{2}", column.ColumnValuePrefix, propertyValue, column.ColumnValueSuffix, column.ColumnWidth == 0 ? "" : string.Format(" width=\"{0}\"", column.ColumnWidth), hasNoWrap ? " nowrap" : string.Empty, align
                                        , hasInnerLink ? " class=\"nopt\"" : null
                                        ));
                            }
                            isFirst = false;
                        }


                        if (isEditMode)
                        {
                            //RowHTML.AppendFormat("<img class=\"progressIndicator\" alt=\"\" src=\"{0}/images/progressIndicator_passive.png\"/>", container.WimRepository);
                        }

                        RowHTML.Append("\n</td></tr>");

                        if (!shouldSkipRowPresentation)
                            build2.Append(RowHTML);
                        else
                        {
                            //  switch zebra
                            if (className == "odd") className = "even";
                            else className = "odd";
                        }
                        #endregion Normal Table>Tr>Td flow
                    }
                }
                
            }


            if (type != 4)
            {
                if (root.m_IsLinqUsed)
                    count = root.m_ListDataRecordCount - root.m_ListDataInterLineCount;

                if (visibleColumnCount == 1)
                    build2.AppendFormat("\n<tfoot>\n<tr>\n\t<td class=\"total\" align=\"right\">{1}: {0} {2}</td>\n</tr>\n</tfoot>", count
                        , Resource.ResourceManager.GetString("grid_total", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                        , Resource.ResourceManager.GetString("grid_records", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                        );
                else
                    build2.AppendFormat("\n<tfoot>\n<tr>\n\t<td class=\"total\" colspan=\"{0}\" align=\"right\">{2}: {1} {3}</td>\n</tr>\n</tfoot>", visibleColumnCount, count
                        , Resource.ResourceManager.GetString("grid_total", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                        , Resource.ResourceManager.GetString("grid_records", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                        );
            }


            if (hasTotal && !sortOrderIsActive)
            {
                StringBuilder build3 = new StringBuilder();

                build3.Append("<tbody class=\"totals\"><tr>");
                foreach (Framework.ListDataColumn column in root.ListDataColumns.List)
                {
                    if (!IsVisibleColumn(column.Type, includeExportFields)) continue;
                    if (column.Total == Sushi.Mediakiwi.Framework.ListDataTotalType.Sum)
                    {
                        decimal total = column.TotalValue;
                        if (root.GridDataCommunication.HasAppliedData)
                        {
                            var tmp = root.GridDataCommunication.GetResultData(Framework.ListDataTotalType.Sum, column.ColumnValuePropertyName);
                            total = Wim.Utility.ConvertToDecimal(tmp, total);
                        }

                        if (column.TotalValueType == typeof(int))
                            build3.AppendFormat("<td class=\"sum\" align=\"right\">{0}{1}{2}</td>", column.ColumnValuePrefix, OutputValue(container, Convert.ToInt32(total)), column.ColumnValueSuffix);
                        else
                            build3.AppendFormat("<td class=\"sum\" align=\"right\">{0}{1}{2}</td>", column.ColumnValuePrefix, OutputValue(container, total), column.ColumnValueSuffix);
                    }
                    else if (column.Total == Sushi.Mediakiwi.Framework.ListDataTotalType.Average)
                    {
                        if (count == 0)
                            build3.AppendFormat("<td class=\"sum\" align=\"right\">{0}{1}{2}</td>", column.ColumnValuePrefix, OutputValue(container, 0), column.ColumnValueSuffix);
                        else 
                            build3.AppendFormat("<td class=\"sum\" align=\"right\">{0}{1}{2}</td>", column.ColumnValuePrefix, OutputValue(container, Decimal.Divide(column.TotalValue, count)), column.ColumnValueSuffix);
                    }
                    else
                        build3.Append("<td class=\"sum\"></td>");

                }
                build3.Append("</tr></tbody>");
                build2.Insert(0, build3.ToString());
            }

            string candidate;
            if (type > 0)
            {
                if (type == 3)
                {
                    //  Middle section of dashboad
//                    string titleBar = string.Format(@"
//<h2 class=""linked"">
//	<span class=""label"">{1}</span>
//	<span class=""link""><a href=""{0}?list={2}"" class=""more"">More</a></span>
//</h2><p>{3}</p>
//"
//                        , container.WimPagePath
//                        , container.CurrentListInstance.wim.ListTitle
//                        , container.CurrentList.ID
//                        , container.CurrentList.Description
//                        );

                    string titleBar = null;
                    candidate = string.Concat(titleBar, paging, "\n<table class=\"data\">", build.ToString(), build2.ToString(), "\n</table>");
                }
                else if (type == 4)
                {
                    //  Left or right section of dashboad
//                    candidate = string.Format(@"
//<div class=""updates"">
//	<h2>{0}</h2>{1}
//	<ul class=""pager"">
//		<li><a href=""{2}?list={3}"" class=""next"">Overview</a></li>
//	</ul>
//</div>
//"
//                    , container.CurrentListInstance.wim.ListTitle
//                    , string.Concat("\n<table class=\"data\">", build.ToString(), build2.ToString(), "\n</table>")
//                    , container.WimPagePath
//                    , container.CurrentList.ID
//                    );
                    candidate = string.Format(@"
<div class=""updates"">
	{0}
</div>
"
                    , string.Concat("\n<table class=\"data\">", build.ToString(), build2.ToString(), "\n</table>")
                    , container.WimPagePath
                    , container.CurrentList.ID
                    );

                }
                else
                    candidate = string.Concat(paging, "\n<table class=\"data\">", build.ToString(), build2.ToString(), "\n</table>");
            }
            else
                candidate = string.Concat(paging, "\n<table class=\"data\">", build.ToString(), build2.ToString(), "\n</table>");

            //  Check


            if (sortOrderIsActive)
            {
                return string.Concat(string.Format(@"
<div class=""clickToSort"">
	<span class=""rowIndicators"">
		<img class=""leftRowIndicator"" alt=""Insert between here"" src=""{0}/images/leftRowIndicator.png""/>
		<img class=""centerRowIndicator"" alt=""Insert between here"" src=""{0}/images/centerRowIndicator.png""/>
		<img class=""rightRowIndicator"" alt=""Insert between here"" src=""{0}/images/rightRowIndicator.png""/>
	</span>
"
                    , container.WimRepository), candidate, "</div>");
            }
            else
                return candidate;


        }


        /// <summary>
        /// Applies the checkbox table cell.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="column">The column.</param>
        /// <param name="uniqueIdentifier">The unique identifier.</param>
        /// <param name="propertyValue">The property value.</param>
        void ApplyCheckboxTableCell(Console container, Framework.ListDataColumn column, string uniqueIdentifier, ref object propertyValue)
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
                string post = container.Request.Params[key];
                if (!string.IsNullOrEmpty(post))
                {
                    isChecked = container.Request.Params[key] == "1";
                }

                if (column.Type == Sushi.Mediakiwi.Framework.ListDataColumnType.Checkbox)
                {
                    propertyValue = string.Format("<input id=\"{0}\" name=\"{0}\" value=\"1\"{1}{2} type=\"checkbox\">"
                        , key
                        , isChecked ? " checked=\"checked\"" : ""
                        , isEnabled ? "" : " disabled=\"disabled\""
                        );
                }
                else if (column.Type == Sushi.Mediakiwi.Framework.ListDataColumnType.RadioBox)
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

        void SetPolution(Console container, object sender)
        {
            if (container.CurrentListInstance.wim.m_ChangedSearchGridItem == null)
                container.CurrentListInstance.wim.m_ChangedSearchGridItem = new List<object>();
            container.CurrentListInstance.wim.m_ChangedSearchGridItem.Add(sender);
        }

        bool HasPostItem(Console container, string name)
        {
            var arr = container.Request.Form.AllKeys;
            foreach (var item in arr)
            {
                if (item == name)
                    return true;
            }
            return false;
        }

        void ApplyEditConfiguration(Console container, object sender, Framework.ListDataColumn column, string uniqueIdentifier, object propertyHelp, ref object properyValue, ref bool shouldSkipRowPresentation)
        {
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
                    System.Web.UI.WebControls.ListItemCollection col =
                      GetProperty(container, sender, column.EditConfiguration.CollectionProperty) as System.Web.UI.WebControls.ListItemCollection;

                    object candidate = GetProperty(container, sender, propertyName);
                    string value = candidate == null ? null : candidate.ToString();

                    if (container.Request.Form.Count > 0 
                        && value != container.Request.Form[name] 
                        && HasPostItem(container, name))
                    {
                        value = container.Request.Form[name];

                        if (property.PropertyType == typeof(int))
                            property.SetValue(sender, Wim.Utility.ConvertToInt(value), null);
                        else if (property.PropertyType == typeof(int?))
                            property.SetValue(sender, Wim.Utility.ConvertToIntNullable(value), null);
                        else 
                            property.SetValue(sender, value, null);

                        SetPolution(container, sender);

                        if (skipRowOption)
                            shouldSkipRowPresentation = true;
                    }

                    StringBuilder optionList = new StringBuilder();
                    foreach (System.Web.UI.WebControls.ListItem li in col)
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
                        , enabledTag);
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

                bool isValueNull = !value.HasValue;
                if (!value.HasValue)
                {
                    if (column.EditConfiguration.NullableCheckedState)
                        value = true;
                }

                bool formresult = !string.IsNullOrEmpty(container.Request.Form[name]);

                if (container.Request.Form.Count > 0 && (value == null || value != formresult || isValueNull)
                    && HasPostItem(container, name)
                    )
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
                if (container.Request.Form.Count > 0 && HasPostItem(container, name) && 
                    ((properyValue == null && !string.IsNullOrEmpty(container.Request.Form[name])) ||
                    (properyValue != null && properyValue.ToString() != container.Request.Form[name]))

                    )
                {
                    properyValue = container.Request.Form[name];
                    sender.GetType().GetProperty(propertyName).SetValue(sender, properyValue, null);

                    SetPolution(container, sender);

                    if (skipRowOption)
                        shouldSkipRowPresentation = true;
                }
                
                htmlCandidate = string.Format("<span class=\"inputMode\"><input id=\"{0}\" name=\"{0}\" value=\"{1}\" type=\"text\"{2}{4} /></span>&nbsp;<label>{3}</label>"
                    , name
                    , properyValue
                    , column.EditConfiguration.Width == 0 ? string.Empty : string.Format(" style=\"width:{0}px\"", column.EditConfiguration.Width)
                    , propertyHelp
                    , enabledTag
                    );
                properyValue = htmlCandidate;
            }

        }

        bool CheckState(Console container, object sender, Framework.ListDataColumn column, ref object properyValue, ref string enabledTag)
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

        System.Reflection.PropertyInfo GetPropertyInfo(Console container, object sender, string property)
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

        object GetProperty(Console container, object sender, string property)
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

        internal string GetThumbnailGridFromListInstance(Framework.WimComponentListRoot root, Console container, int type, bool includeExportFields)
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
                splitlist = new Wim.Utilities.Splitlist(root.ListDataTable, maxViewItemCount, root.CurrentList.Option_Search_MaxViews);
                if (currentPage > splitlist.ListCount)
                    currentPage = splitlist.ListCount;

                object arr = splitlist[currentPage - 1];
                if (arr != null)
                    whilelist = ((System.Data.DataTable)arr).Rows.GetEnumerator();
            }
            else
            {
                splitlist = new Wim.Utilities.Splitlist(root.ListData, maxViewItemCount, root.CurrentList.Option_Search_MaxViews);
                if (currentPage > splitlist.ListCount)
                    currentPage = splitlist.ListCount;

                object arr = splitlist[currentPage - 1];
                if (arr != null)
                    whilelist = ((IList)arr).GetEnumerator();
            }
            
            build.Append(GetListPaging(container, splitlist, currentPage));
            //build.Append("<div id=\"thumbnails\"><ul class=\"centeredThumbnails\">");
            build.Append("<div id=\"thumbnails\"><ul>");


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
                    string thumbnail = null;
                    string url = "#";
                    int id = 0;

                    foreach (Framework.ListDataColumn column in root.ListDataColumns.List)
                    {
                        object propertyValue;

                        if (isDataTable)
                            propertyValue = ((System.Data.DataRow)item)[column.ColumnValuePropertyName];
                        else
                            propertyValue = GetValue(infoCollection, item, column);

                        propertyValue = OutputValue(container, propertyValue);

                        if (propertyValue != null)
                        {
                            if (column.ColumnValuePropertyName == "Title")
                                title = propertyValue.ToString();
                            else if (column.ColumnValuePropertyName == "Icon")
                                thumbnail = propertyValue.ToString();
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
                                    highlightColumn = container.Context.Server.HtmlEncode(highlightColumn.ToString());

                            }
                            else
                            {
                                uniqueIdentifier = GetIndentifierKey(infoCollection, item, root.ListDataColumns.List);
                                highlightColumn = GetHighlightedValue(infoCollection, item, root.ListDataColumns.List);

                                if (highlightColumn != null)
                                    highlightColumn = container.Context.Server.HtmlEncode(highlightColumn.ToString());

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
                                            url = string.Concat(container.WimPagePath, "?", GetPassThroughValue(root, (System.Data.DataRow)item), "=", uniqueIdentifier);
                                        else
                                            url = string.Concat(container.WimPagePath, "?", GetPassThroughValue(infoCollection, root, item), "=", uniqueIdentifier);
                                    }

                                    if (type > 0)
                                    {
                                        //  Paging in a popup layer requires some addition url parameters
                                        url = string.Concat(url, "&openinframe=", container.Request.QueryString["openinframe"], "&referid=", container.Request.QueryString["referid"]);
                                    }
                                }
                            }
                            id = Convert.ToInt32(uniqueIdentifier);
                        }
                        isFirst = false;
                    }

                    build.AppendFormat(@"<li{6}>
	<div class=""border"">
		<label class=""picture"">{5}<a href=""{4}""><img alt=""{2}"" src=""{1}"" /></a></label>
	</div>
	<label class=""title"">{0}</label>
</li>"
                        , Utility.ConvertToFixedLengthText(title, 30, "..", 17, "-")
                        , thumbnail
                        , title
                        , string.Concat("Binary_", id)
                        , url
                        , isFolder ? string.Empty : string.Format(@"<input type=""hidden"" id=""{1}"" value=""{0}"" />", title, id)
                        , isFolder ? string.Empty : "  class=\"link\""
                        );
                }
            }

            build.Append("</ul><div class=\"clear\"></div></div>");
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
            if (!string.IsNullOrEmpty(root.SearchResultItemPassthroughParameter))
            {
                return root.SearchResultItemPassthroughParameter;
            }
            return "item";
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
            return GetValue(infoCollection, item, column.ColumnValuePropertyName, column.DataType);
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
            return GetValue(infoCollection, item, propertyName, null);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="infoCollection">The info collection.</param>
        /// <param name="item">The item.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="castType">Type of the cast.</param>
        /// <returns></returns>
        object GetValue(PropertyInfo[] infoCollection, object item, string propertyName, Type castType)
        {
            return GetValue(infoCollection, item, propertyName, castType, true);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="infoCollection">The info collection.</param>
        /// <param name="item">The item.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="castType">Type of the cast.</param>
        /// <param name="ifNullRecallWithData">if set to <c>true</c> [if null recall with data].</param>
        /// <returns></returns>
        object GetValue(PropertyInfo[] infoCollection, object item, string propertyName, Type castType, bool ifNullRecallWithData)
        {
            string indexer = null;
            if (propertyName != null && propertyName.Contains("."))
            {
                indexer = propertyName.Split('.')[1];
                propertyName = propertyName.Split('.')[0];
            }

            for (int index = 0; index < infoCollection.Length; index++)
            {
                System.Reflection.PropertyInfo info = infoCollection[index];

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
                    return info.GetValue(item, null);
                }
            }
            if (ifNullRecallWithData && indexer == null)
                return GetValue(infoCollection, item, string.Concat("data.", propertyName), castType, false);
            
            return null;
            //throw new Exception(string.Format("Cound not find the requested property [{0}] on [{1}]", propertyName, item.GetType()));
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
                    return GetValue(infoCollection, item, column).ToString();
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
        /// 	<c>true</c> if [is visible export column] [the specified type]; otherwise, <c>false</c>.
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
        /// <returns></returns>
        object OutputValue(GeneratedCms.Console container, object candidate)
        {
            return OutputValue(container, candidate, true);
        }

        /// <summary>
        /// Outputs the value.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="candidate">The candidate.</param>
        /// <param name="convertToWimDecimal">if set to <c>true</c> [convert to wim decimal].</param>
        /// <returns></returns>
        object OutputValue(GeneratedCms.Console container, object candidate, bool convertToWimDecimal)
        {
            if (candidate == null) return null;

            if (candidate.GetType() == typeof(DateTime))
            {
                if (((DateTime)candidate) == DateTime.MinValue) return null;
                DateTime tmp = ((DateTime)candidate);
                
                // [MR:20-03-2019] Converts UTC (database) time to local timezone for display
                if (container.CurrentList.Option_ConvertUTCToLocalTime && tmp.Kind != DateTimeKind.Local)
                    tmp = AppCentre.Data.Supporting.LocalDateTime.GetDate(tmp, container.CurrentListInstance.wim.CurrentSite, true);

                if (tmp.Hour == 0 && tmp.Minute == 0 && tmp.Second == 0 && tmp.Millisecond == 0)
                    return tmp.ToString("dd-MM-yy");
                return tmp.ToString("dd-MM-yy HH:mm");
            }
            else if (candidate.GetType() == typeof(DateTime?))
            {
                if (!((DateTime?)candidate).HasValue) return null;
                DateTime tmp = ((DateTime?)candidate).Value;

                // [MR:20-03-2019] Converts UTC (database) time to local timezone for display
                if (container.CurrentList.Option_ConvertUTCToLocalTime && tmp.Kind != DateTimeKind.Local)
                    tmp = AppCentre.Data.Supporting.LocalDateTime.GetDate(tmp, container.CurrentListInstance.wim.CurrentSite, true);

                if (tmp.Hour == 0 && tmp.Minute == 0 && tmp.Second == 0 && tmp.Millisecond == 0)
                    return tmp.ToString("dd-MM-yy");
                return tmp.ToString("dd-MM-yy HH:mm");
            }
            else if (candidate.GetType() == typeof(Boolean))
            {
                return ((Boolean)candidate)
                ? Wim.Utility.GetIconImageString(Utility.IconImage.accept_16)
                : Wim.Utility.GetIconImageString(Utility.IconImage.No);
                
                //return ((Boolean)candidate)
                //? Resource.ResourceManager.GetString("yes", new CultureInfo(container.CurrentApplicationUser.LanguageCulture))
                //: Resource.ResourceManager.GetString("no", new CultureInfo(container.CurrentApplicationUser.LanguageCulture));
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
