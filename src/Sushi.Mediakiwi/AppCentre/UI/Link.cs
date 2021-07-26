using Sushi.Mediakiwi.AppCentre.UI.Forms;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using System;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    /// <summary>
    /// 
    /// </summary>
    public class Link : BaseImplementation
    {
        public class PageHref
        {
            public string Text { get; set; }
            public string Href { get; set; }
        }

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

            ListLoad += Link_ListLoad;
            ListPreRender += Link_ListPreRender;
            ListSave += Link_ListSave;
            ListDelete += Link_ListDelete;
            ListAction += Link_ListAction;
            ListHeadless += Link_ListHeadless;
        }

        private async Task Link_ListHeadless(HeadlessRequest e)
        {
            var link = await Sushi.Mediakiwi.Data.Link.SelectOneAsync(e.Listitem.ID).ConfigureAwait(false);
            if (link == null || link.ID == 0)
                return;

            var linkref = new PageHref();

            linkref.Text = link.Text;

            if (link.IsInternal)
            {
                var pagelink = await Sushi.Mediakiwi.Data.Page.SelectOneAsync(link.PageID.GetValueOrDefault()).ConfigureAwait(false);
                linkref.Href = Sushi.Mediakiwi.Data.Utility.ConvertUrl(pagelink?.InternalPath);
            }
            else
                linkref.Href = link?.ExternalUrl;

            e.Result = linkref;
        }

        Task Link_ListAction(ComponentActionEventArgs e)
        {
            if (IsPostBack)
            {
                bool hasError = false;
                switch (Form.Type)
                {
                    case 1: hasError = !Implement.PageID.HasValue;
                        if (hasError)
                            wim.Notification.AddError("PageSelect", "Please apply all mandatory fields");
                        break;
                    case 2: hasError = string.IsNullOrEmpty(Implement.ExternalUrl);
                        if (hasError)
                            wim.Notification.AddError("External", "Please apply all mandatory fields");
                        break;
                    case 3: hasError = !Implement.AssetID.HasValue;
                        if (hasError)
                            wim.Notification.AddError("DocumentSelect", "Please apply all mandatory fields");
                        break;
                }
            }
            return Task.CompletedTask;
        }

        Task Link_ListPreRender(ComponentListEventArgs e)
        {
            if (IsPostBack)
            {
                bool hasError = false;
                switch (Form.Type)
                {
                    case 1: hasError = !Implement.PageID.HasValue; 
                        if (hasError)
                            wim.Notification.AddError("PageSelect", "Please apply all mandatory fields");
                        break;
                    case 2: hasError = string.IsNullOrEmpty(Implement.ExternalUrl);
                        if (hasError)
                            wim.Notification.AddError("External", "Please apply all mandatory fields");
                        break;
                    case 3: hasError = !Implement.AssetID.HasValue;
                        if (hasError)
                            wim.Notification.AddError("DocumentSelect", "Please apply all mandatory fields");
                        break;
                }
            }
            return Task.CompletedTask;
        }

        Task Link_ListDelete(ComponentListEventArgs e)
        {
            //  Do not remove
            //Sushi.Mediakiwi.Data.Link link = Sushi.Mediakiwi.Data.Implement.SelectOne(e.SelectedKey);
            //Implement.Delete();

            wim.OnDeleteScript = string.Format("<script type=\"text/javascript\">{0}</script>", "parent.removeLink();");
            return Task.CompletedTask;
        }

        async Task Link_ListSave(ComponentListEventArgs e)
        {
            if (Form.RichTextInnerlink) 
                Implement.Text = "#";

            if (Implement.ID == 0)
                Implement.Created = DateTime.UtcNow;

            Implement.IsInternal = (Form.Type == 1);

            Implement.Alt = Utility.CleanLineFeed(Implement.Alt, false);

            if (string.IsNullOrEmpty(Implement.Text))
            {
                switch (Form.Type)
                {
                    case 1:
                        Page page = await Page.SelectOneAsync(Implement.PageID.Value, false);
                        Implement.Text = page.LinkText; 
                        break;
                    //case 2: 
                    //    Implement.Text = Form.IsExternalLink; 
                    //    break;
                    case 3:
                        Asset asset = await Asset.SelectOneAsync(Implement.AssetID.Value);
                        Implement.Text = asset.Title; 
                        break;
                }
            }

            await Implement.SaveAsync();

            string url = Implement.ExternalUrl;
            if (Implement.IsInternal)
            {
                if (Implement.PageID.HasValue)
                {
                    Page page = await Page.SelectOneAsync(Implement.PageID.Value, false);
                    if (page != null)
                        url = string.Format("{0}{1}.aspx", page.CompletePath, page.Name);
                }
            }

            if (Form.RichTextInnerlink)
            {
                if (Request.Query["referid"] == "tmce")
                {
                    wim.OnSaveScript = string.Format("<script type=\"text/javascript\">{0}</script>", string.Format("parent.insertLink('wim:{0}');", Implement.ID));

                }
                else
                {
                    wim.OnSaveScript = string.Format(@"<div class=""customLink cmd_createlink now_yes""><input type=""hidden"" id=""{0}"" value=""wim:{0}"" /></div>", Implement.ID);
                }
            }
            else
            {
                 wim.OnSaveScript = string.Format(@"<input type=""hidden"" class=""postparent"" id=""{0}"" value=""<a title='url: {2}'>{1}</a> (open in {3})"" />", Implement.ID, Implement.Text, Implement.GetUrl(wim.CurrentSite, false), Implement.TargetInfo);
            }
        }

        LinkForm Form { get; set; }
        Mediakiwi.Data.Link Implement { get; set; }

        async Task Link_ListLoad(ComponentListEventArgs e)
        {
            Implement = await Mediakiwi.Data.Link.SelectOneAsync(e.SelectedKey);

            Form = new LinkForm(Implement);
            FormMaps.Add(Form);

            wim.SetPropertyVisibility("Apply", false);

            if (Request.Query["notitle"] == "1")
            {
                Form.RichTextInnerlink = true;
            }

            if (IsPostBack)
            {
                if (Form.Type == 1)
                {
                    Form.IsInternalLink = true;
                }
                else if (Form.Type == 2)
                {
                    Form.IsExternalLink = true;
                }
                else if (Form.Type == 3)
                {
                    Form.IsInternalDoc = true;
                }
            }
            else
            {
                if (e.SelectedKey == 0)
                {
                    Form.Type = 2;
                    Form.IsExternalLink = true;
                    Implement.ExternalUrl = "https://www.";
                    return;
                }
            }
            if (e.SelectedKey == 0)
                return;

            if (Implement == null || Implement.ID == 0)
            {
                return;
            }

            if (!IsPostBack)
            {
                switch (Implement.Type)
                {
                    case LinkType.ExternalUrl:
                        Form.Type = 2;
                        Form.IsExternalLink = true;
                        break;
                    case LinkType.InternalAsset:
                        Form.Type = 3;
                        Form.IsInternalDoc = true;
                        break;
                    default:
                        Form.Type = 1;
                        Form.IsInternalLink = true;
                        break;
                }
            }

            //m_Title = Implement.Text;
            //m_Target = Implement.Target.ToString();

            //this.m_External = Implement.ExternalUrl;
            //this.PageSelect = Implement.PageID;
            //this.DocumentSelect = Implement.AssetID;

            //m_Alt = Implement.Alt;
        }
    }
}
