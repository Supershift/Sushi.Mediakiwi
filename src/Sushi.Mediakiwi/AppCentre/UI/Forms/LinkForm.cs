using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.UI;

namespace Sushi.Mediakiwi.AppCentre.UI.Forms
{
    public class LinkForm : FormMap<Link>
    {
        public LinkForm(Link link)
        {
            Load(link);

            Map(x => x.Text).TextField("Text", 500, false, false, "If no title is applied the external URL or linktext of the page will be used.").Show(HasCustomTitle);
            Map(x => x.Alt).TextField("Description", 500, false, false, "The alternative (alt) text of the link");
            Map(x => x.Target).Dropdown("Open in", nameof(TargetOptions));
            Map(x => x.Type, this).Radio("Type", nameof(LinkTypeOptions), "typedef", true, true);

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
        public ListItemCollection TargetOptions
        {
            get
            {
                ListItemCollection col = new ListItemCollection();
                col.Add(new ListItem("Same screen", "1"));
                col.Add(new ListItem("New screen", "2"));
                col.Add(new ListItem("Parent screen (from layer)", "4"));
                return col;
            }
        }

        /// <summary>
        /// Gets the bool choice.
        /// </summary>
        /// <value>The bool choice.</value>
        public new ListItemCollection LinkTypeOptions
        {
            get
            {
                ListItemCollection col = new ListItemCollection();
                col.Add(new ListItem("External", $"{(int)LinkType.ExternalUrl}") { Selected = true });
                col.Add(new ListItem("Internal", $"{(int)LinkType.InternalPage}"));
                col.Add(new ListItem("Document", $"{(int)LinkType.InternalAsset}"));
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

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Link"/> is apply.
        /// </summary>
        /// <value><c>true</c> if apply; otherwise, <c>false</c>.</value>
        [Framework.ContentListItem.Button("Apply hyperlink", true, IconTarget = ButtonTarget.TopLeft, IconType = ButtonIconType.Approve)]
        public bool Apply { get; set; }

        #endregion List attributes
    }
}
