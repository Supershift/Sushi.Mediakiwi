using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Sushi.Mediakiwi.Data.Parsers;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.Data
{
    public class EnvironmentVersionLogic
    {
        static IEnvironmentVersionParser _Parser;
        static IEnvironmentVersionParser Parser
        {
            get
            {
                if (_Parser == null)
                    _Parser = Environment.GetInstance<IEnvironmentVersionParser>();
                return _Parser;
            }
        }

        /// <summary>
        /// Flush all the cached content and it's servernodes
        /// </summary>
        /// <param name="setChacheVersion">if set to <c>true</c> [set chache version].</param>
        /// <param name="context">The context.</param>
        public static void Flush(bool setChacheVersion = true, HttpContext context = null)
        {
            Parser.Flush(setChacheVersion, context);
        }

    }
}
