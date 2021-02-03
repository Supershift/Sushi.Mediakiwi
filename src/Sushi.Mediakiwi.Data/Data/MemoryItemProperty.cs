namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    ///
    /// </summary>
    internal class MemoryItemProperty
    {
        public MemoryItemProperty()
        {
        }

        public MemoryItemProperty(int listID, int? listTypeID, bool showEmptyTypes, bool onlyReturnFlexibleProperties, Property[] properties)
        {
            this.ListID = listID;
            this.ListTypeID = listTypeID;
            this.ShowEmptyTypes = showEmptyTypes;
            this.OnlyReturnFlexibleProperties = onlyReturnFlexibleProperties;
            this.Properties = properties;
        }

        internal int ListID;
        internal int? ListTypeID;
        internal bool ShowEmptyTypes;
        internal bool OnlyReturnFlexibleProperties;
        internal Property[] Properties;
    }
}