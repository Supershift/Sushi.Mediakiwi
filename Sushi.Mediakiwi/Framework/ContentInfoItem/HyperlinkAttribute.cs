using Sushi.Mediakiwi.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Sushi.Mediakiwi.Framework.ContentInfoItem
{
    /// <summary>
    /// Possible return types: System.Int32, Sushi.Mediakiwi.Data.Link
    /// </summary>
    public class HyperlinkAttribute : ContentSharedAttribute, IContentInfo
    {
        /// <summary>
        /// Possible return types: System.Int32, Sushi.Mediakiwi.Data.Link
        /// </summary>
        /// <param name="title"></param>
        public HyperlinkAttribute(string title)
            : this(title, false) { }

        /// <summary>
        /// Possible return types: System.Int32, Sushi.Mediakiwi.Data.Link
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        public HyperlinkAttribute(string title, bool mandatory)
            : this(title, mandatory, null) { }

        /// <summary>
        /// Possible return types: System.Int32, Sushi.Mediakiwi.Data.Link
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
            this.SetMultiFieldTitleHTML(Labels.ResourceManager.GetString("input_link", new CultureInfo(Console.CurrentApplicationUser.LanguageCulture)), "icon-external-link");

            if (Property != null && Property.PropertyType == typeof(Data.CustomData))
                SetContentContainer(field);

            m_Candidate = new Sushi.Mediakiwi.Data.Link();
            if (IsInitialLoad || !isEditMode)
            {
                if (IsBluePrint)
                {
                    if (field != null)
                    {
                        int candidate = Data.Utility.ConvertToInt(field.Value);
                        if (candidate > 0)
                            m_Candidate = Data.Link.SelectOne(candidate);
                    }
                }
                else
                {
                    if (Property.PropertyType == typeof(Data.CustomData))
                    {
                        int candidate = Data.Utility.ConvertToInt(m_ContentContainer[field.Property].Value);
                        if (candidate > 0)
                            m_Candidate = Data.Link.SelectOne(candidate);
                    }
                    else if (Property.PropertyType == typeof(Data.Link))
                        m_Candidate = Property.GetValue(SenderInstance, null) as Data.Link;
                    else if (Property.PropertyType == typeof(int?))
                    {
                        int? tmp = Property.GetValue(SenderInstance, null) as int?;
                        if (tmp.HasValue)
                            m_Candidate = Data.Link.SelectOne(tmp.Value);
                    }
                    else
                    {
                        int candidate = Data.Utility.ConvertToInt(Property.GetValue(SenderInstance, null));
                        if (candidate > 0)
                            m_Candidate = Data.Link.SelectOne(candidate);
                    }
                }
            }
            else
            {
                var candidate = SelectedID;
                if (candidate.HasValue)
                    m_Candidate = Data.Link.SelectOne(candidate.Value);
            }

            if (!IsBluePrint && Property != null && Property.CanWrite)
            {
                if (Property.PropertyType == typeof(Data.CustomData))
                    ApplyContentContainer(field, m_Candidate.ID.ToString());
                else if (Property.PropertyType == typeof(int))
                    Property.SetValue(SenderInstance, m_Candidate.ID, null);
                
                else if (Property.PropertyType == typeof(int?))
                {
                    if (m_Candidate != null && m_Candidate.ID != 0)
                        Property.SetValue(SenderInstance, m_Candidate.ID, null);
                    else
                        Property.SetValue(SenderInstance, null, null);
                }
                
                else
                    Property.SetValue(SenderInstance, m_Candidate, null);
            }

            if (m_Candidate.ID > 0)
            {
                OutputText = string.Format("<font class=\"abbr\" title=\"url: {0}\">{2}</font> <span>(open in {1})</span>", m_Candidate.GetUrl(Console.CurrentListInstance.wim.CurrentSite, false), m_Candidate.TargetInfo, m_Candidate.Text);
            }

            //  Inherited content section
            if (ShowInheritedData)
            {
                if (field != null && !string.IsNullOrEmpty(field.InheritedValue))
                {
                    int candidate2 = Data.Utility.ConvertToInt(field.InheritedValue);
                    if (candidate2 > 0)
                    {
                        Data.Link link = Data.Link.SelectOne(candidate2);
                        InhertitedOutputText = string.Format("<a title='url: {0}'>{2}</a> (open in {1})", link.GetUrl(Console.CurrentListInstance.wim.CurrentSite, false), link.TargetInfo, link.Text.Replace("+", " "));
                    }
                }
            }
        }

        Sushi.Mediakiwi.Data.Link m_Candidate;

        /// <summary>
        /// Writes the candidate.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <param name="isEditMode">if set to <c>true</c> [is edit mode].</param>
        /// <param name="isRequired">if set to <c>true</c> [is required].</param>
        /// <returns></returns>
        public Field WriteCandidate(WimControlBuilder build, bool isEditMode, bool isRequired, bool isCloaked)
        {
            this.SetWriteEnvironment();
            this.IsCloaked = isCloaked;
            this.Mandatory = isRequired;
            if (OverrideEditMode) isEditMode = false;
            if (isEditMode && this.IsEnabled())
            {
                Data.IComponentList list = Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.Links);

                string titleTag = string.Concat(Title, Mandatory ? "<em>*</em>" : "");
                int? key = null;
                if (m_Candidate != null)
                    key = m_Candidate.ID;

                ApplyItemSelect(build, true, true, titleTag, this.ID, list.ID.ToString(), null, false, isRequired, false, false, LayerSize.Normal, false, 400,
                    null, new NameItemValue() { Name = this.ID, ID = key, Value = OutputText }
                    );

            }
            else
            {
                build.Append(GetSimpleTextElement(this.Title, this.Mandatory, OutputText, this.InteractiveHelp));
            }
            return ReadCandidate(m_Candidate.ID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool IsValid(bool isRequired)
        {
            this.Mandatory = isRequired;
                if (Console.CurrentListInstance.wim.IsSaveMode)
                {
                    //  Custom error validation
                    if (!base.IsValid(isRequired))
                        return false;

                if (Mandatory)
                    return !(m_Candidate == null || m_Candidate.ID == 0);
                }
                return true;
            
        }


    }
}
