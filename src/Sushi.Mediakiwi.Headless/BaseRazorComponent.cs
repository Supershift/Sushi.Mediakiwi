using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sushi.Mediakiwi.Headless.Config;
using Sushi.Mediakiwi.Headless.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Headless
{
    public class BaseRazorComponent<T> : ComponentBase where T : ISushiApplicationSettings 
    {
        [Inject]
        public NavigationManager NavManager { get; set; }

        [Parameter]
        public ContentComponent Content { get; set; } = new ContentComponent();

        [Parameter]
        public List<ContentComponent> SharedContent { get; set; } = new List<ContentComponent>();

        [Parameter]
        public ContentMetaData PageMetaData { get; set; } = new ContentMetaData();

        [Inject]
        public T Configuration { get; set; }
        
        [Inject]
        public IHttpContextAccessor httpContextAccessor { get; set; }

        [Parameter]
        public bool ClearCache { get; set; }

        [Parameter]
        public bool IsPreview { get; set; }

        [Inject]
        public IServiceProvider serviceProvider { get; set; }
        
        public ILogger logger { get; set; } 

        /// <summary>
        /// When set to TRUE in the constructor, this component will load as ServerPrerender.
        /// Else it will load as Static
        /// </summary>
        public bool DynamicComponent { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);
        }

        public void RedirectTo(string url)
        {
            NavManager.NavigateTo(url);
        }

        public void RedirectToPageNotFound()
        {
            RedirectTo("./notfound");
        }

        protected override void OnInitialized()
        {   
            // We have content, reflect !
            if (Content?.Content?.Count > 0)
            {
                var propertyInfos = this.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                foreach (var propInfo in propertyInfos)
                {
                    var contentProp = Content.Content.FirstOrDefault(x => x.Key == propInfo.Name);
                    if (contentProp.Value != null)
                    {
                        propInfo.SetValue(this, contentProp.Value);
                    }
                }
            }

            if (Content?.InternalInfo != null)
            {
                // When IsPreview is set to true, its probably set by a parent component
                if (IsPreview == false)
                {
                    IsPreview = Content.InternalInfo.IsPreview;
                }

                // When ClearCache is set to true, its probably set by a parent component
                if (ClearCache == false)
                {
                    ClearCache = Content.InternalInfo.ClearCache;
                }
            }

            if (serviceProvider != null)
            {
                var _loggerFactory = serviceProvider.GetService<ILoggerFactory>();
               
                logger = _loggerFactory.CreateLogger<BaseRazorComponent<T>>();
            }

            base.OnInitialized();
        }

        /// <summary>
        /// On component Initialized : load content
        /// </summary>
        /// <returns></returns>
        protected override Task OnInitializedAsync()
        {
            
            // We have content, reflect !
            if (Content?.Content?.Count > 0)
            {
                var propertyInfos = this.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                foreach (var propInfo in propertyInfos)
                {
                    var contentProp = Content.Content.FirstOrDefault(x => x.Key == propInfo.Name);
                    if (contentProp.Value != null)
                    {
                        propInfo.SetValue(this, contentProp.Value);
                    }
                }
            }

            if (Content?.InternalInfo != null)
            {
                // When IsPreview is set to true, its probably set by a parent component
                if (IsPreview == false)
                {
                    IsPreview = Content.InternalInfo.IsPreview;
                }

                // When ClearCache is set to true, its probably set by a parent component
                if (ClearCache == false)
                {
                    ClearCache = Content.InternalInfo.ClearCache;
                }
            }

            if (httpContextAccessor != null)
            {
                if (ClearCache == false)
                {
                    ClearCache = httpContextAccessor.HttpContext.Request.IsClearCacheCall();
                }

                if (IsPreview == false)
                {
                    IsPreview = httpContextAccessor.HttpContext.Request.IsPreviewCall();
                }
            }

            if (serviceProvider != null)
            {
                var _loggerFactory = serviceProvider.GetService<ILoggerFactory>();
                logger = _loggerFactory.CreateLogger<BaseRazorComponent<T>>();
            }

            return base.OnInitializedAsync();
        }
    }
}
