using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.UI;
using System;

using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Sushi.Mediakiwi.Framework.ContentInfoItem
{
    /// <summary>
    /// Possible return types: System.Int32, Sushi.Mediakiwi.Data.Folder
    /// </summary>
    public class FolderSelectAttribute : ContentSharedAttribute, IContentInfo
    {
        /// <summary>
        /// Possible return types: System.Int32, System.Int32[nullable], Sushi.Mediakiwi.Data.Folder
        /// </summary>
        /// <param name="title"></param>
        public FolderSelectAttribute(string title)
            : this(title, false) { }

        /// <summary>
        /// Possible return types: System.Int32, System.Int32[nullable], Sushi.Mediakiwi.Data.Folder
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mandatory"></param>
        public FolderSelectAttribute(string title, bool mandatory)
            : this(title, mandatory, null) { }

        /// <summary>
        /// Possible return types: System.Int32, System.Int32[nullable], Sushi.Mediakiwi.Data.Folder
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        public FolderSelectAttribute(string title, bool mandatory, string interactiveHelp)
            : this(title, mandatory, Sushi.Mediakiwi.Data.FolderType.Page, interactiveHelp) { }

        /// <summary>
        /// Possible return types: System.Int32, System.Int32[nullable], Sushi.Mediakiwi.Data.Folder
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        /// <param name="type">The type.</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        public FolderSelectAttribute(string title, bool mandatory, Sushi.Mediakiwi.Data.FolderType type, string interactiveHelp)
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
            if (Property != null && Property.PropertyType == typeof(Data.CustomData))
                SetContentContainer(field);

            m_Candidate = new Sushi.Mediakiwi.Data.Folder();
            if (IsInitialLoad || !isEditMode)
            {
                if (IsBluePrint)
                {
                    if (field != null)
                    {
                        int candidate = Data.Utility.ConvertToInt(field.Value);
                        if (candidate > 0)
                            m_Candidate = Data.Folder.SelectOne(candidate); 
                    }
                }
                else
                {
                    if (Property.PropertyType == typeof(Data.CustomData))
                    {
                        int candidate = Data.Utility.ConvertToInt(m_ContentContainer[field.Property].Value);
                        if (candidate > 0)
                            m_Candidate = Data.Folder.SelectOne(candidate);
                    }
                    else if (Property.PropertyType == typeof(Data.Folder))
                        m_Candidate = Property.GetValue(SenderInstance, null) as Data.Folder;
                    else if (Property.PropertyType == typeof(int))
                    {
                        int candidate = Data.Utility.ConvertToInt(Property.GetValue(SenderInstance, null));
                        if (candidate > 0)
                            m_Candidate = Data.Folder.SelectOne(candidate);
                    }
                    else if (Property.PropertyType == typeof(int?))
                    {
                        object candidate = Data.Utility.ConvertToInt(Property.GetValue(SenderInstance, null));
                        if (candidate != null)
                            m_Candidate = Data.Folder.SelectOne(Data.Utility.ConvertToInt(candidate));
                    }

                }
            }
            else
            {
                int candidate = Data.Utility.ConvertToInt(Console.Form(this.ID));
                if (candidate > 0)
                    m_Candidate = Data.Folder.SelectOne(candidate);
            }

            //  Possible return types: System.Int32, Sushi.Mediakiwi.Data.Folder

            if (!IsBluePrint && Property != null && Property.CanWrite)
            {
                if (Property.PropertyType == typeof(Data.CustomData))
                    ApplyContentContainer(field, m_Candidate.ID.ToString());
                else if (Property.PropertyType == typeof(int))
                    Property.SetValue(SenderInstance, m_Candidate.ID, null);
                else if (Property.PropertyType == typeof(int?))
                {
                    if (m_Candidate != null && !m_Candidate.IsNewInstance)
                        Property.SetValue(SenderInstance, m_Candidate.ID, null);
                    else
                        Property.SetValue(SenderInstance, null, null);
                }
                else
                    Property.SetValue(SenderInstance, m_Candidate, null);
            }

            if (m_Candidate != null)
                OutputText = m_Candidate.CompletePath;

            //  Inherited content section
            if (ShowInheritedData)
            {
                if (field != null && !string.IsNullOrEmpty(field.InheritedValue))
                {
                    int candidate2 = Data.Utility.ConvertToInt(field.InheritedValue);
                    if (candidate2 > 0)
                    {
                        Data.Folder folder = Data.Folder.SelectOne(candidate2);
                        InhertitedOutputText = folder.CompletePath;
                    }
                }
            }
        }

        Data.FolderType m_FolderType;
        Sushi.Mediakiwi.Data.Folder m_Candidate;

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

            if (OverrideEditMode) isEditMode = false;

            if (m_Candidate != null)
                OutputText = m_Candidate.ID.ToString();
            m_ListItemCollection = new ListItemCollection();
            if (m_FolderType == Sushi.Mediakiwi.Data.FolderType.Undefined)
                m_FolderType = Console.CurrentListInstance.wim.CurrentFolder.Type;

            m_ListItemCollection.Add(new ListItem(""));


            Data.Folder[] folders = Data.Folder.SelectAll(m_FolderType, this.Console.CurrentListInstance.wim.CurrentSite.ID);
            folders = Data.Folder.ValidateAccessRight(folders, this.Console.CurrentApplicationUser);
            foreach (Data.Folder item in folders)
            {
                m_ListItemCollection.Add(new ListItem(item.CompletePath, item.ID.ToString()));
            }

            //  If this folder is not in the default list (list for Administration section, clear the list and only add the selected value
            if (Console.CurrentList.Type == Sushi.Mediakiwi.Data.ComponentListType.ComponentLists)
            {
                int listID = Data.Utility.ConvertToInt(Console.Request.Query["item"]);
                Sushi.Mediakiwi.Data.IComponentList clist = Sushi.Mediakiwi.Data.ComponentList.SelectOne(listID);

                //if (clist.Type != Sushi.Mediakiwi.Data.ComponentListType.Undefined)
                //{
                //    Sushi.Mediakiwi.Data.Folder folder = Sushi.Mediakiwi.Data.Folder.SelectOne(clist.FolderID.GetValueOrDefault());
                //    m_ListItemCollection.Clear();
                //    m_ListItemCollection.Add(new ListItem(folder.CompletePath, folder.ID.ToString()));
                //}
            }

            if (isEditMode)
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
                        optiondata.AppendFormat("\n\t\t\t\t\t\t\t\t\t\t<option value=\"{0}\"{2}>{1}</option>", li.Value, WebUtility.HtmlEncode(li.Text), OutputText == li.Value ? " selected=\"selected\"" : string.Empty);
                    }
                }
                options = optiondata.ToString();

                StringBuilder element = new StringBuilder();

                element.AppendFormat("\n\t\t\t\t\t\t\t\t\t<select id=\"{0}\"{1} name=\"{0}\">{2}"
                            , this.ID
                            , this.InputClassName(IsValid(isRequired))
                            //, isEnabled ? null : " disabled=\"disabled\""
                            , options
                            );

                element.Append("\n\t\t\t\t\t\t\t\t\t</select>");
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
                        className = string.Format(" class=\"{0}error\""
                            , Expression == OutputExpression.FullWidth ? null : "short "
                            );
                    else if (Expression != OutputExpression.FullWidth)
                        className = " class=\"short\"";


                   
                    string titleTag = string.Concat(Title, Mandatory ? "<em>*</em>" : "");

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
            {
                string candidate = null;
                if (m_ListItemCollection != null)
                {
                    foreach (var li in m_ListItemCollection)
                        if (li.Value == OutputText)
                        {
                            candidate = li.Text;
                            break;
                        }
                }
                build.Append(GetSimpleTextElement(this.Title, this.Mandatory, candidate, this.InteractiveHelp));
            }
            return ReadCandidate(OutputText);
        }

        /// <summary>
        /// Writes the candidate.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <param name="isEditMode">if set to <c>true</c> [is edit mode].</param>
        /// <param name="isRequired">if set to <c>true</c> [is required].</param>
        /// <returns></returns>
        public Field WriteCandidate2(WimControlBuilder build, bool isEditMode, bool isRequired)
        {
            this.SetWriteEnvironment();

            this.Mandatory = isRequired;
            if (isEditMode)
            {
                #region Folder building
                StringBuilder listbuild = new StringBuilder();
                int currentLevel = 0;
                Data.Folder previousItem = null;

                if (m_FolderType == Sushi.Mediakiwi.Data.FolderType.Undefined)
                    m_FolderType = Console.CurrentListInstance.wim.CurrentFolder.Type;

                foreach (Data.Folder item in Data.Folder.SelectAll(m_FolderType, this.Console.CurrentListInstance.wim.CurrentSite.ID))
                {
                    if (previousItem == null) previousItem = item;
                    //if (item.Level == 0) continue;

                    if (listbuild.Length != 0)
                    {
                        if (item.Level > currentLevel)
                        {
                            if (m_Candidate != null && !m_Candidate.IsNewInstance && !string.IsNullOrEmpty(previousItem.CompletePath) && m_Candidate.CompletePath.StartsWith(previousItem.CompletePath))
                                listbuild.Append("\n<ul class=\"active opened\">");
                            else
                                listbuild.Append("\n<ul>");
                        }
                        else if (item.Level < currentLevel)
                        {
                            int scaleDown = currentLevel - item.Level;
                            while (scaleDown > 0)
                            {
                                listbuild.Append("\n</ul>");
                                scaleDown--;
                            }
                            listbuild.Append("\n</li>");
                        }
                        else
                            listbuild.Append("\n</li>");

                        currentLevel = item.Level;
                    }

                    if (m_Candidate == null)
                        listbuild.AppendFormat(@"<li><a href=""#"" id=""{0}"" class=""directory link{2}{3}{4}"">{1}</a>", item.ID, item.Name, string.Empty, IsValid(isRequired) ? string.Empty : " error", string.Empty);
                    else
                    {
                        listbuild.AppendFormat(@"<li><a href=""#"" id=""{0}"" class=""directory link{2}{3}{4}"">{1}</a>", item.ID, item.Name, item.ID == m_Candidate.ID ? " active" : string.Empty, IsValid(isRequired) ? string.Empty : " error",
                            string.IsNullOrEmpty(m_Candidate.CompletePath) ? string.Empty :
                            (m_Candidate.CompletePath.StartsWith(item.CompletePath) ? " opened" : string.Empty)
                            );
                    }
                    //listbuild.AppendFormat("L:{0}-C:{1}", item.Level, currentLevel);
                    previousItem = item;

                }

                while (currentLevel > 0)
                {
                    listbuild.Append("\n</li></ul>");
                    currentLevel--;
                }

                listbuild.Append("\n</li>");
                string list = listbuild.ToString();
                #endregion

                string titleTag = string.Concat(Title, Mandatory ? "<em>*</em>" : "");
                build.Append(string.Format(@"{10}
<tr>
	<th><label>{0}:</label></th>{8}
	<td{9}>{7}
		<fieldset class=""binaryList binaryDocument"">
			<input type=""hidden"" name=""{1}"" value=""{5}""/>
			<ul class=""list files{6} opened"">{4}
			</ul>
			<ul class=""buttons selection"">
				<li><button class=""srcMouseHover remove""><img alt="""" src=""{3}/images/icon_remove_link.png""/> <span>Remove</span></button></li>
			</ul>
		</fieldset>{2}
	</td>
</tr>
"
                    , titleTag // 0
                    , this.ID // 1
                    , this.InteractiveHelp // 2
                    , this.Console.WimRepository // 3
                    , list // 4
                    , m_Candidate.ID // 5
                    , IsValid(isRequired) ? string.Empty : " error" // 6
                    , CustomErrorText // 7
                    , ShowInheritedData
                        ? string.Format(@"<th class=""local""><label>{0}:</label></th>
</tr>
<tr>
    <td><div class=""description"">{1}</div></td>
"
                        , titleTag, InhertitedOutputText)
                        : null // 8
                    , (Console.HasDoubleCols) ? " colspan=\"3\" class=\"triple\"" : null // 9
                    , (Console.ExpressionPrevious == OutputExpression.Left) ? "<th><label>&nbsp;</label></th><td>&nbsp;</td></tr>" : null // 10
                    )
                );
            }
            else
                build.Append(GetSimpleTextElement(this.Title, this.Mandatory, this.OutputText, this.InteractiveHelp));
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
                        return !m_Candidate.IsNewInstance;
                }
                return true;
            
        }


    }
}
