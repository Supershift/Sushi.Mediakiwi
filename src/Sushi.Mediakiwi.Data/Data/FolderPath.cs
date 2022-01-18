using Sushi.Mediakiwi.Data.MicroORM;
using Sushi.MicroORM.Mapping;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Represents a Folder entity.
    /// </summary>
    public class FolderPath 
    {
        public class FolderPathMap : DataMap<FolderPath>
        {   
            public FolderPathMap()
            {         
                Table("wim_Folders join wim_Sites on Folder_Site_Key = Site_Key");
             
                Id(x => x.ID, "Folder_Key").Identity();
                Map(x => x.CompletePath, "Folder_CompletePath").Length(1000);
                Map(x => x.DefaultFolder, "Site_DefaultFolder").ReadOnly();
            }
        }

        #region Properties

        public virtual bool IsNewInstance
        {
            get { return this.ID == 0; }
        }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public int ID { get; set; }

        public string CompletePath { get; set; }
        public string DefaultFolder { get; set; }
        #endregion Properties

        /// <summary>
        /// Select all Folders
        /// </summary>
        /// <returns></returns>
        public static List<FolderPath> SelectAll()
        {
            var connector = ConnectorFactory.CreateConnector<FolderPath>();
            var filter = connector.CreateQuery();

            return connector.FetchAll(filter);
        }

        /// <summary>
        /// Select all Folders
        /// </summary>
        /// <returns></returns>
        public static async Task<List<FolderPath>> SelectAllAsync()
        {
            var connector = ConnectorFactory.CreateConnector<FolderPath>();
            var filter = connector.CreateQuery();

            return await connector.FetchAllAsync(filter);
        }

        /// <summary>
        /// Select all Folders
        /// </summary>
        /// <returns></returns>
        public static FolderPath SelectOne(int id)
        {
            var connector = ConnectorFactory.CreateConnector<FolderPath>();

            return connector.FetchSingle(id);
        }


        /// <summary>
        /// Select all Folders
        /// </summary>
        /// <returns></returns>
        public static async Task<FolderPath> SelectOneAsync(int id)
        {
            var connector = ConnectorFactory.CreateConnector<FolderPath>();

            return await connector.FetchSingleAsync(id);
        }
    }
}