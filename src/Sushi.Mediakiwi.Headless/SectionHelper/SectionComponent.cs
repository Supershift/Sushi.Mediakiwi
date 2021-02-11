using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Sushi.Mediakiwi.Headless.SectionHelper.Elements;
using Sushi.Mediakiwi.Headless.SectionHelper.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sushi.Mediakiwi.Headless.SectionHelper
{
    /// <summary>
    /// The Section Component handles rendering the final output of all collected Section Elements for a specific Section
    /// </summary>
    public class SectionComponent : ComponentBase
    {
        /// <summary>
        /// Inject the Section Service from DI
        /// </summary>
        [Inject]
        public SectionService Service { get; set; }

        /// <summary>
        /// The name for this Section
        /// </summary>
        [Parameter]
        public string SectionName { get; set; }
        
        /// <summary>
        /// Is the current SectionName registered ?
        /// </summary>
        private bool _isRegistered;

        /// <summary>
        /// The current Section
        /// </summary>
        private ISection _section;

        /// <summary>
        /// The current Section Element Order
        /// </summary>
        private int _sectionElementOrder;

        /// <summary>
        /// Builds the Render tree, basically renders every SectionElement within this element
        /// </summary>
        /// <param name="builder"></param>
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);

            // If the current section name has not been registered yet, do so
            if (!_isRegistered)
            {
                if (string.IsNullOrWhiteSpace(SectionName))
                {
                    throw new ArgumentOutOfRangeException(nameof(SectionName));
                }
                _section = Service.RegisterSection(SectionName);
                _section.ChangesDone += Service_ChangesDone;
                _isRegistered = true;
            }

            // Keep track of the current Section Element order
            int currentSectionElementOrder = 0;

            // Loop through all Section Element which don't require an update
            foreach (SectionElement element in _section.Elements.Where(x => !x.ShouldUpdate).ToList())
            {
                BuildElement(builder, element, ref currentSectionElementOrder);
            }

            currentSectionElementOrder = _sectionElementOrder;

            //
            if (_isRegistered)
            {
                foreach (SectionElement element in _section.Elements.Where(x => x.ShouldUpdate || x.RenderOrder == -1))
                {
                    BuildElement(builder, element, ref currentSectionElementOrder);
                    currentSectionElementOrder++;
                }
                _sectionElementOrder = currentSectionElementOrder;
            }
            _section.Elements.Where(x => x.ShouldUpdate = true).ToList().ForEach(x => x.ShouldUpdate = false);
        }

        /// <summary>
        /// Render Section Element to output
        /// </summary>
        /// <param name="builder">The RenderTree builder which takes care of the rendering</param>
        /// <param name="element">The Section Element to render</param>
        /// <param name="sectionElementOrder">The Section Element order</param>
        private void BuildElement(RenderTreeBuilder builder, SectionElement element, ref int sectionElementOrder)
        {
            if (element.RenderOrder == -1)
            {
                element.RenderOrder = sectionElementOrder;
            }
            else if (element.ShouldUpdate)
            {
                sectionElementOrder = element.RenderOrder;
            }
            builder.OpenElement(sectionElementOrder, element.Name);
            sectionElementOrder++;
            foreach (var kv in element.AllProperties)
            {
                builder.AddAttribute(sectionElementOrder, kv.Key, kv.Value);
                sectionElementOrder++;
            }
            if (string.IsNullOrWhiteSpace(element?.Content.Value) == false)
            {
                builder.AddContent(sectionElementOrder, element.Content);
                sectionElementOrder++;
            }
            builder.CloseElement();
        }

        protected async void Service_ChangesDone(object sender, EventArgs e)
        {
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        
    }
}