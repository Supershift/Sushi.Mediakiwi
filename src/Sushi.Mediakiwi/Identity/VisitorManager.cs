using System;
using Sushi.Mediakiwi.Data;
using Microsoft.AspNetCore.Http;
using Sushi.Mediakiwi.Utilities;

namespace Sushi.Mediakiwi
{
    public class VisitorManager
    {
        HttpContext Context { get; set; }
        public VisitorManager(HttpContext context)
        {
            Context = context;
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
            if (Context != null && Context.Items["wim.visitor"] != null)
            {
                visitor = (IVisitor)Context.Items["wim.visitor"];
                return visitor;
            }

            //check cookie for visitorID and retrieve from cache or database
            var tmp = this.SelectVisitorByCookie();
            
            if (tmp == null || tmp.ID == 0)
            {
                //no visitor found in cache or databse, create a new instance and save it
                visitor = new Visitor();
                Save(visitor);
                if (Context != null)
                    Context.Items["wim.visitor"] = visitor;
                return visitor;
            }
            else
                visitor = tmp;

            //store visitor in http context
            if (Context != null)
                Context.Items["wim.visitor"] = visitor;
            return visitor;
        }

        /// <summary>
        /// Determines the current visitor ID from cookie. If visitorID is present, the visitor is retrieved. 
        /// If no visitorID is present or no visitor is found for ID, a new visitor is created and stored in database.
        /// </summary>
        /// <param name="alwaysRetrieveFromDatabase"></param>
        /// <returns></returns>
        public IVisitor SelectVisitorByCookie()
        {
            IVisitor visitor = null;
            using (var auth = new Authentication(Context))
            {
                auth.TicketName = CommonConfiguration.AUTHENTICATION_COOKIE;
                auth.Password = Context.Request.Headers["User-Agent"];

                if (Context != null)
                {
                    var v = auth.GetValue("v");
                    if (!string.IsNullOrWhiteSpace(v) && Data.Utility.IsGuid(v, out var id))
                    {
                        visitor = Visitor.Select(id);
                    }
                }

                if (visitor == null)
                    return Reset();

                long lastVisit;
                if (Data.Utility.IsNumeric(auth.GetValue(m_Attribute_TimeStamp), out lastVisit))
                {
                    visitor.LastVisit = new DateTime(lastVisit);
                }
            }
            return visitor;
        }

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
            if (Context != null)
            {
                string userAgent = Context.Request.Headers["User-Agent"];
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
            }

            Visitor.Save((Visitor)entity);
           
            if (entity.GUID == Guid.Empty)
                throw new Exception("Something went wrong with saving the current visitor");

            if (shouldSetCookie)
                SetCookie(entity.GUID, entity.RememberMe);
            
            return true;
        }

        #region Cookie storage
        string m_Attribute_Id = "v";
        string m_Attribute_IdentityVersion = "iv";
        string m_Attribute_TimeStamp = "TimeStamp";


        /// <summary>
        /// Sets the cookie.
        /// </summary>
        public virtual void SetCookie(IVisitor entity)
        {
            SetCookie(entity.GUID, entity.RememberMe);
        }

        /// <summary>
        /// Sets the cookie.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <param name="profileId">The profile id.</param>
        /// <param name="shouldRememberProfileForNextVisit">if set to <c>true</c> [should remember profile for next visit].</param>
        public virtual void SetCookie(Guid guid, bool shouldRememberProfileForNextVisit)
        {
            if (guid == Guid.Empty) return;
            if (Context == null) return;
            using (var auth = new Authentication(Context))
            {
                auth.Password = Context.Request.Headers["User-Agent"];
                auth.TicketName = CommonConfiguration.AUTHENTICATION_COOKIE;

                auth.AddValue(m_Attribute_TimeStamp, DateTime.UtcNow.Ticks.ToString());
                auth.AddValue(m_Attribute_Id, guid.ToString());
                auth.AddValue(m_Attribute_IdentityVersion, "3");
                auth.LifeTime = DateTime.UtcNow.AddMinutes(CommonConfiguration.AUTHENTICATION_TIMEOUT);

                auth.CreateTicket();
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
            using (var auth = new Authentication())
                auth.RemoveTicket(CommonConfiguration.AUTHENTICATION_COOKIE);
        }

        IVisitor Reset()
        {
            var me = new Visitor();
            Visitor.Save(me);
            return me;
        }
        #endregion
    }
}