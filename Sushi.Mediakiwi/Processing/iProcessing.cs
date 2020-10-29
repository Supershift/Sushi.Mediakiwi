using System;
using System.Collections.Generic;
using System.Text;

namespace Wim.Processing
{
    /// <summary>
    /// 
    /// </summary>
    public interface iProcessing
    {
        /// <summary>
        /// Inits the specified component.
        /// </summary>
        /// <param name="component">The component.</param>
        void Init(Sushi.Mediakiwi.Beta.GeneratedCms.Source.External.ComponentConfiguration component);
        /// <summary>
        /// Gets or sets the wim.
        /// </summary>
        /// <value>The wim.</value>
        Sushi.Mediakiwi.Framework.WimComponentListRoot wim { get; set; }
    }
}
