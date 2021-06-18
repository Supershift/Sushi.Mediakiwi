using Sushi.Mediakiwi.Data;
using System.Linq;

public static class GalleryExtension
{

    /// <summary>
    /// Determines whether [has role access] [the specified user].
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>
    /// 	<c>true</c> if [has role access] [the specified user]; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasRoleAccess(this Gallery inGallery, IApplicationUser user)
    {
        if (inGallery.CompletePath == "/") 
            return true;

        if (inGallery.ID == 0 || user.Role().All_Galleries)
            return true;

        var xslist = Gallery.SelectAllAccessible(user);

        ////  [MM:10-jan-2011] add for veiling3000, this should be moved to a StoredProcedure
        //if (Wim.CommonConfiguration.RIGHTS_GALLERY_SUBS_ARE_ALLOWED)
        //{
        //    foreach (var item in xslist)
        //    {
        //        if (inGallery.CompletePath.StartsWith(item.CompletePath, StringComparison.InvariantCultureIgnoreCase))
        //        {
        //            return true;
        //        }
        //    }
        //}

        var selection = from item in xslist where item.ID == inGallery.ID select item;
        bool xs = selection.Count() == 1;
        return xs;
    }

    /// <summary>
    /// Updates the files.
    /// </summary>
    //public static void UpdateFiles(this Gallery inGallery)
    //{
    //    if (inGallery.ID == 0 || HttpContext.Current == null)
    //        return;
    //    Sushi.Mediakiwi.Framework.Functions.CleanRepository cleaner = new Sushi.Mediakiwi.Framework.Functions.CleanRepository();
    //    CreateFolder(inGallery);
    //    cleaner.ScanFiles(inGallery);
    //    cleaner.ScanFolders(inGallery);
    //}

    /// <summary>
    /// Creates the folder.
    /// </summary>
    /// <param name="request">The request.</param>
    //public static void CreateFolder(this Gallery inGallery)
    //{
    //    CleanPath(inGallery);

    //    if (inGallery.ID == 0) return;
    //    //Gallery[] list = SelectAllByBackwardTrail(this.ID);

    //    string folderInfo = Gallery.LocalRepositoryBase;

    //    string[] folders = inGallery.CompletePath.Split('/');

    //    for (int i = 0; i < folders.Length; i++)
    //    {
    //        string folder = folders[i];
    //        folderInfo += string.Concat("\\", folder);

    //        if (!Directory.Exists(folderInfo))
    //            Directory.CreateDirectory(folderInfo);
    //    }

    //}


    /// <summary>
    /// Cleans the path.
    /// </summary>
    //internal static void CleanPath(this Gallery inGallery)
    //{
    //    Regex MustMatch = new Regex(@"^[^\|\?\:\*""<>]*$", RegexOptions.IgnoreCase);
    //    if (!MustMatch.IsMatch(inGallery.CompletePath))
    //    {
    //        string[] split = inGallery.CompletePath.Split('/');
    //        string candidate = "";
    //        foreach (string item in split)
    //        {
    //            string tmp = Wim.Utility.GlobalRegularExpression.Implement.ReplaceNotAcceptableFilenameCharacter.Replace(item, string.Empty);
    //            candidate += candidate == "/" ? tmp : string.Concat("/", tmp);
    //        }
    //        inGallery.CompletePath = candidate;
    //    }
    //}


}

