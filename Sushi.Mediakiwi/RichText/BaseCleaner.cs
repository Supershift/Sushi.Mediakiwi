using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Sushi.Mediakiwi.RichRext
{
    public class BaseCleaner
    {
        protected static RegexOptions DefaultOptions = RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace;
    }
}
