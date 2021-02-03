using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Framework
{
    public class InstallerHelper
    {
        static ISqlEntityParser _DataParser;
        protected ISqlEntityParser Sql
        {
            get
            {
                if (_DataParser == null)
                    _DataParser = Sushi.Mediakiwi.Data.Environment.GetInstance<ISqlEntityParser>();
                return _DataParser;
            }
        }

        protected bool ProcedureExists(string tableName, string createScript = null, string alternativeConnectionString = null)
        {
            string sql = @"select count(*) from sys.objects where name = @PROCEDURE and type = 'P'";

            DataRequest request = new DataRequest(null, alternativeConnectionString);
            request.AddParam("PROCEDURE", tableName, System.Data.SqlDbType.NVarChar);

            return Sql.Execute<int>(sql, request) == 1;
        }

        protected bool TableExists(string tableName, string createScript = null, string alternativeConnectionString = null)
        {
            string sql = @"select count(*) from sys.objects where name = @TABLE and type = 'U'";

            DataRequest data = new DataRequest(null, alternativeConnectionString);
            data.AddParam("TABLE", tableName, System.Data.SqlDbType.NVarChar);

            if (Sql.Execute<int>(sql, data) == 1)
            {
                if (string.IsNullOrEmpty(createScript))
                    Sql.Execute(createScript, data);
                return true;
            }
            return false;
        }

        protected bool ViewExists(string tableName, string createScript = null, string alternativeConnectionString = null)
        {
            string sql = @"select count(*) from sys.objects where name = @VIEW and type = 'V'";

            DataRequest data = new DataRequest(null, alternativeConnectionString);
            data.AddParam("VIEW", tableName, System.Data.SqlDbType.NVarChar);

            return Sql.Execute<int>(sql, data) == 1;
        }

        protected bool PrimaryKeyExists(string key, string createScript = null, string alternativeConnectionString = null)
        {
            string sql = @"select count(*) from sys.all_objects where name = @KEY and type = 'PK'";

            DataRequest data = new DataRequest(null, alternativeConnectionString);
            data.AddParam("KEY", key, System.Data.SqlDbType.NVarChar);
            return Sql.Execute<int>(sql, data) == 1;
        }

        protected void AddPrimaryKey(string tableName, string column, string key = null, string alternativeConnectionString = null)
        {
            if (key == null)
                key = string.Concat("PK_", column);

            string sql = @"ALTER TABLE @TABLE WITH NOCHECK ADD CONSTRAINT @KEY PRIMARY KEY CLUSTERED (@COLUMN)";
            DataRequest data = new DataRequest(null, alternativeConnectionString);

            data.AddParam("TABLE", tableName, System.Data.SqlDbType.NVarChar);
            data.AddParam("COLUMN", column, System.Data.SqlDbType.NVarChar);
            data.AddParam("KEY", key, System.Data.SqlDbType.NVarChar);
            Sql.Execute(sql, data);
        }

        protected void AddForeignKey(string primaryTableName, string primaryColumn, string relatedTableName, string relatedColumn, string key = null, string alternativeConnectionString = null)
        {
            if (key == null)
                key = string.Concat("FK_", relatedColumn, "_", primaryColumn);

            string sql1 = @"
ALTER TABLE 2RELATED_TABLE  WITH CHECK ADD CONSTRAINT @KEY 
	FOREIGN KEY (@RELATED_COLUMN)
	REFERENCES @PRIMAIRY_TABLE (@PRIMAIRY_COLUMN)

ALTER TABLE @RELATED_TABLE CHECK CONSTRAINT @KEY";

            DataRequest data = new DataRequest(null, alternativeConnectionString);
            data.AddParam("PRIMAIRY_TABLE", primaryTableName, System.Data.SqlDbType.NVarChar);
            data.AddParam("PRIMAIRY_COLUMN", primaryColumn, System.Data.SqlDbType.NVarChar);
            data.AddParam("RELATED_TABLE", relatedTableName, System.Data.SqlDbType.NVarChar);
            data.AddParam("RELATED_COLUMN", relatedColumn, System.Data.SqlDbType.NVarChar);
            data.AddParam("KEY", key, System.Data.SqlDbType.NVarChar);
            Sql.Execute(sql1, data);

            string sql2 = @"ALTER TABLE @RELATED_TABLE CHECK CONSTRAINT @KEY";

            data = new DataRequest(null, alternativeConnectionString);
            data.AddParam("RELATED_TABLE", relatedTableName, System.Data.SqlDbType.NVarChar);
            data.AddParam("KEY", key, System.Data.SqlDbType.NVarChar);
            Sql.Execute(sql2, data);
        }

        protected bool ForeignKeyExists(string key, string alternativeConnectionString = null)
        {
            string sql = @"select COUNT(*) from sys.all_objects where name = @KEY and type = 'F'";
            DataRequest data = new DataRequest(null, alternativeConnectionString);
            data.AddParam("KEY", key, System.Data.SqlDbType.NVarChar);
            return Sql.Execute<int>(sql, data) == 1;
        }

        protected bool IndexExists(string index, string alternativeConnectionString = null)
        {
            string sql = @"select COUNT(*) from sys.indexes where name = @INDEX";
            DataRequest data = new DataRequest(null, alternativeConnectionString);
            data.AddParam("INDEX", index, System.Data.SqlDbType.NVarChar);
            return Sql.Execute<int>(sql, data) == 1;
        }

        protected bool ColumnExists(string tableName, string column, string alternativeConnectionString = null)
        {
            string sql = @"select count(*) from syscolumns where id = (select object_id from sys.objects where name = @TABLE) and [Name] = @COLUMN";
            DataRequest data = new DataRequest(null, alternativeConnectionString);
            data.AddParam("TABLE", tableName, System.Data.SqlDbType.NVarChar);
            data.AddParam("COLUMN", column, System.Data.SqlDbType.NVarChar);
            return Sql.Execute<int>(sql, data) == 1;
        }
    }
}
