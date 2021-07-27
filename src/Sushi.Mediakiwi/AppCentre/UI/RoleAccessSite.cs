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
    public class RoleAccessSite : ComponentListTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleAccessSite"/> class.
        /// </summary>
        public RoleAccessSite()
        {
            wim.HideOpenCloseToggle = true;
            wim.HideSaveButtons = true;
            
            ListSearch += RoleAccessSite_ListSearch;
            ListAction += RoleAccessSite_ListAction;
            ListLoad += RoleAccessSite_ListLoad;
        }


        /// <summary>
        /// Gets or sets a value indicating whether this instance is user section.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is user section; otherwise, <c>false</c>.
        /// </value>
        public bool IsUserSection { get; set; }

        async Task RoleAccessSite_ListLoad(ComponentListEventArgs e)
        {
            IApplicationRole role = await ApplicationRole.SelectOneAsync(e.SelectedGroupItemKey).ConfigureAwait(false);
            Role = role.Name;
            IsAccessAllowed = role.IsAccessSite;

            IComponentList list = await Mediakiwi.Data.ComponentList.SelectOneAsync(e.SelectedGroupKey).ConfigureAwait(false);
            IsUserSection = (list.Type == ComponentListType.Users);
            if (IsUserSection)
            {
                var user = await ApplicationUser.SelectOneAsync(e.SelectedGroupItemKey).ConfigureAwait(false);
                User = user.Displayname;
            }
        }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>The user.</value>
        [OnlyVisibleWhenTrue(nameof(IsUserSection)), Framework.ContentListItem.TextLine("User", Expression = OutputExpression.Alternating)]
        public string User { get; set; }

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        /// <value>The role.</value>
        [OnlyVisibleWhenTrue(nameof(IsUserSection), false), Framework.ContentListItem.TextLine("Role", Expression = OutputExpression.Alternating)]
        public string Role { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is access allowed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is access allowed; otherwise, <c>false</c>.
        /// </value>
        [OnlyVisibleWhenTrue(nameof(IsUserSection), false)]
        [Framework.ContentListItem.Choice_Checkbox("Grant access", true, "Checked selection means 'access granted', unchecked means 'access denied'", Expression = OutputExpression.Alternating)]
        public bool IsAccessAllowed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RoleAccessSite"/> is save.
        /// </summary>
        /// <value><c>true</c> if save; otherwise, <c>false</c>.</value>
        [Framework.ContentListItem.Button("Save access list", IconTarget = ButtonTarget.BottomRight)]
        public bool Save { get; set; }

        /// <summary>
        /// Handles the ListAction event of the RoleAccessSite control.
        /// </summary>
        /// <param name="e">The <see cref="ComponentActionEventArgs"/> instance containing the event data.</param>
        async Task RoleAccessSite_ListAction(ComponentActionEventArgs e)
        {
            if (IsUserSection)
            {
                var datalist = wim.Grid.GetRadioValue("HasAccess");
                var user = await ApplicationUser.SelectOneAsync(e.SelectedGroupItemKey).ConfigureAwait(false);
                var dict = new System.Collections.Generic.Dictionary<int, Access>();

                foreach (var item in datalist)
                {
                    Access acc = Access.Inherit;
                    if (item.Value.Equals("Denied", System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        acc = Access.Denied;
                    }
                    else if (item.Value.Equals("Granted", System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        acc = Access.Granted;
                    }

                    dict.Add(item.Key, acc);
                }

                await RoleRight.UpdateAsync(dict, RoleRightType.SiteByUser, user.ID).ConfigureAwait(false);

                //user.IsGrantedSite = IsAccessAllowed;
                //user.Save();
                //
            }
            else
            {
                int[] checklist = wim.Grid.GetCheckboxChecked("HasAccess");
                IApplicationRole role = await ApplicationRole.SelectOneAsync(e.SelectedGroupItemKey).ConfigureAwait(false);
                role.IsAccessSite = IsAccessAllowed;

                await role.SaveAsync().ConfigureAwait(false);
                await RoleRight.UpdateAsync(checklist, RoleRightType.Site, role.ID).ConfigureAwait(false);
            }
        }

        Mediakiwi.Data.Site[] m_AccessList;

        async Task<bool> GetRoleGrantedAccessAsync(IApplicationUser user, int siteID)
        {
            if (m_AccessList == null)
            {
                m_AccessList = await user.SitesAsync(AccessFilter.RoleBased).ConfigureAwait(false);
            }

            int len = (from item in m_AccessList where item.ID == siteID select item).ToArray().Length;
            return len == 0 ? false : true;
        }

        /// <summary>
        /// Handles the ListSearch event of the RoleAccessSite control.
        /// </summary>
        /// <param name="e">The <see cref="ComponentListSearchEventArgs"/> instance containing the event data.</param>
        async Task RoleAccessSite_ListSearch(ComponentListSearchEventArgs e)
        {
            IComponentList list = await Mediakiwi.Data.ComponentList.SelectOneAsync(e.SelectedGroupKey).ConfigureAwait(false);
            IsUserSection = (list.Type == ComponentListType.Users);

            wim.CurrentList.Option_Search_MaxResultPerPage = 500;
            wim.SearchListCanClickThrough = false;

            wim.ListDataColumns.Add(new ListDataColumn("ID", nameof(Mediakiwi.Data.Site.ID), ListDataColumnType.UniqueIdentifier));
            wim.ListDataColumns.Add(new ListDataColumn("Name", nameof(Mediakiwi.Data.Site.Name), ListDataColumnType.HighlightPresent));

            if (IsUserSection)
            {
                wim.ListDataColumns.Add(new ListDataColumn("Granted", "HasAccess", ListDataColumnType.RadioBox) { ColumnWidth = 60 });
                wim.ListDataColumns.Add(new ListDataColumn("Denied", "HasAccess", ListDataColumnType.RadioBox) { ColumnWidth = 60 });
                wim.ListDataColumns.Add(new ListDataColumn("Inherit", "HasAccess", ListDataColumnType.RadioBox) { ColumnWidth = 60 });
                wim.ListDataColumns.Add(new ListDataColumn("Role", "RoleStatus") { ColumnWidth = 30 });

                var user = await ApplicationUser.SelectOneAsync(e.SelectedGroupItemKey).ConfigureAwait(false);
                var allSites = await Mediakiwi.Data.Site.SelectAllAsync().ConfigureAwait(false);
                var allRoleRights = await RoleRight.SelectAllAsync(user.ID, RoleRightType.SiteByUser).ConfigureAwait(false);

                var selection = 
                    from item in allSites
                    join relation in allRoleRights on item.ID equals relation.ItemID
                    into combination
                    from relation in combination.DefaultIfEmpty()
                    select new {
                        item.ID,
                        item.Name,
                        HasRoleReference = (relation == null ? false : true),
                        RoleStatus = GetRoleGrantedAccessAsync(user, item.ID),
                        AccessType = (relation == null ? Access.Inherit : relation.AccessType)
                    };

                foreach (var item in selection)
                {
                    wim.Grid.AddRadioboxValue("HasAccess", item.ID, item.AccessType.ToString());
                }
                wim.ListDataAdd(selection);
            }
            else
            {
                if (IsAccessAllowed)
                {
                    wim.ListDataColumns.Add(new ListDataColumn("Granted", "HasAccess", ListDataColumnType.Checkbox) { ColumnWidth = 60 });
                }
                else
                {
                    wim.ListDataColumns.Add(new ListDataColumn("Denied", "HasAccess", ListDataColumnType.Checkbox) { ColumnWidth = 60 });
                }

                ApplicationUser temp = new ApplicationUser();
                temp.RoleID = e.SelectedGroupItemKey;
                IApplicationRole role = await temp.RoleAsync().ConfigureAwait(false);

                var allowed = await role.SitesAsync(temp).ConfigureAwait(false);
                var allSites = await Mediakiwi.Data.Site.SelectAllAsync(true).ConfigureAwait(false);

                var selection =
                    from item in allSites
                    join relation in allowed on item.GUID equals relation.GUID
                    into combination
                    from relation in combination.DefaultIfEmpty()
                    select new 
                    { 
                        ID = item.ID, 
                        Name = item.Name, 
                        HasAccess = relation == null ? false : true 
                    };

                foreach (var item in selection)
                {
                    if (role.IsAccessSite)
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
                wim.ListDataAdd(selection);
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