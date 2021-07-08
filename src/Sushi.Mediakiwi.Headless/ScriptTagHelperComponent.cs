using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Sushi.Mediakiwi.Headless.Data.TrackingScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Headless
{
    public class ScriptTagHelperComponent : TagHelperComponent
    {     
        /// <summary>
        /// Gets or sets the <see cref="Rendering.ViewContext"/> for the current request.
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }


        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            // Only react on <HEAD> Or <BODY> tags
            if (
                   string.Equals(context.TagName, "head", StringComparison.OrdinalIgnoreCase) == false
                && string.Equals(context.TagName, "body", StringComparison.OrdinalIgnoreCase) == false
                && string.Equals(context.TagName, "html", StringComparison.OrdinalIgnoreCase) == false)
            {
                return Task.CompletedTask;
            }

            List<TrackingScriptSimple> availableScriptTags = new List<TrackingScriptSimple>();

            if (ViewContext.HttpContext.Items.ContainsKey(ContextItemNames.ScriptTags))
            {
                if (ViewContext.HttpContext.Items[ContextItemNames.ScriptTags] is List<TrackingScriptSimple>)
                    availableScriptTags = (List<TrackingScriptSimple>)ViewContext.HttpContext.Items[ContextItemNames.ScriptTags];
                else if (ViewContext.HttpContext.Items[ContextItemNames.ScriptTags] is TrackingScriptSimple)
                    availableScriptTags.Add((TrackingScriptSimple)ViewContext.HttpContext.Items[ContextItemNames.ScriptTags]);
            }

            if (availableScriptTags?.Count > 0)
            {
                var bodyAfterCloseTags = availableScriptTags.Where(x => x.Position == TagPosition.BodyAfterCloseTag).Where(x => x.IsActive == true).ToList();
                var bodyAfterOpenTags = availableScriptTags.Where(x => x.Position == TagPosition.BodyAfterOpenTag).Where(x => x.IsActive == true).ToList();
                var bodyBeforeCloseTags = availableScriptTags.Where(x => x.Position == TagPosition.BodyBeforeCloseTag).Where(x => x.IsActive == true).ToList();
                var headAfterCloseTags = availableScriptTags.Where(x => x.Position == TagPosition.HeadAfterCloseTag).Where(x => x.IsActive == true).ToList();
                var headAfterOpenTags = availableScriptTags.Where(x => x.Position == TagPosition.HeadAfterOpenTag).Where(x => x.IsActive == true).ToList();
                var headBeforeCloseTags = availableScriptTags.Where(x => x.Position == TagPosition.HeadBeforeCloseTag).Where(x => x.IsActive == true).ToList();

                if (string.Equals(context.TagName, "head", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var script in headAfterCloseTags)
                    {
                        output.PostElement.AppendHtmlLine(script.Code);
                    }

                    foreach (var script in headAfterOpenTags)
                    {
                        output.PreContent.AppendHtmlLine(script.Code);
                    }

                    foreach (var script in headBeforeCloseTags)
                    {
                        output.PostContent.AppendHtmlLine(script.Code);
                    }
                }

                if (string.Equals(context.TagName, "body", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var script in bodyAfterCloseTags)
                    {
                        output.PostElement.AppendHtmlLine(script.Code);
                    }

                    foreach (var script in bodyAfterOpenTags)
                    {
                        output.PreContent.AppendHtmlLine(script.Code);
                    }

                    foreach (var script in bodyBeforeCloseTags)
                    {
                        output.PostContent.AppendHtmlLine(script.Code);
                    }
                }

                if (string.Equals(context.TagName, "html", StringComparison.OrdinalIgnoreCase))
                {
                    output.PostContent.AppendHtmlLine($"<!-- {availableScriptTags.Count} scripts injected by ScriptTagHelper. -->");
                }
            }
            else 
            {
                if (string.Equals(context.TagName, "html", StringComparison.OrdinalIgnoreCase))
                {
                    output.PostContent.AppendHtmlLine($"<!-- No scripts injected by ScriptTagHelper. -->");
                }
            }
            return Task.CompletedTask;
        }
    }
}
