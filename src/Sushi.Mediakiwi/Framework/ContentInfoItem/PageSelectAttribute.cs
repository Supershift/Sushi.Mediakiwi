using Sushi.Mediakiwi.Data;
using System;
using System.Linq;
using System.Text;

namespace Sushi.Mediakiwi.Framework.ContentInfoItem
{
    /// <summary>
    /// Possible return types: System.Int32, Page
    /// </summary>
    public class PageSelectAttribute : ContentSharedAttribute, IContentInfo
    {
        /// <summary>
        /// Possible return types: System.Int32, Page
        /// </summary>
        /// <param name="title"></param>
        public PageSelectAttribute(string title)
            : this(title, false) { }

        /// <summary>
        /// Possible return types: System.Int32, Page
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        public PageSelectAttribute(string title, bool mandatory)
            : this(title, mandatory, null) { }

        /// <summary>
        /// Possible return types: System.Int32, Page
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        /// <param name="interactiveHelp"></param>
        public PageSelectAttribute(string title, bool mandatory, string interactiveHelp)
        {
            ContentTypeSelection = ContentType.PageSelect;
            Title = title;
            Mandatory = mandatory;
            InteractiveHelp = interactiveHelp;
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
        public void SetCandidate(Field field, bool isEditMode)
        {
            if (Property != null && Property.PropertyType == typeof(CustomData))
            {
                SetContentContainer(field);
            }

            m_Candidate = new Page();
            if (IsInitialLoad || !isEditMode)
            {
                if (IsBluePrint)
                {
                    if (field != null)
                    {
                        int candidate = Utility.ConvertToInt(field.Value);
                        if (candidate > 0)
                        {
                            m_Candidate = Page.SelectOne(candidate, false);
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
                            m_Candidate = Page.SelectOne(candidate, false);
                        }
                    }
                    else if (Property.PropertyType == typeof(Page))
                    {
                        m_Candidate = Property.GetValue(SenderInstance, null) as Page;
                    }
                    else if (Property.PropertyType == typeof(int))
                    {
                        int candidate = Utility.ConvertToInt(Property.GetValue(SenderInstance, null));
                        if (candidate > 0)
                        {
                            m_Candidate = Page.SelectOne(candidate, false);
                        }
                    }
                    else if (Property.PropertyType == typeof(int?))
                    {
                        object candidate = Utility.ConvertToInt(Property.GetValue(SenderInstance, null));
                        if (candidate != null)
                        {
                            m_Candidate = Page.SelectOne(Utility.ConvertToInt(candidate));
                        }
                    }
                }
            }
            else
            {
                int candidate = Utility.ConvertToInt(Console.Form(ID));
                if (candidate > 0)
                {
                    m_Candidate = Page.SelectOne(candidate, false);
                }
            }

            //  Possible return types: System.Int32, Page
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
            OutputText = m_Candidate.HRef;


            //  Inherited content section
            if (ShowInheritedData && field != null && !string.IsNullOrEmpty(field.InheritedValue))
            {
                int candidate2 = Utility.ConvertToInt(field.InheritedValue);
                if (candidate2 > 0)
                {
                    Page page = Page.SelectOne(candidate2);
                    if (page != null && page.ID != 0)
                    {
                        InhertitedOutputText = page.HRef.Replace("+", " ");
                    }
                }
            }
        }

        Page m_Candidate;


        /// <summary>
        /// Writes the candidate.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <param name="isEditMode"></param>
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


            bool isEnabled = IsEnabled();
            string outputValue = OutputText;

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
                #region Element creation

                Folder currentFolder;

                if (m_Candidate.FolderID == 0)
                {
                    int siteWithPages = 0;
                    if (Console.CurrentListInstance.wim.CurrentSite.HasPages)
                    {
                        siteWithPages = Console.CurrentListInstance.wim.CurrentSite.ID;
                    }
                    else
                    {
                        if (Data.Environment.Current.DefaultSiteID.HasValue)
                        {
                            Site defaultSite = Site.SelectOne(Data.Environment.Current.DefaultSiteID.Value);
                            if (defaultSite.HasPages)
                            {
                                siteWithPages = defaultSite.ID;
                            }
                        }
                    }
                    currentFolder = Folder.SelectOneBySite(siteWithPages, FolderType.Page);
                }
                else
                {
                    currentFolder = m_Candidate.Folder;
                }

                string titleTag = string.Concat(Title, Mandatory ? "<em>*</em>" : "");

                var pages = Page.SelectAll().OrderBy(x => x.InternalPath);

                string options = null;
                string key = "Page.Options";

                StringBuilder optiondata = new StringBuilder();
                optiondata.Append("<option></option>");
                int count = 0;
                foreach (var item in pages)
                {
                    count++;
                    optiondata.AppendFormat("<option value=\"{0}\"{2}>{1}</option>"
                        , item.ID
                        , item.InternalPath
                        , m_Candidate.ID == item.ID ? " selected=\"selected\"" : string.Empty);
                }
                options = optiondata.ToString();

                System.Diagnostics.Trace.WriteLine($"step 3a");

                StringBuilder element = new StringBuilder();

                element.AppendFormat("<select id=\"{0}\"{1} name=\"{0}\">{2}"
                 , ID
                 , InputClassName(IsValid(isRequired), "styled")
                 //, isEnabled ? null : " disabled=\"disabled\""
                 , options
                 );
                element.Append("</select>");


                System.Diagnostics.Trace.WriteLine($"step 4");

                #endregion Element creation

                if (IsCloaked)
                {
                    build.AppendCloaked(element.ToString());
                }
                else
                {
                    #region Wrapper
                    if (ShowInheritedData)
                    {
                        ApplyTranslation(build);
                    }
                    else
                    {
                        if ((Console.ExpressionPrevious == OutputExpression.Left && Expression == OutputExpression.FullWidth) || (Console.ExpressionPrevious == OutputExpression.Left && Expression == OutputExpression.Left))
                            build.Append("<th><label>&nbsp;</label></th><td>&nbsp;</td></tr>");

                        if ((Console.ExpressionPrevious == OutputExpression.FullWidth && Expression == OutputExpression.Right) || (Console.ExpressionPrevious == OutputExpression.Right && Expression == OutputExpression.Right))
                            build.Append("<tr><th><label>&nbsp;</label></th><td>&nbsp;</td>");

                        if (Expression == OutputExpression.FullWidth || Expression == OutputExpression.Left)
                            build.Append("<tr>");
                    }

                    build.AppendFormat("<th><label for=\"{0}\">{1}</label></th>", ID, TitleLabel);

                    build.AppendFormat("<td{0}{1}>{2}"
                        , (Expression == OutputExpression.FullWidth && Console.HasDoubleCols) ? " colspan=\"3\"" : null
                        , InputCellClassName(IsValid(isRequired))
                        , CustomErrorText
                        );

                    build.AppendFormat("<div class=\"{0}\">", (Expression == OutputExpression.FullWidth) ? Class_Wide : "half");

                    build.Append(element.ToString());
                    build.Append("</div></td>");

                    if (Expression == OutputExpression.FullWidth || Expression == OutputExpression.Right)
                    {
                        build.Append("</tr>");
                    }

                    #endregion Wrapper
                }

            }
            else
            {
                build.Append(GetSimpleTextElement(OutputText));
            }

            return ReadCandidate(m_Candidate.ID);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        /// <returns></returns>
        public override bool IsValid(bool isRequired)
        {
            Mandatory = isRequired;
            if (Console.CurrentListInstance.wim.IsSaveMode)
            {
                //  Custom error validation
                if (!base.IsValid(isRequired))
                {
                    return false;
                }

                if (Mandatory && (m_Candidate == null || m_Candidate.ID == 0))
                {
                    if (IsSharedField)
                    {
                        // [MR:03-06-2021] Apply shared field clickable icon.
                        var sharedInfoApply = ApplySharedFieldInformation(IsEnabled(), OutputText);

                        // If we have a document assigned, overwrite the current one
                        if (sharedInfoApply.isShared && string.IsNullOrWhiteSpace(sharedInfoApply.outputValue) == false)
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }
            return true;

        }
    }
}
