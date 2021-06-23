namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// 
    /// </summary>
    public enum ListDataContentType
    {
        /// <summary>
        /// This is a default content
        /// </summary>
        Default,
        /// <summary>
        /// This is a internal page Key; when applied a deeplink is created to this page
        /// </summary>
        InternalPageLinkKey,
        /// <summary>
        /// This is a internal page Key; when applied the linktext is applied
        /// </summary>
        InternalPageKey,
        /// <summary>
        /// This is a link to the edit mode of the current listitem
        /// </summary>
        ItemSelect,
        /// <summary>
        /// This is a link to the edit mode of the current listitem, when applied "edit" is used as textprimer
        /// </summary>
        ItemEditSelect,
        /// <summary>
        /// This is a internal listitem Key; when applied a deeplink is created to the listitem with "Edit" as primer
        /// </summary>
        InternalListItemEditSelect,
        /// <summary>
        /// 
        /// </summary>
        HtmlEnabled
    }
}
