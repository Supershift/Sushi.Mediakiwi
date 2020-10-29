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
    [ToolboxData("<{0}:WimMulti runat='server'></{0}:WimMulti>")]
    public class WimMulti : Base.ContentInfo
    {
        /// <summary>
        /// CTor
        /// </summary>
        public WimMulti() {
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
                    ApplyData(0, this.Text, (int)ContentType.MultiField);
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
            get {
                return m_Text;
            }
            set { m_Text = value; }
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

        public bool EnableTable { get; set;  }

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

                var multiFieldHandler = Sushi.Mediakiwi.Data.Environment.GetInstance<IMultiFieldParser>();
                string value = multiFieldHandler.WriteHTML(MultiField.GetDeserialized(candidate), page);

                if (ApplyFormat)
                {
                    if (string.IsNullOrEmpty(InnerText))
                        writer.Write(ExtendParagraphWrap(value));
                    else
                        writer.Write(ExtendParagraphWrap(string.Format(InnerText, value)));
                }
                else
                    writer.Write(ExtendParagraphWrap(value));
               
                return;
            }
        }

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