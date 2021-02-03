using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Sushi.Mediakiwi.Data.DalReflection;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Represents a Task entity.
    /// </summary>
    [DatabaseTable("wim_Tasks", Order = "Task_Description ASC")]
    public partial class Task : DatabaseEntity
    {
        int m_ID;
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        [DatabaseColumn("Task_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        string m_OnCompleteMessage;
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DatabaseColumn("Task_Message", SqlDbType.NVarChar, Length = 500)]
        public string OnCompleteMessage
        {
            get { return m_OnCompleteMessage; }
            set { m_OnCompleteMessage = value; }
        }

        string m_Description;
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DatabaseColumn("Task_Description", SqlDbType.NVarChar, Length = 50, IsNullable = true)]
        public string Description
        {
            get { return m_Description; }
            set { m_Description = value; }
        }

        bool m_IsActive;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        [DatabaseColumn("Task_IsActive", SqlDbType.Bit)]
        public bool IsActive
        {
            get { return m_IsActive; }
            set { m_IsActive = value; }
        }

        bool m_CompletedOnPublication;
        /// <summary>
        /// Gets or sets a value indicating whether [completed on publication].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [completed on publication]; otherwise, <c>false</c>.
        /// </value>
        [DatabaseColumn("Task_CompleteOnPublication", SqlDbType.Bit)]
        public bool CompletedOnPublication
        {
            get { return m_CompletedOnPublication; }
            set { m_CompletedOnPublication = value; }
        }

        bool m_CompletedOnChange;
        /// <summary>
        /// Gets or sets a value indicating whether [completed on change].
        /// </summary>
        /// <value><c>true</c> if [completed on change]; otherwise, <c>false</c>.</value>
        [DatabaseColumn("Task_CompleteOnChange", SqlDbType.Bit)]
        public bool CompletedOnChange
        {
            get { return m_CompletedOnChange; }
            set { m_CompletedOnChange = value; }
        }

        bool m_CompletedOnTakeDown;
        /// <summary>
        /// Gets or sets a value indicating whether [completed on take down].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [completed on take down]; otherwise, <c>false</c>.
        /// </value>
        [DatabaseColumn("Task_CompleteOnTakeDown", SqlDbType.Bit)]
        public bool CompletedOnTakeDown
        {
            get { return m_CompletedOnTakeDown; }
            set { m_CompletedOnTakeDown = value; }
        }

        bool m_CompletedViaButton;
        /// <summary>
        /// Gets or sets a value indicating whether [completed via button].
        /// </summary>
        /// <value><c>true</c> if [completed via button]; otherwise, <c>false</c>.</value>
        [DatabaseColumn("Task_CompleteViaButton", SqlDbType.Bit)]
        public bool CompletedViaButton
        {
            get { return m_CompletedViaButton; }
            set { m_CompletedViaButton = value; }
        }

        bool m_AutopostToChildren;
        /// <summary>
        /// Gets or sets a value indicating whether [autopost to children].
        /// </summary>
        /// <value><c>true</c> if [autopost to children]; otherwise, <c>false</c>.</value>
        [DatabaseColumn("Task_InheritencePost", SqlDbType.Bit)]
        public bool AutopostToChildren
        {
            get { return m_AutopostToChildren; }
            set { m_AutopostToChildren = value; }
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static Task SelectOne(int ID)
        {
            return (Task)new Task()._SelectOne(ID);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public static Task[] SelectAll()
        {
            List<Task> list = new List<Task>();
            foreach (object o in new Task()._SelectAll()) list.Add((Task)o);
            return list.ToArray();
        }
    }
}