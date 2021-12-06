namespace Sushi.Mediakiwi.API.Transport
{
    public enum ContentResponseTypeEnum
    {
        /// <summary>
        /// The response type is a Page
        /// </summary>
        Page = 0,

        /// <summary>
        /// The response type is a List
        /// </summary>
        List = 1,

        /// <summary>
        /// The response type is a Folder (explorer)
        /// </summary>
        Folder = 2,

        /// <summary>
        /// The response type is a Gallery (explorer)
        /// </summary>
        Gallery = 3
    }
}
