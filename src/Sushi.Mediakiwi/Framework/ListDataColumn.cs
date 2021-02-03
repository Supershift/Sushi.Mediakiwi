using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// 
    /// </summary>
    public enum Align{
        /// <summary>
        /// 
        /// </summary>
        Default = 0,
        /// <summary>
        /// 
        /// </summary>
        Left = 1,
        /// <summary>
        /// 
        /// </summary>
        Center = 2,
        /// <summary>
        /// 
        /// </summary>
        Right = 3
    }

    /// <summary>
    /// Represents a ListDataColumn entity.
    /// </summary>
    public class ListDataColumn
    {
        public ListDataColumn(string columnName, string columnValuePropertyName)
        {
            this.ColumnName = columnName;
            this.ColumnValuePropertyName = columnValuePropertyName;
            this.Type = ListDataColumnType.Default;
        }

        public ListDataColumn(string columnName, string columnValuePropertyName, ListDataColumnType type)
        {
            this.ColumnName = columnName;
            this.ColumnValuePropertyName = columnValuePropertyName;
            this.Type = type;
        }

        System.Type _type;
        int _maxInputLength;
        bool _containsSpaces;
        /// <summary>
        /// Calculates the length.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <param name="info">The info.</param>
        internal void CalculateLength(string html, System.Reflection.PropertyInfo info)
        {
            if (string.IsNullOrEmpty(html) || this.ColumnWidth > 0)
                return;

            if (info != null && _type == null)
                _type = info.PropertyType;

            string txt = Data.Utility.CleanFormatting(html);
            if (txt.Length > _maxInputLength)
                _maxInputLength = txt.Length;

            if (txt.Contains(" ") || txt.Contains("&nbsp;"))
                _containsSpaces = true;
        }

        int? _SuggestedColumnLength;
        /// <summary>
        /// Gets the length of the suggested column.
        /// </summary>
        /// <value>
        /// The length of the suggested column.
        /// </value>
        internal int SuggestedColumnLength
        {
            get{
        
                if (!_SuggestedColumnLength.HasValue)
                {
                    _SuggestedColumnLength = this.ColumnWidth;
                    //if (this.ColumnWidth == 0)
                    //{
                    //    if (_type != null && (_type == typeof(DateTime) || _type == typeof(DateTime?)))
                    //    {
                    //        if (_maxInputLength == 8)
                    //            //  Date
                    //            _SuggestedColumnLength = 110;
                    //        else
                    //            //  DateTime
                    //            _SuggestedColumnLength = 120;
                    //    }
                    //    else if (_type != null && (_type == typeof(int) || _type == typeof(int?)))
                    //    {
                    //        if (!_containsSpaces)
                    //            _SuggestedColumnLength = 20;
                    //    }
                    //    else if (_type != null && (_type == typeof(decimal) || _type == typeof(decimal?)))
                    //    {
                    //        if (!_containsSpaces) 
                    //            _SuggestedColumnLength = 30;
                    //    }
                    //    else if (_type != null && (_type == typeof(bool) || _type == typeof(bool?)))
                    //    {
                    //        if (_maxInputLength > 5)
                    //            _SuggestedColumnLength = 60;
                    //        else
                    //            _SuggestedColumnLength = 30;
                    //    }
                    //    //else if (_maxInputLength == 0)
                    //    //{
                    //    //    _SuggestedColumnLength = 20;
                    //    //}
                    //}
                }
                return _SuggestedColumnLength.Value;
            }
        }

        /// <summary>
        /// The tooltip, you can use H3 for headers.
        /// </summary>
        public string Tooltip;
        /// <summary>
        /// 
        /// </summary>
        public Align Alignment;
        /// <summary>
        /// 
        /// </summary>
        public ListDataEditConfiguration EditConfiguration;
        /// <summary>
        /// 
        /// </summary>
        public ListDataContentType ContentType;
        /// <summary>
        /// 
        /// </summary>
        public ListDataColumnType Type;
        /// <summary>
        /// When exporting to XLS and the content is a number than this option can treat the value as text (f.e. when applying EAN codes).
        /// </summary>
        public bool WhenExportingTreatAsText;
        /// <summary>
        /// 
        /// </summary>
        public ListDataTotalType Total;
        /// <summary>
        /// To which type should the value be cast?
        /// </summary>
        public System.Type DataType;
        /// <summary>
        /// This field is used internally for the calculation of the total type. 
        /// </summary>
        public ListDataTemporaryTotalInfoColumn TotalTemporaryValue;
        /// <summary>
        /// 
        /// </summary>
        public string ColumnName;

        /// <summary>
        /// Gets the column title.
        /// </summary>
        /// <value>
        /// The column title.
        /// </value>
        internal string ColumnTitle
        {
            get
            {
                if (string.IsNullOrEmpty(this.Tooltip))
                    return ColumnName;
                return string.Format("<abbr title=\"{0}\">{1}</abbr>", this.Tooltip, this.ColumnName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string AdditionalQueryStringParameter;
        /// <summary>
        /// 
        /// </summary>
        public string ColumnValuePropertyName;
        /// <summary>
        /// 
        /// </summary>
        public int? PropertyIndex;
        /// <summary>
        /// 
        /// </summary>
        public string ColumnValuePrefix;
        public string ColumnValueSuffix;
        public bool ColumnIsFixed;
        public bool ShrinkTextWhenToLarge;
        internal int ColumnFixedLeftMargin;
        public int ColumnWidth;
        public int? ColumnHeight;
        /// <summary>
        /// 
        /// </summary>
        internal decimal TotalValue;
        internal Type TotalValueType;
    }
}
