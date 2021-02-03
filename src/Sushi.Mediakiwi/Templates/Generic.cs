using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using Wim.Data.DalReflection;
using Wim.Framework;

namespace Wim.Templates
{
    /// <summary>
    /// 
    /// </summary>
    public class Generic : DatabaseEntity, Wim.Templates.IGeneric
    {
        /// <summary>
        /// Selects the instance.
        /// </summary>
        /// <param name="listID">The list ID.</param>
        /// <param name="siteID">The site ID.</param>
        /// <returns></returns>
        public static Generic SelectInstance(int listID, int siteID)
        {
            Generic generic = new Generic();
            generic.PropertyListID = listID;
            generic.ApplyListInformation(Wim.Data.ComponentList.SelectOne(listID));

            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            // <SQL_COL> Will be replaced automatically
            where.Add(new DatabaseDataValueColumn("<SQL_COL>_Site_Key", SqlDbType.Int, siteID));
            return (Generic)generic._SelectOne(where);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="listID">The list ID.</param>
        /// <returns></returns>
        public static Generic[] SelectAll(int listID)
        {
            Generic generic = new Generic();
            generic.PropertyListID = listID;

            Wim.Data.IComponentList clist = Wim.Data.ComponentList.SelectOne(listID);
            generic.ApplyListInformation(clist);

            List<Generic> list = new List<Generic>();
            // <SQL_COL> Will be replaced automatically
            foreach (Generic o in generic._SelectAll())
            {
                Generic implement = (Generic)o;
                list.Add(implement);
            }
            return list.ToArray();
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="listID">The list ID.</param>
        /// <param name="siteID">The site ID.</param>
        /// <returns></returns>
        public static Generic[] SelectAll(int listID, int siteID)
        {
            Generic generic = new Generic();
            generic.PropertyListID = listID;
            
            Wim.Data.IComponentList clist = Wim.Data.ComponentList.SelectOne(listID);
            generic.ApplyListInformation(clist);

            List<Generic> list = new List<Generic>();
            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            // <SQL_COL> Will be replaced automatically
            where.Add(new DatabaseDataValueColumn("<SQL_COL>_Site_Key", SqlDbType.Int, siteID));

            foreach (Generic o in generic._SelectAll(where))
            {
                Generic implement = (Generic)o;
                list.Add(implement);
            }
            return list.ToArray();
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="listID">The list ID.</param>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static Generic SelectOne(int listID, int ID)
        {
            Generic generic = new Generic();
            generic.PropertyListID = listID;

            Wim.Data.IComponentList clist = Wim.Data.ComponentList.SelectOne(listID);
            generic.ApplyListInformation(clist);

            return (Generic)generic._SelectOne(ID);
        }

        /// <summary>
        /// Save a database entity. This method will indentify the need for an INSERT or an UPDATE.
        /// </summary>
        /// <returns></returns>
        public override bool Save()
        {
            if (this.ListID == 0)
                this.ListID = this.List.ID;

            if (this.SiteID == 0)
                this.List.SiteID.GetValueOrDefault();

            if (List == null)
                m_List = Wim.Data.ComponentList.SelectOne(this.ListID);

            if (List.CatalogID == 0)
                throw new Exception(string.Format("There is no catalog assigned to the requested list with the following properties:\n\nName: {2}\nID: {0}\nReference ID: {1}\n\nResolution (WIM): Assign a catalog under Administration > Templates > ComponentLists > Tab: Data.", List.ID, List.ReferenceID, List.Name));

            bool shouldSetSortorder = (this.ID == 0);

            this.PropertyListID = ListID;
            this.SqlColumnPrefix = List.Catalog().ColumnPrefix;
            this.SqlTable = List.Catalog().Table;
            
            bool save = base.Save();

            if (shouldSetSortorder)
                new Wim.Framework.Templates.GenericInstance().Execute(string.Format(@"
update {0} set {2} = {1} where {1} = {3}"
                    , List.Catalog().Table
                    , string.Concat(List.Catalog().ColumnPrefix, "_Key")
                    , string.Concat(List.Catalog().ColumnPrefix, "_SortOrder")
                    , this.ID));


            return save;
        }

        Wim.Data.IComponentList m_List;
        /// <summary>
        /// Gets the component list.
        /// </summary>
        /// <value>The list.</value>
        public Wim.Data.IComponentList List
        {
            get { return m_List; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Generic"/> class.
        /// </summary>
        public Generic()
        {
            if (typeof(Wim.Framework.Templates.GenericInstance) == this.GetType())
                return;

            if (typeof(Wim.Templates.Generic) == this.GetType())
                return;

            ListReference[] listRef = this.GetType().GetCustomAttributes(typeof(ListReference), true) as ListReference[];

            if (listRef.Length == 0)
                throw new Exception("Please apply the [Wim.Framework.ListReference(\"GUID\")] attribute to the class.");

            m_List = Wim.Data.ComponentList.SelectOne(listRef[0].ListGUID, DatabaseMappingPortal);

            if (m_List.ID == 0)
                throw new Exception(string.Format("Could not determine the Componentlist based on the GUID [{0}]", listRef[0]));

            if (m_List.CatalogID == 0)
                throw new Exception(string.Format("There is no catalog assigned to the requested list with the following properties:\n\nName: {2}\nID: {0}\nReference ID: {1}\n\nResolution (WIM): Assign a catalog under Administration > Templates > ComponentLists > Tab: Data.", List.ID, List.ReferenceID, List.Name));

            this.SqlTable = List.Catalog(DatabaseMappingPortal).Table;
            this.SqlOrder = string.Concat(List.Catalog(DatabaseMappingPortal).ColumnPrefix, "_SortOrder");
            this.SqlColumnPrefix = List.Catalog(DatabaseMappingPortal).ColumnPrefix;
            this.PropertyListID = List.ID;
            this.IsGenericEntity = true;
        }

        /// <summary>
        /// Applies the list information.
        /// </summary>
        /// <param name="listID">The list ID.</param>
        internal void ApplyListInformation(int listID)
        {
            Wim.Data.IComponentList list = Wim.Data.ComponentList.SelectOne(listID);
            ApplyListInformation(list);
        }

        /// <summary>
        /// Applies the list information.
        /// </summary>
        /// <param name="list">The list.</param>
        internal void ApplyListInformation(Wim.Data.IComponentList list)
        {
            m_List = list;

            if (m_List.ID == 0)
                throw new Exception(string.Format("Could not determine the Componentlist based on the ID [{0}]", list.ID));

            if (m_List.CatalogID == 0)
                throw new Exception(string.Format("There is no catalog assigned to the requested list with the following properties:\n\nName: {2}\nID: {0}\nReference ID: {1}\n\nResolution (WIM): Assign a catalog under Administration > Templates > ComponentLists > Tab: Data.", List.ID, List.ReferenceID, List.Name));

            this.SqlTable = List.Catalog().Table;
            this.SqlOrder = string.Concat(List.Catalog().ColumnPrefix, "_SortOrder");
            this.SqlColumnPrefix = List.Catalog().ColumnPrefix;
            this.PropertyListID = List.ID;
            this.IsGenericEntity = true;
        }

        int m_ID;
        /// <summary>
        /// Unique identifier of this component
        /// </summary>
        /// <value>The ID.</value>
        [DatabaseColumn("<SQL_COL>_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        int m_ListID;
        /// <summary>
        /// Gets or sets the list ID.
        /// </summary>
        /// <value>The list ID.</value>
        [DatabaseColumn("<SQL_COL>_List_Key", SqlDbType.Int)]
        public int ListID
        {
            get { return m_ListID; }
            set { m_ListID = value; }
        }

        int m_SiteID;
        /// <summary>
        /// Gets or sets the site ID.
        /// </summary>
        /// <value>The site ID.</value>
        [DatabaseColumn("<SQL_COL>_Site_Key", SqlDbType.Int, IsNullable = true)]
        public int SiteID
        {
            get { return m_SiteID; }
            set { m_SiteID = value; }
        }

        Guid m_GUID;
        /// <summary>
        /// Global Unique identifier of this component
        /// </summary>
        /// <value>The GUID.</value>
        [DatabaseColumn("<SQL_COL>_GUID", SqlDbType.UniqueIdentifier, IsMigrationKey = true)]
        public Guid GUID
        {
            get
            {
                if (this.m_GUID == Guid.Empty) this.m_GUID = Guid.NewGuid();
                return m_GUID;
            }
            set { m_GUID = value; }
        }


        DateTime m_Created;
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        [DatabaseColumn("<SQL_COL>_Created", SqlDbType.DateTime, IsNullable = false)]
        public DateTime Created
        {
            get
            {
                if (this.m_Created == DateTime.MinValue) this.m_Created = Wim.Data.Common.DatabaseDateTime;
                return m_Created;
            }
            set { m_Created = value; }
        }

        Wim.Data.CustomData m_Container;
        /// <summary>
        /// Gets or sets the container.
        /// </summary>
        /// <value>The container.</value>
        [Wim.Framework.ContentListItem.ContentContainer()]
        [DatabaseColumn("<SQL_COL>_Data", SqlDbType.Xml, IsNullable = true)]
        public Wim.Data.CustomData Data
        {
            get {
                if (m_Container == null)
                    m_Container = new Wim.Data.CustomData();
                return m_Container; 
            }
            set { m_Container = value; }
        }
        //Wim.Data.CustomData m_Data;
        ///// <summary>
        ///// Gets or sets the data.
        ///// </summary>
        ///// <value>The data.</value>

        //public Wim.Data.CustomData Data
        //{
        //    get
        //    {
        //        if (m_Data == null)
        //            m_Data = new Wim.Data.CustomData();
        //        return m_Data;
        //    }
        //    set { m_Data = value; }
        //}
    }
}
