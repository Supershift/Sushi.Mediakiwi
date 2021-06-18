namespace Sushi.Mediakiwi.Framework.ContentListItem
{
    /// <summary>
    /// Possible return types: System.Int32
    /// </summary>
    public class FolderSelectAttribute : ContentInfoItem.FolderSelectAttribute, IContentInfo, IListContentInfo
    {
        /// <summary>
        /// Possible return types: System.Int32, System.Int32[nullable], Sushi.Mediakiwi.Data.Folder
        /// </summary>
        /// <param name="title"></param>
        public FolderSelectAttribute(string title)
            : base(title, false) { }

        /// <summary>
        /// Possible return types: System.Int32, System.Int32[nullable], Sushi.Mediakiwi.Data.Folder
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        public FolderSelectAttribute(string title, bool mandatory)
            : base(title, mandatory, null) { }

        /// <summary>
        /// Possible return types: System.Int32, System.Int32[nullable], Sushi.Mediakiwi.Data.Folder
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        /// <param name="interactiveHelp"></param>
        public FolderSelectAttribute(string title, bool mandatory, string interactiveHelp)
            : base(title, mandatory, interactiveHelp) { }

        /// <summary>
        /// Possible return types: System.Int32, System.Int32[nullable], Sushi.Mediakiwi.Data.Folder
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        /// <param name="type">The type.</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        public FolderSelectAttribute(string title, bool mandatory, Data.FolderType type, string interactiveHelp)
            : base(title, mandatory, type, interactiveHelp) { }
    }
}
