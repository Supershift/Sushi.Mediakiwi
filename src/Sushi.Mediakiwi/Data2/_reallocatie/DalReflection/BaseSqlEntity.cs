using System;
using System.Linq;
using System.Xml.Serialization;
using System.Reflection;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class NameValue
    {
        /// <summary>
        /// Parses the values.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        public static int[] Parse(NameValue[] collection)
        {
            List<int> arr = new List<int>();
            foreach (NameValue i in collection)
                arr.Add(i.ValueInt32);
            return arr.ToArray();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NameValue"/> class.
        /// </summary>
        public NameValue() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="NameValue"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public NameValue(object name, object value)
        {
            this.Name = name == null ? null : name.ToString();
            this.Value = value == null ? null : value.ToString();
        }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value { get; set; }
        /// <summary>
        /// Gets the value int32.
        /// </summary>
        /// <value>The value int32.</value>
        public Int32 ValueInt32
        {
            get { return Wim.Utility.ConvertToInt(this.Value); }
        }
    }
}

namespace Sushi.Mediakiwi.Data.DalReflection
{
    /// <summary>
    /// 
    /// </summary>
    public interface iData
    {
        string Polution { get; }
        /// <summary>
        /// Gets the polution hash pre save.
        /// </summary>
        /// <value>The polution hash pre save.</value>
        string PolutionHashPreSave { get; }
        /// <summary>
        /// Adds the polution ignore property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="ignorePolutionWarning">if set to <c>true</c> [ignore polution warning].</param>
        void AddPolutionIgnoreProperty(string propertyName, bool ignorePolutionWarning);
    }

    /// <summary>
    /// Represents the base entity of a generic SQL source reflection class
    /// </summary>
    public class BaseSqlEntity : iData
    {
        Sushi.Mediakiwi.Data.DataConnectionType m_ConnectionType;
        /// <summary>
        /// Gets the type of the connection.
        /// </summary>
        /// <value>The type of the connection.</value>
        [XmlIgnore()]
        [Sushi.Mediakiwi.Framework.PolutionIgnore()]
        public virtual Sushi.Mediakiwi.Data.DataConnectionType ConnectionType
        {
            get {
                if (DatabaseEntity.DataConnectionType != DataConnectionType.SqlServer)
                    m_ConnectionType = DatabaseEntity.DataConnectionType;

                return m_ConnectionType; 
            }
            set { m_ConnectionType = value; }
        }

        DatabaseColumnGroup m_CollectionLevel;
        /// <summary>
        /// Gets or sets the collection level.
        /// </summary>
        /// <value>The collection level.</value>
        protected virtual DatabaseColumnGroup CollectionLevel
        {
            get { return m_CollectionLevel; }
            set { 
                if (m_CollectionLevel != value) m_SqlParameters = null;
                m_CollectionLevel = value; 
            }
        }

        List<DatabaseColumnAttribute> m_SqlParameters;
        /// <summary>
        /// All set SQL parameters (scanned using the set attributes).
        /// </summary>
        /// <value>The SQL parameters.</value>
        [XmlIgnore()]
        protected virtual List<DatabaseColumnAttribute> SqlParameters
        {
            get
            {
                if (m_SqlParameters != null) 
                    return m_SqlParameters;

                SetSqlParameters();

                return m_SqlParameters;
            }
        }

        /// <summary>
        /// All set SQL parameters (scanned using the set attributes).
        /// </summary>
        void SetSqlParameters()
        {
            if (m_SqlParameters == null)
                m_SqlParameters = new List<DatabaseColumnAttribute>();

            if (m_SqlParameters.Count > 1 && !m_hasAdditionalProperties)
                return;


            PropertyInfo[] props = this.GetType().GetProperties();
            SetSqlParameters(props, this);
        }

        bool m_hasAdditionalProperties;
        /// <summary>
        /// All set SQL parameters (scanned using the set attributes).
        /// </summary>
        void SetAdditionalSqlParameters()
        {
            if (m_SqlParameters == null)
                m_SqlParameters = new List<DatabaseColumnAttribute>();

            PropertyInfo[] props = this.GetType().GetProperties();
            foreach (PropertyInfo info in props)
            {
                DatabaseEntityAttribute[] attributes = (DatabaseEntityAttribute[])info.GetCustomAttributes(typeof(DatabaseEntityAttribute), false);
                foreach (DatabaseEntityAttribute attribute in attributes)
                {
                    if (attribute != null)
                    {
                        if ((int)attribute.CollectionLevel < (int)CollectionLevel)
                            continue;

                        m_hasAdditionalProperties = true;
                        SetSqlParameters(info.PropertyType.GetProperties(), info.GetValue(this, null));
                    }
                }
            }
        }

