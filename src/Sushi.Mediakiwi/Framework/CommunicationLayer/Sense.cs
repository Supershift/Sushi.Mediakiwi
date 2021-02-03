using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Sushi.Mediakiwi.Framework.CommunicationLayer
{
    class Sense
    {
        public Sense()
        {
        }

        System.Web.UI.Page Page;
        System.Web.HttpResponse Response;
        System.Web.HttpRequest Request;


        internal void Initiate(System.Web.UI.Page callingPage, Guid componentList)
        {
            Sushi.Mediakiwi.Data.IComponentList list = Sushi.Mediakiwi.Data.ComponentList.SelectOne(componentList);

            Initiate(callingPage, list);
        }

        //internal void Initiate(System.Web.UI.Page callingPage, Sushi.Mediakiwi.Data.IComponentList list)
        //{
        //    ThreadCommunication communication = new ThreadCommunication();
        //    communication.CallingPage = callingPage;
        //    communication.List = list;
        //    System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(this.Initiate), communication);
        //}

        //public class ThreadCommunication
        //{
        //    public System.Web.UI.Page CallingPage { get; set; }
        //    public Sushi.Mediakiwi.Data.IComponentList List { get; set; }
        //}

        internal void Initiate(System.Web.UI.Page callingPage, Sushi.Mediakiwi.Data.IComponentList list)
        {
            Page = callingPage;
            Response = callingPage.Response;
            Request = callingPage.Request;

            //System.IO.FileInfo nfo = new System.IO.FileInfo(Page.Server.MapPath(string.Concat(Request.ApplicationPath, "/bin/", list.AssemblyName)));
            //Assembly assem = Assembly.LoadFrom(nfo.FullName);
            //System.Type loadedType = assem.GetType(list.ClassName);
            //object loadedInstance = System.Activator.CreateInstance(loadedType);

            //  Call the SENSE event
            IComponentListTemplate loadedClassInstance = list.GetInstance();
            loadedClassInstance.wim.DoListSense();
            //Sushi.Mediakiwi.Framework.ComponentListClassTemplate loadedClassInstance = (Sushi.Mediakiwi.Framework.ComponentListClassTemplate)loadedInstance;
            //loadedClassInstance.DoListSense();

            Sushi.Mediakiwi.Data.Notification.InsertOne("CommunicationLayer.Sense.Log", Sushi.Mediakiwi.Data.NotificationType.Information, string.Format("Executed List: '{0}'", list.Name));
        }
    }
}
