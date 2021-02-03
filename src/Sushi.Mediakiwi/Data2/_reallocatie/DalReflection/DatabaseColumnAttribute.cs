using System;
using System.Reflection;
using System.Data;
using System.Data.OleDb;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Data.DalReflection
{
 


    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class DatabaseColumnAttribute : Attribute
    {
        public FieldInfo Info2;
        /// <summary>
        /// Corresponding entity property.
        /// </summary>
        public PropertyInfo Info;
        /// <summary>
        /// Corresponding entity.
        /// </summary>
        public Object Entity;
        /// <summary>
        /// The reflection columns can set grouped so the SQL statements (select, insert, update) can 
        /// be targeted to smaller data groups (f.e. only Id and title field required for select all).
        /// </summary>
        public DatabaseColumnGroup CollectionLevel;
        /// <summary>
        /// Property reference for setting the column output value (requires GetPropertyValue to be overridden!). 
        /// This contains a name of a property from which the required T-SQL is returned.
        /// This column is automatically excluded with INSERT and UPDATE.
        /// </summary>
        public string ColumnSubQueryPropertyReference;
        /// <summary>
        /// Subquery for setting the column output value. This column is automatically excluded with INSERT and UPDATE.
        /// </summary>
        public string ColumnSubQuery;
        /// <summary>
        /// The SQL type of the database column.
        /// </summary>
        public string Column;
        string m_ParamVariable;
        /// <summary>
        /// Gets or sets the param variable.
        /// </summary>
        /// <value>The param variable.</value>
        internal string ParamVariable
        {
            get { 
                if (string.IsNullOrEmpty(m_ParamVariable))
                   return Column;
                return m_ParamVariable;
            }
            set { m_ParamVariable = value; }
        }
        /// <summary>
        /// The type of the database column.
        /// </summary>
        public DbType DbType;
        public string DbTypeString
        {
            get { return DbType.ToString(); }
        }
        /// <summary>
        /// The SQL type of the database column (SQL SERVER).
        /// </summary>
        public SqlDbType SqlType;
        /// <summary>
        /// The SQL type of the database column (ODBC).
        /// </summary>
        public OdbcType OdbcType;
        /// <summary>
        /// The SQL type of the database column (OLEDB).
        /// </summary>
        public OleDbType OleDbType;
        /// <summary>
        /// The set length of the database column.
        /// </summary>
        public int Length;
        /// <summary>
        /// Is the database column a primary key?
        /// </summary>
        public bool IsPrimaryKey;
        /// <summary>
        /// Is the primary key an automatically generated identifier? If [false] than it should implement IIdentity
        /// </summary>
        public bool IdentityInsert;
        /// <summary>
        /// Is the database column a migration (import/export) key?
        /// </summary>
        public bool IsMigrationKey;
        /// <summary>
        /// Is the database column automatically generated and should only be read and not written to?
        /// </summary>
        public bool IsOnlyRead;
        /// <summary>
        /// Is the database column nullable?
        /// </summary>
        public bool IsNullable;
        /// <summary>
        /// An attribute defined for use in rapid DAL creation. This specific attribute is used for specifying the database columns.
        /// </summary>
        public DatabaseColumnAttribute(string column, DbType type)
        {
            Column = column;
            this.DbType = type;
        }
        /// <summary>
        /// An attribute defined for use in rapid DAL creation. This specific attribute is used for specifying the database columns.
        /// </summary>
        public DatabaseColumnAttribute(string column, SqlDbType type)
        {
            Column = column;
            this.SqlType = type;
            this.DbType = Convert(type);
        }
        /// <summary>
        /// An attribute defined for use in rapid DAL creation. This specific attribute is used for specifying the database columns.
        /// </summary>
        public DatabaseColumnAttribute(string column, OdbcType type)
        {
            Column = column;
            this.OdbcType = type;
            this.DbType = Convert(type);
        }
        /// <summary>
        /// An attribute defined for use in rapid DAL creation. This specific attribute is used for specifying the database columns.
        /// </summary>
        public DatabaseColumnAttribute(string column, OleDbType type)
        {
            Column = column;
            this.OleDbType = type;
            this.DbType = Convert(type);
        }

        /// <summary>
        /// Converts the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        DbType Convert(SqlDbType type)
        {
            switch (type)
            {
                case SqlDbType.Bit: return DbType.Boolean;
            }

            SqlParameter tmp = new SqlParameter();
            tmp.SqlDbType = type;
            return tmp.DbType;
        }

        /// <summary>
        /// Converts the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        DbType Convert(OdbcType type)
        {
            OdbcParameter tmp = new OdbcParameter();
            tmp.OdbcType = type;
            return tmp.DbType;
        }

        /// <summary>
        /// Converts the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        DbType Convert(OleDbType type)
        {
            OleDbParameter tmp = new OleDbParameter();
            tmp.OleDbType = type;
            return tmp.DbType;
        }
    }
}
