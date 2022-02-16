using Microsoft.AspNetCore.Http;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework.EventArguments;
using System;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Framework
{
    public interface IComponentListTemplate
    {
        object SenderInstance { get; set; }

        void Init(HttpContext context);

        void Init(Beta.GeneratedCms.Console console);

        string FormState { get; set; }
        FormList FormMaps { get; set; }
        //HttpContext Context { get; }
        bool IsEditMode { get; }
        bool IsPostBack { get; }
        bool IsTextMode { get; }
        //Sushi.Mediakiwi.UI.Page Page { get; set; }
        //HttpRequest Request { get; }
        //HttpResponse Response { get; }
        //HttpServerUtility Server { get; }
        WimComponentListRoot wim { get; set; }

        event Func<ComponentActionEventArgs, Task> ListAction;
        event ComponentAsyncEventHandler ListAsync;
        event ComponentDataItemCreatedEventHandler ListDataItemCreated;
        event ComponentDataReportEventHandler ListDataReport;
        event Func<ComponentListEventArgs, Task> ListDelete;
        event Func<ComponentListEventArgs, Task> ListPreRender;
        event Func<ComponentListEventArgs, Task> ListSave;
        event Func<ComponentListSearchEventArgs, Task> ListSearch;
        event Func<ComponentListEventArgs, Task> ListLoad;
        event Func<ComponentListDataReceived, Task> ListDataReceived;
        
        bool ApplyContent(Guid componentlistGUID, Site site);
        bool ApplyContent(Guid componentlistGUID, Site site, bool exceptionAvoided);
        void ApplyListSettings(IComponentList list);
    }
}