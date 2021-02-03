using System.Web;
using System.Web.UI;
using Sushi.Mediakiwi.Data;

namespace Sushi.Mediakiwi.Framework
{
    public interface IComponentListTemplateOLD
    {
        HttpContext Context { get; }
        bool HasListAsync { get; }
        bool HasListDataReport { get; }
        bool HasListLoad { get; }
        bool IsEditMode { get; }
        bool IsPostBack { get; }
        bool IsTextMode { get; }
        System.Web.UI.Page Page { get; set; }
        HttpRequest Request { get; }
        HttpResponse Response { get; }
        HttpServerUtility Server { get; }
        WimComponentListRoot wim { get; set; }

        event ComponentActionEventHandler ListAction;
        event ComponentAsyncEventHandler ListAsync;
        event ComponentDataItemCreatedEventHandler ListDataItemCreated;
        event ComponentDataReportEventHandler ListDataReport;
        event ComponentListEventHandler ListDelete;
        event ComponentListEventHandler ListLoad;
        event ComponentListEventHandler ListPreRender;
        event ComponentListEventHandler ListSave;
        event ComponentSearchEventHandler ListSearch;
        event ComponentActionEventHandler ListSearchedAction;

        void ApplyListSettings(IComponentList list);
    }
}