using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.UI;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace Sushi.Mediakiwi.AppCentre.UI.Forms
{
    public class LinkForm : FormMap<Link>
    {
        public LinkForm(Link link)
        {
            Load(link);

            Map(x => x.Text).TextField("Text", 500, false, false, "If no title is applied the external URL or linktext of the page will be used.").Show(HasCustomTitle);
            Map(x => x.Alt).TextField("Description", 500, false, false, "The alternative (alt) text of the link");
            Map(x => x.Target).Dropdown("Open in", "ListChoice");
            Map<LinkForm>(x => x.Type, this).Radio("Type", "BoolChoice", "typedef", true, true);

            Map(x => x.AssetID).Document("Document", true);
            Map(x => x.PageID).PageSelect("Internal URL", true);
            Map(x => x.ExternalUrl).TextField("External URL", 500, true, false, "Any external URL");
        }

        public override void Evaluate()
        {
            Find(x => x.Text).Show(HasCustomTitle);
            Find(x => x.AssetID).Show(IsInternalDoc);
            Find(x => x.PageID).Show(IsInternalLink);
            Find(x => x.ExternalUrl).Show(IsExternalLink);

            base.Evaluate();
        }

        public int Type { get; set; }


        #region List attributes
        /// <summary>
        /// Gets the list choice.
        /// </summary>
        /// <value>The list choice.</value>
        public ListItemCollection ListChoice
        {
            get
            {
                ListItemCollection col = new ListItemCollection();
                ListItem li;
                //col.Add(new ListItem("< select a target >", ""));
                li = new ListItem("Same screen", "1");
                col.Add(li);
                li = new ListItem("New screen", "2");
                col.Add(li);
                li = new ListItem("Parent screen (from layer)", "4");
                col.Add(li);
                return col;
            }
        }

        /// <summary>
        /// Gets the bool choice.
        /// </summary>
        /// <value>The bool choice.</value>
        public new ListItemCollection BoolChoice
        {
            get
            {
                ListItemCollection col = new ListItemCollection();
                col.Add(new ListItem("External", "2") { Selected = true });
                col.Add(new ListItem("Internal", "1") { });
                col.Add(new ListItem("Document", "3") { });
                return col;
            }
        }
        

        /// <summary>
        /// Gets a value indicating whether this instance has custom title.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has custom title; otherwise, <c>false</c>.
        /// </value>
        public bool HasCustomTitle
        {
            get { return !RichTextInnerlink; }
        }

        public bool RichTextInnerlink { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is internal doc.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is internal doc; otherwise, <c>false</c>.
        /// </value>
        public bool IsInternalDoc { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is internal link.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is internal link; otherwise, <c>false</c>.
        /// </value>
        public bool IsInternalLink { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is external link.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is external link; otherwise, <c>false</c>.
        /// </value>
        public bool IsExternalLink { get; set; }

        private bool m_Apply;
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Link"/> is apply.
        /// </summary>
        /// <value><c>true</c> if apply; otherwise, <c>false</c>.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Button("Apply hyperlink", true, IconTarget = Sushi.Mediakiwi.Framework.ButtonTarget.TopLeft, IconType = Sushi.Mediakiwi.Framework.ButtonIconType.Approve)]
        public bool Apply
        {
            get { return m_Apply; }
            set { m_Apply = value; }
        }
        #endregion List attributes
    }
}
