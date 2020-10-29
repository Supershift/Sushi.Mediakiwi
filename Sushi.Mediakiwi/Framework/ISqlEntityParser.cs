using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.DalReflection;

namespace Sushi.Mediakiwi.Framework
{
    public interface ISqlEntityParser
    {
        Exception LastException { get; set; }
        void FlushCache(object entity);

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
        void BulkInsert<T>(IEnumerable<T> entities, bool identityInsert = false, string alternativeConnectionString = null, SqlBulkCopyOptions sqlBulkCopyOptions = SqlBulkCopyOptions.Default);

        int Insert<T>(T entity, List<IDataParameter> parameterList = null, bool identityInsert = false, string alternativeConnectionString = null);
        Task<int> InsertAsync<T>(T entity, List<IDataParameter> parameterList = null, bool identityInsert = false, string alternativeConnectionString = null);

        int Update<T>(T entity, List<DatabaseDataValueColumn> whereColumns = null, List<IDataParameter> parameterList = null, string alternativeConnectionString = null);
        Task<int> UpdateAsync<T>(T entity, List<DatabaseDataValueColumn> whereColumns = null, List<IDataParameter> parameterList = null, string alternativeConnectionString = null);

        int Save<T>(T entity, string alternativeConnectionString = null, bool flushCache = true);
        Task<int> SaveAsync<T>(T entity, string alternativeConnectionString = null, bool flushCache = true);

        bool Delete<T>(T entity, List<DatabaseDataValueColumn> whereColumns = null, string alternativeConnectionString = null);
        bool Delete<T>(List<DatabaseDataValueColumn> whereColumns, string alternativeConnectionString = null);
        Task<bool> DeleteAsync<T>(T entity, List<DatabaseDataValueColumn> whereColumns = null, string alternativeConnectionString = null);
        Task<bool> DeleteAsync<T>(List<DatabaseDataValueColumn> whereColumns = null, string alternativeConnectionString = null);

        bool Execute(string sqlText, IDataRequest request = null);
        T Execute<T>(string sqlText, IDataRequest request = null);
        List<T> ExecuteList<T>(string sqlText, IDataRequest request = null);

        Task<bool> ExecuteAsync(string sqlText, IDataRequest request = null);
        Task<T> ExecuteAsync<T>(string sqlText, IDataRequest request = null);
        Task<List<T>> ExecuteListAsync<T>(string sqlText, IDataRequest request = null);

        List<T> SelectAll<T>(int componentListID, string alternativeConnectionString = null) where T : class;
        List<T> SelectAll<T>(string storedProcedure, IDataParameter[] parameters, string alternativeConnectionString = null);
        List<T> SelectAll<T>(List<DatabaseDataValueColumn> whereColumns, string cacheReference = null, int? componentListID = default(int?), string alternativeConnectionString = null, int? maxResults = null, string orderByColumn = null) where T : class;
        List<T> SelectAll<T>(IDataRequest request = null) where T : class;

        Task<List<T>> SelectAllAsync<T>(IDataRequest request = null) where T : class;
        Task<List<T>> SelectAllAsync<T>(int componentListID, string alternativeConnectionString = null) where T : class;
        Task<List<T>> SelectAllAsync<T>(string storedProcedure, IDataParameter[] parameters, string alternativeConnectionString = null);
        Task<List<T>> SelectAllAsync<T>(List<DatabaseDataValueColumn> whereColumns, string cacheReference = null, int? componentListID = default(int?), string alternativeConnectionString = null, int? maxResults = null, string orderByColumn = null) where T : class;

        T SelectOne<T>(bool cacheResult = false, string alternativeConnectionString = null);
        T SelectOne<T>(int key, bool cacheResult, string alternativeConnectionString = null);
        T SelectOne<T>(int key, bool cacheResult, string alternativeConnectionString, bool alwasyRetrieveFromDatabase);
        T SelectOne<T>(List<DatabaseDataValueColumn> whereColumns, string cacheReference = null, string alternativeConnectionString = null);
        T SelectOne<T>(List<DatabaseDataValueColumn> whereColumns, string cacheReference, string alternativeConnectionString, bool alwaysRetrieveFromDatabase);
        T SelectOne<T>(Guid key, bool cacheResult = false, string alternativeConnectionString = null);
        T SelectOne<T>(int key, bool cacheResult, int? componentListID, string alternativeConnectionString = null);

        Task<T> SelectOneAsync<T>(bool cacheResult = false, string alternativeConnectionString = null);
        Task<T> SelectOneAsync<T>(int key, bool cacheResult, string alternativeConnectionString = null);
        Task<T> SelectOneAsync<T>(List<DatabaseDataValueColumn> whereColumns, string cacheReference = null, string alternativeConnectionString = null);
        Task<T> SelectOneAsync<T>(Guid key, bool cacheResult = false, string alternativeConnectionString = null);
        Task<T> SelectOneAsync<T>(int key, bool cacheResult, int? componentListID, string alternativeConnectionString = null);



    }
}