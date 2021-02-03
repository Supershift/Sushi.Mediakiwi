using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using Wim.Data.DalReflection;
using Wim.Framework;

namespace Wim.Templates.Templates.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class SimpleGenericsList : ComponentListTemplate
    {
      /// <summary>
        /// Initializes a new instance of the <see cref="GenericList"/> class.
        /// </summary>
        public SimpleGenericsList()
        {
            if (wim.CurrentList.IsSingleInstance)
                wim.CanContainSingleInstancePerDefinedList = true;

            if (wim.CurrentList.CanSortOrder)
            {
                wim.HasSortOrder = true;
                //wim.HasSingleItemSortOrder = true;

                wim.SetSortOrder(wim.CurrentList.Catalog().Table
                    , string.Format("{0}_Key", wim.CurrentList.Catalog().ColumnPrefix)
                    , string.Format("{0}_SortOrder", wim.CurrentList.Catalog().ColumnPrefix));
            }

            this.ListSave += new Wim.Framework.ComponentListEventHandler(GenericList_ListPreSave);
            this.ListSave += new Wim.Framework.ComponentListEventHandler(GenericList_ListSave);
            this.ListLoad += new Wim.Framework.ComponentListEventHandler(GenericList_ListLoad);
            this.ListSearch += new Wim.Framework.ComponentSearchEventHandler(GenericList_ListSearch);

            if (!wim.CurrentList.IsSingleInstance)
                this.ListDelete += new Wim.Framework.ComponentListEventHandler(GenericList_ListDelete);
        }

        /// <summary>
        /// 
        /// </summary>

        Wim.Templates.ISimpleGenerics m_GenericDataInstance;
        /// <summary>
        /// Gets or sets the container.
        /// </summary>
        /// <value>The container.</value>
        [Wim.Framework.ContentListItem.DataExtend()]
        public virtual Wim.Templates.ISimpleGenerics GenericDataInstance
        {
            get { return m_GenericDataInstance; }
            set { 
                m_GenericDataInstance = value;
                if (wim != null && wim.Console != null)
                    wim.Console.CurrentListInstanceItem = value;
            }
        }

        /// <summary>
        /// Handles the ListPreSave event of the GenericList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Wim.Framework.ComponentListEventArgs"/> instance containing the event data.</param>
        void GenericList_ListPreSave(object sender, Wim.Framework.ComponentListEventArgs e)
        {
        }

        /// <summary>
        /// Handles the ListSave event of the GenericList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Wim.Framework.ComponentListEventArgs"/> instance containing the event data.</param>
        public virtual void GenericList_ListSave(object sender, Wim.Framework.ComponentListEventArgs e)
        {
            ((SimpleGenerics)GenericDataInstance).ListID = wim.CurrentList.ID;
            ((SimpleGenerics)GenericDataInstance).SiteID = wim.CurrentSite.ID;
            GenericDataInstance.Save();
        }

        /// <summary>
        /// Handles the ListDelete event of the GenericList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Wim.Framework.ComponentListEventArgs"/> instance containing the event data.</param>
        public virtual void GenericList_ListDelete(object sender, Wim.Framework.ComponentListEventArgs e)
        {
            GenericDataInstance.Delete();
        }

        /// <summary>
        /// Generics the list_ list search.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public virtual void GenericList_ListSearch(object sender, Wim.Framework.ComponentListSearchEventArgs e)
        {
            if (wim.ListDataColumns.List.Count > 0)
                return;

            wim.ListDataColumns.ApplyPropertySearchColumn(wim.CurrentList.ID, "ID");

            Wim.Framework.Templates.SimpleGenericsInstance[] items = Wim.Framework.Templates.SimpleGenericsInstance.SelectAll(wim.CurrentList, wim.CurrentList.Option_Search_MaxResult);
            wim.ListData = items;
        }

        /// <summary>
        /// Handles the ListLoad event of the GenericList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Wim.Framework.ComponentListEventArgs"/> instance containing the event data.</param>
        public virtual void GenericList_ListLoad(object sender, Wim.Framework.ComponentListEventArgs e)
        {
            if (wim.CurrentList.IsSingleInstance)
            {
                GenericDataInstance = Wim.Framework.Templates.SimpleGenericsInstance.SelectSingleInstance(wim.CurrentList.ID, wim.CurrentSite.ID);
            }
            else
                GenericDataInstance = Wim.Framework.Templates.SimpleGenericsInstance.SelectOne(wim.CurrentList.ID, e.SelectedKey);
        }

        System.Web.UI.WebControls.ListItemCollection m_List;
        /// <summary>
        /// Gets the list2.
        /// </summary>
        /// <value>The list2.</value>
        [Wim.Framework.ExposedListCollection("Global list (1:Empty)")]
        public System.Web.UI.WebControls.ListItemCollection List2
        {
            get
            {
                if (m_List == null)
                {
                    m_List = new System.Web.UI.WebControls.ListItemCollection();

                    Wim.Data.PropertySearchColumn searchCol = Wim.Data.PropertySearchColumn.SelectOneHighlight(wim.CurrentList.ID);
                    Wim.Framework.Templates.SimpleGenericsInstance[] instances = Wim.Framework.Templates.SimpleGenericsInstance.SelectAll(wim.CurrentList, wim.CurrentList.Option_Search_MaxResult);

                    m_List.Add(new System.Web.UI.WebControls.ListItem(""));
                    foreach (Wim.Framework.Templates.SimpleGenericsInstance instance in instances)
                    {
                        m_List.Add(new System.Web.UI.WebControls.ListItem(instance.Data[searchCol.Property.FieldName].Value, instance.ID.ToString()));
                    }

                }
                return m_List;
            }
        }


        /// <summary>
        /// Gets the list2.
        /// </summary>
        /// <value>The list2.</value>
        [Wim.Framework.ExposedListCollection("Global list")]
        public System.Web.UI.WebControls.ListItemCollection List3
        {
            get
            {
                if (m_List == null)
                {
                    m_List = new System.Web.UI.WebControls.ListItemCollection();

                    Wim.Data.PropertySearchColumn searchCol = Wim.Data.PropertySearchColumn.SelectOneHighlight(wim.CurrentList.ID);
                    Wim.Framework.Templates.SimpleGenericsInstance[] instances = Wim.Framework.Templates.SimpleGenericsInstance.SelectAll(wim.CurrentList, wim.CurrentList.Option_Search_MaxResult);

                    foreach (Wim.Framework.Templates.SimpleGenericsInstance instance in instances)
                    {
                        m_List.Add(new System.Web.UI.WebControls.ListItem(instance.Data[searchCol.Property.FieldName].Value, instance.ID.ToString()));
                    }

                }
                return m_List;
            }
        }
    }
}

