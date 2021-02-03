using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sushi.Mediakiwi.Framework.Presentation
{
    internal class Utility
    {
        static string _Version;
        internal static string Version
        {
            get
            {
                if (_Version == null)
                {
                    var split = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion.Split('.');
                    _Version = string.Format("{0}.{1}", split[0], split[1]);
                }
                return _Version;
            }
        }
    }
}
