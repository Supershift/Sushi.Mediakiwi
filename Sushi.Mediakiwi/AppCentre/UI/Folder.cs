using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.UI;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    /// <summary>
    /// Represents a Folder entity.
    /// </summary>
    public class Folder : BaseImplementation
    {
        [Sushi.Mediakiwi.Framework.ContentListItem.Button("Copy folder")]
        public bool Btn_CopyFolder { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Folder"/> class.
        /// </summary>
        public Folder()
        {
            wim.OpenInEditMode = true;
            wim.CanSaveAndAddNew = false;
            wim.HideOpenCloseToggle = false;
            wim.HideOpenCloseToggle = true;

            this.IsVisible = true;

            this.ListLoad += new ComponentListEventHandler(Folder_ListLoad);
            this.ListSave += new ComponentListEventHandler(Folder_ListSave);
            this.ListPreRender += Folder_ListPreRender;
            this.ListAction += Folder_ListAction;
        }

        private void Folder_ListAction(object sender, ComponentActionEventArgs e)
        {
            var list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(typeof(Sushi.Mediakiwi.AppCentre.Data.Implementation.Copy));
            Response.Redirect(wim.GetCurrentQueryUrl(true, new KeyValue() { Key = "type", Value = "1" }, new KeyValue() { Key = "list", Value = list.ID.ToString() }));
        }

        void Folder_ListPreRender(object sender, ComponentListEventArgs e)
        {
            if (this.Implement == null || this.Implement.Parent == null)
                return;

            if (this.Implement.Type == Sushi.Mediakiwi.Data.FolderType.List && this.Implement.ParentID.GetValueOrDefault() == this.ParentFolder)
            {
                wim.Notification.AddError("ParentID", "Can not assigned to self!");
            }
        }

        /// <summary>
        /// Handles the ListSave event of the Folder control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentListEventArgs"/> instance containing the event data.</param>
        void Folder_ListSave(object sender, ComponentListEventArgs e)
        {
            if (wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Gallery)
            {
                SaveGalleryFolder(e);
                return;
            }
            bool folderHasMoved = false;
            if (e.SelectedKey != 0)
            {
                int sortOrder = 0;
                if (wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Page)
                {
                    if (m_SortOrder != null && m_SortOrder.Items != null)
                    {
                        foreach (Sushi.Mediakiwi.Data.SubList.SubListitem item in m_SortOrder.Items)
                        {
                            sortOrder++;
                            Sushi.Mediakiwi.Data.Page.UpdateSortOrder(item.ID, sortOrder);
                        }
                    }
                }
                else if (wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.List)
                {
                    folderHasMoved = (m_Implement.ParentID != this.ParentFolder);
                    {
                    }

                    if (m_SortOrder != null && m_SortOrder.Items != null)
                    {
                        foreach (Sushi.Mediakiwi.Data.SubList.SubListitem item in m_SortOrder.Items)
                        {
                            sortOrder++;
                            Sushi.Mediakiwi.Data.ComponentList.UpdateSortOrder(item.ID, sortOrder);
                        }
                    }
                }
            }

            if (e.SelectedKey == 0)
            {
                m_Implement.SortOrderMethod = this.SortOrderMethod;
                m_Implement.Name = this.Name.Trim();
                m_Implement.Description = this.Description;
                m_Implement.ParentID = wim.CurrentFolder.ID;
                m_Implement.Type = wim.CurrentFolder.Type;
                m_Implement.SiteID = wim.CurrentFolder.SiteID;
                m_Implement.IsVisible = this.IsVisible;

                if (string.IsNullOrEmpty(wim.CurrentFolder.CompletePath))
                    m_Implement.CompletePath = string.Concat("/", this.Name, "/");
                else
                    m_Implement.CompletePath = string.Concat(wim.CurrentFolder.CompletePath, this.Name, "/");

                m_Implement.Save();
                int masterId = m_Implement.ID;

                //  Replicate to children
                Sushi.Mediakiwi.Framework.Inheritance.Folder.CreateFolder(m_Implement, wim.CurrentSite);
            }
            else
            {
                // CB 29-5-2012; Aanpassing zodat de root folder ook opgeslagen kan worden
                string completePath = "/";
                if (wim.CurrentFolder.ParentID.HasValue)
                {
                    Sushi.Mediakiwi.Data.Folder parent = Sushi.Mediakiwi.Data.Folder.SelectOne(wim.CurrentFolder.ParentID.Value);
                    completePath = string.Concat(parent.CompletePath, this.Name, "/");
                    if (this.Name != Implement.Name)
                        this.Name = m_Implement.GetPageNameProposal(parent.ID, this.Name);
                    completePath = string.Concat(parent.CompletePath, this.Name, "/");
                }
                // CB end
     
                string oldPath = m_Implement.CompletePath;
                m_Implement.UpdateChildren(completePath);
                m_Implement.CompletePath = completePath;
                m_Implement.Name = this.Name;
                m_Implement.Description = this.Description;
                m_Implement.SortOrderMethod = this.m_SortOrderMethod;
                m_Implement.IsVisible = this.IsVisible;
                
                //  Only allowed for lists!
                if (wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.List)
                    m_Implement.ParentID = this.ParentFolder;

                m_Implement.Save();
                if (wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Page && completePath != oldPath)
                {
                    Framework.Functions.FolderPathLogic.UpdateCompletePath();
                }
                Sushi.Mediakiwi.Data.Folder.VerifyCompletePath();
                wim.FlushCache();
            }

            //Sushi.Mediakiwi.Data.RoleRight.UpdateFolder(m_Roles, m_Implement.ID);
            wim.FlushCache();

            if (wim.IsLayerMode)
                wim.OnSaveScript = string.Format(@"<input type=""hidden"" class=""postParent"">");
            else
                Response.Redirect(string.Concat(wim.Console.WimPagePath, "?folder=", wim.CurrentFolder.ID));
            
        }

        void SaveGalleryFolder(ComponentListEventArgs e)
        {
            if (e.SelectedKey == 0)
            {
                Sushi.Mediakiwi.Data.Gallery parent = Sushi.Mediakiwi.Data.Gallery.SelectOne(wim.CurrentFolder.ID);

                ImplementGallery = new Sushi.Mediakiwi.Data.Gallery();
                ImplementGallery.ParentID = parent.ID;
                ImplementGallery.Name = this.Name.Trim();
                ImplementGallery.IsFolder = true;
                ImplementGallery.IsFixed = false;
                ImplementGallery.TypeID = 0;// this.TypeID;
                ImplementGallery.CompletePath = string.Concat(parent.CompletePath == Sushi.Mediakiwi.Data.Common.FolderRoot ? "" : parent.CompletePath, "/", ImplementGallery.Name);
                ImplementGallery.IsHidden =! this.IsVisible;
                ImplementGallery.Save();
            }
            else
            {
                ImplementGallery.Name = this.Name.Trim();
                ImplementGallery.IsHidden = !this.IsVisible;
                ImplementGallery.Save();
            }

            var url = wim.GetCurrentQueryUrl(true
                , new KeyValue() { Key = "item", Value = ImplementGallery.ID }
                , new KeyValue() { Key = "gallery", Value = ImplementGallery.ID });
            Response.Redirect(url);
        }

        /// <summary>
        /// Handles the ListLoad event of the Folder control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentListEventArgs"/> instance containing the event data.</param>
        void Folder_ListLoad(object sender, ComponentListEventArgs e)
        {
            m_IsExistingFolder = !(e.SelectedKey == 0);

            if (wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Gallery)
            {
                m_IsExistingFolder = false;
                LoadGalleryFolder(e);
                return;
            }

            this.Implement = Sushi.Mediakiwi.Data.Folder.SelectOne(e.SelectedKey);
            this.ParentFolder = 
                this.Implement.ParentID.GetValueOrDefault();
            
            this.GUID = this.Implement.GUID.ToString();
            this.IsVisible = this.Implement.IsVisible;
            if (e.SelectedKey == 0) return;

            wim.ListTitle = this.Implement.Name;

            Utility.ReflectProperty(m_Implement, this);

            //  Only lists can be reassigned
            if (wim.CurrentFolder.Type != Sushi.Mediakiwi.Data.FolderType.List)
                wim.SetPropertyVisibility("ParentFolder", false);

            if (!wim.CurrentApplicationUser.IsDeveloper)
            {
                wim.SetPropertyVisibility("GUID", false);
                wim.SetPropertyVisibility("IsVisible", false);
            }

            if (wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Page)
            {
                List<Sushi.Mediakiwi.Data.Page> list = null;
                Sushi.Mediakiwi.Data.PageSortBy sortOrder = Sushi.Mediakiwi.Data.PageSortBy.SortOrder;
                if (Implement.SortOrderMethod.HasValue)
                {
                    switch (Implement.SortOrderMethod.Value)
                    {
                        case 1:
                            sortOrder = Sushi.Mediakiwi.Data.PageSortBy.CustomDate;
                            break;
                        case 2:
                            sortOrder = Sushi.Mediakiwi.Data.PageSortBy.CustomDateDown;
                            break;
                        case 3:
                            sortOrder = Sushi.Mediakiwi.Data.PageSortBy.LinkText;
                            break;
                        case 4:
                            sortOrder = Sushi.Mediakiwi.Data.PageSortBy.Name;
                            break;
                        case 5:
                        default:
                            sortOrder = Sushi.Mediakiwi.Data.PageSortBy.SortOrder;
                            break;
                    }
                }
                SortOrderMethod = Implement.SortOrderMethod.GetValueOrDefault(5);
                list = Sushi.Mediakiwi.Data.Page.SelectAll(e.SelectedKey, Sushi.Mediakiwi.Data.PageFolderSortType.Folder, Sushi.Mediakiwi.Data.PageReturnProperySet.All, Sushi.Mediakiwi.Data.PageSortBy.SortOrder, false);
                m_SortOrder = new Sushi.Mediakiwi.Data.SubList();
                foreach (Sushi.Mediakiwi.Data.Page item in list)
                {
                    Sushi.Mediakiwi.Data.SubList.SubListitem lo = new Sushi.Mediakiwi.Data.SubList.SubListitem(item.ID, item.Name);                    
                    m_SortOrder.Add(lo);
                }
            }
            else if (wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.List)
            {
                Sushi.Mediakiwi.Data.IComponentList[] list = Sushi.Mediakiwi.Data.ComponentList.SelectAll(e.SelectedKey);
                m_SortOrder = new Sushi.Mediakiwi.Data.SubList();
                foreach (Sushi.Mediakiwi.Data.IComponentList item in list)
                {
                    m_SortOrder.Add(new Sushi.Mediakiwi.Data.SubList.SubListitem(item.ID, item.Name));
                }
            }

            if (m_Implement.ChildCount == 0 && !m_Implement.MasterID.HasValue)
                this.ListDelete += new ComponentListEventHandler(Folder_ListDelete);

            if (m_Implement.Level > 0 && e.SelectedKey > 0)
                m_ShowRoles = true;
        }

        void LoadGalleryFolder(ComponentListEventArgs e)
        {
            wim.SetPropertyVisibility("ParentFolder", false);

            if (e.SelectedKey == 0)
            {
                this.ImplementGallery = new Sushi.Mediakiwi.Data.Gallery();
                return;
            }

            this.ImplementGallery = Sushi.Mediakiwi.Data.Gallery.SelectOne(e.SelectedKey);
            this.GUID = this.ImplementGallery.GUID.ToString();
            Utility.ReflectProperty(ImplementGallery, this);

            this.Name = ImplementGallery.Name;
            this.IsVisible = !ImplementGallery.IsHidden;

            this.ListDelete += new ComponentListEventHandler(Gallery_ListDelete);
        }

        bool m_ShowRoles;
        /// <summary>
        /// Gets a value indicating whether [show roles].
        /// </summary>
        /// <value><c>true</c> if [show roles]; otherwise, <c>false</c>.</value>
        public bool ShowRoles
        {
            get { return m_ShowRoles; }
        }

        /// <summary>
        /// Handles the ListDelete event of the Folder control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentListEventArgs"/> instance containing the event data.</param>
        void Folder_ListDelete(object sender, ComponentListEventArgs e)
        {
            if (m_Implement.ChildCount == 0 && !m_Implement.MasterID.HasValue)
            {
                m_Implement.Delete();
            }
            if (wim.IsLayerMode)
                wim.OnSaveScript = string.Format(@"<input type=""hidden"" class=""postParent"">");
        }

        /// <summary>
        /// Handles the ListDelete event of the Gallery control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentListEventArgs"/> instance containing the event data.</param>
        void Gallery_ListDelete(object sender, ComponentListEventArgs e)
        {
            //if (m_ImplementGallery.AssetCount2 == 0)
            //{
                m_ImplementGallery.Delete();
            //}

        }

        Sushi.Mediakiwi.Data.Folder m_Implement;
        /// <summary>
        /// Gets or sets the implement.
        /// </summary>
        /// <value>The implement.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.DataExtend()]
        public Sushi.Mediakiwi.Data.Folder Implement
        {
            get { return m_Implement; }
            set { m_Implement = value; }
        }

        Sushi.Mediakiwi.Data.Gallery m_ImplementGallery;
        /// <summary>
        /// Gets or sets the implement.
        /// </summary>
        /// <value>The implement.</value>
        //[Sushi.Mediakiwi.Framework.ContentListItem.DataExtend()]
        public Sushi.Mediakiwi.Data.Gallery ImplementGallery
        {
            get { return m_ImplementGallery; }
            set { m_ImplementGallery = value; }
        }

        bool m_IsExistingFolder;
        /// <summary>
        /// Gets a value indicating whether this instance is existing folder.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is existing folder; otherwise, <c>false</c>.
        /// </value>
        public bool IsExistingFolder
        {
            get { return m_IsExistingFolder; }
        }

        //private string m_Name;
        ///// <summary>
        ///// Gets or sets the name.
        ///// </summary>
        ///// <value>The name.</value>
        //[Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsExistingFolder")]
        //[Sushi.Mediakiwi.Framework.ContentListItem.TextLine("Title")]
        //public string Name
        //{
        //    get { return m_Name; }
        //    set { m_Name = value; }
        //}
        //[Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsExistingFolder", false)]

        /// <summary>
        /// Gets or sets the folder title.
        /// </summary>
        /// <value>The folder title.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsNotRoot")]
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Title", 25, true, "", Utility.GlobalRegularExpression.NotAcceptableFilenameCharacter)]
        public string Name { get; set; }

        [Sushi.Mediakiwi.Framework.ContentListItem.FolderSelect("Parent", true, Sushi.Mediakiwi.Data.FolderType.List, "")]
        public int ParentFolder { get; set; }

        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Description", 1024, false)]
        public string Description { get; set; }

        public bool IsNotRoot { get { return !IsExistingFolder || wim.CurrentFolder.ParentID.HasValue; } }

        /// <summary>
        /// Gets or sets the is hidden.
        /// </summary>
        /// <value>The is hidden.</value>
        //[Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsGallery")]
        //[Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Is hidden")]
        //public bool IsHidden { get; set; }

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
                return (wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Gallery);
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
            get { 
                return (wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Gallery && !(ImplementGallery == null || ImplementGallery.ID == 0)); 
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

        private int m_SortOrderMethod;
        /// <summary>
        /// Sorts   pages 
        /// </summary>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsWebPageFolder")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Sortorder", "SortTypes", false)]
        public int SortOrderMethod
        {
            get { return m_SortOrderMethod; }
            set { m_SortOrderMethod = value; }
        }

        private Sushi.Mediakiwi.Data.SubList m_SortOrder;
        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        /// <value>The sort order.</value>
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsExistingFolder")]
        [Sushi.Mediakiwi.Framework.ContentListItem.SubListSelect("Items", "", false, true)]
        public Sushi.Mediakiwi.Data.SubList SortOrder
        {
            get { return m_SortOrder; }
            set { m_SortOrder = value; }
        }

        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Visible")]
        public bool IsVisible { get; set; } = true;

        public bool IsWebPageFolder
        {
            get { return (wim.CurrentFolder.Type == Sushi.Mediakiwi.Data.FolderType.Page); }
        }

        /// <summary>
        /// Gets or sets the GUID.
        /// </summary>
        /// <value>The GUID.</value>
        public string GUID { get; set; }
    }
}
