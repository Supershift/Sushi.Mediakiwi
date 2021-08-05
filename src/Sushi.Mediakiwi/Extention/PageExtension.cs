using Sushi.Mediakiwi.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public static class PageExtension
{

    ///// <summary>
    ///// Saves component and page state to a pageversion and inserts an audit trail
    ///// </summary>
    ///// <param name="targetComponents">The components of the page</param>
    ///// <param name="user">the user performing the operation</param>
    //private static void SaveThisVersion(this Page inPage, ComponentVersion[] targetComponents, IApplicationUser user)
    //{

    //    StringBuilder contentHash = new StringBuilder();
    //    foreach (var version in targetComponents)
    //    {

    //        var content = version.GetContent();

    //        if (content != null && content.Fields != null)
    //        {

    //            foreach (var item in content.Fields)
    //            {
    //                if (!string.IsNullOrEmpty(item.Value))
    //                {
    //                    if (
    //                        item.Type == (int)ContentType.Binary_Image
    //                        || item.Type == (int)ContentType.Hyperlink
    //                        || item.Type == (int)ContentType.Binary_Document
    //                        || item.Type == (int)ContentType.PageSelect
    //                        || item.Type == (int)ContentType.FolderSelect
    //                        || item.Type == (int)ContentType.Choice_Dropdown
    //                        )
    //                    {
    //                        if (item.Value == "0")
    //                            continue;
    //                    }


    //                    contentHash.Append(item.Value);
    //                }
    //            }
    //        }
    //    }
    //    // first backup the current version
    //    var pvOld = new PageVersion();
    //    pvOld.ContentXML = Utility.GetSerialized(targetComponents); ;
    //    pvOld.MetaDataXML = Utility.GetSerialized(inPage);
    //    pvOld.UserID = user.ID;
    //    pvOld.PageID = inPage.ID;
    //    pvOld.TemplateID = inPage.TemplateID;
    //    pvOld.IsArchived = false;
    //    pvOld.Name = inPage.Name;
    //    pvOld.CompletePath = inPage.CompletePath;
    //    pvOld.Hash = Utility.HashString(contentHash.ToString());
    //    pvOld.Save();

    //    //Sushi.Mediakiwi.Framework2.Functions.AuditTrail.Insert(user, inPage, Sushi.Mediakiwi.Framework2.Functions.Auditing.ActionType.Update, pvOld.ID);
    //}


    /// <summary>
    /// Copies component content and page meta data (CustomDate, Description, Expiration, Keywords, LinkText, Name and Title) from the pageversion
    /// </summary>
    /// <param name="pageVersion"></param>
    /// <param name="user">The user who is executing the operation</param>
    internal static async Task CopyFromVersionAsync(this Page inPage, IPageVersion pageVersion, IApplicationUser user)
    {
        var targetComponents = await ComponentVersion.SelectAllAsync(inPage.ID);
        await SaveThisVersionAsync(inPage, targetComponents, user);

        foreach (var c in targetComponents)
        {
            c.Delete();
        }

        var sourceComponents = Utility.GetDeserialized(typeof(ComponentVersion[]), pageVersion.ContentXML) as ComponentVersion[];
        foreach (var c in sourceComponents)
        {
            var component = new ComponentVersion();
            Utility.ReflectProperty(c, component);
            c.ID = 0;
            c.GUID = Guid.NewGuid();
            c.PageID = inPage.ID;
            await c.SaveAsync();
        }

        var sourcePage = Utility.GetDeserialized(typeof(Page), pageVersion.MetaDataXML) as Page;

        inPage.CustomDate = sourcePage.CustomDate;
        inPage.Description = sourcePage.Description;
        inPage.Expiration = sourcePage.Expiration;
        inPage.Keywords = sourcePage.Keywords;
        inPage.LinkText = sourcePage.LinkText;
        inPage.Name = sourcePage.Name;
        inPage.Title = sourcePage.Title;
        await inPage.SaveAsync();

        Sushi.Mediakiwi.Framework.Caching.FlushIndexOfCache($"Data_{inPage.GetType()}");
        //Wim.Utilities.CacheItemManager.FlushIndexOfCacheObjects(string.Concat(.ToString()));
    }

    ///// <summary>
    ///// Copies the page content from the given sourcepage to the current page as the target
    ///// </summary>
    ///// <param name="sourcePage">The page to copy content from</param>
    ///// <param name="user">The user who is executing the operation</param>
    ///// <returns>True; when copying was success, False or exception when it wasn't</returns>
    //public static bool OverridePageContentFromPage(this Page inPage, Page sourcePage, IApplicationUser user)
    //{
    //    if (sourcePage.TemplateID != inPage.TemplateID)
    //        throw new ArgumentException("The sourcePage must match it's templateID with the target page");

    //    var targetComponents = ComponentVersion.SelectAll(inPage.ID);

    //    SaveThisVersion(inPage, targetComponents, user);
    //    foreach (var c in targetComponents)
    //        c.Delete();

    //    var sourceComponents = ComponentVersion.SelectAll(sourcePage.ID);
    //    foreach (var c in sourceComponents)
    //    {
    //        var component = new ComponentVersion();
    //        Utility.ReflectProperty(c, component);
    //        RecreateLinksInComponentForCopy(inPage, c, null);
    //        c.ID = 0;
    //        c.GUID = Guid.NewGuid();
    //        c.PageID = inPage.ID;
    //        c.Save(false);
    //    }

    //    //Wim.Utilities.CacheItemManager.FlushIndexOfCacheObjects(string.Concat("Data_", inPage.GetType().ToString()));
    //    inPage.Save();
    //    return true;
    //}

    public static void RecreateLinksInComponentForCopy(this Page inPage, ComponentVersion component, Dictionary<int, Folder> oldNewFolderMapping = null, List<Link> pageLinks = null)
    {
        var fieldList = new List<Field>();
        var currentContent = component.GetContent();
        if (currentContent != null && currentContent.Fields != null)
        {
            foreach (var field in currentContent.Fields)
            {
                if (field.Type == (int)ContentType.FolderSelect && oldNewFolderMapping != null)
                {
                    int oldFolderView = Utility.ConvertToInt(field.Value, 0);
                    if (oldFolderView > 0 && oldNewFolderMapping.ContainsKey(oldFolderView))
                    {
                        field.Value = oldNewFolderMapping[oldFolderView].ToString();
                    }
                }


                if (field.Type == (int)ContentType.Hyperlink)
                {
                    var link = field.Link;
                    if (link.ID > 0)
                    {
                        var newlink = new Link();
                        Utility.ReflectProperty(link, newlink);

                        newlink.ID = 0;
                        newlink.GUID = Guid.NewGuid();
                        newlink.Save();

                        if (pageLinks != null && newlink.PageID.HasValue)
                        {
                            pageLinks.Add(newlink);
                            field.Value = newlink.ID.ToString();
                        }
                    }
                }
                if (field.Type == (int)ContentType.RichText)
                {
                    var result = MatchAndReplaceTextForLinks(field.Value, pageLinks);
                    field.Value = result;
                }
                fieldList.Add(field);
            }

            var content = new Content();
            content.Fields = fieldList.ToArray();
            component.Serialized_XML = Content.GetSerialized(content);
        }
    }

    public static async Task RecreateLinksInComponentForCopyAsync(this Page inPage, ComponentVersion component, Dictionary<int, Folder> oldNewFolderMapping = null, List<Link> pageLinks = null)
    {
        var fieldList = new List<Field>();
        var currentContent = component.GetContent();
        if (currentContent != null && currentContent.Fields != null)
        {
            foreach (var field in currentContent.Fields)
            {
                if (field.Type == (int)ContentType.FolderSelect && oldNewFolderMapping != null)
                {
                    int oldFolderView = Utility.ConvertToInt(field.Value, 0);
                    if (oldFolderView > 0 && oldNewFolderMapping.ContainsKey(oldFolderView))
                    {
                        field.Value = oldNewFolderMapping[oldFolderView].ToString();
                    }
                }

                if (field.Type == (int)ContentType.Hyperlink)
                {
                    var link = field.Link;
                    if (link.ID > 0)
                    {
                        var newlink = new Link();
                        Utility.ReflectProperty(link, newlink);

                        newlink.ID = 0;
                        newlink.GUID = Guid.NewGuid();
                        await newlink.SaveAsync();

                        if (pageLinks != null && newlink.PageID.HasValue)
                        {
                            pageLinks.Add(newlink);
                            field.Value = newlink.ID.ToString();
                        }
                    }
                }
                if (field.Type == (int)ContentType.RichText)
                {
                    var result = MatchAndReplaceTextForLinks(field.Value, pageLinks);
                    field.Value = result;
                }
                fieldList.Add(field);
            }

            var content = new Content();
            content.Fields = fieldList.ToArray();
            component.Serialized_XML = Content.GetSerialized(content);
        }
    }


    private static Regex getWimLinks = new Regex(@"""wim:(.*?)""", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static string MatchAndReplaceTextForLinks(string value, List<Link> pageLinks = null)
    {
        if (value != null)
        {
            return getWimLinks.Replace(value, delegate (Match match) {
                if (match.Groups.Count > 1)
                {
                    string v = match.Groups[1].Value;
                    var link = Link.SelectOne(Utility.ConvertToInt(v));
                    if (link.ID > 0)
                    {
                        var newlink = new Link();
                        Utility.ReflectProperty(link, newlink);
                        newlink.ID = 0;
                        newlink.GUID = Guid.NewGuid();
                        newlink.Save();
                        if (pageLinks != null && newlink.PageID.HasValue)
                        {
                            pageLinks.Add(newlink);
                        }

                        return $@"""wim:{newlink.ID.ToString()}""";
                    }
                    else
                    {
                        return value;
                    }
                }
                else
                {
                    return value;
                }
            });
        }
        return null;
    }

    /// <summary>
    /// Submits the page for search.
    /// </summary>
    /// <param name="pageID">The page ID.</param>
    internal static async Task SubmitPageForSearchAsync(this Page inPage)
    {
        if (!inPage.IsSearchable)
        {
            return;
        }

        Component[] pageComponents = await Component.SelectAllInheritedAsync(inPage.ID, false, false);

        StringBuilder searchableContent = null;
        foreach (Component component in pageComponents)
        {
            if (!component.IsSearchable)
            {
                continue;
            }

            searchableContent = new StringBuilder();

            //  Get content
            if (component.Serialized_XML == null || component.Serialized_XML.Length == 0)
            {
                continue;
            }

            Content content = Content.GetDeserialized(component.Serialized_XML);

            //  Get correct field content
            if (content.Fields == null || content.Fields.Length == 0)
            {
                continue;
            }

            foreach (Field field in content.Fields)
            {
                if (field.Value == null || field.Value.Length == 0)
                {
                    continue;
                }

                if (field.Type == (int)ContentType.TextField ||
                    field.Type == (int)ContentType.TextArea ||
                    field.Type == (int)ContentType.RichText)
                {
                    searchableContent.Append(" " + field.Value);
                }
            }

            if (searchableContent.Length != 0)
            {
                string finalContent = searchableContent.ToString();

                //ComponentSearch.AddOne(1, component.ID, finalContent, inPage.SiteID);
            }
        }
    }



    /// <summary>
    /// Publishes the page.
    /// </summary>
    /// <param name="context">The context.</param>
    internal static async Task PublishPageAsync(this Page inPage)
    {
        inPage.SetInternalPath();
        try
        {
            inPage.Updated = Common.DatabaseDateTime;
            inPage.Published = inPage.Updated;
            inPage.IsPublished = true;
            inPage.InheritContent = inPage.InheritContentEdited;

            await inPage.CopyComponentsAsync();
            await inPage.SaveAsync();
            await inPage.SubmitPageForSearchAsync();
        }
        catch (Exception ex)
        {
            Notification.InsertOne("Threading (page)", ex.Message);
        }
    }

    /// <summary>
    /// Takes down.
    /// </summary>
    /// <param name="userID">The user ID.</param>
    public static async Task TakeDown(this Page inPage, Sushi.Mediakiwi.Framework.IPagePublication pagePublication, IApplicationUser user)
    {
        //Page.ContextContainer context = new Page.ContextContainer();
        //context.Context = HttpContext.Current;
        //context.User = user;
        //context.PagePublication = pagePublication;

        await inPage.TakeDownPageAsync();

        //if (!string.IsNullOrEmpty(inPage.HRef))
        //    HttpResponse.RemoveOutputCacheItem(inPage.HRef);
    }

    /// <summary>
    /// Takes down page.
    /// </summary>
    /// <param name="context">The context.</param>
    internal static async Task TakeDownPageAsync(this Page inPage)
    {
        try
        {
            inPage.Updated = Common.DatabaseDateTime;
            inPage.Published = DateTime.MinValue;
            inPage.IsPublished = false;
            await inPage.SaveAsync();

            await CleanUpAfterTakeDownAsync(inPage);

            Sushi.Mediakiwi.Framework.Caching.FlushAll();
        }
        catch (Exception ex)
        {
            Notification.InsertOne("Threading (page)", ex.Message);
        }
    }

    /// <summary>
    /// Clean up the component list after take done because some residual components could stil be present.
    /// </summary>
    /// <param name="pageID">The page ID.</param>
    internal static async Task CleanUpAfterTakeDownAsync(this Page inPage)
    {
        await Page.DeleteAllComponentSearchReferencesAsync(inPage.ID);
        await inPage.CleanUpAfterTakeDownAsync();
    }

    /// <summary>
    /// Publishes the page
    /// </summary>
    /// <param name="?">The ?.</param>
    /// <param name="user">The user.</param>
    public static async Task PublishAsync(this Page inPage, Sushi.Mediakiwi.Framework.IPagePublication pagePublication, IApplicationUser user)
    {
        //Page.ContextContainer context = new Page.ContextContainer();
        //context.Context = HttpContext.Current;
        //context.User = user;
        //context.PagePublication = pagePublication;

        if (pagePublication.DoPrePublishValidation(user, inPage))
        {
            await inPage.PublishPageAsync();
            //System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(inPage.PublishPage), context);

            //if (!string.IsNullOrEmpty(inPage.HRef_Short))
            //{
            //    HttpResponse.RemoveOutputCacheItem(inPage.HRef_Short);
            //}
        }
    }

    /// <summary>
    /// Copies the page content from the given sourcepage to the current page as the target
    /// </summary>
    /// <param name="sourcePage">The page to copy content from</param>
    /// <param name="user">The user who is executing the operation</param>
    /// <returns>True; when copying was success, False or exception when it wasn't</returns>
    public static async Task<bool> OverridePageContentFromPageAsync(this Page inPage, Page sourcePage, IApplicationUser user)
    {
        if (sourcePage.TemplateID != inPage.TemplateID)
        {
            throw new ArgumentException("The sourcePage must match it's templateID with the target page");
        }

        var targetComponents = await ComponentVersion.SelectAllAsync(inPage.ID);

        await SaveThisVersionAsync(inPage, targetComponents, user);
        foreach (var c in targetComponents)
        {
           await c.DeleteAsync();
        }

        var sourceComponents = await ComponentVersion.SelectAllAsync(sourcePage.ID);
        foreach (var c in sourceComponents)
        {
            var component = new ComponentVersion();
            Utility.ReflectProperty(c, component);
            await RecreateLinksInComponentForCopyAsync(inPage, c, null);
            c.ID = 0;
            c.GUID = Guid.NewGuid();
            c.PageID = inPage.ID;
            await c.SaveAsync();
        }

        Sushi.Mediakiwi.Framework.Caching.FlushIndexOfCache($"Data_{inPage.GetType()}");
        //Wim.Utilities.CacheItemManager.FlushIndexOfCacheObjects();
        await inPage.SaveAsync();
        return true;
    }

    /// <summary>
    /// Saves component and page state to a pageversion and inserts an audit trail
    /// </summary>
    /// <param name="targetComponents">The components of the page</param>
    /// <param name="user">the user performing the operation</param>
    private static async Task SaveThisVersionAsync(this Page inPage, ComponentVersion[] targetComponents, IApplicationUser user)
    {
        StringBuilder contentHash = new StringBuilder();
        foreach (var version in targetComponents)
        {
            var content = version.GetContent();

            if (content != null && content.Fields != null)
            {
                foreach (var item in content.Fields)
                {
                    if (!string.IsNullOrEmpty(item.Value))
                    {
                        if ((item.Type == (int)ContentType.Binary_Image
                            || item.Type == (int)ContentType.Hyperlink
                            || item.Type == (int)ContentType.Binary_Document
                            || item.Type == (int)ContentType.PageSelect
                            || item.Type == (int)ContentType.FolderSelect
                            || item.Type == (int)ContentType.Choice_Dropdown
                            ) && item.Value == "0")
                        {
                            continue;
                        }

                        contentHash.Append(item.Value);
                    }
                }
            }
        }

        // first backup the current version
        var pvOld = new PageVersion();
        pvOld.ContentXML = Utility.GetSerialized(targetComponents);
        pvOld.MetaDataXML = Utility.GetSerialized(inPage);
        pvOld.UserID = user.ID;
        pvOld.PageID = inPage.ID;
        pvOld.TemplateID = inPage.TemplateID;
        pvOld.IsArchived = false;
        pvOld.Name = inPage.Name;
        pvOld.CompletePath = inPage.CompletePath;
        pvOld.Hash = Utility.HashString(contentHash.ToString());

        await pvOld.SaveAsync();
        await AuditTrail.InsertAsync(user, inPage, ActionType.Update, pvOld.ID);
    }

    internal static async Task CopyComponentsAsync(this Page inPage)
    {
        Component[] liveComponents = await Component.SelectAllInheritedAsync(inPage.ID, true, false);
        ComponentVersion[] stagingComponents = await ComponentVersion.SelectAllAsync(inPage.ID);

        await inPage.CopyComponentsAsync(liveComponents, stagingComponents);
    }

    /// <summary>
    /// Publishes the specified user ID.
    /// </summary>
    /// <param name="userID">The user ID.</param>
    internal static async Task CopyComponentsAsync(this Page inPage, Component[] liveComponents, ComponentVersion[] stagingComponents)
    {
        int sortOrderCount = 0;

        List<int> foundComponentArr = new List<int>();
        foreach (ComponentVersion componentStaged in stagingComponents)
        {
            if (!componentStaged.IsActive)
            {
                continue;
            }
            sortOrderCount++;

            bool foundComponent = false;
            foreach (Component component in liveComponents)
            {
                if (component.GUID == componentStaged.GUID)
                {
                    //  Set the found component
                    foundComponentArr.Add(component.ID);

                    componentStaged.Apply(component);
                    await component.SaveAsync();
                    component.SortOrder = sortOrderCount;
                    foundComponent = true;

                    break;
                }
            }
            if (foundComponent)
            {
                continue;
            }

            try
            {
                Component component2 = new Component();
                componentStaged.Apply(component2);
                component2.SortOrder = sortOrderCount;
                await component2.SaveAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        bool found = false;
        foreach (Component component in liveComponents)
        {
            foreach (int id in foundComponentArr)
            {
                if (id == component.ID)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                await component.DeleteAsync();
            }
            found = false;
        }
    }


    ///// <summary>
    ///// Get all property content from the page component 
    ///// </summary>
    ///// <param name="componentTemplateKey">Component template to search in</param>
    ///// <param name="propertyName">The property to look for and return its value</param>
    ///// <returns>The property values</returns>
    //public static string[] GetComponentProperties(this Page inPage, int componentTemplateKey, string propertyName)
    //{
    //    //  Supporting fields
    //    List<Content> m_ContentItems = new List<Content>();
    //    List<Component> m_Components = new List<Component>();

    //    int m_CurrentCalledcomponentTemplateKey = 0;

    //    if (m_Components == null || m_Components.Count == 0 || componentTemplateKey != m_CurrentCalledcomponentTemplateKey)
    //    {
    //        m_CurrentCalledcomponentTemplateKey = componentTemplateKey;
    //        m_Components = Component.SelectAll(inPage.ID, componentTemplateKey).ToList();

    //        List<Content> contentList = new List<Content>();

    //        foreach (Component component in m_Components)
    //        {
    //            //  Get content
    //            if (component.Serialized_XML == null || component.Serialized_XML.Length == 0)
    //                continue;

    //            //  Get deserialized content
    //            Content content = Content.GetDeserialized(component.Serialized_XML);

    //            //  Validate fields
    //            if (content.Fields == null || content.Fields.Length == 0)
    //                continue;

    //            contentList.Add(content);
    //        }

    //        m_ContentItems = contentList;

    //    }

    //    if (m_ContentItems == null || m_ContentItems.Count == 0)
    //        return null;

    //    List<string> candidates = new List<string>();
    //    foreach (Content content in m_ContentItems)
    //    {
    //        foreach (Content.Field field in content.Fields)
    //        {
    //            if (field.Property == propertyName)
    //            {
    //                if (field.Value == null || field.Value.Length == 0)
    //                    continue;

    //                string candidate = field.Value;

    //                if (field.Type == (int)Sushi.Mediakiwi.Framework.ContentType.RichText)
    //                    candidate = Utility.ApplyRichtextLinks(inPage.Site, field.Value);

    //                candidates.Add(candidate);
    //            }
    //        }
    //    }
    //    return candidates.ToArray();
    //}

    ///// <summary>
    ///// Gets the component properties.
    ///// </summary>
    ///// <param name="sourceTag">The source tag.</param>
    ///// <param name="propertyName">Name of the property.</param>
    ///// <returns></returns>
    //public static string[] GetComponentProperties(this Page inPage, string sourceTag, string propertyName)
    //{
    //    var ct = ComponentTemplate.SelectOneBySourceTag(sourceTag);
    //    if (ct == null || ct.IsNewInstance)
    //        return null;

    //    return inPage.GetComponentProperties(ct.ID, propertyName);
    //}

    ///// <summary>
    ///// Get the first property content from the page component
    ///// </summary>
    ///// <param name="componentTemplateKey">Component template to search in</param>
    ///// <param name="propertyName">The property to look for and return its value</param>
    ///// <returns>The property value</returns>
    //public static string GetComponentProperty(this Page inPage, int componentTemplateKey, string propertyName)
    //{
    //    string[] candidates = inPage.GetComponentProperties(componentTemplateKey, propertyName);
    //    if (candidates != null && candidates.Length > 0)
    //        return candidates[0];
    //    return null;
    //}

    //public static string GetLocalCacheFile(this Page inPage, System.Collections.Specialized.NameValueCollection queryString)
    //{
    //    if (string.IsNullOrEmpty(inPage.Name))
    //        return null;

    //    string add = null;
    //    if (queryString.Count > 0)
    //    {
    //        add = "_q";
    //        foreach (string key in queryString.AllKeys)
    //        {
    //            if (key != "?")
    //                add += string.Concat("_", key, "_", queryString[key]);
    //        }
    //    }

    //    string tmp = string.Concat("/repository/cache/", inPage.InternalPath, add, ".html");
    //    return HttpContext.Current.Server.MapPath(Utility.AddApplicationPath(tmp));
    //}

    ///// <summary>
    ///// Gets the local cache href.
    ///// </summary>
    ///// <param name="queryString">The query string.</param>
    ///// <returns></returns>
    ///// <value>The local cache href.</value>
    //public static string GetLocalCacheHref(this Page inPage, System.Collections.Specialized.NameValueCollection queryString)
    //{
    //    if (string.IsNullOrEmpty(inPage.Name))
    //        return null;

    //    string add = null;
    //    if (queryString.Count > 0)
    //    {
    //        add = "_q";
    //        foreach (string key in queryString.AllKeys)
    //        {
    //            add += string.Concat("_", key, "_", queryString[key]);
    //        }
    //    }

    //    string tmp = string.Concat("/repository/cache/", inPage.InternalPath, add, ".html");
    //    return Utility.AddApplicationPath(tmp);
    //}

    ///// <summary>
    ///// Apply a link to this page.
    ///// </summary>
    ///// <param name="hyperlink">The hyperlink to apply the page linkage properties to.</param>
    //public static void Apply(this Page inPage, HyperLink hyperlink)
    //{
    //    inPage.Apply(hyperlink, false);
    //}

    //public static void Apply(this Page inPage, System.Web.UI.WebControls.HyperLink hyperlink, bool onlySetNavigationUrl)
    //{
    //    inPage.Apply(hyperlink, onlySetNavigationUrl, false);
    //}

    ///// <summary>
    ///// Apply a link to this page.
    ///// </summary>
    ///// <param name="hyperlink">The hyperlink to apply the page linkage properties to.</param>
    ///// <param name="onlySetNavigationUrl">Only set the navigation url, leave the text property as is.</param>
    //public static void Apply(this Page inPage, System.Web.UI.WebControls.HyperLink hyperlink, bool onlySetNavigationUrl, bool setFullUrlPath)
    //{
    //    hyperlink.Visible = false;

    //    if (inPage.Name == null || inPage.LinkText.Trim().Length == 0)
    //        return;
    //    if (inPage.LinkText == null || inPage.LinkText.Trim().Length == 0)
    //        inPage.LinkText = inPage.Name;

    //    if (!inPage.IsPublished)
    //        return;

    //    if (inPage.Publication != DateTime.MinValue)
    //    {
    //        if (DateTime.Now.Ticks < inPage.Publication.Ticks)
    //            return;
    //    }
    //    if (inPage.Expiration != DateTime.MinValue)
    //    {
    //        if (DateTime.Now.Ticks > inPage.Expiration.Ticks)
    //            return;
    //    }

    //    if (setFullUrlPath)
    //        hyperlink.NavigateUrl = inPage.HRefFull;
    //    else
    //        hyperlink.NavigateUrl = inPage.HRef;

    //    if (!onlySetNavigationUrl)
    //        hyperlink.Text = HttpContext.Current.Server.HtmlEncode(inPage.LinkText);

    //    hyperlink.ToolTip = inPage.Description == null ? "" : "";
    //    hyperlink.Visible = true;
    //}

}

