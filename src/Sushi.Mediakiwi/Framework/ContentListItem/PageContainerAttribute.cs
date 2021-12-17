using Sushi.Mediakiwi.Data;
using System;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Framework.ContentListItem
{
    /// <summary>
    /// Possible return types: System.String
    /// </summary>
    public class PageContainerAttribute : ContentSharedAttribute, IContentInfo, IListContentInfo
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
        /// Initializes a new instance of the <see cref="HtmlContainerAttribute"/> class.
        /// </summary>
        public PageContainerAttribute()
        {
        }

        /// <summary>
        /// Sets the candidate.
        /// </summary>
        /// <param name="isEditMode"></param>
        public void SetCandidate(bool isEditMode)
        {
        }

        Page m_PageInstance;

        /// <summary>
        /// Sets the candidate.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="isEditMode">if set to <c>true</c> [is edit mode].</param>
        public void SetCandidate(Field field, bool isEditMode)
        {
            m_PageInstance = Property.GetValue(SenderInstance, null) as Page;
        }

        /// <summary>
        /// Writes the candidate.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <param name="isEditMode"></param>
        /// <returns></returns>
        public Field WriteCandidate(WimControlBuilder build, bool isEditMode, bool isRequired, bool isCloaked)
        {
            this.IsCloaked = isCloaked;
            this.Mandatory = isRequired;
            //6233

            if (m_PageInstance != null && m_PageInstance.ID > 0)
            {
                Console.Item = m_PageInstance.ID;
                Console.ItemType = RequestItemType.Page;

                Beta.GeneratedCms.Source.Component component = new Beta.GeneratedCms.Source.Component();

                WimControlBuilder createdList2 = component.CreateContentList(this.Console, 0, false, out m_PageInstance, null, true);
                build.Append(createdList2.ToString());
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool IsValid(bool isRequired)
        {
            this.Mandatory = isRequired;
            return true;
        }
    }
}


    
