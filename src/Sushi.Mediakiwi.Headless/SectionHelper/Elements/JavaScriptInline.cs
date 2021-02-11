using Microsoft.AspNetCore.Components;

namespace Sushi.Mediakiwi.Headless.SectionHelper.Elements
{

    /// <summary>
    /// Represents an Inline JS Section Element
    /// </summary>
    public class JavaScriptInline : SectionElement
    {
        /// <summary>
        /// Creates a new Inline JS Section Element
        /// </summary>
        /// <param name="content">The (string) content for the Inline JS</param>
        public JavaScriptInline(string content) : base("script", content)
        {
            PrivateProperties.Add("type", "text/javascript");
        }

        /// <summary>
        /// Creates a new Inline JS Section Element
        /// </summary>
        /// <param name="content">The (HTML) content for the Inline JS</param>
        public JavaScriptInline(MarkupString content) : base("script", content)
        {
            PrivateProperties.Add("type", "text/javascript");
        }
    }
}
