using Sushi.Mediakiwi.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Extention
{
    public static class VisitorExtention
    {
        /// <summary>
        /// Select the current visitor object. If no visitor exists an instance is created including a database reference.
        /// By default, the visitor reference is not stored accross sessions. 
        /// </summary>
        /// <returns></returns>
        public static IVisitor Select(this IVisitor inVisitor)
        {
            IVisitor visitor;

            ////if there is a visitor present in the current request context, use that one
            //if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Items["wim.visitor"] != null)
            //{
            //    visitor = (IVisitor)System.Web.HttpContext.Current.Items["wim.visitor"];
            //    return visitor;
            //}

            ////if none present this must be the first attempt to retrieve a visitor in this request. check registry if we need to force a database call
            //bool alwaysRetrieveFromDatabase = CommonConfiguration.ALWAYS_RETRIEVE_VISITOR_FROM_DATABASE;

            ////check cookie for visitorID and retrieve from cache or database
            //var tmp = this.SelectVisitorByCookieVisitorID(alwaysRetrieveFromDatabase);

            //if (tmp == null || tmp.ID == 0)
            //{
            //    //no visitor found in cache or databse, create a new instance and save it
            //    visitor = Environment.GetInstance<IVisitor>();
            //    Save(visitor);
            //    if (System.Web.HttpContext.Current != null)
            //        System.Web.HttpContext.Current.Items["wim.visitor"] = visitor;
            //    return visitor;
            //}
            //else
            //    visitor = tmp;

            ////store visitor in http context
            //if (System.Web.HttpContext.Current != null)
            //    System.Web.HttpContext.Current.Items["wim.visitor"] = visitor;
            return visitor;
        }
    }
}
