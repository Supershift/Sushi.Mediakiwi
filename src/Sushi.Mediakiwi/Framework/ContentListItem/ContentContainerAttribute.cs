using Sushi.Mediakiwi.Data;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Framework.ContentListItem
{
    /// <summary>
    /// Possible return types: Sushi.Mediakiwi.Data.ContentContainer
    /// </summary>
    public class ContentContainerAttribute : ContentSharedAttribute, IContentInfo, IListContentInfo
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
                PropertyType = (Property == null) ? typeof(string).FullName : Property.PropertyType.FullName,
                VueType = Api.MediakiwiFormVueType.undefined,
                ClassName = InputClassName(IsValid(Mandatory)),
                ReadOnly = IsReadOnly,
                ContentTypeID = ContentTypeSelection,
                IsAutoPostback = m_AutoPostBack,
                IsMandatory = Mandatory,
                MaxLength = MaxValueLength,
                HelpText = InteractiveHelp,
                FormSection = GetFormMapClass(),
                Hidden = IsCloaked
            };
        }

        /// <summary>
        /// Possible return types: Sushi.Mediakiwi.Data.ContentContainer
        /// </summary>
        public ContentContainerAttribute()
        {
            this.ContentTypeSelection = ContentType.ContentContainer;
        }

        ///// <summary>
        ///// Initializes a new instance of the <see cref="ContentContainerAttribute"/> class.
        ///// </summary>
        ///// <param name="catalogGUID">The catalog GUID.</param>
        //public ContentContainerAttribute(string catalogGUID)
        //{
        //    this.ContentTypeSelection = ContentType.ContentContainer;
        //    this.SelectedCatalogGUID = new Guid(catalogGUID);
        //}

        //internal Guid SelectedCatalogGUID;

        #region IContentInfo Members


        /// <summary>
        /// Sets the candidate.
        /// </summary>
        /// <param name="isEditMode"></param>
        public void SetCandidate(bool isEditMode)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the candidate.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isEditMode"></param>
        public void SetCandidate(Field value, bool isEditMode)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the candidate.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <param name="isEditMode">if set to <c>true</c> [is edit mode].</param>
        /// <returns></returns>
        public Field WriteCandidate(WimControlBuilder build, bool isEditMode, bool isRequired, bool isCloaked)
        {
            this.IsCloaked = isCloaked;
            this.Mandatory = isRequired;
            //throw new NotImplementedException();
            return null;
        }

        #endregion
    }
}
