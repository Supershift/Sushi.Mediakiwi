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
    public class SimpleGenerics : DatabaseEntity, ISimpleGenerics
    {
        /// <summary>
        /// Gets or sets the cache value.
        /// </summary>
        /// <value>The cache value.</value>
        protected string CacheValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected internal int ListID;
        protected internal int SiteID;

        /// <summary>
        /// Save a database entity. This method will indentify the need for an INSERT or an UPDATE.
        /// </summary>
        /// <returns></returns>
        public override bool Save()
        {
            if (this.ListID == 0)
                this.ListID = this.List.ID;

            if (List == null)
                m_List = Wim.Data.ComponentList.SelectOne(this.ListID);

            if (List.CatalogID == 0)
                throw new Exception(string.Format("There is no catalog assigned to the requested list with the following properties:\n\nName: {2}\nID: {0}\nReference ID: {1}\n\nResolution (WIM): Assign a catalog under Administration > Templates > ComponentLists > Tab: Data.", List.ID, List.ReferenceID, List.Name));

            bool shouldSetSortorder = (this.ID == 0);

            this.PropertyListID = ListID;
            this.SqlColumnPrefix = List.Catalog().ColumnPrefix;
            this.SqlTable = List.Catalog().Table;

            if (List.Catalog().HasCatalogBaseStructure)
            {
                this.AddSqlParameter(string.Concat(List.Catalog().ColumnPrefix, "_List_Key"), ListID, SqlDbType.Int);
                this.AddSqlParameter(string.Concat(List.Catalog().ColumnPrefix, "_Site_Key"), SiteID, SqlDbType.Int);
            }

            bool save = base.Save();

            if (shouldSetSortorder && List.Catalog().HasSortOrder)
                new Wim.Framework.Templates.SimpleGenericsInstance().Execute(string.Format(@"
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
        /// Initializes a new instance of the <see cref="Generics"/> class.
        /// </summary>
        public SimpleGenerics()
        {
            if (typeof(Wim.Framework.Templates.SimpleGenericsInstance) == this.GetType()) return;
            if (typeof(Wim.Templates.SimpleGenerics) == this.GetType()) return;

            ListReference[] listRef = this.GetType().GetCustomAttributes(typeof(ListReference), true) as ListReference[];

            if (listRef.Length == 0)
                throw new Exception("Please apply the [Wim.Framework.ListReference(\"GUID\")] attribute to the class.");

            m_List = Wim.Data.ComponentList.SelectOne(listRef[0].ListGUID, DatabaseMappingPortal);

            if (m_List.ID == 0)
                throw new Exception(string.Format("Could not determine the Componentlist based on the GUID [{0}]", listRef[0]));

            if (m_List.CatalogID == 0)
                throw new Exception(string.Format("There is no catalog assigned to the requested list with the following properties:\n\nName: {2}\nID: {0}\nReference ID: {1}\n\nResolution (WIM): Assign a catalog under Administration > Templates > ComponentLists > Tab: Data.", List.ID, List.ReferenceID, List.Name));

            this.SqlTable = List.Catalog(DatabaseMappingPortal).Table;
            this.SqlColumnPrefix = List.Catalog(DatabaseMappingPortal).ColumnPrefix;

            if (m_List.Catalog(DatabaseMappingPortal).HasSortOrder)
                this.SqlOrder = string.Concat(List.Catalog(DatabaseMappingPortal).ColumnPrefix, "_SortOrder");

            this.SqlConnectionString = List.Catalog(DatabaseMappingPortal).ConnectionString;
            this.SqlTableKey = List.Catalog(DatabaseMappingPortal).ColumnKey;
            this.SqlTableGUID = List.Catalog(DatabaseMappingPortal).ColumnGuid;

            this.PropertyListID = List.ID;
            //  Set to FALSE to avoid the addition in the BaseSQLEntity!
            this.IsGenericEntity = false;
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

            if (List.Catalog().HasSortOrder)
                this.SqlOrder = string.Concat(List.Catalog().ColumnPrefix, "_SortOrder");

            this.SqlColumnPrefix = List.Catalog().ColumnPrefix;
            this.SqlConnectionString = List.Catalog().ConnectionString;
            this.SqlTableKey = List.Catalog().ColumnKey;
            this.SqlTableGUID = List.Catalog().ColumnGuid;
            this.PropertyListID = List.ID;
            
            //  Set to FALSE to avoid the addition in the BaseSQLEntity!
            this.IsGenericEntity = false;
        }

        int m_ID;
        /// <summary>
        /// Unique identifier of this component
        /// </summary>
        /// <value>The ID.</value>
        [DatabaseColumn("<SQL_COL>_Key", SqlDbType.Int, IsPrimaryKey = true, CollectionLevel = DatabaseColumnGroup.Minimal)]
        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
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


        Wim.Data.CustomData m_Container;
        /// <summary>
        /// Gets or sets the container.
        /// </summary>
        /// <value>The container.</value>
        [Wim.Framework.ContentListItem.ContentContainer()]
        [DatabaseColumn("<SQL_COL>_Data", SqlDbType.Xml, IsNullable = true)]
        public Wim.Data.CustomData Data
        {
            get
            {
                if (m_Container == null)
                    m_Container = new Wim.Data.CustomData();
                return m_Container;
            }
            set { m_Container = value; }
        }
    }
}
