using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Sushi.Mediakiwi.Controllers.Data
{
    public class PageContentResponse
    {
        public PageContentResponse()
        {
            Components = new List<ContentComponent>();
            MetaData = new ContentMetaData();
        }

        /// <summary>
        /// Is there an exception thrown ?
        /// </summary>
        public string Exception { get; set; }

        /// <summary>
        /// What is the page ID of the page
        /// </summary>
        public int PageID { get; set; }

        /// <summary>
        /// The Path of the page in the CMS
        /// </summary>
        public string PageInternalPath { get; set; }

        /// <summary>
        /// Sets the page location for StatusCode OK,
        /// or a redirect path for StatusCode NotFound
        /// </summary>
        public string PageLocation { get; set; }

        /// <summary>
        /// All components belonging to this page.
        /// </summary>
        public List<ContentComponent> Components { get; set; }

        /// <summary>
        /// All MetaData belonging to this page.
        /// </summary>
        public ContentMetaData MetaData { get; set; }

        /// <summary>
        /// The Http statuscode for the response
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
    }
}
