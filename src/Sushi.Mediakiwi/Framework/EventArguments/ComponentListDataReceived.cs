using System;
using System.Collections.Generic;

namespace Sushi.Mediakiwi.Framework.EventArguments
{
    public enum ReceivedItemTypeEnum
    { 
        NEW = 0,
        CHANGED = 1,
        UNCHANGED = 2
    }

    public class ComponentListDataReceivedItem
    {
        /// <summary>
        /// Was this received item New, Changed or Unchanged
        /// </summary>
        public ReceivedItemTypeEnum ItemType { get; set; } = ReceivedItemTypeEnum.NEW;

        /// <summary>
        /// Each dictionary entry represents a PropertyName + Value
        /// </summary>
        public Dictionary<string, object> PropertyValues { get; set; } = new Dictionary<string, object>();
    }

    public class ComponentListDataReceived : EventArgs
    {
        /// <summary>
        /// Each dictionary in this collection represents a row (object)
        /// </summary>
        public List<ComponentListDataReceivedItem> ReceivedProperties { get; set; } = new List<ComponentListDataReceivedItem>();
        
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
