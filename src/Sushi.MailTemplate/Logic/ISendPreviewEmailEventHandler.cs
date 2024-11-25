using System;
using System.Threading.Tasks;

namespace Sushi.MailTemplate.Logic
{
    /// <summary>
    /// SendPreviewEmailEventHandler class, to handle preview e-mails. This needs implementation on the referencing project like 
    /// Sushi.MailTemplate.Logic.SendPreviewEmailEventHandler.SendPreviewEmail += OnSendPreviewEmail;
    /// </summary>
    public interface ISendPreviewEmailEventHandler
    {
        /// <summary>
        /// Async invokation of the SendPreviewEmailAsync
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public Task SendPreviewEmailAsync(SendPreviewEmailEventArgs e);
    }

    /// <summary>
    /// SendPreviewEmailEventArgs class
    /// </summary>
    public class SendPreviewEmailEventArgs : EventArgs
    {
        /// <summary>
        /// E-mail address to send from
        /// </summary>
        public string EmailFrom { get; set; }

        /// <summary>
        /// Name to send from
        /// </summary>
        public string EmailFromName { get; set; }

        /// <summary>
        /// E-mail address to send to
        /// </summary>
        public string EmailTo { get; set; }

        public Data.MailTemplate MailTemplate { get; set; }

        /// <summary>
        /// Shows if sending the preview was successful
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Contains the error message when sending was not successful
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}