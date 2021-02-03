using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data;

public static class CatalogExtension
{

    public static Catalog.CatalogColumn[] GetColumns(this Catalog inCatalog)
    {
        return inCatalog.GetColumns(true, inCatalog.Table);
    }

    /// <summary>
    /// Gets the filter columns.
    /// </summary>
    /// <param name="fiterKnown">if set to <c>true</c> [fiter known].</param>
    /// <param name="table">The table.</param>
    /// <returns></returns>
    public static Catalog.CatalogColumn[] GetColumns(this Catalog inCatalog, bool fiterKnown, string table)
    {
        List<Catalog.CatalogColumn> list = new List<Catalog.CatalogColumn>();

        using (Sushi.Mediakiwi.Data.Connection.DataCommander dac = new Sushi.Mediakiwi.Data.Connection.DataCommander(inCatalog.ConnectionString))
        {
            dac.Text = string.Format("select syscolumns.[Name], syscolumns.prec, isnullable, sysTypes.Name from syscolumns left join sysTypes on syscolumns.xusertype = sysTypes.xtype and sysTypes.status = 0 where id = (select id from sysobjects where name = '{0}') order by colorder", table);
            string[] exceptions = new string[0];

            if (fiterKnown)
            {
                exceptions = new string[] {
                            string.Concat(inCatalog.ColumnPrefix, "_key"),
                            string.Concat(inCatalog.ColumnPrefix, "_guid"),
                            string.Concat(inCatalog.ColumnPrefix, "_data")};
            }

            IDataReader reader = dac.ExecReader;

            while (reader.Read())
            {
                string column = reader.GetString(0);
                string columnSqlType = reader.GetString(3);

                bool skipMe = false;
                foreach (string exception in exceptions)
                {
                    if (exception.Equals(column, StringComparison.OrdinalIgnoreCase))
                    {
                        skipMe = true;
                        break;
                    }
                }
                if (skipMe) continue;

                int precision = Wim.Utility.ConvertToInt(reader.GetValue(1));
                bool isNullable = false;

                isNullable = reader.GetInt32(2) == 1;

                Catalog.CatalogColumn c = new Catalog.CatalogColumn();
                c.Index = precision;
                c.Name = column;
                c.Length = precision;
                c.IsNullable = isNullable;
                inCatalog.ApplySqlDbType(c, columnSqlType);

                list.Add(c);
            }
        }

        using (Sushi.Mediakiwi.Data.Connection.DataCommander dac = new Sushi.Mediakiwi.Data.Connection.DataCommander(inCatalog.ConnectionString))
        {
            dac.Text = string.Concat("select top 0 * from ", table);
            IDataReader reader = dac.ExecReader;

            for (int index = 0; index < reader.FieldCount; index++)
            {
                Catalog.CatalogColumn currentColumn = null;

                foreach (Catalog.CatalogColumn col in list)
                {
                    if (col.Name.Equals(reader.GetName(index)))
                    {
                        currentColumn = col;
                        currentColumn.Index = index;
                        break;
                    }
                }
                if (currentColumn == null) 
                    continue;

                currentColumn.DatabaseType = reader.GetDataTypeName(index);
                currentColumn.Type = reader.GetFieldType(index);
            }
        }
        return list.ToArray();
    }

    internal static void ApplySqlDbType(this Catalog inCatalog, Catalog.CatalogColumn col, string type)
    {
        type = type.ToLower();
        if (type == "int" || type == "%library.integer")
        {
            col.ClassType = col.IsNullable ? "int?" : "int";
            col.SqlDbType = SqlDbType.Int;
        }
        else if (type == "bigint")
        {
            col.ClassType = col.IsNullable ? "Int64?" : "Int64";
            col.SqlDbType = SqlDbType.BigInt;
        }
        else if (type == "uniqueidentifier" || (type == "%%library.string" && col.Length == 36))
        {
            col.Type = typeof(Guid);
            col.ClassType = "Guid";
            col.SqlDbType = SqlDbType.UniqueIdentifier;
        }
        else if (type == "nvarchar" || type == "%%library.string")
        {
            col.Type = typeof(String);
            col.ClassType = "string";
            col.SqlDbType = SqlDbType.NVarChar;
        }
        else if (type == "datetime" || type == "%library.timestamp")
        {
            col.Type = typeof(DateTime);
            col.ClassType = col.IsNullable ? "DateTime?" : "DateTime";
            col.SqlDbType = SqlDbType.DateTime;
        }
        else if (type == "decimal" || type == "%library.decimal")
        {
            col.Type = typeof(Decimal);
            col.ClassType = col.IsNullable ? "decimal?" : "decimal";
            col.SqlDbType = SqlDbType.Decimal;
        }
        else if (type == "bit" || type == "%library.boolean")
        {
            col.Type = typeof(Boolean);
            col.ClassType = "bool";
            col.SqlDbType = SqlDbType.Bit;
        }
        else if (type == "xml" || type == "%library.globalcharacterstream")
        {
            col.Type = typeof(Sushi.Mediakiwi.Data.CustomData);
            col.ClassType = "Sushi.Mediakiwi.Data.CustomData";
            col.SqlDbType = SqlDbType.Xml;
        }
        else if (type == "varchar")
        {
            col.Type = typeof(String);
            col.ClassType = "string";
            col.SqlDbType = SqlDbType.VarChar;
        }
    }

