using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    /// <summary>
    /// 
    /// </summary>
    public class RoleAccessGallery : ComponentListTemplate
    {
         /// <summary>
        /// Initializes a new instance of the <see cref="RoleAccessGallery"/> class.
        /// </summary>
        public RoleAccessGallery()
        {
            wim.HideOpenCloseToggle = true;
            wim.HideSaveButtons = true;
            
            ListSearch += RoleAccessGallery_ListSearch;
            ListAction += RoleAccessGallery_ListAction;
            ListLoad += RoleAccessGallery_ListLoad;
        }

        private async Task RoleAccessGallery_ListLoad(ComponentListEventArgs e)
        {
            IApplicationRole role = await ApplicationRole.SelectOneAsync(e.SelectedGroupItemKey).ConfigureAwait(false);
            Role = role.Name;
            IsAccessAllowed = role.IsAccessList;
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
        /// Gets or sets a value indicating whether this <see cref="RoleAccessGallery"/> is save.
        /// </summary>
        /// <value><c>true</c> if save; otherwise, <c>false</c>.</value>
        [Framework.ContentListItem.Button("Save access list", IconTarget = ButtonTarget.BottomRight)]
        public bool Save { get; set; }

        /// <summary>
        /// Handles the ListAction event of the RoleAccessGallery control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Framework.ComponentActionEventArgs"/> instance containing the event data.</param>
        private async Task RoleAccessGallery_ListAction(ComponentActionEventArgs e)
        {
            int[] checklist = wim.Grid.GetCheckboxChecked("HasAccess");

            var role = await ApplicationRole.SelectOneAsync(e.SelectedGroupItemKey).ConfigureAwait(false);
            role.IsAccessGallery = IsAccessAllowed;
            await role.SaveAsync().ConfigureAwait(false);

            await RoleRight.UpdateAsync(checklist, RoleRightType.Gallery, role.ID).ConfigureAwait(false);
        }

        /// <summary>
        /// Handles the ListSearch event of the RoleAccessGallery control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Framework.ComponentListSearchEventArgs"/> instance containing the event data.</param>
        private async Task RoleAccessGallery_ListSearch(ComponentListSearchEventArgs e)
        {
            wim.CurrentList.Option_Search_MaxResultPerPage = 500;
            
            wim.SearchListCanClickThrough = false;
            wim.ListDataColumns.Add(new ListDataColumn("ID", "ID", ListDataColumnType.UniqueIdentifier));
            wim.ListDataColumns.Add(new ListDataColumn("Name", "Path", ListDataColumnType.HighlightPresent));

            if (IsAccessAllowed)
                wim.ListDataColumns.Add(new ListDataColumn("Allowed", "HasAccess", ListDataColumnType.Checkbox));
            else
                wim.ListDataColumns.Add(new ListDataColumn("Denied", "HasAccess", ListDataColumnType.Checkbox));

            ApplicationUser temp = new ApplicationUser(); 
            temp.RoleID = e.SelectedGroupItemKey;
            IApplicationRole role = await temp.SelectRoleAsync().ConfigureAwait(false);

            var selection =
                from item in (await Gallery.SelectAllAsync())
                join relation in role.Galleries(temp) on item.GUID equals relation.GUID
                into combination
                from relation in combination.DefaultIfEmpty()
                select new { ID = item.ID, Name = item.Name, HasAccess = relation == null ? false : true, Path = item.CompletePath };
            
            var ordered =
                from item in selection orderby item.Path select item;

            foreach (var item in selection)
            {
                if (role.IsAccessGallery)
                {
                    if (item.HasAccess)
                        wim.Grid.AddCheckboxValue("HasAccess", item.ID, true);
                }
                else if (!item.HasAccess)
                    wim.Grid.AddCheckboxValue("HasAccess", item.ID, true);
            }

            wim.ListDataAdd(ordered);
        }

        /// <summary>
        /// Gets or sets the search list.
        /// </summary>
        /// <value>The search list.</value>
        [Framework.ContentListItem.DataList()]
        public DataList SearchList { get; set; }
    }
}