using Microsoft.AspNetCore.Http;
using System;

namespace Sushi.Mediakiwi.Framework
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ComponentListRouting : Attribute
    {
        /// <summary>
        /// Set this Service Provider to incorporate Dependancy Injection when
        /// creating an instance of the Routing type
        /// </summary>
        internal IServiceProvider Services { get; set; }

        private readonly Type setType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentListRouting"/> class.
        /// </summary>
        /// <param name="routing">The routing type, this should be an IComponentListRouting interface.</param>
        public ComponentListRouting(Type routing) 
        {
            setType = routing;
        }

        /// <summary>
        /// Creates an instance of the supplied Routing Type, with Dependancy Injection through Service Provider if available.
        /// </summary>
        /// <returns></returns>
        public IComponentListRouting GetRouteObject()
        {
            if (Services != null)
            {
                return Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance(Services, setType) as IComponentListRouting;
            }
            else 
            {
                return Activator.CreateInstance(setType) as IComponentListRouting;
            }
        }
    }

    public interface IComponentListRouting
    {
        /// <summary>
        /// Performs the actual routing.
        /// </summary>
        /// <returns>Return the actual routing information. When NULL is returned than no routing is applied and the fallback class will be called upon.</returns>
        IComponentListTemplate Validate(Data.IComponentList list, ComponentListRoutingArgs args);

    }

    public class ComponentListRoutingArgs 
    {
        /// <summary>
        /// The selected key passed along via the parent
        /// </summary>
        public int? SelectedKey { get; set; }

        /// <summary>
        /// The selected group passed along via the parent
        /// </summary>
        public int? SelectedGroup { get; set; }

        /// <summary>
        /// The selected groupitem passed along via the parent
        /// </summary>
        public int? SelectedGroupItem { get; set; }

        /// <summary>
        /// The HttpContext passed along via the parent, can be null !
        /// </summary>
        public HttpContext RequestContext { get; set; }
    }
}
