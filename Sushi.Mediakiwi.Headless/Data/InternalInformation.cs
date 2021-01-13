using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sushi.Mediakiwi.Headless.Data
{
    /// <summary>
    /// Strictly for communicating internal information across pages and components
    /// </summary>
    [DataContract]
    public class InternalInformation
    {
        /// <summary>
        /// Is this is a preview call ?
        /// </summary>
        [DataMember(Name = "isPreview")]
        public bool IsPreview { get; set; }

        /// <summary>
        /// Is this a Cache clear call ?
        /// </summary>
        [DataMember(Name = "clearCache")]
        public bool ClearCache { get; set; }

        /// <summary>
        /// Add anything you want to communicate 
        /// </summary>
        [DataMember(Name = "customData")]
        public Dictionary<string, object> CustomData { get; set; } = new Dictionary<string, object>();

    }
}
