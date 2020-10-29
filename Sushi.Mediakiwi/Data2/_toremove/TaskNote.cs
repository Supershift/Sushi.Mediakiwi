using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Sushi.Mediakiwi.Data.DalReflection;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Represents a Note entity.
    /// </summary>
    [DatabaseTable("wim_TaskNotes", Order = "TaskNote_Key DESC")]
    public class TaskNote : DatabaseEntity
    {
        int m_ID;
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        [DatabaseColumn("TaskNote_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        int m_UserID;
        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        /// <value>The user ID.</value>
        [DatabaseColumn("TaskNote_User_Key", SqlDbType.Int)]
        public int UserID
        {
            get { return m_UserID; }
            set { m_UserID = value; }
        }

        int m_TaskID;
        /// <summary>
        /// Gets or sets the task ID.
        /// </summary>
        /// <value>The task ID.</value>
        [DatabaseColumn("TaskNote_Task_Key", SqlDbType.Int)]
        public int TaskID
        {
            get { return m_TaskID; }
            set { m_TaskID = value; }
        }

        int? m_PageID;
        /// <summary>
        /// Gets or sets the page ID.
        /// </summary>
        /// <value>The page ID.</value>
        [DatabaseColumn("TaskNote_Page_Key", SqlDbType.Int, IsNullable = true)]
        public int? PageID
        {
            get { return m_PageID; }
            set { m_PageID = value; }
        }

        int? m_CompletedUserID;
        /// <summary>
        /// Gets or sets the completed user ID.
        /// </summary>
        /// <value>The completed user ID.</value>
        [DatabaseColumn("TaskNote_Completed_User_Key", SqlDbType.Int, IsNullable = true)]
        public int? CompletedUserID
        {
            get { return m_CompletedUserID; }
            set { m_CompletedUserID = value; }
        }

        bool m_IsCompleted;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is completed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is completed; otherwise, <c>false</c>.
        /// </value>
        [DatabaseColumn("TaskNote_IsCompleted", SqlDbType.Bit)]
        public bool IsCompleted
        {
            get { return m_IsCompleted; }
            set { m_IsCompleted = value; }
        }

        DateTime m_Completed;
        /// <summary>
        /// Gets or sets the completed.
        /// </summary>
        /// <value>The completed.</value>
        [DatabaseColumn("TaskNote_Completed", SqlDbType.DateTime, IsNullable = true)]
        public DateTime Completed
        {
            get { return m_Completed; }
            set { m_Completed = value; }
        }

        string m_Comment;
        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        /// <value>The comment.</value>
        [DatabaseColumn("TaskNote_Comment", SqlDbType.NText, IsNullable = true)]
        public string Comment
        {
            get { return m_Comment; }
            set { m_Comment = value; }
        }

        string m_CompletedUsername;
        /// <summary>
        /// Gets or sets the completed username.
        /// </summary>
        /// <value>The completed username.</value>
        [DatabaseColumn("TaskNote_Completed", SqlDbType.NVarChar, Length = 50, IsNullable = true, ColumnSubQuery = "(select User_Displayname from wim_Users where User_Key = TaskNote_User_Key)", IsOnlyRead = true)]
        public string CompletedUsername
        {
            get { return m_CompletedUsername; }
            set { m_CompletedUsername = value; }
        }

        DateTime m_Created;
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        [DatabaseColumn("TaskNote_Created", SqlDbType.DateTime)]
        public DateTime Created
        {
            get
            {
                if (this.m_Created == DateTime.MinValue) this.m_Created = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
                return m_Created;
            }
            set { m_Created = value; }
        }

        Task m_Task;
        /// <summary>
        /// Gets the task.
        /// </summary>
        /// <value>The task.</value>
        public Task Task
        {
            get {
                if (m_Task == null)
                    m_Task = Task.SelectOne(this.TaskID);
                return m_Task;
            } 
        }

        bool m_NotifyOwner;
        /// <summary>
        /// Gets or sets a value indicating whether [notify owner].
        /// </summary>
        /// <value><c>true</c> if [notify owner]; otherwise, <c>false</c>.</value>
        [DatabaseColumn("TaskNote_NotifyOwner", SqlDbType.Bit)]
        public bool NotifyOwner
        {
            get { return m_NotifyOwner; }
            set { m_NotifyOwner = value; }
        }

        /// <summary>
        /// Selects all outbound.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="showCompleted">if set to <c>true</c> [show completed].</param>
        /// <returns></returns>
        public static TaskNote[] SelectAllOutbound(int userID, bool showCompleted)
        {
            List<TaskNote> list = new List<TaskNote>();

            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("TaskNote_User_Key", SqlDbType.Int, userID));
            where.Add(new DatabaseDataValueColumn("TaskNote_IsCompleted", SqlDbType.Bit, showCompleted));

            foreach (object o in new TaskNote()._SelectAll(where)) list.Add((TaskNote)o);
            return list.ToArray();
        }
    }
}
