using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Framework.ContentSettingItem
{
    /// <summary>
    /// Possible return types: Sushi.Mediakiwi.Data.SubList
    /// </summary>
    public class SubListSelectAttribute : ContentInfoItem.SubListSelectAttribute, IContentInfo, IContentSettingInfo
    {
        /// <summary>
        /// Possible return types: Sushi.Mediakiwi.Data.SubList
        /// </summary>
        /// <param name="title"></param>
        /// <param name="componentlistGuid">The indentifier GUID of the component list (this can also be a ClassName).</param>
        public SubListSelectAttribute(string title, string componentlistGuid)
            : base(title, componentlistGuid, false) { }

        /// <summary>
        /// Possible return types: Sushi.Mediakiwi.Data.SubList
        /// </summary>
        /// <param name="title"></param>
        /// <param name="componentlistGuid">The indentifier GUID of the component list (this can also be a ClassName).</param>
        /// <param name="mandatory"></param>
        public SubListSelectAttribute(string title, string componentlistGuid, bool mandatory)
            : base(title, componentlistGuid, mandatory, null) { }

        /// <summary>
        /// Possible return types: Sushi.Mediakiwi.Data.SubList
        /// </summary>
        /// <param name="title"></param>
        /// <param name="componentlistGuid">The indentifier GUID of the component list (this can also be a ClassName).</param>
        /// <param name="mandatory"></param>
        /// <param name="interactiveHelp"></param>
        public SubListSelectAttribute(string title, string componentlistGuid, bool mandatory, string interactiveHelp)
            : base(title, componentlistGuid, mandatory, interactiveHelp) { }


        /// <summary>
        /// Possible return types: Sushi.Mediakiwi.Data.SubList
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="componentlistGuid">The indentifier GUID of the component list (this can also be a ClassName).</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        /// <param name="canOnlyOrderSort">if set to <c>true</c> [can only order sort].</param>
        public SubListSelectAttribute(string title, string componentlistGuid, bool mandatory, bool canOnlyOrderSort)
            : base(title, componentlistGuid, mandatory, canOnlyOrderSort) { }

        /// <summary>
        /// Possible return types: Sushi.Mediakiwi.Data.SubList
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="componentlistGuid">The indentifier GUID of the component list (this can also be a ClassName).</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        /// <param name="canOnlyOrderSort">if set to <c>true</c> [can only order sort].</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        public SubListSelectAttribute(string title, string componentlistGuid, bool mandatory, bool canOnlyOrderSort, string interactiveHelp)
            : base(title, componentlistGuid, mandatory, canOnlyOrderSort, interactiveHelp) { }

    }
}
