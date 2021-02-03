using System;
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
    [ToolboxData("<{0}:WimLabel runat='server'></{0}:WimLabel>")]
    public class WimLabel : Base.ContentInfo
    {
        /// <summary>
        /// CTor
        /// </summary>
        public WimLabel() {
            this.Load += new EventHandler(WimLabel_Load);
        }

        void WimLabel_Load(object sender, EventArgs e)
        {
            if (HasReflection)
            {
                if (GetInstance().IsNull)
                {
                    ApplyData(this.MaxLength, this.Text, (int)Sushi.Mediakiwi.Framework.ContentType.TextField);
                }
                else
                    this.Text = GetInstance().Value;
            }
        }

        string m_Text;
        /// <summary>
        /// 
        /// </summary>
        public string Text
        {
            get { return m_Text; }
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

        string m_AssociatedControlID;
        /// <summary>
        /// 
        /// </summary>
        public string AssociatedControlID
        {
            get { return m_AssociatedControlID; }
            set { m_AssociatedControlID = value; }
        }

        int m_MaxLength;
        /// <summary>
        /// 
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

        string m_CssClass;
        /// <summary>
        /// Interactive help text
        /// </summary>
        public string CssClass
        {
            get { return m_CssClass; }
            set { m_CssClass = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {


            //base.Render(writer);
            if (Text != null)
            {
                if (m_CssClass != null) m_CssClass = string.Format(" class=\"{0}\"", m_CssClass);

                if (string.IsNullOrEmpty(AssociatedControlID))
                    writer.Write(string.Format("<label id=\"{0}\"{1}>", this.ID, CssClass));
                else
                {
                    Control c = Parent.FindControl(AssociatedControlID);
                    if (c == null) writer.Write(string.Format("<label id=\"{0}\"{1}>", this.ID, CssClass));
                    else
                        writer.Write(string.Format("<label id=\"{0}\" for=\"{1}\"{2}>", this.ID, c.ClientID, CssClass));
                }
                if (ApplyFormat) writer.Write(string.Format(InnerText, Text));
                else
                    writer.Write(Text);

                writer.Write("</label>");

                return;               
            }
        }
    }
}