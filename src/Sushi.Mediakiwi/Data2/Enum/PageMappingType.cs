using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Page mappings;
    /// 200 - OK
    /// 302 - Found
    /// 301 - Moved Permanent
    /// 404 - Not found
    /// </summary>
    public enum PageMappingType
    {
        /// <summary>
        /// HTTP OK; This is a rewrite
        /// </summary>
        Rewrite200 = 1,
        /// <summary>
        /// HTTP Found; This is a redirect
        /// </summary>
        Redirect302 = 2,
        /// <summary>
        /// HTTP Permanent moved; This is a redirect
        /// </summary>
        Redirect301 = 3,
        /// <summary>
        /// Http not found;
        /// </summary>
        NotFound404 = 4

    }
}
