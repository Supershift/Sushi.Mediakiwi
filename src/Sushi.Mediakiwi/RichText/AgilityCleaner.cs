using HtmlAgilityPack;
using System;
using System.Linq;

namespace Sushi.Mediakiwi.RichRext
{
    /// <summary>
    /// 
    /// </summary>
    public class AgilityCleaner
    {
        /// <summary>
        /// 
        /// </summary>
        readonly string[] m_AllowedElements = new string[] { 
            "p", 
            //"div", 
            //"span", 
            "br", "a", "hr", "img", "figure",  
            "ol", "ul", "li", 
            "table", "tr", "td", "tbody", "thead", "th", 
            "b", "strong", "em", "i", "u", "sub", "sup", "blockquote", "strike" };

        /// <summary>
        /// 
        /// </summary>
        readonly string[][] m_AllowedElementAttributes = new string[][] { 
            //new string[] { "div", "class" }, 
            new string[] { "a", "href", },
            new string[] { "a", "title" },
            new string[] { "a", "target" },
            new string[] { "ul", "style", "list-style-type: circle;" },
            new string[] { "ul", "style", "list-style-type: disc;" },
            new string[] { "ul", "style", "list-style-type: square;" },
            new string[] { "ol", "style", "list-style-type: lower-alpha;" },
            new string[] { "ol", "style", "list-style-type: lower-greek;" },
            new string[] { "ol", "style", "list-style-type: lower-roman;" },
            new string[] { "ol", "style", "list-style-type: upper-alpha;" },
            new string[] { "ol", "style", "list-style-type: upper-roman;" },
            new string[] { "span", "style", "font-weight:bold;" },
            new string[] { "span", "style", "text-decoration:underline;" },
            new string[] { "img", "src" },
            new string[] { "img", "data-id" },
            new string[] { "img", "alt" },
            new string[] { "img", "style" },
            new string[] { "img", "width" },
            new string[] { "img", "height" },
            new string[] { "figure", "class", "left" },
            new string[] { "figure", "class", "center" },
            new string[] { "figure", "class", "right" }
        };

        private CleanupRichTextButtons _stylingCleaner;
        /// <summary>
        /// Gets the styling cleaner.
        /// </summary>
        protected CleanupRichTextButtons StylingCleaner
        {
            get
            {
                if (_stylingCleaner == null)
                {
                    _stylingCleaner = new CleanupRichTextButtons();
                }
                return _stylingCleaner;
            }
        }

        /// <summary>
        /// Cleans the asset URL.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <returns></returns>
        public string CleanAssetUrl(string html)
        {
            if (string.IsNullOrEmpty(html))
                return html;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            CleanAssetUrl(doc);

            System.Text.StringBuilder build = new System.Text.StringBuilder();
            using (System.IO.StringWriter x = new System.IO.StringWriter(build))
            {
                doc.Save(x);
                //using (HtmlTextWriter writer2 = new HtmlTextWriter(x))
                //{
                    x.Flush();
                    string candidate = build.ToString();
                    return candidate;
                //}
            }
        }

