using System;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Data;
using System.Drawing;
using System.Web;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Utilities;
using Sushi.Mediakiwi.UI;
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
            //wim.OpenInEditMode = true;
            this.ListAction += User_ListAction;
            this.ListLoad += User_ListLoad;
            this.ListSave += User_ListSave;
            this.ListSearch += User_ListSearch;
            this.ListPreRender += User_ListPreRender;
            this.ListDelete += User_ListDelete;
        }

        async Task User_ListAction(ComponentActionEventArgs e)
        {
            if (HijackUser)
            {
                wim.CurrentVisitor.Data.Apply("Wim.Reset.Me", wim.CurrentApplicationUser.GUID.ToString());

                var tmp = await ApplicationUser.SelectOneAsync(m_Implement.GUID);

                if (tmp?.ID > 0)
                {
                    tmp.LastLoggedVisit = DateTime.UtcNow;
                    tmp.Save();

                    wim.CurrentVisitor.ApplicationUserID = m_Implement.ID;
                    wim.SaveVisit();

                    await new AuditTrail()
                    {
                        Action = ActionType.Login,
                        Type = ItemType.Undefined,
                        ItemID = wim.CurrentApplicationUser.ID,
                        Message = $"Impersonating [{m_Implement.Email}]",
                        Created = tmp.LastLoggedVisit.Value
                    }.InsertAsync();

                    Response.Redirect(wim.Console.GetSafeUrl());
                }
            }
        }

        async Task User_ListDelete(ComponentListEventArgs e)
        {
            await m_Implement.DeleteAsync();
        }

        Task User_ListPreRender(ComponentListEventArgs e)
        {
            if (m_Implement?.HasUserName(this.Name) == true)
                wim.Notification.AddError("Name", "The applied username already exists");

            if (m_Implement?.HasEmail(this.Email) == true)
                wim.Notification.AddError("Email", "The applied email already exists");

            return Task.CompletedTask;
        }

        async Task User_ListSearch(ComponentListSearchEventArgs e)
        {
            wim.CanAddNewItem = true;
            wim.Page.HideTabs = true;

            wim.ListDataColumns.Add(new ListDataColumn("ID", nameof(Sushi.Mediakiwi.Data.IApplicationUser.ID), ListDataColumnType.UniqueIdentifier));
            wim.ListDataColumns.Add(new ListDataColumn("Name", nameof(Sushi.Mediakiwi.Data.IApplicationUser.Displayname), ListDataColumnType.HighlightPresent));
            wim.ListDataColumns.Add(new ListDataColumn("Email address", nameof(Sushi.Mediakiwi.Data.IApplicationUser.Email))); 
            wim.ListDataColumns.Add(new ListDataColumn("Role", nameof(Sushi.Mediakiwi.Data.IApplicationUser.RoleName)));
            wim.ListDataColumns.Add(new ListDataColumn("Last login", nameof(Sushi.Mediakiwi.Data.IApplicationUser.LastLoggedVisit)) { ColumnWidth = 110, Alignment = Align.Center });
            wim.ListDataColumns.Add(new ListDataColumn("Active", nameof(Sushi.Mediakiwi.Data.IApplicationUser.IsActive)) { ColumnWidth = 30, Alignment = Align.Center });

            if (wim.IsCachedSearchResult)
                return;

            var data = await Sushi.Mediakiwi.Data.ApplicationUser.SelectAllAsync(m_SearchUserName, Utility.ConvertToInt(m_SearchRole));
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

            if (!string.IsNullOrEmpty(m_Password))
            {
                m_Implement.ApplyPassword(m_Password);
            }
            else
            {
                if (m_Implement.Type == 0)
                {
                    using (Authentication auth = new Authentication())
                    {
                        auth.Password = "wimserver";
                        string passwordDecrypt = auth.Decrypt(m_Implement.Password);
                        m_Implement.ApplyPassword(passwordDecrypt);
                    }
                }
            }

            m_Implement.ShowFullWidth = this.ShowFullWidth;
            m_Implement.IsDeveloper = this.IsDeveloper;
            m_Implement.ShowSiteNavigation = this.ShowSiteNavigation;

            if (m_Implement.GUID == Guid.Empty)
                m_Implement.GUID = Guid.NewGuid();

            m_Implement.RoleID = this.RoleID;
            m_Implement.IsActive = IsActive;

            //Parser.Save(m_Implement);
            
            await m_Implement.SaveAsync();

            if (this.SendCredentials)
            {
                m_Implement.SendLoginMail(wim.Console);
            }
        }

        #region SetPasswordMask
        private void SetPasswordMask()
        {
            Sushi.Mediakiwi.Data.Content content =
                Sushi.Mediakiwi.Data.Content.GetDeserialized(wim.ComponentListVersion.Serialized_XML);

            foreach (Field field in content.Fields)
            {
                //  Replace the password value
                if (field.Property == "Password")
                {
                    field.Value = "******";
                    break;
                }
            }

            wim.ComponentListVersion.Serialized_XML = Sushi.Mediakiwi.Data.Content.GetSerialized(content);
            wim.ComponentListVersion.Save();
        }
        #endregion SetPasswordMask

        private string m_tempPassword;

        public Sushi.Mediakiwi.Data.IApplicationUser m_Implement;
        /// <summary>
        /// Handles the ListLoad event of the User control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ComponentListEventArgs"/> instance containing the event data.</param>
        async Task User_ListLoad(ComponentListEventArgs e)
        {
            if (e.SelectedKey > 0)
            {
                m_Implement = await ApplicationUser.SelectOneAsync(e.SelectedKey);
                this.UID = m_Implement.GUID.ToString();
            }

            if (e.SelectedKey == 0)
            {
                PasswordGenerator gen = new PasswordGenerator();
                string newPassword = gen.Generate();
                this.Password = newPassword;
                return;
            }

            Utility.ReflectProperty(m_Implement, this);

            m_tempPassword = m_Implement.Password;

            if (!wim.IsEditMode && !IsPostBack)
                m_Password = "******";
            else
                m_Password = null;

            m_IsActive = m_Implement.IsActive;
            this.Language = m_Implement.Language;
            this.ShowSiteNavigation = m_Implement.ShowSiteNavigation;
            this.ShowFullWidth = m_Implement.ShowFullWidth;
            this.IsDeveloper = m_Implement.IsDeveloper;
            if (e.SelectedKey == 0) return;

            //if (!m_Implement.All_Sites)
                wim.AddTab(new Guid("93D10F58-6A1A-493F-8ADB-E53FC7CEDE19"));

        }

        [Framework.ContentSettingItem.RichText("Login intro", 0)]
        public string Page_Login { get; set; }

        [Framework.ContentSettingItem.Section("New account mail")]
        [Framework.ContentSettingItem.TextField("Mail title", 50)]
        public string Mail_Title { get; set; }

        [Sushi.Mediakiwi.Framework.ContentSettingItem.Binary_Image("Mail logo", CanOnlyAdd = true)]
        public virtual int? Mail_Logo { get; set; }

        [Sushi.Mediakiwi.Framework.ContentSettingItem.RichText("Mail intro", 0, InteractiveHelp = "Use [credentials], [name], [login], [email], [url] and http://url as placeholders.")]
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

        private string m_SearchUserName;
        /// <summary>
        /// Gets or sets the name of the search user.
        /// </summary>
        /// <value>The name of the search user.</value>
        [Framework.ContentListSearchItem.TextField("Username", 50, false, Expression = OutputExpression.Alternating)]
        public string SearchUserName
        {
            get { return m_SearchUserName; }
            set { m_SearchUserName = value; }
        }

        private string m_SearchRole;
        /// <summary>
        /// Gets or sets the search role.
        /// </summary>
        /// <value>The search role.</value>
        [Framework.ContentListSearchItem.Choice_Dropdown("Role", "AvailableRoles", false, false, null, Expression = OutputExpression.Alternating)]
        public string SearchRole
        {
            get { return m_SearchRole; }
            set { m_SearchRole = value; }
        }

        #endregion List search attributes

        #region List attributes

        private string m_Name;
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [OnlyEditableWhenTrue(nameof(IsNewElement))]
        [Framework.ContentListItem.TextField("Username", 50, true, Expression = OutputExpression.Alternating)]
        public string Name
        {
            get { return m_Name;  }
            set { m_Name = value; }
        }

        private string m_Email;
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        [Framework.ContentListItem.TextField("Emailaddress", 255, true, Expression = OutputExpression.Alternating)]
        public string Email
        {
            get { return m_Email; }
            set { m_Email = value; }
        }

        private string m_Displayname;
        /// <summary>
        /// Gets or sets the displayname.
        /// </summary>
        /// <value>The displayname.</value>
        [Framework.ContentListItem.TextField("Displayname", 50, true, Expression = OutputExpression.Alternating)]
        public string Displayname
        {
            get { return m_Displayname; }
            set { m_Displayname = value; }
        }

        private int m_RoleID;
        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        /// <value>The role.</value>
        [Framework.ContentListItem.Choice_Dropdown("Role", "AvailableRoles", true, true, Expression = OutputExpression.Alternating)]
        public int RoleID
        {
            get { return m_RoleID; }
            set { m_RoleID = value; }
        }

        private string m_Password;
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        //[Sushi.Mediakiwi.Framework.ContentListItem.TextField("Password", 50, false, null, Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public string Password
        {
            get { return m_Password; }
            set { m_Password = value; }
        }


        [Framework.ContentListItem.TextField("UID", 50, false, null, Expression = OutputExpression.Alternating)]
        public string UID { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>The language.</value>
        [Framework.ContentListItem.Choice_Dropdown("Language", "AvailableLanguages", true, Expression = OutputExpression.Alternating)]
        public int Language { get; set; }

        private bool m_IsActive;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        [Framework.ContentListItem.Choice_Checkbox("Is active", Expression = OutputExpression.Alternating)]
        public bool IsActive
        {
            get { return m_IsActive; }
            set { m_IsActive = value; }
        }

       
        //[Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Site navigation", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public bool ShowSiteNavigation { get; set; }

        [Framework.ContentListItem.Choice_Checkbox("Is Developer", Expression = OutputExpression.Alternating)]
        public bool IsDeveloper { get; set; }

        private bool _ForceNewStyle = true;
        //[Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Mediakiwi (Beta)", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public bool ForceNewStyle { 
            get
            {
                return _ForceNewStyle;
            }
            set
            {
                _ForceNewStyle = value;
            }
        }

        private bool _ForceNewStyle2 = true;
        //[Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Mediakiwi (Beta:2)", Expression = Sushi.Mediakiwi.Framework.OutputExpression.Alternating)]
        public bool ForceNewStyle2
        {
            get
            {
                return _ForceNewStyle2;
            }
            set
            {
                _ForceNewStyle2 = value;
            }
        }

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
                //Page.Trace.Write("AvailableRoles get{}", string.Format("{0}-{1}", SearchRole, SearchRole2));
                ListItemCollection collection = new ListItemCollection();
                collection.Add(new ListItem("Select a role", ""));
                foreach (Sushi.Mediakiwi.Data.ApplicationRole role in Sushi.Mediakiwi.Data.ApplicationRole.SelectAll())
                {
                    collection.Add(new ListItem(role.Name, role.ID.ToString()));
                }
                return collection;
            }
        }

        /// <summary>
        /// Gets the available roles2.
        /// </summary>
        /// <value>The available roles2.</value>
        public ListItemCollection AvailableRoles2
        {
            get
            {
                //Page.Trace.Write("AvailableRoles2 get{}", string.Format("{0}-{1}", SearchRole, SearchRole2));
                ListItemCollection collection = new ListItemCollection();
                collection.Add(new ListItem("Select a role", ""));
                foreach (Sushi.Mediakiwi.Data.ApplicationRole role in Sushi.Mediakiwi.Data.ApplicationRole.SelectAll())
                {
                    collection.Add(new ListItem(role.Name, role.ID.ToString()));
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
                //Page.Trace.Write("AvailableLanguages get{}", string.Format("{0}", Role));
                ListItemCollection collection = new ListItemCollection();
                ListItem li = new ListItem("English", "1");
                li.Selected = true;
                collection.Add(li);
                collection.Add(new ListItem("Nederlands","2"));
                return collection;
            }
        }

        private bool m_SendCredentials;
        /// <summary>
        /// Gets or sets a value indicating whether [send credentials].
        /// </summary>
        /// <value><c>true</c> if [send credentials]; otherwise, <c>false</c>.</value>
        //[Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsTextMode")]
        [Sushi.Mediakiwi.Framework.ContentListItem.Button("", true, ButtonClassName = "flaticon icon-envelope", InteractiveHelp = "Email login details")]
        public bool SendCredentials
        {
            get { return m_SendCredentials; }
            set { m_SendCredentials = value; }
        }

        [Sushi.Mediakiwi.Framework.ContentListItem.Button("", false, ButtonClassName = "flaticon icon-eye-slash", InteractiveHelp = "Impersonate this user")]
        public bool HijackUser { get; set; }

        //private bool m_CreateNewPassword;
        ///// <summary>
        ///// Gets or sets a value indicating whether [create new password].
        ///// </summary>
        ///// <value><c>true</c> if [create new password]; otherwise, <c>false</c>.</value>
        //[Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("IsNewElement", false)]
        //[Sushi.Mediakiwi.Framework.ContentListItem.Button("Send new password")]
        //public bool CreateNewPassword
        //{
        //    get { return m_CreateNewPassword;  }
        //    set { m_CreateNewPassword = value; }
        //}

        //private Sushi.Mediakiwi.Data.SubList m_Sites;
        ///// <summary>
        ///// Gets or sets the sites.
        ///// </summary>
        ///// <value>The sites.</value>
        //[Sushi.Mediakiwi.Framework.ContentListItem.SubListSelect("Channels", "18b297dc-7e01-404d-bc45-bb3ea3eb344e", false, "Select the channels to which this user has access")]
        //public Sushi.Mediakiwi.Data.SubList Sites
        //{
        //    get { return m_Sites; }
        //    set { m_Sites = value; }
        //}


        #endregion List attributes
    }
}
