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
            var link = await Mediakiwi.Data.Link.SelectOneAsync(e.Listitem.ID).ConfigureAwait(false);
            if (link == null || link.ID == 0)
            {
                return;
            }

            var linkref = new PageHref()
            {
                Text = link.Text
            };

            if (link.IsInternal)
            {
                var pagelink = await Page.SelectOneAsync(link.PageID.GetValueOrDefault()).ConfigureAwait(false);
                linkref.Href = Utility.ConvertUrl(pagelink?.InternalPath);
            }
            else
            {
                linkref.Href = link?.ExternalUrl;
            }

            e.Result = linkref;
        }

        Task Link_ListAction(ComponentActionEventArgs e)
        {
            if (IsPostBack)
            {
                bool hasError = false;
                switch (Form.Type)
                {
                    case (int)LinkType.InternalPage:
                        {
                            hasError = !Implement.PageID.HasValue;
                            if (hasError)
                            {
                                wim.Notification.AddError("PageSelect", "Please apply all mandatory fields");
                            }
                        }
                        break;
                    case (int)LinkType.ExternalUrl:
                        {
                            hasError = string.IsNullOrEmpty(Implement.ExternalUrl);
                            if (hasError)
                            {
                                wim.Notification.AddError("External", "Please apply all mandatory fields");
                            }
                        }
                        break;
                    case (int)LinkType.InternalAsset:
                        {
                            hasError = !Implement.AssetID.HasValue;
                            if (hasError)
                            {
                                wim.Notification.AddError("DocumentSelect", "Please apply all mandatory fields");
                            }
                        }
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
                    case (int)LinkType.InternalPage:
                        {
                            hasError = !Implement.PageID.HasValue;
                            if (hasError)
                            {
                                wim.Notification.AddError("PageSelect", "Please apply all mandatory fields");
                            }
                        }
                        break;
                    case (int)LinkType.ExternalUrl:
                        {
                            hasError = string.IsNullOrEmpty(Implement.ExternalUrl);
                            if (hasError)
                            {
                                wim.Notification.AddError("External", "Please apply all mandatory fields");
                            }
                        }
                        break;
                    case (int)LinkType.InternalAsset:
                        {
                            hasError = !Implement.AssetID.HasValue;
                            if (hasError)
                            {
                                wim.Notification.AddError("DocumentSelect", "Please apply all mandatory fields");
                            }
                        }
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
            {
                Implement.Text = "#";
            }

            if (Implement.ID == 0)
            {
                Implement.Created = DateTime.UtcNow;
            }

            Implement.IsInternal = (Form.Type == (int)LinkType.InternalPage);

            Implement.Alt = Utility.CleanLineFeed(Implement.Alt, false);

            if (string.IsNullOrEmpty(Implement.Text))
            {
                switch (Form.Type)
                {
                    case (int)LinkType.InternalPage:
                        {
                            Page page = await Page.SelectOneAsync(Implement.PageID.Value, false).ConfigureAwait(false);
                            Implement.Text = page.LinkText;
                        }
                        break;
                    //case 2: 
                    //    Implement.Text = Form.IsExternalLink; 
                    //    break;
                    case (int)LinkType.InternalAsset:
                        {
                            Asset asset = await Asset.SelectOneAsync(Implement.AssetID.Value).ConfigureAwait(false);
                            Implement.Text = asset.Title;
                        }
                        break;
                }
            }

            await Implement.SaveAsync();

            // [MR:19-07-2021] Can be removed ?
            //string url = Implement.ExternalUrl;
            //if (Implement.IsInternal && Implement.PageID.HasValue)
            //{
            //    Page page = await Page.SelectOneAsync(Implement.PageID.Value, false).ConfigureAwait(false);
            //    if (page != null)
            //    {
            //        url = string.Format("{0}{1}.aspx", page.CompletePath, page.Name);
            //    }
            //}

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
            Implement = await Mediakiwi.Data.Link.SelectOneAsync(e.SelectedKey).ConfigureAwait(false);

            Form = new LinkForm(Implement);
            FormMaps.Add(Form);

            wim.SetPropertyVisibility(nameof(LinkForm.Apply), false);

            if (Request.Query["referid"] == "tmce")
            {
                if (false)//!wim.CurrentApplicationUser.ShowNewDesign2)
                {
                    wim.OnLoadScript = string.Format("<script type=\"text/javascript\" src=\"{0}\"></script>", wim.AddApplicationPath("repository/wim/scripts/rte/tiny_mce_popup.js"));
                    wim.OnLoadScript += string.Format("<script type=\"text/javascript\" src=\"{0}\"></script>", wim.AddApplicationPath("repository/wim/scripts/rte/plugins/advlink/js/advImplement.js"));
                }
            }

            if (Request.Query["notitle"] == "1")
            {
                Form.RichTextInnerlink = true;
            }

            if (IsPostBack)
            {
                if (Form.Type == (int)LinkType.InternalPage)
                {
                    Form.IsInternalLink = true;
                }
                else if (Form.Type == (int)LinkType.ExternalUrl)
                {
                    Form.IsExternalLink = true;
                }
                else if (Form.Type == (int)LinkType.InternalAsset)
                {
                    Form.IsInternalDoc = true;
                }
            }
            else
            {
                if (e.SelectedKey == 0)
                {
                    Form.Type = (int)LinkType.ExternalUrl;
                    Form.IsExternalLink = true;
                    Implement.ExternalUrl = "https://www.";
                    return;
                }
            }

            if (e.SelectedKey == 0 || Implement == null || Implement.ID == 0)
            {
                return;
            }

            if (!IsPostBack)
            {
                switch (Implement.Type)
                {
                    case LinkType.ExternalUrl:
                        {
                            Form.Type = (int)LinkType.ExternalUrl;
                            Form.IsExternalLink = true;
                        }
                        break;
                    case LinkType.InternalAsset:
                        {
                            Form.Type = (int)LinkType.InternalAsset;
                            Form.IsInternalDoc = true;
                        }
                        break;
                    default:
                        {
                            Form.Type = (int)LinkType.InternalPage;
                            Form.IsInternalLink = true;
                        }
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
