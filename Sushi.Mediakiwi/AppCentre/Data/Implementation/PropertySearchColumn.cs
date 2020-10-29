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
    /// Represents a PropertySearchColumn entity.
    /// </summary>
    public class PropertySearchColumn : BaseImplementation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentListInstance"/> class.
        /// </summary>
        public PropertySearchColumn()
        {
            //wim.ShowInFullWidthMode = true;
            wim.OpenInEditMode = true;
            wim.HasSortOrder = true;
            wim.CanSaveAndAddNew = true;
            wim.HasSingleItemSortOrder = true;
            
            wim.SetSortOrder("wim_PropertySearchColumns", "PropertySearchColumn_Key", "PropertySearchColumn_SortOrder");

            this.ListSave += new ComponentListEventHandler(PropertySearchColumn_ListSave);
            this.ListLoad += new ComponentListEventHandler(PropertySearchColumn_ListLoad);
            this.ListSearch += new ComponentSearchEventHandler(PropertySearchColumn_ListSearch);
            this.ListDelete += new ComponentListEventHandler(PropertySearchColumn_ListDelete);
        }

        private int m_SearchID;
        /// <summary>
        /// Gets or sets the search ID.
        /// </summary>
        /// <value>The search ID.</value>
        [Sushi.Mediakiwi.Framework.ContentListSearchItem.TextField("ID", 255, false)]
        public int SearchID
        {
            get { return m_SearchID; }
            set { m_SearchID = value; }
        }

        private int m_SearchTypeID;
        /// <summary>
        /// Gets or sets the search type ID.
        /// </summary>
        /// <value>The search type ID.</value>
        [Sushi.Mediakiwi.Framework.ContentListSearchItem.TextField("TypeID", 255, false)]
        public int SearchTypeID
        {
            get { return m_SearchTypeID; }
            set { m_SearchTypeID = value; }
        }

        void PropertySearchColumn_ListSearch(object sender, ComponentListSearchEventArgs e)
        {
            wim.ListDataColumns.Add("ID", "ID", ListDataColumnType.UniqueIdentifier);
            wim.ListDataColumns.Add("Title", "DisplayTitle");
            wim.ListDataColumns.Add("Column width", "ColumnWidth");
            wim.ListDataColumns.Add("Is highlight", "IsHighlight");
            wim.ListDataColumns.Add("Only export", "IsOnlyExport");

            wim.ListData = Sushi.Mediakiwi.Data.PropertySearchColumn.SelectAll(wim.Console.GroupItem.Value);
        }

        /// <summary>
        /// Handles the ListLoad event of the ComponentList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentListEventArgs"/> instance containing the event data.</param>
        void PropertySearchColumn_ListLoad(object sender, ComponentListEventArgs e)
        {


            Implement = Sushi.Mediakiwi.Data.PropertySearchColumn.SelectOne(e.SelectedKey);
        }

        /// <summary>
        /// Handles the ListDelete event of the ComponentList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentListEventArgs"/> instance containing the event data.</param>
        void PropertySearchColumn_ListDelete(object sender, ComponentListEventArgs e)
        {
            Implement.Delete();
        }

        /// <summary>
        /// Handles the ListSave event of the ComponentList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentListEventArgs"/> instance containing the event data.</param>
        void PropertySearchColumn_ListSave(object sender, ComponentListEventArgs e)
        {
            Implement.ListID = wim.Console.GroupItem.Value;
            Implement.Save();
        }

        Sushi.Mediakiwi.Data.PropertySearchColumn m_Implement;
        /// <summary>
        /// Gets or sets the implement.
        /// </summary>
        /// <value>The implement.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.DataExtend()]
        public Sushi.Mediakiwi.Data.PropertySearchColumn Implement
        {
            get { return m_Implement; }
            set { m_Implement = value; }
        }


        private Sushi.Mediakiwi.Data.DataList m_List;
        /// <summary>
        /// Gets or sets the list.
        /// </summary>
        /// <value>The list.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.DataList("C7B2C21D-0121-4E58-A0C9-616B31C2E1B1")]
        public Sushi.Mediakiwi.Data.DataList List
        {
            get { return m_List; }
            set { m_List = value; }
        }

        ListItemCollection m_Properties;
        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>The properties.</value>
        public ListItemCollection Properties
        {
            get
            {
                if (m_Properties == null)
                {
                    m_Properties = new ListItemCollection();

                    foreach(Sushi.Mediakiwi.Data.Property p in Sushi.Mediakiwi.Data.Property.SelectAll(wim.Console.GroupItem.Value))
                        m_Properties.Add(new ListItem(p.Title, p.ID.ToString()));
                }
                return m_Properties;
            }
        }

        ListItemCollection m_TotalTypes;
        /// <summary>
        /// Gets the total types.
        /// </summary>
        /// <value>The total types.</value>
        public ListItemCollection TotalTypes
        {
            get
            {
                if (m_TotalTypes == null)
                {
                    m_TotalTypes = new ListItemCollection();

                    m_TotalTypes.Add(new ListItem("Default", ((int)ListDataTotalType.Default).ToString()));
                    m_TotalTypes.Add(new ListItem("Sum", ((int)ListDataTotalType.Sum).ToString()));
                    m_TotalTypes.Add(new ListItem("Average", ((int)ListDataTotalType.Average).ToString()));
                }
                return m_TotalTypes;
            }
        }
    }
}
