using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    /// <summary>
    /// 
    /// </summary>
    public class RoleAccessList : ComponentListTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleAccessList"/> class.
        /// </summary>
        public RoleAccessList()
        {
            wim.HideOpenCloseToggle = true;
            wim.HideSaveButtons = true;

            ListSearch += RoleAccessList_ListSearch;
            ListAction += RoleAccessList_ListAction;
            ListLoad += RoleAccessList_ListLoad;
        }

        private async Task RoleAccessList_ListLoad(ComponentListEventArgs e)
        {
            IApplicationRole role = await ApplicationRole.SelectOneAsync(e.SelectedGroupItemKey).ConfigureAwait(false);
            Role = role.Name;
            IsAccessAllowed = role.IsAccessList;
        }

        private async Task RoleAccessList_ListAction(ComponentActionEventArgs e)
        {
            int[] checklist = wim.Grid.GetCheckboxChecked("HasAccess");

            var role = await ApplicationRole.SelectOneAsync(e.SelectedGroupItemKey).ConfigureAwait(false);
            role.IsAccessList = IsAccessAllowed;
            await role.SaveAsync().ConfigureAwait(false);

            await RoleRight.UpdateAsync(checklist, RoleRightType.List, role.ID).ConfigureAwait(false);
        }

        private async Task RoleAccessList_ListSearch(ComponentListSearchEventArgs e)
        {
            wim.CurrentList.Option_Search_MaxResultPerPage = 500;

            wim.SearchListCanClickThrough = false;

            wim.ListDataColumns.Add(new ListDataColumn("ID", "ID", ListDataColumnType.UniqueIdentifier));
            wim.ListDataColumns.Add(new ListDataColumn("Name", "Path", ListDataColumnType.HighlightPresent));
            wim.ListDataColumns.Add(new ListDataColumn("Folder access", "IsAllowed") { ColumnWidth = 100 });

            if (IsAccessAllowed)
            {
                wim.ListDataColumns.Add(new ListDataColumn("Allowed", "HasAccess", ListDataColumnType.Checkbox) { ColumnWidth = 75, Alignment = Align.Center });
            }
            else
            {
                wim.ListDataColumns.Add(new ListDataColumn("Denied", "HasAccess", ListDataColumnType.Checkbox) { ColumnWidth = 75, Alignment = Align.Center });
            }

            ApplicationUser temp = new ApplicationUser();
            temp.RoleID = e.SelectedGroupItemKey;
            IApplicationRole role = await temp.SelectRoleAsync().ConfigureAwait(false);

            var selection =
                from item in (await Mediakiwi.Data.ComponentList.SelectAllAsync().ConfigureAwait(false))
                join relation in role.Lists(temp) on item.GUID equals relation.GUID
                into combination
                from relation in combination.DefaultIfEmpty()
                where item.Type == ComponentListType.Undefined && item.FolderID.HasValue
                select new 
                { 
                    ID = item.ID, 
                    Name = item.Name, 
                    HasAccess = relation == null ? false : true, 
                    Path = item.CompletePath(), 
                    FolderID = item.FolderID 
                };

            if (role.All_Folders)
            {
                var ordered =
                    from item in selection
                    orderby item.Path
                    select new 
                    { 
                        ID = item.ID, 
                        Name = item.Name, 
                        HasAccess = item.HasAccess, 
                        Path = item.Path, 
                        IsAllowed = true 
                    };

                wim.ListDataAdd(ordered);
            }
            else
            {
                var ordered =
                    from item in selection
                    join relation in role.Folders(temp) on item.FolderID equals relation.ID
                    into combination
                    from relation in combination.DefaultIfEmpty()
                    orderby item.Path
                    select new 
                    { 
                        ID = item.ID, 
                        Name = item.Name, 
                        HasAccess = item.HasAccess, 
                        Path = item.Path, 
                        IsAllowed = relation == null ? false : true 
                    }; 

                wim.ListDataAdd(ordered);
            }


            foreach (var item in selection)
            {
                if (temp.SelectRole().IsAccessList)
                {
                    if (item.HasAccess)
                    {
                        wim.Grid.AddCheckboxValue("HasAccess", item.ID, true);
                    }
                }
                else if (!item.HasAccess)
                {
                    wim.Grid.AddCheckboxValue("HasAccess", item.ID, true);
                }
            }
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
        /// Gets or sets a value indicating whether this <see cref="RoleAccessList"/> is save.
        /// </summary>
        /// <value><c>true</c> if save; otherwise, <c>false</c>.</value>
        [Framework.ContentListItem.Button("Save access list", IsPrimary = true, IconTarget = ButtonTarget.BottomRight)]
        public bool Save { get; set; }

        /// <summary>
        /// Gets or sets the search list.
        /// </summary>
        /// <value>The search list.</value>
        [Framework.ContentListItem.DataList()]
        public DataList SearchList { get; set; }
    }
}
