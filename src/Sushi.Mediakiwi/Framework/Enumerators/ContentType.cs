using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// 
    /// </summary>
    public enum ContentType
    {
        /// <summary>
        /// 
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// 
        /// </summary>
        TextLine = 9,
        /// <summary>
        /// 
        /// </summary>
        TextField = 10,
        /// <summary>
        /// 
        /// </summary>
        TextArea = 11,
        /// <summary>
        /// 
        /// </summary>
        RichText = 12,
        /// <summary>
        /// 
        /// </summary>
        Date = 13,
        /// <summary>
        /// 
        /// </summary>
        DateTime = 15,
        /// <summary>
        /// 
        /// </summary>
        Choice_Radio = 17,
        /// <summary>
        /// 
        /// </summary>
        Choice_Dropdown = 18,
        /// <summary>
        /// 
        /// </summary>
        Binary_Image = 19,
        /// <summary>
        /// 
        /// </summary>
        Binary_Document = 20,
        /// <summary>
        /// 
        /// </summary>
        Hyperlink = 21,
        /// <summary>
        /// 
        /// </summary>
        PageSelect = 22,
        /// <summary>
        /// 
        /// </summary>
        FolderSelect = 23,
        /// <summary>
        /// 
        /// </summary>
        ListItemSelect = 24,
        /// <summary>
        /// 
        /// </summary>
        SubListSelect = 25,
        /// <summary>
        /// 
        /// </summary>
        FileUpload = 26,
        /// <summary>
        /// 
        /// </summary>
        Button = 27,
        /// <summary>
        /// 
        /// </summary>
        Choice_Checkbox = 28,
        /// <summary>
        /// 
        /// </summary>
        MultiImageSelect = 29,
        /// <summary>
        /// Specialtype : Timesheet line 
        /// </summary>
        TimeSheetLine = 30,
        /// <summary>
        /// Specialtype : Text date
        /// </summary>
        TextDate = 31,
        /// <summary>
        /// Section. Replicates a group border (open/close)
        /// </summary>
        Section = 32,
        /// <summary>
        /// 
        /// </summary>
        DataList = 33,
        /// <summary>
        /// 
        /// </summary>
        DataExtend = 34,
        /// <summary>
        /// 
        /// </summary>
        ContentContainer = 35,
        /// <summary>
        /// 
        /// </summary>
        HtmlContainer = 36,
        /// <summary>
        /// 
        /// </summary>
        DocumentSelect = 37,
        /// <summary>
        /// 
        /// </summary>
        MultiField = 38,
        /// <summary>
        /// 
        /// </summary>
        Sourcecode = 39,
        /// <summary>
        /// Upload for multiple assets uploaded in a HTML5 async way
        /// </summary>
        MultiAssetUpload = 40,
    }
}
