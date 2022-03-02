using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.UI;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    /// <summary>
    /// Represents a Folder entity.
    /// </summary>
    public class Folder : BaseImplementation
    {
        [Framework.ContentListItem.Button("Copy folder")]
        public bool Btn_CopyFolder { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="Folder"/> class.
        /// </summary>
        public Folder()
        {
          
            ListInit += Folder_ListInit;
            ListLoad += Folder_ListLoad;
            ListSave += Folder_ListSave;
            ListPreRender += Folder_ListPreRender;
            ListAction += Folder_ListAction;
        }

        private async Task Folder_ListInit()
        {
            wim.OpenInEditMode = true;
            wim.CanSaveAndAddNew = false;
            wim.HideOpenCloseToggle = false;
            wim.HideOpenCloseToggle = true;

        }

        async Task Folder_ListAction(ComponentActionEventArgs e)
        {
            var list = await Mediakiwi.Data.ComponentList.SelectOneAsync(typeof(Copy)).ConfigureAwait(false);
            var url = wim.GetUrl(
                new KeyValue()
                {
                    Key = "type",
                    Value = "1"
                },
                new KeyValue()
                {
                    Key = "list",
                    Value = list.ID.ToString()
                });

            Response.Redirect(url);
        }

        async Task Folder_ListPreRender(ComponentListEventArgs e)
        {
            if (Implement == null || Implement?.ID == 0 || Implement.Parent == null)
            {
                return;
            }

            // When we try to assign the parent folder to ourself, throw error
            if (Implement.Type == FolderType.List && ParentFolder == Implement.ID)
            {
                wim.Notification.AddError(nameof(ParentFolder), "Can not assigned to self!");
            }
        }

        /// <summary>
        /// Handles the ListSave event of the Folder control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ComponentListEventArgs"/> instance containing the event data.</param>
        async Task Folder_ListSave(ComponentListEventArgs e)
        {
            if (wim.CurrentFolder.Type == FolderType.Gallery)
            {
                await SaveGalleryFolderAsync(e).ConfigureAwait(false);
                return;
            }

            if (e.SelectedKey != 0)
            {
                int sortOrder = 0;
                if (wim.CurrentFolder.Type == FolderType.Page)
                {
                    if (SortOrder != null && SortOrder.Items != null)
                    {
                        foreach (SubList.SubListitem item in SortOrder.Items)
                        {
                            sortOrder++;
                            await Page.UpdateSortOrderAsync(item.ID, sortOrder).ConfigureAwait(false);
                        }
                    }
                }
                else if (wim.CurrentFolder.Type == FolderType.List && SortOrder != null && SortOrder.Items != null)
                {
                    foreach (SubList.SubListitem item in SortOrder.Items)
                    {
                        sortOrder++;
                        await Mediakiwi.Data.ComponentList.UpdateSortOrderAsync(item.ID, sortOrder).ConfigureAwait(false);
                    }
                }
            }

            if (e.SelectedKey == 0)
            {
                Implement.SortOrderMethod = SortOrderMethod;
                Implement.Name = Name.Trim();
                Implement.Description = Description;
                Implement.ParentID = wim.CurrentFolder.ID;
                Implement.Type = wim.CurrentFolder.Type;
                Implement.SiteID = wim.CurrentFolder.SiteID;
                Implement.IsVisible = IsVisible;

                if (string.IsNullOrEmpty(wim.CurrentFolder.CompletePath))
                {
                    Implement.CompletePath = string.Concat("/", Name, "/");
                }
                else
                {
                    Implement.CompletePath = string.Concat(wim.CurrentFolder.CompletePath, Name, "/");
                }

                await Implement.SaveAsync().ConfigureAwait(false);

                //  Replicate to children
                await Framework.Inheritance.Folder.CreateFolderAsync(Implement, wim.CurrentSite).ConfigureAwait(false);
            }
            else
            {
                string completePath = "/";
                if (wim.CurrentFolder.ParentID.HasValue)
                {
                    Mediakiwi.Data.Folder parent = await Mediakiwi.Data.Folder.SelectOneAsync(wim.CurrentFolder.ParentID.Value).ConfigureAwait(false);
                    if (Name.Equals(Implement.Name, System.StringComparison.InvariantCultureIgnoreCase) == false)
                    {
                        Name = Implement.GetPageNameProposal(parent.ID, Name);
                    }
                    completePath = string.Concat(parent.CompletePath, Name, "/");
                }

                string oldPath = Implement.CompletePath;
                await Implement.UpdateChildrenAsync(completePath).ConfigureAwait(false);
                Implement.CompletePath = completePath;
                Implement.Name = Name;
                Implement.Description = Description;
                Implement.SortOrderMethod = SortOrderMethod;
                Implement.IsVisible = IsVisible;

                //  Only allowed for lists!
                if (wim.CurrentFolder.Type == FolderType.List)
                {
                    Implement.ParentID = ParentFolder;
                }

                await Implement.SaveAsync().ConfigureAwait(false);
                if (wim.CurrentFolder.Type == FolderType.Page && completePath != oldPath)
                {
                    await Framework.Functions.FolderPathLogic.UpdateCompletePathAsync().ConfigureAwait(false);
                }
                await Mediakiwi.Data.Folder.VerifyCompletePathAsync().ConfigureAwait(false);
            }

            wim.FlushCache();

            if (wim.IsLayerMode)
            {
                wim.OnSaveScript = @"<input type=""hidden"" class=""postParent"">";
            }
            else
            {
                Response.Redirect(string.Concat(wim.Console.WimPagePath, "?folder=", wim.CurrentFolder.ID));
            }
        }

        async Task SaveGalleryFolderAsync(ComponentListEventArgs e)
        {
            if (e.SelectedKey == 0)
            {
                Gallery parent = await Gallery.SelectOneAsync(wim.CurrentFolder.ID).ConfigureAwait(false);

                ImplementGallery = new Gallery();
                if (parent?.ID > 0)
                {
                    ImplementGallery.ParentID = parent.ID;
                }
                ImplementGallery.Name = Name.Trim();
                ImplementGallery.IsFolder = true;
                ImplementGallery.IsFixed = false;
                ImplementGallery.TypeID = 0; 
                ImplementGallery.CompletePath = string.Concat(parent.CompletePath == Mediakiwi.Data.Common.FolderRoot ? "" : parent.CompletePath, "/", ImplementGallery.Name);
                ImplementGallery.IsHidden = !IsVisible;
                ImplementGallery.IsActive = true;

                await ImplementGallery.SaveAsync().ConfigureAwait(false);
            }
            else
            {
                ImplementGallery.Name = Name.Trim();
                ImplementGallery.IsHidden = !IsVisible;

                await ImplementGallery.SaveAsync().ConfigureAwait(false);
            }

            var url = wim.GetUrl(
                    new KeyValue() 
                    { 
                        Key = "item", 
                        Value = ImplementGallery.ID 
                    }, 
                    new KeyValue() 
                    { 
                        Key = "gallery", 
                        Value = ImplementGallery.ID 
                    });

            Response.Redirect(url);
        }

        /// <summary>
        /// Handles the ListLoad event of the Folder control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ComponentListEventArgs"/> instance containing the event data.</param>
        async Task Folder_ListLoad(ComponentListEventArgs e)
        {
            IsExistingFolder = (e.SelectedKey != 0);

            if (wim.CurrentFolder.Type == FolderType.Gallery)
            {
                IsExistingFolder = false;
                await LoadGalleryFolderAsync(e).ConfigureAwait(false);
                return;
            }

            if (IsExistingFolder)
            {
                Implement = await Mediakiwi.Data.Folder.SelectOneAsync(e.SelectedKey).ConfigureAwait(false);
                ParentFolder = Implement.ParentID.GetValueOrDefault();
                GUID = Implement.GUID.ToString();
                IsVisible = Implement.IsVisible;
            }
            else
            {
                Implement = new Mediakiwi.Data.Folder()
                {
                    IsVisible = true
                };
            }

            Utility.ReflectProperty(Implement, this);
            
            if (IsExistingFolder)
            {
                wim.ListTitle = Implement.Name;
            }

            //  Only lists can be reassigned
            if (wim.CurrentFolder.Type != FolderType.List)
            {
                wim.SetPropertyVisibility(nameof(ParentFolder), false);
            }

            if (!wim.CurrentApplicationUser.IsDeveloper)
            {
                wim.SetPropertyVisibility(nameof(GUID), false);
                wim.SetPropertyVisibility(nameof(IsVisible), false);
            }

            if (wim.CurrentFolder.Type == FolderType.Page)
            {
                List<Page> list = null;

                SortOrderMethod = Implement.SortOrderMethod.GetValueOrDefault(5);
                list = await Page.SelectAllAsync(e.SelectedKey, PageFolderSortType.Folder, PageReturnProperySet.All, PageSortBy.SortOrder, false).ConfigureAwait(false);
                SortOrder = new SubList();
                foreach (Page item in list)
                {
                    SubList.SubListitem lo = new SubList.SubListitem(item.ID, item.Name);
                    SortOrder.Add(lo);
                }
            }
            else if (IsExistingFolder && wim.CurrentFolder.Type == FolderType.List)
            {
                var list = await Mediakiwi.Data.ComponentList.SelectAllAsync(e.SelectedKey).ConfigureAwait(false);
                SortOrder = new SubList();
                foreach (IComponentList item in list)
                {
                    SortOrder.Add(new SubList.SubListitem(item.ID, item.Name));
                }
            }

            if (IsExistingFolder && Implement.ChildCount == 0 && !Implement.MasterID.HasValue)
            {
                ListDelete += Folder_ListDelete;
            }

            if (IsExistingFolder && Implement.Level > 0)
            {
                ShowRoles = true;
            }
        }

        async Task LoadGalleryFolderAsync(ComponentListEventArgs e)
        {
            wim.SetPropertyVisibility(nameof(ParentFolder), false);

            if (e.SelectedKey == 0)
            {
                ImplementGallery = new Gallery();
                return;
            }

            ImplementGallery = await Gallery.SelectOneAsync(e.SelectedKey).ConfigureAwait(false);
            GUID = ImplementGallery.GUID.ToString();
            Utility.ReflectProperty(ImplementGallery, this);

            Name = ImplementGallery.Name;
            IsVisible = !ImplementGallery.IsHidden;

            ListDelete += Gallery_ListDelete;
        }

        /// <summary>
        /// Gets a value indicating whether [show roles].
        /// </summary>
        /// <value><c>true</c> if [show roles]; otherwise, <c>false</c>.</value>
        public bool ShowRoles { get; private set; }

        /// <summary>
        /// Handles the ListDelete event of the Folder control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ComponentListEventArgs"/> instance containing the event data.</param>
        async Task Folder_ListDelete(ComponentListEventArgs e)
        {
            if (Implement.ChildCount == 0 && !Implement.MasterID.HasValue)
            {
                await Implement.DeleteAsync().ConfigureAwait(false);
            }

            if (wim.IsLayerMode)
            {
                wim.OnSaveScript = "<input type=\"hidden\" class=\"postParent\">";
            }
        }

        /// <summary>
        /// Handles the ListDelete event of the Gallery control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ComponentListEventArgs"/> instance containing the event data.</param>
        async Task Gallery_ListDelete(ComponentListEventArgs e)
        {
            await ImplementGallery.DeleteAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Gets or sets the implement.
        /// </summary>
        /// <value>The implement.</value>
        [Framework.ContentListItem.DataExtend()]
        public Mediakiwi.Data.Folder Implement { get; set; }

        /// <summary>
        /// Gets or sets the implement.
        /// </summary>
        /// <value>The implement.</value>
        //[Sushi.Mediakiwi.Framework.ContentListItem.DataExtend()]
        public Gallery ImplementGallery { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is existing folder.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is existing folder; otherwise, <c>false</c>.
        /// </value>
        public bool IsExistingFolder { get; private set; }

        /// <summary>
        /// Gets or sets the folder title.
        /// </summary>
        /// <value>The folder title.</value>
        [OnlyVisibleWhenTrue("IsNotRoot")]
        [Framework.ContentListItem.TextField("Title", 50, true, "", Utility.GlobalRegularExpression.NotAcceptableFilenameCharacter)]
        public string Name { get; set; }

        [Framework.ContentListItem.FolderSelect("Parent", true, FolderType.List, "")]
        public int ParentFolder { get; set; }

        [Framework.ContentListItem.TextField("Description", 1024, false)]
        public string Description { get; set; }

        public bool IsNotRoot { get { return !IsExistingFolder || wim.CurrentFolder.ParentID.HasValue; } }

        /// <summary>
        /// Gets a value indicating whether this instance is gallery.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is gallery; otherwise, <c>false</c>.
        /// </value>
        public bool IsGallery
        {
            get
            {
                return (wim.CurrentFolder.Type == FolderType.Gallery);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is gallery.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is gallery; otherwise, <c>false</c>.
        /// </value>
        public bool IsGalleryAndNotNew
        {
            get
            {
                return (wim.CurrentFolder.Type == FolderType.Gallery && ImplementGallery?.ID > 0);
            }
        }

        private ListItemCollection m_SortTypes;
        /// <summary>
        /// Gets or sets the sort types.
        /// </summary>
        /// <value>The sort types.</value>
        public ListItemCollection SortTypes
        {
            get
            {
                if (m_SortTypes == null)
                {
                    m_SortTypes = new ListItemCollection();
                    m_SortTypes.Add(new ListItem("Custom date (ascending)", "1"));
                    m_SortTypes.Add(new ListItem("Custom date (descending)", "2"));
                    m_SortTypes.Add(new ListItem("Linktext", "3"));
                    m_SortTypes.Add(new ListItem("Name of page", "4"));
                    m_SortTypes.Add(new ListItem("SortOrder", "5"));

                }
                return m_SortTypes;
            }
            set { m_SortTypes = value; }
        }

        /// <summary>
        /// Sorts   pages 
        /// </summary>
        [OnlyVisibleWhenTrue(nameof(IsWebPageFolder))]
        [Framework.ContentListItem.Choice_Dropdown("Sortorder", nameof(SortTypes), false)]
        public int SortOrderMethod { get; set; }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        /// <value>The sort order.</value>
        [OnlyVisibleWhenTrue(nameof(IsExistingFolder))]
        [Framework.ContentListItem.SubListSelect("Items", "", false, true)]
        public SubList SortOrder { get; set; }

        [Framework.ContentListItem.Choice_Checkbox("Visible")]
        public bool IsVisible { get; set; } = true;

        public bool IsWebPageFolder
        {
            get { return (wim.CurrentFolder.Type == FolderType.Page); }
        }

        /// <summary>
        /// Gets or sets the GUID.
        /// </summary>
        /// <value>The GUID.</value>
        public string GUID { get; set; }
    }
}
