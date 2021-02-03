using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// 
    /// </summary>
    public enum FolderType
    {
        /// <summary>
        /// 
        /// </summary>
        Page = 1,
        /// <summary>
        /// 
        /// </summary>
        List = 2,
        /// <summary>
        /// 
        /// </summary>
        Administration = 3,
        /// <summary>
        /// 
        /// </summary>
        Gallery = 4,
        /// <summary>
        /// use Undefined if the folder type is based on the current folder
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// The administration_ and_ list
        /// </summary>
        Administration_Or_List = 5
    }
}
