using System;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    public interface IApplicationUser
    {
        [Obsolete("This property isn't used anywhere", false)]
        string NetworkIdentification { get; set; }

        [Obsolete("This property isn't used anywhere", false)]
        bool HasVisitorReference { get; }

        /// <summary>
        /// When was this user created
        /// </summary>
        DateTime Created { get; set; }

        /// <summary>
        /// Customdata for this user
        /// </summary>
        CustomData Data { get; set; }

        /// <summary>
        /// The displayname for this user
        /// </summary>
        string Displayname { get; set; }

        /// <summary>
        /// The users E-mail address
        /// </summary>
        string Email { get; set; }

        /// <summary>
        /// The users Globally Unique Identifier
        /// </summary>
        Guid GUID { get; set; }

        /// <summary>
        /// The users ID
        /// </summary>
        int ID { get; set; }

        /// <summary>
        /// Is this user active and allowed access ?
        /// </summary>
        bool IsActive { get; set; }

        /// <summary>
        /// Is this user a developer ?
        /// </summary>
        bool IsDeveloper { get; set; }

        /// <summary>
        /// What is the users preferred language
        /// </summary>
        int Language { get; set; }

        /// <summary>
        /// What is the users preferred culture
        /// </summary>
        string LanguageCulture { get; }

        /// <summary>
        /// When did this user last login ?
        /// </summary>
        DateTime? LastLoggedVisit { get; set; }

        /// <summary>
        /// The username for this user
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The password for this user
        /// </summary>
        string Password { get; set; }

        /// <summary>
        /// Should the users e-mail address or username be remembered ?
        /// </summary>
        bool RememberMe { get; set; }

        /// <summary>
        /// When set, the user requested a password reset and this GUID is validated upon setting a new password
        /// </summary>
        Guid? ResetKey { get; set; }

        /// <summary>
        /// To which ApplicationRole does this user belong ?
        /// </summary>
        int RoleID { get; set; }

        /// <summary>
        /// What is the name of the AppllicationRole
        /// </summary>
        string RoleName { get; }

        /// <summary>
        /// Is DetailView enabled for this user ?
        /// </summary>
        bool ShowDetailView { get; set; }

        /// <summary>
        /// is FullWidth mode enabled for this user ?
        /// </summary>
        bool ShowFullWidth { get; set; }

        /// <summary>
        /// Does the user want to see hidden items ?
        /// </summary>
        bool ShowHidden { get; set; }

        /// <summary>
        /// Does the user want to see the Site Navigation ?
        /// </summary>
        bool ShowSiteNavigation { get; set; }

        /// <summary>
        /// Does the user want to see the Translation View ?
        /// </summary>
        bool ShowTranslationView { get; set; }

        /// <summary>
        /// The encryption type for this users password
        /// </summary>
        int Type { get; set; }

        /// <summary>
        /// The visitor reference for this user ?
        /// </summary>
        Guid VisitorReference { get; }

        /// <summary>
        /// Encrypts a password based on the set encryption type for this user
        /// </summary>
        /// <param name="password">The unencrypted password</param>
        void ApplyPassword(string password);

        /// <summary>
        /// Is the supplied E-Mail Address already taken ?
        /// </summary>
        /// <param name="email">The e-mail address to check</param>
        /// <returns>TRUE when the supplied e-mail address is taken, FALSE if it isn't</returns>
        bool HasEmail(string email);

        /// <summary>
        /// Is the supplied E-Mail Address already taken ?
        /// </summary>
        /// <param name="email">The e-mail address to check</param>
        /// <returns>TRUE when the supplied e-mail address is taken, FALSE if it isn't</returns>
        Task<bool> HasEmailAsync(string email);

        /// <summary>
        /// Is the supplied Username already taken ?
        /// </summary>
        /// <param name="username">The Username to check</param>
        /// <returns>TRUE when the supplied Username is taken, FALSE if it isn't</returns>
        bool HasUserName(string username);

        /// <summary>
        /// Is the supplied Username already taken ?
        /// </summary>
        /// <param name="username">The Username to check</param>
        /// <returns>TRUE when the supplied Username is taken, FALSE if it isn't</returns>
        Task<bool> HasUserNameAsync(string username);

        /// <summary>
        /// Selects the ApplicationRole attached to this user
        /// </summary>
        /// <returns></returns>
        IApplicationRole SelectRole();

        /// <summary>
        /// Selects the ApplicationRole attached to this user
        /// </summary>
        /// <returns></returns>
        Task<IApplicationRole> SelectRoleAsync();

        /// <summary>
        /// Saves this user
        /// </summary>
        /// <returns>TRUE if succeeded</returns>
        bool Save();

        /// <summary>
        /// Saves this user
        /// </summary>
        /// <returns>TRUE if succeeded</returns>
        Task<bool> SaveAsync();

        /// <summary>
        /// Deletes this user
        /// </summary>
        /// <returns>TRUE if succeeded</returns>
        bool Delete();

        /// <summary>
        /// Deletes this user
        /// </summary>
        /// <returns>TRUE if succeeded</returns>
        Task<bool> DeleteAsync();

        /// <summary>
        /// Is this a new User without ID ?
        /// </summary>
        bool IsNewInstance { get; }
    }
}