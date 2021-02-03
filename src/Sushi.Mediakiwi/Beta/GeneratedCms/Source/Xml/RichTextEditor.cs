using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Beta.GeneratedCms.Source.Xml
{
    /// <summary>
    /// Represents a RichTextEditor entity.
    /// </summary>
    class RichTextEditor
    {
        /// <summary>
        /// Gets the specified repository path.
        /// </summary>
        /// <param name="repositoryPath">The repository path.</param>
        /// <param name="wimPath">The wim path.</param>
        /// <param name="hasError">if set to <c>true</c> [has error].</param>
        /// <returns></returns>
        internal static string Get(string repositoryPath, string wimPath, bool hasError)
        {
            StringBuilder build = new StringBuilder();

            var list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(Sushi.Mediakiwi.Data.ComponentListType.Links);

            build.Append(@"<?xml version=""1.0""?>
<root>
	<stylesheet>
		<link href=""" + repositoryPath + @"/styles/markup.css"" type=""text/css"" rel=""StyleSheet""/>
		<style>
			body {
                padding : 2px 4px 2px 4px;
");
            if (hasError)
                build.Append(@"background-color: #ffd000;");
            
            build.Append(@"
			}
		</style>
	</stylesheet>
	<editorHtml><ul class=""controls htmlControls"">");

            //build.AppendFormat(@"<li><button class=""cmd_toggle srcMouseHover""><img alt=""Text"" src=""{0}/images/cmd_toggleHtml_link.png""/></button></li>", repositoryPath);
            build.AppendFormat(@"<li><button class=""cmd_bigger srcMouseHover""><img alt=""Resize"" src=""{0}/images/cmd_bigger_link.png""/></button></li>", repositoryPath);
            //build.AppendFormat(@"<li><button class=""cmd_redo srcMouseHover""><img alt=""Redo"" src=""{0}/images/cmd_redo_link.png""/></button></li>", repositoryPath);
            //build.AppendFormat(@"<li><button class=""cmd_undo srcMouseHover""><img alt=""Undo"" src=""{0}/images/cmd_undo_link.png""/></button></li>", repositoryPath);
            //build.AppendFormat(@"<li><button class=""cmd_copy srcMouseHover""><img alt=""Copy"" src=""{0}/images/cmd_copy_link.png""/></button></li>", repositoryPath);
            //build.AppendFormat(@"<li><button class=""cmd_cut srcMouseHover""><img alt=""Cut"" src=""{0}/images/cmd_cut_link.png""/></button></li>", repositoryPath);
            //build.AppendFormat(@"<li><button class=""cmd_paste srcMouseHover""><img alt=""Paste"" src=""{0}/images/cmd_paste_link.png""/></button></li>", repositoryPath);
            //build.AppendFormat(@"<li><button class=""cmd_delete srcMouseHover""><img alt=""Delete"" src=""{0}/images/cmd_delete_link.png""/></button></li>", repositoryPath);
            
            //build.AppendFormat(@"<li><button class=""cmd_formatblock arg_h1 srcMouseHover""><img alt=""H1"" src=""{0}/images/cmd_h1_link.png""/></button></li>", repositoryPath);
            //build.AppendFormat(@"<li><button class=""cmd_formatblock arg_h2 srcMouseHover""><img alt=""H2"" src=""{0}/images/cmd_h2_link.png""/></button></li>", repositoryPath);
            //build.AppendFormat(@"<li><button class=""cmd_formatblock arg_h3 srcMouseHover""><img alt=""H3"" src=""{0}/images/cmd_h3_link.png""/></button></li>", repositoryPath);
            
            //build.AppendFormat(@"<li><button class=""cmd_formatblock arg_p srcMouseHover""><img alt=""P"" src=""{0}/images/cmd_p_link.png""/></button></li>", repositoryPath);
            build.AppendFormat(@"<li> <button class=""cmd_bold srcMouseHover""><img alt=""Bold"" src=""{0}/images/cmd_bold_link.png""/></button></li>", repositoryPath);
            build.AppendFormat(@"<li><button class=""cmd_italic srcMouseHover""><img alt=""Italic"" src=""{0}/images/cmd_italic_link.png""/></button></li>", repositoryPath);
            build.AppendFormat(@"<li><button class=""cmd_underline srcMouseHover""><img alt=""Underline"" src=""{0}/images/cmd_underline_link.png""/></button></li>", repositoryPath);
            build.AppendFormat(@"<li> <button class=""cmd_insertorderedlist srcMouseHover""><img alt=""Ordered List"" src=""{0}/images/cmd_insertorderedlist_link.png""/></button></li>", repositoryPath);
            build.AppendFormat(@"<li><button class=""cmd_insertunorderedlist srcMouseHover""><img alt=""Unordered List"" src=""{0}/images/cmd_insertunorderedlist_link.png""/></button></li>", repositoryPath);
            build.AppendFormat(@"<li><button class=""cmd_outdent srcMouseHover""><img alt=""Outdent"" src=""{0}/images/cmd_outdent_link.png""/></button></li>", repositoryPath);
            build.AppendFormat(@"<li><button class=""cmd_indent srcMouseHover""><img alt=""Indent"" src=""{0}/images/cmd_indent_link.png""/></button></li>", repositoryPath);
            //build.AppendFormat(@"<li><button class=""cmd_customlink srcMouseHover""><img alt=""Link"" src=""{0}/images/cmd_createlink_link.png""/></button><input type=""hidden"" value=""{0}/xhtml/customLink.html""/></li>", repositoryPath);
            build.AppendFormat(@"<li> <button class=""cmd_customlink srcMouseHover""><img alt=""Link"" src=""{0}/images/cmd_createlink_link.png""/></button><input type=""hidden"" value=""{1}""/></li>",
                repositoryPath, string.Format("{0}?list={1}&openinframe=1&notitle=1", wimPath, list.GUID)
                );

            if (Sushi.Mediakiwi.Data.Environment.Current["RICHTEXT_FONTCOLOR"] == "1")
            {
                build.AppendFormat(@"
			<li>&nbsp;<button class=""toggleNextNode link family_0 srcMouseHover"" title=""Font Color""><img alt=""Font Color"" src=""{0}/images/cmd_forecolor_link.png""/></button>
				<div class=""hideThisNode""><p>
						<button class=""cmd_forecolor"" value=""aqua"" style=""background-color:aqua;""></button><button class=""cmd_forecolor"" value=""black"" style=""background-color:black;""></button><button class=""cmd_forecolor"" value=""blue"" style=""background-color:blue;""></button><button class=""cmd_forecolor"" value=""fuchsia"" style=""background-color:fuchsia;""></button><button class=""cmd_forecolor"" value=""gray"" style=""background-color:gray;""></button><button class=""cmd_forecolor"" value=""green"" style=""background-color:green;""></button><button class=""cmd_forecolor"" value=""lime"" style=""background-color:lime;""></button><button class=""cmd_forecolor"" value=""maroon"" style=""background-color:maroon;""></button>
						<br/>
						<button class=""cmd_forecolor"" value=""navy"" style=""background-color:navy;""></button><button class=""cmd_forecolor"" value=""olive"" style=""background-color:olive;""></button><button class=""cmd_forecolor"" value=""purple"" style=""background-color:purple;""></button><button class=""cmd_forecolor"" value=""red"" style=""background-color:red;""></button><button class=""cmd_forecolor"" value=""silver"" style=""background-color:silver;""></button><button class=""cmd_forecolor"" value=""teal"" style=""background-color:teal;""></button><button class=""cmd_forecolor"" value=""white"" style=""background-color:white;""></button><button class=""cmd_forecolor"" value=""yellow"" style=""background-color:yellow;""></button>
					</p>
					<p><input type=""text"" value=""#"" class=""colourValue""/><button class=""cmd_forecolor2"" title=""Font Color""><img alt=""Font Color"" src=""{0}/images/cmd_insert_link.png""/></button></p>
				</div>
			</li>
"
                    , repositoryPath);
            }

            build.AppendFormat(@"<li> <button class=""cmd_clean srcMouseHover""><img alt=""Clean"" src=""{0}/images/cmd_clean_link.png""/></button></li>", repositoryPath);
            build.AppendFormat(@"</ul>");
            build.AppendFormat(@"<ul class=""controls textControls""><li><button class=""cmd_toggle srcMouseHover"" title=""HTML-mode""><img alt=""HTML-mode"" src=""{0}/images/cmd_toggleHtml_link.png""/></button></li><li><button class=""cmd_bigger srcMouseHover"" title=""Resize""><img alt=""Resize"" src=""{0}/images/cmd_bigger_link.png""/></button></li></ul>", repositoryPath);



            build.AppendFormat(@"<iframe width=""520"" frameborder=""no"" scrolling=""auto"" allowtransparency=""true"" src=""about:blank""></iframe><editorHtml></root>", repositoryPath);
            //build.AppendFormat(@"<iframe width=""520"" height=""128"" frameborder=""no"" scrolling=""auto"" allowtransparency=""true""></iframe><editorHtml></root>", repositoryPath);
            return build.ToString(); //.Replace("&", "&amp;");

            //		<li><button class=""cmd_toggle srcMouseHover""><img alt=""HTML"" src=""{0}/images/cmd_toggleText_link.png""/></button></li>
            //		<li><button class=""cmd_bigger srcMouseHover""><img alt=""Resize"" src=""{0}/images/cmd_bigger_link.png""/></button></li>
        }
    }
}
