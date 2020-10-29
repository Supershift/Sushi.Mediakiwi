using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Sushi.Mediakiwi.Data.DalReflection;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// 
    /// </summary>
    [DatabaseTable("wim_Catalogs", Order = "Catalog_Title ASC")]
    public class Catalog : DatabaseEntity
    {       
        /// <summary>
        /// 
        /// </summary>
        public class CatalogColumn
        {
            /// <summary>
            /// Gets or sets the type of the database.
            /// </summary>
            /// <value>The type of the database.</value>
            public string DatabaseType { get; set; }
            /// <summary>
            /// Gets or sets the type.
            /// </summary>
            /// <value>The type.</value>
            public Type Type { get; set; }
            public string ClassType { get; set; }
            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>The name.</value>
            public string Name { get; set; }
            /// <summary>
            /// Gets or sets the index.
            /// </summary>
            /// <value>The index.</value>
            public int Index { get; set; }
            /// <summary>
            /// Gets or sets the length.
            /// </summary>
            /// <value>The length.</value>
            public int Length { get; set; }
            /// <summary>
            /// Gets or sets a value indicating whether this instance is nullable.
            /// </summary>
            /// <value>
            /// 	<c>true</c> if this instance is nullable; otherwise, <c>false</c>.
            /// </value>
            public bool IsNullable { get; set; }
            public SqlDbType SqlDbType { get; internal set; } 
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static Catalog SelectOne(int ID)
        {
            //  For Memory allocation in command line tooling
            #region  Command Line Memory Allocation!
            if (System.Web.HttpContext.Current == null)
            {
                if (MemoryAllocationList == null)
                    MemoryAllocationList = new List<Catalog>();

                foreach (Catalog catalog in MemoryAllocationList)
                {
                    if (catalog.ID == ID)
                        return catalog;
                }
            }
            #endregion

            Catalog candidate = new Catalog();
            if (!string.IsNullOrEmpty(SqlConnectionString2)) candidate.SqlConnectionString = SqlConnectionString2;
            candidate = (Catalog)candidate._SelectOne(ID);
            SqlConnectionString2 = null;

            //  For Memory allocation in command line tooling
            if (System.Web.HttpContext.Current == null)
            {
                MemoryAllocationList.Add(candidate);
            }
            return candidate;
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <param name="portal">The portal.</param>
        /// <returns></returns>
        internal static Catalog SelectOne(int ID, Sushi.Mediakiwi.Framework.WimServerPortal portal)
        {
            Catalog candidate = new Catalog();
            if (portal != null) candidate.SqlConnectionString = portal.Connection;
            candidate = (Catalog)candidate._SelectOne(ID);
            return candidate;
        }

        static List<Catalog> MemoryAllocationList;

        /// <summary>
        /// Selects the all_ import export.
        /// </summary>
        /// <param name="portal">The portal.</param>
        /// <returns></returns>
        public static List<Catalog> SelectAll_ImportExport(string portal)
        {
            Catalog implement = new Catalog();
            List<Catalog> list = new List<Catalog>();

            implement.SqlConnectionString = Sushi.Mediakiwi.Data.Common.GetPortal(portal).Connection;
            implement.ConnectionType = Sushi.Mediakiwi.Data.Common.GetPortal(portal).Type;

            foreach (object o in implement._SelectAll(null, false, "CatalogImportExport", portal))
                list.Add((Catalog)o);

            return list;
        }


        bool m_IsExistingTableSet;
        bool m_IsExistingTable;
        /// <summary>
        /// Gets a value indicating whether this instance is existing table.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is existing table; otherwise, <c>false</c>.
        /// </value>
        public bool IsExistingTable
        {
            get
            {
                if (!m_IsExistingTableSet)
                {
                    m_IsExistingTableSet = true;

                    try
                    {
                        Catalog cl = new Catalog();
                        cl.SqlConnectionString = this.ConnectionString;

                        int count = Wim.Utility.ConvertToInt(cl.Execute(string.Concat("select count(*) from ", this.Table)));
                        m_IsExistingTable = true;
                        return m_IsExistingTable;
                    }
                    catch (Exception)
                    {
                        m_IsExistingTable = false;
                        return m_IsExistingTable;
                    }
                }
                return m_IsExistingTable;
            }
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString
        {
            get
            {
                if (this.ConnectionIndex > 0)
                {
                    if (this.ConnectionIndex == 1)
                        return Sushi.Mediakiwi.Data.Common.CurrentPortal.Connection1;
                }
                return this.SqlConnectionString;
            }
        }


        #region MOVED to EXTENSION / LOGIC

        //public CatalogColumn[] GetColumns()
        //{
        //    return GetColumns(true, this.Table);
        //}



        //CatalogColumn[] m_CatalogColumn;
        ///// <summary>
        ///// Gets the filter columns.
        ///// </summary>
        ///// <param name="fiterKnown">if set to <c>true</c> [fiter known].</param>
        ///// <param name="table">The table.</param>
        ///// <returns></returns>
        //public CatalogColumn[] GetColumns(bool fiterKnown, string table)
        //{
        //    //m_CatalogColumn = null;
        //    if (m_CatalogColumn == null || m_CatalogColumn.Length == 0)
        //    {
        //        List<CatalogColumn> list = new List<CatalogColumn>();
        //        Catalog implement = new Catalog();
        //        using (Connection.DataCommander dac = new Connection.DataCommander(this.ConnectionString))
        //        {
        //            //if (implement.ConnectionType == DataConnectionType.InterSystemsCache)
        //            //    dac.Text = string.Format("select name, length, status, usertype from %TSQL_SYS.columns where parent_obj_name = '{0}' order by colid", table);
        //            //else
        //            dac.Text = string.Format("select syscolumns.[Name], syscolumns.prec, isnullable, sysTypes.Name from syscolumns left join sysTypes on syscolumns.xusertype = sysTypes.xtype and sysTypes.status = 0 where id = (select id from sysobjects where name = '{0}') order by colorder", table);
        //            string[] exceptions = new string[0];

        //            if (fiterKnown)
        //            {
        //                exceptions = new string[] {
        //                    string.Concat(this.ColumnPrefix, "_key"),
        //                    string.Concat(this.ColumnPrefix, "_guid"),
        //                    string.Concat(this.ColumnPrefix, "_data")};
        //            }

        //            IDataReader reader = dac.ExecReader;

        //            while (reader.Read())
        //            {
        //                string column = reader.GetString(0);
        //                string columnSqlType = reader.GetString(3);

        //                bool skipMe = false;
        //                foreach (string exception in exceptions)
        //                {
        //                    if (exception.Equals(column, StringComparison.OrdinalIgnoreCase))
        //                    {
        //                        skipMe = true;
        //                        break;
        //                    }
        //                }
        //                if (skipMe) continue;

        //                int precision = Wim.Utility.ConvertToInt(reader.GetValue(1));
        //                bool isNullable = false;

        //                //if (implement.ConnectionType == DataConnectionType.InterSystemsCache)
        //                //{
        //                //    try
        //                //    {
        //                //        isNullable = Wim.Utility.ConvertToInt(reader.GetValue(2)) == 8;
        //                //    }
        //                //    catch (Exception) { }
        //                //}
        //                //else
        //                isNullable = reader.GetInt32(2) == 1;

        //                CatalogColumn c = new CatalogColumn();
        //                c.Index = precision;
        //                c.Name = column;
        //                c.Length = precision;
        //                c.IsNullable = isNullable;
        //                this.ApplySqlDbType(c, columnSqlType);
        //                list.Add(c);
        //            }
        //            //reader.Close();
        //            //reader.Dispose();
        //        }

        //        CatalogColumn[] arr = list.ToArray();
        //        using (Connection.DataCommander dac = new Connection.DataCommander(this.ConnectionString))
        //        {
        //            dac.Text = string.Concat("select top 0 * from ", table);
        //            IDataReader reader = dac.ExecReader;

        //            for (int index = 0; index < reader.FieldCount; index++)
        //            {
        //                CatalogColumn currentColumn = null;

        //                foreach (CatalogColumn col in arr)
        //                {
        //                    if (col.Name.Equals(reader.GetName(index)))
        //                    {
        //                        currentColumn = col;
        //                        currentColumn.Index = index;
        //                        break;
        //                    }
        //                }
        //                if (currentColumn == null) continue;

        //                currentColumn.DatabaseType = reader.GetDataTypeName(index);
        //                currentColumn.Type = reader.GetFieldType(index);

        //                //switch (currentColumn.Type)
        //                //{
        //                //    case System.Int32: 
        //                //        currentColumn.SqlDbType = SqlDbType.Int; 
        //                //        break;
        //                //    case System.Guid: 
        //                //        currentColumn.SqlDbType = SqlDbType.UniqueIdentifier; 
        //                //        break;
        //                //}
        //            }
        //        }
        //        m_CatalogColumn = arr;
        //    }
        //    return m_CatalogColumn;
        //}

        //void ApplySqlDbType(CatalogColumn col, string type)
        //{
        //    type = type.ToLower();
        //    if (type == "int" || type == "%library.integer")
        //    {
        //        col.ClassType = col.IsNullable ? "int?" : "int";
        //        col.SqlDbType = SqlDbType.Int;
        //    }
        //    else if (type == "bigint")
        //    {
        //        col.ClassType = col.IsNullable ? "Int64?" : "Int64";
        //        col.SqlDbType = SqlDbType.BigInt;
        //    }
        //    else if (type == "uniqueidentifier" || (type == "%%library.string" && col.Length == 36))
        //    {
        //        col.Type = typeof(Guid);
        //        col.ClassType = "Guid";
        //        col.SqlDbType = SqlDbType.UniqueIdentifier;
        //    }
        //    else if (type == "nvarchar" || type == "%%library.string")
        //    {
        //        col.Type = typeof(String);
        //        col.ClassType = "string";
        //        col.SqlDbType = SqlDbType.NVarChar;
        //    }
        //    else if (type == "datetime" || type == "%library.timestamp")
        //    {
        //        col.Type = typeof(DateTime);
        //        col.ClassType = col.IsNullable ? "DateTime?" : "DateTime";
        //        col.SqlDbType = SqlDbType.DateTime;
        //    }
        //    else if (type == "decimal" || type == "%library.decimal")
        //    {
        //        col.Type = typeof(Decimal);
        //        col.ClassType = col.IsNullable ? "decimal?" : "decimal";
        //        col.SqlDbType = SqlDbType.Decimal;
        //    }
        //    else if (type == "bit" || type == "%library.boolean")
        //    {
        //        col.Type = typeof(Boolean);
        //        col.ClassType = "bool";
        //        col.SqlDbType = SqlDbType.Bit;
        //    }
        //    else if (type == "xml" || type == "%library.globalcharacterstream")
        //    {
        //        col.Type = typeof(Sushi.Mediakiwi.Data.CustomData);
        //        col.ClassType = "Sushi.Mediakiwi.Data.CustomData";
        //        col.SqlDbType = SqlDbType.Xml;
        //    }
        //    else if (type == "varchar")
        //    {
        //        col.Type = typeof(String);
        //        col.ClassType = "string";
        //        col.SqlDbType = SqlDbType.VarChar;
        //    }
        //}


        //        /// <summary>
        //        /// Validates the SQL table GUID.
        //        /// </summary>
        //        /// <returns></returns>
        //        public bool ValidateSqlTableGuid()
        //        {
        //            try
        //            {
        //                Catalog cl = new Catalog();
        //                cl.SqlConnectionString = this.ConnectionString;

        //                if (IsExistingTable)
        //                    cl.Execute(string.Format("select count({1}_GUID) from {0}", this.Table, this.ColumnPrefix));

        //                return true;
        //            }
        //            catch (Exception)
        //            {
        //                return false;
        //            }
        //        }

        //        /// <summary>
        //        /// Validates the SQL table creation.
        //        /// </summary>
        //        /// <returns></returns>
        //        public bool ValidateSqlTableCreation()
        //        {
        //            string message = "";
        //            return ValidateSqlTableCreation(null, out message);
        //        }

        //        /// <summary>
        //        /// Validates the SQL table creation.
        //        /// </summary>
        //        /// <param name="message">The message.</param>
        //        /// <returns></returns>
        //        public bool ValidateSqlTableCreation(out string message)
        //        {
        //            return ValidateSqlTableCreation(null, out message);
        //        }

        //        /// <summary>
        //        /// Validates the SQL table key.
        //        /// </summary>
        //        /// <returns></returns>
        //        public bool ValidateSqlTableKey()
        //        {
        //            try
        //            {
        //                Catalog cl = new Catalog();
        //                cl.SqlConnectionString = this.ConnectionString;

        //                if (IsExistingTable)
        //                    cl.Execute(string.Format("select count({1}_Key) from {0}", this.Table, this.ColumnPrefix));

        //                return true;
        //            }
        //            catch (Exception)
        //            {
        //                return false;
        //            }
        //        }

        //        /// <summary>
        //        /// Validates the SQL table creation.
        //        /// </summary>
        //        /// <returns></returns>
        //        public bool ValidateSqlTableCreation(string portal, out string message)
        //        {
        //            message = "";

        //            if (string.IsNullOrEmpty(this.Table))
        //                return false;
        //            if (string.IsNullOrEmpty(this.ColumnPrefix))
        //                return false;

        //            if (!string.IsNullOrEmpty(portal))
        //            {
        //                this.SqlConnectionString = Sushi.Mediakiwi.Data.Common.GetPortal(portal).Connection;
        //                this.ConnectionType = Sushi.Mediakiwi.Data.Common.GetPortal(portal).Type;
        //            }

        //            if (!ValidateSqlTableKey() && string.IsNullOrEmpty(this.ColumnKey))
        //                throw new Exception(string.Format("Could not determine the {0}_Key (int) column in the table '{1}'. This has to be a primary key column.", this.ColumnPrefix, this.Table));

        //            if (!ValidateSqlTableGuid() && string.IsNullOrEmpty(this.ColumnGuid))
        //                throw new Exception(string.Format("Could not determine the {0}_Key (int) column in the table '{1}'. This has to be a primary key column.", this.ColumnPrefix, this.Table));

        //            Catalog cl = new Catalog();
        //            cl.SqlConnectionString = this.ConnectionString;

        //            try
        //            {
        //                int count = Wim.Utility.ConvertToInt(cl.Execute(string.Concat("select count(*) from ", this.Table)));

        //                #region Check for HasSortOrder
        //                if (this.HasSortOrder)
        //                {
        //                    try
        //                    {
        //                        cl.Execute(string.Format("select count({1}_SortOrder) from {0}", this.Table, this.ColumnPrefix));
        //                    }
        //                    catch (Exception)
        //                    {
        //                        if (message.Length == 0) message = string.Format("The column {0}_SortOrder was not found. This has been added to the catalog.", this.ColumnPrefix);

        //                        cl.Execute(string.Format("alter table {0} add {1}_SortOrder int NULL", this.Table, this.ColumnPrefix));
        //                        cl.Execute(string.Format("update {0} set {1}_SortOrder = {1}_Key", this.Table, this.ColumnPrefix));
        //                    }
        //                }
        //                #endregion

        //                #region Check for Data
        //                try
        //                {
        //                    cl.Execute(string.Format("select count({1}_Data) from {0}", this.Table, this.ColumnPrefix));
        //                }
        //                catch (Exception)
        //                {
        //                    if (message.Length == 0) message = string.Format("The column {0}_Data was not found. This has been added to the catalog.", this.ColumnPrefix);

        //                    //if (cl.ConnectionType == DataConnectionType.InterSystemsCache)
        //                    //    cl.Execute(string.Format("alter table {0} add {1}_Data NTEXT NULL", this.Table, this.ColumnPrefix));
        //                    //else
        //                    cl.Execute(string.Format("alter table {0} add {1}_Data XML NULL", this.Table, this.ColumnPrefix));
        //                }
        //                #endregion

        //                return true;
        //            }
        //            catch (Exception)
        //            {
        //                #region InterSystemsCache
        //                //                if (cl.ConnectionType == DataConnectionType.InterSystemsCache)
        //                //                {
        //                //                    if (this.HasSortOrder && this.HasCatalogBaseStructure)
        //                //                    {
        //                //                        try
        //                //                        {
        //                //                            cl.Execute(string.Format(@"CREATE TABLE {0} (
        //                //{1}_Key int IDENTITY(1,1) NOT NULL,
        //                //{1}_GUID char(36) NOT NULL,
        //                //{1}_List_Key int NOT NULL,
        //                //{1}_Site_Key int NOT NULL,
        //                //{1}_Data ntext NULL,
        //                //{1}_Created datetime NOT NULL,
        //                //{1}_SortOrder int NULL)", this.Table, this.ColumnPrefix));

        //                //                            cl.Execute(string.Format(@"ALTER TABLE {0} WITH NOCHECK ADD CONSTRAINT PK_{1}_Key PRIMARY KEY CLUSTERED ({1}_Key)", this.Table, this.ColumnPrefix));
        //                //                            return true;
        //                //                        }
        //                //                        catch (Exception ex)
        //                //                        {
        //                //                            Sushi.Mediakiwi.Data.Notification.InsertOne("Validate SqlTable Creation", ex);
        //                //                            return false;
        //                //                        }
        //                //                    }
        //                //                    else if (this.HasSortOrder && !this.HasCatalogBaseStructure)
        //                //                    {
        //                //                        try
        //                //                        {
        //                //                            cl.Execute(string.Format(@"CREATE TABLE {0} (
        //                //{1}_Key int IDENTITY(1,1) NOT NULL,
        //                //{1}_GUID char(36) NOT NULL,
        //                //{1}_Data ntext NULL,
        //                //{1}_SortOrder int NULL)", this.Table, this.ColumnPrefix));

        //                //                            cl.Execute(string.Format(@"ALTER TABLE {0} WITH NOCHECK ADD CONSTRAINT PK_{1}_Key PRIMARY KEY CLUSTERED ({1}_Key)", this.Table, this.ColumnPrefix));
        //                //                            return true;
        //                //                        }
        //                //                        catch (Exception ex)
        //                //                        {
        //                //                            Sushi.Mediakiwi.Data.Notification.InsertOne("Validate SqlTable Creation", ex);
        //                //                            return false;
        //                //                        }
        //                //                    }
        //                //                    else if (!this.HasSortOrder && this.HasCatalogBaseStructure)
        //                //                    {
        //                //                        try
        //                //                        {
        //                //                            cl.Execute(string.Format(@"CREATE TABLE {0} (
        //                //{1}_Key int IDENTITY(1,1) NOT NULL,
        //                //{1}_GUID char(36) NOT NULL,
        //                //{1}_List_Key int NOT NULL,
        //                //{1}_Site_Key int NOT NULL,
        //                //{1}_Data ntext NULL,
        //                //{1}_Created datetime NOT NULL)", this.Table, this.ColumnPrefix));

        //                //                            cl.Execute(string.Format(@"ALTER TABLE {0} WITH NOCHECK ADD CONSTRAINT PK_{1}_Key PRIMARY KEY CLUSTERED ({1}_Key)", this.Table, this.ColumnPrefix));
        //                //                            return true;
        //                //                        }
        //                //                        catch (Exception ex)
        //                //                        {
        //                //                            Sushi.Mediakiwi.Data.Notification.InsertOne("Validate SqlTable Creation", ex);
        //                //                            return false;
        //                //                        }
        //                //                    }
        //                //                    else
        //                //                    {
        //                //                        try
        //                //                        {
        //                //                            cl.Execute(string.Format(@"CREATE TABLE {0} (
        //                //{1}_Key int IDENTITY(1,1) NOT NULL,
        //                //{1}_GUID char(36) NOT NULL,
        //                //{1}_Data ntext NULL)", this.Table, this.ColumnPrefix));

        //                //                            cl.Execute(string.Format(@"ALTER TABLE {0} WITH NOCHECK ADD CONSTRAINT PK_{1}_Key PRIMARY KEY CLUSTERED ({1}_Key)", this.Table, this.ColumnPrefix));
        //                //                            return true;
        //                //                        }
        //                //                        catch (Exception ex)
        //                //                        {
        //                //                            Sushi.Mediakiwi.Data.Notification.InsertOne("Validate SqlTable Creation", ex);
        //                //                            return false;
        //                //                        }
        //                //                    }
        //                //                }
        //                #endregion
        //                //                else
        //                //{
        //                if (this.HasSortOrder && this.HasCatalogBaseStructure)
        //                {
        //                    try
        //                    {
        //                        cl.Execute(string.Format(@"CREATE TABLE {0} (
        //{1}_Key int IDENTITY(1,1) NOT NULL,
        //{1}_GUID uniqueidentifier NOT NULL,
        //{1}_List_Key int NOT NULL,
        //{1}_Site_Key int NOT NULL,
        //{1}_Data xml NULL,
        //{1}_Created datetime NOT NULL,
        //{1}_SortOrder int NULL)", this.Table, this.ColumnPrefix));

        //                        cl.Execute(string.Format(@"ALTER TABLE {0} WITH NOCHECK ADD CONSTRAINT PK_{1}_Key PRIMARY KEY CLUSTERED ({1}_Key)", this.Table, this.ColumnPrefix));
        //                        return true;
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        Sushi.Mediakiwi.Data.Notification.InsertOne("Validate SqlTable Creation", ex);
        //                        return false;
        //                    }
        //                }
        //                else if (this.HasSortOrder && !this.HasCatalogBaseStructure)
        //                {
        //                    try
        //                    {
        //                        cl.Execute(string.Format(@"CREATE TABLE {0} (
        //{1}_Key int IDENTITY(1,1) NOT NULL,
        //{1}_GUID uniqueidentifier NOT NULL,
        //{1}_Data xml NULL,
        //{1}_SortOrder int NULL)", this.Table, this.ColumnPrefix));

        //                        cl.Execute(string.Format(@"ALTER TABLE {0} WITH NOCHECK ADD CONSTRAINT PK_{1}_Key PRIMARY KEY CLUSTERED ({1}_Key)", this.Table, this.ColumnPrefix));
        //                        return true;
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        Sushi.Mediakiwi.Data.Notification.InsertOne("Validate SqlTable Creation", ex);
        //                        return false;
        //                    }
        //                }
        //                else if (!this.HasSortOrder && this.HasCatalogBaseStructure)
        //                {
        //                    try
        //                    {
        //                        cl.Execute(string.Format(@"CREATE TABLE {0} (
        //{1}_Key int IDENTITY(1,1) NOT NULL,
        //{1}_GUID uniqueidentifier NOT NULL,
        //{1}_List_Key int NOT NULL,
        //{1}_Site_Key int NOT NULL,
        //{1}_Data xml NULL,
        //{1}_Created datetime NOT NULL)", this.Table, this.ColumnPrefix));

        //                        cl.Execute(string.Format(@"ALTER TABLE {0} WITH NOCHECK ADD CONSTRAINT PK_{1}_Key PRIMARY KEY CLUSTERED ({1}_Key)", this.Table, this.ColumnPrefix));
        //                        return true;
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        Sushi.Mediakiwi.Data.Notification.InsertOne("Validate SqlTable Creation", ex);
        //                        return false;
        //                    }
        //                }
        //                else
        //                {
        //                    try
        //                    {
        //                        cl.Execute(string.Format(@"CREATE TABLE {0} (
        //{1}_Key int IDENTITY(1,1) NOT NULL,
        //{1}_GUID uniqueidentifier NOT NULL,
        //{1}_Data xml NULL)", this.Table, this.ColumnPrefix));

        //                        cl.Execute(string.Format(@"ALTER TABLE {0} WITH NOCHECK ADD CONSTRAINT PK_{1}_Key PRIMARY KEY CLUSTERED ({1}_Key)", this.Table, this.ColumnPrefix));
        //                        return true;
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        Sushi.Mediakiwi.Data.Notification.InsertOne("Validate SqlTable Creation", ex);
        //                        return false;
        //                    }
        //                }
        //                //}
        //            }
        //        }



        #endregion MOVED to EXTENSION / LOGIC

        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

        int m_ID;
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        [DatabaseColumn("Catalog_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }
        Guid m_GUID;
        /// <summary>
        /// Gets or sets the GUID.
        /// </summary>
        /// <value>The GUID.</value>
        [DatabaseColumn("Catalog_GUID", SqlDbType.UniqueIdentifier, IsMigrationKey = true)]
        public Guid GUID
        {
            get
            {
                if (this.m_GUID == Guid.Empty) this.m_GUID = Guid.NewGuid();
                return m_GUID;
            }
            set { m_GUID = value; }
        }

        string m_Title;
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Title", 50, true, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        [DatabaseColumn("Catalog_Title", SqlDbType.NVarChar, Length = 50)]
        public string Title
        {
            get { return m_Title; }
            set { m_Title = value; }
        }

        string m_Table;
        /// <summary>
        /// Gets or sets the table.
        /// </summary>
        /// <value>The table.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Table", 50, true, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        [DatabaseColumn("Catalog_Table", SqlDbType.VarChar, Length = 50)]
        public string Table
        {
            get { return m_Table; }
            set { m_Table = value; }
        }

        bool mIsActive;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Is active", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        [DatabaseColumn("Catalog_IsActive", SqlDbType.Bit)]
        public bool IsActive
        {
            get { return mIsActive; }
            set { mIsActive = value; }
        }

        string m_ColumnPrefix;
        /// <summary>
        /// Gets or sets the column prefix.
        /// </summary>
        /// <value>The column prefix.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Columnprefix", 25, true, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        [DatabaseColumn("Catalog_ColumnPrefix", SqlDbType.VarChar, Length = 25)]
        public string ColumnPrefix
        {
            get { return m_ColumnPrefix; }
            set { m_ColumnPrefix = value; }
        }

        DateTime m_Created;
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        [DatabaseColumn("Catalog_Created", SqlDbType.DateTime, IsNullable = false)]
        public DateTime Created
        {
            get
            {
                if (this.m_Created == DateTime.MinValue) this.m_Created = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
                return m_Created;
            }
            set { m_Created = value; }
        }

        ///// <summary>
        ///// Gets or sets a value indicating whether this instance has sort order.
        ///// </summary>
        ///// <value>
        ///// 	<c>true</c> if this instance has sort order; otherwise, <c>false</c>.
        ///// </value>
        /// <summary>
        /// Gets or sets a value indicating whether this instance has data column.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has data column; otherwise, <c>false</c>.
        /// </value>
        [Sushi.Mediakiwi.Framework.OnlyEditableWhenTrue("IsNew")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Has sort order", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        [DatabaseColumn("Catalog_HasSortOrder", SqlDbType.Bit)]
        public bool HasSortOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has internal references.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has internal references; otherwise, <c>false</c>.
        /// </value>
        [Sushi.Mediakiwi.Framework.OnlyEditableWhenTrue("IsNew")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("List & site specific", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        [DatabaseColumn("Catalog_HasInternalRef", SqlDbType.Bit)]
        public bool HasCatalogBaseStructure { get; set; }

        /// <summary>
        /// Gets or sets the connection.
        /// </summary>
        /// <value>The connection.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Connection", "Connections")]
        [DatabaseColumn("Catalog_Connection", SqlDbType.Int, IsNullable = true)]
        public int ConnectionIndex { get; set; }

        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Portal", "Portals")]
        [DatabaseColumn("Catalog_Portal", SqlDbType.VarChar, IsNullable = true)]
        public string PortalName { get; set; }

        /// <summary>
        /// Gets or sets the column key.
        /// </summary>
        /// <value>The column key.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Primary key", 30, true)]
        [DatabaseColumn("Catalog_ColumnKey", SqlDbType.VarChar, Length = 30, IsNullable = true)]
        public string ColumnKey { get; set; }

        /// <summary>
        /// Gets or sets the column GUID.
        /// </summary>
        /// <value>The column GUID.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Migration key", 30, false)]
        [DatabaseColumn("Catalog_ColumnGuid", SqlDbType.VarChar, Length = 30, IsNullable = true)]
        public string ColumnGuid { get; set; }

        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Data column(xml)", 30, false)]
        [DatabaseColumn("Catalog_ColumnData", SqlDbType.VarChar, Length = 30, IsNullable = true)]
        public string ColumnData { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is new.
        /// </summary>
        /// <value><c>true</c> if this instance is new; otherwise, <c>false</c>.</value>
        public bool IsNew
        {
            get { return this.ID == 0; }
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <returns></returns>
        public static Catalog SelectOne(string tableName)
        {
            List<DatabaseDataValueColumn> whereClause = new List<DatabaseDataValueColumn>();
            whereClause.Add(new DatabaseDataValueColumn("Catalog_Table", SqlDbType.NVarChar, tableName));

            return (Catalog)new Catalog()._SelectOne(whereClause);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public static Catalog[] SelectAll()
        {
            List<Catalog> list = new List<Catalog>();
            foreach (object obj in new Catalog()._SelectAll()) list.Add((Catalog)obj);
            return list.ToArray();
        }

        //public static Catalog[] SelectAll(int whereValue)
        //{
        //    List<Catalog> list = new List<Catalog>();
        //    List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
        //    where.Add(new DatabaseDataValueColumn("cat_", SqlDbType., whereValue));

        //    foreach (object o in new Catalog()._SelectAll(where)) list.Add((Catalog)o);
        //    return list.ToArray();
        //}

        /// <summary>
        /// Deletes the SQL table.
        /// </summary>
        /// <returns></returns>
        public bool DeleteSqlTable()
        {
            Catalog cl = new Catalog();
            cl.Execute(string.Concat("drop table ", this.Table));
            return true;
        }

        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard
    }
}
