using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Sushi.Mediakiwi.DataEntities
{
    public interface IDataDetail
    {
        bool ShowAll { get; set; }
        int PageSize { get; set; }
        int CurrentPage { get; set; }
        int? ResultCount { get; set; }
    }

    public class WebDataDetail: IDataDetail
    {

        #region IDataDetail Members

        public bool ShowAll
        {
            get { return CurrentPage == -1; }
            set { }
        }

        public int PageSize { get; set; }

        public int CurrentPage { get; set; }

        public int? ResultCount { get; set; }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class GridDataDetail: IDataDetail
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridDataDetail"/> class.
        /// </summary>
        /// <param name="root">The root.</param>
        public GridDataDetail(WimComponentListRoot root)
        {
            //m_currentPage = Wim.Utility.ConvertToInt(HttpContext.Current.Request.Params["set"], 1) - 1;
            var set = root.Console.Context.Request.Query["set"].ToString();
            if (!string.IsNullOrEmpty(set) && set.Contains(','))
            {
                set = set.Split(',')[0];
            }
            m_currentPage = Utility.ConvertToInt(set, 1) - 1;

            if (root.Console.Context.Request.Query["set"] == "all")
                m_currentPage = -1;
            
            // Whenever XLS export mode is active, show every result.
            if (root.IsExportMode_XLS)
            {
                m_currentPage = -1;
                ShowAll = true;
            }

            MaxResultCount = root.CurrentList.Option_Search_MaxViews;
            PageSize = root.CurrentList.Option_Search_MaxResultPerPage;
        }

        /// <summary>
        /// Gets the max result count.
        /// </summary>
        public int MaxResultCount { get; private set; }
        /// <summary>
        /// Gets the current page (when [-1] it should show all records).
        /// </summary>
        private int m_currentPage = 0;
        public int CurrentPage { get { return m_currentPage; } set { } }
        /// <summary>
        /// Gets a value indicating whether [show all].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show all]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowAll
        {
            get { return CurrentPage == -1; }
            set {}
        }
        /// <summary>
        /// Gets the size of the page.
        /// </summary>
        /// <value>
        /// The size of the page.
        /// </value>
        public int PageSize { get; set; }
        /// <summary>
        /// Gets or sets the result count, this is a mandatory field for when using this object.
        /// </summary>
        /// <value>
        /// The result count.
        /// </value>
        public int? ResultCount { get; set; }
        /// <summary>
        /// Gets or sets the result summary.
        /// </summary>
        /// <value>
        /// The result summary.
        /// </value>
        public void AddResultItem(ListDataTotalType totaltype, string column, object value)
        {
            if (this.TotalData == null)
                this.TotalData = new List<TotalItem>();

            TotalData.Add(new TotalItem() { Type = totaltype, Column = column, Value = value });
        }
        /// <summary>
        /// Gets the result data.
        /// </summary>
        /// <param name="totaltype">The totaltype.</param>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        internal object GetResultData(ListDataTotalType totaltype, string column)
        {
            var x = (from item in this.TotalData where item.Type == totaltype && item.Column == column select item).FirstOrDefault();
            if (x == null) return null;
            
            return x.Value;
        }
        /// <summary>
        /// Gets a value indicating whether this instance has applied data.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has applied data; otherwise, <c>false</c>.
        /// </value>
        internal bool HasAppliedData
        {
            get
            {
                if (TotalData == null || this.TotalData.Count == 0) return false;
                return true;
            }
        }
        /// <summary>
        /// Gets or sets the total data.
        /// </summary>
        /// <value>
        /// The total data.
        /// </value>
        internal List<TotalItem> TotalData { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public class TotalItem
        {
            /// <summary>
            /// Gets or sets the type.
            /// </summary>
            /// <value>
            /// The type.
            /// </value>
            public ListDataTotalType Type { get; set; }
            /// <summary>
            /// Gets or sets the column.
            /// </summary>
            /// <value>
            /// The column.
            /// </value>
            public string Column { get; set; }
            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            /// <value>
            /// The value.
            /// </value>
            public object Value { get; set; }
        }
 
    }
}
