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
    /// Represents a ComponentList entity.
    /// </summary>
    public class SharedComponentList : ComponentListTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SharedComponentList"/> class.
        /// </summary>
        public SharedComponentList()
        {
            //  Component version
            wim.OpenInEditMode = false;

            if (Request.QueryString["item"] != "0")
                wim.ItemIsComponent = true;

            wim.HideOpenCloseToggle = true;
            wim.CanSaveAndAddNew = false;
            wim.CanAddNewItem = true;
            wim.ShowInFullWidthMode = false;

            this.ListSearch += new ComponentSearchEventHandler(SharedComponentList_ListSearch);
            this.ListLoad += new ComponentListEventHandler(SharedComponentList_ListLoad);
            this.ListSave += new ComponentListEventHandler(SharedComponentList_ListSave);
            this.ListDelete += new ComponentListEventHandler(SharedComponentList_ListDelete);
        }

        void SharedComponentList_ListDelete(object sender, ComponentListEventArgs e)
        {
            Sushi.Mediakiwi.Data.ComponentVersion version = Sushi.Mediakiwi.Data.ComponentVersion.SelectOne(e.SelectedKey);
            version.Delete();
        }

        void SharedComponentList_ListSave(object sender, ComponentListEventArgs e)
        {
            if (e.SelectedKey > 0)
            {
                return;
            }
            else
            {
                Sushi.Mediakiwi.Data.ComponentVersion version = new Sushi.Mediakiwi.Data.ComponentVersion();
                version.TemplateID = this.TemplateID;
                version.InstanceName = Name;
                version.IsActive = true;
                version.SiteID = wim.CurrentSite.ID;
                version.Save();
            }
        }

        void SharedComponentList_ListSearch(object sender, ComponentListSearchEventArgs e)
        {
            wim.ListDataColumns.Add("ID", "ID", ListDataColumnType.UniqueIdentifier);
            wim.ListDataColumns.Add("Name", "InstanceName", ListDataColumnType.HighlightPresent);
            wim.ListDataColumns.Add("Date", "Created", 80);

            wim.ListData = Sushi.Mediakiwi.Data.ComponentVersion.SelectAllSharedForSite(wim.CurrentSite.ID);
        }

        void SharedComponentList_ListLoad(object sender, ComponentListEventArgs e)
        {
            if (e.SelectedKey > 0)
                wim.AddTab(new Guid("2b951c36-fb88-45d3-9a23-ab7f18948c64"), e.SelectedKey);
        }

        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Name", 50, true)]
        public string Name { get; set; }

        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Template", "TemplateList", true)]
        public int TemplateID { get; set; }

        ListItemCollection m_TemplateList;
        public ListItemCollection TemplateList
        {
            get
            {
                if (m_TemplateList == null)
                {
                    m_TemplateList = new ListItemCollection();
                    m_TemplateList.Add(new ListItem(""));
                    foreach (var t in Sushi.Mediakiwi.Data.ComponentTemplate.SelectAllShared())
                    {
                        m_TemplateList.Add(new ListItem(t.Name, t.ID.ToString()));
                    }
                }
                return m_TemplateList;
            }
        }
    }
}
