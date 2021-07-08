using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.UI;
using System;
using System.Net;
using System.Text;

namespace Sushi.Mediakiwi.Framework.ContentInfoItem
{
    /// <summary>
    /// Possible return types: System.Int32, Folder
    /// </summary>
    public class FolderSelectAttribute : ContentSharedAttribute, IContentInfo
    {
        /// <summary>
        /// Possible return types: System.Int32, System.Int32[nullable], Folder
        /// </summary>
        /// <param name="title"></param>
        public FolderSelectAttribute(string title)
            : this(title, false) { }

        /// <summary>
        /// Possible return types: System.Int32, System.Int32[nullable], Folder
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        public FolderSelectAttribute(string title, bool mandatory)
            : this(title, mandatory, null) { }

        /// <summary>
        /// Possible return types: System.Int32, System.Int32[nullable], Folder
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        public FolderSelectAttribute(string title, bool mandatory, string interactiveHelp)
            : this(title, mandatory, FolderType.Page, interactiveHelp) { }

        /// <summary>
        /// Possible return types: System.Int32, System.Int32[nullable], Folder
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        /// <param name="type">The type.</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        public FolderSelectAttribute(string title, bool mandatory, FolderType type, string interactiveHelp)
        {
            m_CanHaveExpression = true;
            ContentTypeSelection = ContentType.FolderSelect;
            Title = title;
            Mandatory = mandatory;
            InteractiveHelp = interactiveHelp;
            m_FolderType = type;
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
                SetContentContainer(field);

            m_Candidate = new Folder();
            if (IsInitialLoad || !isEditMode)
            {
                if (IsBluePrint)
                {
                    if (field != null)
                    {
                        int candidate = Utility.ConvertToInt(field.Value);
                        if (candidate > 0)
                        {
                            m_Candidate = Folder.SelectOne(candidate);
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
                            m_Candidate = Folder.SelectOne(candidate);
                        }
                    }
                    else if (Property.PropertyType == typeof(Folder))
                    {
                        m_Candidate = Property.GetValue(SenderInstance, null) as Folder;
                    }
                    else if (Property.PropertyType == typeof(int))
                    {
                        int candidate = Utility.ConvertToInt(Property.GetValue(SenderInstance, null));
                        if (candidate > 0)
                        {
                            m_Candidate = Folder.SelectOne(candidate);
                        }
                    }
                    else if (Property.PropertyType == typeof(int?))
                    {
                        object candidate = Utility.ConvertToInt(Property.GetValue(SenderInstance, null));
                        if (candidate != null)
                        {
                            m_Candidate = Folder.SelectOne(Utility.ConvertToInt(candidate));
                        }
                    }

                }
            }
            else
            {
                int candidate = Utility.ConvertToInt(Console.Form(ID));
                if (candidate > 0)
                {
                    m_Candidate = Folder.SelectOne(candidate);
                }
            }

            //  Possible return types: System.Int32, Folder

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
                    if (m_Candidate != null && !m_Candidate.IsNewInstance)
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

            if (m_Candidate != null)
            {
                OutputText = m_Candidate.CompletePath;
            }

            //  Inherited content section
            if (ShowInheritedData && field != null && !string.IsNullOrEmpty(field.InheritedValue))
            {
                int candidate2 = Utility.ConvertToInt(field.InheritedValue);
                if (candidate2 > 0)
                {
                    Folder folder = Folder.SelectOne(candidate2);
                    InhertitedOutputText = folder.CompletePath;
                }
            }
        }

        FolderType m_FolderType;
        Folder m_Candidate;

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

            if (OverrideEditMode)
            {
                isEditMode = false;
            }

