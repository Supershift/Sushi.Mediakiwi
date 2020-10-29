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

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    /// <summary>
    /// 
    /// </summary>
    public class Task : BaseImplementation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Task"/> class.
        /// </summary>
        public Task()
        {
            this.ListDelete += new ComponentListEventHandler(Task_ListDelete);
            this.ListSave += new ComponentListEventHandler(Task_ListSave);
            this.ListLoad += new ComponentListEventHandler(Task_ListLoad);
            this.ListSearch += new ComponentSearchEventHandler(Task_ListSearch);
        }

        void Task_ListSearch(object sender, ComponentListSearchEventArgs e)
        {
            wim.CanAddNewItem = true;
            wim.ListDataColumns.Add("", "TaskId", ListDataColumnType.UniqueIdentifier);
            wim.ListDataColumns.Add("Task", "Description", ListDataColumnType.HighlightPresent);
            wim.ListDataColumns.Add("Is active", "IsActive");

            if (wim.IsCachedSearchResult)
                return;

            wim.ListData = Sushi.Mediakiwi.Data.Task.SelectAll();
        }

        void Task_ListLoad(object sender, ComponentListEventArgs e)
        {
            if (e.SelectedKey == 0)
                return;

            Sushi.Mediakiwi.Data.Task task = Sushi.Mediakiwi.Data.Task.SelectOne(e.SelectedKey);
            m_Task = task.Description;
            m_IsActive = task.IsActive ? "1" : "0";

            m_IsCompletedOnPublication = task.CompletedOnPublication ? "1" : "0";
            m_IsCompletedOnChange = task.CompletedOnChange ? "1" : "0";
            m_IsCompletedOnTakeDown = task.CompletedOnTakeDown ? "1" : "0";
            m_IsCompletedViaButton = task.CompletedViaButton ? "1" : "0";
            m_IsInheritencePost = task.AutopostToChildren ? "1" : "0";
            m_CompletionMessage = task.OnCompleteMessage;
        }

        void Task_ListSave(object sender, ComponentListEventArgs e)
        {
            //if (e.SelectedKey == 0)
            //{
            //    //  Add new
            //    Sushi.Mediakiwi.Data.Task.InsertOne(m_Task,
            //        m_IsActive == "1",
            //        m_IsCompletedOnPublication == "1",
            //        m_IsCompletedOnChange == "1",
            //        m_IsCompletedOnTakeDown == "1",
            //        m_IsCompletedViaButton == "1",
            //        m_IsInheritencePost == "1",
            //        m_CompletionMessage
            //        );
            //}
            //else
            //{
            //    //  Update existing
            //    Sushi.Mediakiwi.Data.Task.UpdateOne(e.SelectedKey, m_Task, m_IsActive == "1",
            //        m_IsCompletedOnPublication == "1",
            //        m_IsCompletedOnChange == "1",
            //        m_IsCompletedOnTakeDown == "1",
            //        m_IsCompletedViaButton == "1",
            //        m_IsInheritencePost == "1",
            //        m_CompletionMessage
            //        );
            //}
        }

        void Task_ListDelete(object sender, ComponentListEventArgs e)
        {
            //Sushi.Mediakiwi.Data.Task.DeleteOne(e.SelectedKey);
        }

        #region List attributes
        private string m_Task;
        /// <summary>
        /// Gets or sets the task title.
        /// </summary>
        /// <value>The task title.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Title", 50, true)]
        public string TaskTitle
        {
            get { return m_Task; }
            set { m_Task = value; }
        }

        private string m_IsActive;
        /// <summary>
        /// Gets or sets the is active.
        /// </summary>
        /// <value>The is active.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Radio("Is active", "BoolChoice", "IsActive", true)]
        public string IsActive
        {
            get { return m_IsActive; }
            set { m_IsActive = value; }
        }

        private string m_SubText = "Task completion options";
        /// <summary>
        /// Gets or sets the sub text.
        /// </summary>
        /// <value>The sub text.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextLine(null)]
        public string SubText
        {
            get { return m_SubText; }
            set { m_SubText = value; }
        }

        private string m_IsCompletedOnPublication;
        /// <summary>
        /// Gets or sets the is completed on publication.
        /// </summary>
        /// <value>The is completed on publication.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Radio("On publication", "BoolChoice", "IsCompletedOnPublication", true)]
        public string IsCompletedOnPublication
        {
            get { return m_IsCompletedOnPublication; }
            set { m_IsCompletedOnPublication = value; }
        }

        private string m_IsCompletedOnChange;
        /// <summary>
        /// Gets or sets the is completed on change.
        /// </summary>
        /// <value>The is completed on change.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Radio("On change", "BoolChoice", "IsCompletedOnChange", true)]
        public string IsCompletedOnChange
        {
            get { return m_IsCompletedOnChange; }
            set { m_IsCompletedOnChange = value; }
        }

        private string m_IsCompletedOnTakeDown;
        /// <summary>
        /// Gets or sets the is completed on take down.
        /// </summary>
        /// <value>The is completed on take down.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Radio("On take offline", "BoolChoice", "IsCompletedOnTakeDown", true)]
        public string IsCompletedOnTakeDown
        {
            get { return m_IsCompletedOnTakeDown; }
            set { m_IsCompletedOnTakeDown = value; }
        }

        private string m_IsCompletedViaButton;
        /// <summary>
        /// Gets or sets the is completed via button.
        /// </summary>
        /// <value>The is completed via button.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Radio("Via button", "BoolChoice", "IsCompletedViaButton", true)]
        public string IsCompletedViaButton
        {
            get { return m_IsCompletedViaButton; }
            set { m_IsCompletedViaButton = value; }
        }

        private string m_IsInheritencePost;
        /// <summary>
        /// Gets or sets the is inheritence post.
        /// </summary>
        /// <value>The is inheritence post.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Radio("Inheritance posting", "BoolChoice", "IsInheritencePost", true, "If selected the created tasks are also posted for all child sites")]
        public string IsInheritencePost
        {
            get { return m_IsInheritencePost; }
            set { m_IsInheritencePost = value; }
        }

        private string m_CompletionMessage;
        /// <summary>
        /// Gets or sets the completion message.
        /// </summary>
        /// <value>The completion message.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextArea("Completion message", 500, false, "If the task is completed this text is shown to the user")]
        public string CompletionMessage
        {
            get { return m_CompletionMessage; }
            set { m_CompletionMessage = value; }
        }
        #endregion 
    }
}
