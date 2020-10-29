using System;
using System.Text.RegularExpressions;

using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web;


using Sushi.Mediakiwi.Data;

namespace Sushi.Mediakiwi.Framework
{
    public delegate void ComponentDataReportEventHandler(object sender, ComponentDataReportEventArgs e);
    /// <summary>
    /// 
    /// </summary>
    public delegate void ComponentListEventHandler(IComponentListTemplate sender, ComponentListEventArgs e);

    public delegate void ComponentAsyncEventHandler(object sender, ComponentAsyncEventArgs e);

    /// <summary>
    /// 
    /// </summary>
    public delegate void ComponentActionEventHandler(object sender, ComponentActionEventArgs e);

    /// <summary>
    /// 
    /// </summary>
    //public delegate void ComponentSearchEventHandler(object sender);
    /// <summary>
    /// 
    /// </summary>
    public delegate void ComponentSearchEventHandler(object sender, ComponentListSearchEventArgs e);

    public delegate void ComponentDataItemCreatedEventHandler(object sender, ListDataItemCreatedEventArgs e);
    

    /// <summary>
    /// 
    /// </summary>
    public delegate void ComponentTemplateEventHandler(object sender, ComponentTemplateEventArgs e);

    public delegate void ContentInfoEventHandler(object sender, ContentInfoEventArgs e);

}