        /// <summary>
        /// Cleans the asset URL.
        /// </summary>
        /// <param name="doc">The doc.</param>
        void CleanAssetUrl(HtmlDocument doc)
        {
            var nodes = doc.DocumentNode.SelectNodes("//img");
            if (nodes == null) 
                return;
            foreach (HtmlNode node in nodes)
            {
                var src = node.Attributes["src"];
                if (src != null)
                {
                    string document = node.Attributes["src"].Value;
                    if (!string.IsNullOrEmpty(document))
                    {
                        if (document.Contains("doc.ashx?"))
                        {
                            string key = document.Split('?')[1];
                            //21-10-20:TODO
                            //node.Attributes["src"].Value = Utils.AddApplicationPath(string.Concat("doc.ashx?", key));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Cleans the HTML.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <returns></returns>
        public string CleanHTML(string html)
        {
            HtmlDocument doc = new HtmlDocument();

            if (!string.IsNullOrEmpty(html))
            {
                html = StylingCleaner.ApplyBold(html);
                html = StylingCleaner.ApplyUnderline(html);
                html = StylingCleaner.ApplyItalic(html);
                html = html
                    .Replace("<p>", string.Empty)
                    .Replace("</p>", "<br>")
                    .Replace("<div>", string.Empty)
                    .Replace("</div>", "<br>")
                    ;

                while (html.Contains("&nbsp;&nbsp;"))
                {
                    html = html.Replace("&nbsp;&nbsp;", "&nbsp;");
                }
            }

            doc.LoadHtml(html);

            CleanAssetUrl(doc);
            NodeParser(doc.DocumentNode.ChildNodes);

            System.Text.StringBuilder build = new System.Text.StringBuilder();

            string cleanText = string.Empty;
            using (System.IO.StringWriter x = new System.IO.StringWriter(build))
            {
                doc.Save(x);
                x.Flush();

                //using (HtmlTextWriter writer2 = new HtmlTextWriter(x))
                //{
                //    writer2.Flush();
                    cleanText = build.ToString();
                    //candidate = StylingCleaner.ApplyBold(candidate);
                    //candidate = StylingCleaner.ApplyUnderline(candidate);
                    //candidate = StylingCleaner.ApplyItalic(candidate);

                    cleanText = cleanText
                        .Replace("<remove>", string.Empty)
                        .Replace("</remove>", string.Empty);
                //}
            }

            doc.LoadHtml(cleanText);
            if (StripParserErrors(doc, ref html))
            {
                return html;
            }
            return cleanText;
        }

        /// <summary>
        /// Strips the parser errors.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="html">The HTML.</param>
        /// <returns></returns>
        bool StripParserErrors(HtmlDocument doc, ref string html)
        {
            int minus = 0;
            foreach (var item in doc.ParseErrors)
            {
                int begin = item.StreamPosition - minus;
                try
                {
                    int ending = html.IndexOf(">", item.StreamPosition) + 1 - minus;

                    //  To reflect the position change
                    minus = ending - begin;
                    //Response.Write(HtmlAgilityPack.HtmlDocument.HtmlEncode(html.Substring(begin, ending)));

                    string part1 = html.Substring(0, begin);
                    string part2 = html.Substring(ending, html.Length - ending);

                    html = string.Concat(part1, part2);
                }
                catch (Exception)
                {
                }

            }
            return doc.ParseErrors.Count() > 0;
        }

        /// <summary>
        /// Parse the nodes
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        void NodeParser(HtmlNodeCollection nodes)
        {
            // Loop through the node collection
            for (int index = 0; index < nodes.Count; index++)
            {
                // Get the current node
                var node = nodes[index];

                // Recursive execute this function if the node has children
                if (node.HasChildNodes)
                    NodeParser(node.ChildNodes);

                // Check the node type for Element or Comment. The former must be parsed, the latter must be removed.
                if (node.NodeType == HtmlNodeType.Element)
                {
                    // Remove the element if its not available in the allowedElements
                    var found = (from item in m_AllowedElements where item == node.Name select item).FirstOrDefault();

                    if (string.IsNullOrEmpty(found))
                    {
                        node.Name = "remove";
                        node.Attributes.RemoveAll();
                    }
                    else
                    {
                        // Find the options based on the node name (Multiple results can occur, item[0] being the node name)
                        var attributeOptions = (from item in m_AllowedElementAttributes where item[0] == node.Name select item);

                        // Remove all of them if not allowed
                        if (attributeOptions.Count() == 0)
                        {
                            node.Attributes.RemoveAll();
                        }
                        else
                        {
                            // Loop through each of the attributes of the current node
                            for (int index2 = 0; index2 < node.Attributes.Count; index2++)
                            {
                                // Get the checklist for the current attribute (item[1] being the attribute name)
                                var checkList = (from item in attributeOptions where item[1] == node.Attributes[index2].Name select item).ToList();

                                // Remove if node checklists where found
                                if (checkList == null || checkList.Count() == 0)
                                {
                                    node.Attributes[index2].Remove();
                                    index2--;
                                }
                                else
                                {
                                    // Validate if the there is only one checklist
                                    // or if the checlist has only 1 item present (being the attribute name)
                                    if (checkList.Count == 1 && checkList[0].Count() == 2)
                                    {
                                        //  Do Nothing
                                    }
                                    else
                                    {
                                        try
                                        {
                                            // Validate the attribute value if present in the m_AllowedElementAttributes (item[2] being the attr value)
                                            var findType = (from item in checkList where item[2] == node.Attributes[index2].Value select item).FirstOrDefault();
                                            if (findType == null)
                                            {
                                                node.Attributes[index2].Remove();
                                                index2--;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            throw new Exception("Expected a third attributes validation tag (m_AlloweddElementAttributes)", ex);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (node.NodeType == HtmlNodeType.Comment)
                {
                    node.Remove();
                    index--;
                }
            }
        }
    }
}