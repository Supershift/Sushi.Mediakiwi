using Sushi.Mediakiwi.Data;
using System;
using System.Collections.Generic;
using System.Text;


namespace Sushi.Mediakiwi.Framework.ContentInfoItem
{
    /// <summary>
    /// Possible return types: System.Int32, Sushi.Mediakiwi.Data.Page
    /// </summary>
    public class PageSelectAttribute : ContentSharedAttribute, IContentInfo
    {
        /// <summary>
        /// Possible return types: System.Int32, Sushi.Mediakiwi.Data.Page
        /// </summary>
        /// <param name="title"></param>
        public PageSelectAttribute(string title)
            : this(title, false) { }

        /// <summary>
        /// Possible return types: System.Int32, Sushi.Mediakiwi.Data.Page
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        public PageSelectAttribute(string title, bool mandatory)
            : this(title, mandatory, null) { }

        /// <summary>
        /// Possible return types: System.Int32, Sushi.Mediakiwi.Data.Page
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
            if (Property != null && Property.PropertyType == typeof(Data.CustomData))
                SetContentContainer(field);

            m_Candidate = new Sushi.Mediakiwi.Data.Page();
            if (IsInitialLoad || !isEditMode)
            {
                if (IsBluePrint)
                {
                    if (field != null)
                    {
                        int candidate = Data.Utility.ConvertToInt(field.Value);
                        if (candidate > 0)
                            m_Candidate = Data.Page.SelectOne(candidate, false);
                    }
                }
                else
                {
                    if (Property.PropertyType == typeof(Data.CustomData))
                    {
                        int candidate = Data.Utility.ConvertToInt(m_ContentContainer[field.Property].Value);
                        if (candidate > 0)
                            m_Candidate = Data.Page.SelectOne(candidate, false);
                    }
                    else if (Property.PropertyType == typeof(Data.Page))
                        m_Candidate = Property.GetValue(SenderInstance, null) as Data.Page;
                    else if (Property.PropertyType == typeof(int))
                    {
                        int candidate = Data.Utility.ConvertToInt(Property.GetValue(SenderInstance, null));
                        if (candidate > 0)
                            m_Candidate = Data.Page.SelectOne(candidate, false);
                    }
                    else if (Property.PropertyType == typeof(int?))
                    {
                        object candidate = Data.Utility.ConvertToInt(Property.GetValue(SenderInstance, null));
                        if (candidate != null)
                            m_Candidate = Data.Page.SelectOne(Data.Utility.ConvertToInt(candidate));
                    }
                }
            }
            else
            {
                int candidate = Data.Utility.ConvertToInt(Console.Form(this.ID));
                if (candidate > 0)
                    m_Candidate = Data.Page.SelectOne(candidate, false);
            }

            //  Possible return types: System.Int32, Sushi.Mediakiwi.Data.Page
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
            OutputText = m_Candidate.HRef;


            //  Inherited content section
            if (ShowInheritedData)
            {
                if (field != null && !string.IsNullOrEmpty(field.InheritedValue))
                {
                    int candidate2 = Data.Utility.ConvertToInt(field.InheritedValue);
                    if (candidate2 > 0)
                    {
                        Data.Page page = Data.Page.SelectOne(candidate2);
                        if (page != null && page.ID != 0)
                            InhertitedOutputText = page.HRef.Replace("+", " ");
                    }
                }
            }
        }

        Sushi.Mediakiwi.Data.Page m_Candidate;


