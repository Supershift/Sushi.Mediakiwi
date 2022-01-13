using Sushi.Mediakiwi.Data;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Framework.ContentInfoItem
{
    /// <summary>
    /// Possible return types: System.Int32, Link
    /// </summary>
    public class HyperlinkAttribute : ContentSharedAttribute, IContentInfo
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
        /// Possible return types: System.Int32, Link
        /// </summary>
        /// <param name="title"></param>
        public HyperlinkAttribute(string title)
            : this(title, false) { }

        /// <summary>
        /// Possible return types: System.Int32, Link
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        public HyperlinkAttribute(string title, bool mandatory)
            : this(title, mandatory, null) { }

        /// <summary>
        /// Possible return types: System.Int32, Link
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        /// <param name="interactiveHelp"></param>
        public HyperlinkAttribute(string title, bool mandatory, string interactiveHelp)
        {
            ContentTypeSelection = ContentType.Hyperlink;
            Title = title;
            Mandatory = mandatory;
            InteractiveHelp = interactiveHelp;
        }

        /// <summary>
        /// Sets the candidate.
        /// </summary>
        /// <param name="isEditMode">if set to <c>true</c> [is edit mode].</param>
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
            SetMultiFieldTitleHTML(Labels.ResourceManager.GetString("input_link", new CultureInfo(Console.CurrentApplicationUser.LanguageCulture)), "icon-external-link");

            if (Property != null && Property.PropertyType == typeof(CustomData))
                SetContentContainer(field);

            m_Candidate = new Link();
            if (IsInitialLoad || !isEditMode)
            {
                if (IsBluePrint)
                {
                    if (field != null)
                    {
                        int candidate = Utility.ConvertToInt(field.Value);
                        if (candidate > 0)
                        {
                            m_Candidate = Link.SelectOne(candidate);
                        }
                    }
                }
                else
                {
                    if (Property.PropertyType == typeof(CustomData))
                    {
                        int candidate = Utility.ConvertToInt(m_ContentContainer[field.Property].Value);
                        if (candidate > 0)
                        {
                            m_Candidate = Link.SelectOne(candidate);
                        }
                    }
                    else if (Property.PropertyType == typeof(Link))
                    {
                        m_Candidate = Property.GetValue(SenderInstance, null) as Link;
                    }
                    else if (Property.PropertyType == typeof(int?))
                    {
                        int? tmp = Property.GetValue(SenderInstance, null) as int?;
                        if (tmp.HasValue)
                        {
                            m_Candidate = Link.SelectOne(tmp.Value);
                        }
                    }
                    else
                    {
                        int candidate = Utility.ConvertToInt(Property.GetValue(SenderInstance, null));
                        if (candidate > 0)
                        {
                            m_Candidate = Link.SelectOne(candidate);
                        }
                    }
                }
            }
            else
            {
                var candidate = SelectedID;
                if (candidate.HasValue)
                {
                    m_Candidate = Link.SelectOne(candidate.Value);
                }
            }

            if (!IsBluePrint && Property != null && Property.CanWrite)
            {
                if (Property.PropertyType == typeof(CustomData))
                {
                    ApplyContentContainer(field, m_Candidate.ID.ToString());
                }
                else if (Property.PropertyType == typeof(int))
                {
                    Property.SetValue(SenderInstance, m_Candidate.ID, null);
                }
                else if (Property.PropertyType == typeof(int?))
                {
                    if (m_Candidate != null && m_Candidate.ID != 0)
                    {
                        Property.SetValue(SenderInstance, m_Candidate.ID, null);
                    }
                    else
                    {
                        Property.SetValue(SenderInstance, null, null);
                    }
                }

                else
                {
                    Property.SetValue(SenderInstance, m_Candidate, null);
                }
            }

            if (m_Candidate.ID > 0)
            {
                OutputText = string.Format("<font class=\"abbr\" title=\"url: {0}\">{2}</font> <span>(open in {1})</span>", m_Candidate.GetUrl(Console.CurrentListInstance.wim.CurrentSite, false), m_Candidate.TargetInfo, m_Candidate.Text);
            }

            //  Inherited content section
            if (ShowInheritedData && field != null && !string.IsNullOrEmpty(field.InheritedValue))
            {
                int candidate2 = Utility.ConvertToInt(field.InheritedValue);
                if (candidate2 > 0)
                {
                    Link link = Link.SelectOne(candidate2);
                    InhertitedOutputText = string.Format("<a title='url: {0}'>{2}</a> (open in {1})", link.GetUrl(Console.CurrentListInstance.wim.CurrentSite, false), link.TargetInfo, link.Text.Replace("+", " "));
                }
            }
        }

        Link m_Candidate;

        /// <summary>
        /// Writes the candidate.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <param name="isEditMode">if set to <c>true</c> [is edit mode].</param>
        /// <param name="isRequired">if set to <c>true</c> [is required].</param>
        /// <returns></returns>
        public Field WriteCandidate(WimControlBuilder build, bool isEditMode, bool isRequired, bool isCloaked)
        {
            SetWriteEnvironment();
            IsCloaked = isCloaked;
            Mandatory = isRequired;
            if (OverrideEditMode)
            {
                isEditMode = false;
            }

            string outputValue = OutputText;

            bool isEnabled = IsEnabled();

            // [MR:03-06-2021] Apply shared field clickable icon.
            var sharedInfoApply = ApplySharedFieldInformation(isEnabled, outputValue);

            // If we have a document assigned, overwrite the current one
            if (sharedInfoApply.isShared)
            {
                // Enable readonly when shared
                isEnabled = sharedInfoApply.isEnabled;

                // When Currently not cloaked, do so if its a shared field
                if (IsCloaked == false && sharedInfoApply.isHidden)
                {
                    IsCloaked = sharedInfoApply.isHidden;
                }

                OutputText = sharedInfoApply.outputValue;
            }

            if (isEditMode && isEnabled)
            {
                IComponentList list = ComponentList.SelectOne(ComponentListType.Links);

                string titleTag = string.Concat(Title, Mandatory ? "<em>*</em>" : "");
                int? key = null;
                if (m_Candidate != null)
                    key = m_Candidate.ID;

                ApplyItemSelect(build, true, true, titleTag, ID, list.ID.ToString(), null, false, isRequired, false, false, LayerSize.Normal, false, 450,
                    null, new NameItemValue() { Name = ID, ID = key, Value = OutputText }
                    );

            }
            else
            {
                build.Append(GetSimpleTextElement(OutputText));
            }
            return ReadCandidate(m_Candidate.ID);
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

                if (Mandatory)
                {
                    var hasValue = HasSharedValue();
                    if (hasValue.isSharedField)
                    {
                        return hasValue.hasValue;
                    }

                    return (m_Candidate?.ID > 0);
                }
            }
            return true;
        }
    }
}
