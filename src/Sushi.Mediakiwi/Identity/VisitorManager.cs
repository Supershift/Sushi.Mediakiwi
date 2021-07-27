using Microsoft.AspNetCore.Http;
using Sushi.Mediakiwi.Authentication;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Utilities;
using System;
using System.Threading.Tasks;

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
            var tmp = SelectVisitorByCookie();

            if (tmp == null || tmp.ID == 0)
            {
                //no visitor found in cache or databse, create a new instance and save it
                visitor = new Visitor();
                Save(visitor);
                if (Context != null)
                {
                    Context.Items["wim.visitor"] = visitor;
                }
                return visitor;
            }
            else
            {
                visitor = tmp;
            }

            //store visitor in http context
            if (Context != null)
            {
                Context.Items["wim.visitor"] = visitor;
            }
            return visitor;
        }

        /// <summary>
        /// Select the current visitor object. If no visitor exists an instance is created including a database reference.
        /// By default, the visitor reference is not stored accross sessions. 
        /// </summary>
        /// <returns></returns>
        public async Task<IVisitor> SelectAsync()
        {
            IVisitor visitor;

            //if there is a visitor present in the current request context, use that one
            if (Context != null && Context.Items["wim.visitor"] != null)
            {
                visitor = (IVisitor)Context.Items["wim.visitor"];
                return visitor;
            }

            //check cookie for visitorID and retrieve from cache or database
            var tmp = await SelectVisitorByCookieAsync().ConfigureAwait(false);

            if (tmp == null || tmp.ID == 0)
            {
                //no visitor found in cache or databse, create a new instance and save it
                visitor = new Visitor();
                await SaveAsync(visitor).ConfigureAwait(false);
                if (Context != null)
                {
                    Context.Items["wim.visitor"] = visitor;
                }
                return visitor;
            }
            else
            {
                visitor = tmp;
            }

            //store visitor in http context
            if (Context != null)
            {
                Context.Items["wim.visitor"] = visitor;
            }
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
            using (var auth = new AuthenticationLogic(Context))
            {
                auth.TicketName = CommonConfiguration.AUTHENTICATION_COOKIE;
                auth.Password = Context.Request.Headers["User-Agent"];

                if (Context != null)
                {
                    var v = auth.GetValue("v");
                    if (!string.IsNullOrWhiteSpace(v) && Utility.IsGuid(v, out var id))
                    {
                        visitor = Visitor.Select(id);
                    }
                }

                if (visitor == null)
                    return Reset();

                long lastVisit;
                if (Utility.IsNumeric(auth.GetValue(m_Attribute_TimeStamp), out lastVisit))
                {
                    visitor.LastVisit = new DateTime(lastVisit);
                }
            }
            return visitor;
        }

        /// <summary>
        /// Determines the current visitor ID from cookie. If visitorID is present, the visitor is retrieved. 
        /// If no visitorID is present or no visitor is found for ID, a new visitor is created and stored in database.
        /// </summary>
        /// <param name="alwaysRetrieveFromDatabase"></param>
        /// <returns></returns>
        public async Task<IVisitor> SelectVisitorByCookieAsync()
        {
            IVisitor visitor = null;
            using (var auth = new AuthenticationLogic(Context))
            {
                auth.TicketName = CommonConfiguration.AUTHENTICATION_COOKIE;
                auth.Password = Context.Request.Headers["User-Agent"];

                if (Context != null)
                {
                    var v = auth.GetValue("v");
                    if (!string.IsNullOrWhiteSpace(v) && Utility.IsGuid(v, out var id))
                    {
                        visitor = await Visitor.SelectAsync(id).ConfigureAwait(false);
                    }
                }

                if (visitor == null)
                {
                    return await ResetAsync().ConfigureAwait(false);
                }

                long lastVisit;
                if (Utility.IsNumeric(auth.GetValue(m_Attribute_TimeStamp), out lastVisit))
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

        public async Task<bool> SaveAsync(IVisitor entity, bool shouldRememberVisitorForNextVisit = true)
        {
            return await Save2Async(entity, shouldRememberVisitorForNextVisit).ConfigureAwait(false);
        }

        /// <summary>
        /// Saves the specified should remember profile for next visit.
        /// </summary>
        /// <param name="shouldRememberVisitorForNextVisit">if set to <c>true</c> [should remember visitor for next visit].</param>
        /// <returns></returns>
        bool Save2(IVisitor entity, bool? shouldRememberVisitorForNextVisit = true, bool shouldSetCookie = true)
        {
            if (entity.Created == DateTime.MinValue)
            {
                entity.Created = DateTime.UtcNow;
            }

            if (entity.GUID == Guid.Empty)
            {
                entity.GUID = Guid.NewGuid();
            }

            if (shouldRememberVisitorForNextVisit.HasValue)
            {
                entity.RememberMe = shouldRememberVisitorForNextVisit.GetValueOrDefault();
            }

            bool isSaved;
            entity.Updated = Data.Common.DatabaseDateTime;

            //  Ignore empty and undefined visitor!
            if (Context != null)
            {
                string userAgent = Context.Request.Headers["User-Agent"];
                if (string.IsNullOrWhiteSpace(userAgent))
                {
                    //  This can not be, so possibly a load balancer/bot?
                    //  Do not SAVE, as this will generate lots of traffic!
                    return false;
                }

                //  If there is no data, and no profile details to be stored, why should we save the record?
                if ((entity.Data == null || entity.Data.Serialized == null) && !entity.ProfileID.HasValue)
                {
                    return false;
                }
            }

            Visitor.Save((Visitor)entity);

            if (entity.GUID == Guid.Empty)
            {
                throw new Exception("Something went wrong with saving the current visitor");
            }

            if (shouldSetCookie)
            {
                SetCookie(entity.GUID, entity.RememberMe);
            }

            return true;
        }

        /// <summary>
        /// Saves the specified should remember profile for next visit.
        /// </summary>
        /// <param name="shouldRememberVisitorForNextVisit">if set to <c>true</c> [should remember visitor for next visit].</param>
        /// <returns></returns>
        async Task<bool> Save2Async(IVisitor entity, bool? shouldRememberVisitorForNextVisit = true, bool shouldSetCookie = true)
        {
            if (entity.Created == DateTime.MinValue)
            {
                entity.Created = DateTime.UtcNow;
            }

            if (entity.GUID == Guid.Empty)
            {
                entity.GUID = Guid.NewGuid();
            }

            if (shouldRememberVisitorForNextVisit.HasValue)
            {
                entity.RememberMe = shouldRememberVisitorForNextVisit.GetValueOrDefault();
            }

            bool isSaved;
            entity.Updated = Data.Common.DatabaseDateTime;

            //  Ignore empty and undefined visitor!
            if (Context != null)
            {
                string userAgent = Context.Request.Headers["User-Agent"];
                if (string.IsNullOrWhiteSpace(userAgent))
                {
                    //  This can not be, so possibly a load balancer/bot?
                    //  Do not SAVE, as this will generate lots of traffic!
                    return false;
                }

                //  If there is no data, and no profile details to be stored, why should we save the record?
                if ((entity.Data == null || entity.Data.Serialized == null) && !entity.ProfileID.HasValue)
                {
                    return false;
                }
            }

            await Visitor.SaveAsync((Visitor)entity).ConfigureAwait(false);

            if (entity.GUID == Guid.Empty)
            {
                throw new Exception("Something went wrong with saving the current visitor");
            }

            if (shouldSetCookie)
            {
                SetCookie(entity.GUID, entity.RememberMe);
            }

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
            using (var auth = new AuthenticationLogic(Context))
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
            return true;
        }

        IVisitor Reset()
        {
            var me = new Visitor();
            Visitor.Save(me);
            return me;
        }

        async Task<IVisitor> ResetAsync()
        {
            var me = new Visitor();
            await Visitor.SaveAsync(me).ConfigureAwait(false);
            return me;
        }

        #endregion
    }
}