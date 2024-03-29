using Sushi.Mediakiwi.Data;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Framework.ContentListItem
{
    /// <summary>
    /// Possible return types: System.Object, when apply Sushi.Mediakiwi.Data.Content
    /// </summary>
    public class DataExtendAttribute : ContentSharedAttribute, IContentInfo, IListContentInfo
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
        /// 
        /// </summary>
        public DataExtendAttribute()
        {
            ContentTypeSelection = ContentType.DataExtend;
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
            //string candidate = null;

            m_Implement = Property.GetValue(SenderInstance, null);
            //if (value != null)
            //    candidate = value.ToString();

            //int listInt;
            //if (Data.Utility.IsNumeric(candidate, out listInt))
            //    m_List = Data.ComponentList.SelectOne(listInt);

            //Guid listGuid;
            //if (Data.Utility.IsGuid(candidate, out listGuid))
            //    m_List = Data.ComponentList.SelectOne(listGuid);
        }

        internal object m_Implement;

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
            //Beta.GeneratedCms.Source.GridCreation grid = new Sushi.Mediakiwi.Beta.GeneratedCms.Source.GridCreation();
            //Beta.GeneratedCms.Console tmp = Console.ReplicateInstance(this.m_List);

            //build.Append("</tbody></table><br/>");
            //build.Append(grid.GetGridFromListInstance(tmp.CurrentListInstance.wim, tmp, 0));
            //build.Append("<table><tbody>");

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