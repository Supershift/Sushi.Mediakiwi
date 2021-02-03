using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUrlReWriter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="page"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        bool IsExistingPage(string foldername, string pagename, out Sushi.Mediakiwi.Data.Page page, out string queryString);
    }
}
