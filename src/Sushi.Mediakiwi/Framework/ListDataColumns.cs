using Sushi.Mediakiwi.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// 
    /// </summary>
    public class ListDataColumns
    {
        private List<ListDataColumn> m_ColumnList;
        /// <summary>
        /// 
        /// </summary>
        public List<ListDataColumn> List
        {
            set
            {
                m_ColumnList = value;
            }
            get
            {
                if (m_ColumnList == null)
                    m_ColumnList = new List<ListDataColumn>();
                return m_ColumnList;
            }
        }

        private bool m_HasUniqueIdentifier = false;
        private bool m_HasHighlighted = false;

        private string m_ColumnHighlight;
        /// <summary>
        /// 
        /// </summary>
        public string ColumnHighlight
        {
            get { return m_ColumnHighlight; }
        }

        private string m_ColumnUniqueIdentifier;
        /// <summary>
        /// 
        /// </summary>
        public string ColumnUniqueIdentifier
        {
            get { return m_ColumnUniqueIdentifier; }
        }

        private string m_ColumnItemUrl;
        /// <summary>
        /// Gets the column item URL. When applied the item URL (click one) property is used for click on a item.
        /// </summary>
        /// <value>The column item URL.</value>
        public string ColumnItemUrl
        {
            get { return m_ColumnItemUrl; }
            set { m_ColumnItemUrl = value; }
        }

        /// <summary>
        /// Clear the ListDataColumn list.
        /// </summary>
        public void Clear()
        {
            if (m_ColumnList == null)
                return;

            m_ColumnList.Clear();
            m_HasHighlighted = false;
            m_HasUniqueIdentifier = false;
            m_ColumnHighlight = null;
            m_ColumnUniqueIdentifier = null;
        }

        /// <summary>
        /// Applies the property search column.
        /// </summary>
        /// <param name="listID">The list ID.</param>
        /// <param name="columnUniqueIdentifierPropertyName">Name of the column unique identifier property.</param>
        public void ApplyPropertySearchColumn(int listID, string columnUniqueIdentifierPropertyName)
        {
            ApplyPropertySearchColumn(listID, columnUniqueIdentifierPropertyName, false);
        }

        public void ApplyPropertySearchColumn(int listID, string columnUniqueIdentifierPropertyName, bool showID)
        {
            ApplyPropertySearchColumn(listID, columnUniqueIdentifierPropertyName, showID, false);
        }

        /// <summary>
        /// Applies the property search column.
        /// </summary>
        /// <param name="listID">The list ID.</param>
        /// <param name="columnUniqueIdentifierPropertyName">Name of the column unique identifier property.</param>
        /// <param name="showID">if set to <c>true</c> [show ID].</param>
        public void ApplyPropertySearchColumn(int listID, string columnUniqueIdentifierPropertyName, bool showID, bool hasColumnMapping)
        {
            if (!string.IsNullOrEmpty(columnUniqueIdentifierPropertyName))
            {
                if (showID)
                    this.Add("", columnUniqueIdentifierPropertyName, ListDataColumnType.UniqueIdentifierPresent, 20);
                else
                    this.Add("", columnUniqueIdentifierPropertyName, ListDataColumnType.UniqueIdentifier);
            }

            foreach (Sushi.Mediakiwi.Data.PropertySearchColumn psc in Sushi.Mediakiwi.Data.PropertySearchColumn.SelectAll(listID))
            {
                Sushi.Mediakiwi.Framework.ListDataColumnType lct = ListDataColumnType.Default;
                if (psc.IsHighlight)
                {
                    if (psc.IsOnlyExport)
                        lct = ListDataColumnType.Highlight;
                    else
                        lct = ListDataColumnType.HighlightPresent;
                }
                else if (psc.IsOnlyExport)
                {
                    lct = Sushi.Mediakiwi.Framework.ListDataColumnType.ExportOnly;
                }

                this.Add(psc.DisplayTitle
                    , (psc.Property.IsFixed || psc.Property.IsPresentProperty || hasColumnMapping) ? psc.Property.FieldName : string.Concat("Data.", psc.Property.FieldName)
                    , lct
                    , Sushi.Mediakiwi.Framework.ListDataContentType.Default
                    , null
                    , (Sushi.Mediakiwi.Framework.ListDataTotalType)psc.TotalType
                    , null
                    , psc.ColumnWidth.GetValueOrDefault()
                    , (psc.Property.ContentTypeID == ContentType.Date || psc.Property.ContentTypeID == ContentType.DateTime)
                        ? typeof(DateTime)
                        : null
                    );
            }
        }

        /// <summary>
        /// Add a column to the listSearch output grid 
        /// </summary>
        /// <param name="columnName">Name of the column to be presented</param>
        /// <param name="columnPropertyName">Name of the property from the IList (or Datatable columnName) object to show</param>
        [Obsolete("Please use Add(new ListDataColumn(\"columnName\", \"columnPropertyName\") { .. })", false)]
        public void Add(string columnName, string columnPropertyName)
        {
            Add(columnName, columnPropertyName, ListDataColumnType.Default);
        }

        /// <summary>
        /// Adds the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="columnPropertyName">Name of the column property.</param>
        /// <param name="editConfiguration">The edit configuration.</param>
        [Obsolete("Please use Add(new ListDataColumn(\"columnName\", \"columnPropertyName\") { .. })", false)]
        public void Add(string columnName, string columnPropertyName, ListDataEditConfiguration editConfiguration)
        {
            Add(columnName, columnPropertyName, ListDataColumnType.Default, ListDataContentType.Default, null, ListDataTotalType.Default, editConfiguration, 0);
        }

        /// <summary>
        /// Adds the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="columnPropertyName">Name of the column property.</param>
        /// <param name="columnWidth">Width of the column.</param>
        /// <param name="editConfiguration">The edit configuration.</param>
        [Obsolete("Please use Add(new ListDataColumn(\"columnName\", \"columnPropertyName\") { .. })", false)]
        public void Add(string columnName, string columnPropertyName, int columnWidth, ListDataEditConfiguration editConfiguration)
        {
            Add(columnName, columnPropertyName, ListDataColumnType.Default, ListDataContentType.Default, null, ListDataTotalType.Default, editConfiguration, columnWidth);
        }

        /// <summary>
        /// Adds the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="columnPropertyName">Name of the column property.</param>
        /// <param name="columnWidth">Width of the column.</param>
        [Obsolete("Please use Add(new ListDataColumn(\"columnName\", \"columnPropertyName\") { .. })", false)]
        public void Add(string columnName, string columnPropertyName, int columnWidth)
        {
            Add(columnName, columnPropertyName, ListDataColumnType.Default, ListDataContentType.Default, null, ListDataTotalType.Default, columnWidth);
        }

        /// <summary>
        /// Adds the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="columnPropertyName">Name of the column property.</param>
        /// <param name="columnWidth">Width of the column.</param>
        /// <param name="alignment">The alignment.</param>
        [Obsolete("Please use Add(new ListDataColumn(\"columnName\", \"columnPropertyName\") { .. })", false)]
        public void Add(string columnName, string columnPropertyName, int columnWidth, Align alignment)
        {
            Add(columnName, columnPropertyName, ListDataColumnType.Default, ListDataContentType.Default, null, ListDataTotalType.Default, null, columnWidth, null, alignment);
        }

        /// <summary>
        /// Adds the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="columnPropertyName">Name of the column property.</param>
        /// <param name="columnWidth">Width of the column.</param>
        /// <param name="alignment">The alignment.</param>
        /// <param name="totalType">The total type.</param>
        [Obsolete("Please use Add(new ListDataColumn(\"columnName\", \"columnPropertyName\") { .. })", false)]
        public void Add(string columnName, string columnPropertyName, int columnWidth, Align alignment, ListDataTotalType totalType)
        {
            Add(columnName, columnPropertyName, ListDataColumnType.Default, ListDataContentType.Default, null, totalType, null, columnWidth, null, alignment);
        }

        /// <summary>
        /// Add a column to the listSearch output grid 
        /// </summary>
        /// <param name="columnName">Name of the column to be presented</param>
        /// <param name="columnPropertyName">Name of the property from the IList object to show</param>
        /// <param name="columnType">Type of the column</param>
        [Obsolete("Please use Add(new ListDataColumn(\"columnName\", \"columnPropertyName\") { .. })", false)]
        public void Add(string columnName, string columnPropertyName, ListDataColumnType columnType)
        {
            Add(columnName, columnPropertyName, columnType, ListDataContentType.Default, null, ListDataTotalType.Default);
        }

        /// <summary>
        /// Adds the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="columnPropertyName">Name of the column property.</param>
        /// <param name="columnType">Type of the column.</param>
        /// <param name="columnWidth">Width of the column.</param>
        [Obsolete("Please use Add(new ListDataColumn(\"columnName\", \"columnPropertyName\") { .. })", false)]
        public void Add(string columnName, string columnPropertyName, ListDataColumnType columnType, int columnWidth)
        {
            Add(columnName, columnPropertyName, columnType, ListDataContentType.Default, null, ListDataTotalType.Default, columnWidth);
        }

        /// <summary>
        /// Add a column to the listSearch output grid
        /// </summary>
        /// <param name="columnName">Name of the column to be presented</param>
        /// <param name="columnPropertyName">Name of the property from the IList object to show</param>
        /// <param name="columnType">Type of the column</param>
        /// <param name="totalType">The total type.</param>
        [Obsolete("Please use Add(new ListDataColumn(\"columnName\", \"columnPropertyName\") { .. })", false)]
        public void Add(string columnName, string columnPropertyName, ListDataColumnType columnType, ListDataTotalType totalType)
        {
            Add(columnName, columnPropertyName, columnType, ListDataContentType.Default, null, totalType);
        }

        /// <summary>
        /// Add a column to the listSearch output grid 
        /// </summary>
        /// <param name="columnName">Name of the column to be presented</param>
        /// <param name="columnPropertyName">Name of the property from the IList object to show</param>
        /// <param name="columnType">Type of the column</param>
        /// <param name="contentType"></param>
        [Obsolete("Please use Add(new ListDataColumn(\"columnName\", \"columnPropertyName\") { .. })", false)]
        public void Add(string columnName, string columnPropertyName, ListDataColumnType columnType, ListDataContentType contentType)
        {
            Add(columnName, columnPropertyName, columnType, contentType, null, ListDataTotalType.Default);
        }

        /// <summary>
        /// Add a column to the listSearch output grid
        /// </summary>
        /// <param name="columnName">Name of the column to be presented</param>
        /// <param name="columnPropertyName">Name of the property from the IList object to show</param>
        /// <param name="columnType">Type of the column</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="customTypeIdentifier">The custom type identifier.</param>
        [Obsolete("Please use Add(new ListDataColumn(\"columnName\", \"columnPropertyName\") { .. })", false)]
        public void Add(string columnName, string columnPropertyName, ListDataColumnType columnType, ListDataContentType contentType, string queryStringAdditionProperty)
        {
            Add(columnName, columnPropertyName, columnType, contentType, queryStringAdditionProperty, ListDataTotalType.Default);
        }

        /// <summary>
        /// Adds the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="columnPropertyName">Name of the column property.</param>
        /// <param name="columnType">Type of the column.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="customTypeIdentifier">The custom type identifier.</param>
        /// <param name="totalType">The total type.</param>
        [Obsolete("Please use Add(new ListDataColumn(\"columnName\", \"columnPropertyName\") { .. })", false)]
        public void Add(string columnName, string columnPropertyName, ListDataColumnType columnType, ListDataContentType contentType, string queryStringAdditionProperty, ListDataTotalType totalType)
        {
            Add(columnName, columnPropertyName, columnType, contentType, queryStringAdditionProperty, totalType, 0);
        }

        /// <summary>
        /// Adds the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="columnPropertyName">Name of the column property.</param>
        /// <param name="columnType">Type of the column.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="customTypeIdentifier">The custom type identifier.</param>
        /// <param name="totalType">The total type.</param>
        /// <param name="columnWidth">Width of the column.</param>
        [Obsolete("Please use Add(new ListDataColumn(\"columnName\", \"columnPropertyName\") { .. })", false)]
        public void Add(string columnName, string columnPropertyName, ListDataColumnType columnType, ListDataContentType contentType, string queryStringAdditionProperty, ListDataTotalType totalType, int columnWidth)
        {
            Add(columnName, columnPropertyName, columnType, contentType, queryStringAdditionProperty, totalType, null, columnWidth);
        }

        /// <summary>
        /// Adds the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="columnPropertyName">Name of the column property.</param>
        /// <param name="columnType">Type of the column.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="queryStringAdditionProperty">The query string addition property.</param>
        /// <param name="totalType">The total type.</param>
        /// <param name="editConfiguration">The edit configuration.</param>
        /// <param name="columnWidth">Width of the column.</param>
        [Obsolete("Please use Add(new ListDataColumn(\"columnName\", \"columnPropertyName\") { .. })", false)]
        public void Add(string columnName, string columnPropertyName, ListDataColumnType columnType, ListDataContentType contentType, string queryStringAdditionProperty, ListDataTotalType totalType, ListDataEditConfiguration editConfiguration, int columnWidth)
        {
            Add(columnName, columnPropertyName, columnType, contentType, queryStringAdditionProperty, totalType, editConfiguration, columnWidth, null);
        }

        /// <summary>
        /// Adds the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="columnPropertyName">Name of the column property.</param>
        /// <param name="columnType">Type of the column.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="queryStringAdditionProperty">The query string addition property.</param>
        /// <param name="totalType">The total type.</param>
        /// <param name="editConfiguration">The edit configuration.</param>
        /// <param name="columnWidth">Width of the column.</param>
        /// <param name="type">The type.</param>
        [Obsolete("Please use Add(new ListDataColumn(\"columnName\", \"columnPropertyName\") { .. })", false)]
        public void Add(string columnName, string columnPropertyName, ListDataColumnType columnType, ListDataContentType contentType, string queryStringAdditionProperty, ListDataTotalType totalType, ListDataEditConfiguration editConfiguration, int columnWidth, System.Type type)
        {
            Add(columnName, columnPropertyName, columnType, contentType, queryStringAdditionProperty, totalType, editConfiguration, columnWidth, type, Align.Default);
        }

        /// <summary>
        /// Adds the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="columnPropertyName">Name of the column property.</param>
        /// <param name="columnWidth">Width of the column.</param>
        /// <param name="alignment">The alignment.</param>
        /// <param name="columnType">Type of the column.</param> 
        [Obsolete("Please use Add(new ListDataColumn(\"columnName\", \"columnPropertyName\") { .. })", false)]
        public void Add(string columnName, string columnPropertyName, int columnWidth, Align alignment, ListDataColumnType columnType)
        {
            Add(columnName, columnPropertyName, columnType, ListDataContentType.Default, null, ListDataTotalType.Default, null, columnWidth, null, alignment);
        }
        /// <summary>
        /// Add a column to the listSearch output grid
        /// </summary>
        /// <param name="columnName">Name of the column to be presented</param>
        /// <param name="columnPropertyName">Name of the property from the IList object to show</param>
        /// <param name="columnType">Type of the column</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="queryStringAdditionProperty">The query string addition property.</param>
        /// <param name="totalType">The total type.</param>
        /// <param name="editConfiguration">The edit configuration.</param>
        /// <param name="columnWidth">Width of the column.</param>
        /// <param name="type">The type.</param>
        /// <param name="alignment">The alignment.</param>
        [Obsolete("Please use Add(new ListDataColumn(\"columnName\", \"columnPropertyName\") { .. })", false)]
        public void Add(string columnName, string columnPropertyName, ListDataColumnType columnType, ListDataContentType contentType, string queryStringAdditionProperty, ListDataTotalType totalType, ListDataEditConfiguration editConfiguration, int columnWidth, System.Type type, Align alignment)
        {
            if (columnType == ListDataColumnType.Checkbox || columnType == Sushi.Mediakiwi.Framework.ListDataColumnType.RadioBox)
            {
                //if (columnWidth == 0)
                //    columnWidth = 20;

                if (alignment == Align.Default)
                    alignment = Align.Center;
            }

            bool isHighlight = (
                columnType == ListDataColumnType.Highlight ||
                columnType == ListDataColumnType.UniqueHighlightedIdentifier ||
                columnType == ListDataColumnType.UniqueHighlightedIdentifierPresent
                );
            bool isUniqueIdentifier = (
                columnType == ListDataColumnType.UniqueIdentifier ||
                columnType == ListDataColumnType.UniqueIdentifierPresent ||
                columnType == ListDataColumnType.UniqueHighlightedIdentifier ||
                columnType == ListDataColumnType.UniqueHighlightedIdentifierPresent
                );

            if (isUniqueIdentifier)
            {
                m_ColumnUniqueIdentifier = columnPropertyName;
                if (m_HasUniqueIdentifier)
                    return;
                //    throw new Exception("The ListDataColumn collection already has a column set the unique identifier");
                m_HasUniqueIdentifier = true;
            }

            if (isHighlight)
            {
                m_ColumnHighlight = columnPropertyName;
                if (m_HasHighlighted)
                    return;
                //    throw new Exception("The ListDataColumn collection already has a column set to be highlighted");
                m_HasHighlighted = true;
            }

            ListDataColumn col = new ListDataColumn(columnName, columnPropertyName);
            col.Type = columnType;
            //col.ColumnName = columnName;
            col.ContentType = contentType;
            col.Total = totalType;
            //col.ColumnValuePropertyName = columnPropertyName;
            col.ColumnWidth = columnWidth;
            col.EditConfiguration = editConfiguration;
            col.DataType = type;
            col.Alignment = alignment;
            //if (customTypeIdentifier.HasValue) col.AdditionalQueryStringParameter = "&type=" + customTypeIdentifier.ToString();
            List.Add(col);
        }

        public void Add(ListDataColumn listColumn)
        {
            bool isHighlight = (
                listColumn.Type == ListDataColumnType.Highlight ||
                listColumn.Type == ListDataColumnType.UniqueHighlightedIdentifier ||
                listColumn.Type == ListDataColumnType.UniqueHighlightedIdentifierPresent
                );
            bool isUniqueIdentifier = (
                listColumn.Type == ListDataColumnType.UniqueIdentifier ||
                listColumn.Type == ListDataColumnType.UniqueIdentifierPresent ||
                listColumn.Type == ListDataColumnType.UniqueHighlightedIdentifier ||
                listColumn.Type == ListDataColumnType.UniqueHighlightedIdentifierPresent
                );

            if (isUniqueIdentifier)
            {
                m_ColumnUniqueIdentifier = listColumn.ColumnValuePropertyName;
                if (m_HasUniqueIdentifier)
                    return;
                m_HasUniqueIdentifier = true;
            }

            if (isHighlight)
            {
                m_ColumnHighlight = listColumn.ColumnValuePropertyName;
                if (m_HasHighlighted)
                    return;
                m_HasHighlighted = true;
            }

            List.Add(listColumn);
        }

        /// <summary>
        /// Removes the specified column property name.
        /// </summary>
        /// <param name="columnPropertyName">Name of the column property.</param>
        /// <returns></returns>
        public bool Remove(string columnPropertyName)
        {
            if (this.List == null) return false;
            foreach (ListDataColumn col in this.List)
            {
                if (col.ColumnValuePropertyName == columnPropertyName)
                {
                    this.List.Remove(col);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets or sets the parameter property.
        /// </summary>
        /// <value>The parameter property.</value>
        public string ParameterProperty { get; set; }
        public string QueryParameter { get; set; }

        /// <summary>
        /// Adds the querystring parameter property.
        /// </summary>
        /// <param name="columnPropertyName">Name of the column property.</param>
        /// <param name="queryParameter">The query parameter.</param>
        public void AddQuerystringParameterProperty(string columnPropertyName, string queryParameter)
        {
            ParameterProperty = columnPropertyName;
            QueryParameter = queryParameter;
        }
    }
}
