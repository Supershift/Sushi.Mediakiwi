using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Data.DalReflection
{
    /// <summary>
    /// Database data value connect type options
    /// </summary>
    public enum DatabaseDataValueConnectType
    {
        /// <summary>
        /// 
        /// </summary>
        And,
        /// <summary>
        /// Using Or will combine the previous and the current columns in a Or group (A = 1 or B = 2)
        /// </summary>
        Or,
        /// <summary>
        ///  Using OrUngrouped will just add the or statement without setting an or group
        /// </summary>
        OrUngrouped
    }
}
