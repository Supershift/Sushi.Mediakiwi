using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.UI;
using System;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Framework.ContentInfoItem
{
    /// <summary>
    /// Possible return types: System.String[], System.Int32[]
    /// </summary>
    public class ListItemSelectAttribute : ContentSharedAttribute, IContentInfo
    {
        public async Task<Api.MediakiwiField> GetApiFieldAsync()
        {
            return new Api.MediakiwiField()
            {
                Event = m_AutoPostBack ? Api.MediakiwiJSEvent.Change : Api.MediakiwiJSEvent.None,
                Title = MandatoryWrap(Title),
                Value = OutputText,
                Expression = Expression,
                PropertyName = ID,
                PropertyType = (Property == null) ? typeof(DateTime).FullName : Property.PropertyType.FullName,
                VueType = Api.MediakiwiFormVueType.undefined,
                ReadOnly = IsReadOnly,
                ContentTypeID = ContentTypeSelection,
                IsAutoPostback = m_AutoPostBack,
                IsMandatory = Mandatory,
                MaxLength = MaxValueLength,
                HelpText = InteractiveHelp,
                FormSection = GetFormMapClass()
            };
        }

        /// <summary>
        /// Possible return types: System.String[], System.Int32[], string (CSV)
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="collectionPropertyName">Name of the collection property.</param>
        public ListItemSelectAttribute(string title, string collectionPropertyName)
            : this(title, collectionPropertyName, false) { }

        /// <summary>
        /// Possible return types: System.String[], System.Int32[], string (CSV)
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="collectionPropertyName">Name of the collection property.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        public ListItemSelectAttribute(string title, string collectionPropertyName, bool mandatory)
            : this(title, collectionPropertyName, mandatory, null) { }

        /// <summary>
        /// Possible return types: System.String[], System.Int32[], string (CSV)
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="collectionPropertyName">Name of the collection property.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        public ListItemSelectAttribute(string title, string collectionPropertyName, bool mandatory, string interactiveHelp)
        {
            ContentTypeSelection = ContentType.ListItemSelect;
            Title = title;
            CollectionProperty = collectionPropertyName;
            Mandatory = mandatory;
            InteractiveHelp = interactiveHelp;
        }

        private string m_CollectionProperty;
        /// <summary>
        /// 
        /// </summary>
        public string CollectionProperty
        {
            set { m_CollectionProperty = value; }
            get { return m_CollectionProperty; }
        }

        private bool m_CanReuseItem;
        /// <summary>
        /// 
        /// </summary>
        public bool CanReuseItem
        {
            set { m_CanReuseItem = value; }
            get { return m_CanReuseItem; }
        }

        /// <summary>
        /// Sets the candidate.
        /// </summary>
        /// <param name="isEditMode"></param>
        public void SetCandidate(bool isEditMode)
        {
            SetCandidate(null, isEditMode);
        }

        Choice_DropdownAttribute _ReplacementAttribute;

        /// <summary>
        /// Sets the candidate.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="isEditMode">if set to <c>true</c> [is edit mode].</param>
        public void SetCandidate(Field field, bool isEditMode)
        {

            _ReplacementAttribute = new Choice_DropdownAttribute(Title, Collection);
            Utility.ReflectProperty(this, _ReplacementAttribute);
            _ReplacementAttribute.Console = Console;
            _ReplacementAttribute.m_ListItemCollection = m_ListItemCollection;
            _ReplacementAttribute.CollectionProperty = CollectionProperty;
            _ReplacementAttribute.SetCandidate(field, isEditMode);
        }

        string[] m_Candidate;

        ListItemCollection m_CollectionSelected;

        /// <summary>
        /// Writes the candidate.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <param name="isEditMode">if set to <c>true</c> [is edit mode].</param>
        /// <param name="isRequired">if set to <c>true</c> [is required].</param>
        /// <returns></returns>
        public Field WriteCandidate(WimControlBuilder build, bool isEditMode, bool isRequired, bool isCloaked)
        {
            return _ReplacementAttribute.WriteCandidate(build, isEditMode, isRequired, isCloaked);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool IsValid(bool isRequired)
        {
            Mandatory = isRequired;
            if (Console?.CurrentListInstance?.wim?.IsSaveMode == true)
            {
                //  Custom error validation
                if (!base.IsValid(isRequired))
                {
                    return false;
                }

                if (Mandatory && (m_Candidate == null || m_Candidate.Length == 0))
                {
                    var hasValue = HasSharedValue();
                    if (hasValue.isSharedField)
                    {
                        return hasValue.hasValue;
                    }

                    return false;
                }
            }
            return true;

        }
    }
}
