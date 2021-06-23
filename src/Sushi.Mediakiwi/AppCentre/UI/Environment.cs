using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
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

            ListLoad += Environment_ListLoad;
            ListSave += Environment_ListSave;
        }


        string m_PasswordAtStart;

        /// <summary>
        /// Handles the ListSave event of the Environment control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ComponentListEventArgs"/> instance containing the event data.</param>
        async Task Environment_ListSave(ComponentListEventArgs e)
        {
            if (!Implement.Password.Equals(m_PasswordAtStart))
                Implement.Password = Utility.HashStringByMD5(Implement.Password);

            await Implement.SaveAsync();

            //  Redirect as path could be changed!
            Response.Redirect(wim.GetUrl());
            //FolderLogic.CreatePath(wim.Console, wim.CurrentList));
        }

        /// <summary>
        /// Handles the ListLoad event of the Environment control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ComponentListEventArgs"/> instance containing the event data.</param>
        Task Environment_ListLoad(ComponentListEventArgs e)
        {
            Implement = Mediakiwi.Data.Environment.Current;
            m_PasswordAtStart = Implement.Password;
            FormMaps.Add(new Forms.EnvironmentForm(Implement));
            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets or sets the implement.
        /// </summary>
        /// <value>The implement.</value>
        public IEnvironment Implement { get; set; }
    }
}
