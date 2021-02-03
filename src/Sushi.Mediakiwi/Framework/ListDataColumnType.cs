using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// 
    /// </summary>
    public enum ListDataColumnType
    {
        /// <summary>
        /// This is a default data column
        /// </summary>
        Default = 0,
        /// <summary>
        /// This column defines the unique identier used in ComponentListEventArgs.
        /// The column doesn't show in the presented list.
        /// </summary>
        UniqueIdentifier = 1,
        /// <summary>
        /// This column defines the unique identier used in ComponentListEventArgs.
        /// The column shows up in the the presented list.
        /// </summary>
        UniqueIdentifierPresent = 2,
        /// <summary>
        /// This column defines the unique identier used in ComponentListEventArgs and also shows up as human readable reference in derived lists.
        /// The column doesn't show in the presented list.
        /// </summary>
        UniqueHighlightedIdentifier = 3,
        /// <summary>
        /// This column defines the unique identier used in ComponentListEventArgs and also shows up as human readable reference in derived lists.
        /// The column shows up in the the presented list.
        /// </summary>
        UniqueHighlightedIdentifierPresent = 4,
        /// <summary>
        /// This column shows up as human readable reference in derived lists.
        /// The column doesn't show in the presented list.
        /// </summary>
        Highlight = 5,
        /// <summary>
        /// This column shows up as human readable reference in derived lists.
        /// The column shows up in the the presented list.
        /// </summary>
        HighlightPresent = 6,
        /// <summary>
        /// This columns is only exposed for export.
        /// </summary>
        ExportOnly = 7,
        /// <summary>
        /// Return a checkbox item with the columnpropertyname as id.
        /// </summary>
        Checkbox = 8 ,
        /// <summary>
        /// Return a checkbox item with the columnpropertyname as id.
        /// </summary>
        RadioBox = 9,
        /// <summary>
        /// The view only
        /// </summary>
        ViewOnly = 10
    }
}
