using System;
using System.Collections.Generic;
using System.Text;
using Wim.Processing;

namespace Sushi.Mediakiwi.AppCentre.Processing
{
    /// <summary>
    /// 
    /// </summary>
    public class Leftnavigation : Base, iProcessing
    {
        /// <summary>
        /// Inits the specified data.
        /// </summary>
        /// <param name="component">The component.</param>
        public void Init(Sushi.Mediakiwi.Beta.GeneratedCms.Source.External.ComponentConfiguration component)
        {
            Sushi.Mediakiwi.Data.IComponentList[] entries = Sushi.Mediakiwi.Data.ComponentList.SelectAll(wim.CurrentFolder.ID);
            StringBuilder build = new StringBuilder();
            foreach (Sushi.Mediakiwi.Data.ComponentList entry in entries)
            {
                string link = string.Concat(wim.Console.WimPagePath, "?list=", entry.ID);
                string text = entry.Name;
                build.Append(component.DataInstance.Replace("<wimvalue:link />", link).Replace("<wimvalue:text />", text));
            }
            component.Data = component.Data.Replace("<wim:datainstance />", build.ToString());
        }
    }
}