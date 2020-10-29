using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Sushi.Mediakiwi.Data.DalReflection;

namespace Sushi.Mediakiwi.Framework
{
    public interface ISqlEntityParser2
    {
        void AddParam(string name, object itemvalue, SqlDbType type);
        bool Delete<T>(T entity, List<DatabaseDataValueColumn> whereColumns = null, string alternativeConnectionString = null);
        bool Execute(string sqlText, bool throwException = true, string alternativeConnectionString = null);
        T Execute<T>(string sqlText, bool cacheResult = false, string alternativeConnectionString = null);
        List<T> ExecuteList<T>(string sqlText, bool cacheResult = false, string alternativeConnectionString = null);
        int Insert<T>(T entity, bool identityInsert = false, string alternativeConnectionString = null);
        int Save<T>(T entity, string alternativeConnectionString = null);
        List<T> SelectAll<T>(int componentListID, string alternativeConnectionString = null) where T : class;
        List<T> SelectAll<T>(string storedProcedure, SqlParameter[] parameters, string alternativeConnectionString = null);
        List<T> SelectAll<T>(List<DatabaseDataValueColumn> whereColumns = null, string cacheReference = null, int? componentListID = default(int?), string alternativeConnectionString = null) where T : class;
        T SelectOne<T>(bool cacheResult = false, string alternativeConnectionString = null);
        T SelectOne<T>(int key, bool cacheResult, string alternativeConnectionString = null);
        T SelectOne<T>(List<DatabaseDataValueColumn> whereColumns, string cacheReference = null, string alternativeConnectionString = null);
        T SelectOne<T>(int key, bool cacheResult, int? componentListID, string alternativeConnectionString = null);
        int Update<T>(T entity, List<DatabaseDataValueColumn> whereColumns = null, string alternativeConnectionString = null);
        T SelectOne<T>(System.Guid key, bool cacheResult = false, string alternativeConnectionString = null);
    }
}