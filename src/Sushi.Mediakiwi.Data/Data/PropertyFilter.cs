namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    ///
    /// </summary>
    public class PropertyFilter
    {
        public bool IsSqlColumn { get; set; }
        public string PropertyName { get; set; }
        public string PropertyValue { get; set; }
        public System.Data.SqlDbType PropertyType { get; set; }
    }
}