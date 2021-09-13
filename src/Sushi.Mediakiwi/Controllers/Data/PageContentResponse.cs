using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.Text;

namespace Sushi.Mediakiwi.Controllers.Data
{
    /// <summary>
    /// The response from the Content API
    /// </summary>
    [DataContract]
    public class PageContentResponse
    {
        public PageContentResponse()
        {
            Components = new List<ContentComponent>();
            MetaData = new ContentMetaData();
        }

        /// <summary>
        /// The PadeID from mediakiwi
        /// </summary>
        [DataMember(Name = "pageId")]
        public int PageID { get; set; }

        /// <summary>
        /// The Page Location (path) to the Page template
        /// </summary>
        [DataMember(Name = "pageLocation")]
        public string PageLocation { get; set; }

        /// <summary>
        /// The Internal path from MediaKiwi
        /// </summary>
        [DataMember(Name = "pageInternalPath")]
        public string PageInternalPath { get; set; }

        /// <summary>
        /// All page Components
        /// </summary>
        [DataMember(Name = "components")]
        public List<ContentComponent> Components { get; set; }

        /// <summary>
        /// The Http statuscode for the response
        /// </summary>
        [DataMember(Name = "statusCode")]
        public HttpStatusCode StatusCode { get; set; }


        /// <summary>
        /// The exception (if any)
        /// </summary>
        [DataMember(Name = "exception")]
        public string Exception { get; set; }

        /// <summary>
        /// The Metadata for this page
        /// </summary>
        [DataMember(Name = "metaData")]
        public ContentMetaData MetaData { get; set; }

        /// <summary>
        /// Internal information for this Page (if any)
        /// </summary>
        [DataMember(Name = "internalInfo")]
        public InternalInformation InternalInfo { get; set; } = new InternalInformation();
    }
}
