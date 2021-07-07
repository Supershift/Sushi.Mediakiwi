using System.Collections.Generic;

namespace Sushi.Mediakiwi.Controllers.Data
{
    public class SharedFieldUsagePage
    {
        /// <summary>
        /// The title of the page that uses the same field
        /// </summary>
        public string PageTitle { get; set; }

        /// <summary>
        /// The path of the page that uses the same field
        /// </summary>
        public string PagePath { get; set; }

        /// <summary>
        /// Is this page published ?
        /// </summary>
        public bool PagePublished { get; set; }

        /// <summary>
        /// List of components using this field
        /// </summary>
        public ICollection<string> Components { get; set; } = new List<string>();
    }
}
