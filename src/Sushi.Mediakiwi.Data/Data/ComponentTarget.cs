using Sushi.Mediakiwi.Data.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    [DataMap(typeof(ComponentTargetMap))]
    public class ComponentTarget : IComponentTarget
    {
        public class ComponentTargetMap : DataMap<ComponentTarget>
        {
            public ComponentTargetMap()
            {
                Table("wim_ComponentTargets");

                Id(x => x.ID, "ComponentTarget_Key").Identity();
                Map(x => x.PageID, "ComponentTarget_Page_Key");
                Map(x => x.Source, "ComponentTarget_Component_Source");
                Map(x => x.Target, "ComponentTarget_Component_Target");
            }
        }

        #region Properties

        public int ID { get; set; }

        public int PageID { get; set; }

        public Guid Source { get; set; }

        public Guid Target { get; set; }

        #endregion Properties

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        public void Delete()
        {
            var connector = ConnectorFactory.CreateConnector<ComponentTarget>();
            connector.Delete(this);
        }

        /// <summary>
        /// Deletes this instance Async.
        /// </summary>
        public async Task DeleteAsync()
        {
            var connector = ConnectorFactory.CreateConnector<ComponentTarget>();
            await connector.DeleteAsync(this);
        }

        /// <summary>
        /// Selects one target based on the ID
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static IComponentTarget SelectOne(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentTarget>();
            return connector.FetchSingle(ID);
        }

        /// <summary>
        /// Selects one target based on the ID Async
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static async Task<IComponentTarget> SelectOneAsync(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentTarget>();
            return await connector.FetchSingleAsync(ID);
        }

        /// <summary>
        /// Selects all targets based on the PageID
        /// </summary>
        /// <param name="pageID"></param>
        /// <returns></returns>
        public static IComponentTarget[] SelectAll(int pageID)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentTarget>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.PageID, pageID);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all targets based on the PageID
        /// </summary>
        /// <param name="pageID"></param>
        /// <returns></returns>
        public static async Task<IComponentTarget[]> SelectAllAsync(int pageID)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentTarget>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.PageID, pageID);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Deletes all from Component Targets on the same page and with
        /// the same target
        /// </summary>
        public virtual void DeleteComplete()
        {
            var connector = ConnectorFactory.CreateConnector<ComponentTarget>();
            var filter = connector.CreateQuery();
            filter.AddParameter("@thisTarget", Target);
            filter.AddParameter("@thisSource", Source);
            filter.AddParameter("@thisPageId", PageID);

            connector.ExecuteNonQuery(@"
DELETE FROM [wim_ComponentTargets] 
WHERE [ComponentTarget_Page_Key] = @thisPageId 
AND [ComponentTarget_Component_Target] = @thisTarget 
AND NOT [ComponentTarget_Component_Source] = @thisSource", filter);
			connector.Cache?.FlushRegion(connector.CacheRegion);
        }

        /// <summary>
        /// Deletes all from Component Targets on the same page and with
        /// the same target Async
        /// </summary>
        public async virtual Task DeleteCompleteAsync()
        {
            var connector = ConnectorFactory.CreateConnector<ComponentTarget>();
            var filter = connector.CreateQuery();
            filter.AddParameter("@thisTarget", Target);
            filter.AddParameter("@thisSource", Source);
            filter.AddParameter("@thisPageId", PageID);

            await connector.ExecuteNonQueryAsync(@"
DELETE FROM [wim_ComponentTargets] 
WHERE [ComponentTarget_Page_Key] = @thisPageId 
AND [ComponentTarget_Component_Target] = @thisTarget 
AND NOT [ComponentTarget_Component_Source] = @thisSource", filter);
			connector.Cache?.FlushRegion(connector.CacheRegion);
        }

        /// <summary>
        /// Selects all with supplied Source GUID
        /// </summary>
        /// <param name="componentGuid"></param>
        /// <returns></returns>
        public static IComponentTarget[] SelectAll(Guid componentGuid)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentTarget>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.Source, componentGuid);

            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all with supplied Source GUID
        /// </summary>
        /// <param name="componentGuid"></param>
        /// <returns></returns>
        public static async Task<IComponentTarget[]> SelectAllAsync(Guid componentGuid)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentTarget>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.Source, componentGuid);

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Saves this instance
        /// </summary>
        /// <returns></returns>
        public virtual bool Save()
        {
            var connector = ConnectorFactory.CreateConnector<ComponentTarget>();
            try
            {
                connector.Save(this);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Saves this instance Async
        /// </summary>
        /// <returns></returns>
        public async virtual Task<bool> SaveAsync()
        {
            var connector = ConnectorFactory.CreateConnector<ComponentTarget>();
            try
            {
                await connector.SaveAsync(this);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool IsNewInstance
        {
            get { return this.ID == 0; }
        }
    }
}