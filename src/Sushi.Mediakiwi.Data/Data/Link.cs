using Sushi.Mediakiwi.Data.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Represents a Link entity.
    /// </summary>
    [DataMap(typeof(LinkMap))]
    public class Link : IExportable
    {
        public class LinkMap : DataMap<Link>
        {
            public LinkMap()
            {
                Table("wim_Links");
                Id(x => x.ID, "Link_Key").Identity();
                Map(x => x.GUID, "Link_GUID");
                Map(x => x.Text, "Link_Text").Length(500);
                Map(x => x.Target, "Link_Target");
                Map(x => x.IsInternal, "Link_IsInternal");
                Map(x => x.PageID, "Link_Page_Key");
                Map(x => x.AssetID, "Link_Asset_Key");
                Map(x => x.ExternalUrl, "Link_ExternalUrl").Length(500);
                Map(x => x.Alt, "Link_Description").Length(500);
                Map(x => x.Created, "Link_Created");
            }
        }

        #region Properties

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public int ID { get; set; }

        private Guid m_GUID;

        /// <summary>
        /// Unique identifier used for import/export
        /// </summary>
        /// <value>The GUID.</value>
        public Guid GUID
        {
            get
            {
                if (m_GUID == Guid.Empty)
                {
                    m_GUID = Guid.NewGuid();
                }
                return m_GUID;
            }
            set { m_GUID = value; }
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the target.
        /// </summary>
        /// <value>The target.</value>
        public int Target { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is internal.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is internal; otherwise, <c>false</c>.
        /// </value>
        public bool IsInternal { get; set; }

        public LinkType Type
        {
            get
            {
                if (ID == 0)
                {
                    return LinkType.Undefined;
                }

                if (PageID.HasValue && PageID.Value > 0)
                {
                    return LinkType.InternalPage;
                }

                if (!string.IsNullOrWhiteSpace(ExternalUrl))
                {
                    return LinkType.ExternalUrl;
                }

                if (AssetID.HasValue && AssetID.Value > 0)
                {
                    return LinkType.InternalAsset;
                }

                return LinkType.Undefined;
            }
        }

        Asset m_Asset;
        /// <summary>
        /// Gets the asset.
        /// </summary>
        /// <value>The asset.</value>
        public Asset Asset
        {
            get
            {
                if (m_Asset == null && AssetID.GetValueOrDefault() > 0)
                {
                    m_Asset = Asset.SelectOne(AssetID.GetValueOrDefault());
                }
                return m_Asset;
            }
        }

        /// <summary>
        /// Gets or sets the page ID.
        /// </summary>
        /// <value>The page ID.</value>
        public int? PageID { get; set; }

        /// <summary>
        /// Gets or sets the asset ID.
        /// </summary>
        /// <value>The asset ID.</value>
        public int? AssetID { get; set; }

        /// <summary>
        /// Gets or sets the external URL.
        /// </summary>
        /// <value>The external URL.</value>
        public string ExternalUrl { get; set; }

        /// <summary>
        /// Gets or sets the alt.
        /// </summary>
        /// <value>The alt.</value>
        public string Alt { get; set; }

        /// <summary>
        /// Gets the target info.
        /// </summary>
        /// <value>The target info.</value>
        public string TargetInfo
        {
            get
            {
                switch (Target)
                {
                    default: return "unkown";
                    case 1: return "same frame";
                    case 2: return "new screen";
                    case 3: return "popup layer";
                    case 4: return "parent frame";
                }
            }
        }

        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created { get; set; }

        public DateTime? Updated
        {
            get { return null; }
        }
        #endregion Properties

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static Link SelectOne(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<Link>();
            return connector.FetchSingle(ID);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static async Task<Link> SelectOneAsync(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<Link>();
            return await connector.FetchSingleAsync(ID).ConfigureAwait(false);
        }

        /// <summary>
        /// Saves this instance
        /// </summary>
        public void Save()
        {
            var connector = ConnectorFactory.CreateConnector<Link>();
            connector.Save(this);
        }

        /// <summary>
        /// Saves this instance Async
        /// </summary>
        public async Task SaveAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Link>();
            await connector.SaveAsync(this).ConfigureAwait(false);
        }

        /// <summary>
        /// Saves this instance Async
        /// </summary>
        public async Task DeleteAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Link>();
            await connector.DeleteAsync(this).ConfigureAwait(false);
        }
    }
}