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
    public class GenericList : ComponentListTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericList"/> class.
        /// </summary>
        public GenericList()
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
            this.ListPreRender += GenericList_ListPreRender;
            this.ListSave += new Wim.Framework.ComponentListEventHandler(GenericList_ListPreSave);
            this.ListSave += new Wim.Framework.ComponentListEventHandler(GenericList_ListSave);
            this.ListLoad += new Wim.Framework.ComponentListEventHandler(GenericList_ListLoad);
            this.ListSearch += new Wim.Framework.ComponentSearchEventHandler(GenericList_ListSearch);
            this.ListAction += new Framework.ComponentActionEventHandler(GenericList_ListAction);

            if (!wim.CurrentList.IsSingleInstance)
                this.ListDelete += new Wim.Framework.ComponentListEventHandler(GenericList_ListDelete);
        }

     

        [Wim.Framework.OnlyVisibleWhenTrue("ShowCopyInheritedContent"), Wim.Framework.ContentListItem.Button("Copy inherited content", IconTarget = Wim.Framework.ButtonTarget.TopRight, IconType = Wim.Framework.ButtonIconType.Sorting, AskConfirmation = true)]
        public bool CopyInheritedContent { get; set; }
        public bool ShowCopyInheritedContent { get; set; }

        void GenericList_ListAction(object sender, Framework.ComponentActionEventArgs e)
        {
            if (CopyInheritedContent)
            {
                if (wim.CurrentList.IsSingleInstance)
                {
                    int masterID = wim.CurrentSite.MasterID.Value;
                    var masterData = Wim.Framework.Templates.GenericInstance.SelectSingleInstance(wim.CurrentList.ID, masterID);
                    var currentData = Wim.Framework.Templates.GenericInstance.SelectSingleInstance(wim.CurrentList.ID, wim.CurrentSite.ID);

                    currentData.ListID = wim.CurrentList.ID;
                    currentData.SiteID = wim.CurrentSite.ID;
                    currentData.Data.CopyFromMaster(wim.CurrentList.ID, masterData.Data, wim.CurrentSite.ID);
                    currentData.Save();

                    Response.Redirect(wim.GetCurrentQueryUrl(true));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>

        Wim.Templates.IGeneric m_GenericDataInstance;
        /// <summary>
        /// Gets or sets the container.
        /// </summary>
        /// <value>The container.</value>
        [Wim.Framework.ContentListItem.DataExtend()]
        public virtual Wim.Templates.IGeneric GenericDataInstance
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
            GenericDataInstance.SiteID = wim.CurrentSite.ID;
            GenericDataInstance.ListID = wim.CurrentList.ID;
        }

        private void GenericList_ListPreRender(IComponentListTemplate sender, ComponentListEventArgs e)
        {
        }

        /// <summary>
        /// Handles the ListSave event of the GenericList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Wim.Framework.ComponentListEventArgs"/> instance containing the event data.</param>
        public virtual void GenericList_ListSave(object sender, Wim.Framework.ComponentListEventArgs e)
        {
            GenericDataInstance.Save();

            //foreach (Wim.Data.Property prop in Wim.Data.Property.SelectAll(wim.CurrentList.ID))
            //{
            //    if (prop.IsFilter)
            //    {
            //        Wim.Data.DataFilter filter = Wim.Data.DataFilter.SelectOne(prop.ID, GenericDataInstance.ID);
            //        prop.ApplyFilter(filter, GenericDataInstance.Data, GenericDataInstance.ID);
            //    }
            //}
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
            //if (wim.CurrentList.ClassName != "Wim.Templates.Templates.UI.GenericList")
            //    return;

            if (wim.ListDataColumns.List.Count > 0)
                return;

            //wim.CanAddNewItem = true;
            
            wim.ListDataColumns.ApplyPropertySearchColumn(wim.CurrentList.ID, "ID");

            Wim.Framework.Templates.GenericInstance[] items = Wim.Framework.Templates.GenericInstance.SelectAll(wim.CurrentList.ID, wim.CurrentSite.ID, wim.CurrentList.Option_Search_MaxResult);
            wim.ListData = items;
        }

        System.Web.UI.WebControls.ListItemCollection m_List;
        /// <summary>
        /// Gets the list.
        /// </summary>
        /// <value>The list.</value>
        [Wim.Framework.ExposedListCollection("Site specific list (1:Empty)")]
        public System.Web.UI.WebControls.ListItemCollection List
        {
            get
            {
                if (m_List == null)
                {
                    m_List = new System.Web.UI.WebControls.ListItemCollection();

                    Wim.Data.PropertySearchColumn searchCol = Wim.Data.PropertySearchColumn.SelectOneHighlight(wim.CurrentList.ID);
                    Wim.Framework.Templates.GenericInstance[] instances = Wim.Framework.Templates.GenericInstance.SelectAll(wim.CurrentList.ID, wim.CurrentSite == null ? 0 : wim.CurrentSite.ID, wim.CurrentList.Option_Search_MaxResult);

                    m_List.Add(new System.Web.UI.WebControls.ListItem(""));
                    foreach (Wim.Framework.Templates.GenericInstance instance in instances)
                    {
                        m_List.Add(new System.Web.UI.WebControls.ListItem(instance.Data[searchCol.Property.FieldName].Value, instance.ID.ToString()));
                    }

                }
                return m_List;
            }
        }


        /// <summary>
        /// Gets the list.
        /// </summary>
        /// <value>The list.</value>
        [Wim.Framework.ExposedListCollection("Site specific list")]
        public System.Web.UI.WebControls.ListItemCollection List1
        {
            get
            {
                if (m_List == null)
                {
                    m_List = new System.Web.UI.WebControls.ListItemCollection();

                    Wim.Data.PropertySearchColumn searchCol = Wim.Data.PropertySearchColumn.SelectOneHighlight(wim.CurrentList.ID);
                    Wim.Framework.Templates.GenericInstance[] instances = Wim.Framework.Templates.GenericInstance.SelectAll(wim.CurrentList.ID, wim.CurrentSite == null ? 0 : wim.CurrentSite.ID, wim.CurrentList.Option_Search_MaxResult);

                    foreach (Wim.Framework.Templates.GenericInstance instance in instances)
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
        [Wim.Framework.ExposedListCollection("Global list (1:Empty)")]
        public System.Web.UI.WebControls.ListItemCollection List2
        {
            get
            {
                if (m_List == null)
                {
                    m_List = new System.Web.UI.WebControls.ListItemCollection();

                    Wim.Data.PropertySearchColumn searchCol = Wim.Data.PropertySearchColumn.SelectOneHighlight(wim.CurrentList.ID);
                    Wim.Framework.Templates.GenericInstance[] instances = Wim.Framework.Templates.GenericInstance.SelectAll(wim.CurrentList.ID, null, wim.CurrentList.Option_Search_MaxResult);

                    m_List.Add(new System.Web.UI.WebControls.ListItem(""));
                    foreach (Wim.Framework.Templates.GenericInstance instance in instances)
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
                    Wim.Framework.Templates.GenericInstance[] instances = Wim.Framework.Templates.GenericInstance.SelectAll(wim.CurrentList.ID, null, wim.CurrentList.Option_Search_MaxResult);

                    foreach (Wim.Framework.Templates.GenericInstance instance in instances)
                    {
                        m_List.Add(new System.Web.UI.WebControls.ListItem(instance.Data[searchCol.Property.FieldName].Value, instance.ID.ToString()));
                    }

                }
                return m_List;
            }
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
                if (wim.CurrentSite.MasterID.HasValue)
                    ShowCopyInheritedContent = true;
                GenericDataInstance = Wim.Framework.Templates.GenericInstance.SelectSingleInstance(wim.CurrentList.ID, wim.CurrentSite.ID);
            }
            else
                GenericDataInstance = Wim.Framework.Templates.GenericInstance.SelectOne(wim.CurrentList.ID, e.SelectedKey);
        }
    }
}
