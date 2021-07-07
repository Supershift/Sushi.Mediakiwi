namespace Sushi.Mediakiwi.Headless.SectionHelper.Elements
{
    /// <summary>
    /// Represents a CSS file Section Element
    /// </summary>
    public class StylesheetFile : SectionElement
    {
        /// <summary>
        /// The Uri of this CSS file
        /// </summary>
        public string Uri
        {
            get
            {
                return PrivateProperties.ContainsKey("href") ? PrivateProperties["href"] : null;
            }
        }

        /// <summary>
        /// Creates a new CSS file Section Element
        /// </summary>
        /// <param name="uri">The Uri for the CSS file</param>
        public StylesheetFile(string uri) : base("link")
        {
            PrivateProperties.Add("rel", "stylesheet");
            PrivateProperties.Add("type", "text/css");
            PrivateProperties.Add("href", uri);
        }
    }
}
