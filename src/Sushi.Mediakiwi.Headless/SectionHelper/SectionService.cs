using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Sushi.Mediakiwi.Headless.SectionHelper.Elements;
using Sushi.Mediakiwi.Headless.SectionHelper.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sushi.Mediakiwi.Headless.SectionHelper
{
    /// <summary>
    /// Section Service Extension for adding the SectionService to the ServiceCollection
    /// </summary>
    public static class SectionServiceExtensions
    {
        /// <summary>
        /// Adds the SectionService service to the ServiceCollection. 
        /// This service is used to add Javascript & Stylesheet to a page section on the fly.
        /// </summary>
        /// <param name="services"></param>
        public static void AddSectionHelper(this IServiceCollection services)
        {
            services.AddScoped<SectionService>();
        }
    }

    /// <summary>
    /// The SectionService handles everything concerning custom (Javascript / StyleSheet) Sections
    /// </summary>
    public class SectionService
    {
        /// <summary>
        /// The Last URL which was handled by the Service
        /// </summary>
        private string _lastHandledUri;

        /// <summary>
        /// Every known section for this request 
        /// </summary>
        private readonly List<Section> _sections = new List<Section>();
        
        private NavigationManager _uriHelper { get; set; }

        /// <summary>
        /// SectionService constructor with Navigation Manager reference
        /// </summary>
        /// <param name="uriHelper"></param>
        public SectionService(NavigationManager uriHelper)
        {
            _uriHelper = uriHelper;
            _lastHandledUri = _uriHelper.Uri;
            _uriHelper.LocationChanged += UriHelper_LocationChanged;
        }

        /// <summary>
        /// Adds a new SectionElement to the Section Elements collection
        /// </summary>
        /// <param name="sectionName">The name of the section to add the Element to</param>
        /// <param name="element">The element to add</param>
        public void AddElement(string sectionName, SectionElement element)
        {
            Section section = _sections.FirstOrDefault(x => x.Name == sectionName);
            if (section == null)
            {
                section = new Section(sectionName);
                _sections.Add(section);
            }
            section.Elements.Add(element);
            section.InvokeChangesDone();
        }

        /// <summary>
        /// Registers a new Section to add Section Elements to
        /// </summary>
        /// <param name="sectionName">The name of the new Section</param>
        /// <returns></returns>
        public ISection RegisterSection(string sectionName)
        {
            Section section = _sections.FirstOrDefault(x => x.Name == sectionName);
            if (section == null)
            {
                section = new Section(sectionName);
                _sections.Add(section);
            }
            return section;
        }

        /// <summary>
        /// Will fire when the location is changed, so after a navigation took place
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UriHelper_LocationChanged(object sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
        {
            if (_lastHandledUri != e.Location)
            {
                _lastHandledUri = e.Location;
                foreach (Section s in _sections)
                {
                    s.Elements.Clear();
                    s.InvokeChangesDone();
                }
            }
            else
            {
                foreach (Section s in _sections)
                {
                    s.Elements.Where(x => x is JavaScriptInline).ToList().ForEach(x => { x.ShouldUpdate = true; x.RenderOrder = -1; });
                    s.InvokeChangesDone();
                }
            }
        }

        #region Static Element Constructors

        /// <summary>
        /// Creates a Javascript file element
        /// </summary>
        /// <param name="url">The URL / Path to the JS File</param>
        /// <returns>A new JavaScriptFile element</returns>
        public static JavaScriptFile JavascriptFile(string url)
        {
            return new JavaScriptFile(url);
        }

        /// <summary>
        /// Creates a JavaScript Inline element
        /// </summary>
        /// <param name="content">The (string) content</param>
        /// <returns>A new JavaScriptInline element</returns>
        public static JavaScriptInline JavaScriptInline(string content)
        {
            return new JavaScriptInline(content);
        }

        /// <summary>
        /// Creates a JavaScript Inline element
        /// </summary>
        /// <param name="content">The (raw HTML) content</param>
        /// <returns>A new JavaScriptInline element</returns>
        public static JavaScriptInline JavaScriptInline(MarkupString content)
        {
            return new JavaScriptInline(content);
        }

        /// <summary>
        /// Creates a StyleSheet File element
        /// </summary>
        /// <param name="url">The URL / Path to the Stylesheet File</param>
        /// <returns>A new StyleSheetFile element</returns>
        public static StylesheetFile StyleSheetFile(string url)
        {
            return new StylesheetFile(url);
        }

        /// <summary>
        /// Creates a StyleSheet Inline element
        /// </summary>
        /// <param name="content">The (string) content</param>
        /// <returns>A new StyleSheetInline element</returns>
        public static StyleSheetInline StyleSheetInline(string content)
        {
            return new StyleSheetInline(content);
        }

        /// <summary>
        /// Creates a StyleSheet Inline element
        /// </summary>
        /// <param name="content">The (raw HTML) content</param>
        /// <returns>A new StyleSheetInline element</returns>
        public static StyleSheetInline StyleSheetInline(MarkupString content)
        {
            return new StyleSheetInline(content);
        }

        #endregion Static Element Constructors

    }
}