        /// <summary>
        /// Writes the candidate.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <param name="isEditMode"></param>
        /// <returns></returns>
        public Field WriteCandidate(WimControlBuilder build, bool isEditMode, bool isRequired, bool isCloaked)
        {
            this.SetWriteEnvironment();
            this.IsCloaked = isCloaked;
            this.Mandatory = isRequired;
            if (OverrideEditMode) isEditMode = false;
            if (isEditMode)
            {
                #region Element creation

                System.Diagnostics.Trace.WriteLine($"step 1");

                Sushi.Mediakiwi.Data.Folder currentFolder;

                if (m_Candidate.FolderID == 0)
                {
                    int siteWithPages = 0;
                    if (this.Console.CurrentListInstance.wim.CurrentSite.HasPages)
                    {
                        siteWithPages = this.Console.CurrentListInstance.wim.CurrentSite.ID;
                    }
                    else
                    {
                        if (Sushi.Mediakiwi.Data.Environment.Current.DefaultSiteID.HasValue)
                        {
                            Sushi.Mediakiwi.Data.Site defaultSite = Sushi.Mediakiwi.Data.Site.SelectOne(Sushi.Mediakiwi.Data.Environment.Current.DefaultSiteID.Value);
                            if (defaultSite.HasPages)
                                siteWithPages = defaultSite.ID;
                        }
                    }
                    currentFolder = Sushi.Mediakiwi.Data.Folder.SelectOneBySite(siteWithPages, Sushi.Mediakiwi.Data.FolderType.Page);
                }
                else
                    currentFolder = m_Candidate.Folder;

                System.Diagnostics.Trace.WriteLine($"step 2");

                string titleTag = string.Concat(Title, Mandatory ? "<em>*</em>" : "");

                var pages = Data.Page.SelectAll();


                System.Diagnostics.Trace.WriteLine($"step 3: pages = {pages.Length}");

                string options = null;
                string key = "Sushi.Mediakiwi.Data.Page.Options";
              
                StringBuilder optiondata = new StringBuilder();
                optiondata.Append("<option></option>");
                int count = 0;
                foreach (var item in pages)
                {
                    count++;
                    optiondata.AppendFormat("\n\t\t\t\t\t\t\t\t\t\t<option value=\"{0}\"{2}>{1}</option>"
                        , item.ID, item.CompletePath, m_Candidate.ID == item.ID ? " selected=\"selected\"" : string.Empty);
                }
                options = optiondata.ToString();
              
                System.Diagnostics.Trace.WriteLine($"step 3a");

                StringBuilder element = new StringBuilder();

                element.AppendFormat("\n\t\t\t\t\t\t\t\t\t<select id=\"{0}\"{1} name=\"{0}\">{2}"
                 , this.ID
                 , this.InputClassName(IsValid(isRequired), "styled")
                 //, isEnabled ? null : " disabled=\"disabled\""
                 , options
                 );
                element.Append("\n\t\t\t\t\t\t\t\t\t</select>");


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
                        this.ApplyTranslation(build);
                    }
                    else
                    {
                        if ((Console.ExpressionPrevious == OutputExpression.Left && Expression == OutputExpression.FullWidth) || (Console.ExpressionPrevious == OutputExpression.Left && Expression == OutputExpression.Left))
                            build.Append("\t\t\t\t\t\t\t<th><label>&nbsp;</label></th><td>&nbsp;</td></tr>");

                        if ((Console.ExpressionPrevious == OutputExpression.FullWidth && Expression == OutputExpression.Right) || (Console.ExpressionPrevious == OutputExpression.Right && Expression == OutputExpression.Right))
                            build.Append("\t\t\t\t\t\t<tr><th><label>&nbsp;</label></th>\n\t\t\t\t\t\t\t<td>&nbsp;</td>");

                        if (Expression == OutputExpression.FullWidth || Expression == OutputExpression.Left)
                            build.Append("\t\t\t\t\t\t<tr>");
                    }

                    build.AppendFormat("\n\t\t\t\t\t\t\t<th><label for=\"{0}\">{1}</label></th>", this.ID, this.TitleLabel);

                    //if (ShowInheritedData)
                    //    build.AppendFormat("\t\t\t\t\t\t\t<th class=\"local\"><label>{0}:</label></th>\t\t\t\t\t\t</tr>\t\t\t\t\t\t<tr>\t\t\t\t\t\t\t<td><div class=\"description\">{1}</div></td>\n", this.ID, this.TitleLabel);

                    build.AppendFormat("\n\t\t\t\t\t\t\t<td{0}{1}>{2}"
                        , (Expression == OutputExpression.FullWidth && Console.HasDoubleCols) ? " colspan=\"3\"" : null
                        , this.InputCellClassName(this.IsValid(isRequired))
                        , CustomErrorText
                        );

                    build.AppendFormat("\n\t\t\t\t\t\t\t\t<div class=\"{0}\">", (Expression == OutputExpression.FullWidth) ? this.Class_Wide : "half");

                    build.Append(element.ToString());
                    build.Append("\n\t\t\t\t\t\t\t\t</div>");
                    build.Append("\n\t\t\t\t\t\t\t</td>");

                    if (Expression == OutputExpression.FullWidth || Expression == OutputExpression.Right)
                        build.Append("\n\t\t\t\t\t\t</tr>\n");
                    #endregion Wrapper
                }

            }
            else
                build.Append(GetSimpleTextElement(this.Title, this.Mandatory, this.OutputText, this.InteractiveHelp));
            return ReadCandidate(m_Candidate.ID);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
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
