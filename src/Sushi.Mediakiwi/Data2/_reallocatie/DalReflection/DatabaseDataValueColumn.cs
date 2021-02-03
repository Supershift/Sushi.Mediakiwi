using System;
using System.Reflection;
using System.Data;
using System.Data.SqlClient;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Data.DalReflection
{
    /// <summary>
    /// 
    /// </summary>
    public class DatabaseDataValueColumn
    {
        public string Property;
        /// <summary>
        /// A direct SQL statement
        /// </summary>
        public string SqlText;
        /// <summary>
        /// The database column name.
        /// </summary>
        public string Column;
        /// <summary>
        /// The database column value.
        /// </summary>
        public object DbColValue;
        /// <summary>
        /// The type of the database column.
        /// </summary>
        public DbType DbType;
        /// <summary>
        /// The type of the database column (SQL Server).
        /// </summary>
        public SqlDbType SqlType;
        /// <summary>
        /// The type of the database column (ODBC).
        /// </summary>
        public OdbcType OdbcType;
        /// <summary>
        /// The type of the database column (OLEDB).
        /// </summary>
        public OleDbType OleDbType;
        /// <summary>
        /// The database column maximum length.
        /// </summary>
        public int Length;
        /// <summary>
        /// The type of compare
        /// </summary>
        public DatabaseDataValueCompareType CompareType;
        /// <summary>
        /// The type of connection between parameters
        /// </summary>
        public DatabaseDataValueConnectType ConnectType;

        public DatabaseDataValueColumn(string property, object value)
        {
            this.Property = property;
            this.DbColValue = value;
        }

        /// <summary>
        /// A Direct SQL text for the where clause.
        /// </summary>
        /// <param name="sqlText">The SQL text.</param>
        public DatabaseDataValueColumn(string sqlText)
        {
            this.SqlText = sqlText;
        }

        #region SQL
        /// <summary>
        /// Used to define T-SQL parameters with there corresping values. F.e. for use in SelectOne where clauses.
        /// </summary>
        public DatabaseDataValueColumn(string column, SqlDbType type)
            : this(column, type, null) { }

        /// <summary>
        /// Used to define T-SQL parameters with there corresping values. F.e. for use in SelectOne where clauses.
        /// </summary>
        public DatabaseDataValueColumn(string column, SqlDbType type, object dbColValue)
            : this(column, type, dbColValue, 0) { }

        /// <summary>
        /// Used to define T-SQL parameters with there corresping values. F.e. for use in SelectOne where clauses.
        /// </summary>
        public DatabaseDataValueColumn(string column, SqlDbType type, object dbColValue, int length)
            : this(column, type, dbColValue, length, DatabaseDataValueCompareType.Default) { }

        /// <summary>
        /// Used to define T-SQL parameters with there corresping values. F.e. for use in SelectOne where clauses.
        /// </summary>
        public DatabaseDataValueColumn(string column, SqlDbType type, object dbColValue, DatabaseDataValueCompareType compareType)
            : this(column, type, dbColValue, 0, compareType) { }

        /// <summary>
        /// Used to define T-SQL parameters with there corresping values. F.e. for use in SelectOne where clauses.
        /// </summary>
        public DatabaseDataValueColumn(string column, SqlDbType type, object dbColValue, DatabaseDataValueConnectType connectType)
            : this(column, type, dbColValue, 0, DatabaseDataValueCompareType.Default, connectType) { }

        /// <summary>
        /// Used to define T-SQL parameters with there corresping values. F.e. for use in SelectOne where clauses.
        /// </summary>
        public DatabaseDataValueColumn(string column, SqlDbType type, object dbColValue, DatabaseDataValueCompareType compareType, DatabaseDataValueConnectType connectType)
            : this(column, type, dbColValue, 0, compareType, connectType) { }

        /// <summary>
        /// Used to define T-SQL parameters with there corresping values. F.e. for use in SelectOne where clauses.
        /// </summary>
        public DatabaseDataValueColumn(string column, SqlDbType type, object dbColValue, int length, DatabaseDataValueCompareType compareType)
            : this(column, type, dbColValue, length, compareType, DatabaseDataValueConnectType.And) { }

        /// <summary>
        /// Used to define T-SQL parameters with there corresping values. F.e. for use in SelectOne where clauses.
        /// </summary>
        public DatabaseDataValueColumn(string column, SqlDbType type, object dbColValue, int length, DatabaseDataValueCompareType compareType, DatabaseDataValueConnectType connectType)
        {
            Column = column;
            SqlType = type;
            this.DbType = Convert(type);
            DbColValue = dbColValue;
            Length = length;
            this.CompareType = compareType;
            this.ConnectType = connectType;
        }
        #endregion

        #region ODBC
        /// <summary>
        /// Used to define T-SQL parameters with there corresping values. F.e. for use in SelectOne where clauses.
        /// </summary>
        public DatabaseDataValueColumn(string column, OdbcType type)
            : this(column, type, null) { }

        /// <summary>
        /// Used to define T-SQL parameters with there corresping values. F.e. for use in SelectOne where clauses.
        /// </summary>
        public DatabaseDataValueColumn(string column, OdbcType type, object dbColValue)
            : this(column, type, dbColValue, 0) { }

        /// <summary>
        /// Used to define T-SQL parameters with there corresping values. F.e. for use in SelectOne where clauses.
        /// </summary>
        public DatabaseDataValueColumn(string column, OdbcType type, object dbColValue, int length)
            : this(column, type, dbColValue, length, DatabaseDataValueCompareType.Default) { }

        /// <summary>
        /// Used to define T-SQL parameters with there corresping values. F.e. for use in SelectOne where clauses.
        /// </summary>
        public DatabaseDataValueColumn(string column, OdbcType type, object dbColValue, DatabaseDataValueCompareType compareType)
            : this(column, type, dbColValue, 0, compareType) { }

        /// <summary>
        /// Used to define T-SQL parameters with there corresping values. F.e. for use in SelectOne where clauses.
        /// </summary>
        public DatabaseDataValueColumn(string column, OdbcType type, object dbColValue, DatabaseDataValueConnectType connectType)
            : this(column, type, dbColValue, 0, DatabaseDataValueCompareType.Default, connectType) { }

        /// <summary>
        /// Used to define T-SQL parameters with there corresping values. F.e. for use in SelectOne where clauses.
        /// </summary>
        public DatabaseDataValueColumn(string column, OdbcType type, object dbColValue, DatabaseDataValueCompareType compareType, DatabaseDataValueConnectType connectType)
            : this(column, type, dbColValue, 0, compareType, connectType) { }

        /// <summary>
        /// Used to define T-SQL parameters with there corresping values. F.e. for use in SelectOne where clauses.
        /// </summary>
        public DatabaseDataValueColumn(string column, OdbcType type, object dbColValue, int length, DatabaseDataValueCompareType compareType)
            : this(column, type, dbColValue, length, compareType, DatabaseDataValueConnectType.And) { }

        /// <summary>
        /// Used to define T-SQL parameters with there corresping values. F.e. for use in SelectOne where clauses.
        /// </summary>
        public DatabaseDataValueColumn(string column, OdbcType type, object dbColValue, int length, DatabaseDataValueCompareType compareType, DatabaseDataValueConnectType connectType)
        {
            Column = column;
            this.OdbcType = type;
            this.DbType = Convert(type); 
            DbColValue = dbColValue;
            Length = length;
            this.CompareType = compareType;
            this.ConnectType = connectType;
        }
        #endregion 

        #region OLEDB
        /// <summary>
        /// Used to define T-SQL parameters with there corresping values. F.e. for use in SelectOne where clauses.
        /// </summary>
        public DatabaseDataValueColumn(string column, OleDbType type)
            : this(column, type, null) { }

        /// <summary>
        /// Used to define T-SQL parameters with there corresping values. F.e. for use in SelectOne where clauses.
        /// </summary>
        public DatabaseDataValueColumn(string column, OleDbType type, object dbColValue)
            : this(column, type, dbColValue, 0) { }

        /// <summary>
        /// Used to define T-SQL parameters with there corresping values. F.e. for use in SelectOne where clauses.
        /// </summary>
        public DatabaseDataValueColumn(string column, OleDbType type, object dbColValue, int length)
            : this(column, type, dbColValue, length, DatabaseDataValueCompareType.Default) { }

        /// <summary>
        /// Used to define T-SQL parameters with there corresping values. F.e. for use in SelectOne where clauses.
        /// </summary>
        public DatabaseDataValueColumn(string column, OleDbType type, object dbColValue, DatabaseDataValueCompareType compareType)
            : this(column, type, dbColValue, 0, compareType) { }

        /// <summary>
        /// Used to define T-SQL parameters with there corresping values. F.e. for use in SelectOne where clauses.
        /// </summary>
        public DatabaseDataValueColumn(string column, OleDbType type, object dbColValue, DatabaseDataValueConnectType connectType)
            : this(column, type, dbColValue, 0, DatabaseDataValueCompareType.Default, connectType) { }

        /// <summary>
        /// Used to define T-SQL parameters with there corresping values. F.e. for use in SelectOne where clauses.
        /// </summary>
        public DatabaseDataValueColumn(string column, OleDbType type, object dbColValue, DatabaseDataValueCompareType compareType, DatabaseDataValueConnectType connectType)
            : this(column, type, dbColValue, 0, compareType, connectType) { }

        /// <summary>
        /// Used to define T-SQL parameters with there corresping values. F.e. for use in SelectOne where clauses.
        /// </summary>
        public DatabaseDataValueColumn(string column, OleDbType type, object dbColValue, int length, DatabaseDataValueCompareType compareType)
            : this(column, type, dbColValue, length, compareType, DatabaseDataValueConnectType.And) { }

        /// <summary>
        /// Used to define T-SQL parameters with there corresping values. F.e. for use in SelectOne where clauses.
        /// </summary>
        public DatabaseDataValueColumn(string column, OleDbType type, object dbColValue, int length, DatabaseDataValueCompareType compareType, DatabaseDataValueConnectType connectType)
        {
            Column = column;
            this.OleDbType = type;
            this.DbType = Convert(type);
            DbColValue = dbColValue;
            Length = length;
            this.CompareType = compareType;
            this.ConnectType = connectType;
        }
        #endregion 
        
        #region Generic
        /// <summary>
        /// Used to define T-SQL parameters with there corresping values. F.e. for use in SelectOne where clauses.
        /// </summary>
        public DatabaseDataValueColumn(string column, DbType type)
            : this(column, type, null) { }

        /// <summary>
        /// Used to define T-SQL parameters with there corresping values. F.e. for use in SelectOne where clauses.
        /// </summary>
        public DatabaseDataValueColumn(string column, DbType type, object dbColValue)
            : this(column, type, dbColValue, 0) { }

        /// <summary>
        /// Used to define T-SQL parameters with there corresping values. F.e. for use in SelectOne where clauses.
        /// </summary>
        public DatabaseDataValueColumn(string column, DbType type, object dbColValue, int length)
            : this(column, type, dbColValue, length, DatabaseDataValueCompareType.Default) { }

        /// <summary>
        /// Used to define T-SQL parameters with there corresping values. F.e. for use in SelectOne where clauses.
        /// </summary>
        public DatabaseDataValueColumn(string column, DbType type, object dbColValue, DatabaseDataValueCompareType compareType)
            : this(column, type, dbColValue, 0, compareType) { }

        /// <summary>
        /// Used to define T-SQL parameters with there corresping values. F.e. for use in SelectOne where clauses.
        /// </summary>
        public DatabaseDataValueColumn(string column, DbType type, object dbColValue, DatabaseDataValueConnectType connectType)
            : this(column, type, dbColValue, 0, DatabaseDataValueCompareType.Default, connectType) { }

        /// <summary>
        /// Used to define T-SQL parameters with there corresping values. F.e. for use in SelectOne where clauses.
        /// </summary>
        public DatabaseDataValueColumn(string column, DbType type, object dbColValue, DatabaseDataValueCompareType compareType, DatabaseDataValueConnectType connectType)
            : this(column, type, dbColValue, 0, compareType, connectType) { }

        /// <summary>
        /// Used to define T-SQL parameters with there corresping values. F.e. for use in SelectOne where clauses.
        /// </summary>
        public DatabaseDataValueColumn(string column, DbType type, object dbColValue, int length, DatabaseDataValueCompareType compareType)
            : this(column, type, dbColValue, length, compareType, DatabaseDataValueConnectType.And) { }

        /// <summary>
        /// Used to define T-SQL parameters with there corresping values. F.e. for use in SelectOne where clauses.
        /// </summary>
        public DatabaseDataValueColumn(string column, DbType type, object dbColValue, int length, DatabaseDataValueCompareType compareType, DatabaseDataValueConnectType connectType)
        {
            Column = column;
            this.DbType = type;
            DbColValue = dbColValue;
            Length = length;
            this.CompareType = compareType;
            this.ConnectType = connectType;
        }
        #endregion


        /// <summary>
        /// Converts the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        DbType Convert(SqlDbType type)
        {
            //switch (type)
            //{
            //    case SqlDbType.Bit: return DbType.Boolean;
            //}

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