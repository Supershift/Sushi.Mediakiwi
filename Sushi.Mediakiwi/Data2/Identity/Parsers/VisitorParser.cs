using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Data.Identity;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.Data.Identity.Parsers
{
    public class VisitorParser : IVisitorParser
    {
        static string[] _AgentSplit;

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

        #region Methods

        /// <summary>
        /// Applies the campaign.
        /// </summary>
        /// <param name="campaignID">The campaign ID.</param>
        /// <param name="autoSave">if set to <c>true</c> [auto save].</param>
        public virtual void ApplyCampaign(IVisitor entity, int campaignID, bool autoSave)
        {
            entity.Data.Apply("campaign.id", campaignID);
            if (autoSave)
                Save(entity);
        }

        /// <summary>
        /// Clears the campaign.
        /// </summary>
        /// <param name="autoSave">if set to <c>true</c> [auto save].</param>
        public virtual void ClearCampaign(IVisitor entity, bool autoSave)
        {
            entity.Data.Apply("campaign.id", null);
            if (autoSave)
                Save(entity);
        }

        public virtual bool Logout(IVisitor entity, int? redirectionPageID)
        {
            entity.IsLoggedIn = false;
            Save(entity);

            if (redirectionPageID.HasValue)
            {
                Sushi.Mediakiwi.Data.Page page = Sushi.Mediakiwi.Data.Page.SelectOne(redirectionPageID.Value);
                if (page != null && !page.IsNewInstance)
                    System.Web.HttpContext.Current.Response.Redirect(page.HRef);
            }
            return true;
        }


        /// <summary>
        /// Select the current visitor object. If no visitor exists an instance is created including a database reference.
        /// By default, the visitor reference is not stored accross sessions. 
        /// </summary>
        /// <returns></returns>
        public virtual IVisitor Select()
        {
            IVisitor visitor;

            //if there is a visitor present in the current request context, use that one
            if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Items["wim.visitor"] != null)
            {
                visitor = (IVisitor)System.Web.HttpContext.Current.Items["wim.visitor"];
                return visitor;
            }

            //if none present this must be the first attempt to retrieve a visitor in this request. check registry if we need to force a database call
            bool alwaysRetrieveFromDatabase = CommonConfiguration.ALWAYS_RETRIEVE_VISITOR_FROM_DATABASE;

            //check cookie for visitorID and retrieve from cache or database
            var tmp = this.SelectVisitorByCookieVisitorID(alwaysRetrieveFromDatabase);
            
            if (tmp == null || tmp.ID == 0)
            {
                //no visitor found in cache or databse, create a new instance and save it
                visitor = Environment.GetInstance<IVisitor>();
                Save(visitor);
                if (System.Web.HttpContext.Current != null)
                    System.Web.HttpContext.Current.Items["wim.visitor"] = visitor;
                return visitor;
            }
            else
                visitor = tmp;

            //store visitor in http context
            if (System.Web.HttpContext.Current != null)
                System.Web.HttpContext.Current.Items["wim.visitor"] = visitor;
            return visitor;
        }

        /// <summary>
        /// Selects the specified visitor reference.
        /// </summary>
        /// <param name="visitorReference">The visitor reference.</param>
        /// <returns></returns>
        public virtual IVisitor Select(Guid visitorReference)
        {
            return DataParser.SelectOne<IVisitor>(visitorReference);
        }
        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public virtual IVisitor SelectOne(int ID)
        {
            return DataParser.SelectOne<IVisitor>(ID, false, null);
        }

        ///// <summary>
        ///// Selects the one.
        ///// </summary>
        ///// <param name="ID">The ID.</param>
        ///// <param name="info">The info.</param>
        ///// <returns></returns>
        //public static Visitor SelectOne(int ID, SqlInfo info)
        //{
        //    Visitor implement = new Visitor();
        //    if (info != null)
        //    {
        //        implement.SqlRowCount = info.SqlRowCount;
        //        implement.SqlOrder = info.SqlOrder;
        //        implement.SqlJoin = info.SqlJoin;
        //        implement.SqlGroup = info.SqlGroup;
        //    }

        //    return (Visitor)implement._SelectOne(ID);
        //}

        ///// <summary>
        ///// Selects the one.
        ///// </summary>
        ///// <param name="where">The where.</param>
        ///// <returns></returns>
        //public static Visitor SelectOne(List<DatabaseDataValueColumn> where)
        //{
        //    return SelectOne(where, null);
        //}

        ///// <summary>
        ///// Selects the one.
        ///// </summary>
        ///// <param name="where">The where.</param>
        ///// <param name="info">The info.</param>
        ///// <returns></returns>
        //public static Visitor SelectOne(List<DatabaseDataValueColumn> where, SqlInfo info)
        //{
        //    Visitor implement = new Visitor();
        //    if (info != null)
        //    {
        //        implement.SqlRowCount = info.SqlRowCount;
        //        implement.SqlOrder = info.SqlOrder;
        //        implement.SqlJoin = info.SqlJoin;
        //        implement.SqlGroup = info.SqlGroup;
        //    }

        //    return (Visitor)implement._SelectOne(where);
        //}

        /// <summary>
        /// Selects all visitors with the profile id
        /// </summary>
        /// <param name="profileId">The profile id.</param>
        /// <param name="visitorID">The visitor ID.</param>
        /// <returns></returns>
        public virtual IVisitor[] SelectAllByProfile(int profileId, int visitorID)
        {
            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("Visitor_Profile_Key", SqlDbType.Int, profileId));

            int minutes = Wim.Utility.ConvertToInt(Sushi.Mediakiwi.Data.Environment.Current["EXPIRATION_COOKIE_PROFILE"], 0);

            var result = DataParser.SelectAll<IVisitor>(where);
            List<IVisitor> list = new List<IVisitor>();
            foreach (var visitor in result)
            {
                // [MR:06-06-2018] Was :
                // if (visitor.ID != visitorID && visitor.LastRequestDone.Subtract(DateTime.Now).Minutes < minutes)
                // this results in 15:21:00 - 14:21:00 being 0 Minutes  :)
                if (visitor.ID != visitorID && visitor.LastRequestDone.Subtract(DateTime.Now).TotalMinutes < minutes)
                    list.Add(visitor);
            }
            return list.ToArray();
        }

        ///// <summary>
        ///// Selects all.
        ///// </summary>
        ///// <param name="where">The where.</param>
        ///// <returns></returns>
        //public static IVisitor[] SelectAll(List<DatabaseDataValueColumn> where)
        //{
        //    return SelectAll(where, null);
        //}

        ///// <summary>
        ///// Selects all.
        ///// </summary>
        ///// <param name="where">The where.</param>
        ///// <param name="info">The info.</param>
        ///// <returns></returns>
        //public static Visitor[] SelectAll(List<DatabaseDataValueColumn> where, SqlInfo info)
        //{
        //    Visitor implement = new Visitor();

        //    if (info != null)
        //    {
        //        implement.SqlRowCount = info.SqlRowCount;
        //        implement.SqlOrder = info.SqlOrder;
        //        implement.SqlJoin = info.SqlJoin;
        //        implement.SqlGroup = info.SqlGroup;
        //    }

        //    List<Visitor> list = new List<Visitor>();
        //    foreach (object o in implement._SelectAll(where)) list.Add((Visitor)o);
        //    return list.ToArray();
        //}

        public bool SaveData(IVisitor entity)
        {
            return Save2(entity, null, true);
        }

        public bool Save(IVisitor entity, bool shouldRememberVisitorForNextVisit = true)
        {
            return Save2(entity, shouldRememberVisitorForNextVisit);
        }

        /// <summary>
        /// Saves the specified should remember profile for next visit.
        /// </summary>
        /// <param name="shouldRememberVisitorForNextVisit">if set to <c>true</c> [should remember visitor for next visit].</param>
        /// <returns></returns>
        bool Save2(IVisitor entity, bool? shouldRememberVisitorForNextVisit = true, bool shouldSetCookie = true)
        {
            if (entity.Created == DateTime.MinValue)
                entity.Created = DateTime.UtcNow;

            if (entity.GUID == Guid.Empty)
                entity.GUID = Guid.NewGuid();

            if (shouldRememberVisitorForNextVisit.HasValue)
                entity.RememberMe = shouldRememberVisitorForNextVisit.GetValueOrDefault();

            bool isSaved;
            entity.Updated = Sushi.Mediakiwi.Data.Common.DatabaseDateTime;

            //  Ignore empty and undefined visitor!
            if (System.Web.HttpContext.Current != null)
            {
                string userAgent = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"];
                if (string.IsNullOrEmpty(userAgent))
                {
                    //  This can not be, so possibly a load balancer/bot?
                    //  Do not SAVE, as this will generate lots of traffic!
                    return false;
                }
                //  If there is no data, and no profile details to be stored, why should we save the record?
                if (entity.Data == null || entity.Data.Serialized == null)
                {
                    if (!entity.ProfileID.HasValue)
                        return false;
                }
                //  Wim exclude list
                if (!string.IsNullOrEmpty(Wim.CommonConfiguration.WIM_COOKIE_EXCLUDELIST))
                {
                    if (_AgentSplit == null)
                        _AgentSplit = Wim.CommonConfiguration.WIM_COOKIE_EXCLUDELIST.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                    if (_AgentSplit != null && _AgentSplit.Length > 0)
                    {
                        var look = (from item in _AgentSplit where userAgent.Contains(item) select item).Count();
                        if (look > 0)
                            return false;
                    }
                }
            }

            isSaved = DataParser.Save<IVisitor>(entity) > 0;

            if (isSaved)
            {
                if (entity.GUID == Guid.Empty)
                    throw new Exception("Something went wrong with saving the current visitor");

                if (shouldSetCookie)
                    SetCookie(entity.ID, entity.GUID, entity.ProfileID, entity.RememberMe);
            }
            return isSaved;
        }
        ///// <summary>
        ///// Save a database entity. This method will indentify the need for an INSERT or an UPDATE.
        ///// By default the profile reference is stored in a Cookie
        ///// </summary>
        ///// <returns></returns>
        //public virtual bool Save(IVisitor visitor)
        //{
        //    if (DataParser.Save<IVisitor>(visitor) > 0)
        //        return true;
        //    return false;
        //}

        #region Cookie storage

        string m_Attribute_IdentityVersion = "iv";
        string m_Attribute_Visitor = "Vistor";
        string m_Attribute_VisitorID = "VisitorID";
        string m_Attribute_UserGUID = "UserGUID";
        string m_Attribute_TimeStamp = "TimeStamp";
        string m_Attribute_IpAddress = "IP";

        string m_TicketName;
        /// <summary>
        /// Gets the name of the ticket.
        /// </summary>
        /// <value>The name of the ticket.</value>
        public string TicketName
        {
            get
            {

                if (string.IsNullOrEmpty(m_TicketName))
                {
                    m_TicketName =
                        string.IsNullOrEmpty(Wim.CommonConfiguration.WIM_COOKIE) ?
                            string.Concat("v_", Sushi.Mediakiwi.Data.Environment.Current.Title.Replace(" ", "_"))
                            : Wim.CommonConfiguration.WIM_COOKIE;
                }
                return m_TicketName;
            }
        }

        /// <summary>
        /// Sets the cookie.
        /// </summary>
        public virtual void SetCookie(IVisitor entity)
        {
            SetCookie(entity.ID, entity.GUID, entity.ProfileID, entity.RememberMe);
        }

        /// <summary>
        /// Sets the cookie.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <param name="profileId">The profile id.</param>
        /// <param name="shouldRememberProfileForNextVisit">if set to <c>true</c> [should remember profile for next visit].</param>
        public virtual void SetCookie(int id, Guid guid, int? profileId, bool shouldRememberProfileForNextVisit)
        {
            if (guid == Guid.Empty) return;
            if (System.Web.HttpContext.Current == null) return;
            using (Wim.Utilities.Authentication auth = new Wim.Utilities.Authentication())
            {
                auth.EncryptionPassword = guid.ToString();
                auth.CustomTicketName = m_TicketName;

                auth.AddCustomTicketAttribute(m_Attribute_VisitorID, id.ToString());
                
                // [MR:03-11-2015] this created a situation where the time here was an hour behind
                // the actual time, thus invalidating The visitor Logged In state
                //auth.AddCustomTicketAttribute(m_Attribute_TimeStamp, DateTime.UtcNow.Ticks.ToString());
                auth.AddCustomTicketAttribute(m_Attribute_TimeStamp, Common.DatabaseDateTime.Ticks.ToString());

                //auth.AddPublicCustomTicketAttribute("v", id.ToString());
                auth.AddPublicCustomTicketAttribute("v", auth.Encrypt(id.ToString(), System.Web.HttpContext.Current.Request.UserAgent));
                auth.AddPublicCustomTicketAttribute(m_Attribute_IdentityVersion, "3");

                int minutes = Wim.Utility.ConvertToInt(Sushi.Mediakiwi.Data.Environment.Current["EXPIRATION_COOKIE_VISITOR"], 0);
                // [MR:03-11-2015] this created a situation where the time here was an hour behind
                // the actual time, thus invalidating The visitor Logged In state
                if (minutes > 0 && shouldRememberProfileForNextVisit)
                    auth.CustomTicketLifeTime = Common.DatabaseDateTime.AddMinutes(minutes);
                auth.CreateCustomTicket();
            }
        }

        /// <summary>
        /// Remove the profile cookie
        /// </summary>
        /// <returns></returns>
        public virtual bool Clear()
        {
            RemoveCookie();
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        internal void RemoveCookie()
        {
            using (Wim.Utilities.Authentication auth = new Wim.Utilities.Authentication())
                auth.RemoveCustomTicket(m_TicketName);
        }

        /// <summary>
        /// Sets the info from cookie.
        /// </summary>
        public IVisitor SetInfoFromCookie()
        {
            return SelectVisitorByCookieVisitorID(false);
        }

        IVisitor Reset()
        {
            var me = Sushi.Mediakiwi.Data.Environment.GetInstance<IVisitor>();
            me.Save();
            return me;
        }

        /// <summary>
        /// Determines the current visitor ID from cookie. If visitorID is present, the visitor is retrieved. 
        /// If no visitorID is present or no visitor is found for ID, a new visitor is created and stored in database.
        /// </summary>
        /// <param name="alwaysRetrieveFromDatabase"></param>
        /// <returns></returns>
        public IVisitor SelectVisitorByCookieVisitorID(bool alwaysRetrieveFromDatabase)
        {
            IVisitor visitor = null;
            using (Wim.Utilities.Authentication auth = new Wim.Utilities.Authentication())
            {
                auth.CustomTicketName = this.TicketName;

                Wim.Utilities.Authentication.TicketConversionInfo detection2;

                int id = 0;
                //var v = auth.GetPublicCustomTicketValue("v");
                if (System.Web.HttpContext.Current != null)
                {
                    var v = auth.Decrypt(auth.GetPublicCustomTicketValue("v"), System.Web.HttpContext.Current.Request.UserAgent);
                    if (!string.IsNullOrWhiteSpace(v) && Wim.Utility.IsNumeric(v, out id))
                    {
                        visitor = DataParser.SelectOne<IVisitor>(id, true, null, alwaysRetrieveFromDatabase);
                        auth.EncryptionPassword = visitor.GUID.ToString();
                    }
                }

                if (visitor == null)
                    return Reset();

                int check;

                visitor.IsNewVisitor = true;
                if (Wim.Utility.IsNumeric(auth.GetCustomTicketValue(m_Attribute_VisitorID, out detection2), out check))
                {
                    if (check == id)
                    {
                        visitor.ID = check;
                        visitor.IsNewVisitor = false;
                    }
                    else
                        return Reset();
                }
                else
                    return Reset();

                long lastVisit;
                if (Wim.Utility.IsNumeric(auth.GetCustomTicketValue(m_Attribute_TimeStamp), out lastVisit))
                {
                    visitor.LastVisit = new DateTime(lastVisit);
                }
            }
            return visitor;
        }
        #endregion

        #endregion
    }
}