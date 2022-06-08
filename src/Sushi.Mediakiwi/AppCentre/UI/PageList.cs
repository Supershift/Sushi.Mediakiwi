using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.Collections;
using Sushi.Mediakiwi.AppCentre.Data.Implementation;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.UI;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
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

            ListLoad += PageList_ListLoad;
            ListSearch += PageList_ListSearch;
            ListHeadless += PageList_ListHeadless;
        }

        private async Task PageList_ListHeadless(HeadlessRequest e)
        {
            var page = await Page.SelectOneAsync(e.Listitem.ID).ConfigureAwait(false);
            if (page == null || page.ID == 0)
            {
                return;
            }

            e.Result = new PageHref
            {
                Href = Utility.ConvertUrl(page.InternalPath),
                Text = string.IsNullOrWhiteSpace(page.LinkText) ? page.Name : page.LinkText
            };
        }

        async Task PageList_ListSearch(ComponentListSearchEventArgs e)
        {
            wim.ListDataColumns.Add(new ListDataColumn("", nameof(Page.ID), ListDataColumnType.UniqueIdentifier));
            wim.ListDataColumns.Add(new ListDataColumn("Path", nameof(Page.InternalPath), ListDataColumnType.HighlightPresent));
            wim.ListDataColumns.Add(new ListDataColumn("Published", nameof(Page.IsPublished)) { ColumnWidth = 60 });
            wim.ListDataColumns.Add(new ListDataColumn("Modified", nameof(Page.Updated)) { ColumnWidth = 90 });

            if (e.SelectedGroupItemKey == 0)
            {
                if (string.IsNullOrWhiteSpace(FilterText))
                {
                    wim.ListDataAdd(await Page.SelectAllAsync().ConfigureAwait(false));
                }
                else
                {
                    wim.ListDataAdd(await Page.SelectAllAsync(FilterText, true).ConfigureAwait(false));
                }
            }
            else
            {
                int groupElementId = Utility.ConvertToInt(e.SelectedGroupItemKey);
                var results = await Page.SelectAllBasedOnPageTemplateAsync(groupElementId).ConfigureAwait(false);
                wim.ListDataAdd(results);
            }
        }

        async Task PageList_ListLoad(ComponentListEventArgs e)
        {
            if (e.SelectedKey == 0)
            {
                return;
            }

            var page = await Page.SelectOneAsync(e.SelectedKey, false).ConfigureAwait(false);
            if (page?.ID > 0)
            {
                Response.Redirect(string.Concat(wim.Console.WimPagePath, "?page=", page.ID));
            }
        }

        [Sushi.Mediakiwi.Framework.ContentListSearchItem.TextField("Search for", 50, AutoPostBack = true, Expression = OutputExpression.FullWidth)]
        public string FilterText { get; set; }

        [Sushi.Mediakiwi.Framework.ContentListItem.DataList("63c0c71c-e301-4a29-9a75-73874cb6622e")]
        public DataList List { get; set; }
    }
}
