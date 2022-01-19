using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
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

            ListSearch += Site_ListSearch;
            ListLoad += Site_ListLoad;
            ListSave += Site_ListSave;
            //ListAction += new ComponentActionEventHandler(Site_ListAction);
        }

        //[Sushi.Mediakiwi.Framework.ContentListItem.Button("Disconnect inheritence")]
        //public bool Disconnect { get; set; } 

        void Site_ListAction(object sender, ComponentActionEventArgs e)
        {
            //if (Disconnect)
            //{
            //    if (m_Implement.MasterID.HasValue)
            //    {
            //        var items = Sushi.Mediakiwi.Data.ComponentList.SelectAllBySite(m_Implement.MasterID.Value);

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
        /// <param name="e">The <see cref="ComponentListEventArgs"/> instance containing the event data.</param>
        async Task Site_ListDelete(ComponentListEventArgs e)
        {
            await Implement.DeleteAsync().ConfigureAwait(false);
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
        /// <param name="e">The <see cref="ComponentListEventArgs"/> instance containing the event data.</param>
        async Task Site_ListSave(ComponentListEventArgs e)
        {
            await Implement.SaveAsync().ConfigureAwait(false);

            if (Implement.HasLists)
            {
                var listsite = await Mediakiwi.Data.Folder.SelectOneBySiteAsync(Implement.ID, FolderType.List).ConfigureAwait(false);
                if (listsite == null || listsite.ID == 0)
                {
                    var folder = new Mediakiwi.Data.Folder();
                    folder.IsVisible = true;
                    folder.Type = FolderType.List;
                    folder.Name = Common.FolderRoot;
                    folder.CompletePath = Common.FolderRoot;
                    folder.SiteID = Implement.ID;
                    await folder.SaveAsync().ConfigureAwait(false);
                }
            }
            if (Implement.HasPages)
            {
                var listsite = await Mediakiwi.Data.Folder.SelectOneBySiteAsync(Implement.ID, FolderType.Page).ConfigureAwait(false);
                if (listsite == null || listsite.ID == 0)
                {
                    var folder = new Mediakiwi.Data.Folder();
                    folder.IsVisible = true;
                    folder.Type = FolderType.Page;
                    folder.Name = Common.FolderRoot;
                    folder.CompletePath = Common.FolderRoot;
                    folder.SiteID = Implement.ID;
                    await folder.SaveAsync().ConfigureAwait(false);
                }
            }

            await ResetDefaultFolderAsync().ConfigureAwait(false);

            Response.Redirect(wim.GetUrl(new KeyValue()
            {
                Key = "item",
                Value = Implement.ID
            }));
        }

        /// <summary>
        /// Resets the default folder.
        /// </summary>
        private async Task ResetDefaultFolderAsync()
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
                    {
                        Implement.DefaultFolder = Implement.DefaultFolder.Substring(0, Implement.DefaultFolder.Length - 1);
                    }
                }

                if (Implement.DefaultFolder.Length > 0 && Implement.DefaultFolder.ToCharArray()[0] != '/')
                {
                    Implement.DefaultFolder = $"/{Implement.DefaultFolder}";
                }
            }

            wim.FlushCache();
            await Framework.Functions.FolderPathLogic.UpdateCompletePathAsync().ConfigureAwait(false);
            
            // if a master is connected, created the inheritance tree
            if (Implement.MasterID.HasValue && Implement.HasPages)
            {
                await Framework.Inheritance.Folder.CreateFolderTreeAsync(Implement.MasterID.Value, Implement.ID, FolderType.Page).ConfigureAwait(false);
                await Framework.Inheritance.Page.CreatePageTreeAsync(Implement.MasterID.Value, Implement.ID).ConfigureAwait(false);
            }

            if (Implement.MasterID.HasValue && Implement.HasLists)
            {
                await Framework.Inheritance.Folder.CreateFolderTreeAsync(Implement.MasterID.Value, Implement.ID, FolderType.List).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Gets or sets the implement.
        /// </summary>
        /// <value>The implement.</value>
        [Framework.ContentListItem.DataExtend()]
        public Mediakiwi.Data.Site Implement { get; set; }

        /// <summary>
        /// Handles the ListLoad event of the Site control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ComponentListEventArgs"/> instance containing the event data.</param>
        async Task Site_ListLoad(ComponentListEventArgs e)
        {
            Implement = await Mediakiwi.Data.Site.SelectOneAsync(e.SelectedKey).ConfigureAwait(false);
            InheritenceIsSet = Implement.MasterID.HasValue;

            if (!Implement.HasPages && !Implement.HasLists)
            {
                ListDelete += Site_ListDelete;
            }
            //ResetDefaultFolder();

            wim.CurrentSite = Implement;
            FormMaps.Add(new Forms.SiteForm(Implement));
        }

        /// <summary>
        /// Site_s the list search.
        /// </summary>
        /// <param name="sender">The sender.</param>
        async Task Site_ListSearch(ComponentListSearchEventArgs e)
        {
            wim.CanAddNewItem = true;
            wim.ListDataColumns.Add(new ListDataColumn("ID", nameof(Mediakiwi.Data.Site.ID), ListDataColumnType.UniqueIdentifier));
            wim.ListDataColumns.Add(new ListDataColumn("Site", nameof(Mediakiwi.Data.Site.Name), ListDataColumnType.HighlightPresent));
            wim.ListDataColumns.Add(new ListDataColumn("Inherits from", nameof(Mediakiwi.Data.Site.Master)));
            wim.ListDataColumns.Add(new ListDataColumn("Active", nameof(Mediakiwi.Data.Site.IsActive)) { ColumnWidth = 60 });
            wim.ListDataColumns.Add(new ListDataColumn("Pages", nameof(Mediakiwi.Data.Site.PageCount)) { ColumnWidth = 60 });
            wim.ListDataColumns.Add(new ListDataColumn("Lists", nameof(Mediakiwi.Data.Site.ListCount)) { ColumnWidth = 60 });
            wim.ListDataColumns.Add(new ListDataColumn("Created", nameof(Mediakiwi.Data.Site.Created)) { ColumnWidth = 100 });

            if (wim.IsCachedSearchResult)
            {
                return;
            }

            if (SearchSiteName == null)
            {
                var results = await Mediakiwi.Data.Site.SelectAllAsync().ConfigureAwait(false);
                wim.ListDataAdd(results);
            }
            else
            {
                var results = await Mediakiwi.Data.Site.SelectAllAsync(SearchSiteName).ConfigureAwait(false);
                wim.ListDataAdd(results);
            }
        }

        /// <summary>
        /// Gets or sets the name of the search site.
        /// </summary>
        /// <value>The name of the search site.</value>
        [Framework.ContentListSearchItem.TextField("Site title", 100, false)]
        public string SearchSiteName { get; set; }

        /// <summary>
        /// Gets a value indicating whether [inheritence is set].
        /// </summary>
        /// <value><c>true</c> if [inheritence is set]; otherwise, <c>false</c>.</value>
        public bool InheritenceIsSet { get; private set; }
    }
}
