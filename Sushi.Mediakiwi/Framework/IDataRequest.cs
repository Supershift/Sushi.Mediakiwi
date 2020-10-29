using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Sushi.Mediakiwi.Data.DalReflection;

namespace Sushi.Mediakiwi.Framework
{
    public interface IDataRequest
    {
        string AlternativeConnectionString { get; }
        string CacheReference { get; }
        int? MaxResults { get; set; }
        string OrderByColumn { get; set; }
        void AddParam(string name, object value, SqlDbType type);
        void AddWhere(string column, SqlDbType type, object dbColValue);
        void AddWhere(string sql);

        DataEntities.IDataDetail PagingData { get; set; }

        List<DatabaseDataValueColumn> WhereClause
        {
            get;
        }
        List<SqlParameter> SqlParameters
        {
            get;
        }
    }
}