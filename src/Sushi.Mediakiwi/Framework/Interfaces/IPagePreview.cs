namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// The page preview interface
    /// </summary>
    public interface IPagePreview
    {
        /// <summary>
        /// Gets the preview URL.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        System.Uri GetPreviewUrl(Sushi.Mediakiwi.Data.Page page);
        /// <summary>
        /// Gets the online URL.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        System.Uri GetOnlineUrl(Sushi.Mediakiwi.Data.Page page);
    }

    public class PagePublication : IPagePublication
    {
        public PagePublication()
        {
            this.AskConfirmation = false;
        }

        public virtual bool AskConfirmation { get; protected set; }
        public virtual string ConfirmationQuestion { get; protected set; }
        public virtual string ConfirmationTitle { get; protected set; }
        public virtual string ConfirmationRejectLabel { get; protected set; }
        public virtual string ConfirmationAcceptLabel { get; protected set; }

        public virtual void DoPostPublishValidation(Data.IApplicationUser user, Data.Page page)
        {
        }

        public virtual bool DoPrePublishValidation(Data.IApplicationUser user, Data.Page page)
        {
            return true;
        }

        public virtual bool CanPublish(Data.IApplicationUser user, Data.Page page)
        {
            return true;
        }

        public virtual bool CanTakeOffline(Data.IApplicationUser user, Data.Page page)
        {
            return true;
        }

        public virtual void ValidateConfirmation(Data.IApplicationUser user, Data.Page page)
        {
        }
    }
}