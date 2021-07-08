using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using Sushi.Mediakiwi.Headless.Config;
using Sushi.Mediakiwi.Headless.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Headless
{
    /// <summary>
    /// A <see cref="TagHelper"/> that renders a Razor component.
    /// </summary>
    [HtmlTargetElement(TagHelperName, Attributes = SlotTitleParam, TagStructure = TagStructure.WithoutEndTag)]
    public sealed class SlotTagHelper : TagHelper
    {
        private ISushiApplicationSettings settings { get; set; }

        public SlotTagHelper(IServiceProvider serviceProvider)
        {
            settings = serviceProvider.GetService<ISushiApplicationSettings>();
        }

        private const string TagHelperName = "slot";
        private const string SlotParameterName = "params";
        private const string SlotParameterPrefix = "param-";
        private const string SlotTitleParam = "title";
        private IDictionary<string, object> _parameters;

        /// <summary>
        /// Gets or sets the <see cref="Rendering.ViewContext"/> for the current request.
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        /// <summary>
        /// Gets or sets values for component parameters.
        /// </summary>
        [HtmlAttributeName(SlotParameterName, DictionaryAttributePrefix = SlotParameterPrefix)]
        public IDictionary<string, object> Parameters
        {
            get
            {
                if (_parameters == null)
                    _parameters = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

                return _parameters;
            }
            set => _parameters = value;
        }

        /// <summary>
        /// Gets or sets the component type. This value is required.
        /// </summary>
        [HtmlAttributeName(SlotTitleParam)]
        public string SlotTitle { get; set; }


        public Type GetCompType(ContentComponent comp, string assemblyName = null)
        {
            Type compType = null;
            if (string.IsNullOrWhiteSpace(comp.ComponentName) == true)
                return compType;

            Assembly assembly = null;
            if (assemblyName == null)
                assembly = Assembly.GetEntryAssembly();
            else
                assembly = Assembly.Load(assemblyName);

            List<Type> possibleTypes = assembly.ExportedTypes
                .Where(x => x.IsSubclassOf(typeof(Microsoft.AspNetCore.Components.ComponentBase)))
                .Where(x => x.FullName.ToLowerInvariant().Contains(comp.ComponentName.ToLowerInvariant())).ToList();

            // Do we have to exclude certain component namespaces ?
            if (possibleTypes?.Count > 0 && settings?.MediaKiwi?.ExcludeComponentNameSpaces?.Count > 0)
            {
                // Loop throught excluded namespaces
                foreach (var excludedNameSpace in settings.MediaKiwi.ExcludeComponentNameSpaces)
                {
                    possibleTypes.RemoveAll(x => x.FullName.StartsWith(excludedNameSpace, StringComparison.InvariantCultureIgnoreCase));
                }
            }

            // We should now have only one allowed type
            compType = possibleTypes.FirstOrDefault();

            if (compType == null && settings?.MediaKiwi?.ComponentAssemblies?.Count > 0)
            {
                if (string.IsNullOrWhiteSpace(assemblyName) == false)
                {
                    // If this is the last assembly name in the collection, exit here, 
                    // else we keep on repeating
                    if (settings.MediaKiwi.ComponentAssemblies.IndexOf(assemblyName) == settings.MediaKiwi.ComponentAssemblies.Count - 1)
                        return null;
                }

                foreach (var componentAssembly in settings.MediaKiwi.ComponentAssemblies)
                {
                    compType = GetCompType(comp, componentAssembly);
                    if (compType != null)
                        break;
                }
            }
            return compType;
        }
        

        /// <inheritdoc />
        public async override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            // Check if we have components registered
            var cont = ViewContext.HttpContext.Items[ContextItemNames.PageContent] as PageContentResponse;
            
            if (string.IsNullOrWhiteSpace(SlotTitle) == false)
            {
                if (cont?.Components?.Exists(x => string.IsNullOrWhiteSpace(x.Slot) == false && x.Slot.ToLowerInvariant() == SlotTitle.ToLowerInvariant()) == true)
                {
                    var slotComponents = cont.Components?.Where(x => string.IsNullOrWhiteSpace(x.Slot) == false && x.Slot.ToLowerInvariant() == SlotTitle.ToLowerInvariant())?.ToList();
                    if (slotComponents.Count > 0)
                    {
                        foreach (var comp in slotComponents.OrderBy(x => x.SortOrder))
                        {
                            var compType = GetCompType(comp);

                            if (compType != null)// && compType.IsAssignableFrom(typeof(Microsoft.AspNetCore.Mvc.ViewComponent)))
                            {
                                var type = Type.GetType("Microsoft.AspNetCore.Mvc.ViewFeatures.IComponentRenderer, Microsoft.AspNetCore.Mvc.ViewFeatures, Version=3.1.1.0, Culture=neutral, PublicKeyToken=adb9793829ddae60");
                                var componentRenderer = ViewContext.HttpContext.RequestServices.GetRequiredService(type);
                                var method = type.GetMethod("RenderComponentAsync");

                                // Pass content component as param to component
                                Parameters[nameof(BaseRazorComponent<ISushiApplicationSettings>.Content)] = comp;

                                // Pass shared components as param to component
                                Parameters[nameof(BaseRazorComponent<ISushiApplicationSettings>.SharedContent)] = cont.SharedComponents;

                                // Pass Page MetaData as param to component
                                Parameters[nameof(BaseRazorComponent<ISushiApplicationSettings>.PageMetaData)] = cont.MetaData;

                                // Set ClearCache property
                                Parameters[nameof(BaseRazorComponent<ISushiApplicationSettings>.ClearCache)] = ViewContext.HttpContext.Request.IsClearCacheCall();

                                // Set IsPreview property
                                Parameters[nameof(BaseRazorComponent<ISushiApplicationSettings>.IsPreview)] = ViewContext.HttpContext.Request.IsPreviewCall();

                                // Get dynamic property
                                var tempComp = Activator.CreateInstance(compType);
                                var isDynamic = (bool)compType.GetProperty("DynamicComponent").GetValue(tempComp);
                                int renderMethod = 1;
                                if (isDynamic)
                                    renderMethod = 3;

                                var result = await (Task<IHtmlContent>)method.Invoke(componentRenderer, new object[] { ViewContext, compType, renderMethod, Parameters });

                                // Reset the TagName. We don't want `component` to render.
                                output.TagName = null;
                                output.Content.AppendHtml(result);
                            }
                        }
                    }
                }
            }
        }
    }
}
