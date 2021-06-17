using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.MicroORM;

namespace Sushi.Mediakiwi.Data
{
    [DataMap(typeof(AuditTrailMap))]
    public class AuditTrail
    {
        public class AuditTrailMap : DataMap<AuditTrail>
        {
            public AuditTrailMap()
            {
                Table("wim_AuditTrails");
                Id(x => x.ID, "AuditTrail_Key").Identity();
                Map(x => x.EntityID, "AuditTrail_Entity_Key").SqlType(SqlDbType.Int);
                Map(x => x.Action, "AuditTrail_Action").SqlType(SqlDbType.Int);
                Map(x => x.Type, "AuditTrail_Type").SqlType(SqlDbType.Int);
                Map(x => x.ItemID, "AuditTrail_Listitem_Key").SqlType(SqlDbType.Int);
                Map(x => x.VersionID, "AuditTrail_Versioning_Key").SqlType(SqlDbType.Int);
                Map(x => x.Created, "AuditTrail_Created").SqlType(SqlDbType.DateTime);
                Map(x => x.Message, "AuditTrail_Message").SqlType(SqlDbType.NVarChar).Length(512);
            }
        }

        #region properties
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public int? ID { get; set; }

        /// <summary>
        /// Gets or sets the item ID.
        /// </summary>
        /// <value>The item ID.</value>
        public int? EntityID { get; set; }

        /// <summary>
        /// Gets or sets the item type ID.
        /// </summary>
        /// <value>The item type ID.</value>
        public ActionType Action { get; set; }


        /// <summary>
        /// Gets or sets the action type ID.
        /// </summary>
        /// <value>The action type ID.</value>
        public ItemType Type { get; set; }

        /// <summary>
        /// Gets or sets the element ID.
        /// </summary>
        /// <value>The element ID.</value>
        public int? ItemID { get; set; }

        /// <summary>
        /// Gets or sets the version ID.
        /// </summary>
        /// <value>The version ID.</value>
        public int? VersionID { get; set; }

        private DateTime m_Created;
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created { get; set; } = DateTime.UtcNow;
     

        public string Message { get; set; }
        #endregion properties
        
        /// <summary>
        /// Inserts the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="page">The page.</param>
        /// <param name="action">The action.</param>
        /// <param name="versionID">The version ID.</param>
        public static async Task InsertAsync(IApplicationUser user, Page page, ActionType action, int? versionID)
        {
            AuditTrail item = new AuditTrail();

            item.EntityID = user == null ? 0 : user.ID;
            item.Type = ItemType.Page;
            item.ItemID = page.ID;
            item.Action = action;
            item.VersionID = versionID;
            item.Message = (string.Format("{0} :: {1} :: {2}", user.Displayname, page.CompletePath, action.ToString()));

            await item.InsertAsync();
        }

        public void Insert()
        {
            var connector = ConnectorFactory.CreateConnector<AuditTrail>();
            connector.Insert(this);
        }

        public async Task InsertAsync()
        {
            var connector = ConnectorFactory.CreateConnector<AuditTrail>();
            await connector.InsertAsync(this);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        public static AuditTrail[] SelectAll()
        {
            var connector = ConnectorFactory.CreateConnector<AuditTrail>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.ID, SortOrder.DESC);
            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        public static async Task<AuditTrail[]> SelectAllAsync()
        {
            var connector = ConnectorFactory.CreateConnector<AuditTrail>();
            var filter = connector.CreateDataFilter();
            filter.AddOrder(x => x.ID, SortOrder.DESC);
            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }
    }
}