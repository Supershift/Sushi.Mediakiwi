using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.MircoORM;

namespace Sushi.Mediakiwi.Data
{
    [DataMap(typeof(RegistryMap))]
    public class Registry : IRegistry
    {
        public class RegistryMap : DataMap<Registry>
        {
            public RegistryMap()
            {
                Table("wim_Registry");
                Id(x => x.ID, "Registry_Key").Identity();
                Map(x => x.GUID, "Registry_GUID");
                Map(x => x.Name, "Registry_Name").Length(50);
                Map(x => x.Type, "Registry_Type");
                Map(x => x.Value, "Registry_Value").Length(512);
                Map(x => x.Description, "Registry_Description").Length(512);
            }
        }

        public Registry()
        {
            this.GUID = Guid.NewGuid();
        }

        #region Properties

        public virtual int ID { get; set; }

        /// <summary>
        /// Unique identifier used for import/export
        /// </summary>
        public virtual Guid GUID { get; set; }

        /// <summary>
        /// Gets or sets the name of this registry item.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of this registry item
        /// </summary>
        public virtual int Type { get; set; }

        /// <summary>
        /// Gets the name and description of this registry item.
        /// </summary>
        public virtual string NameDescription
        {
            get
            {
                return string.Format("<b>{0}</b><br/>{1}", this.Name, this.Description);
            }
        }

        /// <summary>
        /// Gets or sets the value of this registry item.
        /// </summary>
        public virtual string Value { get; set; }

        /// <summary>
        /// Gets or sets the description of this registry item.
        /// </summary>
        public virtual string Description { get; set; }

        #endregion Properties

        public virtual void Save()
        {
            var connector = ConnectorFactory.CreateConnector<Registry>();
            connector.Save(this);
        }

        public virtual async Task SaveAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Registry>();
            await connector.SaveAsync(this);
        }

        public virtual void Delete()
        {
            var connector = ConnectorFactory.CreateConnector<Registry>();
            connector.Delete(this);
        }

        public virtual async Task DeleteAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Registry>();
            await connector.DeleteAsync(this);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public static IRegistry[] SelectAll()
        {
            var connector = ConnectorFactory.CreateConnector<Registry>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.Name);
            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all Async.
        /// </summary>
        /// <returns></returns>
        public static async Task<IRegistry[]> SelectAllAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Registry>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.Name);
            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The identifier.</param>
        /// <returns></returns>
        public static IRegistry SelectOne(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<Registry>();
            return connector.FetchSingle(ID);
        }

        /// <summary>
        /// Selects the one Async.
        /// </summary>
        /// <param name="ID">The identifier.</param>
        /// <returns></returns>
        public static async Task<IRegistry> SelectOneAsync(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<Registry>();
            return await connector.FetchSingleAsync(ID);
        }

        /// <summary>
        /// Selects the name of the one by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static IRegistry SelectOneByName(string name)
        {
            var connector = ConnectorFactory.CreateConnector<Registry>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.Name, name);

            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Selects the name of the one by Async.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static async Task<IRegistry> SelectOneByNameAsync(string name)
        {
            var connector = ConnectorFactory.CreateConnector<Registry>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.Name, name);

            return await connector.FetchSingleAsync(filter);
        }
    }
}