using System;
using System.Collections.Generic;

namespace Sushi.Mediakiwi.Framework.EventArguments
{

    public class ComponentListDataReceived : EventArgs
    {
        /// <summary>
        /// Each dictionary in this collection represents a row (object)
        /// </summary>
        public List<Dictionary<string, object>> ReceivedProperties { get; set; } = new List<Dictionary<string, object>>();
        
        /// <summary>
        /// From where was this data sent to the list ?
        /// </summary>
        public string DataSource { get; set; }

        /// <summary>
        /// The target type for the received properties
        /// </summary>
        public string FullTypeName { get; set; }

    }
}
