using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.UI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Represents a DataList entity.
    /// </summary>
    public class DataList
    {
        internal List<Field> m_Fields;

        /// <summary>
        /// Adds the property value.
        /// </summary>
        /// <param name="filterProperty">The filter property.</param>
        /// <param name="type">The type.</param>
        /// <param name="value">The value.</param>
        public void AddPropertyValue(string filterProperty, ContentType type, object value)
        {
            if (m_Fields == null)
                m_Fields = new List<Field>();

            m_Fields.Add(new Field(filterProperty, type, value == null ? null : value.ToString()));
        }

        internal IComponentList ComponentListOverride { get; set; }

        /// <summary>
        /// Sets the component list override.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        public void SetComponentListOverride(Guid guid)
        {
            this.ComponentListOverride = ComponentList.SelectOne(guid);
        }

        /// <summary>
        /// Sets the component list override.
        /// </summary>
        /// <param name="list">The list.</param>
        public void SetComponentListOverride(IComponentList list)
        {
            this.ComponentListOverride = list;
        }
    }
}

namespace Sushi.Mediakiwi.Framework.ContentListItem
{
    /// <summary>
    /// Possible return types: Data.DataList (the value is the list GUID)
    /// </summary>
    public class DataListAttribute : ContentSharedAttribute, IContentInfo, IListContentInfo
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
                Hidden = IsCloaked,
            };
        }

        /// <summary>
        /// Possible return types: Data.DataList. If not list is applied it will take the current list. 
        /// </summary>
        public DataListAttribute()
        {
            ContentTypeSelection = ContentType.DataList;
        }

        /// <summary>
        /// Possible return types: Data.DataList (the value is the list GUID)
        /// </summary>
        public DataListAttribute(string componentList)
        {
            ContentTypeSelection = ContentType.DataList;
            this.Collection = componentList;
        }

        public bool BelowButtons { get; set; }
        public bool IsPartOfForm { get; set; }
        public bool HidePaging { get; set; }

        public DataListAttribute(Type T)
        {
            ContentTypeSelection = ContentType.DataList;
            this.Collection = ComponentList.SelectOne(T.ToString()).GUID.ToString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataListAttribute"/> class.
        /// </summary>
        /// <param name="componentListReferenceID">The component list reference ID.</param>
        public DataListAttribute(int componentListReferenceID)
        {
            ContentTypeSelection = ContentType.DataList;
            this.Collection = ComponentList.SelectOneByReference(componentListReferenceID).GUID.ToString();
        }

        public bool HasThumbnailOption { get; set; }

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
            object value = Property.GetValue(SenderInstance, null);
            if (value != null)
            {
                m_Candidate = value as DataList;
                
                if (m_Candidate.ComponentListOverride != null)
                {
                    m_List = m_Candidate.ComponentListOverride;
                    return;
                }
            }

            if (string.IsNullOrEmpty(this.Collection))
            {
                this.Collection = this.Console.CurrentList.GUID.ToString();
                m_List = this.Console.CurrentList;
            }

            int listInt;
            if (Utility.IsNumeric(this.Collection, out listInt))
                m_List = ComponentList.SelectOne(listInt);

            Guid listGuid;
            if (Utility.IsGuid(this.Collection, out listGuid))
                m_List = ComponentList.SelectOne(listGuid);

            if (m_List.IsNewInstance)
                throw new Exception(string.Format("Could not find the selected ComponentList[{0}]", Collection));
        }

        internal DataList m_Candidate;
        internal IComponentList m_List;

        /// <summary>
        /// Writes the candidate.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <param name="isEditMode"></param>
        /// <returns></returns>
        public Field WriteCandidate(WimControlBuilder build, bool isEditMode, bool isRequired, bool isCloaked)
        {
            Mandatory = isRequired;
            IsCloaked = isCloaked;

            Beta.GeneratedCms.Source.Component component = new Beta.GeneratedCms.Source.Component();

            Beta.GeneratedCms.Console tmp;

            //  If the requested list is simular to the current list there is no need for a different context. 
            //  With this setup the properties can be called internally.


            if (m_List.ID == Console.CurrentList.ID)
                tmp = Console;
            else
                tmp = Console.ReplicateInstance(m_List);
            

            if (m_Candidate != null && m_Candidate.m_Fields != null)
            {
                Content content = new Content();
                content.Fields = m_Candidate.m_Fields.ToArray();
                component.CreateSearchList(tmp, 0, content, true);
            }
            else
            {
                component.CreateSearchList(tmp, 0, null, true);
            }

            DataGrid grid = new DataGrid();

            if (!IsPartOfForm || BelowButtons)
            {
                build.Append("<bottombuttonbar />");
            }

            // Add datalist to API fields output 
            var apiField = Task.Run(async () => await GetApiFieldAsync().ConfigureAwait(false)).Result;
            build.ApiResponse.Fields.Add(apiField);


            if (HasThumbnailOption && !tmp.CurrentApplicationUser.ShowDetailView)
            {
                build.Append(grid.GetThumbnailGridFromListInstance(tmp.CurrentListInstance.wim, tmp, 0, false));
            }
            else
            {
                var table = grid.GetGridFromListInstance(tmp.CurrentListInstance.wim, tmp, 0, false, Console.CurrentListInstance, HidePaging);

                if (!IsPartOfForm)
                {
                    build.Append("</section>");
                    build.AppendFormat("<section id=\"datagrid\" class=\"searchTable{0}\">"
                        , tmp.CurrentListInstance.wim.CurrentList.Option_SearchAsync ? " async" : string.Empty
                        );
                }
                build.Append(table);
                if (build.SearchGrid == null)
                    build.SearchGrid = table;
                else
                    build.SearchGrid += table;
            }

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