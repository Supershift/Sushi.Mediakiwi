using Microsoft.AspNetCore.Http;
using Sushi.Mediakiwi.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Framework.ContentInfoItem
{
    internal class SublistParsing
    {
        internal static Sushi.Mediakiwi.Data.SubList Parse(
            HttpContext context, 
            System.Reflection.PropertyInfo property, 
            object senderInstance, 
            Field field,
            Data.CustomData contentContainer,
            string[] selectedValues,
            string selectedValue,
            string selectedKey,
            string id, 
            bool canContainOneItem)
        {
            Sushi.Mediakiwi.Data.SubList m_Candidate = new Data.SubList();
            if (canContainOneItem)
            {
                string candidate = null;
                string idToCheck = null;
                
                candidate = selectedValue;
                idToCheck = selectedKey;

                if (!string.IsNullOrEmpty(candidate) && candidate.Contains("|"))
                {
                    if (m_Candidate == null)
                        m_Candidate = new Sushi.Mediakiwi.Data.SubList(candidate.Split('|')[0], candidate.Split('|')[1]);
                    else
                        m_Candidate.Add(new Sushi.Mediakiwi.Data.SubList.SubListitem(candidate.Split('|')[0], candidate.Split('|')[1]));

                }
            }
            else
            {
                string candidate = context.Request.HasFormContentType ? context.Request.Form[id].ToString() : string.Empty;
                bool hasPostback = context.Request.HasFormContentType ? context.Request.Form.Count > 0 : false;
            
                if (property != null && context.Request.HasFormContentType && context.Request.Form != null && !hasPostback)
                {
                    //  20090324:MM Exception for postback change visibility
                    if (property.PropertyType == typeof(Data.CustomData))
                    {
                        string xx = contentContainer[field.Property].Value;
                        m_Candidate = contentContainer[field.Property].ParseSubList();
                    }
                    else if (property.PropertyType == typeof(Data.SubList))
                        m_Candidate = property.GetValue(senderInstance, null) as Data.SubList;

                }
                else
                {
                    if (m_Candidate == null)
                        m_Candidate = new Sushi.Mediakiwi.Data.SubList();

                    var values = selectedValues;
                    foreach (string value in values)
                    {
                        if (!string.IsNullOrEmpty(value))
                        {
                            string[] split = value.Split('|');
                            if (split.Length == 2)
                                m_Candidate.Add(new Sushi.Mediakiwi.Data.SubList.SubListitem(split[0], split[1]));

                        }
                    }
                }
            }
            return m_Candidate;
        }
    }

    /// <summary>
    /// Possible return types: Sushi.Mediakiwi.Data.SubList, Sushi.Mediakiwi.Data.iSubList
    /// </summary>
    public class SubListSelectAttribute : ContentEditableSharedAttribute, IContentInfo
    {
        /// <summary>
        /// Possible return types: Sushi.Mediakiwi.Data.SubList, Sushi.Mediakiwi.Data.iSubList
        /// </summary>
        /// <param name="title"></param>
        /// <param name="componentlistGuid">The indentifier GUID of the component list (this can also be a ClassName).</param>
        public SubListSelectAttribute(string title, string componentlistGuid)
            : this(title, componentlistGuid, false) { }

        /// <summary>
        /// Possible return types: Sushi.Mediakiwi.Data.SubList, Sushi.Mediakiwi.Data.iSubList
        /// </summary>
        /// <param name="title"></param>
        /// <param name="componentlistGuid">TThe indentifier GUID of the component list (this can also be a ClassName).</param>
        /// <param name="mandatory"></param>
        public SubListSelectAttribute(string title, string componentlistGuid, bool mandatory)
            : this(title, componentlistGuid, mandatory, null) { }

        /// <summary>
        /// Possible return types: Sushi.Mediakiwi.Data.SubList, Sushi.Mediakiwi.Data.iSubList
        /// </summary>
        /// <param name="title"></param>
        /// <param name="componentlistGuid">The indentifier GUID of the component list (this can also be a ClassName).</param>
        /// <param name="mandatory"></param>
        /// <param name="interactiveHelp"></param>
        public SubListSelectAttribute(string title, string componentlistGuid, bool mandatory, string interactiveHelp)
            : this(title, componentlistGuid, mandatory, false, interactiveHelp) { }

        /// <summary>
        /// Possible return types: Sushi.Mediakiwi.Data.SubList, Sushi.Mediakiwi.Data.iSubList
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="componentlistGuid">The indentifier GUID of the component list (this can also be a ClassName).</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        /// <param name="canOnlyOrderSort">if set to <c>true</c> [can only order sort].</param>
        public SubListSelectAttribute(string title, string componentlistGuid, bool mandatory, bool canOnlyOrderSort)
            : this(title, componentlistGuid, mandatory, canOnlyOrderSort, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubListSelectAttribute"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="componentlistGuid">The indentifier GUID of the component list (this can also be a ClassName).</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        /// <param name="canOnlyOrderSort">if set to <c>true</c> [can only order sort].</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        public SubListSelectAttribute(string title, string componentlistGuid, bool mandatory, bool canOnlyOrderSort, string interactiveHelp)
            : this(title, componentlistGuid, mandatory, canOnlyOrderSort, false, false, interactiveHelp) { }

        /// <summary>
        /// Possible return types: Sushi.Mediakiwi.Data.SubList, Sushi.Mediakiwi.Data.iSubList
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="componentlistGuid">The indentifier GUID of the component list (this can also be a ClassName).</param>
        /// <param name="mandatory">if set to <c>true</c> [mandatory].</param>
        /// <param name="canOnlyOrderSort">if set to <c>true</c> [can only order sort].</param>
        /// <param name="canContainOneItem">if set to <c>true</c> [can contain one item].</param>
        /// <param name="interactiveHelp">The interactive help.</param>
        public SubListSelectAttribute(string title, string componentlistGuid, bool mandatory, bool canOnlyOrderSort, bool canContainOneItem, bool canClickOnItem,  string interactiveHelp)
        {
            //if (canContainOneItem)
                m_CanHaveExpression = true;

            ContentTypeSelection = ContentType.SubListSelect;
            Title = title;
            Mandatory = mandatory;
            InteractiveHelp = interactiveHelp;

            //if (!Data.Utility.IsGuid(componentlistGuid))
            //    componentlistGuid = Sushi.Mediakiwi.Data.ComponentList.SelectOne(componentlistGuid).GUID.ToString();

            Componentlist = componentlistGuid;
            CanOnlyOrderSort = canOnlyOrderSort;
            CanContainOneItem = canContainOneItem;
            CanClickOnItem = canClickOnItem;
        }


        /// <summary>
        /// Gets or sets a value indicating whether AutoPostback is true (only applicable for singleItem sublists
        /// </summary>
        /// <value><c>true</c> if [auto postback]; otherwise, <c>false</c>.</value>
        public bool AutoPostback
        {
            set { m_AutoPostBack = value; }
            get { return m_AutoPostBack; }
        }

        private string m_Componentlist;
        /// <summary>
        /// Gets or sets the componentlist.
        /// </summary>
        /// <value>The componentlist.</value>
        public string Componentlist
        {
            set { m_Componentlist = value; }
            get { return m_Componentlist; }
        }

        private bool m_CanOnlyOrderSort;
        /// <summary>
        /// Gets or sets the can only order sort.
        /// </summary>
        /// <value>The can only order sort.</value>
        public bool CanOnlyOrderSort
        {
            set { m_CanOnlyOrderSort = value; }
            get { return m_CanOnlyOrderSort; }
        }

        private bool m_CanContainOneItem;
        /// <summary>
        /// Gets or sets a value indicating whether this instance can contain one item.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can contain one item; otherwise, <c>false</c>.
        /// </value>
        public bool CanContainOneItem
        {
            set { m_CanContainOneItem = value; }
            get { return m_CanContainOneItem; }
        }

        public bool CanClickOnItem { get; set; }

        private bool m_AddNewItemsOnTop;
        /// <summary>
        /// When <c>True</c>, newly selected items will go on top, instead of the bottom.
        /// </summary>
        public bool AddNewItemsOnTop
        {
            get { return m_AddNewItemsOnTop; }
            set { m_AddNewItemsOnTop = value; }
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
            //  Set because of new post _0 detection
            //this.IsMultiFile = this.m_IsNewDesign;

            if (Property != null && Property.PropertyType == typeof(Data.CustomData))
            {
                SetContentContainer(field);

                if (field.PropertyInfo.ListSelect.HasValue)
                    Componentlist = field.PropertyInfo.ListSelect.GetValueOrDefault().ToString();
            }

            m_Candidate = null;
            if (IsInitialLoad || !isEditMode)
            {
                if (IsBluePrint)
                {
                    if (field != null)
                    {
                        if (!string.IsNullOrEmpty(field.Value))
                            m_Candidate = Data.SubList.GetDeserialized(field.Value);
                    }
                }
                else
                {
                    if (Property.PropertyType == typeof(Data.CustomData))
                    {
                        string xx = m_ContentContainer[field.Property].Value;
                        m_Candidate = m_ContentContainer[field.Property].ParseSubList();
                    }
                    else if (Property.PropertyType == typeof(Data.SubList))
                        m_Candidate = Property.GetValue(SenderInstance, null) as Data.SubList;
                    else if (Property.PropertyType.GetInterface(typeof(Data.ISubList).FullName) != null)
                    {
                        m_interfaceCandidate = Property.GetValue(SenderInstance, null) as Data.ISubList;
                        m_Candidate = new Data.SubList();
                        if (m_interfaceCandidate != null)
                            m_Candidate.Add(m_interfaceCandidate.GetListItemValue());
                    }
                }
            }
            else
            {
                //  [MM:14.12.14] Moved to central location
                m_Candidate = SublistParsing.Parse(
                    this.Context, 
                    this.Property, 
                    this.SenderInstance, 
                    field, 
                    m_ContentContainer, 
                    this.SelectedValues, 
                    this.SelectedValue, 
                    this.SelectedKey, 
                    this.ID, 
                    this.CanContainOneItem
                    );
            }

            if (!IsBluePrint && Property != null && Property.CanWrite)
            {
                if (Property.PropertyType == typeof(Data.CustomData))
                {
                    ApplyContentContainer(field, m_Candidate == null ? null : m_Candidate.Serialized);
                }
                else if (Property.PropertyType == typeof(Data.SubList))
                    Property.SetValue(SenderInstance, m_Candidate, null);

                else if (Property.PropertyType.GetInterface(typeof(Data.ISubList).FullName) != null)
                {
                    m_interfaceCandidate = Property.GetValue(SenderInstance, null) as Data.ISubList;
                    if (m_interfaceCandidate == null)
                        m_interfaceCandidate = System.Activator.CreateInstance(Property.PropertyType) as Data.ISubList;

                    m_interfaceCandidate = m_interfaceCandidate.SetListItemValue(m_Candidate.Items);
                    Property.SetValue(SenderInstance, m_interfaceCandidate, null);
                }
            }

            OutputText = null;
            if (m_Candidate != null && m_Candidate.Items != null && m_Candidate.Items.Length > 0)
            {
                if (this.CanContainOneItem)
                    OutputText = string.Concat(m_Candidate.Items[0].TextID, "|", m_Candidate.Items[0].Description);
                else
                {
                    OutputText = "";
                    foreach (Data.SubList.SubListitem item in m_Candidate.Items)
                    {
                        OutputText += string.Concat(item.TextID, "|", item.Description, "\n");
                    }
                }
            }
        }

        Data.ISubList m_interfaceCandidate;
        Data.SubList m_Candidate;


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
            if (OverrideEditMode) 
                isEditMode = false;

            if (isEditMode)
            {
                // Edit mode
                Data.IComponentList list = Data.ComponentList.SelectOne(Data.Utility.ConvertToGuid(this.Componentlist));
                if (this.CanContainOneItem)
                {
                    #region Half Width
                    if (Expression != OutputExpression.FullWidth)
                    {
                        string titleTag = string.Concat(Title, Mandatory ? "<em>*</em>" : "");
                        string candidate = null;
                        int? key = null;
                        if (m_Candidate != null && m_Candidate.Items != null && m_Candidate.Items.Length > 0)
                        {
                            key = m_Candidate.Items[0].ID;
                            candidate = m_Candidate.Items[0].Description;
                        }

                        ApplyItemSelect(
                            build,
                            this.CanContainOneItem, this.CanClickOnItem,
                            titleTag,
                            this.ID,
                            this.Componentlist,
                            m_Candidate == null ? null : m_Candidate.UrlAddition,
                            this.AutoPostback,
                            isRequired,
                            false,
                            AddNewItemsOnTop,
                            LayerSize.Normal,
                            null,
                            null,
                            new NameItemValue() { Name = this.ID, ID = key, Value = candidate }
                            );
                    }
                    #endregion 
                    else
                    {
                        #region Full Width
                        string titleTag = string.Concat(Title, Mandatory ? "<em>*</em>" : "");

                        string candidate = null;
                        string key = null;
                        if (m_Candidate != null && m_Candidate.Items != null && m_Candidate.Items.Length > 0)
                        {
                            key = m_Candidate.Items[0].TextID;
                            candidate = m_Candidate.Items[0].Description;
                        }

                        ApplyItemSelect(
                            build,
                            this.CanContainOneItem, this.CanClickOnItem,
                            titleTag,
                            this.ID,
                            this.Componentlist,
                            m_Candidate == null ? null : m_Candidate.UrlAddition,
                            this.AutoPostback, 
                            isRequired, 
                            false, 
                            AddNewItemsOnTop, 
                            LayerSize.Normal, 
                            null, 
                            null,
                            new NameItemValue() { Name = this.ID, TextID = key, Value = candidate }
                            );
                        #endregion
                    }
                }
                else
                {
                    List<NameItemValue> arr = new List<NameItemValue>();
                    if (m_Candidate != null && m_Candidate.Items != null && m_Candidate.Items.Length > 0)
                    {
                        foreach (Data.SubList.SubListitem item in m_Candidate.Items)
                        {
                            
                            arr.Add(new NameItemValue() { Name = this.ID, Value = item.Description, ID = item.ID, TextID = item.TextID } );
                        }
                    }

                    string titleTag = string.Concat(Title, Mandatory ? "<em>*</em>" : "");

                    ApplyItemSelect(
                        build,
                        this.CanContainOneItem, this.CanClickOnItem,
                        titleTag,
                        this.ID,
                        this.Componentlist,
                        m_Candidate == null ? null : m_Candidate.UrlAddition,
                        this.AutoPostback,
                        isRequired,
                        this.CanOnlyOrderSort,
                        AddNewItemsOnTop,
                        LayerSize.Normal,
                        null,
                        null,
                        arr.ToArray()
                        );
                }
            }
            else
            {
                //  Read mode
                string candidate = null;
                if (m_Candidate != null && m_Candidate.Items != null && m_Candidate.Items.Length > 0)
                {
                    if (m_Candidate.Items.Length == 1)
                    {
                        candidate = m_Candidate.Items[0].Description;
                    }
                    else
                    {
                        candidate = "<div class=\"optionInfo\">\n<ul>";
                        foreach (var item in m_Candidate.Items)
                        {
                            candidate += string.Format("\n\t<li>{0}</li>", item.Description);
                        }
                        candidate += "\n</ul></div>";
                    }
                }
               
                build.Append(GetSimpleTextElement(this.Title, this.Mandatory, candidate, this.InteractiveHelp));
            }

            if (m_Candidate == null || m_Candidate.Items == null || m_Candidate.Items.Length == 0)
                return ReadCandidate(null);
            return ReadCandidate(m_Candidate.Serialized);
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

                if (Mandatory && (m_Candidate == null || m_Candidate.Items == null || m_Candidate.Items.Length == 0))
                    return false;
            }
            return true;
        }
    }
}
