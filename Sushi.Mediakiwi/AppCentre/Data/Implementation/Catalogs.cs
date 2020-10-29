using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    /// <summary>
    /// 
    /// </summary>
    public class Catalogs : BaseImplementation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Catalogs"/> class.
        /// </summary>
        public Catalogs()
        {
            wim.OpenInEditMode = true;
            this.ListSave += new ComponentListEventHandler(Catalogs_ListSave);
            this.ListDelete += new ComponentListEventHandler(Catalogs_ListDelete);
            this.ListLoad += new ComponentListEventHandler(Catalogs_ListLoad);
            this.ListSearch += new ComponentSearchEventHandler(Catalogs_ListSearch);
            this.ListPreRender += new ComponentListEventHandler(Catalogs_ListPreRender);
        }

        void Catalogs_ListPreRender(object sender, ComponentListEventArgs e)
        {
            //if (!IsPostBack) return;
            //if (!Implement.ValidateSqlTableKey() && string.IsNullOrEmpty(Implement.ColumnKey))
            //{
            //    wim.Notification.AddError(string.Format("There is no primary key column found with the name {0}_Key (int). This has to be added manualy.", Implement.ColumnPrefix));
            //    wim.Notification.AddError("Table", "");

            //    if (!Implement.ValidateSqlTableGuid() && string.IsNullOrEmpty(Implement.ColumnGuid))
            //    {
            //        wim.Notification.AddError(string.Format("There is no migration key column found with the name {0}_GUID (uniqueindentifier). This has to be added manualy.", Implement.ColumnPrefix));
            //    }
            //    return;
            //}
            //if (!Implement.ValidateSqlTableGuid() && string.IsNullOrEmpty(Implement.ColumnGuid))
            //{
            //    wim.Notification.AddError(string.Format("There is no migration key column found with the name {0}_GUID (uniqueindentifier). This has to be added manualy.", Implement.ColumnPrefix));
            //    wim.Notification.AddError("Table", "");
            //}
        }

        void Catalogs_ListSave(object sender, ComponentListEventArgs e)
        {
            Implement.Save();
            //string message;
            ////Implement.ValidateSqlTableCreation(out message);
            //wim.Notification.AddNotification(message);
        }

        void Catalogs_ListDelete(object sender, ComponentListEventArgs e)
        {
            Implement.Delete();
        }

        void Catalogs_ListLoad(object sender, ComponentListEventArgs e)
        {
            Implement = Sushi.Mediakiwi.Data.Catalog.SelectOne(e.SelectedKey);
            if (e.SelectedKey == 0 && !IsPostBack)
                Implement.Table = "cat_";
        }

        void Catalogs_ListSearch(object sender, ComponentListSearchEventArgs e)
        {
            wim.CanAddNewItem = true;
            wim.ListDataColumns.Add("ID", "ID", ListDataColumnType.UniqueIdentifier);
            wim.ListDataColumns.Add("Title", "Title", ListDataColumnType.HighlightPresent);
            wim.ListDataColumns.Add("Table", "Table");
            wim.ListDataColumns.Add("IsActive", "IsActive");
            wim.ListDataColumns.Add("Created", "Created");

            wim.ListData = Sushi.Mediakiwi.Data.Catalog.SelectAll();
        }


        Sushi.Mediakiwi.Data.Catalog m_Implement;
        /// <summary>
        /// Gets or sets the implement.
        /// </summary>
        /// <value>The implement.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.DataExtend()]
        public Sushi.Mediakiwi.Data.Catalog Implement
        {
            get { return m_Implement; }
            set { m_Implement = value; }
        }


        ListItemCollection m_Connections;
        /// <summary>
        /// Gets the connections.
        /// </summary>
        /// <value>The connections.</value>
        public ListItemCollection Connections
        {
            get
            {
                if (m_Connections == null)
                {
                    m_Connections = new ListItemCollection();
                    
                    m_Connections.Add(new ListItem(Sushi.Mediakiwi.Data.Common.CurrentPortal.Name, "0"));

                    if (!string.IsNullOrEmpty(Sushi.Mediakiwi.Data.Common.CurrentPortal.Connection1))
                        m_Connections.Add(new ListItem(Sushi.Mediakiwi.Data.Common.CurrentPortal.Name1, "1"));
                }
                return m_Connections;
            }
        }

        ListItemCollection m_Portals;
        /// <summary>
        /// Gets the connections.
        /// </summary>
        /// <value>The connections.</value>
        public ListItemCollection Portals
        {
            get
            {
                if (m_Portals == null)
                {
                    m_Portals = new ListItemCollection();
                    Sushi.Mediakiwi.Framework.WimServerConfiguration config = Sushi.Mediakiwi.Framework.WimServerConfiguration.GetConfig();
                    if (config != null && config.PortalCollection != null)
                    {
                        foreach (Sushi.Mediakiwi.Framework.WimServerPortal portal in config.PortalCollection)
                        {
                            m_Portals.Add(new ListItem(portal.Name));
                        }
                    }
                }
                return m_Portals;
            }
        }
    }
}
