using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// 
    /// </summary>
    public interface iOption
    {
        /// <summary>
        /// Optionses this instance.
        /// </summary>
        /// <returns></returns>
        iNameValue[] Options(int optionListID);
    }
}
