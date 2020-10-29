using System;
using System.Collections.Generic;
using System.Text;
using Wim.Processing;

namespace Sushi.Mediakiwi.AppCentre.Processing
{
    /// <summary>
    /// 
    /// </summary>
    public class LoggedInBox : Base, iProcessing
    {
        /// <summary>
        /// Inits the specified data.
        /// </summary>
        /// <param name="component">The component.</param>
        public void Init(Sushi.Mediakiwi.Beta.GeneratedCms.Source.External.ComponentConfiguration component)
        {
            component.Data = component.Data.Replace("<wimvalue:name />", wim.CurrentApplicationUser.Displayname);
            component.Data = component.Data.Replace("<wimvalue:role />", wim.CurrentApplicationUserRole.Name);
        }
    }
}
