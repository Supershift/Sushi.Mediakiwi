using Microsoft.AspNetCore.Http;
using Sushi.Mediakiwi.Data;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Framework.Interfaces
{
    public interface IListModule
    {
        /// <summary>
        /// The title of this module, exposed to the CMS
        /// </summary>
        public string ModuleTitle { get; }

        /// <summary>
        /// Determines if this module should show up on the supplied list for the supplied user
        /// </summary>
        /// <param name="inList">The page being viewed</param>
        /// <param name="inUser">The user viewing the page</param>
        /// <returns>True when should show</returns>
        bool ShowOnList(IComponentListTemplate inList, IApplicationUser inUser);

        /// <summary>
        /// Executes the custom code for this Implementation
        /// </summary>
        /// <param name="inList">The list being viewed</param>
        /// <param name="inUser">The user viewing the list</param>
        /// <param name="context">The executing HttpContext pipeline</param>
        /// <returns><see cref="ModuleExecutionResult"/></returns>
        Task<ModuleExecutionResult> ExecuteAsync(IComponentListTemplate inList, IApplicationUser inUser, HttpContext context);

        /// <summary>
        /// Show the button in search Mode
        /// </summary>
        bool ShowInSearchMode { get; set; }

        /// <summary>
        /// Show the button in edit Mode
        /// </summary>
        bool ShowInEditMode { get; set; }

        /// <summary>
        /// The Icon CssClass to apply (mediakiwi internal)
        /// </summary>
        string IconClass { get; set; }

        /// <summary>
        /// The Icon URL for a custom Icon
        /// </summary>
        string IconURL { get; set; }

        /// <summary>
        /// The tooltip that shows when hovering over the icon
        /// </summary>
        string Tooltip { get; set; }

        /// <summary>
        /// Is a confirmation message required ?
        /// </summary>
        bool ConfirmationNeeded { get; set; }

        /// <summary>
        /// The confirmation message title
        /// </summary>
        string ConfirmationTitle { get; set; }

        /// <summary>
        /// The confirmation message content
        /// </summary>
        string ConfirmationQuestion { get; set; }

    }
}
