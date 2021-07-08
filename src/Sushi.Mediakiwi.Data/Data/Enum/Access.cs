namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Role Access enum
    /// </summary>
    public enum Access
    {
        /// <summary>
        /// Access granted
        /// </summary>
        Granted = 1,

        /// <summary>
        /// Access Denied
        /// </summary>
        Denied = 2,

        /// <summary>
        /// Access inherited from parent
        /// </summary>
        Inherit = 3
    }
}