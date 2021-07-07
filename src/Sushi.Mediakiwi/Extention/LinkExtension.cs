using Sushi.Mediakiwi.Data;


public static class LinkExtension
{

    /// <summary>
    /// Gets the URL.
    /// </summary>
    /// <param name="currentSite">The current site.</param>
    /// <returns></returns>
    public static string GetUrl(this Link inLink, Site currentSite)
    {
        return GetUrl(inLink, currentSite, true);
    }


    /// <summary>
    /// Gets or sets the URL based on either an internal or external URL
    /// </summary>
    /// <param name="currentSite">The current site.</param>
    /// <param name="onlyReturnPublished">if set to <c>true</c> [only return published].</param>
    /// <returns></returns>
    /// <value>The URL.</value>
    public static string GetUrl(this Link inLink, Site currentSite, bool onlyReturnPublished)
    {
        string m_Url = string.Empty;
        if (inLink.Type == LinkType.InternalAsset)
        {
            m_Url = inLink.Asset.DownloadUrl;
        }
        else if (inLink.IsInternal)
        {
            Page page = null;
            if (inLink.PageID.HasValue)
            {
                if (currentSite == null)
                    page = Page.SelectOne(inLink.PageID.Value, onlyReturnPublished);
                else
                    page = Page.SelectOneChild(inLink.PageID.Value, currentSite.ID, onlyReturnPublished);
            }

            if (page == null)
                return null;

            m_Url = page.HRef;
        }
        else
        {
            if (inLink.ExternalUrl == null || inLink.ExternalUrl.Trim().Length == 0)
                return null;

            m_Url = inLink.ExternalUrl;
        }

        return m_Url;
    }

    ///// <summary>
    ///// Apply the link properties to a HyperLink control.
    ///// </summary>
    ///// <param name="hyperlink">Hyperlink control to set</param>
    //public static void Apply(this Link inLink, System.Web.UI.WebControls.HyperLink hyperlink)
    //{
    //    Apply(inLink, hyperlink, false);
    //}

    ///// <summary>
    ///// Apply the link properties to a HyperLink control.
    ///// </summary>
    ///// <param name="hyperlink">Hyperlink control to set</param>
    ///// <param name="onlySetNavigationUrl">Should only the navigationUrl be set?</param>
    //public static void Apply(this Link inLink, System.Web.UI.WebControls.HyperLink hyperlink, bool onlySetNavigationUrl)
    //{
    //    Apply(inLink, hyperlink, onlySetNavigationUrl, null);
    //}

    ///// <summary>
    ///// Apply the link properties to a HyperLink control.
    ///// </summary>
    ///// <param name="hyperlink">Hyperlink control to set</param>
    ///// <param name="onlySetNavigationUrl">Should only the navigationUrl be set?</param>
    ///// <param name="currentSite">When inheritence is used apply the current site object to adapt the link.Href for internal links</param>
    //public static void Apply(this Link inLink, System.Web.UI.WebControls.HyperLink hyperlink, bool onlySetNavigationUrl, Site currentSite)
    //{
    //    hyperlink.Visible = false;

    //    hyperlink.NavigateUrl = GetUrl(inLink, currentSite);

    //    if (!onlySetNavigationUrl)
    //        hyperlink.Text = System.Web.HttpContext.Current.Server.HtmlEncode(inLink.Text);

    //    hyperlink.ToolTip = inLink.Alt;

    //    if (inLink.Target == 2)
    //        hyperlink.Target = "_blank";

    //    hyperlink.Visible = true;
    //}
}
