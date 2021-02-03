using System;
using System.Collections.Generic;
using System.Text;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.Beta.GeneratedCms.Source.Xml
{
    class Timesheet
    {
        internal static string Get(Console container, string componentListReference)
        {
            WimControlBuilder build = new WimControlBuilder();

            SpecialTypes.TimesheetLineAttribute attr = new Sushi.Mediakiwi.Framework.ContentListItem.SpecialTypes.TimesheetLineAttribute(componentListReference);
            attr.ID = string.Concat("ux_", DateTime.Now.Ticks.ToString());
            attr.Console = container;

            build.Append(@"<?xml version=""1.0""?>");
            
            attr.WriteCandidate(build, true, false, false);
            return build.ToString();
        }
    }
}
