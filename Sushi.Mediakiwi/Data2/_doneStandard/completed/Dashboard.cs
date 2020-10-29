using System;
using System.Web;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Sushi.Mediakiwi.Data.DalReflection;
using System.Linq;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Represents a Dashboard entity.
    /// </summary>
    [DatabaseTable("wim_Dashboards", Order = "Dashboard_Title")]
    public class Dashboard : DatabaseEntity
    {
        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

        #region ListItem
        /// <summary>
        /// 
        /// </summary>
        [DatabaseTable("wim_DashboardLists", Order = "DashboardList_SortOrder ASC")]
        public class ListItem : DatabaseEntity
        {
            /// <summary>
            /// Selects all.
            /// </summary>
            /// <returns></returns>
            public static ListItem[] SelectAll()
            {
                List<ListItem> list = new List<ListItem>();
                ListItem candidate = new ListItem();
                if (!string.IsNullOrEmpty(SqlConnectionString2)) candidate.SqlConnectionString = SqlConnectionString2;
                foreach (object obj in candidate._SelectAll(true)) list.Add((ListItem)obj);
                SqlConnectionString2 = null;
                return list.ToArray();
            }

            /// <summary>
            /// Selects all.
            /// </summary>
            /// <param name="dashboardID">The dashboard ID.</param>
            /// <returns></returns>
            public static ListItem[] SelectAll(int dashboardID)
            {
                var candidate = from item in SelectAll() where item.DashboardID == dashboardID select item;
                return candidate.ToArray();
            }

            /// <summary>
            /// Gets or sets the dashboard ID.
            /// </summary>
            /// <value>The dashboard ID.</value>
            [DatabaseColumn("DashboardList_Dashboard_Key", SqlDbType.Int)]
            public int DashboardID { get; set; }

            /// <summary>
            /// Gets or sets the list ID.
            /// </summary>
            /// <value>The list ID.</value>
            [DatabaseColumn("DashboardList_List_Key", SqlDbType.Int)]
            public int ListID { get; set; }

            /// <summary>
            /// Gets or sets the column ID.
            /// </summary>
            /// <value>The column ID.</value>
            [DatabaseColumn("DashboardList_Column", SqlDbType.TinyInt)]
            public int ColumnID { get; set; }

            /// <summary>
            /// Gets or sets the column ID.
            /// </summary>
            /// <value>The column ID.</value>
            [DatabaseColumn("DashboardList_SortOrder", SqlDbType.Int)]
            public int SortOrder { get; set; }
        }
        #endregion

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public static Dashboard[] SelectAll()
        {
            List<Dashboard> list = new List<Dashboard>();

            Dashboard implement = new Dashboard();
            foreach (object o in implement._SelectAll()) list.Add((Dashboard)o);
            return list.ToArray();
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static Dashboard SelectOne(int ID)
        {
            return (Dashboard)new Dashboard()._SelectOne(ID);
        }

        int m_ID;
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        [DatabaseColumn("Dashboard_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        private Guid m_GUID;
        /// <summary>
        /// Gets or sets the GUID.
        /// </summary>
        /// <value>The GUID.</value>
        [DatabaseColumn("Dashboard_Guid", SqlDbType.UniqueIdentifier, IsMigrationKey = true)]
        public Guid GUID
        {
            get {
                if (this.m_GUID == Guid.Empty) this.m_GUID = Guid.NewGuid();
                return m_GUID; 
            }
            set { m_GUID = value; }
        }

        string m_Name;
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Name", 50, true)]
        [DatabaseColumn("Dashboard_Name", SqlDbType.NVarChar, Length = 50, IsNullable = false)]
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        string m_Title;
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Title", 50, false, "Use [user] as a replacement tag for the the application user displayname")]
        [DatabaseColumn("Dashboard_Title", SqlDbType.NVarChar, Length = 50, IsNullable = true)]
        public string Title
        {
            get { return m_Title; }
            set { m_Title = value; }
        }

        string m_Body;
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.RichText("Intro", 0, false)]
        [DatabaseColumn("Dashboard_Body", SqlDbType.NText, IsNullable = true)]
        public string Body
        {
            get { return m_Body; }
            set { m_Body = value; }
        }

        //int m_Type;
        ///// <summary>
        ///// Gets or sets the type (1= 2 even colummn).
        ///// </summary>
        ///// <value>The type.</value>
        //[Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Type", "TypeCollection", false)]
        //[DatabaseColumn("Dashboard_Type", SqlDbType.TinyInt)]
        //[Obsolete("MEDIAKIWI")]
        //public int Type
        //{
        //    get { return m_Type; }
        //    set { m_Type = value; }
        //}

        DateTime m_Created;
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        [DatabaseColumn("Dashboard_Created", SqlDbType.DateTime)]
        public DateTime Created
        {
            get {
                if (m_Created == DateTime.MinValue) m_Created = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
                return m_Created; 
            }
            set { m_Created = value; }
        }

        /// <summary>
        /// Updates the specified dashboard.
        /// </summary>
        /// <param name="dashboard">The dashboard.</param>
        /// <param name="column">The column.</param>
        /// <param name="subList">The sub list.</param>
        public static void Update(int dashboard, int column, Sushi.Mediakiwi.Data.SubList subList)
        {
            Prepare(dashboard, column);
            if (subList != null && subList.Items != null && subList.Items.Length > 0)
            {
                int sortOrder = 0;
                Dashboard implement = new Dashboard();
                foreach (Sushi.Mediakiwi.Data.SubList.SubListitem item in subList.Items)
                {
                    sortOrder++;

                    int count = Wim.Utility.ConvertToInt(implement.Execute(string.Format("select COUNT(*) FROM wim_DashboardLists where DashboardList_Dashboard_Key = {0} and DashboardList_List_Key = {1} and DashboardList_Column = {2}", dashboard, item.ID, column)));
                    if (count == 0)
                        implement.Execute(string.Format("insert into wim_DashboardLists(DashboardList_Dashboard_Key, DashboardList_List_Key, DashboardList_Column, DashboardList_Update, DashboardList_SortOrder) values ({0}, {1}, {2}, 1, {3})", dashboard, item.ID, column, sortOrder));
                    else
                        implement.Execute(string.Format("update wim_DashboardLists set DashboardList_Update = 1, DashboardList_SortOrder = {3} where DashboardList_Dashboard_Key = {0} and DashboardList_List_Key = {1} and DashboardList_Column = {2}", dashboard, item.ID, column, sortOrder));


//                    implement.Execute(string.Format(@"
//if (select COUNT(*) FROM wim_DashboardLists where DashboardList_Dashboard_Key = {0} and DashboardList_List_Key = {1} and DashboardList_Column = {2}) = 0
//    insert into wim_DashboardLists(DashboardList_Dashboard_Key, DashboardList_List_Key, DashboardList_Column, DashboardList_Update, DashboardList_SortOrder) values ({0}, {1}, {2}, 1, {3})
// else
//    update wim_DashboardLists set DashboardList_Update = 1, DashboardList_SortOrder = {3} where DashboardList_Dashboard_Key = {0} and DashboardList_List_Key = {1} and DashboardList_Column = {2}
//"
//                        , dashboard, item.ID, column, sortOrder));
                }
            }
            CleanUp(dashboard, column);
        }

        /// <summary>
        /// Prepares the specified dashboard.
        /// </summary>
        /// <param name="dashboard">The dashboard.</param>
        /// <param name="column">The column.</param>
        static void Prepare(int dashboard, int column)
        {
            Dashboard implement = new Dashboard();
            implement.Execute(string.Format("update wim_DashboardLists set DashboardList_Update = 0 where DashboardList_Dashboard_Key = {0} and DashboardList_Column = {1}", dashboard, column));
        }

        /// <summary>
        /// Cleans up.
        /// </summary>
        /// <param name="dashboard">The dashboard.</param>
        /// <param name="column">The column.</param>
        static void CleanUp(int dashboard, int column)
        {
            Dashboard implement = new Dashboard();
            implement.Execute(string.Format("delete from wim_DashboardLists where DashboardList_Update = 0 and DashboardList_Dashboard_Key = {0} and DashboardList_Column = {1}", dashboard, column));
        }

        private Data.IComponentList[] _DashboardTarget;
        /// <summary>
        /// Gets the dashboard target1.
        /// </summary>
        /// <value>The dashboard target1.</value>
        public Data.IComponentList[] DashboardTarget
        {
            get
            {
                if (_DashboardTarget == null)
                    _DashboardTarget = ComponentList.SelectAllDashboardLists(this.ID, 0);
                return _DashboardTarget;
            }
        }

        //private Data.ComponentList[] m_DashboardTarget1;
        ///// <summary>
        ///// Gets the dashboard target1.
        ///// </summary>
        ///// <value>The dashboard target1.</value>
        //[Obsolete("MEDIAKIWI")]
        //public Data.ComponentList[] DashboardTarget1
        //{
        //    get
        //    {
        //        if (m_DashboardTarget1 == null)
        //            m_DashboardTarget1 = ComponentList.SelectAllDashboardLists(this.ID, 1);
        //        return m_DashboardTarget1;
        //    }
        //}

        //private Data.ComponentList[] m_DashboardTarget2;
        ///// <summary>
        ///// Gets the dashboard target2.
        ///// </summary>
        ///// <value>The dashboard target2.</value>
        //[Obsolete("MEDIAKIWI")]
        //public Data.ComponentList[] DashboardTarget2
        //{
        //    get
        //    {
        //        if (m_DashboardTarget2 == null)
        //            m_DashboardTarget2 = ComponentList.SelectAllDashboardLists(this.ID, 2);
        //        return m_DashboardTarget2;
        //    }
        //}

        //private Data.ComponentList[] m_DashboardTarget3;
        ///// <summary>
        ///// Gets the dashboard target2.
        ///// </summary>
        ///// <value>The dashboard target2.</value>
        //[Obsolete("MEDIAKIWI")]
        //public Data.ComponentList[] DashboardTarget3
        //{
        //    get
        //    {
        //        if (m_DashboardTarget3 == null)
        //            m_DashboardTarget3 = ComponentList.SelectAllDashboardLists(this.ID, 3);
        //        return m_DashboardTarget3;
        //    }
        //}

        //private Data.ComponentList[] m_DashboardTarget4;
        ///// <summary>
        ///// Gets the dashboard target2.
        ///// </summary>
        ///// <value>The dashboard target2.</value>
        //[Obsolete("MEDIAKIWI")]
        //public Data.ComponentList[] DashboardTarget4
        //{
        //    get
        //    {
        //        if (m_DashboardTarget4 == null)
        //            m_DashboardTarget4 = ComponentList.SelectAllDashboardLists(this.ID, 4);
        //        return m_DashboardTarget4;
        //    }
        //}


        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard
    }
}
