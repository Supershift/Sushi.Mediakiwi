using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Web;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.UI;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Logic;
using System.Threading.Tasks;

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

            this.ListLoad += Environment_ListLoad;
            this.ListSave += Environment_ListSave;
        }


        string m_PasswordAtStart;

        /// <summary>
        /// Handles the ListSave event of the Environment control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentListEventArgs"/> instance containing the event data.</param>
        async Task Environment_ListSave(ComponentListEventArgs e)
        {
            if (!Implement.Password.Equals(m_PasswordAtStart))
                this.Implement.Password = Utility.HashStringByMD5(Implement.Password);

            await this.Implement.SaveAsync();

            //  Redirect as path could be changed!
            Response.Redirect(wim.GetUrl());
            //FolderLogic.CreatePath(wim.Console, wim.CurrentList));
        }

        /// <summary>
        /// Handles the ListLoad event of the Environment control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentListEventArgs"/> instance containing the event data.</param>
        Task Environment_ListLoad(ComponentListEventArgs e)
        {
            this.Implement = Sushi.Mediakiwi.Data.Environment.Current;
            m_PasswordAtStart = this.Implement.Password;
            this.FormMaps.Add(new Forms.EnvironmentForm(this.Implement));
            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets or sets the implement.
        /// </summary>
        /// <value>The implement.</value>
        public Sushi.Mediakiwi.Data.IEnvironment Implement { get; set; }
    }
}
