using Sushi.Mediakiwi.Authentication;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.UI;
using Sushi.Mediakiwi.Utilities;
using System;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    public class User : BaseImplementation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        public User()
        {
            ListAction += User_ListAction;
            ListLoad += User_ListLoad;
            ListSave += User_ListSave;
            ListSearch += User_ListSearch;
            ListPreRender += User_ListPreRender;
            ListDelete += User_ListDelete;
        }

        async Task User_ListAction(ComponentActionEventArgs e)
        {
            if (HijackUser)
            {
                wim.CurrentVisitor.Data.Apply("Wim.Reset.Me", wim.CurrentApplicationUser.GUID.ToString());

                var tmp = await ApplicationUser.SelectOneAsync(m_Implement.GUID).ConfigureAwait(false);

                if (tmp?.ID > 0)
                {
                    tmp.LastLoggedVisit = DateTime.UtcNow;
                    await tmp.SaveAsync().ConfigureAwait(false);

                    wim.CurrentVisitor.ApplicationUserID = m_Implement.ID;
                    await wim.SaveVisitAsync();

                    var audit = new AuditTrail()
                    {
                        Action = ActionType.Login,
                        Type = ItemType.Undefined,
                        ItemID = wim.CurrentApplicationUser.ID,
                        Message = $"Impersonating [{m_Implement.Email}]",
                        Created = tmp.LastLoggedVisit.Value
                    };

                    await audit.InsertAsync().ConfigureAwait(false);

                    Response.Redirect(wim.Console.WimPagePath);
                }
            }
        }

        async Task User_ListDelete(ComponentListEventArgs e)
        {
            await m_Implement.DeleteAsync().ConfigureAwait(false);
        }

        Task User_ListPreRender(ComponentListEventArgs e)
        {
            if (m_Implement?.HasUserName(Name) == true)
            {
                wim.Notification.AddError(nameof(Name), "The applied username already exists");
            }

            if (m_Implement?.HasEmail(Email) == true)
            {
                wim.Notification.AddError(nameof(Email), "The applied email already exists");
            }

            return Task.CompletedTask;
        }

        public string Backend2 { get; set; }

        async Task User_ListSearch(ComponentListSearchEventArgs e)
        {
            wim.CanAddNewItem = true;
            wim.Page.HideTabs = true;

            Map(x => x.m_SearchUserName, this).TextField("Username", 50).Expression(OutputExpression.Alternating);
            Map(x => x.m_SearchRole, this).Dropdown("Role2", nameof(AvailableRoles)).Expression(OutputExpression.Alternating);

            FormMaps.Add(this);

            wim.ListDataColumns.Add(new ListDataColumn("ID", nameof(IApplicationUser.ID), ListDataColumnType.UniqueIdentifier));
            wim.ListDataColumns.Add(new ListDataColumn("Name", nameof(IApplicationUser.Displayname), ListDataColumnType.HighlightPresent));
            wim.ListDataColumns.Add(new ListDataColumn("Email address", nameof(IApplicationUser.Email)));
            wim.ListDataColumns.Add(new ListDataColumn("Role", nameof(IApplicationUser.RoleName)));
            wim.ListDataColumns.Add(new ListDataColumn("Last login", nameof(IApplicationUser.LastLoggedVisit)) { ColumnWidth = 110, Alignment = Align.Center });
            wim.ListDataColumns.Add(new ListDataColumn("Active", nameof(IApplicationUser.IsActive)) { ColumnWidth = 30, Alignment = Align.Center });

            if (wim.IsCachedSearchResult)
            {
                return;
            }

            var data = await ApplicationUser.SelectAllAsync(m_SearchUserName, Utility.ConvertToInt(m_SearchRole)).ConfigureAwait(false);
            wim.ListDataAdd(data);
        }

        async Task User_ListSave(ComponentListEventArgs e)
        {
            string password = "";
            if (m_Implement?.ID > 0)
            {
                password = m_Implement.Password;
            }
            else
            {
                m_Implement = new ApplicationUser();

                PasswordGenerator gen = new PasswordGenerator();
                string newPassword = gen.Generate();
                Password = newPassword;
            }

            Utility.ReflectProperty(this, m_Implement);

            m_Implement.Password = password;

            if (!string.IsNullOrEmpty(Password))
            {
                m_Implement.ApplyPassword(Password);
            }
            else
            {
                if (m_Implement.Type == 0)
                {
                    using (var auth = new AuthenticationLogic())
                    {
                        auth.Password = "wimserver";
                        string passwordDecrypt = auth.Decrypt(m_Implement.Password);
                        m_Implement.ApplyPassword(passwordDecrypt);
                    }
                }
            }

            m_Implement.ShowFullWidth = ShowFullWidth;
            m_Implement.IsDeveloper = IsDeveloper;
            m_Implement.ShowSiteNavigation = ShowSiteNavigation;

            if (m_Implement.GUID == Guid.Empty)
            {
                m_Implement.GUID = Guid.NewGuid();
            }

            m_Implement.RoleID = RoleID;
            m_Implement.IsActive = IsActive;

            await m_Implement.SaveAsync().ConfigureAwait(false);

            if (SendCredentials)
            {
                m_Implement.SendLoginMail(wim.Console);
            }
        }


        public IApplicationUser m_Implement;
        /// <summary>
        /// Handles the ListLoad event of the User control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ComponentListEventArgs"/> instance containing the event data.</param>
        async Task User_ListLoad(ComponentListEventArgs e)
        {
            if (e.SelectedKey > 0)
            {
                m_Implement = await ApplicationUser.SelectOneAsync(e.SelectedKey).ConfigureAwait(false);
                UID = m_Implement.GUID.ToString();
            }

            if (e.SelectedKey == 0)
            {
                PasswordGenerator gen = new PasswordGenerator();
                string newPassword = gen.Generate();
                Password = newPassword;
                return;
            }

            Utility.ReflectProperty(m_Implement, this);

            if (!wim.IsEditMode && !IsPostBack)
            {
                Password = "******";
            }
            else
            {
                Password = null;
            }

            IsActive = m_Implement.IsActive;
            Language = m_Implement.Language;
            ShowSiteNavigation = m_Implement.ShowSiteNavigation;
            ShowFullWidth = m_Implement.ShowFullWidth;
            IsDeveloper = m_Implement.IsDeveloper;

            if (e.SelectedKey == 0)
            {
                return;
            }

            wim.AddTab(new Guid("93D10F58-6A1A-493F-8ADB-E53FC7CEDE19"));
        }

        [Framework.ContentSettingItem.RichText("Login intro", 0)]
        public string Page_Login { get; set; }

        [Framework.ContentSettingItem.Section("New account mail")]
        [Framework.ContentSettingItem.TextField("Mail title", 50)]
        public string Mail_Title { get; set; }

        [Framework.ContentSettingItem.Binary_Image("Mail logo", CanOnlyAdd = true)]
        public virtual int? Mail_Logo { get; set; }

        [Framework.ContentSettingItem.RichText("Mail intro", 0, InteractiveHelp = "Use [credentials], [name], [login], [email], [url] and http://url as placeholders.")]
        public string Mail_Intro { get; set; }

        [Framework.ContentSettingItem.Section("Forgot password mail")]
        [Framework.ContentSettingItem.TextField("Mail title", 50)]
        public string Mail_ForgotTitle { get; set; }

        [Framework.ContentSettingItem.RichText("Mail intro", 0, InteractiveHelp = "Use [credentials], [name], [login], [email], [url] and http://url as placeholders.")]
        public string Mail_ForgotIntro { get; set; }

        [Framework.ContentSettingItem.TextField("New password title", 50)]
        public string Page_Apply_T { get; set; }

        [Framework.ContentSettingItem.RichText("New password intro", 0)]
        public string Page_Apply_B { get; set; }


        #region List search attributes

        private string m_SearchUserName { get; set; }
        private string m_SearchRole { get; set; }

        #endregion List search attributes

        #region List attributes

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [OnlyEditableWhenTrue(nameof(IsNewElement))]
        [Framework.ContentListItem.TextField("Username", 50, true, Expression = OutputExpression.Alternating)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        [Framework.ContentListItem.TextField("Emailaddress", 255, true, Expression = OutputExpression.Alternating)]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the displayname.
        /// </summary>
        /// <value>The displayname.</value>
        [Framework.ContentListItem.TextField("Displayname", 50, true, Expression = OutputExpression.Alternating)]
        public string Displayname { get; set; }

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        /// <value>The role.</value>
        [Framework.ContentListItem.Choice_Dropdown("Role", nameof(AvailableRoles), true, true, Expression = OutputExpression.Alternating)]
        public int RoleID { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        //[Sushi.Mediakiwi.Framework.ContentListItem.TextField("Password", 50, false, null, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public string Password { get; set; }

        [Framework.ContentListItem.TextField("UID", 50, false, null, Expression = OutputExpression.Alternating)]
        public string UID { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>The language.</value>
        [Framework.ContentListItem.Choice_Dropdown("Language", nameof(AvailableLanguages), true, Expression = OutputExpression.Alternating)]
        public int Language { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        [Framework.ContentListItem.Choice_Checkbox("Is active", Expression = OutputExpression.Alternating)]
        public bool IsActive { get; set; }

        //[Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Site navigation", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public bool ShowSiteNavigation { get; set; }

        [Framework.ContentListItem.Choice_Checkbox("Is Developer", Expression = OutputExpression.Alternating)]
        public bool IsDeveloper { get; set; }

        //[Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Mediakiwi (Beta)", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public bool ForceNewStyle { get; set; }

        //[Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Mediakiwi (Beta:2)", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public bool ForceNewStyle2 { get; set; } = true;

        //[Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Full width", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public bool ShowFullWidth { get; set; }

        /// <summary>
        /// Gets the available roles.
        /// </summary>
        /// <value>The available roles.</value>
        public ListItemCollection AvailableRoles
        {
            get
            {
                ListItemCollection collection = new ListItemCollection();
                collection.Add(new ListItem("Select a role", ""));
                foreach (var role in ApplicationRole.SelectAll())
                {
                    collection.Add(new ListItem(role.Name, $"{role.ID}"));
                }
                return collection;
            }
        }

        /// <summary>
        /// Gets the available languages.
        /// </summary>
        /// <value>The available languages.</value>
        public ListItemCollection AvailableLanguages
        {
            get
            {
                ListItemCollection collection = new ListItemCollection();
                collection.Add(new ListItem("English", "1") { Selected = true });
                collection.Add(new ListItem("Nederlands", "2"));
                return collection;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [send credentials].
        /// </summary>
        /// <value><c>true</c> if [send credentials]; otherwise, <c>false</c>.</value>
        [Framework.ContentListItem.Button("", true, ButtonClassName = "flaticon icon-envelope", InteractiveHelp = "Email login details")]
        public bool SendCredentials { get; set; }

        [Framework.ContentListItem.Button("", false, ButtonClassName = "flaticon icon-eye-slash", InteractiveHelp = "Impersonate this user")]
        public bool HijackUser { get; set; }

        #endregion List attributes
    }
}
