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

        private void PerformReplacements(TrackingScriptSimple script)
        {
            if (script?.ReplacementItems?.Count > 0)
            {
                // Are all marked as mandatory ?
                var allMandatory = script.ReplacementItems.Count == script.ReplacementItems.Count(x => x.MustBeFilled);

                // are all values filled ?
                var allFilled = script.ReplacementItems.Count == script.ReplacementItems.Count(x => string.IsNullOrWhiteSpace(x.Value) == false);

                // if all replacement values are marked as mandatory, but not all are filled, empty the code.
                if (allMandatory && !allFilled) 
                {
                    script.Code = string.Empty;
                    return;
                }

                // Only continue when we have a non-empty code
                if (string.IsNullOrWhiteSpace(script.Code) == false)
                {
                    foreach (var repitem in script.ReplacementItems)
                    {
                        if (string.IsNullOrWhiteSpace(repitem.Value) == false)
                        {
                            script.Code = script.Code.Replace($"[{repitem.Code}]", repitem.Value, StringComparison.InvariantCultureIgnoreCase);
                        }
                        else if (string.IsNullOrWhiteSpace(repitem.Fallback) == false)
                        {
                            script.Code = script.Code.Replace($"[{repitem.Code}]", repitem.Fallback, StringComparison.InvariantCultureIgnoreCase);
                        }
                    }
                }
            }
        }

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
                if (ViewContext.HttpContext.Items[ContextItemNames.ScriptTags] is List<TrackingScriptSimple> scriptList)
                {
                    availableScriptTags = scriptList;
                }
                else if (ViewContext.HttpContext.Items[ContextItemNames.ScriptTags] is TrackingScriptSimple scriptItem)
                {
                    availableScriptTags.Add(scriptItem);
                }
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
                        PerformReplacements(script);
                        if (string.IsNullOrWhiteSpace(script.Code) == false)
                        {
                            output.PostElement.AppendHtmlLine(script.Code);
                        }
                    }

                    foreach (var script in headAfterOpenTags)
                    {
                        PerformReplacements(script);
                        if (string.IsNullOrWhiteSpace(script.Code) == false)
                        {
                            output.PreContent.AppendHtmlLine(script.Code);
                        }
                    }

                    foreach (var script in headBeforeCloseTags)
                    {
                        PerformReplacements(script);
                        if (string.IsNullOrWhiteSpace(script.Code) == false)
                        {
                            output.PostContent.AppendHtmlLine(script.Code);
                        }
                    }
                }

                if (string.Equals(context.TagName, "body", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var script in bodyAfterCloseTags)
                    {
                        PerformReplacements(script);
                        if (string.IsNullOrWhiteSpace(script.Code) == false)
                        {
                            output.PostElement.AppendHtmlLine(script.Code);
                        }
                    }

                    foreach (var script in bodyAfterOpenTags)
                    {
                        PerformReplacements(script);
                        if (string.IsNullOrWhiteSpace(script.Code) == false)
                        {
                            output.PreContent.AppendHtmlLine(script.Code);
                        }
                    }

                    foreach (var script in bodyBeforeCloseTags)
                    {
                        PerformReplacements(script);
                        if (string.IsNullOrWhiteSpace(script.Code) == false)
                        {
                            output.PostContent.AppendHtmlLine(script.Code);
                        }
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