            if (m_Candidate != null)
            {
                OutputText = m_Candidate.ID.ToString();
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

            m_ListItemCollection = new ListItemCollection();
            if (m_FolderType == FolderType.Undefined)
            {
                m_FolderType = Console.CurrentListInstance.wim.CurrentFolder.Type;
            }

            m_ListItemCollection.Add(new ListItem(""));


            if (m_FolderType == FolderType.Gallery)
            {
                var galleries = Gallery.SelectAllAccessible(Console.CurrentApplicationUser);
                foreach (var item in galleries)
                {
                    m_ListItemCollection.Add(new ListItem(item.CompletePath, item.ID.ToString()));
                }
            }
            else
            {
                Folder[] folders = Folder.SelectAll(m_FolderType, Console.CurrentListInstance.wim.CurrentSite.ID);
                folders = Folder.ValidateAccessRight(folders, Console.CurrentApplicationUser);
                foreach (Folder item in folders)
                {
                    m_ListItemCollection.Add(new ListItem(item.CompletePath, item.ID.ToString()));
                }
            }

            //  If this folder is not in the default list (list for Administration section, clear the list and only add the selected value
            if (Console.CurrentList.Type == ComponentListType.ComponentLists)
            {
                int listID = Utility.ConvertToInt(Console.Request.Query["item"]);
                IComponentList clist = ComponentList.SelectOne(listID);

                //if (clist.Type != ComponentListType.Undefined)
                //{
                //    Folder folder = Folder.SelectOne(clist.FolderID.GetValueOrDefault());
                //    m_ListItemCollection.Clear();
                //    m_ListItemCollection.Add(new ListItem(folder.CompletePath, folder.ID.ToString()));
                //}
            }

            if (isEditMode && isEnabled)
            {
                #region Element creation

                int count = 0;

                string options = null;
                StringBuilder optiondata = new StringBuilder();

                if (m_ListItemCollection != null)
                {
                    foreach (var li in m_ListItemCollection)
                    {
                        count++;
                        optiondata.AppendFormat("<option value=\"{0}\"{2}>{1}</option>", li.Value, WebUtility.HtmlEncode(li.Text), OutputText == li.Value ? " selected=\"selected\"" : string.Empty);
                    }
                }
                options = optiondata.ToString();

                StringBuilder element = new StringBuilder();

                element.AppendFormat("<select id=\"{0}\"{1} name=\"{0}\">{2}"
                            , ID
                            , InputClassName(IsValid(isRequired))
                            //, isEnabled ? null : " disabled=\"disabled\""
                            , options
                            );

                element.Append("</select>");

                #endregion Element creation

                if (IsCloaked)
                {
                    build.AppendCloaked(element.ToString());
                }
                else
                {
                    #region Wrapper
                    string className = null;
                    //if (AutoPostBack)
                    //    className = string.Format(" class=\"postBack {0}{1}\"", IsValid ? string.Empty : " error"
                    //        , Expression == OutputExpression.FullWidth ? null : " short"
                    //        );
                    //else 
                    if (!IsValid(isRequired))
                    {
                        className = string.Format(" class=\"{0}error\""
                            , Expression == OutputExpression.FullWidth ? null : "short "
                            );
                    }
                    else if (Expression != OutputExpression.FullWidth)
                    {
                        className = " class=\"short\"";
                    }

                    string titleTag = string.Concat(Title, Mandatory ? "<em>*</em>" : "");

                    if (ShowInheritedData)
                    {
                        ApplyTranslation(build);
                    }
                    else
                    {
                        if ((Console.ExpressionPrevious == OutputExpression.Left && Expression == OutputExpression.FullWidth) || (Console.ExpressionPrevious == OutputExpression.Left && Expression == OutputExpression.Left))
                        {
                            build.Append("<th><label>&nbsp;</label></th><td>&nbsp;</td></tr>");
                        }

                        if ((Console.ExpressionPrevious == OutputExpression.FullWidth && Expression == OutputExpression.Right) || (Console.ExpressionPrevious == OutputExpression.Right && Expression == OutputExpression.Right))
                        {
                            build.Append("<tr><th><label>&nbsp;</label></th><td>&nbsp;</td>");
                        }

                        if (Expression == OutputExpression.FullWidth || Expression == OutputExpression.Left)
                        {
                            build.Append("<tr>");
                        }
                    }

                    build.AppendFormat("<th><label for=\"{0}\">{1}</label></th>", ID, TitleLabel);

                    //if (ShowInheritedData)
                    //    build.AppendFormat("\t\t\t\t\t\t\t<th class=\"local\"><label>{0}:</label></th>\t\t\t\t\t\t</tr>\t\t\t\t\t\t<tr>\t\t\t\t\t\t\t<td><div class=\"description\">{1}</div></td>\n", ID, TitleLabel);

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
                string candidate = null;

                if (sharedInfoApply.isShared == true)
                {
                    candidate = OutputText;
                }
                else
                {
                    if (m_ListItemCollection != null)
                    {
                        foreach (var li in m_ListItemCollection)
                        {
                            if (li.Value == OutputText)
                            {
                                candidate = li.Text;
                                break;
                            }
                        }
                    }
                }

                build.Append(GetSimpleTextElement(candidate));
            }
            return ReadCandidate(OutputText);
        }


        /// <summary>
        /// 
        /// </summary>
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

                if (Mandatory)
                {
                    return !m_Candidate.IsNewInstance;
                }
            }
            return true;
        }
    }
}
