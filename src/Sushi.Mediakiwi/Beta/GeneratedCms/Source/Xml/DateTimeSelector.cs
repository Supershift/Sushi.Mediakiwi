using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Beta.GeneratedCms.Source.Xml
{
    class DateTimeSelector
    {
        internal static string Get(string repositoryPath)
        {
            StringBuilder build = new StringBuilder();

            build.AppendFormat(@"<?xml version=""1.0""?>
<root>
	<div class=""dateBorder"">
		<ul class=""controls"">
			<li><button class=""previous srcMouseHover""><img alt=""Previous Month"" src=""{0}/images/icon_left_link.png""/></button></li>
			<li><button class=""next srcMouseHover""><img alt=""Next Month"" src=""{0}/images/icon_right_link.png""/></button></li>
		</ul>
		<table class=""dateTable"">
			<caption>
				<select class=""month"">", repositoryPath);

            build.Append(@"
					<option value=""0"">jan</option>
					<option value=""1"">feb</option>
					<option value=""2"">mar</option>
					<option value=""3"">apr</option>
					<option value=""4"">mei</option>
					<option value=""5"">jun</option>
					<option value=""6"">jul</option>
					<option value=""7"">aug</option>
					<option value=""8"">sep</option>
					<option value=""9"">okt</option>
					<option value=""10"">nov</option>
					<option value=""11"">dec</option>
				</select>
				<select class=""year"">
					<option value=""2007"" selected=""selected"">2007</option>
				</select>
			</caption>
			<thead>
				<tr>
					<th scope=""col"">s</th>
					<th scope=""col"">m</th>
					<th scope=""col"">t</th>
					<th scope=""col"">w</th>
					<th scope=""col"">t</th>
					<th scope=""col"">f</th>
					<th scope=""col"">s</th>
				</tr>
			</thead>
			<tbody>
				<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td></tr>
				<tr><td>{7}</td><td>{8}</td><td>{9}</td><td>{10}</td><td>{11}</td><td>{12}</td><td>{13}</td></tr>
				<tr><td>{14}</td><td>{15}</td><td>{16}</td><td>{17}</td><td>{18}</td><td>{19}</td><td>{20}</td></tr>
				<tr><td>{21}</td><td>{22}</td><td>{23}</td><td>{24}</td><td>{25}</td><td>{26}</td><td>{27}</td></tr>
				<tr><td>{28}</td><td>{29}</td><td>{30}</td><td>{31}</td><td>{32}</td><td>{33}</td><td>{34}</td></tr>
				<tr><td>{35}</td><td>{36}</td><td>{37}</td><td>{38}</td><td>{39}</td><td>{40}</td><td>{41}</td></tr>
			</tbody>
		</table>
	</div>
</root>");

            return build.ToString();
        }
    }
}
