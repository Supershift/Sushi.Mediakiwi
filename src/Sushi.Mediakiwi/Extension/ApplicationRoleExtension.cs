using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sushi.Mediakiwi.Data;

public static class ApplicationRoleExtension
{

    /// <summary>
    /// Gets the sites.
    /// </summary>
    /// <value>The sites.</value>
    public static Site[] Sites(this IApplicationRole inRole, IApplicationUser user)
    {
        return Site.SelectAllAccessible(user, AccessFilter.RoleAndUser);
    }

    /// <summary>
    /// Gets the lists.
    /// </summary>
    /// <value>The lists.</value>
    public static IComponentList[] Lists(this IApplicationRole inRole, IApplicationUser user)
    {
        return ComponentList.SelectAllAccessibleLists(user, RoleRightType.List);
    }

    /// <summary>
    /// Listses the specified user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns></returns>
    public static Gallery[] Galleries(this IApplicationRole inRole, IApplicationUser user)
    {
        return Gallery.SelectAllAccessible(user);
    }

    /// <summary>
    /// Folderses this instance.
    /// </summary>
    /// <returns></returns>
    public static Folder[] Folders(this IApplicationRole inRole, IApplicationUser user)
    {
        return Folder.SelectAllAccessible(user);
    }
}
