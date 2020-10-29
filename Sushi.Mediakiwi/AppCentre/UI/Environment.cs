using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Web;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.UI;
using Sushi.Mediakiwi.Data;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    /// <summary>
    /// Represents a Environment entity.
    /// </summary>
    public class Environment : BaseImplementation
    {
     
        

        /// <summary>
        /// Initializes a new instance of the <see cref="Environment"/> class.
        /// </summary>
        public Environment()
        {
            wim.CanContainSingleInstancePerDefinedList = true;

            this.ListLoad += new ComponentListEventHandler(Environment_ListLoad);
            this.ListSave += new ComponentListEventHandler(Environment_ListSave);
        }


        string m_PasswordAtStart;

        /// <summary>
        /// Handles the ListSave event of the Environment control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentListEventArgs"/> instance containing the event data.</param>
        void Environment_ListSave(object sender, ComponentListEventArgs e)
        {
            if (!Implement.Password.Equals(m_PasswordAtStart))
                this.Implement.Password = Utility.HashStringByMD5(Implement.Password);

            if (!Implement.RelativePath.StartsWith("/"))
                Implement.RelativePath = string.Concat("/", Implement.RelativePath);

            this.Implement.Save();

            //  Redirect as path could be changed!
            string url = wim.AddApplicationPath(this.Implement.RelativePath);
            Response.Redirect(string.Concat(url, "?list=", wim.CurrentList.ID));
        }

        /// <summary>
        /// Handles the ListLoad event of the Environment control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentListEventArgs"/> instance containing the event data.</param>
        void Environment_ListLoad(object sender, ComponentListEventArgs e)
        {
            this.Implement = Sushi.Mediakiwi.Data.Environment.Current;
            m_PasswordAtStart = this.Implement.Password;
            this.FormMaps.Add(new Forms.EnvironmentForm(this.Implement));
        }

        /// <summary>
        /// Gets or sets the implement.
        /// </summary>
        /// <value>The implement.</value>
        public Sushi.Mediakiwi.Data.IEnvironment Implement { get; set; }
    }
}
