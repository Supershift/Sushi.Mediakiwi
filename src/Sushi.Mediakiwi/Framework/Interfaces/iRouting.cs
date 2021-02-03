using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sushi.Mediakiwi.Framework
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ComponentListRouting : Attribute
    {
        public IComponentListRouting Routing { get; internal set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentListRouting"/> class.
        /// </summary>
        /// <param name="routing">The routing type, this should be an IComponentListRouting interface.</param>
        public ComponentListRouting(Type routing) {
            this.Routing = System.Activator.CreateInstance(routing) as IComponentListRouting;
        }
    }

    public interface IComponentListRouting
    {
        /// <summary>
        /// Performs the actual routing.
        /// </summary>
        /// <returns>Return the actual routing information. When NULL is returned than no routing is applied and the fallback class will be called upon.</returns>
        IComponentListTemplate Validate(Sushi.Mediakiwi.Data.IComponentList list, ComponentListRoutingArgs args);
    }

    public class ComponentListRoutingArgs 
    {
        public int? SelectedKey { get; set; }
        public int? SelectedGroup { get; set; }
        public int? SelectedGroupItem { get; set; }
    }
}
