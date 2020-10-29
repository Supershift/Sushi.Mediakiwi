using System;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Data;
using System.Collections;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Sushi.Mediakiwi.Framework.ControlLib
{
    /// <summary>
    /// 
    /// </summary>
    [ToolboxData("<{0}:WimText runat='server'></{0}:WimText>")]
    public class WimText : Base.ContentInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// 
            /// </summary>
            TextLine = 9,
            /// <summary>
            /// 
            /// </summary>
            TextField = 10,
            /// <summary>
            /// 
            /// </summary>
            TextArea = 11,
            /// <summary>
            /// 
            /// </summary>
            RichText = 12
        }

        /// <summary>
        /// CTor
        /// </summary>
        public WimText()
        {
            this.Init += new EventHandler(WimText_Init);
            this.Load += new EventHandler(WimText_Load);
        }

        void WimText_Init(object sender, EventArgs e)
        {
            if (HasReflection)
            {
                Sushi.Mediakiwi.Data.CustomDataItem instance = GetInstance();
                if (instance.IsNull)
                {
                    if ((int)this.TextMode == 0)
                        this.TextMode = Type.TextField;

                    ApplyData(this.MaxLength, this.Text, (int)this.TextMode);
                }
                else
                {
                    string data = GetInstance().Value;
                    this.Text = data;
                }
            }
        }

        void WimText_Load(object sender, EventArgs e)
        {

        }


        internal Sushi.Mediakiwi.Data.Component _Component { get; set; }

        string m_Text;
        /// <summary>
        /// 
        /// </summary>
        public string Text
        {
            get
            {
                return m_Text;
            }
            set { m_Text = value; }
        }



        string m_ValidationExpression;
        /// <summary>
        /// Must match regular expression
        /// </summary>
        public string ValidationExpression
        {
            get { return m_ValidationExpression; }
            set { m_ValidationExpression = value; }
        }

        Type m_TextMode;
        /// <summary>
        /// Type of text input
        /// </summary>
        public Type TextMode
        {
            get { return m_TextMode; }
            set { m_TextMode = value; }
        }

        int m_MaxLength;
        /// <summary>
        /// Type of text input
        /// </summary>
        public int MaxLength
        {
            get { return m_MaxLength; }
            set { m_MaxLength = value; }
        }

        bool m_ApplyFormat;
        /// <summary>
        /// Apply format replacement: {0}
        /// </summary>
        public bool ApplyFormat
        {
            get { return m_ApplyFormat; }
            set { m_ApplyFormat = value; }
        }

        public bool EnableTable { get; set; }

        //bool m_NoLineBreakConversion;
        ///// <summary>
        ///// Apply format replacement: {0}
        ///// </summary>
        //public bool NoLineBreakConversion
        //{
        //    get { return m_NoLineBreakConversion; }
        //    set { m_NoLineBreakConversion = value; }
        //}

        bool m_ApplyParagraphClean;
        ///// <summary>
        ///// Clear the paragraph wrapper. This is automatically added when applying RichText editor.
        ///// </summary>
        /// <summary>
        /// Gets or sets a value indicating whether all paragraph tags are removed from the text property of the control. Paragraph endings are replaced by linebreaks. 
        /// Only applies to RichText textmode. Default value is false.
        /// </summary>
        public bool ApplyParagraphClean
        {
            get { return m_ApplyParagraphClean; }
            set { m_ApplyParagraphClean = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the complete content of the text property is wrapped in a paragraph tag.
        /// Only applies to RichText textmode. Default value is false.
        /// </summary>
        public bool NoParagraphWrapper { get; set; }


        internal void CleanTable()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            if (Text != null)
            {
                var candidate = this.Text;
                var tableHandler = Data.Environment.GetInstance<IHtmlTableParser>();

                Data.Page page = null;
                if (HttpContext.Current.Items["Wim.Page"] != null)
                {
                    page = (Sushi.Mediakiwi.Data.Page)HttpContext.Current.Items["Wim.Page"];
                }
                // [CB; 22-1-2016; dit stond uit en daarom had youfone niet meer mooie tabellen.)
                if (Sushi.Mediakiwi.Framework.ContentInfoItem.RichTextAttribute._table != null)
                {
                    candidate = Sushi.Mediakiwi.Framework.ContentInfoItem.RichTextAttribute._table.Replace(candidate, delegate (Match m)
                    {
                        return tableHandler.ParseData(page, _Component, this.ID, m.Value);
                    });
                }   


                if (TextMode == Type.RichText && !ApplyParagraphClean)
                {
                    if (ApplyFormat)
                    {
                        if (string.IsNullOrEmpty(InnerText))
                            writer.Write(ExtendParagraphWrap(candidate));
                        else
                            writer.Write(ExtendParagraphWrap(string.Format(InnerText, candidate)));
                    }
                    else
                        writer.Write(ExtendParagraphWrap(candidate));
                }
                //else if (TextMode == Type.MultiField)
                //{
                //    var multiFieldHandler = Sushi.Mediakiwi.Data.Environment.GetInstance<IMultiFieldParser>();
                //    string value = multiFieldHandler.WriteHTML(MultiField.GetDeserialized(candidate), page);

                //    if (ApplyFormat)
                //    {
                //        if (string.IsNullOrEmpty(InnerText))
                //            writer.Write(ExtendParagraphWrap(value));
                //        else
                //            writer.Write(ExtendParagraphWrap(string.Format(InnerText, value)));
                //    }
                //    else
                //        writer.Write(ExtendParagraphWrap(value));
                //}
                else
                {
                    if (ApplyFormat)
                    {
                        if (string.IsNullOrEmpty(InnerText))
                            writer.Write(candidate);
                        else
                            writer.Write(string.Format(InnerText, candidate));
                    }
                    else
                        writer.Write(candidate);
                }
                return;
            }
        }

        /// <summary>
        /// Gets or sets the table tag.
        /// </summary>
        /// <value>
        /// The table tag.
        /// </value>
        public string TableTag { get; set; }
        /// <summary>
        /// Gets or sets the table row tag even.
        /// </summary>
        /// <value>
        /// The table row tag even.
        /// </value>
        public string TableRowTagEven { get; set; }
        /// <summary>
        /// Gets or sets the table row tag odd.
        /// </summary>
        /// <value>
        /// The table row tag odd.
        /// </value>
        public string TableRowTagOdd { get; set; }
        /// <summary>
        /// Gets or sets the table row first cell.
        /// </summary>
        /// <value>
        /// The table row first cell.
        /// </value>
        public string TableRowFirstCell { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [convert table head cell to TH].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [convert table head cell to TH]; otherwise, <c>false</c>.
        /// </value>
        public bool ConvertTableHeadCellToTH { get; set; }

        string CleanParagraphWrap(string input)
        {
            return Wim.Utility.CleanParagraphWrap(input);
        }

        string ExtendParagraphWrap(string input)
        {
            if (string.IsNullOrEmpty(input)) return null;
            if (!input.StartsWith("<p>", StringComparison.OrdinalIgnoreCase))
                input = string.Concat("<p>", input, "</p>");
            return input;
        }
    }
}