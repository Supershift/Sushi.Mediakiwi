using Sushi.Mediakiwi.Data.MicroORM;
using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Represents a ApplicationUser entity.
    /// </summary>
    [DataMap(typeof(ApplicationUserMap))]
    public class ApplicationUser : IApplicationUser
    {
        private string m_Ticket;

        internal string Ticket
        {
            get
            {
                if (string.IsNullOrEmpty(m_Ticket))
                    m_Ticket = string.Concat("w_", Environment.Current.Title.Replace(" ", "_"));

                return m_Ticket;
            }
        }

        internal class ApplicationUserMap : DataMap<ApplicationUser>
        {
            public ApplicationUserMap() : this(false)
            {
                
            }

            public ApplicationUserMap(bool isSave)
            {
                if (isSave)
                {
                    Table("wim_Users");
                }
                else
                {
                    Table("wim_Users left join wim_Roles on Role_Key = User_Role_Key");
                }

                Id(x => x.ID, "User_Key").Identity().SqlType(SqlDbType.Int);
                Map(x => x.GUID, "User_Guid").SqlType(SqlDbType.UniqueIdentifier);
                Map(x => x.Name, "User_Name").SqlType(SqlDbType.NVarChar).Length(50);
                Map(x => x.Displayname, "User_Displayname").SqlType(SqlDbType.NVarChar).Length(50);
                Map(x => x.RoleID, "User_Role_Key").SqlType(SqlDbType.Int);
                Map(x => x.Language, "User_Language").SqlType(SqlDbType.Int);
                Map(x => x.Email, "User_Email").SqlType(SqlDbType.NVarChar).Length(255);
                Map(x => x.NetworkIdentification, "User_Network").SqlType(SqlDbType.NVarChar).Length(50);
                Map(x => x.ResetKey, "User_Reset").SqlType(SqlDbType.UniqueIdentifier);
                Map(x => x.Type, "User_Type").SqlType(SqlDbType.Int);
                Map(x => x.Password, "User_Password").SqlType(SqlDbType.NVarChar).Length(50);
                Map(x => x.IsActive, "User_IsActive").SqlType(SqlDbType.Bit);

                Map(x => x.Created, "User_Created").SqlType(SqlDbType.DateTime);
                Map(x => x.LastLoggedVisit, "User_LastLoggedVisit").SqlType(SqlDbType.DateTime);
                Map(x => x.RememberMe, "User_RememberMe").SqlType(SqlDbType.Bit);
                Map(x => x.DataString, "User_Data").SqlType(SqlDbType.Xml);

                Map(x => x.ShowDetailView, "User_Detailview").SqlType(SqlDbType.Bit);
                Map(x => x.ShowTranslationView, "User_Translationview").SqlType(SqlDbType.Bit);

                Map(x => x.RoleName, "Role_Name").ReadOnly();
            }
        }


        #region properties        

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public int ID { get; set; }

        private Guid m_GUID;

        /// <summary>
        /// Gets or sets the GUID.
        /// </summary>
        /// <value>The GUID.</value>
        public Guid GUID
        {
            get
            {
                if (m_GUID == Guid.Empty)
                {
                    m_GUID = Guid.NewGuid();
                }
                return m_GUID;
            }
            set { m_GUID = value; }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the displayname.
        /// </summary>
        /// <value>The displayname.</value>
        public string Displayname { get; set; }

        /// <summary>
        /// Gets or sets the name of the role.
        /// </summary>
        /// <value>The name of the role.</value>
        public string RoleName { get; set; }

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        /// <value>The role.</value>
        public int RoleID { get; set; }

        /// <summary>
        /// Roles this instance.
        /// </summary>
        /// <returns></returns>
        public IApplicationRole SelectRole()
        {
            return ApplicationRole.SelectOne(RoleID);
        }

        /// <summary>
        /// Roles this instance.
        /// </summary>
        /// <returns></returns>
        public async Task<IApplicationRole> SelectRoleAsync()
        {
            return await ApplicationRole.SelectOneAsync(RoleID).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>The language.</value>
        public int Language { get; set; }

        /// <summary>
        /// Gets the language culture.
        /// </summary>
        /// <value>The language culture.</value>
        public string LanguageCulture
        {
            get
            {
                if (Language == 2)
                {
                    return "nl-NL";
                }
                return "en-GB";
            }
        }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the network identification.
        /// </summary>
        /// <value>The network identification.</value>
        public string NetworkIdentification { get; set; }

        /// <summary>
        /// Gets or sets the network identification.
        /// </summary>
        /// <value>
        /// The network identification.
        /// </value>
        public Guid? ResetKey { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public int Type { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        public string Password { get; set; }

        /// <summary>
        ///
        /// </summary>

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool IsActive { get; set; }

        private DateTime m_Created;

        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created
        {
            get
            {
                if (m_Created == DateTime.MinValue)
                {
                    m_Created = Common.DatabaseDateTime;
                }
                return m_Created;
            }
            set { m_Created = value; }
        }

        //DateTime m_LastLoggedVisit;
        /// <summary>
        /// Gets or sets the last logged visit.
        /// </summary>
        /// <value>The last logged visit.</value>
        public DateTime? LastLoggedVisit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [remember me].
        /// </summary>
        /// <value><c>true</c> if [remember me]; otherwise, <c>false</c>.</value>
        public bool RememberMe { get; set; }

        /// <summary>
        /// Respresents the DATA propery as serialized XML
        /// </summary>
        public string DataString { get; set; }

        private CustomData m_Data;

        /// <summary>
        /// Holds all customData properties
        /// </summary>
        public CustomData Data
        {
            get
            {
                if (m_Data == null)
                {
                    m_Data = new CustomData(DataString);
                }
                return m_Data;
            }
            set
            {
                m_Data = value;
                DataString = m_Data.Serialized;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show detail view].
        /// </summary>
        /// <value><c>true</c> if [show detail view]; otherwise, <c>false</c>.</value>
        public bool ShowDetailView { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show translation view].
        /// </summary>
        /// <value><c>true</c> if [show translation view]; otherwise, <c>false</c>.</value>
        public bool ShowTranslationView { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is developer.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is developer; otherwise, <c>false</c>.
        /// </value>
        public bool IsDeveloper
        {
            get
            {
                if (Data == null)
                {
                    return false;
                }
                return Data["IsDeveloper"].ParseBoolean(false);
            }
            set
            {
                if (Data != null)
                {
                    Data.Apply("IsDeveloper", value);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show the environment in full width mode
        /// </summary>
        public bool ShowFullWidth
        {
            get
            {
                if (Data == null)
                {
                    return false;
                }
                return Data["FW"].ParseBoolean(false);
            }
            set
            {
                if (Data != null)
                {
                    Data.Apply("FW", value);
                }
            }
        }

        /// <summary>
        /// Only posible in combination with "Is Developer"
        /// </summary>
        public bool ShowHidden
        {
            get
            {
                if (!IsDeveloper || Data == null)
                {
                    return false;
                }

                return Data["ShowHidden"].ParseBoolean(false);
            }
            set
            {
                if (Data != null)
                {
                    if (value == false)
                    {
                        Data.Apply("ShowHidden", null);
                    }
                    else
                    {
                        Data.Apply("ShowHidden", value);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show site navigation].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show site navigation]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowSiteNavigation
        {
            get
            {
                if (Data == null)
                {
                    return false;
                }
                return Data["SiteNav"].ParseBoolean(false);
            }
            set
            {
                if (Data != null)
                {
                    Data.Apply("SiteNav", value);
                }
            }
        }

        #endregion properties

        /// <summary>
        /// Select a Application User based on its primary key
        /// </summary>
        /// <param name="ID">Uniqe identifier of the Menu</param>
        public static async Task<IApplicationUser> SelectOneAsync(int ID, bool onlyReturnActive = false)
        {
            var connector = ConnectorFactory.CreateConnector<ApplicationUser>();
            var filter = connector.CreateQuery();

            filter.Add(x => x.ID, ID);
            if (onlyReturnActive)
            {
                filter.Add(x => x.IsActive, true);
            }

            return await connector.FetchSingleAsync(filter).ConfigureAwait(false);
        }

        /// <summary>
        /// Select a Application User based on its primary key
        /// </summary>
        /// <param name="ID">Uniqe identifier of the Menu</param>
        public static IApplicationUser SelectOne(string email, bool onlyReturnActive = false)
        {
            var connector = ConnectorFactory.CreateConnector<ApplicationUser>();
            var filter = connector.CreateQuery();

            filter.Add(x => x.Email, email);
            if (onlyReturnActive)
            {
                filter.Add(x => x.IsActive, true);
            }
            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Select a Application User based on its primary key
        /// </summary>
        /// <param name="ID">Uniqe identifier of the Menu</param>
        public static IApplicationUser SelectOne(int ID, bool onlyReturnActive = false)
        {
            var connector = ConnectorFactory.CreateConnector<ApplicationUser>();
            var filter = connector.CreateQuery();

            filter.Add(x => x.ID, ID);
            if (onlyReturnActive)
            {
                filter.Add(x => x.IsActive, true);
            }
            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Select a Application User based on the name
        /// </summary>
        /// <param name="username">The username.</param>
        public static IApplicationUser SelectOne(string username)
        {
            var connector = ConnectorFactory.CreateConnector<ApplicationUser>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.Name, username);
            
            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Select a Application User based on the name
        /// </summary>
        /// <param name="username">The username.</param>
        public static async Task<IApplicationUser> SelectOneAsync(string username)
        {
            var connector = ConnectorFactory.CreateConnector<ApplicationUser>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.Name, username);
            
            return await connector.FetchSingleAsync(filter).ConfigureAwait(false);
        }

        /// <summary>
        /// Select a Application User based on the name
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="ignoreApplicationUserID">Filter will be set to ignore this UserID</param>
        public static IApplicationUser SelectOne(string username, int? ignoreApplicationUserID)
        {
            var connector = ConnectorFactory.CreateConnector<ApplicationUser>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.Name, username);
            if (ignoreApplicationUserID.HasValue)
            {
                filter.Add(x => x.ID, ignoreApplicationUserID.Value, ComparisonOperator.NotEqualTo);
            }
            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Select a Application User based on the name
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="ignoreApplicationUserID">Filter will be set to ignore this UserID</param>
        public static async Task<IApplicationUser> SelectOneAsync(string username, int? ignoreApplicationUserID)
        {
            var connector = ConnectorFactory.CreateConnector<ApplicationUser>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.Name, username);
            if (ignoreApplicationUserID.HasValue)
            {
                filter.Add(x => x.ID, ignoreApplicationUserID.Value, ComparisonOperator.NotEqualTo);
            }
            return await connector.FetchSingleAsync(filter).ConfigureAwait(false);
        }

        /// <summary>
        /// Select a Application User based on the email
        /// </summary>
        /// <param name="email">The email.</param>
        public static IApplicationUser SelectOneByEmail(string email)
        {
            var connector = ConnectorFactory.CreateConnector<ApplicationUser>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.Email, email);
            
            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Select a Application User based on the email
        /// </summary>
        /// <param name="email">The email.</param>
        public static async Task<IApplicationUser> SelectOneByEmailAsync(string email)
        {
            var connector = ConnectorFactory.CreateConnector<ApplicationUser>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.Email, email);
            
            return await connector.FetchSingleAsync(filter).ConfigureAwait(false);
        }

        /// <summary>
        /// Select a Application User based on the email
        /// </summary>
        /// <param name="email">The email.</param>
        public static IApplicationUser SelectOneByEmail(string email, int? ignoreApplicationUserID)
        {
            var connector = ConnectorFactory.CreateConnector<ApplicationUser>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.Email, email);
            if (ignoreApplicationUserID.HasValue)
            {
                filter.Add(x => x.ID, ignoreApplicationUserID.Value, ComparisonOperator.NotEqualTo);
            }
            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Select a Application User based on the email
        /// </summary>
        /// <param name="email">The email.</param>
        public static async Task<IApplicationUser> SelectOneByEmailAsync(string email, int? ignoreApplicationUserID)
        {
            var connector = ConnectorFactory.CreateConnector<ApplicationUser>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.Email, email);
            if (ignoreApplicationUserID.HasValue)
            {
                filter.Add(x => x.ID, ignoreApplicationUserID.Value, ComparisonOperator.NotEqualTo);
            }
            return await connector.FetchSingleAsync(filter).ConfigureAwait(false);
        }

        /// <summary>
        /// Select a Application User based on GUID
        /// </summary>
        /// <param name="applicationUserGUID">The application user GUID.</param>
        public static IApplicationUser SelectOne(Guid applicationUserGUID)
        {
            var connector = ConnectorFactory.CreateConnector<ApplicationUser>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.GUID, applicationUserGUID);
            
            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Select a Application User based on the email
        /// </summary>
        /// <param name="email">The email.</param>
        public static async Task<IApplicationUser> SelectOneAsync(Guid applicationUserGUID)
        {
            var connector = ConnectorFactory.CreateConnector<ApplicationUser>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.GUID, applicationUserGUID);
            
            return await connector.FetchSingleAsync(filter).ConfigureAwait(false);
        }

        /// <summary>
        /// Based on the Apply function. (ApplicationUser.cs (270))
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static IApplicationUser SelectOne(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return new ApplicationUser();

            var connector = ConnectorFactory.CreateConnector<ApplicationUser>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.Name, username);
            filter.Add(x => x.Password, password);
            
            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Based on the Apply function. (ApplicationUser.cs (270))
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static async Task<IApplicationUser> SelectOneAsync(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return new ApplicationUser();
            }
            var connector = ConnectorFactory.CreateConnector<ApplicationUser>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.Name, username);
            filter.Add(x => x.Password, password);
            
            return await connector.FetchSingleAsync(filter).ConfigureAwait(false);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        public static IApplicationUser[] SelectAll()
        {
            var connector = ConnectorFactory.CreateConnector<ApplicationUser>();
            var filter = connector.CreateQuery();
            
            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        public static async Task<IApplicationUser[]> SelectAllAsync()
        {
            var connector = ConnectorFactory.CreateConnector<ApplicationUser>();
            var filter = connector.CreateQuery();
            var result = await connector.FetchAllAsync(filter).ConfigureAwait(false);
            
            return result.ToArray();
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="username">The Display Name.</param>
        /// <param name="role">The role.</param>
        public static IApplicationUser[] SelectAll(string username, int role)
        {
            var connector = ConnectorFactory.CreateConnector<ApplicationUser>();
            var filter = connector.CreateQuery();

            if (!string.IsNullOrWhiteSpace(username))
            {
                filter.Add(x => x.Displayname, username);
            }
            if (role > 0)
            {
                filter.Add(x => x.RoleID, role);
            }
            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="username">The Display Name.</param>
        /// <param name="role">The role.</param>
        public static async Task<IApplicationUser[]> SelectAllAsync(string username, int role)
        {
            var connector = ConnectorFactory.CreateConnector<ApplicationUser>();
            var filter = connector.CreateQuery();

            if (!string.IsNullOrWhiteSpace(username))
            {
                filter.Add(x => x.Displayname, $"%{username}%", ComparisonOperator.Like);
            }
            if (role > 0)
            {
                filter.Add(x => x.RoleID, role);
            }
            var result = await connector.FetchAllAsync(filter).ConfigureAwait(false);
            return result.ToArray();
        }

        /// <summary>
        /// Selects all active application Users for a specific role.
        /// </summary>
        /// <param name="role">If role is set, a filter will be set for this specific role</param>
        public static IApplicationUser[] SelectAll(int? role)
        {
            var connector = ConnectorFactory.CreateConnector<ApplicationUser>();
            var filter = connector.CreateQuery();
            if (role.HasValue)
            {
                filter.Add(x => x.RoleID, role.Value);
            }
            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all active application Users for a specific role.
        /// </summary>
        /// <param name="role">If role is set, a filter will be set for this specific role</param>
        public static async Task<IApplicationUser[]> SelectAllAsync(int? role)
        {
            var connector = ConnectorFactory.CreateConnector<ApplicationUser>();
            var filter = connector.CreateQuery();
            if (role.HasValue)
            {
                filter.Add(x => x.RoleID, role.Value);
            }
            var result = await connector.FetchAllAsync(filter).ConfigureAwait(false);
            return result.ToArray();
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="role">If role is set, a filter will be set for this specific role</param>
        /// <param name="onlyReturnActive">if set to <c>true</c> [only return active].</param>
        /// <returns></returns>
        public static IApplicationUser[] SelectAll(int? role, bool onlyReturnActive)
        {
            var connector = ConnectorFactory.CreateConnector<ApplicationUser>();
            var filter = connector.CreateQuery();
            if (role.HasValue)
            {
                filter.Add(x => x.RoleID, role.Value);
            }
            if (onlyReturnActive)
            {
                filter.Add(x => x.IsActive, true);
            }
            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="role">If role is set, a filter will be set for this specific role</param>
        /// <param name="onlyReturnActive">if set to <c>true</c> [only return active].</param>
        /// <returns></returns>
        public static async Task<IApplicationUser[]> SelectAllAsync(int? role, bool onlyReturnActive)
        {
            var connector = ConnectorFactory.CreateConnector<ApplicationUser>();
            var filter = connector.CreateQuery();
            if (role.HasValue)
            {
                filter.Add(x => x.RoleID, role.Value);
            }
            if (onlyReturnActive)
            {
                filter.Add(x => x.IsActive, true);
            }
            
            var result = await connector.FetchAllAsync(filter).ConfigureAwait(false);
            return result.ToArray();
        }

        /// <summary>
        /// Determines whether [has user name] [the specified username].
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="ignoreApplicationUserID">The ignore application user ID.</param>
        /// <returns>
        /// 	<c>true</c> if [has user name] [the specified username]; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasUserName(string username, int? ignoreApplicationUserID)
        {
            IApplicationUser tmp = SelectOne(username, ignoreApplicationUserID);

            if (tmp == null || tmp.IsNewInstance)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Determines whether [has user name] [the specified username].
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="ignoreApplicationUserID">The ignore application user ID.</param>
        /// <returns>
        /// 	<c>true</c> if [has user name] [the specified username]; otherwise, <c>false</c>.
        /// </returns>
        public static async Task<bool> HasUserNameAsync(string username, int? ignoreApplicationUserID)
        {
            IApplicationUser tmp = await SelectOneAsync(username, ignoreApplicationUserID).ConfigureAwait(false);

            if (tmp == null || tmp.IsNewInstance)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Determines whether [has user name] [the specified username].
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>
        ///   <c>true</c> if [has user name] [the specified username]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasUserName(string username)
        {
            return HasUserName(username, ID);
        }

        /// <summary>
        /// Determines whether [has user name] [the specified username].
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>
        ///   <c>true</c> if [has user name] [the specified username]; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> HasUserNameAsync(string username)
        {
            return await HasUserNameAsync(username, ID).ConfigureAwait(false);
        }

        public bool HasEmail(string email)
        {
            return HasEmail(email, ID);
        }

        public async Task<bool> HasEmailAsync(string email)
        {
            return await HasEmailAsync(email, ID).ConfigureAwait(false);
        }

        /// <summary>
        /// Determines whether the specified email has email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="ignoreApplicationUserID">The ignore application user ID.</param>
        /// <returns>
        /// 	<c>true</c> if the specified email has email; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasEmail(string email, int? ignoreApplicationUserID)
        {
            IApplicationUser tmp = SelectOneByEmail(email, ignoreApplicationUserID);

            if (tmp == null || tmp.IsNewInstance)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Determines whether the specified email has email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="ignoreApplicationUserID">The ignore application user ID.</param>
        /// <returns>
        /// 	<c>true</c> if the specified email has email; otherwise, <c>false</c>.
        /// </returns>
        public static async Task<bool> HasEmailAsync(string email, int? ignoreApplicationUserID)
        {
            IApplicationUser tmp = await SelectOneByEmailAsync(email, ignoreApplicationUserID).ConfigureAwait(false);

            if (tmp == null || tmp.IsNewInstance)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Selects the specified emailaddress.
        /// </summary>
        /// <param name="emailaddress">The emailaddress.</param>
        /// <returns></returns>
        public static IApplicationUser Select(string emailaddress)
        {
            var tmp = SelectOneByEmail(emailaddress);

            if (tmp == null || tmp.IsNewInstance || !tmp.IsActive)
            {
                return new ApplicationUser();
            }
            return tmp;
        }

        /// <summary>
        /// Selects the specified emailaddress.
        /// </summary>
        /// <param name="emailaddress">The emailaddress.</param>
        /// <returns></returns>
        public static async Task<IApplicationUser> SelectAsync(string emailaddress)
        {
            var tmp = await SelectOneByEmailAsync(emailaddress).ConfigureAwait(false);

            if (tmp == null || tmp.IsNewInstance || !tmp.IsActive)
            {
                return new ApplicationUser();
            }
            return tmp;
        }

        public bool Delete()
        {
            var connector = ConnectorFactory.CreateConnector(new ApplicationUserMap(true));
            connector.Delete(this);
            return true;
        }

        public async Task<bool> DeleteAsync()
        {
            var connector = ConnectorFactory.CreateConnector(new ApplicationUserMap(true));
            await connector.DeleteAsync(this).ConfigureAwait(false);
            return true;
        }

        /// <summary>
        /// Gets a value indicating whether this instance has a visitor reference (cookie).
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has visitor reference; otherwise, <c>false</c>.
        /// </value>
        public bool HasVisitorReference
        {
            get
            {
                if (VisitorReference == Guid.Empty)
                {
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// By default the profile reference is NOT stored in a Cookie. Use Save(true) for saving a cookie.
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            if (string.IsNullOrEmpty(Name))
            {
                return false;
            }

            if (HasUserName(Name, ID))
            {
                throw new Exception("The applied username already exists");
            }

            if (HasEmail(Email, ID))
            {
                throw new Exception("The applied email already exists");
            }

            DataString = m_Data?.Serialized;

            var connector = ConnectorFactory.CreateConnector(new ApplicationUserMap(true));
            connector.Save(this);

            return true;
        }

        /// <summary>
        /// By default the profile reference is NOT stored in a Cookie. Use Save(true) for saving a cookie.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SaveAsync()
        {
            if (string.IsNullOrEmpty(Name))
            {
                return false;
            }

            if (await HasUserNameAsync(Name, ID).ConfigureAwait(false))
            {
                throw new Exception("The applied username already exists");
            }

            if (await HasEmailAsync(Email, ID).ConfigureAwait(false))
            {
                throw new Exception("The applied email already exists");
            }

            DataString = m_Data?.Serialized;

            var connector = ConnectorFactory.CreateConnector(new ApplicationUserMap(true));
            await connector.SaveAsync(this).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Gets the alternative name suggestion.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="ignoreApplicationUserID">The ignore application user ID.</param>
        /// <returns></returns>
        public static string GetAlternativeNameSuggestion(string name, int? ignoreApplicationUserID)
        {
            string candidate = name;
            int count = 0;
            while (HasUserName(candidate, ignoreApplicationUserID.GetValueOrDefault()))
            {
                count++;
                candidate = string.Concat(name, count);
            }
            return candidate;
        }

        /// <summary>
        /// Applies the password.
        /// </summary>
        /// <param name="password">The password.</param>
        public void ApplyPassword(string password)
        {
            Password = Utility.HashStringByMD5(string.Concat(Name, password));
            Type = 1;
        }

        public bool IsNewInstance
        {
            get { return ID == 0; }
        }

        private Guid m_VisitorReference;
        /// <summary>
        /// Gets the externally (cookie) stored profile reference.
        /// </summary>
        /// <value>The profile reference.</value>
        public Guid VisitorReference
        {
            get
            {
                return m_VisitorReference;
            }
        }
    }
}