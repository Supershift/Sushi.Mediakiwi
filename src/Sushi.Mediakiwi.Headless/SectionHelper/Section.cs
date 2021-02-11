using Sushi.Mediakiwi.Headless.SectionHelper.Elements;
using Sushi.Mediakiwi.Headless.SectionHelper.Interfaces;
using System;
using System.Collections.Generic;

namespace Sushi.Mediakiwi.Headless.SectionHelper
{
    /// <summary>
    /// Represents a Section which renders SectionElements added to it
    /// </summary>
    public class Section : ISection
    {
        /// <summary>
        /// The Name of this section
        /// </summary>
        public string Name { get; private set; }


        /// <summary>
        /// Fires when changes are Done
        /// </summary>
        public event EventHandler<EventArgs> ChangesDone;

        /// <summary>
        /// Creates a new Section
        /// </summary>
        /// <param name="name">The name of this section</param>
        public Section(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The Section Elements contained in this Section
        /// </summary>
        public List<SectionElement> Elements { get; private set; } = new List<SectionElement>();

        /// <summary>
        /// Invoke the ChangesDone event
        /// </summary>
        public void InvokeChangesDone()
        {
            ChangesDone?.Invoke(this, EventArgs.Empty);
        }
    }
}
