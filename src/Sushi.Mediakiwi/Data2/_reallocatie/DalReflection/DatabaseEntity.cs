using System;
using System.Collections.Generic;
using System.Text;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.Data.DalReflection
{

    /// <summary>
    /// 
    /// </summary>
    public class DatabaseEntity : BaseSqlEntity
    {
        
        /// <summary>
        /// 
        /// </summary>
        

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseEntity"/> class.
        /// </summary>
        public DatabaseEntity()
        {
            if (this.PropertyListID.HasValue) return;

            ListReference[] listRef = this.GetType().GetCustomAttributes(typeof(ListReference), true) as ListReference[];
            if (listRef != null && listRef.Length == 1)
            {
                this.PropertyListID = Sushi.Mediakiwi.Data.ComponentList.SelectOne(listRef[0].ListGUID, DatabaseMappingPortal).ID;
            }
        }

        /// <summary>
        /// Gets all database column attributes.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns></returns>
        protected static Sushi.Mediakiwi.Data.DalReflection.DatabaseColumnAttribute[] GetAllDatabaseColumnAttributes(object candidate)
        {
            List<Sushi.Mediakiwi.Data.DalReflection.DatabaseColumnAttribute> list = new List<Sushi.Mediakiwi.Data.DalReflection.DatabaseColumnAttribute>();

            System.Reflection.PropertyInfo[] candidateItems = candidate.GetType().GetProperties();
            foreach (System.Reflection.PropertyInfo item in candidateItems)
            {
                Sushi.Mediakiwi.Data.DalReflection.DatabaseColumnAttribute[] attr = item.GetCustomAttributes(typeof(Sushi.Mediakiwi.Data.DalReflection.DatabaseColumnAttribute), true) as Sushi.Mediakiwi.Data.DalReflection.DatabaseColumnAttribute[];
                if (attr != null && attr.Length == 1)
                {
                    list.Add(attr[0]);
                }
            }
            return list.ToArray();
        }
        public virtual bool Save()
        {
            return Save(true);
        }
        /// <summary>
        /// Save a database entity. This method will indentify the need for an INSERT or an UPDATE.
        /// </summary>
        /// <returns></returns>
        public virtual bool Save(bool doFlush = true)
        {
            bool isNew = false;
            bool save;
            if (PrimairyKeyValue.HasValue && PrimairyKeyValue != 0)
                save = Update();
            else
            {
                isNew = true;
                save = Insert();
            }

            // [MR:17-11-2015] Dit was :
            // if (isNew && System.Web.HttpContext.Current?.Items["wim.Saved.ID"] == null)
            if (isNew && System.Web.HttpContext.Current?.Items.Contains("wim.Saved.ID") == false)
                System.Web.HttpContext.Current.Items["wim.Saved.ID"] = PrimairyKeyValue;
            if (doFlush)
                FlushCache();

            return save;
        }

        /// <summary>
        /// Flushes the cache.
        /// </summary>
        protected void FlushCache()
        {
            string key = string.Concat("Data_", this.GetType().ToString());
            Wim.Utilities.CacheItemManager.FlushIndexOfCacheObjects(key);
            
        }

        /// <summary>
        /// Flushes the cache.
        /// </summary>
        /// <param name="type">The type.</param>
        public static void FlushCache(Type type)
        {
            string key = string.Concat("Data_", type.ToString());
            Wim.Utilities.CacheItemManager.FlushIndexOfCacheObjects(key);
        }

        public virtual SaveState SaveData()
        {
            string sql;
            return SaveData(out sql, false, null);
        }
        
        /// <summary>
        /// Save a database entity. This method will indentify the need for an INSERT, UPDATE or UNCHANGED. This last status is based on the Polution Hash.
        /// </summary>
        /// <returns></returns>
        public virtual SaveState SaveData(out string sqlStatement, bool onlyReturnStatement, List<DatabaseDataValueColumn> whereColumns)
        {
            this.SqlOnlySetStatement = onlyReturnStatement;

            SaveState state;

            if (PrimairyKeyValue.HasValue && PrimairyKeyValue != 0)
            {
                if (this.IsPoluted())
                {
                    state = SaveState.Updated;

                    if (whereColumns == null)
                        Update();
                    else
                        Update(whereColumns);
                }
                else
                {
                    if (onlyReturnStatement)
                    {
                        if (whereColumns == null)
                            Update();
                        else
                            Update(whereColumns);
                    }
                    state = SaveState.UnChanged;
                }
            }
            else
            {
                state = SaveState.Inserted;
                Insert();
            }

            sqlStatement = this.SqlLastExecuted;

            //  Only clear cache when inserted or updated.
            if (state != SaveState.UnChanged)
                Wim.Utilities.CacheItemManager.FlushIndexOfCacheObjects(string.Concat("Data_", this.GetType().ToString()));

            if (state == SaveState.Inserted && System.Web.HttpContext.Current != null)
                System.Web.HttpContext.Current.Items["wim.Saved.ID"] = PrimairyKeyValue;

            return state;
        }


        /// <summary>
        /// Delete an implementation record.
        /// </summary>
        /// <returns></returns>
        public override bool Delete()
        {
            Wim.Utilities.CacheItemManager.FlushIndexOfCacheObjects(string.Concat("Data_", this.GetType().ToString()));
            return base.Delete();
        }

        /// <summary>
        /// Saves the specified GUID.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <returns></returns>
        public virtual bool Save(Guid guid)
        {
            return Save(guid, null);
        }

        /// <summary>
        /// Saves the specified portal.
        /// </summary>
        /// <param name="portal">The portal.</param>
        /// <returns></returns>
        public virtual bool Save(string portal)
        {
            return Save(Guid.Empty, portal);
        }

        /// <summary>
        /// Save a database entity based on the Migration key.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <param name="portal">The portal.</param>
        /// <returns></returns>
        public virtual bool Save(Guid guid, string portal)
        {
            if (!string.IsNullOrEmpty(portal))
            {
                this.SqlConnectionString = Sushi.Mediakiwi.Data.Common.GetPortal(portal).Connection;
                this.ConnectionType = Sushi.Mediakiwi.Data.Common.GetPortal(portal).Type;
            }

            int key;

            if (guid != Guid.Empty)
                key = GetKeyByGuid(guid);
            else
                key = this.PrimairyKeyValue.GetValueOrDefault();
            
            DatabaseColumnAttribute primaryDataColumn = null;
            foreach (DatabaseColumnAttribute param in this.SqlParameters)
            {
                if (param.IsPrimaryKey && param.Column != null)
                {
                    primaryDataColumn = param;
                    break;
                }
            }
            if (primaryDataColumn != null)
                primaryDataColumn.Info.SetValue(this, key, null);

            if (key == 0)
                return Insert();
            else
            {
                if (guid == Guid.Empty)
                {
                    return Update();
                }
                else
                    return Update(guid);
            }
        }

        static string m_SqlConnectionString2;
        /// <summary>
        /// The Database connection string
        /// </summary>
        /// <value>The SQL connection string2.</value>
        [System.Xml.Serialization.XmlIgnore()]
        public static string SqlConnectionString2
        {
            get { return m_SqlConnectionString2; }
            set { m_SqlConnectionString2 = value; 
            }
        }

        static Sushi.Mediakiwi.Data.DataConnectionType m_DataConnectionType;
        /// <summary>
        /// Gets or sets the type of the data connection.
        /// </summary>
        /// <value>The type of the data connection.</value>
        [System.Xml.Serialization.XmlIgnore()]
        public static Sushi.Mediakiwi.Data.DataConnectionType DataConnectionType
        {
            get { return m_DataConnectionType; }
            set { m_DataConnectionType = value; }
        }
    }
}
