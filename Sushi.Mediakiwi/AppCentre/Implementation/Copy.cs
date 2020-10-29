using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    public class Copy : BaseImplementation
    {
        //public Copy()
        //{
        //    wim.CanAddNewItem = false;
        //    wim.CanContainSingleInstancePerDefinedList = true;
        //    wim.OpenInEditMode = true;

        //    this.ListLoad += Copy_ListLoad;
        //    this.ListSave += Copy_ListSave;
        //}

        //[Sushi.Mediakiwi.Framework.ContentListItem.TextField("_name", 150, true, null)]
        //public string Name { get; set; }
        //[Sushi.Mediakiwi.Framework.ContentListItem.FolderSelect("_folder", true)]
        //public Sushi.Mediakiwi.Data.Folder FolderID { get; set; }
        //[Sushi.Mediakiwi.Framework.ContentListItem.TextLine("")]
        //public string Information { get; set; }

        //private void Copy_ListLoad(Framework.IComponentListTemplate sender, Framework.ComponentListEventArgs e)
        //{
        //    wim.ListTitle = "Copy folder";
        //    wim.CurrentList.Label_Save = "Copy";

        //    int type = Wim.Utility.ConvertToInt(Request.QueryString["type"]);
        //    switch(type)
        //    {
        //        case 1:
        //            DefineFolder(e);
        //            break;
        //        case 2:
        //            DefinePage(e);
        //            break;
        //    }
        //}

        //Sushi.Mediakiwi.Data.Page[] _Pages;
        //Sushi.Mediakiwi.Data.Folder[] _Folders;
        //Sushi.Mediakiwi.Data.Folder _Folder;
        //Sushi.Mediakiwi.Data.Page _Page;

        //void DefineFolder(Framework.ComponentListEventArgs e)
        //{
        //    _Folder = Sushi.Mediakiwi.Data.Folder.SelectOne(e.SelectedKey);
        //    this.Name = $"{_Folder.Name}";
        //    this.FolderID = Sushi.Mediakiwi.Data.Folder.SelectOne(e.SelectedKey).Parent;

        //    _Pages = Sushi.Mediakiwi.Data.Page.SelectAll(_Folder.CompletePath, true);
        //    _Folders = Sushi.Mediakiwi.Data.Folder.SelectAll(_Folder.Type, _Folder.SiteID, _Folder.CompletePath, true);

        //    this.Information = $"{_Folders.Length} Folders to be copied. {_Pages.Length} Pages to be copied: ";
        //    foreach (var f in _Folders)
        //        this.Information += $"<br/>{f.CompletePath}";
        //    foreach (var p in _Pages)
        //        this.Information += $"<br/>{p.CompletePath}";
        //}

        //void DefinePage(Framework.ComponentListEventArgs e)
        //{
        //    this._Page = Sushi.Mediakiwi.Data.Page.SelectOne(e.SelectedKey);
        //    this.FolderID = Sushi.Mediakiwi.Data.Folder.SelectOne(_Page.FolderID);
        //    this.Name = _Page.Name;

        //    if (_Page.ID == 0)
        //        wim.Page.Body.Form.RefreshParent();
        //}

        //private void Copy_ListSave(Framework.IComponentListTemplate sender, Framework.ComponentListEventArgs e)
        //{
        //    int type = Wim.Utility.ConvertToInt(Request.QueryString["type"]);
        //    switch (type)
        //    {
        //        case 1:
        //            SaveFolder(e);
        //            break;
        //        case 2:
        //            SavePage(e);
        //            break;
        //    }
        //}

        //void SaveFolder(Framework.ComponentListEventArgs e)
        //{
        //    var newRoot = this.FolderID.CompletePath;
        //    var oldRoot = this._Folder.CompletePath;

        //    Dictionary<int, Sushi.Mediakiwi.Data.Folder> newFolders = new Dictionary<int, Sushi.Mediakiwi.Data.Folder>();        
        //    // Old to new page ID
        //    Dictionary<int, Sushi.Mediakiwi.Data.Page> oldNewPageMapping = new Dictionary<int, Sushi.Mediakiwi.Data.Page>();
        //    List<Sushi.Mediakiwi.Data.Link> pageLinksToCheck = new List<Sushi.Mediakiwi.Data.Link>();

        //    this.Information += $"===============================";
        //    foreach (var f in _Folders)
        //    {
        //        var target = f.Clone();
        //        var middle = target.CompletePath.Remove(0, oldRoot.Length);
        //        if (middle.Length > 0)
        //            middle += "/";
        //        else
        //        {
        //            target.Name = this.Name;
        //            target.CompletePath = $"{newRoot}{middle}{target.Name}/";
        //        }

        //        if (target.ParentID == _Folder.ParentID)
        //            target.ParentID = this.FolderID.ID;
        //        else
        //        {
        //            target.CompletePath = $"{newFolders[f.ParentID.Value].CompletePath}{target.Name}/";
        //            target.ParentID = newFolders[f.ParentID.Value].ID;
        //        }

        //        target.Save(false); 
        //        if (!newFolders.ContainsKey(f.ID))
        //            newFolders.Add(f.ID, target);

                 
        //    }
        //    // do all pages
        //    foreach(var page in _Pages)
        //    { 
        //        var newPage = CopyAndPublishPage(page, newFolders[page.FolderID], false, null, newFolders, pageLinksToCheck);
        //        oldNewPageMapping.Add(page.ID, newPage);
        //    };

        //    // pagelink patch id's
        //    foreach (var pageLink in pageLinksToCheck)
        //    {
        //        if (pageLink.PageID.HasValue && oldNewPageMapping.ContainsKey(pageLink.PageID.Value))
        //        {
        //            pageLink.PageID = oldNewPageMapping[pageLink.PageID.Value].ID;
        //            pageLink.Save();
        //        }
        //    }
        //    // page links
        //    var pagePublicationHandler = Sushi.Mediakiwi.Data.Environment.GetInstance<IPagePublication>();
        //    if (oldNewPageMapping != null)
        //    {
        //        foreach (var page in oldNewPageMapping.Values)
        //        { 
        //            var components = Sushi.Mediakiwi.Data.ComponentVersion.SelectAll(page.ID);
        //            foreach (var component in components)
        //            {
        //                bool changed = false;
        //                var fieldList = new List<Sushi.Mediakiwi.Data.Content.Field>();
        //                var currentContent = component.GetContent();
        //                if (currentContent != null && currentContent.Fields != null)
        //                {
        //                    foreach (var field in currentContent.Fields)
        //                    {
        //                        if (field.Type == (int)ContentType.PageSelect)
        //                        {
        //                            int oldPageID = Wim.Utility.ConvertToInt(field.Value, 0);
        //                            if (oldPageID > 0 && oldNewPageMapping.ContainsKey(oldPageID))
        //                            {

        //                                field.Value = oldNewPageMapping[oldPageID].ID.ToString();
        //                                changed = true;
                               
        //                            }
        //                        }
        //                        fieldList.Add(field);
        //                    }
        //                }
        //                if (changed)
        //                {
        //                    var content = new Sushi.Mediakiwi.Data.Content();
        //                    content.Fields = fieldList.ToArray();
        //                    component.Serialized_XML = Sushi.Mediakiwi.Data.Content.GetSerialized(content);
        //                    component.Save(false);
        //                    page.Publish(pagePublicationHandler, wim.CurrentApplicationUser);
        //                }


        //            }
                   
        //        }
        //    }
        //    Wim.Utilities.CacheItemManager.FlushAll();
        //    var u = wim.Console.UrlBuild.GetFolderRequest(this.FolderID);
        //    wim.Page.Body.Form.RefreshParent(u);
        //}

        //Sushi.Mediakiwi.Data.Page CopyAndPublishPage(Sushi.Mediakiwi.Data.Page page, Sushi.Mediakiwi.Data.Folder newFolder, bool unPublishPage = false, string name = null, 
        //    Dictionary<int, Sushi.Mediakiwi.Data.Folder> oldNewFolderMapping = null, List<Sushi.Mediakiwi.Data.Link> pageLinks = null)
        //{
        //    Sushi.Mediakiwi.Data.Page p = new Sushi.Mediakiwi.Data.Page();
        //    Wim.Utility.ReflectProperty(page, p);
        //    p.ID = 0;
        //    p.GUID = Guid.NewGuid();
        //    p.FolderID = newFolder.ID;
        //    p.Folder = newFolder;


        //    // Subfolder re-creation
        //    if (oldNewFolderMapping != null &&
        //        p.SubFolderID > 0 &&
        //        oldNewFolderMapping.ContainsKey(p.SubFolderID))
        //    {
        //        p.SubFolderID = oldNewFolderMapping[p.SubFolderID].ID;
        //    }

        //    if (unPublishPage)  
        //        p.IsPublished = false;
        //    if (name != null)
        //        p.Name = name;
        //    p.Save();

        //    var components = Sushi.Mediakiwi.Data.ComponentVersion.SelectAll(page.ID);
        //    foreach (var c in components)
        //    {
        //        var component = new Sushi.Mediakiwi.Data.ComponentVersion();
        //        Wim.Utility.ReflectProperty(c, component);
        //        p.RecreateLinksInComponentForCopy(c, oldNewFolderMapping,  pageLinks);
        //        c.ID = 0;
        //        c.GUID = Guid.NewGuid();
        //        c.PageID = p.ID;
        //        c.Save(false);
        //    }
        //    if (p.IsPublished)
        //    {
        //        var pagePublicationHandler = Sushi.Mediakiwi.Data.Environment.GetInstance<IPagePublication>();
        //        p.Publish(pagePublicationHandler, wim.CurrentApplicationUser);
        //    }
        //    return p;
        //}

        //void SavePage(Framework.ComponentListEventArgs e)
        //{
        //    var p = CopyAndPublishPage(_Page, FolderID, true, this.Name);
        //    var u = wim.Console.UrlBuild.GetPageRequest(p);
        //    wim.Page.Body.Form.RefreshParent(u);
        //}
    }
}
