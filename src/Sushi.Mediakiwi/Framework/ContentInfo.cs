using Sushi.Mediakiwi.Framework.ContentListItem;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Framework
{

    public class ContentInfo
    {
        protected bool? IsHidden { get; set; }
        protected bool? IsCloaked { get; set; }
        protected bool? IsReadOnly { get; set; }
        internal System.Reflection.PropertyInfo Property { get; set; }
        internal object SenderInstance { get; set; }

        List<IContentInfo> _Elements;
        internal ContentInfo(System.Reflection.PropertyInfo property, bool? isHidden, bool? isReadOnly, bool? isCloaked, List<IContentInfo> Elements)
        {
            IsHidden = isHidden;
            IsReadOnly = isReadOnly;
            IsCloaked = isCloaked;

            _Elements = Elements;
            Property = property;
        }

        void Add(IContentInfo element)
        {
            if (SenderInstance != null)
                element.SenderInstance = SenderInstance;
            
            element.Property = Property;

            if (IsReadOnly.HasValue)
                element.IsReadOnly = IsReadOnly.Value;
            if (IsHidden.HasValue)
                element.IsHidden = IsHidden.Value;
            if (IsHidden.HasValue)
                element.IsHidden = IsHidden.Value;

            _Elements.Add(element);
        }
        /// <summary>
        /// Possible return types: System.String, System.Int32, System.Int32[nullable], System.Decimal, System.Decimal[nullable], System.Guid
        /// </summary>
        /// <param name="title"></param>
        /// <param name="maxlength"></param>
        /// <param name="mandatory"></param>
        /// <param name="autoPostback"></param>
        /// <param name="interactiveHelp"></param>
        /// <param name="mustMatchRegex"></param>
        /// <returns></returns>
        public TextContentSettings TextField(string title, int? maxlength = null, bool mandatory = false, bool autoPostback = false, string interactiveHelp = null, string mustMatchRegex = null)
        {
            var element = new TextFieldAttribute(title, maxlength.GetValueOrDefault(), mandatory, autoPostback, interactiveHelp, mustMatchRegex);
            Add(element);
            return new TextContentSettings(element);
        }

        /// <summary>
        /// Possible return types: System.DateTime
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        /// <param name="interactiveHelp"></param>
        /// <returns></returns>
        public ContentSettings Date(string title, bool mandatory = false, string interactiveHelp = null)
        {
            var element = new DateAttribute(title, mandatory, interactiveHelp);
            Add(element);
            return new ContentSettings(element);
        }
        /// <summary>
        /// Possible return types: Possible return types: System.Int32, System.String, List<int>, List<string>
        /// </summary>
        /// <param name="title"></param>
        /// <param name="collectionPropertyName"></param>
        /// <param name="mandatory"></param>
        /// <param name="autoPostback"></param>
        /// <param name="interactiveHelp"></param>
        /// <returns></returns>
        public ContentSettings Dropdown(string title, string collectionPropertyName, bool mandatory = false, bool autoPostback = false, bool isMultiSelect = false, string interactiveHelp = null)
        {
            var element = new Choice_DropdownAttribute(title, collectionPropertyName, mandatory, autoPostback, interactiveHelp) { IsMultiSelect = isMultiSelect };
            Add(element);
            return new ContentSettings(element);
        }
        public ContentSettings Tagging(string title, string collectionPropertyName = null, bool mandatory = false, bool autoPostback = false, string interactiveHelp = null)
        {
            var element = new Choice_DropdownAttribute(title, collectionPropertyName, mandatory, autoPostback, interactiveHelp) { IsTagging = true, IsMultiSelect = true };
            Add(element);
            return new ContentSettings(element);
        }
        /// <summary>
        /// Possible return types: System.Int16, System.Int32, System.String, System.Int32[nullable]
        /// </summary>
        /// <param name="title"></param>
        /// <param name="collectionPropertyName"></param>
        /// <param name="groupName"></param>
        /// <param name="mandatory"></param>
        /// <param name="autoPostback"></param>
        /// <param name="interactiveHelp"></param>
        /// <returns></returns>
        public ContentSettings Radio(string title, string collectionPropertyName, string groupName, bool mandatory = false, bool autoPostback = false, string interactiveHelp = null)
        {
            var element = new Choice_RadioAttribute(title, collectionPropertyName, groupName, mandatory, autoPostback, interactiveHelp);
            Add(element);
            return new ContentSettings(element);
        }

        /// <summary>
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        /// <param name="interactiveHelp"></param>
        /// <returns>Sushi.Mediakiwi.Framework.FileUpload</returns>
        public ContentSettings FileUpload(string title, bool mandatory = false, string accept = null, string interactiveHelp = null)
        {
            var element = new FileUploadAttribute(title, mandatory, accept, interactiveHelp);
            Add(element);
            return new ContentSettings(element);
        }

        /// <summary>
        /// Possible return types: System.String
        /// </summary>
        /// <param name="title"></param>
        /// <param name="isClosedContainer"></param>
        /// <param name="isClosedStateReferringProperty"></param>
        /// <returns></returns>
        public ContentSettings Section(string title, bool isClosedContainer = false, bool canOpenClose = false, bool canDelete = false)
        {
            var element = new Sushi.Mediakiwi.Framework.ContentListItem.SectionAttribute(isClosedContainer, canOpenClose, canDelete, title);
            Add(element);
            return new ContentSettings(element);
        }
        /// <summary>
        /// Possible return types: System.Boolean
        /// </summary>
        /// <param name="title"></param>
        /// <param name="autoPostback"></param>
        /// <param name="interactiveHelp"></param>
        /// <returns></returns>
        public ContentSettings Checkbox(string title, bool autoPostback = false, string interactiveHelp = null)
        {
            var element = new Sushi.Mediakiwi.Framework.ContentListItem.Choice_CheckboxAttribute(title, autoPostback, interactiveHelp);
            Add(element);
            return new ContentSettings(element);
        }
        /// <summary>
        /// Possible return types: System.String
        /// </summary>
        /// <param name="title"></param>
        /// <param name="maxlength"></param>
        /// <param name="mandatory"></param>
        /// <param name="interactiveHelp"></param>
        /// <param name="mustMatchRegex"></param>
        /// <param name="isSourceCode"></param>
        /// <returns></returns>
        public ContentSettings TextArea(string title, int? maxlength = null, bool mandatory = false, string interactiveHelp = null, string mustMatchRegex = null, bool isSourceCode = false)
        {
            var element = new Sushi.Mediakiwi.Framework.ContentListItem.TextAreaAttribute(title, maxlength.GetValueOrDefault(), mandatory, interactiveHelp, mustMatchRegex, isSourceCode);
            Add(element);
            return new ContentSettings(element);
        }
        /// <summary>
        /// Possible return types: System.Boolean
        /// </summary>
        /// <param name="title"></param>
        /// <param name="triggerSave"></param>
        /// <returns></returns>
        public ButtonSettings Button(string title, ButtonTarget position = ButtonTarget.TopRight, bool triggerSave = true, bool triggerValidation = false, bool persistState = false)
        {
            var element = new Sushi.Mediakiwi.Framework.ContentListItem.ButtonAttribute(title, triggerSave, triggerValidation, persistState);
            element.IconTarget = position;
            Add(element);
            return new ButtonSettings(element);
        }
        /// <summary>
        /// Possible return types: System.Int32, System.String, Sushi.Mediakiwi.Data.Document, Sushi.Mediakiwi.Data.AssetInfo
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        /// <param name="gallery"></param>
        /// <param name="interactiveHelp"></param>
        /// <returns></returns>
        public ContentSettings Document(string title, bool mandatory = false, string gallery = null, string interactiveHelp = null)
        {
            var element = new Sushi.Mediakiwi.Framework.ContentListItem.Binary_DocumentAttribute(title, mandatory, gallery, interactiveHelp);
            Add(element);
            return new ContentSettings(element);
        }
        /// <summary>
        /// Possible return types: System.Int32, System.Int32[nullable], System.String, Sushi.Mediakiwi.Data.Image
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        /// <param name="gallery"></param>
        /// <param name="interactiveHelp"></param>
        /// <returns></returns>
        public ContentSettings Image(string title, bool mandatory = false, string gallery = null, string interactiveHelp = null)
        {
            var element = new Sushi.Mediakiwi.Framework.ContentListItem.Binary_ImageAttribute(title, mandatory, gallery, interactiveHelp);
            Add(element);
            return new ContentSettings(element);
        }
        /// <summary>
        /// Possible return types: System.DateTime
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        /// <param name="isDbSortField"></param>
        /// <param name="interactiveHelp"></param>
        /// <returns></returns>
        public ContentSettings DateTime(string title, bool mandatory = false, bool isDbSortField = false, string interactiveHelp = null)
        {
            var element = new Sushi.Mediakiwi.Framework.ContentListItem.DateTimeAttribute(title, mandatory, isDbSortField, interactiveHelp);
            Add(element);
            return new ContentSettings(element);
        }
        /// <summary>
        /// Possible return types: System.Int32, System.Int32[nullable], Sushi.Mediakiwi.Data.Folder
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        /// <param name="type"></param>
        /// <param name="interactiveHelp"></param>
        /// <returns></returns>
        public ContentSettings FolderSelect(string title, bool mandatory = false, Sushi.Mediakiwi.Data.FolderType type = Data.FolderType.List, string interactiveHelp = null)
        {
            var element = new Sushi.Mediakiwi.Framework.ContentListItem.FolderSelectAttribute(title, mandatory, type, interactiveHelp);
            Add(element);
            return new ContentSettings(element);
        }
        /// <summary>
        /// Possible return types: System.String
        /// </summary>
        /// <param name="noPadding"></param>
        /// <returns></returns>
        public ContentSettings HtmlContainer(bool noPadding = false)
        {
            var element = new Sushi.Mediakiwi.Framework.ContentListItem.HtmlContainerAttribute(noPadding);
            Add(element);
            return new ContentSettings(element);
        }
        /// <summary>
        /// Possible return types: System.Int32, Sushi.Mediakiwi.Data.Link
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        /// <param name="interactiveHelp"></param>
        /// <returns></returns>
        public ContentSettings Hyperlink(string title, bool mandatory = false, string interactiveHelp = null)
        {
            var element = new Sushi.Mediakiwi.Framework.ContentListItem.HyperlinkAttribute(title, mandatory, interactiveHelp);
            Add(element);
            return new ContentSettings(element);
        }
        /// <summary>
        /// Possible return types: System.String[], System.Int32[], string (CSV)
        /// </summary>
        /// <param name="title"></param>
        /// <param name="collectionPropertyName"></param>
        /// <param name="mandatory"></param>
        /// <param name="interactiveHelp"></param>
        /// <returns></returns>
        public ContentSettings ListItemSelect(string title, string collectionPropertyName, bool mandatory = false, string interactiveHelp = null)
        {
            var element = new Sushi.Mediakiwi.Framework.ContentListItem.ListItemSelectAttribute(title, collectionPropertyName, mandatory, interactiveHelp);
            Add(element);
            return new ContentSettings(element);
        }
        /// <summary>
        /// Possible return types: System.String
        /// </summary>
        /// <param name="title"></param>
        /// <param name="interactiveHelp"></param>
        /// <returns></returns>
        public ContentSettings MultiField(string title, string interactiveHelp = null)
        {
            var element = new MultiFieldAttribute(title, interactiveHelp);
            Add(element);
            return new ContentSettings(element);
        }
        /// <summary>
        /// Possible return types: System.String
        /// </summary>
        /// <returns></returns>
        public ContentSettings PageContainer()
        {
            var element = new PageContainerAttribute();
            Add(element);
            return new ContentSettings(element);
        }
        public void Component(IEnumerable<MetaData> metadata)
        {
            if (metadata != null)
            {
                foreach (var meta in metadata)
                {
                    Meta(meta);
                }
            }
        }

        private void Meta(MetaData metadata)
        {
            var element = metadata.GetContentInfo();
            element.FieldName = metadata.Name;
            element.Mandatory = metadata.Mandatory == "1";
            element.IsSharedField = metadata.IsSharedField == "1";

            if (!string.IsNullOrWhiteSpace(metadata.MaxValueLength))
            {
                element.MaxValueLength = Convert.ToInt32(metadata.MaxValueLength, System.Globalization.CultureInfo.InvariantCulture);
            }

            Add(element);
        }

        /// <summary>
        /// Possible return types: System.Int32, Sushi.Mediakiwi.Data.Page
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        /// <param name="interactiveHelp"></param>
        /// <returns></returns>
        public ContentSettings PageSelect(string title, bool mandatory = false, string interactiveHelp = null)
        {
            var element = new PageSelectAttribute(title, mandatory, interactiveHelp);
            Add(element);
            return new ContentSettings(element);
        }
        /// <summary>
        /// Possible return types: System.String
        /// </summary>
        /// <param name="title"></param>
        /// <param name="maxlength"></param>
        /// <param name="mandatory"></param>
        /// <param name="interactiveHelp"></param>
        /// <returns></returns>
        public ContentSettings RichText(string title, int? maxlength = null, bool mandatory = false, string interactiveHelp = null)
        {
            var element = new Sushi.Mediakiwi.Framework.ContentListItem.RichTextAttribute(title, maxlength.GetValueOrDefault(), mandatory, interactiveHelp);
            Add(element);
            return new ContentSettings(element);
        }
        /// <summary>
        /// Possible return types: Sushi.Mediakiwi.Data.SubList, Sushi.Mediakiwi.Data.iSubList
        /// </summary>
        /// <param name="title"></param>
        /// <param name="interactiveHelp"></param>
        /// <returns></returns>
        public ContentSettings SortList(string title, string interactiveHelp = null)
        {
            var element = new Sushi.Mediakiwi.Framework.ContentListItem.SubListSelectAttribute(title, null, false, true, interactiveHelp);
            Add(element);
            return new ContentSettings(element);
        }
        /// <summary>
        /// Possible return types: Sushi.Mediakiwi.Data.SubList, Sushi.Mediakiwi.Data.iSubList
        /// </summary>
        /// <param name="title"></param>
        /// <param name="componentlist"></param>
        /// <param name="mandatory"></param>
        /// <param name="canOnlyOrderSort"></param>
        /// <param name="interactiveHelp"></param>
        /// <returns></returns>
        public SublistSettings SubListSelect(string title, Type componentlist, bool mandatory = false, bool canOnlyOrderSort = false, bool autoPostback = false, string interactiveHelp = null)
        {
            var data = Data.ComponentList.SelectOne(componentlist.ToString());
            if (data != null && data.ID > 0)
            {
                var element = new SubListSelectAttribute(title, data.GUID.ToString(), mandatory, canOnlyOrderSort, interactiveHelp);
                element.AutoPostback = autoPostback;
                Add(element);
                return new SublistSettings(element);
            }
            return null;
        }

        /// <summary>
        /// Possible return types: Sushi.Mediakiwi.Data.SubList, Sushi.Mediakiwi.Data.iSubList
        /// </summary>
        /// <param name="title"></param>
        /// <param name="componentlistGuid"></param>
        /// <param name="mandatory"></param>
        /// <param name="canOnlyOrderSort"></param>
        /// <param name="interactiveHelp"></param>
        /// <returns></returns>
        internal SublistSettings SubListSelect(string title, Guid componentlistGuid, bool mandatory = false, bool canOnlyOrderSort = false, bool autoPostback = false, string interactiveHelp = null)
        {
            var data = Data.ComponentList.SelectOne(componentlistGuid);
            if (data != null && data.ID > 0)
            {
                var element = new SubListSelectAttribute(title, data.GUID.ToString(), mandatory, canOnlyOrderSort, interactiveHelp);
                element.AutoPostback = autoPostback;
                Add(element);
                return new SublistSettings(element);
            }
            return null;
        }

        /// <summary>
        /// Possible return types: System.String
        /// </summary>
        /// <param name="title"></param>
        /// <param name="IsClosedContainer"></param>
        /// <param name="interactiveHelp"></param>
        /// <returns></returns>
        public ContentSettings TextLine(string title, bool IsClosedContainer = false, string interactiveHelp = null)
        {
            var element = new Sushi.Mediakiwi.Framework.ContentListItem.TextLineAttribute(title, IsClosedContainer, interactiveHelp);
            Add(element);
            return new ContentSettings(element);
        }
    }
}
