namespace Sushi.Mediakiwi
{
    /// <summary>
    /// Request type definition
    /// </summary>
    public enum RequestItemType
    {
        Undefined = 0,
        /// <summary>
        /// A list item request
        /// </summary>
        Item = 1,
        /// <summary>
        /// A page request
        /// </summary>
        Page = 2,
        /// <summary>
        /// An asset request
        /// </summary>
        Asset = 3,
        /// <summary>
        /// The dashboard
        /// </summary>
        Dashboard = 4
    }
}
