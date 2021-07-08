namespace Sushi.Mediakiwi.Framework.ContentSettingItem
{
    /// <summary>
    /// Possible return types: System.Int32
    /// </summary>
    public class Binary_DocumentAttribute : ContentInfoItem.Binary_DocumentAttribute, IContentInfo, IContentSettingInfo
    {
        /// <summary>
        /// Possible return types: System.Int32, Sushi.Mediakiwi.Data.AssetInfo
        /// </summary>
        /// <param name="title"></param>
        public Binary_DocumentAttribute(string title)
            : base(title, false) { }

        /// <summary>
        /// Possible return types: System.Int32, Sushi.Mediakiwi.Data.AssetInfo
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        public Binary_DocumentAttribute(string title, bool mandatory)
            : base(title, mandatory, null) { }

        /// <summary>
        /// Possible return types: System.Int32, Sushi.Mediakiwi.Data.AssetInfo
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        /// <param name="interactiveHelp"></param>
        public Binary_DocumentAttribute(string title, bool mandatory, string interactiveHelp)
            : base(title, mandatory, interactiveHelp) { }

        /// <summary>
        /// Possible return types: System.Int32, Sushi.Mediakiwi.Data.AssetInfo
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        /// <param name="gallery">The gallery.</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        public Binary_DocumentAttribute(string title, bool mandatory, string gallery, string interactiveHelp)
            : base(title, mandatory, gallery, interactiveHelp) { }
    }
}
