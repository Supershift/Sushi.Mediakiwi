using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;


public static class EnvironmentExtension
{
    /// <summary>
    /// Gets the path.
    /// </summary>
    /// <value>The path.</value>
    public static string GetPath(this IEnvironment inEnvironment, Sushi.Mediakiwi.Beta.GeneratedCms.Console container)
    {
        return container.AddApplicationPath(Sushi.Mediakiwi.CommonConfiguration.PORTAL_PATH); 
    }

    static ConcurrentBag<IPageModule> _registeredPageModules = new ConcurrentBag<IPageModule>();

    public static void AddPageModule<TPageModule>(this IEnvironment inEnvironment, TPageModule pageModule) where TPageModule : class, IPageModule
    {
        if (_registeredPageModules.Contains(pageModule) == false)
        {
            _registeredPageModules.Add(pageModule);
        }
    }

    public static ICollection<IPageModule> GetPageModules(this IEnvironment inEnvironment)
    {
        return _registeredPageModules.ToList();
    }
}
