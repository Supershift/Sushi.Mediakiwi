using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    /// <summary>
    /// Represents a Browsing entity.
    /// </summary>
    public partial class Browsing
    {       /// <summary>
        /// Represents a BrowseItem entity.
        /// </summary>
        public class BrowseItem
        {
            /// <summary>
            /// Gets or sets the type URL.
            /// </summary>
            /// <value>The type URL.</value>
            public string TypeUrl { get; set; }
            private int m_ID;
            /// <summary>
            /// Gets or sets the id.
            /// </summary>
            /// <value>The id.</value>
            public int ID
            {
                get { return m_ID; }
                set { m_ID = value; }
            }

            /// <summary>
            /// Gets or sets the ID text.
            /// </summary>
            /// <value>The ID text.</value>
            public string IDText { get; set; }

            private string m_Icon;
            /// <summary>
            /// Gets or sets the icon.
            /// </summary>
            /// <value>The icon.</value>
            public string Icon
            {
                get { return m_Icon; }
                set { m_Icon = value; }
            }

            private string m_HiddenField = "";
            /// <summary>
            /// Gets or sets the hidden field.
            /// </summary>
            /// <value>The hidden field.</value>
            public string HiddenField
            {
                get { return m_HiddenField; }
                set { m_HiddenField = value; }
            }

            private string m_Title;
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            /// <value>The title.</value>
            public string Title
            {
                get { return m_Title; }
                set { m_Title = value; }
            }

            private string m_Info1;
            /// <summary>
            /// Gets or sets the info1.
            /// </summary>
            /// <value>The info1.</value>
            public string Info1
            {
                get { return m_Info1; }
                set { m_Info1 = value; }
            }

            private string m_Info2;
            /// <summary>
            /// Gets or sets the info2.
            /// </summary>
            /// <value>The info2.</value>
            public string Info2
            {
                get { return m_Info2; }
                set { m_Info2 = value; }
            }

            private DateTime m_Info3;
            /// <summary>
            /// Gets or sets the info3.
            /// </summary>
            /// <value>The info3.</value>
            public DateTime Info3
            {
                get { return m_Info3; }
                set { m_Info3 = value; }
            }

            private string m_Info4;
            /// <summary>
            /// Gets or sets the info4.
            /// </summary>
            /// <value>The info4.</value>
            public string Info4
            {
                get { return m_Info4; }
                set { m_Info4 = value; }
            }

            private string m_PassThrough;
            /// <summary>
            /// Gets or sets the pass through.
            /// </summary>
            /// <value>The pass through.</value>
            public string PassThrough
            {
                get { return m_PassThrough; }
                set { m_PassThrough = value; }
            }

            public string EditLink { get; set; }
            public string DownloadLink { get; set; }
        }
    }
}
