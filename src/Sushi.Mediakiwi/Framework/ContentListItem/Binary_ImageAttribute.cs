namespace Sushi.Mediakiwi.Framework.ContentListItem
{
    /// <summary>
    /// Possible return types: System.Int32, System.Int32[nullable], System.String, Sushi.Mediakiwi.Data.Image
    /// </summary>
    public class Binary_ImageAttribute : ContentInfoItem.Binary_ImageAttribute, IContentInfo, IListContentInfo
    {
        /// <summary>
        /// Possible return types: System.Int32, System.Int32[nullable], System.String, Sushi.Mediakiwi.Data.Image
        /// </summary>
        /// <param name="title"></param>
        public Binary_ImageAttribute(string title)
            : base(title, false) { }

        /// <summary>
        /// Possible return types: System.Int32, System.Int32[nullable], System.String, Sushi.Mediakiwi.Data.Image
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        public Binary_ImageAttribute(string title, bool mandatory)
            : base(title, mandatory, null) { }

        /// <summary>
        /// Possible return types: System.Int32, System.Int32[nullable], System.String, Sushi.Mediakiwi.Data.Image
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        /// <param name="interactiveHelp"></param>
        public Binary_ImageAttribute(string title, bool mandatory, string interactiveHelp)
            : base(title, mandatory, interactiveHelp) { }

        /// <summary>
        /// Possible return types: System.Int32, System.Int32[nullable], System.String, Sushi.Mediakiwi.Data.Image
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        /// <param name="gallery">The gallery.</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        public Binary_ImageAttribute(string title, bool mandatory, string gallery, string interactiveHelp)
            : base(title, mandatory, gallery, interactiveHelp) { }

        /// <summary>
        /// Possible return types: System.Int32, System.Int32[nullable], System.String, Sushi.Mediakiwi.Data.Image
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        /// <param name="gallery">The gallery.</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        /// <param name="canOnlyCreate">Indicates the user should only have the possibility to upload a new image, not select an existing one.</param>
        public Binary_ImageAttribute(string title, bool mandatory, string gallery, string interactiveHelp, bool canOnlyCreate)
            : base(title, mandatory, gallery, interactiveHelp, canOnlyCreate) { }
    }
}
