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
    /// Represents a TaskInbox entity.
    /// </summary>
    public class TaskInbox : BaseImplementation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskInbox"/> class.
        /// </summary>
        public TaskInbox()
        {
            this.ListLoad += new ComponentListEventHandler(TaskInbox_ListLoad);
            this.ListSearch += new ComponentSearchEventHandler(TaskInbox_ListSearch);
        }

        /// <summary>
        /// Tasks the inbox_ list search.
        /// </summary>
        /// <param name="sender">The sender.</param>
        void TaskInbox_ListSearch(object sender, ComponentListSearchEventArgs e)
        {
            if (m_SearchFilter == "0")
            {
                wim.ListDataColumns.Add("Id", "NoteId", ListDataColumnType.UniqueIdentifier);
                wim.ListDataColumns.Add("Task", "Description", ListDataColumnType.HighlightPresent);
                wim.ListDataColumns.Add("Page", "PageId", ListDataColumnType.Default, ListDataContentType.InternalPageKey);
                wim.ListDataColumns.Add("Created", "Created");
                wim.ListDataColumns.Add("Issued by", "IssuedUsername");
            }
            else if (m_SearchFilter == "1")
            {
                wim.ListDataColumns.Add("Id", "NoteId", ListDataColumnType.UniqueIdentifier);
                wim.ListDataColumns.Add("Task", "Description", ListDataColumnType.HighlightPresent);
                wim.ListDataColumns.Add("Page", "PageId", ListDataColumnType.Default, ListDataContentType.InternalPageKey);
                wim.ListDataColumns.Add("Created", "Created");
                wim.ListDataColumns.Add("Issued by", "IssuedUsername");
                wim.ListDataColumns.Add("Completed", "Completed");

                if (wim.IsCachedSearchResult)
                    return;

                //wim.ListData = Sushi.Mediakiwi.Data.Task.Note.SelectAll_Completed(wim.CurrentApplicationUser.Id);
                return;
            }
            else
            {
                wim.ListDataColumns.Add("Id", "NoteId", ListDataColumnType.UniqueIdentifier);
                wim.ListDataColumns.Add("Task", "Description", ListDataColumnType.HighlightPresent, ListDataContentType.ItemSelect);
                wim.ListDataColumns.Add("Page", "PageId", ListDataColumnType.Default, ListDataContentType.InternalPageKey);
                wim.ListDataColumns.Add("Created", "Created");
            }

            if (wim.IsCachedSearchResult)
                return;

            //wim.ListData = Sushi.Mediakiwi.Data.Task.Note.SelectAll_Subscribed(wim.CurrentApplicationUser.Role);
        }

        /// <summary>
        /// 
        /// </summary>
        protected Sushi.Mediakiwi.Data.TaskNote note;
        /// <summary>
        /// Handles the ListLoad event of the TaskInbox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentListEventArgs"/> instance containing the event data.</param>
        void TaskInbox_ListLoad(object sender, ComponentListEventArgs e)
        {
            if (e.SelectedKey == 0)
                return;

            //note = Sushi.Mediakiwi.Data.Task.Note.SelectOne(e.SelectedKey);
            var wimUser = Sushi.Mediakiwi.Data.ApplicationUser.SelectOne(note.UserID);

            m_Date = note.Created.ToString("dd-MM-yy hh:mm tt");
            m_From = Server.HtmlEncode(string.Format("{0} <{1}>", wimUser.Name, wimUser.Email));
            m_CurrentPage = GenerateInternalPageLink(note.PageID.GetValueOrDefault(), "&task=" + note.ID.ToString());
            wim.ListTitle = string.Format("Task: {0}", note.Task.Description);
            m_TaskComment = note.Comment;

            m_CompletedByAction = "";
            if (note.Task.CompletedOnChange) m_CompletedByAction += "When the page is changed<br/>";
            if (note.Task.CompletedOnPublication) m_CompletedByAction += "When the page is published<br/>";
            if (note.Task.CompletedOnTakeDown) m_CompletedByAction += "When the page is unpublished<br/>";
            if (note.Task.CompletedViaButton) m_CompletedByAction += "Using a custom action (Complete button)<br/>";

            m_Additional = note.NotifyOwner ? "The issuer has requested a receipt on completion" : "none";
            if (note.Completed == DateTime.MinValue)
                m_Status = "Task is awaiting approval";
            else
                m_Status = string.Format("<b>{0}</b> completed this task at {1}", note.CompletedUsername, note.Completed.ToString("dd-MM-yy hh:mm tt"));
        }

        /// <summary>
        /// Generate a deeplink within Wim to a certain page.
        /// </summary>
        /// <param name="pageKey">The page key.</param>
        /// <param name="queryAddition">The query addition.</param>
        /// <returns></returns>
        public string GenerateInternalPageLink(int pageKey, string queryAddition)
        {
            if (pageKey == 0) return null;
            Sushi.Mediakiwi.Data.Page page = Sushi.Mediakiwi.Data.Page.SelectOne(pageKey, false);

            if (page == null) return null;
            string candidate = string.Format("<a href=\"/\" class=\"link\" onclick=\"parent.pagelink({0},false,'{1}');return false;\">{2}</a>",
                page.ID,
                queryAddition,
                page.HRef);

            return candidate;
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

        private string m_Date;
        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>The date.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextLine("Issue date")]
        public string Date
        {
            get { return m_Date; }
            set { m_Date = value; }
        }

        private string m_From;
        /// <summary>
        /// Gets or sets from.
        /// </summary>
        /// <value>From.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextLine("From")]
        public string From
        {
            get { return m_From; }
            set { m_From = value; }
        }

        private string m_CurrentPage;
        /// <summary>
        /// Gets or sets the current page.
        /// </summary>
        /// <value>The current page.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextLine("Page")]
        public string CurrentPage
        {
            get { return m_CurrentPage; }
            set { m_CurrentPage = value; }
        }

        private string m_TaskComment;
        /// <summary>
        /// Gets or sets the task comment.
        /// </summary>
        /// <value>The task comment.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextLine("Notes")]
        public string TaskComment
        {
            get { return m_TaskComment; }
            set { m_TaskComment = value; }
        }

        private string m_CompletedByAction;
        /// <summary>
        /// Gets or sets the completed by action.
        /// </summary>
        /// <value>The completed by action.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextLine("Task is completed")]
        public string CompletedByAction
        {
            get { return m_CompletedByAction; }
            set { m_CompletedByAction = value; }
        }

        private string m_Additional;
        /// <summary>
        /// Gets or sets the additional.
        /// </summary>
        /// <value>The additional.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextLine("Additional")]
        public string Additional
        {
            get { return m_Additional; }
            set { m_Additional = value; }
        }

        private string m_Status;
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextLine("Status")]
        public string Status
        {
            get { return m_Status; }
            set { m_Status = value; }
        }
        #endregion
    }
}
