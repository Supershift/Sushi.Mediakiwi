using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    public class Copy : BaseImplementation
    {
        public Copy()
        {
            wim.CanAddNewItem = false;
            wim.CanContainSingleInstancePerDefinedList = true;
            wim.OpenInEditMode = true;

            ListLoad += Copy_ListLoad;
            ListSave += Copy_ListSave;
        }

        private async Task Copy_ListSave(ComponentListEventArgs arg)
        {
            int type = Utility.ConvertToInt(Context.Request.Query["type"]);
            switch (type)
            {
                case 1:
                    {
                      await SaveFolderAsync(arg);
                    }
                    break;
                case 2:
                    {
                      await SavePageAsync(arg);
                    }
                    break;
            }
        }

        private async Task Copy_ListLoad(ComponentListEventArgs arg)
        {
            wim.ListTitle = "Copy folder";
            wim.CurrentList.Label_Save = "Copy";
            
            int type = Utility.ConvertToInt(Context.Request.Query["type"]);
            switch (type)
            {
                case 1:
                    {
                        await DefineFolderAsync(arg);
                    }
                    break;
                case 2:
                    {
                        await DefinePageAsync(arg);
                    }
                    break;
            }
        }

        [Framework.ContentListItem.TextField("_name", 150, true, null)]
        public string Name { get; set; }

        [Framework.ContentListItem.FolderSelect("_folder", true)]
        public Mediakiwi.Data.Folder FolderID { get; set; }

        [Framework.ContentListItem.TextLine("")]
        public string Information { get; set; }


        Page[] _Pages;
        Mediakiwi.Data.Folder[] _Folders;
        Mediakiwi.Data.Folder _Folder;
        Page _Page;

        private async Task DefineFolderAsync(ComponentListEventArgs e)
        {
            _Folder = await Mediakiwi.Data.Folder.SelectOneAsync(e.SelectedKey);
            Name = $"{_Folder.Name}";
            FolderID = (await Mediakiwi.Data.Folder.SelectOneAsync(e.SelectedKey)).Parent;

            _Pages = await Page.SelectAllAsync(_Folder.CompletePath, true);
            _Folders = await Mediakiwi.Data.Folder.SelectAllAsync(_Folder.Type, _Folder.SiteID, _Folder.CompletePath, true);

            Information = $"{_Folders.Length} Folders to be copied. {_Pages.Length} Pages to be copied: ";
            foreach (var f in _Folders)
            {
                Information += $"<br/>{f.CompletePath}";
            }

            foreach (var p in _Pages)
            {
                Information += $"<br/>{p.CompletePath}";
            }
        }

        private async Task DefinePageAsync(ComponentListEventArgs e)
        {
            _Page = await Page.SelectOneAsync(e.SelectedKey);
            FolderID = await Mediakiwi.Data.Folder.SelectOneAsync(_Page.FolderID);
            Name = _Page.Name;

            if (_Page.ID == 0)
            {
                wim.Page.Body.Form.RefreshParent();
            }
        }

        private async Task SaveFolderAsync(ComponentListEventArgs e)
        {
            var newRoot = FolderID.CompletePath;
            var oldRoot = _Folder.CompletePath;

            Dictionary<int, Mediakiwi.Data.Folder> newFolders = new Dictionary<int, Mediakiwi.Data.Folder>();
            // Old to new page ID
            Dictionary<int, Page> oldNewPageMapping = new Dictionary<int, Page>();
            List<Mediakiwi.Data.Link> pageLinksToCheck = new List<Mediakiwi.Data.Link>();

            Information += $"===============================";
            foreach (var f in _Folders)
            {
                var target = f.Clone();
                var middle = target.CompletePath.Remove(0, oldRoot.Length);
                if (middle.Length > 0)
                {
                    middle += "/";
                }
                else
                {
                    target.Name = Name;
                    target.CompletePath = $"{newRoot}{middle}{target.Name}/";
                }

                if (target.ParentID == _Folder.ParentID)
                {
                    target.ParentID = FolderID.ID;
                }
                else
                {
                    target.CompletePath = $"{newFolders[f.ParentID.Value].CompletePath}{target.Name}/";
                    target.ParentID = newFolders[f.ParentID.Value].ID;
                }

                await target.SaveAsync();
                if (!newFolders.ContainsKey(f.ID))
                {
                    newFolders.Add(f.ID, target);
                }
            }

            // do all pages
            foreach (var page in _Pages)
            {
                var newPage = await CopyAndPublishPageAsync(page, newFolders[page.FolderID], false, null, newFolders, pageLinksToCheck);
                oldNewPageMapping.Add(page.ID, newPage);
            }

            // pagelink patch id's
            foreach (var pageLink in pageLinksToCheck)
            {
                if (pageLink.PageID.HasValue && oldNewPageMapping.ContainsKey(pageLink.PageID.Value))
                {
                    pageLink.PageID = oldNewPageMapping[pageLink.PageID.Value].ID;
                    await pageLink.SaveAsync();
                }
            }

            // page links
            //var pagePublicationHandler = Mediakiwi.Data.Environment.GetInstance<IPagePublication>();
            if (oldNewPageMapping != null)
            {
                foreach (var page in oldNewPageMapping.Values)
                {
                    var components = await ComponentVersion.SelectAllAsync(page.ID);
                    foreach (var component in components)
                    {
                        bool changed = false;
                        var fieldList = new List<Field>();
                        var currentContent = component.GetContent();
                        if (currentContent != null && currentContent.Fields != null)
                        {
                            foreach (var field in currentContent.Fields)
                            {
                                if (field.Type == (int)ContentType.PageSelect)
                                {
                                    int oldPageID = Utility.ConvertToInt(field.Value, 0);
                                    if (oldPageID > 0 && oldNewPageMapping.ContainsKey(oldPageID))
                                    {
                                        field.Value = oldNewPageMapping[oldPageID].ID.ToString();
                                        changed = true;
                                    }
                                }
                                fieldList.Add(field);
                            }
                        }
                        if (changed)
                        {
                            var content = new Content();
                            content.Fields = fieldList.ToArray();
                            component.Serialized_XML = Content.GetSerialized(content);
                            await component.SaveAsync();
                            //page.Publish(pagePublicationHandler, wim.CurrentApplicationUser);
                        }
                    }
                }
            }

            // TODO: Enable this functionality [MR:17-06-2021]
            // Wim.Utilities.CacheItemManager.FlushAll();
            var u = wim.Console.UrlBuild.GetFolderRequest(FolderID);
            wim.Page.Body.Form.RefreshParent(u);
        }

       private async Task<Page> CopyAndPublishPageAsync(
           Page page, 
           Mediakiwi.Data.Folder newFolder, 
           bool unPublishPage = false, 
           string name = null,
           Dictionary<int, Mediakiwi.Data.Folder> oldNewFolderMapping = null, 
           List<Mediakiwi.Data.Link> pageLinks = null)
        {
            Page p = new Page();
            Utility.ReflectProperty(page, p);

            p.ID = 0;
            p.GUID = Guid.NewGuid();
            p.FolderID = newFolder.ID;
            p.Folder = newFolder;


            // Subfolder re-creation
            if (oldNewFolderMapping != null && p.SubFolderID > 0 && oldNewFolderMapping.ContainsKey(p.SubFolderID))
            {
                p.SubFolderID = oldNewFolderMapping[p.SubFolderID].ID;
            }

            if (unPublishPage)
            {
                p.IsPublished = false;
            }

            if (name != null)
            {
                p.Name = name;
            }

            await p.SaveAsync();

            var components = await ComponentVersion.SelectAllAsync(page.ID);
            foreach (var c in components)
            {
                var component = new ComponentVersion();

                Utility.ReflectProperty(c, component);
                await p.RecreateLinksInComponentForCopyAsync(c, oldNewFolderMapping, pageLinks);
                c.ID = 0;
                c.GUID = Guid.NewGuid();
                c.PageID = p.ID;
                await c.SaveAsync();
            }

            //if (p.IsPublished)
            //{
            //    var pagePublicationHandler = Mediakiwi.Data.Environment.GetInstance<IPagePublication>();
            //    p.Publish(pagePublicationHandler, wim.CurrentApplicationUser);
            //}

            return p;
        }

        private async Task SavePageAsync(ComponentListEventArgs e)
        {
            var p = await CopyAndPublishPageAsync(_Page, FolderID, true, Name);
            var u = wim.Console.UrlBuild.GetPageRequest(p);
            wim.Page.Body.Form.RefreshParent(u);
        }
    }
}
