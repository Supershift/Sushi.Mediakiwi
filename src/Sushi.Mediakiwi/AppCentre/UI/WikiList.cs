using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.AppCentre.UI
{
    public class WikiList : ComponentListTemplate
    {
        public const string REGISTRY_SETTING_HELP_ON_PAGES = "SHOW_WIKI_HELP_ON_PAGES";
        public const string REGISTRY_SETTING_HELP_ON_LISTS = "SHOW_WIKI_HELP_ON_LISTS";
        public const string WIKI_LIST_GUID = "08B77637-2B0D-4E13-95F9-40213CB4FFAF";

        public WikiList()
        {
            ListInit += WikiList_ListInit;
            ListSearch += WikiList_ListSearch;
            ListLoad += WikiList_ListLoad;
            ListSave += WikiList_ListSave;
            ListDelete += WikiList_ListDelete;

            ListDataItemCreated += List_ListDataItemCreated;

        }

        private async Task WikiList_ListInit()
        {
            wim.CurrentList.Option_CanCreate = true;
            wim.CurrentList.Option_CanSaveAndAddNew = false;
            wim.CurrentList.Option_HasExportXLS = false;
            wim.CurrentList.Label_NewRecord = "Add new article";
            wim.CurrentList.Label_Save = "Save article";
            wim.CurrentList.Label_Saved = "The article has been saved";
        }

        private async Task WikiList_ListDelete(ComponentListEventArgs e)
        {
            await Article.DeleteAsync(Implement);
        }

        private async Task WikiList_ListSave(ComponentListEventArgs e)
        {
            Implement.Title = Title;
            Implement.Data = Data;
            Implement.Summary = Summary;

            if (Implement.ID > 0)
            {
                Implement.Updated = Common.DatabaseDateTime;
            }

            Implement.ComponentListID = wim.CurrentList.ID;
            Implement.AuthorID = wim.CurrentApplicationUser.ID;
            await Article.SaveAsync(Implement);
        }

        private async Task WikiList_ListLoad(ComponentListEventArgs e)
        {
            if (!wim.CurrentApplicationUserRole.CanSeeAdmin)
            {
                wim.HideEditOption = true;
                wim.HideCreateNew = true;
            }
            int forListID = Utils.ConvertToInt(Context.Request.Query["forListID"], -1);
            int forPageID = Utils.ConvertToInt(Context.Request.Query["forPageID"], -1);
            string defaultTitle = Context.Request.Query["listName"];
            if (forListID > 0)
            {
                Implement = await Article.SelectOneForListOrNewAsync(forListID, defaultTitle);
            }
            else if (forPageID > 0)
            {
                Implement = await Article.SelectOneForPageOrNewAsync(forPageID, defaultTitle);
            }
            else
            {
                Implement = await Article.SelectOneAsync(e.SelectedKey);
            }
            if (Implement == null)
            {
                Implement = new Article();
            }

            Title = Implement.Title;
            Data = Implement.Data;
            Summary = Implement.Summary;

            if (!wim.IsEditMode)
            {
                RenderContent();
            }
        }

        private async Task WikiList_ListSearch(ComponentListSearchEventArgs e)
        {
            wim.ListDataColumns.Add(new ListDataColumn("", nameof(ArticleList.ID), ListDataColumnType.UniqueIdentifier));
            wim.ListDataColumns.Add(new ListDataColumn("Title", nameof(ArticleList.Title)));
            wim.ListDataColumns.Add(new ListDataColumn("Summary", nameof(ArticleList.Summary)));
            wim.ListDataColumns.Add(new ListDataColumn("Author", nameof(ArticleList.Author)));
            wim.ListDataColumns.Add(new ListDataColumn("Updated", nameof(ArticleList.Updated)));

            var results = await Article.SelectListAsync(wim.CurrentList.ID);
            wim.ListDataAdd(results);

            wim.Page.Body.Grid.Table.IgnoreCreation = true;
            wim.Page.Body.Grid.ClassName = "style";
        }

        private void List_ListDataItemCreated(object sender, ListDataItemCreatedEventArgs e)
        {
            var article = (ArticleList)e.Item;

            e.InnerHTML = $@"
                <div {e.GetClickableNodeAttribute("item")}>
                    <h3>{article.Title}</h3>
                    <span>{article.Author} - {article.Updated:dd-MMM-yy HH:mm}</span>
                    <p>{Utils.CleanLineFeed(article.Summary, true, true, true)}</p>
                </div>";
        }

        public Article Implement { get; set; }

        async void RenderContent()
        {
            wim.SetPropertyVisibility(nameof(Title), false);
            wim.SetPropertyVisibility(nameof(Data), false);
            wim.SetPropertyVisibility(nameof(Summary), false);

            wim.Page.Head.EnableColorCodingLibrary = true;

            StringBuilder html = new StringBuilder();

            html.Append($@"<h2>{Title}</h2><style>  div.item a  {{ margin: 0px !important;}} a.inlineLink {{ display: inline !important; padding: 0px!important; margin: 0px !important;}} </style>");
            if (!String.IsNullOrEmpty(Summary))
            {
                html.Append($@"<hr style=""background-color: black""/><i>{Summary}</i><hr style=""background-color: black""/>");
            }

            if (!string.IsNullOrEmpty(Data))
            {
                var lines = Framework.MultiField.GetDeserialized(Data);
                foreach (var line in lines)
                {
                    if (string.IsNullOrEmpty(line.Value))
                    {
                        continue;
                    }

                    if (line.Type == (int)ContentType.RichText)
                    {
                        var c = line.Value.Replace("[code]", "[code]code_section:", StringComparison.InvariantCultureIgnoreCase);
                        var split = c.Split(new string[] { "[code]", "[/code]" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var item in split)
                        {
                            string code = Utils.CleanConcurrentBreaks(Utils.CleanLeadingAndTrailingLineFeed(item.Replace("code_section:", string.Empty, StringComparison.InvariantCultureIgnoreCase)), true);
                            if (item.StartsWith("code_section:"))
                            {
                                html.Append($@"<textarea cols=""32"" rows=""8"" class=""long SourceCode"" data-done=""1"">{code}</textarea>");
                            }
                            else
                            {
                                html.Append($"<p>{code.Replace("<a ", @"<a class=""inlineLink"" ", StringComparison.InvariantCultureIgnoreCase)}</p>");
                            }
                        }

                    }
                    else if (line.Type == (int)ContentType.TextField)
                    {
                        html.Append($@"<h3>{line.Value}</h3>");
                    }
                    else if (line.Type == (int)ContentType.Hyperlink)
                    {
                        html.Append($@"<a href=""{await line.Link.GetHrefAsync()}""><font class=""icon-external-link""></font>{line.Link.Text}</a>");
                    }
                    else if (line.Type == (int)ContentType.Binary_Document)
                    {
                        html.Append($@"<a href=""{line.Asset.RemoteLocation}""><font class=""icon-download""></font>{line.Document.Title}</a>");
                    }
                    else if (line.Type == (int)ContentType.Binary_Image)
                    {
                        html.Append($@"<figure> <img src=""{line.Image.RemoteLocation}"" alt=""{line.Image.Title}"" /></figure>");
                    }
                    else if (line.Type == (int)ContentType.Sourcecode)
                    {
                        html.Append($@"<textarea cols=""32"" rows=""8"" class=""long SourceCode"" data-done=""1"">{line.Value}</textarea>");
                    }
                }
            }

            string output = Utils.ApplyRichtextLinks(wim.CurrentSite, Utils.CleanConcurrentBreaks(html.ToString(), true));

            await wim.Page.Body.AddAsync($@"
                <section class=""component style"" id=""extern"">
                    <article>
                        <div class=""item"">{output}</div>
                    </article>
                </section>", false, Body.BodyTarget.Nested);
        }

        [Framework.ContentListItem.TextField("Title", 100, true)]
        public string Title { get; set; }


        [Framework.ContentListItem.TextArea("Summary", 0, false)]
        public string Summary { get; set; }

        [Framework.ContentListItem.MultiField("Article input")]
        public string Data { get; set; }
    }
}
