using Microsoft.AspNetCore.Http;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.UI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Framework
{
    public class FormList 
    {
        WimComponentListRoot _wim;
        public FormList(WimComponentListRoot wim)
        {
            _wim = wim;
            List = new List<IFormMap>();
        }

        public void Add(IFormMap map)
        {
            map.Init(_wim);
            List.Add(map);
        }

        public int Count
        {
            get {
                return this.List.Count;
            }
        }

        public List<IFormMap> List { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ComponentListTemplate : FormMapList, IComponentListTemplate
    {
        public string FormState { get; set; }

        public FormList FormMaps { get; set; }

        public ComponentListTemplate()
        {
            wim = new WimComponentListRoot(this);
            FormMaps = new FormList(wim);
            FormMaps.Add(new StateForm(this));
        }

        /// <summary>
        /// 
        /// </summary>
        public void Init(HttpContext context)
        {
            wim.Init(context);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is post back.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is post back; otherwise, <c>false</c>.
        /// </value>
        public bool IsPostBack
        {
            get
            {
                return (Request.Method == "POST");
            }
        }

        /// <summary>
        /// Gets the request.
        /// </summary>
        /// <value>The request.</value>
        protected HttpRequest Request
        {
            get
            {
                return Context.Request;
            }
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        protected HttpContext Context
        {
            get
            {
                return wim.Console.Context;
            }
        }

        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <value>The response.</value>
        protected HttpResponse Response
        {
            get
            {
                return Context.Response;
            }
        }

        /// <summary>
        /// Applies the content.
        /// </summary>
        /// <param name="componentlistGUID">The componentlist GUID.</param>
        /// <param name="site">The site.</param>
        /// <returns></returns>
        public bool ApplyContent(Guid componentlistGUID, Site site)
        {
            return ApplyContent(componentlistGUID, site, true);
        }

        /// <summary>
        /// Applies the content.
        /// </summary>
        /// <param name="componentlistGUID">The componentlist GUID.</param>
        /// <param name="site">The site.</param>
        /// <param name="exceptionAvoided">if set to <c>true</c> [exception avoided].</param>
        /// <returns></returns>
        public bool ApplyContent(Guid componentlistGUID, Site site, bool exceptionAvoided)
        {
            wim.CurrentSite = site;

            if (site == null)
            {
                if (!exceptionAvoided)
                    throw new Exception(string.Format("ApplyContent of Component list '{0}' resulted in an error: The Site doesn't exist", componentlistGUID));
                return false;
            }

            IComponentList list = ComponentList.SelectOne(componentlistGUID);
            ComponentListVersion version = ComponentListVersion.SelectOne(list.ID, site.ID);
            if (version == null)
            {
                if (!exceptionAvoided)
                    throw new Exception(string.Format("ApplyContent of Component list '{0}' resulted in an error: ComponentListVersion doesn't exist for site: {1}!", componentlistGUID, site.ID));
                return false;
            }

            Templates.PropertySet pset = new Templates.PropertySet();
            pset.SetValue(site, this, version.Content, null);
            return true;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is edit mode.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is edit mode; otherwise, <c>false</c>.
        /// </value>
        public bool IsEditMode
        {
            get { return wim.IsEditMode; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is text mode.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is text mode; otherwise, <c>false</c>.
        /// </value>
        public bool IsTextMode
        {
            get { return !wim.IsEditMode; }
        }


        WimComponentListRoot m_wim;
        /// <summary>
        /// </summary>
        /// <value></value>
        public WimComponentListRoot wim
        {
            get { return m_wim; }
            set { m_wim = value; }
        }

        //public event ComponentSearchEventHandler ListSearch;
        //[Obsolete("MEDIAKIWI: Please use ListAction, this will be removed later on")]
        //public event ComponentActionEventHandler ListSearchedAction;
        //public event ComponentListEventHandler ListLoad;
        public event Func<ComponentListEventArgs, Task> ListPreRender;
        public event Func<ComponentListEventArgs, Task> ListSave;
        public event Func<ComponentListEventArgs, Task> ListDelete;
        public event Func<ComponentActionEventArgs, Task> ListAction;
        public event ComponentAsyncEventHandler ListAsync;
        public event ComponentDataReportEventHandler ListDataReport;
        public event EventHandler ListSense;
        public event Func<Task> ListInit;
        public event Func<ComponentListSearchEventArgs, Task> ListSearch;
        public event Func<ComponentListEventArgs, Task> ListLoad;
        public event Func<Task> ListConfigure;

        internal async Task OnListConfigure()
        {
            Func<Task> handler = ListConfigure;

            if (handler == null)
            {
                return;
            }

            m_IsSearched = true;
            Delegate[] invocationList = handler.GetInvocationList();
            Task[] handlerTasks = new Task[invocationList.Length];

            for (int i = 0; i < invocationList.Length; i++)
            {
                handlerTasks[i] = ((Func<Task>)invocationList[i])();
            }

            await Task.WhenAll(handlerTasks).ConfigureAwait(false);
        }

        internal async Task OnListInit()
        {
            Func<Task> handler = ListInit;

            if (handler == null)
            {
                return;
            }

            m_IsSearched = true;
            Delegate[] invocationList = handler.GetInvocationList();
            Task[] handlerTasks = new Task[invocationList.Length];

            for (int i = 0; i < invocationList.Length; i++)
            {
                handlerTasks[i] = ((Func<Task>)invocationList[i])();
            }

            await Task.WhenAll(handlerTasks).ConfigureAwait(false);
        }

        internal void OnListAsync(ComponentAsyncEventArgs e)
        {
            if (ListAsync != null)
                ListAsync(this, e);
        }
        internal bool HasListAsync
        {
            get { return (ListAsync == null) ? false : true; }
        }


        /// <summary>
        /// Raises the <see cref="E:ListLoad"/> event.
        /// </summary>
        /// <param name="e">The <see cref="ComponentListEventArgs"/> instance containing the event data.</param>
        internal void OnListDataReport(ComponentDataReportEventArgs e)
        {
            if (ListDataReport != null)
                ListDataReport(this, e);
        }
        internal bool HasListDataReport
        {
            get { return (ListDataReport == null) ? false : true; }
        }

        internal async Task OnListLoad(ComponentListEventArgs c)
        {
            Func<ComponentListEventArgs, Task> handler = ListLoad;

            if (handler == null)
            {
                return;
            }

            m_IsSearched = true;
            Delegate[] invocationList = handler.GetInvocationList();
            Task[] handlerTasks = new Task[invocationList.Length];

            for (int i = 0; i < invocationList.Length; i++)
            {
                handlerTasks[i] = ((Func<ComponentListEventArgs, Task>)invocationList[i])(c);
            }

            await Task.WhenAll(handlerTasks);
        }

        internal bool HasListLoad
        {
            get { return (ListLoad == null) ? false : true; }
        }



        internal async Task OnListAction(ComponentActionEventArgs e)
        {
            Func<ComponentActionEventArgs, Task> handler = ListAction;

            if (handler == null)
            {
                return;
            }

            m_IsSearched = true;
            Delegate[] invocationList = handler.GetInvocationList();
            Task[] handlerTasks = new Task[invocationList.Length];

            for (int i = 0; i < invocationList.Length; i++)
            {
                handlerTasks[i] = ((Func<ComponentActionEventArgs, Task>)invocationList[i])(e);
            }

            await Task.WhenAll(handlerTasks);
        }
        internal bool HasListAction
        {
            get { return (ListAction == null) ? false : true; }
        }

        internal async Task OnListPreRender(ComponentListEventArgs e)
        {
            Func<ComponentListEventArgs, Task> handler = ListPreRender;

            if (handler == null)
            {
                return;
            }

            m_IsSearched = true;
            Delegate[] invocationList = handler.GetInvocationList();
            Task[] handlerTasks = new Task[invocationList.Length];

            for (int i = 0; i < invocationList.Length; i++)
            {
                handlerTasks[i] = ((Func<ComponentListEventArgs, Task>)invocationList[i])(e);
            }

            await Task.WhenAll(handlerTasks);
        }
        internal bool HasListPreRender
        {
            get { return (ListPreRender == null) ? false : true; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        internal async Task OnListSave(ComponentListEventArgs e)
        {
            Func<ComponentListEventArgs, Task> handler = ListSave;

            if (handler == null)
            {
                return;
            }

            m_IsSearched = true;
            Delegate[] invocationList = handler.GetInvocationList();
            Task[] handlerTasks = new Task[invocationList.Length];

            for (int i = 0; i < invocationList.Length; i++)
            {
                handlerTasks[i] = ((Func<ComponentListEventArgs, Task>)invocationList[i])(e);
            }

            await Task.WhenAll(handlerTasks);
        }
        internal bool HasListSave
        {
            get { return (ListSave == null) ? false : true; }
        }

        internal async Task OnListDelete(ComponentListEventArgs e)
        {
            Func<ComponentListEventArgs, Task> handler = ListDelete;

            if (handler == null)
            {
                return;
            }

            m_IsSearched = true;
            Delegate[] invocationList = handler.GetInvocationList();
            Task[] handlerTasks = new Task[invocationList.Length];

            for (int i = 0; i < invocationList.Length; i++)
            {
                handlerTasks[i] = ((Func<ComponentListEventArgs, Task>)invocationList[i])(e);
            }

            await Task.WhenAll(handlerTasks);
        }
        internal bool HasListDelete
        {
            get { return (ListDelete == null) ? false : true; }
        }



        bool m_IsSearched;

        internal async Task OnListSearch(ComponentListSearchEventArgs c)
        {
            Func<ComponentListSearchEventArgs, Task> handler = ListSearch;

            if (handler == null)
            {
                return;
            }

            m_IsSearched = true;
            Delegate[] invocationList = handler.GetInvocationList();
            Task[] handlerTasks = new Task[invocationList.Length];

            for (int i = 0; i < invocationList.Length; i++)
            {
                handlerTasks[i] = ((Func<ComponentListSearchEventArgs, Task>)invocationList[i])(c);
            }

            await Task.WhenAll(handlerTasks);
        }


        internal bool HasListSearch
        {
            get { return (ListSearch == null) ? false : true; }
        }
        internal bool ByPassAjaxRequest { get; set; }
        internal bool IsFormatRequest_AJAX { 
            get { 
                if (ByPassAjaxRequest) 
                    return true; 
                return wim.Console.Form(Constants.AJAX_PARAM) == "1"; 
            } 
        }
        public event ComponentDataItemCreatedEventHandler ListDataItemCreated;

        internal void OnListDataItemCreated(ListDataItemCreatedEventArgs c)
        {
            if (ListDataItemCreated != null) ListDataItemCreated(this, c);
        }
        internal bool HasListDataItemCreated
        {
            get { return (ListDataItemCreated == null) ? false : true; }
        }


        internal void OnListSense()
        {
            if (ListSense != null) ListSense(this, null);
        }

        internal bool HasListSense
        {
            get { return (ListSense == null) ? false : true; }
        }

        public Translator RenameTitle
        {
            get; set;
        }

        public Translator RenameInteractiveHelp
        {
            get; set;
        }


        /// <summary>
        /// Applies the list settings.
        /// </summary>
        internal void ApplyListSettings()
        {
            ApplyListSettings(wim.CurrentList);
        }
        /// <summary>
        /// Applies the list settings.
        /// </summary>
        /// <param name="list">The list.</param>
        public void ApplyListSettings(IComponentList list)
        {
            if (list == null || list.Settings == null || list.Settings.Items == null || list.Settings.Items.Length == 0)
                return;

            foreach (var item in list.Settings.Items)
                item.Apply(this);
        }

  

    }
}
