using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.Data.DalReflection;
using System.Web.UI.WebControls;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    /// <summary>
    /// 
    /// </summary>
    public class PropertyOption : ComponentListTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyOption"/> class.
        /// </summary>
        public PropertyOption()
        {
            //wim.ShowInFullWidthMode = true;
            wim.OpenInEditMode = true;
            wim.CanAddNewItem = true;
            wim.HasSortOrder = true;
            wim.HasSingleItemSortOrder = true;
            wim.SetSortOrder("wim_PropertyOptions", "PropertyOption_Key", "PropertyOption_SortOrder");

            this.ListLoad += new ComponentListEventHandler(PropertyOption_ListLoad);
            this.ListSave += new ComponentListEventHandler(PropertyOption_ListSave);
            this.ListSearch += new ComponentSearchEventHandler(PropertyOption_ListSearch);
            this.ListDelete += new ComponentListEventHandler(PropertyOption_ListDelete);
        }

        void PropertyOption_ListDelete(object sender, ComponentListEventArgs e)
        {
            Wim.Utilities.CacheItemManager.FlushIndexOfCacheObjects("Data_Sushi.Mediakiwi.Data.Property");
            Implement.Delete();
        }

        void PropertyOption_ListSearch(object sender, ComponentListSearchEventArgs e)
        {
            wim.CanAddNewItem = true;

            wim.ListDataColumns.Add("ID", "ID", Sushi.Mediakiwi.Framework.ListDataColumnType.UniqueIdentifierPresent, 30);
            wim.ListDataColumns.Add("Name", "Name", Sushi.Mediakiwi.Framework.ListDataColumnType.HighlightPresent);
            wim.ListDataColumns.Add("Value", "Value");

            int propertyID = Convert.ToInt32(Request.QueryString["group2item"]);
            wim.ListData = Sushi.Mediakiwi.Data.PropertyOption.SelectAll(propertyID);
        }


        void PropertyOption_ListSave(object sender, ComponentListEventArgs e)
        {
            Wim.Utilities.CacheItemManager.FlushIndexOfCacheObjects("Data_Sushi.Mediakiwi.Data.Property");
            Implement.Save();
        }

        void PropertyOption_ListLoad(object sender, ComponentListEventArgs e)
        {
            wim.CanAddNewItem = true;

            int propertyID = Convert.ToInt32(Request.QueryString["group2item"]);
            Sushi.Mediakiwi.Data.Property p = Sushi.Mediakiwi.Data.Property.SelectOne(propertyID);
            m_Element = p.Title;
            Implement = Sushi.Mediakiwi.Data.PropertyOption.SelectOne(e.SelectedKey);
            Implement.PropertyID = propertyID;
        }

        string m_Element;
        /// <summary>
        /// Gets or sets the element.
        /// </summary>
        /// <value>The element.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextLine("Element label")]
        public string Element
        {
            get { return m_Element; }
            set { m_Element = value; }
        }

        Sushi.Mediakiwi.Data.PropertyOption m_Implement;
        /// <summary>
        /// Gets or sets the implement.
        /// </summary>
        /// <value>The implement.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.DataExtend()]
        public Sushi.Mediakiwi.Data.PropertyOption Implement
        {
            get { return m_Implement; }
            set { m_Implement = value; }
        }

        string m_List;
        /// <summary>
        /// Gets or sets the list.
        /// </summary>
        /// <value>The list.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.DataList("9CA13663-54E0-4701-9658-E2DDDF769F78")]
        public string List
        {
            get { return m_List; }
            set { m_List = value; }
        }
    }
}
