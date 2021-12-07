using Microsoft.AspNetCore.Http;
using Sushi.Mediakiwi;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using System;
using System.Linq;

public static class ComponentListExtension
{
    /// <summary>
    /// Get the corresponding catalog for a specific portal
    /// </summary>
    /// <param name="portal">The portal.</param>
    /// <returns></returns>
    //public static Catalog Catalog(this IComponentList inComponentList, WimServerPortal portal = null)
    //{
    //    if (inComponentList.CatalogID > 0)
    //        return Sushi.Mediakiwi.Data.Catalog.SelectOne(inComponentList.CatalogID, portal);

    //    return null;
    //}

    /// <summary>
    /// Determine if the user is allowed to view this componentList
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="portal">The portal.</param>
    /// <returns>
    ///   <c>true</c> if [has role access2] [the specified user]; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasRoleAccess(this IComponentList inComponentList, IApplicationUser user)
    {
        if (inComponentList == null || inComponentList.ID == 0 || user.SelectRole().All_Lists) 
            return true;

        var selection = RoleRightAccessItem.Select(user.RoleID, (int)RoleRightType.List, (int)RoleRightType.List);
        var candidate = from item in selection where item.ID == inComponentList.ID select item;
        bool xs = selection.Count() == 1;
        return xs;
    }

    /// <summary>
    /// Get an instance of the class behind this componentList
    /// </summary>
    /// <returns></returns>
    /// <value>The instance.</value>
    public static IComponentListTemplate GetInstance(this IComponentList inComponentList, HttpContext context)
    {
        IComponentListTemplate m_Instance = null;
        Type candidate = null;
        object instance = null;


        if (m_Instance == null)
        {
            var assembly = inComponentList.AssemblyName;
            if (assembly.Equals("Wim.Framework.dll", StringComparison.CurrentCultureIgnoreCase))
            {
                assembly = "Sushi.Mediakiwi.dll";
                inComponentList.AssemblyName = assembly;
                inComponentList.ClassName = inComponentList.ClassName.Replace("Wim", "Sushi.Mediakiwi");
                inComponentList.Save();
            }

            instance = Utils.CreateInstance(inComponentList.AssemblyName, inComponentList.ClassName, out candidate, false, context.RequestServices);

            //  [11 nov 14:MM] Added routing support
            #region Routing support

            // BD 2016-10-07: Added Nullcheck due to fatal errors
            if (instance != null)
            {
                var routingAttributes = instance.GetType().GetCustomAttributes(typeof(ComponentListRouting), true);
                if (routingAttributes != null && routingAttributes.Length > 0)
                {
                    //  Take first routing and process it
                    var routingAttribute = routingAttributes[0] as ComponentListRouting;
                    if (routingAttribute != null && routingAttribute.Routing != null)
                    {
                        //  When routing exists, validate this route, when NULL, ignore it.
                        ComponentListRoutingArgs e = null;
                        //  Dirty code, but SOLID priciples can not apply (yet).
                        if (context != null)
                        {
                            e = new ComponentListRoutingArgs()
                            {
                                SelectedKey = Utility.ConvertToIntNullable(context.Request.Query["item"]),
                                SelectedGroup = Utility.ConvertToIntNullable(context.Request.Query["group"]),
                                SelectedGroupItem = Utility.ConvertToIntNullable(context.Request.Query["groupitem"])
                            };
                        }

                        var instanceCandidate = routingAttribute.Routing.Validate(inComponentList, e);
                        if (instanceCandidate != null)
                            instance = instanceCandidate;
                    }
                }
            }
            #endregion Routing support
        }

        if (instance != null)
        {
            m_Instance = (IComponentListTemplate)instance;
            m_Instance.Init(context);
            if (m_Instance.wim.PassOverClassInstance != null)
                m_Instance = m_Instance.wim.PassOverClassInstance;

            m_Instance.wim.CurrentList = inComponentList;
        }

        return m_Instance;
    }

    public static IComponentListTemplate GetInstance(this IComponentList inComponentList, Sushi.Mediakiwi.Beta.GeneratedCms.Console console)
    {
        IComponentListTemplate m_Instance = null;
        Type candidate = null;
        object instance = null;


        if (m_Instance == null)
        {
            var assembly = inComponentList.AssemblyName;
            if (assembly.Equals("Wim.Framework.dll", StringComparison.CurrentCultureIgnoreCase))
            {
                assembly = "Sushi.Mediakiwi.dll";
                inComponentList.AssemblyName = assembly;
                inComponentList.ClassName = inComponentList.ClassName.Replace("Wim", "Sushi.Mediakiwi");
                inComponentList.Save();
            }

            instance = Utils.CreateInstance(inComponentList.AssemblyName, inComponentList.ClassName, out candidate, false, console.Context.RequestServices);

            //  [11 nov 14:MM] Added routing support
            #region Routing support

            // BD 2016-10-07: Added Nullcheck due to fatal errors
            if (instance != null)
            {
                var routingAttributes = instance.GetType().GetCustomAttributes(typeof(ComponentListRouting), true);
                if (routingAttributes != null && routingAttributes.Length > 0)
                {
                    //  Take first routing and process it
                    var routingAttribute = routingAttributes[0] as ComponentListRouting;
                    if (routingAttribute != null && routingAttribute.Routing != null)
                    {
                        //  When routing exists, validate this route, when NULL, ignore it.
                        ComponentListRoutingArgs e = null;
                        //  Dirty code, but SOLID priciples can not apply (yet).
                        if (console.Context != null)
                        {
                            e = new ComponentListRoutingArgs()
                            {
                                SelectedKey = Utility.ConvertToIntNullable(console.Context.Request.Query["item"]),
                                SelectedGroup = Utility.ConvertToIntNullable(console.Context.Request.Query["group"]),
                                SelectedGroupItem = Utility.ConvertToIntNullable(console.Context.Request.Query["groupitem"])
                            };
                        }

                        var instanceCandidate = routingAttribute.Routing.Validate(inComponentList, e);
                        if (instanceCandidate != null)
                            instance = instanceCandidate;
                    }
                }
            }
            #endregion Routing support
        }

        if (instance != null)
        {
            m_Instance = (IComponentListTemplate)instance;
            m_Instance.Init(console.Context);
            if (m_Instance.wim.PassOverClassInstance != null)
                m_Instance = m_Instance.wim.PassOverClassInstance;
        }

        return m_Instance;
    }
}

