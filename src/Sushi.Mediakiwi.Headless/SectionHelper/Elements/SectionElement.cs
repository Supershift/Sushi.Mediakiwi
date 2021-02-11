using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Headless.SectionHelper.Elements
{
    /// <summary>
    /// Represents a single Section Element
    /// </summary>
    public class SectionElement
    {
        /// <summary>
        /// The name of this Section Element
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The HTML/CSS/JS content for this Section Element
        /// </summary>
        public MarkupString Content { get; protected set; }

        /// <summary>
        /// The order of rendering for this Section Element
        /// </summary>
        public int RenderOrder { get; set; } = -1;

        /// <summary>
        /// Should this Section Element fire an update ?
        /// </summary>
        public bool ShouldUpdate { get; set; }

        /// <summary>
        /// The public properties for this Section Element
        /// </summary>
        protected Dictionary<string, string> PublicProperties { get; } = new Dictionary<string, string>();

        /// <summary>
        /// The private properties for this Section Element
        /// </summary>
        protected Dictionary<string, string> PrivateProperties { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Adds a public property to this Section Element
        /// </summary>
        /// <param name="name">The name of the Property to add</param>
        /// <param name="value">The value of the Property to add</param>
        public void AddProperty(string name, string value)
        {
            PublicProperties.Add(name, value);
            ShouldUpdate = true;
        }

        /// <summary>
        /// Removes a public property from this Section Element
        /// </summary>
        /// <param name="name">The name of the Property to remove</param>
        public void RemoveProperty(string name)
        {
            PublicProperties.Remove(name);
            ShouldUpdate = true;
        }

        /// <summary>
        /// Combined public & private properties
        /// </summary>
        public Dictionary<string, string> AllProperties
        {
            get
            {
                Dictionary<string, string> d = new Dictionary<string, string>();
                foreach (var kv in PrivateProperties)
                {
                    d.Add(kv.Key, kv.Value);
                }
                foreach (var kv in PublicProperties)
                {
                    d.Add(kv.Key, kv.Value);
                }
                return d;
            }
        }

        /// <summary>
        /// Creates a Section Element
        /// </summary>
        /// <param name="name">The name of the new Section Element</param>
        public SectionElement(string name) : this(name, string.Empty) { }

        /// <summary>
        /// Creates a Section Element
        /// </summary>
        /// <param name="name">The name of the new Section Element</param>
        /// <param name="content">The (string) content of the new Section Element</param>
        public SectionElement(string name, string content)
        {
            Name = name;
            Content = new MarkupString(content);
        }

        /// <summary>
        /// Creates a Section Element
        /// </summary>
        /// <param name="name">The name of the new Section Element</param>
        /// <param name="content">The (HTML) content of the new Section Element</param>
        public SectionElement(string name, MarkupString content)
        {
            Name = name;
            Content = content;
        }
    }
}
