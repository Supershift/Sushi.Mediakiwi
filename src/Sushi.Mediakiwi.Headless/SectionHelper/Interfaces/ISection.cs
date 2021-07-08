using Sushi.Mediakiwi.Headless.SectionHelper.Elements;
using System;
using System.Collections.Generic;

namespace Sushi.Mediakiwi.Headless.SectionHelper.Interfaces
{
    /// <summary>
    /// The base Interface for a Section
    /// </summary>
    public interface ISection
    {
        /// <summary>
        /// The name of the Section
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Fires when all Changes are done
        /// </summary>
        event EventHandler<EventArgs> ChangesDone;

        /// <summary>
        /// The Section Elements contained in this Section
        /// </summary>
        List<SectionElement> Elements { get; }
    }

}
