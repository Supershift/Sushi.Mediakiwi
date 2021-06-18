using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.Data;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    /// <summary>
    /// Represents a Site entity.
    /// </summary>
    public class Site : BaseImplementation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Site"/> class.
        /// </summary>
        public Site()
        {
            wim.CanAddNewItem = true;

            this.ListSearch += Site_ListSearch;
            this.ListLoad += Site_ListLoad;
            this.ListSave += Site_ListSave;
            //this.ListAction += new ComponentActionEventHandler(Site_ListAction);
        }

        //[Sushi.Mediakiwi.Framework.ContentListItem.Button("Disconnect inheritence")]
        //public bool Disconnect { get; set; } 

        void Site_ListAction(object sender, ComponentActionEventArgs e)
        {
            //if (Disconnect)
            //{
            //    if (this.m_Implement.MasterID.HasValue)
            //    {
            //        var items = Sushi.Mediakiwi.Data.ComponentList.SelectAllBySite(this.m_Implement.MasterID.Value);

            //        foreach (var item in items)
            //        {
            //            Sushi.Mediakiwi.Data.ComponentList list = new Sushi.Mediakiwi.Data.ComponentList();
            //            Wim.Utility.ReflectProperty(item, list);
            //            list.ID = 0;
            //            list.SiteID = e.SelectedKey;
            //            list.FolderID = Sushi.Mediakiwi.Data.Folder.SelectOne(item.FolderID.Value, e.SelectedKey).ID;
            //            list.Save();
            //        }
            //    }

            //    Sushi.Mediakiwi.Framework.Inheritance.Folder.RemoveInheritence(e.SelectedKey, Sushi.Mediakiwi.Data.FolderType.Page);
            //    Sushi.Mediakiwi.Framework.Inheritance.Folder.RemoveInheritence(e.SelectedKey, Sushi.Mediakiwi.Data.FolderType.List);
            //    Sushi.Mediakiwi.Framework.Inheritance.Page.RemoveInheritence(e.SelectedKey);
            //    Response.Redirect(wim.GetCurrentQueryUrl(true));
            //}
        }

        /// <summary>
        /// Handles the ListDelete event of the Site control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentListEventArgs"/> instance containing the event data.</param>
        async Task Site_ListDelete(ComponentListEventArgs e)
        {
            await this.Implement.DeleteAsync();
        }

        /// <summary>
        /// Creates the site folders.
        /// </summary>
        /// <param name="siteID">The site ID.</param>
        //void CreateSiteFolders(int siteID)
        //{
        //    Sushi.Mediakiwi.Data.Folder webFolder = new Sushi.Mediakiwi.Data.Folder();
        //    webFolder.Type = Sushi.Mediakiwi.Data.FolderType.Page;
        //    webFolder.Name = Sushi.Mediakiwi.Data.Common.FolderRoot;
        //    webFolder.CompletePath = Sushi.Mediakiwi.Data.Common.FolderRoot;
        //    webFolder.SiteID = siteID;
        //    webFolder.Save();

        //    Sushi.Mediakiwi.Data.Folder logicFolder = new Sushi.Mediakiwi.Data.Folder();
        //    logicFolder.Type = Sushi.Mediakiwi.Data.FolderType.List;
        //    logicFolder.Name = Sushi.Mediakiwi.Data.Common.FolderRoot;
        //    logicFolder.CompletePath = Sushi.Mediakiwi.Data.Common.FolderRoot;
        //    logicFolder.SiteID = siteID;
        //    logicFolder.Save();
        //}

        /// <summary>
        /// Handles the ListSave event of the Site control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentListEventArgs"/> instance containing the event data.</param>
        async Task Site_ListSave(ComponentListEventArgs e)
        {
            await this.Implement.SaveAsync();

            if (this.Implement.HasLists)
            {
                var listsite = Sushi.Mediakiwi.Data.Folder.SelectOneBySite(this.Implement.ID, FolderType.List);
                if (listsite == null || listsite.ID == 0)
                {
                    var folder = new Mediakiwi.Data.Folder();
                    folder.IsVisible = true;
                    folder.Type = FolderType.List;
                    folder.Name = Common.FolderRoot;
                    folder.CompletePath = Common.FolderRoot;
                    folder.SiteID = this.Implement.ID;
                    folder.Save();
                }
            }
            if (this.Implement.HasPages)
            {
                var listsite = Sushi.Mediakiwi.Data.Folder.SelectOneBySite(this.Implement.ID, FolderType.Page);
                if (listsite == null || listsite.ID == 0)
                {
                    var folder = new Mediakiwi.Data.Folder();
                    folder.IsVisible = true;
                    folder.Type = FolderType.Page;
                    folder.Name = Common.FolderRoot;
                    folder.CompletePath = Common.FolderRoot;
                    folder.SiteID = this.Implement.ID;
                    folder.Save();
                }
            }

            ResetDefaultFolder();

            Response.Redirect(wim.GetUrl(new KeyValue() { Key = "item", Value = this.Implement.ID }));
        }

        /// <summary>
        /// Resets the default folder.
        /// </summary>
        private void ResetDefaultFolder()
        {
            if (!string.IsNullOrEmpty(Implement.DefaultFolder))
            {
                while ((Implement.DefaultFolder.LastIndexOf("/") + 1) == Implement.DefaultFolder.Length)
                {
                    if (Implement.DefaultFolder.Length == 1)
                    {
                        Implement.DefaultFolder = string.Empty;
                        break;
                    }
                    else
                        Implement.DefaultFolder = Implement.DefaultFolder.Substring(0, Implement.DefaultFolder.Length - 1);
                }

                if (Implement.DefaultFolder.Length > 0)
                {
                    if (Implement.DefaultFolder.ToCharArray()[0] != '/')
                        Implement.DefaultFolder = string.Format("/{0}", Implement.DefaultFolder);
                }
            }
            wim.FlushCache();
            Sushi.Mediakiwi.Framework.Functions.FolderPathLogic.UpdateCompletePath();

            // if a master is connected, created the inheritance tree
            if (Implement.MasterID.HasValue && Implement.HasLists)
            {
                Sushi.Mediakiwi.Framework.Inheritance.Folder.CreateFolderTree(Implement.MasterID.Value, Implement.ID, FolderType.List);
            }
        }

        Sushi.Mediakiwi.Data.Site m_Implement;
        /// <summary>
        /// Gets or sets the implement.
        /// </summary>
        /// <value>The implement.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.DataExtend()]
        public Sushi.Mediakiwi.Data.Site Implement
        {
            get { return m_Implement; }
            set { m_Implement = value; }
        }

        /// <summary>
        /// Handles the ListLoad event of the Site control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentListEventArgs"/> instance containing the event data.</param>
        async Task Site_ListLoad(ComponentListEventArgs e)
        {
            this.Implement = await Mediakiwi.Data.Site.SelectOneAsync(e.SelectedKey);
            this.m_InheritenceIsSet = this.Implement.MasterID.HasValue;

            if (!Implement.HasPages && !Implement.HasLists)
                this.ListDelete += Site_ListDelete;

            //ResetDefaultFolder();

            wim.CurrentSite = this.Implement;
            this.FormMaps.Add(new Forms.SiteForm(this.Implement));
        }

        /// <summary>
        /// Site_s the list search.
        /// </summary>
        /// <param name="sender">The sender.</param>
        async Task Site_ListSearch(ComponentListSearchEventArgs e)
        {
            wim.CanAddNewItem = true;
            wim.ListDataColumns.Add("ID", "ID", ListDataColumnType.UniqueIdentifier);
            wim.ListDataColumns.Add("Site", "Name", ListDataColumnType.HighlightPresent);
            wim.ListDataColumns.Add("Inherits from", "Master");
            wim.ListDataColumns.Add("Active", "IsActive", 60);
            wim.ListDataColumns.Add("Pages", "PageCount", 60);
            wim.ListDataColumns.Add("Lists", "ListCount", 60);
            wim.ListDataColumns.Add("Created", "Created", 100);
            if (wim.IsCachedSearchResult)
                return;

            if (m_SearchSiteName == null)
                wim.ListDataAdd(await Sushi.Mediakiwi.Data.Site.SelectAllAsync());
            else
                wim.ListDataAdd(await Sushi.Mediakiwi.Data.Site.SelectAllAsync(m_SearchSiteName));
        }

        private string m_SearchSiteName;
        /// <summary>
        /// Gets or sets the name of the search site.
        /// </summary>
        /// <value>The name of the search site.</value>
        [Sushi.Mediakiwi.Framework.ContentListSearchItem.TextField("Site title", 100, false)]
        public string SearchSiteName
        {
            get { return m_SearchSiteName; }
            set { m_SearchSiteName = value; }
        }

        bool m_InheritenceIsSet;
        /// <summary>
        /// Gets a value indicating whether [inheritence is set].
        /// </summary>
        /// <value><c>true</c> if [inheritence is set]; otherwise, <c>false</c>.</value>
        public bool InheritenceIsSet
        {
            get { return m_InheritenceIsSet; }
        }
    }
}
