using Sushi.Mediakiwi.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// Exposes Properties and Methods which makes it possible to extend the standard MediaKiwi page navigation
    /// with custom icons
    /// </summary>
    public interface IPageModule
    {
        /// <summary>
        /// Determines if this module should show up on the supplied page for the supplied user
        /// </summary>
        /// <param name="inPage">The page being viewed</param>
        /// <param name="inUser">The user viewing the page</param>
        /// <returns>True when should show</returns>
        bool ShowOnPage(Page inPage, IApplicationUser inUser);

        /// <summary>
        /// Executes the custom code for this Implementation
        /// </summary>
        /// <param name="inPage">The page being viewed</param>
        /// <param name="inUser">The user viewing the page</param>
        /// <returns><see cref="ModuleExecutionResult"/></returns>
        ModuleExecutionResult Execute(Page inPage, IApplicationUser inUser);

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
