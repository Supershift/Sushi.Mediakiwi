using Sushi.Mediakiwi.Data.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    [DataMap(typeof(PageVersionMap))]
    public partial class PageVersion : IPageVersion
    {
        public PageVersion()
        {
            this.Created = DateTime.UtcNow;
        }

        public class PageVersionMap : DataMap<PageVersion>
        {
            public PageVersionMap()
            {
                Table("wim_PageVersions");
                Id(x => x.ID, "PageVersion_Key").Identity();
                Map(x => x.PageID, "PageVersion_Page_Key");
                Map(x => x.TemplateID, "PageVersion_PageTemplate_Key");
                Map(x => x.Created, "PageVersion_Created");
                Map(x => x.ContentXML, "PageVersion_Content").SqlType(System.Data.SqlDbType.Xml);
                Map(x => x.MetaDataXML, "PageVersion_MetaData").SqlType(System.Data.SqlDbType.Xml);
                Map(x => x.Name, "PageVersion_Name");
                Map(x => x.CompletePath, "PageVersion_CompletePath");
                Map(x => x.Hash, "PageVersion_Hash");
                Map(x => x.UserID, "PageVersion_User_Key");
                Map(x => x.IsArchived, "PageVersion_IsArchived");
            }
        }

        #region Properties

        public virtual bool IsNewInstance
        {
            get { return this.ID == 0; }
        }

        public virtual int ID { get; set; }

        public virtual int PageID { get; set; }

        public virtual int TemplateID { get; set; }

        public virtual int UserID { get; set; }

        public virtual DateTime Created { get; set; }

        public virtual string ContentXML { get; set; }

        public virtual string MetaDataXML { get; set; }

        public virtual bool IsArchived { get; set; }

        public virtual string Name { get; set; }

        public virtual string CompletePath { get; set; }

        public virtual string Hash { get; set; }

        public virtual string RollBackTo { get; set; }



        #endregion Properties

        /// <summary>
        /// Get an instande of the connector
        /// </summary>
        static Caching.CachedConnector<PageVersion> Connector
        {
            get
            {
                var connector = ConnectorFactory.CreateConnector<PageVersion>();
                // do not cache the select, also preventing from generating cache references 
                connector.UseCacheOnSelect = false;
                return connector;
            }
        }

        /// <summary>
        /// Select a single page version by Identifier
        /// </summary>
        /// <param name="ID">The PageVersion Identifier</param>
        /// <returns></returns>
        public static IPageVersion SelectOne(int ID)
        {
            var connector = Connector;
            return connector.FetchSingle(ID);
        }

        /// <summary>
        /// Select a single page version by Identifier Async
        /// </summary>
        /// <param name="ID">The PageVersion Identifier</param>
        /// <returns></returns>
        public static async Task<IPageVersion> SelectOneAsync(int ID)
        {
            var connector = Connector;
            return await connector.FetchSingleAsync(ID);
        }

        /// <summary>
        /// Saves this PageVersion Instance
        /// </summary>
        /// <returns></returns>
        public virtual bool Save()
        {
            var connector = Connector;
            connector.Save(this);
            return true;
        }

        /// <summary>
        /// Saves this PageVersion Instance Async
        /// </summary>
        /// <returns></returns>
        public async virtual Task<bool> SaveAsync()
        {
            var connector = Connector;
            await connector.SaveAsync(this);
            return true;
        }

        /// <summary>
        /// Select all Page Versions by page ID
        /// </summary>
        /// <param name="pageID">The Page Identifier for which to retrieve all page versions</param>
        /// <returns></returns>
        public static List<IPageVersion> SelectAllOfPage(int pageID)
        {
            var connector = Connector;
            var filter = connector.CreateQuery();
            filter.Add(x => x.PageID, pageID);

            return connector.FetchAll(filter).ToList<IPageVersion>();
        }

        /// <summary>
        /// Select all Page Versions by page ID Async
        /// </summary>
        /// <param name="pageID">The Page Identifier for which to retrieve all page versions</param>
        /// <returns></returns>
        public static async Task<List<IPageVersion>> SelectAllOfPageAsync(int pageID)
        {
            var connector = Connector;
            var filter = connector.CreateQuery();
            filter.Add(x => x.PageID, pageID);

            var result = await connector.FetchAllAsync(filter);
            return result.ToList<IPageVersion>();
        }
    }
}