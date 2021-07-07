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

        /// <summary>
        /// Adds an item to the CustomData dictionary
        /// </summary>
        /// <param name="name">The name for the dictionary item</param>
        /// <param name="value">The value for the dictionary item</param>
        /// <param name="overWriteIsExists">When TRUE, overwrite an existing item, else do NOT overwrite an existing item</param>
        public void Add(string name, object value, bool overWriteIsExists = false)
        {
            if (CustomData.ContainsKey(name) == false)
            {
                CustomData.Add(name, value);
            }
            else if (overWriteIsExists)
            {
                CustomData[name] = value;
            }
        }

    }
}
