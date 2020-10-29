using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.Data;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    /// <summary>
    /// 
    /// </summary>
    public class TaskOutbox : BaseImplementation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskOutbox"/> class.
        /// </summary>
        public TaskOutbox()
        {
            this.ListSearch += new ComponentSearchEventHandler(TaskOutbox_ListSearch);
        }

        void TaskOutbox_ListSearch(object sender, ComponentListSearchEventArgs e)
        {
            bool showCompleted = false;
            if (m_SearchFilter == "1")
            {
                showCompleted = true;
                wim.ListDataColumns.Add("Id", "NoteId", ListDataColumnType.UniqueIdentifier);
                wim.ListDataColumns.Add("Task", "Description", ListDataColumnType.HighlightPresent);
                wim.ListDataColumns.Add("Page", "PageId", ListDataColumnType.Default, ListDataContentType.InternalPageKey);
                wim.ListDataColumns.Add("Created", "Created");
                wim.ListDataColumns.Add("Completed by", "CompletedUsername");
                wim.ListDataColumns.Add("Completed", "Completed");
            }
            else
            {
                wim.ListDataColumns.Add("Id", "NoteId", ListDataColumnType.UniqueIdentifier);
                wim.ListDataColumns.Add("Task", "Description", ListDataColumnType.HighlightPresent);
                wim.ListDataColumns.Add("Page", "PageId", ListDataColumnType.Default, ListDataContentType.InternalPageKey);
                wim.ListDataColumns.Add("Created", "Created");
            }
            if (wim.IsCachedSearchResult)
                return;

            wim.ListData = Sushi.Mediakiwi.Data.TaskNote.SelectAllOutbound(wim.CurrentApplicationUser.RoleID, showCompleted);
        }

        #region List attributes
        /// <summary>
        /// Gets the filter.
        /// </summary>
        /// <value>The filter.</value>
        public ListItemCollection Filter
        {
            get
            {
                ListItemCollection col = new ListItemCollection();
                ListItem li;
                li = new ListItem("Show outstanding tasks", "0");
                li.Selected = true;
                col.Add(li);
                li = new ListItem("Show completed tasks", "1");
                col.Add(li);
                return col;
            }
        }

        private string m_SearchFilter;
        /// <summary>
        /// Gets or sets the search filter.
        /// </summary>
        /// <value>The search filter.</value>
        [Sushi.Mediakiwi.Framework.ContentListSearchItem.Choice_Dropdown("Result", "Filter", false)]
        public string SearchFilter
        {
            get { return m_SearchFilter; }
            set { m_SearchFilter = value; }
        }
        #endregion
    }
}
