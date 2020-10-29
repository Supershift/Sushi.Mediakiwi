using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    /// <summary>
    /// 
    /// </summary>
    public class VersionUpdater : BaseImplementation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VersionUpdater"/> class.
        /// </summary>
        public VersionUpdater()
        {
            wim.CanContainSingleInstancePerDefinedList = true;

            this.ListLoad += new Sushi.Mediakiwi.Framework.ComponentListEventHandler(VersionUpdater_ListLoad);
            this.ListAction += new Sushi.Mediakiwi.Framework.ComponentActionEventHandler(VersionUpdater_ListAction);
        }

        void VersionUpdater_ListAction(object sender, Sushi.Mediakiwi.Framework.ComponentActionEventArgs e)
        {
            //m_RequiredUpdates = new Sushi.Mediakiwi.Data.SubList();

            //bool allUpdated = true;
            //for (int count = m_Environment + 1; count < m_Assembly + 1; count++)
            //{
            //    Type m_LoadedType = m_Assem.GetType(string.Concat("Wim.Update.Version.Update_", count));

            //    Update.iVersion instance = null;
            //    if (m_LoadedType != null)
            //        instance = System.Activator.CreateInstance(m_LoadedType) as Update.iVersion;

            //    if (instance != null)
            //    {
            //        bool update = instance.Execute();

            //        if (!update)
            //            allUpdated = false;

            //        m_RequiredUpdates.Add(new Sushi.Mediakiwi.Data.SubList.SubListitem(count, string.Concat("<b>wimUpdate - ", count, update ? " [OK]" : " [ERROR]", "</b><br/>", instance.Description)));
            //        if (!update)
            //            break;
            //    }
            //}

            //if (allUpdated)
            //    Response.Redirect(Wim.Utility.GetSafeUrl(Request));
        }

        int m_Assembly;
        int m_Environment;

        Assembly m_Assem;
        void VersionUpdater_ListLoad(object sender, Sushi.Mediakiwi.Framework.ComponentListEventArgs e)
        {
            //this.AssemblyVersion = CommonConfiguration.Version;

            //decimal currentVersion = Sushi.Mediakiwi.Data.Environment.SelectVersion();

            //this.EnvironmentVersion = currentVersion.ToString("N").Replace(',', '.');

            //m_Assembly = Convert.ToInt32(Decimal.Multiply(Utility.ConvertToDecimal(CommonConfiguration.Version), 100));
            //m_Environment = Convert.ToInt32(Decimal.Multiply(currentVersion, 100));

            m_Info0 = "It has been detected that the assembly (codebase) has a different version then the environment (database). To continue both version have to be equal.<br/><br/>It could be the case that the environment currently is being update. Address your technical contact for further details.";

            wim.Notification.AddNotification(m_Info0);
            //if (m_Environment == m_Assembly)
            //{
            //    this.m_Info1 = "No updates are required.";
            //    return;
            //}
            //else if (m_Environment < m_Assembly)
            //{
            //    m_CanUpdate = true;
            //}
            //else if (m_Environment > m_Assembly)
            //{
            //    this.m_Info1 = "The assembly version is lower then the environment.<br/>Please contact the System Administrator to get an updated assembly version.";
            //    return;
            //}

            //m_RequiredUpdates = new Sushi.Mediakiwi.Data.SubList();

            //System.IO.FileInfo nfo = new System.IO.FileInfo(Server.MapPath(string.Concat(Request.ApplicationPath, "/bin/Wim.Update.dll")));
            //m_Assem = Assembly.LoadFrom(nfo.FullName);

            //for (int count = m_Environment + 1; count < m_Assembly + 1; count++)
            //{
            //    Type m_LoadedType = m_Assem.GetType(string.Concat("Wim.Update.Version.Update_", count));

            //    Update.iVersion instance = null;
            //    if (m_LoadedType != null)
            //        instance = System.Activator.CreateInstance(m_LoadedType) as Update.iVersion;

            //    if (instance != null)
            //        m_RequiredUpdates.Add(new Sushi.Mediakiwi.Data.SubList.SubListitem(count, string.Concat("<b>wimUpdate - ", count, "</b><br/>", instance.Description)));
            //}
        }

        bool m_CanUpdate;
        /// <summary>
        /// Gets a value indicating whether this instance can update.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can update; otherwise, <c>false</c>.
        /// </value>
        public bool CanUpdate
        {
            get { return m_CanUpdate; }
        }

        private string m_Info0;
        /// <summary>
        /// Gets or sets the info.
        /// </summary>
        /// <value>The info.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextLine("Information")]
        public string Info0
        {
            get { return m_Info0; }
            set { m_Info0 = value; }
        }

        //private string m_AssemblyVersion;
        ///// <summary>
        ///// Gets or sets the assembly version.
        ///// </summary>
        ///// <value>The assembly version.</value>
        //[Sushi.Mediakiwi.Framework.ContentListItem.TextLine("Assembly version")]
        //public string AssemblyVersion
        //{
        //    get { return m_AssemblyVersion; }
        //    set { m_AssemblyVersion = value; }
        //}

        //private string m_EnvironmentVersion;
        ///// <summary>
        ///// Gets or sets the assembly version.
        ///// </summary>
        ///// <value>The assembly version.</value>
        //[Sushi.Mediakiwi.Framework.ContentListItem.TextLine("Environment version")]
        //public string EnvironmentVersion
        //{
        //    get { return m_EnvironmentVersion; }
        //    set { m_EnvironmentVersion = value; }
        //}

        //private string m_Info1;
        ///// <summary>
        ///// Gets or sets the info.
        ///// </summary>
        ///// <value>The info.</value>
        //[Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("CanUpdate", State = false)]
        //[Sushi.Mediakiwi.Framework.ContentListItem.TextLine("Information")]
        //public string Info1
        //{
        //    get { return m_Info1; }
        //    set { m_Info1 = value; }
        //}

        //private bool m_Update;
        ///// <summary>
        ///// Gets or sets a value indicating whether this <see cref="VersionUpdater"/> is update.
        ///// </summary>
        ///// <value><c>true</c> if update; otherwise, <c>false</c>.</value>
        //[Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("CanUpdate")]
        //[Sushi.Mediakiwi.Framework.ContentListItem.Button("Update environment")]
        //public bool Update
        //{
        //    get { return m_Update; }
        //    set { m_Update = value; }
        //}

        //private string m_Info = "Updates";
        ///// <summary>
        ///// Gets or sets the info.
        ///// </summary>
        ///// <value>The info.</value>
        //[Sushi.Mediakiwi.Framework.OnlyVisibleWhenTrue("CanUpdate")]
        //[Sushi.Mediakiwi.Framework.ContentListItem.TextLine(null)]
        //public string Info
        //{
        //    get { return m_Info; }
        //    set { m_Info = value; }
        //}

        //private Sushi.Mediakiwi.Data.SubList m_RequiredUpdates;
        ///// <summary>
        ///// Gets or sets the assembly version.
        ///// </summary>
        ///// <value>The assembly version.</value>

        //[Sushi.Mediakiwi.Framework.ContentListItem.SubListSelect("Required updates", null)]
        //public Sushi.Mediakiwi.Data.SubList RequiredUpdates
        //{
        //    get { return m_RequiredUpdates; }
        //    set { m_RequiredUpdates = value; }
        //}


    }
}
