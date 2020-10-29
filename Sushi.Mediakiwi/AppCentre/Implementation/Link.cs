using System;
using System.IO;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    /// <summary>
    /// 
    /// </summary>
    public class Link : BaseImplementation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Link"/> class.
        /// </summary>
        public Link()
        {
            wim.OpenInEditMode = true;
            wim.CanContainSingleInstancePerDefinedList = true;

            wim.ListDataDependendProperties.Add("PageSelect");
            wim.ListDataDependendProperties.Add("External");
            wim.HideOpenCloseToggle = true;

            this.ListLoad += new ComponentListEventHandler(Link_ListLoad);
            this.ListPreRender += new ComponentListEventHandler(Link_ListPreRender);
            this.ListSave += new ComponentListEventHandler(Link_ListSave);
            this.ListDelete += new ComponentListEventHandler(Link_ListDelete);
            this.ListAction += new ComponentActionEventHandler(Link_ListAction);
        }

        void Link_ListAction(object sender, ComponentActionEventArgs e)
        {
            if (IsPostBack)
            {
                bool hasError = false;
                switch (this.Type)
                {
                    case "1": hasError = !PageSelect.HasValue;
                        if (hasError)
                            wim.Notification.AddError("PageSelect", "Please apply all mandatory fields");
                        break;
                    case "2": hasError = string.IsNullOrEmpty(External);
                        if (hasError)
                            wim.Notification.AddError("External", "Please apply all mandatory fields");
                        break;
                    case "3": hasError = !DocumentSelect.HasValue;
                        if (hasError)
                            wim.Notification.AddError("DocumentSelect", "Please apply all mandatory fields");
                        break;
                }
            }
        }

        void Link_ListPreRender(object sender, ComponentListEventArgs e)
        {
            if (IsPostBack)
            {
                bool hasError = false;
                switch (this.Type)
                {
                    case "1": hasError = !PageSelect.HasValue; 
                        if (hasError)
                            wim.Notification.AddError("PageSelect", "Please apply all mandatory fields");
                        break;
                    case "2": hasError = string.IsNullOrEmpty(External);
                        if (hasError)
                            wim.Notification.AddError("External", "Please apply all mandatory fields");
                        break;
                    case "3": hasError = !DocumentSelect.HasValue;
                        if (hasError)
                            wim.Notification.AddError("DocumentSelect", "Please apply all mandatory fields");
                        break;
                }
            }
        }

        void Link_ListDelete(object sender, ComponentListEventArgs e)
        {
            //  Do not remove
            //Sushi.Mediakiwi.Data.Link link = Sushi.Mediakiwi.Data.Link.SelectOne(e.SelectedKey);
            //link.Delete();

            if (wim.ShowNewDesign2)
                wim.OnDeleteScript = string.Format("<script type=\"text/javascript\">{0}</script>", "parent.removeLink();");
            else
                wim.OnDeleteScript = string.Format("<script type=\"text/javascript\">{0}</script>", "remove();");
        }

        void Link_ListSave(object sender, ComponentListEventArgs e)
        {
            if (m_RichTextInnerlink) m_Title = "#";

            Sushi.Mediakiwi.Data.Link link = new Sushi.Mediakiwi.Data.Link();

            link.ID = e.SelectedKey;
            link.Text = m_Title;
            link.Target = Wim.Utility.ConvertToInt(m_Target);
            link.IsInternal = (m_Type == "1");

            link.ExternalUrl = null;
            link.PageID = null;
            link.AssetID = null;

            switch (this.Type)
            {
                case "1": link.PageID = PageSelect; break;
                case "2": link.ExternalUrl = External; break;
                case "3": link.AssetID = DocumentSelect; break;
            }

            link.Alt = Wim.Utility.CleanLineFeed(m_Alt, false);

            if (string.IsNullOrEmpty(link.Text))
            {
                switch (this.Type)
                {
                    case "1":
                        Sushi.Mediakiwi.Data.Page page = Sushi.Mediakiwi.Data.Page.SelectOne(link.PageID.Value, false);
                        link.Text = page.LinkText; 
                        break;
                    case "2": 
                        link.Text = External; 
                        break;
                    case "3":
                        Sushi.Mediakiwi.Data.Asset asset = Sushi.Mediakiwi.Data.Asset.SelectOne(link.AssetID.Value);
                        link.Text = asset.Title; 
                        break;
                }
            }

            link.Save();

            string url = link.ExternalUrl;
            if (link.IsInternal)
            {
                if (link.PageID.HasValue)
                {
                    Sushi.Mediakiwi.Data.Page page = Sushi.Mediakiwi.Data.Page.SelectOne(link.PageID.Value, false);
                    if (page != null)
                        url = string.Format("{0}{1}.aspx", page.CompletePath, page.Name);
                }
            }



            if (RichTextInnerlink)
            {
                if (Request.QueryString["referid"] == "tmce")
                {
                    if (wim.ShowNewDesign2)
                        wim.OnSaveScript = string.Format("<script type=\"text/javascript\">{0}</script>", string.Format("parent.insertLink('wim:{0}');", link.ID));
                    else
                        wim.OnSaveScript = string.Format("<script type=\"text/javascript\">{0}</script>", string.Format("insert('wim:{0}');", link.ID));

                }
                else
                {
                    wim.OnSaveScript = string.Format(@"<div class=""customLink cmd_createlink now_yes""><input type=""hidden"" id=""{0}"" value=""wim:{0}"" /></div>", link.ID);
                }
            }
            else
            {
                if (wim.Console.IsNewDesign)
                {
                    wim.OnSaveScript = string.Format(@"<input type=""hidden"" class=""postparent"" id=""{0}"" value=""<a title='url: {2}'>{1}</a> (open in {3})"" />", link.ID, link.Text, link.GetUrl(wim.CurrentSite, false), link.TargetInfo);
                }
                else
                    wim.OnSaveScript = string.Format(@"<div class=""textOptionSelect now_yes""><input type=""hidden"" id=""{0}"" value=""<a title='url: {2}'>{1}</a> (open in {3})"" /></div>", link.ID, link.Text, link.GetUrl(wim.CurrentSite, false), link.TargetInfo);
            }
        }

        private bool m_RichTextInnerlink;
        /// <summary>
        /// Gets or sets a value indicating whether [rich text innerlink].
        /// </summary>
        /// <value><c>true</c> if [rich text innerlink]; otherwise, <c>false</c>.</value>
        public bool RichTextInnerlink
        {
            get { return m_RichTextInnerlink; }
            set { m_RichTextInnerlink = value; }
        }

        private Sushi.Mediakiwi.AppCentre.Data.Supporting.Section m_environment;
        void Link_ListLoad(object sender, ComponentListEventArgs e)
        {
            if (wim.Console.IsNewDesign)
                wim.SetPropertyVisibility("Apply", false);

            if (Request.QueryString["referid"] == "tmce")
            {
                if (false)//!wim.CurrentApplicationUser.ShowNewDesign2)
                {
                    wim.OnLoadScript = string.Format("<script type=\"text/javascript\" src=\"{0}\"></script>", Wim.Utility.AddApplicationPath("repository/wim/scripts/rte/tiny_mce_popup.js"));
                    wim.OnLoadScript += string.Format("<script type=\"text/javascript\" src=\"{0}\"></script>", Wim.Utility.AddApplicationPath("repository/wim/scripts/rte/plugins/advlink/js/advlink.js"));
                }
            }

            if (Request.Params["notitle"] != null && Request.Params["notitle"] == "1")
                m_RichTextInnerlink = true;

            m_environment = (Sushi.Mediakiwi.AppCentre.Data.Supporting.Section)Wim.Utility.ConvertToInt(Request.Params["env"]);

            if (IsPostBack)
            {
                if (Request.Params["Type"] == "1")
                    this.IsInternalLink = true;
                else if (Request.Params["Type"] == "2")
                    this.IsExternalLink = true;
                else if (Request.Params["Type"] == "3")
                    this.IsInternalDoc = true;
            }
            else
            {
                if (e.SelectedKey == 0)
                {
                    this.IsInternalLink = true;
                    return;
                }
            }
            if (e.SelectedKey == 0)
                return;

            Sushi.Mediakiwi.Data.Link link = Sushi.Mediakiwi.Data.Link.SelectOne(e.SelectedKey);
            if (link == null || link.IsNewInstance)
            {
                return;
            }

            if (!IsPostBack)
            {
                switch (link.Type)
                {
                    case Sushi.Mediakiwi.Data.Link.LinkType.ExternalUrl:
                        this.Type = "2";
                        this.IsExternalLink = true;
                        break;
                    case Sushi.Mediakiwi.Data.Link.LinkType.InternalAsset:
                        this.Type = "3";
                        this.IsInternalDoc = true;
                        break;
                    default:
                        this.Type = "1";
                        this.IsInternalLink = true;
                        break;
                }
            }

            m_Title = link.Text;
            m_Target = link.Target.ToString();

            this.m_External = link.ExternalUrl;
            this.PageSelect = link.PageID;
            this.DocumentSelect = link.AssetID;

            m_Alt = link.Alt;


        }



        #region List attributes

        private string m_Title;
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("HasCustomTitle")]
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Link text", 500, false, "If no title is applied the external URL or linktext of the page will be used.")]
        public string Title
        {
            get { return m_Title; }
            set { m_Title = value; }
        }

        private string m_Target;
        /// <summary>
        /// Gets or sets the target.
        /// </summary>
        /// <value>The target.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Open in", "ListChoice", true)]
        public string Target
        {
            get { return m_Target; }
            set { m_Target = value; }
        }

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

                if (Wim.CommonConfiguration.HAS_POPUPLAYER_OPTION)
                {
                    li = new ListItem("Popup layer ", "3");
                    col.Add(li);
                }
                return col;
            }
        }

        private string m_Type = "1";
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Radio("Type", "BoolChoice", "typedef", true, true)]
        public string Type
        {
            get { return m_Type; }
            set { m_Type = value; }
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
                ListItem li;
                li = new ListItem("Internal", "1");
                li.Selected = true;
                col.Add(li);
                li = new ListItem("External", "2");
                col.Add(li);
                li = new ListItem("Document", "3");
                col.Add(li);
                return col;
            }
        }


        /// <summary>
        /// Gets or sets the document select.
        /// </summary>
        /// <value>The document select.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsInternalDoc")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Binary_Document("Document", false)]
        public int? DocumentSelect { get; set; }

        /// <summary>
        /// Gets or sets the page select.
        /// </summary>
        /// <value>The page select.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsInternalLink")]
        [Sushi.Mediakiwi.Framework.ContentListItem.PageSelect("Internal URL", false)]
        public int? PageSelect { get; set; }

        private string m_External = "http://";
        /// <summary>
        /// Gets or sets the external.
        /// </summary>
        /// <value>The external.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsExternalLink")]
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("External URL", 500, true)]
        public string External
        {
            get { return m_External; }
            set { m_External = value; }
        }

        private string m_Alt;
        /// <summary>
        /// Gets or sets the alt.
        /// </summary>
        /// <value>The alt.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Link description", 500, false)]
        public string Alt
        {
            get { return m_Alt; }
            set { m_Alt = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has custom title.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has custom title; otherwise, <c>false</c>.
        /// </value>
        public bool HasCustomTitle
        {
            get { return !m_RichTextInnerlink; }
        }

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
