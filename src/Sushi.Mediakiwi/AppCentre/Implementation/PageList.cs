using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Sushi.Mediakiwi.Framework;
using System.Globalization;
using System.Collections;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    class PageList : BaseImplementation
    {
        //private bool m_OpenInPopup;
        ///// <summary>
        ///// Gets or sets the title.
        ///// </summary>
        ///// <value>The title.</value>
        //[Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Open in popup")]
        //public bool OpenInPopup
        //{
        //    get { return m_OpenInPopup; }
        //    set { m_OpenInPopup = value; }
        //}

        public PageList()
        {
            wim.CanAddNewItem = false;
            wim.OpenInEditMode = true;

            this.ListLoad += new ComponentListEventHandler(PageList_ListLoad);
            this.ListSearch += new ComponentSearchEventHandler(PageList_ListSearch);
        }

        void PageList_ListSearch(object sender, ComponentListSearchEventArgs e)
        {
            wim.ListDataColumns.Add("", "ID", ListDataColumnType.UniqueIdentifier);
            wim.ListDataColumns.Add("Name", "Name", ListDataColumnType.HighlightPresent);
            wim.ListDataColumns.Add("Path", "HRef");
            wim.ListDataColumns.Add("Published", "IsPublished", 60);
            wim.ListDataColumns.Add("Modified", "Updated", 90);

            int groupElementId = Wim.Utility.ConvertToInt(e.SelectedGroupItemKey);
            wim.ListData = Sushi.Mediakiwi.Data.Page.SelectAllBasedOnPageTemplate(groupElementId);
        }

        void PageList_ListLoad(object sender, ComponentListEventArgs e)
        {
            if (e.SelectedKey == 0)
                return;

            Sushi.Mediakiwi.Data.Page page = Sushi.Mediakiwi.Data.Page.SelectOne(e.SelectedKey, false);
            if (page != null && !page.IsNewInstance)
            {
                Response.Redirect(string.Concat(wim.Console.WimPagePath, "?page=", page.ID));
            }
        }

        Sushi.Mediakiwi.Data.DataList m_List;
        [Sushi.Mediakiwi.Framework.ContentListItem.DataList("63c0c71c-e301-4a29-9a75-73874cb6622e")]
        public Sushi.Mediakiwi.Data.DataList List
        {
            get { return m_List; }
            set { m_List = value; }
        }
    }
}