    /// <summary>
    /// Validates the SQL table creation.
    /// </summary>
    /// <returns></returns>
    public static bool ValidateSqlTableCreation(this Catalog inCatalog)
    {
        string message = "";
        return ValidateSqlTableCreation(inCatalog, null, out message);
    }

    /// <summary>
    /// Validates the SQL table creation.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <returns></returns>
    public static bool ValidateSqlTableCreation(this Catalog inCatalog, out string message)
    {
        return ValidateSqlTableCreation(inCatalog, null, out message);
    }

    /// <summary>
    /// Validates the SQL table key.
    /// </summary>
    /// <returns></returns>
    public static bool ValidateSqlTableKey(this Catalog inCatalog)
    {
        try
        {
            Catalog cl = new Catalog();
            cl.SqlConnectionString = inCatalog.ConnectionString;

            if (inCatalog.IsExistingTable)
                cl.Execute(string.Format("select count({1}_Key) from {0}", inCatalog.Table, inCatalog.ColumnPrefix));

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Validates the SQL table GUID.
    /// </summary>
    /// <returns></returns>
    public static bool ValidateSqlTableGuid(this Catalog inCatalog)
    {
        try
        {
            Catalog cl = new Catalog();
            cl.SqlConnectionString = inCatalog.ConnectionString;

            if (inCatalog.IsExistingTable)
                cl.Execute(string.Format("select count({1}_GUID) from {0}", inCatalog.Table, inCatalog.ColumnPrefix));

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Validates the SQL table creation.
    /// </summary>
    /// <returns></returns>
    public static bool ValidateSqlTableCreation(this Catalog inCatalog, string portal, out string message)
    {
        message = "";

        if (string.IsNullOrEmpty(inCatalog.Table))
            return false;
        if (string.IsNullOrEmpty(inCatalog.ColumnPrefix))
            return false;

        if (!string.IsNullOrEmpty(portal))
        {
            inCatalog.SqlConnectionString = Common.GetPortal(portal).Connection;
            inCatalog.ConnectionType = Common.GetPortal(portal).Type;
        }

        if (!ValidateSqlTableKey(inCatalog) && string.IsNullOrEmpty(inCatalog.ColumnKey))
            throw new Exception(string.Format("Could not determine the {0}_Key (int) column in the table '{1}'. This has to be a primary key column.", inCatalog.ColumnPrefix, inCatalog.Table));

        if (!ValidateSqlTableGuid(inCatalog) && string.IsNullOrEmpty(inCatalog.ColumnGuid))
            throw new Exception(string.Format("Could not determine the {0}_Key (int) column in the table '{1}'. This has to be a primary key column.", inCatalog.ColumnPrefix, inCatalog.Table));

        Catalog cl = new Catalog();
        cl.SqlConnectionString = inCatalog.ConnectionString;

        try
        {
            int count = Wim.Utility.ConvertToInt(cl.Execute(string.Concat("select count(*) from ", inCatalog.Table)));

            #region Check for HasSortOrder

            if (inCatalog.HasSortOrder)
            {
                try
                {
                    cl.Execute(string.Format("select count({1}_SortOrder) from {0}", inCatalog.Table, inCatalog.ColumnPrefix));
                }
                catch (Exception)
                {
                    if (message.Length == 0) message = string.Format("The column {0}_SortOrder was not found. This has been added to the catalog.", inCatalog.ColumnPrefix);

                    cl.Execute(string.Format("alter table {0} add {1}_SortOrder int NULL", inCatalog.Table, inCatalog.ColumnPrefix));
                    cl.Execute(string.Format("update {0} set {1}_SortOrder = {1}_Key", inCatalog.Table, inCatalog.ColumnPrefix));
                }
            }

            #endregion

            #region Check for Data

            try
            {
                cl.Execute(string.Format("select count({1}_Data) from {0}", inCatalog.Table, inCatalog.ColumnPrefix));
            }
            catch (Exception)
            {
                if (message.Length == 0) message = string.Format("The column {0}_Data was not found. This has been added to the catalog.", inCatalog.ColumnPrefix);
                cl.Execute(string.Format("alter table {0} add {1}_Data XML NULL", inCatalog.Table, inCatalog.ColumnPrefix));
            }

            #endregion

            return true;
        }
        catch (Exception)
        {
        
            if (inCatalog.HasSortOrder && inCatalog.HasCatalogBaseStructure)
            {
                try
                {
                    cl.Execute(string.Format(@"CREATE TABLE {0} (
{1}_Key int IDENTITY(1,1) NOT NULL,
{1}_GUID uniqueidentifier NOT NULL,
{1}_List_Key int NOT NULL,
{1}_Site_Key int NOT NULL,
{1}_Data xml NULL,
{1}_Created datetime NOT NULL,
{1}_SortOrder int NULL)", inCatalog.Table, inCatalog.ColumnPrefix));

                    cl.Execute(string.Format(@"ALTER TABLE {0} WITH NOCHECK ADD CONSTRAINT PK_{1}_Key PRIMARY KEY CLUSTERED ({1}_Key)", inCatalog.Table, inCatalog.ColumnPrefix));
                    return true;
                }
                catch (Exception ex)
                {
                    Notification.InsertOne("Validate SqlTable Creation", ex);
                    return false;
                }
            }
            else if (inCatalog.HasSortOrder && !inCatalog.HasCatalogBaseStructure)
            {
                try
                {
                    cl.Execute(string.Format(@"CREATE TABLE {0} (
{1}_Key int IDENTITY(1,1) NOT NULL,
{1}_GUID uniqueidentifier NOT NULL,
{1}_Data xml NULL,
{1}_SortOrder int NULL)", inCatalog.Table, inCatalog.ColumnPrefix));

                    cl.Execute(string.Format(@"ALTER TABLE {0} WITH NOCHECK ADD CONSTRAINT PK_{1}_Key PRIMARY KEY CLUSTERED ({1}_Key)", inCatalog.Table, inCatalog.ColumnPrefix));
                    return true;
                }
                catch (Exception ex)
                {
                    Notification.InsertOne("Validate SqlTable Creation", ex);
                    return false;
                }
            }
            else if (!inCatalog.HasSortOrder && inCatalog.HasCatalogBaseStructure)
            {
                try
                {
                    cl.Execute(string.Format(@"CREATE TABLE {0} (
{1}_Key int IDENTITY(1,1) NOT NULL,
{1}_GUID uniqueidentifier NOT NULL,
{1}_List_Key int NOT NULL,
{1}_Site_Key int NOT NULL,
{1}_Data xml NULL,
{1}_Created datetime NOT NULL)", inCatalog.Table, inCatalog.ColumnPrefix));

                    cl.Execute(string.Format(@"ALTER TABLE {0} WITH NOCHECK ADD CONSTRAINT PK_{1}_Key PRIMARY KEY CLUSTERED ({1}_Key)", inCatalog.Table, inCatalog.ColumnPrefix));
                    return true;
                }
                catch (Exception ex)
                {
                    Notification.InsertOne("Validate SqlTable Creation", ex);
                    return false;
                }
            }
            else
            {
                try
                {
                    cl.Execute(string.Format(@"CREATE TABLE {0} (
{1}_Key int IDENTITY(1,1) NOT NULL,
{1}_GUID uniqueidentifier NOT NULL,
{1}_Data xml NULL)", inCatalog.Table, inCatalog.ColumnPrefix));

                    cl.Execute(string.Format(@"ALTER TABLE {0} WITH NOCHECK ADD CONSTRAINT PK_{1}_Key PRIMARY KEY CLUSTERED ({1}_Key)", inCatalog.Table, inCatalog.ColumnPrefix));
                    return true;
                }
                catch (Exception ex)
                {
                    Notification.InsertOne("Validate SqlTable Creation", ex);
                    return false;
                }
            }
        }
    }
}
