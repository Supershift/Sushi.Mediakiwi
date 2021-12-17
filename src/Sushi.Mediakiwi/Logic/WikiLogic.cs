using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Interfaces;
using System;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Logic
{
    internal class WikiLogic : ITrailExtension
    {
        public async Task<string> RenderWikiButtonsAsync(string propertyButton, Beta.GeneratedCms.Console console)
        {
            bool settingList = true; // TODO: get from config
            var settingPages = true; // TODO: get from config
            if (settingList == false && settingPages == false)
            {
                return propertyButton;
            }

            var existingArticle = await Article.CheckIfItemExistsAsync(console.CurrentList?.ID, console.CurrentPage?.ID);

            if (console.CurrentListInstance.wim.CurrentApplicationUserRole.CanSeeAdmin || existingArticle.ID > 0)
            {
                string mkWikiUrl = null;
                if (existingArticle.ID > 0)
                {
                    mkWikiUrl = string.Concat(console.WimPagePath, "?", "list=", WikiList.WIKI_LIST_GUID, "&openinframe=1&item=", existingArticle.ID);
                }
                else if (settingPages != null && settingPages.Value == "1" && console.CurrentPage != null)
                {
                    mkWikiUrl = string.Concat(console.WimPagePath, "?", "list=", WikiList.WIKI_LIST_GUID, "&openinframe=1&item=-1&forPageID=", console.CurrentPage.ID, "&listName=" + console.CurrentPage.Name);
                }
                else if (settingList != null && settingList.Value == "1" && console.CurrentList != null)
                {
                    mkWikiUrl = string.Concat(console.WimPagePath, "?", "list=", WikiList.WIKI_LIST_GUID, "&openinframe=1&item=-1&forListID=", console.CurrentList.ID, "&listName=" + console.CurrentList.Name);
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
            return propertyButton;
        }
    }
}