        /// <summary>
        /// Sets the SQL parameters.
        /// </summary>
        /// <param name="props">The props.</param>
        /// <param name="entity">The entity.</param>
        void SetSqlParameters(PropertyInfo[] props, object entity)
        {
            try
            {
                foreach (PropertyInfo info in props)
                {
                    DatabaseColumnAttribute[] attributes = (DatabaseColumnAttribute[])info.GetCustomAttributes(typeof(DatabaseColumnAttribute), false);
                    foreach (DatabaseColumnAttribute attribute in attributes)
                    {
                        //  All = 0 || Basic = 1 || Minimal = 2
                        //  Give me everything from Minimal ==> CollectionLevel >= 2
                        //  Give me everything from Basic ==> CollectionLevel >= 1
                        //  Give me everything from All ==> CollectionLevel >= 0
                        if (attribute != null)
                        {
                            attribute.Info = info;
                            attribute.Entity = entity;

                            if ((int)attribute.CollectionLevel < (int)CollectionLevel)
                                continue;

                            if (!string.IsNullOrEmpty(this.SqlColumnPrefix) && attribute.Column.Contains("<SQL_COL>"))
                            {
                                if (attribute.Column == "<SQL_COL>_Key" && !string.IsNullOrEmpty(this.SqlTableKey))
                                    attribute.Column = this.SqlTableKey;
                                else
                                    if (attribute.Column == "<SQL_COL>_GUID" && !string.IsNullOrEmpty(this.SqlTableGUID))
                                        attribute.Column = this.SqlTableGUID;
                                    else
                                        attribute.Column = attribute.Column.Replace("<SQL_COL>", this.SqlColumnPrefix);
                            }

                            m_SqlParameters.Add(attribute);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Sushi.Mediakiwi.Data.Notification.InsertOne("WIM error", ex);
            }

            if (m_SqlParameters == null)
                throw new Exception(string.Format("You forgot to apply any [DatabaseColumnAttribute] attribute to the {0} class",
                    this.GetType().FullName));
        }

        /// <summary>
        /// Return the value of a property using reflection.
        /// Has to be overridden when using ColumnSubQueryPropertyReference.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        protected virtual string GetPropertyValue(string name)
        {
            if (string.IsNullOrEmpty(name)) return "null";

            PropertyInfo prop = this.GetType().GetProperty(name);
            if (prop == null || !prop.CanRead)
                throw new Exception(string.Format("The requested property ('{0}') is not accesible or does not exist", name));

            if (prop.PropertyType != typeof(string))
                throw new Exception(string.Format("The requested property ('{0}') is not of the correct type (System.String)", name));

            object value = prop.GetValue(this, null);

            if (value == null)
                return "null";
            return value.ToString();
        }

        string m_SqlTable;
        /// <summary>
        /// Table
        /// </summary>
        /// <value>The SQL table.</value>
        [XmlIgnore()]
        protected virtual string SqlTable
        {
            get { return m_SqlTable; }
            set { m_SqlTable = value; }
        }

        /// <summary>
        /// Gets the SQL table checked (check for spaces when using [TableName] XX.
        /// </summary>
        /// <value>The SQL table checked.</value>
        [XmlIgnore()]
        protected virtual string SqlTableChecked
        {
            get {
                if (this.SqlTable.Contains(" "))
                    return this.SqlTable.Split(' ')[0];
                else
                    return this.SqlTable;                
            }
        }

        string m_SqlJoin;
        /// <summary>
        /// Addition joins
        /// </summary>
        /// <value>The SQL join.</value>
        [XmlIgnore()]
        protected virtual string SqlJoin
        {
            get { 
                return m_SqlJoin;
            }
            set { m_SqlJoin = value; }
        }



        string m_SqlGroup;
        /// <summary>
        /// Group by clause
        /// </summary>
        /// <value>The SQL group.</value>
        [XmlIgnore()]
        protected virtual string SqlGroup
        {
            get {
                return m_SqlGroup;
            }
            set { m_SqlGroup = value; }
        }

        string m_SqlOrder;
        /// <summary>
        /// Order by clause
        /// </summary>
        /// <value>The SQL order.</value>
        [XmlIgnore()]
        protected virtual string SqlOrder
        {
            get {
                return m_SqlOrder; 
            }
            set { m_SqlOrder = value; }
        }

        string m_SqlLastExecuted;
        /// <summary>
        /// Last executed SQL Statement
        /// </summary>
        /// <value>The SQL last executed.</value>
        [XmlIgnore()]
        protected virtual string SqlLastExecuted
        {
            get { return m_SqlLastExecuted; }
            set { m_SqlLastExecuted = value; }
        }

        [XmlIgnore()]
        string m_SqlColumnPrefix;
        /// <summary>
        /// Gets or sets the SQL column prefix.
        /// </summary>
        /// <value>The SQL column prefix.</value>
        protected virtual string SqlColumnPrefix
        {
            get { return m_SqlColumnPrefix; }
            set {
                m_SqlColumnPrefix = value; 
            }
        }

        /// <summary>
        /// Gets or sets the SQL table key.
        /// </summary>
        /// <value>The SQL table key.</value>
        [XmlIgnore()]
        protected virtual string SqlTableKey { get; set; }

        /// <summary>
        /// Gets or sets the SQL table GUID.
        /// </summary>
        /// <value>The SQL table GUID.</value>
        [XmlIgnore()]
        protected virtual string SqlTableGUID { get; set; }

        [XmlIgnore()]
        protected internal virtual int? m_PropertyListTypeID { get; set; }
        /// <summary>
        /// Gets or sets the property list type ID. This is used for showing customdata in data grid presentation.
        /// </summary>
        /// <value>The property list type ID.</value>
        [XmlIgnore()]
        protected virtual int? PropertyListTypeID
        {
            get { return m_PropertyListTypeID; }
            set { m_PropertyListTypeID = value; }
        }

        /// <summary>
        /// Sets the property list information.
        /// </summary>
        /// <param name="propertyListID">The property list ID.</param>
        protected internal virtual void SetPropertyListInformation(int propertyListID)
        {
            this.PropertyListID = propertyListID;
        }

        int? m_PropertyListID;
        /// <summary>
        /// Gets or sets the property list ID.
        /// </summary>
        /// <value>The property list ID.</value>
        [XmlIgnore()]
        protected virtual int? PropertyListID
        {
            get { return m_PropertyListID; }
            set { m_PropertyListID = value; }
        }

        int? m_PropertyListItemID2;

        int? m_PropertyListItemID;
        /// <summary>
        /// Gets or sets the property list item ID.
        /// </summary>
        /// <value>The property list item ID.</value>
        [XmlIgnore()]
        protected virtual int? PropertyListItemID
        {
            get { return m_PropertyListItemID; }
            set { m_PropertyListItemID = value; }
        }

        protected internal virtual bool IsGenericEntity { get; set; }

        //string m_SqlSelectColumnPrefix;
        ///// <summary>
        ///// Gets or sets the SQL select column prefix. This value is added (prefix) to each column reference in a select statement: f.e. COLUMN_KEY becomes A.COLUMN_KEY where A (without the dot (.) is the SqlSelectColumnPrefix
        ///// </summary>
        ///// <value>The SQL select column prefix.</value>
        //protected string SqlSelectColumnPrefix
        //{
        //    get { return m_SqlSelectColumnPrefix; }
        //    set { m_SqlSelectColumnPrefix = value; }
        //}

        bool m_SqlOnlySetStatement;
        /// <summary>
        /// Sets only the SQL statement and does not execute it
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [SQL only set statement]; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore()]
        protected virtual bool SqlOnlySetStatement
        {
            get { return m_SqlOnlySetStatement; }
            set { m_SqlOnlySetStatement = value; }
        }

        int m_SqlRowCount;
        /// <summary>
        /// Result count
        /// </summary>
        /// <value>The SQL row count.</value>
        [XmlIgnore()]
        protected virtual int SqlRowCount
        {
            get { return m_SqlRowCount; }
            set { m_SqlRowCount = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected internal virtual Sushi.Mediakiwi.Framework.WimServerPortal DatabaseMappingPortal { get; set; } 

        /// <summary>
        /// CTor
        /// </summary>
        public BaseSqlEntity()
        {
            this.m_ConnectionType = Sushi.Mediakiwi.Data.Common.DatabaseConnectionType;
            DatabaseTableAttribute[] attributes =
                (DatabaseTableAttribute[])this.GetType().GetCustomAttributes(typeof(DatabaseTableAttribute), true);

            string portal = null;
            foreach (DatabaseTableAttribute attribute in attributes)
            {
                if (attribute == null) continue;

                if (!string.IsNullOrEmpty(attribute.Join)) m_SqlJoin = attribute.Join;
                if (!string.IsNullOrEmpty(attribute.Order)) m_SqlOrder = attribute.Order;
                if (!string.IsNullOrEmpty(attribute.Group)) m_SqlGroup = attribute.Group;
                if (!string.IsNullOrEmpty(attribute.Portal)) portal = attribute.Portal;

                m_SqlTable = attribute.Name;
                if (m_SqlTable == null)
                {
                    throw new Exception(string.Format("You forgot to apply the [DatabaseTableAttribute] attribute to the {0} class",
                        this.GetType().FullName));
                }
            }
            DatabaseMappingPortal = Sushi.Mediakiwi.Data.Common.GetCurrentMappingConnection(this.GetType());

            if (DatabaseMappingPortal == null && portal != null)
            {
                DatabaseMappingPortal = Sushi.Mediakiwi.Data.Common.GetPortal(portal);
                if (DatabaseMappingPortal == null)
                {
                    throw new Exception(string.Format("Could not find the requested portal [{0}] in web.config", portal));
                }
            }

            if (DatabaseMappingPortal != null) this.SqlConnectionString = DatabaseMappingPortal.Connection;
        }

        /// <summary>
        /// The order of the result.
        /// </summary>
        /// <value>The result order.</value>
        [XmlIgnore()]
        string ResultOrder
        {
            get { return string.IsNullOrEmpty(this.SqlOrder) 
                    ? null
                    : string.Concat(" order by ", this.SqlOrder); 
            }
        }

        /// <summary>
        /// The Group of the result.
        /// </summary>
        /// <value>The result group.</value>
        [XmlIgnore()]
        string ResultGroup
        {
            get
            {
                return string.IsNullOrEmpty(this.SqlGroup)
                  ? null
                  : string.Concat(" group by ", this.SqlGroup);
            }
        }

        /// <summary>
        /// The name of the primary key column.
        /// </summary>
        /// <value>The primairy key column.</value>
        [XmlIgnore()]
        protected virtual string PrimairyKeyColumn
        {
            get
            {
                if (m_primary == null) SetPrimary();
                if (m_primary == null) return null;
                else
                    return m_primary.Column;
            }
        }


        /// <summary>
        /// The name of the primary key column.
        /// </summary>
        /// <value>The migration key column.</value>
        [XmlIgnore()]
        protected virtual string MigrationKeyColumn
        {
            get
            {
                if (m_mgration == null) SetMigration();
                if (m_mgration == null) return null;
                else
                    return m_mgration.Column;
            }
        }

        DatabaseColumnAttribute m_mgration;
        /// <summary>
        /// Set (scan for) the set migration key.
        /// </summary>
        void SetMigration()
        {
            foreach (DatabaseColumnAttribute param in SqlParameters)
            {
                if (param.IsMigrationKey)
                {
                    m_mgration = param;
                    break;
                }
            }
        }

        DatabaseColumnAttribute m_primary;
        /// <summary>
        /// Set (scan for) the set primary key.
        /// </summary>
        void SetPrimary()
        {
            foreach (DatabaseColumnAttribute param in SqlParameters)
            {
                if (param == null) continue;
                if (param.IsPrimaryKey)
                {
                    m_primary = param;
                    break;
                }
            }
        }


        /// <summary>
        /// Is the current instance a new db entry (based on the set primary key).
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is new instance; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore()]
        public virtual bool IsNewInstance
        {
            get
            {
                if (PrimairyKeyValue.HasValue && PrimairyKeyValue.Value != 0)
                    return false;
                return true;
            }
        }

        /// <summary>
        /// The value of the primary key column.
        /// </summary>
        /// <value>The primairy key value.</value>
        [XmlIgnore()]
        protected virtual int? PrimairyKeyValue
        {
            get
            {
                if (m_primary == null) SetPrimary();
                if (m_primary == null) return null;

                int value;
                if (Wim.Utility.IsNumeric(m_primary.Info.GetValue(this, null), out value))
                    return value;
                return null;
            }
        }

        /// <summary>
        /// Gets the migration key value.
        /// </summary>
        /// <value>The migration key value.</value>
        [XmlIgnore()]
        protected virtual Guid? MigrationKeyValue
        {
            get
            {
                if (m_mgration == null) return null;

                Guid value;
                if (Wim.Utility.IsGuid(m_mgration.Info.GetValue(this, null), out value))
                    return value;
                return null;
            }
        }

        /// <summary>
        /// Select a implementation based on the specified attributed and the applied primary key identifier.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        protected virtual object _SelectOne(int key)
        {
            return _SelectOne(key, true);
        }

        string m_CacheKey;
        string m_CurrentCacheKey;
        protected internal virtual Wim.Utilities.CacheItemManager m_CacheManager { get; set; }
        System.Collections.Hashtable m_DatabaseEntities;
        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        [XmlIgnore()]
        System.Collections.Hashtable Items
        {
            get
            {
                if (m_CacheManager == null)
                    m_CacheManager = new Wim.Utilities.CacheItemManager();

                if (m_DatabaseEntities == null || m_CacheKey != m_CurrentCacheKey)
                {
                    if (m_CacheManager.IsCached(m_CacheKey))
                        m_DatabaseEntities = (System.Collections.Hashtable)m_CacheManager.GetItem(m_CacheKey);
                    else
                    {
                        m_DatabaseEntities = new System.Collections.Hashtable();
                    }
                }
                m_CurrentCacheKey = m_CacheKey;

                return m_DatabaseEntities;
            }
        }

        void SaveCache()
        {
            if (m_DatabaseEntities == null) return;
            m_CacheManager.Add(m_CacheKey, m_DatabaseEntities, Wim.CommonConfiguration.DefaultCacheTimeSpan, null);
        }

        //void AddTrace(string category)
        //{
        //    if (System.Web.HttpContext.Current == null || !System.Web.HttpContext.Current.Trace.IsEnabled) return;
        //    AddTrace(category, this.SqlLastExecuted);
        //    AddTrace("Params", this.SqlLastExecutedWhereClause);
        //}

        ///// <summary>
        ///// Adds the trace.
        ///// </summary>
        ///// <param name="category">The category.</param>
        ///// <param name="message">The message.</param>
        //protected void AddTrace(string category, string message)
        //{
        //    if (!IsTraceEnabled()) return;
        //    System.Web.HttpContext.Current.Trace.Write(category, message);
        //}

        /// <summary>
        /// Determines whether [is trace enabled].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is trace enabled]; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool IsTraceEnabled()
        {
            if (System.Web.HttpContext.Current == null || !System.Web.HttpContext.Current.Trace.IsEnabled) return false;
            return true;
        }

        string m_SqlConnectionString;
        /// <summary>
        /// The Database connection string (default : ConfigurationManager.AppSettings["connect"])
        /// </summary>
        /// <value>The SQL connection string.</value>
        [XmlIgnore()]
        [Sushi.Mediakiwi.Framework.PolutionIgnore()]
        public virtual string SqlConnectionString
        {
            get {
              
                if (string.IsNullOrEmpty(m_SqlConnectionString))
                {
                    if (this.DatabaseMappingPortal == null)
                        m_SqlConnectionString = Sushi.Mediakiwi.Data.Common.DatabaseConnectionString;
                    else
                        m_SqlConnectionString = DatabaseMappingPortal.Connection;
                }
                else if (this.DatabaseMappingPortal != null && this.DatabaseMappingPortal.Connection != m_SqlConnectionString)
                    m_SqlConnectionString = DatabaseMappingPortal.Connection;

                return m_SqlConnectionString; 
            }
            set { m_SqlConnectionString = value; }
        }

        string m_SqlConnectionDatabase;
        /// <summary>
        /// Gets the SQL connection database.
        /// </summary>
        /// <value>The SQL connection database.</value>
        string SqlConnectionDatabase
        {
            get
            {
                if (string.IsNullOrEmpty(m_SqlConnectionDatabase))
                {
                    string[] split = this.SqlConnectionString.Split(';');
                    foreach (string item in split)
                    {
                        if (item.StartsWith("database", StringComparison.OrdinalIgnoreCase))
                        {
                            if (item.Contains("="))
                                m_SqlConnectionDatabase = item.Split('=')[1];
                            break;
                        }
                    }
                }
                return m_SqlConnectionDatabase;
            }
        }

        /// <summary>
        /// Cleans up.
        /// </summary>
        void CleanUp()
        {
            m_SqlParameters = null;
        }

        /// <summary>
        /// _s the select one.
        /// </summary>
        /// <param name="whereColumns">The where columns.</param>
        /// <returns></returns>
        protected virtual object _SelectOne(List<DatabaseDataValueColumn> whereColumns)
        {
            return _SelectOne(whereColumns, null, null);
        }

        /// <summary>
        /// _s the select one.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="cacheResult">if set to <c>true</c> [cache result].</param>
        /// <returns></returns>
        protected virtual object _SelectOne(int key, bool cacheResult)
        {
            return _SelectOne(key, cacheResult, null);
        }

        /// <summary>
        /// Select a implementation based on the specified attributed and the applied primary key identifier.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="cacheResult">if set to <c>true</c> [cache result].</param>
        /// <param name="componentListID">The component list ID.</param>
        /// <returns></returns>
        protected virtual object _SelectOne(int key, bool cacheResult, int? componentListID)
        {
            if (key == 0) return this;

            if (componentListID.HasValue)
            {
                var list2 = Sushi.Mediakiwi.Data.ComponentList.SelectOne(componentListID.Value);
                this.SqlTable = list2.Catalog().Table;
                this.SqlColumnPrefix = list2.Catalog().ColumnPrefix;
            }

            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            list.Add(new DatabaseDataValueColumn(PrimairyKeyColumn, SqlDbType.Int, key));

            //  Added a additional ckey element as of the Filter options
            string ckey = this.PropertyListItemID.HasValue ? string.Concat(key, "#", this.PropertyListItemID) : key.ToString();

            object t;
            if (cacheResult)
                t = this._SelectOne(list, "ID", ckey);
            else
                t = this._SelectOne(list, null, null);

            return t;
        }

        // MV: I had to introduce this because the signature change (add cacheNewInstance) with default value did not work
        protected virtual object _SelectOne(List<DatabaseDataValueColumn> whereColumns, string cacheReference, string cacheValue)
        {
            return _SelectOne(whereColumns, cacheReference, cacheValue, true);
        }

        /// <summary>
        /// Select a implementation based on the specified attributed and the applied whereColumns which define the where clause.
        /// </summary>
        /// <param name="whereColumns">The where columns.</param>
        /// <param name="cacheReference">The Reference and the Value form a combined unique cachekey.</param>
        /// <param name="cacheValue">The Reference and the Value form a combined unique cachekey.</param>
        /// <param name="cacheNewInstance">If set to true, a new instance is cached when there is no row found in database. If false, no cache entry is made for non-existing database entities</param>
        /// <returns></returns>
        protected virtual object _SelectOne(List<DatabaseDataValueColumn> whereColumns, string cacheReference, string cacheValue, bool cacheNewInstance)
        {
            bool cacheResult = false;
            if (!string.IsNullOrEmpty(cacheReference) && !string.IsNullOrEmpty(cacheValue))
            {
                cacheResult = true;
                m_CacheKey = string.Concat(this.SqlConnectionDatabase, ":Data_", this.GetType().ToString(), ".", cacheReference);
                if (Items.ContainsKey(cacheValue))
                {
                    // MV 2017-05-01: when requested, 'new instance' entities can be ignored from cache
                    if (cacheNewInstance == false)
                    {
                        var candidate = Items[cacheValue] as BaseSqlEntity;
                        if (candidate == null || !candidate.IsNewInstance)
                            return Items[cacheValue] as object;
                        else
                        {
                            //this is the path where the cached version is ignored because it is 'new instance'
                        }
                    }
                    else
                        return Items[cacheValue] as object;
                }
            }

            object result;
            if (ConnectionType != DataConnectionType.SqlServer)
            {
                result = this._SelectOneOdbcOrOleDb(whereColumns);
                if (cacheResult)
                {
                    this.Items[cacheValue] = result;
                    this.SaveCache();
                }

                return result;
            }

            string selectColumns = "";
            string whereClause = "";

            using (Connection.SqlCommander dac = new Connection.SqlCommander(SqlConnectionString))
            {
                whereClause = GetWhereClause(dac, whereColumns);

                SetAdditionalSqlParameters();
                SetSqlParameters();

                Sushi.Mediakiwi.Data.Property[] m_propertyList = null; 
                foreach (DatabaseColumnAttribute param in SqlParameters)
                {
                    if (!string.IsNullOrEmpty(param.ColumnSubQuery))
                        selectColumns += string.Format("({1}) as {0}, ", param.Column, param.ColumnSubQuery).ToLower();
                    else if (!string.IsNullOrEmpty(param.ColumnSubQueryPropertyReference))
                        selectColumns += string.Format("({1}) as {0}, ", param.Column, GetPropertyValue(param.ColumnSubQueryPropertyReference)).ToLower();
                    else
                    {
                        selectColumns += string.Concat(param.Column, ", ").ToLower();
                    }
                }

                //  NEW 15-05-2009
                if (this.PropertyListID.HasValue)
                {
                    //Sushi.Mediakiwi.Data.Property.SqlConnectionString2 = this.SqlConnectionString;
                    //m_propertyList = Sushi.Mediakiwi.Data.Property.SelectAll(this.PropertyListID.Value, this.PropertyListItemID, false);
                    m_propertyList = Sushi.Mediakiwi.Data.Property.SelectAll(this.PropertyListID.Value, this.PropertyListItemID, false, false, true, DatabaseMappingPortal);
                    foreach (Sushi.Mediakiwi.Data.Property p in m_propertyList)
                    {
                        if (!string.IsNullOrEmpty(p.Filter))
                        {
                            string filter = p.Filter.ToLower();
                            if (!selectColumns.Contains(string.Concat(filter, ", ")))
                                selectColumns += string.Concat(filter, ", ");
                        }
                    }
                }

                if (selectColumns.Length == 0)
                    throw new Exception("No columns set for the select statement");

                string sql = string.Format("set rowcount 1 select {0} from {1} {2} {3}{4}",
                    selectColumns.Substring(0, selectColumns.Length - 2),
                    SqlTable,
                    SqlJoin,
                    whereClause, ResultOrder
                    );

                SqlLastExecuted = sql;
                if (SqlOnlySetStatement) return null;

                dac.SqlText = sql;
                long start = DateTime.Now.Ticks;
                SqlDataReader reader = dac.ExecReader;

                bool found = false;
                while (reader.Read())
                {
                    int index = 0;

                    found = true;
                    foreach (DatabaseColumnAttribute param in SqlParameters)
                    {
                        object value = GetData(reader, index, param);

                        if (param.Info.PropertyType == typeof(Sushi.Mediakiwi.Data.CustomData) && this.PropertyListID.HasValue)
                        {
                            Sushi.Mediakiwi.Data.CustomData customData = SetPropertyValueCustomData(param.Info, value, param.Entity);
                            //Sushi.Mediakiwi.Data.Property[] properties = Sushi.Mediakiwi.Data.Property.SelectAll(this.PropertyListID.Value, this.PropertyListItemID, false);
                            foreach (Sushi.Mediakiwi.Data.Property p in m_propertyList)
                            {
                                if (!string.IsNullOrEmpty(p.Filter))
                                {
                                    if (reader[p.Filter] == DBNull.Value && !customData[p.FieldName].IsNull)
                                        continue;

                                    customData.ApplyObject(p.FieldName, reader[p.Filter]);
                                }
                            }
                        }
                        else
                            SetPropertyValue(param.Info, value, param.Entity);

                        index++;
                    }
                    CheckValidity(false);
                }
                //  The selectOneby not found, please reset the key
                if (!found)
                {
                    if (PrimairyKeyValue.HasValue && PrimairyKeyValue.Value > 0)
                    {
                        var selection = (from item in SqlParameters where item.Column == PrimairyKeyColumn select item).ToArray();
                        if (selection.Length == 1)
                        {
                            SetPropertyValue(selection[0].Info, 0, selection[0].Entity);
                        }
                    }
                }

                this.SqlLastExecutedWhereClause = dac.m_Parameterlist;
                LoggingWrite(start, sql, found);
                SetPolutionHash();
                
                
            }

            CleanUp();

            result = this;            
            //CB: Caching new instances is like stupid as hell and gives me errors when using certain lists
            // MV 2017-05-01: Caching new instances is not stupid, we cache the fact that there is no row in the database for the given key
            // Changed behavior to save empty results by default
            //if (cacheResult && !this.IsNewInstance)
            if (cacheResult)
            {
                this.Items[cacheValue] = result;
                this.SaveCache();
            }

            return result;
        }

        /// <summary>
        /// _s the select one ODBC or OLE db.
        /// </summary>
        /// <param name="whereColumns">The where columns.</param>
        /// <returns></returns>
        object _SelectOneOdbcOrOleDb(List<DatabaseDataValueColumn> whereColumns)
        {
            string selectColumns = "";
            string whereClause = "";

            using (Connection.DataCommander dac = new Connection.DataCommander(SqlConnectionString))
            {
                dac.ConnectionType = this.ConnectionType;

                SetAdditionalSqlParameters();
                SetSqlParameters();

                foreach (DatabaseColumnAttribute param in SqlParameters)
                {
                    if (!string.IsNullOrEmpty(param.ColumnSubQuery))
                        selectColumns += string.Format("({1}) as {0}, ", param.Column, param.ColumnSubQuery).ToLower();
                    else if (!string.IsNullOrEmpty(param.ColumnSubQueryPropertyReference))
                        selectColumns += string.Format("({1}) as {0}, ", param.Column, GetPropertyValue(param.ColumnSubQueryPropertyReference)).ToLower();
                    else
                        selectColumns += string.Concat(param.Column, ", ").ToLower();
                }

                //  NEW 15-05-2009
                if (this.PropertyListID.HasValue)
                {
                    Sushi.Mediakiwi.Data.Property[] properties = Sushi.Mediakiwi.Data.Property.SelectAll(this.PropertyListID.Value, this.PropertyListItemID, false, false, true, DatabaseMappingPortal);
                        //Sushi.Mediakiwi.Data.Property.SelectAll(this.PropertyListID.Value, this.PropertyListItemID, false);
                    foreach (Sushi.Mediakiwi.Data.Property p in properties)
                    {
                        if (!string.IsNullOrEmpty(p.Filter))
                        {
                            string filter = p.Filter.ToLower();
                            if (!selectColumns.Contains(string.Concat(filter, ", ")))
                                selectColumns += string.Concat(filter, ", ");
                        }
                    }
                }

                if (selectColumns.Length == 0)
                    throw new Exception("No columns set for the select statement");

                whereClause = GetWhereClause(dac, whereColumns);

                string sql = string.Format("select top 1 {0} from {1} {2} {3}{4}",
                    selectColumns.Substring(0, selectColumns.Length - 2),
                    SqlTable,
                    SqlJoin,
                    whereClause, ResultOrder
                    );

                SqlLastExecuted = sql;
                if (SqlOnlySetStatement) return null;

                dac.Text = sql;

                long start = DateTime.Now.Ticks;
                IDataReader reader = dac.ExecReader;

                bool found = false;
                while (reader.Read())
                {
                    int index = 0;
                    found = true;
                    foreach (DatabaseColumnAttribute param in SqlParameters)
                    {
                        object value = GetData(reader, index, param);
                        if (param.Info.PropertyType == typeof(Sushi.Mediakiwi.Data.CustomData) && this.PropertyListID.HasValue)
                        {
                            Sushi.Mediakiwi.Data.CustomData customData = SetPropertyValueCustomData(param.Info, value, param.Entity);
                            Sushi.Mediakiwi.Data.Property[] properties = Sushi.Mediakiwi.Data.Property.SelectAll(this.PropertyListID.Value, this.PropertyListItemID, false, false, true, DatabaseMappingPortal);
                                //Sushi.Mediakiwi.Data.Property.SelectAll(this.PropertyListID.Value, this.PropertyListItemID, false);
                            foreach (Sushi.Mediakiwi.Data.Property p in properties)
                            {
                                if (!string.IsNullOrEmpty(p.Filter))
                                {
                                    if (reader[p.Filter] == DBNull.Value && !customData[p.FieldName].IsNull)
                                        continue;

                                    customData.ApplyObject(p.FieldName, reader[p.Filter]);
                                }
                            }
                        }
                        else
                            SetPropertyValue(param.Info, value, param.Entity);
                        index++;
                    }
                    CheckValidity(false);
                }
                //  The selectOneby not found, please reset the key
                if (!found)
                {
                    if (PrimairyKeyValue.HasValue && PrimairyKeyValue.Value > 0)
                    {
                        var selection = (from item in SqlParameters where item.Column == PrimairyKeyColumn select item).ToArray();
                        if (selection.Length == 1)
                        {
                            SetPropertyValue(selection[0].Info, 0, selection[0].Entity);
                        }
                    }
                }

                this.SqlLastExecutedWhereClause = dac.m_Parameterlist;
                LoggingWrite(start, sql, found);
                SetPolutionHash();
                
                
            }

            CleanUp();
            return this;
        }


        string m_PolutionHash;
        /// <summary>
        /// Gets the polution hash.
        /// </summary>
        /// <value>The polution hash.</value>
        [Sushi.Mediakiwi.Framework.PolutionIgnore()]
        public virtual string PolutionHash
        {
            get { return m_PolutionHash; }
            set { m_PolutionHash = value; }
        }

        string m_PolutionHashPreSave;
        /// <summary>
        /// Gets the polution hash pre save.
        /// </summary>
        /// <value>The polution hash pre save.</value>
        [Sushi.Mediakiwi.Framework.PolutionIgnore()]
        public virtual string PolutionHashPreSave
        {
            get {
                SetPolutionHash(false);
                return m_PolutionHashPreSave; }
        }

        /// <summary>
        /// Determines whether [is data match] [the specified compare].
        /// </summary>
        /// <param name="compare">The compare.</param>
        /// <returns>
        /// 	<c>true</c> if [is data match] [the specified compare]; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsPoluted(iData compare)
        {
            return IsPoluted(compare, false);
        }

        /// <summary>
        /// Determines whether the specified compare is poluted.
        /// </summary>
        /// <param name="compare">The compare.</param>
        /// <param name="ignoreProperties">The ignore properties.</param>
        /// <returns>
        /// 	<c>true</c> if the specified compare is poluted; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsPoluted(iData compare, params string[] ignoreProperties)
        {
            return IsPoluted(compare, false, ignoreProperties);
        }

        /// <summary>
        /// Determines whether [is data match] [the specified compare].
        /// </summary>
        /// <param name="compare">The compare.</param>
        /// <param name="ignorePrimary">if set to <c>true</c> [ignore primary].</param>
        /// <returns>
        /// 	<c>true</c> if [is data match] [the specified compare]; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsPoluted(iData compare, bool ignorePrimary)
        {
            return IsPoluted(compare, ignorePrimary, false);
        }

        /// <summary>
        /// Determines whether the specified compare is poluted.
        /// </summary>
        /// <param name="compare">The compare.</param>
        /// <param name="ignorePrimary">if set to <c>true</c> [ignore primary].</param>
        /// <param name="ignoreProperties">The ignore properties.</param>
        /// <returns>
        /// 	<c>true</c> if the specified compare is poluted; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsPoluted(iData compare, bool ignorePrimary, params string[] ignoreProperties)
        {
            return IsPoluted(compare, ignorePrimary, false, ignoreProperties);
        }

        /// <summary>
        /// Determines whether the specified compare is poluted.
        /// </summary>
        /// <param name="compare">The compare.</param>
        /// <param name="ignorePrimary">if set to <c>true</c> [ignore primary].</param>
        /// <param name="ignoreMigration">if set to <c>true</c> [ignore migration].</param>
        /// <returns>
        /// 	<c>true</c> if the specified compare is poluted; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsPoluted(iData compare, bool ignorePrimary, bool ignoreMigration)
        {
            return IsPoluted(compare, ignorePrimary, ignoreMigration, null);
        }

        /// <summary>
        /// Determines whether [is data match] [the specified compare].
        /// </summary>
        /// <param name="compare">The compare.</param>
        /// <param name="ignorePrimary">if set to <c>true</c> [ignore primary].</param>
        /// <param name="ignoreMigration">if set to <c>true</c> [ignore migration].</param>
        /// <param name="ignoreProperties">The ignore properties.</param>
        /// <returns>
        /// 	<c>true</c> if [is data match] [the specified compare]; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsPoluted(iData compare, bool ignorePrimary, bool ignoreMigration, params string[] ignoreProperties)
        {
            if (ignorePrimary)
            {
                if (m_primary == null) SetPrimary();
                if (m_primary != null)
                {
                    this.AddPolutionIgnoreProperty(m_primary.Info.Name, true);
                    compare.AddPolutionIgnoreProperty(m_primary.Info.Name, true);
                }
            }
            if (ignoreMigration)
            {
                if (m_mgration == null) SetMigration();
                if (m_mgration != null)
                {
                    this.AddPolutionIgnoreProperty(m_mgration.Info.Name, true);
                    compare.AddPolutionIgnoreProperty(m_mgration.Info.Name, true);
                }
            }

            if (ignoreProperties != null)
            {
                foreach (string item in ignoreProperties)
                {
                    this.AddPolutionIgnoreProperty(item, true);
                    compare.AddPolutionIgnoreProperty(item, true);
                }
            }

            if (compare.PolutionHashPreSave.Equals(this.PolutionHashPreSave))
                return false;
            return true;
        }

        /// <summary>
        /// Determines whether this instance is poluted.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance is poluted; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsPoluted()
        {
            
            return this.PolutionHash != this.PolutionHashPreSave;
        }

        /// <summary>
        /// Sets the polution hash.
        /// </summary>
        void SetPolutionHash()
        {
            SetPolutionHash(true);
        }

        public virtual void AddPolutionIgnoreProperty(string propertyName)
        {
            AddPolutionIgnoreProperty(new string[] { propertyName });
        }

        /// <summary>
        /// Adds a property array to the Polution ignore exception list. 
        /// </summary>
        /// <param name="propertyNameCollection">The property name collection.</param>
        public virtual void AddPolutionIgnoreProperty(string[] propertyNameCollection)
        {
            foreach (string propertyName in propertyNameCollection)
                AddPolutionIgnoreProperty(propertyName, false);
        }

        /// <summary>
        /// Adds a property to the Polution ignore exception list.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="ignorePolutionWarning">if set to <c>true</c> [ignore polution warning].</param>
        public virtual void AddPolutionIgnoreProperty(string propertyName, bool ignorePolutionWarning)
        {
            if (!string.IsNullOrEmpty(m_PolutionHash) && !ignorePolutionWarning)
                throw new Exception("The polution hash has already been set. Please apply the Polution property before the Select method call.");

            if (this.PolutionIgnore == null)
                this.PolutionIgnore = new System.Collections.Hashtable();
            
            if (!this.PolutionIgnore.ContainsKey(propertyName))
                this.PolutionIgnore.Add(propertyName, "1");
        }

        System.Collections.Hashtable PolutionIgnore { get; set; }

        /// <summary>
        /// Sets the polution hash.
        /// </summary>
        /// <param name="resetHash">if set to <c>true</c> [reset hash].</param>
        void SetPolutionHash(bool resetHash)
        {
            if (resetHash) m_PolutionHash = null;

            string valueInfo = string.Concat("type=", this.GetType().Name);

            StringBuilder build = new StringBuilder();
            build.Append(valueInfo);

            foreach (PropertyInfo info in this.GetType().GetProperties())
            {
                
                if (info.CanRead && info.CanWrite)
                {
                    if (info.GetCustomAttributes(typeof(Sushi.Mediakiwi.Framework.PolutionIgnore), true).Length > 0)
                        continue;

                    if (this.PolutionIgnore != null && this.PolutionIgnore.ContainsKey(info.Name))
                        continue;

                    object value = info.GetValue(this, null);
                    
                    if (value != null)
                    {
                        if (value is BaseSqlEntity)
                            valueInfo = ((BaseSqlEntity)value).PolutionHash;
                        else if (value is Sushi.Mediakiwi.Data.CustomData)
                        {
                            valueInfo = ((Sushi.Mediakiwi.Data.CustomData)value).Serialized;

                            if (this.PolutionIgnore != null)
                            {
                                Sushi.Mediakiwi.Data.CustomData tmp = new CustomData();
                                tmp.ApplySerialized(valueInfo);

                                System.Collections.IDictionaryEnumerator ienum = PolutionIgnore.GetEnumerator();

                                while (ienum.MoveNext())
                                {
                                    string ignore = ienum.Key.ToString();
                                    tmp.Apply(ignore, null);
                                }
                                valueInfo = tmp.Serialized;
                            }

                        }
                        else
                            valueInfo = value.ToString();
                    }
                    else
                        valueInfo = null;

                    build.Append(string.Concat("&", info.Name, "=", valueInfo));
                    
                }
            }
            this.BUILD = build.ToString().Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty);
            this.m_PolutionHashPreSave = Wim.Utility.HashString(build.ToString());
            if (string.IsNullOrEmpty(m_PolutionHash))
            {
                m_PolutionHash = this.m_PolutionHashPreSave;
            }
        }

        [System.Xml.Serialization.XmlIgnore()]
        public virtual string Polution
        {
            get { return BUILD; }
        }
        protected string BUILD;

        void LoggingWrite(long start, string sql)
        {
            LoggingWrite(start, sql, true);
        }

        void LoggingWrite(long start, string sql, bool hasResult)
        {
            if (System.Web.HttpContext.Current == null) return;

            object count = System.Web.HttpContext.Current.Items["wim.sqlexecutionCount"];
            int counter = (count == null ? 0 : (int)count) + 1;
            System.Web.HttpContext.Current.Items["wim.sqlexecutionCount"] = counter;

            if (!Wim.CommonConfiguration.SQL_DEBUG) return;

            long end = DateTime.Now.Ticks;
            double total = new TimeSpan(end - start).TotalSeconds;
            double total2 = new TimeSpan(end - start).TotalMilliseconds;


            object pipe = System.Web.HttpContext.Current.Items["wim.sqlexecutionTime"];
            double open = pipe == null ? 0 : (double)pipe;
            double save = (total + open);


            System.Web.HttpContext.Current.Items["wim.sqlexecutionTime"] = save;

            if (System.Web.HttpContext.Current != null || System.Web.HttpContext.Current.Trace.IsEnabled)
            {
                System.Web.HttpContext.Current.Trace.Write("Sql-Execution",
                    string.Format("The following Sql execution took: {0} seconds for table {2} in total of {1}.",
                    total, save, this.SqlTable)
                    );
            }

            object planArr = System.Web.HttpContext.Current.Items["wim.sqlexecutionPlan"];
            List<SqlExecutionInformation> planList = 
                planArr == null 
                    ? new List<SqlExecutionInformation>() : (List<SqlExecutionInformation>)planArr;

            if (!hasResult)
                this.SqlLastExecutedWhereClause += " [NO RESULT FOUND]";

            string stackTrace = null;
            if (Wim.CommonConfiguration.SQL_DEBUG_STACKTRACE)
                stackTrace = System.Environment.StackTrace;

            planList.Add(new SqlExecutionInformation(SqlConnectionDatabase, this.SqlTable, sql, this.SqlLastExecutedWhereClause, total, this.GetType().ToString(), stackTrace));
            
            System.Web.HttpContext.Current.Items["wim.sqlexecutionPlan"] = planList;
            System.Web.HttpContext.Current.Trace.Write("Sql-Execution", sql);
        }

        public class SqlExecutionInformation
        {
            public SqlExecutionInformation() { }
            public SqlExecutionInformation(string sqlDatabase, string sqlTable, string sqlText, string sqlWhere, double time, string objectType) :
                this(sqlDatabase, sqlTable, sqlText, sqlWhere, time, objectType, null)
            { }
            
            public SqlExecutionInformation(string sqlDatabase, string sqlTable, string sqlText, string sqlWhere, double time, string objectType, string stackTrace) 
            {
                this.SqlDatabase = sqlDatabase;
                this.SqlTable = sqlTable;
                this.SqlText = sqlText;
                this.SqlWhere = sqlWhere;
                this.Time = time;
                this.ObjectType = objectType;
                this.StackTrace = stackTrace;
            }

            public bool IsCode { get; set; }
            public string ObjectType { get; set; }
            public string SqlText { get; set; }
            public string SqlWhere { get; set; }
            public string SqlTable { get; set; }
            public string SqlDatabase { get; set; }
            public double Time { get; set; }
            public string StackTrace { get; set; }
        }

        /// <summary>
        /// _s the exists.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <returns></returns>
        protected virtual bool Exists(Guid guid)
        {
            if (ConnectionType != DataConnectionType.SqlServer)
                return this.ExistsOdbcOrOleDb(guid);

            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            list.Add(new DatabaseDataValueColumn(MigrationKeyColumn, SqlDbType.UniqueIdentifier, guid));

            using (Connection.SqlCommander dac = new Connection.SqlCommander(SqlConnectionString))
            {
                string sql = string.Format("select count(*) from {0} {1} {2}",
                    SqlTable,
                    SqlJoin,
                    GetWhereClause(dac, list)
                    );

                SqlLastExecuted = sql;
                if (SqlOnlySetStatement) return false;

                dac.SqlText = sql;

                bool exists = false;
                long start = DateTime.Now.Ticks;
                SqlDataReader reader = dac.ExecReader;
                while (reader.Read())
                    if (reader.GetInt32(0) == 1) exists = true;
                LoggingWrite(start, sql);
                return exists;
            }
            return false;
        }

        /// <summary>
        /// Gets the key by GUID.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <returns></returns>
        protected virtual int GetKeyByGuid(Guid guid)
        {
            if (ConnectionType != DataConnectionType.SqlServer)
                return this.GetKeyByGuidOdbcOrOleDb(guid);

            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            list.Add(new DatabaseDataValueColumn(MigrationKeyColumn, SqlDbType.UniqueIdentifier, guid));
            
            using (Connection.SqlCommander dac = new Connection.SqlCommander(SqlConnectionString))
            {
                string sql = string.Format("select {3} from {0} {1} {2}",
                    SqlTable,
                    SqlJoin,
                    GetWhereClause(dac, list), PrimairyKeyColumn
                    );

                SqlLastExecuted = sql;
                dac.SqlText = sql;

                int key = 0;
                long start = DateTime.Now.Ticks;
                SqlDataReader reader = dac.ExecReader;
                
                while (reader.Read())
                    key = reader.GetInt32(0);

                LoggingWrite(start, sql);
                
                return key;
            }
            return 0;
        }

        /// <summary>
        /// Existses the ODBC or OLE db.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <returns></returns>
        bool ExistsOdbcOrOleDb(Guid guid)
        {
            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            list.Add(new DatabaseDataValueColumn(MigrationKeyColumn, SqlDbType.UniqueIdentifier, guid));

            using (Connection.DataCommander dac = new Connection.DataCommander(SqlConnectionString))
            {
                dac.ConnectionType = this.ConnectionType;
                
                string sql = string.Format("select count(*) from {0} {1} {2}",
                    SqlTable,
                    SqlJoin,
                    GetWhereClause(dac, list)
                    );

                SqlLastExecuted = sql;
                if (SqlOnlySetStatement) return false;

                dac.Text = sql;
                bool exists = false;
                long start = DateTime.Now.Ticks;
                IDataReader reader = dac.ExecReader;
                while (reader.Read())
                    if (reader.GetInt32(0) == 1) exists = true;
                LoggingWrite(start, sql);
                
                return exists;
            }
            return false;
        }

        /// <summary>
        /// Gets the key by GUID.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <returns></returns>
        int GetKeyByGuidOdbcOrOleDb(Guid guid)
        {
            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            list.Add(new DatabaseDataValueColumn(MigrationKeyColumn, SqlDbType.UniqueIdentifier, guid));

            using (Connection.DataCommander dac = new Connection.DataCommander(SqlConnectionString))
            {
                dac.ConnectionType = this.ConnectionType;

                string sql = string.Format("select {3} from {0} {1} {2}",
                    SqlTable,
                    SqlJoin,
                    GetWhereClause(dac, list), PrimairyKeyColumn
                    );

                SqlLastExecuted = sql;

                int key = 0;
                dac.Text = sql;
                long start = DateTime.Now.Ticks;

                IDataReader reader = dac.ExecReader;
                while (reader.Read())
                    key = reader.GetInt32(0);

                LoggingWrite(start, sql);
                
                return key;
            }
            return 0;
        }

        /// <summary>
        /// Select all implementations based on the specified attributed.
        /// </summary>
        /// <param name="whereColumns">The where columns.</param>
        /// <returns></returns>
        protected virtual object[] _SelectAll(List<DatabaseDataValueColumn> whereColumns)
        {
            return _SelectAll(whereColumns, false);
        }

        /// <summary>
        /// Select all implementations based on the specified attributed.
        /// </summary>
        /// <param name="storedProcedure">The stored procedure.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        protected virtual List<object> _SelectAll(string storedProcedure, SqlParameter[] parameters)
        {
            if (ConnectionType != DataConnectionType.SqlServer)
            {
                List<object> result = this._SelectAllOdbcOrOleDb(storedProcedure, parameters);
                return result;
            }

            using (Connection.SqlCommander dac = new Connection.SqlCommander(SqlConnectionString))
            {
                dac.Commandtype = CommandType.StoredProcedure;
                dac.SqlText = storedProcedure;


                foreach (SqlParameter param in parameters)
                    dac.SetParameter(param.ParameterName, param.Value, param.SqlDbType, param.Size, param.Direction);

                long start = DateTime.Now.Ticks;
                SqlDataReader reader = dac.ExecReader;
                List<object> list = new List<object>();
                while (reader.Read())
                {
                    int index = 0;

                    foreach (DatabaseColumnAttribute param in SqlParameters)
                    {
                        object value = GetData(reader, index, param);

                        if (value == DBNull.Value)
                        {
                            SetPropertyValue(param.Info, null, param.Entity);
                            continue;
                        }

                        SetPropertyValue(param.Info, value, param.Entity);
                        index++;
                    }
                    CheckValidity(false);
                    SetPolutionHash();

                    list.Add(this.MemberwiseClone());
                }
                this.SqlLastExecutedWhereClause = dac.m_Parameterlist;
                LoggingWrite(start, storedProcedure);
                return list;
            }
        }

        List<object> _SelectAllOdbcOrOleDb(string storedProcedure, SqlParameter[] parameters)
        {


            using (Connection.DataCommander dac = new Connection.DataCommander(SqlConnectionString))
            {
                dac.Commandtype = CommandType.StoredProcedure;
                dac.Text = storedProcedure;

                foreach (SqlParameter param in parameters)
                    dac.SetParameter(param.ParameterName, param.Value, param.DbType, param.Size, param.Direction);

                long start = DateTime.Now.Ticks;
                IDataReader reader = dac.ExecReader;

                List<object> list = new List<object>();

                while (reader.Read())
                {
                    int index = 0;

                    foreach (DatabaseColumnAttribute param in SqlParameters)
                    {
                        object value = GetData(reader, index, param);

                        if (value == DBNull.Value)
                        {
                            SetPropertyValue(param.Info, null, param.Entity);
                            continue;
                        }

                        SetPropertyValue(param.Info, value, param.Entity);

                        index++;
                    }
                    CheckValidity(false);
                    SetPolutionHash();
                    list.Add(this.MemberwiseClone());
                }
                this.SqlLastExecutedWhereClause = dac.m_Parameterlist;
                LoggingWrite(start, storedProcedure);

                return list;
            }
        }

        /// <summary>
        /// _s the select all.
        /// </summary>
        /// <param name="whereColumns">The where columns.</param>
        /// <param name="onlyReturnPrimaryKeyList">if set to <c>true</c> [only return primary key list].</param>
        /// <returns></returns>
        protected object[] _SelectAll(List<DatabaseDataValueColumn> whereColumns, bool onlyReturnPrimaryKeyList)
        {
            return _SelectAll(whereColumns, onlyReturnPrimaryKeyList, null, null);
        }

        /// <summary>
        /// Gets the cached array.
        /// </summary>
        /// <param name="cacheReference">The cache reference.</param>
        /// <param name="cacheValue">The cache value.</param>
        /// <returns></returns>
        protected object[] GetCachedArray(string cacheReference, string cacheValue)
        {
            string ID = string.Concat("Data.Tables.", this.SqlTable, ".", cacheReference, "$", cacheValue);

            if (m_CacheManager == null)
                m_CacheManager = new Wim.Utilities.CacheItemManager();
            
            if (m_CacheManager.IsCached(ID))
                return m_CacheManager.GetItem(ID) as object[];
            
            return null;
        }

        /// <summary>
        /// Select all implementations based on the specified attributed.
        /// </summary>
        /// <param name="whereColumns">The where columns.</param>
        /// <param name="onlyReturnPrimaryKeyList">if set to <c>true</c> [only return primary key list].</param>
        /// <param name="cacheReference">The Reference and the Value form a combined unique cachekey.</param>
        /// <param name="cacheValue">The Reference and the Value form a combined unique cachekey.</param>
        /// <returns></returns>
        protected object[] _SelectAll(List<DatabaseDataValueColumn> whereColumns, bool onlyReturnPrimaryKeyList, string cacheReference, string cacheValue)
        {
            return _SelectAll(whereColumns, onlyReturnPrimaryKeyList, cacheReference, cacheValue, null);
        }

        /// <summary>
        /// _s the search all.
        /// </summary>
        /// <param name="componentListID">The component list ID.</param>
        /// <param name="searchData">The search data.</param>
        /// <returns></returns>
        protected object[] _SearchAll(int componentListID, int? siteID, string searchData)
        {
            return _SearchAll(componentListID, siteID, searchData, null);
        }

        /// <summary>
        /// _s the search all.
        /// </summary>
        /// <param name="componentListID">The component list ID.</param>
        /// <param name="searchData">The search data.</param>
        /// <param name="whereColumns">The where columns.</param>
        /// <returns></returns>
        protected object[] _SearchAll(int componentListID, int? siteID, string searchData, List<DatabaseDataValueColumn> whereColumns)
        {
            return _SearchAll(componentListID, siteID, searchData, whereColumns, false, null, null);
        }

        /// <summary>
        /// _s the search all.
        /// </summary>
        /// <param name="componentListID">The component list ID.</param>
        /// <param name="siteID">The site ID.</param>
        /// <param name="searchData">The search data.</param>
        /// <param name="whereColumns">The where columns.</param>
        /// <param name="onlyReturnPrimaryKeyList">if set to <c>true</c> [only return primary key list].</param>
        /// <param name="cacheReference">The cache reference.</param>
        /// <param name="cacheValue">The cache value.</param>
        /// <returns></returns>
        protected object[] _SearchAll(int componentListID, int? siteID, string searchData, List<DatabaseDataValueColumn> whereColumns, bool onlyReturnPrimaryKeyList, string cacheReference, string cacheValue)
        {
            if (!String.IsNullOrEmpty(searchData))
            {
                if (string.IsNullOrEmpty(SqlJoin)) SqlJoin = "";

                //if (this.ConnectionType == DataConnectionType.InterSystemsCache)
                //{
                //    if (siteID.HasValue)
                //        SqlJoin += string.Format(@" join wim_ComponentListVersions v on ComponentListVersion_ComponentList_Key = {1} and ComponentListVersion_IsActive = 1 and ComponentListVersion_Listitem_Key = {0} and ComponentListVersion_Site_Key = {2}", this.PrimairyKeyColumn, componentListID, siteID);
                //    else
                //        SqlJoin += string.Format(@" join wim_ComponentListVersions v on ComponentListVersion_ComponentList_Key = {1} and ComponentListVersion_IsActive = 1 and ComponentListVersion_Listitem_Key = {0}", this.PrimairyKeyColumn, componentListID);

                //    whereColumns.Add(new DatabaseDataValueColumn("ComponentListVersion_XML", SqlDbType.Xml, string.Concat("%", searchData, "%"), DatabaseDataValueCompareType.Like));
                //}
                //else
                //{
                    if (siteID.HasValue)
                        SqlJoin += string.Format(@" join wim_ComponentListVersions v on ComponentListVersion_ComponentList_Key = {2} and ComponentListVersion_IsActive = 1 and ComponentListVersion_Listitem_Key = {1} and ComponentListVersion_Site_Key = {3} join FREETEXTTABLE  (wim_ComponentListVersions, ComponentListVersion_XML, '{0}') as x on v.ComponentListVersion_Key = x.[KEY]", searchData, this.PrimairyKeyColumn, componentListID, siteID);
                    else
                        SqlJoin += string.Format(@" join wim_ComponentListVersions v on ComponentListVersion_ComponentList_Key = {2} and ComponentListVersion_IsActive = 1 and ComponentListVersion_Listitem_Key = {1} join FREETEXTTABLE  (wim_ComponentListVersions, ComponentListVersion_XML, '{0}') as x on v.ComponentListVersion_Key = x.[KEY]", searchData, this.PrimairyKeyColumn, componentListID);
                //}
            }

            return _SelectAll(whereColumns, false, null, null);
        }

        List<SqlParameter> m_additionalParameterList;
        /// <summary>
        /// Adds a parameter to the Execute command or the insert/update (save) command
        /// </summary>
        /// <param name="name">The name (for execute command an variable and for insert/update an database columnname).</param>
        /// <param name="itemvalue">The itemvalue.</param>
        /// <param name="type">The type.</param>
        protected void AddSqlParameter(string name, object itemvalue, SqlDbType type)
        {
            if (m_additionalParameterList == null)
                m_additionalParameterList = new List<SqlParameter>();

            SqlParameter p = new SqlParameter();
            p.Value = itemvalue;
            p.ParameterName = name;
            p.SqlDbType = type;

            m_additionalParameterList.Add(p);
        }


        /// <summary>
        /// Select all implementations based on the specified attributed.
        /// </summary>
        /// <param name="whereColumns">The where columns.</param>
        /// <param name="onlyReturnPrimaryKeyList">if set to <c>true</c> [only return primary key list].</param>
        /// <param name="cacheReference">The Reference and the Value form a combined unique cachekey.</param>
        /// <param name="cacheValue">The Reference and the Value form a combined unique cachekey.</param>
        /// <param name="componentListID">The component list ID.</param>
        /// <returns></returns>
        protected object[] _SelectAll(List<DatabaseDataValueColumn> whereColumns, bool onlyReturnPrimaryKeyList, string cacheReference, string cacheValue, int? componentListID)
        {
            if (componentListID.HasValue)
            {
                var list2 = Sushi.Mediakiwi.Data.ComponentList.SelectOne(componentListID.Value);
                this.SqlTable = list2.Catalog().Table;
                this.SqlColumnPrefix = list2.Catalog().ColumnPrefix;
            }

            string ID = null;
            if (!string.IsNullOrEmpty(cacheReference) && !string.IsNullOrEmpty(cacheValue) && !Wim.CommonConfiguration.ForceNoCache)
            {
                ID = string.Concat(this.SqlConnectionDatabase, ":Data_", this.GetType().ToString(), ".", cacheReference, "$", cacheValue);
                if (ID != null)
                {
                    if (m_CacheManager == null)
                        m_CacheManager = new Wim.Utilities.CacheItemManager();

                    if (m_CacheManager.IsCached(ID))
                    {
                        return m_CacheManager.GetItem(ID) as object[];
                    }
                }
            }

            object[] result;
            if (ConnectionType != DataConnectionType.SqlServer)
            {
                result = this._SelectAllOdbcOrOleDb(whereColumns, onlyReturnPrimaryKeyList);
                if (ID != null)
                {
                    if (m_CacheManager == null)
                        m_CacheManager = new Wim.Utilities.CacheItemManager();

                    m_CacheManager.Add(ID, result, Wim.CommonConfiguration.DefaultCacheTimeSpan, null);
                }

                return result;
            }




            string selectColumns = "";
            string whereClause = "";

            Sushi.Mediakiwi.Data.Property[] m_propertyList = null;
            if (onlyReturnPrimaryKeyList) selectColumns += string.Concat(PrimairyKeyColumn, ", ");
            else
            {
                SetAdditionalSqlParameters();
                SetSqlParameters();


                foreach (DatabaseColumnAttribute param in SqlParameters)
                {
                    if (!string.IsNullOrEmpty(param.ColumnSubQuery))
                        selectColumns += string.Format("({1}) as {0}, ", param.Column, param.ColumnSubQuery);
                    else if (!string.IsNullOrEmpty(param.ColumnSubQueryPropertyReference))
                        selectColumns += string.Format("({1}) as {0}, ", param.Column, GetPropertyValue(param.ColumnSubQueryPropertyReference));
                    else
                        selectColumns += string.Concat(param.Column, ", ");
                }

                //  NEW 15-05-2009
                if (this.PropertyListID.HasValue)
                {
                    //Sushi.Mediakiwi.Data.Property.SqlConnectionString2 = this.SqlConnectionString;
                    m_propertyList = Sushi.Mediakiwi.Data.Property.SelectAll(this.PropertyListID.Value, this.PropertyListItemID, false, false, true, DatabaseMappingPortal);
                    foreach (Sushi.Mediakiwi.Data.Property p in m_propertyList)
                    {
                        if (!string.IsNullOrEmpty(p.Filter) && !selectColumns.Contains(string.Concat(p.Filter, ", ")))
                            selectColumns += string.Concat(p.Filter, ", ");
                    }
                }
            }
            
            using (Connection.SqlCommander dac = new Connection.SqlCommander(SqlConnectionString))
            {
                whereClause = GetWhereClause(dac, whereColumns);
                if (selectColumns.Length == 0) return null;

                string rowcount = null;
                if (m_SqlRowCount != 0) rowcount = string.Concat("set rowcount ", m_SqlRowCount, " ");

                string sqlText = string.Format("{5}select {0} from {1} {2} {3}{6}{4}",
                    selectColumns.Substring(0, selectColumns.Length - 2),
                    SqlTable,
                    SqlJoin,
                    whereClause, ResultOrder, rowcount, ResultGroup
                    );

                List<object> list = new List<object>();

                SqlLastExecuted = sqlText;
                if (SqlOnlySetStatement)
                    return list.ToArray();
                
                dac.SqlText = sqlText;

                long start = DateTime.Now.Ticks;
                SqlDataReader reader = dac.ExecReader;
                
                while (reader.Read())
                {
                    if (onlyReturnPrimaryKeyList)
                    {
                        list.Add(reader[PrimairyKeyColumn]);
                    }
                    else
                    {
                        int index = 0;
                        foreach (DatabaseColumnAttribute param in SqlParameters)
                        {
                            object value = GetData(reader, index, param);
                            if (param.Info.PropertyType == typeof(Sushi.Mediakiwi.Data.CustomData) && this.PropertyListID.HasValue)
                            {

                                Sushi.Mediakiwi.Data.CustomData customData = SetPropertyValueCustomData(param.Info, value, param.Entity);
                                //Sushi.Mediakiwi.Data.Property[] properties = Sushi.Mediakiwi.Data.Property.SelectAll(this.PropertyListID.Value, this.PropertyListItemID, false);
                                foreach (Sushi.Mediakiwi.Data.Property p in m_propertyList)
                                {
                                    if (!string.IsNullOrEmpty(p.Filter))
                                    {
                                        if (reader[p.Filter] == DBNull.Value && !customData[p.FieldName].IsNull)
                                            continue;

                                        customData.ApplyObject(p.FieldName, reader[p.Filter]);
                                    }
                                }
                            }
                            else
                            {
                                SetPropertyValue(param.Info, value, param.Entity);
                            }
                            index++;
                        }
                        CheckValidity(false);
                        SetPolutionHash();        
                        list.Add(this.MemberwiseClone());
                    }
                    
                }
                LoggingWrite(start, sqlText);
                

                result = list.ToArray();
                if (ID != null)
                {
                    if (m_CacheManager == null)
                        m_CacheManager = new Wim.Utilities.CacheItemManager();

                    m_CacheManager.Add(ID, result, Wim.CommonConfiguration.DefaultCacheTimeSpan, null);
                }
                
                return result;
            }
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="index">The index.</param>
        /// <param name="param">The param.</param>
        /// <returns></returns>
        object GetData(IDataReader reader, int index, DatabaseColumnAttribute param)
        {
            if (param.Column.Contains(" ") || param.Column.Contains(".") || param.Column.Contains(","))
                return reader[index];

            return reader[param.Column];
        }

        object[] _SelectAllOdbcOrOleDb(List<DatabaseDataValueColumn> whereColumns, bool onlyReturnPrimaryKeyList)
        {
            string selectColumns = "";
            string whereClause = "";

            if (onlyReturnPrimaryKeyList) selectColumns += string.Concat(PrimairyKeyColumn, ", ");
            else
            {
                SetAdditionalSqlParameters();
                SetSqlParameters();

                foreach (DatabaseColumnAttribute param in SqlParameters)
                {
                    if (!string.IsNullOrEmpty(param.ColumnSubQuery))
                        selectColumns += string.Format("({1}) as {0}, ", param.Column, param.ColumnSubQuery);
                    else if (!string.IsNullOrEmpty(param.ColumnSubQueryPropertyReference))
                        selectColumns += string.Format("({1}) as {0}, ", param.Column, GetPropertyValue(param.ColumnSubQueryPropertyReference));
                    else
                        selectColumns += string.Concat(param.Column, ", ");
                }

                //  NEW 15-05-2009
                if (this.PropertyListID.HasValue)
                {
                    Sushi.Mediakiwi.Data.Property[] properties = Sushi.Mediakiwi.Data.Property.SelectAll(this.PropertyListID.Value, this.PropertyListItemID, false, false, true, DatabaseMappingPortal);
                        //Sushi.Mediakiwi.Data.Property.SelectAll(this.PropertyListID.Value, this.PropertyListItemID, false);
                    foreach (Sushi.Mediakiwi.Data.Property p in properties)
                    {
                        if (!string.IsNullOrEmpty(p.Filter) && !selectColumns.Contains(string.Concat(p.Filter, ", ")))
                            selectColumns += string.Concat(p.Filter, ", ");
                    }
                }
            }

            

            using (Connection.DataCommander dac = new Connection.DataCommander(SqlConnectionString))
            {
                dac.ConnectionType = this.ConnectionType;
               
                if (selectColumns.Length == 0) return null;

                string rowcount = null;
                if (m_SqlRowCount != 0) rowcount = string.Concat(" top ", m_SqlRowCount);

                whereClause = GetWhereClause(dac, whereColumns);

                string sqlText = string.Format("select{5} {0} from {1} {2} {3}{6}{4}",
                    selectColumns.Substring(0, selectColumns.Length - 2),
                    SqlTable,
                    SqlJoin,
                    whereClause, ResultOrder, rowcount, ResultGroup
                    );

                List<object> list = new List<object>();

                SqlLastExecuted = sqlText;
                if (SqlOnlySetStatement)
                    return list.ToArray();

                dac.Text = sqlText;

                long start = DateTime.Now.Ticks;
                IDataReader reader = dac.ExecReader;

                while (reader.Read())
                {
                    if (onlyReturnPrimaryKeyList)
                    {
                        list.Add(reader[PrimairyKeyColumn]);
                    }
                    else
                    {
                        int index = 0;
                        foreach (DatabaseColumnAttribute param in SqlParameters)
                        {
                            object value = GetData(reader, index, param);

                            if (param.Info.PropertyType == typeof(Sushi.Mediakiwi.Data.CustomData) && this.PropertyListID.HasValue)
                            {
                                Sushi.Mediakiwi.Data.CustomData customData = SetPropertyValueCustomData(param.Info, value, param.Entity);
                                Sushi.Mediakiwi.Data.Property[] properties = Sushi.Mediakiwi.Data.Property.SelectAll(this.PropertyListID.Value, this.PropertyListItemID, false, false, true, DatabaseMappingPortal);

                                    //Sushi.Mediakiwi.Data.Property.SelectAll(this.PropertyListID.Value, this.PropertyListItemID, false);
                                foreach (Sushi.Mediakiwi.Data.Property p in properties)
                                {
                                    if (!string.IsNullOrEmpty(p.Filter))
                                    {
                                        if (reader[p.Filter] == DBNull.Value && !customData[p.FieldName].IsNull)
                                            continue;

                                        customData.ApplyObject(p.FieldName, reader[p.Filter]);
                                    }
                                }
                            }
                            else
                                SetPropertyValue(param.Info, value, param.Entity);

                            index++;
                        }
                        CheckValidity(false);
                        SetPolutionHash();

                        list.Add(this.MemberwiseClone());
                    }

                }
                this.SqlLastExecutedWhereClause = dac.m_Parameterlist;
                LoggingWrite(start, sqlText);
                
                return list.ToArray();
            }
        }

        //protected object _SelectAll(bool cacheResult)
        //{
        //    if (key == 0) return this;

        //    List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
        //    list.Add(new DatabaseDataValueColumn(PrimairyKeyColumn, SqlDbType.Int, key));

        //    object t;
        //    if (cacheResult)
        //        t = this._SelectOne(list, "ID", key.ToString());
        //    else
        //        t = this._SelectOne(list, null, null);

        //    return t;
        //}

        /// <summary>
        /// _s the select all.
        /// </summary>
        /// <returns></returns>
        protected object[] _SelectAll()
        {
            return _SelectAll(false);
        }

        /// <summary>
        /// Select all implementations based on the specified attributed.
        /// </summary>
        /// <returns></returns>
        protected object[] _SelectAll(bool cacheResult)
        {
            object[] t;
            if (cacheResult)
                t = this._SelectAll(null, false, "ALL", "ALL");
            else
                t = this._SelectAll(null, false, null, null);

            return t;
        }

        /// <summary>
        /// _s the select all.
        /// </summary>
        /// <param name="componentListID">The component list ID.</param>
        /// <returns></returns>
        protected object[] _SelectAll(int componentListID)
        {
            object[] t = this._SelectAll(null, false, null, null, componentListID);
            return t;
        }

        /// <summary>
        /// _s the select all ODBC or OLE db.
        /// </summary>
        /// <returns></returns>
        protected object[] _SelectAllOdbcOrOleDb()
        {
            string selectColumns = "";

            SetAdditionalSqlParameters();
            SetSqlParameters();

            foreach (DatabaseColumnAttribute param in SqlParameters)
            {
                if (!string.IsNullOrEmpty(param.ColumnSubQuery))
                    selectColumns += string.Format("({1}) as {0}, ", param.Column, param.ColumnSubQuery);
                else if (!string.IsNullOrEmpty(param.ColumnSubQueryPropertyReference))
                    selectColumns += string.Format("({1}) as {0}, ", param.Column, GetPropertyValue(param.ColumnSubQueryPropertyReference));
                else
                    selectColumns += string.Concat(param.Column, ", ");
            }
            if (selectColumns.Length == 0) return null;

            string rowcount = null;
            if (m_SqlRowCount != 0) rowcount = string.Concat(" top ", m_SqlRowCount);

            string sqlText = string.Format("select{4} {0} from {1} {2}{5}{3}",
                selectColumns.Substring(0, selectColumns.Length - 2),
                SqlTable, SqlJoin, ResultOrder, rowcount, ResultGroup
                );

            List<object> list = new List<object>();
            using (Connection.DataCommander dac = new Connection.DataCommander(SqlConnectionString))
            {
                dac.ConnectionType = this.ConnectionType;

                SqlLastExecuted = sqlText;
                if (SqlOnlySetStatement) return null;

                dac.Text = sqlText;
                long start = DateTime.Now.Ticks;
                IDataReader reader = dac.ExecReader;
                while (reader.Read())
                {
                    for (int index = 0; index < SqlParameters.Count; index++)
                    {
                        if (SqlParameters[index].Info.PropertyType == typeof(Sushi.Mediakiwi.Data.CustomData) && this.PropertyListID.HasValue)
                        {
                            Sushi.Mediakiwi.Data.CustomData customData = SetPropertyValueCustomData(SqlParameters[index].Info, reader[index], SqlParameters[index].Entity);
                            Sushi.Mediakiwi.Data.Property[] properties = Sushi.Mediakiwi.Data.Property.SelectAll(this.PropertyListID.Value, this.PropertyListItemID, false, false, true, DatabaseMappingPortal);
                                //Sushi.Mediakiwi.Data.Property.SelectAll(this.PropertyListID.Value, this.PropertyListItemID, false);
                            foreach (Sushi.Mediakiwi.Data.Property p in properties)
                            {
                                if (!string.IsNullOrEmpty(p.Filter))
                                {
                                    if (reader[p.Filter] == DBNull.Value && !customData[p.FieldName].IsNull)
                                        continue;

                                    customData.ApplyObject(p.FieldName, reader[p.Filter]);
                                }
                            }
                        }
                        else
                            SetPropertyValue(SqlParameters[index].Info, reader[index], SqlParameters[index].Entity);

                        //if (reader[index] == DBNull.Value)
                        //{
                        //    SetPropertyValue(SqlParameters[index].Info, null, SqlParameters[index].Entity);
                        //    continue;
                        //}

                        //SetPropertyValue(SqlParameters[index].Info, reader[index], SqlParameters[index].Entity);
                    }

                    CheckValidity(false);
                    SetPolutionHash();

                    list.Add(this.MemberwiseClone());
                }
                this.SqlLastExecutedWhereClause = dac.m_Parameterlist;
                LoggingWrite(start, sqlText);
                
            }
            return list.ToArray();
        }

        /// <summary>
        /// Selects the name value collection.
        /// </summary>
        /// <param name="nameColumn">The name column.</param>
        /// <returns></returns>
        protected NameValue[] SelectNameValueCollection(string nameColumn)
        {
            return SelectNameValueCollection(nameColumn, null);
        }

        /// <summary>
        /// Selects the name value collection.
        /// </summary>
        /// <param name="nameColumn">The name column.</param>
        /// <param name="valueColumn">The value column.</param>
        /// <returns></returns>
        protected NameValue[] SelectNameValueCollection(string nameColumn, string valueColumn)
        {
            return SelectNameValueCollection(nameColumn, valueColumn, null);
        }

        /// <summary>
        /// Selects the name value collection.
        /// </summary>
        /// <param name="nameColumn">The name column.</param>
        /// <param name="valueColumn">The value column.</param>
        /// <param name="whereColumns">The where columns.</param>
        /// <returns></returns>
        protected NameValue[] SelectNameValueCollection(string nameColumn, string valueColumn, List<DatabaseDataValueColumn> whereColumns)
        {
            return SelectNameValueCollection(nameColumn, valueColumn, whereColumns, true);
        }

        /// <summary>
        /// Selects the name value collection.
        /// </summary>
        /// <param name="nameColumn">The name column.</param>
        /// <param name="valueColumn">The value column.</param>
        /// <param name="whereColumns">The where columns.</param>
        /// <returns></returns>
        protected NameValue[] SelectNameValueCollection(string nameColumn, string valueColumn, List<DatabaseDataValueColumn> whereColumns, bool orderAscending)
        {
            if (this.ConnectionType == DataConnectionType.SqlServer)
            {
                using (Connection.SqlCommander dac = new Connection.SqlCommander(SqlConnectionString))
                {

                    string whereClause = GetWhereClause(dac, whereColumns);

                    string rowcount = null;
                    if (m_SqlRowCount != 0) rowcount = string.Concat("set rowcount ", m_SqlRowCount, " ");

                    string sqlText = string.Format("{4}select distinct {0} from {1} {2} {3}{5}",
                        valueColumn == null ? nameColumn : string.Concat(nameColumn, ",", valueColumn),
                        SqlTable,
                        SqlJoin,
                        whereClause, rowcount, ResultGroup
                        );

                    dac.SqlText = sqlText;
                    IDataReader reader = dac.ExecReader;
                    List<NameValue> list = new List<NameValue>();

                    int cols = 0;
                    while (reader.Read())
                    {
                        if (cols == 0)
                            cols = reader.FieldCount;
                        if (cols == 0)
                            throw new Exception("No columns added to the query: SelectNameValueCollection");

                        if (cols == 1)
                            list.Add(new NameValue(reader[0], reader[0]));
                        else
                            list.Add(new NameValue(reader[0], reader[1]));

                    }

                    if (orderAscending)
                        return (from item in list orderby item.Name ascending select item).ToArray();
                    return (from item in list orderby item.Name descending select item).ToArray();
                }
            }
            else
            {
                using (Connection.DataCommander dac = new Connection.DataCommander(SqlConnectionString))
                {
                    dac.ConnectionType = this.ConnectionType;

                    string whereClause = GetWhereClause(dac, whereColumns);

                    string rowcount = null;
                    if (m_SqlRowCount != 0) rowcount = string.Concat("set rowcount ", m_SqlRowCount, " ");

                    string sqlText = string.Format("{4}select {0} from {1} {2} {3}{5}",
                        string.Concat(nameColumn, ",", valueColumn),
                        SqlTable,
                        SqlJoin,
                        whereClause, rowcount, ResultGroup
                        );

                    dac.Text = sqlText;
                    IDataReader reader = dac.ExecReader;
                    List<NameValue> list = new List<NameValue>();

                    int cols = 0;
                    while (reader.Read())
                    {
                        if (cols == 0)
                            cols = reader.FieldCount;
                        if (cols == 0)
                            throw new Exception("No columns added to the query: SelectNameValueCollection");

                        if (cols == 1)
                            list.Add(new NameValue(reader[0], reader[0]));
                        else
                            list.Add(new NameValue(reader[0], reader[1]));

                    }
                    if (orderAscending)
                        return (from item in list orderby item.Name ascending select item).ToArray();
                    return (from item in list orderby item.Name descending select item).ToArray();
                }
            }
        }

        /// <summary>
        /// Execute a custom SQL statement
        /// </summary>
        /// <param name="sqlText">The SQL text.</param>
        /// <returns></returns>
        public virtual object Execute(string sqlText)
        {
            object result;
            if (ConnectionType != DataConnectionType.SqlServer)
            {
                result = this.ExecuteOdbcOrOleDb(sqlText);
                return result;
            }

            using (Connection.SqlCommander dac = new Connection.SqlCommander(SqlConnectionString))
            {
                if (m_additionalParameterList != null)
                {
                    foreach (SqlParameter p in m_additionalParameterList)
                        dac.SetParameter(p);
                }

                dac.SqlText = sqlText;

                long start = DateTime.Now.Ticks;
                result = dac.ExecScalar();
                LoggingWrite(start, sqlText);
                
                return result;
            }
        }

        object ExecuteOdbcOrOleDb(string sqlText)
        {
            using (Connection.DataCommander dac = new Connection.DataCommander(SqlConnectionString))
            {
                dac.ConnectionType = this.ConnectionType;

                dac.Text = sqlText;
                long start = DateTime.Now.Ticks;
                object result = dac.ExecScalar();
                LoggingWrite(start, sqlText);
                
                return result;
                
            }
        }

        //OdbcType GetOdbcMapping(SqlDbType type)
        //{
        //    switch (type)
        //    {
        //        case SqlDbType.Int: return OdbcType.Int;
        //        case SqlDbType.TinyInt: return OdbcType.TinyInt;
        //        case SqlDbType.SmallInt: return OdbcType.SmallInt;
        //        case SqlDbType.Bit: return OdbcType.Bit;
        //        case SqlDbType.DateTime: return OdbcType.DateTime;
        //        case SqlDbType.Decimal: return OdbcType.Decimal;
        //        case SqlDbType.Money: return OdbcType.Decimal;
        //        case SqlDbType.NChar: return OdbcType.NChar;
        //        case SqlDbType.VarChar: return OdbcType.VarChar;
        //        case SqlDbType.NText: return OdbcType.NText;
        //        case SqlDbType.NVarChar: return OdbcType.NVarChar;
        //        case SqlDbType.UniqueIdentifier: return OdbcType.VarChar;
        //        case SqlDbType.Xml: return OdbcType.NText;
        //    }
        //    throw new Exception(string.Concat("Could not find a suitable SQL to ODBC mapping for ", type.ToString(), "!"));
        //}

        /// <summary>
        /// Inserts this instance.
        /// </summary>
        /// <returns></returns>
        public virtual bool Insert()
        {
            return Insert(false);
        }

        /// <summary>
        /// Insert an implementation record.
        /// </summary>
        /// <returns></returns>
        public virtual bool Insert(bool identityInsert)
        {
            if (ConnectionType != DataConnectionType.SqlServer)
                return this.InsertOdbcOrOleDb();

            m_SqlParameters = null;
            string insertColumns = " ";
            string valuesColumns = "";
            string primaryParameter = "";
            string returnCall = null;
            DatabaseColumnAttribute primaryDataColumn = null;

            using (Connection.SqlCommander dac = new Connection.SqlCommander(SqlConnectionString))
            {
                DatabaseColumnAttribute customDataParam = null;

                foreach (DatabaseColumnAttribute param in SqlParameters)
                {
                    if (!identityInsert && param.IsPrimaryKey && param.Column != null)
                    {
                        primaryDataColumn = param;
                        returnCall = string.Format("set @{0} = @@IDENTITY", param.Column);
                        primaryParameter = string.Concat("@", param.Column);
                        dac.SetParameterOutput(primaryParameter, param.SqlType, param.Length);
                    }
                    else
                    {
                        if (param.IsOnlyRead) continue;
                        if (string.IsNullOrEmpty(param.ColumnSubQuery) && string.IsNullOrEmpty(param.ColumnSubQueryPropertyReference))
                        {
                            //  Double check
                            if (insertColumns.Contains(string.Concat(" ", param.Column.ToLower(), ", ")))
                                continue;

                            if (param.Info.PropertyType == typeof(Sushi.Mediakiwi.Data.CustomData) && customDataParam == null)
                            {
                                customDataParam = param;
                                continue;
                            }

                            insertColumns += string.Concat(param.Column, ", ").ToLower();
                            valuesColumns += string.Concat("@", param.Column, ", ");
                            dac.SetParameterInput(string.Concat("@", param.Column), GetPropertyValue(param.Info), param.SqlType, param.Length);
                        }
                    }
                }

                if (m_additionalParameterList != null)
                {
                    foreach (SqlParameter p in m_additionalParameterList)
                    {
                        insertColumns += string.Concat(p.ParameterName, ", ").ToLower();
                        valuesColumns += string.Concat("@", p.ParameterName, ", ");
                        dac.SetParameterInput(string.Concat("@", p.ParameterName), p.Value, p.SqlDbType);
                    }
                }

                if (!string.IsNullOrEmpty(insertColumns))
                    insertColumns = insertColumns.ToLower();

                //  Perform custom data entry last!
                if (customDataParam != null)
                {
                    Sushi.Mediakiwi.Data.CustomData data = customDataParam.Info.GetValue(this, null) as Sushi.Mediakiwi.Data.CustomData;
                    if (data != null)
                    {
                        //Sushi.Mediakiwi.Data.CustomData clone = data.Clone();
                        if (this.PropertyListID.HasValue)
                        {
                            var list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(this.PropertyListID.Value);
                            Sushi.Mediakiwi.Data.Property[] properties = Sushi.Mediakiwi.Data.Property.SelectAll(this.PropertyListID.Value, this.PropertyListItemID, false, false, true, DatabaseMappingPortal);
                                //Sushi.Mediakiwi.Data.Property.SelectAll(this.PropertyListID.Value, this.PropertyListItemID, false);
                            foreach (Sushi.Mediakiwi.Data.Property prop in properties)
                            {
                                if (!string.IsNullOrEmpty(prop.Filter))
                                {
                                    //  Double check
                                    if (insertColumns.Contains(string.Concat(" ", prop.Filter.ToLower(), ", ")))
                                        continue;

                                    insertColumns += string.Concat(prop.Filter, ", ").ToLower();
                                    valuesColumns += string.Concat("@P", prop.ID, ", ");

                                    Sushi.Mediakiwi.Data.CustomDataItem item = data[prop.FieldName];

                                    System.Type type = System.Type.GetType(prop.FilterType);
                                    object value = item.ParseSqlParameterValue(type);
                                    dac.SetParameterInput(string.Concat("@P", prop.ID), value, item.ParseSqlParameterType(type));
                                    
                                    //data.Apply(prop.FieldName, null);
                                }
                                //else
                                //{
                                //    if (data[prop.FieldName].IsNull)
                                //        data.Apply(prop.FieldName, null);
                                //    else if (data[prop.FieldName].ParseInt().GetValueOrDefault() == 0)
                                //    {
                                //        if (
                                //            prop.TypeID == (int)Sushi.Mediakiwi.Framework.ContentType.Binary_Document
                                //            || prop.TypeID == (int)Sushi.Mediakiwi.Framework.ContentType.Binary_Image
                                //            || prop.TypeID == (int)Sushi.Mediakiwi.Framework.ContentType.Choice_Checkbox
                                //            || prop.TypeID == (int)Sushi.Mediakiwi.Framework.ContentType.FolderSelect
                                //            || prop.TypeID == (int)Sushi.Mediakiwi.Framework.ContentType.PageSelect
                                //            )
                                //            data.Apply(prop.FieldName, null);
                                //    }
                                //}
                            }
                        }

                        insertColumns += string.Concat(customDataParam.Column, ", ").ToLower();
                        valuesColumns += string.Concat("@", customDataParam.Column, ", ");
                        dac.SetParameterInput(string.Concat("@", customDataParam.Column), data.Serialized, customDataParam.SqlType, customDataParam.Length);

                    }

                }

                string sqlText;

                if (insertColumns.Length == 0)
                {
                    sqlText = string.Format("insert into {0} DEFAULT VALUES", this.SqlTableChecked);
                }
                else
                {
                    sqlText = string.Format("insert into {0} ({1}) values ({2}) {3}",
                        this.SqlTableChecked,
                        insertColumns.Substring(0, insertColumns.Length - 2),
                        valuesColumns.Substring(0, valuesColumns.Length - 2),
                        returnCall);
                }
                SqlLastExecuted = dac.ApplyParameters(sqlText.Replace(returnCall, string.Empty));
                
                if (SqlOnlySetStatement) return true;

                dac.SqlText = sqlText;

                CheckValidity(true);

                long start = DateTime.Now.Ticks;
                dac.ExecNonQuery();
                LoggingWrite(start, sqlText);
                if (returnCall != null)
                    primaryDataColumn.Info.SetValue(this, dac.GetParamInt(primaryParameter), null);

                return true;
            }
        }

        /// <summary>
        /// Inserts the ODBC or OLEDB
        /// </summary>
        /// <returns></returns>
        bool InsertOdbcOrOleDb()
        {
            m_SqlParameters = null;

            string insertColumns = " ";
            string valuesColumns = "";
            string returnCall = null;
            DatabaseColumnAttribute primaryDataColumn = null;

            using (Connection.DataCommander dac = new Connection.DataCommander(SqlConnectionString))
            {
                dac.ConnectionType = this.ConnectionType;

                DatabaseColumnAttribute customDataParam = null;

                foreach (DatabaseColumnAttribute param in SqlParameters)
                {
                    if (param.IsPrimaryKey && param.Column != null)
                    {
                        primaryDataColumn = param;
                        returnCall = string.Concat("SELECT LAST_IDENTITY() FROM ", this.SqlTable);
                    }
                    else
                    {
                        if (param.IsOnlyRead) continue;
                        if (string.IsNullOrEmpty(param.ColumnSubQuery) && string.IsNullOrEmpty(param.ColumnSubQueryPropertyReference))
                        {
                            //  Double check
                            if (insertColumns.Contains(string.Concat(" ", param.Column.ToLower(), ", ")))
                                continue;

                            insertColumns += string.Concat(param.Column, ", ");
                            valuesColumns += "?, ";
                            dac.SetParameterInput(param.Column, GetPropertyValue(param.Info), param.DbType, param.Length);


                            //  [20090128: Marc Molenwijk]
                            #region Custom colums
                            if (this.PropertyListID.HasValue && param.Info.PropertyType == typeof(Sushi.Mediakiwi.Data.CustomData) && customDataParam == null)
                            {
                                customDataParam = param;
                            }
                            #endregion
                        }
                    }

                }

                if (m_additionalParameterList != null)
                {
                    foreach (SqlParameter p in m_additionalParameterList)
                    {
                        insertColumns += string.Concat(p.ParameterName, ", ");
                        valuesColumns += "?, ";
                        dac.SetParameterInput(string.Concat("@", p.ParameterName), p.Value, p.SqlDbType);
                    }
                }

                if (!string.IsNullOrEmpty(insertColumns))
                    insertColumns = insertColumns.ToLower();

                if (customDataParam != null)
                {
                    Sushi.Mediakiwi.Data.CustomData data = customDataParam.Info.GetValue(this, null) as Sushi.Mediakiwi.Data.CustomData;
                    if (data != null)
                    {
                        var list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(this.PropertyListID.Value);
                        Sushi.Mediakiwi.Data.Property[] properties = Sushi.Mediakiwi.Data.Property.SelectAll(this.PropertyListID.Value, this.PropertyListItemID, false, false, true, DatabaseMappingPortal);
                            //Sushi.Mediakiwi.Data.Property.SelectAll(this.PropertyListID.Value, this.PropertyListItemID, false);
                        foreach (Sushi.Mediakiwi.Data.Property prop in properties)
                        {
                            if (!string.IsNullOrEmpty(prop.Filter))
                            {
                                //  Double check
                                if (insertColumns.Contains(string.Concat(" ", prop.Filter.ToLower(), ", ")))
                                    continue;

                                insertColumns += string.Concat(prop.Filter, ", ");
                                valuesColumns += "?, ";

                                Sushi.Mediakiwi.Data.CustomDataItem item = data[prop.FieldName];
                                System.Type type = System.Type.GetType(prop.FilterType);
                                dac.SetParameterInput(string.Concat("@P", prop.ID), item.ParseSqlParameterValue(type), item.ParseSqlParameterType(type));
                            }
                        }
                    }
                }

                string sqlText;

                if (insertColumns.Length == 0)
                {
                    sqlText = string.Format("insert into {0} DEFAULT VALUES", this.SqlTableChecked);
                }
                else
                {
                    sqlText = string.Format("insert into {0} ({1}) values ({2})",
                        this.SqlTableChecked,
                        insertColumns.Substring(0, insertColumns.Length - 2),
                        valuesColumns.Substring(0, valuesColumns.Length - 2));
                }
                SqlLastExecuted = sqlText;
                if (SqlOnlySetStatement) return true;

                dac.Text = sqlText;

                CheckValidity(true);

                if (returnCall != null)
                {
                    dac.IsPartOfTransaction = true;

                    long start = DateTime.Now.Ticks;
                    dac.ExecNonQuery();
                    LoggingWrite(start, sqlText);
                    dac.ClearParameters();
                    dac.Text = returnCall;
                    primaryDataColumn.Info.SetValue(this, Wim.Utility.ConvertToInt(dac.ExecScalar()), null);
                    
                }
                else
                {
                    long start = DateTime.Now.Ticks;
                    dac.ExecNonQuery();
                    LoggingWrite(start, sqlText);
                    
                }

                return true;
            }
        }
        /// <summary>
        /// Gets the where clause.
        /// </summary>
        /// <param name="dac">The dac.</param>
        /// <param name="whereColumns">The where columns.</param>
        /// <returns></returns>
        string GetWhereClause(Connection.SqlCommander dac, List<DatabaseDataValueColumn> whereColumns)
        {
            return GetWhereClause(dac, whereColumns, true);
        }

        /// <summary>
        /// Gets the where clause.
        /// </summary>
        /// <param name="dac">The dac.</param>
        /// <param name="whereColumns">The where columns.</param>
        /// <param name="isSelect">if set to <c>true</c> [is select].</param>
        /// <returns></returns>
        string GetWhereClause(Connection.SqlCommander dac, List<DatabaseDataValueColumn> whereColumns, bool isSelect)
        {
            //  [20110725:MM] Validate NULL values
            if (whereColumns != null)
            {
                var count = (from item in whereColumns where item == null select item);
                if (count.Count() > 0)
                    whereColumns = (from item in whereColumns where item != null select item).ToList();
            }
            //  [20110725:MM:END]

            if (m_additionalParameterList != null)
            {
                foreach (SqlParameter p in m_additionalParameterList)
                    dac.SetParameterInput(p.ParameterName, p.Value, p.SqlDbType);
            }

            if (IsGenericEntity && isSelect && this.PropertyListID > 0)
            {
                if (whereColumns == null) whereColumns = new List<DatabaseDataValueColumn>();
                whereColumns.Add(new DatabaseDataValueColumn("<SQL_COL>_List_Key", SqlDbType.Int, PropertyListID));
            }

            if (whereColumns == null || whereColumns.Count == 0) return null;
            string whereClause = "where ";

            int index = 0;

            bool orGroupIsSet = false;

            foreach (DatabaseDataValueColumn column in whereColumns)
            {
                string param = string.Concat("@C", index);

                if (!string.IsNullOrEmpty(column.Column))
                {
                    if (!string.IsNullOrEmpty(this.SqlColumnPrefix) && column.Column.Contains("<SQL_COL>"))
                        column.Column = column.Column.Replace("<SQL_COL>", this.SqlColumnPrefix);
                }
                else
                {
                    if (!string.IsNullOrEmpty(this.SqlColumnPrefix) && column.SqlText.Contains("<SQL_COL>"))
                        column.SqlText = column.SqlText.Replace("<SQL_COL>", this.SqlColumnPrefix);
                }

                //  If a NEXT column exists please validate its connecttype.
                DatabaseDataValueColumn nextcolumn = null;

                while (nextcolumn == null)
                {
                    if (whereColumns.Count > index + 1)
                    {
                        nextcolumn = whereColumns[index + 1];

                        if (column.ConnectType == DatabaseDataValueConnectType.And && nextcolumn.ConnectType == DatabaseDataValueConnectType.Or)
                        {
                            orGroupIsSet = true;
                            whereClause += "(";
                        }
                    }
                    else
                        break;
                }

                if (column.CompareType == DatabaseDataValueCompareType.Default)
                {
                    if (!string.IsNullOrEmpty(column.SqlText))
                    {
                        whereClause += column.SqlText;
                    }
                    else if (column.DbColValue == null)
                    {
                        whereClause += string.Concat(column.Column, " IS NULL");
                    }
                    else
                    {
                        whereClause += string.Concat(column.Column, " = ", param);
                        dac.SetParameterInput(param, column.DbColValue, column.SqlType, column.Length);
                    }
                }
                else if (column.CompareType == DatabaseDataValueCompareType.Like)
                {
                    whereClause += string.Concat(column.Column, " like ", param);
                    dac.SetParameterInput(param, column.DbColValue, column.SqlType, column.Length);
                }
                else if (column.CompareType == DatabaseDataValueCompareType.In)
                {
                    whereClause += string.Format("{0} in ({1})", column.Column, column.DbColValue);
                }
                else if (column.CompareType == DatabaseDataValueCompareType.BiggerThen)
                {
                    whereClause += string.Concat(column.Column, " > ", param);
                    dac.SetParameterInput(param, column.DbColValue, column.SqlType, column.Length);
                }
                else if (column.CompareType == DatabaseDataValueCompareType.BiggerThenOrEquals)
                {
                    whereClause += string.Concat(column.Column, " >= ", param);
                    dac.SetParameterInput(param, column.DbColValue, column.SqlType, column.Length);
                }
                else if (column.CompareType == DatabaseDataValueCompareType.SmallerThen)
                {
                    whereClause += string.Concat(column.Column, " < ", param);
                    dac.SetParameterInput(param, column.DbColValue, column.SqlType, column.Length);
                }
                else if (column.CompareType == DatabaseDataValueCompareType.SmallerThenOrEquals)
                {
                    whereClause += string.Concat(column.Column, " <= ", param);
                    dac.SetParameterInput(param, column.DbColValue, column.SqlType, column.Length);
                }
                else if (column.CompareType == DatabaseDataValueCompareType.OrIn)
                {
                    string[] valueArr = column.DbColValue as string[];
                    if (valueArr != null && valueArr.Length > 0)
                    {
                        string tmp = "";
                        foreach (string valueItm in valueArr)
                        {
                            if (tmp.Length > 0) tmp += " or ";

                            if (string.Equals(valueItm, "null", StringComparison.OrdinalIgnoreCase))
                                tmp += string.Concat(column.Column, " is null");
                            else
                                tmp += string.Format("{0} in ({1})", column.Column, valueItm);
                        }
                        if (tmp.Length > 0) whereClause += string.Format(" ({0}) ", tmp);
                    }
                }

                //  If a NEXT column exists please validate its connecttype.
                if (nextcolumn != null)
                {
                    if (nextcolumn.ConnectType == DatabaseDataValueConnectType.And)
                    {
                        if (orGroupIsSet)
                        {
                            orGroupIsSet = false;
                            whereClause += ") and ";
                        }
                        else
                            whereClause += " and ";
                    }
                    else if (nextcolumn.ConnectType == DatabaseDataValueConnectType.Or || nextcolumn.ConnectType == DatabaseDataValueConnectType.OrUngrouped)
                    {
                        whereClause += " or ";
                    }
                }
                index++;
            }
            
            if (orGroupIsSet) whereClause += ")";
            
            SqlLastExecutedWhereClause = whereClause;
            return whereClause;
        }

        // [MR:23-01-2020] Commented, will not be used ever EVER again
        ///// <summary>
        ///// Gets the filter where clause.
        ///// </summary>
        ///// <param name="dac">The dac.</param>
        ///// <param name="filterColumns">The filter columns.</param>
        ///// <returns></returns>
        //string GetFilterWhereClause(Connection.SqlCommander dac, List<DatabaseDataValueColumn> filterColumns)
        //{
        //    if (filterColumns == null || filterColumns.Count == 0) return null;
        //    string whereClause = "";

        //    int index = 0;

        //    bool orGroupIsSet = false;
        //    bool isSingleFilter = filterColumns.Count == 1;

        //    foreach (DatabaseDataValueColumn column in filterColumns)
        //    {
        //        string param = string.Concat("@F", index);

        //        //  If a NEXT column exists please validate its connecttype.
        //        DatabaseDataValueColumn nextcolumn = null;

        //        string type = GetType(column.SqlType);
        //        if (type == null)
        //            continue;
                   
        //        if (!this.PropertyListID.HasValue)
        //            throw new Exception("The PropertyListID Property is not set; this is required for Filter: tags.");

        //        Sushi.Mediakiwi.Data.Property p = Sushi.Mediakiwi.Data.Property.SelectOne(this.PropertyListID.Value, column.Column, null);

        //        column.Column = string.Concat("DataFilter_", type);

        //        #region Next Column designation
        //        while (nextcolumn == null)
        //        {
        //            if (filterColumns.Count > index + 1)
        //            {
        //                nextcolumn = filterColumns[index + 1];
        //                if (column.ConnectType == DatabaseDataValueConnectType.And && nextcolumn.ConnectType == DatabaseDataValueConnectType.Or)
        //                {
        //                    orGroupIsSet = true;
        //                    whereClause += "(";
        //                }
        //            }
        //            else
        //                break;
        //        }
        //        #endregion

        //        if (!isSingleFilter)
        //            whereClause += "DataFilter_Item_Key in (select DataFilter_Item_Key from wim_DataFilters where ";

        //        if (column.CompareType == DatabaseDataValueCompareType.Default)
        //        {
        //            if (!string.IsNullOrEmpty(column.SqlText))
        //            {
        //                whereClause += column.SqlText;
        //            }
        //            else if (column.DbColValue == null)
        //            {
        //                whereClause += string.Concat(column.Column, " IS NULL");
        //            }
        //            else
        //            {
        //                whereClause += string.Concat(column.Column, " = ", param);
        //                dac.SetParameterInput(param, column.DbColValue, column.SqlType, column.Length);
        //            }
        //        }
        //        else if (column.CompareType == DatabaseDataValueCompareType.Like)
        //        {
        //            whereClause += string.Concat(column.Column, " like ", param);
        //            dac.SetParameterInput(param, column.DbColValue, column.SqlType, column.Length);
        //        }
        //        else if (column.CompareType == DatabaseDataValueCompareType.In)
        //        {
        //            whereClause += string.Format("{0} in ({1})", column.Column, column.DbColValue);
        //        }
        //        else if (column.CompareType == DatabaseDataValueCompareType.BiggerThen)
        //        {
        //            whereClause += string.Concat(column.Column, " > ", param);
        //            dac.SetParameterInput(param, column.DbColValue, column.SqlType, column.Length);
        //        }
        //        else if (column.CompareType == DatabaseDataValueCompareType.BiggerThenOrEquals)
        //        {
        //            whereClause += string.Concat(column.Column, " >= ", param);
        //            dac.SetParameterInput(param, column.DbColValue, column.SqlType, column.Length);
        //        }
        //        else if (column.CompareType == DatabaseDataValueCompareType.SmallerThen)
        //        {
        //            whereClause += string.Concat(column.Column, " < ", param);
        //            dac.SetParameterInput(param, column.DbColValue, column.SqlType, column.Length);
        //        }
        //        else if (column.CompareType == DatabaseDataValueCompareType.SmallerThenOrEquals)
        //        {
        //            whereClause += string.Concat(column.Column, " =< ", param);
        //            dac.SetParameterInput(param, column.DbColValue, column.SqlType, column.Length);
        //        }
        //        else if (column.CompareType == DatabaseDataValueCompareType.OrIn)
        //        {
        //            string[] valueArr = column.DbColValue as string[];
        //            if (valueArr != null && valueArr.Length > 0)
        //            {
        //                string tmp = "";
        //                foreach (string valueItm in valueArr)
        //                {
        //                    if (tmp.Length > 0) tmp += " or ";

        //                    if (string.Equals(valueItm, "null", StringComparison.OrdinalIgnoreCase))
        //                        tmp += string.Concat(column.Column, " is null");
        //                    else
        //                        tmp += string.Format("{0} in ({1})", column.Column, valueItm);
        //                }
        //                if (tmp.Length > 0) whereClause += string.Format(" ({0}) ", tmp);
        //            }
        //        }

        //        if (isSingleFilter)
        //        {
        //            if (string.IsNullOrEmpty(this.SqlJoin))
        //                this.SqlJoin = string.Format("join wim_DataFilters on DataFilter_Item_Key = {0}_Key and DataFilter_Property_Key = {1} and {2}", this.SqlColumnPrefix, p.ID, whereClause);
        //            else
        //                this.SqlJoin += string.Format(" join wim_DataFilters on DataFilter_Item_Key = {0}_Key and DataFilter_Property_Key = {1} and {2}", this.SqlColumnPrefix, p.ID, whereClause);
        //        }
        //        else
        //        {
        //            whereClause += ") ";

        //            //  If a NEXT column exists please validate its connecttype.
        //            if (nextcolumn != null)
        //            {
        //                if (nextcolumn.ConnectType == DatabaseDataValueConnectType.And)
        //                {
        //                    if (orGroupIsSet)
        //                    {
        //                        orGroupIsSet = false;
        //                        whereClause += ") and ";
        //                    }
        //                    else
        //                        whereClause += " and ";
        //                }
        //                else if (nextcolumn.ConnectType == DatabaseDataValueConnectType.Or || nextcolumn.ConnectType == DatabaseDataValueConnectType.OrUngrouped)
        //                {
        //                    whereClause += " or ";
        //                }
        //            }
        //        }
        //        index++;
        //    }

        //    if (orGroupIsSet) whereClause += ")";

        //    if (isSingleFilter)
        //        return null;

        //    return string.Concat("select distinct DataFilter_Item_Key from wim_DataFilters where ", whereClause);
        //}

        ///// <summary>
        ///// Gets the filter where clause.
        ///// </summary>
        ///// <param name="dac">The dac.</param>
        ///// <param name="filterColumns">The filter columns.</param>
        ///// <returns></returns>
        //string GetFilterWhereClause(Connection.DataCommander dac, List<DatabaseDataValueColumn> filterColumns)
        //{



        //    if (filterColumns == null || filterColumns.Count == 0) return null;
        //    string whereClause = "";

        //    int index = 0;

        //    bool orGroupIsSet = false;
        //    bool isSingleFilter = filterColumns.Count == 1;

        //    foreach (DatabaseDataValueColumn column in filterColumns)
        //    {
        //        string param = string.Concat("@F", index);

        //        //  If a NEXT column exists please validate its connecttype.
        //        DatabaseDataValueColumn nextcolumn = null;

        //        string type = GetType(column.SqlType);
        //        if (type == null)
        //            continue;

        //        if (!this.PropertyListID.HasValue)
        //            throw new Exception("The PropertyListID Property is not set; this is required for Filter: tags.");

        //        Sushi.Mediakiwi.Data.Property p = Sushi.Mediakiwi.Data.Property.SelectOne(this.PropertyListID.Value, column.Column, null);

        //        column.Column = string.Concat("DataFilter_", type);

        //        #region Next Column designation
        //        while (nextcolumn == null)
        //        {
        //            if (filterColumns.Count > index + 1)
        //            {
        //                nextcolumn = filterColumns[index + 1];
        //                if (column.ConnectType == DatabaseDataValueConnectType.And && nextcolumn.ConnectType == DatabaseDataValueConnectType.Or)
        //                {
        //                    orGroupIsSet = true;
        //                    whereClause += "(";
        //                }
        //            }
        //            else
        //                break;
        //        }
        //        #endregion

        //        if (!isSingleFilter)
        //            whereClause += "DataFilter_Item_Key in (select DataFilter_Item_Key from wim_DataFilters where ";

        //        if (column.CompareType == DatabaseDataValueCompareType.Default)
        //        {
        //            if (!string.IsNullOrEmpty(column.SqlText))
        //            {
        //                whereClause += column.SqlText;
        //            }
        //            else if (column.DbColValue == null)
        //            {
        //                whereClause += string.Concat(column.Column, " IS NULL");
        //            }
        //            else
        //            {
        //                whereClause += string.Concat(column.Column, " = ", param);
        //                dac.SetParameterInput(param, column.DbColValue, column.SqlType, column.Length);
        //            }
        //        }
        //        else if (column.CompareType == DatabaseDataValueCompareType.Like)
        //        {
        //            whereClause += string.Concat(column.Column, " like ", param);
        //            dac.SetParameterInput(param, column.DbColValue, column.SqlType, column.Length);
        //        }
        //        else if (column.CompareType == DatabaseDataValueCompareType.In)
        //        {
        //            whereClause += string.Format("{0} in ({1})", column.Column, column.DbColValue);
        //        }
        //        else if (column.CompareType == DatabaseDataValueCompareType.BiggerThen)
        //        {
        //            whereClause += string.Concat(column.Column, " > ", param);
        //            dac.SetParameterInput(param, column.DbColValue, column.SqlType, column.Length);
        //        }
        //        else if (column.CompareType == DatabaseDataValueCompareType.BiggerThenOrEquals)
        //        {
        //            whereClause += string.Concat(column.Column, " >= ", param);
        //            dac.SetParameterInput(param, column.DbColValue, column.SqlType, column.Length);
        //        }
        //        else if (column.CompareType == DatabaseDataValueCompareType.SmallerThen)
        //        {
        //            whereClause += string.Concat(column.Column, " < ", param);
        //            dac.SetParameterInput(param, column.DbColValue, column.SqlType, column.Length);
        //        }
        //        else if (column.CompareType == DatabaseDataValueCompareType.SmallerThenOrEquals)
        //        {
        //            whereClause += string.Concat(column.Column, " =< ", param);
        //            dac.SetParameterInput(param, column.DbColValue, column.SqlType, column.Length);
        //        }
        //        else if (column.CompareType == DatabaseDataValueCompareType.OrIn)
        //        {
        //            string[] valueArr = column.DbColValue as string[];
        //            if (valueArr != null && valueArr.Length > 0)
        //            {
        //                string tmp = "";
        //                foreach (string valueItm in valueArr)
        //                {
        //                    if (tmp.Length > 0) tmp += " or ";

        //                    if (string.Equals(valueItm, "null", StringComparison.OrdinalIgnoreCase))
        //                        tmp += string.Concat(column.Column, " is null");
        //                    else
        //                        tmp += string.Format("{0} in ({1})", column.Column, valueItm);
        //                }
        //                if (tmp.Length > 0) whereClause += string.Format(" ({0}) ", tmp);
        //            }
        //        }

        //        if (isSingleFilter)
        //        {
        //            if (string.IsNullOrEmpty(this.SqlJoin))
        //                this.SqlJoin = string.Format("join wim_DataFilters on DataFilter_Item_Key = {0}_Key and DataFilter_Property_Key = {1} and {2}", this.SqlColumnPrefix, p.ID, whereClause);
        //            else
        //                this.SqlJoin += string.Format(" join wim_DataFilters on DataFilter_Item_Key = {0}_Key and DataFilter_Property_Key = {1} and {2}", this.SqlColumnPrefix, p.ID, whereClause);
        //        }
        //        else
        //        {
        //            whereClause += ") ";

        //            //  If a NEXT column exists please validate its connecttype.
        //            if (nextcolumn != null)
        //            {
        //                if (nextcolumn.ConnectType == DatabaseDataValueConnectType.And)
        //                {
        //                    if (orGroupIsSet)
        //                    {
        //                        orGroupIsSet = false;
        //                        whereClause += ") and ";
        //                    }
        //                    else
        //                        whereClause += " and ";
        //                }
        //                else if (nextcolumn.ConnectType == DatabaseDataValueConnectType.Or || nextcolumn.ConnectType == DatabaseDataValueConnectType.OrUngrouped)
        //                {
        //                    whereClause += " or ";
        //                }
        //            }
        //        }
        //        index++;
        //    }

        //    if (orGroupIsSet) whereClause += ")";

        //    if (isSingleFilter)
        //        return null;

        //    return string.Concat("select distinct DataFilter_Item_Key from wim_DataFilters where ", whereClause);
        //}

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <param name="type">The type.</param>
        string GetType(SqlDbType type)
        {
            switch (type)
            {
                case SqlDbType.Bit:
                    return "B"; 
                case SqlDbType.Int:
                    return "I"; 
                case SqlDbType.VarChar:
                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.Text:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                    return "C"; 

                case SqlDbType.Decimal:
                case SqlDbType.Money:
                    return "D"; 

                case SqlDbType.DateTime:
                case SqlDbType.Date:
                    return "T"; 
                default:
                    return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual internal string SqlLastExecutedWhereClause { get; set; }

        string GetWhereClause(Connection.DataCommander dac, List<DatabaseDataValueColumn> whereColumns)
        {
            return GetWhereClause(dac, whereColumns, true);
        }

        /// <summary>
        /// Gets the where clause.
        /// </summary>
        /// <param name="dac">The dac.</param>
        /// <param name="whereColumns">The where columns.</param>
        /// <param name="isSelect">if set to <c>true</c> [is select].</param>
        /// <returns></returns>
        string GetWhereClause(Connection.DataCommander dac, List<DatabaseDataValueColumn> whereColumns, bool isSelect)
        {
            if (IsGenericEntity && isSelect && this.PropertyListID > 0)
            {
                if (whereColumns == null) whereColumns = new List<DatabaseDataValueColumn>();
                whereColumns.Add(new DatabaseDataValueColumn("<SQL_COL>_List_Key", SqlDbType.Int, PropertyListID));
            }

            if (whereColumns == null || whereColumns.Count == 0) return null;
            string whereClause = "where ";

            int index = 0;

            bool orGroupIsSet = false;
            foreach (DatabaseDataValueColumn column in whereColumns)
            {
                string param = "?";// string.Concat("@C", index);

                if (!string.IsNullOrEmpty(column.Column))
                {
                    if (!string.IsNullOrEmpty(this.SqlColumnPrefix) && column.Column.Contains("<SQL_COL>"))
                        column.Column = column.Column.Replace("<SQL_COL>", this.SqlColumnPrefix);

                }
                else
                {
                    if (!string.IsNullOrEmpty(this.SqlColumnPrefix) && column.SqlText.Contains("<SQL_COL>"))
                        column.SqlText = column.SqlText.Replace("<SQL_COL>", this.SqlColumnPrefix);
                }



                //  If a NEXT column exists please validate its connecttype.
                DatabaseDataValueColumn nextcolumn = null;
                if (whereColumns.Count > index + 1)
                {
                    nextcolumn = whereColumns[index + 1];
                    if (column.ConnectType == DatabaseDataValueConnectType.And && nextcolumn.ConnectType == DatabaseDataValueConnectType.Or)
                    {
                        orGroupIsSet = true;
                        whereClause += "(";
                    }
                }
                if (column.CompareType == DatabaseDataValueCompareType.Default)
                {
                    if (!string.IsNullOrEmpty(column.SqlText))
                    {
                        whereClause += column.SqlText;
                    }
                    else if (column.DbColValue == null)
                    {
                        whereClause += string.Concat(column.Column, " IS NULL");
                    }
                    else
                    {
                        whereClause += string.Concat(column.Column, " = ", param);
                        dac.SetParameterInput(column.Column, column.DbColValue, column.DbType, column.Length);
                    }
                }
                else if (column.CompareType == DatabaseDataValueCompareType.Like)
                {
                    whereClause += string.Concat(column.Column, " like ", param);
                    dac.SetParameterInput(column.Column, column.DbColValue, column.DbType, column.Length);
                }
                else if (column.CompareType == DatabaseDataValueCompareType.In)
                {
                    whereClause += string.Format("{0} in ({1})", column.Column, column.DbColValue);
                }
                else if (column.CompareType == DatabaseDataValueCompareType.BiggerThen)
                {
                    whereClause += string.Concat(column.Column, " > ", param);
                    dac.SetParameterInput(column.Column, column.DbColValue, column.DbType, column.Length);
                }
                else if (column.CompareType == DatabaseDataValueCompareType.BiggerThenOrEquals)
                {
                    whereClause += string.Concat(column.Column, " >= ", param);
                    dac.SetParameterInput(column.Column, column.DbColValue, column.DbType, column.Length);
                }
                else if (column.CompareType == DatabaseDataValueCompareType.SmallerThen)
                {
                    whereClause += string.Concat(column.Column, " < ", param);
                    dac.SetParameterInput(column.Column, column.DbColValue, column.DbType, column.Length);
                }
                else if (column.CompareType == DatabaseDataValueCompareType.SmallerThenOrEquals)
                {
                    whereClause += string.Concat(column.Column, " <= ", param);
                    dac.SetParameterInput(column.Column, column.DbColValue, column.DbType, column.Length);
                }
                else if (column.CompareType == DatabaseDataValueCompareType.OrIn)
                {
                    string[] valueArr = column.DbColValue as string[];
                    if (valueArr != null && valueArr.Length > 0)
                    {
                        string tmp = "";
                        foreach (string valueItm in valueArr)
                        {
                            if (tmp.Length > 0) tmp += " or ";

                            if (string.Equals(valueItm, "null", StringComparison.OrdinalIgnoreCase))
                                tmp += string.Concat(column.Column, " is null");
                            else
                                tmp += string.Format("{0} in ({1})", column.Column, valueItm);
                        }
                        if (tmp.Length > 0) whereClause += string.Format(" ({0}) ", tmp);
                    }
                }

                //  If a NEXT column exists please validate its connecttype.
                if (nextcolumn != null)
                {
                    if (nextcolumn.ConnectType == DatabaseDataValueConnectType.And)
                    {
                        if (orGroupIsSet)
                        {
                            orGroupIsSet = false;
                            whereClause += ") and ";
                        }
                        else
                            whereClause += " and ";
                    }
                    else if (nextcolumn.ConnectType == DatabaseDataValueConnectType.Or || nextcolumn.ConnectType == DatabaseDataValueConnectType.OrUngrouped)
                    {
                        whereClause += " or ";
                    }
                }
                index++;
            }
            if (orGroupIsSet) whereClause += ")";
            return whereClause;
        }


        /// <summary>
        /// Update an implementaion record.
        /// </summary>
        /// <param name="whereColumns">The where columns.</param>
        /// <returns></returns>
        protected virtual bool Update(List<DatabaseDataValueColumn> whereColumns)
        {
            //this.SqlConnectionString
            if (ConnectionType != DataConnectionType.SqlServer)
                return this.UpdateOdbcOrOleDb(whereColumns);

            m_SqlParameters = null;

            string updateColumns = " ";
            string whereClause = "";

            
            using (Connection.SqlCommander dac = new Connection.SqlCommander(SqlConnectionString))
            {
                DatabaseColumnAttribute customDataParam = null;

                foreach (DatabaseColumnAttribute param in SqlParameters)
                {
                    if (param.IsPrimaryKey) continue;
                    if (param.IsOnlyRead) continue;
                    if (string.IsNullOrEmpty(param.ColumnSubQuery) && string.IsNullOrEmpty(param.ColumnSubQueryPropertyReference))
                    {
                        //  Double check
                        if (updateColumns.Contains(string.Concat(" ", param.Column.ToLower(), "= ")))
                            continue;

                        if (param.Info.PropertyType == typeof(Sushi.Mediakiwi.Data.CustomData) && customDataParam == null)
                        {
                            customDataParam = param;
                            continue;
                        }

                        updateColumns += string.Concat(param.Column, "= ", "@", param.Column, ", ");
                        dac.SetParameterInput(string.Concat("@", param.Column), GetPropertyValue(param.Info), param.SqlType, param.Length);
                    }
                }

                if (m_additionalParameterList != null)
                {
                    foreach (SqlParameter p in m_additionalParameterList)
                    {
                        updateColumns += string.Concat(p.ParameterName, "= ", "@", p.ParameterName, ", ");
                        dac.SetParameterInput(string.Concat("@", p.ParameterName), p.Value, p.SqlDbType);
                    }
                }

                if (!string.IsNullOrEmpty(updateColumns))
                    updateColumns = updateColumns.ToLower();

                if (customDataParam != null)
                {
                    Sushi.Mediakiwi.Data.CustomData data = customDataParam.Info.GetValue(this, null) as Sushi.Mediakiwi.Data.CustomData;
                    
                    if (data != null)
                    {
                        //Sushi.Mediakiwi.Data.CustomData clone = data.Clone();
                        if (this.PropertyListID.HasValue)
                        {
                            var list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(this.PropertyListID.Value);
                            Sushi.Mediakiwi.Data.Property[] properties = Sushi.Mediakiwi.Data.Property.SelectAll(this.PropertyListID.Value, this.PropertyListItemID, false, false, true, DatabaseMappingPortal);
                                //Sushi.Mediakiwi.Data.Property.SelectAll(this.PropertyListID.Value, this.PropertyListItemID, false);
                            foreach (Sushi.Mediakiwi.Data.Property prop in properties)
                            {
                                //data[prop.FieldName].IsParsed = true;
                             
                                if (!string.IsNullOrEmpty(prop.Filter))
                                {

                                    //  Double check
                                    if (updateColumns.Contains(string.Concat(" ", prop.Filter.ToLower(), "= ")))
                                        continue;

                                    updateColumns += string.Concat(prop.Filter, "= ", "@P", prop.ID, ", ");

                                    Sushi.Mediakiwi.Data.CustomDataItem item = data[prop.FieldName];
                                    System.Type type = System.Type.GetType(prop.FilterType);
                                    dac.SetParameterInput(string.Concat("@P", prop.ID), item.ParseSqlParameterValue(type), item.ParseSqlParameterType(type));

                                    //data.Apply(prop.FieldName, null);
                                    //clone.Apply(prop.FieldName, null);
                                }
                                //else
                                //{
                                //    if (data[prop.FieldName].IsNull)
                                //        data.Apply(prop.FieldName, null);
                                //    else if (data[prop.FieldName].ParseInt().GetValueOrDefault() == 0)
                                //    {
                                //        if (
                                //            prop.TypeID == (int)Sushi.Mediakiwi.Framework.ContentType.Binary_Document
                                //            || prop.TypeID == (int)Sushi.Mediakiwi.Framework.ContentType.Binary_Image
                                //            || prop.TypeID == (int)Sushi.Mediakiwi.Framework.ContentType.Choice_Checkbox
                                //            || prop.TypeID == (int)Sushi.Mediakiwi.Framework.ContentType.FolderSelect
                                //            || prop.TypeID == (int)Sushi.Mediakiwi.Framework.ContentType.PageSelect
                                //            )
                                //            data.Apply(prop.FieldName, null);
                                //    }
                                //}
                            }
                            //foreach (CustomDataItem item in data.Items)
                            //{
                            //    if (!item.IsParsed)
                            //    {
                            //        string xx = item.Property;
                            //        //data.Apply(item.Property, null);
                            //    }
                            //}
                        }
                        updateColumns += string.Concat(customDataParam.Column, "= ", "@", customDataParam.Column, ", ");
                        dac.SetParameterInput(string.Concat("@", customDataParam.Column), data.Serialized, customDataParam.SqlType, customDataParam.Length);
                    }
                }

                if (updateColumns.Length == 0) return false;

                whereClause = GetWhereClause(dac, whereColumns);

                string sqlText = string.Format("update {0} set {1} {2}",
                    SqlTableChecked,
                    updateColumns.Substring(0, updateColumns.Length - 2),
                    whereClause);

                SqlLastExecuted = dac.ApplyParameters(sqlText);
                
                if (SqlOnlySetStatement)
                    return true;

                dac.SqlText = sqlText;


                CheckValidity(true);
                long start = DateTime.Now.Ticks;
                dac.ExecNonQuery();
                LoggingWrite(start, sqlText);
                
                return true;
            }
        }

        bool UpdateOdbcOrOleDb(List<DatabaseDataValueColumn> whereColumns)
        {
            m_SqlParameters = null;

            string updateColumns = " ";
            string whereClause = "";

            using (Connection.DataCommander dac = new Connection.DataCommander(SqlConnectionString))
            {
                dac.ConnectionType = this.ConnectionType;

                
                DatabaseColumnAttribute customDataParam = null;
                foreach (DatabaseColumnAttribute param in SqlParameters)
                {
                    if (param.IsPrimaryKey) continue;
                    if (param.IsOnlyRead) continue;
                    if (string.IsNullOrEmpty(param.ColumnSubQuery) && string.IsNullOrEmpty(param.ColumnSubQueryPropertyReference))
                    {
                        //  Double check
                        if (updateColumns.Contains(string.Concat(" ", param.Column.ToLower(), "=")))
                            continue;

                        updateColumns += string.Concat(param.Column, "= ?, ");
                        dac.SetParameterInput(param.Column, GetPropertyValue(param.Info), param.DbType, param.Length);

                        //  [20090128:MM]
                        #region Custom columns
                        if (this.PropertyListID.HasValue && param.Info.PropertyType == typeof(Sushi.Mediakiwi.Data.CustomData) && customDataParam == null)
                        {
                            customDataParam = param;
                        }
                        #endregion
                    }
                }

                if (m_additionalParameterList != null)
                {
                    foreach (SqlParameter p in m_additionalParameterList)
                    {
                        updateColumns += string.Concat(p.ParameterName, "= ?, ");
                        dac.SetParameterInput(string.Concat("@", p.ParameterName), p.Value, p.SqlDbType);
                    }
                }

                if (!string.IsNullOrEmpty(updateColumns))
                    updateColumns = updateColumns.ToLower();

                if (customDataParam != null)
                {
                    Sushi.Mediakiwi.Data.CustomData data = customDataParam.Info.GetValue(this, null) as Sushi.Mediakiwi.Data.CustomData;
                    if (data != null)
                    {
                        var list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(this.PropertyListID.Value);
                        Sushi.Mediakiwi.Data.Property[] properties = Sushi.Mediakiwi.Data.Property.SelectAll(this.PropertyListID.Value, this.PropertyListItemID, false, false, true, DatabaseMappingPortal);
                            //Sushi.Mediakiwi.Data.Property.SelectAll(this.PropertyListID.Value, this.PropertyListItemID, false);
                        foreach (Sushi.Mediakiwi.Data.Property prop in properties)
                        {
                            if (!string.IsNullOrEmpty(prop.Filter))
                            {
                                //  Double check
                                if (updateColumns.Contains(string.Concat(" ", prop.Filter.ToLower(), "=")))
                                    continue;

                                updateColumns += string.Concat(prop.Filter, "= ?, ");

                                Sushi.Mediakiwi.Data.CustomDataItem item = data[prop.FieldName];
                                System.Type type = System.Type.GetType(prop.FilterType);
                                dac.SetParameterInput(string.Concat("@P", prop.ID), item.ParseSqlParameterValue(type), item.ParseSqlParameterType(type));
                            }
                        }
                    }
                }

                if (updateColumns.Length == 0) return false;


                whereClause = GetWhereClause(dac, whereColumns, false);

                string sqlText = string.Format("update {0} set {1} {2}",
                    SqlTableChecked,
                    updateColumns.Substring(0, updateColumns.Length - 2),
                    whereClause);

                SqlLastExecuted = sqlText;
                if (SqlOnlySetStatement) return true;

                dac.Text = sqlText;
                CheckValidity(true);
                long start = DateTime.Now.Ticks;
                dac.ExecNonQuery();
                LoggingWrite(start, sqlText);
                

                return true;
            }
        }

        /// <summary>
        /// Update an implementaion record.
        /// </summary>
        /// <returns></returns>
        public virtual bool Update()
        {
            List<DatabaseDataValueColumn> whereColumns = new List<DatabaseDataValueColumn>();
            whereColumns.Add(new DatabaseDataValueColumn(PrimairyKeyColumn, SqlDbType.Int, PrimairyKeyValue));
            return Update(whereColumns);
        }

        /// <summary>
        /// Update an implementaion record.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <returns></returns>
        protected virtual internal bool Update(Guid guid)
        {
            List<DatabaseDataValueColumn> whereColumns = new List<DatabaseDataValueColumn>();
            whereColumns.Add(new DatabaseDataValueColumn(string.Concat(MigrationKeyColumn, "='", guid.ToString(), "'")));
            return Update(whereColumns);
        }



        /// <summary>
        /// Set a property value.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="value">The value.</param>
        /// <param name="entity">The entity.</param>
        void SetPropertyValue(PropertyInfo info, object value, object entity)
        {
            if (value == DBNull.Value) value = null;

            if (value != null)
            {
                //  System.Uri
                if (info.PropertyType == typeof(System.Uri))
                    value = new Uri(value.ToString());
                
                    //  System.Timestamp
                else if (info.PropertyType == typeof(System.TimeSpan))
                {
                    TimeSpan ts;
                    if (TimeSpan.TryParse(value.ToString(), out ts))
                        value = ts;
                }
                else if (info.PropertyType == typeof(System.Decimal))// && value.GetType() == typeof(System.Double))
                {
                    value = Convert.ToDecimal(value);
                }
                else if (info.PropertyType == typeof(System.Int32))// && value.GetType() == typeof(System.Double))
                {
                    value = Convert.ToInt32(value);
                }
                //  System.GUID
                else if (info.PropertyType == typeof(System.Guid))
                    value = new Guid(value.ToString());

                else if (info.PropertyType == typeof(System.String))
                    value = value.ToString();
                
                else if (info.PropertyType == typeof(Data.CustomData))
                {
                    Data.CustomData tmp = new CustomData();
                    tmp.ApplySerialized(value.ToString());
                    value = tmp;
                }

                else if (info.PropertyType == typeof(Data.AssetInfo))
                {
                    Data.AssetInfo tmp = new AssetInfo();
                    tmp.AssetID = Convert.ToInt32(value);
                    value = tmp;
                }
            }

            try
            {
                info.SetValue(entity, value, null);
            }
            catch (Exception innerException)
            {
                string message = string.Format("Dalreflection: Error whilst setting the {1} property of type {0} with type {2}"
                    , info.PropertyType.ToString() //0
                    , info.Name //1
                    , value == null ? "unkown (=NULL)" : value.GetType().ToString() //2
                    );
                throw new Exception(message, innerException);
            }
        }

        Sushi.Mediakiwi.Data.CustomData SetPropertyValueCustomData(PropertyInfo info, object value, object entity)
        {
            if (value == DBNull.Value) value = null;

            Data.CustomData tmp = new CustomData();

            if (value != null)
                tmp.ApplySerialized(value.ToString());

            value = tmp;

            try
            {
                info.SetValue(entity, value, null);
            }
            catch (Exception innerException)
            {
                string message = string.Format("Dalreflection: Error whilst setting the {1} property of type {0} with type {2}", info.PropertyType.ToString(), info.Name, value == null ? "unkown (=NULL)" : value.GetType().ToString());
                throw new Exception(message, innerException);
            }
            return tmp;
        }

        /// <summary>
        /// Get a property value.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <returns></returns>
        object GetPropertyValue(PropertyInfo info)
        {
            object value = info.GetValue(this, null);

            if (info.PropertyType == typeof(DateTime))
            {
                if (((DateTime)value) == DateTime.MinValue) value = null;
            }
            else if (info.PropertyType == typeof(Guid))
            {
                if (((Guid)value) == Guid.Empty) value = null;
                //else if (Sushi.Mediakiwi.Data.Common.IsOdbc)
                //{
                //    return value.ToString();
                //}
            }

            //  System.TimeSpan
            else if (info.PropertyType == typeof(System.TimeSpan))
            {
                System.TimeSpan tmp = (System.TimeSpan)info.GetValue(this, null);
                value = tmp.ToString();
            }

            //  System.Uri
            else if (info.PropertyType == typeof(System.Uri))
            {
                System.Uri tmp = (System.Uri)info.GetValue(this, null);
                value = tmp.ToString();
            }

            //else if (info.PropertyType == typeof(Data.ContentContainer))
            //{
            //    Data.ContentContainer tmp = (Data.ContentContainer)info.GetValue(this, null);
            //    if (tmp != null)
            //        value = tmp.Serialized;
            //}

            else if (info.PropertyType == typeof(Data.CustomData))
            {
                Data.CustomData tmp = (Data.CustomData)info.GetValue(this, null);
                if (tmp != null)
                    value = tmp.Serialized;
            }
            else if (info.PropertyType == typeof(Data.AssetInfo))
            {
                Data.AssetInfo tmp = (Data.AssetInfo)info.GetValue(this, null);
                if (tmp != null)
                    value = tmp.AssetID;
            }
            return value;
        }

        /// <summary>
        /// Delete an implementation record.
        /// </summary>
        /// <returns></returns>
        public virtual bool Delete()
        {
            List<DatabaseDataValueColumn> whereColumns = new List<DatabaseDataValueColumn>();
            whereColumns.Add(new DatabaseDataValueColumn(PrimairyKeyColumn, SqlDbType.Int, PrimairyKeyValue));
            return Delete(whereColumns);
        }

        /// <summary>
        /// Delete an implementation record.
        /// </summary>
        /// <param name="whereColumns">The where columns.</param>
        /// <returns></returns>
        public virtual bool Delete(List<DatabaseDataValueColumn> whereColumns)
        {
            if (ConnectionType != DataConnectionType.SqlServer)
                return this.DeleteOdbcOrOleDb(whereColumns);

            string whereClause = "";
            using (Connection.SqlCommander dac = new Connection.SqlCommander(SqlConnectionString))
            {
                whereClause= GetWhereClause(dac, whereColumns, false);
                if (whereClause.Length == 0) return false;
                
                string sqlText = string.Format("delete from {0} {1}",
                    this.SqlTableChecked,
                    whereClause);

                SqlLastExecuted = sqlText;
                if (SqlOnlySetStatement) return false;

                dac.SqlText = sqlText;
                long start = DateTime.Now.Ticks;
                dac.ExecNonQuery();
                LoggingWrite(start, sqlText);

                Wim.Utilities.CacheItemManager.FlushIndexOfCacheObjects(string.Concat("Data_", this.GetType().ToString()));
                return true;
            }
        }

        /// <summary>
        /// Deletes the ODBC or OLE db.
        /// </summary>
        /// <param name="whereColumns">The where columns.</param>
        /// <returns></returns>
        bool DeleteOdbcOrOleDb(List<DatabaseDataValueColumn> whereColumns)
        {
            string whereClause = "";
            using (Connection.DataCommander dac = new Connection.DataCommander(SqlConnectionString))
            {
                dac.ConnectionType = this.ConnectionType;

                whereClause = GetWhereClause(dac, whereColumns, false);

                if (whereClause.Length == 0) return false;
                string sqlText = string.Format("delete from {0} {1}",
                    this.SqlTableChecked,
                    whereClause);

                SqlLastExecuted = sqlText;
                if (SqlOnlySetStatement) return false;

                dac.Text = sqlText;
                long start = DateTime.Now.Ticks;
                dac.ExecNonQuery();
                LoggingWrite(start, sqlText);
                

                return true;
            }
        }

        /// <summary>
        /// Check the database entity on validity. This is performed through scanning the set attribute parameters.
        /// </summary>
        /// <param name="isDatabaseUpdate">if set to <c>true</c> [is database update].</param>
        /// <returns></returns>
        private bool CheckValidity(bool isDatabaseUpdate)
        {
            foreach (DatabaseColumnAttribute param in SqlParameters)
            {
                if (!param.IsNullable && param.Info.GetValue(param.Entity, null) == null)
                {
                    if (isDatabaseUpdate && param.IsOnlyRead) continue;
                    if (string.IsNullOrEmpty(param.ColumnSubQuery))
                        throw new Exception(string.Format("Property {0} can not be set as nullable for the executed query", param.Info.Name));

                }
            }
            return true;
        }
    }
}