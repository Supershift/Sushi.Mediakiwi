using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    /// <summary>
    /// 
    /// </summary>
    public class RoleAccessFolder : ComponentListTemplate
    {
         /// <summary>
        /// Initializes a new instance of the <see cref="RoleAccessFolder"/> class.
        /// </summary>
        public RoleAccessFolder()
        {
            wim.HideOpenCloseToggle = true;
            wim.HideSaveButtons = true;

            ListSearch += RoleAccessFolder_ListSearch;
            ListAction += RoleAccessFolder_ListAction;
            ListLoad += RoleAccessFolder_ListLoad;
        }

        private async Task RoleAccessFolder_ListLoad(ComponentListEventArgs e)
        {
            IApplicationRole role = await ApplicationRole.SelectOneAsync(e.SelectedGroupItemKey).ConfigureAwait(false);
            Role = role.Name;
            IsAccessAllowed = role.IsAccessFolder;
        }

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        /// <value>The role.</value>
        [Framework.ContentListItem.TextLine("Role")]
        public string Role { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is access allowed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is access allowed; otherwise, <c>false</c>.
        /// </value>
        [Framework.ContentListItem.Choice_Checkbox("Access list", true, "Checked selection means 'access allowed', unchecked means 'access denied'")]
        public bool IsAccessAllowed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RoleAccessFolder"/> is save.
        /// </summary>
        /// <value><c>true</c> if save; otherwise, <c>false</c>.</value>
        [Framework.ContentListItem.Button("Save access list", IconTarget = ButtonTarget.BottomRight)]
        public bool Save { get; set; }

        /// <summary>
        /// Handles the ListAction event of the RoleAccessFolder control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Framework.ComponentActionEventArgs"/> instance containing the event data.</param>
        private async Task RoleAccessFolder_ListAction(ComponentActionEventArgs e)
        {
            int[] checklist = wim.Grid.GetCheckboxChecked("HasAccess");

            var role = await ApplicationRole.SelectOneAsync(e.SelectedGroupItemKey).ConfigureAwait(false);
            role.IsAccessFolder = IsAccessAllowed;
            await role.SaveAsync().ConfigureAwait(false);

            await RoleRight.UpdateAsync(checklist, RoleRightType.Folder, role.ID).ConfigureAwait(false);
        }

        /// <summary>
        /// Handles the ListSearch event of the RoleAccessFolder control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Framework.ComponentListSearchEventArgs"/> instance containing the event data.</param>
        private async Task RoleAccessFolder_ListSearch(ComponentListSearchEventArgs e)
        {
            wim.CurrentList.Option_Search_MaxResultPerPage = 500;
            
            wim.SearchListCanClickThrough = false;
            wim.ListDataColumns.Add(new ListDataColumn("ID", "ID", ListDataColumnType.UniqueIdentifier));
            wim.ListDataColumns.Add(new ListDataColumn("Name", "Path", ListDataColumnType.HighlightPresent));
            wim.ListDataColumns.Add(new ListDataColumn("Type", "Type"));
            
            if (IsAccessAllowed)
                wim.ListDataColumns.Add(new ListDataColumn("Allowed", "HasAccess", ListDataColumnType.Checkbox));
            else
                wim.ListDataColumns.Add(new ListDataColumn("Denied", "HasAccess", ListDataColumnType.Checkbox));

            ApplicationUser temp = new ApplicationUser();
            temp.RoleID = e.SelectedGroupItemKey;
            IApplicationRole role = await temp.SelectRoleAsync().ConfigureAwait(false);

            var selection =
                from item in (await Mediakiwi.Data.Folder.SelectAllAsync().ConfigureAwait(false))
                join relation in role.Folders(temp) on item.GUID equals relation.GUID
                into combination
                from relation in combination.DefaultIfEmpty()
                //where item.Type != Data.FolderType.Administration
                select new { SiteID = item.SiteID, ID = item.ID, Name = item.Name, HasAccess = relation == null ? false : true, Path = string.Concat(item.Site.Name, item.CompletePath).Replace("[internal]", "Admin"), Type = item.Type == FolderType.Page ? "Web" : item.Type == FolderType.List ? "Settings" : "Admin" };


            foreach (var item in selection)
            {
                
                if (role.IsAccessFolder)
                {
                    if (item.HasAccess)
                        wim.Grid.AddCheckboxValue("HasAccess", item.ID, true);
                }
                else if (!item.HasAccess)
                    wim.Grid.AddCheckboxValue("HasAccess", item.ID, true);
            }

            if (role.All_Sites)
            {
                var ordered =
                    from item in selection
                    orderby item.Type ascending, item.Path
                    select item;

                wim.ListDataAdd(ordered);
            }
            else
            {
                var ordered =
                    from item in selection
                    join relation in role.Sites(wim.CurrentApplicationUser) on item.SiteID equals relation.ID
                    into combination
                    from relation in combination.DefaultIfEmpty()
                    select new { SiteID = item.SiteID, ID = item.ID, Name = item.Name, HasAccess = item.HasAccess, Type = item.Type, Path = item.Path, IsPresent = relation != null };

                ordered =
                    from item in ordered
                    where item.IsPresent || item.Type == "Admin"
                    orderby item.Type ascending, item.Path
                    select item;

                wim.ListDataAdd(ordered);
            }
        }

        /// <summary>
        /// Gets or sets the search list.
        /// </summary>
        /// <value>The search list.</value>
        [Framework.ContentListItem.DataList()]
        public DataList SearchList { get; set; }
    }
}