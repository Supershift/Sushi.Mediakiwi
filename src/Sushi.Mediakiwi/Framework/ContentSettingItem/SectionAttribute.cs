using Sushi.Mediakiwi.Data;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Framework.ContentSettingItem
{
    /// <summary>
    /// Possible return types: System.String
    /// </summary>
    public class SectionAttribute : ContentSharedAttribute, IContentInfo, IContentSettingInfo
    {
        public async Task<Api.MediakiwiField> GetApiFieldAsync()
        {
            return new Api.MediakiwiField()
            {
                Title = Title,
                Value = OutputText,
                Expression = Expression,
                PropertyName = ID,
                PropertyType = (Property == null) ? typeof(string).FullName : Property.PropertyType.FullName,
                VueType = Api.MediakiwiFormVueType.wimSection,
                ContentTypeID = ContentTypeSelection,
                IsAutoPostback = m_AutoPostBack,
                IsMandatory = Mandatory,
                MaxLength = MaxValueLength,
                HelpText = InteractiveHelp,
                FormSection = GetFormMapClass()
            };
        }

        /// <summary>
        /// Possible return types: System.String
        /// </summary>
        public SectionAttribute()
            : this(false, null) { }

        /// <summary>
        /// Possible return types: System.String
        /// </summary>
        /// <param name="isClosedContainer">if set to <c>true</c> [is closed container].</param>
        public SectionAttribute(bool isClosedContainer)
            : this(isClosedContainer, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SectionAttribute" /> class.
        /// </summary>
        /// <param name="isClosedContainer">if set to <c>true</c> [is closed container].</param>
        public SectionAttribute(string title)
            : this(false, null, title) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SectionAttribute"/> class.
        /// </summary>
        /// <param name="isClosedContainer">if set to <c>true</c> [is closed container].</param>
        /// <param name="isClosedStateReferringProperty">The is closed state referring property.</param>
        public SectionAttribute(bool isClosedContainer, string isClosedStateReferringProperty)
            : this (isClosedContainer, isClosedStateReferringProperty, null) {}

        /// <summary>
        /// Possible return types: System.String
        /// </summary>
        /// <param name="isClosedContainer">if set to <c>true</c> [is closed container].</param>
        /// <param name="isClosedStateReferringProperty">The is closed state referring property (this value overrules isClosedContainer).</param>
        /// <param name="title">The title.</param>
        public SectionAttribute(bool isClosedContainer, string isClosedStateReferringProperty, string title)
        {
            ContentTypeSelection = ContentType.Section;
            m_IsClosedContainer = isClosedContainer;
            m_IsClosedStateReferringProperty = isClosedStateReferringProperty;
            OutputText = title;
        }

        internal bool m_IsClosedContainer;
        internal string m_IsClosedStateReferringProperty;

        /// <summary>
        /// Gets a value indicating whether this instance is closed state referring property.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is closed state referring property; otherwise, <c>false</c>.
        /// </value>
        internal bool IsClosedStateReferringProperty
        {
            get
            {
                return (bool)GetProperty(SenderInstance, m_IsClosedStateReferringProperty);
            }
        }

        /// <summary>
        /// Sets the candidate.
        /// </summary>
        /// <param name="isEditMode"></param>
        public void SetCandidate(bool isEditMode)
        {
            SetCandidate(null, isEditMode);
        }

        /// <summary>
        /// Sets the candidate.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="isEditMode">if set to <c>true</c> [is edit mode].</param>
        public void SetCandidate(Field field, bool isEditMode)
        {
            string candidate = null;

            if (string.IsNullOrEmpty(this.OutputText))
            {
                object value = Property.GetValue(SenderInstance, null);
                if (value != null)
                    candidate = value.ToString();
                OutputText = candidate;
            }
        }

        /// <summary>
        /// Writes the candidate.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <param name="isEditMode"></param>
        /// <returns></returns>
        public Field WriteCandidate(WimControlBuilder build, bool isEditMode, bool isRequired, bool isCloaked)
        {
            this.Mandatory = isRequired;
            this.IsCloaked = isCloaked;

            build.Append(GetSimpleTextElement(OutputText));
            return null;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        /// <returns></returns>
        [System.Xml.Serialization.XmlIgnore()]
        public new bool IsValid
        {
            get
            {
                return true;
            }
        }
    }
}