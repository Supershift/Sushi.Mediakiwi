using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.Data.Parsers
{
    public class ApplicationUserParser : IApplicationUserParser
    {
        static ISqlEntityParser _DataParser;
        static ISqlEntityParser DataParser
        {
            get
            {
                if (_DataParser == null)
                    _DataParser = Environment.GetInstance<ISqlEntityParser>();
                return _DataParser;
            }
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public virtual void Clear()
        {
            if (!Wim.CommonConfiguration.IS_LOAD_BALANCED) return;
            DataParser.Execute("truncate table wim_Users");
        }


        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public virtual IApplicationUser SelectOne(int ID)
        {
            return DataParser.SelectOne<IApplicationUser>(ID, true);
        }

        const string CACHEKEYPREFIX = "Data.User.";

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns></returns>
        public virtual IApplicationUser SelectOne(string username)
        {
            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            list.Add(new DatabaseDataValueColumn("User_Name", SqlDbType.NVarChar, username));
            return DataParser.SelectOne<IApplicationUser>(list);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="applicationUserGUID">The application user GUID.</param>
        /// <returns></returns>
        public virtual IApplicationUser SelectOne(Guid applicationUserGUID)
        {
            //ApplicationUser tmp = new ApplicationUser();

            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            list.Add(new DatabaseDataValueColumn("User_Guid", SqlDbType.UniqueIdentifier, applicationUserGUID));
            list.Add(new DatabaseDataValueColumn("User_IsActive", SqlDbType.Bit, true));

            //tmp = (ApplicationUser)tmp._SelectOne(list, "GUID", applicationUserGUID.ToString());
            //return tmp;

            return DataParser.SelectOne<IApplicationUser>(list, applicationUserGUID.ToString());
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public virtual IApplicationUser[] SelectAll()
        {
            return DataParser.SelectAll<IApplicationUser>().ToArray();
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="role">The role.</param>
        /// <returns></returns>
        public virtual IApplicationUser[] SelectAll(string username, int role)
        {
            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            if (!string.IsNullOrEmpty(username))
            {
                username = string.Concat("%", username.Replace(" ", "%"), "%");
                where.Add(new DatabaseDataValueColumn("User_Displayname", SqlDbType.NVarChar, username, DatabaseDataValueCompareType.Like));
            }

            if (role > 0)
                where.Add(new DatabaseDataValueColumn("User_Role_Key", SqlDbType.Int, role));

            //foreach (object o in implement._SelectAll(where))
            //    list.Add((ApplicationUser)o);
            //return list.ToArray();
            return DataParser.SelectAll<IApplicationUser>(where).ToArray();
        }

        /// <summary>
        /// Selects all active application Users for a specific role.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <returns></returns>
        public virtual IApplicationUser[] SelectAll(int? role)
        {
            return SelectAll(role, true);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="onlyReturnActive">if set to <c>true</c> [only return active].</param>
        /// <returns></returns>
        public virtual IApplicationUser[] SelectAll(int? role, bool onlyReturnActive)
        {
            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();

            if (role.HasValue)
                where.Add(new DatabaseDataValueColumn("User_Role_Key", SqlDbType.Int, role));

            if (onlyReturnActive)
                where.Add(new DatabaseDataValueColumn("User_IsActive", SqlDbType.Bit, true));

            return DataParser.SelectAll<IApplicationUser>(where).ToArray();
        }

        //TODO Is this correct!?
        public virtual bool Save(IApplicationUser entity)
        {
            return (DataParser.Save<IApplicationUser>(entity) > 0);
        }


        /// <summary>
        /// Determines whether the specified email has email. (Based on HasEmail)
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="ignoreApplicationUserID">The ignore application user ID.</param>
        /// <returns>
        /// <c>true</c> if the specified email has email; otherwise, <c>false</c>.
        /// </returns>
        public virtual IApplicationUser SelectOne(string email, int? ignoreApplicationUserID)
        {
            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("User_Email", SqlDbType.NVarChar, email));

            if (ignoreApplicationUserID.HasValue)
                where.Add(new DatabaseDataValueColumn("NOT User_Key", SqlDbType.Int, ignoreApplicationUserID.Value));

            return DataParser.SelectOne<IApplicationUser>(where, null, null);
        }

        /// <summary>
        /// Determines whether the specified Username is in use
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="ignoreApplicationUserID">The ignore application user ID.</param>
        /// <returns>
        /// <c>true</c> if the specified email has email; otherwise, <c>false</c>.
        /// </returns>
        public virtual IApplicationUser SelectOneByUserName(string username, int? ignoreApplicationUserID)
        {
            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("User_Name", SqlDbType.NVarChar, username));

            if (ignoreApplicationUserID.HasValue)
                where.Add(new DatabaseDataValueColumn("NOT User_Key", SqlDbType.Int, ignoreApplicationUserID.Value));

            return DataParser.SelectOne<IApplicationUser>(where, null, null);
        }

        /// <summary>
        /// Based on the Apply function. (ApplicationUser.cs (270))
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public virtual IApplicationUser SelectOne(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return new ApplicationUser();

            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            if (username.Contains("@"))
                list.Add(new DatabaseDataValueColumn("User_Email", SqlDbType.VarChar, username));
            else
                list.Add(new DatabaseDataValueColumn("User_Name", SqlDbType.VarChar, username));

            return DataParser.SelectOne<IApplicationUser>(list);
        }

        public virtual IApplicationUser SelectOneByEmail(string emailaddress)
        {
            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            list.Add(new DatabaseDataValueColumn("User_Email", SqlDbType.VarChar, emailaddress));
            return DataParser.SelectOne<IApplicationUser>(list);
        }

        /// <summary>
        /// Delete an implementation record.
        /// </summary>
        /// <returns></returns>
        public virtual bool Delete(IApplicationUser entity)
        {
            return DataParser.Delete<IApplicationUser>(entity);
        }
    }
}