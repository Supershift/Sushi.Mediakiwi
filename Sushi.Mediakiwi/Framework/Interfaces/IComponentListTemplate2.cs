using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.AspNetCore.Http;
using Sushi.Mediakiwi.Data;

namespace Sushi.Mediakiwi.Framework
{
    public interface IComponentListTemplate
    {
        void Init(HttpContext context);
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
        event EventHandler ListSense;

        bool ApplyContent(Guid componentlistGUID, Site site);
        bool ApplyContent(Guid componentlistGUID, Site site, bool exceptionAvoided);
        void ApplyListSettings(IComponentList list);

        //Translator RenameTitle { get; set; }
        //Translator RenameInteractiveHelp { get; set; }
    }
}