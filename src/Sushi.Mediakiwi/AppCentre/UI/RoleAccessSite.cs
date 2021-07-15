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

        Task RoleAccessSite_ListLoad(ComponentListEventArgs e)
        {
            IApplicationRole role = ApplicationRole.SelectOne(e.SelectedGroupItemKey);
            Role = role.Name;
            IsAccessAllowed = role.IsAccessSite;

            IComponentList list = Mediakiwi.Data.ComponentList.SelectOne(e.SelectedGroupKey);
            IsUserSection = (list.Type == ComponentListType.Users);
            if (IsUserSection)
            {
                var user = ApplicationUser.SelectOne(e.SelectedGroupItemKey);
                User = user.Displayname;
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>The user.</value>
        [OnlyVisibleWhenTrue("IsUserSection"), Framework.ContentListItem.TextLine("User", Expression = OutputExpression.Alternating)]
        public string User { get; set; }

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        /// <value>The role.</value>
        [OnlyVisibleWhenTrue("IsUserSection", false), Framework.ContentListItem.TextLine("Role", Expression = OutputExpression.Alternating)]
        public string Role { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is access allowed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is access allowed; otherwise, <c>false</c>.
        /// </value>
        [OnlyVisibleWhenTrue("IsUserSection", false)]
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
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ComponentActionEventArgs"/> instance containing the event data.</param>
        Task RoleAccessSite_ListAction(ComponentActionEventArgs e)
        {
            if (IsUserSection)
            {
                var datalist = wim.Grid.GetRadioValue("HasAccess");

                var user = ApplicationUser.SelectOne(e.SelectedGroupItemKey);

                System.Collections.Generic.Dictionary<int, Access> dict = 
                    new System.Collections.Generic.Dictionary<int, Access>();

                foreach (var item in datalist)
                {
                    dict.Add(item.Key,
                        item.Value.Equals("Denied") ? Access.Denied :
                        item.Value.Equals("Granted") ? Access.Granted :
                        Access.Inherit);
                }
                RoleRight.Update(dict, RoleRightType.SiteByUser, user.ID);

                //user.IsGrantedSite = IsAccessAllowed;
                //user.Save();
                //
            }
            else
            {
                int[] checklist = wim.Grid.GetCheckboxChecked("HasAccess");
                IApplicationRole role = ApplicationRole.SelectOne(e.SelectedGroupItemKey);
                role.IsAccessSite = IsAccessAllowed;
                role.Save();

                RoleRight.Update(checklist, RoleRightType.Site, role.ID);
            }
            return Task.CompletedTask;
        }

        Mediakiwi.Data.Site[] m_AccessList;
        bool GetRoleGrantedAccess(IApplicationUser user, int siteID)
        {
            if (m_AccessList == null)
                m_AccessList = user.Sites(AccessFilter.RoleBased);

            int len = (from item in m_AccessList where item.ID == siteID select item).ToArray().Length;
            return len == 0 ? false : true;
        }

        /// <summary>
        /// Handles the ListSearch event of the RoleAccessSite control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ComponentListSearchEventArgs"/> instance containing the event data.</param>
        Task RoleAccessSite_ListSearch(ComponentListSearchEventArgs e)
        {
            IComponentList list = Mediakiwi.Data.ComponentList.SelectOne(e.SelectedGroupKey);
            IsUserSection = (list.Type == ComponentListType.Users);

            wim.CurrentList.Option_Search_MaxResultPerPage = 500;
            
            wim.SearchListCanClickThrough = false;
            wim.ListDataColumns.Add("ID", "ID", ListDataColumnType.UniqueIdentifier);
            wim.ListDataColumns.Add("Name", "Name", ListDataColumnType.HighlightPresent);

            if (IsUserSection)
            {
                wim.ListDataColumns.Add("Granted", "HasAccess", ListDataColumnType.RadioBox, 60);
                wim.ListDataColumns.Add("Denied", "HasAccess", ListDataColumnType.RadioBox, 60);
                wim.ListDataColumns.Add("Inherit", "HasAccess", ListDataColumnType.RadioBox, 60);

                wim.ListDataColumns.Add("Role", "RoleStatus", 30);
                var user = ApplicationUser.SelectOne(e.SelectedGroupItemKey);

                var selection =
                    from item in Mediakiwi.Data.Site.SelectAll()
                    join relation in RoleRight.SelectAll(user.ID, RoleRightType.SiteByUser) on item.ID equals relation.ItemID
                    into combination
                    from relation in combination.DefaultIfEmpty()
                    select new {
                        item.ID,
                        item.Name,
                        HasRoleReference = (relation == null ? false : true),
                        RoleStatus = GetRoleGrantedAccess(user, item.ID),
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
                    wim.ListDataColumns.Add("Granted", "HasAccess", ListDataColumnType.Checkbox, 60);
                }
                else
                    wim.ListDataColumns.Add("Denied", "HasAccess", ListDataColumnType.Checkbox, 60);


                ApplicationUser temp = new ApplicationUser();
                temp.RoleID = e.SelectedGroupItemKey;
                IApplicationRole role = temp.Role();

                var allowed = role.Sites(temp);

                var selection =
                    from item in Mediakiwi.Data.Site.SelectAll(true)
                    join relation in allowed on item.GUID equals relation.GUID
                    into combination
                    from relation in combination.DefaultIfEmpty()
                    select new { ID = item.ID, Name = item.Name, HasAccess = relation == null ? false : true };

                foreach (var item in selection)
                {
                    if (role.IsAccessSite)
                    {
                        if (item.HasAccess)
                            wim.Grid.AddCheckboxValue("HasAccess", item.ID, true);
                    }
                    else if (!item.HasAccess)
                        wim.Grid.AddCheckboxValue("HasAccess", item.ID, true);
                }
                wim.ListDataAdd(selection);
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets or sets the search list.
        /// </summary>
        /// <value>The search list.</value>
        [Framework.ContentListItem.DataList()]
        public DataList SearchList { get; set; }
    }
}