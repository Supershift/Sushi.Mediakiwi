using System;
using Sushi.Mediakiwi.Data;

public static class ComponentVersionExtension
{   
    /// <summary>
    /// save overwrite. this implementation can also flush the cache automaticly
    /// </summary>
    /// <param name="doFlush">Default is set to true to flush the cache after insert or update</param>
    /// <returns></returns>
    //internal static bool Save(this ComponentVersion inVersion, bool doFlush = true) 
    //{
    //    bool save;

    //    inVersion.Updated = DateTime.Now;

    //    if (inVersion?.ID >  0)
    //        save = inVersion.Update();
    //    else
    //        save = inVersion.Insert();

    //    if (doFlush)
    //        Wim.Utilities.CacheItemManager.FlushIndexOfCacheObjects(string.Concat("Data_", inVersion.GetType().ToString()));

    //    if (HttpContext.Current != null && HttpContext.Current.Items["wim.Saved.ID"] == null)
    //        HttpContext.Current.Items["wim.Saved.ID"] = inVersion.ID;

    //    return save;
    //}

    public static void Publish(this ComponentVersion inComponentVersion)
    {
        try
        {
            var component = Component.SelectOne(inComponentVersion.GUID);
            inComponentVersion.Apply(component);
            component.Save();

            //if (HttpContext.Current != null)
            //    EnvironmentVersionLogic.Flush();
        }
        catch (Exception ex)
        {
            inComponentVersion.Name = ex.Message;
        }
    }

    public static void TakeDown(this ComponentVersion inComponentVersion)
    {
        try
        {
            var published = Component.SelectOne(inComponentVersion.GUID);
            published.Delete();

            //if (HttpContext.Current != null)
            //    EnvironmentVersionLogic.Flush();
        }
        catch (Exception ex)
        {
            inComponentVersion.Name = ex.Message;
        }
    }

}
