namespace Sushi.Mediakiwi.Headless.SectionHelper.Elements
{
    /// <summary>
    /// Represents a JavaScript file Section Element
    /// </summary>
    public class JavaScriptFile : SectionElement
    {
        /// <summary>
        /// The Uri of this Javascript file
        /// </summary>
        public string Uri
        {
            get
            {
                return PrivateProperties["src"];
            }
        }

        /// <summary>
        /// Creates a new JavaScript file Section Element
        /// </summary>
        /// <param name="uri">The Uri for the JavaScript file</param>
        public JavaScriptFile(string uri) : base("script")
        {
            PrivateProperties.Add("type", "text/javascript");
            PrivateProperties.Add("src", uri);
        }
    }
}
