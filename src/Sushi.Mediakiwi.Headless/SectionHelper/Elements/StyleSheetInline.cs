using Microsoft.AspNetCore.Components;

namespace Sushi.Mediakiwi.Headless.SectionHelper.Elements
{
    /// <summary>
    /// Represents an Inline CSS Section Element
    /// </summary>
    public class StyleSheetInline : SectionElement
    {
        /// <summary>
        /// Creates a new Inline JS Section Element
        /// </summary>
        /// <param name="content">The (string) content for the Inline JS</param>
        public StyleSheetInline(string content) : base("style", content)
        {
            PrivateProperties.Add("type", "text/css");
        }

        /// <summary>
        /// Creates a new Inline CSS Section Element
        /// </summary>
        /// <param name="content">The (HTML) content for the Inline JS</param>
        public StyleSheetInline(MarkupString content) : base("style", content)
        {
            PrivateProperties.Add("type", "text/css");
        }
    }
}
