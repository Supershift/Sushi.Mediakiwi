using System;
using System.Collections.Generic;
using System.Text;
using Wim.Processing;

namespace Sushi.Mediakiwi.AppCentre.Processing
{
    //class Filtergrid : Base, iProcessing
    //{        
        
    //    /// <summary>
    //    /// Inits the specified data.
    //    /// </summary>
    //    /// <param name="component">The component.</param>
    //    public void Init(Sushi.Mediakiwi.Beta.GeneratedCms.Source.External.ComponentConfiguration component)
    //    {
    //        Sushi.Mediakiwi.Beta.GeneratedCms.Source.GridCreation grid = new Sushi.Mediakiwi.Beta.GeneratedCms.Source.GridCreation();
    //        component.Data = component.Data.Replace("<wimvalue:grid />", grid.GetGridFromListInstance(wim, wim.Console, 0));

    //        component.Data = component.Data.Replace("<table class=\"data\">", "<table class=\"list paged\">");

    //        //Sushi.Mediakiwi.Data.ComponentList[] entries = Sushi.Mediakiwi.Data.ComponentList.SelectAll(wim.CurrentFolder.ID);
    //        //StringBuilder build = new StringBuilder();
    //        //foreach (Sushi.Mediakiwi.Data.ComponentList entry in entries)
    //        //{
    //        //    string link = string.Concat(wim.Console.WimPagePath, "?list=", entry.ID);
    //        //    string text = entry.Name;
    //        //    build.Append(component.DataInstance.Replace("<wimvalue:link />", link).Replace("<wimvalue:text />", text));
    //        //}
    //        //component.Data = component.Data.Replace("<wim:datainstance />", build.ToString());
    //    }
    //}
}
