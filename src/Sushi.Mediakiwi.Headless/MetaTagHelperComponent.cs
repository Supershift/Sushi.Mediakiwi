using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Sushi.Mediakiwi.Headless.Data;
using System;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Headless
{
    public class MetaTagHelperComponent : TagHelperComponent
    {     
        /// <summary>
        /// Gets or sets the <see cref="Rendering.ViewContext"/> for the current request.
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }


        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (string.Equals(context.TagName, "head", StringComparison.OrdinalIgnoreCase))
            {
                if (ViewContext?.HttpContext?.Items?.ContainsKey(ContextItemNames.PageContent) == true)
                {
                    PageContentResponse pageContent = ViewContext.HttpContext.Items[ContextItemNames.PageContent] as PageContentResponse;
                    if (pageContent != null)
                    {
                        output.PreContent.AppendLine();
                        output.PreContent.AppendHtmlLine("<!-- Processed by the MetaTagHelperComponent -->");
                        foreach (var metaTag in pageContent.MetaData.MetaTags)
                        {
                            switch (metaTag.RenderKey)
                            {
                                default:
                                case MetaTagRenderKey.NAME:
                                    output.PreContent.AppendHtmlLine($"<meta name=\"{metaTag.Name}\" content=\"{metaTag.Value}\" />");
                                    break;
                                case MetaTagRenderKey.PROPERTY:
                                    output.PreContent.AppendHtmlLine($"<meta property=\"{metaTag.Name}\" content=\"{metaTag.Value}\" />");
                                    break;
                            }
                        }
                    }
                }

            }
            return Task.CompletedTask;
        }
    }
}
