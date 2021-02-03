using System;
using System.Web;
using System.Web.UI;

namespace Sushi.Mediakiwi.Framework.ControlLib
{
    /// <summary>
    /// Represents a WimButton entity.
    /// </summary>
    public class WimButton : System.Web.UI.Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WimButton"/> class.
        /// </summary>
        public WimButton()
        {
            this.Load += new EventHandler(WimButton_Load);
        }

        void WimButton_Load(object sender, EventArgs e)
        {
            if (System.Web.HttpContext.Current == null) return;

            if (System.Web.HttpContext.Current.Request.Form[this.UniqueID] != null)
            {
                if (Click != null) Click(this, e);
            }
        }

        /// <summary>
        /// Occurs when [click].
        /// </summary>
        public event EventHandler Click;

        int m_RepeaterItemReferenceID;
        /// <summary>
        /// Gets or sets the repeater item reference ID.
        /// </summary>
        /// <value>The repeater item reference ID.</value>
        public int RepeaterItemReferenceID
        {
            get { return m_RepeaterItemReferenceID; }
            set { m_RepeaterItemReferenceID = value; }
        }

        string m_Text;
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get { return m_Text; }
            set { m_Text = value; }
        }

        string m_CssClass;
        /// <summary>
        /// Gets or sets the CSS class.
        /// </summary>
        /// <value>The CSS class.</value>
        public string CssClass
        {
            get { return m_CssClass; }
            set { m_CssClass = value; }
        }

        string m_FormId;

        /// <summary>
        /// Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"></see> object, which writes the content to be rendered on the client.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"></see> object that receives the server control content.</param>
        protected override void Render(HtmlTextWriter writer)
        {
            //if (Context.Request.Browser.Browser.Equals("ie", StringComparison.OrdinalIgnoreCase) && Context.Request.Browser.MajorVersion == 6)
            //{
            //    writer.WriteBeginTag("input");
            //    writer.WriteAttribute("id", this.ClientID);
            //    writer.WriteAttribute("name", this.UniqueID);
            //    if (!string.IsNullOrEmpty(m_CssClass)) writer.WriteAttribute("class", m_CssClass);
            //    writer.WriteAttribute("type", "submit");
            //    writer.WriteAttribute("value", this.Text);
            //    writer.Write(" />");
            //    return;
            //}

            writer.WriteBeginTag("button");
            writer.WriteAttribute("id", this.ClientID);
            writer.WriteAttribute("name", this.UniqueID);
            if (!string.IsNullOrEmpty(m_CssClass)) writer.WriteAttribute("class", m_CssClass);
            writer.WriteAttribute("type", "submit");
            writer.WriteAttribute("value", "1");

            writer.Write(">");
            if (!string.IsNullOrEmpty(this.Text))
            {
                this.Controls.Clear();
                this.Controls.Add(new LiteralControl(this.Text));
            }
            if (HasControls())
                this.RenderChildren(writer);
            writer.WriteEndTag("button");
        }
    }
}
