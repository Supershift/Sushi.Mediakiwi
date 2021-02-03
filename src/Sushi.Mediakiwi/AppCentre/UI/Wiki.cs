using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Data.Parsers;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.AppCentre.UI
{
    public class WikiList : ComponentListTemplate
    {
        public const string REGISTRY_SETTING_HELP_ON_PAGES = "SHOW_WIKI_HELP_ON_PAGES";
        public const string REGISTRY_SETTING_HELP_ON_LISTS = "SHOW_WIKI_HELP_ON_LISTS";
        public const string WIKI_LIST_GUID = "D03439F0-73D7-4C78-B51E-50310A00F6DA";

        [Framework.ContentSettingItem.Choice_Checkbox("Show 'Back to overview' button")]
        public bool ShowBackButton { get; set; }

        [Framework.ContentSettingItem.TextField("Label for 'Back to overview' button", 50, false)]
        public string LabelBackButton { get; set; }

        public WikiList()
        {
            ListSearch += WikiList_ListSearch;
            ListLoad += WikiList_ListLoad;
            ListSave += WikiList_ListSave;
            ListDelete += WikiList_ListDelete;
            ListDataItemCreated += List_ListDataItemCreated;

            wim.Page.Body.Grid.HidePager = true;
            wim.CurrentList.Option_CanCreate = true;
            wim.CurrentList.Option_CanSaveAndAddNew = false;
            wim.CurrentList.Option_HasExportXLS = false;
            wim.CurrentList.Label_NewRecord = "Add new article";
            wim.CurrentList.Label_Save = "Save article";
            wim.CurrentList.Label_Saved = "The article has been saved";
        }


        private void List_ListDataItemCreated(object sender, ListDataItemCreatedEventArgs e)
        {
            var article = (ArticleList)e.Item;
          
           e.InnerHTML = string.Format(@"
                <div {0}>
                    <h3>{1}</h3>
                    <p>{2}</p>
                </div>
                ", e.GetClickableNodeAttribute("item")
                   , article.Title
                   , Wim.Utility.CleanLineFeed(article.Summary, true, true, true)
                   , article.Updated
                   , article.Author
                   );
            //e.InnerHTML = string.Format(@"
            //    <div {0}>
            //        <h3>{1}</h3>
            //        <span>{3} - {4:dd-MMM-yy HH:mm}</span>
            //        <p>{2}</p>
            //    </div>
            //    ", e.GetClickableNodeAttribute("item")
            //, article.Title
            //, Wim.Utility.CleanLineFeed(article.Summary, true, true, true)
            //, article.Updated
            //, article.Author
            //);

        }

        private void WikiList_ListDelete(object sender, ComponentListEventArgs e)
        {
            ArticleParser.Delete(Implement);
        }

        private void WikiList_ListSave(object sender, ComponentListEventArgs e)
        {
            Implement.Title = Title;
            Implement.Data = Data;
            Implement.Summary = Summary;

            if (Implement.ID > 0)
                Implement.Updated = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;

            Implement.ComponentListID = wim.CurrentList.ID;
            Implement.AuthorID = wim.CurrentApplicationUser.ID;
            ArticleParser.Save(Implement);

            wim.Notification.AddNotificationAlert("Data was saved", true);
            Response.Redirect(wim.GetCurrentQueryUrl(true));
        }

        public Article Implement { get; set; }
        public string ListOverviewURL
        {
            get
            {
                return wim.GetCurrentQueryUrl(true, new KeyValue[] { new KeyValue("item", true) });
            }
        }

        private void WikiList_ListLoad(object sender, ComponentListEventArgs e)
        {


            if (!wim.CurrentApplicationUserRole.CanSeeAdmin)
            {
                wim.HideTitle = true;
                wim.HideEditOption = true;
                wim.HideCreateNew = true;
            }
            int forListID = Wim.Utility.ConvertToInt(Request["forListID"], -1);
            int forPageID = Wim.Utility.ConvertToInt(Request["forPageID"], -1);
            string defaultTitle = Request["listName"];
            if (forListID > 0)
            {
                Implement = ArticleParser.SelectOneForListOrNew(forListID, defaultTitle);
            }
            else if (forPageID > 0)
            {
                Implement = ArticleParser.SelectOneForPageOrNew(forPageID, defaultTitle);
            }
            else
            {
                Implement = ArticleParser.SelectOne(e.SelectedKey);
            }
            if (Implement == null)
                Implement = new Article();

            Title = Implement.Title;
            Data = Implement.Data;
            Summary = Implement.Summary;

            if (!wim.IsEditMode)
            {
                RenderContent();
                wim.Form.AddElement(this, nameof(ButtonBack), ShowBackButton, true, new Framework.ContentListItem.ButtonAttribute(LabelBackButton, false)
                {
                    CustomUrlProperty = nameof(ListOverviewURL)
                });
            }
        }

        void RenderContent()
        {
            wim.SetPropertyVisibility(nameof(Title), false);
            wim.SetPropertyVisibility(nameof(Data), false);
            wim.SetPropertyVisibility(nameof(Summary), false);

            wim.Page.Head.EnableColorCodingLibrary = true;

            StringBuilder html = new StringBuilder();

            html.AppendFormat(@"<h1>{0}</h1><style>  div.item a  {{ margin: 0px !important;}} a.inlineLink {{ display: inline !important; padding: 0px!important; margin: 0px !important;}} </style>", Title);
            if (!String.IsNullOrEmpty(Summary))
                html.AppendFormat(@"<hr style=""background-color: black""/><i>{0}</i><hr style=""background-color: black""/>", Summary);

            if (!string.IsNullOrEmpty(Data))
            {
                var lines = MultiField.GetDeserialized(Data);
                foreach (var line in lines)
                {
                    if (string.IsNullOrEmpty(line.Value))
                        continue;

                    if (line.Type == (int)ContentType.RichText)
                    {
                        var c = line.Value.Replace("[code]", "[code]code_section:");
                        var split = c.Split(new string[] { "[code]", "[/code]" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var item in split)
                        {
                            string code = Wim.Utility.CleanConcurrentBreaks(Wim.Utility.CleanLeadingAndTrailingLineFeed(item.Replace("code_section:", string.Empty)), true);
                            if (item.StartsWith("code_section:"))
                                html.AppendFormat(@"<textarea cols=""32"" rows=""8"" class=""long SourceCode"" data-done=""1"">{0}</textarea>",
                                    code);
                            else
                                html.AppendFormat("<p>{0}</p>", code.Replace("<a ", @"<a class=""inlineLink"" "));
                        }

                    }
                    else if (line.Type == (int)ContentType.TextField)
                    {
                        html.AppendFormat(@"<h3>{0}</h3>", line.Value);
                    }
                    else if (line.Type == (int)ContentType.Hyperlink)
                    {
                        html.AppendFormat(@"<a href=""{0}""><font class=""icon-external-link""></font>{1}</a>", line.Link.Url, line.Link.Text);
                    }
                    else if (line.Type == (int)ContentType.Binary_Document)
                    {
                        //if (line.Asset.IsImage)
                        //    html.AppendFormat(@"<figure> <img src=""{0}"" alt=""{1}"" /></figure>", line.Asset.Url, line.Document.Title);
                        //else
                        html.AppendFormat(@"<a href=""{0}""><font class=""icon-download""></font>{1}</a>", line.Asset.Url, line.Document.Title);
                    }
                    else if (line.Type == (int)ContentType.Binary_Image)
                    {
                        html.AppendFormat(@"<figure> <img src=""{0}"" alt=""{1}"" /></figure>", line.Image.Url, line.Image.Title);
                    }
                    else if (line.Type == (int)ContentType.Sourcecode)
                    {
                        html.AppendFormat(@"<textarea cols=""32"" rows=""8"" class=""long SourceCode"" data-done=""1"">{0}</textarea>", line.Value);
                    }
                }
            }
            string output = Wim.Utility.ApplyRichtextLinks(this.wim.CurrentSite, Wim.Utility.CleanConcurrentBreaks(html.ToString(), true));
          
            wim.Page.Body.Add(string.Format(@"
<section class=""component style"" id=""extern"">
    <article>
        <div class=""item"">{0}
        </div>
    </article>
</section>
"
                , output), false, Body.BodyTarget.Nested);
        }

        private void WikiList_ListSearch(object sender, ComponentListSearchEventArgs e)
        {

            wim.ListDataColumns.Add(new ListDataColumn("", nameof(ArticleList.ID), ListDataColumnType.UniqueIdentifier));
            wim.ListDataColumns.Add(new ListDataColumn("Title", nameof(ArticleList.Title)));
            wim.ListDataColumns.Add(new ListDataColumn("Summary", nameof(ArticleList.Summary)));
            wim.ListDataColumns.Add(new ListDataColumn("Author", nameof(ArticleList.Author)));
            wim.ListDataColumns.Add(new ListDataColumn("Updated", nameof(ArticleList.Updated)));
            wim.ListDataApply(ArticleParser.SelectList(wim.CurrentList.ID));
            wim.Page.Body.Grid.Table.IgnoreCreation = true;
            wim.Page.Body.Grid.ClassName = "style";

            if (!wim.CurrentApplicationUserRole.CanSeeAdmin)
            {
                wim.HideCreateNew = true;
            }
        }

        [Framework.ContentListItem.TextField("Title", 100, true)]
        public string Title { get; set; }


        [Framework.ContentListItem.TextArea("Summary", 0, false)]
        public string Summary { get; set; }

        [Framework.ContentListItem.MultiField("Article input")]
        public string Data { get; set; }

        public DataTemplate Component { get; set; }

        public bool ButtonBack { get; set; }
    }
}
