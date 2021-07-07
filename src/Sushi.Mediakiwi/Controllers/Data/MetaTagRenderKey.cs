using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Controllers.Data
{
    public enum MetaTagRenderKey
    {
        /// <summary>
        /// Renders as <meta name=\"key\" />
        /// </summary>
        NAME = 0,
        /// <summary>
        /// Renders as <meta property=\"key\" />
        /// </summary>
        PROPERTY = 10
    }
}
