using Sushi.Mediakiwi.AppCentre.UI;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Interfaces;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Logic
{
    public class WikiTrailExtension : ITrailExtension
    {
        public async Task<string> RenderExtrasAsync(string propertyButton, Beta.GeneratedCms.Console console)
        {
            // When both Wiki options are disabled, return initial value
            if (CommonConfiguration.Wiki_HelpOnPages == false && CommonConfiguration.Wiki_HelpOnLists == false)
            {
                return propertyButton;
            }

            var wikiList = await ComponentList.SelectOneAsync(typeof(WikiList));

            if (wikiList?.ID > 0)
            {
                var existingArticle = await Article.CheckIfItemExistsAsync(console.CurrentList?.ID, console.CurrentPage?.ID);
                if (console.CurrentListInstance.wim.CurrentApplicationUserRole.CanSeeAdmin || existingArticle.ID > 0)
                {
                    string mkWikiUrl = null;
                    if (existingArticle.ID > 0)
                    {
                        mkWikiUrl = console.UrlBuild.GetUrl(new[]
                        {
                            new KeyValue("list", wikiList.ID),
                            new KeyValue("openinframe", "1"),
                            new KeyValue("item", existingArticle.ID),
                        });
                    }
                    else if (CommonConfiguration.Wiki_HelpOnPages && console.CurrentPage != null)
                    {
                        mkWikiUrl = console.UrlBuild.GetUrl(new[]
                        {
                            new KeyValue("list", wikiList.ID),
                            new KeyValue("openinframe", "1"),
                            new KeyValue("item", -1),
                            new KeyValue("forPageID", console.CurrentPage.ID),
                            new KeyValue("listName", console.CurrentPage.Name),
                        });
                    }
                    else if (CommonConfiguration.Wiki_HelpOnLists && console.CurrentList != null)
                    {
                        mkWikiUrl = console.UrlBuild.GetUrl(new[]
                        {
                            new KeyValue("list", wikiList.ID),
                            new KeyValue("openinframe", "1"),
                            new KeyValue("item", -1),
                            new KeyValue("forListID", console.CurrentList.ID),
                            new KeyValue("listName", console.CurrentList.Name),
                        });
                    }

                    if (mkWikiUrl != null)
                    {
                        propertyButton += $@"
                <script>
                function openHelpWindow() {{
                    window.open(""{mkWikiUrl}"", ""_blank"", ""toolbar=yes,scrollbars=yes,resizable=yes,top=10,left=200,width=1006,height=600"");
                }}
                </script> ";

                        propertyButton += $@"<a href=""#help"" onclick=""openHelpWindow()"" class=""icon-question-circle""></a>";
                    }
                }
            }

            return propertyButton;
        }
    }
}
