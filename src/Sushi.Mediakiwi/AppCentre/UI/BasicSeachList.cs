﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Web;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.UI;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.AppCentre.Data
{
    public class BasicSearchList : ComponentListTemplate
    {
        public BasicSearchList()
        {
            this.ListSearch += BasicSeachList_ListSearch;
        }


        [Sushi.Mediakiwi.Framework.ContentListSearchItem.TextField("Search for", 50, AutoPostBack = false, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public string FilterText { get; set; }

        [Sushi.Mediakiwi.Framework.ContentListSearchItem.Choice_Dropdown("Type", "FilterTypes", false, false, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public int? FilterType { get; set; }

        [Sushi.Mediakiwi.Framework.ContentListSearchItem.Choice_Dropdown("Channel", "SearchSites", false, false, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Right)]
        public int? FilterSite { get; set; }

        private ListItemCollection m_SearchSites;
        /// <summary>
        /// Gets the search sites.
        /// </summary>
        /// <value>The search sites.</value>
        public ListItemCollection SearchSites
        {
            get
            {
                if (m_SearchSites != null) return m_SearchSites;

                m_SearchSites = new ListItemCollection();
                ListItem li;
                m_SearchSites.Add(new ListItem("", ""));

                foreach (Sushi.Mediakiwi.Data.Site site in Sushi.Mediakiwi.Data.Site.SelectAll())
                {
                    //if (site.MasterID.GetValueOrDefault() > 0) continue;
                    li = new ListItem(site.Name, site.ID.ToString());
                    m_SearchSites.Add(li);
                }
                return m_SearchSites;
            }
        }

        private ListItemCollection m_FilterTypes;
        /// <summary>
        /// Gets the search sites.
        /// </summary>
        /// <value>The search sites.</value>
        public ListItemCollection FilterTypes
        {
            get
            {
                if (m_FilterTypes == null)
                {
                    m_FilterTypes = new ListItemCollection();
                    m_FilterTypes.Add(new ListItem(""));
                    m_FilterTypes.Add(new ListItem("List", "1"));
                    m_FilterTypes.Add(new ListItem("Folder", "2"));
                    m_FilterTypes.Add(new ListItem("Folder/container", "8"));
                    m_FilterTypes.Add(new ListItem("Page", "3"));
                    m_FilterTypes.Add(new ListItem("Dashboard", "4"));
                    m_FilterTypes.Add(new ListItem("Document gallery", "5"));
                    m_FilterTypes.Add(new ListItem("Website", "6"));
                    m_FilterTypes.Add(new ListItem("Section", "7"));
                }
                return m_FilterTypes;
            }
        }

        async Task BasicSeachList_ListSearch(ComponentListSearchEventArgs e)
        {
            wim.CanAddNewItem = true;
            wim.ListDataColumns.Add(new ListDataColumn("ID", "ID", ListDataColumnType.UniqueIdentifier));
            wim.ListDataColumns.Add(new ListDataColumn("Title", "Title"));
            wim.ListDataColumns.Add(new ListDataColumn("", "TitleHighlighted", ListDataColumnType.Highlight));
            
            wim.ListDataColumns.Add(new ListDataColumn("Description", "Description"));
            wim.ListDataColumns.Add(new ListDataColumn("Type", "Type") { ColumnWidth = 70 } );

            if (string.IsNullOrEmpty(this.FilterText) && this.FilterType == 0)
                return;

            wim.ListDataAdd(await Sushi.Mediakiwi.Data.SearchView.SelectAllAsync(FilterSite, FilterType, FilterText));
        }
    }
}