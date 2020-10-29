using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.Framework
{
    public static class DataRecordExtensions
    {
        public static bool HasColumn(this IDataRecord dr, string columnName)
        {
            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (dr.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }
    }

    public class DataRequest : IDataRequest
    {
        public DataRequest(string cacheReference = null, string alternativeConnectionString = null)
        {
            CacheReference = cacheReference;
            AlternativeConnectionString = alternativeConnectionString;
            WhereClause = new List<DatabaseDataValueColumn>();
            SqlParameters = new List<SqlParameter>();
        }

        public string AlternativeConnectionString { get; }
        public string CacheReference { get; }
        public int? MaxResults { get; set; }
        public string OrderByColumn { get; set; }
        /// <summary>
        /// Represents paging information applied to a Wim ComponentList. If a paging object is applied and filled the SqlEntityParser will run 2 queries on SelectAll(IDataRequest).
        /// One query to retrieve the total count and one query to retrieve only the items for the specific page.
        /// </summary>
        public DataEntities.IDataDetail PagingData { get; set; }

        public List<SqlParameter> SqlParameters { get; private set; }
        public void AddParam(string name, object value, SqlDbType type)
        {
            if (SqlParameters == null)
                SqlParameters = new List<SqlParameter>();

            SqlParameter p = new SqlParameter();
            p.Value = value;
            p.ParameterName = name;
            p.SqlDbType = type;
            SqlParameters.Add(p);
        }

        public List<DatabaseDataValueColumn> WhereClause { get; private set; }
        public void AddWhere(string column, SqlDbType type, object value)
        {
            if (WhereClause == null)
                WhereClause = new List<DatabaseDataValueColumn>();

            var where = new DatabaseDataValueColumn(column, type, value);
            WhereClause.Add(where);
        }
        public void AddWhere(string sql)
        {
            if (WhereClause == null)
                WhereClause = new List<DatabaseDataValueColumn>();

            var where = new DatabaseDataValueColumn(sql);
            WhereClause.Add(where);
        }
        public void AddWhere(string property, object value)
        {
            if (WhereClause == null)
                WhereClause = new List<DatabaseDataValueColumn>();

            var where = new DatabaseDataValueColumn(property, value);
            WhereClause.Add(where);
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class SqlEntityParser : ISqlEntityParser
    {
        ///// <summary>
        ///// All set SQL parameters (scanned using the set attributes).
        ///// </summary>
        DatabaseColumnAttribute[] GetSqlParameters(object entity, SqlInformation sqlInfo)
        {
            System.Type type = entity.GetType();
            if (_reflected.ContainsKey(type))
                return _reflected[type];

            PropertyInfo[] props = type.GetProperties();
            var parameters = GetSqlParameters(props, entity, sqlInfo);

            _reflected.TryAdd(type, parameters);

            return parameters;
        }

        static ConcurrentDictionary<Type, DatabaseColumnAttribute[]> _reflected = new ConcurrentDictionary<Type, DatabaseColumnAttribute[]>();

        /// <summary>
        /// Gets the SQL parameters.
        /// </summary>
        /// <param name="props">The props.</param>
        /// <param name="entity">The entity.</param>
        DatabaseColumnAttribute[] GetSqlParameters<T>(PropertyInfo[] props, T entity, SqlInformation sqlInfo)
        {
            List<DatabaseColumnAttribute> sqlParameters = new List<DatabaseColumnAttribute>();
            foreach (PropertyInfo info in props)
            {
                DatabaseColumnAttribute[] attributes = (DatabaseColumnAttribute[])info.GetCustomAttributes(typeof(DatabaseColumnAttribute), false);
                foreach (DatabaseColumnAttribute attribute in attributes)
                {
                    if (attribute != null)
                    {
                        attribute.Info = info;
                        attribute.Entity = entity;

                        if (!string.IsNullOrEmpty(sqlInfo.SqlColumnPrefix) && attribute.Column.Contains("<SQL_COL>"))
                        {
                            if (attribute.Column == "<SQL_COL>_Key" && !string.IsNullOrEmpty(sqlInfo.SqlTableKey))
                                attribute.Column = sqlInfo.SqlTableKey;
                            else
                                if (attribute.Column == "<SQL_COL>_GUID" && !string.IsNullOrEmpty(sqlInfo.SqlTableGUID))
                                attribute.Column = sqlInfo.SqlTableGUID;
                            else
                                attribute.Column = attribute.Column.Replace("<SQL_COL>", sqlInfo.SqlColumnPrefix);
                        }

                        sqlParameters.Add(attribute);
                    }
                }
            }

            //if (sqlParameters == null || sqlParameters.Count == 0)
            //    throw new Exception(string.Format("You forgot to apply any [DatabaseColumnAttribute] attribute to the {0} class",
            //        entity.GetType().FullName));

            return sqlParameters.ToArray();
        }

        /// <summary>
        /// Return the value of a property using reflection.
        /// Has to be overridden when using ColumnSubQueryPropertyReference.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        string GetPropertyValue(string name, object entity)
        {
            if (string.IsNullOrEmpty(name)) return "null";

            PropertyInfo prop = entity.GetType().GetProperty(name);
            if (prop == null || !prop.CanRead)
                throw new Exception(string.Format("The requested property ('{0}') is not accesible or does not exist", name));

            if (prop.PropertyType != typeof(string))
                throw new Exception(string.Format("The requested property ('{0}') is not of the correct type (System.String)", name));

            object value = prop.GetValue(entity, null);

            if (value == null)
                return "null";
            return value.ToString();
        }

        public string SqlLastExecuted { get; set; }

        protected class SqlInformation
        {
            public string SqlTable;
            /// <summary>
            /// Gets the SQL table checked (check for spaces when using [TableName] XX.
            /// </summary>
            /// <value>The SQL table checked.</value>
            public string SqlTableChecked
            {
                get
                {
                    if (SqlTable.Contains(" "))
                        return SqlTable.Split(' ')[0];
                    else
                        return SqlTable;
                }
            }

            public string SqlJoin { get; set; }
            public string SqlGroup { get; set; }
            public string SqlOrder { get; set; }
            public string SqlColumnPrefix { get; set; }
            public string SqlTableKey { get; set; }
            public string SqlTableGUID { get; set; }
            public int PropertyListTypeID { get; set; }
            public int? PropertyListID { get; set; }
            public int? PropertyListItemID { get; set; }
            public bool IsGenericEntity { get; set; }
            public int SqlRowCount { get; set; }

            /// <summary>
            /// The order of the result.
            /// </summary>
            /// <value>The result order.</value>
            [XmlIgnore()]
            public string ResultOrder
            {
                get
                {
                    return string.IsNullOrEmpty(SqlOrder)
                      ? null
                      : string.Concat(" order by ", SqlOrder);
                }
            }

            /// <summary>
            /// The Group of the result.
            /// </summary>
            /// <value>The result group.</value>
            [XmlIgnore()]
            public string ResultGroup
            {
                get
                {
                    return string.IsNullOrEmpty(SqlGroup)
                      ? null
                      : string.Concat(" group by ", SqlGroup);
                }
            }
        }

        ///// <summary>
        ///// Sets the property list information.
        ///// </summary>
        ///// <param name="propertyListID">The property list ID.</param>
        //void SetPropertyListInformation(int propertyListID)
        //{
        //    PropertyListID = propertyListID;
        //}



        /// <summary>
        /// 
        /// </summary>
        Sushi.Mediakiwi.Framework.WimServerPortal DatabaseMappingPortal { get; set; }

        T Init<T>(out SqlInformation sqlInfo, T entity = default(T))
        {
            sqlInfo = new SqlInformation();
            var type = typeof(T);

            if (entity == null && type.IsInterface && Sushi.Mediakiwi.Data.Environment.ContainsDependency<T>())
            {
                entity = (T)Sushi.Mediakiwi.Data.Environment.GetInstance(type);
                type = entity.GetType();
            }
            else if (entity == null)
            {
                entity = Activator.CreateInstance<T>();
                type = entity.GetType();
            }
            else if (entity != null)
                type = entity.GetType();

            DatabaseTableAttribute[] attributes =
                (DatabaseTableAttribute[])type.GetCustomAttributes(typeof(DatabaseTableAttribute), true);

            string portal = null;
            foreach (DatabaseTableAttribute attribute in attributes)
            {
                if (attribute == null) continue;

                if (!string.IsNullOrEmpty(attribute.Join)) sqlInfo.SqlJoin = attribute.Join;
                if (!string.IsNullOrEmpty(attribute.Order)) sqlInfo.SqlOrder = attribute.Order;
                if (!string.IsNullOrEmpty(attribute.Group)) sqlInfo.SqlGroup = attribute.Group;
                if (!string.IsNullOrEmpty(attribute.Portal)) portal = attribute.Portal;

                sqlInfo.SqlTable = attribute.Name;
                if (sqlInfo.SqlTable == null)
                {
                    throw new Exception(string.Format("You forgot to apply the [DatabaseTableAttribute] attribute to the {0} class",
                        type.FullName));
                }
            }
            DatabaseMappingPortal = Sushi.Mediakiwi.Data.Common.GetCurrentMappingConnection(type);

            if (DatabaseMappingPortal == null && portal != null)
            {
                DatabaseMappingPortal = Sushi.Mediakiwi.Data.Common.GetPortal(portal);
                if (DatabaseMappingPortal == null)
                {
                    throw new Exception(string.Format("Could not find the requested portal [{0}] in web.config", portal));
                }
            }

            if (DatabaseMappingPortal != null) SqlConnectionString = DatabaseMappingPortal.Connection;
            return entity;
        }



        DatabaseColumnAttribute Migration(DatabaseColumnAttribute[] sqlParams)
        {
            foreach (DatabaseColumnAttribute param in sqlParams)
            {
                if (param.IsMigrationKey)
                {
                    return param;
                }
            }
            return null;
        }

        DatabaseColumnAttribute Primary(DatabaseColumnAttribute[] sqlParams)
        {
            foreach (DatabaseColumnAttribute param in sqlParams)
            {
                if (param == null) continue;
                if (param.IsPrimaryKey)
                {
                    return param;
                }
            }
            return null;
        }

        int? PrimairyKeyValue(DatabaseColumnAttribute[] sqlParams, object entity)
        {
            var primary = Primary(sqlParams);
            if (primary == null)
                return null;

            int value;
            if (Wim.Utility.IsNumeric(primary.Info.GetValue(entity, null), out value))
                return value;
            return null;
        }

        int? PrimairyKeyValue(DatabaseColumnAttribute sqlParam, object entity)
        {
            if (sqlParam == null)
                return null;

            int value;
            if (Wim.Utility.IsNumeric(sqlParam.Info.GetValue(entity, null), out value))
                return value;
            return null;
        }

        Guid MigrationKeyValue(DatabaseColumnAttribute[] sqlParams, object entity)
        {
            var migration = Migration(sqlParams);
            if (migration == null)
                return Guid.Empty;

            Guid value;
            if (Wim.Utility.IsGuid(migration.Info.GetValue(entity, null), out value))
                return value;

            return Guid.Empty;
        }

        string m_SqlConnectionString;
        /// <summary>
        /// The Database connection string (default : ConfigurationManager.AppSettings["connect"])
        /// </summary>
        /// <value>The SQL connection string.</value>
        string SqlConnectionString
        {
            get
            {

                if (string.IsNullOrEmpty(m_SqlConnectionString))
                {
                    if (DatabaseMappingPortal == null)
                        m_SqlConnectionString = Sushi.Mediakiwi.Data.Common.DatabaseConnectionString;
                    else
                        m_SqlConnectionString = DatabaseMappingPortal.Connection;
                }
                else if (DatabaseMappingPortal != null && DatabaseMappingPortal.Connection != m_SqlConnectionString)
                    m_SqlConnectionString = DatabaseMappingPortal.Connection;

                return m_SqlConnectionString;
            }
            set { m_SqlConnectionString = value; }
        }

        string _HashedSqlConnectionString;
        string HashedConnectionString
        {
            get
            {
                if (_HashedSqlConnectionString == null)
                    _HashedSqlConnectionString = Wim.Utility.HashString(SqlConnectionString);
                return _HashedSqlConnectionString;
            }
        }

        /// <summary>
        /// _s the select one.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="cacheResult">if set to <c>true</c> [cache result].</param>
        /// <returns></returns>
        public T SelectOne<T>(int key, bool cacheResult, string alternativeConnectionString = null)
        {
            return SelectOne<T>(key, cacheResult, alternativeConnectionString, false);
        }

        /// <summary>
        /// _s the select one.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="cacheResult">if set to <c>true</c> [cache result].</param>
        /// <returns></returns>
        public T SelectOne<T>(int key, bool cacheResult, string alternativeConnectionString, bool alwaysRetrieveFromDatabase)
        {
            return SelectOne<T>(key, cacheResult, null, alternativeConnectionString, alwaysRetrieveFromDatabase);
        }

        /// <summary>
        /// Select a implementation based on the specified attributed and the applied migration key identifier.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="cacheResult">if set to <c>true</c> [cache result].</param>
        /// <returns></returns>
        public T SelectOne<T>(Guid key, bool cacheResult = false, string alternativeConnectionString = null)
        {
            SqlInformation sqlInfo;
            var entity = Init<T>(out sqlInfo);
            if (key == Guid.Empty) return (T)entity;

            var sqlcol = GetSqlParameters(entity, sqlInfo);
            var column = Migration(sqlcol);

            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            //[MR:13-11-2015] A Guid was being cast to an Integer :
            //list.Add(new DatabaseDataValueColumn(column.Column, SqlDbType.Int, key));
            list.Add(new DatabaseDataValueColumn(column.Column, SqlDbType.UniqueIdentifier, key));

            //  Added a additional ckey element as of the Filter options
            string ckey = null;
            if (cacheResult)
                ckey = sqlInfo.PropertyListItemID.HasValue ? string.Concat(key, "#", sqlInfo.PropertyListItemID) : key.ToString();

            T t;

            t = SelectOne<T>(list, ckey, alternativeConnectionString);

            return t;
        }

        /// <summary>
        /// Select a implementation based on the specified attributed and the applied primary key identifier.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="cacheResult">if set to <c>true</c> [cache result].</param>
        /// <param name="componentListID">The component list ID.</param>
        /// <returns></returns>
        public T SelectOne<T>(int key, bool cacheResult, int? componentListID, string alternativeConnectionString = null)
        {
            return SelectOne<T>(key, cacheResult, componentListID, alternativeConnectionString, false);
        }

        /// <summary>
        /// Select a implementation based on the specified attributed and the applied primary key identifier.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="cacheResult">if set to <c>true</c> [cache result].</param>
        /// <param name="componentListID">The component list ID.</param>
        /// <returns></returns>
        public T SelectOne<T>(int key, bool cacheResult, int? componentListID, string alternativeConnectionString, bool alwaysRetrieveFromDatabase)
        {
            SqlInformation sqlInfo;
            var entity = Init<T>(out sqlInfo);
            if (key == 0) return (T)entity;

            if (componentListID.HasValue)
            {
                var list2 = Sushi.Mediakiwi.Data.ComponentList.SelectOne(componentListID.Value);
                sqlInfo.SqlTable = list2.Catalog().Table;
                sqlInfo.SqlColumnPrefix = list2.Catalog().ColumnPrefix;
            }

            var sqlcol = GetSqlParameters(entity, sqlInfo);
            var column = Primary(sqlcol);

            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            list.Add(new DatabaseDataValueColumn(column.Column, SqlDbType.Int, key));

            //  Added a additional ckey element as of the Filter options
            string ckey = null;
            if (cacheResult)
                ckey = sqlInfo.PropertyListItemID.HasValue ? string.Concat(key, "#", sqlInfo.PropertyListItemID) : key.ToString();

            T t;

            t = SelectOne<T>(list, ckey, alternativeConnectionString, alwaysRetrieveFromDatabase);


            return t;
        }

        string CacheKey<T>(IDataRequest request = null)
        {
            return CacheKey<T>(request?.CacheReference, request?.AlternativeConnectionString);
        }

        string CacheKey<T>(string cacheReference = null, string alternativeConnectionString = null)
        {
            if (cacheReference == null)
                return null;

            var cachetype = typeof(T);
            var key =
                string.IsNullOrEmpty(alternativeConnectionString)
                    ? string.Concat(HashedConnectionString, ":Data_", cachetype.ToString(), ".", cacheReference)
                    : string.Concat(Wim.Utility.HashString(alternativeConnectionString), ":Data_", cachetype.ToString(), ".", cacheReference);
            return key;
        }

        string GetSelectColumns(DatabaseColumnAttribute[] sqlParameters, object entity, SqlInformation sqlInfo, Sushi.Mediakiwi.Data.Property[] propertyList = null)
        {
            string selectColumns = string.Empty;
            foreach (DatabaseColumnAttribute param in sqlParameters)
            {
                if (!string.IsNullOrEmpty(param.ColumnSubQuery))
                    selectColumns += string.Format("({1}) as {0}, ", param.Column, param.ColumnSubQuery).ToLower();
                else if (!string.IsNullOrEmpty(param.ColumnSubQueryPropertyReference))
                    selectColumns += string.Format("({1}) as {0}, ", param.Column, GetPropertyValue(param.ColumnSubQueryPropertyReference, entity)).ToLower();
                else
                {
                    selectColumns += string.Concat(param.Column, ", ").ToLower();
                }
            }

            //  NEW 15-05-2009
            if (sqlInfo.PropertyListID.HasValue)
            {
                propertyList = Sushi.Mediakiwi.Data.Property.SelectAll(sqlInfo.PropertyListID.Value, sqlInfo.PropertyListItemID, false, false, true, DatabaseMappingPortal);
                foreach (Sushi.Mediakiwi.Data.Property p in propertyList)
                {
                    if (!string.IsNullOrEmpty(p.Filter))
                    {
                        string filter = p.Filter.ToLower();
                        if (!selectColumns.Contains(string.Concat(filter, ", ")))
                            selectColumns += string.Concat(filter, ", ");
                    }
                }
            }
            return selectColumns.Substring(0, selectColumns.Length - 2);
        }

        public T SelectOne<T>(IDataRequest request = null) where T : class
        {
            var key = CacheKey<T>(request);
            if (key != null)
            {
                T outcome;
                if (Caching.IsCached<T>(key, out outcome))
                    return outcome;
            }

            SqlInformation sqlInfo;
            var entity = Init<T>(out sqlInfo);

            string whereClause = "";

            var sqlParameters = GetSqlParameters(entity, sqlInfo);

            Sushi.Mediakiwi.Data.Property[] propertyList = null;
            if (sqlInfo.PropertyListID.HasValue)
                propertyList = Sushi.Mediakiwi.Data.Property.SelectAll(sqlInfo.PropertyListID.Value, sqlInfo.PropertyListItemID, false, false, true, DatabaseMappingPortal);

            string selectColumns = GetSelectColumns(sqlParameters, entity, sqlInfo, propertyList);

            using (Sushi.Mediakiwi.Data.Connection.SqlCommander dac = new Sushi.Mediakiwi.Data.Connection.SqlCommander(string.IsNullOrEmpty(request?.AlternativeConnectionString) ? SqlConnectionString : request.AlternativeConnectionString))
            {
                whereClause = GetWhereClause(dac, request, sqlParameters, sqlInfo);
                if (selectColumns.Length == 0) return entity;

                string sqlText = string.Format("select top 1 {0} from {1} {2} {3}{5}{4}",
                    selectColumns,
                    sqlInfo.SqlTable,
                    sqlInfo.SqlJoin,
                    whereClause, sqlInfo.ResultOrder, sqlInfo.ResultGroup
                    );
                var result = SelectOne<T>(entity, sqlText, dac, sqlInfo, propertyList);
                if (key != null)
                    Caching.Add(key, result);
                return result;
            }

        }

        T SelectOne<T>(T entity, string sqlText, Sushi.Mediakiwi.Data.Connection.SqlCommander dac, SqlInformation sqlInfo, Property[] propertyList = null)
        {
            var start = DateTime.Now.Ticks;

            SqlLastExecuted = sqlText;
            dac.SqlText = sqlText;

            var sqlParameters = GetSqlParameters(entity, sqlInfo);
            //var primaryColumn = Primary(sqlParameters);

            SqlDataReader reader = dac.ExecReader;
            LoggingWrite(start, sqlText, sqlInfo);
            while (reader.Read())
            {
                var clone = (T)Activator.CreateInstance(entity.GetType());
                int index = 0;

                if (sqlParameters.Length == 0)
                    clone = (T)reader[0];

                foreach (DatabaseColumnAttribute param in sqlParameters)
                {
                    object value = GetData(reader, index, param);
                    if (param.Info.PropertyType == typeof(Sushi.Mediakiwi.Data.CustomData) && sqlInfo.PropertyListID.HasValue)
                    {

                        Sushi.Mediakiwi.Data.CustomData customData = SetPropertyValueCustomData(param.Info, value, clone);
                        foreach (Sushi.Mediakiwi.Data.Property p in propertyList)
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
                        SetPropertyValue(param.Info, value, clone);
                    }
                    index++;
                }
                CheckValidity(false, clone, sqlParameters);

                return clone;
            }
            return entity;
        }

        /// <summary>
        /// Select a implementation based on the specified attributed and the applied whereColumns which define the where clause.
        /// </summary>
        /// <param name="whereColumns">The where columns.</param>
        /// <param name="cacheReference">The Reference and the Value form a combined unique cachekey.</param>
        /// <returns></returns>
        public T SelectOne<T>(List<DatabaseDataValueColumn> whereColumns, string cacheReference = null, string alternativeConnectionString = null)
        {
            return SelectOne<T>(whereColumns, cacheReference, alternativeConnectionString, false);
        }


        /// <summary>
        /// Select a implementation based on the specified attributed and the applied whereColumns which define the where clause.
        /// </summary>
        /// <param name="whereColumns">The where columns.</param>
        /// <param name="cacheReference">The Reference and the Value form a combined unique cachekey.</param>
        /// <param name="alwaysRetrieveFromDatabase">If set to true, always call database to retrieve entry and do not look in cache. If cacheReference is specified the db result is stored in cache.</param>
        /// <returns></returns>
        public T SelectOne<T>(List<DatabaseDataValueColumn> whereColumns, string cacheReference, string alternativeConnectionString, bool alwaysRetrieveFromDatabase)
        {
            var key = CacheKey<T>(cacheReference, alternativeConnectionString);

            if (!alwaysRetrieveFromDatabase && key != null)
            {
                T outcome;
                if (Caching.IsCached<T>(key, out outcome))
                    return outcome;
            }
            SqlInformation sqlInfo;
            var entity = Init<T>(out sqlInfo);
            var type = entity.GetType();

            string whereClause = "";

            var sqlParameters = GetSqlParameters(entity, sqlInfo);
            var primaryColumn = Primary(sqlParameters);

            if (primaryColumn.IdentityInsert)
            {
                //  Custom set identifier, so verify the required interface implementation.
                if (entity is IIdentity)
                {
                    //  OK
                }
                else
                {
                    throw new Exception($"The entity {entity.ToString()} requires the implementation of IIdentity");
                }
            }

            Sushi.Mediakiwi.Data.Property[] propertyList = null;
            if (sqlInfo.PropertyListID.HasValue)
                propertyList = Sushi.Mediakiwi.Data.Property.SelectAll(sqlInfo.PropertyListID.Value, sqlInfo.PropertyListItemID, false, false, true, DatabaseMappingPortal);

            string selectColumns = GetSelectColumns(sqlParameters, entity, sqlInfo, propertyList);

            using (Sushi.Mediakiwi.Data.Connection.SqlCommander dac = new Sushi.Mediakiwi.Data.Connection.SqlCommander(string.IsNullOrEmpty(alternativeConnectionString) ? SqlConnectionString : alternativeConnectionString))
            {
                whereClause = GetWhereClause(dac, whereColumns, sqlInfo);

                if (selectColumns.Length == 0)
                    throw new Exception("No columns set for the select statement");

                string sql = string.Format("select top 1 {0} from {1} {2} {3}{4}",
                    selectColumns,
                    sqlInfo.SqlTable,
                    sqlInfo.SqlJoin,
                    whereClause, sqlInfo.ResultOrder
                    );

                SqlLastExecuted = sql;
                var start = DateTime.Now.Ticks;
                dac.SqlText = sql;
                SqlDataReader reader = dac.ExecReader;

                bool found = false;
                while (reader.Read())
                {
                    int index = 0;

                    found = true;
                    foreach (DatabaseColumnAttribute param in sqlParameters)
                    {
                        object value = GetData(reader, index, param);

                        if (param.Info.PropertyType == typeof(Sushi.Mediakiwi.Data.CustomData) && sqlInfo.PropertyListID.HasValue)
                        {
                            Sushi.Mediakiwi.Data.CustomData customData = SetPropertyValueCustomData(param.Info, value, entity);
                            foreach (Sushi.Mediakiwi.Data.Property p in propertyList)
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
                            SetPropertyValue(param.Info, value, entity);

                        index++;
                    }
                    CheckValidity(false, entity, sqlParameters);

                    //  If identity insert is turned on
                    if (primaryColumn.IdentityInsert)
                        ((IIdentity)entity).IsExistingInstance = true;
                }
                LoggingWrite(start, sql, sqlInfo);
                //  The selectOneby not found, please reset the key
                if (!found)
                {
                    //if (PrimairyKeyValue.HasValue && PrimairyKeyValue.Value > 0)
                    //{
                    if (primaryColumn != null)
                    {
                        SetPropertyValue(primaryColumn.Info, 0, entity);
                    }
                    //}
                }

                SqlLastExecutedWhereClause = dac.m_Parameterlist;
            }
            if (key != null)
                Caching.Add(key, entity);

            return (T)entity;
        }


        ///// <summary>
        ///// Gets the key by GUID.
        ///// </summary>
        ///// <param name="guid">The GUID.</param>
        ///// <returns></returns>
        //int GetKeyByGuid(Guid guid, object entity)
        //{
        //    var sqlParameters = GetSqlParameters(entity, sqlInfo);
        //    var primaryColumn = Primary(sqlParameters);
        //    var migrationColumn = Migration(sqlParameters);

        //    List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
        //    list.Add(new DatabaseDataValueColumn(migrationColumn.Column, SqlDbType.UniqueIdentifier, guid));

        //    using (Sushi.Mediakiwi.Data.Connection.SqlCommander dac = new Sushi.Mediakiwi.Data.Connection.SqlCommander(SqlConnectionString))
        //    {
        //        string sql = string.Format("select {3} from {0} {1} {2}",
        //            SqlTable,
        //            SqlJoin,
        //            GetWhereClause(dac, list), primaryColumn.Column
        //            );

        //        SqlLastExecuted = sql;
        //        dac.SqlText = sql;

        //        int key = 0;
        //        long start = DateTime.Now.Ticks;
        //        SqlDataReader reader = dac.ExecReader;

        //        while (reader.Read())
        //            key = reader.GetInt32(0);
        //        LoggingWrite(start, sql);
        //        return key;
        //    }
        //}


        /// <summary>
        /// Select all implementations based on the specified attributed.
        /// </summary>
        /// <param name="storedProcedure">The stored procedure.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public List<T> SelectAll<T>(string storedProcedure, IDataParameter[] parameters, string alternativeConnectionString = null)
        {
            SqlInformation sqlInfo;
            T entity = Init<T>(out sqlInfo);

            using (Sushi.Mediakiwi.Data.Connection.SqlCommander dac = new Sushi.Mediakiwi.Data.Connection.SqlCommander(string.IsNullOrEmpty(alternativeConnectionString) ? SqlConnectionString : alternativeConnectionString))
            {
                dac.Commandtype = CommandType.StoredProcedure;
                dac.SqlText = storedProcedure;

                var sqlParameters = GetSqlParameters(entity, sqlInfo);
                //var primaryColumn = Primary(sqlParameters);

                foreach (SqlParameter param in parameters)
                    dac.SetParameter(param.ParameterName, param.Value, param.SqlDbType, param.Size, param.Direction);

                long start = DateTime.Now.Ticks;
                SqlDataReader reader = dac.ExecReader;
                List<T> list = new List<T>();
                while (reader.Read())
                {
                    var clone = (T)Activator.CreateInstance(entity.GetType());
                    int index = 0;

                    foreach (DatabaseColumnAttribute param in sqlParameters)
                    {
                        object value = GetData(reader, index, param);

                        if (value == DBNull.Value)
                        {
                            SetPropertyValue(param.Info, null, clone);
                            continue;

                        }

                        SetPropertyValue(param.Info, value, clone);
                        index++;
                    }
                    CheckValidity<T>(false, clone, sqlParameters);

                    list.Add(clone);
                }
                SqlLastExecutedWhereClause = dac.m_Parameterlist;
                LoggingWrite(start, storedProcedure, sqlInfo);
                return list;
            }
        }

        /// <summary>
        /// Select all implementations based on the specified attributed.
        /// </summary>
        /// <param name="whereColumns">The where columns.</param>
        /// <param name="cacheReference">The Reference and the Value form a combined unique cachekey.</param>
        /// 
        /// <param name="componentListID">The component list ID.</param>
        /// <returns></returns>
        public List<T> SelectAll<T>(List<DatabaseDataValueColumn> whereColumns, string cacheReference = null, int? componentListID = null, string alternativeConnectionString = null
            , int? maxResults = null, string orderByColumn = null) where T : class
        {
            var key = CacheKey<T>(cacheReference, alternativeConnectionString);
            if (key != null)
            {
                List<T> outcome;
                if (Caching.IsCached<T>(key, out outcome))
                    return outcome;
            }

            SqlInformation sqlInfo;
            var entity = Init<T>(out sqlInfo);

            if (componentListID.HasValue)
            {
                var list2 = Sushi.Mediakiwi.Data.ComponentList.SelectOne(componentListID.Value);
                sqlInfo.SqlTable = list2.Catalog().Table;
                sqlInfo.SqlColumnPrefix = list2.Catalog().ColumnPrefix;
            }

            string whereClause = "";

            var sqlParameters = GetSqlParameters(entity, sqlInfo);

            Sushi.Mediakiwi.Data.Property[] propertyList = null;
            if (sqlInfo.PropertyListID.HasValue)
                propertyList = Sushi.Mediakiwi.Data.Property.SelectAll(sqlInfo.PropertyListID.Value, sqlInfo.PropertyListItemID, false, false, true, DatabaseMappingPortal);

            string selectColumns = GetSelectColumns(sqlParameters, entity, sqlInfo, propertyList);

            string order = sqlInfo.ResultOrder;
            if (!string.IsNullOrWhiteSpace(orderByColumn))
                order = " ORDER BY " + orderByColumn;

            using (Sushi.Mediakiwi.Data.Connection.SqlCommander dac = new Sushi.Mediakiwi.Data.Connection.SqlCommander(string.IsNullOrEmpty(alternativeConnectionString) ? SqlConnectionString : alternativeConnectionString))
            {
                whereClause = GetWhereClause(dac, whereColumns, sqlInfo);
                if (selectColumns.Length == 0) return new List<T>();

                string rowcount = null;
                if (sqlInfo.SqlRowCount != 0) rowcount = string.Concat("set rowcount ", sqlInfo.SqlRowCount, " ");

                string top = null;
                if (maxResults.HasValue) top = $"TOP({maxResults.Value})";

                string sqlText = string.Format("{5}select {7}{0} from {1} {2} {3}{6}{4}",
                    selectColumns,
                    sqlInfo.SqlTable,
                    sqlInfo.SqlJoin,
                    whereClause, order, rowcount, sqlInfo.ResultGroup,
                    top
                    );
                var result = SelectAll<T>(entity, sqlText, dac, sqlInfo, propertyList);
                if (key != null)
                    Caching.Add(key, result);
                return result;
            }

        }

        public List<T> SelectAll<T>(IDataRequest request = null) where T : class
        {
            var key = CacheKey<T>(request);
            if (key != null)
            {
                List<T> outcome;
                if (Caching.IsCached<T>(key, out outcome))
                    return outcome;
            }

            SqlInformation sqlInfo;
            var entity = Init<T>(out sqlInfo);

            string whereClause = "";

            var sqlParameters = GetSqlParameters(entity, sqlInfo);

            Sushi.Mediakiwi.Data.Property[] propertyList = null;
            if (sqlInfo.PropertyListID.HasValue)
                propertyList = Sushi.Mediakiwi.Data.Property.SelectAll(sqlInfo.PropertyListID.Value, sqlInfo.PropertyListItemID, false, false, true, DatabaseMappingPortal);

            string selectColumns = GetSelectColumns(sqlParameters, entity, sqlInfo, propertyList);

            //order by
            string order = sqlInfo.ResultOrder;
            if (request != null && !string.IsNullOrWhiteSpace(request.OrderByColumn))
                order = " ORDER BY " + request.OrderByColumn;

            using (Sushi.Mediakiwi.Data.Connection.SqlCommander dac = new Sushi.Mediakiwi.Data.Connection.SqlCommander(string.IsNullOrEmpty(request?.AlternativeConnectionString) ? SqlConnectionString : request.AlternativeConnectionString))
            {
                whereClause = GetWhereClause(dac, request, sqlParameters, sqlInfo);
                if (selectColumns.Length == 0) return new List<T>();

                string rowcount = null;
                if (sqlInfo.SqlRowCount != 0) rowcount = string.Concat("set rowcount ", sqlInfo.SqlRowCount, " ");

                string top = null;
                if (request != null && request.MaxResults.HasValue) top = $"TOP({request.MaxResults.Value})";

                //apply paging if supplied on data request
                //a count query is run first, and secondly a query to retrieve the data from the page
                //todo: make both queries run in one roundtrip
                string pagingOffset = null;
                if (request?.PagingData != null)
                {
                    using (Sushi.Mediakiwi.Data.Connection.SqlCommander dacCountQuery = new Sushi.Mediakiwi.Data.Connection.SqlCommander(string.IsNullOrEmpty(request?.AlternativeConnectionString) ? SqlConnectionString : request.AlternativeConnectionString))
                    {
                        string whereClauseCountQuery = GetWhereClause(dacCountQuery, request, sqlParameters, sqlInfo);
                        //create count query
                        string countQuery = $"{rowcount} select count(*) from {sqlInfo.SqlTable} {sqlInfo.SqlJoin} {whereClauseCountQuery}";
                        var count = SelectScalar<T>(countQuery, dacCountQuery, sqlInfo);
                        if (count is int)
                            request.PagingData.ResultCount = (int)count;
                    }
                    if (!request.PagingData.ShowAll)
                    {
                        //create offset query text 
                        //TODO: use parameters for this
                        pagingOffset = $"OFFSET {request.PagingData.CurrentPage * request.PagingData.PageSize} ROWS FETCH NEXT {request.PagingData.PageSize} ROWS ONLY";

                        //if paging is applied, the offset/fetch next construct is used. no need to apply TOP() in this case                    
                        top = null;
                    }
                }

                string sqlText = $"{rowcount} select {top}{selectColumns} from {sqlInfo.SqlTable} {sqlInfo.SqlJoin} {whereClause} {sqlInfo.ResultGroup} {order} {pagingOffset}";






                var result = SelectAll<T>(entity, sqlText, dac, sqlInfo, propertyList);
                if (key != null)
                    Caching.Add(key, result);
                return result;
            }

        }

        List<T> SelectAll<T>(T entity, string sqlText, Sushi.Mediakiwi.Data.Connection.SqlCommander dac, SqlInformation sqlInfo, Property[] propertyList = null)
        {
            var start = DateTime.Now.Ticks;

            List<T> list = new List<T>();

            SqlLastExecuted = sqlText;
            dac.SqlText = sqlText;

            var sqlParameters = GetSqlParameters(entity, sqlInfo);
            //var primaryColumn = Primary(sqlParameters);

            SqlDataReader reader = dac.ExecReader;
            while (reader.Read())
            {
                var clone = (T)Activator.CreateInstance(entity.GetType());
                int index = 0;
                foreach (DatabaseColumnAttribute param in sqlParameters)
                {
                    object value = GetData(reader, index, param);
                    if (param.Info.PropertyType == typeof(Sushi.Mediakiwi.Data.CustomData) && sqlInfo.PropertyListID.HasValue)
                    {

                        Sushi.Mediakiwi.Data.CustomData customData = SetPropertyValueCustomData(param.Info, value, clone);
                        foreach (Sushi.Mediakiwi.Data.Property p in propertyList)
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
                        SetPropertyValue(param.Info, value, clone);
                    }
                    index++;
                }
                CheckValidity(false, clone, sqlParameters);
                list.Add(clone);
            }
            LoggingWrite(start, sqlText, sqlInfo);
            return list;
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

            if (reader.HasColumn(param.Column))
                return reader[param.Column];
            return null;
        }

        /// <summary>
        /// Select all implementations based on the specified attributed.
        /// </summary>
        /// <returns></returns>
        protected List<T> SelectAll<T>(bool cacheResult = false, string alternativeConnectionString = null) where T : class
        {
            List<T> t;
            if (cacheResult)
                t = SelectAll<T>(null, "all", null, alternativeConnectionString);
            else
                t = SelectAll<T>(null, null, null, alternativeConnectionString);

            return t;
        }

        /// <summary>
        /// _s the select all.
        /// </summary>
        /// <param name="componentListID">The component list ID.</param>
        /// <returns></returns>
        public virtual List<T> SelectAll<T>(int componentListID, string alternativeConnectionString = null) where T : class
        {
            List<T> t = SelectAll<T>(null, null, componentListID, alternativeConnectionString);
            return t;
        }

        /// <summary>
        /// The value [*] can be used for a replacement value for all columns set by attributes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlText"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual List<T> ExecuteList<T>(string sqlText, IDataRequest request = null)
        {
            var key = CacheKey<T>(request);

            if (key != null)
            {
                List<T> outcome;
                if (Caching.IsCached<T>(key, out outcome))
                    return outcome;
            }

            SqlInformation sqlInfo;
            var entity = Init<T>(out sqlInfo);
            var sqlParameters = GetSqlParameters(entity, sqlInfo);

            if (sqlText.Contains("[*]"))
            {
                string selectColumns = GetSelectColumns(sqlParameters, entity, sqlInfo);
                sqlText = sqlText.Replace("[*]", selectColumns);
            }

            using (Sushi.Mediakiwi.Data.Connection.SqlCommander dac = new Sushi.Mediakiwi.Data.Connection.SqlCommander(string.IsNullOrEmpty(request?.AlternativeConnectionString) ? SqlConnectionString : request.AlternativeConnectionString))
            {
                sqlText = InjectWhereClause(sqlText, GetWhereClause(dac, request, sqlParameters, sqlInfo));
                //if (request?.SqlParameters != null)
                //{
                //    foreach (SqlParameter p in request.SqlParameters)
                //        dac.SetParameter(p);
                //}
                var result = SelectAll<T>(entity, sqlText, dac, sqlInfo);
                if (key != null)
                    Caching.Add(key, result);

                return result;
            }
        }

        string InjectWhereClause(string sql, string whereClause)
        {
            if (string.IsNullOrEmpty(whereClause))
                return sql;

            if (sql.IndexOf("order by", StringComparison.CurrentCultureIgnoreCase) > -1)
            {
                var split = sql.Split(new string[] { "order by" }, StringSplitOptions.RemoveEmptyEntries);
                return string.Concat(split[0], whereClause, " order by ", split[1]);
            }
            return string.Concat(sql, " ", whereClause);
        }

        public Exception LastException { get; set; }
        /// <summary>
        /// The value [*] can be used for a replacement value for all columns set by attributes
        /// </summary>
        /// <param name="sqlText"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual bool Execute(string sqlText, IDataRequest request = null)
        {
            Execute<object>(sqlText, request);
            return true;
        }

        /// <summary>
        /// The value [*] can be used for a replacement value for all columns set by attributes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlText"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual T Execute<T>(string sqlText, IDataRequest request = null)
        {
            var key = CacheKey<T>(request);

            if (key != null)
            {
                T outcome;
                if (Caching.IsCached<T>(key, out outcome))
                    return outcome;
            }

            SqlInformation sqlInfo;
            var entity = Init<T>(out sqlInfo);
            var sqlParameters = GetSqlParameters(entity, sqlInfo);

            if (sqlText.Contains("[*]"))
            {
                string selectColumns = GetSelectColumns(sqlParameters, entity, sqlInfo);
                sqlText = sqlText.Replace("[*]", selectColumns);
            }

            T result;
            using (Data.Connection.SqlCommander dac = new Data.Connection.SqlCommander(string.IsNullOrEmpty(request?.AlternativeConnectionString) ? SqlConnectionString : request.AlternativeConnectionString))
            {
                sqlText = InjectWhereClause(sqlText, GetWhereClause(dac, request, sqlParameters, sqlInfo));
                //if (request?.SqlParameters != null)
                //{
                //    foreach (SqlParameter p in request.SqlParameters)
                //        dac.SetParameter(p);
                //}
                result = SelectOne<T>(entity, sqlText, dac, sqlInfo);
                if (key != null)
                    Caching.Add(key, result);

                return result;

                //dac.SqlText = sqlText;
                //long start = DateTime.Now.Ticks;
                //result = (T)dac.ExecReader();
                //LoggingWrite(start, sqlText);                
                //if (key != null)
                //{
                //    Caching.Add(key, result);
                //}
                //return result;
            }
        }

        object SelectScalar<T>(string sqlText, Sushi.Mediakiwi.Data.Connection.SqlCommander dac, SqlInformation sqlInfo)
        {
            var start = DateTime.Now.Ticks;

            SqlLastExecuted = sqlText;
            dac.SqlText = sqlText;

            var result = dac.ExecScalar();
            LoggingWrite(start, sqlText, sqlInfo);
            return result;
        }

        async Task<object> SelectScalarAsync<T>(string sqlText, Sushi.Mediakiwi.Data.Connection.SqlCommander dac, SqlInformation sqlInfo)
        {
            var start = DateTime.Now.Ticks;

            SqlLastExecuted = sqlText;
            dac.SqlText = sqlText;

            var result = dac.ExecScalar();
            LoggingWrite(start, sqlText, sqlInfo);
            return result;
        }

        /// <summary>
        /// Inserts a collection of entities of type T using Sql Bulk Copy. The SqlDbType defined on the column attributes is ignored. The Sql Type is derived from the .NET type of the property.
        /// A list of supported types can be found here: https://msdn.microsoft.com/en-us/library/system.data.datacolumn.datatype(v=vs.110).aspx
        /// This method does support System.Transaction.TransactionScope
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <param name="identityInsert"></param>
        /// <param name="alternativeConnectionString"></param>
        /// <param name="sqlBulkCopyOptions"></param>
        public virtual void BulkInsert<T>(IEnumerable<T> entities, bool identityInsert = false, string alternativeConnectionString = null, SqlBulkCopyOptions sqlBulkCopyOptions = SqlBulkCopyOptions.Default)
        {
            if (entities == null || entities.Count() == 0)
                return;

            //sets the correct connection string and gets table mapping data from the class
            //there is also a dummy instance of the requested type created
            SqlInformation sqlInfo;
            var dummyEntity = Init<T>(out sqlInfo);

            //get column attributes
            var columnAttributes = GetSqlParameters(dummyEntity, sqlInfo);

            //create a datatable based on table
            var dataTable = new DataTable(sqlInfo.SqlTableChecked);
            var primaryKey = new List<DataColumn>();
            foreach (var columnAttribute in columnAttributes)
            {
                //create a datacolumn for each column attribute that is not read only
                //datatable does not use the SqlDbTypes but instead uses internal mapping to map .Net types

                if (!columnAttribute.IsOnlyRead)
                {
                    var column = new DataColumn();

                    column.AllowDBNull = columnAttribute.IsNullable;
                    if (columnAttribute.IsPrimaryKey)
                    {
                        //if we are going to auto increment, we can skip this this column                        
                        column.AutoIncrement = columnAttribute.IdentityInsert == false && identityInsert == false;
                        if (column.AutoIncrement)
                            continue;
                        primaryKey.Add(column);
                    }
                    column.ColumnName = columnAttribute.Column;

                    //nullable types are not supported, the underlying type needs to be provided 
                    var type = columnAttribute.Info.PropertyType;
                    var underlyingType = Nullable.GetUnderlyingType(type);
                    if (underlyingType != null)
                        type = underlyingType;
                    column.DataType = type;

                    if (columnAttribute.Length > 0 && column.DataType == typeof(string))
                        column.MaxLength = columnAttribute.Length;

                    dataTable.Columns.Add(column);
                }
            }
            //set the primary key for this table (composite primary keys are supported)
            dataTable.PrimaryKey = primaryKey.ToArray();

            //create rows in the datatable for each entity
            foreach (var entity in entities)
            {
                var row = dataTable.NewRow();
                foreach (var columnAttribute in columnAttributes)
                {
                    //set values in the row for each column (and only if the column exists in the table definition)
                    if (dataTable.Columns.Contains(columnAttribute.Column))
                    {
                        var value = GetPropertyValue(columnAttribute.Info, entity);
                        //if null, we must use DBNull
                        if (value == null)
                            value = DBNull.Value;
                        row[columnAttribute.Column] = value;
                    }
                }
                dataTable.Rows.Add(row);
            }

            //create a sql connection (this allows sqlBulkCopy to enlist in a transaction scope, because the sqlConnection automatically enlists when open is called)
            var start = DateTime.Now.Ticks;

            string connectionString = string.IsNullOrEmpty(alternativeConnectionString) ? SqlConnectionString : alternativeConnectionString;
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                //insert using sqlBulkCopy
                using (var bulkCopy = new SqlBulkCopy(sqlConnection, sqlBulkCopyOptions, null))
                {
                    //we need to explicitly define a column mapping, otherwise the ordinal position of the columns in the datatable is used instead of name
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        var column = dataTable.Columns[i];
                        bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                    }

                    bulkCopy.DestinationTableName = dataTable.TableName;
                    bulkCopy.WriteToServer(dataTable);
                }
            }

            LoggingWrite(start, $"Bulk insert of {entities.Count()} into {dataTable.TableName}", sqlInfo);
        }

        /// <summary>
        /// Insert an implementation record.
        /// </summary>
        /// <returns></returns>
        public virtual int Insert<T>(T entity, List<IDataParameter> parameterList = null, bool identityInsert = false, string alternativeConnectionString = null)
        {
            SqlInformation sqlInfo;
            entity = Init<T>(out sqlInfo, entity);

            string insertColumns = " ";
            string valuesColumns = "";
            string primaryParameter = "";
            string returnCall = null;
            DatabaseColumnAttribute primaryDataColumn = null;

            var sqlParameters = GetSqlParameters(entity, sqlInfo);
            //var primaryColumn = Primary(sqlParameters);

            using (Sushi.Mediakiwi.Data.Connection.SqlCommander dac = new Sushi.Mediakiwi.Data.Connection.SqlCommander(string.IsNullOrEmpty(alternativeConnectionString) ? SqlConnectionString : alternativeConnectionString))
            {
                DatabaseColumnAttribute customDataParam = null;

                foreach (DatabaseColumnAttribute param in sqlParameters)
                {
                    if (param.IsPrimaryKey)
                        primaryDataColumn = param;

                    if (!identityInsert && param.IsPrimaryKey && param.Column != null)
                    {
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
                            dac.SetParameterInput(string.Concat("@", param.Column), GetPropertyValue(param.Info, entity), param.SqlType, param.Length);
                        }
                    }
                }

                if (parameterList != null)
                {
                    foreach (SqlParameter p in parameterList)
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
                    Sushi.Mediakiwi.Data.CustomData data = customDataParam.Info.GetValue(entity, null) as Sushi.Mediakiwi.Data.CustomData;
                    if (data != null)
                    {
                        //Sushi.Mediakiwi.Data.CustomData clone = data.Clone();
                        if (sqlInfo.PropertyListID.HasValue)
                        {
                            var list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(sqlInfo.PropertyListID.Value);
                            Sushi.Mediakiwi.Data.Property[] properties = Sushi.Mediakiwi.Data.Property.SelectAll(sqlInfo.PropertyListID.Value, sqlInfo.PropertyListItemID, false, false, true, DatabaseMappingPortal);
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

                                }

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
                    sqlText = string.Format("insert into {0} DEFAULT VALUES", sqlInfo.SqlTableChecked);
                }
                else
                {
                    sqlText = string.Format("insert into {0} ({1}) values ({2}) {3}",
                        sqlInfo.SqlTableChecked,
                        insertColumns.Substring(0, insertColumns.Length - 2),
                        valuesColumns.Substring(0, valuesColumns.Length - 2),
                        returnCall);
                }
                if (string.IsNullOrEmpty(returnCall))
                    SqlLastExecuted = dac.ApplyParameters(sqlText);
                else
                    SqlLastExecuted = dac.ApplyParameters(sqlText.Replace(returnCall, string.Empty));

                dac.SqlText = sqlText;

                CheckValidity<T>(true, entity, sqlParameters);
                var start = DateTime.Now.Ticks;
                dac.ExecNonQuery();
                LoggingWrite(start, sqlText, sqlInfo);
                int primaryKey = 0;
                if (returnCall != null)
                {
                    primaryKey = dac.GetParamInt(primaryParameter);
                    primaryDataColumn.Info.SetValue(entity, primaryKey, null);
                }
                if (identityInsert)
                    primaryKey = (int)primaryDataColumn.Info.GetValue(entity);

                return primaryKey;
            }
        }

        /// <summary>
        /// Gets the where clause.
        /// </summary>
        /// <param name="dac">The dac.</param>
        /// <param name="whereColumns">The where columns.</param>
        /// <param name="isSelect">if set to <c>true</c> [is select].</param>
        /// <returns></returns>
        string GetWhereClause(Sushi.Mediakiwi.Data.Connection.SqlCommander dac, List<DatabaseDataValueColumn> whereColumns, SqlInformation sqlInfo, List<IDataParameter> parameterList = null, bool isSelect = true)
        {
            //  [20110725:MM] Validate NULL values
            if (whereColumns != null)
            {
                var count = (from item in whereColumns where item == null select item);
                if (count.Count() > 0)
                    whereColumns = (from item in whereColumns where item != null select item).ToList();
            }
            //  [20110725:MM:END]

            if (parameterList != null)
            {
                foreach (SqlParameter p in parameterList)
                    dac.SetParameterInput(p.ParameterName, p.Value, p.SqlDbType);
            }

            if (sqlInfo.IsGenericEntity && isSelect && sqlInfo.PropertyListID > 0)
            {
                if (whereColumns == null) whereColumns = new List<DatabaseDataValueColumn>();
                whereColumns.Add(new DatabaseDataValueColumn("<SQL_COL>_List_Key", SqlDbType.Int, sqlInfo.PropertyListID));
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
                    if (!string.IsNullOrEmpty(sqlInfo.SqlColumnPrefix) && column.Column.Contains("<SQL_COL>"))
                        column.Column = column.Column.Replace("<SQL_COL>", sqlInfo.SqlColumnPrefix);
                }
                else
                {
                    if (!string.IsNullOrEmpty(sqlInfo.SqlColumnPrefix) && column.SqlText.Contains("<SQL_COL>"))
                        column.SqlText = column.SqlText.Replace("<SQL_COL>", sqlInfo.SqlColumnPrefix);
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
                    if (column.DbColValue == null)
                    { }
                    else if (typeof(string[]) == column.DbColValue?.GetType())
                    {
                        var cast = (string[])column.DbColValue;
                        if (cast.Length > 0)
                            whereClause += string.Format("{0} in ({1})", column.Column, Wim.Utility.ConvertToCsvString(cast, true));
                    }
                    else if (typeof(int[]) == column.DbColValue?.GetType())
                    {
                        var cast = (int[])column.DbColValue;
                        if (cast.Length > 0)
                            whereClause += string.Format("{0} in ({1})", column.Column, Wim.Utility.ConvertToCsvString(cast));
                    }
                    else
                    {
                        whereClause += string.Format("{0} in ({1})", column.Column, column.DbColValue);
                    }
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

        /// <summary>
        /// Returns the SQL text for the where clause and defines the parameters in the where clause on the dac object
        /// </summary>
        /// <param name="dac"></param>
        /// <param name="request"></param>
        /// <param name="columns"></param>
        /// <param name="sqlInfo"></param>
        /// <param name="isSelect"></param>
        /// <returns></returns>
        string GetWhereClause(Sushi.Mediakiwi.Data.Connection.SqlCommander dac, IDataRequest request, DatabaseColumnAttribute[] columns, SqlInformation sqlInfo, bool isSelect = true)
        {
            if (request?.WhereClause != null)
            {
                foreach (var col in request?.WhereClause)
                {
                    if (col.Property == null)
                        continue;
                    var candidate = columns.Where(x => x.Info.Name == col.Property).FirstOrDefault();
                    if (candidate == null)
                        throw new Exception($"Could not find property {col.Property}");

                    col.Column = candidate.Column;
                    col.SqlType = candidate.SqlType;
                }
            }
            var whereColumns = request?.WhereClause;

            if (request?.SqlParameters != null)
            {
                foreach (SqlParameter p in request.SqlParameters)
                    dac.SetParameterInput(p.ParameterName, p.Value, p.SqlDbType);
            }

            if (sqlInfo.IsGenericEntity && isSelect && sqlInfo.PropertyListID > 0)
            {
                if (whereColumns == null) whereColumns = new List<DatabaseDataValueColumn>();
                whereColumns.Add(new DatabaseDataValueColumn("<SQL_COL>_List_Key", SqlDbType.Int, sqlInfo.PropertyListID));
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
                    if (!string.IsNullOrEmpty(sqlInfo.SqlColumnPrefix) && column.Column.Contains("<SQL_COL>"))
                        column.Column = column.Column.Replace("<SQL_COL>", sqlInfo.SqlColumnPrefix);
                }
                else
                {
                    if (!string.IsNullOrEmpty(sqlInfo.SqlColumnPrefix) && column.SqlText.Contains("<SQL_COL>"))
                        column.SqlText = column.SqlText.Replace("<SQL_COL>", sqlInfo.SqlColumnPrefix);
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
                    if (column.DbColValue == null)
                    { }
                    else if (typeof(string[]) == column.DbColValue?.GetType())
                        whereClause += string.Format("{0} in ({1})", column.Column, Wim.Utility.ConvertToCsvString((string[])column.DbColValue, true));
                    else if (typeof(int[]) == column.DbColValue?.GetType())
                        whereClause += string.Format("{0} in ({1})", column.Column, Wim.Utility.ConvertToCsvString((int[])column.DbColValue));
                    else
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
        string SqlLastExecutedWhereClause;

        string GetWhereClause(Sushi.Mediakiwi.Data.Connection.DataCommander dac, List<DatabaseDataValueColumn> whereColumns, SqlInformation sqlInfo)
        {
            return GetWhereClause(dac, whereColumns, true, sqlInfo);
        }

        /// <summary>
        /// Gets the where clause.
        /// </summary>
        /// <param name="dac">The dac.</param>
        /// <param name="whereColumns">The where columns.</param>
        /// <param name="isSelect">if set to <c>true</c> [is select].</param>
        /// <returns></returns>
        string GetWhereClause(Sushi.Mediakiwi.Data.Connection.DataCommander dac, List<DatabaseDataValueColumn> whereColumns, bool isSelect, SqlInformation sqlInfo)
        {
            if (sqlInfo.IsGenericEntity && isSelect && sqlInfo.PropertyListID > 0)
            {
                if (whereColumns == null) whereColumns = new List<DatabaseDataValueColumn>();
                whereColumns.Add(new DatabaseDataValueColumn("<SQL_COL>_List_Key", SqlDbType.Int, sqlInfo.PropertyListID));
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
                    if (!string.IsNullOrEmpty(sqlInfo.SqlColumnPrefix) && column.Column.Contains("<SQL_COL>"))
                        column.Column = column.Column.Replace("<SQL_COL>", sqlInfo.SqlColumnPrefix);

                }
                else
                {
                    if (!string.IsNullOrEmpty(sqlInfo.SqlColumnPrefix) && column.SqlText.Contains("<SQL_COL>"))
                        column.SqlText = column.SqlText.Replace("<SQL_COL>", sqlInfo.SqlColumnPrefix);
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
                    if (column.DbColValue == null)
                    { }
                    else if (typeof(string[]) == column.DbColValue?.GetType())
                    {
                        var cast = (string[])column.DbColValue;
                        if (cast.Length > 0)
                            whereClause += string.Format("{0} in ({1})", column.Column, Wim.Utility.ConvertToCsvString(cast, true));
                    }
                    else if (typeof(int[]) == column.DbColValue?.GetType())
                    {
                        var cast = (int[])column.DbColValue;
                        if (cast.Length > 0)
                            whereClause += string.Format("{0} in ({1})", column.Column, Wim.Utility.ConvertToCsvString(cast));
                    }
                    else
                    {
                        whereClause += string.Format("{0} in ({1})", column.Column, column.DbColValue);
                    }
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
        public int Update<T>(T entity, List<DatabaseDataValueColumn> whereColumns = null, List<IDataParameter> parameterList = null, string alternativeConnectionString = null)
        {
            SqlInformation sqlInfo;
            entity = Init<T>(out sqlInfo, entity);

            var sqlParameters = GetSqlParameters(entity, sqlInfo);
            var primaryColumn = Primary(sqlParameters);

            if (whereColumns == null)
                whereColumns = new List<DatabaseDataValueColumn>();

            if (primaryColumn != null)
                whereColumns.Add(new DatabaseDataValueColumn(primaryColumn.Column, SqlDbType.Int, PrimairyKeyValue(primaryColumn, entity)));

            if (whereColumns.Count == 0)
                throw new Exception("Cannot update entity, no primary key or custom where clause");


            string updateColumns = " ";
            string whereClause = "";


            using (Sushi.Mediakiwi.Data.Connection.SqlCommander dac = new Sushi.Mediakiwi.Data.Connection.SqlCommander(string.IsNullOrEmpty(alternativeConnectionString) ? SqlConnectionString : alternativeConnectionString))
            {
                DatabaseColumnAttribute customDataParam = null;

                foreach (DatabaseColumnAttribute param in sqlParameters)
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
                        dac.SetParameterInput(string.Concat("@", param.Column), GetPropertyValue(param.Info, entity), param.SqlType, param.Length);
                    }
                }

                if (parameterList != null)
                {
                    foreach (SqlParameter p in parameterList)
                    {
                        updateColumns += string.Concat(p.ParameterName, "= ", "@", p.ParameterName, ", ");
                        dac.SetParameterInput(string.Concat("@", p.ParameterName), p.Value, p.SqlDbType);
                    }
                }

                if (!string.IsNullOrEmpty(updateColumns))
                    updateColumns = updateColumns.ToLower();

                if (customDataParam != null)
                {
                    Sushi.Mediakiwi.Data.CustomData data = customDataParam.Info.GetValue(entity, null) as Sushi.Mediakiwi.Data.CustomData;

                    if (data != null)
                    {
                        //Sushi.Mediakiwi.Data.CustomData clone = data.Clone();
                        if (sqlInfo.PropertyListID.HasValue)
                        {
                            var list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(sqlInfo.PropertyListID.Value);
                            Sushi.Mediakiwi.Data.Property[] properties = Sushi.Mediakiwi.Data.Property.SelectAll(sqlInfo.PropertyListID.Value, sqlInfo.PropertyListItemID, false, false, true, DatabaseMappingPortal);
                            //Sushi.Mediakiwi.Data.Property.SelectAll(PropertyListID.Value, PropertyListItemID, false);
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

                                }

                            }
                        }
                        updateColumns += string.Concat(customDataParam.Column, "= ", "@", customDataParam.Column, ", ");
                        dac.SetParameterInput(string.Concat("@", customDataParam.Column), data.Serialized, customDataParam.SqlType, customDataParam.Length);
                    }
                }

                if (updateColumns.Length == 0) return 0;

                whereClause = GetWhereClause(dac, whereColumns, sqlInfo);

                string sqlText = string.Format("update {0} set {1} {2}",
                    sqlInfo.SqlTableChecked,
                    updateColumns.Substring(0, updateColumns.Length - 2),
                    whereClause);

                SqlLastExecuted = dac.ApplyParameters(sqlText);
                dac.SqlText = sqlText;

                CheckValidity<T>(true, entity, sqlParameters);
                var start = DateTime.Now.Ticks;
                dac.ExecNonQuery();
                LoggingWrite(start, sqlText, sqlInfo);

                return PrimairyKeyValue(primaryColumn, entity).GetValueOrDefault();
            }
        }

        /// <summary>
        /// Save the entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns>The primary key</returns>
        public int Save<T>(T entity, string alternativeConnectionString = null, bool flushCache = true)
        {
            SqlInformation sqlInfo;
            entity = Init<T>(out sqlInfo, entity);

            int key = 0;
            bool isNew = false;

            var sqlParameters = GetSqlParameters(entity, sqlInfo);
            var primaryColumn = Primary(sqlParameters);
            //var migrationColumn = Migration(sqlParameters);
            var primaryKey = PrimairyKeyValue(sqlParameters, entity);

            bool isUpdatable = (primaryKey.HasValue && primaryKey != 0);
            if (primaryColumn.IdentityInsert)
                isUpdatable = ((IIdentity)entity).IsExistingInstance;

            if (isUpdatable)
                key = Update(entity, null, null, alternativeConnectionString);
            else
            {
                isNew = true;
                key = Insert(entity, null, primaryColumn.IdentityInsert, alternativeConnectionString);
            }

            // [MR:17-11-2015] Dit was :
            // if (isNew && System.Web.HttpContext.Current?.Items["wim.Saved.ID"] == null)
            if (isNew && System.Web.HttpContext.Current?.Items.Contains("wim.Saved.ID") == false)
                System.Web.HttpContext.Current.Items["wim.Saved.ID"] = key;
            if (flushCache)
                FlushCache(entity);

            return key;
        }

        public void FlushCache(object entity)
        {
            string key = string.Concat("Data_", entity.GetType().ToString());
            Wim.Utilities.CacheItemManager.FlushIndexOfCacheObjects(key);
        }

        /// <summary>
        /// Set a property value.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="value">The value.</param>
        /// <param name="entity">The entity.</param>
        void SetPropertyValue<T>(PropertyInfo info, object value, T entity)
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

                else if (info.PropertyType == typeof(Sushi.Mediakiwi.Data.CustomData))
                {
                    Sushi.Mediakiwi.Data.CustomData tmp = new CustomData();
                    tmp.ApplySerialized(value.ToString());
                    value = tmp;
                }

                else if (info.PropertyType == typeof(Sushi.Mediakiwi.Data.AssetInfo))
                {
                    Sushi.Mediakiwi.Data.AssetInfo tmp = new AssetInfo();
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

            Sushi.Mediakiwi.Data.CustomData tmp = new CustomData();

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
        object GetPropertyValue(PropertyInfo info, object entity)
        {
            object value = info.GetValue(entity, null);

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
                System.TimeSpan tmp = (System.TimeSpan)info.GetValue(entity, null);
                value = tmp.ToString();
            }

            //  System.Uri
            else if (info.PropertyType == typeof(System.Uri))
            {
                System.Uri tmp = (System.Uri)info.GetValue(entity, null);
                value = tmp.ToString();
            }

            //else if (info.PropertyType == typeof(Data.ContentContainer))
            //{
            //    Data.ContentContainer tmp = (Data.ContentContainer)info.GetValue(this, null);
            //    if (tmp != null)
            //        value = tmp.Serialized;
            //}

            else if (info.PropertyType == typeof(Sushi.Mediakiwi.Data.CustomData))
            {
                Sushi.Mediakiwi.Data.CustomData tmp = (Sushi.Mediakiwi.Data.CustomData)info.GetValue(entity, null);
                if (tmp != null)
                    value = tmp.Serialized;
            }
            else if (info.PropertyType == typeof(Sushi.Mediakiwi.Data.AssetInfo))
            {
                Sushi.Mediakiwi.Data.AssetInfo tmp = (Sushi.Mediakiwi.Data.AssetInfo)info.GetValue(entity, null);
                if (tmp != null)
                    value = tmp.AssetID;
            }
            return value;
        }

        /// <summary>
        /// Delete an implementation record.
        /// </summary>
        /// <param name="whereColumns">The where columns.</param>
        /// <returns></returns>
        public bool Delete<T>(T entity, List<DatabaseDataValueColumn> whereColumns = null, string alternativeConnectionString = null)
        {
            SqlInformation sqlInfo;
            entity = Init<T>(out sqlInfo, entity);
            if (whereColumns == null)
                whereColumns = new List<DatabaseDataValueColumn>();

            var sqlParameters = GetSqlParameters(entity, sqlInfo);
            var primaryColumn = Primary(sqlParameters);
            //var migrationColumn = Migration(sqlParameters);

            if (primaryColumn != null)
                whereColumns.Add(new DatabaseDataValueColumn(primaryColumn.Column, SqlDbType.Int, PrimairyKeyValue(sqlParameters, entity)));

            if (whereColumns.Count == 0)
                throw new Exception("Cannot delete entity, no primary column and no custom where columns");

            string whereClause = "";
            using (Sushi.Mediakiwi.Data.Connection.SqlCommander dac = new Sushi.Mediakiwi.Data.Connection.SqlCommander(string.IsNullOrEmpty(alternativeConnectionString) ? SqlConnectionString : alternativeConnectionString))
            {
                whereClause = GetWhereClause(dac, whereColumns, sqlInfo, null, false);
                if (whereClause.Length == 0) return false;

                string sqlText = string.Format("delete from {0} {1}",
                    sqlInfo.SqlTableChecked,
                    whereClause);

                SqlLastExecuted = sqlText;

                dac.SqlText = sqlText;
                long start = DateTime.Now.Ticks;
                dac.ExecNonQuery();
                LoggingWrite(start, sqlText, sqlInfo);
                Wim.Utilities.CacheItemManager.FlushIndexOfCacheObjects(string.Concat("Data_", entity.GetType().ToString()));
                return true;
            }
        }

        /// <summary>
        /// Deletes all records mathcing the whereColumns
        /// </summary>
        /// <param name="whereColumns">The where columns.</param>
        /// <returns></returns>
        public bool Delete<T>(List<DatabaseDataValueColumn> whereColumns, string alternativeConnectionString = null)
        {
            SqlInformation sqlInfo;

            T entity = Init<T>(out sqlInfo);
            if (whereColumns == null)
                whereColumns = new List<DatabaseDataValueColumn>();

            var sqlParameters = GetSqlParameters(entity, sqlInfo);

            if (whereColumns.Count == 0)
                throw new Exception("Cannot delete entity, no custom where columns");

            string whereClause = "";
            using (Sushi.Mediakiwi.Data.Connection.SqlCommander dac = new Sushi.Mediakiwi.Data.Connection.SqlCommander(string.IsNullOrEmpty(alternativeConnectionString) ? SqlConnectionString : alternativeConnectionString))
            {
                whereClause = GetWhereClause(dac, whereColumns, sqlInfo, null, false);
                if (whereClause.Length == 0) return false;

                string sqlText = string.Format("delete from {0} {1}",
                    sqlInfo.SqlTableChecked,
                    whereClause);

                SqlLastExecuted = sqlText;

                dac.SqlText = sqlText;
                long start = DateTime.Now.Ticks;
                dac.ExecNonQuery();
                LoggingWrite(start, sqlText, sqlInfo);
                Wim.Utilities.CacheItemManager.FlushIndexOfCacheObjects(string.Concat("Data_", entity.GetType().ToString()));
                return true;
            }
        }

        private bool CheckValidity<T>(bool isDatabaseUpdate, T entity, DatabaseColumnAttribute[] sqlParameters)
        {
            foreach (DatabaseColumnAttribute param in sqlParameters)
            {
                if (!param.IsNullable && param.Info.GetValue(entity, null) == null)
                {
                    if (isDatabaseUpdate && param.IsOnlyRead) continue;
                    if (string.IsNullOrEmpty(param.ColumnSubQuery))
                        throw new Exception($"Property {param.Info.Name} with SQL column {param.Column} can not be set as nullable for the executed query");


                }
            }
            return true;
        }

        public T SelectOne<T>(bool cacheResult = false, string alternativeConnectionString = null)
        {
            SqlInformation sqlInfo;
            Init<T>(out sqlInfo);

            int key = 0;
            //  Added a additional ckey element as of the Filter options
            string ckey = null;
            if (cacheResult)
                ckey = sqlInfo.PropertyListItemID.HasValue ? string.Concat(key, "#", sqlInfo.PropertyListItemID) : key.ToString();

            T t;
            List<DatabaseDataValueColumn> where = null;

            t = SelectOne<T>(where, ckey, alternativeConnectionString);

            return t;
        }

        void LoggingWrite(long start, string sql, SqlInformation sqlInfo, bool hasResult = true)
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
                    total, save, sqlInfo.SqlTable)
                    );
            }

            object planArr = System.Web.HttpContext.Current.Items["wim.sqlexecutionPlan"];
            List<BaseSqlEntity.SqlExecutionInformation> planList =
                planArr == null
                    ? new List<BaseSqlEntity.SqlExecutionInformation>() : (List<BaseSqlEntity.SqlExecutionInformation>)planArr;

            if (!hasResult)
                this.SqlLastExecutedWhereClause += " [NO RESULT FOUND]";

            string stackTrace = null;
            if (Wim.CommonConfiguration.SQL_DEBUG_STACKTRACE)
                stackTrace = System.Environment.StackTrace;

            planList.Add(new BaseSqlEntity.SqlExecutionInformation("DB", sqlInfo.SqlTable, sql, this.SqlLastExecutedWhereClause, total, this.GetType().ToString(), stackTrace));

            System.Web.HttpContext.Current.Items["wim.sqlexecutionPlan"] = planList;
            System.Web.HttpContext.Current.Trace.Write("Sql-Execution", sql);
        }

        /// <summary>
        /// Async delete an implementation record.
        /// </summary>
        /// <param name="whereColumns">The where columns.</param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync<T>(T entity, List<DatabaseDataValueColumn> whereColumns = null, string alternativeConnectionString = null)
        {
            SqlInformation sqlInfo;
            entity = Init<T>(out sqlInfo, entity);
            if (whereColumns == null)
                whereColumns = new List<DatabaseDataValueColumn>();

            var sqlParameters = GetSqlParameters(entity, sqlInfo);
            var primaryColumn = Primary(sqlParameters);
            //var migrationColumn = Migration(sqlParameters);

            whereColumns.Add(new DatabaseDataValueColumn(primaryColumn.Column, SqlDbType.Int, PrimairyKeyValue(sqlParameters, entity)));

            string whereClause = "";
            using (Sushi.Mediakiwi.Data.Connection.SqlCommander dac = new Sushi.Mediakiwi.Data.Connection.SqlCommander(string.IsNullOrEmpty(alternativeConnectionString) ? SqlConnectionString : alternativeConnectionString))
            {
                whereClause = GetWhereClause(dac, whereColumns, sqlInfo, null, false);
                if (whereClause.Length == 0) return false;

                string sqlText = string.Format("delete from {0} {1}",
                    sqlInfo.SqlTableChecked,
                    whereClause);

                SqlLastExecuted = sqlText;

                dac.SqlText = sqlText;
                long start = DateTime.Now.Ticks;
                await dac.ExecNonQueryAsync();
                LoggingWrite(start, sqlText, sqlInfo);
                Wim.Utilities.CacheItemManager.FlushIndexOfCacheObjects(string.Concat("Data_", entity.GetType().ToString()));
                return true;
            }
        }

        /// <summary>
        /// Deletes all records mathcing the whereColumns
        /// </summary>
        /// <param name="whereColumns">The where columns.</param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync<T>(List<DatabaseDataValueColumn> whereColumns, string alternativeConnectionString = null)
        {
            SqlInformation sqlInfo;

            T entity = Init<T>(out sqlInfo);
            if (whereColumns == null)
                whereColumns = new List<DatabaseDataValueColumn>();

            var sqlParameters = GetSqlParameters(entity, sqlInfo);

            if (whereColumns.Count == 0)
                throw new Exception("Cannot delete entity, no custom where columns");

            string whereClause = "";
            using (Sushi.Mediakiwi.Data.Connection.SqlCommander dac = new Sushi.Mediakiwi.Data.Connection.SqlCommander(string.IsNullOrEmpty(alternativeConnectionString) ? SqlConnectionString : alternativeConnectionString))
            {
                whereClause = GetWhereClause(dac, whereColumns, sqlInfo, null, false);
                if (whereClause.Length == 0) return false;

                string sqlText = string.Format("delete from {0} {1}",
                    sqlInfo.SqlTableChecked,
                    whereClause);

                SqlLastExecuted = sqlText;

                dac.SqlText = sqlText;
                long start = DateTime.Now.Ticks;
                await dac.ExecNonQueryAsync();
                LoggingWrite(start, sqlText, sqlInfo);
                Wim.Utilities.CacheItemManager.FlushIndexOfCacheObjects(string.Concat("Data_", entity.GetType().ToString()));
                return true;
            }
        }

        public async Task<bool> ExecuteAsync(string sqlText, IDataRequest request = null)
        {
            await ExecuteAsync<object>(sqlText, request);
            return true;
        }

        public async Task<T> ExecuteAsync<T>(string sqlText, IDataRequest request = null)
        {
            var key = CacheKey<T>(request);

            if (key != null)
            {
                T outcome;
                if (Caching.IsCached<T>(key, out outcome))
                    return outcome;
            }

            SqlInformation sqlInfo;
            var entity = Init<T>(out sqlInfo);
            var sqlParameters = GetSqlParameters(entity, sqlInfo);

            if (sqlText.Contains("[*]"))
            {
                string selectColumns = GetSelectColumns(sqlParameters, entity, sqlInfo);
                sqlText = sqlText.Replace("[*]", selectColumns);
            }

            T result;
            using (Data.Connection.SqlCommander dac = new Data.Connection.SqlCommander(string.IsNullOrEmpty(request?.AlternativeConnectionString) ? SqlConnectionString : request.AlternativeConnectionString))
            {
                sqlText = InjectWhereClause(sqlText, GetWhereClause(dac, request, sqlParameters, sqlInfo));

                result = await SelectOneAsync<T>(entity, sqlText, dac, sqlInfo);
                if (key != null)
                    Caching.Add(key, result);

                return result;


            }
        }

        async Task<T> SelectOneAsync<T>(T entity, string sqlText, Sushi.Mediakiwi.Data.Connection.SqlCommander dac, SqlInformation sqlInfo, Property[] propertyList = null)
        {
            var start = DateTime.Now.Ticks;

            SqlLastExecuted = sqlText;
            dac.SqlText = sqlText;

            var sqlParameters = GetSqlParameters(entity, sqlInfo);
            //var primaryColumn = Primary(sqlParameters);

            SqlDataReader reader = dac.ExecReader;
            LoggingWrite(start, sqlText, sqlInfo);
            while (await reader.ReadAsync())
            {
                var clone = (T)Activator.CreateInstance(entity.GetType());
                int index = 0;

                if (sqlParameters.Length == 0)
                    clone = (T)reader[0];

                foreach (DatabaseColumnAttribute param in sqlParameters)
                {
                    object value = GetData(reader, index, param);
                    if (param.Info.PropertyType == typeof(Sushi.Mediakiwi.Data.CustomData) && sqlInfo.PropertyListID.HasValue)
                    {

                        Sushi.Mediakiwi.Data.CustomData customData = SetPropertyValueCustomData(param.Info, value, clone);
                        foreach (Sushi.Mediakiwi.Data.Property p in propertyList)
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
                        SetPropertyValue(param.Info, value, clone);
                    }
                    index++;
                }
                CheckValidity(false, clone, sqlParameters);

                return clone;
            }
            return entity;
        }

        public async Task<List<T>> ExecuteListAsync<T>(string sqlText, IDataRequest request = null)
        {
            var key = CacheKey<T>(request);

            if (key != null)
            {
                List<T> outcome;
                if (Caching.IsCached<T>(key, out outcome))
                    return outcome;
            }

            SqlInformation sqlInfo;
            var entity = Init<T>(out sqlInfo);
            var sqlParameters = GetSqlParameters(entity, sqlInfo);

            if (sqlText.Contains("[*]"))
            {
                string selectColumns = GetSelectColumns(sqlParameters, entity, sqlInfo);
                sqlText = sqlText.Replace("[*]", selectColumns);
            }

            using (Sushi.Mediakiwi.Data.Connection.SqlCommander dac = new Sushi.Mediakiwi.Data.Connection.SqlCommander(string.IsNullOrEmpty(request?.AlternativeConnectionString) ? SqlConnectionString : request.AlternativeConnectionString))
            {
                sqlText = InjectWhereClause(sqlText, GetWhereClause(dac, request, sqlParameters, sqlInfo));

                var result = await SelectAllAsync<T>(entity, sqlText, dac, sqlInfo);
                if (key != null)
                    Caching.Add(key, result);

                return result;
            }
        }

        public async Task<List<T>> SelectAllAsync<T>(int componentListID, string alternativeConnectionString = null) where T : class
        {
            List<T> t = await SelectAllAsync<T>(null, null, componentListID, alternativeConnectionString);
            return t;
        }

        public async Task<List<T>> SelectAllAsync<T>(string storedProcedure, IDataParameter[] parameters, string alternativeConnectionString = null)
        {
            SqlInformation sqlInfo;
            T entity = Init<T>(out sqlInfo);

            using (Sushi.Mediakiwi.Data.Connection.SqlCommander dac = new Sushi.Mediakiwi.Data.Connection.SqlCommander(string.IsNullOrEmpty(alternativeConnectionString) ? SqlConnectionString : alternativeConnectionString))
            {
                dac.Commandtype = CommandType.StoredProcedure;
                dac.SqlText = storedProcedure;

                var sqlParameters = GetSqlParameters(entity, sqlInfo);
                //var primaryColumn = Primary(sqlParameters);

                foreach (SqlParameter param in parameters)
                    dac.SetParameter(param.ParameterName, param.Value, param.SqlDbType, param.Size, param.Direction);

                long start = DateTime.Now.Ticks;
                SqlDataReader reader = dac.ExecReader;
                List<T> list = new List<T>();
                while (await reader.ReadAsync())
                {
                    var clone = (T)Activator.CreateInstance(entity.GetType());
                    int index = 0;

                    foreach (DatabaseColumnAttribute param in sqlParameters)
                    {
                        object value = GetData(reader, index, param);

                        if (value == DBNull.Value)
                        {
                            SetPropertyValue(param.Info, null, clone);
                            continue;

                        }

                        SetPropertyValue(param.Info, value, clone);
                        index++;
                    }
                    CheckValidity<T>(false, clone, sqlParameters);

                    list.Add(clone);
                }
                SqlLastExecutedWhereClause = dac.m_Parameterlist;
                LoggingWrite(start, storedProcedure, sqlInfo);
                return list;
            }
        }

        public async Task<List<T>> SelectAllAsync<T>(List<DatabaseDataValueColumn> whereColumns, string cacheReference = null, int? componentListID = default(int?), string alternativeConnectionString = null, int? maxResults = default(int?), string orderByColumn = null) where T : class
        {
            var key = CacheKey<T>(cacheReference, alternativeConnectionString);
            if (key != null)
            {
                List<T> outcome;
                if (Caching.IsCached<T>(key, out outcome))
                    return outcome;
            }

            SqlInformation sqlInfo;
            var entity = Init<T>(out sqlInfo);

            if (componentListID.HasValue)
            {
                var list2 = Sushi.Mediakiwi.Data.ComponentList.SelectOne(componentListID.Value);
                sqlInfo.SqlTable = list2.Catalog().Table;
                sqlInfo.SqlColumnPrefix = list2.Catalog().ColumnPrefix;
            }

            string whereClause = "";

            var sqlParameters = GetSqlParameters(entity, sqlInfo);

            Sushi.Mediakiwi.Data.Property[] propertyList = null;
            if (sqlInfo.PropertyListID.HasValue)
                propertyList = Sushi.Mediakiwi.Data.Property.SelectAll(sqlInfo.PropertyListID.Value, sqlInfo.PropertyListItemID, false, false, true, DatabaseMappingPortal);

            string selectColumns = GetSelectColumns(sqlParameters, entity, sqlInfo, propertyList);

            string order = sqlInfo.ResultOrder;
            if (!string.IsNullOrWhiteSpace(orderByColumn))
                order = " ORDER BY " + orderByColumn;

            using (Sushi.Mediakiwi.Data.Connection.SqlCommander dac = new Sushi.Mediakiwi.Data.Connection.SqlCommander(string.IsNullOrEmpty(alternativeConnectionString) ? SqlConnectionString : alternativeConnectionString))
            {
                whereClause = GetWhereClause(dac, whereColumns, sqlInfo);
                if (selectColumns.Length == 0) return new List<T>();

                string rowcount = null;
                if (sqlInfo.SqlRowCount != 0) rowcount = string.Concat("set rowcount ", sqlInfo.SqlRowCount, " ");

                string top = null;
                if (maxResults.HasValue) top = $"TOP({maxResults.Value})";

                string sqlText = string.Format("{5}select {7}{0} from {1} {2} {3}{6}{4}",
                    selectColumns,
                    sqlInfo.SqlTable,
                    sqlInfo.SqlJoin,
                    whereClause, order, rowcount, sqlInfo.ResultGroup,
                    top
                    );
                var result = await SelectAllAsync<T>(entity, sqlText, dac, sqlInfo, propertyList);
                if (key != null)
                    Caching.Add(key, result);
                return result;
            }

        }

        public async Task<T> SelectOneAsync<T>(bool cacheResult = false, string alternativeConnectionString = null)
        {
            SqlInformation sqlInfo;
            Init<T>(out sqlInfo);

            int key = 0;
            //  Added a additional ckey element as of the Filter options
            string ckey = sqlInfo.PropertyListItemID.HasValue ? string.Concat(key, "#", sqlInfo.PropertyListItemID) : key.ToString();

            T t;
            List<DatabaseDataValueColumn> where = null;
            if (cacheResult)
                t = await SelectOneAsync<T>(where, ckey);
            else
                t = await SelectOneAsync<T>(where);

            return t;
        }

        public async Task<T> SelectOneAsync<T>(int key, bool cacheResult, string alternativeConnectionString = null)
        {
            return await SelectOneAsync<T>(key, cacheResult, null, alternativeConnectionString);
        }

        public async Task<T> SelectOneAsync<T>(List<DatabaseDataValueColumn> whereColumns, string cacheReference = null, string alternativeConnectionString = null)
        {
            var key = CacheKey<T>(cacheReference, alternativeConnectionString);

            if (key != null)
            {
                T outcome;
                if (Caching.IsCached<T>(key, out outcome))
                    return outcome;
            }
            SqlInformation sqlInfo;
            var entity = Init<T>(out sqlInfo);
            var type = entity.GetType();

            string whereClause = "";

            var sqlParameters = GetSqlParameters(entity, sqlInfo);
            var primaryColumn = Primary(sqlParameters);

            if (primaryColumn.IdentityInsert)
            {
                //  Custom set identifier, so verify the required interface implementation.
                if (entity is IIdentity)
                {
                    //  OK
                }
                else
                {
                    throw new Exception($"The entity {entity.ToString()} requires the implementation of IIdentity");
                }
            }



            Sushi.Mediakiwi.Data.Property[] propertyList = null;
            if (sqlInfo.PropertyListID.HasValue)
                propertyList = Sushi.Mediakiwi.Data.Property.SelectAll(sqlInfo.PropertyListID.Value, sqlInfo.PropertyListItemID, false, false, true, DatabaseMappingPortal);

            string selectColumns = GetSelectColumns(sqlParameters, entity, sqlInfo, propertyList);

            using (Sushi.Mediakiwi.Data.Connection.SqlCommander dac = new Sushi.Mediakiwi.Data.Connection.SqlCommander(string.IsNullOrEmpty(alternativeConnectionString) ? SqlConnectionString : alternativeConnectionString))
            {
                whereClause = GetWhereClause(dac, whereColumns, sqlInfo);

                if (selectColumns.Length == 0)
                    throw new Exception("No columns set for the select statement");

                string sql = string.Format("select top 1 {0} from {1} {2} {3}{4}",
                    selectColumns,
                    sqlInfo.SqlTable,
                    sqlInfo.SqlJoin,
                    whereClause, sqlInfo.ResultOrder
                    );

                SqlLastExecuted = sql;
                var start = DateTime.Now.Ticks;
                dac.SqlText = sql;
                SqlDataReader reader = dac.ExecReader;

                bool found = false;
                while (await reader.ReadAsync())
                {
                    int index = 0;

                    found = true;
                    foreach (DatabaseColumnAttribute param in sqlParameters)
                    {
                        object value = GetData(reader, index, param);

                        if (param.Info.PropertyType == typeof(Sushi.Mediakiwi.Data.CustomData) && sqlInfo.PropertyListID.HasValue)
                        {
                            Sushi.Mediakiwi.Data.CustomData customData = SetPropertyValueCustomData(param.Info, value, entity);
                            foreach (Sushi.Mediakiwi.Data.Property p in propertyList)
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
                            SetPropertyValue(param.Info, value, entity);

                        index++;
                    }
                    CheckValidity(false, entity, sqlParameters);

                    //  If identity insert is turned on
                    if (primaryColumn.IdentityInsert)
                        ((IIdentity)entity).IsExistingInstance = true;
                }
                LoggingWrite(start, sql, sqlInfo);
                //  The selectOneby not found, please reset the key
                if (!found)
                {
                    //if (PrimairyKeyValue.HasValue && PrimairyKeyValue.Value > 0)
                    //{
                    if (primaryColumn != null)
                    {
                        SetPropertyValue(primaryColumn.Info, 0, entity);
                    }
                    //}
                }

                SqlLastExecutedWhereClause = dac.m_Parameterlist;
            }
            if (key != null)
                Caching.Add(key, entity);

            return (T)entity;
        }

        public async Task<T> SelectOneAsync<T>(Guid key, bool cacheResult = false, string alternativeConnectionString = null)
        {
            SqlInformation sqlInfo;
            var entity = Init<T>(out sqlInfo);
            if (key == Guid.Empty) return (T)entity;

            var sqlcol = GetSqlParameters(entity, sqlInfo);
            var column = Migration(sqlcol);

            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            //[MR:13-11-2015] A Guid was being casted to an Integer :
            //list.Add(new DatabaseDataValueColumn(column.Column, SqlDbType.Int, key));
            list.Add(new DatabaseDataValueColumn(column.Column, SqlDbType.UniqueIdentifier, key));

            //  Added a additional ckey element as of the Filter options
            string ckey = sqlInfo.PropertyListItemID.HasValue ? string.Concat(key, "#", sqlInfo.PropertyListItemID) : key.ToString();

            T t;
            if (cacheResult)
                t = await SelectOneAsync<T>(list, "MID", ckey);
            else
                t = await SelectOneAsync<T>(list);

            return t;
        }

        public async Task<T> SelectOneAsync<T>(int key, bool cacheResult, int? componentListID, string alternativeConnectionString = null)
        {
            SqlInformation sqlInfo;
            var entity = Init<T>(out sqlInfo);
            if (key == 0) return (T)entity;

            if (componentListID.HasValue)
            {
                var list2 = Sushi.Mediakiwi.Data.ComponentList.SelectOne(componentListID.Value);
                sqlInfo.SqlTable = list2.Catalog().Table;
                sqlInfo.SqlColumnPrefix = list2.Catalog().ColumnPrefix;
            }

            var sqlcol = GetSqlParameters(entity, sqlInfo);
            var column = Primary(sqlcol);

            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            list.Add(new DatabaseDataValueColumn(column.Column, SqlDbType.Int, key));

            //  Added a additional ckey element as of the Filter options
            string ckey = sqlInfo.PropertyListItemID.HasValue ? string.Concat(key, "#", sqlInfo.PropertyListItemID) : key.ToString();

            T t;
            if (cacheResult)
                t = await SelectOneAsync<T>(list, ckey);
            else
                t = await SelectOneAsync<T>(list);

            return t;
        }

        /// <summary>
        /// Save the entity async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns>The primary key</returns>
        public async Task<int> SaveAsync<T>(T entity, string alternativeConnectionString = null, bool flushCache = true)
        {
            SqlInformation sqlInfo;
            entity = Init<T>(out sqlInfo, entity);

            int key = 0;
            bool isNew = false;

            var sqlParameters = GetSqlParameters(entity, sqlInfo);
            var primaryColumn = Primary(sqlParameters);
            //var migrationColumn = Migration(sqlParameters);
            var primaryKey = PrimairyKeyValue(sqlParameters, entity);

            bool isUpdatable = (primaryKey.HasValue && primaryKey != 0);
            if (primaryColumn.IdentityInsert)
                isUpdatable = ((IIdentity)entity).IsExistingInstance;

            if (isUpdatable)
                key = await UpdateAsync(entity, null, null, alternativeConnectionString);
            else
            {
                isNew = true;
                key = await InsertAsync(entity, null, primaryColumn.IdentityInsert, alternativeConnectionString);
            }

            // [MR:17-11-2015] Dit was :
            // if (isNew && System.Web.HttpContext.Current?.Items["wim.Saved.ID"] == null)
            if (isNew && System.Web.HttpContext.Current?.Items.Contains("wim.Saved.ID") == false)
                System.Web.HttpContext.Current.Items["wim.Saved.ID"] = key;
            if (flushCache)
                FlushCache(entity);

            return key;
        }

        /// <summary>
        /// Insert an implementation record async.
        /// </summary>
        /// <returns></returns>
        public virtual async Task<int> InsertAsync<T>(T entity, List<IDataParameter> parameterList = null, bool identityInsert = false, string alternativeConnectionString = null)
        {
            SqlInformation sqlInfo;
            entity = Init<T>(out sqlInfo, entity);

            string insertColumns = " ";
            string valuesColumns = "";
            string primaryParameter = "";
            string returnCall = null;
            DatabaseColumnAttribute primaryDataColumn = null;

            var sqlParameters = GetSqlParameters(entity, sqlInfo);
            //var primaryColumn = Primary(sqlParameters);

            using (Sushi.Mediakiwi.Data.Connection.SqlCommander dac = new Sushi.Mediakiwi.Data.Connection.SqlCommander(string.IsNullOrEmpty(alternativeConnectionString) ? SqlConnectionString : alternativeConnectionString))
            {
                DatabaseColumnAttribute customDataParam = null;

                foreach (DatabaseColumnAttribute param in sqlParameters)
                {
                    if (param.IsPrimaryKey)
                        primaryDataColumn = param;

                    if (!identityInsert && param.IsPrimaryKey && param.Column != null)
                    {
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
                            dac.SetParameterInput(string.Concat("@", param.Column), GetPropertyValue(param.Info, entity), param.SqlType, param.Length);
                        }
                    }
                }

                if (parameterList != null)
                {
                    foreach (SqlParameter p in parameterList)
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
                    Sushi.Mediakiwi.Data.CustomData data = customDataParam.Info.GetValue(entity, null) as Sushi.Mediakiwi.Data.CustomData;
                    if (data != null)
                    {
                        //Sushi.Mediakiwi.Data.CustomData clone = data.Clone();
                        if (sqlInfo.PropertyListID.HasValue)
                        {
                            var list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(sqlInfo.PropertyListID.Value);
                            Sushi.Mediakiwi.Data.Property[] properties = Sushi.Mediakiwi.Data.Property.SelectAll(sqlInfo.PropertyListID.Value, sqlInfo.PropertyListItemID, false, false, true, DatabaseMappingPortal);
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

                                }

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
                    sqlText = string.Format("insert into {0} DEFAULT VALUES", sqlInfo.SqlTableChecked);
                }
                else
                {
                    sqlText = string.Format("insert into {0} ({1}) values ({2}) {3}",
                        sqlInfo.SqlTableChecked,
                        insertColumns.Substring(0, insertColumns.Length - 2),
                        valuesColumns.Substring(0, valuesColumns.Length - 2),
                        returnCall);
                }
                if (string.IsNullOrEmpty(returnCall))
                    SqlLastExecuted = dac.ApplyParameters(sqlText);
                else
                    SqlLastExecuted = dac.ApplyParameters(sqlText.Replace(returnCall, string.Empty));

                dac.SqlText = sqlText;

                CheckValidity<T>(true, entity, sqlParameters);
                var start = DateTime.Now.Ticks;
                await dac.ExecNonQueryAsync();
                LoggingWrite(start, sqlText, sqlInfo);
                int primaryKey = 0;
                if (returnCall != null)
                {
                    primaryKey = dac.GetParamInt(primaryParameter);
                    primaryDataColumn.Info.SetValue(entity, primaryKey, null);
                }
                if (identityInsert)
                    primaryKey = (int)primaryDataColumn.Info.GetValue(entity);

                return primaryKey;
            }
        }

        /// <summary>
        /// Update an implementaion record Asyn
        /// </summary>
        /// <param name="whereColumns">The where columns.</param>
        /// <returns></returns>
        public async Task<int> UpdateAsync<T>(T entity, List<DatabaseDataValueColumn> whereColumns = null, List<IDataParameter> parameterList = null, string alternativeConnectionString = null)
        {
            SqlInformation sqlInfo;
            entity = Init<T>(out sqlInfo, entity);

            var sqlParameters = GetSqlParameters(entity, sqlInfo);
            var primaryColumn = Primary(sqlParameters);

            if (whereColumns == null)
                whereColumns = new List<DatabaseDataValueColumn>();

            whereColumns.Add(new DatabaseDataValueColumn(primaryColumn.Column, SqlDbType.Int, PrimairyKeyValue(primaryColumn, entity)));


            string updateColumns = " ";
            string whereClause = "";


            using (Sushi.Mediakiwi.Data.Connection.SqlCommander dac = new Sushi.Mediakiwi.Data.Connection.SqlCommander(string.IsNullOrEmpty(alternativeConnectionString) ? SqlConnectionString : alternativeConnectionString))
            {
                DatabaseColumnAttribute customDataParam = null;

                foreach (DatabaseColumnAttribute param in sqlParameters)
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
                        dac.SetParameterInput(string.Concat("@", param.Column), GetPropertyValue(param.Info, entity), param.SqlType, param.Length);
                    }
                }

                if (parameterList != null)
                {
                    foreach (SqlParameter p in parameterList)
                    {
                        updateColumns += string.Concat(p.ParameterName, "= ", "@", p.ParameterName, ", ");
                        dac.SetParameterInput(string.Concat("@", p.ParameterName), p.Value, p.SqlDbType);
                    }
                }

                if (!string.IsNullOrEmpty(updateColumns))
                    updateColumns = updateColumns.ToLower();

                if (customDataParam != null)
                {
                    Sushi.Mediakiwi.Data.CustomData data = customDataParam.Info.GetValue(entity, null) as Sushi.Mediakiwi.Data.CustomData;

                    if (data != null)
                    {
                        //Sushi.Mediakiwi.Data.CustomData clone = data.Clone();
                        if (sqlInfo.PropertyListID.HasValue)
                        {
                            var list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(sqlInfo.PropertyListID.Value);
                            Sushi.Mediakiwi.Data.Property[] properties = Sushi.Mediakiwi.Data.Property.SelectAll(sqlInfo.PropertyListID.Value, sqlInfo.PropertyListItemID, false, false, true, DatabaseMappingPortal);
                            //Sushi.Mediakiwi.Data.Property.SelectAll(PropertyListID.Value, PropertyListItemID, false);
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

                                }

                            }
                        }
                        updateColumns += string.Concat(customDataParam.Column, "= ", "@", customDataParam.Column, ", ");
                        dac.SetParameterInput(string.Concat("@", customDataParam.Column), data.Serialized, customDataParam.SqlType, customDataParam.Length);
                    }
                }

                if (updateColumns.Length == 0) return 0;

                whereClause = GetWhereClause(dac, whereColumns, sqlInfo);

                string sqlText = string.Format("update {0} set {1} {2}",
                    sqlInfo.SqlTableChecked,
                    updateColumns.Substring(0, updateColumns.Length - 2),
                    whereClause);

                SqlLastExecuted = dac.ApplyParameters(sqlText);
                dac.SqlText = sqlText;

                CheckValidity<T>(true, entity, sqlParameters);
                var start = DateTime.Now.Ticks;
                await dac.ExecNonQueryAsync();
                LoggingWrite(start, sqlText, sqlInfo);

                return PrimairyKeyValue(primaryColumn, entity).GetValueOrDefault();
            }
        }

        public async Task<List<T>> SelectAllAsync<T>(IDataRequest request = null) where T : class
        {

            var key = CacheKey<T>(request);
            if (key != null)
            {
                List<T> outcome;
                if (Caching.IsCached<T>(key, out outcome))
                    return outcome;
            }

            SqlInformation sqlInfo;
            var entity = Init<T>(out sqlInfo);

            string whereClause = "";

            var sqlParameters = GetSqlParameters(entity, sqlInfo);

            Sushi.Mediakiwi.Data.Property[] propertyList = null;
            if (sqlInfo.PropertyListID.HasValue)
                propertyList = Sushi.Mediakiwi.Data.Property.SelectAll(sqlInfo.PropertyListID.Value, sqlInfo.PropertyListItemID, false, false, true, DatabaseMappingPortal);

            string selectColumns = GetSelectColumns(sqlParameters, entity, sqlInfo, propertyList);

            //order by
            string order = sqlInfo.ResultOrder;
            if (request != null && !string.IsNullOrWhiteSpace(request.OrderByColumn))
                order = " ORDER BY " + request.OrderByColumn;

            using (Sushi.Mediakiwi.Data.Connection.SqlCommander dac = new Sushi.Mediakiwi.Data.Connection.SqlCommander(string.IsNullOrEmpty(request?.AlternativeConnectionString) ? SqlConnectionString : request.AlternativeConnectionString))
            {
                whereClause = GetWhereClause(dac, request, sqlParameters, sqlInfo);
                if (selectColumns.Length == 0) return new List<T>();

                string rowcount = null;
                if (sqlInfo.SqlRowCount != 0) rowcount = string.Concat("set rowcount ", sqlInfo.SqlRowCount, " ");

                string top = null;
                if (request != null && request.MaxResults.HasValue) top = $"TOP({request.MaxResults.Value})";

                //apply paging if supplied on data request
                //a count query is run first, and secondly a query to retrieve the data from the page
                //todo: make both queries run in one roundtrip
                string pagingOffset = null;
                if (request?.PagingData != null)
                {
                    using (Sushi.Mediakiwi.Data.Connection.SqlCommander dacCountQuery = new Sushi.Mediakiwi.Data.Connection.SqlCommander(string.IsNullOrEmpty(request?.AlternativeConnectionString) ? SqlConnectionString : request.AlternativeConnectionString))
                    {
                        string whereClauseCountQuery = GetWhereClause(dacCountQuery, request, sqlParameters, sqlInfo);
                        //create count query
                        string countQuery = $"{rowcount} select count(*) from {sqlInfo.SqlTable} {sqlInfo.SqlJoin} {whereClauseCountQuery}";
                        var count = await SelectScalarAsync<T>(countQuery, dacCountQuery, sqlInfo);
                        if (count is int)
                            request.PagingData.ResultCount = (int)count;
                    }
                    if (!request.PagingData.ShowAll)
                    {
                        //create offset query text 
                        //TODO: use parameters for this
                        pagingOffset = $"OFFSET {request.PagingData.CurrentPage * request.PagingData.PageSize} ROWS FETCH NEXT {request.PagingData.PageSize} ROWS ONLY";

                        //if paging is applied, the offset/fetch next construct is used. no need to apply TOP() in this case                    
                        top = null;
                    }
                }

                string sqlText = $"{rowcount} select {top}{selectColumns} from {sqlInfo.SqlTable} {sqlInfo.SqlJoin} {whereClause} {sqlInfo.ResultGroup} {order} {pagingOffset}";








                var result = await SelectAllAsync<T>(entity, sqlText, dac, sqlInfo, propertyList);
                if (key != null)
                    Caching.Add(key, result);
                return result;
            }

        }

        async Task<List<T>> SelectAllAsync<T>(T entity, string sqlText, Sushi.Mediakiwi.Data.Connection.SqlCommander dac, SqlInformation sqlInfo, Property[] propertyList = null)
        {
            var start = DateTime.Now.Ticks;

            List<T> list = new List<T>();

            SqlLastExecuted = sqlText;
            dac.SqlText = sqlText;

            var sqlParameters = GetSqlParameters(entity, sqlInfo);
            //var primaryColumn = Primary(sqlParameters);

            SqlDataReader reader = dac.ExecReader;
            while (await reader.ReadAsync())
            {
                var clone = (T)Activator.CreateInstance(entity.GetType());
                int index = 0;
                foreach (DatabaseColumnAttribute param in sqlParameters)
                {
                    object value = GetData(reader, index, param);
                    if (param.Info.PropertyType == typeof(Sushi.Mediakiwi.Data.CustomData) && sqlInfo.PropertyListID.HasValue)
                    {

                        Sushi.Mediakiwi.Data.CustomData customData = SetPropertyValueCustomData(param.Info, value, clone);
                        foreach (Sushi.Mediakiwi.Data.Property p in propertyList)
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
                        SetPropertyValue(param.Info, value, clone);
                    }
                    index++;
                }
                CheckValidity(false, clone, sqlParameters);
                list.Add(clone);
            }
            LoggingWrite(start, sqlText, sqlInfo);
            return list;
        }
    }

}
