using Sushi.Mediakiwi.Beta.GeneratedCms.Source;
using System;
using System.Collections;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Framework
{
    public class ComponentListDataBindingEventArgs : ComponentListSearchEventArgs
    {
        public ComponentListDataBindingEventArgs(int selectedItemKey, int selectedGroupKey, int selectedGroupItemKey)
            : base(selectedItemKey, selectedGroupKey, selectedGroupItemKey) { }
    }

    public class ComponentListSetupEventArgs : ComponentListSearchEventArgs
    {
        public ComponentListSetupEventArgs(int selectedItemKey, int selectedGroupKey, int selectedGroupItemKey)
            : base(selectedItemKey, selectedGroupKey, selectedGroupItemKey) { }
    }

    public enum DataItemType
    {
        TableRow = 0,
        TableCell = 1,
        Custom = 2
    }


    /// <summary>
    /// 
    /// </summary>
    public class ListDataSoure 
    {
        /// <summary>
        /// Gets or sets the data entities.
        /// </summary>
        /// <value>
        /// The data entities.
        /// </value>
        public IEnumerator DataEntities { get; set; }
        /// <summary>
        /// Gets or sets the visible columns.
        /// </summary>
        /// <value>
        /// The visible columns.
        /// </value>
        public int VisibleColumns { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ListDataItemCreatedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the source.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        public ListDataSoure Source { get; internal set; }
        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public DataItemType Type { get; internal set; }
        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <value>
        /// The item.
        /// </value>
        public object Item { get; internal set; }

        public void SetRowTitle(string title)
        {
            SetRowTitle(title, false);
        }

        /// <summary>
        /// Sets the row title.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="hasBackgroundHighlight">if set to <c>true</c> [has background highlight].</param>
        public void SetRowTitle(string title, bool hasBackgroundHighlight)
        {
            int fixedCount = 0;
            int fixedWidth = 0;
            foreach (var item in this.Columns)
            {
                if (item.ColumnIsFixed)
                {
                    fixedWidth += item.ColumnWidth;
                    fixedCount++;
                }
            }

            if (fixedCount > 0)
                this.InnerHTML = string.Format("<tr class=\"nosort{4}\"><td colspan=\"{1}\" class=\"fixed\" width=\"{2}\"><h4>{3}</h4></td><td colspan=\"{0}\"{4}>&nbsp;</td></tr>"
                    , this.Source.VisibleColumns - fixedCount
                    , fixedCount
                    , fixedWidth
                    , title
                    , hasBackgroundHighlight ? " highlight" : null
                    );
            else
                this.InnerHTML = string.Format("<tr class=\"nosort{2}\"><td colspan=\"{0}\"><h4>{1}</h4></td></tr>"
                    , this.Source.VisibleColumns
                    , title
                    , hasBackgroundHighlight ? " highlight" : null);
        }

        /// <summary>
        /// Gets the item key.
        /// </summary>
        /// <value>
        /// The item key.
        /// </value>
        public int? ItemKey { get; internal set; }
        /// <summary>
        /// Gets or sets the item value.
        /// </summary>
        /// <value>
        /// The item value.
        /// </value>
        //public Object ItemValue { get; set; }
        /// <summary>
        /// Sets the inner HTML for the itemType (for custom whole row, for TableRow whole row, for tableCell only the content)
        /// </summary>
        /// <value>
        /// The inner HTML.
        /// </value>
        public object InnerHTML { get; set; }
        /// <summary>
        /// Gets or sets the node attributes required for the opening of the instance item. Contains also Layer options. Use for custom presentation records
        /// </summary>
        /// <value>
        /// The get clickable node.
        /// </value>
        public string NodeAttributeHTML { get; internal set; }
        /// <summary>
        /// Get the clickable node attributes, this implements NodeAttributeHTML
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public string GetClickableNodeAttribute(string className = null)
        {
            if (string.IsNullOrEmpty(className))
                return string.Format("class=\"mk-dataitem\" {0}", NodeAttributeHTML);
            return string.Format("class=\"{0} mk-dataitem\" {1}", className, NodeAttributeHTML);
        }
        /// <summary>
        /// Gets or sets the attribute.
        /// </summary>
        /// <value>
        /// The attribute.
        /// </value>
        public GridDataItemAttribute Attribute { get; set; }
        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        public int Index { get; internal set; }
        /// <summary>
        /// Gets the column property.
        /// </summary>
        /// <value>
        /// The column property.
        /// </value>
        public string ColumnProperty { get; internal set; }
        /// <summary>
        /// Gets the column.
        /// </summary>
        /// <value>
        /// The column.
        /// </value>
        internal ListDataColumn[] Columns { get; set; }
        public ListDataColumn Column { get; internal set; }
        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.Attribute.ToString(this.InnerHTML, this.Column);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ComponentListSearchEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentListEventArgs"/> class.
        /// </summary>
        /// <param name="selectedItemKey">The selected item key.</param>
        /// <param name="selectedGroupKey">The selected group key.</param>
        /// <param name="selectedGroupItemKey">The selected group item key.</param>
        public ComponentListSearchEventArgs(int selectedItemKey, int selectedGroupKey, int selectedGroupItemKey)
        {
            SelectedItemKey = selectedItemKey;
            SelectedGroupKey = selectedGroupKey;
            SelectedGroupItemKey = selectedGroupItemKey;
        }

        private int m_SelectedItemKey;
        /// <summary>
        /// Gets or sets the selected group key.
        /// </summary>
        /// <value>The selected group key.</value>
        public int SelectedItemKey
        {
            get { return m_SelectedItemKey; }
            set { m_SelectedItemKey = value; }
        }

        private int m_SelectedGroupKey;
        /// <summary>
        /// Gets or sets the selected group key.
        /// </summary>
        /// <value>The selected group key.</value>
        public int SelectedGroupKey
        {
            get { return m_SelectedGroupKey; }
            set { m_SelectedGroupKey = value; }
        }

        private int m_SelectedGroupItemKey;
        /// <summary>
        /// Gets or sets the selected group item key.
        /// </summary>
        /// <value>The selected group item key.</value>
        public int SelectedGroupItemKey
        {
            get { return m_SelectedGroupItemKey; }
            set { m_SelectedGroupItemKey = value; }
        }
    }

    public class ComponentDataReportEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentListEventArgs"/> class.
        /// </summary>
        public ComponentDataReportEventArgs()
        {
        }
        /// <summary>
        /// The reported count used for overview
        /// </summary>
        public int? ReportCount { get; set; }
        /// <summary>
        /// Should the report count be and alert code (mostly color red)
        /// </summary>
        public bool IsAlert { get; set; }
        /// <summary>
        /// When trying to initia
        /// </summary>
        public Exception Exception { get; internal set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ComponentListEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedKey"></param>
        public ComponentListEventArgs(int selectedKey)
            : this(selectedKey, 0) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedKey"></param>
        /// <param name="componentVersionKey"></param>
        public ComponentListEventArgs(int selectedKey, int componentVersionKey)
            : this(selectedKey, 0, componentVersionKey) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentListEventArgs"/> class.
        /// </summary>
        /// <param name="selectedKey">The selected key.</param>
        /// <param name="previousSelectedKey">The previous selected key.</param>
        /// <param name="componentVersionKey">The component version key.</param>
        public ComponentListEventArgs(int selectedKey, int previousSelectedKey, int componentVersionKey)
            : this(selectedKey, previousSelectedKey, componentVersionKey, 0, 0, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentListEventArgs"/> class.
        /// </summary>
        /// <param name="selectedKey">The selected key.</param>
        /// <param name="previousSelectedKey">The previous selected key.</param>
        /// <param name="componentVersionKey">The component version key.</param>
        /// <param name="selectedGroupKey">The selected group key.</param>
        /// <param name="selectedGroupItemKey">The selected group item key.</param>
        public ComponentListEventArgs(int selectedKey, int previousSelectedKey, int componentVersionKey, int selectedGroupKey, int selectedGroupItemKey, bool? isValidForm)
        {
            SelectedKey = selectedKey;
            SelectedGroupKey = selectedGroupKey;
            SelectedGroupItemKey = selectedGroupItemKey;
            VersionKey = componentVersionKey;
            PreviousSelectedKey = previousSelectedKey;
            IsValidForm = isValidForm;
        }

        public bool? IsValidForm { get; set; }

        private int m_PreviousSelectedKey;
        /// <summary>
        /// Gets or sets the previous selected key.
        /// </summary>
        /// <value>The previous selected key.</value>
        public int PreviousSelectedKey
        {
            get { return m_PreviousSelectedKey; }
            set { m_PreviousSelectedKey = value; }
        }

        private int m_SelectedGroupKey;
        /// <summary>
        /// Gets or sets the selected group key.
        /// </summary>
        /// <value>The selected group key.</value>
        public int SelectedGroupKey
        {
            get { return m_SelectedGroupKey; }
            set { m_SelectedGroupKey = value; }
        }

        private int m_SelectedGroupItemKey;
        /// <summary>
        /// Gets or sets the selected group item key.
        /// </summary>
        /// <value>The selected group item key.</value>
        public int SelectedGroupItemKey
        {
            get { return m_SelectedGroupItemKey; }
            set { m_SelectedGroupItemKey = value; }
        }

        private int m_SelectedKey;
        /// <summary>
        /// Gets or sets the selected key.
        /// </summary>
        /// <value>The selected key.</value>
        public int SelectedKey
        {
            get { return m_SelectedKey; }
            set { m_SelectedKey = value; }
        }

        private int m_VersionKey;
        /// <summary>
        /// Gets or sets the version key.
        /// </summary>
        /// <value>The version key.</value>
        public int VersionKey
        {
            get { return m_VersionKey; }
            set { m_VersionKey = value; }
        }
    }

    public class ComponentAsyncEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedKey"></param>
        public ComponentAsyncEventArgs(int selectedKey)
            : this(selectedKey, 0) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedKey"></param>
        /// <param name="componentVersionKey"></param>
        public ComponentAsyncEventArgs(int selectedKey, int componentVersionKey)
            : this(selectedKey, 0, componentVersionKey) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentListEventArgs"/> class.
        /// </summary>
        /// <param name="selectedKey">The selected key.</param>
        /// <param name="previousSelectedKey">The previous selected key.</param>
        /// <param name="componentVersionKey">The component version key.</param>
        public ComponentAsyncEventArgs(int selectedKey, int previousSelectedKey, int componentVersionKey)
            : this(selectedKey, previousSelectedKey, componentVersionKey, 0, 0) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentListEventArgs"/> class.
        /// </summary>
        /// <param name="selectedKey">The selected key.</param>
        /// <param name="previousSelectedKey">The previous selected key.</param>
        /// <param name="componentVersionKey">The component version key.</param>
        /// <param name="selectedGroupKey">The selected group key.</param>
        /// <param name="selectedGroupItemKey">The selected group item key.</param>
        public ComponentAsyncEventArgs(int selectedKey, int previousSelectedKey, int componentVersionKey, int selectedGroupKey, int selectedGroupItemKey)
        {
            SelectedKey = selectedKey;
            SelectedGroupKey = selectedGroupKey;
            SelectedGroupItemKey = selectedGroupItemKey;
            PreviousSelectedKey = previousSelectedKey;
        }

        /// <summary>
        /// Gets or sets the Asynchronous result.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public ASyncResult Data { get; set; }

        public string Property { get; set; }
        public string Query { get; set; }
        public ASyncQueryType SearchType { get; set; }

        Component _component;
        Beta.GeneratedCms.Console _container;
        internal void ApplyData(Component component, Beta.GeneratedCms.Console container)
        {
            _component = component;
            _container = container;
        }

        /// <summary>
        /// Loads the post data; this will trigger the Load and possible PreRender events which will generate a performance hit.
        /// </summary>
        public async Task LoadPostDataAsync()
        {
            await _component.CreateListAsync(_container, _container.OpenInFrame);
        }

        private int m_PreviousSelectedKey;
        /// <summary>
        /// Gets or sets the previous selected key.
        /// </summary>
        /// <value>The previous selected key.</value>
        public int PreviousSelectedKey
        {
            get { return m_PreviousSelectedKey; }
            set { m_PreviousSelectedKey = value; }
        }

        private int m_SelectedGroupKey;
        /// <summary>
        /// Gets or sets the selected group key.
        /// </summary>
        /// <value>The selected group key.</value>
        public int SelectedGroupKey
        {
            get { return m_SelectedGroupKey; }
            set { m_SelectedGroupKey = value; }
        }

        private int m_SelectedGroupItemKey;
        /// <summary>
        /// Gets or sets the selected group item key.
        /// </summary>
        /// <value>The selected group item key.</value>
        public int SelectedGroupItemKey
        {
            get { return m_SelectedGroupItemKey; }
            set { m_SelectedGroupItemKey = value; }
        }

        private int m_SelectedKey;
        /// <summary>
        /// Gets or sets the selected key.
        /// </summary>
        /// <value>The selected key.</value>
        public int SelectedKey
        {
            get { return m_SelectedKey; }
            set { m_SelectedKey = value; }
        }
    }

    public class ContentInfoEventArgs : EventArgs
    {

        public ContentInfoEventArgs(object value)
        {
            Value = value;
        }

        public object Value { get; set; }
    }
}
