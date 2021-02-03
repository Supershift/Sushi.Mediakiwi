using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Use this to set change the state of a page
    /// </summary>
    public enum PageStateChangeRequest
    {
        /// <summary>
        /// Do not change the page state
        /// </summary>
        Keep,
        /// <summary>
        /// Set the page state to published (this does not publish the page).
        /// </summary>
        Publish,
        /// <summary>
        /// Set the page state to unpublished (this does not unpublish the page).
        /// </summary>
        Unpublish,
        /// <summary>
        /// Set the page state to as edited
        /// </summary>
        SetChanged,
        /// <summary>
        /// Set the page state to as not edited - the live and staged page version are the same
        /// </summary>
        SetUnChanged
    }
}
