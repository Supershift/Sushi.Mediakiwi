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
    public class ProfileParser : IProfileParser
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
        /// Select the current visitor object. If no visitor exists an instance is created including a database reference.
        /// By default, the visitor reference is not stored accross sessions. 
        /// </summary>
        /// <returns></returns>
        public virtual IProfile Select()
        {
            IProfile tmp;
     
            if (System.Web.HttpContext.Current.Items["wim.profile"] != null)
            {
                tmp = System.Web.HttpContext.Current.Items["wim.profile"] as Sushi.Mediakiwi.Data.Identity.Profile;
                if (tmp != null)
                    return tmp;
            }

            int? profileID = Visitor.Select().ProfileID;
            if (profileID.HasValue)
            {
                var p = SelectOne(profileID.Value);;
                System.Web.HttpContext.Current.Items["wim.profile"] = p;
                return p;
            }
            System.Web.HttpContext.Current.Items["wim.profile"] = new Profile();
            return new Profile();
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public virtual IProfile[] SelectAll()
        {
            return DataParser.SelectAll<IProfile>().ToArray();
        }


        /// <summary>
        /// Save a database entity. This method will indentify the need for an INSERT or an UPDATE.
        /// By default the profile reference is stored in a Cookie
        /// </summary>
        /// <returns></returns>
        public virtual bool Save(IProfile entity)
        {
            return Save(entity, entity.RememberMe);
        }

        /// <summary>
        /// Saves the specified should remember profile for next visit.
        /// </summary>
        /// <param name="shouldRememberVisitorForNextVisit">if set to <c>true</c> [should remember visitor for next visit].</param>
        /// <param name="isLoggedIn">if set to <c>true</c> [is logged in].</param>
        /// <returns></returns>
        public virtual bool Save(IProfile entity, bool shouldRememberVisitorForNextVisit, bool? isLoggedIn = null)
        {
            if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Items["wim.profile.logout"] != null)
                return false;

            entity.RememberMe = shouldRememberVisitorForNextVisit;

            entity.ID = DataParser.Save<IProfile>(entity);

            if (System.Web.HttpContext.Current != null)
                System.Web.HttpContext.Current.Items["wim.profile"] = null;

            IVisitor visitor = Visitor.Select();
            visitor.ProfileID = entity.ID;
            if (isLoggedIn.HasValue && isLoggedIn.Value)
                visitor.IsLoggedIn = isLoggedIn.Value;

            visitor.Save();

            return true;
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public virtual IProfile SelectOne(int ID)
        {
            //CB 2016-03-22: NEVER SET A PROFILE TO CACHE! THIS HAS BEEN AN ERROR TWO TIMES ON MP --> rage.quit() 
            return DataParser.SelectOne<IProfile>(ID, false);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public virtual IProfile SelectOne(string email)
        {
            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("Profile_Email", SqlDbType.NVarChar, email));
            return DataParser.SelectOne<IProfile>(where);

        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="email">The Guid.</param>
        /// <returns></returns>
        public virtual IProfile SelectOne(Guid guid)
        {
            return DataParser.SelectOne<IProfile>(guid);
        }
        public virtual IProfile Login(string email, string password)
        {
            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            
            where.Add(new DatabaseDataValueColumn("profile_email", SqlDbType.NVarChar, email));
            where.Add(new DatabaseDataValueColumn("profile_password", SqlDbType.NVarChar, password));

            return DataParser.SelectOne<IProfile>(where);
        }

        /// <summary>
        /// Removes the profile identifying cookie. Any subsequent changes to this instances and ignored in this context call.
        /// </summary>
        /// <param name="redirectionPageID">The redirection page ID.</param>
        /// <returns></returns>
        public virtual bool Logout(int? redirectionPageID = null)
        {
            Visitor.Select().Logout(null);

            if (redirectionPageID.HasValue)
            {
                Sushi.Mediakiwi.Data.Page page = Sushi.Mediakiwi.Data.Page.SelectOne(redirectionPageID.Value);
                if (page != null && !page.IsNewInstance)
                    System.Web.HttpContext.Current.Response.Redirect(page.HRef);
            }
            return true;
        }
    }
}