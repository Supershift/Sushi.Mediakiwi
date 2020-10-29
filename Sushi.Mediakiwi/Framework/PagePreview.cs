using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// The page preview handler
    /// </summary>
    public class PagePreview : IPagePreview
    {
        /// <summary>
        /// Gets the preview URL.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        public virtual Uri GetPreviewUrl(Data.Page page)
        {
            return new Uri(string.Concat(page.HRefFull, "?preview=1"));
        }

        /// <summary>
        /// Gets the online URL.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        public virtual Uri GetOnlineUrl(Data.Page page)
        {
            return new Uri(page.HRefFull);
        }
    }
}
