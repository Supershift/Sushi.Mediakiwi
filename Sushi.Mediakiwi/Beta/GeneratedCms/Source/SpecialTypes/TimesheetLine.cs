using System;
using System.Collections.Generic;
using System.Text;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.Beta.GeneratedCms.Source.SpecialTypes
{
    class TimesheetLine
    {
        public TimesheetLine()
        {

        }

        public void CreateBlock(Console container, WimControlBuilder build, WimControlBuilder build2, string componentListReference)
        {
            //build.Append(build2.ToString());
            //CreateFooter(container, build);
            CreateHeader(container, build, componentListReference);
            
        }

        public void CreateHeader(Console container, WimControlBuilder build, string componentListReference)
        {
            build.AppendFormat(@"
<dl class=""application fixed"">
	<dt class=""active"">
		<span class=""label toggleNextNode parent_1"">&nbsp;</span>
        <span class=""close toggleNextNode parent_1""><button class=""toClose"">Close</button><button class=""toOpen"">Open</button></span>
	</dt>
	<dd class=""showThisNode"">
		<table class=""data timesheet"">
			<thead>
				<tr>
					<th scope=""col"" class=""title""><input type=""hidden"" value=""{0}?xml=timesheet&list={1}""/>Project</th>
					<th scope=""col"" class=""day"">M</th>
					<th scope=""col"" class=""day"">T</th>
					<th scope=""col"" class=""day"">W</th>
					<th scope=""col"" class=""day"">T</th>
					<th scope=""col"" class=""day"">F</th>
					<th scope=""col"" class=""day"">S</th>
					<th scope=""col"" class=""day"">S</th>
					<th scope=""col"" class=""total"">Tot.</th>
				</tr>
			</thead>
			<tfoot>
				<tr>
					<td class=""title""></td>
					<td class=""day"">8</td>
					<td class=""day"">8</td>
					<td class=""day"">8</td>
					<td class=""day"">8</td>
					<td class=""day"">8</td>
					<td class=""day"">8</td>
					<td class=""day"">8</td>
					<td class=""total"">40</td>
				</tr>
			</tfoot>
			<tbody>
"
                , container.WimPagePath, componentListReference);
        }

        public void CreateFooter(Console container, StringBuilder build)
        {
            if (container.View == 2)
            {
                build.Append("\n</tbody></table>");
                return;
            }
            build.Append("\n</tbody></table></dd></dl>");
        }
    }
}
