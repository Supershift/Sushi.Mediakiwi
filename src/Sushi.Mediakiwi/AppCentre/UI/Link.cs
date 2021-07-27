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

        LinkForm Form { get; set; }
        Mediakiwi.Data.Link Implement { get; set; }

        #region CTor

        /// <summary>
        /// Initializes a new instance of the <see cref="Link"/> class.
        /// </summary>
        public Link()
        {
            wim.OpenInEditMode = true;
            wim.CanContainSingleInstancePerDefinedList = true;

            wim.ListDataDependendProperties.Add(nameof(Mediakiwi.Data.Link.PageID));
            wim.ListDataDependendProperties.Add(nameof(Mediakiwi.Data.Link.ExternalUrl));
            wim.HideOpenCloseToggle = true;

            ListLoad += Link_ListLoad;
            ListPreRender += Link_ListPreRender;
            ListSave += Link_ListSave;
            ListDelete += Link_ListDelete;
            ListAction += Link_ListAction;
            ListHeadless += Link_ListHeadless;
        }

        #endregion CTor

        #region List HeadLess

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

            switch (link.Type)
            {
                default:
                case LinkType.Undefined:
                case LinkType.ExternalUrl:
                    {
                        linkref.Href = link.ExternalUrl;
                    }
                    break;
                case LinkType.InternalPage:
                    {
                        var pagelink = await Page.SelectOneAsync(link.PageID.GetValueOrDefault()).ConfigureAwait(false);
                        linkref.Href = Utility.ConvertUrl(pagelink?.InternalPath);
                    }
                    break;
                case LinkType.InternalAsset:
                    {
                        if (string.IsNullOrWhiteSpace(link?.Asset?.RemoteLocation) == false)
                        {
                            linkref.Href = link.Asset.RemoteLocation;
                        }
                    }
                    break;
            }

            e.Result = linkref;
        }

        #endregion List HeadLess

        #region List Action

        Task Link_ListAction(ComponentActionEventArgs e)
        {
            if (IsPostBack)
            {
                switch (Form.Type)
                {
                    case LinkType.InternalPage:
                        {
                            if (!Implement.PageID.HasValue)
                            {
                                wim.Notification.AddError(nameof(Mediakiwi.Data.Link.PageID), "Please apply all mandatory fields");
                            }
                        }
                        break;
                    case LinkType.ExternalUrl:
                        {
                            if (string.IsNullOrWhiteSpace(Implement.ExternalUrl))
                            {
                                wim.Notification.AddError(nameof(Mediakiwi.Data.Link.ExternalUrl), "Please apply all mandatory fields");
                            }
                        }
                        break;
                    case LinkType.InternalAsset:
                        {
                            if (!Implement.AssetID.HasValue)
                            {
                                wim.Notification.AddError(nameof(Mediakiwi.Data.Link.AssetID), "Please apply all mandatory fields");
                            }
                        }
                        break;
                }
            }
            return Task.CompletedTask;
        }

        #endregion List Action

        #region List Prerender

        Task Link_ListPreRender(ComponentListEventArgs e)
        {
            if (IsPostBack)
            {
                switch (Form.Type)
                {
                    case LinkType.InternalPage:
                        {
                            if (!Implement.PageID.HasValue)
                            {
                                wim.Notification.AddError(nameof(Mediakiwi.Data.Link.PageID), "Please apply all mandatory fields");
                            }
                        }
                        break;
                    case LinkType.ExternalUrl:
                        {
                            if (string.IsNullOrWhiteSpace(Implement.ExternalUrl))
                            {
                                wim.Notification.AddError(nameof(Mediakiwi.Data.Link.ExternalUrl), "Please apply all mandatory fields");
                            }
                        }
                        break;
                    case LinkType.InternalAsset:
                        {
                            if (!Implement.AssetID.HasValue)
                            {
                                wim.Notification.AddError(nameof(Mediakiwi.Data.Link.AssetID), "Please apply all mandatory fields");
                            }
                        }
                        break;
                }
            }
            return Task.CompletedTask;
        }
        #endregion List Prerender

        #region List Delete

        async Task Link_ListDelete(ComponentListEventArgs e)
        {
            await Implement.DeleteAsync().ConfigureAwait(false);
            wim.OnDeleteScript = "<script type=\"text/javascript\">parent.removeLink();</script>";
        }

        #endregion List Delete

        #region List Save

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

            Implement.IsInternal = (Form.Type == LinkType.InternalPage);
            Implement.Alt = Utility.CleanLineFeed(Implement.Alt, false);

            if (string.IsNullOrEmpty(Implement.Text))
            {
                switch (Form.Type)
                {
                    case LinkType.InternalPage:
                        {
                            Page page = await Page.SelectOneAsync(Implement.PageID.Value, false).ConfigureAwait(false);
                            Implement.Text = page.LinkText;
                        }
                        break;
                    case LinkType.InternalAsset:
                        {
                            Asset asset = await Asset.SelectOneAsync(Implement.AssetID.Value).ConfigureAwait(false);
                            Implement.Text = asset.Title;
                        }
                        break;
                }
            }

            // Clean obsolete properties, based off type
            switch (Form.Type)
            {
                case LinkType.InternalPage:
                    {
                        Implement.ExternalUrl = string.Empty;
                        Implement.AssetID = null;
                    }
                    break;
                case LinkType.ExternalUrl:
                    {
                        Implement.PageID = null;
                        Implement.AssetID = null;
                    }
                    break;
                case LinkType.InternalAsset:
                    {
                        Implement.ExternalUrl = string.Empty;
                        Implement.PageID = null;
                    }
                    break;
            }

            await Implement.SaveAsync().ConfigureAwait(false);

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
                    wim.OnSaveScript = $"<script type=\"text/javascript\">parent.insertLink('wim:{Implement.ID}');</script>";

                }
                else
                {
                    wim.OnSaveScript = $@"<div class=""customLink cmd_createlink now_yes""><input type=""hidden"" id=""{Implement.ID}"" value=""wim:{Implement.ID}"" /></div>";
                }
            }
            else
            {
                 wim.OnSaveScript = $@"<input type=""hidden"" class=""postparent"" id=""{Implement.ID}"" value=""<a title='url: {Implement.GetUrl(wim.CurrentSite, false)}'>{Implement.Text}</a> (open in {Implement.TargetInfo})"" />";
            }
        }
        #endregion List Save

        #region List Load

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
                switch (Form.Type)
                {
                    default:
                    case LinkType.Undefined:
                    case LinkType.ExternalUrl:
                        {
                            Form.IsExternalLink = true;
                            if (string.IsNullOrEmpty(Implement.ExternalUrl))
                            {
                                Implement.ExternalUrl = "https://www.";
                            }
                        }
                        break;
                    case LinkType.InternalPage:
                        {
                            Form.IsInternalLink = true;
                        }
                        break;
                    case LinkType.InternalAsset:
                        {
                            Form.IsInternalDoc = true;
                        }
                        break;
                }
            }
            else
            {
                if (e.SelectedKey == 0)
                {
                    Form.Type = LinkType.ExternalUrl;
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
                            Form.Type = LinkType.ExternalUrl;
                            Form.IsExternalLink = true;
                        }
                        break;
                    case LinkType.InternalAsset:
                        {
                            Form.Type = LinkType.InternalAsset;
                            Form.IsInternalDoc = true;
                        }
                        break;
                    default:
                        {
                            Form.Type = LinkType.InternalPage;
                            Form.IsInternalLink = true;
                        }
                        break;
                }
            }
        }

        #endregion List Load
    }
}
