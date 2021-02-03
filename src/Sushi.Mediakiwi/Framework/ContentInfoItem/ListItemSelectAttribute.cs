using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Framework.ContentInfoItem
{
    /// <summary>
    /// Possible return types: System.String[], System.Int32[]
    /// </summary>
    public class ListItemSelectAttribute : ContentSharedAttribute, IContentInfo
    {
        /// <summary>
        /// Possible return types: System.String[], System.Int32[], string (CSV)
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="collectionPropertyName">Name of the collection property.</param>
        public ListItemSelectAttribute(string title, string collectionPropertyName)
            : this(title, collectionPropertyName, false) { }

        /// <summary>
        /// Possible return types: System.String[], System.Int32[], string (CSV)
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="collectionPropertyName">Name of the collection property.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        public ListItemSelectAttribute(string title, string collectionPropertyName, bool mandatory)
            : this(title, collectionPropertyName, mandatory, null) { }

        /// <summary>
        /// Possible return types: System.String[], System.Int32[], string (CSV)
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="collectionPropertyName">Name of the collection property.</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        public ListItemSelectAttribute(string title, string collectionPropertyName, bool mandatory, string interactiveHelp)
        {
            ContentTypeSelection = ContentType.ListItemSelect;
            Title = title;
            CollectionProperty = collectionPropertyName;
            Mandatory = mandatory;
            InteractiveHelp = interactiveHelp;
        }

        private string m_CollectionProperty;
        /// <summary>
        /// 
        /// </summary>
        public string CollectionProperty
        {
            set { m_CollectionProperty = value; }
            get { return m_CollectionProperty; }
        }

        private bool m_CanReuseItem;
        /// <summary>
        /// 
        /// </summary>
        public bool CanReuseItem
        {
            set { m_CanReuseItem = value; }
            get { return m_CanReuseItem; }
        }

        /// <summary>
        /// Sets the candidate.
        /// </summary>
        /// <param name="isEditMode"></param>
        public void SetCandidate(bool isEditMode)
        {
            SetCandidate(null, isEditMode);
        }

        Choice_DropdownAttribute _ReplacementAttribute;

        /// <summary>
        /// Sets the candidate.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="isEditMode">if set to <c>true</c> [is edit mode].</param>
        public void SetCandidate(Field field, bool isEditMode)
        {
            if (true)//Console.CurrentApplicationUser.ShowNewDesign2)
            {
                _ReplacementAttribute = new Choice_DropdownAttribute(this.Title, this.Collection)
                {
                };
                Data.Utility.ReflectProperty(this, _ReplacementAttribute);
                _ReplacementAttribute.Console = this.Console;
                _ReplacementAttribute.SetCandidate(field, isEditMode);
                return;
            }

            if (Property != null && Property.PropertyType == typeof(Data.CustomData))
                SetContentContainer(field);

            m_Candidate = null;

            if (!IsBluePrint)
            {
                if (Property.PropertyType == typeof(Data.CustomData))
                {
                    m_ListItemCollection = new Sushi.Mediakiwi.UI.ListItemCollection();
                    foreach (Sushi.Mediakiwi.Data.PropertyOption option in field.PropertyInfo.Options)
                    {
                        m_ListItemCollection.Add(new ListItem(option.Name, option.Value));
                    }
                    m_ListItemCollection = GetCollection(this.CollectionProperty, Property.Name, Console.CurrentListInstance, SenderSponsorInstance);
                }
                else
                    m_ListItemCollection = GetCollection(this.CollectionProperty, Property.Name, SenderSponsorInstance, SenderInstance);
            }

            if (IsInitialLoad || !isEditMode)
            {
                if (IsBluePrint)
                {
                    if (field != null)
                    {
                        if (field.Value != null)
                            m_Candidate = field.Value.Split(',');
                    }
                }
                else
                {
                    if (Property.PropertyType == typeof(Data.CustomData))
                    {
                        if (m_ContentContainer[field.Property].Value != null)
                            m_Candidate = m_ContentContainer[field.Property].Value.Split(',');
                    }
                    else
                    {
                        object value = Property.GetValue(SenderInstance, null);
                        if (value != null)
                        {
                            if (value.GetType() == typeof(string[]))
                                m_Candidate = value as string[];
                            else if (value.GetType() == typeof(int[]))
                            {
                                int[] tmp = value as int[];

                                m_Candidate = new string[tmp.Length];
                                for (int index = 0; index < tmp.Length; index++)
                                    m_Candidate[index] = tmp[index].ToString();
                            }
                            else if (value.GetType() == typeof(string))
                            {
                                m_Candidate = value.ToString().Split(',');
                            }
                            else
                                throw new Exception(string.Format("The type of the property '{0}' is not as expected: string[] or int[]", field.Property));
                        }
                    }
                }
            }
            else
            {
                string candidateList = Console.Form(this.ID);
                if (!string.IsNullOrEmpty(candidateList))
                {
                    string[] candidates = candidateList.Split('\n');
                    m_Candidate = new String[candidates.Length - 1];

                    for (int index = 0; index < candidates.Length -1; index++)
                    {
                        m_Candidate[index] = candidates[index].Split('|')[0];
                    }
                }
            }

            if (m_Candidate != null && m_Candidate.Length > 0)
            {
                m_CollectionSelected = new Sushi.Mediakiwi.UI.ListItemCollection();
                foreach (string item in m_Candidate)
                {
                    var li = m_ListItemCollection.FindByValue(item);
                    if (li != null)
                    {
                        m_CollectionSelected.Add(li);
                        m_ListItemCollection.Remove(li);
                    }
                }
            }

            // Possible return types: System.String[], System.Int32[], string (CSV)
            if (!IsBluePrint && Property != null && Property.CanWrite)
            {
                if (Property.PropertyType == typeof(Data.CustomData))
                    ApplyContentContainer(field, Data.Utility.ConvertToCsvString(m_Candidate));
                else
                {
                    if (m_Candidate == null)
                        Property.SetValue(SenderInstance, null, null);

                    else if (Property.PropertyType == typeof(int[]))
                    {
                        Property.SetValue(SenderInstance, Data.Utility.ConvertToIntArray(m_Candidate), null);
                    }
                    else if (Property.PropertyType == typeof(string[]))
                    {
                        Property.SetValue(SenderInstance, m_Candidate, null);
                    }
                    else
                    {
                        string output = Data.Utility.ConvertToCsvString(m_Candidate, false);
                        Property.SetValue(SenderInstance, output, null);
                    }
                }
            }


            //  Inherited content section
            if (ShowInheritedData)
            {
                if (field != null && !string.IsNullOrEmpty(field.InheritedValue))
                {
                    Sushi.Mediakiwi.UI.ListItemCollection selected = null;
                    foreach (string item in field.InheritedValue.Split(','))
                    {
                        var li = m_ListItemCollection.FindByValue(item);
                        if (li != null)
                        {
                            if (selected == null) selected = new Sushi.Mediakiwi.UI.ListItemCollection();
                            selected.Add(li);
                        }
                    }

                    if (selected != null && selected.Count > 0)
                    {
                        InhertitedOutputText = "<div class=\"optionInfo\">\n<ul>";
                        foreach (var li in selected)
                        {
                            InhertitedOutputText += string.Format("\n\t<li>{0}</li>", li.Text);
                        }
                        InhertitedOutputText += "\n</ul></div>";
                    }
                }
            }
        }

        string[] m_Candidate;

        Sushi.Mediakiwi.UI.ListItemCollection m_CollectionSelected;

        /// <summary>
        /// Writes the candidate.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <param name="isEditMode">if set to <c>true</c> [is edit mode].</param>
        /// <param name="isRequired">if set to <c>true</c> [is required].</param>
        /// <returns></returns>
        public Field WriteCandidate(WimControlBuilder build, bool isEditMode, bool isRequired, bool isCloaked)
        {
            if (true)//Console.CurrentApplicationUser.ShowNewDesign2)
                return _ReplacementAttribute.WriteCandidate(build, isEditMode, isRequired, isCloaked);

            this.SetWriteEnvironment();
            this.IsCloaked = isCloaked;
            this.Mandatory = isRequired;
            if (OverrideEditMode) isEditMode = false;
            if (isEditMode)
            {
                string options1 = "";
                if (m_ListItemCollection != null && m_ListItemCollection.Count > 0)
                {
                    foreach (var li in m_ListItemCollection)
                        options1 += string.Format("\n\t\t<option value=\"{0}\">{1}</option>", li.Value, li.Text);
                }
                string options2 = "";
                this.OutputText = "";
                if (m_CollectionSelected != null && m_CollectionSelected.Count > 0)
                {
                    foreach (var li in m_CollectionSelected)
                    {
                        if (string.IsNullOrEmpty(li.Text)) continue;

                        options2 += string.Format("\n\t\t<option value=\"{0}\">{1}</option>", li.Value, li.Text);
                        this.OutputText += string.Concat(li.Value, "|", li.Text, "\n");
                    }
                }

                string titleTag = string.Concat(Title, Mandatory ? "<em>*</em>" : "");
                build.Append(string.Format(@"{11}
<tr>
	<th><label for=""{1}A"">{0}:</label></th>{9}
	<td{10}>{8}
		<fieldset class=""listItemSelect"">
			<textarea class=""hidden"" cols=""64"" rows=""2"" id=""{1}"" name=""{1}"">{3}</textarea>
            <select id=""{1}a"" class=""editSelect to_{1}${4}"" size=""6"">{6}
			</select>
			<ul class=""move"">
				<li><button class=""toLeft srcMouseHover editSelect from_{1}a to_{1}$""><img alt=""To Right"" src=""{5}/images/icon_right_link.png""/></button></li>
				<li><button class=""toRight srcMouseHover editSelect from_{1}$ to_{1}a""><img alt=""To left"" src=""{5}/images/icon_left_link.png""/></button></li>
			</ul>
			<select id=""{1}$"" class=""editSelect to_{1}a{4}"" size=""6"">{7}
			</select>
			<ul class=""order"">
				<li><button class=""orderUp srcMouseHover editSelect up_{1}$""><img alt=""Up"" src=""{5}/images/icon_up_link.png""/></button></li>
				<li><button class=""orderDown srcMouseHover editSelect down_{1}$""><img alt=""Down"" src=""{5}/images/icon_down_link.png""/></button></li>
			</ul>
		</fieldset>{2}
	</td>
</tr>
"
                    , titleTag // 0
                    , this.ID // 1
                    , this.InteractiveHelp // 2
                    , this.OutputText // 3
                    , IsValid(isRequired) ? string.Empty : " error" // 4
                    , this.Console.WimRepository // 5
                    , options1 // 6
                    , options2 // 7
                    , CustomErrorText // 8
                    , ShowInheritedData
                        ? string.Format(@"<th class=""local""><label>{0}:</label></th>
</tr>
<tr>
    <td><div class=""description"">{1}</div></td>
"
                        , titleTag, InhertitedOutputText)
                        : null // 9
                    , (Console.HasDoubleCols) ? " colspan=\"3\" class=\"triple\"" : null // 10
                    , (Console.ExpressionPrevious == OutputExpression.Left) ? "<th><label>&nbsp;</label></th><td>&nbsp;</td></tr>" : null // 11
                    )
                );
            }
            else
            {
                string candidate = null;
                if (m_CollectionSelected != null && m_CollectionSelected.Count > 0)
                {
                    candidate = "<div class=\"optionInfo\">\n<ul>";
                    foreach (var li in m_CollectionSelected)
                    {
                        if (string.IsNullOrEmpty(li.Text)) continue;

                        candidate += string.Format("\n\t<li>{0}</li>", li.Text);
                    }
                    candidate += "\n</ul></div>";
                }

                build.Append(GetSimpleTextElement(this.Title, this.Mandatory, candidate, this.InteractiveHelp));
            }
            return ReadCandidate(Data.Utility.ConvertToCsvString(m_Candidate));
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

                    if (Mandatory && (m_Candidate == null || m_Candidate.Length == 0))
                        return false;
                }
                return true;
            
        }


    }
}
