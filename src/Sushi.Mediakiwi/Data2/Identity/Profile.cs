using System;
using System.Linq;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.Data.Identity
{
    /// <summary>
    /// This is the base class of profile. It can be extented using Sushi.Mediakiwi.Data.DalReflection.
    /// </summary>
    [DatabaseTable("wim_Profiles")]
    [ListReference("d6170f90-01ba-4700-b9c8-bdfdf12b5438")]
    public class Profile : IProfile, ISaveble
    {
        static IProfileParser _Parser;
        static IProfileParser Parser
        {
            get
            {
                if (_Parser == null)
                    _Parser = Environment.GetInstance<IProfileParser>();
                return _Parser;
            }
        }

        public Profile()
        {
            this.Created = DateTime.UtcNow;// Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
            this.GUID = Guid.NewGuid();
            this.Data = new CustomData();
        }

        /// <summary>
        /// Select the current visitor object. If no visitor exists an instance is created including a database reference.
        /// By default, the visitor reference is not stored accross sessions. 
        /// </summary>
        /// <returns></returns>
        public static IProfile Select()
        {
            return Parser.Select();
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public static IProfile[] SelectAll()
        {
            return Parser.SelectAll();
        }


        /// <summary>
        /// Save a database entity. This method will indentify the need for an INSERT or an UPDATE.
        /// By default the profile reference is stored in a Cookie
        /// </summary>
        /// <returns></returns>
        public virtual bool Save()
        {
            return Parser.Save(this);
        }

        /// <summary>
        /// Saves the specified should remember profile for next visit.
        /// </summary>
        /// <param name="shouldRememberVisitorForNextVisit">if set to <c>true</c> [should remember visitor for next visit].</param>
        /// <param name="isLoggedIn">if set to <c>true</c> [is logged in].</param>
        /// <returns></returns>
        public bool Save(bool shouldRememberVisitorForNextVisit, bool? isLoggedIn = null)
        {
            return Parser.Save(this, shouldRememberVisitorForNextVisit, isLoggedIn);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static IProfile SelectOne(int ID)
        {
            return Parser.SelectOne(ID);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public static IProfile SelectOne(string email)
        {
            return Parser.SelectOne(email);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="email">The Guid.</param>
        /// <returns></returns>
        public static IProfile SelectOne(Guid guid)
        {
            return Parser.SelectOne(guid);
        }




        internal static IProfile Login(string email, string password)
        {
            return Parser.Login(email, password);
        }

        /// <summary>
        /// Removes the profile identifying cookie. Any subsequent changes to this instances and ignored in this context call.
        /// </summary>
        /// <param name="redirectionPageID">The redirection page ID.</param>
        /// <returns></returns>
        public virtual bool Logout(int? redirectionPageID = null)
        {
            return Parser.Logout(redirectionPageID);
        }

        #region properties
        public bool IsNewInstance { get { return ID == 0; } }

        [DatabaseColumn("Profile_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public int ID { get; set; }

        [DatabaseColumn("Profile_Guid", SqlDbType.UniqueIdentifier, IsMigrationKey = true)]
        public Guid GUID { get; set; }
        [Sushi.Mediakiwi.Framework.ContentListItem.ContentContainer()]
        [DatabaseColumn("Profile_Data", SqlDbType.Xml, IsNullable = true)]
        public Sushi.Mediakiwi.Data.CustomData Data { get; set; }
        [DatabaseColumn("Profile_Created", SqlDbType.DateTime)]
        public DateTime Created { get; set; }
        public Guid ProfileReference { get;  }
        [DatabaseColumn("profile_Email", SqlDbType.NVarChar, Length = 255, IsNullable = true)]
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Email", 255, false)]
        public string Email { get; set; }
        [DatabaseColumn("profile_Password", SqlDbType.NVarChar, Length = 50, IsNullable = true)]
        [Sushi.Mediakiwi.Framework.OnlyVisibleWhenFalse("HidePasswordField")]
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Password", 50, false)]
        public string Password { get; set; }
        [DatabaseColumn("Profile_RememberMe", SqlDbType.Bit, IsNullable = true)]
        public bool RememberMe { get; set; }
        #endregion
    }
}