using Sushi.Mediakiwi.Data;
using System.Linq;

public static class SiteExtension
{

    /// <summary>
    /// Determines whether [has role access] [the specified role ID].
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>
    /// 	<c>true</c> if [has role access] [the specified role ID]; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasRoleAccess(this Site inSite, IApplicationUser user)
    {
        if (inSite.ID == 0 || user.Role().All_Sites) return true;
        var selection = from item in Site.SelectAllAccessible(user, AccessFilter.RoleAndUser) where item.ID == inSite.ID select item;
        bool xs = selection.Count() == 1;
        return xs;
    }

    internal static void CreateSiteFolders(this Site inSite, int siteID)
    {
        Folder webFolder = new Folder();
        webFolder.Type = FolderType.Page;
        webFolder.Name = Common.FolderRoot;
        webFolder.CompletePath = Common.FolderRoot;
        webFolder.SiteID = siteID;
        webFolder.Save();

        Folder logicFolder = new Folder();
        logicFolder.Type = FolderType.List;
        logicFolder.Name = Common.FolderRoot;
        logicFolder.CompletePath = Common.FolderRoot;
        logicFolder.SiteID = siteID;
        logicFolder.Save();
    }

}

