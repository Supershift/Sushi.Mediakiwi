using Sushi.Mediakiwi.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public static class PageExtension
{

    /// <summary>
    /// Copies component content and page meta data (CustomDate, Description, Expiration, Keywords, LinkText, Name and Title) from the pageversion
    /// </summary>
    /// <param name="pageVersion"></param>
    /// <param name="user">The user who is executing the operation</param>
    internal static async Task CopyFromVersionAsync(this Page inPage, IPageVersion pageVersion, IApplicationUser user)
    {
        var targetComponents = await ComponentVersion.SelectAllAsync(inPage.ID).ConfigureAwait(false);
        await SaveThisVersionAsync(inPage, targetComponents, user).ConfigureAwait(false);

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
            await c.SaveAsync().ConfigureAwait(false);
        }

        var sourcePage = Utility.GetDeserialized(typeof(Page), pageVersion.MetaDataXML) as Page;

        inPage.CustomDate = sourcePage.CustomDate;
        inPage.Description = sourcePage.Description;
        inPage.Expiration = sourcePage.Expiration;
        inPage.Keywords = sourcePage.Keywords;
        inPage.LinkText = sourcePage.LinkText;
        inPage.Name = sourcePage.Name;
        inPage.Title = sourcePage.Title;
        await inPage.SaveAsync().ConfigureAwait(false);

        Sushi.Mediakiwi.Framework.Caching.FlushIndexOfCache($"Data_{inPage.GetType()}");
        //Wim.Utilities.CacheItemManager.FlushIndexOfCacheObjects(string.Concat(.ToString()));
    }

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
                        await newlink.SaveAsync().ConfigureAwait(false);

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

        Component[] pageComponents = await Component.SelectAllInheritedAsync(inPage.ID, false, false).ConfigureAwait(false);

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
            inPage.InheritContent = inPage.IsLocalized == false;

            await inPage.CopyComponentsAsync().ConfigureAwait(false);
            await inPage.SaveAsync().ConfigureAwait(false);
            await inPage.SubmitPageForSearchAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Notification.InsertOne("Threading (page)", ex.Message);
        }
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
            inPage.Published = null;
            inPage.IsPublished = false;

            await inPage.SaveAsync().ConfigureAwait(false);
            await CleanUpAfterTakeDownAsync(inPage).ConfigureAwait(false);
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
        await Page.DeleteAllComponentSearchReferencesAsync(inPage.ID).ConfigureAwait(false);
        await inPage.CleanUpAfterTakeDownAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Publishes the page
    /// </summary>
    /// <param name="?">The ?.</param>
    /// <param name="user">The user.</param>
    public static async Task PublishAsync(this Page inPage, Sushi.Mediakiwi.Framework.IPagePublication pagePublication, IApplicationUser user)
    {
        if (pagePublication.DoPrePublishValidation(user, inPage))
        {
            await inPage.PublishPageAsync().ConfigureAwait(false);
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

        var targetComponents = await ComponentVersion.SelectAllAsync(inPage.ID).ConfigureAwait(false);

        await SaveThisVersionAsync(inPage, targetComponents, user).ConfigureAwait(false);
        foreach (var c in targetComponents)
        {
           await c.DeleteAsync().ConfigureAwait(false);
        }

        var sourceComponents = await ComponentVersion.SelectAllAsync(sourcePage.ID).ConfigureAwait(false);
        foreach (var c in sourceComponents)
        {
            var component = new ComponentVersion();
            Utility.ReflectProperty(c, component);
            await RecreateLinksInComponentForCopyAsync(inPage, c, null).ConfigureAwait(false);
            c.ID = 0;
            c.GUID = Guid.NewGuid();
            c.PageID = inPage.ID;
            await c.SaveAsync().ConfigureAwait(false);
        }

        Sushi.Mediakiwi.Framework.Caching.FlushIndexOfCache($"Data_{inPage.GetType()}");
        //Wim.Utilities.CacheItemManager.FlushIndexOfCacheObjects();
        await inPage.SaveAsync().ConfigureAwait(false);
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

        await pvOld.SaveAsync().ConfigureAwait(false);
        await AuditTrail.InsertAsync(user, inPage, ActionType.Update, pvOld.ID).ConfigureAwait(false);
    }

    internal static async Task CopyComponentsAsync(this Page inPage)
    {
        Component[] liveComponents = await Component.SelectAllInheritedAsync(inPage.ID, true, false).ConfigureAwait(false);
        ComponentVersion[] stagingComponents = await ComponentVersion.SelectAllAsync(inPage.ID).ConfigureAwait(false);

        await inPage.CopyComponentsAsync(liveComponents, stagingComponents).ConfigureAwait(false);
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
                    await component.SaveAsync().ConfigureAwait(false);
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
                await component2.SaveAsync().ConfigureAwait(false);
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
                await component.DeleteAsync().ConfigureAwait(false);
            }
            found = false;
        }
    }
}

