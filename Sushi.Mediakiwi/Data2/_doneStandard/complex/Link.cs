using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Sushi.Mediakiwi.Data.DalReflection;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Represents a Link entity.
    /// </summary>
    [DatabaseTable("wim_Links")]
    public class Link : DatabaseEntity, iExportable
    {
     
        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <param name="databaseMapName">Name of the database map.</param>
        /// <returns></returns>
        public static Link SelectOne(int ID, string databaseMapName)
        {
            Link implement = new Link();

            if (!string.IsNullOrEmpty(databaseMapName))
            {
                var connection = Sushi.Mediakiwi.Data.Common.GetCurrentMappingConnectionByName(databaseMapName);
                if (connection != null)
                    implement.SqlConnectionString = connection.Connection;
            }
            return (Link)implement._SelectOne(ID);
        }


        /// <summary>
        /// Selects the one_ import export.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <param name="portal">The portal.</param>
        /// <returns></returns>
        public static Link SelectOne(Guid guid, string portal)
        {
            Link implement = new Link();

            implement.SqlConnectionString = Sushi.Mediakiwi.Data.Common.GetPortal(portal).Connection;
            implement.ConnectionType = Sushi.Mediakiwi.Data.Common.GetPortal(portal).Type;

            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("Link_GUID", SqlDbType.UniqueIdentifier, guid));

            return (Link)implement._SelectOne(where);
        }


        Sushi.Mediakiwi.Data.Site m_CurrentSite;
        Sushi.Mediakiwi.Data.Site CurrentSite
        {
            get
            {
                if (m_CurrentSite == null)
                {
                    if (System.Web.HttpContext.Current == null)
                        return null;

                    if (System.Web.HttpContext.Current.Items["Wim.Site"] == null)
                        return null;

                    m_CurrentSite = System.Web.HttpContext.Current.Items["Wim.Site"] as Sushi.Mediakiwi.Data.Site;
                }
                return m_CurrentSite;
            }
        }

        /// <summary>
        /// Gets the exposed URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public string Url
        {
            get
            {
                return this.GetUrl(CurrentSite);
            }
        }


        #region MOVED to EXTENSION / LOGIC
        
        ///// <summary>
        ///// Gets the URL.
        ///// </summary>
        ///// <param name="currentSite">The current site.</param>
        ///// <returns></returns>
        //public string GetUrl(Sushi.Mediakiwi.Data.Site currentSite)
        //{
        //    return GetUrl(currentSite, true);
        //}

        //string m_Url;
        ///// <summary>
        ///// Gets or sets the URL based on either an internal or external URL
        ///// </summary>
        ///// <param name="currentSite">The current site.</param>
        ///// <param name="onlyReturnPublished">if set to <c>true</c> [only return published].</param>
        ///// <returns></returns>
        ///// <value>The URL.</value>
        //public string GetUrl(Sushi.Mediakiwi.Data.Site currentSite, bool onlyReturnPublished)
        //{
        //    if (string.IsNullOrEmpty(m_Url))
        //    {
        //        if (this.Type == LinkType.InternalAsset)
        //        {
        //            m_Url = this.Asset.DownloadUrl;
        //        }
        //        else if (IsInternal)
        //        {
        //            Sushi.Mediakiwi.Data.Page page = null;
        //            if (PageID.HasValue)
        //            {
        //                if (currentSite == null) page = Sushi.Mediakiwi.Data.Page.SelectOne(PageID.Value, onlyReturnPublished);
        //                else
        //                    page = Sushi.Mediakiwi.Data.Page.SelectOneChild(PageID.Value, currentSite.ID, onlyReturnPublished);
        //            }

        //            if (page == null)
        //                return null;

        //            m_Url = page.HRef;
        //        }
        //        else
        //        {
        //            if (ExternalUrl == null || ExternalUrl.Trim().Length == 0)
        //                return null;

        //            m_Url = ExternalUrl;
        //        }
        //    }
        //    return m_Url;
        //}

        ///// <summary>
        ///// Apply the link properties to a HyperLink control.
        ///// </summary>
        ///// <param name="hyperlink">Hyperlink control to set</param>
        //public void Apply(System.Web.UI.WebControls.HyperLink hyperlink)
        //{
        //    Apply(hyperlink, false);
        //}

        ///// <summary>
        ///// Apply the link properties to a HyperLink control.
        ///// </summary>
        ///// <param name="hyperlink">Hyperlink control to set</param>
        ///// <param name="onlySetNavigationUrl">Should only the navigationUrl be set?</param>
        //public void Apply(System.Web.UI.WebControls.HyperLink hyperlink, bool onlySetNavigationUrl)
        //{
        //    Apply(hyperlink, onlySetNavigationUrl, null);
        //}

        ///// <summary>
        ///// Apply the link properties to a HyperLink control.
        ///// </summary>
        ///// <param name="hyperlink">Hyperlink control to set</param>
        ///// <param name="onlySetNavigationUrl">Should only the navigationUrl be set?</param>
        ///// <param name="currentSite">When inheritence is used apply the current site object to adapt the link.Href for internal links</param>
        //public void Apply(System.Web.UI.WebControls.HyperLink hyperlink, bool onlySetNavigationUrl, Sushi.Mediakiwi.Data.Site currentSite)
        //{
        //    hyperlink.Visible = false;

        //    hyperlink.NavigateUrl = GetUrl(currentSite);

        //    if (!onlySetNavigationUrl)
        //        hyperlink.Text = System.Web.HttpContext.Current.Server.HtmlEncode(Text);

        //    hyperlink.ToolTip = Alt;

        //    if (Target == 2)
        //        hyperlink.Target = "_blank";

        //    hyperlink.Visible = true;
        //}

        #endregion MOVED to EXTENSION / LOGIC

        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

        int m_ID;
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        [DatabaseColumn("Link_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        private Guid m_GUID;
        /// <summary>
        /// Unique identifier used for import/export
        /// </summary>
        /// <value>The GUID.</value>
        [DatabaseColumn("Link_GUID", SqlDbType.UniqueIdentifier, IsMigrationKey = true)]
        public Guid GUID
        {
            get
            {
                if (this.m_GUID == Guid.Empty) this.m_GUID = Guid.NewGuid();
                return m_GUID;
            }
            set { m_GUID = value; }
        }

        string m_Text;
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        [DatabaseColumn("Link_Text", SqlDbType.NVarChar, Length = 500)]
        public string Text
        {
            get { return m_Text; }
            set { m_Text = value; }
        }

        int m_Target;
        /// <summary>
        /// Gets or sets the target.
        /// </summary>
        /// <value>The target.</value>
        [DatabaseColumn("Link_Target", SqlDbType.Int)]
        public int Target
        {
            get { return m_Target; }
            set { m_Target = value; }
        }

        bool m_IsInternal;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is internal.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is internal; otherwise, <c>false</c>.
        /// </value>
        [DatabaseColumn("Link_IsInternal", SqlDbType.Bit)]
        public bool IsInternal
        {
            get { return m_IsInternal; }
            set { m_IsInternal = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public enum LinkType
        {
            /// <summary>
            /// 
            /// </summary>
            Undefined = 0,
            /// <summary>
            /// 
            /// </summary>
            InternalPage = 1,
            /// <summary>
            /// 
            /// </summary>
            ExternalUrl = 2,
            /// <summary>
            /// 
            /// </summary>
            InternalAsset = 3
        }

        public LinkType Type
        {
            get
            {
                if (this.ID == 0) return LinkType.Undefined;
                if (this.PageID.HasValue && this.PageID.Value > 0) return LinkType.InternalPage;
                if (!string.IsNullOrEmpty(this.ExternalUrl)) return LinkType.ExternalUrl;
                if (this.AssetID.HasValue && this.AssetID.Value > 0) return LinkType.InternalAsset;
                return LinkType.Undefined;
            }
        }

        /// <summary>
        /// Gets or sets the page ID.
        /// </summary>
        /// <value>The page ID.</value>
        [DatabaseColumn("Link_Page_Key", SqlDbType.Int, IsNullable = true)]
        public int? PageID { get; set; }

        /// <summary>
        /// Gets or sets the asset ID.
        /// </summary>
        /// <value>The asset ID.</value>
        [DatabaseColumn("Link_Asset_Key", SqlDbType.Int, IsNullable = true)]
        public int? AssetID { get; set; }

        string m_ExternalUrl;
        /// <summary>
        /// Gets or sets the external URL.
        /// </summary>
        /// <value>The external URL.</value>
        [DatabaseColumn("Link_ExternalUrl", SqlDbType.VarChar, Length = 500, IsNullable = true)]
        public string ExternalUrl
        {
            get { return m_ExternalUrl; }
            set { m_ExternalUrl = value; }
        }

        /// <summary>
        /// Gets the target info.
        /// </summary>
        /// <value>The target info.</value>
        public string TargetInfo
        {
            get {

                if (this.Target == 1)
                    return "same frame";
                if (this.Target == 2)
                    return "new screen";
                if (this.Target == 3)
                    return "popup layer";
                if (this.Target == 4)
                    return "parent frame";
                return "unkown";
            }
        }

        string m_Alt;
        /// <summary>
        /// Gets or sets the alt.
        /// </summary>
        /// <value>The alt.</value>
        [DatabaseColumn("Link_Description", SqlDbType.VarChar, Length = 500, IsNullable = true)]
        public string Alt
        {
            get { return m_Alt; }
            set { m_Alt = value; }
        }

        DateTime m_Created;
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        [DatabaseColumn("Link_Created", SqlDbType.DateTime)]
        public DateTime Created
        {
            get
            {
                if (this.m_Created == DateTime.MinValue) this.m_Created = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
                return m_Created;
            }
            set { m_Created = value; }
        }


        public DateTime? Updated
        {
            get { return null; }
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static Link SelectOne(int ID)
        {
            return (Link)new Link()._SelectOne(ID);
        }

        Sushi.Mediakiwi.Data.Asset m_Asset;
        /// <summary>
        /// Gets the asset.
        /// </summary>
        /// <value>The asset.</value>
        public Sushi.Mediakiwi.Data.Asset Asset
        {
            get
            {
                if (m_Asset == null) m_Asset = Asset.SelectOne(this.AssetID.GetValueOrDefault());
                return m_Asset;
            }
        }


        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard

    }
}