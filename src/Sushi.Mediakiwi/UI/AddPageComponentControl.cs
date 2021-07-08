using Sushi.Mediakiwi.Data;
using System.Collections.Generic;

namespace Sushi.Mediakiwi.UI
{
    public struct PageComponentOption
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public bool OnlyOneUsage { get; set; }
        public bool HideOnInit { get; set; }
    }
    /// <summary>
    /// This class holds all functionality regarding the add component on page functions
    /// </summary>
    public class AddPageComponentControl
    {
        public bool StartOpen { get; set; }
        public List<PageComponentOption> Options { get; set; }
        private Page page;
        private Beta.GeneratedCms.Console container;

        public AddPageComponentControl(Page page, Beta.GeneratedCms.Console container)
        {
            Options = new List<PageComponentOption>();
            this.page = page;
            this.container = container;
        }
        private const string HTML_CONTAINER = @"<section id=""cms"" ><div class=""controls""   id=""addPageComponentControlItems"" style=""margin:0px auto;{1}"">
                                <div class=""cmsBlocks"" >
                                    <a href=""#1"" class=""closer icon-x"" onclick=""mediakiwi.pageComponentControl.closeOptions()""></a>  
                                    {0}
                                </div> 
                            </div><br class=""clear""/><footer id=""addPageComponentControlAdder"" style=""{2}"">
                                <a href=""#1"" class=""plusBtn icon-plus"" onclick=""mediakiwi.pageComponentControl.showOptions()""></a>
                            </footer></section><script type=""text/javascript"">
                                {3}
                            </script>";
        private const string HTML_CONTAINER_BLOCK = @"<a href="""" {5}  onclick=""return mediakiwi.pageComponentControl.addItemToPage(this, {0}, {3}, {4})"" class=""block"">
                                        <span class=""icon-{2}""></span>
                                        {1}
                                    </a>";


        public string RenderControl()
        {
            if (Options.Count == 0) return string.Empty;
            string stateAdder = "";
            string stateItems = "display: none";
    
            if (StartOpen)
            {
                  stateAdder = "display: none";
                  stateItems = "";
            }
            if (Options!= null && Options.Count >0) 
            {
             string result = string.Empty;
             string optionCache = "mediakiwi.pageComponentControl.options = []; " + System.Environment.NewLine;
                foreach(var item in Options)
                {
                    string oneUsageAttr = "";
                    if (item.OnlyOneUsage)
                        oneUsageAttr = @" removeAfterUse=""1"" ";
                    //                                                0         1       2           3       4
                    string itemHTML = string.Format(HTML_CONTAINER_BLOCK, item.ID, item.Name, item.Icon, page.ID, container.Group.GetValueOrDefault(),
                                                // 5
                                                oneUsageAttr);
                    optionCache = string.Concat(optionCache, "mediakiwi.pageComponentControl.options['", item.ID, "'] =  '", itemHTML.Replace("'", "\\'").Replace(System.Environment.NewLine, " "), "'; ", System.Environment.NewLine);
                    if (!item.HideOnInit) 
                        result += itemHTML;
                }

                return string.Format(HTML_CONTAINER, result, stateItems, stateAdder, optionCache);
            }
            return string.Empty;
           
        }
    }
}
