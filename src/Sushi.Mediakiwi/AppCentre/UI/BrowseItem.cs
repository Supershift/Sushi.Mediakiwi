using System;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    /// <summary>
    /// Represents a Browsing entity.
    /// </summary>
    public partial class Browsing
    {       /// <summary>
        /// Represents a BrowseItem entity.
        /// </summary>
        public class BrowseItem
        {
            /// <summary>
            /// Gets or sets the type URL.
            /// </summary>
            /// <value>The type URL.</value>
            public string TypeUrl { get; set; }
            
            /// <summary>
            /// Gets or sets the id.
            /// </summary>
            /// <value>The id.</value>
            public int ID { get; set; }

            /// <summary>
            /// Gets or sets the ID text.
            /// </summary>
            /// <value>The ID text.</value>
            public string IDText { get; set; }

            /// <summary>
            /// Gets or sets the icon.
            /// </summary>
            /// <value>The icon.</value>
            public string Icon { get; set; }

            /// <summary>
            /// Gets or sets the hidden field.
            /// </summary>
            /// <value>The hidden field.</value>
            public string HiddenField { get; set; } = "";

            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            /// <value>The title.</value>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the info1.
            /// </summary>
            /// <value>The info1.</value>
            public string Info1 { get; set; }

            /// <summary>
            /// Gets or sets the info2.
            /// </summary>
            /// <value>The info2.</value>
            public string Info2 { get; set; }

            /// <summary>
            /// Gets or sets the info3.
            /// </summary>
            /// <value>The info3.</value>
            public DateTime Info3 { get; set; }

            /// <summary>
            /// Gets or sets the info4.
            /// </summary>
            /// <value>The info4.</value>
            public string Info4 { get; set; }

            /// <summary>
            /// Gets or sets the pass through.
            /// </summary>
            /// <value>The pass through.</value>
            public string PassThrough { get; set; }

            public string EditLink { get; set; }
            public string DownloadLink { get; set; }
        }
    }
}
