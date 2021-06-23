using Sushi.Mediakiwi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Logic
{
    public class SearchLogic
    {
        public static async Task<List<SearchResult>> ExtractContentAsync(DateTime? sinceLastModified = null, bool isPreviewContent = false)
        {
            var result = new List<SearchResult>();

            PageLogic logic = new PageLogic(null);

            Page[] pages;
            if (sinceLastModified.HasValue)
            {
                pages = await Page.SelectAllAsync(sinceLastModified.Value).ConfigureAwait(false);
            }
            else
            {
                pages = await Page.SelectAllAsync().ConfigureAwait(false);
            }

            foreach (var page in pages)
            {
                if (page.IsSearchable)
                {
                    var pageresult = new SearchResult
                    { 
                        Title = page.Title,
                        Description = page.Description,
                        Url = page.HRefFull,
                        Data = new List<string>(),
                        LastModified = page.Updated
                    };

                    if (!string.IsNullOrWhiteSpace(page.Title))
                    {
                        pageresult.Data.Add(page.Title);
                    }

                    if (!string.IsNullOrWhiteSpace(page.Description))
                    {
                        pageresult.Data.Add(page.Description);
                    }

                    var content = await logic.GetPageContentAsync(page, null, isPreviewContent).ConfigureAwait(false);

                    if (content.Components != null && content.Components.Any())
                    {
                        foreach (var component in content.Components)
                        {
                            foreach (var item in component.Content)
                            {
                                ExtractText(pageresult.Data, item.Value);
                            }
                        }
                    }
                    result.Add(pageresult);
                }
            }

            return result;
        }

        static void ExtractText(List<string> collection, ContentItem item)
        {
            if (item == null)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(item.Text))
            {
                collection.Add(item.Text);
            }

            if (item.MultiFieldContent != null && item.MultiFieldContent.Any())
            {
                foreach (var nested in item.MultiFieldContent)
                {
                    ExtractText(collection, nested.Value);
                }
            }
        }
    }
}
