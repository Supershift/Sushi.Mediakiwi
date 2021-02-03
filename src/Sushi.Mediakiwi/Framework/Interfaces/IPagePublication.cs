using System;
namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// This interface exposes the page publication hooks.
    /// </summary>
    public interface IPagePublication
    {
        /// <summary>
        /// Should a confirmation message be shown?
        /// </summary>
        bool AskConfirmation { get; }
        /// <summary>
        /// The label of the accept button
        /// </summary>
        string ConfirmationAcceptLabel { get; }
        /// <summary>
        /// The text (can also be HTML) of the confirmation message
        /// </summary>
        string ConfirmationQuestion { get; }
        /// <summary>
        /// The label of the reject (cancel) button
        /// </summary>
        string ConfirmationRejectLabel { get; }
        /// <summary>
        /// The title of the confirmation box
        /// </summary>
        string ConfirmationTitle { get; }
        /// <summary>
        /// Perform post publication processes.
        /// </summary>
        /// <param name="user">The user entity</param>
        /// <param name="page">The page entity</param>
        void DoPostPublishValidation(Sushi.Mediakiwi.Data.IApplicationUser user, Sushi.Mediakiwi.Data.Page page);
        /// <summary>
        /// Perform pre publication validation processes, if the publication should not commence than return false.
        /// </summary>
        /// <param name="user">The user entity</param>
        /// <param name="page">The page entity</param>
        /// <returns>Can the publication process proceed?</returns>
        bool DoPrePublishValidation(Sushi.Mediakiwi.Data.IApplicationUser user, Sushi.Mediakiwi.Data.Page page);
        /// <summary>
        /// Is the current application user allowed to publish this page? 
        /// </summary>
        /// <param name="user">The user entity</param>
        /// <param name="page">The page entity</param>
        /// <returns>If true than the publish button will be visible</returns>
        bool CanPublish(Sushi.Mediakiwi.Data.IApplicationUser user, Sushi.Mediakiwi.Data.Page page);
        /// <summary>
        /// Is the current application user allowed to take this page offline? 
        /// </summary>
        /// <param name="user">The user entity</param>
        /// <param name="page">The page entity</param>
        /// <returns>If true than the take offline button will be visible</returns>
        bool CanTakeOffline(Sushi.Mediakiwi.Data.IApplicationUser user, Sushi.Mediakiwi.Data.Page page);
        /// <summary>
        /// The confirmation properties should be set in this method.
        /// </summary>
        /// <param name="user">The user entity</param>
        /// <param name="page">The page entity</param>
        void ValidateConfirmation(Sushi.Mediakiwi.Data.IApplicationUser user, Sushi.Mediakiwi.Data.Page page);
    }
}
