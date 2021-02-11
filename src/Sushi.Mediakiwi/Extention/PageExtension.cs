﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Sushi.Mediakiwi.Data;

public static class PageExtension
{

    /// <summary>
    /// Saves component and page state to a pageversion and inserts an audit trail
    /// </summary>
    /// <param name="targetComponents">The components of the page</param>
    /// <param name="user">the user performing the operation</param>
    private static void SaveThisVersion(this Page inPage, ComponentVersion[] targetComponents, IApplicationUser user)
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
                        if (
                            item.Type == (int)Sushi.Mediakiwi.Framework.ContentType.Binary_Image
                            || item.Type == (int)Sushi.Mediakiwi.Framework.ContentType.Hyperlink
                            || item.Type == (int)Sushi.Mediakiwi.Framework.ContentType.Binary_Document
                            || item.Type == (int)Sushi.Mediakiwi.Framework.ContentType.PageSelect
                            || item.Type == (int)Sushi.Mediakiwi.Framework.ContentType.FolderSelect
                            || item.Type == (int)Sushi.Mediakiwi.Framework.ContentType.Choice_Dropdown
                            )
                        {
                            if (item.Value == "0")
                                continue;
                        }


                        contentHash.Append(item.Value);
                    }
                }
            }
        }
        // first backup the current version
        var pvOld = new PageVersion();
        pvOld.ContentXML = Wim.Utility.GetSerialized(targetComponents); ;
        pvOld.MetaDataXML = Wim.Utility.GetSerialized(inPage);
        pvOld.UserID = user.ID;
        pvOld.PageID = inPage.ID;
        pvOld.TemplateID = inPage.TemplateID;
        pvOld.IsArchived = false;
        pvOld.Name = inPage.Name;
        pvOld.CompletePath = inPage.CompletePath;
        pvOld.Hash = Wim.Utility.HashString(contentHash.ToString());
        pvOld.Save();

        Sushi.Mediakiwi.Framework2.Functions.AuditTrail.Insert(user, inPage, Sushi.Mediakiwi.Framework2.Functions.Auditing.ActionType.Update, pvOld.ID);
    }


    /// <summary>
    /// Copies component content and page meta data (CustomDate, Description, Expiration, Keywords, LinkText, Name and Title) from the pageversion
    /// </summary>
    /// <param name="pageVersion"></param>
    /// <param name="user">The user who is executing the operation</param>
    internal static void CopyFromVersion(this Page inPage, IPageVersion pageVersion, IApplicationUser user)
    {
        var targetComponents = ComponentVersion.SelectAll(inPage.ID);
        SaveThisVersion(inPage, targetComponents, user);
        foreach (var c in targetComponents)
        {
            c.Delete();
        }
        var sourceComponents = Wim.Utility.GetDeserialized(typeof(ComponentVersion[]), pageVersion.ContentXML) as ComponentVersion[];
        foreach (var c in sourceComponents)
        {
            var component = new ComponentVersion();
            Wim.Utility.ReflectProperty(c, component);
            c.ID = 0;
            c.GUID = Guid.NewGuid();
            c.PageID = inPage.ID;
            c.Save(false);
        }

        var sourcePage = Wim.Utility.GetDeserialized(typeof(Page), pageVersion.MetaDataXML) as Page;

        inPage.CustomDate = sourcePage.CustomDate;
        inPage.Description = sourcePage.Description;
        inPage.Expiration = sourcePage.Expiration;
        inPage.Keywords = sourcePage.Keywords;
        inPage.LinkText = sourcePage.LinkText;
        inPage.Name = sourcePage.Name;
        inPage.Title = sourcePage.Title;
        inPage.Save();

        Wim.Utilities.CacheItemManager.FlushIndexOfCacheObjects(string.Concat("Data_", inPage.GetType().ToString()));
    }

    /// <summary>
    /// Copies the page content from the given sourcepage to the current page as the target
    /// </summary>
    /// <param name="sourcePage">The page to copy content from</param>
    /// <param name="user">The user who is executing the operation</param>
    /// <returns>True; when copying was success, False or exception when it wasn't</returns>
    public static bool OverridePageContentFromPage(this Page inPage, Page sourcePage, IApplicationUser user)
    {
        if (sourcePage.TemplateID != inPage.TemplateID)
            throw new ArgumentException("The sourcePage must match it's templateID with the target page");

        var targetComponents = ComponentVersion.SelectAll(inPage.ID);

        SaveThisVersion(inPage, targetComponents, user);
        foreach (var c in targetComponents)
            c.Delete();

        var sourceComponents = ComponentVersion.SelectAll(sourcePage.ID);
        foreach (var c in sourceComponents)
        {
            var component = new ComponentVersion();
            Wim.Utility.ReflectProperty(c, component);
            RecreateLinksInComponentForCopy(inPage, c, null);
            c.ID = 0;
            c.GUID = Guid.NewGuid();
            c.PageID = inPage.ID;
            c.Save(false);
        }

        Wim.Utilities.CacheItemManager.FlushIndexOfCacheObjects(string.Concat("Data_", inPage.GetType().ToString()));
        inPage.Save();
        return true;
    }

    public static void RecreateLinksInComponentForCopy(this Page inPage, ComponentVersion component, Dictionary<int, Folder> oldNewFolderMapping = null, List<Link> pageLinks = null)
    {
        var fieldList = new List<Content.Field>();
        var currentContent = component.GetContent();
        if (currentContent != null && currentContent.Fields != null)
        {
            foreach (var field in currentContent.Fields)
            {
                if (field.Type == (int)Sushi.Mediakiwi.Framework.ContentType.FolderSelect && oldNewFolderMapping != null)
                {
                    int oldFolderView = Wim.Utility.ConvertToInt(field.Value, 0);
                    if (oldFolderView > 0 && oldNewFolderMapping.ContainsKey(oldFolderView))
                        field.Value = oldNewFolderMapping[oldFolderView].ToString();
                }


                if (field.Type == (int)Sushi.Mediakiwi.Framework.ContentType.Hyperlink)
                {
                    var link = field.Link;
                    if (link.ID > 0)
                    {
                        var newlink = new Sushi.Mediakiwi.Data.Link();
                        Wim.Utility.ReflectProperty(link, newlink);
                        newlink.ID = 0;
                        newlink.GUID = Guid.NewGuid();
                        newlink.Save(false);
                        if (pageLinks != null && newlink.PageID.HasValue)
                            pageLinks.Add(newlink);
                        field.Value = newlink.ID.ToString();
                    }
                }
                if (field.Type == (int)Sushi.Mediakiwi.Framework.ContentType.RichText)
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
            return getWimLinks.Replace(value, delegate (Match match)
            {
                if (match.Groups.Count > 1)
                {
                    string v = match.Groups[1].Value;
                    var link = Sushi.Mediakiwi.Data.Link.SelectOne(Wim.Utility.ConvertToInt(v));
                    if (link.ID > 0)
                    {
                        var newlink = new Link();
                        Wim.Utility.ReflectProperty(link, newlink);
                        newlink.ID = 0;
                        newlink.GUID = Guid.NewGuid();
                        newlink.Save(false);
                        if (pageLinks != null && newlink.PageID.HasValue)
                            pageLinks.Add(newlink);
                        return $@"""wim:{newlink.ID.ToString()}""";
                    }
                    else
                        return value;
                }
                else
                    return value;
            });
        }
        return null;
    }


    /// <summary>
    /// Submits the page for search.
    /// </summary>
    /// <param name="pageID">The page ID.</param>
    internal static void SubmitPageForSearch(this Page inPage)
    {
        if (!inPage.IsSearchable)
        {
            Page.DeleteAllComponentSearchReferences(inPage.ID);
            return;
        }

        Component[] pageComponents = Component.SelectAll(inPage.ID);

        StringBuilder searchableContent = null;
        foreach (Component component in pageComponents)
        {
            if (!component.IsSearchable) continue;

            searchableContent = new StringBuilder();

            //  Get content
            if (component.Serialized_XML == null || component.Serialized_XML.Length == 0)
                continue;

            Content content = Content.GetDeserialized(component.Serialized_XML);

            //  Get correct field content
            if (content.Fields == null || content.Fields.Length == 0)
                continue;

            foreach (Content.Field field in content.Fields)
            {
                if (field.Value == null || field.Value.Length == 0)
                    continue;

                if (field.Type == (int)Sushi.Mediakiwi.Framework.ContentType.TextField ||
                    field.Type == (int)Sushi.Mediakiwi.Framework.ContentType.TextArea ||
                    field.Type == (int)Sushi.Mediakiwi.Framework.ContentType.RichText)
                {
                    searchableContent.Append(" " + field.Value);
                }
            }

            if (searchableContent.Length != 0)
            {
                string finalContent = searchableContent.ToString();

                ComponentSearch.AddOne(1, component.ID, finalContent, inPage.SiteID);
            }
        }
    }



    /// <summary>
    /// Publishes the page.
    /// </summary>
    /// <param name="context">The context.</param>
    internal static void PublishPage(this Page inPage, object context)
    {
        inPage.SetCompletePath();
        Page.ContextContainer c = (Page.ContextContainer)context;
        try
        {
            inPage.Updated = Common.DatabaseDateTime;
            inPage.Published = inPage.Updated;
            inPage.IsPublished = true;
            inPage.InheritContent = inPage.InheritContentEdited;

            int userID = c.User == null ? 0 : c.User.ID;
            inPage.CopyComponents(userID);
            //Publish(c.UserID, false);

            inPage.Save();

            EnvironmentVersionLogic.Flush(true, c.Context);

            inPage.SubmitPageForSearch();

            if (c.PagePublication != null)
                c.PagePublication.DoPostPublishValidation(c.User, inPage);

        }
        catch (Exception ex)
        {
            Notification.InsertOne("Threading (page)", ex);
        }
    }

    /// <summary>
    /// Takes down.
    /// </summary>
    /// <param name="userID">The user ID.</param>
    public static void TakeDown(this Page inPage, Sushi.Mediakiwi.Framework.IPagePublication pagePublication, IApplicationUser user)
    {
        Page.ContextContainer context = new Page.ContextContainer();
        context.Context = HttpContext.Current;
        context.User = user;
        context.PagePublication = pagePublication;

        System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(inPage.TakeDownPage), context);

        if (!string.IsNullOrEmpty(inPage.HRef))
            HttpResponse.RemoveOutputCacheItem(inPage.HRef);
    }

    /// <summary>
    /// Takes down page.
    /// </summary>
    /// <param name="context">The context.</param>
    internal static void TakeDownPage(this Page inPage, object context)
    {
        Page.ContextContainer c = (Page.ContextContainer)context;
        try
        {
            inPage.Updated = Common.DatabaseDateTime;
            inPage.Published = DateTime.MinValue;
            inPage.IsPublished = false;
            inPage.Save();

            inPage.CleanUpAfterTakeDown();

            EnvironmentVersionLogic.Flush(true, c.Context);
        }
        catch (Exception ex)
        {
            Notification.InsertOne("Threading (page)", ex);
        }
    }

    /// <summary>
    /// Clean up the component list after take done because some residual components could stil be present.
    /// </summary>
    /// <param name="pageID">The page ID.</param>
    internal static void CleanUpAfterTakeDown(this Page inPage)
    {

        Page.DeleteAllComponentSearchReferences(inPage.ID);
        using (Sushi.Mediakiwi.Data.Connection.DataCommander dac = new Sushi.Mediakiwi.Data.Connection.DataCommander(inPage.SqlConnectionString))
        {
            //  First cleanup ComponentSearch and secondly cleanup Components
            dac.Text = @"delete from wim_Components where Component_Page_Key = @Page_Key";
            dac.SetParameterInput("@Page_Key", inPage.ID, System.Data.SqlDbType.Int);
            dac.ExecNonQuery();
        }
    }

    /// <summary>
    /// Publishes the page
    /// </summary>
    /// <param name="?">The ?.</param>
    /// <param name="user">The user.</param>
    public static void Publish(this Page inPage, Sushi.Mediakiwi.Framework.IPagePublication pagePublication, IApplicationUser user)
    {
        Page.ContextContainer context = new Page.ContextContainer();
        context.Context = HttpContext.Current;
        context.User = user;
        context.PagePublication = pagePublication;

        if (pagePublication.DoPrePublishValidation(user, inPage))
        {
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(inPage.PublishPage), context);

            if (!string.IsNullOrEmpty(inPage.HRef_Short))
                HttpResponse.RemoveOutputCacheItem(inPage.HRef_Short);
        }
    }

    internal static void CopyComponents(this Page inPage, int userID)
    {
        Component[] liveComponents = Component.SelectAllInherited(inPage.ID, true);
        ComponentVersion[] stagingComponents = ComponentVersion.SelectAll(inPage.ID);

        inPage.CopyComponents(userID, liveComponents, stagingComponents);
    }

    /// <summary>
    /// Publishes the specified user ID.
    /// </summary>
    /// <param name="userID">The user ID.</param>
    internal static void CopyComponents(this Page inPage, int userID, Sushi.Mediakiwi.Data.Component[] liveComponents, Sushi.Mediakiwi.Data.ComponentVersion[] stagingComponents)
    {
        Guid batchGuid = Guid.NewGuid();

        int sortOrderCount = 0;

        List<int> foundComponentArr = new List<int>();
        foreach (ComponentVersion componentStaged in stagingComponents)
        {
            if (!componentStaged.IsActive) continue;
            sortOrderCount++;

            bool foundComponent = false;
            foreach (Component component in liveComponents)
            {
                if (component.GUID == componentStaged.GUID)
                {
                    //  Set the found component
                    foundComponentArr.Add(component.ID);

                    componentStaged.Apply(component);
                    component.Save();
                    component.SortOrder = sortOrderCount;
                    foundComponent = true;
                    break;
                }
            }
            if (foundComponent) continue;

            try
            {
                Component component2 = new Component();
                componentStaged.Apply(component2);
                component2.SortOrder = sortOrderCount;
                component2.Save();
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
                component.Delete();
            found = false;
        }
    }


    /// <summary>
    /// Get all property content from the page component 
    /// </summary>
    /// <param name="componentTemplateKey">Component template to search in</param>
    /// <param name="propertyName">The property to look for and return its value</param>
    /// <returns>The property values</returns>
    public static string[] GetComponentProperties(this Page inPage, int componentTemplateKey, string propertyName)
    {
        //  Supporting fields
        List<Content> m_ContentItems = new List<Content>();
        List<Component> m_Components = new List<Component>();

        int m_CurrentCalledcomponentTemplateKey = 0;

        if (m_Components == null || m_Components.Count == 0 || componentTemplateKey != m_CurrentCalledcomponentTemplateKey)
        {
            m_CurrentCalledcomponentTemplateKey = componentTemplateKey;
            m_Components = Component.SelectAll(inPage.ID, componentTemplateKey).ToList();

            List<Content> contentList = new List<Content>();

            foreach (Component component in m_Components)
            {
                //  Get content
                if (component.Serialized_XML == null || component.Serialized_XML.Length == 0)
                    continue;

                //  Get deserialized content
                Content content = Content.GetDeserialized(component.Serialized_XML);

                //  Validate fields
                if (content.Fields == null || content.Fields.Length == 0)
                    continue;

                contentList.Add(content);
            }

            m_ContentItems = contentList;

        }

        if (m_ContentItems == null || m_ContentItems.Count == 0)
            return null;

        List<string> candidates = new List<string>();
        foreach (Content content in m_ContentItems)
        {
            foreach (Content.Field field in content.Fields)
            {
                if (field.Property == propertyName)
                {
                    if (field.Value == null || field.Value.Length == 0)
                        continue;

                    string candidate = field.Value;

                    if (field.Type == (int)Sushi.Mediakiwi.Framework.ContentType.RichText)
                        candidate = Wim.Utility.ApplyRichtextLinks(inPage.Site, field.Value);

                    candidates.Add(candidate);
                }
            }
        }
        return candidates.ToArray();
    }

    /// <summary>
    /// Gets the component properties.
    /// </summary>
    /// <param name="sourceTag">The source tag.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns></returns>
    public static string[] GetComponentProperties(this Page inPage, string sourceTag, string propertyName)
    {
        var ct = ComponentTemplate.SelectOneBySourceTag(sourceTag);
        if (ct == null || ct.IsNewInstance)
            return null;

        return inPage.GetComponentProperties(ct.ID, propertyName);
    }

    /// <summary>
    /// Get the first property content from the page component
    /// </summary>
    /// <param name="componentTemplateKey">Component template to search in</param>
    /// <param name="propertyName">The property to look for and return its value</param>
    /// <returns>The property value</returns>
    public static string GetComponentProperty(this Page inPage, int componentTemplateKey, string propertyName)
    {
        string[] candidates = inPage.GetComponentProperties(componentTemplateKey, propertyName);
        if (candidates != null && candidates.Length > 0)
            return candidates[0];
        return null;
    }

    public static string GetLocalCacheFile(this Page inPage, System.Collections.Specialized.NameValueCollection queryString)
    {
        if (string.IsNullOrEmpty(inPage.Name))
            return null;

        string add = null;
        if (queryString.Count > 0)
        {
            add = "_q";
            foreach (string key in queryString.AllKeys)
            {
                if (key != "?")
                    add += string.Concat("_", key, "_", queryString[key]);
            }
        }

        string tmp = string.Concat("/repository/cache/", inPage.InternalPath, add, ".html");
        return HttpContext.Current.Server.MapPath(Wim.Utility.AddApplicationPath(tmp));
    }

    /// <summary>
    /// Gets the local cache href.
    /// </summary>
    /// <param name="queryString">The query string.</param>
    /// <returns></returns>
    /// <value>The local cache href.</value>
    public static string GetLocalCacheHref(this Page inPage, System.Collections.Specialized.NameValueCollection queryString)
    {
        if (string.IsNullOrEmpty(inPage.Name))
            return null;

        string add = null;
        if (queryString.Count > 0)
        {
            add = "_q";
            foreach (string key in queryString.AllKeys)
            {
                add += string.Concat("_", key, "_", queryString[key]);
            }
        }

        string tmp = string.Concat("/repository/cache/", inPage.InternalPath, add, ".html");
        return Wim.Utility.AddApplicationPath(tmp);
    }

    /// <summary>
    /// Apply a link to this page.
    /// </summary>
    /// <param name="hyperlink">The hyperlink to apply the page linkage properties to.</param>
    public static void Apply(this Page inPage, System.Web.UI.WebControls.HyperLink hyperlink)
    {
        inPage.Apply(hyperlink, false);
    }

    public static void Apply(this Page inPage, System.Web.UI.WebControls.HyperLink hyperlink, bool onlySetNavigationUrl)
    {
        inPage.Apply(hyperlink, onlySetNavigationUrl, false);
    }

    /// <summary>
    /// Apply a link to this page.
    /// </summary>
    /// <param name="hyperlink">The hyperlink to apply the page linkage properties to.</param>
    /// <param name="onlySetNavigationUrl">Only set the navigation url, leave the text property as is.</param>
    public static void Apply(this Page inPage, System.Web.UI.WebControls.HyperLink hyperlink, bool onlySetNavigationUrl, bool setFullUrlPath)
    {
        hyperlink.Visible = false;

        if (inPage.Name == null || inPage.LinkText.Trim().Length == 0)
            return;
        if (inPage.LinkText == null || inPage.LinkText.Trim().Length == 0)
            inPage.LinkText = inPage.Name;

        if (!inPage.IsPublished)
            return;

        if (inPage.Publication != DateTime.MinValue)
        {
            if (DateTime.Now.Ticks < inPage.Publication.Ticks)
                return;
        }
        if (inPage.Expiration != DateTime.MinValue)
        {
            if (DateTime.Now.Ticks > inPage.Expiration.Ticks)
                return;
        }

        if (setFullUrlPath)
            hyperlink.NavigateUrl = inPage.HRefFull;
        else
            hyperlink.NavigateUrl = inPage.HRef;

        if (!onlySetNavigationUrl)
            hyperlink.Text = HttpContext.Current.Server.HtmlEncode(inPage.LinkText);

        hyperlink.ToolTip = inPage.Description == null ? "" : "";
        hyperlink.Visible = true;
    }

}
