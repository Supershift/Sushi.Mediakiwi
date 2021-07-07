using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Collections;
using Sushi.Mediakiwi.AppCentre.Data.Implementation;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.UI;

namespace Wim.AppCentre.Data.Implementation
{
    public class PageList : BaseImplementation
    {
        public class PageHref
        {
            public string Href { get; set; }
            public string Text { get; set; }
        }

        public PageList()
        {
            wim.CanAddNewItem = false;
            wim.OpenInEditMode = true;

            this.ListLoad += PageList_ListLoad;
            this.ListSearch += PageList_ListSearch;
            this.ListHeadless += PageList_ListHeadless;
        }

        private async Task PageList_ListHeadless(HeadlessRequest e)
        {
            var page = await Sushi.Mediakiwi.Data.Page.SelectOneAsync(e.Listitem.ID).ConfigureAwait(false);
            if (page == null || page.ID == 0)
                return;

            e.Result = new PageHref
            {
                Href = Sushi.Mediakiwi.Data.Utility.ConvertUrl(page.InternalPath),
                Text = string.IsNullOrWhiteSpace(page.LinkText) ? page.Name : page.LinkText
            };
        }

        async Task PageList_ListSearch(ComponentListSearchEventArgs e)
        {
            wim.ListDataColumns.Add("", "ID", ListDataColumnType.UniqueIdentifier);
            wim.ListDataColumns.Add("Path", "InternalPath", ListDataColumnType.HighlightPresent);
            wim.ListDataColumns.Add("Published", "IsPublished", 60);
            wim.ListDataColumns.Add("Modified", "Updated", 90);

            if (e.SelectedGroupItemKey == 0)
            {
                if (string.IsNullOrWhiteSpace(this.FilterText))
                {
                    wim.ListDataApply(await Page.SelectAllAsync().ConfigureAwait(false));
                }
                else
                {
                    wim.ListDataApply(await Page.SelectAllAsync(this.FilterText, true).ConfigureAwait(false));
                }
            }
            else
            {
                int groupElementId = Utility.ConvertToInt(e.SelectedGroupItemKey);
                wim.ListDataApply(Page.SelectAllBasedOnPageTemplate(groupElementId));
            }
        }

        async Task PageList_ListLoad(ComponentListEventArgs e)
        {
            if (e.SelectedKey == 0)
                return;

            var page = await Page.SelectOneAsync(e.SelectedKey, false).ConfigureAwait(false);
            if (page != null && page.ID > 0)
            {
                Response.Redirect(string.Concat(wim.Console.WimPagePath, "?page=", page.ID));
            }
        }

        [Sushi.Mediakiwi.Framework.ContentListSearchItem.TextField("Search for", 50, AutoPostBack = true, Expression = OutputExpression.FullWidth)]
        public string FilterText { get; set; }

        DataList m_List;
        [Sushi.Mediakiwi.Framework.ContentListItem.DataList("63c0c71c-e301-4a29-9a75-73874cb6622e")]
        public DataList List
        {
            get { return m_List; }
            set { m_List = value; }
        }
    }
}
